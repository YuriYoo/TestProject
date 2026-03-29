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
	public class RouterAgent : BaseAgent, IWorkflowAgent
	{
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

		public AgentType Type => AgentType.Router;
		private readonly IStreamingExecutionEngine executionEngine;

		public RouterAgent(IKernelService kernelService, IStreamingExecutionEngine executionEngine, AgentContext context) : base(SystemPrompt, context)
		{
			this.executionEngine = executionEngine;

			kernel = kernelService.BuildKernel(context);
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

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<WorkflowState> ExecuteAsync(CancellationToken cancellationToken)
		{
			Log.Information("Router 正在路由...");

			if (context.ThinkingRounds == 0)
			{
				Reset();
				AddUserMessage($"用户输入: {context.OriginalRequest}");
			}
			else
			{
				AddUserMessage($"你必须根据你的判断调用以下两个函数之一： `route_to_planner` 或 `route_to_developer` ");
			}

			// 清空 NextState，等待模型执行结果
			context.NextState = null;

			// 运行执行引擎，条件是：如果 NextState 被修改，说明工具被调用，流应当中断
			await executionEngine.ExecuteStreamAsync(this, Type, cancellationToken,
				() => context.NextState == null,
				() => Utility.Utility.ChatHistorySave(Type, chatHistory));

			// 返回插件赋予的下一个状态；如果没设置，说明没有结束
			return context.NextState ?? WorkflowState.Routing;
		}
	}
}
