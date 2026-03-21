using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAgent.Utility
{
	public static class Win32API
	{
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		public const int GWL_HWNDPARENT = -8;
		public const int WM_POPUPSYSTEMMENU = 0x313;

		/// <summary>设置控件是否可以重绘</summary>
		public const int WM_SETREDRAW = 0x000B;

		/// <summary>指定窗口包含 “系统菜单”即窗口标题栏左上角图标（或右键点击标题栏时）弹出的菜单，通常包含 “还原、移动、大小、最小化、最大化、关闭” 等选项</summary>
		public const int WS_SYSMENU = 0x80000;

		/// <summary>在窗口为绘制一个边框</summary>
		public const int WS_BORDER = 0x00800000;

		/// <summary>扩展样式: 让系统对窗口及其子控件进行整体重绘, 使用双缓冲合成, 可以减少闪烁(该样式可能对某些控件如TextBox的绘制有轻微影响, 需测试兼容性)</summary>
		public const int WS_EX_COMPOSITED = 0x02000000;

		/// <summary>无边框样式下点击任务栏程序时允许最小化</summary>
		public const int WS_MINIMIZEBOX = 0x00020000;

		/// <summary>窗口拖动和阴影效果</summary>
		public const int CS_DROPSHADOW = 0x20000;

		/// <summary>判断鼠标当前点击 / 悬停的位置属于窗口的哪个区域</summary>
		public const int WM_NCHITTEST = 0x84;

		/// <summary>系统命令消息, 表示将向Windows发送的消息类型</summary>
		public const int WM_SYSCOMMAND = 0x0112;

		/// <summary>鼠标移动消息</summary>
		public const int WM_MOUSEMOVE = 0x0200;

		/// <summary>鼠标按下消息</summary>
		public const int WM_LBUTTONDOWN = 0x00A1;

		/// <summary>当窗口显示状态改变时发送（如显示、隐藏、最小化 / 恢复）</summary>
		public const int WM_SHOWWINDOW = 0x18;

		/// <summary>
		/// 窗口位置、大小、Z 轴顺序即将改变时发送。允许窗口调整这些变化（如限制最小大小）。
		/// </summary>
		public const int WM_WINDOWPOSCHANGING = 0x46;

		/// <summary>
		/// 当窗口被激活（获得焦点）或停用（失去焦点）时发送。常用于更新窗口状态（如激活时高亮边框，停用时淡化）
		/// 参数含义：
		/// - wParam：低字节表示激活状态（1 = 激活，0 = 停用），高字节表示是否由鼠标激活；
		/// - lParam：指向被激活 / 停用窗口的句柄（HWND）。
		/// </summary>
		public const int WM_ACTIVATE = 0x06;

		/// <summary>
		/// 当窗口需要获取最小化、最大化的尺寸限制时发送（如用户点击最大化按钮前）
		/// 系统会传递一个MINMAXINFO结构体指针，控件可通过修改该结构体设置：
		/// - 最大化后的窗口尺寸（避免超出屏幕）；
		/// - 最小化后的窗口尺寸；
		/// - 最大化时的偏移量（如避开任务栏）。
		/// 自定义窗口大小限制时常需处理此消息。
		/// </summary>
		public const int WM_GETMINMAXINFO = 0x1C;

		/// <summary>
		/// 当窗口的非客户区（标题栏、边框、菜单等系统区域）激活或停用时发送。
		/// 系统默认会通过此消息更新非客户区外观（如激活时标题栏变蓝，停用时变灰）。
		/// 自定义非客户区样式时，需重写此消息的绘制逻辑（如修改标题栏颜色）。
		/// 参数含义：
		/// - wParam：1 = 激活，0 = 停用；
		/// - lParam：通常为 0。
		/// </summary>
		public const int WM_NCACTIVATE = 0x86;


		// 边缘方向常量, 对应不同的调整方向
		public const int HTLEFT = 10;       // 左边缘
		public const int HTRIGHT = 11;      // 右边缘
		public const int HTTOP = 12;        // 上边缘
		public const int HTBOTTOM = 15;     // 下边缘
		public const int HTTOPLEFT = 13;    // 左上角落
		public const int HTTOPRIGHT = 14;   // 右上角落
		public const int HTBOTTOMLEFT = 16; // 左下角落
		public const int HTBOTTOMRIGHT = 17;// 右下角落

		// 鼠标滚轮的 Windows 消息常量
		public const int WM_MOUSEWHEEL = 0x020A;

		public enum ScrollBarType : uint
		{
			SbHorz = 0,
			SbVert = 1,
			SbCtl = 2,
			SbBoth = 3
		}

		public enum Message : uint
		{
			WM_VSCROLL = 0x0115
		}

		public enum ScrollBarCommands : uint
		{
			SB_LINEDOWN = 1,
			SB_THUMBPOSITION = 4
		}

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(nint hWnd, int Msg, nint wParam, nint lParam);

		[DllImport("User32.dll")]
		public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hP, IntPtr hC, string sC, string sW);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

		[DllImport("User32.dll")]
		public extern static int GetScrollPos(IntPtr hWnd, int nBar);

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string libname);
	}
}
