using SimpleAgent.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.UserControls
{
	public class FlatRichTextBox : RichTextBox
	{
		protected override void WndProc(ref Message m)
		{
			// 如果捕获到鼠标滚轮消息，并且该控件有父容器
			if (m.Msg == Win32API.WM_MOUSEWHEEL && Parent != null)
			{
				// 直接把消息转发给父容器
				Win32API.SendMessage(Parent.Handle, m.Msg, m.WParam, m.LParam);

				// 将结果置零，告诉系统我们已经处理过这个消息了，跳过基类的处理
				m.Result = IntPtr.Zero;
				return;
			}

			// 其他消息正常让基类处理
			base.WndProc(ref m);
		}
	}
}
