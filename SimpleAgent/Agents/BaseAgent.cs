using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Agents
{
	public abstract class BaseAgent
	{
		public ChatHistory chatHistory;
		public IChatCompletionService chatCompletionService;

		protected OpenAIPromptExecutionSettings settings;
		protected Kernel kernel;

		public BaseAgent(string systemPrompt)
		{
			chatHistory = [];
			chatHistory.AddSystemMessage(systemPrompt);
		}

		/// <summary>
		/// 添加用户消息
		/// </summary>
		/// <param name="message"></param>
		public void AddUserMessage(string message)
		{
			chatHistory.AddUserMessage(message);
		}

		/// <summary>
		/// 添加智能体消息
		/// </summary>
		/// <param name="message"></param>
		public void AddAssistantMessage(string message)
		{
			chatHistory.AddAssistantMessage(message);
		}

		/// <summary>
		/// 获取模型回复(异步流式输出)
		/// </summary>
		/// <returns></returns>
		public async IAsyncEnumerable<StreamingChatMessageContent> GetChatMessageContentAsync()
		{
			await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel))
			{
				yield return chunk;
			}
		}
	}
}
