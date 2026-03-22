using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Models
{
	/// <summary>
	/// 应用程序全局设置
	/// </summary>
	public class AppSettings
	{
		// ============================================================
		// AI服务配置
		// ============================================================

		/// <summary>API密钥</summary>
		public string ApiKey { get; set; } = string.Empty;

		/// <summary>
		/// 自定义API基础URL
		/// 用于本地模型（如Ollama: http://localhost:11434/v1）
		/// 或第三方兼容服务（如通义千问、DeepSeek等）
		/// </summary>
		public string ApiBaseUrl { get; set; } = "http://192.168.9.110/";

		/// <summary>主模型标识符</summary>
		public string ModelId { get; set; } = "glm-4.7-flash";

		// ============================================================
		// 智能体配置
		// ============================================================

		/// <summary>命令行输出截断</summary>
		public int TerminalTruncation { get; set; } = 5000;

		/// <summary>命令行超时时间(毫秒)</summary>
		public int TerminalTimeout { get; set; } = 60000;

		/// <summary>Http响应截断</summary>
		public int HttpTerminal { get; set; } = 3000;

		/// <summary>Http请求超时时间</summary>
		public int HttpTimeout { get; set; } = 15000;

		/// <summary>是否启用多智能体协作模式（关闭则使用单一Agent）</summary>
		public bool EnableMultiAgent { get; set; } = true;

		/// <summary>
		/// 编排模式
		/// - Sequential：顺序执行（规划→编码→审查）
		/// - Adaptive：自适应（根据任务复杂度自动选择）
		/// </summary>
		public OrchestrationMode OrchestrationMode { get; set; } = OrchestrationMode.Sequential;

		/// <summary>AI输出最大Token数</summary>
		public int MaxTokens { get; set; } = 128000;

		/// <summary>生成温度（0.0-2.0）</summary>
		public double Temperature { get; set; } = 0.2;

		// ============================================================
		// 工作目录配置
		// ============================================================

		/// <summary>
		/// 默认工作目录
		/// AI生成的代码将在此目录下创建和操作文件
		/// </summary>
		public string WorkingDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AICoder";
	}

	/// <summary>多智能体编排模式枚举</summary>
	public enum OrchestrationMode
	{
		/// <summary>顺序模式：规划 → 编码 → 审查，依次执行</summary>
		Sequential,
		/// <summary>自适应模式：根据任务复杂度自动选择最佳策略</summary>
		Adaptive
	}
}
