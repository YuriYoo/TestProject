using Microsoft.Extensions.Logging;
using SimpleAgent.Factory;
using SimpleAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAgent.Services
{
	public class ConversationManager
	{
		private readonly ILogger<ConversationManager> logger;
		private readonly AgentContextRepository contextRepository;
		private readonly ConversationRepository treeRepository;
		private readonly IOrchestratorFactory orchestratorFactory;

		/// <summary>内存中维护的完整树形结构数据（单点事实来源）</summary>
		public List<ConversationTreeNode> TreeData { get; private set; } = [];

		/// <summary>当前运行的编排器</summary>
		public MultiAgentOrchestrator? CurrentOrchestrator { get; private set; }

		/// <summary>当前使用的上下文</summary>
		public AgentContext? CurrentContext => CurrentOrchestrator?.context;

		/// <summary>当会话切换完成时触发，UI 层监听此事件进行重绘</summary>
		public event Action<AgentContext>? OnConversationSwitched;

		/// <summary>首次数据加载时触发</summary>
		public event Action<List<ConversationTreeNode>>? OnLoaded;

		public ConversationManager(
			ILogger<ConversationManager> logger,
			AgentContextRepository contextRepository,
			ConversationRepository treeRepository,
			IOrchestratorFactory orchestratorFactory)
		{
			this.logger = logger;
			this.contextRepository = contextRepository;
			this.treeRepository = treeRepository;
			this.orchestratorFactory = orchestratorFactory;
		}

		/// <summary>
		/// 加载所有数据
		/// </summary>
		/// <returns></returns>
		public async Task<bool> Load()
		{
			TreeData = await treeRepository.LoadTreeAsync();
			if (TreeData.Count > 0)
			{
				var sub = TreeData[0].Children;
				if (sub.Count > 0)
				{
					var guid = sub[0].ConversationId;
					await SwitchConversationAsync(guid);
					OnLoaded?.Invoke(TreeData);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 保存所有数据
		/// </summary>
		/// <returns></returns>
		public async Task Save()
		{
			await treeRepository.SaveTreeAsync(TreeData);
			await SaveContextAsync();
		}

		/// <summary>
		/// 创建一个新的项目(同时会创建一个新的会话)
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public async Task<ConversationTreeNode> CreateProject(string name, string path)
		{
			ConversationTreeNode projectData = new()
			{
				Name = name,
				Path = path,
				IsProject = true,
				ConversationId = Guid.Empty,
				Children = []
			};
			await CreateConversation(projectData, path);
			TreeData.Insert(0, projectData);
			return projectData;
		}

		/// <summary>
		/// 创建新的会话
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public async Task<ConversationTreeNode> CreateConversation(ConversationTreeNode parNode, string path)
		{
			ConversationTreeNode conversationData = new()
			{
				Name = $"新会话_{DateTime.Now:yyyyMMdd_HHmmss}",
				Path = path,
				IsProject = false,
				ConversationId = Guid.NewGuid(),
			};
			parNode.Children.Insert(0, conversationData);
			await SwitchConversationAsync(conversationData.ConversationId);
			await Save();
			return conversationData;
		}

		/// <summary>
		/// 删除节点数据
		/// </summary>
		/// <param name="parNode"></param>
		/// <param name="subNode"></param>
		public async Task Delete(ConversationTreeNode parNode, ConversationTreeNode? subNode)
		{
			CurrentOrchestrator = null;
			if (subNode != null)
			{
				parNode.Children.Remove(subNode);
			}
			else
			{
				TreeData.Remove(parNode);
			}
			await Save();
		}

		/// <summary>
		/// 切换当前会话
		/// </summary>
		public async Task SwitchConversationAsync(Guid conversationId)
		{
			try
			{
				// 如果当前有正在进行的会话，先保存它
				await SaveContextAsync(conversationId);

				// 加载新的上下文
				var context = await LoadContextAsync(conversationId);

				// 触发事件
				OnConversationSwitched?.Invoke(context);

				logger.LogInformation("已切换到会话: {id}", conversationId);
			}
			catch (Exception ex)
			{
				logger.LogError("切换会话失败: {msg}", ex.Message);
			}
		}

		/// <summary>
		/// 保存当前上下文
		/// </summary>
		private async Task SaveContextAsync(Guid? conversationId = null)
		{
			if (CurrentContext != null && (conversationId == null || CurrentContext.ConversationId != conversationId))
			{
				await contextRepository.SaveContextAsync(CurrentContext);
			}
		}

		/// <summary>
		/// 加载上下文
		/// </summary>
		/// <returns></returns>
		private async Task<AgentContext> LoadContextAsync(Guid conversationId)
		{
			// 加载或创建新的会话上下文
			var context = await contextRepository.GetOrCreateContextAsync(conversationId);

			// 重新创建 Orchestrator
			CurrentOrchestrator = orchestratorFactory.CreateOrchestrator(context);

			return context;
		}
	}
}
