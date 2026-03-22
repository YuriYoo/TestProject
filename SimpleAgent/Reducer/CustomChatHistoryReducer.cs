using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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
		private readonly IChatCompletionService _chatCompletionService;
		private readonly string _summarizationPrompt;
		private readonly int _maxTokenThreshold;

		/// <summary>
		/// 注入聊天完成服务，并支持自定义摘要提示词
		/// </summary>
		/// <param name="chatCompletionService"></param>
		/// <param name="summarizationPrompt"></param>
		public CustomChatHistoryReducer(IChatCompletionService chatCompletionService, int maxTokenThreshold = 200000,
			string summarizationPrompt = "请简要总结以下对话内容，提取核心信息与结论：\n")
		{
			_chatCompletionService = chatCompletionService;
			_summarizationPrompt = summarizationPrompt;
			_maxTokenThreshold = maxTokenThreshold;
		}

		public async Task<IEnumerable<ChatMessageContent>?> ReduceAsync(
			IReadOnlyList<ChatMessageContent> chatHistory,
			CancellationToken cancellationToken = default)
		{
			// 安全检查，如果消息太少则无需化简
			var count = EstimateTokenCount(chatHistory);
			Trace.WriteLine($"当前Token数: {count}");
			if (chatHistory == null || count < _maxTokenThreshold) return null;

			Trace.WriteLine($"[警告] 模型上下文过长触发自动压缩");
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
			var summaryChat = new ChatHistory(_summarizationPrompt);

			// 将消息格式化为便于 LLM 理解的文本块
			var conversationText = string.Join("\n", messagesToSummarize.Select(m => $"{m.Role}: {m.Content}"));
			summaryChat.AddUserMessage(conversationText);

			// 调用大模型生成摘要
			var summaryResult = await _chatCompletionService.GetChatMessageContentAsync(summaryChat, cancellationToken: cancellationToken);

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
	}
}
