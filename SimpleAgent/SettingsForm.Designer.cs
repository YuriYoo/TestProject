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
            numMaxOutTokens = new NumericUpDown();
            lblMaxOutTokens = new Label();
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
            ButtonPanel = new Panel();
            MainPanel.SuspendLayout();
            ContentPanel.SuspendLayout();
            AIServiceGroup.SuspendLayout();
            AIServiceLayout.SuspendLayout();
            AgentGroup.SuspendLayout();
            AgentLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxOutTokens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTruncation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTimeout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTerminal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTimeout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxThinkingRounds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSubMaxThinkingRounds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxTokens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numContextCompressionThreshold).BeginInit();
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
            ContentPanel.Dock = DockStyle.Fill;
            ContentPanel.Location = new Point(13, 13);
            ContentPanel.Name = "ContentPanel";
            ContentPanel.Size = new Size(574, 624);
            ContentPanel.TabIndex = 1;
            // 
            // AIServiceGroup
            // 
            AIServiceGroup.Controls.Add(AIServiceLayout);
            AIServiceGroup.Dock = DockStyle.Top;
            AIServiceGroup.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            AIServiceGroup.Location = new Point(0, 324);
            AIServiceGroup.Margin = new Padding(0);
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
            AIServiceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            AIServiceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            AIServiceLayout.Controls.Add(lblApiKey, 0, 0);
            AIServiceLayout.Controls.Add(txtApiKey, 1, 0);
            AIServiceLayout.Controls.Add(lblApiBaseUrl, 0, 1);
            AIServiceLayout.Controls.Add(txtApiBaseUrl, 1, 1);
            AIServiceLayout.Controls.Add(lblModelId, 0, 2);
            AIServiceLayout.Controls.Add(txtModelId, 1, 2);
            AIServiceLayout.Dock = DockStyle.Fill;
            AIServiceLayout.Location = new Point(8, 24);
            AIServiceLayout.Margin = new Padding(0);
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
            lblApiKey.AutoSize = true;
            lblApiKey.Dock = DockStyle.Fill;
            lblApiKey.Font = new Font("Microsoft YaHei UI", 9F);
            lblApiKey.Location = new Point(5, 5);
            lblApiKey.Margin = new Padding(0);
            lblApiKey.Name = "lblApiKey";
            lblApiKey.Size = new Size(160, 32);
            lblApiKey.TabIndex = 0;
            lblApiKey.Text = "API 密钥:";
            lblApiKey.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtApiKey
            // 
            txtApiKey.Dock = DockStyle.Fill;
            txtApiKey.Font = new Font("Microsoft YaHei UI", 9F);
            txtApiKey.Location = new Point(168, 8);
            txtApiKey.Name = "txtApiKey";
            txtApiKey.PasswordChar = '●';
            txtApiKey.Size = new Size(382, 23);
            txtApiKey.TabIndex = 1;
            // 
            // lblApiBaseUrl
            // 
            lblApiBaseUrl.AutoSize = true;
            lblApiBaseUrl.Dock = DockStyle.Fill;
            lblApiBaseUrl.Font = new Font("Microsoft YaHei UI", 9F);
            lblApiBaseUrl.Location = new Point(5, 37);
            lblApiBaseUrl.Margin = new Padding(0);
            lblApiBaseUrl.Name = "lblApiBaseUrl";
            lblApiBaseUrl.Size = new Size(160, 32);
            lblApiBaseUrl.TabIndex = 2;
            lblApiBaseUrl.Text = "API 基础URL:";
            lblApiBaseUrl.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtApiBaseUrl
            // 
            txtApiBaseUrl.Dock = DockStyle.Fill;
            txtApiBaseUrl.Font = new Font("Microsoft YaHei UI", 9F);
            txtApiBaseUrl.Location = new Point(168, 40);
            txtApiBaseUrl.Name = "txtApiBaseUrl";
            txtApiBaseUrl.Size = new Size(382, 23);
            txtApiBaseUrl.TabIndex = 3;
            // 
            // lblModelId
            // 
            lblModelId.AutoSize = true;
            lblModelId.Dock = DockStyle.Fill;
            lblModelId.Font = new Font("Microsoft YaHei UI", 9F);
            lblModelId.Location = new Point(5, 69);
            lblModelId.Margin = new Padding(0);
            lblModelId.Name = "lblModelId";
            lblModelId.Size = new Size(160, 34);
            lblModelId.TabIndex = 4;
            lblModelId.Text = "模型标识符:";
            lblModelId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtModelId
            // 
            txtModelId.Dock = DockStyle.Fill;
            txtModelId.Font = new Font("Microsoft YaHei UI", 9F);
            txtModelId.Location = new Point(168, 72);
            txtModelId.Name = "txtModelId";
            txtModelId.Size = new Size(382, 23);
            txtModelId.TabIndex = 5;
            // 
            // AgentGroup
            // 
            AgentGroup.Controls.Add(AgentLayout);
            AgentGroup.Dock = DockStyle.Top;
            AgentGroup.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            AgentGroup.Location = new Point(0, 0);
            AgentGroup.Margin = new Padding(0);
            AgentGroup.Name = "AgentGroup";
            AgentGroup.Padding = new Padding(8);
            AgentGroup.Size = new Size(574, 324);
            AgentGroup.TabIndex = 1;
            AgentGroup.TabStop = false;
            AgentGroup.Text = "智能体配置";
            // 
            // AgentLayout
            // 
            AgentLayout.ColumnCount = 2;
            AgentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            AgentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            AgentLayout.Controls.Add(numMaxOutTokens, 1, 7);
            AgentLayout.Controls.Add(lblMaxOutTokens, 0, 7);
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
            AgentLayout.Controls.Add(lblContextCompressionThreshold, 0, 8);
            AgentLayout.Controls.Add(numContextCompressionThreshold, 1, 8);
            AgentLayout.Dock = DockStyle.Fill;
            AgentLayout.Location = new Point(8, 24);
            AgentLayout.Margin = new Padding(0);
            AgentLayout.MinimumSize = new Size(0, 266);
            AgentLayout.Name = "AgentLayout";
            AgentLayout.Padding = new Padding(5);
            AgentLayout.RowCount = 9;
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            AgentLayout.Size = new Size(558, 292);
            AgentLayout.TabIndex = 0;
            // 
            // numMaxOutTokens
            // 
            numMaxOutTokens.Dock = DockStyle.Fill;
            numMaxOutTokens.Font = new Font("Microsoft YaHei UI", 9F);
            numMaxOutTokens.Location = new Point(168, 232);
            numMaxOutTokens.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numMaxOutTokens.Name = "numMaxOutTokens";
            numMaxOutTokens.Size = new Size(382, 23);
            numMaxOutTokens.TabIndex = 18;
            numMaxOutTokens.ThousandsSeparator = true;
            // 
            // lblMaxOutTokens
            // 
            lblMaxOutTokens.AutoSize = true;
            lblMaxOutTokens.Dock = DockStyle.Fill;
            lblMaxOutTokens.Font = new Font("Microsoft YaHei UI", 9F);
            lblMaxOutTokens.Location = new Point(5, 229);
            lblMaxOutTokens.Margin = new Padding(0);
            lblMaxOutTokens.Name = "lblMaxOutTokens";
            lblMaxOutTokens.Size = new Size(160, 32);
            lblMaxOutTokens.TabIndex = 17;
            lblMaxOutTokens.Text = "单次输出最大Token:";
            lblMaxOutTokens.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTerminalTruncation
            // 
            lblTerminalTruncation.AutoSize = true;
            lblTerminalTruncation.Dock = DockStyle.Fill;
            lblTerminalTruncation.Font = new Font("Microsoft YaHei UI", 9F);
            lblTerminalTruncation.Location = new Point(5, 5);
            lblTerminalTruncation.Margin = new Padding(0);
            lblTerminalTruncation.Name = "lblTerminalTruncation";
            lblTerminalTruncation.Size = new Size(160, 32);
            lblTerminalTruncation.TabIndex = 0;
            lblTerminalTruncation.Text = "命令行输出截断:";
            lblTerminalTruncation.TextAlign = ContentAlignment.MiddleLeft;
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
            lblTerminalTimeout.AutoSize = true;
            lblTerminalTimeout.Dock = DockStyle.Fill;
            lblTerminalTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            lblTerminalTimeout.Location = new Point(5, 37);
            lblTerminalTimeout.Margin = new Padding(0);
            lblTerminalTimeout.Name = "lblTerminalTimeout";
            lblTerminalTimeout.Size = new Size(160, 32);
            lblTerminalTimeout.TabIndex = 2;
            lblTerminalTimeout.Text = "命令行超时时间(ms):";
            lblTerminalTimeout.TextAlign = ContentAlignment.MiddleLeft;
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
            lblHttpTerminal.AutoSize = true;
            lblHttpTerminal.Dock = DockStyle.Fill;
            lblHttpTerminal.Font = new Font("Microsoft YaHei UI", 9F);
            lblHttpTerminal.Location = new Point(5, 69);
            lblHttpTerminal.Margin = new Padding(0);
            lblHttpTerminal.Name = "lblHttpTerminal";
            lblHttpTerminal.Size = new Size(160, 32);
            lblHttpTerminal.TabIndex = 4;
            lblHttpTerminal.Text = "HTTP响应截断:";
            lblHttpTerminal.TextAlign = ContentAlignment.MiddleLeft;
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
            lblHttpTimeout.AutoSize = true;
            lblHttpTimeout.Dock = DockStyle.Fill;
            lblHttpTimeout.Font = new Font("Microsoft YaHei UI", 9F);
            lblHttpTimeout.Location = new Point(5, 101);
            lblHttpTimeout.Margin = new Padding(0);
            lblHttpTimeout.Name = "lblHttpTimeout";
            lblHttpTimeout.Size = new Size(160, 32);
            lblHttpTimeout.TabIndex = 6;
            lblHttpTimeout.Text = "HTTP请求超时(ms):";
            lblHttpTimeout.TextAlign = ContentAlignment.MiddleLeft;
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
            lblMaxThinkingRounds.AutoSize = true;
            lblMaxThinkingRounds.Dock = DockStyle.Fill;
            lblMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            lblMaxThinkingRounds.Location = new Point(5, 133);
            lblMaxThinkingRounds.Margin = new Padding(0);
            lblMaxThinkingRounds.Name = "lblMaxThinkingRounds";
            lblMaxThinkingRounds.Size = new Size(160, 32);
            lblMaxThinkingRounds.TabIndex = 8;
            lblMaxThinkingRounds.Text = "模型最大思考轮次:";
            lblMaxThinkingRounds.TextAlign = ContentAlignment.MiddleLeft;
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
            lblSubMaxThinkingRounds.AutoSize = true;
            lblSubMaxThinkingRounds.Dock = DockStyle.Fill;
            lblSubMaxThinkingRounds.Font = new Font("Microsoft YaHei UI", 9F);
            lblSubMaxThinkingRounds.Location = new Point(5, 165);
            lblSubMaxThinkingRounds.Margin = new Padding(0);
            lblSubMaxThinkingRounds.Name = "lblSubMaxThinkingRounds";
            lblSubMaxThinkingRounds.Size = new Size(160, 32);
            lblSubMaxThinkingRounds.TabIndex = 10;
            lblSubMaxThinkingRounds.Text = "子代理最大思考轮次:";
            lblSubMaxThinkingRounds.TextAlign = ContentAlignment.MiddleLeft;
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
            lblMaxTokens.AutoSize = true;
            lblMaxTokens.Dock = DockStyle.Fill;
            lblMaxTokens.Font = new Font("Microsoft YaHei UI", 9F);
            lblMaxTokens.Location = new Point(5, 197);
            lblMaxTokens.Margin = new Padding(0);
            lblMaxTokens.Name = "lblMaxTokens";
            lblMaxTokens.Size = new Size(160, 32);
            lblMaxTokens.TabIndex = 12;
            lblMaxTokens.Text = "模型最大上下文:";
            lblMaxTokens.TextAlign = ContentAlignment.MiddleLeft;
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
            lblContextCompressionThreshold.AutoSize = true;
            lblContextCompressionThreshold.Dock = DockStyle.Fill;
            lblContextCompressionThreshold.Font = new Font("Microsoft YaHei UI", 9F);
            lblContextCompressionThreshold.Location = new Point(5, 261);
            lblContextCompressionThreshold.Margin = new Padding(0);
            lblContextCompressionThreshold.Name = "lblContextCompressionThreshold";
            lblContextCompressionThreshold.Size = new Size(160, 32);
            lblContextCompressionThreshold.TabIndex = 16;
            lblContextCompressionThreshold.Text = "上下文压缩阈值(%):";
            lblContextCompressionThreshold.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numContextCompressionThreshold
            // 
            numContextCompressionThreshold.Dock = DockStyle.Fill;
            numContextCompressionThreshold.Font = new Font("Microsoft YaHei UI", 9F);
            numContextCompressionThreshold.Location = new Point(168, 264);
            numContextCompressionThreshold.Name = "numContextCompressionThreshold";
            numContextCompressionThreshold.Size = new Size(382, 23);
            numContextCompressionThreshold.TabIndex = 15;
            // 
            // ButtonPanel
            // 
            ButtonPanel.AutoSize = true;
            ButtonPanel.BackColor = Color.WhiteSmoke;
            ButtonPanel.Dock = DockStyle.Fill;
            ButtonPanel.Location = new Point(10, 640);
            ButtonPanel.Margin = new Padding(0);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new Size(580, 50);
            ButtonPanel.TabIndex = 0;
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
            MainPanel.PerformLayout();
            ContentPanel.ResumeLayout(false);
            AIServiceGroup.ResumeLayout(false);
            AIServiceLayout.ResumeLayout(false);
            AIServiceLayout.PerformLayout();
            AgentGroup.ResumeLayout(false);
            AgentLayout.ResumeLayout(false);
            AgentLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxOutTokens).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTruncation).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTerminalTimeout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTerminal).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHttpTimeout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxThinkingRounds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSubMaxThinkingRounds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxTokens).EndInit();
            ((System.ComponentModel.ISupportInitialize)numContextCompressionThreshold).EndInit();
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
        private NumericUpDown numContextCompressionThreshold;
        private UserControls.FlatButton btnBrowseWorkingDir;
        private Label lblMaxOutTokens;
        private Label lblContextCompressionThreshold;
        private NumericUpDown numMaxOutTokens;
    }
}
