using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	public class PlannerWorkflowPlugin
	{
		/// <summary>计划完成回调</summary>
		public Action<string>? OnPlanFinalized { get; set; }

		[KernelFunction("finalize_plan")]
		[Description("【极其重要】只有在你已经向用户输出了完整的 Markdown 开发计划，并且用户明确回复了“同意”、“没问题”、“就按这个做”等确认语后，才允许调用此函数结束规划！如果用户只是在回答你的提问，或者还在提修改意见，严禁调用此函数！")]
		public string FinalizePlan([Description("详细的开发计划")] string plan)
		{
			OnPlanFinalized?.Invoke(plan);
			return "[系统通知] 已与用户讨论完成！请立即停止调用任何工具，并直接向用户回复：'已确定开发计划，即将交由 Developer 开始编写代码'。";
		}
	}
}
