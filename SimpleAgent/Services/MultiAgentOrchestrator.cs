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
        private readonly ISettingsService settings;
        private readonly IKernelService kernelService;
        private readonly ChatUIService chatUIService;

        private readonly AgentContext _context = new();

        private PlannerAgent _plannerAgent;
        private DeveloperAgent _developerAgent;
        private ReviewerAgent _reviewerAgent;
        private RouterAgent _routerAgent;

        private TaskCompletionSource<string>? _userInputTcs;

        private string workingDirectory;

        private WorkflowState _currentState;

        /// <summary>Token使用更新事件</summary>
        public event Action<AgentType, ChatTokenUsage?> OnTokenUsageUpdated;

        /// <summary>重置用户输入状态事件</summary>
        public event Action OnResetUserInputState;

        public MultiAgentOrchestrator(ILogger<MultiAgentOrchestrator> logger, ISettingsService settings, IKernelService kernelService, ChatUIService chatUIService)
        {
            this.logger = logger;
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

            _routerAgent = new(kernelService, workingDirectory, () =>
            {
                _plannerAgent.AddUserMessage(_context.OriginalRequest);
                _currentState = WorkflowState.Planning;
            },
            () =>
            {
                // 直接用用户输入作为开发计划
                _context.DetailedPlan = _context.OriginalRequest;
                _currentState = WorkflowState.Developing;
            });
        }

        /// <summary>
        /// 向UI界面发送工具调用消息（专门处理模型调用插件时的显示）
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        private int SendToolMessage(AgentType agentType, string? name, int line, string? arguments)
        {
            string outstr;
            JObject args = [];
            try
            {
                if (line >= 0 && !string.IsNullOrWhiteSpace(arguments)) args = JObject.Parse(arguments);
            }
            catch (Exception)
            {
                Trace.WriteLine($"工具参数解析失败: {arguments}");
            }

            switch (name)
            {
                case "file_system-read_file":
                    outstr = $"读取文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-list_directory":
                    outstr = $"获取文件列表";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("directoryPath", out var directoryPath) ? directoryPath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-path_exists":
                    outstr = $"路径判断";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("path", out var path) ? path.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-get_working_directory":
                    outstr = $"获取工作目录";
                    if (line >= 0)
                    {
                        outstr = $"[ {outstr} 工具调用完成]";
                    }
                    break;

                case "file_system-create_directory":
                    outstr = $"创建目录";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("directoryPath", out var directoryPath) ? directoryPath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-delete_file":
                    outstr = $"删除文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  路径: {arg1}";
                    }
                    break;

                case "file_system-append_file":
                    outstr = $"向文件追加内容";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("content", out var content) ? content.ToString().Length + " 字节" : "参数错误";
                        var arg2 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  追加内容长度: {arg1}  路径: {arg2}";
                    }
                    break;

                case "file_system-write_file":
                    outstr = $"全量写入文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("content", out var content) ? content.ToString().Length + " 字节" : "参数错误";
                        var arg2 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  内容长度: {arg1}  路径: {arg2}";
                    }
                    break;

                case "file_system-edit_file":
                    outstr = $"编辑文件";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("filePath", out var filePath) ? filePath : "参数错误";
                        var arg2 = args.TryGetValue("searchBlock", out var searchBlock) ? searchBlock.ToString().Length + " 字节" : "参数错误";
                        var arg3 = args.TryGetValue("replaceBlock", out var replaceBlock) ? replaceBlock.ToString().Length + " 字节" : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  原始内容长度: {arg2}  修改内容长度: {arg3}  路径: {arg1}";
                    }
                    break;

                case "terminal-execute_command":
                    outstr = $"执行命令";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("command", out var command) ? command.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  命令: {arg1}";
                    }
                    break;

                case "terminal-start_background_service":
                    outstr = $"启动后台服务";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        var arg2 = args.TryGetValue("command", out var command) ? command.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}  命令: {arg2}";
                    }
                    break;

                case "terminal-stop_background_service":
                    outstr = $"停止后台服务";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}";
                    }
                    break;

                case "terminal-get_service_logs":
                    outstr = $"读取服务日志";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("serviceId", out var serviceId) ? serviceId.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  服务ID: {arg1}";
                    }
                    break;

                case "http_test-send_http_request":
                    outstr = $"发送HTTP请求";
                    if (line >= 0)
                    {
                        var arg1 = args.TryGetValue("method", out var method) ? method.ToString() : "参数错误";
                        var arg2 = args.TryGetValue("url", out var url) ? url.ToString() : "参数错误";
                        outstr = $"[ {outstr} 工具调用完成]  方法: {arg1}  URL: {arg2}";
                    }
                    break;

                default:
                    if (name != null && name.StartsWith("workflow-"))
                    {
                        outstr = line < 0 ? "切换智能体" : "[切换智能体完成]";
                    }
                    else
                    {
                        outstr = line < 0 ? $"未知 {name}" : $"[未知工具 {name} 调用完成]";
                    }
                    break;
            }

            return chatUIService.SendToolMessage(agentType, outstr, line);
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
        public async Task RunWorkflowAsync(string userInput)
        {
            // 记录用户的原始请求
            _context.OriginalRequest = userInput;

            // 重置
            _context.DevCycleCount = 0;

            // 收到消息，进入路由状态
            _currentState = WorkflowState.Routing;

            while (_currentState != WorkflowState.Idle && _currentState != WorkflowState.Completed)
            {
                Trace.WriteLine($"开始状态: {_currentState}");
                switch (_currentState)
                {
                    // 路由判断
                    case WorkflowState.Routing:
                        await HandleRoutingAsync();
                        break;

                    // 讨论规划
                    case WorkflowState.Planning:
                        await HandlePlanningAsync();
                        SaveChatHistory(AgentType.Planner, _plannerAgent.chatHistory);
                        break;

                    // 开发测试
                    case WorkflowState.Developing:
                        await HandleDevelopingAsync();

                        var kernel = kernelService.BuildKernel(workingDirectory);
                        var chatService = kernel.GetRequiredService<IChatCompletionService>();
                        var myReducer = new CustomChatHistoryReducer(chatService);
                        await _developerAgent.chatHistory.ReduceInPlaceAsync(myReducer, CancellationToken.None);

                        SaveChatHistory(AgentType.Developer, _developerAgent.chatHistory);
                        break;

                    // 验收修改
                    case WorkflowState.Reviewing:
                        await HandleReviewingAsync();
                        SaveChatHistory(AgentType.Reviewer, _reviewerAgent.chatHistory);
                        break;
                }
                Trace.WriteLine($"结束状态: {_currentState}");
            }

            Trace.WriteLine("本轮任务已全部处理完毕，等待下一次用户输入。");
        }

        private void SaveChatHistory(AgentType agentType, ChatHistory chatHistory)
        {
            var path = $"logs\\{DateTime.Now:yyyyMMdd_HHmmss}_{agentType}.txt";
            StringBuilder sb = new();
            foreach (var message in chatHistory)
            {
                sb.AppendLine($"===== {message.Role} =====");
                sb.AppendLine($"[Items]");
                if (message.Items.Count > 0)
                {
                    foreach (var item in message.Items)
                    {
                        sb.AppendLine($"- Mime:{item.MimeType}  Inner:{item.InnerContent}");
                        if (item.Metadata != null)
                        {
                            foreach (var meta in item.Metadata)
                            {
                                sb.AppendLine($"- {meta.Key}: {meta.Value}");
                                if (meta.Key == "ChatResponseMessage.FunctionToolCalls")
                                {
                                    if (meta.Value is List<OpenAI.Chat.ChatToolCall> toolCalls)
                                    {
                                        foreach (var toolCall in toolCalls)
                                        {
                                            sb.AppendLine($"  - ID:{toolCall.Id}  Kind:{toolCall.Kind}  FuncName:{toolCall.FunctionName}  FuncArgs:{toolCall.FunctionArguments}");
                                        }
                                    }
                                    else
                                    {
                                        sb.AppendLine($"  - 转换失败: {meta.Value?.GetType()}");
                                    }
                                }
                            }
                        }
                    }
                }
                if (message.Metadata != null && message.Metadata.Count > 0)
                {
                    sb.AppendLine($"[Metadata]");
                    foreach (var meta in message.Metadata)
                    {
                        sb.AppendLine($"- {meta.Key}: {meta.Value}");
                        if (meta.Key == "ChatResponseMessage.FunctionToolCalls")
                        {
                            if (meta.Value is List<OpenAI.Chat.ChatToolCall> toolCalls)
                            {
                                foreach (var toolCall in toolCalls)
                                {
                                    sb.AppendLine($"  - ID:{toolCall.Id}  Kind:{toolCall.Kind}  FuncName:{toolCall.FunctionName}  FuncArgs:{toolCall.FunctionArguments}");
                                }
                            }
                            else
                            {
                                sb.AppendLine($"  - 转换失败: {meta.Value?.GetType()}");
                            }
                        }
                    }
                }
                sb.AppendLine($"[Content]");
                sb.AppendLine($"{message.Content}");
            }
            File.WriteAllText(path, sb.ToString());
        }

        /// <summary>
        /// 处理路由判断
        /// </summary>
        /// <returns></returns>
        private async Task HandleRoutingAsync()
        {
            Trace.WriteLine("正在路由...");
            _routerAgent.Reset();
            _routerAgent.AddUserMessage($"用户输入: {_context.OriginalRequest}");

            using var cts = new CancellationTokenSource();

            try
            {
                StringBuilder sb = new();
                await foreach (var chunk in _routerAgent.GetStreamingChatMessageContentsAsync(cts))
                {
                    sb.Append(chunk.Content);
                    if (_currentState != WorkflowState.Routing)
                    {
                        // 触发取消，安全释放底层资源
                        cts.Cancel();
                    }
                }
                Trace.WriteLine($"路由消息: {sb}");
            }
            catch (OperationCanceledException)
            {
                // 捕获取消信息
                Trace.WriteLine("已安全截断 Router 的输出。");
            }
        }

        /// <summary>
        /// 处理需求规划
        /// </summary>
        /// <returns></returns>
        private async Task HandlePlanningAsync()
        {
            Trace.WriteLine("Planner: 正在与您规划需求...");

            string? currentCallId = null;
            string? currentFunctionName = null;
            int currentLine = -1;
            StringBuilder argumentsBuilder = new();

            StringBuilder sb = new();
            await foreach (var chunk in _plannerAgent.GetStreamingChatMessageContentsAsync())
            {
                sb.Append(chunk.Content);
                //SendUIMessage(MessageType.AI, AgentType.Planner, chunk.Content ?? "");

                // 遍历当前 chunk 中的所有内容项
                foreach (var item in chunk.Items)
                {
                    switch (item)
                    {
                        // 普通的消息文本内容
                        case StreamingTextContent textContent:
                            //sb.Append(textContent.Text);
                            chatUIService.SendAIMessage(AgentType.Planner, textContent.Text ?? "");
                            break;

                        // 工具/函数调用的内容（模型决定调用插件时会触发）这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
                        case StreamingFunctionCallUpdateContent functionCallUpdate:
                            // 如果遇到了新的 CallId，说明开始了一个新的工具调用
                            if (!string.IsNullOrEmpty(functionCallUpdate.CallId) && functionCallUpdate.CallId != currentCallId)
                            {
                                // 如果之前已经有未结束的调用（并行调用场景），在这里触发上一个调用的结束
                                if (currentCallId != null)
                                {
                                    SendToolMessage(AgentType.Planner, currentFunctionName, currentLine, argumentsBuilder.ToString());
                                }

                                // 记录新调用的状态
                                currentCallId = functionCallUpdate.CallId;
                                currentFunctionName = functionCallUpdate.Name;
                                argumentsBuilder.Clear();

                                currentLine = SendToolMessage(AgentType.Planner, functionCallUpdate.Name, -1, functionCallUpdate.Arguments);
                            }

                            // 持续拼接参数 (JSON 片段)
                            if (functionCallUpdate.Arguments != null)
                            {
                                argumentsBuilder.Append(functionCallUpdate.Arguments);
                            }
                            break;

                        // 你还可以按需处理其他类型，例如 StreamingFileReferenceContent 等
                        default:
                            chatUIService.SendAIMessage(AgentType.Planner, $"未知的流内容: {item.GetType()}");
                            break;
                    }
                }

                // 尝试从当前 chunk 提取 Token 消耗
                var usage = ExtractTokenUsage(chunk.Metadata);
                OnTokenUsageUpdated?.Invoke(AgentType.Planner, usage);

                if (_currentState != WorkflowState.Planning)
                {
                    Trace.WriteLine("计划已完成，主动中断 Planner 后续输出...");
                    break;
                }
            }

            // 【判断结束】：流结束时，如果还有缓存的 CallId，说明该工具调用的参数已经全部接收完毕
            if (currentCallId != null)
            {
                SendToolMessage(AgentType.Planner, currentFunctionName, currentLine, argumentsBuilder.ToString());
            }

            if (sb.Length > 0)
            {
                _plannerAgent.AddAssistantMessage(sb.ToString());
            }
            chatUIService.SendSystemMessage(AgentType.Planner);

            // 如果状态变了（因为模型调用了 FinalizePlan 触发了回调），循环会进入下一阶段
            // 如果没变，说明还需要向用户提问，可以在这里挂起，等待用户输入并加入 _plannerHistory
            if (_currentState == WorkflowState.Planning)
            {
                Trace.WriteLine("等待用户输入反馈...");

                // 通知 UI 开启输入框
                OnResetUserInputState.Invoke();

                // 初始化一个新的 TaskCompletionSource
                _userInputTcs = new TaskCompletionSource<string>();

                // 异步等待，此时 RunWorkflowAsync 的执行会在这里暂停，并且交出控制权，不会阻塞 UI 线程
                string userFeedback = await _userInputTcs.Task;

                // 用户输入后，代码从这里恢复执行，将用户的回复加入 Planner 的上下文
                _plannerAgent.AddUserMessage(userFeedback);

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
            Trace.WriteLine("Developer: 正在编写和测试代码...");
            _context.DevCycleCount++;

            if (_context.DevCycleCount > 500)
            {
                throw new Exception("开发-测试循环次数超限，强制中止！");
            }

            // 首次发送的为执行计划, 后续的为 Reviewer 修改
            if (_context.DevCycleCount == 1)
            {
                _developerAgent.AddUserMessage($"请根据以下计划开发：\n{_context.DetailedPlan}");
            }
            else if (!string.IsNullOrEmpty(_context.ReviewerFeedback))
            {
                _developerAgent.AddUserMessage($"Reviewer 打回，要求修改：{_context.ReviewerFeedback}");
                _context.ReviewerFeedback = string.Empty;
            }
            else
            {
                _developerAgent.AddUserMessage("继续");
                _developerAgent.AddDeveloperMessage("如果你确定已经完成计划，也已经进行过测试且编译没有问题，请调用 `submit_for_review` 提交审查。如果没有完成任务请不要停止，继续完成你的工作。");
            }

            string? currentCallId = null;
            string? currentFunctionName = null;
            int currentLine = -1;
            StringBuilder argumentsBuilder = new();

            StringBuilder sb = new();
            await foreach (var chunk in _developerAgent.GetStreamingChatMessageContentsAsync())
            {
                sb.Append(chunk.Content);

                // 遍历当前 chunk 中的所有内容项
                foreach (var item in chunk.Items)
                {
                    switch (item)
                    {
                        // 普通的消息文本内容
                        case StreamingTextContent textContent:
                            //sb.Append(textContent.Text);
                            chatUIService.SendAIMessage(AgentType.Developer, textContent.Text ?? "");
                            break;

                        // 工具/函数调用的内容（模型决定调用插件时会触发）这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
                        case StreamingFunctionCallUpdateContent functionCallUpdate:

                            // 如果遇到了新的 CallId，说明开始了一个新的工具调用
                            if (!string.IsNullOrEmpty(functionCallUpdate.CallId) && functionCallUpdate.CallId != currentCallId)
                            {
                                // 如果之前已经有未结束的调用（并行调用场景），在这里触发上一个调用的结束
                                if (currentCallId != null)
                                {
                                    SendToolMessage(AgentType.Developer, currentFunctionName, currentLine, argumentsBuilder.ToString());
                                }

                                // 记录新调用的状态
                                currentCallId = functionCallUpdate.CallId;
                                currentFunctionName = functionCallUpdate.Name;
                                argumentsBuilder.Clear();

                                currentLine = SendToolMessage(AgentType.Developer, functionCallUpdate.Name, -1, functionCallUpdate.Arguments);
                            }

                            // 持续拼接参数 (JSON 片段)
                            if (functionCallUpdate.Arguments != null)
                            {
                                argumentsBuilder.Append(functionCallUpdate.Arguments);
                            }
                            break;

                        // 你还可以按需处理其他类型，例如 StreamingFileReferenceContent 等
                        default:
                            chatUIService.SendAIMessage(AgentType.Developer, $"未知的流内容: {item.GetType()}");
                            break;
                    }
                }

                // 尝试从当前 chunk 提取 Token 消耗
                var usage = ExtractTokenUsage(chunk.Metadata);
                OnTokenUsageUpdated?.Invoke(AgentType.Developer, usage);

                if (_currentState != WorkflowState.Developing)
                {
                    Trace.WriteLine("开发已完成，主动中断 Developer 后续输出...");
                    break;
                }
            }

            // 【判断结束】：流结束时，如果还有缓存的 CallId，说明该工具调用的参数已经全部接收完毕
            if (currentCallId != null || currentLine >= 0)
            {
                SendToolMessage(AgentType.Developer, currentFunctionName, currentLine, argumentsBuilder.ToString());
            }

            if (sb.Length > 0)
            {
                _developerAgent.AddAssistantMessage(sb.ToString());
            }
            chatUIService.SendSystemMessage(AgentType.Developer);
        }

        /// <summary>
        /// 处理验收修改
        /// </summary>
        /// <returns></returns>
        private async Task HandleReviewingAsync()
        {
            Trace.WriteLine("Reviewer: 正在验收...");
            _reviewerAgent.AddUserMessage($"【以下为原计划】\n\n{_context.DetailedPlan}\n\n\n\n【以下为开发者提交的摘要】\n\n{_context.DeveloperSummary}");

            StringBuilder sb = new();
            await foreach (var chunk in _reviewerAgent.GetStreamingChatMessageContentsAsync())
            {
                sb.Append(chunk.Content);
                chatUIService.SendAIMessage(AgentType.Reviewer, chunk.Content ?? "");

                // 尝试从当前 chunk 提取 Token 消耗
                var usage = ExtractTokenUsage(chunk.Metadata);
                OnTokenUsageUpdated?.Invoke(AgentType.Reviewer, usage);

                if (_currentState != WorkflowState.Reviewing)
                {
                    Trace.WriteLine("审查已完成，主动中断 Reviewer 后续输出...");
                    break;
                }
            }

            if (sb.Length > 0)
            {
                _reviewerAgent.AddAssistantMessage(sb.ToString());
            }
            chatUIService.SendSystemMessage(AgentType.Reviewer);
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
                    Trace.WriteLine($"提取 Token 消耗失败: {ex.Message}");
                }
            }

            // 如果当前 Chunk 没有 Usage 数据，则返回 0
            return null;
        }
    }
}
