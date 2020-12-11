// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using System.Runtime.InteropServices;

namespace PluginCore.Utilities
{
    public class IconExtractor
    {
        /// <summary>
        /// Gets the system icon specified by index in shell32.dll
        /// </summary>
        public static Icon GetSysIcon(int icNo)
        {
            var hIcon = Win32.ExtractIcon(Win32.GetModuleHandle(string.Empty), "shell32.dll", icNo);
            var icon = (Icon)Icon.FromHandle(hIcon).Clone();
            Win32.DestroyIcon(hIcon);
            return icon;
        }

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon. If the strPath is invalid or there is no icon, the default icon is returned.
        /// </summary>
        public static Icon GetFileIcon(string path, bool small, bool overlays)
        {
            var info = new Win32.SHFILEINFO(true);
            var cbFileInfo = Marshal.SizeOf(info);
            var flags = Win32.SHGFI.Icon | Win32.SHGFI.UseFileAttributes;
            if (small) flags |= Win32.SHGFI.SmallIcon; 
            else flags |= Win32.SHGFI.LargeIcon;
            if (overlays) flags |= Win32.SHGFI.AddOverlays; // Get overlays too...
            Win32.SHGetFileInfo(path, 0x00000080, out info, (uint)cbFileInfo, flags);
            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            Win32.DestroyIcon(info.hIcon);
            return icon;
        }
        public static Icon GetFileIcon(string path, bool small) => GetFileIcon(path, small, false);

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon. If the strPath is invalid or there is no icon, the default icon is returned.
        /// </summary>
        public static Icon GetFolderIcon(string path, bool small, bool overlays)
        {
            var info = new Win32.SHFILEINFO(true);
            var cbFileInfo = Marshal.SizeOf(info);
            var flags = Win32.SHGFI.Icon | Win32.SHGFI.UseFileAttributes;
            if (small) flags |= Win32.SHGFI.SmallIcon; else flags |= Win32.SHGFI.LargeIcon;
            if (overlays) flags |= Win32.SHGFI.AddOverlays; // Get overlays too...
            Win32.SHGetFileInfo(path, 0x00000010, out info, (uint)cbFileInfo, flags);
            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            Win32.DestroyIcon(info.hIcon);
            return icon;
        }
        public static Icon GetFolderIcon(string path, bool small) => GetFileIcon(path, small, false);
    }
    
}
