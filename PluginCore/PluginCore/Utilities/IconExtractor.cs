using System;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace PluginCore.Utilities
{
    public class IconExtractor
	{
        [DllImport("User32.dll")]
        private static extern Int32 DestroyIcon(IntPtr hIcon);

        [DllImport("Kernel32.dll")]
        private static extern Int32 GetModuleHandle(String lpModuleName);

		[DllImport("Shell32.dll")]
        private static extern IntPtr ExtractIcon(Int32 hInst, String FileName, Int32 nIconIndex);

		[DllImport("Shell32.dll")]
        private static extern Int32 SHGetFileInfo(String pszPath, UInt32 dwFileAttributes, out SHFILEINFO psfi, UInt32 cbfileInfo, SHGFI uFlags);

		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
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
			[MarshalAs(UnmanagedType.LPStr, SizeConst=260)]
			public String szDisplayName;
			[MarshalAs(UnmanagedType.LPStr, SizeConst=80)]
			public String szTypeName;
		};

		private enum SHGFI
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

        /// <summary>
        /// Gets the system icon specified by index in Shell32.dll
        /// </summary>
        public static Icon GetSysIcon(Int32 icNo)
		{
			IntPtr hIcon = ExtractIcon(GetModuleHandle(String.Empty), "Shell32.dll", icNo);
            Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
            DestroyIcon(hIcon);
            return icon;
		}

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon. If the strPath is invalid or there is no icon, the default icon is returned.
        /// </summary>
        public static Icon GetFileIcon(String path, Boolean small, Boolean overlays)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            Int32 cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags = SHGFI.Icon | SHGFI.UseFileAttributes;
            if (small) flags |= SHGFI.SmallIcon; else flags |= SHGFI.LargeIcon;
            if (overlays) flags |= SHGFI.AddOverlays; // Get overlays too...
            SHGetFileInfo(path, 0x00000080, out info, (UInt32)cbFileInfo, flags);
            Icon icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            DestroyIcon(info.hIcon);
            return icon;
        }
        public static Icon GetFileIcon(String path, Boolean small)
        {
            return GetFileIcon(path, small, false);
        }

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon. If the strPath is invalid or there is no icon, the default icon is returned.
        /// </summary>
        public static Icon GetFolderIcon(String path, Boolean small, Boolean overlays)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            Int32 cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags = SHGFI.Icon | SHGFI.UseFileAttributes;
            if (small) flags |= SHGFI.SmallIcon; else flags |= SHGFI.LargeIcon;
            if (overlays) flags |= SHGFI.AddOverlays; // Get overlays too...
            SHGetFileInfo(path, 0x00000010, out info, (UInt32)cbFileInfo, flags);
            Icon icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            DestroyIcon(info.hIcon);
            return icon;
        }
        public static Icon GetFolderIcon(String path, Boolean small)
        {
            return GetFileIcon(path, small, false);
        }

	}
	
}
