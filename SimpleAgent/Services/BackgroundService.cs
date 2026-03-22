using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
	public class BackgroundService
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
		private static readonly Dictionary<string, RunningService> _backgroundServices = [];

		public static ListBox serverListBox;

		/// <summary>
		/// 启动后台服务并捕获启动日志
		/// </summary>
		/// <param name="command"></param>
		/// <param name="serviceId"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static async Task<string> StartService(string command, string serviceId, string path)
		{
			if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(serviceId)) return "[错误] 命令或服务ID不能为空。";
			if (_backgroundServices.ContainsKey(serviceId)) return $"[错误] 服务 ID '{serviceId}' 已存在且可能正在运行。请先停止它。";

			try
			{
				var process = new Process { StartInfo = CreateProcessStartInfo(command, path) };
				var runningService = new RunningService { Process = process };

				// 绑定数据接收事件，写入到线程安全的缓存中
				process.OutputDataReceived += (sender, e) => runningService.AppendLog("STDOUT", e.Data);
				process.ErrorDataReceived += (sender, e) => runningService.AppendLog("STDERR", e.Data);

				if (!process.Start()) return "[错误] 无法启动进程。";

				// 启动异步读取操作，这不会阻塞当前线程
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				// 将进程注册到字典中管理
				AddServer(serviceId, runningService);

				// 等待 3 秒钟，为了捕捉服务器因为端口被占用、语法错误等原因导致的“见光死”。
				await Task.Delay(3000);

				if (process.HasExited)
				{
					// 如果 3 秒内进程退出了，说明启动失败
					RemoveServer(serviceId);
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

		/// <summary>
		/// 停止并清理后台服务
		/// </summary>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		public static string StopService(string serviceId)
		{
			if (_backgroundServices.TryGetValue(serviceId, out var service))
			{
				try
				{
					if (!service.Process.HasExited)
					{
						service.Process.Kill(true); // 终结整个进程树
					}
					RemoveServer(serviceId);
					return $"[成功] 已成功停止并清理后台服务 '{serviceId}'。";
				}
				catch (Exception ex)
				{
					return $"[错误] 停止服务时发生异常: {ex.Message}";
				}
			}
			return $"[错误] 找不到名为 '{serviceId}' 的后台服务，它可能已经退出或从未启动。";
		}

		public static void StopAll()
		{
			foreach (var serviceId in _backgroundServices.Keys.ToList())
			{
				StopService(serviceId);
			}
		}

		/// <summary>
		/// 获取后台服务的最新日志输出，如果服务已经退出了，则返回最后的日志和退出状态，并清理字典
		/// </summary>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		public static string GetServiceLogs(string serviceId)
		{
			if (_backgroundServices.TryGetValue(serviceId, out var service))
			{
				if (service.Process.HasExited)
				{
					// 如果进程已经挂了，把最后的遗言打印出来并清理字典
					var finalLogs = service.FlushLogs();
					RemoveServer(serviceId);
					return $"[提示] 服务 '{serviceId}' 已停止运行 (退出码: {service.Process.ExitCode})。\n[最后日志]:\n{finalLogs}";
				}

				return $"[服务 '{serviceId}' 的最新日志]:\n{service.FlushLogs()}";
			}

			return $"[错误] 找不到名为 '{serviceId}' 的服务，它可能未启动或已经被清理。";
		}

		/// <summary>
		/// 添加服务到字典中
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="runningService"></param>
		private static void AddServer(string serviceId, RunningService runningService)
		{
			_backgroundServices[serviceId] = runningService;
			serverListBox?.Items.Add(serviceId);
		}

		/// <summary>
		/// 从字典中移除服务
		/// </summary>
		/// <param name="serviceId"></param>
		private static void RemoveServer(string serviceId)
		{
			_backgroundServices.Remove(serviceId);
			if (serverListBox != null && serverListBox.Items.Contains(serviceId))
			{
				serverListBox.Items.Remove(serviceId);
			}
		}

		/// <summary>
		/// 创建进程启动信息
		/// </summary>
		/// <param name="command"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private static ProcessStartInfo CreateProcessStartInfo(string command, string path)
		{
			return new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/c {command}",
				WorkingDirectory = path,
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
		public static string TruncateByChars(string text, int x)
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
