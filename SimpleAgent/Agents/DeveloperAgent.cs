using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
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
    public class DeveloperAgent : BaseAgent, IWorkflowAgent
    {
        public AgentType Type => AgentType.Developer;
        private readonly IStreamingExecutionEngine _executionEngine;
        private readonly ISettingsService settingsService;
        private readonly IChatHistoryReducer historyReducer;
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

        public DeveloperAgent(IKernelService kernelService, IChatHistoryReducer historyReducer, ISettingsService settingsService, IStreamingExecutionEngine executionEngine, AgentContext context) : base(SystemPrompt)
        {
            _executionEngine = executionEngine;
            this.historyReducer = historyReducer;
            this.settingsService = settingsService;
            kernel = kernelService.BuildKernel(settingsService.Current.WorkingDirectory);
            kernel.Plugins.AddFromObject(new WorkflowPlugin(context), "workflow");

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

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowState> ExecuteAsync(AgentContext context, CancellationToken cancellationToken)
        {
            Log.Information("Developer 正在编写和测试代码...");
            context.DevCycleCount++;

            if (context.DevCycleCount > settingsService.Current.MaxDevCycle)
            {
                Log.Information("已超过最大开发轮次，强制中止！");
                throw new Exception("开发-测试循环次数超限，强制中止！");
            }

            // 首次发送的为执行计划, 后续的为 Reviewer 修改
            if (context.DevCycleCount == 1)
            {
                // 如果没有计划(用户直接向Coder发起请求)
                if (string.IsNullOrEmpty(context.DetailedPlan))
                {
                    context.DetailedPlan = context.OriginalRequest;
                    AddUserMessage(context.DetailedPlan);
                }
                // 如果有计划, 且已经是完善阶段了
                else if (context.IsImprove)
                {
                    // 如果计划变更了
                    if (context.IsChangePlan)
                    {
                        AddUserMessage($"请根据以下计划开发：\n{context.DetailedPlan}");
                    }
                    // 如果计划没变, 说明用户直接向Coder提出意见
                    else
                    {
                        AddUserMessage(context.OriginalRequest);
                    }
                }
                // 如果有计划, 且还没到完善阶段
                else
                {
                    AddUserMessage($"请根据以下计划开发：\n{context.DetailedPlan}");
                }
            }
            else if (!string.IsNullOrEmpty(context.ReviewerFeedback))
            {
                AddUserMessage($"Reviewer 驳回，要求做如下修改：\n{context.ReviewerFeedback}");
                context.ReviewerFeedback = string.Empty;
            }
            else
            {
                AddUserMessage("继续");
                AddDeveloperMessage("如果你确定已经完成计划，请调用 `submit_for_review` 提交审查。如果没有完成任务请不要停止，继续完成你的工作。");
            }

            // 清空 NextState，等待模型执行结果
            context.NextState = null;

            // 运行执行引擎，条件是：如果 NextState 被修改，说明工具被调用，流应当中断
            await _executionEngine.ExecuteStreamAsync(this, Type, cancellationToken,
                () => context.NextState == null,
                () => Utility.Utility.ChatHistorySave(Type, chatHistory));

            // 上下文压缩
            // TODO: 将上下文数量写入到AgentContext中, 直接在此处先判断是否超过压缩阈值
            await chatHistory.ReduceInPlaceAsync(historyReducer, CancellationToken.None);

            // 返回插件赋予的下一个状态；如果没设置，说明还没有结束对话
            return context.NextState ?? WorkflowState.Developing;
        }
    }
}
