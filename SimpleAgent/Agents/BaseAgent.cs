using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using SimpleAgent.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		protected int seed = Random.Shared.Next();

		//protected const int PlannerSeed = -1;
		//protected const int DeveloperSeed = -1;
		//protected const int ReviewerSeed = -1;
		//protected const int RouterSeed = -1;

		protected const int PlannerSeed = 1560201831;
		protected const int DeveloperSeed = 666262285;
		protected const int ReviewerSeed = 400363365;
		protected const int RouterSeed = 189011865;

		private readonly string systemPrompt;

		public BaseAgent(string systemPrompt)
		{
			chatHistory = [];
			this.systemPrompt = systemPrompt;
			chatHistory.AddSystemMessage(systemPrompt);
		}

		/// <summary>
		/// 重置对话历史（保留系统提示）
		/// </summary>
		public void Reset()
		{
			chatHistory.Clear();
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
		/// 添加系统消息
		/// </summary>
		/// <param name="message"></param>
		public void AddSystemMessage(string message)
		{
			chatHistory.AddSystemMessage(message);
		}

		/// <summary>
		/// 添加开发者消息
		/// </summary>
		/// <param name="message"></param>
		public void AddDeveloperMessage(string message)
		{
			chatHistory.AddDeveloperMessage(message);
		}

		/// <summary>
		/// 获取模型回复(异步流式输出)
		/// </summary>
		/// <returns></returns>
		public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync()
		{
			await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel))
			{
				yield return chunk;
			}
		}

		/// <summary>
		/// 获取模型回复(异步一次性输出)
		/// </summary>
		/// <returns></returns>
		public async Task<Microsoft.SemanticKernel.ChatMessageContent> GetChatMessageContentAsync()
		{
			return await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
		}
	}
}
