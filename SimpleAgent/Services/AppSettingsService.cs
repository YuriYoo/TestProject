using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using SimpleAgent.Models;
using SimpleAgent.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    public interface ISettingsService
    {
        /// <summary>获取当前应用程序的设置</summary>
        AppSettings Current { get; }

        /// <summary>加载本地保存的设置</summary>
        AppSettings Load();

        /// <summary>保存设置到本地</summary>
        bool Save();

        /// <summary>重置设置为默认值</summary>
        void Reset();
    }

    /// <summary>
    /// 应用程序设置持久化服务
    /// 负责将AppSettings对象序列化为JSON文件并从文件恢复
    /// </summary>
    public class AppSettingsService : ISettingsService
    {
        /// <summary>配置文件存储目录</summary>
        private static readonly string AppDataDir = Environment.CurrentDirectory;

        /// <summary>配置文件完整路径</summary>
        private static readonly string SettingsFilePath = Path.Combine(AppDataDir, "settings.json");

        ILogger<AppSettingsService> logger;

        /// <summary>获取当前应用程序的设置</summary>
        public AppSettings Current => current;

        /// <summary>当前加载的设置</summary>
        private AppSettings current = new();

        public AppSettingsService(ILogger<AppSettingsService> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 从文件加载设置
        /// 如果文件不存在或读取失败，返回默认设置
        /// </summary>
        public AppSettings Load()
        {
            try
            {
                // 确保目录存在
                Directory.CreateDirectory(AppDataDir);

                if (File.Exists(SettingsFilePath))
                {
                    logger.LogInformation("正在从本地文件加载设置: {SettingsFilePath}", SettingsFilePath);
                    string json = File.ReadAllText(SettingsFilePath);
                    current = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                    FormatCorrection();
                    return current;
                }
                else
                {
                    logger.LogInformation("未找到设置文件，正在创建默认设置");
                    Save();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("加载设置失败，将会使用默认值: {msg}", ex.Message);
            }

            // 返回默认设置
            return current;
        }

        /// <summary>
        /// 保存设置到文件
        /// </summary>
        /// <returns>保存是否成功</returns>
        public bool Save()
        {
            try
            {
                logger.LogInformation("正在将设置保存到本地: {SettingsFilePath}", SettingsFilePath);
                Directory.CreateDirectory(AppDataDir);
                string json = JsonConvert.SerializeObject(current, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning("保存设置失败: {msg}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 重置设置为默认值
        /// </summary>
        public void Reset()
        {
            current = new();
            Save();
            logger.LogInformation("已将程序设置重置为默认值");
        }

        /// <summary>
        /// 修正配置格式
        /// </summary>
        private void FormatCorrection()
        {
            if (!current.ApiBaseUrl.EndsWith('/'))
            {
                current.ApiBaseUrl += '/';
            }
        }
    }

}
