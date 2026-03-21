using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SimpleAgent
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new MainForm());

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
	}
}