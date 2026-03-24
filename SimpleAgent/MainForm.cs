using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using SimpleAgent.Agents;
using SimpleAgent.Services;
using SimpleAgent.UserControls;
using SimpleAgent.Utility;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

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

        private readonly ILogger<MainForm> logger;
        private readonly ISettingsService settings;
        private readonly IBackgroundService backgroundService;
        private readonly IKernelService kernelService;
        private readonly GPUStackClient stackClient;
        private readonly ChatUIService chatUIService;

        private readonly RouterAgent routerAgent;
        private MultiAgentOrchestrator multiAgentOrchestrator;

        public MainForm(ILogger<MainForm> logger, ISettingsService settings, IKernelService kernelService, IBackgroundService backgroundService, MultiAgentOrchestrator multiAgentOrchestrator, GPUStackClient stackClient, ChatUIService chatUIService)
        {
            this.logger = logger;
            this.settings = settings;
            this.kernelService = kernelService;

            this.multiAgentOrchestrator = multiAgentOrchestrator;
            multiAgentOrchestrator.Initialization(settings.Current.WorkingDirectory);
            multiAgentOrchestrator.OnTokenUsageUpdated += UpdateTokens;
            multiAgentOrchestrator.OnResetUserInputState += RecoveryState;

            InitializeComponent();

            this.stackClient = stackClient;

            this.chatUIService = chatUIService;
            chatUIService.Initialization(this);

            this.backgroundService = backgroundService;
            backgroundService.OnAddServer += (serviceId) => BackgroundServerListBox.Items.Add(serviceId);
            backgroundService.OnRemoveServer += BackgroundServerListBox.Items.Remove;

            // 创建路由智能体
            routerAgent = new(kernelService, settings.Current.WorkingDirectory);

            // 初始化
            InitializeAgentTab();

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
            chatUIService.SendUserMessage(AgentType.Planner, text);
            UserInput.Clear();
            UserInput.Focus();

            // 禁用按钮，防止 AI 回复期间重复点击
            SendButton.Enabled = false;

            // 判断是否需要进行规划
            var plan = await routerAgent.HandleRoutingAsync(text);
            if (plan)
            {
                if (isStart)
                {
                    multiAgentOrchestrator.ProvideUserInput(text);
                }
                else
                {
                    isStart = true;
                    await multiAgentOrchestrator.RunWorkflowAsync(text, WorkflowState.Planning);
                }
            }
            else
            {
                await multiAgentOrchestrator.RunWorkflowAsync(text, WorkflowState.Developing);
            }

            RecoveryState();
            isStart = false;
            Trace.WriteLine("/////////// 已恢复初始状态 ///////////");
        }

        public void RecoveryState()
        {
            // 恢复 UI 状态
            SendButton.Text = "发送";
            SendButton.Enabled = true;

            // 焦点还给输入框，方便连续对话
            UserInput.Focus();
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

        #region 聊天区域相关

        /// <summary>
        /// 聊天区域宽度变更时更新自控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlannerChatPanel_SizeChanged(object sender, EventArgs e)
        {
            if (PlannerChatPanel.Visible)
            {
                UpdateChatPanelWidth(PlannerChatPanel);
            }
        }

        private void CoderChatPanel_SizeChanged(object sender, EventArgs e)
        {
            if (CoderChatPanel.Visible)
            {
                UpdateChatPanelWidth(CoderChatPanel);
            }
        }

        private void ReviewerChatPanel_SizeChanged(object sender, EventArgs e)
        {
            if (ReviewerChatPanel.Visible)
            {
                UpdateChatPanelWidth(ReviewerChatPanel);
            }
        }

        private void UpdateChatPanelWidth(FlowLayoutPanel chatPanel)
        {
            // 暂停布局逻辑，提高性能并减少闪烁
            chatPanel.SuspendLayout();

            foreach (Control ctrl in chatPanel.Controls)
            {
                // 关键计算：Panel的内部宽度 - 控件的左右边距 - 滚动条预留宽度
                // 如果没有滚动条，可以不减去 SystemInformation.VerticalScrollBarWidth
                //ctrl.Width = ChatPanel.ClientSize.Width - ctrl.Margin.Horizontal;
                ctrl.Width = chatPanel.ClientSize.Width;
                chatPanel.HorizontalScroll.Enabled = false;
                chatPanel.HorizontalScroll.Visible = false;
            }

            chatPanel.ResumeLayout();
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
                UpdateChatPanelWidth(PlannerChatPanel);

            };
            CoderAgentTab.Click += (sender, e) =>
            {
                PlannerAgentTab.IsSelected = false;
                CoderAgentTab.IsSelected = true;
                ReviewerAgentTab.IsSelected = false;

                PlannerChatPanel.Visible = false;
                CoderChatPanel.Visible = true;
                ReviewerChatPanel.Visible = false;
                UpdateChatPanelWidth(CoderChatPanel);
            };
            ReviewerAgentTab.Click += (sender, e) =>
            {
                PlannerAgentTab.IsSelected = false;
                CoderAgentTab.IsSelected = false;
                ReviewerAgentTab.IsSelected = true;

                PlannerChatPanel.Visible = false;
                CoderChatPanel.Visible = false;
                ReviewerChatPanel.Visible = true;
                UpdateChatPanelWidth(ReviewerChatPanel);
            };
        }

        /// <summary>
        /// 更新界面上的 Token 消耗统计
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name=""></param>
        private void UpdateTokens(AgentType agentType, ChatTokenUsage? usage)
        {
            if (usage == null) return;
            switch (agentType)
            {
                case AgentType.Planner:
                    // 只有当 usage.TotalTokens > 0 时，说明这是包含了使用统计的最终 Chunk
                    if (usage.TotalTokenCount > 0)
                    {
                        PAllTokens.Text = $"{int.Parse(PAllTokens.Text) + usage.TotalTokenCount}";
                        PInTokens.Text = $"{int.Parse(PInTokens.Text) + usage.InputTokenCount}";
                        POutTokens.Text = $"{int.Parse(POutTokens.Text) + usage.OutputTokenCount}";
                    }
                    break;
                case AgentType.Developer:
                    if (usage.TotalTokenCount > 0)
                    {
                        DAllTokens.Text = $"{int.Parse(DAllTokens.Text) + usage.TotalTokenCount}";
                        DInTokens.Text = $"{int.Parse(DInTokens.Text) + usage.InputTokenCount}";
                        DOutTokens.Text = $"{int.Parse(DOutTokens.Text) + usage.OutputTokenCount}";
                    }
                    break;
                case AgentType.Reviewer:
                    if (usage.TotalTokenCount > 0)
                    {
                        RAllTokens.Text = $"{int.Parse(RAllTokens.Text) + usage.TotalTokenCount}";
                        RInTokens.Text = $"{int.Parse(RInTokens.Text) + usage.InputTokenCount}";
                        ROutTokens.Text = $"{int.Parse(ROutTokens.Text) + usage.OutputTokenCount}";
                    }
                    break;
            }
        }

        #endregion

        #region GPUStack状态

        private async void MainForm_Load(object sender, EventArgs e)
        {
            if (await stackClient.CheckGPUStackIsStartAsync())
            {
                GPUStackStatusLabel.ForeColor = Color.SeaGreen;
                GPUStackStatusLabel.Text = "[ 服务在线 ]";
            }
            else
            {
                GPUStackStatusLabel.ForeColor = Color.Firebrick;
                GPUStackStatusLabel.Text = "[ 服务离线 ]";
            }

            if (await stackClient.CheckModelIsOnlineAsync())
            {
                ModelStatusLabel.ForeColor = Color.SeaGreen;
                ModelStatusLabel.Text = "[ 模型在线 ]";
            }
            else
            {
                ModelStatusLabel.ForeColor = Color.Firebrick;
                ModelStatusLabel.Text = "[ 模型离线 ]";
            }

            var status = await stackClient.GetGlobalGpuStatusAsync();
            if (status != null)
            {
                MemoryLabel.Text = $"显存占用率：{status.GlobalMemoryUtilizationPercent:F2}%  {status.UsedMemoryGB:F0} / {status.TotalMemoryGB:F0}GB";
                CoreLabel.Text = $"算力负载：{status.AverageCoreUtilizationPercent:F2}%";
            }
        }

        private void RefreshLabel_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("刷新状态");
            MainForm_Load(sender, e);
        }

        #endregion

        #region 后台服务相关

        private void BackgroundServerMenuItem_Stop_MouseDown(object sender, MouseEventArgs e)
        {
            Trace.WriteLine($"{BackgroundServerListBox.SelectedItem}");
            if (BackgroundServerListBox.SelectedItem != null)
            {
                string serviceId = BackgroundServerListBox.SelectedItem.ToString()!;
                backgroundService.StopService(serviceId);
            }
        }

        private void BackgroundServerMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (BackgroundServerListBox.SelectedItem == null)
            {
                e.Cancel = true; // 取消显示上下文菜单
            }
        }

        #endregion

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            chatUIService.ShowQuestion("测试问题", QuestionMode.NoSelect,
            [
                "选项一",
                "选项二",
                "选项三",
                "选项四",
                "选项五",
                "选项六",
            ], (ind, opt) =>
            {
                Trace.WriteLine(string.Join(',', ind));
                Trace.WriteLine(string.Join(',', opt));
            });
        }
    }
}
