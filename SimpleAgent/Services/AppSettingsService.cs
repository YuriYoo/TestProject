using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleAgent.Models;

namespace SimpleAgent.Services
{
	/// <summary>
	/// 应用程序设置持久化服务
	/// 负责将AppSettings对象序列化为JSON文件并从文件恢复
	/// </summary>
	public class AppSettingsService
	{
		/// <summary>配置文件存储目录</summary>
		private static readonly string AppDataDir = Environment.CurrentDirectory;

		/// <summary>配置文件完整路径</summary>
		private static readonly string SettingsFilePath = Path.Combine(AppDataDir, "settings.json");

		/// <summary>当前加载的设置（单例缓存）</summary>
		private AppSettings _currentSettings = new();

		/// <summary>
		/// 获取当前应用程序设置
		/// 首次调用时从文件加载，之后返回缓存的设置对象
		/// </summary>
		public AppSettings Settings => _currentSettings;

		/// <summary>
		/// 从文件加载设置
		/// 如果文件不存在或读取失败，返回默认设置
		/// </summary>
		public AppSettings Load()
		{
			try
			{
				Trace.WriteLine($"[设置服务] 尝试从文件加载设置: {SettingsFilePath}");

				// 确保目录存在
				Directory.CreateDirectory(AppDataDir);

				if (File.Exists(SettingsFilePath))
				{
					string json = File.ReadAllText(SettingsFilePath);
					_currentSettings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
					return _currentSettings;
				}
			}
			catch (Exception ex)
			{
				// 读取失败时使用默认设置，不中断程序运行
				System.Diagnostics.Debug.WriteLine($"[设置服务] 加载设置失败，使用默认值: {ex.Message}");
			}

			// 返回默认设置
			_currentSettings = new AppSettings();
			return _currentSettings;
		}

		/// <summary>
		/// 保存设置到文件
		/// </summary>
		/// <param name="settings">要保存的设置对象</param>
		/// <returns>保存是否成功</returns>
		public bool Save(AppSettings settings)
		{
			try
			{
				Directory.CreateDirectory(AppDataDir);
				string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
				File.WriteAllText(SettingsFilePath, json);
				_currentSettings = settings;
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[设置服务] 保存设置失败: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// 重置设置为默认值
		/// </summary>
		public void Reset()
		{
			_currentSettings = new AppSettings();
			Save(_currentSettings);
		}
	}

}
