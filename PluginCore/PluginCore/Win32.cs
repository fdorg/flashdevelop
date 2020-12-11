// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PluginCore.Helpers;

namespace PluginCore
{
    public class Win32
    {
        /// <summary>
        /// Checks if Win32 functionality should be used
        /// </summary>
        public static bool ShouldUseWin32() => PlatformHelper.IsRunningOnWindows();

        #region Enums

        public enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010,
            ShellIconSize = 0x4,
            AddOverlays = 0x000000020
        }

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

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        #endregion

        #region Constants

        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_BOTH = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_LEFT = 6;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;
        public const int WM_NCPAINT = 0x85;
        public const int WM_ERASEBKGND = 0x14;
        public const int WM_PAINT = 0xf;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_COPYDATA = 74;
        public const int WM_MOUSEWHEEL = 0x20A;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_CHANGEUISTATE = 0x127;
        public const int WM_SETREDRAW = 0xB;
        public const int WM_PRINTCLIENT = 0x0318;
        public const int WM_NCHITTEST = 0x84;
        public const int SIF_RANGE = 0x0001;
        public const int SIF_PAGE = 0x0002;
        public const int SIF_POS = 0x0004;
        public const int SIF_DISABLENOSCROLL = 0x0008;
        public const int SIF_TRACKPOS = 0x0010;
        public const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
        public const uint LVM_SCROLL = 0x1014;
        public const uint SWP_SHOWWINDOW = 64;
        public const int SW_RESTORE = 9;
        public const int PRF_CLIENT = 0x00000004;
        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const int TVS_EX_DOUBLEBUFFER = 0x0004;
        public const int TV_FIRST = 0x1100;
        public const int VK_ESCAPE = 0x1B;
        public const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const int EM_GETRECT = 0xB2;
        public const int EM_GETLINECOUNT = 0xBA;
        public const int EM_LINEINDEX = 0xBB;

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref RECT lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

        [DllImport("user32.Dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, SCROLLINFO scrollInfo);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint SetForegroundWindow(IntPtr hwnd);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern bool PathCompactPathEx([MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszOut, [MarshalAs(UnmanagedType.LPTStr)] string pszSource, [MarshalAs(UnmanagedType.U4)] int cchMax, [MarshalAs(UnmanagedType.U4)] int dwReserved);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder longPath, int longPathLength);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, int cchBuffer);

        [DllImport("shell32.dll", EntryPoint = "#28")]
        public static extern uint SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, out IntPtr ppidl, ref int rgflnOut);

        [DllImport("shell32.dll", EntryPoint = "SHGetPathFromIDListW")]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        [DllImport("user32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        public static extern int GetModuleHandle(string lpModuleName);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(int hInst, string FileName, int nIconIndex);

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);

        #endregion

        #region Window

        /// <summary>
        /// Sets the window specified by handle to fullscreen
        /// </summary>
        public static void SetWinFullScreen(IntPtr hwnd)
        {
            var screen = Screen.FromHandle(hwnd);
            var screenTop = screen.WorkingArea.Top;
            var screenLeft = screen.WorkingArea.Left;
            var screenWidth = screen.WorkingArea.Width;
            var screenHeight = screen.WorkingArea.Height;
            SetWindowPos(hwnd, IntPtr.Zero, screenLeft, screenTop, screenWidth, screenHeight, SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Restores the window with Win32
        /// </summary>
        public static void RestoreWindow(IntPtr handle)
        {
            if (IsIconic(handle)) ShowWindow(handle, SW_RESTORE);
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// 
        /// </summary>
        public static SCROLLINFO GetFullScrollInfo(Control lv, bool horizontalBar)
        {
            var fnBar = (horizontalBar ? SB_HORZ : SB_VERT);
            var scrollInfo = new SCROLLINFO {fMask = SIF_ALL};
            return GetScrollInfo(lv.Handle, fnBar, scrollInfo) ? scrollInfo : null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Point GetScrollPos(Control ctrl) => new Point(GetScrollPos(ctrl.Handle, SB_HORZ), GetScrollPos(ctrl.Handle, SB_VERT));

        /// <summary>
        /// 
        /// </summary>
        public static void SetScrollPos(Control ctrl, Point scrollPosition)
        {
            SetScrollPos(ctrl.Handle, SB_HORZ, scrollPosition.X, true);
            SetScrollPos(ctrl.Handle, SB_VERT, scrollPosition.Y, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ScrollToLeft(Control ctrl) => SendMessage(ctrl.Handle, WM_HSCROLL, SB_LEFT, 0);

        #endregion
    }
}