using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Renderer
{
	/// <summary>
	/// 自定义渲染器，重写边框绘制方法
	/// </summary>
	internal class MenuStripRenderer : ToolStripProfessionalRenderer
	{
		/// <summary>
		/// 构造函数：可传入自定义颜色表（可选）
		/// </summary>
		//public ToolStripMenuItemRenderer() : base(new ProfessionalColorTable()) { }

		/// <summary>正常时背景画刷</summary>
		private Brush itemBackColorBrush = Brushes.Transparent;

		/// <summary>选中时背景画刷</summary>
		private Brush itemSelectedBackColorBrush = Brushes.Gainsboro;

		/// <summary>按下状态的边框画笔</summary>
		private Pen pressedBorderPen = Pens.Gray;

		/// <summary>下边框线</summary>
		private Pen bottomBorderPen = Pens.Gainsboro;

		/// <summary>
		/// 重写菜单项背景绘制
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item is not ToolStripMenuItem item) return;

			// 正常时背景
			var backBrush = itemBackColorBrush;
			// 选中时背景
			if (item.Selected) backBrush = itemSelectedBackColorBrush;
			// 按下时背景
			else if (item.Pressed) backBrush = (SolidBrush)Brushes.White;

			// 绘制区域
			Rectangle bounds = new(0, 0, item.Width, item.Height);

			// 填充背景
			e.Graphics.FillRectangle(backBrush, bounds);

			// 按下时绘制边框
			if (item.Pressed && !item.IsOnDropDown)
			{
				e.Graphics.DrawRectangle(pressedBorderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			}
		}

		/// <summary>
		/// 重写菜单边框绘制
		/// </summary>
		/// <param name="e"></param>
		/*protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			// 不调用 base.OnRenderToolStripBorder(e); 从而移除默认的全框线
			base.OnRenderToolStripBorder(e);
			// 绘制底部的直线，减去线条宽度偏移以确保线完整显示在控件范围内
			//e.Graphics.DrawLine(bottomBorderPen, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
		}*/

		/// <summary>
		/// 重写菜单项文字绘制
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			// 自定义文字颜色（根据状态）
			e.TextColor = e.Item.Enabled ? Color.Black : Color.Gray;
			// 调用基类方法完成绘制
			base.OnRenderItemText(e);
		}

		/// <summary>
		/// 绘制图像边距背景
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			e.Graphics.FillRectangle(itemBackColorBrush, e.AffectedBounds);
		}

		/// <summary>
		/// 重写图标绘制
		/// </summary>
		/// <param name="e"></param>
		/*protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			if (e.Image == null) return;
			// 图标默认在左，可调整X坐标（如右移5px）
			Rectangle imageRect = e.ImageRectangle;
			imageRect.X += 5; // 左间距增加5px
			e.Graphics.DrawImage(e.Image, imageRect);
		}*/
	}
}
