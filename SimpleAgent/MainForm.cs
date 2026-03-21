using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;
using SimpleAgent.Services;
using SimpleAgent.UserControls;
using SimpleAgent.Utility;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent
{
	public partial class MainForm : Form
	{
		/// <summary>创建参数</summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				//cp.Style |= Win32API.WS_SYSMENU;
				cp.Style |= Win32API.WS_MINIMIZEBOX;
				cp.Style |= Win32API.WS_BORDER;
				cp.ClassStyle |= Win32API.CS_DROPSHADOW;
				//cp.ExStyle |= Win32API.WS_EX_COMPOSITED;
				return cp;
			}
		}

		private AppSettingsService settingsService;
		private ChatUIService chatUIService;
		private KernelService kernelService;
		private MultiAgentOrchestrator multiAgentOrchestrator;

		public MainForm()
		{
			InitializeComponent();
			InitializeAgentTab();

			settingsService = new AppSettingsService();
			settingsService.Load();
			settingsService.Save(settingsService.Settings);

			chatUIService = new(this);
			kernelService = new(settingsService.Settings);
			multiAgentOrchestrator = new(kernelService, chatUIService, this);

			// 设置自定义渲染器
			TopMenu.Renderer = new Renderer.MenuStripRenderer();

			// 允许按下 Enter 键直接发送
			UserInput.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					SendButton.PerformClick();
					// 消除回车提示音
					e.SuppressKeyPress = true;
				}
			};

			SendButton.Click += SendButton_Click;
		}

		bool isStart = false;
		private async void SendButton_Click(object? sender, EventArgs e)
		{
			string text = UserInput.Text.Trim();
			if (string.IsNullOrEmpty(text)) return;

			// 显示用户输入
			chatUIService.SendMessage(MessageType.User, AgentType.Planner, text);
			UserInput.Clear();

			// 禁用按钮，防止 AI 回复期间重复点击
			SendButton.Enabled = false;

			if (isStart)
			{
				multiAgentOrchestrator.ProvideUserInput(text);
			}
			else
			{
				isStart = true;
				await multiAgentOrchestrator.RunWorkflowAsync(text);
			}

			RecoveryState();
		}

		public void RecoveryState()
		{
			// 恢复 UI 状态
			SendButton.Text = "发送";
			SendButton.Enabled = true;

			// 焦点还给输入框，方便连续对话
			UserInput.Focus();
		}

		/// <summary>
		/// 辅助方法：向聊天框追加带有颜色的文本
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		private void AppendText(string text, Color color)
		{
			ChatHistory.SelectionStart = ChatHistory.TextLength;
			ChatHistory.SelectionLength = 0;
			ChatHistory.SelectionColor = color;
			ChatHistory.AppendText(text);
			ChatHistory.SelectionColor = ChatHistory.ForeColor;

			// 自动滚动到最新消息
			ChatHistory.ScrollToCaret();
		}

		#region Windows基础功能

		/// <summary>
		/// 最大化应用程序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TopMenu_Maximize_Click(object sender, EventArgs e)
		{
			MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
			if (WindowState != FormWindowState.Maximized)
			{
				WindowState = FormWindowState.Maximized;
			}
			else
			{
				WindowState = FormWindowState.Normal;
			}
		}

		/// <summary>
		/// 窗口状态改变时更新最大化按钮的图标
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_SizeChanged(object sender, EventArgs e)
		{
			TopMenu_Maximize.Text = WindowState == FormWindowState.Maximized ? "⧉" : "☐";
		}

		/// <summary>
		/// 最小化应用程序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TopMenu_Minimize_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		/// <summary>
		/// 关闭应用程序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TopMenu_Close_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// 窗口移动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TopMenu_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Cursor.Current = Cursors.SizeAll;
				Win32API.ReleaseCapture();
				Win32API.SendMessage(Handle, 0xA1, 0x2, 0);
			}
		}

		/// <summary>
		/// Windows消息处理: 添加调整窗口大小的功能
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				// 更改鼠标所处区域的判断以执行默认的修改窗口大小逻辑
				case Win32API.WM_NCHITTEST:
					// 最大化时禁用调整窗口大小
					if (WindowState == FormWindowState.Maximized) return;

					// 从消息参数中获取鼠标在屏幕上的坐标
					Point pos = new(m.LParam.ToInt32());

					// 将屏幕坐标转换为窗口客户区的相对坐标（相对于窗口左上角）
					pos = PointToClient(pos);

					int edge = IsResizeMode(pos, ClientSize);
					if (edge != 0)
					{
						m.Result = edge;
						return;
					}
					break;
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// 判断调整窗口大小的模式
		/// </summary>
		/// <param name="point"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		private int IsResizeMode(Point point, Size size)
		{
			// 边缘方向, 默认0, 非边缘
			int edge = 0;

			// 边缘可拖动区域的大小（像素）
			int borderSize = 6;

			// 判断当前鼠标所在的边缘方向
			if (point.X <= borderSize)
			{
				if (point.Y <= borderSize) edge = Win32API.HTTOPLEFT;
				else if (point.Y >= ClientSize.Height - borderSize) edge = Win32API.HTBOTTOMLEFT;
				else edge = Win32API.HTLEFT;
			}
			else if (point.X >= ClientSize.Width - borderSize)
			{
				if (point.Y <= borderSize) edge = Win32API.HTTOPRIGHT;
				else if (point.Y >= ClientSize.Height - borderSize) edge = Win32API.HTBOTTOMRIGHT;
				else edge = Win32API.HTRIGHT;
			}
			else if (point.Y <= borderSize)
			{
				edge = Win32API.HTTOP;
			}
			else if (point.Y >= ClientSize.Height - borderSize)
			{
				edge = Win32API.HTBOTTOM;
			}

			return edge;
		}

		#endregion

		/// <summary>
		/// 聊天区域宽度变更时更新自控件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatPanel_SizeChanged(object sender, EventArgs e)
		{
			// 暂停布局逻辑，提高性能并减少闪烁
			PlannerChatPanel.SuspendLayout();

			foreach (Control ctrl in PlannerChatPanel.Controls)
			{
				// 关键计算：Panel的内部宽度 - 控件的左右边距 - 滚动条预留宽度
				// 如果没有滚动条，可以不减去 SystemInformation.VerticalScrollBarWidth
				//ctrl.Width = ChatPanel.ClientSize.Width - ctrl.Margin.Horizontal;
				ctrl.Width = PlannerChatPanel.ClientSize.Width;
				PlannerChatPanel.HorizontalScroll.Enabled = false;
				PlannerChatPanel.HorizontalScroll.Visible = false;
			}

			PlannerChatPanel.ResumeLayout();
		}

		/// <summary>
		/// 初始化智能体组件
		/// </summary>
		private void InitializeAgentTab()
		{
			PlannerAgentTab.Click += (sender, e) =>
			{
				PlannerAgentTab.IsSelected = true;
				CoderAgentTab.IsSelected = false;
				ReviewerAgentTab.IsSelected = false;

				PlannerChatPanel.Visible = true;
				CoderChatPanel.Visible = false;
				ReviewerChatPanel.Visible = false;

			};
			CoderAgentTab.Click += (sender, e) =>
			{
				PlannerAgentTab.IsSelected = false;
				CoderAgentTab.IsSelected = true;
				ReviewerAgentTab.IsSelected = false;

				PlannerChatPanel.Visible = false;
				CoderChatPanel.Visible = true;
				ReviewerChatPanel.Visible = false;
			};
			ReviewerAgentTab.Click += (sender, e) =>
			{
				PlannerAgentTab.IsSelected = false;
				CoderAgentTab.IsSelected = false;
				ReviewerAgentTab.IsSelected = true;

				PlannerChatPanel.Visible = false;
				CoderChatPanel.Visible = false;
				ReviewerChatPanel.Visible = true;
			};
		}
	}
}
