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
			ChatHistory = new RichTextBox();
			UserInput = new TextBox();
			SendButton = new SimpleAgent.UserControls.FlatButton();
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
			toolStripStatusLabel1 = new ToolStripStatusLabel();
			PlannerChatPanel = new FlowLayoutPanel();
			ContentContainer = new TableLayoutPanel();
			MiddlePanel = new Panel();
			MiddlePanel2 = new TableLayoutPanel();
			ChatPanelContainer = new Panel();
			CoderChatPanel = new FlowLayoutPanel();
			ReviewerChatPanel = new FlowLayoutPanel();
			AgentTabPanel = new TableLayoutPanel();
			PlannerAgentTab = new SimpleAgent.UserControls.AgentTab();
			CoderAgentTab = new SimpleAgent.UserControls.AgentTab();
			ReviewerAgentTab = new SimpleAgent.UserControls.AgentTab();
			TopMenu.SuspendLayout();
			StatusBar.SuspendLayout();
			ContentContainer.SuspendLayout();
			MiddlePanel.SuspendLayout();
			MiddlePanel2.SuspendLayout();
			ChatPanelContainer.SuspendLayout();
			AgentTabPanel.SuspendLayout();
			SuspendLayout();
			// 
			// ChatHistory
			// 
			ChatHistory.BackColor = Color.White;
			ChatHistory.BorderStyle = BorderStyle.None;
			ChatHistory.Location = new Point(935, 3);
			ChatHistory.Name = "ChatHistory";
			ChatHistory.ReadOnly = true;
			ChatHistory.Size = new Size(334, 126);
			ChatHistory.TabIndex = 0;
			ChatHistory.Text = "";
			// 
			// UserInput
			// 
			UserInput.BackColor = Color.WhiteSmoke;
			UserInput.Dock = DockStyle.Fill;
			UserInput.Location = new Point(5, 578);
			UserInput.Margin = new Padding(5, 5, 0, 5);
			UserInput.MaxLength = 0;
			UserInput.Multiline = true;
			UserInput.Name = "UserInput";
			UserInput.Size = new Size(617, 70);
			UserInput.TabIndex = 1;
			UserInput.Text = "帮我用web开发一个笔记软件";
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
			SendButton.Location = new Point(627, 578);
			SendButton.Margin = new Padding(5);
			SendButton.Name = "SendButton";
			SendButton.Size = new Size(50, 70);
			SendButton.TabIndex = 2;
			SendButton.Text = "发送";
			SendButton.UseVisualStyleBackColor = false;
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
			StatusBar.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
			StatusBar.LayoutStyle = ToolStripLayoutStyle.Flow;
			StatusBar.Location = new Point(4, 694);
			StatusBar.Name = "StatusBar";
			StatusBar.Size = new Size(1272, 22);
			StatusBar.SizingGrip = false;
			StatusBar.TabIndex = 4;
			StatusBar.Text = "状态栏";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new Size(131, 17);
			toolStripStatusLabel1.Text = "toolStripStatusLabel1";
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
			PlannerChatPanel.Size = new Size(682, 523);
			PlannerChatPanel.TabIndex = 3;
			PlannerChatPanel.WrapContents = false;
			PlannerChatPanel.SizeChanged += ChatPanel_SizeChanged;
			// 
			// ContentContainer
			// 
			ContentContainer.ColumnCount = 3;
			ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
			ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			ContentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));
			ContentContainer.Controls.Add(MiddlePanel, 1, 0);
			ContentContainer.Controls.Add(ChatHistory, 2, 0);
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
			// MiddlePanel
			// 
			MiddlePanel.Controls.Add(MiddlePanel2);
			MiddlePanel.Dock = DockStyle.Fill;
			MiddlePanel.Location = new Point(250, 0);
			MiddlePanel.Margin = new Padding(0);
			MiddlePanel.Name = "MiddlePanel";
			MiddlePanel.Size = new Size(682, 653);
			MiddlePanel.TabIndex = 4;
			// 
			// MiddlePanel2
			// 
			MiddlePanel2.BackColor = Color.White;
			MiddlePanel2.ColumnCount = 2;
			MiddlePanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			MiddlePanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
			MiddlePanel2.Controls.Add(UserInput, 0, 2);
			MiddlePanel2.Controls.Add(SendButton, 1, 2);
			MiddlePanel2.Controls.Add(ChatPanelContainer, 0, 1);
			MiddlePanel2.Controls.Add(AgentTabPanel, 0, 0);
			MiddlePanel2.Dock = DockStyle.Fill;
			MiddlePanel2.Location = new Point(0, 0);
			MiddlePanel2.Name = "MiddlePanel2";
			MiddlePanel2.RowCount = 3;
			MiddlePanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
			MiddlePanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			MiddlePanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
			MiddlePanel2.Size = new Size(682, 653);
			MiddlePanel2.TabIndex = 4;
			// 
			// ChatPanelContainer
			// 
			MiddlePanel2.SetColumnSpan(ChatPanelContainer, 2);
			ChatPanelContainer.Controls.Add(PlannerChatPanel);
			ChatPanelContainer.Controls.Add(CoderChatPanel);
			ChatPanelContainer.Controls.Add(ReviewerChatPanel);
			ChatPanelContainer.Dock = DockStyle.Fill;
			ChatPanelContainer.Location = new Point(0, 50);
			ChatPanelContainer.Margin = new Padding(0);
			ChatPanelContainer.Name = "ChatPanelContainer";
			ChatPanelContainer.Size = new Size(682, 523);
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
			CoderChatPanel.Size = new Size(682, 523);
			CoderChatPanel.TabIndex = 4;
			CoderChatPanel.Visible = false;
			CoderChatPanel.WrapContents = false;
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
			ReviewerChatPanel.Size = new Size(682, 523);
			ReviewerChatPanel.TabIndex = 5;
			ReviewerChatPanel.Visible = false;
			ReviewerChatPanel.WrapContents = false;
			// 
			// AgentTabPanel
			// 
			AgentTabPanel.ColumnCount = 3;
			MiddlePanel2.SetColumnSpan(AgentTabPanel, 2);
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
			AgentTabPanel.Size = new Size(682, 50);
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
			PlannerAgentTab.Size = new Size(221, 44);
			PlannerAgentTab.TabIndex = 0;
			// 
			// CoderAgentTab
			// 
			CoderAgentTab.AgentName = "编程智能体";
			CoderAgentTab.BackColor = Color.WhiteSmoke;
			CoderAgentTab.Dock = DockStyle.Fill;
			CoderAgentTab.IsSelected = false;
			CoderAgentTab.Location = new Point(229, 4);
			CoderAgentTab.Margin = new Padding(2, 4, 2, 2);
			CoderAgentTab.Name = "CoderAgentTab";
			CoderAgentTab.Size = new Size(223, 44);
			CoderAgentTab.TabIndex = 1;
			// 
			// ReviewerAgentTab
			// 
			ReviewerAgentTab.AgentName = "审查智能体";
			ReviewerAgentTab.BackColor = Color.WhiteSmoke;
			ReviewerAgentTab.Dock = DockStyle.Fill;
			ReviewerAgentTab.IsSelected = false;
			ReviewerAgentTab.Location = new Point(456, 4);
			ReviewerAgentTab.Margin = new Padding(2, 4, 4, 2);
			ReviewerAgentTab.Name = "ReviewerAgentTab";
			ReviewerAgentTab.Size = new Size(222, 44);
			ReviewerAgentTab.TabIndex = 2;
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
			SizeChanged += MainForm_SizeChanged;
			TopMenu.ResumeLayout(false);
			TopMenu.PerformLayout();
			StatusBar.ResumeLayout(false);
			StatusBar.PerformLayout();
			ContentContainer.ResumeLayout(false);
			MiddlePanel.ResumeLayout(false);
			MiddlePanel2.ResumeLayout(false);
			MiddlePanel2.PerformLayout();
			ChatPanelContainer.ResumeLayout(false);
			AgentTabPanel.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private RichTextBox ChatHistory;
		private TextBox UserInput;
		private SimpleAgent.UserControls.FlatButton SendButton;
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
		private ToolStripStatusLabel toolStripStatusLabel1;
		private TableLayoutPanel ContentContainer;
		private Panel MiddlePanel;
		private TableLayoutPanel MiddlePanel2;
		private Panel ChatPanelContainer;
		private TableLayoutPanel AgentTabPanel;
		public FlowLayoutPanel PlannerChatPanel;
		public FlowLayoutPanel ReviewerChatPanel;
		public FlowLayoutPanel CoderChatPanel;
		public UserControls.AgentTab PlannerAgentTab;
		public UserControls.AgentTab CoderAgentTab;
		public UserControls.AgentTab ReviewerAgentTab;
	}
}
