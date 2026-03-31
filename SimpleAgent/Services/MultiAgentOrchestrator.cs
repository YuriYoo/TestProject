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
using SimpleAgent.Factory;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Reducer;
using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    /// <summary>
    /// 统一的工作流智能体接口
    /// </summary>
    public interface IWorkflowAgent
    {
        AgentType Type { get; }

        /// <summary>
        /// 执行当前 Agent 的逻辑，并返回下一个状态
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowState> ExecuteAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// 多智能体编排
    /// </summary>
    public class MultiAgentOrchestrator
    {
        private readonly ILogger<MultiAgentOrchestrator> logger;
        private readonly ISettingsService settingsService;

        /// <summary>注册状态与 Agent 的映射字典</summary>
        private readonly Dictionary<WorkflowState, IWorkflowAgent> agentPipeline;
        public readonly AgentContext context = new();

        /// <summary>用户输入</summary>
        private TaskCompletionSource<string>? userInputTcs;

        /// <summary>全局停止</summary>
        private CancellationTokenSource? workflowCts;

        private CancellationTokenSource routerAgentCts;

        /// <summary>重置用户输入状态事件</summary>
        public event Action? OnResetUserInputState;

        public MultiAgentOrchestrator(ILogger<MultiAgentOrchestrator> logger, ISettingsService settingsService, IAgentFactory agentFactory, AgentContext context)
        {
            this.logger = logger;
            this.context = context;
            this.settingsService = settingsService;

            var pa = agentFactory.CreateAgent<PlannerAgent>(context);
            var da = agentFactory.CreateAgent<DeveloperAgent>(context);
            var ra = agentFactory.CreateAgent<ReviewerAgent>(context);
            var oa = agentFactory.CreateAgent<RouterAgent>(context);

            agentPipeline = new()
            {
                { WorkflowState.Planning, pa },
                { WorkflowState.Developing, da },
                { WorkflowState.Reviewing, ra },
                { WorkflowState.Routing, oa },
            };
        }

        /// <summary>
        /// 强制终止当前工作流
        /// </summary>
        public void StopWorkflow()
        {
            workflowCts?.Cancel();
            routerAgentCts?.Cancel();
        }

        /// <summary>
        /// 接收来自 UI 的用户输入（用于在规划阶段恢复工作流）
        /// </summary>
        /// <param name="userInput">用户输入的反馈或确认信息</param>
        public void ProvideUserInput(string userInput)
        {
            // 如果当前正在等待用户输入，则将用户的内容设置进去，这会触发 await 后的代码继续执行
            if (userInputTcs != null && !userInputTcs.Task.IsCompleted)
            {
                userInputTcs.TrySetResult(userInput);
            }
        }

        /// <summary>
        /// 运行路由判断
        /// </summary>
        /// <returns></returns>
        public async Task<WorkflowState> RunRoutingAsync()
        {
            var routerAgent = agentPipeline[WorkflowState.Routing];
            routerAgentCts = new();
            while (true)
            {
                var state = await routerAgent.ExecuteAsync(routerAgentCts.Token);
                if (state != WorkflowState.Routing) return state;
            }
        }

        /// <summary>
        /// 运行工作流(异步)
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public async Task RunWorkflowAsync(string userInput, WorkflowState startState)
        {
            context.OriginalRequest = userInput;
            context.CurrentState = startState;
            context.DeveloperSummary = string.Empty;
            context.ReviewerFeedback = string.Empty;
            context.ThinkingRounds = 0;

            workflowCts = new CancellationTokenSource();

            try
            {
                while (context.CurrentState != WorkflowState.Idle && context.CurrentState != WorkflowState.Completed)
                {
                    if (context.ThinkingRounds > settingsService.Current.MaxThinkingRounds)
                    {
                        logger.LogWarning("已超过最大开发轮次，强制中止！");
                        workflowCts.Cancel();
                        break;
                    }

                    logger.LogInformation("进入节点: {state}", context.CurrentState);
                    if (agentPipeline.TryGetValue(context.CurrentState, out var currentAgent))
                    {
                        var returnedState = await currentAgent.ExecuteAsync(workflowCts.Token);

                        // 状态没有推进，意味着模型正在等待用户的对话补充
                        if (returnedState == context.CurrentState)
                        {
                            if (returnedState == WorkflowState.Planning)
                            {
                                logger.LogInformation("等待用户输入反馈...");
                                OnResetUserInputState?.Invoke();
                                userInputTcs = new TaskCompletionSource<string>();
                                string userFeedback = await userInputTcs.Task;
                                context.OriginalRequest = userFeedback;
                            }

                            context.ThinkingRounds++;
                        }
                        else
                        {
                            context.CurrentState = returnedState;
                            context.ThinkingRounds = 0;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"无法处理状态: {context.CurrentState}");
                    }
                    logger.LogInformation("退出节点: {state}", context.CurrentState);
                }
            }
            catch (OperationCanceledException)
            {
                // 捕获到终止
                context.CurrentState = WorkflowState.Idle;
                logger.LogInformation("强行终止了工作流。");
                //chatUIService.SendSystemMessage(AgentType.System, "[系统通知] 任务已被手动强制终止。");
            }
            finally
            {
                workflowCts?.Dispose();
                workflowCts = null;
            }

            context.TakingRounds++;
            context.IsChangePlan = false;
            logger.LogInformation("本轮任务已全部处理完毕，等待下一次用户输入。");
        }
    }
}
