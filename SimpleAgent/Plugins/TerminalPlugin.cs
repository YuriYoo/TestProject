using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SimpleAgent.Models;
using SimpleAgent.Services;
using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Plugins
{
    public class TerminalPlugin
    {
        private readonly ISettingsService settings;
        private readonly IBackgroundService backgroundService;
        private readonly AgentContext context;

        /// <summary>定义 Windows 危险命令黑名单</summary>
        private readonly string[] blacklistedCommands = ["rmdir /s /q", "del /f /s /q"];
        //private readonly string[] blacklistedCommands = { "rmdir ", "rd ", "del ", "format ", "diskpart" };

        public TerminalPlugin(ISettingsService settings, IBackgroundService backgroundService, AgentContext context)
        {
            this.settings = settings;
            this.context = context;
            this.backgroundService = backgroundService;
        }

        [KernelFunction("start_background_service")]
        [Description("【重要】当需要使用一个会运行较长时间的命令（如 'dotnet run', 'npm install', 'python server.py'）时，必须且只能使用此函数！它会在后台启动服务而不阻塞你。")]
        public async Task<string> StartBackgroundServiceAsync(
            [Description("要执行的完整启动命令，例如 'dotnet run'")] string command,
            [Description("为你启动的这个服务起一个简短的英文标识符（如 'web-api', 'frontend'），用于后续停止服务")] string serviceId)
        {
            return await backgroundService.StartService(command, serviceId, context.WorkingDirectory);
        }

        [KernelFunction("stop_background_service")]
        [Description("停止一个之前通过 start_background_service 启动的后台服务。测试完成后应养成清理后台服务的习惯。")]
        public string StopBackgroundService(
            [Description("要停止的服务标识符（之前启动时定义的 ID）")] string serviceId)
        {
            return backgroundService.StopService(serviceId);
        }

        [KernelFunction("get_service_logs")]
        [Description("获取后台服务运行期间产生的最新日志。获取后会清空已读取的日志缓冲，下次获取将只会返回新的日志。")]
        public string GetServiceLogs(
            [Description("要获取日志的服务标识符（启动时定义的 ID）")] string serviceId)
        {
            return backgroundService.GetServiceLogs(serviceId);
        }

        [KernelFunction("execute_command")]
        [Description("在 Windows 命令行 (cmd) 中执行终端命令。可用于检测软件环境、编译代码 (如 dotnet build)、运行测试 (如 dotnet test)、或执行脚本。")]
        public async Task<string> ExecuteCommandAsync(
            [Description("需要执行的完整 CMD 命令，例如 'dotnet test' 或 'python script.py'，如果需要长时间运行请使用 'start_background_service' 工具")] string command,
            [Description("运行命令的超时时间(毫秒)，防止命令无法停止时把你阻塞住，默认 10000 毫秒")] int timeout = 10000)
        {
            if (string.IsNullOrWhiteSpace(command)) return "[错误] 命令不能为空。";

            // 拦截 Windows 环境下的常见破坏性命令
            if (blacklistedCommands.Any(command.ToLowerInvariant().Contains))
            {
                return "[错误] 该命令包含被禁止的危险操作，已拦截。";
            }

            try
            {
                var sb = new StringBuilder();
                using var process = new Process { StartInfo = BackgroundService.CreateProcessStartInfo(command, context.WorkingDirectory) };
                if (process == null) return "[错误] 无法启动命令行进程。";
                process.Start();

                // 异步读取标准输出和错误输出
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // 使用 CancellationToken 实现异步超时控制
                var time = timeout > 0 ? Math.Max(timeout, settings.Current.TerminalTimeout) : settings.Current.TerminalTimeout;
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout));

                try
                {
                    // 异步等待进程退出，不会阻塞线程
                    await process.WaitForExitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 超时触发，强制结束进程树 (包含可能衍生出的子进程)
                    process.Kill(true);
                    return $"[错误] 命令运行超过了{timeout}ms, 已自动终止。";
                }

                // 确保完整读取输出流
                var output = await outputTask;
                var error = await errorTask;

                sb.AppendLine($"[退出码] {process.ExitCode}");
                if (!string.IsNullOrWhiteSpace(output)) sb.AppendLine($"[标准输出]\n{output}");
                if (!string.IsNullOrWhiteSpace(error)) sb.AppendLine($"[错误输出]\n{error}");

                if (process.ExitCode == 0)
                    sb.AppendLine("[状态] 命令执行成功");
                else
                    sb.AppendLine($"[状态] 命令以错误码 {process.ExitCode} 退出");

                return Utility.Utility.TruncateByChars(sb.ToString(), settings.Current.TerminalTruncation);
            }
            catch (Exception ex)
            {
                return $"[错误] 执行命令时发生异常: {ex.Message}";
            }
        }
    }
}
