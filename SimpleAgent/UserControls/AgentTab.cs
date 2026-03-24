using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAgent.UserControls
{
	public partial class AgentTab : UserControl
	{
		[Category("外观")]
		[Description("智能体名称")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string AgentName
		{
			get => AgentLabel.Text;
			set
			{
				AgentLabel.Text = value;
			}
		}

		[Category("外观")]
		[Description("是否选中")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				if (value)
				{
					BackColor = Color.White;
				}
				else
				{
					BackColor = Color.WhiteSmoke;
				}
			}
		}
		private bool isSelected = false;

		public new event EventHandler? Click;

		public AgentTab()
		{
			InitializeComponent();
			AgentLabel.Click += AgentLabel_Click;
			StatusLabel.Click += StatusLabel_Click;
		}

		private void StatusLabel_Click(object? sender, EventArgs e)
		{
			Click?.Invoke(this, EventArgs.Empty);
		}

		private void AgentLabel_Click(object? sender, EventArgs e)
		{
			Click?.Invoke(this, EventArgs.Empty);
		}

		public void PerformClick()
		{
			Click?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// 设置是否正在运行
		/// </summary>
		/// <param name="isRunning"></param>
		public void SetRunning(bool isRunning)
		{
			if (isRunning)
			{
				StatusLabel.Text = "运行中";
				StatusLabel.ForeColor = Color.LimeGreen;
			}
			else
			{
				StatusLabel.Text = "待机中";
				StatusLabel.ForeColor = Color.DarkGray;
			}
		}
	}
}
