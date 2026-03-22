using Microsoft.SemanticKernel;
using SimpleAgent.Services;
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
		/// <summary>用于包装后台进程及其日志缓存</summary>
		private class RunningService
		{
			public Process Process { get; set; }
			private readonly StringBuilder _logs = new();
			private readonly object _lockObj = new();

			public void AppendLog(string logType, string data)
			{
				if (string.IsNullOrWhiteSpace(data)) return;
				lock (_lockObj)
				{
					// 给每行日志打上时间戳和类型标签
					_logs.AppendLine($"[{DateTime.Now:HH:mm:ss}] [{logType}] {data}");
				}
			}

			public string FlushLogs()
			{
				lock (_lockObj)
				{
					if (_logs.Length == 0) return "暂无新日志产生。";
					var currentLogs = _logs.ToString();
					_logs.Clear(); // 阅后即焚，清空缓冲区
					return TruncateByChars(currentLogs, AppSettingsService.Settings.TerminalTruncation);
				}
			}
		}

		/// <summary>用于管理后台运行的进程字典</summary>
		private readonly Dictionary<string, RunningService> _backgroundServices = [];

		private readonly string _workingDirectory;

		/// <summary>定义 Windows 危险命令黑名单</summary>
		private readonly string[] blacklistedCommands = { "rmdir /s /q", "del /f /s /q" };
		//private readonly string[] blacklistedCommands = { "rmdir ", "rd ", "del ", "format ", "diskpart" };

		public TerminalPlugin(string workingDirectory)
		{
			_workingDirectory = workingDirectory;
		}

		[KernelFunction("start_background_service")]
		[Description("【重要】当需要启动一个会长时间运行的服务器（如 'dotnet run', 'npm start', 'python server.py'）时，必须且只能使用此函数！它会在后台启动服务而不阻塞你。")]
		public async Task<string> StartBackgroundServiceAsync(
			[Description("要执行的完整启动命令，例如 'dotnet run'")] string command,
			[Description("为你启动的这个服务起一个简短的英文标识符（如 'web-api', 'frontend'），用于后续停止服务")] string serviceId)
		{
			if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(serviceId)) return "[错误] 命令或服务ID不能为空。";
			if (_backgroundServices.ContainsKey(serviceId)) return $"[错误] 服务 ID '{serviceId}' 已存在且可能正在运行。请先停止它。";

			try
			{
				var process = new Process { StartInfo = CreateProcessStartInfo(command) };
				var runningService = new RunningService { Process = process };

				// 绑定数据接收事件，写入到线程安全的缓存中
				process.OutputDataReceived += (sender, e) => runningService.AppendLog("STDOUT", e.Data);
				process.ErrorDataReceived += (sender, e) => runningService.AppendLog("STDERR", e.Data);

				if (!process.Start()) return "[错误] 无法启动进程。";

				// 启动异步读取操作，这不会阻塞当前线程
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				// 将进程注册到字典中管理
				_backgroundServices[serviceId] = runningService;

				// 等待 3 秒钟，为了捕捉服务器因为端口被占用、语法错误等原因导致的“见光死”。
				await Task.Delay(3000);

				if (process.HasExited)
				{
					// 如果 3 秒内进程退出了，说明启动失败
					_backgroundServices.Remove(serviceId);
					return $"[失败] 服务启动失败了。退出码: {process.ExitCode}\n[错误日志]:\n{runningService.FlushLogs()}";
				}

				// 如果 3 秒后还在运行，说明大概率启动成功了，返回当前截获的前 3 秒日志
				return $"[成功] 后台服务 '{serviceId}' 已启动（PID: {process.Id}）\n[启动日志]:\n{runningService.FlushLogs()}";
			}
			catch (Exception ex)
			{
				return $"[错误] 启动后台服务时发生异常: {ex.Message}";
			}
		}

		[KernelFunction("stop_background_service")]
		[Description("停止一个之前通过 start_background_service 启动的后台服务。测试完成后应养成清理后台服务的习惯。")]
		public string StopBackgroundService(
			[Description("要停止的服务标识符（之前启动时定义的 ID）")] string serviceId)
		{
			if (_backgroundServices.TryGetValue(serviceId, out var service))
			{
				try
				{
					if (!service.Process.HasExited)
					{
						service.Process.Kill(true); // 终结整个进程树
					}
					_backgroundServices.Remove(serviceId);
					return $"[成功] 已成功停止并清理后台服务 '{serviceId}'。";
				}
				catch (Exception ex)
				{
					return $"[错误] 停止服务时发生异常: {ex.Message}";
				}
			}
			return $"[错误] 找不到名为 '{serviceId}' 的后台服务，它可能已经退出或从未启动。";
		}

		[KernelFunction("get_service_logs")]
		[Description("获取后台服务运行期间产生的最新日志。获取后会清空已读取的日志缓冲，下次获取将只会返回新的日志。")]
		public string GetServiceLogs(
			[Description("要获取日志的服务标识符（启动时定义的 ID）")] string serviceId)
		{
			if (_backgroundServices.TryGetValue(serviceId, out var service))
			{
				if (service.Process.HasExited)
				{
					// 如果进程已经挂了，把最后的遗言打印出来并清理字典
					var finalLogs = service.FlushLogs();
					_backgroundServices.Remove(serviceId);
					return $"[提示] 服务 '{serviceId}' 已停止运行 (退出码: {service.Process.ExitCode})。\n[最后日志]:\n{finalLogs}";
				}

				return $"[服务 '{serviceId}' 的最新日志]:\n{service.FlushLogs()}";
			}

			return $"[错误] 找不到名为 '{serviceId}' 的服务，它可能未启动或已经被清理。";
		}

		[KernelFunction("execute_command")]
		[Description("在 Windows 命令行 (cmd) 中执行终端命令。可用于编译代码 (如 dotnet build)、运行测试 (如 dotnet test)、或执行脚本。")]
		public async Task<string> ExecuteCommandAsync(
			[Description("需要执行的完整 CMD 命令，例如 'dotnet test' 或 'python script.py'")] string command)
		{
			if (string.IsNullOrWhiteSpace(command)) return "[错误] 命令不能为空。";
			if (command.StartsWith("rmdir /s /q") || command.StartsWith("del /f /s /q")) return "[错误] 该命令会引起不可控的后果，禁止使用该命令。";

			// 改进的安全校验：拦截 Windows 环境下的常见破坏性命令
			if (blacklistedCommands.Any(command.ToLowerInvariant().Contains))
			{
				return "[错误] 该命令包含被禁止的危险操作，已拦截。";
			}

			var sb = new StringBuilder();

			try
			{
				using var process = new Process { StartInfo = CreateProcessStartInfo(command) };
				if (process == null) return "[错误] 无法启动命令行进程。";
				process.Start();

				// 异步读取标准输出和错误输出
				var outputTask = process.StandardOutput.ReadToEndAsync();
				var errorTask = process.StandardError.ReadToEndAsync();

				// 使用 CancellationToken 实现异步超时控制
				using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(AppSettingsService.Settings.TerminalTimeout));

				try
				{
					// 异步等待进程退出，不会阻塞线程
					await process.WaitForExitAsync(cts.Token);
				}
				catch (OperationCanceledException)
				{
					// 超时触发，强制结束进程树 (包含可能衍生出的子进程)
					process.Kill(true);
					return $"[错误] 命令运行超过了{AppSettingsService.Settings.TerminalTimeout}秒, 已自动终止。";
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

				return TruncateByChars(sb.ToString(), AppSettingsService.Settings.TerminalTruncation);
			}
			catch (Exception ex)
			{
				return $"[错误] 执行命令时发生异常: {ex.Message}";
			}
		}

		/// <summary>
		/// 创建进程启动信息
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		private ProcessStartInfo CreateProcessStartInfo(string command)
		{
			return new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/c {command}",
				WorkingDirectory = _workingDirectory,
				UseShellExecute = false, // 必须为 false 才能重定向输出
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8,
			};
		}

		/// <summary>
		/// 保留字符串的指定数量的字符
		/// 将保留 x / 2 个字符在开头，x / 2 个字符在结尾，中间用提示信息替代
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		private static string TruncateByChars(string text, int x)
		{
			// 如果字符串为空，或者总长度还没有x长，则直接返回原字符串
			if (string.IsNullOrEmpty(text) || text.Length <= x)
			{
				return text;
			}

			int half = x / 2;
			string head = text.Substring(0, half);
			string tail = text.Substring(text.Length - half);

			return $"{head}\n...[内容过长，已被系统隐藏 {text.Length - x} 个字符]...\n{tail}";
		}
	}
}
