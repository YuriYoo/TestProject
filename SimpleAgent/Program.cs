using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using SimpleAgent.Agents;
using SimpleAgent.Factory;
using SimpleAgent.Plugins;
using SimpleAgent.Reducer;
using SimpleAgent.Services;
using SimpleAgent.Utility;
using static System.Windows.Forms.DataFormats;

namespace SimpleAgent
{
    internal static class Program
    {
        /// <summary>声明全局服务提供者</summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            // 配置 Serilog 规则
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                // Debug模式下输出信息到调试控制台
                .WriteTo.Debug()
                // 屏蔽 HttpClient 工厂底层的 Debug 和 Info 日志，只看警告和错误
                .MinimumLevel.Override("Microsoft.Extensions.Http", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File(
                    path: "Logs/app_log_.txt",
                    rollingInterval: RollingInterval.Day, // 每天自动创建一个新文件
                    retainedFileCountLimit: 30,           // 最多保留30天的日志
                    buffered: false                       // 实时写入文件（防止崩溃时丢失）
                )
                .CreateLogger();

            // 配置 DI 容器 (ServiceCollection)
            var services = new ServiceCollection();

            // 注册日志服务
            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            // 注册命名的 HttpClient 并绑定日志处理程序
            services.AddTransient<HttpLoggingHandler>();
            services.AddHttpClient(Constant.HttpClientName).AddHttpMessageHandler<HttpLoggingHandler>();

            // 注册智能体
            services.AddTransient<IWorkflowAgent, PlannerAgent>();
            services.AddTransient<IWorkflowAgent, DeveloperAgent>();
            services.AddTransient<IWorkflowAgent, ReviewerAgent>();
            services.AddTransient<IWorkflowAgent, RouterAgent>();
            services.AddTransient<IWorkflowAgent, SubDeveloperAgent>();

            // 注册窗体和业务类
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IKernelService, KernelService>();
            services.AddSingleton<IBackgroundService, Services.BackgroundService>();
            services.AddSingleton<IStreamingExecutionEngine, StreamingExecutionEngine>();
            services.AddSingleton<AgentContextRepository>();
            services.AddTransient<MultiAgentOrchestrator>();
            services.AddSingleton<GPUStackClient>();
            services.AddSingleton<ChatUIService>();
            services.AddTransient<MainForm>();

            // 工厂类
            services.AddSingleton<IAgentFactory, AgentFactory>();
            services.AddSingleton<IOrchestratorFactory, OrchestratorFactory>();

            // 注册插件
            services.AddTransient<FileSystemPlugin>();
            services.AddTransient<TerminalPlugin>();
            services.AddTransient<HttpTestPlugin>();
            services.AddTransient<WorkflowPlugin>();
            services.AddTransient<SubAgentPlugin>();

            // 上下文裁剪
            services.AddTransient<IChatHistoryReducer, CustomChatHistoryReducer>();

            // 构建服务提供者
            ServiceProvider = services.BuildServiceProvider();

            // 核心的事件解耦
            var engine = ServiceProvider.GetRequiredService<IStreamingExecutionEngine>();
            var chatUI = ServiceProvider.GetRequiredService<ChatUIService>();

            // 把 UI 更新方法绑定到独立引擎的事件上，业务代码无需知道 UI 的存在
            engine.OnMessageReceived += chatUI.SendAIMessage;
            engine.OnStreamCompleted += chatUI.SendCompletedMessage;
            engine.OnToolCall += chatUI.SendToolMessage;

            try
            {
                // 从 DI 容器中“解析”出主窗体
                var mainForm = ServiceProvider.GetRequiredService<MainForm>();

                Application.Run(mainForm);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// UI 线程异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            PerformCleanup();
            // 可以选择记录日志并退出，或者让用户决定是否继续运行
            Log.Error("发生UI线程错误，程序将退出。错误信息：{msg}", e.Exception.Message);
            Application.Exit();
        }

        /// <summary>
        /// 非 UI 线程（后台线程）异常处理（一旦触发，程序必然会崩溃退出，无法阻止，但可以趁机清理）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PerformCleanup();
            var ex = e.ExceptionObject as Exception;
            Log.Error("发生严重系统错误，程序必须退出。错误信息：{msg}", ex?.Message ?? "未知错误");
            // 注意：Environment.Exit 会立即终止进程，确保不会弹出系统默认的崩溃窗口
            Environment.Exit(1);
        }

        /// <summary>
        /// 程序退出事件（正常退出、被AppDomain卸载或因异常引发的退出）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            PerformCleanup();
        }

        /// <summary>
        /// 统一的清理方法
        /// </summary>
        static void PerformCleanup()
        {
            try
            {
                var service = ServiceProvider.GetService<IBackgroundService>();
                service?.StopAll();
            }
            catch (Exception)
            {

            }
        }
    }
}