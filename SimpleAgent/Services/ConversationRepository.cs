using Serilog;
using SimpleAgent.Models;
using SimpleAgent.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SimpleAgent.Services
{
	public class ConversationRepository
	{
		private readonly string _storageDirectory;
		private readonly AgentContextRepository contextRepository;
		private TreeNode? selectedNode = null;

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

		public ConversationRepository(AgentContextRepository contextRepository)
		{
			this.contextRepository = contextRepository;

			// 获取本地 AppData 目录
			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			_storageDirectory = Path.Combine(appDataPath, "SimpleAgent", "Conversations", "tree.json");
		}

		/// <summary>
		/// 创建会话节点
		/// </summary>
		/// <param name="treeView"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		public TreeNode? CreateConversationNode(TreeView treeView, Guid guid)
		{
			var node = treeView.SelectedNode?.Parent ?? treeView.SelectedNode;
			if (node == null) return null;
			var path = (node.Tag as ConversationTreeNode).Path;

			ConversationTreeNode subNode = new()
			{
				Name = $"新会话_{DateTime.Now:yyyyMMdd_HHmmss}",
				Path = path,
				IsProject = false,
				ConversationId = guid,
			};

			TreeNode subTreeNode = new(subNode.Name)
			{
				Tag = subNode,
				ToolTipText = path,
				Checked = true,
			};
			node.Nodes.Insert(0, subTreeNode);
			treeView.SelectedNode = subTreeNode;
			SwitchConversation(subTreeNode);
			SaveConversationTree(treeView);
			return subTreeNode;
		}

		/// <summary>
		/// 创建项目节点
		/// </summary>
		/// <param name="treeView"></param>
		/// <param name="guid"></param>
		/// <param name="path"></param>
		public TreeNode CreateProjectNode(TreeView treeView, Guid guid, string path)
		{
			string name = new DirectoryInfo(path).Name;

			// 创建项目节点
			ConversationTreeNode projectNode = new()
			{
				Name = name,
				Path = path,
				IsProject = true,
				ConversationId = Guid.Empty,
			};

			TreeNode projectTreeNode = new(name)
			{
				Tag = projectNode,
				ToolTipText = path,
			};

			treeView.Nodes.Insert(0, projectTreeNode);
			treeView.SelectedNode = projectTreeNode;
			projectTreeNode.Expand();
			CreateConversationNode(treeView, guid);
			return projectTreeNode;
		}

		public void DeleteNode(TreeNode node)
		{
			var view = node.TreeView;
			if (node.Parent == null)
			{
				foreach (TreeNode item in node.Nodes)
				{
					view.Nodes.Remove(item);
				}
			}
			view.Nodes.Remove(node);
			SaveConversationTree(view);
		}

		/// <summary>
		/// 切换会话
		/// </summary>
		/// <param name="node"></param>
		public bool SwitchConversation(TreeNode node)
		{
			if (node == null || node.Parent == null || node.IsSelected || selectedNode == node) return false;
			selectedNode = node;
			Trace.WriteLine($"切换到节点: {node.Text}");
			return true;
		}

		/// <summary>
		/// 保存会话树
		/// </summary>
		public void SaveConversationTree(TreeView treeView)
		{
			try
			{
				var treeData = new List<ConversationTreeNode>();
				foreach (TreeNode projectNode in treeView.Nodes)
				{
					var projectNodeData = projectNode.Tag as ConversationTreeNode;
					projectNodeData.Name = projectNode.Text;
					projectNodeData.Path = projectNode.ToolTipText;
					if (projectNode.Nodes.Count > 0) projectNodeData.Children = [];
					foreach (TreeNode conversationNode in projectNode.Nodes)
					{
						var subNode = conversationNode.Tag as ConversationTreeNode;
						subNode.Name = conversationNode.Text;
						subNode.Path = conversationNode.ToolTipText;
						projectNodeData.Children.Add(subNode);
					}
					treeData.Add(projectNodeData);
				}

				var json = JsonSerializer.Serialize(treeData, jsonOptions);
				File.WriteAllText(_storageDirectory, json);
			}
			catch (Exception ex)
			{
				Log.Error("保存对话树失败: {msg}", ex.Message);
			}
		}

		/// <summary>
		/// 加载会话树
		/// </summary>
		/// <param name="treeView"></param>
		public bool LoadConversationTree(TreeView treeView)
		{
			try
			{
				var json = File.ReadAllText(_storageDirectory);
				var treeData = JsonSerializer.Deserialize<List<ConversationTreeNode>>(json);

				if (treeData != null && treeData.Count > 0)
				{
					treeView.Nodes.Clear();
					foreach (var projectNodeData in treeData)
					{
						var projectNode = new TreeNode(projectNodeData.Name)
						{
							ToolTipText = projectNodeData.Path,
							Tag = projectNodeData
						};

						if (projectNodeData.Children != null)
						{
							foreach (var conversationNodeData in projectNodeData.Children)
							{
								var conversationNode = new TreeNode(conversationNodeData.Name)
								{
									ToolTipText = conversationNodeData.Path,
									Tag = conversationNodeData
								};
								projectNode.Nodes.Add(conversationNode);
							}
						}

						treeView.Nodes.Add(projectNode);
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Error("加载对话树失败: {msg}", ex.Message);
			}
			return false;
		}
	}
}
