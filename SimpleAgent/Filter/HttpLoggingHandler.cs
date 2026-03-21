using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Filter
{
	/// <summary>
	/// 创建一个拦截 HTTP 请求和响应的 Handler
	/// </summary>
	public class HttpLoggingHandler : DelegatingHandler
	{
		public HttpLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// 打印原始请求
			if (request.Content != null)
			{
				// 将请求内容加载到内存缓冲区，防止流被意外消耗
				await request.Content.LoadIntoBufferAsync();

				string requestBody = await request.Content.ReadAsStringAsync();
				Trace.WriteLine($"========== [HTTP请求开始] ==========\n{requestBody}");
			}

			var response = await base.SendAsync(request, cancellationToken);
			Trace.WriteLine($"[响应状态] {response.StatusCode} : {response.Content.Headers.ContentLength}");

			// 检查是否为流式响应 (Server-Sent Events)
			string? contentType = response.Content?.Headers?.ContentType?.MediaType;
			if (contentType == "text/event-stream")
			{
				// 如果是流式响应，不能 ReadAsStringAsync()，否则会阻塞程序并破坏 SK 后续的流读取。
				//Trace.WriteLine($"========== [HTTP RESPONSE] ==========\n[这是一个流式(Streaming)响应，在 Handler 层跳过 Body 打印以保护流]\n=====================================");
				Trace.WriteLine($"========== [HTTP请求结束] ==========");
				return response;
			}

			// 处理普通的非流式响应
			if (response.Content != null)
			{
				// 将响应内容加载到内存缓冲区，确保 SK 稍后也能读取它
				await response.Content.LoadIntoBufferAsync();
				string responseBody = await response.Content.ReadAsStringAsync();
				Trace.WriteLine($"{responseBody}");
				Trace.WriteLine($"========== [HTTP请求结束] ==========");
			}

			return response;
		}
	}
}
