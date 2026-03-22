using Microsoft.SemanticKernel;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
	/// <summary>
	/// HTTP 网络请求插件
	/// 提供 AI 智能体直接发起网络请求以测试 API 接口的能力
	/// </summary>
	public class HttpTestPlugin
	{
		private readonly HttpClient _httpClient;

		public HttpTestPlugin()
		{
			// 复用 HttpClient 以提高性能，并设置严格的超时时间，防止被死锁的服务卡住
			_httpClient = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(AppSettingsService.Settings.HttpTimeout)
			};
		}

		[KernelFunction("send_http_request")]
		[Description("发送 HTTP 请求，极度适用于测试本地启动的 Web API 服务或外部接口是否正常工作。")]
		public async Task<string> SendHttpRequestAsync(
			[Description("请求的完整 URL，如 'http://localhost:5000/api/users'")] string url,
			[Description("HTTP 方法，支持 GET, POST, PUT, DELETE 等。默认 GET")] string method = "GET",
			[Description("请求体 (Body)，通常是 JSON 字符串。如果是 GET 请求请留空")] string body = "",
			[Description("自定义请求头，必须是合法的 JSON 对象字符串，例如 '{\"Authorization\": \"Bearer xxx\", \"Content-Type\": \"application/json\"}'。如无需要请留空")] string headersJson = "")
		{
			if (string.IsNullOrWhiteSpace(url)) return "[错误] URL 不能为空。";

			try
			{
				var httpMethod = new HttpMethod(method.ToUpperInvariant());
				var request = new HttpRequestMessage(httpMethod, url);

				// 解析并添加请求头
				if (!string.IsNullOrWhiteSpace(headersJson))
				{
					try
					{
						var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson);
						if (headers != null)
						{
							foreach (var header in headers)
							{
								// Content-Type 需要加在 Content 上，而不是 Request Message 上
								if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) continue;
								request.Headers.TryAddWithoutValidation(header.Key, header.Value);
							}
						}
					}
					catch
					{
						return "[错误] headersJson 格式非法，请确保它是正确的 JSON 字符串格式。";
					}
				}

				// 处理请求体
				if (!string.IsNullOrWhiteSpace(body) && httpMethod != HttpMethod.Get)
				{
					// 尝试从请求头中获取 Content-Type，默认为 application/json
					string contentType = "application/json";
					if (!string.IsNullOrWhiteSpace(headersJson) && headersJson.Contains("Content-Type", StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson);
							if (headers != null && headers.TryGetValue("Content-Type", out var type) || headers!.TryGetValue("content-type", out type))
							{
								contentType = type;
							}
						}
						catch { /* 忽略解析错误，使用默认值 */ }
					}

					request.Content = new StringContent(body, Encoding.UTF8, contentType);
				}

				// 发送请求
				using var response = await _httpClient.SendAsync(request);

				// 读取响应体
				string responseBody = await response.Content.ReadAsStringAsync();

				// 限制响应体长度，防止 AI 上下文被超长返回结果撑爆
				if (responseBody.Length > AppSettingsService.Settings.HttpTerminal)
				{
					responseBody = string.Concat(responseBody.AsSpan(0, AppSettingsService.Settings.HttpTerminal), "\n...[内容过长，已被系统截断]...");
				}

				// 拼装友好的返回结果给 AI
				var sb = new StringBuilder();
				sb.AppendLine($"[状态码] {(int)response.StatusCode} {response.StatusCode}");
				sb.AppendLine("[响应头]");
				foreach (var header in response.Headers)
				{
					sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
				}
				sb.AppendLine($"[响应体]\n{responseBody}");

				return sb.ToString();
			}
			catch (TaskCanceledException)
			{
				return $"[错误] 请求超时 ({AppSettingsService.Settings.HttpTimeout}秒)。可能原因：服务未启动、端口被防火墙拦截，或者该接口发生死锁未返回任何数据。";
			}
			catch (HttpRequestException ex)
			{
				return $"[错误] 网络请求失败。可能原因：目标服务拒绝连接 (确保服务正在运行且端口正确)。详细异常：{ex.Message}";
			}
			catch (Exception ex)
			{
				return $"[错误] 发送请求时发生未知异常: {ex.Message}";
			}
		}
	}
}