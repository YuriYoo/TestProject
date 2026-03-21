using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	public class DeveloperWorkflowPlugin
	{
		/// <summary>开完完成回调</summary>
		public Action<string>? OnDevelopmentSubmitted { get; set; }

		[KernelFunction("submit_for_review")]
		[Description("开发完成且本地编译、测试全部通过后，必须调用此函数提交给 Reviewer 验收")]
		public string SubmitForReview([Description("本次修改的内容、文件以及测试结果总结")] string summary)
		{
			OnDevelopmentSubmitted?.Invoke(summary);
			return "[系统通知] 开发完成！请立即停止调用任何工具，并直接向用户回复：'已完成开发，已为您安排 Reviewer 进行项目审查'。";
		}
	}
}
