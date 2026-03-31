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
        private readonly ChatHistoryRepository historyRepository;
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
            ChatHistoryRepository historyRepository,
            IOrchestratorFactory orchestratorFactory)
        {
            this.logger = logger;
            this.contextRepository = contextRepository;
            this.treeRepository = treeRepository;
            this.historyRepository = historyRepository;
            this.orchestratorFactory = orchestratorFactory;
        }

        /// <summary>
        /// 加载所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Load()
        {
            await LoadTree();
            if (TreeData.Count > 0)
            {
                var sub = TreeData[0].Children;
                if (sub.Count > 0)
                {
                    await SwitchConversationAsync(sub[0]);
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
            await SaveTree();
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
            TreeData.Insert(0, projectData);
            await CreateConversation(projectData, path);
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
            await SwitchConversationAsync(conversationData);
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
                contextRepository.DeleteContext(subNode.ConversationId);
            }
            else
            {
                foreach (ConversationTreeNode child in parNode.Children)
                {
                    contextRepository.DeleteContext(child.ConversationId);
                }
                TreeData.Remove(parNode);
            }
            await Save();
        }

        /// <summary>
        /// 切换当前会话
        /// </summary>
        public async Task SwitchConversationAsync(ConversationTreeNode node)
        {
            try
            {
                // 如果当前有正在进行的会话，先保存它
                await SaveContextAsync(node.ConversationId);

                // 加载新的上下文
                var context = await LoadContextAsync(node);

                // 触发事件
                OnConversationSwitched?.Invoke(context);

                logger.LogInformation("已切换到会话: {id}", node.ConversationId);
            }
            catch (Exception ex)
            {
                logger.LogError("切换会话失败: {msg}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 保存会话树
        /// </summary>
        public async Task SaveTree()
        {
            await treeRepository.SaveTreeAsync(TreeData);
        }

        /// <summary>
        /// 加载会话树
        /// </summary>
        public async Task LoadTree()
        {
            TreeData = await treeRepository.LoadTreeAsync();
        }

        /// <summary>
        /// 保存当前上下文
        /// </summary>
        private async Task SaveContextAsync(Guid? conversationId = null)
        {
            if (CurrentContext != null && (conversationId == null || CurrentContext.ConversationId != conversationId))
            {
                await contextRepository.SaveContextAsync(CurrentContext);
                historyRepository.Save(CurrentContext.ConversationId);
            }
        }

        /// <summary>
        /// 加载上下文
        /// </summary>
        /// <returns></returns>
        private async Task<AgentContext> LoadContextAsync(ConversationTreeNode node)
        {
            // 加载或创建新的会话上下文
            var context = await contextRepository.GetOrCreateContextAsync(node.ConversationId);
            historyRepository.Load(node.ConversationId);
            context.WorkingDirectory = node.Path;

            // 重新创建 Orchestrator
            CurrentOrchestrator = orchestratorFactory.CreateOrchestrator(context);

            return context;
        }
    }
}
