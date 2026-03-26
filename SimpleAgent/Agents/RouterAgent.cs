using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Agents
{
    public class RouterAgent : BaseAgent
    {
        public const string AgentName = "Router";
        public const string NickName = "路由智能体";
        private const string SystemPrompt = @"
# Role
你是一个智能请求路由系统。

# Objective
根据用户的输入，判断请求的复杂度并进行分类。

# Workflow
1. 根据规则选择调用的工具。
2. 结束对话。

# Rules & Constraints
1. 【关键指令】如果用户提出的是新功能、模糊的想法、或者需要拆解的大规模重构，你必须立即调用 `route_to_planner` 函数。
2. 【关键指令】如果用户提出的是明确的简单修改、修复特定的Bug、或者单一的文件调整，你必须立即调用 `route_to_developer` 函数。
3. 你只需要调用工具，不要输出任何解释、不要与用户打招呼、不要与用户对话。
4. 你必须调用以下两个函数之一，且只能调用一次： `route_to_planner` 或 `route_to_developer` ";

        private WorkflowState state = WorkflowState.Idle;
        private CancellationTokenSource? _routingCts;

        public RouterAgent(IKernelService kernelService, string workingDirectory) : base(SystemPrompt)
        {
            kernel = kernelService.BuildKernel(workingDirectory);
            kernel.Plugins.AddFromObject(new WorkflowPlugin
            {
                OnRouteToPlanner = () =>
                {
                    state = WorkflowState.Planning;
                },
                OnRouteToDeveloper = () =>
                {
                    state = WorkflowState.Developing;
                }
            }, "workflow");

            KernelFunction[] kernelFunctions = [
                kernel.Plugins.GetFunction("workflow", "route_to_developer"),
                kernel.Plugins.GetFunction("workflow", "route_to_planner"),
            ];

            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(kernelFunctions),
                Temperature = 0.2,
                Seed = RouterSeed < 0 ? seed : RouterSeed,
            };

            Log.Logger.Information("Router初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
        }

        public void StopRouting()
        {
            _routingCts?.Cancel();
        }

        /// <summary>
        /// 处理路由判断
        /// </summary>
        /// <returns>是否需要规划</returns>
        public async Task<bool> HandleRoutingAsync(string userInput)
        {
            Log.Information("正在路由...");
            Reset();
            AddUserMessage($"用户输入: {userInput}");

            using var cts = new CancellationTokenSource();
            _routingCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, _routingCts.Token);

            try
            {
                StringBuilder sb = new();
                state = WorkflowState.Routing;
                await foreach (var chunk in GetStreamingChatMessageContentsAsync(linkedCts))
                {
                    sb.Append(chunk.Content);
                    if (state != WorkflowState.Routing)
                    {
                        // 触发取消，安全释放底层资源
                        cts.Cancel();
                    }
                }

                if (state == WorkflowState.Developing)
                {
                    return false;
                }
                else if (state == WorkflowState.Planning)
                {
                    return true;
                }
                else
                {
                    Log.Warning("Router 状态错误: {state}", state);
                }
            }
            catch (OperationCanceledException)
            {
                if (_routingCts.IsCancellationRequested)
                    Log.Information("用户强制取消了路由。");
                else
                    Log.Information("已安全截断 Router 的输出。");
            }
            finally
            {
                _routingCts.Dispose();
                _routingCts = null;
            }

            return false;
        }
    }
}
