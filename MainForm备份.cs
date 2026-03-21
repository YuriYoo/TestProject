using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

		// Semantic Kernel 核心组件
		private Kernel _kernel;
		private IChatCompletionService _chatCompletion;
		private ChatHistory _chatHistory;

		private ChatCompletionAgent agent;

		public MainForm()
		{
			InitializeComponent();
			InitializeSemanticKernel();

			// 设置自定义渲染器
			TopMenu.Renderer = new Renderer.MenuStripRenderer();

			ChatPanel.HorizontalScroll.Enabled = false;
			ChatPanel.HorizontalScroll.Visible = false;

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

		private void InitializeSemanticKernel()
		{
			try
			{
				// 创建 Kernel Builder 并配置大模型
				// 如果使用的是国内代理接口或第三方中转，可以使用 HttpClient 进行更高级的配置
				var builder = Kernel.CreateBuilder();
				builder.AddOpenAIChatCompletion(
					modelId: "glm-4.7-flash",
					endpoint: new Uri("http://192.168.9.110/v1"),
					//modelId: "qwen3.5:9b",
					//endpoint: new Uri("http://111.163.74.194:2434/v1"),
					apiKey: "gpustack_ffb094a6a6b5de1d_b4d98b09a0ff053c1b89ea8f020c1750"
				);

				// 将插件注册到 Kernel 中
				builder.Plugins.AddFromType<FileSystemPlugin>("FileSystem");
				builder.Plugins.AddFromType<TerminalPlugin>("Terminal");

				_kernel = builder.Build();

				// 开启自动工具调用行为，这会让大模型全自动接管流程
				var executionSettings = new OpenAIPromptExecutionSettings
				{
					FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
					Temperature = 0.2 // 编程任务建议调低温度以保证逻辑严谨
				};

				// 获取对话补全服务
				//_chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

				// 初始化对话历史，并设置系统提示词 (System Prompt) 赋予它编程助手的角色
				//_chatHistory = new ChatHistory("你是一个强大的纯粹 AI 编程助手。你需要根据用户的指令提供代码、解答问题。你的回复应该简洁、专业，重点放在代码和技术实现上。");

				_chatHistory = new();

				// 创建 ChatCompletionAgent
				agent = new ChatCompletionAgent
				{
					Name = "AutoCodingAssistant",
					// 通过 System Instructions 赋予它自主规划的意识
					Instructions = @"你是一个全自动的软件工程师。
1. 在开始行动前，请在心里制定分步执行计划。
2. 使用你拥有的工具一步一步执行。每次执行工具后，仔细分析返回的结果。
3. 如果遇到错误（例如编译失败），请自主分析原因，修改代码并重新尝试。
4. 不要每做一步都问我，请自主完成整个任务。只有当所有步骤都成功验证后，才向我输出最终的成功总结。",
					Kernel = _kernel,
					Arguments = new KernelArguments(executionSettings)
				};

				AppendText("系统: Semantic Kernel 初始化成功！你可以开始提问了。\n\n", Color.Green);
			}
			catch (Exception ex)
			{
				AppendText($"系统: SK 初始化失败，请检查 API Key 或网络连通性。\n错误信息: {ex.Message}\n\n", Color.Red);
			}
		}

		private ChatMessageItem SendMessage(string message, MessageType messageType)
		{
			// 创建新的消息控件
			ChatMessageItem msgItem = new(message, messageType)
			{
				Width = ChatPanel.ClientSize.Width
			};

			// 添加到滚动面板
			ChatPanel.Controls.Add(msgItem);

			// 自动滚动到最底部
			ChatPanel.ScrollControlIntoView(msgItem);

			return msgItem;
		}

		/// <summary>
		/// 核心交互逻辑
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void SendButton_Click(object? sender, EventArgs e)
		{
			string text = UserInput.Text.Trim();
			//if (string.IsNullOrEmpty(text) || _chatCompletion == null) return;
			if (string.IsNullOrEmpty(text) || agent == null) return;

			// 显示用户输入
			SendMessage(text, MessageType.User);
			_chatHistory.AddUserMessage(text);
			UserInput.Clear();

			// 禁用按钮，防止 AI 回复期间重复点击
			SendButton.Enabled = false;
			try
			{
				//var message = await GetChatMessageContentAsync();
				//var message = await GetStreamingChatMessageContentsAsync();

				// 将 AI 的回复加入对话历史，确保它有上下文记忆
				//_chatHistory.AddAssistantMessage(message);

				Trace.WriteLine($"历史数:{_chatHistory.Count}");
				AgentMessage();
				Trace.WriteLine($"历史数:{_chatHistory.Count}");
			}
			catch (Exception ex)
			{
				SendMessage($"[调用失败]: {ex.Message}", MessageType.System);
			}
			finally
			{
				// 恢复 UI 状态
				SendButton.Text = "发送";
				SendButton.Enabled = true;

				// 焦点还给输入框，方便连续对话
				UserInput.Focus();
			}
		}

		private async void AgentMessage()
		{
			Trace.WriteLine("🤖 AI 正在自主规划和执行任务中，请稍候...");

			// Agent 会在后台自动进行多次 Function Calling 的往返，直到任务完成才返回结果
			await foreach (var message in agent.InvokeAsync(_chatHistory))
			{
				// 打印最终由 Agent 总结的回复
				SendMessage($"[{message.Message.Role}]: {message.Message.Content}", MessageType.AI);
			}
		}

		private async Task<string> GetStreamingChatMessageContentsAsync()
		{
			// 初始化一个 StringBuilder 用来收集完整的回复内容
			StringBuilder fullContent = new();

			// 配置执行设置，允许大模型自动调用本地函数
			OpenAIPromptExecutionSettings settings = new()
			{
				// 告诉 AI 自动执行 Kernel 中注册的函数
				ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
			};

			// 调用流式接口
			var streamingResponse = _chatCompletion.GetStreamingChatMessageContentsAsync(
				chatHistory: _chatHistory,
				executionSettings: settings,
				kernel: _kernel
			);

			bool reply = false;
			var item = SendMessage("思考中...", MessageType.AI);

			// 使用 await foreach 异步迭代流
			await foreach (var chunk in streamingResponse)
			{
				// 这里的 chunk.Content 包含了当前那一小段文字
				if (chunk.Content != null)
				{
					if (!reply)
					{
						reply = true;
						item.ClearText();
					}

					item.AppendText(chunk.Content);
					fullContent.Append(chunk.Content);
				}
			}

			// 当流结束时，将完整的回复返回
			return fullContent.ToString();
		}

		private async Task<string> GetChatMessageContentAsync()
		{
			// 配置执行设置，允许大模型自动调用本地函数
			OpenAIPromptExecutionSettings settings = new()
			{
				// 告诉 AI 自动执行 Kernel 中注册的函数
				ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
			};

			// 异步调用大模型 (UI 不会卡死)
			var response = await _chatCompletion.GetChatMessageContentAsync(
				chatHistory: _chatHistory,
				executionSettings: settings,
				kernel: _kernel
			);

			return response.Content;
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
			ChatPanel.SuspendLayout();

			foreach (Control ctrl in ChatPanel.Controls)
			{
				// 关键计算：Panel的内部宽度 - 控件的左右边距 - 滚动条预留宽度
				// 如果没有滚动条，可以不减去 SystemInformation.VerticalScrollBarWidth
				//ctrl.Width = ChatPanel.ClientSize.Width - ctrl.Margin.Horizontal;
				ctrl.Width = ChatPanel.ClientSize.Width;
				ChatPanel.HorizontalScroll.Enabled = false;
				ChatPanel.HorizontalScroll.Visible = false;
			}

			ChatPanel.ResumeLayout();
		}
	}
}
