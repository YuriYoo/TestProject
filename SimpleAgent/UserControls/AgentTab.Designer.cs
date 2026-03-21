namespace SimpleAgent.UserControls
{
	partial class AgentTab
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			AgentLabel = new Label();
			StatusLabel = new Label();
			SuspendLayout();
			// 
			// AgentLabel
			// 
			AgentLabel.Dock = DockStyle.Fill;
			AgentLabel.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
			AgentLabel.Location = new Point(0, 0);
			AgentLabel.Margin = new Padding(0);
			AgentLabel.Name = "AgentLabel";
			AgentLabel.Padding = new Padding(0, 4, 0, 0);
			AgentLabel.Size = new Size(200, 30);
			AgentLabel.TabIndex = 0;
			AgentLabel.Text = "规划智能体";
			AgentLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// StatusLabel
			// 
			StatusLabel.Dock = DockStyle.Bottom;
			StatusLabel.ForeColor = Color.DarkGray;
			StatusLabel.Location = new Point(0, 30);
			StatusLabel.Name = "StatusLabel";
			StatusLabel.Size = new Size(200, 20);
			StatusLabel.TabIndex = 1;
			StatusLabel.Text = "待机中";
			StatusLabel.TextAlign = ContentAlignment.TopCenter;
			// 
			// AgentTab
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.WhiteSmoke;
			Controls.Add(AgentLabel);
			Controls.Add(StatusLabel);
			Margin = new Padding(2);
			Name = "AgentTab";
			Size = new Size(200, 50);
			ResumeLayout(false);
		}

		#endregion

		private Label AgentLabel;
		private Label StatusLabel;
	}
}
