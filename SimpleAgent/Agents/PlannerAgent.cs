using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Agents
{
	public class PlannerAgent : BaseAgent
	{
		public const string AgentName = "Planner";
		public const string NickName = "规划智能体";
		private const string SystemPrompt = @"
# Role
你是一位资深软件产品经理兼架构师。

# Objective
帮助用户澄清软件需求，并将模糊的想法转化为结构化的、可执行的 Markdown 开发计划。

# Workflow
1. 倾听用户的需求。如果需求模糊，主动提出 1~4 个关键问题来澄清细节（如：框架选择、边界条件、预期输出），只有用户提出查看当前的项目内容或根据当前项目情况进行评估，才可以查看当前目录中的文件。
2. 当需求明确后，编写一份包含“功能描述、拆解步骤、验收标准”的开发计划草案给用户确认。
3. 根据用户的反馈不断的完善和修改计划。
4. 与用户确认计划是否可以执行。

# Constraints
- 你不能自己编写底层业务代码，你的产出物是“计划”。
- 【关键指令】仅当用户明确表示“计划没问题”、“可以开始开发”或类似同意的表述时，你才可以调用，并且必须调用 `finalize_plan` 函数，并将最终版的 Markdown 计划作为参数传入，此时不需要回复用户任何文字。在此之前，保持与用户的对话。";

		public PlannerAgent(KernelService kernelService, Action<string> onPlanFinalized) : base(SystemPrompt)
		{
			kernel = kernelService.CreateReadOnlyKernel();
			var controlPlugin = new PlannerWorkflowPlugin
			{
				OnPlanFinalized = onPlanFinalized
			};
			kernel.Plugins.AddFromObject(controlPlugin);

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
				Temperature = 0.4
			};
		}
	}
}
