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
        /// </summary>
        public string ApiBaseUrl { get; set; } = "http://192.168.9.8/";

        /// <summary>主模型标识符</summary>
        public string ModelId { get; set; } = "glm47-gguf";

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

        /// <summary>最大思考轮次</summary>
        public int MaxThinkingRounds { get; set; } = 100;

        /// <summary>子代理最大开发轮次</summary>
        public int SubMaxThinkingRounds { get; set; } = 100;

        /// <summary>AI输出最大Token数</summary>
        public int MaxTokens { get; set; } = 128000;

        /// <summary>上下文压缩阈值(%)</summary>
        public int ContextCompressionThreshold { get; set; } = 80;

        // ============================================================
        // 工作目录配置
        // ============================================================

        /// <summary>
        /// 默认工作目录
        /// AI生成的代码将在此目录下创建和操作文件
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AICoder";
    }
}
