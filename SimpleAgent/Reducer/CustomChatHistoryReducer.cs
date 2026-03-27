using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Reducer
{
    public class CustomChatHistoryReducer : IChatHistoryReducer
    {
        private const string SummarizationPrompt = "请简要总结以下对话内容，提取核心信息与结论：\n";

        private readonly IKernelService kernelService;
        private readonly ISettingsService settingsService;
        private readonly ILogger<CustomChatHistoryReducer> logger;

        /// <summary>
        /// 注入聊天完成服务，并支持自定义摘要提示词
        /// </summary>
        public CustomChatHistoryReducer(ILogger<CustomChatHistoryReducer> logger, IKernelService kernelService, ISettingsService settingsService)
        {
            this.kernelService = kernelService;
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public async Task<IEnumerable<ChatMessageContent>?> ReduceAsync(IReadOnlyList<ChatMessageContent> chatHistory, CancellationToken cancellationToken = default)
        {
            // 安全检查，如果消息太少则无需化简
            //var count = EstimateTokenCount(chatHistory);
            var count = CalculateTokenCount(chatHistory);
            var maxToken = settingsService.Current.MaxTokens;
            var threshold = settingsService.Current.ContextCompressionThreshold;
            logger.LogInformation("正在判断是否需要压缩上下文，当前总Token占用: {count} / {max}", count, maxToken);
            if (chatHistory == null || count / maxToken < threshold * 0.01) return null;

            logger.LogInformation($"已超出上下文阈值，触发自动压缩");
            var reducedHistory = new List<ChatMessageContent>();
            var messagesToSummarize = new List<ChatMessageContent>();

            // 查找最开始的 SystemPrompt 和第一条 User 消息
            var systemMessage = chatHistory.FirstOrDefault(m => m.Role == AuthorRole.System);
            var firstUserMessage = chatHistory.FirstOrDefault(m => m.Role == AuthorRole.User);

            // 将它们加入化简后的历史记录中
            if (systemMessage != null) reducedHistory.Add(systemMessage);
            if (firstUserMessage != null) reducedHistory.Add(firstUserMessage);

            // 收集剩余需要被删除并进行摘要的消息
            foreach (var msg in chatHistory)
            {
                // 排除已经明确要保留的消息实例
                if (msg == systemMessage || msg == firstUserMessage) continue;
                messagesToSummarize.Add(msg);
            }

            // 如果没有多余的消息需要摘要，则返回 null 表示未发生化简操作
            if (messagesToSummarize.Count == 0) return null;

            // 对被删除的消息进行摘要处理
            var summaryChat = new ChatHistory(SummarizationPrompt);

            // 将消息格式化为便于 LLM 理解的文本块
            var conversationText = string.Join("\n", messagesToSummarize.Select(m => $"{m.Role}: {m.Content}"));
            summaryChat.AddUserMessage(conversationText);

            // 调用大模型生成摘要
            var kernel = kernelService.BuildKernel(null);
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var summaryResult = await chatService.GetChatMessageContentAsync(summaryChat, cancellationToken: cancellationToken);

            // 将摘要作为单条消息添加回历史记录
            // 推荐使用 System 角色，这样大模型在后续对话中会将其视为客观的上下文事实
            var summaryMessage = new ChatMessageContent(AuthorRole.System, $"[之前已删除对话的摘要]: {summaryResult.Content}");
            reducedHistory.Add(summaryMessage);

            // 返回全新的历史记录列表
            return reducedHistory;
        }

        /// <summary>
        /// 基于字符长度粗略估算Token
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <returns></returns>
        private int EstimateTokenCount(IReadOnlyList<ChatMessageContent> chatHistory)
        {
            int estimatedTokens = 0;
            foreach (var msg in chatHistory)
            {
                if (!string.IsNullOrEmpty(msg.Content))
                {
                    estimatedTokens += msg.Content.Length / 2;
                }
            }
            return estimatedTokens;
        }

        /// <summary>
        /// 通过ChatTokenUsage计算Token数
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <returns></returns>
        private int CalculateTokenCount(IReadOnlyList<ChatMessageContent> chatHistory)
        {
            int token = 0;
            foreach (var msg in chatHistory)
            {
                // 检查是否包含 "Usage" 键
                if (msg.Metadata != null && msg.Metadata.TryGetValue("Usage", out var usageObject) && usageObject != null)
                {
                    try
                    {
                        // 获取 Semantic Kernel / OpenAI 底层的 Usage 对象属性
                        if (usageObject is OpenAI.Chat.ChatTokenUsage usage)
                        {
                            if (usage.TotalTokenCount > 0)
                            {
                                token += usage.TotalTokenCount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("裁剪时提取 Token 消耗失败: {msg}", ex.Message);
                    }
                }
            }
            return token;
        }
    }
}
