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

namespace SimpleAgent.Agents
{
    public class DeveloperAgent : BaseAgent
    {
        public const string AgentName = "Developer";
        public const string NickName = "开发智能体";
        private const string SystemPrompt = @"
# Role
你是一个资深的软件架构师。

# Objective
根据提供的《开发计划》或《Bug修复说明》，通过调用工具、拆分任务、委派任务，完成你所收到的任务。

# Workflow
1. 读取计划：仔细阅读输入的需求或 Reviewer 的反馈。
2. 拆分任务：将计划分解为多个可独立执行的子任务，例如：项目创建、登录模块开发、设置模块开发等等。
3. 委派任务：将子任务，通过调用 `delegate_sub_task` 委派给子代理完成，你作为管理者不要执行任务，你只负责整合和最终验收。
4. 项目整合：在所有任务都完成后，你作为技术负责人，必须对项目的完成情况进行验证，使用终端工具运行编译和测试。
5. 修复错误：如果项目没有完成或遇到错误，不要停止！阅读错误日志，继续委派任务修改错误，再次运行测试，直到没有任何报错。

# Constraints
- 不要向用户提问，你必须自己解决项目中遇到的所有问题。
- 不要回答用户的消息，你只需要专注于完成任务。
- 【关键指令】任何涉及项目创建、代码编写、重构或 Bug 修复，你必须调用 `delegate_sub_task` 委派给子代理完成，你只负责阅读源码进行任务拆解，以及在子代理完成后运行测试验收。
- 【关键指令】合理利用子代理，你需要将任务分给多个子代理，不要让一子代理负责所有任务。
- 【关键指令】只有当所有任务都已经完成，且你在本地运行的编译和测试全部通过后，你才可以停止。
- 【关键指令】当你停止时才允许且必须调用 `submit_for_review` 函数，并在参数中附上你修改了哪些文件的简要总结。";

        // - 【绝对禁止】你作为管理者，绝对不能直接使用 `write_file`、`edit_file`、`append_file` 来编写主体业务代码！

        public DeveloperAgent(IKernelService kernelService, string workingDirectory, Action<string> submittedAction) : base(SystemPrompt)
        {
            kernel = kernelService.BuildKernel(workingDirectory);
            kernel.Plugins.AddFromObject(new WorkflowPlugin { OnDevelopmentSubmitted = submittedAction }, "workflow");

            KernelFunction[] kernelFunctions = [
                kernel.Plugins.GetFunction("sub_agent", "delegate_sub_task"),
                kernel.Plugins.GetFunction("file_system", "read_file"),
                kernel.Plugins.GetFunction("file_system", "list_directory"),
                kernel.Plugins.GetFunction("file_system", "path_exists"),
                kernel.Plugins.GetFunction("file_system", "get_working_directory"),
                kernel.Plugins.GetFunction("file_system", "create_directory"),
                kernel.Plugins.GetFunction("file_system", "delete_file"),
                //kernel.Plugins.GetFunction("file_system", "append_file"),
                //kernel.Plugins.GetFunction("file_system", "write_file"),
                //kernel.Plugins.GetFunction("file_system", "edit_file"),
                kernel.Plugins.GetFunction("terminal", "start_background_service"),
                kernel.Plugins.GetFunction("terminal", "stop_background_service"),
                kernel.Plugins.GetFunction("terminal", "get_service_logs"),
                kernel.Plugins.GetFunction("terminal", "execute_command"),
                kernel.Plugins.GetFunction("http_test", "send_http_request"),
                kernel.Plugins.GetFunction("workflow", "submit_for_review"),
            ];

            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
                Temperature = 0.2,
                Seed = DeveloperSeed < 0 ? seed : DeveloperSeed,
            };

            Log.Logger.Information("Developer初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
        }
    }
}
