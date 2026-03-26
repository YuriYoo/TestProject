using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SimpleAgent.Agents;
using SimpleAgent.Services;
using SimpleAgent.Utility;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent.Plugins
{
    public class SubAgentPlugin
    {
        /// <summary>工作目录</summary>
        public string WorkingDirectory { get; set; } = string.Empty;

        private readonly ILogger<SubAgentPlugin> logger;
        private readonly IKernelService kernelService;
        private readonly ISettingsService settingsService;
        private readonly ChatUIService chatUIService;
        private readonly IStreamingExecutionEngine executionEngine;

        public SubAgentPlugin(ILogger<SubAgentPlugin> logger, IKernelService kernelService, ISettingsService settingsService, IStreamingExecutionEngine executionEngine, ChatUIService chatUIService)
        {
            this.logger = logger;
            this.kernelService = kernelService;
            this.settingsService = settingsService;
            this.chatUIService = chatUIService;
            this.executionEngine = executionEngine;
        }

        [KernelFunction("delegate_sub_task")]
        [Description("当你需要执行开发一个功能或修复一个Bug等类似任务时，使用此工具唤醒一个子代理去专门完成该部分工作。这能让你保持清晰的全局视野。")]
        public async Task<string> DelegateSubTaskAsync(
            [Description("详细的子任务说明（告诉子代理它需要做什么、重点修改哪些文件、如何测试）")] string taskDescription,
            // SK 会自动注入当前的 cancellationToken
            CancellationToken cancellationToken)
        {
            chatUIService.SendSystemMessage(AgentType.Developer, $"正在唤醒子代理处理子任务：\n{taskDescription}");

            bool isFinished = false;
            string subAgentResult = string.Empty;

            // 初始化子代理
            var subAgent = new SubDeveloperAgent(kernelService, WorkingDirectory, (summary) =>
            {
                isFinished = true;
                subAgentResult = summary;
            });

            subAgent.AddUserMessage($"主代理交给了你一项任务：\n{taskDescription}");

            // 运行子代理的独立思考循环
            int safetyCounter = 0;
            while (!isFinished && safetyCounter < settingsService.Current.SubMaxDevCycle)
            {
                safetyCounter++;
                await executionEngine.ExecuteStreamAsync(subAgent, AgentType.SubDeveloper, cancellationToken, () => !isFinished);
                cancellationToken.ThrowIfCancellationRequested();

                // 如果子代理结束了还没完成任务
                if (!isFinished)
                {
                    subAgent.AddUserMessage("继续你的任务");
                    subAgent.AddDeveloperMessage("如果你确定已经完成了主代理交给你的任务，也已经通过了测试和编译，请调用 `finish_subtask` 结束工作。如果你没有完成任务请不要停止，继续完成你的工作。");
                }
            }

            if (!isFinished)
            {
                chatUIService.SendSystemMessage(AgentType.Developer, $"子代理未能完成任务，交还给主代理重新分配。");
                return "[子代理异常]: 子任务执行超时或达到最大轮次仍旧未能完成，请主代理亲自检查或重新分配。";
            }

            chatUIService.SendSystemMessage(AgentType.Developer, $"子代理已完成任务，交还控制权。");
            return $"[子代理执行完毕，汇报如下]:\n{subAgentResult}";
        }
    }
}