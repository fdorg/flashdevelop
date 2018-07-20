using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlashDebugger
{
	public class Win32
	{
		public const int WM_NCLBUTTONDOWN = 0x00A1;
		public const int WM_LBUTTONDOWN = 0x0201;
        
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

		public enum HitTest
		{
			HTCLIENT = 1,
			HTCAPTION = 2,
			HTSYSMENU = 3,
			HTGROWBOX = 4,
			HTMENU = 5,
			HTHSCROLL = 6,
			HTVSCROLL = 7,
			HTMINBUTTON = 8,
			HTMAXBUTTON = 9,
			HTLEFT = 10,
			HTRIGHT = 11,
			HTTOP = 12,
			HTTOPLEFT = 13,
			HTTOPRIGHT = 14,
			HTBOTTOM = 15,
			HTBOTTOMLEFT = 16,
			HTBOTTOMRIGHT = 17,
			HTBORDER = 18,
			HTOBJECT = 19,
			HTCLOSE = 20,
			HTHELP = 21
		}

	}

}
