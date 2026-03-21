using SimpleAgent.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
	public class ChatUIService
	{
		/// <summary>智能体标签页Map</summary>
		private Dictionary<AgentType, AgentTab> agentTabs;

		/// <summary>智能体聊天面板Map</summary>
		private Dictionary<AgentType, FlowLayoutPanel> chatPanels;

		/// <summary>智能体是否正在输出Map (null时表示需要创建新消息, 否则表示已有需要追加聊天内容)</summary>
		private Dictionary<AgentType, ChatMessageItem?> inChat;

		public ChatUIService(MainForm mainForm)
		{
			agentTabs = new()
			{
				{ AgentType.Planner, mainForm.PlannerAgentTab },
				{ AgentType.Developer, mainForm.CoderAgentTab },
				{ AgentType.Reviewer, mainForm.ReviewerAgentTab },
			};

			chatPanels = new()
			{
				{ AgentType.Planner, mainForm.PlannerChatPanel },
				{ AgentType.Developer, mainForm.CoderChatPanel },
				{ AgentType.Reviewer, mainForm.ReviewerChatPanel },
			};

			inChat = new()
			{
				{ AgentType.Planner, null },
				{ AgentType.Developer, null },
				{ AgentType.Reviewer, null },
			};
		}

		public void SendMessage(MessageType messageType, AgentType agentType, string message)
		{
			if (agentType == AgentType.Router)
			{
				Trace.WriteLine("[警告] 路由智能体消息不需要显示");
				return;
			}

			switch (messageType)
			{
				case MessageType.System:
					// 如果是系统消息直接发送, 并且强制结束AI的消息流
					if (string.IsNullOrWhiteSpace(message))
					{
						CreateMessageItem(messageType, chatPanels[agentType], message);
					}
					EndChat(agentType);
					break;
				case MessageType.User:
					// 如果是用户消息直接发送, 并且强制结束AI的消息流
					CreateMessageItem(messageType, chatPanels[agentType], message);
					EndChat(agentType);
					break;
				case MessageType.AI:
					// 如果之前没有进行中的消息则创建新消息
					if (inChat[agentType] == null)
					{
						var item = CreateMessageItem(messageType, chatPanels[agentType], message);
						inChat[agentType] = item;
						agentTabs[agentType].SetRunning(true);
						agentTabs[agentType].PerformClick();
					}
					// 如果已有进行中的消息则追加内容
					else
					{
						AppendMessage(chatPanels[agentType], inChat[agentType], message);
					}
					break;
			}
		}

		private void EndChat(AgentType agentType)
		{
			inChat[agentType] = null;
			agentTabs[agentType].SetRunning(false);
		}

		/// <summary>
		/// 创建一条消息记录
		/// </summary>
		/// <param name="messageType">消息类型</param>
		/// <param name="panel">发送到哪个消息面板</param>
		/// <param name="message">消息内容</param>
		/// <returns></returns>
		private ChatMessageItem CreateMessageItem(MessageType messageType, FlowLayoutPanel panel, string message)
		{
			var item = new ChatMessageItem(messageType, message)
			{
				Width = panel.ClientSize.Width
			};

			panel.Controls.Add(item);
			panel.SuspendLayout();
			panel.VerticalScroll.Value = panel.VerticalScroll.Maximum;
			panel.ResumeLayout();

			return item;
		}

		/// <summary>
		/// 向现有聊天框追加消息
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="item"></param>
		/// <param name="message"></param>
		private void AppendMessage(FlowLayoutPanel panel, ChatMessageItem item, string message)
		{
			item.AppendText(message);
			panel.SuspendLayout();
			panel.VerticalScroll.Value = panel.VerticalScroll.Maximum;
			panel.ResumeLayout();
		}
	}
}
