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

		public ChatMessageItem(MessageType type)
		{
			this.type = type;
			InitializeComponent();
			UpdateRole(type);
			if (type == MessageType.AI)
			{
				AppendText("思考中...", false, Color.DarkOliveGreen);
			}
			ChatMessage.ContentsResized += ChatMessage_ContentsResized;
		}

		public void AppendText(string text, bool addNewLine = false, Color? color = null)
		{
			if (type == MessageType.AI && ChatMessage.Text == "思考中...")
			{
				ClearText();
			}

			if (addNewLine && !IsTextEndingWithNewLine())
			{
				ChatMessage.AppendText(Environment.NewLine);
			}

			if (color.HasValue)
			{
				// 记住当前的默认颜色
				Color defaultColor = ChatMessage.ForeColor;

				// 将光标定位到文本末尾
				ChatMessage.Select(ChatMessage.TextLength, 0);

				// 设置将要插入的文本的颜色
				ChatMessage.SelectionColor = color.Value;

				ChatMessage.AppendText(text);

				// 将光标处的新输入颜色恢复为默认颜色，防止影响后续追加或用户键盘输入的文本
				ChatMessage.Select(ChatMessage.TextLength, 0);
				ChatMessage.SelectionColor = defaultColor;
			}
			else
			{
				ChatMessage.AppendText(text);
			}

			if (addNewLine)
			{
				ChatMessage.AppendText(Environment.NewLine);
			}
		}

		public void ClearText()
		{
			ChatMessage.Clear();
		}

		/// <summary>
		/// 判断最后一个字符是否是换行符
		/// </summary>
		/// <returns></returns>
		private bool IsTextEndingWithNewLine()
		{
			if (ChatMessage.TextLength == 0) return true;

			// 检查最后一个字符是不是换行符 \n 或 \r
			char lastChar = ChatMessage.Text[ChatMessage.TextLength - 1];
			return lastChar == '\n' || lastChar == '\r';
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
