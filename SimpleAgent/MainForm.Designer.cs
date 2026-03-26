namespace SimpleAgent
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            UserInput = new TextBox();
            TopMenu = new MenuStrip();
            TopMenu_AppName = new ToolStripTextBox();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            设置ToolStripMenuItem = new ToolStripMenuItem();
            模型配置ToolStripMenuItem = new ToolStripMenuItem();
            帮助ToolStripMenuItem = new ToolStripMenuItem();
            使用文档ToolStripMenuItem = new ToolStripMenuItem();
            关于ToolStripMenuItem = new ToolStripMenuItem();
            TopMenu_Close = new SimpleAgent.UserControls.ToolStripButton();
            TopMenu_Maximize = new SimpleAgent.UserControls.ToolStripButton();
            TopMenu_Minimize = new SimpleAgent.UserControls.ToolStripButton();
            StatusBar = new StatusStrip();
            GPUStackStatusLabel = new ToolStripStatusLabel();
            ModelStatusLabel = new ToolStripStatusLabel();
            MemoryLabel = new ToolStripStatusLabel();
            CoreLabel = new ToolStripStatusLabel();
            RefreshLabel = new ToolStripStatusLabel();
            PlannerChatPanel = new FlowLayoutPanel();
            QuestionDialog = new SimpleAgent.UserControls.QuestionDialog();
            ContentContainer = new TableLayoutPanel();
            LeftPanel = new TableLayoutPanel();
            label6 = new Label();
            label5 = new Label();
            label3 = new Label();
            label4 = new Label();
            MiddlePanel = new TableLayoutPanel();
            ChatButtonPanel = new Panel();
            StopButton = new SimpleAgent.UserControls.FlatButton();
            SendButton = new SimpleAgent.UserControls.FlatButton();
            ChatPanelContainer = new Panel();
            CoderChatPanel = new FlowLayoutPanel();
            ReviewerChatPanel = new FlowLayoutPanel();
            AgentTabPanel = new TableLayoutPanel();
            PlannerAgentTab = new SimpleAgent.UserControls.AgentTab();
            CoderAgentTab = new SimpleAgent.UserControls.AgentTab();
            ReviewerAgentTab = new SimpleAgent.UserControls.AgentTab();
            RightPanel = new TableLayoutPanel();
            label2 = new Label();
            BackgroundServerListBox = new ListBox();
            BackgroundServerMenuStrip = new ContextMenuStrip(components);
            BackgroundServerMenuItem_Stop = new ToolStripMenuItem();
            label1 = new Label();
            TokensPanel = new TableLayoutPanel();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            PAllTokens = new Label();
            PInTokens = new Label();
            POutTokens = new Label();
            DAllTokens = new Label();
            DInTokens = new Label();
            DOutTokens = new Label();
            RAllTokens = new Label();
            RInTokens = new Label();
            ROutTokens = new Label();
            TopMenu.SuspendLayout();
            StatusBar.SuspendLayout();
            ContentContainer.SuspendLayout();
            LeftPanel.SuspendLayout();
            MiddlePanel.SuspendLayout();
            ChatButtonPanel.SuspendLayout();
            ChatPanelContainer.SuspendLayout();
            AgentTabPanel.SuspendLayout();
            RightPanel.SuspendLayout();
            BackgroundServerMenuStrip.SuspendLayout();
            TokensPanel.SuspendLayout();
            SuspendLayout();
            // 
            // UserInput
            // 
            UserInput.BackColor = Color.WhiteSmoke;
            UserInput.BorderStyle = BorderStyle.FixedSingle;
            UserInput.Dock = DockStyle.Fill;
            UserInput.Location = new Point(5, 572);
            UserInput.Margin = new Padding(5, 5, 0, 5);
            UserInput.MaxLength = 0;
            UserInput.Multiline = true;
            UserInput.Name = "UserInput";
            UserInput.Size = new Size(611, 70);
            UserInput.TabIndex = 1;
            UserInput.Text = "帮我用web开发一个笔记软件";
            // 
            // TopMenu
            // 
            TopMenu.AutoSize = false;
            TopMenu.BackColor = SystemColors.Control;
            TopMenu.GripMargin = new Padding(0);
            TopMenu.Items.AddRange(new ToolStripItem[] { TopMenu_AppName, 文件ToolStripMenuItem, 设置ToolStripMenuItem, 帮助ToolStripMenuItem, TopMenu_Close, TopMenu_Maximize, TopMenu_Minimize });
            TopMenu.Location = new Point(4, 4);
            TopMenu.Name = "TopMenu";
            TopMenu.Padding = new Padding(0, 0, 0, 5);
            TopMenu.Size = new Size(1272, 37);
            TopMenu.TabIndex = 3;
            TopMenu.Text = "顶部菜单";
            TopMenu.MouseDown += TopMenu_MouseDown;
            // 
            // TopMenu_AppName
            // 
            TopMenu_AppName.BackColor = SystemColors.Control;
            TopMenu_AppName.BorderStyle = BorderStyle.None;
            TopMenu_AppName.Enabled = false;
            TopMenu_AppName.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            TopMenu_AppName.Name = "TopMenu_AppName";
            TopMenu_AppName.ReadOnly = true;
            TopMenu_AppName.ShortcutsEnabled = false;
            TopMenu_AppName.Size = new Size(120, 32);
            TopMenu_AppName.Text = "AI编程助手";
            TopMenu_AppName.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripSeparator1, 退出ToolStripMenuItem });
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 32);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(124, 22);
            toolStripMenuItem1.Text = "打开项目";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(121, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(124, 22);
            退出ToolStripMenuItem.Text = "退出";
            // 
            // 设置ToolStripMenuItem
            // 
            设置ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 模型配置ToolStripMenuItem });
            设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            设置ToolStripMenuItem.Size = new Size(44, 32);
            设置ToolStripMenuItem.Text = "设置";
            // 
            // 模型配置ToolStripMenuItem
            // 
            模型配置ToolStripMenuItem.Name = "模型配置ToolStripMenuItem";
            模型配置ToolStripMenuItem.Size = new Size(124, 22);
            模型配置ToolStripMenuItem.Text = "模型配置";
            // 
            // 帮助ToolStripMenuItem
            // 
            帮助ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 使用文档ToolStripMenuItem, 关于ToolStripMenuItem });
            帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            帮助ToolStripMenuItem.Size = new Size(44, 32);
            帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 使用文档ToolStripMenuItem
            // 
            使用文档ToolStripMenuItem.Name = "使用文档ToolStripMenuItem";
            使用文档ToolStripMenuItem.Size = new Size(124, 22);
            使用文档ToolStripMenuItem.Text = "使用文档";
            // 
            // 关于ToolStripMenuItem
            // 
            关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            关于ToolStripMenuItem.Size = new Size(124, 22);
            关于ToolStripMenuItem.Text = "关于";
            // 
            // TopMenu_Close
            // 
            TopMenu_Close.Alignment = ToolStripItemAlignment.Right;
            TopMenu_Close.AutoSize = false;
            TopMenu_Close.BackColor = Color.Transparent;
            TopMenu_Close.BackgroundImageLayout = ImageLayout.Center;
            TopMenu_Close.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            TopMenu_Close.Margin = new Padding(0);
            TopMenu_Close.MouseDownBackColor = Color.Firebrick;
            TopMenu_Close.MouseOverBackColor = Color.LightCoral;
            TopMenu_Close.Name = "TopMenu_Close";
            TopMenu_Close.Size = new Size(32, 32);
            TopMenu_Close.Text = "✕";
            TopMenu_Close.Click += TopMenu_Close_Click;
            // 
            // TopMenu_Maximize
            // 
            TopMenu_Maximize.Alignment = ToolStripItemAlignment.Right;
            TopMenu_Maximize.AutoSize = false;
            TopMenu_Maximize.BackColor = Color.Transparent;
            TopMenu_Maximize.BackgroundImageLayout = ImageLayout.Center;
            TopMenu_Maximize.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            TopMenu_Maximize.Margin = new Padding(0);
            TopMenu_Maximize.MouseDownBackColor = Color.Gainsboro;
            TopMenu_Maximize.MouseOverBackColor = Color.White;
            TopMenu_Maximize.Name = "TopMenu_Maximize";
            TopMenu_Maximize.Size = new Size(32, 32);
            TopMenu_Maximize.Text = "☐";
            TopMenu_Maximize.Click += TopMenu_Maximize_Click;
            // 
            // TopMenu_Minimize
            // 
            TopMenu_Minimize.Alignment = ToolStripItemAlignment.Right;
            TopMenu_Minimize.AutoSize = false;
            TopMenu_Minimize.BackColor = Color.Transparent;
            TopMenu_Minimize.BackgroundImageLayout = ImageLayout.Center;
            TopMenu_Minimize.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            TopMenu_Minimize.Margin = new Padding(0);
            TopMenu_Minimize.MouseDownBackColor = Color.Gainsboro;
            TopMenu_Minimize.MouseOverBackColor = Color.White;
            TopMenu_Minimize.Name = "TopMenu_Minimize";
            TopMenu_Minimize.Size = new Size(32, 32);
            TopMenu_Minimize.Text = "─";
            TopMenu_Minimize.Click += TopMenu_Minimize_Click;
            // 
            // StatusBar
            // 
            StatusBar.AllowMerge = false;
            StatusBar.GripMargin = new Padding(0);
            StatusBar.Items.AddRange(new ToolStripItem[] { GPUStackStatusLabel, ModelStatusLabel, MemoryLabel, CoreLabel, RefreshLabel });
            StatusBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            StatusBar.Location = new Point(4, 694);
            StatusBar.Name = "StatusBar";
            StatusBar.Size = new Size(1272, 22);
            StatusBar.SizingGrip = false;
            StatusBar.TabIndex = 4;
            StatusBar.Text = "状态栏";
            // 
            // GPUStackStatusLabel
            // 
            GPUStackStatusLabel.Margin = new Padding(0, 4, 0, 0);
            GPUStackStatusLabel.Name = "GPUStackStatusLabel";
            GPUStackStatusLabel.Size = new Size(84, 18);
            GPUStackStatusLabel.Text = "[ 服务连接中 ]";
            // 
            // ModelStatusLabel
            // 
            ModelStatusLabel.Margin = new Padding(10, 4, 0, 0);
            ModelStatusLabel.Name = "ModelStatusLabel";
            ModelStatusLabel.Size = new Size(84, 18);
            ModelStatusLabel.Text = "[ 模型连接中 ]";
            // 
            // MemoryLabel
            // 
            MemoryLabel.Margin = new Padding(10, 4, 0, 0);
            MemoryLabel.Name = "MemoryLabel";
            MemoryLabel.Size = new Size(154, 18);
            MemoryLabel.Text = "显存占用率：0%  0 / 0 GB";
            // 
            // CoreLabel
            // 
            CoreLabel.Margin = new Padding(10, 4, 0, 0);
            CoreLabel.Name = "CoreLabel";
            CoreLabel.Size = new Size(86, 18);
            CoreLabel.Text = "算力负载：0%";
            // 
            // RefreshLabel
            // 
            RefreshLabel.ForeColor = SystemColors.HotTrack;
            RefreshLabel.Margin = new Padding(20, 4, 0, 0);
            RefreshLabel.Name = "RefreshLabel";
            RefreshLabel.Size = new Size(56, 18);
            RefreshLabel.Text = "刷新状态";
            RefreshLabel.Click += RefreshLabel_Click;
            // 
            // PlannerChatPanel
            // 
            PlannerChatPanel.AutoScroll = true;
            PlannerChatPanel.BackColor = Color.White;
            PlannerChatPanel.Dock = DockStyle.Fill;
            PlannerChatPanel.FlowDirection = FlowDirection.TopDown;
            PlannerChatPanel.Location = new Point(0, 0);
            PlannerChatPanel.Margin = new Padding(5, 5, 5, 0);
            PlannerChatPanel.Name = "PlannerChatPanel";
            PlannerChatPanel.Size = new Size(676, 517);
            PlannerChatPanel.TabIndex = 3;
            PlannerChatPanel.WrapContents = false;
            PlannerChatPanel.SizeChanged += PlannerChatPanel_SizeChanged;
            // 
            // QuestionDialog
            // 
            QuestionDialog.AutoSize = true;
            QuestionDialog.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            QuestionDialog.BackColor = Color.WhiteSmoke;
            QuestionDialog.Dock = DockStyle.Bottom;
            QuestionDialog.Location = new Point(0, 429);
            QuestionDialog.Margin = new Padding(0);
            QuestionDialog.MinimumSize = new Size(220, 0);
            QuestionDialog.Name = "QuestionDialog";
            QuestionDialog.Size = new Size(676, 88);
            QuestionDialog.TabIndex = 0;
            QuestionDialog.Visible = false;
            // 
            // ContentContainer
            // 
            ContentContainer.ColumnCount = 3;
            ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));
            ContentContainer.Controls.Add(LeftPanel, 0, 0);
            ContentContainer.Controls.Add(MiddlePanel, 1, 0);
            ContentContainer.Controls.Add(RightPanel, 2, 0);
            ContentContainer.Dock = DockStyle.Fill;
            ContentContainer.Location = new Point(4, 41);
            ContentContainer.Margin = new Padding(0);
            ContentContainer.Name = "ContentContainer";
            ContentContainer.RowCount = 1;
            ContentContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ContentContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            ContentContainer.Size = new Size(1272, 653);
            ContentContainer.TabIndex = 5;
            // 
            // LeftPanel
            // 
            LeftPanel.BackColor = Color.White;
            LeftPanel.ColumnCount = 1;
            LeftPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            LeftPanel.Controls.Add(label6, 0, 3);
            LeftPanel.Controls.Add(label5, 0, 2);
            LeftPanel.Controls.Add(label3, 0, 0);
            LeftPanel.Controls.Add(label4, 0, 1);
            LeftPanel.Dock = DockStyle.Fill;
            LeftPanel.Location = new Point(0, 0);
            LeftPanel.Margin = new Padding(0);
            LeftPanel.Name = "LeftPanel";
            LeftPanel.RowCount = 4;
            LeftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            LeftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            LeftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            LeftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            LeftPanel.Size = new Size(250, 653);
            LeftPanel.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label6.ForeColor = Color.Silver;
            label6.Location = new Point(0, 237);
            label6.Margin = new Padding(0);
            label6.Name = "label6";
            label6.Size = new Size(250, 416);
            label6.TabIndex = 7;
            label6.Text = "待开发";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.WhiteSmoke;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label5.Location = new Point(0, 207);
            label5.Margin = new Padding(0);
            label5.Name = "label5";
            label5.Padding = new Padding(10, 0, 10, 0);
            label5.Size = new Size(250, 30);
            label5.TabIndex = 5;
            label5.Text = "对话列表";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.WhiteSmoke;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label3.Location = new Point(0, 0);
            label3.Margin = new Padding(0);
            label3.Name = "label3";
            label3.Padding = new Padding(10, 0, 10, 0);
            label3.Size = new Size(250, 30);
            label3.TabIndex = 3;
            label3.Text = "项目信息";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label4.ForeColor = Color.Silver;
            label4.Location = new Point(0, 30);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(250, 177);
            label4.TabIndex = 6;
            label4.Text = "待开发";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MiddlePanel
            // 
            MiddlePanel.BackColor = Color.White;
            MiddlePanel.ColumnCount = 2;
            MiddlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MiddlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            MiddlePanel.Controls.Add(UserInput, 0, 2);
            MiddlePanel.Controls.Add(ChatButtonPanel, 1, 2);
            MiddlePanel.Controls.Add(ChatPanelContainer, 0, 1);
            MiddlePanel.Controls.Add(AgentTabPanel, 0, 0);
            MiddlePanel.Dock = DockStyle.Fill;
            MiddlePanel.Location = new Point(253, 3);
            MiddlePanel.Name = "MiddlePanel";
            MiddlePanel.RowCount = 3;
            MiddlePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            MiddlePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MiddlePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            MiddlePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            MiddlePanel.Size = new Size(676, 647);
            MiddlePanel.TabIndex = 4;
            // 
            // ChatButtonPanel
            // 
            ChatButtonPanel.Controls.Add(SendButton);
            ChatButtonPanel.Controls.Add(StopButton);
            ChatButtonPanel.Dock = DockStyle.Fill;
            ChatButtonPanel.Location = new Point(621, 572);
            ChatButtonPanel.Margin = new Padding(5);
            ChatButtonPanel.Name = "ChatButtonPanel";
            ChatButtonPanel.Size = new Size(50, 70);
            ChatButtonPanel.TabIndex = 5;
            // 
            // StopButton
            // 
            StopButton.BackColor = Color.IndianRed;
            StopButton.Dock = DockStyle.Fill;
            StopButton.FlatAppearance.BorderSize = 0;
            StopButton.FlatAppearance.MouseDownBackColor = Color.Maroon;
            StopButton.FlatAppearance.MouseOverBackColor = Color.Brown;
            StopButton.FlatStyle = FlatStyle.Flat;
            StopButton.ForeColor = Color.White;
            StopButton.Location = new Point(0, 0);
            StopButton.Margin = new Padding(0);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(50, 70);
            StopButton.TabIndex = 3;
            StopButton.Text = "停止";
            StopButton.UseVisualStyleBackColor = false;
            StopButton.Click += StopButton_Click;
            // 
            // SendButton
            // 
            SendButton.BackColor = Color.DodgerBlue;
            SendButton.Dock = DockStyle.Fill;
            SendButton.FlatAppearance.BorderSize = 0;
            SendButton.FlatAppearance.MouseDownBackColor = Color.SteelBlue;
            SendButton.FlatAppearance.MouseOverBackColor = Color.DeepSkyBlue;
            SendButton.FlatStyle = FlatStyle.Flat;
            SendButton.ForeColor = Color.White;
            SendButton.Location = new Point(0, 0);
            SendButton.Margin = new Padding(0);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(50, 70);
            SendButton.TabIndex = 2;
            SendButton.Text = "发送";
            SendButton.UseVisualStyleBackColor = false;
            // 
            // ChatPanelContainer
            // 
            MiddlePanel.SetColumnSpan(ChatPanelContainer, 2);
            ChatPanelContainer.Controls.Add(QuestionDialog);
            ChatPanelContainer.Controls.Add(PlannerChatPanel);
            ChatPanelContainer.Controls.Add(CoderChatPanel);
            ChatPanelContainer.Controls.Add(ReviewerChatPanel);
            ChatPanelContainer.Dock = DockStyle.Fill;
            ChatPanelContainer.Location = new Point(0, 50);
            ChatPanelContainer.Margin = new Padding(0);
            ChatPanelContainer.Name = "ChatPanelContainer";
            ChatPanelContainer.Size = new Size(676, 517);
            ChatPanelContainer.TabIndex = 3;
            // 
            // CoderChatPanel
            // 
            CoderChatPanel.AutoScroll = true;
            CoderChatPanel.BackColor = Color.White;
            CoderChatPanel.Dock = DockStyle.Fill;
            CoderChatPanel.FlowDirection = FlowDirection.TopDown;
            CoderChatPanel.Location = new Point(0, 0);
            CoderChatPanel.Margin = new Padding(5, 5, 5, 0);
            CoderChatPanel.Name = "CoderChatPanel";
            CoderChatPanel.Size = new Size(676, 517);
            CoderChatPanel.TabIndex = 4;
            CoderChatPanel.Visible = false;
            CoderChatPanel.WrapContents = false;
            CoderChatPanel.SizeChanged += CoderChatPanel_SizeChanged;
            // 
            // ReviewerChatPanel
            // 
            ReviewerChatPanel.AutoScroll = true;
            ReviewerChatPanel.BackColor = Color.White;
            ReviewerChatPanel.Dock = DockStyle.Fill;
            ReviewerChatPanel.FlowDirection = FlowDirection.TopDown;
            ReviewerChatPanel.Location = new Point(0, 0);
            ReviewerChatPanel.Margin = new Padding(5, 5, 5, 0);
            ReviewerChatPanel.Name = "ReviewerChatPanel";
            ReviewerChatPanel.Size = new Size(676, 517);
            ReviewerChatPanel.TabIndex = 5;
            ReviewerChatPanel.Visible = false;
            ReviewerChatPanel.WrapContents = false;
            ReviewerChatPanel.SizeChanged += ReviewerChatPanel_SizeChanged;
            // 
            // AgentTabPanel
            // 
            AgentTabPanel.ColumnCount = 3;
            MiddlePanel.SetColumnSpan(AgentTabPanel, 2);
            AgentTabPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            AgentTabPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            AgentTabPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            AgentTabPanel.Controls.Add(PlannerAgentTab, 0, 0);
            AgentTabPanel.Controls.Add(CoderAgentTab, 1, 0);
            AgentTabPanel.Controls.Add(ReviewerAgentTab, 2, 0);
            AgentTabPanel.Dock = DockStyle.Fill;
            AgentTabPanel.Location = new Point(0, 0);
            AgentTabPanel.Margin = new Padding(0);
            AgentTabPanel.Name = "AgentTabPanel";
            AgentTabPanel.RowCount = 1;
            AgentTabPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            AgentTabPanel.Size = new Size(676, 50);
            AgentTabPanel.TabIndex = 4;
            // 
            // PlannerAgentTab
            // 
            PlannerAgentTab.AgentName = "规划智能体";
            PlannerAgentTab.BackColor = Color.White;
            PlannerAgentTab.Dock = DockStyle.Fill;
            PlannerAgentTab.IsSelected = true;
            PlannerAgentTab.Location = new Point(4, 4);
            PlannerAgentTab.Margin = new Padding(4, 4, 2, 2);
            PlannerAgentTab.Name = "PlannerAgentTab";
            PlannerAgentTab.Size = new Size(219, 44);
            PlannerAgentTab.TabIndex = 0;
            // 
            // CoderAgentTab
            // 
            CoderAgentTab.AgentName = "编程智能体";
            CoderAgentTab.BackColor = Color.WhiteSmoke;
            CoderAgentTab.Dock = DockStyle.Fill;
            CoderAgentTab.IsSelected = false;
            CoderAgentTab.Location = new Point(227, 4);
            CoderAgentTab.Margin = new Padding(2, 4, 2, 2);
            CoderAgentTab.Name = "CoderAgentTab";
            CoderAgentTab.Size = new Size(221, 44);
            CoderAgentTab.TabIndex = 1;
            // 
            // ReviewerAgentTab
            // 
            ReviewerAgentTab.AgentName = "审查智能体";
            ReviewerAgentTab.BackColor = Color.WhiteSmoke;
            ReviewerAgentTab.Dock = DockStyle.Fill;
            ReviewerAgentTab.IsSelected = false;
            ReviewerAgentTab.Location = new Point(452, 4);
            ReviewerAgentTab.Margin = new Padding(2, 4, 4, 2);
            ReviewerAgentTab.Name = "ReviewerAgentTab";
            ReviewerAgentTab.Size = new Size(220, 44);
            ReviewerAgentTab.TabIndex = 2;
            // 
            // RightPanel
            // 
            RightPanel.BackColor = Color.White;
            RightPanel.ColumnCount = 1;
            RightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            RightPanel.Controls.Add(label2, 0, 2);
            RightPanel.Controls.Add(BackgroundServerListBox, 0, 3);
            RightPanel.Controls.Add(label1, 0, 0);
            RightPanel.Controls.Add(TokensPanel, 0, 1);
            RightPanel.Dock = DockStyle.Fill;
            RightPanel.Location = new Point(932, 0);
            RightPanel.Margin = new Padding(0);
            RightPanel.Name = "RightPanel";
            RightPanel.RowCount = 4;
            RightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            RightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            RightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            RightPanel.Size = new Size(340, 653);
            RightPanel.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.WhiteSmoke;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label2.Location = new Point(0, 423);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Padding = new Padding(10, 0, 10, 0);
            label2.Size = new Size(340, 30);
            label2.TabIndex = 3;
            label2.Text = "运行中的后台服务";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // BackgroundServerListBox
            // 
            BackgroundServerListBox.BorderStyle = BorderStyle.None;
            BackgroundServerListBox.ContextMenuStrip = BackgroundServerMenuStrip;
            BackgroundServerListBox.Dock = DockStyle.Fill;
            BackgroundServerListBox.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            BackgroundServerListBox.Location = new Point(0, 453);
            BackgroundServerListBox.Margin = new Padding(0);
            BackgroundServerListBox.Name = "BackgroundServerListBox";
            BackgroundServerListBox.Size = new Size(340, 200);
            BackgroundServerListBox.TabIndex = 1;
            // 
            // BackgroundServerMenuStrip
            // 
            BackgroundServerMenuStrip.Items.AddRange(new ToolStripItem[] { BackgroundServerMenuItem_Stop });
            BackgroundServerMenuStrip.Name = "BackgroundServerMenuStrip";
            BackgroundServerMenuStrip.Size = new Size(125, 26);
            BackgroundServerMenuStrip.Opening += BackgroundServerMenuStrip_Opening;
            // 
            // BackgroundServerMenuItem_Stop
            // 
            BackgroundServerMenuItem_Stop.Name = "BackgroundServerMenuItem_Stop";
            BackgroundServerMenuItem_Stop.Size = new Size(124, 22);
            BackgroundServerMenuItem_Stop.Text = "停止服务";
            BackgroundServerMenuItem_Stop.MouseDown += BackgroundServerMenuItem_Stop_MouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.WhiteSmoke;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(0);
            label1.Name = "label1";
            label1.Padding = new Padding(10, 0, 10, 0);
            label1.Size = new Size(340, 30);
            label1.TabIndex = 2;
            label1.Text = "智能体会话信息";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TokensPanel
            // 
            TokensPanel.ColumnCount = 3;
            TokensPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            TokensPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            TokensPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            TokensPanel.Controls.Add(label7, 0, 0);
            TokensPanel.Controls.Add(label8, 0, 3);
            TokensPanel.Controls.Add(label9, 0, 6);
            TokensPanel.Controls.Add(label10, 0, 1);
            TokensPanel.Controls.Add(label11, 1, 1);
            TokensPanel.Controls.Add(label12, 2, 1);
            TokensPanel.Controls.Add(label13, 0, 4);
            TokensPanel.Controls.Add(label14, 1, 4);
            TokensPanel.Controls.Add(label15, 2, 4);
            TokensPanel.Controls.Add(label16, 0, 7);
            TokensPanel.Controls.Add(label17, 1, 7);
            TokensPanel.Controls.Add(label18, 2, 7);
            TokensPanel.Controls.Add(PAllTokens, 0, 2);
            TokensPanel.Controls.Add(PInTokens, 1, 2);
            TokensPanel.Controls.Add(POutTokens, 2, 2);
            TokensPanel.Controls.Add(DAllTokens, 0, 5);
            TokensPanel.Controls.Add(DInTokens, 1, 5);
            TokensPanel.Controls.Add(DOutTokens, 2, 5);
            TokensPanel.Controls.Add(RAllTokens, 0, 8);
            TokensPanel.Controls.Add(RInTokens, 1, 8);
            TokensPanel.Controls.Add(ROutTokens, 2, 8);
            TokensPanel.Dock = DockStyle.Fill;
            TokensPanel.Location = new Point(4, 34);
            TokensPanel.Margin = new Padding(4);
            TokensPanel.Name = "TokensPanel";
            TokensPanel.RowCount = 9;
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TokensPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            TokensPanel.Size = new Size(332, 385);
            TokensPanel.TabIndex = 4;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.WhiteSmoke;
            TokensPanel.SetColumnSpan(label7, 3);
            label7.Dock = DockStyle.Fill;
            label7.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            label7.Location = new Point(0, 0);
            label7.Margin = new Padding(0);
            label7.Name = "label7";
            label7.Size = new Size(332, 30);
            label7.TabIndex = 0;
            label7.Text = "规划智能体信息";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.WhiteSmoke;
            TokensPanel.SetColumnSpan(label8, 3);
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            label8.Location = new Point(0, 110);
            label8.Margin = new Padding(0);
            label8.Name = "label8";
            label8.Size = new Size(332, 30);
            label8.TabIndex = 1;
            label8.Text = "编程智能体信息";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.WhiteSmoke;
            TokensPanel.SetColumnSpan(label9, 3);
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            label9.Location = new Point(0, 220);
            label9.Margin = new Padding(0);
            label9.Name = "label9";
            label9.Size = new Size(332, 30);
            label9.TabIndex = 2;
            label9.Text = "审查智能体信息";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Fill;
            label10.Font = new Font("Microsoft YaHei UI", 10.5F);
            label10.Location = new Point(0, 30);
            label10.Margin = new Padding(0);
            label10.Name = "label10";
            label10.Size = new Size(110, 30);
            label10.TabIndex = 3;
            label10.Text = "总Tokens";
            label10.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Dock = DockStyle.Fill;
            label11.Font = new Font("Microsoft YaHei UI", 10.5F);
            label11.Location = new Point(110, 30);
            label11.Margin = new Padding(0);
            label11.Name = "label11";
            label11.Size = new Size(110, 30);
            label11.TabIndex = 4;
            label11.Text = "输入Tokens";
            label11.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Dock = DockStyle.Fill;
            label12.Font = new Font("Microsoft YaHei UI", 10.5F);
            label12.Location = new Point(220, 30);
            label12.Margin = new Padding(0);
            label12.Name = "label12";
            label12.Size = new Size(112, 30);
            label12.TabIndex = 5;
            label12.Text = "输出Tokens";
            label12.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Dock = DockStyle.Fill;
            label13.Font = new Font("Microsoft YaHei UI", 10.5F);
            label13.Location = new Point(0, 140);
            label13.Margin = new Padding(0);
            label13.Name = "label13";
            label13.Size = new Size(110, 30);
            label13.TabIndex = 6;
            label13.Text = "总Tokens";
            label13.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Dock = DockStyle.Fill;
            label14.Font = new Font("Microsoft YaHei UI", 10.5F);
            label14.Location = new Point(110, 140);
            label14.Margin = new Padding(0);
            label14.Name = "label14";
            label14.Size = new Size(110, 30);
            label14.TabIndex = 7;
            label14.Text = "输入Tokens";
            label14.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Dock = DockStyle.Fill;
            label15.Font = new Font("Microsoft YaHei UI", 10.5F);
            label15.Location = new Point(220, 140);
            label15.Margin = new Padding(0);
            label15.Name = "label15";
            label15.Size = new Size(112, 30);
            label15.TabIndex = 8;
            label15.Text = "输出Tokens";
            label15.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Dock = DockStyle.Fill;
            label16.Font = new Font("Microsoft YaHei UI", 10.5F);
            label16.Location = new Point(0, 250);
            label16.Margin = new Padding(0);
            label16.Name = "label16";
            label16.Size = new Size(110, 30);
            label16.TabIndex = 9;
            label16.Text = "总Tokens";
            label16.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Dock = DockStyle.Fill;
            label17.Font = new Font("Microsoft YaHei UI", 10.5F);
            label17.Location = new Point(110, 250);
            label17.Margin = new Padding(0);
            label17.Name = "label17";
            label17.Size = new Size(110, 30);
            label17.TabIndex = 10;
            label17.Text = "输入Tokens";
            label17.TextAlign = ContentAlignment.BottomCenter;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Dock = DockStyle.Fill;
            label18.Font = new Font("Microsoft YaHei UI", 10.5F);
            label18.Location = new Point(220, 250);
            label18.Margin = new Padding(0);
            label18.Name = "label18";
            label18.Size = new Size(112, 30);
            label18.TabIndex = 11;
            label18.Text = "输出Tokens";
            label18.TextAlign = ContentAlignment.BottomCenter;
            // 
            // PAllTokens
            // 
            PAllTokens.AutoSize = true;
            PAllTokens.Dock = DockStyle.Top;
            PAllTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            PAllTokens.ForeColor = SystemColors.MenuHighlight;
            PAllTokens.Location = new Point(0, 60);
            PAllTokens.Margin = new Padding(0);
            PAllTokens.Name = "PAllTokens";
            PAllTokens.Size = new Size(110, 35);
            PAllTokens.TabIndex = 12;
            PAllTokens.Text = "0";
            PAllTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PInTokens
            // 
            PInTokens.AutoSize = true;
            PInTokens.Dock = DockStyle.Top;
            PInTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            PInTokens.ForeColor = SystemColors.MenuHighlight;
            PInTokens.Location = new Point(110, 60);
            PInTokens.Margin = new Padding(0);
            PInTokens.Name = "PInTokens";
            PInTokens.Size = new Size(110, 35);
            PInTokens.TabIndex = 13;
            PInTokens.Text = "0";
            PInTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // POutTokens
            // 
            POutTokens.AutoSize = true;
            POutTokens.Dock = DockStyle.Top;
            POutTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            POutTokens.ForeColor = SystemColors.MenuHighlight;
            POutTokens.Location = new Point(220, 60);
            POutTokens.Margin = new Padding(0);
            POutTokens.Name = "POutTokens";
            POutTokens.Size = new Size(112, 35);
            POutTokens.TabIndex = 14;
            POutTokens.Text = "0";
            POutTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DAllTokens
            // 
            DAllTokens.AutoSize = true;
            DAllTokens.Dock = DockStyle.Top;
            DAllTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            DAllTokens.ForeColor = SystemColors.MenuHighlight;
            DAllTokens.Location = new Point(0, 170);
            DAllTokens.Margin = new Padding(0);
            DAllTokens.Name = "DAllTokens";
            DAllTokens.Size = new Size(110, 35);
            DAllTokens.TabIndex = 15;
            DAllTokens.Text = "0";
            DAllTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DInTokens
            // 
            DInTokens.AutoSize = true;
            DInTokens.Dock = DockStyle.Top;
            DInTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            DInTokens.ForeColor = SystemColors.MenuHighlight;
            DInTokens.Location = new Point(110, 170);
            DInTokens.Margin = new Padding(0);
            DInTokens.Name = "DInTokens";
            DInTokens.Size = new Size(110, 35);
            DInTokens.TabIndex = 16;
            DInTokens.Text = "0";
            DInTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DOutTokens
            // 
            DOutTokens.AutoSize = true;
            DOutTokens.Dock = DockStyle.Top;
            DOutTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            DOutTokens.ForeColor = SystemColors.MenuHighlight;
            DOutTokens.Location = new Point(220, 170);
            DOutTokens.Margin = new Padding(0);
            DOutTokens.Name = "DOutTokens";
            DOutTokens.Size = new Size(112, 35);
            DOutTokens.TabIndex = 17;
            DOutTokens.Text = "0";
            DOutTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RAllTokens
            // 
            RAllTokens.AutoSize = true;
            RAllTokens.Dock = DockStyle.Top;
            RAllTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            RAllTokens.ForeColor = SystemColors.MenuHighlight;
            RAllTokens.Location = new Point(0, 280);
            RAllTokens.Margin = new Padding(0);
            RAllTokens.Name = "RAllTokens";
            RAllTokens.Size = new Size(110, 35);
            RAllTokens.TabIndex = 18;
            RAllTokens.Text = "0";
            RAllTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RInTokens
            // 
            RInTokens.AutoSize = true;
            RInTokens.Dock = DockStyle.Top;
            RInTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            RInTokens.ForeColor = SystemColors.MenuHighlight;
            RInTokens.Location = new Point(110, 280);
            RInTokens.Margin = new Padding(0);
            RInTokens.Name = "RInTokens";
            RInTokens.Size = new Size(110, 35);
            RInTokens.TabIndex = 19;
            RInTokens.Text = "0";
            RInTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ROutTokens
            // 
            ROutTokens.AutoSize = true;
            ROutTokens.Dock = DockStyle.Top;
            ROutTokens.Font = new Font("Bahnschrift Condensed", 21.75F, FontStyle.Bold);
            ROutTokens.ForeColor = SystemColors.MenuHighlight;
            ROutTokens.Location = new Point(220, 280);
            ROutTokens.Margin = new Padding(0);
            ROutTokens.Name = "ROutTokens";
            ROutTokens.Size = new Size(112, 35);
            ROutTokens.TabIndex = 20;
            ROutTokens.Text = "0";
            ROutTokens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 720);
            Controls.Add(ContentContainer);
            Controls.Add(StatusBar);
            Controls.Add(TopMenu);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MainMenuStrip = TopMenu;
            MinimumSize = new Size(740, 460);
            Name = "MainForm";
            Padding = new Padding(4);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI编程助手";
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            TopMenu.ResumeLayout(false);
            TopMenu.PerformLayout();
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            ContentContainer.ResumeLayout(false);
            LeftPanel.ResumeLayout(false);
            LeftPanel.PerformLayout();
            MiddlePanel.ResumeLayout(false);
            MiddlePanel.PerformLayout();
            ChatButtonPanel.ResumeLayout(false);
            ChatPanelContainer.ResumeLayout(false);
            ChatPanelContainer.PerformLayout();
            AgentTabPanel.ResumeLayout(false);
            RightPanel.ResumeLayout(false);
            RightPanel.PerformLayout();
            BackgroundServerMenuStrip.ResumeLayout(false);
            TokensPanel.ResumeLayout(false);
            TokensPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox UserInput;
		private MenuStrip TopMenu;
		private ToolStripMenuItem 文件ToolStripMenuItem;
		private ToolStripMenuItem 设置ToolStripMenuItem;
		private ToolStripMenuItem 帮助ToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItem1;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem 退出ToolStripMenuItem;
		private ToolStripMenuItem 模型配置ToolStripMenuItem;
		private ToolStripMenuItem 使用文档ToolStripMenuItem;
		private ToolStripMenuItem 关于ToolStripMenuItem;
		private UserControls.ToolStripButton TopMenu_Minimize;
		private UserControls.ToolStripButton TopMenu_Close;
		private UserControls.ToolStripButton TopMenu_Maximize;
		private ToolStripTextBox TopMenu_AppName;
		private StatusStrip StatusBar;
		private ToolStripStatusLabel GPUStackStatusLabel;
		private TableLayoutPanel ContentContainer;
		private TableLayoutPanel MiddlePanel;
		private Panel ChatPanelContainer;
		private TableLayoutPanel AgentTabPanel;
		public FlowLayoutPanel PlannerChatPanel;
		public FlowLayoutPanel ReviewerChatPanel;
		public FlowLayoutPanel CoderChatPanel;
		public UserControls.AgentTab PlannerAgentTab;
		public UserControls.AgentTab CoderAgentTab;
		public UserControls.AgentTab ReviewerAgentTab;
		private TableLayoutPanel RightPanel;
		private TableLayoutPanel LeftPanel;
		private ListBox BackgroundServerListBox;
		private Label label2;
		private Label label1;
		private ContextMenuStrip BackgroundServerMenuStrip;
		private ToolStripMenuItem BackgroundServerMenuItem_Stop;
		private Label label5;
		private Label label3;
		private Label label6;
		private Label label4;
		private ToolStripStatusLabel ModelStatusLabel;
		private ToolStripStatusLabel MemoryLabel;
		private ToolStripStatusLabel CoreLabel;
		private ToolStripStatusLabel RefreshLabel;
		private TableLayoutPanel TokensPanel;
		private Label label7;
		private Label label8;
		private Label label9;
		private Label label10;
		private Label label11;
		private Label label12;
		private Label label13;
		private Label label14;
		private Label label15;
		private Label label16;
		private Label label17;
		private Label label18;
		public Label PAllTokens;
		public Label PInTokens;
		public Label POutTokens;
		public Label DAllTokens;
		public Label DInTokens;
		public Label DOutTokens;
		public Label RAllTokens;
		public Label RInTokens;
		public Label ROutTokens;
        public UserControls.QuestionDialog QuestionDialog;
        private UserControls.FlatButton SendButton;
        private Panel ChatButtonPanel;
        private UserControls.FlatButton StopButton;
    }
}
