using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SimpleAgent.Plugins
{
    public class SubWorkflowPlugin
    {
        /// <summary>子代理完成任务回调</summary>
        public Action<string>? OnSubTaskFinished { get; set; }

        [KernelFunction("finish_subtask")]
        [Description("当你（子代理）完成了主代理委派给你的所有子任务，并且本地测试通过后，必须调用此函数结束你的工作，并向主代理汇报。")]
        public string FinishSubTask([Description("你做了哪些修改、解决了什么问题的详细总结")] string summary)
        {
            OnSubTaskFinished?.Invoke(summary);
            return "[系统通知] 子任务已结束，控制权已交还给主代理。";
        }
    }
}
