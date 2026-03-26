using System.IO;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SimpleAgent.Services
{
    internal class ChatHistoryManager
    {
        /// <summary>
        /// 预定义全局的序列化配置
        /// </summary>
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            // 格式化输出，方便阅读
            WriteIndented = true,

            // 防止中文和特殊符号被转义为 \uXXXX 格式，保持明文显示
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// 将完整的 ChatHistory 序列化并保存到本地文件
        /// </summary>
        /// <param name="chatHistory">当前的对话历史</param>
        /// <param name="filePath">保存路径</param>
        public static void Save(ChatHistory chatHistory, string filePath)
        {
            // 原生序列化：SK 内部已处理好多态和复杂对象的映射
            string json = JsonSerializer.Serialize(chatHistory, jsonOptions);

            // 写入文件
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 从本地文件加载并反序列化还原为 ChatHistory
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <returns>还原后的 ChatHistory 对象，如果文件不存在则返回全新实例</returns>
        public static ChatHistory Load(string filePath)
        {
            // 如果文件不存在，直接返回一个空的 ChatHistory
            if (!File.Exists(filePath)) return [];

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json)) return [];

            // 原生反序列化：完美还原 Token Usage, Function Calls 等所有细节
            var chatHistory = JsonSerializer.Deserialize<ChatHistory>(json, jsonOptions);

            // 确保不会返回 null
            return chatHistory ?? [];
        }
    }
}
