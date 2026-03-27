using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    public interface IKernelService
    {
        /// <summary>
        /// 构建Kernel实例
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Kernel BuildKernel(AgentContext? context);
    }

    /// <summary>
    /// SemanticKernel核心服务
    /// 负责初始化Kernel、注册插件、提供AI聊天和流式输出能力
    /// 是整个AI系统的中枢控制层
    /// </summary>
    public class KernelService : IKernelService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISettingsService settings;
        private readonly IHttpClientFactory httpClientFactory;

        public KernelService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ISettingsService settings)
        {
            this.serviceProvider = serviceProvider;
            this.settings = settings;
            this.httpClientFactory = httpClientFactory;
        }

        public Kernel BuildKernel(AgentContext? context)
        {
            var builder = Kernel.CreateBuilder();

            var httpClient = httpClientFactory.CreateClient(Constant.HttpClientName);
            httpClient.BaseAddress = new Uri($"{settings.Current.ApiBaseUrl}v1");

            // 自定义兼容端点
            builder.AddOpenAIChatCompletion(settings.Current.ModelId, settings.Current.ApiKey, httpClient: httpClient);

            // 将自定义的日志拦截器注册到服务中
            builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionLoggingFilter>();

            // 注册工具
            if (context != null)
            {
                var httpTestPlugin = ActivatorUtilities.CreateInstance<HttpTestPlugin>(serviceProvider);
                var fileSystemPlugin = ActivatorUtilities.CreateInstance<FileSystemPlugin>(serviceProvider, context);
                var terminalPlugin = ActivatorUtilities.CreateInstance<TerminalPlugin>(serviceProvider, context);
                var workflowPlugin = ActivatorUtilities.CreateInstance<WorkflowPlugin>(serviceProvider, context);
                var subAgentPlugin = ActivatorUtilities.CreateInstance<SubAgentPlugin>(serviceProvider, context);

                builder.Plugins.AddFromObject(httpTestPlugin, "http_test");
                builder.Plugins.AddFromObject(fileSystemPlugin, "file_system");
                builder.Plugins.AddFromObject(terminalPlugin, "terminal");
                builder.Plugins.AddFromObject(workflowPlugin, "workflow");
                builder.Plugins.AddFromObject(subAgentPlugin, "sub_agent");
            }

            // 构建Kernel
            return builder.Build();
        }
    }
}
