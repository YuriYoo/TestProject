using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.UserControls
{
	[DefaultEvent("ValueChanged")]
	public class FlatTrackBar : Control
	{
		private int _minimum = 0;
		private int _maximum = 100;
		private int _value = 50;
		private Orientation _orientation = Orientation.Horizontal; // 新增方向属性

		private int _trackHeight = 4;
		private Size _thumbSize = new Size(14, 14);
		private Color _trackColor = Color.FromArgb(224, 224, 224);
		private Color _sliderColor = Color.FromArgb(30, 144, 255);

		private bool _isDragging = false;
		private bool _isHoveringThumb = false;
		private Rectangle _thumbRectangle;

		public event EventHandler ValueChanged;

		public FlatTrackBar()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint |
						  ControlStyles.OptimizedDoubleBuffer |
						  ControlStyles.ResizeRedraw |
						  ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
			Size = new Size(150, 25);
		}

		#region 属性

		[Category("扁平化设置"), Description("滑动条的方向")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Orientation Orientation
		{
			get => _orientation;
			set
			{
				if (_orientation != value)
				{
					_orientation = value;
					// 切换方向时，交换长宽通常更符合操作直觉
					this.Size = new Size(this.Height, this.Width);
					Invalidate();
				}
			}
		}

		[Category("扁平化设置")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
		{
			get => _minimum;
			set { _minimum = value; if (_value < _minimum) Value = _minimum; Invalidate(); }
		}

		[Category("扁平化设置")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
		{
			get => _maximum;
			set { _maximum = value; if (_value > _maximum) Value = _maximum; Invalidate(); }
		}

		[Category("扁平化设置")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
		{
			get => _value;
			set
			{
				int newValue = Math.Max(_minimum, Math.Min(_maximum, value));
				if (_value != newValue)
				{
					_value = newValue;
					ValueChanged?.Invoke(this, EventArgs.Empty);
					Invalidate();
				}
			}
		}

		[Category("扁平化设置")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SliderColor
		{
			get => _sliderColor;
			set { _sliderColor = value; Invalidate(); }
		}

		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			int padding = _thumbSize.Width / 2;
			float ratio = (float)(Value - Minimum) / (Maximum - Minimum);

			if (Orientation == Orientation.Horizontal)
			{
				// --- 横向绘制逻辑 ---
				Rectangle drawRect = new Rectangle(padding, 0, this.Width - _thumbSize.Width, this.Height);
				int thumbX = drawRect.X + (int)(ratio * drawRect.Width);
				int trackY = (this.Height - _trackHeight) / 2;

				// 背景轨道
				DrawTrack(g, new Rectangle(drawRect.X, trackY, drawRect.Width, _trackHeight));
				// 已滑动部分
				DrawSlider(g, new Rectangle(drawRect.X, trackY, thumbX - drawRect.X, _trackHeight));
				// 滑块
				_thumbRectangle = new Rectangle(thumbX - padding, (this.Height - _thumbSize.Height) / 2, _thumbSize.Width, _thumbSize.Height);
			}
			else
			{
				// --- 纵向绘制逻辑 ---
				Rectangle drawRect = new Rectangle(0, padding, this.Width, this.Height - _thumbSize.Height);
				// 纵向通常底部为最小，所以坐标需要反转 (1 - ratio)
				int thumbY = drawRect.Y + (int)((1 - ratio) * drawRect.Height);
				int trackX = (this.Width - _trackHeight) / 2;

				// 背景轨道
				DrawTrack(g, new Rectangle(trackX, drawRect.Y, _trackHeight, drawRect.Height));
				// 已滑动部分 (从底部向上画)
				int sliderHeight = drawRect.Bottom - thumbY;
				DrawSlider(g, new Rectangle(trackX, thumbY, _trackHeight, sliderHeight));
				// 滑块
				_thumbRectangle = new Rectangle((this.Width - _thumbSize.Width) / 2, thumbY - padding, _thumbSize.Width, _thumbSize.Height);
			}

			// 绘制滑块
			Color thumbColor = _isDragging ? GetDarkerColor(_sliderColor, 0.8f) : (_isHoveringThumb ? GetDarkerColor(_sliderColor, 0.9f) : _sliderColor);
			using (SolidBrush brush = new SolidBrush(thumbColor))
			{
				//g.FillEllipse(brush, _thumbRectangle);
				g.FillRectangle(brush, _thumbRectangle);
			}
		}

		private void DrawTrack(Graphics g, Rectangle rect)
		{
			using (GraphicsPath path = GetRoundRectanglePath(rect, _trackHeight))
			using (SolidBrush brush = new SolidBrush(_trackColor))
				g.FillPath(brush, path);
		}

		private void DrawSlider(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) return;
			using (GraphicsPath path = GetRoundRectanglePath(rect, _trackHeight))
			using (SolidBrush brush = new SolidBrush(_sliderColor))
				g.FillPath(brush, path);
		}

		#region 鼠标交互修改

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_isDragging = true;
				UpdateValueFromMouse(e.Location);
				this.Capture = true;
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_isHoveringThumb = _thumbRectangle.Contains(e.Location);
			if (_isDragging) UpdateValueFromMouse(e.Location);
			Invalidate();
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isDragging = false;
			this.Capture = false;
			Invalidate();
			base.OnMouseUp(e);
		}

		private void UpdateValueFromMouse(Point mousePos)
		{
			float ratio = 0;
			if (Orientation == Orientation.Horizontal)
			{
				int trackWidth = this.Width - _thumbSize.Width;
				ratio = (float)(mousePos.X - _thumbSize.Width / 2) / trackWidth;
			}
			else
			{
				int trackHeight = this.Height - _thumbSize.Height;
				// 纵向反转：鼠标越靠下(Y越大)，Value越小
				ratio = 1 - (float)(mousePos.Y - _thumbSize.Height / 2) / trackHeight;
			}

			Value = Minimum + (int)(Math.Max(0, Math.Min(1, ratio)) * (Maximum - Minimum));
		}

		#endregion

		// 辅助方法 (复用之前的)
		private GraphicsPath GetRoundRectanglePath(Rectangle rect, int radius)
		{
			GraphicsPath path = new GraphicsPath();
			int d = radius;
			if (d <= 0) { path.AddRectangle(rect); return path; }
			path.AddArc(rect.X, rect.Y, d, d, 180, 90);
			path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
			path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
			path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
			path.CloseFigure();
			return path;
		}

		private Color GetDarkerColor(Color color, float factor) =>
			Color.FromArgb(color.A, (int)(color.R * factor), (int)(color.G * factor), (int)(color.B * factor));
	}
}
