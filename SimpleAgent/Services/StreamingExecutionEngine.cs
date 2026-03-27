using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using SimpleAgent.Agents;
using SimpleAgent.Utility;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    /// <summary>工具调用事件委托</summary>
    public delegate int ToolCallEventHandler(AgentType agentType, string? name, string? arguments, int line);

    /// <summary>
    /// 流式执行引擎接口
    /// </summary>
    public interface IStreamingExecutionEngine
    {
        // 定义事件：解耦 UI，业务层只抛出事件

        /// <summary>收到消息事件</summary>
        event Action<AgentType, string>? OnMessageReceived;

        /// <summary>工具调用事件</summary>
        event ToolCallEventHandler? OnToolCall;

        /// <summary>流完成事件</summary>
        event Action<AgentType>? OnStreamCompleted;

        /// <summary>Token刷新事件</summary>
        event Action<AgentType, ChatTokenUsage?>? OnTokenUsage;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="agentType"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="keepRunningCondition">持续运行条件(true:继续 false:终止)</param>
        /// <param name="saveLogAction"></param>
        /// <returns></returns>
        Task ExecuteStreamAsync(BaseAgent agent, AgentType agentType, CancellationToken cancellationToken, Func<bool> keepRunningCondition, Action? saveLogAction = null);
    }

    public class StreamingExecutionEngine : IStreamingExecutionEngine
    {
        public event Action<AgentType, string>? OnMessageReceived;
        public event ToolCallEventHandler? OnToolCall;
        public event Action<AgentType>? OnStreamCompleted;
        public event Action<AgentType, ChatTokenUsage?>? OnTokenUsage;

        public async Task ExecuteStreamAsync(BaseAgent agent, AgentType agentType, CancellationToken cancellationToken, Func<bool> keepRunningCondition, Action? saveLogAction = null)
        {
            string? currentCallId = null;
            string? currentFunctionName = null;
            int currentLine = -1;

            StringBuilder argumentsBuilder = new();
            StringBuilder sb = new();

            using var cts = new CancellationTokenSource();
            // 将内部的CTS与外部强制终止的CTS链接起来
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            bool isSaveLog = false;

            try
            {
                await foreach (var chunk in agent.GetStreamingChatMessageContentsAsync(linkedCts))
                {
                    // 处理普通文本消息
                    var textContents = chunk.Items.OfType<StreamingTextContent>();
                    foreach (var textContent in textContents)
                    {
                        sb.Append(textContent.Text);
                        OnMessageReceived?.Invoke(agentType, textContent.Text ?? "");
                    }

                    // 处理工具调用（模型决定调用插件时会触发）这里会流式输出函数名、参数等, Arguments 参数是一段段 JSON 字符串流式到达的
                    var functionCallUpdates = chunk.Items.OfType<StreamingFunctionCallUpdateContent>();
                    foreach (var functionCallUpdate in functionCallUpdates)
                    {
                        // 如果遇到了新的 CallId，说明开始了一个新的工具调用
                        if (!string.IsNullOrEmpty(functionCallUpdate.CallId) && functionCallUpdate.CallId != currentCallId)
                        {
                            // 如果之前已经有未结束的调用（并行调用场景），在这里触发上一个调用的结束
                            if (currentCallId != null && OnToolCall != null)
                            {
                                OnToolCall.Invoke(agentType, currentFunctionName, argumentsBuilder.ToString(), currentLine);
                            }

                            // 记录新调用的状态
                            currentCallId = functionCallUpdate.CallId;
                            currentFunctionName = functionCallUpdate.Name;
                            argumentsBuilder.Clear();

                            if (OnToolCall != null)
                            {
                                currentLine = OnToolCall.Invoke(agentType, functionCallUpdate.Name, functionCallUpdate.Arguments, -1);
                            }
                        }

                        // 持续拼接参数 (JSON 片段)
                        if (functionCallUpdate.Arguments != null)
                        {
                            argumentsBuilder.Append(functionCallUpdate.Arguments);
                        }
                    }

                    // 处理 Token 更新
                    if (chunk.Metadata != null && chunk.Metadata.TryGetValue("Usage", out var usageObject) && usageObject is ChatTokenUsage usage)
                    {
                        OnTokenUsage?.Invoke(agentType, usage);
                    }

                    // 检查是否需要打断执行
                    if (!keepRunningCondition())
                    {
                        cts.Cancel();
                        if (!isSaveLog && saveLogAction != null)
                        {
                            isSaveLog = true;
                            saveLogAction.Invoke();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested) throw;
            }

            // 结束时收尾最后一次 ToolCall
            if ((currentCallId != null || currentLine >= 0) && OnToolCall != null)
            {
                OnToolCall.Invoke(agentType, currentFunctionName, argumentsBuilder.ToString(), currentLine);
            }

            if (sb.Length > 0)
            {
                agent.AddAssistantMessage(sb.ToString());
            }

            OnStreamCompleted?.Invoke(agentType);
        }
    }
}