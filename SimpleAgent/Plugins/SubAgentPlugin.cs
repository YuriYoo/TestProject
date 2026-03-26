using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SimpleAgent.Agents;
using SimpleAgent.Services;
using SimpleAgent.Utility;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent.Plugins
{
    public class SubAgentPlugin
    {
        /// <summary>工作目录</summary>
        public string WorkingDirectory { get; set; } = string.Empty;

        private readonly ILogger<SubAgentPlugin> logger;
        private readonly IKernelService kernelService;
        private readonly ISettingsService settingsService;
        private readonly ChatUIService chatUIService;

        public SubAgentPlugin(ILogger<SubAgentPlugin> logger, IKernelService kernelService, ISettingsService settingsService, ChatUIService chatUIService)
        {
            this.logger = logger;
            this.kernelService = kernelService;
            this.settingsService = settingsService;
            this.chatUIService = chatUIService;
        }

        [KernelFunction("delegate_sub_task")]
        [Description("当你需要执行开发一个功能或修复一个Bug等类似任务时，使用此工具唤醒一个子代理去专门完成该部分工作。这能让你保持清晰的全局视野。")]
        public async Task<string> DelegateSubTaskAsync(
            [Description("详细的子任务说明（告诉子代理它需要做什么、重点修改哪些文件、如何测试）")] string taskDescription,
            // SK 会自动注入当前的 cancellationToken
            CancellationToken cancellationToken)
        {
            chatUIService.SendSystemMessage(AgentType.Developer, $"正在唤醒子代理处理子任务：\n{taskDescription}");

            bool isFinished = false;
            string subAgentResult = string.Empty;

            // 初始化子代理
            var subAgent = new SubDeveloperAgent(kernelService, WorkingDirectory, (summary) =>
            {
                isFinished = true;
                subAgentResult = summary;
            });

            subAgent.AddUserMessage($"主代理交给了你一项任务：\n{taskDescription}");

            // 运行子代理的独立思考循环
            int safetyCounter = 0;
            while (!isFinished && safetyCounter < settingsService.Current.SubMaxDevCycle)
            {
                safetyCounter++;
                await ProcessAgentStreamAsync(subAgent, cancellationToken, () => { return !isFinished; });
                cancellationToken.ThrowIfCancellationRequested();

                // 如果子代理结束了还没完成任务
                if (!isFinished)
                {
                    subAgent.AddUserMessage("继续你的任务");
                    subAgent.AddDeveloperMessage("如果你确定已经完成了主代理交给你的任务，也已经通过了测试和编译，请调用 `finish_subtask` 结束工作。如果你没有完成任务请不要停止，继续完成你的工作。");
                }
            }

            if (!isFinished)
            {
                chatUIService.SendSystemMessage(AgentType.Developer, $"子代理未能完成任务，交还给主代理重新分配。");
                return "[子代理异常]: 子任务执行超时或达到最大轮次仍旧未能完成，请主代理亲自检查或重新分配。";
            }

            chatUIService.SendSystemMessage(AgentType.Developer, $"子代理已完成任务，交还控制权。");
            return $"[子代理执行完毕，汇报如下]:\n{subAgentResult}";
        }


        /// <summary>
        /// 通用的智能体流式输出与工具调用处理方法
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="keepRunningCondition">持续运行条件(true:继续 false:终止)</param>
        /// <returns></returns>
        private async Task ProcessAgentStreamAsync(BaseAgent agent, CancellationToken cancellationToken, Func<bool> keepRunningCondition)
        {
            AgentType agentType = AgentType.SubDeveloper;
            string? currentCallId = null;
            string? currentFunctionName = null;
            int currentLine = -1;

            StringBuilder argumentsBuilder = new();
            StringBuilder sb = new();

            using var cts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            try
            {
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

                    // 通过委托判断是否需要因为状态改变而中断流
                    if (!keepRunningCondition())
                    {
                        // 触发取消，安全释放底层资源
                        cts.Cancel();
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
                if (linkedCts != null && linkedCts.IsCancellationRequested)
                {
                    // 抛出异常，让外层捕获
                    throw;
                }
                else
                {
                    // 捕获取消信息
                    logger.LogInformation("已安全截断 {Type} 的输出。", agentType);
                }
            }
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
    }
}