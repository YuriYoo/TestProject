using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleAgent.UserControls
{
    public partial class QuestionDialog : UserControl
    {
        /// <summary>选择模式</summary>
        private QuestionMode _mode = QuestionMode.NoSelect;

        /// <summary>选项列表</summary>
        private List<string> _options = [];

        /// <summary>当前选择的选项索引</summary>
        private List<int> _selectedIndices = [];

        public event Action<List<int>, List<string>>? ConfirmClicked;
        public event Action? CancelClicked;

        public QuestionDialog()
        {
            InitializeComponent();
        }

        public QuestionDialog(string question, QuestionMode mode, List<string> options)
        {
            InitializeComponent();
            SetQuestion(question, mode, options);
        }

        /// <summary>
        /// 设置选项问题
        /// </summary>
        /// <param name="question"></param>
        /// <param name="mode"></param>
        /// <param name="options"></param>
        public void SetQuestion(string question, QuestionMode mode, List<string> options)
        {
            _mode = mode;
            _options = options ?? [];
            _selectedIndices.Clear();

            QuestionText.Text = question;
            BuildOptions();
        }

        /// <summary>
        /// 构建选项列表
        /// </summary>
        private void BuildOptions()
        {
            SuspendLayout();
            OptionPanel.Controls.Clear();
            if (_mode != QuestionMode.NoSelect)
            {
                for (int i = 0; i < _options.Count; i++)
                {
                    var option = _options[i];
                    var optionItem = new QuestionOptionItem(option)
                    {
                        Width = OptionPanel.Width,
                        Dock = DockStyle.Bottom,
                        Tag = i
                    };

                    // 设置切换选项事件
                    optionItem.SelectedChanged += (s, e) =>
                    {
                        if (_mode == QuestionMode.SingleSelect)
                        {
                            if (optionItem.IsSelected)
                            {
                                foreach (Control ctrl in OptionPanel.Controls)
                                {
                                    if (ctrl is QuestionOptionItem item && item != optionItem)
                                    {
                                        item.IsSelected = false;
                                    }
                                }
                                _selectedIndices.Clear();
                                _selectedIndices.Add((int)optionItem.Tag);
                            }
                        }
                        else if (_mode == QuestionMode.MultiSelect)
                        {
                            var index = (int)optionItem.Tag;
                            if (optionItem.IsSelected)
                            {
                                if (!_selectedIndices.Contains(index))
                                {
                                    _selectedIndices.Add(index);
                                }
                            }
                            else
                            {
                                _selectedIndices.Remove(index);
                            }
                        }
                    };

                    // 设置点击事件
                    optionItem.Clicked += (s, e) =>
                    {
                        if (_mode == QuestionMode.SingleSelect)
                        {
                            optionItem.IsSelected = true;
                        }
                        else if (_mode == QuestionMode.MultiSelect)
                        {
                            optionItem.IsSelected = !optionItem.IsSelected;
                        }
                    };

                    OptionPanel.Controls.Add(optionItem);
                }
            }
            // 重新计算尺寸
            ResumeLayout();
            Visible = true;
        }

        /// <summary>
        /// 获取用户选择的选项列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetSelectedOptions()
        {
            var list = new List<string>();
            foreach (var item in _selectedIndices)
            {
                list.Add(_options[item]);
            }
            return list;
        }

        /// <summary>
        /// 获取用户选择的选项索引列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedIndices()
        {
            return [.. _selectedIndices];
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ConfirmClicked?.Invoke(GetSelectedIndices(), GetSelectedOptions());
            Reset();
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelClicked?.Invoke();
            Reset();
        }

        private void Reset()
        {
            Visible = false;
            ConfirmClicked = null;
            CancelClicked = null;
        }

        /// <summary>
        /// 重写方法告诉父容器“需要多大”
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (Control ctrl in QuestionPanel.Controls)
            {
                int ctrlRight = ctrl.Right; // Location.X + Width
                int ctrlBottom = ctrl.Bottom; // Location.Y + Height

                ctrlRight += ctrl.Margin.Right;
                ctrlBottom += ctrl.Margin.Bottom;

                if (ctrlRight > maxWidth) maxWidth = ctrlRight;
                if (ctrlBottom > maxHeight) maxHeight = ctrlBottom;
            }

            // 加上 Padding
            //maxWidth += Padding.Right;
            //maxHeight += Padding.Bottom;

            // 返回计算后的尺寸
            return new Size(maxWidth, maxHeight);
        }
    }
}
