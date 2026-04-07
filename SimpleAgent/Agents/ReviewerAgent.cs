using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleAgent.Agents
{
	public class ReviewerAgent : BaseAgent, IWorkflowAgent
	{
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

		public AgentType Type => AgentType.Reviewer;
		private readonly IStreamingExecutionEngine executionEngine;
        private readonly ISettingsService setting;

        public ReviewerAgent(IKernelService kernelService, IStreamingExecutionEngine executionEngine, ISettingsService setting, AgentContext context) : base(SystemPrompt, context)
		{
			this.executionEngine = executionEngine;
			this.setting = setting;

			kernel = kernelService.BuildKernel(context);
			KernelFunction[] kernelFunctions = [
				kernel.Plugins.GetFunction("file_system", "read_file"),
				kernel.Plugins.GetFunction("file_system", "list_directory"),
				kernel.Plugins.GetFunction("file_system", "path_exists"),
				kernel.Plugins.GetFunction("file_system", "get_working_directory"),
				kernel.Plugins.GetFunction("workflow", "pass_review"),
				kernel.Plugins.GetFunction("workflow", "fail_review"),
				kernel.Plugins.GetFunction("http_test", "send_http_request"),
			];

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
				Temperature = 0.1,
                MaxTokens = setting.Current.MaxOutTokens,
                Seed = ReviewerSeed < 0 ? seed : ReviewerSeed,
			};

            if (context.ChatHistory.TryGetValue(AgentType.Reviewer, out ChatHistory? value))
            {
                chatHistory = value;
                if (chatHistory.Count < 1) chatHistory.AddSystemMessage(SystemPrompt);
            }
            Log.Logger.Information("Reviewer初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
		}

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<WorkflowState> ExecuteAsync(CancellationToken cancellationToken)
		{
			Log.Information("Reviewer 正在验收...");

			// 装载用户上下文
			if (context.TakingRounds == 0 || context.IsChangePlan)
			{
				AddUserMessage($"【以下为原计划】\n{context.DetailedPlan}\n\n【以下为开发者提交的摘要】\n{context.DeveloperSummary}");
			}
			else
			{
				AddUserMessage($"【以下为原始需求】\n{context.OriginalRequest}\n\n【以下为开发者提交的摘要】\n{context.DeveloperSummary}");
			}

			// 清空 NextState，等待模型执行结果
			context.NextState = null;

			// 运行执行引擎，条件是：如果 NextState 被修改，说明工具被调用，流应当中断
			await executionEngine.ExecuteStreamAsync(this, Type, cancellationToken,
				() => context.NextState == null,
				() => Utility.Utility.ChatHistorySave(Type, chatHistory));

			// 返回插件赋予的下一个状态；如果没设置，说明没有结束
			return context.NextState ?? WorkflowState.Reviewing;
		}
	}
}
