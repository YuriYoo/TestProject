using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace SimpleAgent.UserControls
{
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(Button))]
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
	public class ToolStripButton : ToolStripControlHost
	{
		private Button host;

		[Category("外观")]
		[Description("鼠标经过时的背景颜色")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color MouseOverBackColor
		{
			get => mouseOverBackColor;
			set
			{
				mouseOverBackColor = value;
				host.FlatAppearance.MouseOverBackColor = value;
			}
		}
		private Color mouseOverBackColor = Color.White;

		[Category("外观")]
		[Description("鼠标按下时的背景颜色")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color MouseDownBackColor
		{
			get => mouseDownBackColor;
			set
			{
				mouseDownBackColor = value;
				host.FlatAppearance.MouseDownBackColor = value;
			}
		}
		private Color mouseDownBackColor = Color.Gainsboro;

		public event EventHandler<MouseEventArgs> MouseClick;

		public ToolStripButton() : base(CreateControlInstance())
		{
			host = (Button)Control;

			Text = host.Text;
			AutoSize = host.AutoSize;
			Size = host.Size;
			Margin = host.Margin;

			host.FlatAppearance.BorderSize = 0;
			host.FlatAppearance.MouseOverBackColor = mouseOverBackColor;
			host.FlatAppearance.MouseDownBackColor = mouseDownBackColor;

			host.MouseClick += HostMouseClick;
		}

		private static Button CreateControlInstance()
		{
			Button control = new()
			{
				Text = " ",
				AutoSize = false,
				Size = new(32, 32),
				BackColor = Color.Transparent,
				FlatStyle = FlatStyle.Flat,
				Margin = new Padding(0),
				TabStop = false,
			};
			return control;
		}

		/// <summary>
		/// 鼠标点击内部事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HostMouseClick(object? sender, MouseEventArgs e)
		{
			// 解决点击按钮后按钮持续获得焦点, 使得MenuStrip中的项无法获得焦点, 以至于不显示鼠标悬停效果
			(sender as Button)?.Parent?.Focus();

			// 执行鼠标点击事件
			MouseClick?.Invoke(this, e);
		}
	}
}
