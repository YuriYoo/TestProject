using SimpleAgent.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Services
{
    public class ChatUIService
    {
        /// <summary>智能体标签页Map</summary>
        private Dictionary<AgentType, AgentTab> agentTabs;

        /// <summary>智能体聊天面板Map</summary>
        private Dictionary<AgentType, FlowLayoutPanel> chatPanels;

        /// <summary>智能体是否正在输出Map (null时表示需要创建新消息, 否则表示已有需要追加聊天内容)</summary>
        private Dictionary<AgentType, ChatMessageItem?> inChat;

        /// <summary>问题弹窗</summary>
        private QuestionDialog questionDialog;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mainForm"></param>
        public void Initialization(MainForm mainForm)
        {
            agentTabs = new()
            {
                { AgentType.Planner, mainForm.PlannerAgentTab },
                { AgentType.Developer, mainForm.CoderAgentTab },
                { AgentType.Reviewer, mainForm.ReviewerAgentTab },
            };

            chatPanels = new()
            {
                { AgentType.Planner, mainForm.PlannerChatPanel },
                { AgentType.Developer, mainForm.CoderChatPanel },
                { AgentType.Reviewer, mainForm.ReviewerChatPanel },
            };

            inChat = new()
            {
                { AgentType.Planner, null },
                { AgentType.Developer, null },
                { AgentType.Reviewer, null },
            };

            questionDialog = mainForm.QuestionDialog;
        }

        /// <summary>
        /// 显示向用户提问的窗口
        /// </summary>
        /// <param name="question"></param>
        /// <param name="mode"></param>
        /// <param name="options"></param>
        public async Task<(bool confirm, List<int>? indices, List<string>? options)> ShowQuestion(string question, QuestionMode mode, List<string>? options)
        {
            var tcs = new TaskCompletionSource<(bool, List<int>?, List<string>?)>();

            questionDialog.SetQuestion(question, mode, options);
            questionDialog.ConfirmClicked += (indices, selectedOptions) =>
            {
                tcs.SetResult((true, indices, selectedOptions));
            };
            questionDialog.CancelClicked += () =>
            {
                tcs.SetResult((false, null, null));
            };

            return await tcs.Task;
        }

        public void SendUserMessage(AgentType agentType, string message)
        {
            if (agentType == AgentType.Router)
            {
                Trace.WriteLine("[警告] 路由智能体消息不需要显示");
                return;
            }

            // 如果是用户消息直接发送, 并且强制结束AI的消息流
            EndChat(agentType);
            CreateMessageItem(MessageType.User, agentType, message);
        }

        public void SendSystemMessage(AgentType agentType, string message = "")
        {
            if (agentType == AgentType.Router)
            {
                Trace.WriteLine("[警告] 路由智能体消息不需要显示");
                return;
            }

            // 如果是系统消息直接发送, 并且强制结束AI的消息流
            EndChat(agentType);

            if (!string.IsNullOrWhiteSpace(message))
            {
                CreateMessageItem(MessageType.System, agentType, message);
            }
        }

        public void SendAIMessage(AgentType agentType, string message)
        {
            if (agentType == AgentType.Router)
            {
                Trace.WriteLine("[警告] 路由智能体消息不需要显示");
                return;
            }

            // 子代理特殊处理
            var chatType = agentType != AgentType.SubDeveloper ? agentType : AgentType.Developer;

            // 如果之前没有进行中的消息则创建新消息
            if (inChat[chatType] == null)
            {
                var item = CreateMessageItem(MessageType.AI, agentType, message);
                inChat[chatType] = item;

                // 更新标签页状态为正在运行, 并跳转
                agentTabs[chatType].SetRunning(true);
                agentTabs[chatType].PerformClick();

                // 滚动到最底部
                var panel = chatPanels[chatType];
                //panel.SuspendLayout();
                panel.VerticalScroll.Value = panel.VerticalScroll.Maximum;
                //panel.ResumeLayout();
            }

            // 如果已有进行中的消息则追加内容
            else
            {
                AppendText(chatType, message);
            }
        }

        public int SendToolMessage(AgentType agentType, string message, int line = -1)
        {
            if (string.IsNullOrEmpty(message))
            {
                return line;
            }
            if (agentType == AgentType.Router)
            {
                Trace.WriteLine("[警告] 路由智能体消息不需要显示");
                return line;
            }

            var chatType = agentType != AgentType.SubDeveloper ? agentType : AgentType.Developer;

            // 如果之前没有进行中的消息则创建新消息
            if (inChat[chatType] == null)
            {
                var item = CreateMessageItem(MessageType.AI, agentType, message);
                inChat[chatType] = item;

                // 更新标签页状态为正在运行, 并跳转
                agentTabs[chatType].SetRunning(true);
                agentTabs[chatType].PerformClick();
            }

            if (line < 0)
            {
                var textBox = inChat[chatType].ChatMessage;
                AppendText(chatType, $"[正在调用 {message} 工具] ... ", true, Color.DeepSkyBlue);
                int outLine = textBox.GetLineFromCharIndex(textBox.TextLength);
                AppendNewLine(textBox);
                return outLine;
            }
            else
            {
                ReplaceLine(chatType, line, $"{message}", Color.ForestGreen);
                return line;
            }
        }

        /// <summary>
        /// 创建一条消息记录
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="agentType">发送到哪个消息面板</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        private ChatMessageItem CreateMessageItem(MessageType messageType, AgentType agentType, string message = "")
        {
            var chatType = agentType != AgentType.SubDeveloper ? agentType : AgentType.Developer;

            var panel = chatPanels[chatType];
            if (messageType == MessageType.AI && string.IsNullOrWhiteSpace(message))
            {
                message = "思考中...";
            }
            var item = new ChatMessageItem(messageType, message)
            {
                Width = panel.ClientSize.Width,
            };
            if (agentType == AgentType.SubDeveloper)
            {
                item.ChatName.Text = "子代理";
            }

            panel.Controls.Add(item);
            panel.ScrollControlIntoView(item);
            return item;
        }

        private void EndChat(AgentType agentType)
        {
            if (agentType == AgentType.SubDeveloper)
            {
                agentType = AgentType.Developer;
            }
            inChat[agentType] = null;
            agentTabs[agentType].SetRunning(false);
        }

        /// <summary>
        /// 向现有聊天框追加文本, 可选是否添加换行和设置文本颜色
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="text"></param>
        /// <param name="addNewLine"></param>
        /// <param name="color"></param>
        public void AppendText(AgentType agentType, string text, bool addNewLine = false, Color? color = null)
        {
            var item = inChat[agentType];
            if (string.IsNullOrEmpty(text) || item == null) return;

            if (item.ChatMessage.Text == "思考中...")
            {
                ClearText(item.ChatMessage);
            }

            if (addNewLine) AppendNewLine(item.ChatMessage);

            if (color.HasValue)
            {
                // 记住当前的默认颜色
                Color defaultColor = item.ChatMessage.ForeColor;

                // 将光标定位到文本末尾
                item.ChatMessage.Select(item.ChatMessage.TextLength, 0);

                // 设置将要插入的文本的颜色
                item.ChatMessage.SelectionColor = color.Value;

                item.ChatMessage.AppendText(text);

                // 将光标处的新输入颜色恢复为默认颜色，防止影响后续追加或用户键盘输入的文本
                item.ChatMessage.Select(item.ChatMessage.TextLength, 0);
                item.ChatMessage.SelectionColor = defaultColor;
            }
            else
            {
                item.ChatMessage.AppendText(text);
            }

            var panel = chatPanels[agentType];
            panel.SuspendLayout();
            panel.VerticalScroll.Value = panel.VerticalScroll.Maximum;
            panel.ResumeLayout();
        }


        /// <summary>
        /// 添加换行
        /// </summary>
        /// <param name="canRepeat">是否允许重复</param>
        public void AppendNewLine(FlatRichTextBox textBox, bool canRepeat = false)
        {
            if (canRepeat || !IsTextEndingWithNewLine(textBox))
            {
                textBox.AppendText(Environment.NewLine);
            }
        }

        /// <summary>
        /// 删除最后一行
        /// </summary>
        /// <param name="agentType"></param>
        private void DeleteLastLine(AgentType agentType)
        {
            var item = inChat[agentType];
            if (item == null) return;
            var richTextBox = item.ChatMessage;
            if (richTextBox.Lines.Length > 0)
            {
                int lastLineIndex = richTextBox.Lines.Length - 1;

                // 获取最后一行的第一个字符在整个文本中的索引
                int startIndex = richTextBox.GetFirstCharIndexFromLine(lastLineIndex);

                // 如果要连同上一行的换行符一起删除，可以做个判断
                // if (startIndex > 0) startIndex -= 1; 

                // 选中从最后一行开头到文本末尾的所有内容
                richTextBox.ReadOnly = false;
                richTextBox.Select(startIndex, richTextBox.TextLength - startIndex);

                // 将选中的文本替换为空，即删除
                richTextBox.SelectedText = "";
                richTextBox.ReadOnly = true;
            }
        }

        /// <summary>
        /// 替换指定行的文字
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="line">行号(从0开始)</param>
        /// <param name="newText"></param>
        private void ReplaceLine(AgentType agentType, int line, string newText, Color color)
        {
            var item = inChat[agentType];
            if (item == null) return;
            var richTextBox = item.ChatMessage;

            // 获取目标行的起始字符索引
            int startIndex = richTextBox.GetFirstCharIndexFromLine(line);

            // 如果返回 -1，说明行号超出了范围
            if (startIndex != -1)
            {
                int lengthToSelect;

                // 获取下一行的起始索引，用来计算当前行的长度（避开使用 Lines 属性）
                int nextLineStartIndex = richTextBox.GetFirstCharIndexFromLine(line + 1);

                if (nextLineStartIndex == -1)
                {
                    // 如果下一行索引为 -1，说明当前行已经是最后一行
                    lengthToSelect = richTextBox.TextLength - startIndex;
                }
                else
                {
                    // 如果不是最后一行，当前行长度 = 下一行起始点 - 当前行起始点 - 1 (减去1是为了保留换行符 '\n')
                    lengthToSelect = nextLineStartIndex - startIndex - 1;
                }

                // 选中要替换的文本
                richTextBox.ReadOnly = false;
                richTextBox.Select(startIndex, lengthToSelect);
                richTextBox.SelectionColor = color;

                // 替换内容
                richTextBox.SelectedText = newText;
                richTextBox.ReadOnly = true;
            }
        }

        /// <summary>
        /// 获取聊天框中最后的文本
        /// </summary>
        /// <param name="agentType"></param>
        private string GetLastString(AgentType agentType, int len)
        {
            var item = inChat[agentType];
            if (item == null) return string.Empty;

            var richTextBox = item.ChatMessage;
            if (richTextBox.TextLength > len)
            {
                return richTextBox.Text.Substring(richTextBox.Text.Length - len, len);
            }
            else
            {
                return richTextBox.Text;
            }
        }

        /// <summary>
        /// 清空文本框内容
        /// </summary>
        /// <param name="textBox"></param>
        private void ClearText(FlatRichTextBox textBox)
        {
            textBox.Clear();
        }

        /// <summary>
        /// 判断最后一个字符是否是换行符
        /// </summary>
        /// <returns></returns>
        private bool IsTextEndingWithNewLine(FlatRichTextBox textBox)
        {
            if (textBox.TextLength == 0) return true;

            // 检查最后一个字符是不是换行符 \n 或 \r
            char lastChar = textBox.Text[textBox.TextLength - 1];
            return lastChar == '\n' || lastChar == '\r';
        }
    }
}
