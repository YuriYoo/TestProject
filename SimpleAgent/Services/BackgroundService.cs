using Microsoft.Extensions.Logging;
using SimpleAgent.Plugins;
using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    public interface IBackgroundService
    {
        public Action<string>? OnAddServer { get; set; }

        public Action<string>? OnRemoveServer { get; set; }

        /// <summary>
        /// 启动后台服务并捕获启动日志
        /// </summary>
        /// <param name="command"></param>
        /// <param name="serviceId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> StartService(string command, string serviceId, string path);

        /// <summary>
        /// 停止并清理后台服务
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        string StopService(string serviceId);

        /// <summary>
        /// 停止全部后台服务
        /// </summary>
        void StopAll();

        /// <summary>
        /// 获取后台服务的最新日志输出，如果服务已经退出了，则返回最后的日志和退出状态，并清理字典
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        string GetServiceLogs(string serviceId);
    }

    public class BackgroundService : IBackgroundService
    {
        /// <summary>用于包装后台进程及其日志缓存</summary>
        private class RunningService
        {
            public Process Process { get; }
            private readonly StringBuilder _logs = new();
            private readonly object _lockObj = new();
            private readonly int terminalTruncation;

            public RunningService(Process process, int terminalTruncation)
            {
                Process = process;
                this.terminalTruncation = terminalTruncation;
            }

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
                    return Utility.Utility.TruncateByChars(currentLogs, terminalTruncation);
                }
            }
        }

        private readonly ILogger<BackgroundService> logger;
        private readonly ISettingsService settings;

        /// <summary>用于管理后台运行的进程字典</summary>
        private readonly Dictionary<string, RunningService> _backgroundServices = [];

        public Action<string>? OnAddServer { get; set; }

        public Action<string>? OnRemoveServer { get; set; }

        public BackgroundService(ILogger<BackgroundService> logger, ISettingsService settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public async Task<string> StartService(string command, string serviceId, string path)
        {
            if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(serviceId)) return "[错误] 命令或服务ID不能为空。";
            if (_backgroundServices.ContainsKey(serviceId)) return $"[错误] 服务 ID '{serviceId}' 已存在且可能正在运行。请先停止它。";

            try
            {
                var process = new Process { StartInfo = CreateProcessStartInfo(command, path) };
                var runningService = new RunningService(process, settings.Current.TerminalTruncation);

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
                    StopService(serviceId);
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

        public string StopService(string serviceId)
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

        public void StopAll()
        {
            foreach (var serviceId in _backgroundServices.Keys.ToList())
            {
                StopService(serviceId);
            }
        }

        public string GetServiceLogs(string serviceId)
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
        private void AddServer(string serviceId, RunningService runningService)
        {
            _backgroundServices[serviceId] = runningService;
            OnAddServer?.Invoke(serviceId);
        }

        /// <summary>
        /// 从字典中移除服务
        /// </summary>
        /// <param name="serviceId"></param>
        private void RemoveServer(string serviceId)
        {
            _backgroundServices.Remove(serviceId);
            OnRemoveServer?.Invoke(serviceId);
        }

        /// <summary>
        /// 创建进程启动信息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ProcessStartInfo CreateProcessStartInfo(string command, string path)
        {
            return new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = path,
                UseShellExecute = false, // 必须为 false 才能重定向输出
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
        }
    }
}
