/*
 * Win32 controls scrolling management
 */
 
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PluginCore;
using System.Drawing;

namespace Win32
{
	public class Scrolling
	{
		public const int SB_HORZ = 0;
		public const int SB_VERT = 1; 
		const int SB_LEFT = 6;
		const int SB_RIGHT = 7;
		const int WM_HSCROLL = 0x0114;
		const int WM_VSCROLL = 0x0115;
		
		[DllImport("user32", CharSet=CharSet.Auto)] 
		public static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos); 
		
		[DllImport("User32.dll" )]
		public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
		
		[DllImport("user32", CharSet=CharSet.Auto)] 
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		public static void scrollToBottom(Control ctrl)
		{
		    int min, max;
		    GetScrollRange(ctrl.Handle, SB_VERT, out min, out max);
		    SendMessage(ctrl.Handle, WM_VSCROLL, SB_RIGHT, max);
		}
		
		public static void scrollToTop(Control ctrl)
		{
			SendMessage(ctrl.Handle, WM_VSCROLL, SB_LEFT, 0);
		}
		
		public static void scrollToRight(Control ctrl)
		{
		    int min, max;
		    GetScrollRange(ctrl.Handle, SB_HORZ, out min, out max);
		    SendMessage(ctrl.Handle, WM_HSCROLL, SB_RIGHT, max);
		}
		
		public static void scrollToLeft(Control ctrl)
		{
			SendMessage(ctrl.Handle, WM_HSCROLL, SB_LEFT, 0);
		}

        public static Point GetScrollPos(Control ctrl)
        {
            return new Point(
                GetScrollPos(ctrl.Handle, SB_HORZ),
                GetScrollPos(ctrl.Handle, SB_VERT));
        }

        public static void SetScrollPos(Control ctrl, Point scrollPosition)
        {
            SetScrollPos(ctrl.Handle, SB_HORZ, scrollPosition.X, true);
            SetScrollPos(ctrl.Handle, SB_VERT, scrollPosition.Y, true);
        }
	}
}
