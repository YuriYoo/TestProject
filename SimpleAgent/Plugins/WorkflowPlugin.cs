using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	public class WorkflowPlugin
    {
        /// <summary>调用Planner回调</summary>
        public Action? OnRouteToPlanner { get; set; }

        /// <summary>调用Developer回调</summary>
        public Action? OnRouteToDeveloper { get; set; }

        /// <summary>计划完成回调</summary>
        public Action<string>? OnPlanFinalized { get; set; }

        /// <summary>开完完成回调</summary>
        public Action<string>? OnDevelopmentSubmitted { get; set; }

        /// <summary>验收通过回调</summary>
        public Action? OnReviewPassed { get; set; }

        /// <summary>验收失败回调</summary>
        public Action<string>? OnReviewFailed { get; set; }

        /// <summary>子代理完成任务回调</summary>
        public Action<string>? OnSubTaskFinished { get; set; }


        [KernelFunction("route_to_planner")]
        [Description("需要调用 Planner 拆解任务时调用此函数")]
        public string RouteToPlanner()
        {
            OnRouteToPlanner?.Invoke();
            return "[系统通知] 路由成功！请立即停止调用任何工具，并直接结束对话。";
        }

        [KernelFunction("route_to_developer")]
        [Description("需要调用 Developer 直接修改代码时调用此函数")]
        public string RouteToDeveloper()
        {
            OnRouteToDeveloper?.Invoke();
            return "[系统通知] 路由成功！请立即停止调用任何工具，并直接结束对话。";
        }

        [KernelFunction("finalize_plan")]
		[Description("【极其重要】只有在你已经向用户输出了完整的 Markdown 开发计划，并且用户明确回复了“同意”、“没问题”、“就按这个做”等确认语后，才允许调用此函数结束规划！如果用户只是在回答你的提问，或者还在提修改意见，严禁调用此函数！")]
		public string FinalizePlan([Description("详细的开发计划")] string plan)
		{
			OnPlanFinalized?.Invoke(plan);
			return "[系统通知] 已与用户讨论完成！请立即停止调用任何工具，并直接向用户回复：'已确定开发计划，即将交由 Developer 开始编写代码'。";
		}

        [KernelFunction("submit_for_review")]
        [Description("开发完成且本地编译、测试全部通过后，必须调用此函数提交给 Reviewer 验收")]
        public string SubmitForReview([Description("本次修改的内容、文件以及测试结果总结")] string summary)
        {
            OnDevelopmentSubmitted?.Invoke(summary);
            return "[系统通知] 开发完成！请立即停止调用任何工具，并直接向用户回复：'已完成开发，已为您安排 Reviewer 进行项目审查'。";
        }

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

        [KernelFunction("finish_subtask")]
        [Description("当你（子代理）完成了主代理委派给你的所有子任务，并且本地测试通过后，必须调用此函数结束你的工作，并向主代理汇报。")]
        public string FinishSubTask([Description("你做了哪些修改、解决了什么问题的详细总结")] string summary)
        {
            OnSubTaskFinished?.Invoke(summary);
            return "[系统通知] 子任务已结束，控制权已交还给主代理。";
        }
    }
}
