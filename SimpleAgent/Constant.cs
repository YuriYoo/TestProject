using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent
{
    public class Constant
    {
        public const string HttpClientName = "OpenAIClient";
    }

    /// <summary>
    /// 系统状态枚举
    /// </summary>
    public enum WorkflowState
    {
        /// <summary>等待用户输入</summary>
        Idle,
        /// <summary>路由判断中</summary>
        Routing,
        /// <summary>规划讨论中</summary>
        Planning,
        /// <summary>代码开发与测试中</summary>
        Developing,
        /// <summary>验收测试中</summary>
        Reviewing,
        /// <summary>本轮任务结束</summary>
        Completed
    }

    /// <summary>
    /// 智能体类型
    /// </summary>
    public enum AgentType
    {
        Router,
        Planner,
        Developer,
        Reviewer,
        SubDeveloper,
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        System,
        User,
        AI,
    }

    /// <summary>
    /// 问题类型
    /// </summary>
    public enum QuestionMode
    {
        /// <summary>无选项</summary>
        NoSelect,
        /// <summary>单选</summary>
        SingleSelect,
        /// <summary>多选</summary>
        MultiSelect,
    }
}
