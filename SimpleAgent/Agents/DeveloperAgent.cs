using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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
2. 创建或探索项目：如果当前计划是在规划一个新的项目，请在工作目录下创建一个新的项目文件夹进行开发；如果当前计划是对现有项目的修改，请使用文件读取工具查看现有的代码结构和内容。
3. 编写代码：使用 `write_file` 工具创建代码文件，使用 `edit_file` 工具修改已有文件。
4. 验证代码：你必须使用终端工具运行编译和测试。
5. 修复错误：如果终端返回错误日志，不要停止！阅读错误日志，修改代码，再次运行测试，直到没有任何报错。

# Constraints
- 不要向用户提问，你必须自己解决编译和代码逻辑错误。
- 不要回答用户的消息，你只需要专注于完成开发任务。
- 【关键指令】在调用任何工具之前，你必须先用一句简短的话说明你的分析过程和要做的事。绝对不要连续使用相同的参数重复调用同一个工具！
- 【关键指令】只有当代码修改完毕，且你在本地运行的编译和测试全部通过后，你才可以停止。
- 【关键指令】当你停止时才允许且必须调用 `submit_for_review` 函数，并在参数中附上你修改了哪些文件的简要总结。";

		public DeveloperAgent(KernelService kernelService, Action<string> submittedAction) : base(SystemPrompt)
		{
			kernel = kernelService.BuildKernel();
			kernel.Plugins.AddFromObject(new DeveloperWorkflowPlugin { OnDevelopmentSubmitted = submittedAction }, "workflow");

			/*KernelFunction[] kernelFunctions = [
				kernel.Plugins.GetFunction("file_system", "read_file"),
				kernel.Plugins.GetFunction("file_system", "list_directory"),
				kernel.Plugins.GetFunction("file_system", "path_exists"),
				kernel.Plugins.GetFunction("file_system", "get_working_directory"),
				kernel.Plugins.GetFunction("file_system", "delete_file"),
				kernel.Plugins.GetFunction("file_system", "create_directory"),
				kernel.Plugins.GetFunction("file_system", "append_file"),
				kernel.Plugins.GetFunction("file_system", "write_file"),
				kernel.Plugins.GetFunction("terminal", "execute_command"),
				kernel.Plugins.GetFunction("terminal", "start_background_service"),
				kernel.Plugins.GetFunction("terminal", "stop_background_service"),
				kernel.Plugins.GetFunction("terminal", "get_service_logs"),
				kernel.Plugins.GetFunction("workflow", "submit_for_review"),
				kernel.Plugins.GetFunction("http_test", "send_http_request"),
			];*/

			chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
			settings = new OpenAIPromptExecutionSettings
			{
				FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
				Temperature = 0.2,
				Seed = DeveloperSeed < 0 ? seed : DeveloperSeed,
			};

			Trace.WriteLine($"Developer初始化成功, Seed:{settings.Seed}  Temperature:{settings.Temperature}  TopP:{settings.TopP}");
		}
	}
}
