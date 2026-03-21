namespace SimpleAgent.UserControls
{
	partial class ChatMessageItem
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
			ChatName = new Label();
			ChatMessage = new FlatRichTextBox();
			SuspendLayout();
			// 
			// ChatName
			// 
			ChatName.BackColor = Color.AliceBlue;
			ChatName.Dock = DockStyle.Top;
			ChatName.ForeColor = Color.SteelBlue;
			ChatName.Location = new Point(5, 5);
			ChatName.Margin = new Padding(0);
			ChatName.Name = "ChatName";
			ChatName.Size = new Size(362, 17);
			ChatName.TabIndex = 0;
			ChatName.Text = "我";
			// 
			// ChatMessage
			// 
			ChatMessage.AcceptsTab = true;
			ChatMessage.BackColor = Color.White;
			ChatMessage.BorderStyle = BorderStyle.None;
			ChatMessage.Dock = DockStyle.Fill;
			ChatMessage.Location = new Point(5, 22);
			ChatMessage.Margin = new Padding(0);
			ChatMessage.Name = "ChatMessage";
			ChatMessage.ReadOnly = true;
			ChatMessage.ScrollBars = RichTextBoxScrollBars.None;
			ChatMessage.Size = new Size(362, 23);
			ChatMessage.TabIndex = 1;
			ChatMessage.Text = "";
			// 
			// ChatMessageItem
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.White;
			Controls.Add(ChatMessage);
			Controls.Add(ChatName);
			DoubleBuffered = true;
			Margin = new Padding(0);
			MinimumSize = new Size(120, 50);
			Name = "ChatMessageItem";
			Padding = new Padding(5);
			Size = new Size(372, 50);
			ResumeLayout(false);
		}

		#endregion

		private Label ChatName;
		public FlatRichTextBox ChatMessage;
	}
}
