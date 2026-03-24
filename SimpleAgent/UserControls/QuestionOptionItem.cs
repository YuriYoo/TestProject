using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace SimpleAgent.UserControls
{
    public partial class QuestionOptionItem : UserControl
    {
        private string _text = string.Empty;
        private Color _normalBackColor = Color.White;
        private Color _hoverBackColor = Color.FromArgb(230, 247, 255);
        private Color _selectedBackColor = Color.FromArgb(230, 247, 255);
        private Color _selectedBorderColor = Color.FromArgb(0, 120, 215);
        private Color _normalBorderColor = Color.White;
        private bool _isHovered = false;


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Invalidate();
                    SelectedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private bool _isSelected = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string OptionText
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
            }
        }

        public event EventHandler? SelectedChanged;
        public event EventHandler? Clicked;

        public QuestionOptionItem()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        public QuestionOptionItem(string text) : this()
        {
            _text = text;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var padding = 10;
            var paddingAll = padding * 2;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color backColor;
            Color borderColor;

            if (_isSelected)
            {
                backColor = _selectedBackColor;
                borderColor = _selectedBorderColor;
            }
            else if (_isHovered)
            {
                backColor = _hoverBackColor;
                borderColor = _normalBorderColor;
            }
            else
            {
                backColor = _normalBackColor;
                borderColor = _normalBorderColor;
            }

            // 绘制背景
            using (var backBrush = new SolidBrush(backColor))
            {
                g.FillRectangle(backBrush, DisplayRectangle);
            }

            // 绘制边框
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.DrawRectangle(borderPen, new(1, 1, DisplayRectangle.Width - 2, DisplayRectangle.Height - 2));
            }

            // 绘制文字
            if (!string.IsNullOrEmpty(_text))
            {
                using var textBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
                var font = new Font("Microsoft YaHei UI", 9F);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.Word,
                };

                // 由于计算精度的问题, 宽度只减 padding 绘制的效果好一些
                var textRect = new Rectangle(padding, padding, DisplayRectangle.Width - padding, DisplayRectangle.Height - paddingAll);
                var textSize = g.MeasureString(_text, font, textRect.Width, format);
                var requiredHeight = (int)textSize.Height + paddingAll;
                if (Height != requiredHeight && requiredHeight >= MinimumSize.Height)
                {
                    Height = requiredHeight;
                }

                g.DrawString(_text, font, textBrush, textRect, format);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
