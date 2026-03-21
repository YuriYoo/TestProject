using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAgent.Plugins;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Agents
{
	public class ReviewerAgent : BaseAgent
	{
		public const string AgentName = "Reviewer";
		public const string NickName = "审查智能体";
		private const string SystemPrompt = @"
# Role
你是一位极其严格的软件质量保证（QA）工程师。

# Objective
对比《原始开发计划》与《开发者提交的修改摘要》，决定本次开发是否满足了所有需求。

# Workflow
1. 逐条核对《原始开发计划》中的验收标准。
2. 检查《开发者提交的修改摘要》是否覆盖了这些标准。

# Constraints
- 你不负责修改代码，只负责审查。
- 【关键指令】如果所有需求均已满足，你必须调用 `pass_review` 函数。
- 【关键指令】如果发现有遗漏的功能，或者开发者提交的总结中存在明显的逻辑风险，你必须调用 `fail_review` 函数，并在参数中清晰地列出“缺失了什么”或“哪里需要重做”，以便打回给开发者。";

		public ReviewerAgent(KernelService kernelService, Action onReviewPassed, Action<string> onReviewFailed) : base(SystemPrompt)
		{
			kernel = kernelService.CreateReadOnlyKernel();
			var controlPlugin = new ReviewerWorkflowPlugin
			{
				OnReviewPassed = onReviewPassed,
				OnReviewFailed = onReviewFailed
			};
			kernel.Plugins.AddFromObject(controlPlugin);
			kernel.Plugins.AddFromType<HttpTestPlugin>();

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
				Temperature = 0.1
			};
		}
	}
}
