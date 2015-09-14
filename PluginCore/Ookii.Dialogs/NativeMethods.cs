using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ookii.Dialogs.Interop;

namespace Ookii.Dialogs
{
    static class NativeMethods
    {
        public static bool IsWindowsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
            }
        }

        public static bool IsWindowsXPOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1, 2600);
            }
        }

        #region LoadLibrary

        public const int ErrorFileNotFound = 2;

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeModuleHandle LoadLibraryEx(
            string lpFileName,
            IntPtr hFile,
            LoadLibraryExFlags dwFlags
            );

        [DllImport("kernel32", SetLastError = true),
        ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [Flags]
        public enum LoadLibraryExFlags : uint
        {
            DontResolveDllReferences = 0x00000001,
            LoadLibraryAsDatafile = 0x00000002,
            LoadWithAlteredSearchPath = 0x00000008,
            LoadIgnoreCodeAuthzLevel = 0x00000010
        }

        #endregion

        #region KnownFolder Definitions

        internal enum FFFP_MODE
        {
            FFFP_EXACTMATCH,
            FFFP_NEARESTPARENTMATCH
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct KNOWNFOLDER_DEFINITION
        {
            internal KF_CATEGORY category;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszCreator;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszDescription;
            internal Guid fidParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszRelativePath;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszParsingName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszToolTip;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszLocalizedName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSecurity;
            internal uint dwAttributes;
            internal KF_DEFINITION_FLAGS kfdFlags;
            internal Guid ftidType;
        }

        internal enum KF_CATEGORY
        {
            KF_CATEGORY_VIRTUAL = 0x00000001,
            KF_CATEGORY_FIXED = 0x00000002,
            KF_CATEGORY_COMMON = 0x00000003,
            KF_CATEGORY_PERUSER = 0x00000004
        }

        [Flags]
        internal enum KF_DEFINITION_FLAGS
        {
            KFDF_PERSONALIZE = 0x00000001,
            KFDF_LOCAL_REDIRECT_ONLY = 0x00000002,
            KFDF_ROAMABLE = 0x00000004,
        }


        // Property System structs and consts
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;
            internal uint pid;
        }

        #endregion

        #region File Operations Definitions

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,
            FDAP_TOP = 0x00000001,
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

        internal enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attirbutes together.
            SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
            SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
        }

        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
            SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
        }

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        internal enum CDCONTROLSTATE
        {
            CDCS_INACTIVE = 0x00000000,
            CDCS_ENABLED = 0x00000001,
            CDCS_VISIBLE = 0x00000002
        }

        #endregion

        #region Shell Parsing Names

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

        public static IShellItem CreateItemFromParsingName(string path)
        {
            object item;
            Guid guid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"); // IID_IShellItem
            int hr = SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out item);
            if( hr != 0 )
                throw new Win32Exception(hr);
            return (IShellItem)item;
        }

        #endregion

        #region Desktop Window Manager

        public const int WM_NCHITTEST = 0x0084;
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        public enum HitTestResult
        {
            Error = -2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            Caption = 2,
            SysMenu = 3,
            GrowBox = 4,
            Size = GrowBox,
            Menu = 5,
            HScroll = 6,
            VScroll = 7,
            MinButton = 8,
            MaxButton = 9,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18,
            Reduce = MinButton,
            Zoom = MaxButton,
            SizeFirst = Left,
            SizeLast = BottomRight,
            Object = 19,
            Close = 20,
            Help = 21
        }

        public struct MARGINS
        {
            public MARGINS(Padding value)
            {
                Left = value.Left;
                Right = value.Right;
                Top = value.Top;
                Bottom = value.Bottom;
            }
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern SafeDeviceHandle CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(SafeDeviceHandle hDC, SafeGDIHandle hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, SafeDeviceHandle hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DrawThemeTextEx(IntPtr hTheme, SafeDeviceHandle hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        [DllImport("gdi32.dll")]
        public static extern SafeGDIHandle CreateDIBSection(IntPtr hdc, BITMAPINFO pbmi, uint iUsage, IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetThemeTextExtent(IntPtr hTheme, SafeDeviceHandle hdc, int iPartId, int iStateId, string text, int iCharCount, int dwTextFlags, [In]ref RECT bounds, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.U4)]
            public DrawThemeTextFlags dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public Point ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public int pfnDrawTextCallback;
            public IntPtr lParam;
        }

        [Flags()]
        public enum DrawThemeTextFlags
        {
            TextColor = 1 << 0,
            BorderColor = 1 << 1,
            ShadowColor = 1 << 2,
            ShadowType = 1 << 3,
            ShadowOffset = 1 << 4,
            BorderSize = 1 << 5,
            FontProp = 1 << 6,
            ColorProp = 1 << 7,
            StateId = 1 << 8,
            CalcRect = 1 << 9,
            ApplyOverlay = 1 << 10,
            GlowSize = 1 << 11,
            Callback = 1 << 12,
            Composited = 1 << 13
        }

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public byte bmiColors_rgbBlue;
            public byte bmiColors_rgbGreen;
            public byte bmiColors_rgbRed;
            public byte bmiColors_rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle rectangle)
            {
                Left = rectangle.X;
                Top = rectangle.Y;
                Right = rectangle.Right;
                Bottom = rectangle.Bottom;
            }

            public override string ToString()
            {
                return "Left: " + Left + ", " + "Top: " + Top + ", Right: " + Right + ", Bottom: " + Bottom;
            }
        }

        #endregion
    }
}
