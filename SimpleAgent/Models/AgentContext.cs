using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleAgent.Models
{
    public class AgentContext
    {
        /// <summary>对话ID(固定)</summary>
        public Guid ConversationId { get; set; }

        /// <summary>工作路径(固定)</summary>
        public string WorkingDirectory { get; set; } = "AICoder";

        /// <summary>对话轮次(固定递增)</summary>
        public int TakingRounds { get; set; } = 0;

        /// <summary>思考循环次数(每轮中每个智能体各自递增)</summary>
        public int ThinkingRounds { get; set; } = 0;

        /// <summary>详细计划(每轮变化)</summary>
        public string DetailedPlan { get; set; } = string.Empty;

        /// <summary>用户的原始请求(每次变化)</summary>
        public string OriginalRequest { get; set; } = string.Empty;

        /// <summary>开发者提交(每次变化)</summary>
        public string DeveloperSummary { get; set; } = string.Empty;

        /// <summary>审查员反馈(每次变化)</summary>
        public string ReviewerFeedback { get; set; } = string.Empty;

        /// <summary>后续流程中是否变更了计划(每次变化)</summary>
        public bool IsChangePlan { get; set; } = false;

        /// <summary>当前的状态</summary>
        public WorkflowState CurrentState { get; set; } = WorkflowState.Idle;

        /// <summary>下一个将要流转的状态，由 WorkflowPlugin 在执行工具时进行设置</summary>
        public WorkflowState? NextState { get; set; }

        /// <summary>智能体上下文(固定)</summary>
        [JsonIgnore] public Dictionary<AgentType, ChatHistory> ChatHistory { get; set; } = [];
    }
}
