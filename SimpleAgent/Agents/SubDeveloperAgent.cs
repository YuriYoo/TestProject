using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;
using SimpleAgent.Plugins;
using SimpleAgent.Services;

namespace SimpleAgent.Agents
{
    public class SubDeveloperAgent : BaseAgent
    {
        public const string AgentName = "SubDeveloper";
        public const string NickName = "子代理智能体";
        private const string SystemPrompt = @"
# Role
你是一个执行特定子任务的独立子代理（Sub-Agent）。你由主 Developer 唤醒。

# Objective
仅专注于完成主代理分配给你的【特定子任务】，禁止做任何多余的事情。

# Workflow
1. 阅读任务：明确你要执行的任务。
2. 执行任务：使用你现有的工具完成任务。
3. 验证任务：你必须验证任务是否完成。
4. 解决错误：遇到报错必须自己阅读日志并修复，直到没有任何错误。
5. 交付：当代码测试通过后，你必须调用 `finish_subtask` 向主代理汇报。

# Constraints
- 不要向用户提问，你必须自己解决编译和代码逻辑错误。
- 不要回答用户的消息，你只需要专注于完成收到的任务。
- 【关键指令】绝对不要修改与你收到的任务无关的文件。
- 【关键指令】在调用任何工具之前，你必须先用一句简短的话说明你的分析过程和要做的事。绝对不要连续使用相同的参数重复调用同一个工具！
- 【关键指令】只有当本地测试通过后，才允许且必须调用 `finish_subtask`。";

        public SubDeveloperAgent(IKernelService kernelService, string workingDirectory, Action<string> onFinished) : base(SystemPrompt)
        {
            kernel = kernelService.BuildKernel(workingDirectory);
            // 注册子代理专属的结束工具
            kernel.Plugins.AddFromObject(new WorkflowPlugin { OnSubTaskFinished = onFinished }, "workflow");

            KernelFunction[] kernelFunctions = [
                kernel.Plugins.GetFunction("file_system", "read_file"),
                kernel.Plugins.GetFunction("file_system", "list_directory"),
                kernel.Plugins.GetFunction("file_system", "path_exists"),
                kernel.Plugins.GetFunction("file_system", "get_working_directory"),
                kernel.Plugins.GetFunction("file_system", "create_directory"),
                kernel.Plugins.GetFunction("file_system", "delete_file"),
                kernel.Plugins.GetFunction("file_system", "append_file"),
                kernel.Plugins.GetFunction("file_system", "write_file"),
                kernel.Plugins.GetFunction("file_system", "edit_file"),
                kernel.Plugins.GetFunction("terminal", "execute_command"),
                kernel.Plugins.GetFunction("terminal", "start_background_service"),
                kernel.Plugins.GetFunction("terminal", "stop_background_service"),
                kernel.Plugins.GetFunction("terminal", "get_service_logs"),
                kernel.Plugins.GetFunction("http_test", "send_http_request"),
                kernel.Plugins.GetFunction("workflow", "finish_subtask"),
            ];

            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
                Temperature = 0.1, // 子代理需要更稳定的输出
                Seed = seed,
            };
            Log.Logger.Information("SubDeveloper初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
        }
    }
}