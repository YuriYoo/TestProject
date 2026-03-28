namespace SimpleAgent
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            MainPanel = new TableLayoutPanel();
            ContentPanel = new Panel();
            AIServiceGroup = new GroupBox();
            AIServiceLayout = new TableLayoutPanel();
            lblApiKey = new Label();
            txtApiKey = new TextBox();
            lblApiBaseUrl = new Label();
            txtApiBaseUrl = new TextBox();
            lblModelId = new Label();
            txtModelId = new TextBox();
            AgentGroup = new GroupBox();
            AgentLayout = new TableLayoutPanel();
            lblTerminalTruncation = new Label();
            numTerminalTruncation = new NumericUpDown();
            lblTerminalTimeout = new Label();
            numTerminalTimeout = new NumericUpDown();
            lblHttpTerminal = new Label();
            numHttpTerminal = new NumericUpDown();
            lblHttpTimeout = new Label();
            numHttpTimeout = new NumericUpDown();
            lblMaxThinkingRounds = new Label();
            numMaxThinkingRounds = new NumericUpDown();
            lblSubMaxThinkingRounds = new Label();
            numSubMaxThinkingRounds = new NumericUpDown();
            lblMaxTokens = new Label();
            numMaxTokens = new NumericUpDown();
            lblContextCompressionThreshold = new Label();
            numContextCompressionThreshold = new NumericUpDown();
            WorkingDirGroup = new GroupBox();
            WorkingDirLayout = new TableLayoutPanel();
            lblWorkingDirectory = new Label();
            txtWorkingDirectory = new TextBox();
            btnBrowseWorkingDir = new SimpleAgent.UserControls.FlatButton();
            ButtonPanel = new Panel();
            btnReset = new SimpleAgent.UserControls.FlatButton();
            btnCancel = new SimpleAgent.UserControls.FlatButton();
            btnSave = new SimpleAgent.UserControls.FlatButton();
            MainPanel.SuspendLayout();
            ContentPanel.SuspendLayout();
            AIServiceGroup.SuspendLayout();
            AIServiceLayout.SuspendLayout();
            AgentGroup.SuspendLayout();
            AgentLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTerminalTruncation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTimeout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTerminal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTimeout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxThinkingRounds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSubMaxThinkingRounds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxTokens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numContextCompressionThreshold).BeginInit();
            WorkingDirGroup.SuspendLayout();
            WorkingDirLayout.SuspendLayout();
            ButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.ColumnCount = 1;
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainPanel.Controls.Add(ContentPanel, 0, 0);
            MainPanel.Controls.Add(ButtonPanel, 0, 1);
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Padding = new Padding(10);
            MainPanel.RowCount = 2;
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            MainPanel.Size = new Size(600, 700);
            MainPanel.TabIndex = 0;
            // 
            // ContentPanel
            // 
            ContentPanel.AutoScroll = true;
            ContentPanel.BackColor = Color.WhiteSmoke;
            ContentPanel.Controls.Add(AIServiceGroup);
            ContentPanel.Controls.Add(AgentGroup);
            ContentPanel.Controls.Add(WorkingDirGroup);
            ContentPanel.Dock = DockStyle.Fill;
            ContentPanel.Location = new Point(13, 13);
            ContentPanel.Name = "ContentPanel";
            ContentPanel.Size = new Size(574, 624);
            ContentPanel.TabIndex = 1;
            // 
            // AIServiceGroup
            // 
            AIServiceGroup.BackColor = Color.White;
            AIServiceGroup.Controls.Add(AIServiceLayout);
            AIServiceGroup.Dock = DockStyle.Top;
            AIServiceGroup.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            AIServiceGroup.Location = new Point(0, 400);
            AIServiceGroup.Name = "AIServiceGroup";
            AIServiceGroup.Padding = new Padding(8);
            AIServiceGroup.Size = new Size(574, 140);
            AIServiceGroup.TabIndex = 0;
            AIServiceGroup.TabStop = false;
            AIServiceGroup.Text = "AI 服务配置";
            // 
            // AIServiceLayout
            // 
            AIServiceLayout.ColumnCount = 2;
            AIServiceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            AIServiceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            AIServiceLayout.Controls.Add(lblApiKey, 0, 0);
            AIServiceLayout.Controls.Add(txtApiKey, 1, 0);
            AIServiceLayout.Controls.Add(lblApiBaseUrl, 0, 1);
            AIServiceLayout.Controls.Add(txtApiBaseUrl, 1, 1);
            AIServiceLayout.Controls.Add(lblModelId, 0, 2);
            AIServiceLayout.Controls.Add(txtModelId, 1, 2);
            AIServiceLayout.Dock = DockStyle.Fill;
            AIServiceLayout.Location = new Point(8, 24);
            AIServiceLayout.Name = "AIServiceLayout";
            AIServiceLayout.Padding = new Padding(5);
            AIServiceLayout.RowCount = 3;
            AIServiceLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AIServiceLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AIServiceLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AIServiceLayout.Size = new Size(558, 108);
            AIServiceLayout.TabIndex = 0;
            // 
            // lblApiKey
            // 
            lblApiKey.Anchor = AnchorStyles.Left;
            lblApiKey.AutoSize = true;
            lblApiKey.Font = new Font("Microsoft YaHei UI", 9F);
            lblApiKey.Location = new Point(8, 12);
            lblApiKey.Name = "lblApiKey";
            lblApiKey.Size = new Size(58, 17);
            lblApiKey.TabIndex = 0;
            lblApiKey.Text = "API 密钥:";
            // 
            // txtApiKey
            // 
            txtApiKey.Dock = DockStyle.Fill;
            txtApiKey.Font = new Font("Microsoft YaHei UI", 9F);
            txtApiKey.Location = new Point(128, 8);
            txtApiKey.Name = "txtApiKey";
            txtApiKey.PasswordChar = '●';
            txtApiKey.Size = new Size(422, 23);
            txtApiKey.TabIndex = 1;
            // 
            // lblApiBaseUrl
            // 
            lblApiBaseUrl.Anchor = AnchorStyles.Left;
            lblApiBaseUrl.AutoSize = true;
            lblApiBaseUrl.Font = new Font("Microsoft YaHei UI", 9F);
            lblApiBaseUrl.Location = new Point(8, 44);
            lblApiBaseUrl.Name = "lblApiBaseUrl";
            lblApiBaseUrl.Size = new Size(81, 17);
            lblApiBaseUrl.TabIndex = 2;
            lblApiBaseUrl.Text = "API 基础URL:";
            // 
            // txtApiBaseUrl
            // 
            txtApiBaseUrl.Dock = DockStyle.Fill;
            txtApiBaseUrl.Font = new Font("Microsoft YaHei UI", 9F);
            txtApiBaseUrl.Location = new Point(128, 40);
            txtApiBaseUrl.Name = "txtApiBaseUrl";
            txtApiBaseUrl.Size = new Size(422, 23);
            txtApiBaseUrl.TabIndex = 3;
            // 
            // lblModelId
            // 
            lblModelId.Anchor = AnchorStyles.Left;
            lblModelId.AutoSize = true;
            lblModelId.Font = new Font("Microsoft YaHei UI", 9F);
            lblModelId.Location = new Point(8, 77);
            lblModelId.Name = "lblModelId";
            lblModelId.Size = new Size(71, 17);
            lblModelId.TabIndex = 4;
            lblModelId.Text = "模型标识符:";
            // 
            // txtModelId
            // 
            txtModelId.Dock = DockStyle.Fill;
            txtModelId.Font = new Font("Microsoft YaHei UI", 9F);
            txtModelId.Location = new Point(128, 72);
            txtModelId.Name = "txtModelId";
            txtModelId.Size = new Size(422, 23);
            txtModelId.TabIndex = 5;
            // 
            // AgentGroup
            // 
            AgentGroup.BackColor = Color.White;
            AgentGroup.Controls.Add(AgentLayout);
            AgentGroup.Dock = DockStyle.Top;
            AgentGroup.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            AgentGroup.Location = new Point(0, 80);
            AgentGroup.Name = "AgentGroup";
            AgentGroup.Padding = new Padding(8);
            AgentGroup.Size = new Size(574, 320);
            AgentGroup.TabIndex = 1;
            AgentGroup.TabStop = false;
            AgentGroup.Text = "智能体配置";
            // 
            // AgentLayout
            // 
            AgentLayout.ColumnCount = 2;
            AgentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            AgentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            AgentLayout.Controls.Add(lblTerminalTruncation, 0, 0);
            AgentLayout.Controls.Add(numTerminalTruncation, 1, 0);
            AgentLayout.Controls.Add(lblTerminalTimeout, 0, 1);
            AgentLayout.Controls.Add(numTerminalTimeout, 1, 1);
            AgentLayout.Controls.Add(lblHttpTerminal, 0, 2);
            AgentLayout.Controls.Add(numHttpTerminal, 1, 2);
            AgentLayout.Controls.Add(lblHttpTimeout, 0, 3);
            AgentLayout.Controls.Add(numHttpTimeout, 1, 3);
            AgentLayout.Controls.Add(lblMaxThinkingRounds, 0, 4);
            AgentLayout.Controls.Add(numMaxThinkingRounds, 1, 4);
            AgentLayout.Controls.Add(lblSubMaxThinkingRounds, 0, 5);
            AgentLayout.Controls.Add(numSubMaxThinkingRounds, 1, 5);
            AgentLayout.Controls.Add(lblMaxTokens, 0, 6);
            AgentLayout.Controls.Add(numMaxTokens, 1, 6);
            AgentLayout.Controls.Add(lblContextCompressionThreshold, 0, 7);
            AgentLayout.Controls.Add(numContextCompressionThreshold, 1, 7);
            AgentLayout.Dock = DockStyle.Fill;
            AgentLayout.Location = new Point(8, 24);
            AgentLayout.Name = "AgentLayout";
            AgentLayout.Padding = new Padding(5);
            AgentLayout.RowCount = 8;
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.Size = new Size(558, 288);
            AgentLayout.TabIndex = 0;
            // 
            // lblTerminalTruncation
            // 
            lblTerminalTruncation.Anchor = AnchorStyles.Left;
            lblTerminalTruncation.AutoSize = true;
            lblTerminalTruncation.Font = new Font("Microsoft YaHei UI", 9F);
            lblTerminalTruncation.Location = new Point(8, 12);
            lblTerminalTruncation.Name = "lblTerminalTruncation";
            lblTerminalTruncation.Size = new Size(95, 17);
            lblTerminalTruncation.TabIndex = 0;
            lblTerminalTruncation.Text = "命令行输出截断:";
            // 
            // numTerminalTruncation
            // 
            numTerminalTruncation.Dock = DockStyle.Fill;
            numTerminalTruncation.Font = new Font("Microsoft YaHei UI", 9F);
            numTerminalTruncation.Location = new Point(168, 8);
            numTerminalTruncation.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numTerminalTruncation.Name = "numTerminalTruncation";
            numTerminalTruncation.Size = new Size(382, 23);
            numTerminalTruncation.TabIndex = 1;
            numTerminalTruncation.ThousandsSeparator = true;
            // 
            // lblTerminalTimeout
            // 
            lblTerminalTimeout.Anchor = AnchorStyles.Left;
            lblTerminalTimeout.AutoSize = true;
            lblTerminalTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            lblTerminalTimeout.Location = new Point(8, 44);
            lblTerminalTimeout.Name = "lblTerminalTimeout";
            lblTerminalTimeout.Size = new Size(120, 17);
            lblTerminalTimeout.TabIndex = 2;
            lblTerminalTimeout.Text = "命令行超时时间(ms):";
            // 
            // numTerminalTimeout
            // 
            numTerminalTimeout.Dock = DockStyle.Fill;
            numTerminalTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            numTerminalTimeout.Location = new Point(168, 40);
            numTerminalTimeout.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numTerminalTimeout.Name = "numTerminalTimeout";
            numTerminalTimeout.Size = new Size(382, 23);
            numTerminalTimeout.TabIndex = 3;
            numTerminalTimeout.ThousandsSeparator = true;
            // 
            // lblHttpTerminal
            // 
            lblHttpTerminal.Anchor = AnchorStyles.Left;
            lblHttpTerminal.AutoSize = true;
            lblHttpTerminal.Font = new Font("Microsoft YaHei UI", 9F);
            lblHttpTerminal.Location = new Point(8, 76);
            lblHttpTerminal.Name = "lblHttpTerminal";
            lblHttpTerminal.Size = new Size(89, 17);
            lblHttpTerminal.TabIndex = 4;
            lblHttpTerminal.Text = "HTTP响应截断:";
            // 
            // numHttpTerminal
            // 
            numHttpTerminal.Dock = DockStyle.Fill;
            numHttpTerminal.Font = new Font("Microsoft YaHei UI", 9F);
            numHttpTerminal.Location = new Point(168, 72);
            numHttpTerminal.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numHttpTerminal.Name = "numHttpTerminal";
            numHttpTerminal.Size = new Size(382, 23);
            numHttpTerminal.TabIndex = 5;
            numHttpTerminal.ThousandsSeparator = true;
            // 
            // lblHttpTimeout
            // 
            lblHttpTimeout.Anchor = AnchorStyles.Left;
            lblHttpTimeout.AutoSize = true;
            lblHttpTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            lblHttpTimeout.Location = new Point(8, 108);
            lblHttpTimeout.Name = "lblHttpTimeout";
            lblHttpTimeout.Size = new Size(114, 17);
            lblHttpTimeout.TabIndex = 6;
            lblHttpTimeout.Text = "HTTP请求超时(ms):";
            // 
            // numHttpTimeout
            // 
            numHttpTimeout.Dock = DockStyle.Fill;
            numHttpTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            numHttpTimeout.Location = new Point(168, 104);
            numHttpTimeout.Maximum = new decimal(new int[] { 300000, 0, 0, 0 });
            numHttpTimeout.Name = "numHttpTimeout";
            numHttpTimeout.Size = new Size(382, 23);
            numHttpTimeout.TabIndex = 7;
            numHttpTimeout.ThousandsSeparator = true;
            // 
            // lblMaxThinkingRounds
            // 
            lblMaxThinkingRounds.Anchor = AnchorStyles.Left;
            lblMaxThinkingRounds.AutoSize = true;
            lblMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            lblMaxThinkingRounds.Location = new Point(8, 140);
            lblMaxThinkingRounds.Name = "lblMaxThinkingRounds";
            lblMaxThinkingRounds.Size = new Size(83, 17);
            lblMaxThinkingRounds.TabIndex = 8;
            lblMaxThinkingRounds.Text = "最大思考轮次:";
            // 
            // numMaxThinkingRounds
            // 
            numMaxThinkingRounds.Dock = DockStyle.Fill;
            numMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            numMaxThinkingRounds.Location = new Point(168, 136);
            numMaxThinkingRounds.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numMaxThinkingRounds.Name = "numMaxThinkingRounds";
            numMaxThinkingRounds.Size = new Size(382, 23);
            numMaxThinkingRounds.TabIndex = 9;
            // 
            // lblSubMaxThinkingRounds
            // 
            lblSubMaxThinkingRounds.Anchor = AnchorStyles.Left;
            lblSubMaxThinkingRounds.AutoSize = true;
            lblSubMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            lblSubMaxThinkingRounds.Location = new Point(8, 172);
            lblSubMaxThinkingRounds.Name = "lblSubMaxThinkingRounds";
            lblSubMaxThinkingRounds.Size = new Size(119, 17);
            lblSubMaxThinkingRounds.TabIndex = 10;
            lblSubMaxThinkingRounds.Text = "子代理最大开发轮次:";
            // 
            // numSubMaxThinkingRounds
            // 
            numSubMaxThinkingRounds.Dock = DockStyle.Fill;
            numSubMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            numSubMaxThinkingRounds.Location = new Point(168, 168);
            numSubMaxThinkingRounds.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSubMaxThinkingRounds.Name = "numSubMaxThinkingRounds";
            numSubMaxThinkingRounds.Size = new Size(382, 23);
            numSubMaxThinkingRounds.TabIndex = 11;
            // 
            // lblMaxTokens
            // 
            lblMaxTokens.Anchor = AnchorStyles.Left;
            lblMaxTokens.AutoSize = true;
            lblMaxTokens.Font = new Font("Microsoft YaHei UI", 9F);
            lblMaxTokens.Location = new Point(8, 204);
            lblMaxTokens.Name = "lblMaxTokens";
            lblMaxTokens.Size = new Size(107, 17);
            lblMaxTokens.TabIndex = 12;
            lblMaxTokens.Text = "AI输出最大Token:";
            // 
            // numMaxTokens
            // 
            numMaxTokens.Dock = DockStyle.Fill;
            numMaxTokens.Font = new Font("Microsoft YaHei UI", 9F);
            numMaxTokens.Location = new Point(168, 200);
            numMaxTokens.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numMaxTokens.Name = "numMaxTokens";
            numMaxTokens.Size = new Size(382, 23);
            numMaxTokens.TabIndex = 13;
            numMaxTokens.ThousandsSeparator = true;
            // 
            // lblContextCompressionThreshold
            // 
            lblContextCompressionThreshold.Anchor = AnchorStyles.Left;
            lblContextCompressionThreshold.AutoSize = true;
            lblContextCompressionThreshold.Font = new Font("Microsoft YaHei UI", 9F);
            lblContextCompressionThreshold.Location = new Point(8, 247);
            lblContextCompressionThreshold.Name = "lblContextCompressionThreshold";
            lblContextCompressionThreshold.Size = new Size(114, 17);
            lblContextCompressionThreshold.TabIndex = 14;
            lblContextCompressionThreshold.Text = "上下文压缩阈值(%):";
            // 
            // numContextCompressionThreshold
            // 
            numContextCompressionThreshold.Dock = DockStyle.Fill;
            numContextCompressionThreshold.Font = new Font("Microsoft YaHei UI", 9F);
            numContextCompressionThreshold.Location = new Point(168, 232);
            numContextCompressionThreshold.Name = "numContextCompressionThreshold";
            numContextCompressionThreshold.Size = new Size(382, 23);
            numContextCompressionThreshold.TabIndex = 15;
            // 
            // WorkingDirGroup
            // 
            WorkingDirGroup.BackColor = Color.White;
            WorkingDirGroup.Controls.Add(WorkingDirLayout);
            WorkingDirGroup.Dock = DockStyle.Top;
            WorkingDirGroup.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            WorkingDirGroup.Location = new Point(0, 0);
            WorkingDirGroup.Name = "WorkingDirGroup";
            WorkingDirGroup.Padding = new Padding(8);
            WorkingDirGroup.Size = new Size(574, 80);
            WorkingDirGroup.TabIndex = 2;
            WorkingDirGroup.TabStop = false;
            WorkingDirGroup.Text = "工作目录配置";
            // 
            // WorkingDirLayout
            // 
            WorkingDirLayout.ColumnCount = 3;
            WorkingDirLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            WorkingDirLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            WorkingDirLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            WorkingDirLayout.Controls.Add(lblWorkingDirectory, 0, 0);
            WorkingDirLayout.Controls.Add(txtWorkingDirectory, 1, 0);
            WorkingDirLayout.Controls.Add(btnBrowseWorkingDir, 2, 0);
            WorkingDirLayout.Dock = DockStyle.Fill;
            WorkingDirLayout.Location = new Point(8, 24);
            WorkingDirLayout.Name = "WorkingDirLayout";
            WorkingDirLayout.Padding = new Padding(5);
            WorkingDirLayout.RowCount = 1;
            WorkingDirLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            WorkingDirLayout.Size = new Size(558, 48);
            WorkingDirLayout.TabIndex = 0;
            // 
            // lblWorkingDirectory
            // 
            lblWorkingDirectory.Anchor = AnchorStyles.Left;
            lblWorkingDirectory.AutoSize = true;
            lblWorkingDirectory.Font = new Font("Microsoft YaHei UI", 9F);
            lblWorkingDirectory.Location = new Point(8, 15);
            lblWorkingDirectory.Name = "lblWorkingDirectory";
            lblWorkingDirectory.Size = new Size(59, 17);
            lblWorkingDirectory.TabIndex = 0;
            lblWorkingDirectory.Text = "工作目录:";
            // 
            // txtWorkingDirectory
            // 
            txtWorkingDirectory.Dock = DockStyle.Fill;
            txtWorkingDirectory.Font = new Font("Microsoft YaHei UI", 9F);
            txtWorkingDirectory.Location = new Point(108, 8);
            txtWorkingDirectory.Name = "txtWorkingDirectory";
            txtWorkingDirectory.ReadOnly = true;
            txtWorkingDirectory.Size = new Size(362, 23);
            txtWorkingDirectory.TabIndex = 1;
            // 
            // btnBrowseWorkingDir
            // 
            btnBrowseWorkingDir.BackColor = Color.White;
            btnBrowseWorkingDir.Dock = DockStyle.Fill;
            btnBrowseWorkingDir.FlatStyle = FlatStyle.Flat;
            btnBrowseWorkingDir.Font = new Font("Microsoft YaHei UI", 9F);
            btnBrowseWorkingDir.Location = new Point(476, 8);
            btnBrowseWorkingDir.Name = "btnBrowseWorkingDir";
            btnBrowseWorkingDir.Size = new Size(74, 32);
            btnBrowseWorkingDir.TabIndex = 2;
            btnBrowseWorkingDir.Text = "浏览...";
            btnBrowseWorkingDir.UseVisualStyleBackColor = true;
            btnBrowseWorkingDir.Click += BtnBrowseWorkingDir_Click;
            // 
            // ButtonPanel
            // 
            ButtonPanel.BackColor = Color.WhiteSmoke;
            ButtonPanel.Controls.Add(btnReset);
            ButtonPanel.Controls.Add(btnCancel);
            ButtonPanel.Controls.Add(btnSave);
            ButtonPanel.Dock = DockStyle.Fill;
            ButtonPanel.Location = new Point(13, 643);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new Size(574, 44);
            ButtonPanel.TabIndex = 0;
            // 
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReset.BackColor = Color.Gray;
            btnReset.FlatAppearance.MouseDownBackColor = Color.DimGray;
            btnReset.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Microsoft YaHei UI", 9F);
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(299, 8);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(80, 32);
            btnReset.TabIndex = 2;
            btnReset.Text = "重置";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += BtnReset_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(239, 83, 80);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 0, 0);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.Red;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft YaHei UI", 9F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(479, 8);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackColor = SystemColors.MenuHighlight;
            btnSave.FlatAppearance.MouseDownBackColor = SystemColors.HotTrack;
            btnSave.FlatAppearance.MouseOverBackColor = Color.DodgerBlue;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(389, 8);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(80, 32);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(600, 700);
            Controls.Add(MainPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "设置";
            MainPanel.ResumeLayout(false);
            ContentPanel.ResumeLayout(false);
            AIServiceGroup.ResumeLayout(false);
            AIServiceLayout.ResumeLayout(false);
            AIServiceLayout.PerformLayout();
            AgentGroup.ResumeLayout(false);
            AgentLayout.ResumeLayout(false);
            AgentLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTerminalTruncation).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTimeout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTerminal).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTimeout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxThinkingRounds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSubMaxThinkingRounds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxTokens).EndInit();
            ((System.ComponentModel.ISupportInitialize)numContextCompressionThreshold).EndInit();
            WorkingDirGroup.ResumeLayout(false);
            WorkingDirLayout.ResumeLayout(false);
            WorkingDirLayout.PerformLayout();
            ButtonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel MainPanel;
        private Panel ButtonPanel;
        private UserControls.FlatButton btnSave;
        private UserControls.FlatButton btnCancel;
        private UserControls.FlatButton btnReset;
        private Panel ContentPanel;
        private GroupBox AIServiceGroup;
        private TableLayoutPanel AIServiceLayout;
        private Label lblApiKey;
        private TextBox txtApiKey;
        private Label lblApiBaseUrl;
        private TextBox txtApiBaseUrl;
        private Label lblModelId;
        private TextBox txtModelId;
        private GroupBox AgentGroup;
        private TableLayoutPanel AgentLayout;
        private Label lblTerminalTruncation;
        private NumericUpDown numTerminalTruncation;
        private Label lblTerminalTimeout;
        private NumericUpDown numTerminalTimeout;
        private Label lblHttpTerminal;
        private NumericUpDown numHttpTerminal;
        private Label lblHttpTimeout;
        private NumericUpDown numHttpTimeout;
        private Label lblMaxThinkingRounds;
        private NumericUpDown numMaxThinkingRounds;
        private Label lblSubMaxThinkingRounds;
        private NumericUpDown numSubMaxThinkingRounds;
        private Label lblMaxTokens;
        private NumericUpDown numMaxTokens;
        private Label lblContextCompressionThreshold;
        private NumericUpDown numContextCompressionThreshold;
        private GroupBox WorkingDirGroup;
        private TableLayoutPanel WorkingDirLayout;
        private Label lblWorkingDirectory;
        private TextBox txtWorkingDirectory;
        private UserControls.FlatButton btnBrowseWorkingDir;
    }
}
