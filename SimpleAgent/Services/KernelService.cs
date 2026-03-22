using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net.Http;
using System.Runtime.CompilerServices;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using Microsoft.Extensions.DependencyInjection;
using SimpleAgent.Filter;
using Microsoft.Extensions.Logging;

namespace SimpleAgent.Services
{
	/// <summary>
	/// SemanticKernel核心服务
	/// 负责初始化Kernel、注册插件、提供AI聊天和流式输出能力
	/// 是整个AI系统的中枢控制层
	/// </summary>
	public class KernelService
	{
		/// <summary>http客户端</summary>
		private HttpClient _httpClient;

		/// <summary>当前工作目录</summary>
		private string _workingDirectory;

		//private ILoggerFactory _loggerFactory;

		/// <summary>初始化KernelService</summary>
		public KernelService(/*, ILoggerFactory loggerFactory*/)
		{
			if (string.IsNullOrEmpty(AppSettingsService.Settings.ApiBaseUrl)) throw new InvalidOperationException("必须填写API调用地址");

			//_loggerFactory = loggerFactory;

			var handler = new HttpLoggingHandler(new HttpClientHandler());
			_httpClient = new(handler) { BaseAddress = new Uri($"{AppSettingsService.Settings.ApiBaseUrl}v1") };
			UpdateSettings();
		}

		/// <summary>
		/// 根据当前设置构建Kernel实例
		/// </summary>
		public Kernel BuildKernel()
		{
			var builder = Kernel.CreateBuilder();

			// 挂载统一的日志工厂
			//builder.Services.AddSingleton(_loggerFactory);

			// 根据提供商类型注册不同的Chat Completion服务

			// OpenAI官方API，或自定义端点（Ollama、OpenRouter、国内服务商等）
			//builder.AddOpenAIChatCompletion(AppSettingsService.Settings.ModelId, AppSettingsService.Settings.ApiKey, httpClient: CreateHttpClientWithBaseUrl(AppSettingsService.Settings.ApiBaseUrl));

			// 微软Azure OpenAI服务
			//builder.AddAzureOpenAIChatCompletion(deploymentName: AppSettingsService.Settings.AzureDeploymentName, endpoint: AppSettingsService.Settings.AzureEndpoint, apiKey: AppSettingsService.Settings.ApiKey);

			// 自定义兼容端点
			builder.AddOpenAIChatCompletion(AppSettingsService.Settings.ModelId, AppSettingsService.Settings.ApiKey, httpClient: _httpClient);
			//builder.AddOpenAIChatCompletion(AppSettingsService.Settings.ModelId, new Uri(AppSettingsService.Settings.ApiBaseUrl), AppSettingsService.Settings.ApiKey);

			// 将自定义的日志拦截器注册到服务中
			builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionLoggingFilter>();

			builder.Plugins.AddFromType<HttpTestPlugin>("http_test");
			builder.Plugins.AddFromObject(new FileSystemPlugin(_workingDirectory), "file_system");
			builder.Plugins.AddFromObject(new TerminalPlugin(_workingDirectory), "terminal");

			// 构建Kernel
			return builder.Build();
		}

		/// <summary>
		/// 更新工作目录（需要重新构建Kernel）
		/// </summary>
		public void SetWorkingDirectory(string directory)
		{
			// 确保工作目录本身是绝对路径，并且以目录分隔符结尾，防止 "C:\Work" 匹配到 "C:\Workspace"
			_workingDirectory = Path.GetFullPath(directory);
			if (!_workingDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				_workingDirectory += Path.DirectorySeparatorChar;
			}
			Directory.CreateDirectory(directory);
		}

		/// <summary>
		/// 更新设置（需要重新构建Kernel）
		/// </summary>
		public void UpdateSettings()
		{
			SetWorkingDirectory(AppSettingsService.Settings.WorkingDirectory);
		}
	}
}
