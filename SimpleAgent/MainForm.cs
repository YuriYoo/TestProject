using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using SimpleAgent.Agents;
using SimpleAgent.Factory;
using SimpleAgent.Models;
using SimpleAgent.Services;
using SimpleAgent.UserControls;
using SimpleAgent.Utility;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;

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
        private readonly AgentContextRepository contextRepository;
        private readonly ConversationRepository conversationRepository;
        private readonly ConversationManager conversationManager;
        private readonly GPUStackClient stackClient;
        private readonly ChatUIService chatUIService;

        private readonly IOrchestratorFactory orchestratorFactory;

        private MultiAgentOrchestrator multiAgentOrchestrator;
        private AgentContext currentContext;

        /// <summary>当前会话节点</summary>
        private TreeNode? selectedNode = null;

        public MainForm(ILogger<MainForm> logger,
            IOrchestratorFactory orchestratorFactory,
            AgentContextRepository contextRepository,
            ConversationRepository conversationRepository,
            ChatHistoryRepository chatHistoryRepository,
            ConversationManager conversationManager,
            ISettingsService settings,
            IBackgroundService backgroundService,
            IStreamingExecutionEngine streamingExecutionEngine,
            GPUStackClient stackClient,
            ChatUIService chatUIService)
        {
            InitializeComponent();
            chatHistoryRepository.PlannerPanel = PlannerChatPanel;
            chatHistoryRepository.DeveloperPanel = CoderChatPanel;
            chatHistoryRepository.ReviewerPanel = ReviewerChatPanel;

            this.logger = logger;
            this.settings = settings;
            this.orchestratorFactory = orchestratorFactory;
            this.contextRepository = contextRepository;
            this.conversationRepository = conversationRepository;
            this.conversationManager = conversationManager;
            conversationManager.OnConversationSwitched += OnSwitchConversation;
            conversationManager.OnLoaded += OnConversationLoaded;

            this.stackClient = stackClient;

            this.chatUIService = chatUIService;
            chatUIService.Initialization(this);

            this.backgroundService = backgroundService;
            backgroundService.OnAddServer += (serviceId) => BackgroundServerListBox.Items.Add(serviceId);
            backgroundService.OnRemoveServer += BackgroundServerListBox.Items.Remove;

            streamingExecutionEngine.OnTokenUsage += UpdateTokens;

            // 初始化
            InitializeAgentTab();

            // 加载会话树
            // 会话存储TODO: 初始化加载
            var isLoad = conversationManager.Load().Result;
            if (isLoad)
            {
                ActivateConversation();
                selectedNode = ConversationTreeView.SelectedNode;
            }

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

        private void StopButton_Click(object sender, EventArgs e)
        {
            multiAgentOrchestrator.StopWorkflow();
        }

        private async void SendButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ReviewerAgentTab.IsSelected)
                {
                    _ = await chatUIService.ShowQuestion("仅可以与 [规划智能体] 或 [编程智能体] 进行对话，请根据需要进行切换。", QuestionMode.NoSelect, null);
                    return;
                }

                string text = UserInput.Text.Trim();
                if (string.IsNullOrEmpty(text)) return;

                UserInput.Clear();
                ActivationStopButton();

                if (isStart)
                {
                    multiAgentOrchestrator.ProvideUserInput(text);
                    chatUIService.SendUserMessage(AgentType.Planner, text);
                    return;
                }

                // 显示用户输入
                if (PlannerAgentTab.IsSelected)
                {
                    chatUIService.SendUserMessage(AgentType.Planner, text);
                }
                else
                {
                    chatUIService.SendUserMessage(AgentType.Developer, text);
                }

                multiAgentOrchestrator.context.OriginalRequest = text;

                // 判断是否需要进行规划
                var state = await multiAgentOrchestrator.RunRoutingAsync();

                // 需要进行规划
                if (state == WorkflowState.Planning)
                {
                    // 当前在规划页面
                    if (PlannerAgentTab.IsSelected)
                    {
                        RunPlanning(text);
                    }
                    // 当前不在规划页面
                    else
                    {
                        var (confirm, indices, options) = await chatUIService.ShowQuestion("对于该需求，模型建议先进行规划再开发，是否确认转交给[规划智能体]？", QuestionMode.NoSelect, null);
                        // 确认转交给 Planner
                        if (confirm)
                        {
                            CoderChatPanel.Controls.RemoveAt(CoderChatPanel.Controls.Count - 1);
                            chatUIService.SendUserMessage(AgentType.Planner, text);
                            RunPlanning(text);
                        }
                        // 不转交, 继续使用Coder
                        else
                        {
                            RunDeveloping(text);
                        }
                    }
                }

                // 直接进行开发
                else if (state == WorkflowState.Developing)
                {
                    // 当前在开发页面
                    if (CoderAgentTab.IsSelected)
                    {
                        RunDeveloping(text);
                    }
                    // 当前不在开发页面
                    else
                    {
                        var (confirm, indices, options) = await chatUIService.ShowQuestion("对于该需求，模型建议可以直接开发，是否确认转交给[编程智能体]？", QuestionMode.NoSelect, null);
                        // 确认转交给 Coder
                        if (confirm)
                        {
                            PlannerChatPanel.Controls.RemoveAt(PlannerChatPanel.Controls.Count - 1);
                            chatUIService.SendUserMessage(AgentType.Developer, text);
                            RunDeveloping(text);
                        }
                        // 不转交, 继续使用Planner
                        else
                        {
                            RunPlanning(text);
                        }
                    }
                }
                else
                {
                    logger.LogError("理论上不可能走到这里");
                }
            }
            catch (OperationCanceledException)
            {
                ActivationSendButton();
                logger.LogInformation("用户强制结束");
            }
            catch (Exception ex)
            {
                logger.LogError("发送消息失败: {msg}", ex.Message);
            }
        }

        /// <summary>
        /// 从Coder开始运行
        /// </summary>
        /// <param name="text"></param>
        private async void RunDeveloping(string text)
        {
            try
            {
                await multiAgentOrchestrator.RunWorkflowAsync(text, WorkflowState.Developing);
                ActivationSendButton();
                conversationManager?.Save();
                Trace.WriteLine("/////////// 已恢复初始状态2 ///////////");
            }
            catch (Exception ex)
            {
                logger.LogError("从Coder开始运行失败: {msg}", ex.Message);
            }
        }

        /// <summary>
        /// 从Planner开始运行
        /// </summary>
        /// <param name="text"></param>
        private async void RunPlanning(string text)
        {
            try
            {
                isStart = true;
                await multiAgentOrchestrator.RunWorkflowAsync(text, WorkflowState.Planning);
                ActivationSendButton();
                isStart = false;
                conversationManager?.Save();
                Trace.WriteLine("/////////// 已恢复初始状态 ///////////");
            }
            catch (Exception ex)
            {
                logger.LogError("从Planner开始运行失败: {msg}", ex.Message);
            }
        }

        /// <summary>
        /// 激活发送按钮, 关闭停止按钮
        /// </summary>
        public void ActivationSendButton()
        {
            // 恢复 UI 状态
            SendButton.Enabled = true;
            SendButton.Visible = true;
            QuestionDialog.Visible = false;
            chatUIService.SetStopRunning();

            // 焦点还给输入框，方便连续对话
            UserInput.Focus();
        }

        /// <summary>
        /// 激活停止按钮, 关闭发送按钮
        /// </summary>
        public void ActivationStopButton()
        {
            SendButton.Enabled = false;
            SendButton.Visible = false;

            // 焦点还给输入框
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
        /// 打开设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettings(object sender, EventArgs e)
        {
            using SettingsForm settingsForm = new(settings);
            settingsForm.ShowDialog(this);
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
                UserInput.Focus();
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
                UserInput.Focus();
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
                UserInput.Focus();
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
                        PAllTokens.Text = $"{usage.TotalTokenCount}";
                        PInTokens.Text = $"{usage.InputTokenCount}";
                        POutTokens.Text = $"{usage.OutputTokenCount}";
                    }
                    break;
                case AgentType.Developer:
                    if (usage.TotalTokenCount > 0)
                    {
                        DAllTokens.Text = $"{usage.TotalTokenCount}";
                        DInTokens.Text = $"{usage.InputTokenCount}";
                        DOutTokens.Text = $"{usage.OutputTokenCount}";
                    }
                    break;
                case AgentType.Reviewer:
                    if (usage.TotalTokenCount > 0)
                    {
                        RAllTokens.Text = $"{usage.TotalTokenCount}";
                        RInTokens.Text = $"{usage.InputTokenCount}";
                        ROutTokens.Text = $"{usage.OutputTokenCount}";
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

        #region 对话列表

        /// <summary>
        /// 有会话被加载时用于激活会话
        /// </summary>
        private void ActivateConversation()
        {
            CreateConversationButton.Enabled = true;

            MiddlePanel.Visible = true;
            RightPanel.Visible = true;
        }

        /// <summary>
        /// 点击节点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConversationTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 会话存储TODO: 切换
            if (e.Node == null || e.Node.Parent == null || e.Node.IsSelected || selectedNode == e.Node) return;
            selectedNode = e.Node;
            var node = selectedNode.Tag as ConversationTreeNode;
            await conversationManager.SwitchConversationAsync(node);
        }

        /// <summary>
        /// 切换会话
        /// </summary>
        private void OnSwitchConversation(AgentContext context)
        {
            multiAgentOrchestrator = conversationManager.CurrentOrchestrator;
            if (PlannerAgentTab.IsSelected) UpdateChatPanelWidth(PlannerChatPanel);
            if (CoderAgentTab.IsSelected) UpdateChatPanelWidth(CoderChatPanel);
            if (ReviewerAgentTab.IsSelected) UpdateChatPanelWidth(ReviewerChatPanel);
        }

        private void OnConversationLoaded(List<ConversationTreeNode> tree)
        {
            foreach (var projectNodeData in tree)
            {
                var projectNode = new TreeNode(projectNodeData.Name) { Tag = projectNodeData, ToolTipText = projectNodeData.Path };

                if (projectNodeData.Children != null)
                {
                    foreach (var convData in projectNodeData.Children)
                    {
                        var convNode = new TreeNode(convData.Name) { Tag = convData, ToolTipText = convData.Path };
                        projectNode.Nodes.Add(convNode);
                    }
                }
                ConversationTreeView.Nodes.Add(projectNode);
            }

            if (tree.Count > 0 && tree[0].Children.Count > 0)
            {
                ConversationTreeView.Nodes[0].Expand();
                var node = ConversationTreeView.Nodes[0].Nodes[0];
                ConversationTreeView.SelectedNode = node;
                selectedNode = node;
            }
        }

        /// <summary>
        /// 清除当前会话
        /// </summary>
        private void ClearConversation()
        {
            PlannerChatPanel.Controls.Clear();
            CoderChatPanel.Controls.Clear();
            ReviewerChatPanel.Controls.Clear();
        }

        #endregion

        /// <summary>
        /// 打开项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenProject(object sender, EventArgs e)
        {
            // 会话存储TODO: 创建项目
            using FolderBrowserDialog folderDialog = new();
            folderDialog.Description = "请选择项目文件夹";
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;
                string name = new DirectoryInfo(selectedPath).Name;
                var node = await conversationManager.CreateProject(name, selectedPath);
                if (conversationManager.CurrentOrchestrator != null)
                {
                    var subNode = node.Children[0];
                    TreeNode subTreeNode = new(subNode.Name)
                    {
                        Tag = subNode,
                        ToolTipText = subNode.Path,
                    };
                    TreeNode projectTreeNode = new(name)
                    {
                        Tag = node,
                        ToolTipText = selectedPath,
                    };
                    projectTreeNode.Nodes.Add(subTreeNode);
                    ConversationTreeView.Nodes.Insert(0, projectTreeNode);
                    projectTreeNode.Expand();
                    ConversationTreeView.SelectedNode = subTreeNode;
                    selectedNode = subTreeNode;

                    multiAgentOrchestrator = conversationManager.CurrentOrchestrator;
                    multiAgentOrchestrator.OnResetUserInputState += ActivationSendButton;
                    ActivateConversation();
                }
            }
        }

        private async void CreateConversationButton_Click(object sender, EventArgs e)
        {
            // 会话存储TODO: 创建会话
            if (ConversationTreeView.SelectedNode == null) return;
            var parNode = ConversationTreeView.SelectedNode.Parent ?? ConversationTreeView.SelectedNode;
            var node = await conversationManager.CreateConversation(parNode.Tag as ConversationTreeNode, ConversationTreeView.SelectedNode.ToolTipText);
            if (conversationManager.CurrentOrchestrator != null)
            {
                TreeNode subTreeNode = new(node.Name)
                {
                    Tag = node,
                    ToolTipText = node.Path,
                };
                if (ConversationTreeView.SelectedNode.Parent == null)
                {
                    ConversationTreeView.SelectedNode.Nodes.Insert(0, subTreeNode);
                }
                else
                {
                    ConversationTreeView.SelectedNode.Parent.Nodes.Insert(0, subTreeNode);
                }
                ConversationTreeView.SelectedNode = subTreeNode;
                selectedNode = subTreeNode;
                multiAgentOrchestrator = conversationManager.CurrentOrchestrator;
                multiAgentOrchestrator.OnResetUserInputState += ActivationSendButton;
                ActivateConversation();
            }
        }

        private void OpenProjectButton_Click(object sender, EventArgs e)
        {
            OpenProject(sender, e);
        }

        private async void ConversationTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            // 会话存储TODO: 删除会话
            if (ConversationTreeView.SelectedNode == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = MessageBox.Show("会话信息删除后将无法恢复，是否确认删除？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    // 如果删除的是所选的
                    var node = ConversationTreeView.SelectedNode;
                    if (selectedNode == node)
                    {
                        ClearConversation();
                        ConversationTreeView.SelectedNode = null;
                        selectedNode = null;
                    }
                    var par = node.Parent == null ? node : node.Parent;
                    var sub = node.Parent == null ? null : node;
                    await conversationManager.Delete(par.Tag as ConversationTreeNode, sub?.Tag as ConversationTreeNode);
                    /*if (node.Parent == null)
					{
						foreach (TreeNode item in node.Nodes)
						{
							ConversationTreeView.Nodes.Remove(item);
						}
					}*/
                    ConversationTreeView.Nodes.Remove(node);
                }
            }
        }

        private void ConversationTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.CancelEdit) return;
            if (e.Node?.Tag is ConversationTreeNode node && e.Label != null)
            {
                node.Name = e.Label;
                conversationManager.SaveTree();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            conversationManager?.Save();
        }
    }
}
