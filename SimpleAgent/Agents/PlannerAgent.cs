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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleAgent.Agents
{
    public class PlannerAgent : BaseAgent, IWorkflowAgent
    {
        public const string NickName = "规划智能体";
        private const string SystemPrompt = @"
# Role
你是一位资深软件产品经理兼架构师。

# Objective
帮助用户澄清软件需求，并将模糊的想法转化为结构化的、可执行的 Markdown 开发计划。

# Workflow
1. 倾听用户的需求。如果需求模糊，主动提出 1~4 个关键问题来澄清细节（如：框架选择、边界条件、预期输出）。如果用户没有回答你的问题，你需要基于现有的信息做出合理的假设，并继续后续步骤。
2. 如果用户在制作一个新的项目，则需要在计划中明确指出在工作目录下新建一个文件夹进行开发，不要与已有的文件混淆。
3. 当需求明确后，编写一份包含“功能描述、拆解步骤、验收标准”的开发计划草案给用户确认。
4. 根据用户的反馈不断的完善和修改计划。
5. 与用户确认计划是否可以执行。
6. 用户明确表示同意后，调用 `finalize_plan` 函数完成规划。

# Constraints
- 你不能自己编写底层业务代码，你的产出物是“计划”。
- 你可以向用户询问是否满意计划，但是不需要向用户说明调用什么函数。
- 【关键指令】仅当用户明确表示“计划没问题”、“可以开始开发”或类似同意的表述时，你才可以调用 `finalize_plan` 函数，并将最终版的 Markdown 计划作为参数传入，此时不需要回复用户任何文字。";

        public AgentType Type => AgentType.Planner;
        private readonly IStreamingExecutionEngine executionEngine;
        private readonly ISettingsService setting;

        public PlannerAgent(IKernelService kernelService, IStreamingExecutionEngine executionEngine, ISettingsService setting, AgentContext context) : base(SystemPrompt, context)
        {
            this.executionEngine = executionEngine;
            this.setting = setting;

            kernel = kernelService.BuildKernel(context);
            KernelFunction[] kernelFunctions = [
                kernel.Plugins.GetFunction("file_system", "read_file"),
                kernel.Plugins.GetFunction("file_system", "list_directory"),
                kernel.Plugins.GetFunction("file_system", "path_exists"),
                kernel.Plugins.GetFunction("file_system", "get_working_directory"),
                kernel.Plugins.GetFunction("workflow", "finalize_plan"),
            ];

            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(kernelFunctions),
                Temperature = 0.4,
                MaxTokens = setting.Current.MaxOutTokens,
                Seed = PlannerSeed < 0 ? seed : PlannerSeed,
            };

            if (context.ChatHistory.TryGetValue(AgentType.Planner, out ChatHistory? value))
            {
                chatHistory = value;
                if (chatHistory.Count < 1) chatHistory.AddSystemMessage(SystemPrompt);
            }
            Log.Information("Planner初始化成功, Seed:{Seed}  Temperature:{Temperature}", settings.Seed, settings.Temperature);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowState> ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.Information("Planner 正在与您规划需求...");

            // 装载用户上下文
            if (string.IsNullOrEmpty(context.DetailedPlan))
            {
                AddUserMessage(context.OriginalRequest);
            }
            else if (context.TakingRounds > 0)
            {
                context.IsChangePlan = true;
                AddUserMessage(context.OriginalRequest);
            }

            // 清空 NextState，等待模型执行结果
            context.NextState = null;

            // 运行执行引擎，条件是：如果 NextState 被修改，说明工具被调用，流应当中断
            await executionEngine.ExecuteStreamAsync(this, Type, cancellationToken,
                () => context.NextState == null,
                () => Utility.Utility.ChatHistorySave(Type, chatHistory));

            // 返回插件赋予的下一个状态；如果没设置，说明大模型在等待用户回答问题，保持当前状态
            return context.NextState ?? WorkflowState.Planning;
        }
    }
}
