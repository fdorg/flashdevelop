using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AppMan
{
    public class Win32
    {
        /// <summary>
        /// Public static props
        /// </summary>
        public static Boolean IsRunningOnMono;
        public static Int32 HWND_BROADCAST = 0xffff;
        public static Int32 WM_SHOWME;

        static Win32()
        {
            IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
            if (!IsRunningOnMono) WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        }

        #region Externs

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wp, IntPtr lp);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern UInt32 GetFullPathName(String lpFileName, UInt32 nBufferLength, StringBuilder lpBuffer, IntPtr mustBeNull);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(String lpFileName, EFileAccess dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        
        [DllImport("user32.dll")]
        public static extern Boolean PostMessage(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern Int32 RegisterWindowMessage(String message);

        #endregion

        #region Long Path

        // See: http://bcl.codeplex.com/wikipage?title=Long%20Path&referringTitle=Home
        private const string LongPathPrefix = @"\\?\";

        [Flags]
        internal enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        /// <summary>
        /// 
        /// </summary>
        public static FileStream LongPathFileOpen(string path)
        {
            string normalizedPath = NormalizeLongPath(path);
            SafeFileHandle handle = CreateFile(normalizedPath, EFileAccess.GenericWrite, (uint)FileShare.None, IntPtr.Zero, (uint)FileMode.Create, (uint)FileOptions.None, IntPtr.Zero);
            if (handle.IsInvalid) throw new Exception("Error while resolving path.");
            return new FileStream(handle, FileAccess.Write, 1024, false);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static string NormalizeLongPath(string path)
        {
            StringBuilder buffer = new StringBuilder(path.Length + 1);
            uint length = GetFullPathName(path, (uint)buffer.Capacity, buffer, IntPtr.Zero);
            if (length > buffer.Capacity)
            {
                buffer.Capacity = (int)length;
                length = GetFullPathName(path, length, buffer, IntPtr.Zero);
            }
            if (length == 0 || length > 32000)
            {
                throw new Exception("Error while resolving path.");
            }
            return LongPathPrefix + buffer.ToString();
        }

        #endregion

    }

}

