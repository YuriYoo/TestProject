using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using SimpleAgent.Plugins;
using SimpleAgent.Services;
using static System.Windows.Forms.DataFormats;

namespace SimpleAgent
{
    internal static class Program
    {
        /// <summary>声明全局服务提供者</summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 处理 UI 线程的未处理异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // 处理 非 UI 线程（后台线程）的未处理异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // 处理程序正常退出或被 AppDomain 卸载时的事件
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);


            // 配置 Serilog 规则
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // 设置最低记录级别
                .WriteTo.Console()    // 输出到控制台
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

            // 注册窗体和业务类
            services.AddSingleton<ISettingsService, AppSettingsService>();
            services.AddSingleton<IKernelService, KernelService>();
            services.AddSingleton<IBackgroundService, Services.BackgroundService>();
            services.AddTransient<MultiAgentOrchestrator>();
            services.AddSingleton<GPUStackClient>();
            services.AddSingleton<ChatUIService>();
            services.AddTransient<MainForm>();

            // 注册插件
            services.AddTransient<TerminalPlugin>();
            services.AddTransient<FileSystemPlugin>();
            services.AddTransient<WorkflowPlugin>();
            services.AddTransient<HttpTestPlugin>();

            // 构建服务提供者
            ServiceProvider = services.BuildServiceProvider();

            try
            {
                // 从 DI 容器中“解析”出主窗体
                var mainForm = ServiceProvider.GetRequiredService<MainForm>();

                //ApplicationConfiguration.Initialize();
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

            // Semantic Kernel 记录敏感的提示词（Prompt）和回复，方便在面板中查看
            /*AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

			Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://127.0.0.1:4317");

			// 创建现代的应用程序构建器
			var builder = Host.CreateApplicationBuilder();
			builder.AddServiceDefaults();

			// 把主窗体注册到依赖注入容器中
			builder.Services.AddTransient<MainForm>();

			// 构建并启动后台服务
			var host = builder.Build();

			// 必须手动启动 Host，唤醒 OpenTelemetry 后台服务
			host.Start();

			try
			{
				// 运行窗体，这会阻塞主线程，直到用户关闭窗体
				Application.Run(host.Services.GetRequiredService<MainForm>());
			}
			finally
			{
				// 窗体关闭后，优雅地停止 Host。
				// 这一步极其重要！它会强制 OpenTelemetry 把还没来得及发送的最后一批日志（Flush）发送给 Dashboard！
				host.StopAsync().GetAwaiter().GetResult();
				host.Dispose();
			}*/
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
            //MessageBox.Show("发生UI线程错误，程序将退出。错误信息：" + e.Exception.Message);
            Application.Exit();
        }

        /// <summary>
        /// 非 UI 线程异常处理（一旦触发，程序必然会崩溃退出，无法阻止，但可以趁机清理）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PerformCleanup();
            var ex = e.ExceptionObject as Exception;
            //MessageBox.Show("发生严重系统错误，程序必须退出。错误信息：" + (ex?.Message ?? "未知错误"));
            // 注意：Environment.Exit 会立即终止进程，确保不会弹出系统默认的崩溃窗口
            Environment.Exit(1);
        }

        /// <summary>
        /// 程序退出事件（正常退出或因异常引发的退出）
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