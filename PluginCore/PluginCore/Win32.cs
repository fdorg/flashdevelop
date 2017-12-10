﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using PluginCore.Helpers;

namespace PluginCore
{
    public class Win32
    {
        /// <summary>
        /// Checks if Win32 functionality should be used
        /// </summary>
        public static Boolean ShouldUseWin32()
        {
            return PlatformHelper.IsRunningOnWindows();
        }

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

        public struct COPYDATASTRUCT
        {
            public Int32 dwData;
            public Int32 cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public SHFILEINFO(Boolean b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public Int32 iIcon;
            public UInt32 dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public String szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public String szTypeName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(Win32.SCROLLINFO));
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

        public const Int32 SB_HORZ = 0;
        public const Int32 SB_VERT = 1;
        public const Int32 SB_BOTH = 3;
        public const Int32 SB_THUMBPOSITION = 4;
        public const Int32 SB_THUMBTRACK = 5;
        public const Int32 SB_LEFT = 6;
        public const Int32 SB_RIGHT = 7;
        public const Int32 SB_ENDSCROLL = 8;
        public const Int32 WM_NCPAINT = 0x85;
        public const Int32 WM_ERASEBKGND = 0x14;
        public const Int32 WM_PAINT = 0xf;
        public const Int32 WM_HSCROLL = 0x0114;
        public const Int32 WM_VSCROLL = 0x0115;
        public const Int32 WM_COPYDATA = 74;
        public const Int32 WM_MOUSEWHEEL = 0x20A;
        public const Int32 WM_KEYDOWN = 0x100;
        public const Int32 WM_KEYUP = 0x101;
        public const Int32 WM_NCLBUTTONDOWN = 0x00A1;
        public const Int32 WM_LBUTTONDOWN = 0x0201;
        public const Int32 WM_CHANGEUISTATE = 0x127;
        public const Int32 WM_SETREDRAW = 0xB;
        public const Int32 WM_PRINTCLIENT = 0x0318;
        public const Int32 WM_NCHITTEST = 0x84;
        public const Int32 SIF_RANGE = 0x0001;
        public const Int32 SIF_PAGE = 0x0002;
        public const Int32 SIF_POS = 0x0004;
        public const Int32 SIF_DISABLENOSCROLL = 0x0008;
        public const Int32 SIF_TRACKPOS = 0x0010;
        public const Int32 SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
        public const UInt32 LVM_SCROLL = 0x1014;
        public const UInt32 SWP_SHOWWINDOW = 64;
        public const Int32 SW_RESTORE = 9;
        public const Int32 PRF_CLIENT = 0x00000004;
        public const Int32 TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const Int32 TVS_EX_DOUBLEBUFFER = 0x0004;
        public const Int32 TV_FIRST = 0x1100;
        public const Int32 VK_ESCAPE = 0x1B;
        public const Int32 EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const Int32 EM_GETRECT = 0xB2;
        public const Int32 EM_GETLINECOUNT = 0xBA;
        public const Int32 EM_LINEINDEX = 0xBB;

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        public static extern Boolean IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, Int32 x, Int32 y, Int32 width, Int32 height, UInt32 flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, ref RECT lParam);

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr handle, Int32 messg, Int32 wparam, Int32 lparam);

        [DllImport("user32.Dll", EntryPoint = "PostMessageA")]
        public static extern Boolean PostMessage(IntPtr hWnd, UInt32 msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern Boolean ShowScrollBar(IntPtr hWnd, Int32 wBar, Boolean bShow);

        [DllImport("user32.dll")]
        public static extern Int32 GetScrollPos(IntPtr hWnd, Int32 nBar);

        [DllImport("user32.dll")]
        public static extern Int32 SetScrollPos(IntPtr hWnd, Int32 nBar, Int32 nPos, Boolean bRedraw);

        [DllImport("user32.dll")]
        public static extern Boolean GetScrollInfo(IntPtr hWnd, Int32 fnBar, SCROLLINFO scrollInfo);

        [DllImport("user32.dll")]
        public static extern Boolean ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern UInt32 SetForegroundWindow(IntPtr hwnd);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern Boolean PathCompactPathEx([MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszOut, [MarshalAs(UnmanagedType.LPTStr)] String pszSource, [MarshalAs(UnmanagedType.U4)] Int32 cchMax, [MarshalAs(UnmanagedType.U4)] Int32 dwReserved);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetLongPathName([MarshalAs(UnmanagedType.LPTStr)] String path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder longPath, Int32 longPathLength);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetShortPathName(String lpszLongPath, StringBuilder lpszShortPath, Int32 cchBuffer);

        [DllImport("shell32.dll", EntryPoint = "#28")]
        public static extern UInt32 SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] String pszPath, out IntPtr ppidl, ref Int32 rgflnOut);

        [DllImport("shell32.dll", EntryPoint = "SHGetPathFromIDListW")]
        public static extern Boolean SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        [DllImport("user32.dll")]
        public static extern Int32 DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetModuleHandle(String lpModuleName);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(Int32 hInst, String FileName, Int32 nIconIndex);

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetFileInfo(String pszPath, UInt32 dwFileAttributes, out SHFILEINFO psfi, UInt32 cbfileInfo, SHGFI uFlags);

        #endregion

        #region Window

        /// <summary>
        /// Sets the window specified by handle to fullscreen
        /// </summary>
        public static void SetWinFullScreen(IntPtr hwnd)
        {
            Screen screen = Screen.FromHandle(hwnd);
            Int32 screenTop = screen.WorkingArea.Top;
            Int32 screenLeft = screen.WorkingArea.Left;
            Int32 screenWidth = screen.WorkingArea.Width;
            Int32 screenHeight = screen.WorkingArea.Height;
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
        public static SCROLLINFO GetFullScrollInfo(Control lv, Boolean horizontalBar)
        {
            Int32 fnBar = (horizontalBar ? SB_HORZ : SB_VERT);
            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.fMask = SIF_ALL;
            if (GetScrollInfo(lv.Handle, fnBar, scrollInfo)) return scrollInfo;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Point GetScrollPos(Control ctrl)
        {
            return new Point(GetScrollPos(ctrl.Handle, SB_HORZ), GetScrollPos(ctrl.Handle, SB_VERT));
        }

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
        public static void ScrollToLeft(Control ctrl)
        {
            SendMessage(ctrl.Handle, WM_HSCROLL, SB_LEFT, 0);
        }

        #endregion

    }

}

