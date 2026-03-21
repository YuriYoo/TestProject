using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	public class ReviewerWorkflowPlugin
	{
		/// <summary>验收通过回调</summary>
		public Action? OnReviewPassed { get; set; }

		/// <summary>验收失败回调</summary>
		public Action<string>? OnReviewFailed { get; set; }


		[KernelFunction("pass_review")]
		[Description("Reviewer 验收通过时调用此函数")]
		public string PassReview()
		{
			OnReviewPassed?.Invoke();
			return "[系统通知] 验收已通过！请立即停止调用任何工具，并直接向用户回复：'已完成项目验收'。";
		}

		[KernelFunction("fail_review")]
		[Description("Reviewer 验收不通过时调用此函数，并附带修改意见")]
		public string FailReview([Description("不通过的原因和要求开发者修改的意见")] string feedback)
		{
			OnReviewFailed?.Invoke(feedback);
			return "[系统通知] 验收未通过！请立即停止调用任何工具，并直接向用户回复：'项目验收未通过，已为您安排 Developer 继续修改'。";
		}
	}
}
