namespace SimpleAgent.UserControls
{
	partial class QuestionDialog
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
            btnConfirm = new FlatButton();
            btnCancel = new FlatButton();
            QuestionPanel = new TableLayoutPanel();
            QuestionText = new Label();
            OptionPanel = new Panel();
            QuestionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // btnConfirm
            // 
            btnConfirm.BackColor = SystemColors.MenuHighlight;
            btnConfirm.Dock = DockStyle.Fill;
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.FlatAppearance.MouseDownBackColor = SystemColors.HotTrack;
            btnConfirm.FlatAppearance.MouseOverBackColor = Color.DodgerBlue;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(3, 51);
            btnConfirm.MinimumSize = new Size(100, 34);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(104, 34);
            btnConfirm.TabIndex = 2;
            btnConfirm.Text = "确认";
            btnConfirm.UseVisualStyleBackColor = false;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(239, 83, 80);
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 0, 0);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.Red;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft YaHei UI", 9F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(113, 51);
            btnCancel.MinimumSize = new Size(100, 34);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(104, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // QuestionPanel
            // 
            QuestionPanel.AutoSize = true;
            QuestionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            QuestionPanel.BackColor = Color.WhiteSmoke;
            QuestionPanel.ColumnCount = 2;
            QuestionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            QuestionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            QuestionPanel.Controls.Add(btnCancel, 1, 2);
            QuestionPanel.Controls.Add(btnConfirm, 0, 2);
            QuestionPanel.Controls.Add(QuestionText, 0, 0);
            QuestionPanel.Controls.Add(OptionPanel, 0, 1);
            QuestionPanel.Dock = DockStyle.Bottom;
            QuestionPanel.Location = new Point(0, 0);
            QuestionPanel.Margin = new Padding(4);
            QuestionPanel.MinimumSize = new Size(200, 50);
            QuestionPanel.Name = "QuestionPanel";
            QuestionPanel.RowCount = 3;
            QuestionPanel.RowStyles.Add(new RowStyle());
            QuestionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            QuestionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            QuestionPanel.Size = new Size(220, 88);
            QuestionPanel.TabIndex = 5;
            // 
            // lblQuestion
            // 
            QuestionText.AutoSize = true;
            QuestionText.BackColor = Color.White;
            QuestionPanel.SetColumnSpan(QuestionText, 2);
            QuestionText.Dock = DockStyle.Fill;
            QuestionText.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            QuestionText.ForeColor = SystemColors.HotTrack;
            QuestionText.Location = new Point(4, 4);
            QuestionText.Margin = new Padding(4);
            QuestionText.MinimumSize = new Size(200, 40);
            QuestionText.Name = "lblQuestion";
            QuestionText.Padding = new Padding(10);
            QuestionText.Size = new Size(212, 40);
            QuestionText.TabIndex = 0;
            QuestionText.Text = "问题";
            // 
            // OptionPanel
            // 
            OptionPanel.AutoSize = true;
            OptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            QuestionPanel.SetColumnSpan(OptionPanel, 2);
            OptionPanel.Dock = DockStyle.Fill;
            OptionPanel.Location = new Point(0, 48);
            OptionPanel.Margin = new Padding(0);
            OptionPanel.MinimumSize = new Size(200, 0);
            OptionPanel.Name = "OptionPanel";
            OptionPanel.Size = new Size(220, 1);
            OptionPanel.TabIndex = 0;
            // 
            // QuestionDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(QuestionPanel);
            Margin = new Padding(0);
            MinimumSize = new Size(220, 0);
            Name = "QuestionDialog";
            Size = new Size(220, 88);
            QuestionPanel.ResumeLayout(false);
            QuestionPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private FlatButton btnConfirm;
		private FlatButton btnCancel;
        private TableLayoutPanel QuestionPanel;
        private Panel OptionPanel;
        private Label QuestionText;
    }
}
