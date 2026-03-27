using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Services;

namespace SimpleAgent.Agents
{
    public class SubDeveloperAgent : BaseAgent, IWorkflowAgent
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

        public AgentType Type => AgentType.SubDeveloper;
        private readonly IStreamingExecutionEngine executionEngine;
        private readonly ISettingsService settingsService;

        // 用于控制子代理生命周期的内部状态
        private bool _isFinished = false;
        private string _subAgentResult = string.Empty;

        public SubDeveloperAgent(IKernelService kernelService, ISettingsService settingsService, IStreamingExecutionEngine executionEngine, AgentContext context) : base(SystemPrompt)
        {
            this.executionEngine = executionEngine;
            this.settingsService = settingsService;

            kernel = kernelService.BuildKernel(context);
            var sub = new SubWorkflowPlugin()
            {
                OnSubTaskFinished = (summary) =>
                {
                    _isFinished = true;
                    _subAgentResult = summary;
                }
            };
            kernel.Plugins.AddFromObject(sub, "sub_workflow");

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
                kernel.Plugins.GetFunction("sub_workflow", "finish_subtask"),
            ];

            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
                Temperature = 0.1, // 子代理需要更稳定的输出
                Seed = seed,
            };
            Log.Information("SubDeveloper初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowState> ExecuteAsync(AgentContext context, CancellationToken cancellationToken)
        {
            return WorkflowState.Developing;
        }

        /// <summary>
        /// 启动子代理的独立思考循环
        /// </summary>
        public async Task<string> RunAsync(string taskDescription, CancellationToken cancellationToken)
        {
            AddUserMessage($"主代理交给了你一项任务：\n{taskDescription}");

            int safetyCounter = 0;
            while (!_isFinished && safetyCounter < settingsService.Current.SubMaxThinkingRounds)
            {
                safetyCounter++;

                // 复用全局的流式执行引擎
                await executionEngine.ExecuteStreamAsync(this, AgentType.SubDeveloper, cancellationToken, () => !_isFinished);
                cancellationToken.ThrowIfCancellationRequested();

                // 如果子代理结束了还没完成任务
                if (!_isFinished)
                {
                    AddUserMessage("继续你的任务");
                    AddDeveloperMessage("如果你确定已经完成了主代理交给你的任务，也已经通过了测试和编译，请调用 `finish_subtask` 结束工作。如果你没有完成任务请不要停止，继续完成你的工作。");
                }
            }

            if (!_isFinished)
            {
                // TODO: 修复消息通知
                //chatUIService.SendSystemMessage(AgentType.Developer, $"子代理未能完成任务，交还给主代理重新分配。");
                return "[子代理异常]: 子任务执行超时或达到最大轮次仍旧未能完成，请主代理亲自检查或重新分配。";
            }

            // TODO: 修复消息通知
            //chatUIService.SendSystemMessage(AgentType.Developer, $"子代理已完成任务，交还控制权。");
            return $"[子代理执行完毕，汇报如下]:\n{_subAgentResult}";
        }
    }
}