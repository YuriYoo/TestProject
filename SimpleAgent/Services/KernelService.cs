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
    public interface IKernelService
    {
        /// <summary>
        /// 构建Kernel实例
        /// </summary>
        /// <param name="directory">工作目录</param>
        /// <returns></returns>
        Kernel BuildKernel(string directory);
    }

    /// <summary>
    /// SemanticKernel核心服务
    /// 负责初始化Kernel、注册插件、提供AI聊天和流式输出能力
    /// 是整个AI系统的中枢控制层
    /// </summary>
    public class KernelService : IKernelService
    {
        /// <summary>http客户端</summary>
        private HttpClient _httpClient;

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<KernelService> logger;
        private readonly ISettingsService settings;

        public KernelService(IServiceProvider serviceProvider, ILogger<KernelService> logger, ISettingsService settings)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.settings = settings;

            if (string.IsNullOrEmpty(settings.Current.ApiBaseUrl)) throw new InvalidOperationException("必须填写API调用地址");

            var handler = new HttpLoggingHandler(new HttpClientHandler());
            _httpClient = new(handler) { BaseAddress = new Uri($"{settings.Current.ApiBaseUrl}v1") };
        }

        public Kernel BuildKernel(string directory)
        {
            directory = SetWorkingDirectory(directory);
            var builder = Kernel.CreateBuilder();

            // 自定义兼容端点
            builder.AddOpenAIChatCompletion(settings.Current.ModelId, settings.Current.ApiKey, httpClient: _httpClient);

            // 将自定义的日志拦截器注册到服务中
            builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionLoggingFilter>();

            // 注册工具
            var fileSystemPlugin = serviceProvider.GetRequiredService<FileSystemPlugin>();
            var terminalPlugin = serviceProvider.GetRequiredService<TerminalPlugin>();
            var httpTestPlugin = serviceProvider.GetRequiredService<HttpTestPlugin>();

            fileSystemPlugin.WorkingDirectory = directory;
            terminalPlugin.WorkingDirectory = directory;

            builder.Plugins.AddFromObject(httpTestPlugin, "http_test");
            builder.Plugins.AddFromObject(fileSystemPlugin, "file_system");
            builder.Plugins.AddFromObject(terminalPlugin, "terminal");

            // 构建Kernel
            return builder.Build();
        }

        /// <summary>
        /// 设置工作目录
        /// </summary>
        private string SetWorkingDirectory(string directory)
        {
            // 确保工作目录本身是绝对路径，并且以目录分隔符结尾，防止 "C:\Work" 匹配到 "C:\Workspace"
            var _workingDirectory = Path.GetFullPath(directory);
            if (!_workingDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                _workingDirectory += Path.DirectorySeparatorChar;
            }
            Directory.CreateDirectory(directory);
            return _workingDirectory;
        }
    }
}
