using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace SimpleAgent.UserControls
{
    public class FlatTreeView : TreeView
    {
        // 定义扁平化主题颜色
        private Color _nodeSelectedBackColor = Color.FromArgb(229, 243, 255); // 选中时的背景色（浅蓝）
        private Color _nodeSelectedForeColor = Color.FromArgb(0, 120, 215);   // 选中时的文字颜色（深蓝）
        private Color _nodeHoverBackColor = Color.FromArgb(243, 243, 243);    // 鼠标悬浮背景色（浅灰）
        private Color _nodeForeColor = Color.FromArgb(51, 51, 51);            // 默认文字颜色（深灰）
        private Color _arrowColor = Color.FromArgb(150, 150, 150);            // 展开/折叠箭头的颜色

        public FlatTreeView()
        {
            // 基础扁平化属性设置
            this.DrawMode = TreeViewDrawMode.OwnerDrawAll; // 开启全自绘
            this.ShowLines = false;                        // 隐藏虚线
            this.ShowPlusMinus = false;                    // 隐藏默认的加减号
            this.FullRowSelect = true;                     // 开启整行选中
            this.ItemHeight = 32;                          // 增加节点高度，更符合现代UI
            this.BorderStyle = BorderStyle.None;           // 移除3D边框
            this.Font = new Font("Microsoft YaHei", 10f);  // 使用更现代的字体
            this.BackColor = Color.White;
            this.HideSelection = false;                    // 失去焦点时依然保持选中高亮
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            // 1. 确定当前节点的状态（是否选中）
            bool isSelected = (e.State & TreeNodeStates.Selected) != 0;

            // 背景色和前景色
            Color backColor = isSelected ? _nodeSelectedBackColor : this.BackColor;
            Color foreColor = isSelected ? _nodeSelectedForeColor : _nodeForeColor;

            // 2. 绘制背景
            using (SolidBrush bgBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            // 3. 计算缩进和图标位置
            int indent = e.Node.Level * 14; // 每一级的缩进像素
            int arrowX = e.Bounds.Left + indent + 10;
            int midY = e.Bounds.Top + e.Bounds.Height / 2;

            // 4. 绘制展开/折叠的小三角箭头
            if (e.Node.Nodes.Count > 0)
            {
                using (SolidBrush arrowBrush = new SolidBrush(isSelected ? _nodeSelectedForeColor : _arrowColor))
                {
                    if (e.Node.IsExpanded)
                    {
                        // 向下的三角形 (▼)
                        Point[] points = {
                            new Point(arrowX - 4, midY - 2),
                            new Point(arrowX + 4, midY - 2),
                            new Point(arrowX, midY + 3)
                        };
                        e.Graphics.FillPolygon(arrowBrush, points);
                    }
                    else
                    {
                        // 向右的三角形 (▶)
                        Point[] points = {
                            new Point(arrowX - 2, midY - 4),
                            new Point(arrowX + 3, midY),
                            new Point(arrowX - 2, midY + 4)
                        };
                        e.Graphics.FillPolygon(arrowBrush, points);
                    }
                }
            }

            // 5. 绘制节点文字
            int textX = arrowX + 4; // 文字起始X坐标
            Rectangle textRect = new Rectangle(textX, e.Bounds.Top, e.Bounds.Width - textX, e.Bounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                e.Node.Text,
                this.Font,
                textRect,
                foreColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
            );
        }

        // 双击节点自动展开/折叠，防止双击文本时默认选中的闪烁
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseDoubleClick(e);
            if (e.Node == null) return;
            if (e.Node.IsExpanded)
            {
                e.Node.Expand();
            }
            else
            {
                e.Node.Collapse();
            }
        }
    }
}
