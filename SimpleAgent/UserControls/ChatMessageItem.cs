using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAgent.UserControls
{
	public partial class ChatMessageItem : UserControl
	{
		MessageType type;

		public ChatMessageItem()
		{
			InitializeComponent();
		}

		public ChatMessageItem(MessageType type, string message = "")
		{
			this.type = type;
			InitializeComponent();
			UpdateRole(type);
			ChatMessage.ContentsResized += ChatMessage_ContentsResized;
			ChatMessage.AppendText(message);
			/*if (type == MessageType.AI)
			{
				AppendText("思考中...", false, Color.DarkOliveGreen);
			}*/
		}

		/// <summary>
		/// 更新角色
		/// </summary>
		private void UpdateRole(MessageType type)
		{
			switch (type)
			{
				case MessageType.System:
					ChatName.Text = "系统";
					ChatName.BackColor = Color.Lavender;
					ChatName.ForeColor = Color.BlueViolet;
					break;
				case MessageType.User:
					ChatName.Text = "用户";
					ChatName.BackColor = Color.AliceBlue;
					ChatName.ForeColor = Color.SteelBlue;
					break;
				case MessageType.AI:
					ChatName.Text = $"智能体";
					ChatName.BackColor = Color.Honeydew;
					ChatName.ForeColor = Color.DarkOliveGreen;
					break;
				default:
					ChatName.Text = $"未知";
					ChatName.BackColor = Color.LightGray;
					ChatName.ForeColor = Color.Black;
					break;
			}
		}

		/// <summary>
		/// 更新文本框高度
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatMessage_ContentsResized(object? sender, ContentsResizedEventArgs e)
		{
			Height = e.NewRectangle.Height + 27;
		}
	}
}
