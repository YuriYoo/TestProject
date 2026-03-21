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
using Serilog;

namespace SimpleAgent.Services
{
	/// <summary>
	/// SemanticKernel核心服务
	/// 负责初始化Kernel、注册插件、提供AI聊天和流式输出能力
	/// 是整个AI系统的中枢控制层
	/// </summary>
	public class KernelService
	{
		/// <summary>当前应用设置</summary>
		private AppSettings _settings;

		/// <summary>http客户端</summary>
		private HttpClient _httpClient;

		/// <summary>当前工作目录</summary>
		private string _workingDirectory;

		/// <summary>初始化KernelService</summary>
		public KernelService(AppSettings settings)
		{
			if (string.IsNullOrEmpty(settings.ApiBaseUrl)) throw new InvalidOperationException("必须填写API调用地址");

			var handler = new HttpLoggingHandler(new HttpClientHandler());
			_httpClient = new(handler) { BaseAddress = new Uri(settings.ApiBaseUrl) };
			UpdateSettings(settings);
		}

		/// <summary>
		/// 根据当前设置构建Kernel实例
		/// </summary>
		public Kernel BuildKernel()
		{
			var builder = Kernel.CreateBuilder();

			// 根据提供商类型注册不同的Chat Completion服务

			// OpenAI官方API，或自定义端点（Ollama、OpenRouter、国内服务商等）
			//builder.AddOpenAIChatCompletion(_settings.ModelId, _settings.ApiKey, httpClient: CreateHttpClientWithBaseUrl(_settings.ApiBaseUrl));

			// 微软Azure OpenAI服务
			//builder.AddAzureOpenAIChatCompletion(deploymentName: _settings.AzureDeploymentName, endpoint: _settings.AzureEndpoint, apiKey: _settings.ApiKey);

			// 自定义兼容端点
			builder.AddOpenAIChatCompletion(_settings.ModelId, _settings.ApiKey, httpClient: _httpClient);
			//builder.AddOpenAIChatCompletion(_settings.ModelId, new Uri(_settings.ApiBaseUrl), _settings.ApiKey);

			// 将自定义的日志拦截器注册到服务中
			builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionLoggingFilter>();

			// 构建Kernel
			return builder.Build();
		}

		/// <summary>
		/// 创建只读Kernel
		/// </summary>
		/// <returns></returns>
		public Kernel CreateReadOnlyKernel()
		{
			var kernel = BuildKernel();
			kernel.Plugins.AddFromObject(new FileSystemReaderPlugin(_workingDirectory), "FileSystemReader");
			return kernel;
		}

		/// <summary>
		/// 创建读写Kernel
		/// </summary>
		/// <returns></returns>
		public Kernel CreateWriterKernel()
		{
			var kernel = CreateReadOnlyKernel();
			kernel.Plugins.AddFromObject(new FileSystemWriterPlugin(_workingDirectory), "FileSystemWriter");
			return kernel;
		}

		/// <summary>
		/// 创建拥有所有插件权限的Kernel
		/// </summary>
		/// <returns></returns>
		public Kernel CreateCompleteKernel()
		{
			var kernel = CreateWriterKernel();
			kernel.Plugins.AddFromObject(new TerminalPlugin(_workingDirectory), "Terminal");
			return kernel;
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
		public void UpdateSettings(AppSettings settings)
		{
			_settings = settings;
			SetWorkingDirectory(settings.WorkingDirectory);
		}
	}
}
