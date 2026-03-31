using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SimpleAgent.Agents;
using SimpleAgent.Factory;
using SimpleAgent.Models;
using SimpleAgent.Services;
using SimpleAgent.Utility;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent.Plugins
{
    public class SubAgentPlugin
    {
        private readonly IAgentFactory agentFactory;
        private readonly ChatUIService chatUIService;
        private readonly AgentContext context;

        public SubAgentPlugin(IAgentFactory agentFactory, ChatUIService chatUIService, AgentContext context)
        {
            this.agentFactory = agentFactory;
            this.chatUIService = chatUIService;
            this.context = context;
        }

        [KernelFunction("delegate_sub_task")]
        [Description("当你需要执行一个任务时，使用此工具唤醒一个子代理去专门完成该任务。这能让你保持清晰的全局视野。")]
        public async Task<string> DelegateSubTaskAsync(
            [Description("详细的子任务说明（告诉子代理它需要做什么、重点修改哪些文件、如何测试）")] string taskDescription,
            CancellationToken cancellationToken) // SK 会自动注入当前的 cancellationToken
        {
            chatUIService.SendSystemMessage(AgentType.Developer, $"正在唤醒子代理处理子任务：\n{taskDescription}");

            var subAgent = agentFactory.CreateAgent<SubDeveloperAgent>(context);
            string result = await subAgent.RunAsync(taskDescription, cancellationToken);

            chatUIService.SendSystemMessage(AgentType.Developer, $"子代理已完成任务，交还控制权。");
            return result;
        }
    }
}