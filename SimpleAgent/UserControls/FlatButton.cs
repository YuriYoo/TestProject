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
	public class FlatButton : Button
	{
		/// <summary>不要显示焦点提示框</summary>
		protected override bool ShowFocusCues
		{
			get { return false; }
		}

		public FlatButton()
		{
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 0;
			FlatAppearance.MouseOverBackColor = Color.WhiteSmoke;
			FlatAppearance.MouseDownBackColor = Color.Gainsboro;
			BackColor = Color.White;
		}

		/// <summary>
		/// 去除“默认按钮”在窗口失去焦点时绘制的黑边框
		/// </summary>
		/// <param name="value"></param>
		public override void NotifyDefault(bool value)
		{
			// 强行告诉系统：我不是默认按钮，不要画那个黑框
			// 注意：这里必须传 false，且不要调用 base.NotifyDefault(value)
			base.NotifyDefault(false);
		}
	}
}
