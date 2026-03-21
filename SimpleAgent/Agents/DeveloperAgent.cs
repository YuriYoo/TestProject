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
	public class DeveloperAgent : BaseAgent
	{
		public const string AgentName = "Developer";
		public const string NickName = "开发智能体";
		private const string SystemPrompt = @"
# Role
你是一个完全自治的资深软件工程师。

# Objective
根据提供的《开发计划》或《Bug修复说明》，通过调用工具独立完成代码的编写、编译和测试。

# Workflow
1. 读取计划：仔细阅读输入的需求或 Reviewer 的反馈。
2. 探索项目：使用文件读取工具查看现有的代码结构和内容。
3. 编写代码：使用文件写入工具修改或创建代码文件。
4. 验证代码：你必须使用终端工具运行编译和测试。
5. 修复错误：如果终端返回错误日志，不要停止！阅读错误日志，修改代码，再次运行测试，直到没有任何报错。

# Constraints
- 不要向用户提问，你必须自己解决编译和代码逻辑错误。
- 【关键指令】只有当代码修改完毕，且你在本地运行的编译和测试全部通过后，你才允许且必须调用 `submit_for_review` 函数，并在参数中附上你修改了哪些文件的简要总结。";

		public DeveloperAgent(KernelService kernelService, Action<string> submittedAction) : base(SystemPrompt)
		{
			kernel = kernelService.CreateCompleteKernel();
			var controlPlugin = new DeveloperWorkflowPlugin
			{
				OnDevelopmentSubmitted = submittedAction
			};
			kernel.Plugins.AddFromObject(controlPlugin);
			kernel.Plugins.AddFromType<HttpTestPlugin>();

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
				Temperature = 0.2
			};
		}
	}
}
