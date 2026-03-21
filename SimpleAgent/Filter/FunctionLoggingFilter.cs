using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.Diagnostics;

namespace SimpleAgent.Filter
{
	public class FunctionLoggingFilter : IFunctionInvocationFilter
	{
		public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
		{
			// 执行前的拦截 (Pre-execution)
			var pluginName = context.Function.PluginName;
			var functionName = context.Function.Name;
			var arguments = string.Join(", ", context.Arguments.Select(a => $"{a.Key}: {a.Value}"));

			Trace.WriteLine($"[日志 - 开始调用] {pluginName}.{functionName}, 调用参数: {arguments}");

			var stopwatch = Stopwatch.StartNew();
			try
			{
				// 将控制权交还给下一个过滤器或实际的函数
				await next(context);
				stopwatch.Stop();

				// 执行后的拦截 (Post-execution)
				var result = context.Result?.GetValue<object>();

				Trace.WriteLine($"[日志 - 调用成功] 耗时: {stopwatch.ElapsedMilliseconds}ms, 返回结果: {result}");
			}
			catch (Exception ex)
			{
				// 异常处理拦截
				stopwatch.Stop();
				Trace.WriteLine($"[日志 - 调用失败] 耗时: {stopwatch.ElapsedMilliseconds}ms, 错误信息: {ex.Message}");
				throw;
			}
		}
	}
}
