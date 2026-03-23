using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Models
{
    internal class AgentContext
    {
        /// <summary>用户的原始请求</summary>
        public string OriginalRequest { get; set; } = string.Empty;

        /// <summary>详细计划</summary>
        public string DetailedPlan { get; set; } = string.Empty;

        /// <summary>开发者提交</summary>
        public string DeveloperSummary { get; set; } = string.Empty;

        /// <summary>审查员反馈</summary>
        public string ReviewerFeedback { get; set; } = string.Empty;

        /// <summary>记录 开发-反馈 循环次数，防止死循环</summary>
        public int DevCycleCount { get; set; } = 0;

        /// <summary>当前的状态</summary>
        public WorkflowState CurrentState { get; set; } = WorkflowState.Idle;
    }
}
