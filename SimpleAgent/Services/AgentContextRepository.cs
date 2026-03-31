using Microsoft.SemanticKernel.ChatCompletion;
using Serilog;
using SimpleAgent.Models;
using System.IO;
using System.Text.Json;

namespace SimpleAgent.Services
{
    public class AgentContextRepository
    {
        private readonly string _storageDirectory;

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

        public AgentContextRepository()
        {
            // 获取本地 AppData 目录
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _storageDirectory = Path.Combine(appDataPath, "SimpleAgent", "Conversations");

            // 如果目录不存在，初始化时自动创建
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        /// <summary>
        /// 获取一个上下文，如果不存在则创建一个新的
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<AgentContext> GetOrCreateContextAsync(Guid conversationId)
        {
            // 尝试从本地文件读取
            var context = await LoadContextAsync(conversationId);

            // 如果文件不存在，就新建一个
            context ??= new AgentContext { ConversationId = conversationId };

            return context;
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<AgentContext?> LoadContextAsync(Guid conversationId)
        {
            // 如果文件不存在，说明是全新的对话，返回 null 让工厂去初始化
            string basePath = GetFilePath(conversationId);
            if (!File.Exists(basePath)) return null;

            try
            {
                // 异步读取本地文件并反序列化
                string contextFilePath = GetFilePath(conversationId);
                using FileStream stream = File.OpenRead(contextFilePath);
                var context = JsonSerializer.Deserialize<AgentContext>(stream);
                if (context == null) return null;

                var pc = Load(GetFilePath(conversationId, AgentType.Planner));
                context.ChatHistory.Add(AgentType.Planner, pc);
                var dc = Load(GetFilePath(conversationId, AgentType.Developer));
                context.ChatHistory.Add(AgentType.Developer, dc);
                var rc = Load(GetFilePath(conversationId, AgentType.Reviewer));
                context.ChatHistory.Add(AgentType.Reviewer, rc);

                return context;
            }
            catch (Exception ex)
            {
                Log.Error("读取本地对话文件失败: {msg}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 保存上下文
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task SaveContextAsync(AgentContext context)
        {
            // 如果目录不存在，初始化时自动创建
            var basePath = Path.Combine(_storageDirectory, context.ConversationId.ToString());
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            try
            {
                // 保存基础文件
                string contextFilePath = GetFilePath(context.ConversationId);
                using FileStream stream = File.Create(contextFilePath);
                await JsonSerializer.SerializeAsync(stream, context, jsonOptions);

                // 保存历史记录
                foreach (var item in context.ChatHistory)
                {
                    await Save(item.Value, GetFilePath(context.ConversationId, item.Key));
                }
            }
            catch (Exception ex)
            {
                Log.Error("保存本地对话文件失败: {msg}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 删除上下文
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteContext(Guid guid)
        {
            var path = Path.Combine(_storageDirectory, guid.ToString());
            if (!Directory.Exists(path)) return;
            Directory.Delete(path, true);
        }

        /// <summary>
        /// 将完整的 ChatHistory 序列化并保存到本地文件
        /// </summary>
        /// <param name="chatHistory">当前的对话历史</param>
        /// <param name="filePath">保存路径</param>
        private async Task Save(ChatHistory chatHistory, string filePath)
        {
            using FileStream stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, chatHistory, jsonOptions);
        }

        /// <summary>
        /// 从本地文件加载并反序列化还原为 ChatHistory
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <returns>还原后的 ChatHistory 对象，如果文件不存在则返回全新实例</returns>
        private ChatHistory Load(string filePath)
        {
            // 如果文件不存在，直接返回一个空的 ChatHistory
            if (!File.Exists(filePath)) return [];

            using FileStream stream = File.OpenRead(filePath);
            var context = JsonSerializer.Deserialize<ChatHistory>(stream);

            // 确保不会返回 null
            return context ?? [];
        }

        /// <summary>
        /// 获取上下文基础文件的完整路径
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        private string GetFilePath(Guid conversationId)
        {
            return Path.Combine(_storageDirectory, conversationId.ToString(), "context.json");
        }

        /// <summary>
        /// 获取上下文历史记录文件的完整路径
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        private string GetFilePath(Guid conversationId, AgentType type)
        {
            return Path.Combine(_storageDirectory, conversationId.ToString(), $"{type}.json");
        }
    }
}
