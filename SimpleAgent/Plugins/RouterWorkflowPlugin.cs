using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	public class RouterWorkflowPlugin
	{
		/// <summary>调用Planner回调</summary>
		public Action? OnRouteToPlanner { get; set; }

		/// <summary>调用Developer回调</summary>
		public Action? OnRouteToDeveloper { get; set; }

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
	}
}
