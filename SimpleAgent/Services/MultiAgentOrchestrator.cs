using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using SimpleAgent.Agents;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Reducer;
using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    /// <summary>
    /// 多智能体编排
    /// </summary>
    public class MultiAgentOrchestrator
    {
        private readonly ILogger<MultiAgentOrchestrator> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly ISettingsService settings;
        private readonly IKernelService kernelService;
        private readonly ChatUIService chatUIService;

        private readonly AgentContext _context = new();

        private PlannerAgent _plannerAgent;
        private DeveloperAgent _developerAgent;
        private ReviewerAgent _reviewerAgent;

        private TaskCompletionSource<string>? _userInputTcs;

        private string workingDirectory;

        private WorkflowState _currentState;

        /// <summary>全局停止</summary>
        private CancellationTokenSource? _workflowCts;

        /// <summary>Token使用更新事件</summary>
        public event Action<AgentType, ChatTokenUsage?> OnTokenUsageUpdated;

        /// <summary>重置用户输入状态事件</summary>
        public event Action OnResetUserInputState;

        public MultiAgentOrchestrator(ILogger<MultiAgentOrchestrator> logger, IServiceProvider serviceProvider, ISettingsService settings, IKernelService kernelService, ChatUIService chatUIService)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.settings = settings;
            this.kernelService = kernelService;
            this.chatUIService = chatUIService;
        }

        /// <summary>
        /// 初始化流程
        /// </summary>
        /// <param name="workingDirectory"></param>
        public void Initialization(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;

            _plannerAgent = new(kernelService, workingDirectory, (plan) =>
            {
                _context.DetailedPlan = plan;
                _currentState = WorkflowState.Developing; // 完成时切换状态
            });

            _developerAgent = new(kernelService, workingDirectory, (summary) =>
            {
                _context.DeveloperSummary = summary;
                _currentState = WorkflowState.Reviewing; // 完成时切换状态
            });

            _reviewerAgent = new(kernelService, workingDirectory, () =>
            {
                _currentState = WorkflowState.Completed;
            },
            (feedback) =>
            {
                _context.ReviewerFeedback = feedback;
                _currentState = WorkflowState.Developing; // 打回重做
            });
        }

        /// <summary>
        /// 强制终止当前工作流
        /// </summary>
        public void StopWorkflow()
        {
            _workflowCts?.Cancel();
        }

        /// <summary>
        /// 向UI界面发送工具调用消息（专门处理模型调用插件时的显示）
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        private int SendToolMessage(AgentType agentType, string? name, int line, string? arguments)
        {
            string msg = Utility.Utility.ToolMessageFormatter(name, line, arguments);
            return chatUIService.SendToolMessage(agentType, msg, line);
        }

        /// <summary>
        /// 接收来自 UI 的用户输入（用于在规划阶段恢复工作流）
        /// </summary>
        /// <param name="userInput">用户输入的反馈或确认信息</param>
        public void ProvideUserInput(string userInput)
        {
            // 如果当前正在等待用户输入，则将用户的内容设置进去，这会触发 await 后的代码继续执行
            if (_userInputTcs != null && !_userInputTcs.Task.IsCompleted)
            {
                _userInputTcs.TrySetResult(userInput);
            }
        }

        /// <summary>
        /// 运行工作流
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public async Task RunWorkflowAsync(string userInput, WorkflowState startState)
        {
            _currentState = startState;
            _context.OriginalRequest = userInput;
            _context.DeveloperSummary = string.Empty;
            _context.ReviewerFeedback = string.Empty;
            _context.DevCycleCount = 0;

            _workflowCts = new CancellationTokenSource();

            try
            {
                while (_currentState != WorkflowState.Idle && _currentState != WorkflowState.Completed)
                {
                    logger.LogInformation("开始状态: {state}", _currentState);
                    switch (_currentState)
                    {
                        // 讨论规划
                        case WorkflowState.Planning:
                            await HandlePlanningAsync();
                            break;

                        // 开发测试
                        case WorkflowState.Developing:
                            await HandleDevelopingAsync();
                            break;

                        // 验收修改
                        case WorkflowState.Reviewing:
                            await HandleReviewingAsync();
                            break;
                    }
                    logger.LogInformation("结束状态: {state}", _currentState);
                }
            }
            catch (OperationCanceledException)
            {
                // 捕获到手动终止
                _currentState = WorkflowState.Idle;
                logger.LogInformation("用户手动终止了工作流。");
                //chatUIService.SendSystemMessage(AgentType.System, "[系统通知] 任务已被手动强制终止。");
            }
            finally
            {
                _workflowCts?.Dispose();
                _workflowCts = null;
            }

            _context.IsImprove = true;
            _context.IsChangePlan = false;
            logger.LogInformation("本轮任务已全部处理完毕，等待下一次用户输入。");
        }

        /// <summary>
        /// 通用的智能体流式输出与工具调用处理方法
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="agentType"></param>
        /// <param name="keepRunningCondition">持续运行条件(true:继续 false:终止)</param>
        /// <returns></returns>
        private async Task ProcessAgentStreamAsync(BaseAgent agent, AgentType agentType, Func<bool> keepRunningCondition)
        {
            string? currentCallId = null;
            string? currentFunctionName = null;
            int currentLine = -1;

            StringBuilder argumentsBuilder = new();
            StringBuilder sb = new();

            using var cts = new CancellationTokenSource();
            // 将内部的CTS与外部强制终止的CTS链接起来
            using var linkedCts = _workflowCts != null ? CancellationTokenSource.CreateLinkedTokenSource(cts.Token, _workflowCts.Token) : cts;
            try
            {
                var isSaveLog = false;
                await foreach (var chunk in agent.GetStreamingChatMessageContentsAsync(linkedCts))
                {
                    // 获取所有普通的消息文本内容
                    var textContents = chunk.Items.OfType<StreamingTextContent>();
                    foreach (var textContent in textContents)
                    {
                        sb.Append(textContent.Text);
                        chatUIService.SendAIMessage(agentType, textContent.Text ?? "");
                    }

                    // 获取所有工具调用的内容（模型决定调用插件时会触发）这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
                    var functionCallUpdates = chunk.Items.OfType<StreamingFunctionCallUpdateContent>();
                    foreach (var functionCallUpdate in functionCallUpdates)
                    {
                        // 如果遇到了新的 CallId，说明开始了一个新的工具调用
                        if (!string.IsNullOrEmpty(functionCallUpdate.CallId) && functionCallUpdate.CallId != currentCallId)
                        {
                            // 如果之前已经有未结束的调用（并行调用场景），在这里触发上一个调用的结束
                            if (currentCallId != null)
                            {
                                SendToolMessage(agentType, currentFunctionName, currentLine, argumentsBuilder.ToString());
                            }

                            // 记录新调用的状态
                            currentCallId = functionCallUpdate.CallId;
                            currentFunctionName = functionCallUpdate.Name;
                            argumentsBuilder.Clear();

                            currentLine = SendToolMessage(agentType, functionCallUpdate.Name, -1, functionCallUpdate.Arguments);
                        }

                        // 持续拼接参数 (JSON 片段)
                        if (functionCallUpdate.Arguments != null)
                        {
                            argumentsBuilder.Append(functionCallUpdate.Arguments);
                        }
                        break;
                    }

                    // 尝试从当前 chunk 提取 Token 消耗
                    var usage = ExtractTokenUsage(chunk.Metadata);
                    OnTokenUsageUpdated?.Invoke(agentType, usage);

                    // 通过委托判断是否需要因为状态改变而中断流
                    if (!keepRunningCondition())
                    {
                        // 触发取消，安全释放底层资源
                        cts.Cancel();
                        if (!isSaveLog)
                        {
                            isSaveLog = true;
                            switch (agentType)
                            {
                                case AgentType.Planner:
                                    Utility.Utility.ChatHistorySave(agentType, _plannerAgent.chatHistory);
                                    break;
                                case AgentType.Developer:
                                    Utility.Utility.ChatHistorySave(agentType, _developerAgent.chatHistory);
                                    break;
                                case AgentType.Reviewer:
                                    Utility.Utility.ChatHistorySave(agentType, _reviewerAgent.chatHistory);
                                    break;
                            }
                        }
                    }
                }

                // 流结束时，如果还有缓存的 CallId，说明该工具调用的参数已经全部接收完毕
                if (currentCallId != null || currentLine >= 0)
                {
                    SendToolMessage(agentType, currentFunctionName, currentLine, argumentsBuilder.ToString());
                }

                if (sb.Length > 0)
                {
                    agent.AddAssistantMessage(sb.ToString());
                }
                chatUIService.SendSystemMessage(agentType);
            }
            catch (OperationCanceledException)
            {
                if (_workflowCts != null && _workflowCts.IsCancellationRequested)
                {
                    // 抛出异常，让外层的 RunWorkflowAsync 捕获以彻底中断整个状态机
                    throw;
                }
                else
                {
                    logger.LogInformation("已安全截断 {Type} 的输出。", agentType);
                }
            }
        }

        /// <summary>
        /// 处理需求规划
        /// </summary>
        /// <returns></returns>
        private async Task HandlePlanningAsync()
        {
            logger.LogInformation("Planner 正在与您规划需求...");

            if (string.IsNullOrEmpty(_context.DetailedPlan))
            {
                _plannerAgent.AddUserMessage(_context.OriginalRequest);
            }
            else if (_context.IsImprove)
            {
                _context.IsChangePlan = true;
                _plannerAgent.AddUserMessage(_context.OriginalRequest);
            }

            await ProcessAgentStreamAsync(_plannerAgent, AgentType.Planner, () => _currentState == WorkflowState.Planning);

            // 如果状态变了（因为模型调用了 FinalizePlan 触发了回调），循环会进入下一阶段
            // 如果没变，说明还需要向用户提问，可以在这里挂起，等待用户输入并加入 _plannerHistory
            if (_currentState == WorkflowState.Planning)
            {
                logger.LogInformation("等待用户输入反馈...");

                // 通知 UI 开启输入框
                OnResetUserInputState.Invoke();

                // 初始化一个新的 TaskCompletionSource
                _userInputTcs = new TaskCompletionSource<string>();

                // 异步等待，此时 RunWorkflowAsync 的执行会在这里暂停，并且交出控制权，不会阻塞 UI 线程
                string userFeedback = await _userInputTcs.Task;

                // 用户输入后，代码从这里恢复执行，将用户的回复加入 Planner 的上下文
                _context.OriginalRequest = userFeedback;

                // 循环结束，外层的 while 会再次进入 HandlePlanningAsync，让 Planner 根据用户反馈继续对话
            }
        }

        /// <summary>
        /// 处理开发测试
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task HandleDevelopingAsync()
        {
            logger.LogInformation("Developer 正在编写和测试代码...");
            _context.DevCycleCount++;

            if (_context.DevCycleCount > settings.Current.MaxDevCycle)
            {
                logger.LogInformation("已超过最大开发轮次，强制中止！");
                throw new Exception("开发-测试循环次数超限，强制中止！");
            }

            // 首次发送的为执行计划, 后续的为 Reviewer 修改
            if (_context.DevCycleCount == 1)
            {
                // 如果没有计划(用户直接向Coder发起请求)
                if (string.IsNullOrEmpty(_context.DetailedPlan))
                {
                    _context.DetailedPlan = _context.OriginalRequest;
                    _developerAgent.AddUserMessage(_context.DetailedPlan);
                }
                // 如果有计划, 且已经是完善阶段了
                else if (_context.IsImprove)
                {
                    // 如果计划变更了
                    if (_context.IsChangePlan)
                    {
                        _developerAgent.AddUserMessage($"请根据以下计划开发：\n{_context.DetailedPlan}");
                    }
                    // 如果计划没变, 说明用户直接向Coder提出意见
                    else
                    {
                        _developerAgent.AddUserMessage(_context.OriginalRequest);
                    }
                }
                // 如果有计划, 且还没到完善阶段
                else
                {
                    _developerAgent.AddUserMessage($"请根据以下计划开发：\n{_context.DetailedPlan}");
                }
            }
            else if (!string.IsNullOrEmpty(_context.ReviewerFeedback))
            {
                _developerAgent.AddUserMessage($"Reviewer 驳回，要求做如下修改：\n{_context.ReviewerFeedback}");
                _context.ReviewerFeedback = string.Empty;
            }
            else
            {
                _developerAgent.AddUserMessage("继续");
                _developerAgent.AddDeveloperMessage("如果你确定已经完成计划，请调用 `submit_for_review` 提交审查。如果没有完成任务请不要停止，继续完成你的工作。");
            }

            // 直接调用通用方法
            await ProcessAgentStreamAsync(_developerAgent, AgentType.Developer, () => _currentState == WorkflowState.Developing);

            // 上下文压缩
            // TODO: 将上下文数量写入到AgentContext中, 直接在此处先判断是否超过压缩阈值
            var myReducer = serviceProvider.GetRequiredService<IChatHistoryReducer>();
            await _developerAgent.chatHistory.ReduceInPlaceAsync(myReducer, CancellationToken.None);
        }

        /// <summary>
        /// 处理验收修改
        /// </summary>
        /// <returns></returns>
        private async Task HandleReviewingAsync()
        {
            logger.LogInformation("Reviewer 正在验收...");
            if (!_context.IsImprove || _context.IsChangePlan)
            {
                _reviewerAgent.AddUserMessage($"【以下为原计划】\n{_context.DetailedPlan}\n\n【以下为开发者提交的摘要】\n{_context.DeveloperSummary}");
            }
            else
            {
                _reviewerAgent.AddUserMessage($"【以下为原始需求】\n{_context.OriginalRequest}\n\n【以下为开发者提交的摘要】\n{_context.DeveloperSummary}");
            }

            await ProcessAgentStreamAsync(_reviewerAgent, AgentType.Reviewer, () => _currentState == WorkflowState.Reviewing);
        }

        /// <summary>
        /// 从 Chunk 的元数据中提取 Token 消耗信息
        /// </summary>
        /// <param name="metadata">Chunk 的 Metadata 字典</param>
        /// <returns>返回包含 Input(Prompt), Output(Completion), Total 的元组</returns>
        private ChatTokenUsage? ExtractTokenUsage(IReadOnlyDictionary<string, object?>? metadata)
        {
            // 检查是否包含 "Usage" 键
            if (metadata != null && metadata.TryGetValue("Usage", out var usageObject) && usageObject != null)
            {
                try
                {
                    // 获取 Semantic Kernel / OpenAI 底层的 Usage 对象属性
                    if (usageObject is ChatTokenUsage usage)
                    {
                        return usage;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("提取 Token 消耗失败: {msg}", ex.Message);
                }
            }

            // 如果当前 Chunk 没有 Usage 数据，则返回 0
            return null;
        }
    }
}
