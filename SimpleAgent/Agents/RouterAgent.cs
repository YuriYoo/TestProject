using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

# Rules & Constraints
1. 【关键指令】如果用户提出的是新功能、模糊的想法、或者需要拆解的大规模重构，你必须立即调用 `route_to_planner` 函数。
2. 【关键指令】如果用户提出的是明确的简单修改、修复特定的Bug、或者单一的文件调整，你必须立即调用 `route_to_developer` 函数。
3. 不要输出任何解释、不要与用户打招呼。
4. 你必须调用以下两个函数之一，且只能调用一次： `route_to_planner` 或 `route_to_developer` ";

		public RouterAgent(KernelService kernelService, Action onRouteToPlanner, Action onRouteToDeveloper) : base(SystemPrompt)
		{
			kernel = kernelService.BuildKernel();
			kernel.Plugins.AddFromObject(new RouterWorkflowPlugin { OnRouteToPlanner = onRouteToPlanner, OnRouteToDeveloper = onRouteToDeveloper }, "router_workflow");

			KernelFunction[] kernelFunctions = [
				kernel.Plugins.GetFunction("router_workflow", "route_to_developer"),
				kernel.Plugins.GetFunction("router_workflow", "route_to_planner"),
			];

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
				Temperature = 0.2,
				Seed = RouterSeed < 0 ? seed : RouterSeed,
			};

			Trace.WriteLine($"Router初始化成功, Seed:{settings.Seed}  Temperature:{settings.Temperature}  TopP:{settings.TopP}");
		}
	}
}
