using System;
using System.IO;
using System.Linq;
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
        /// Tells if we are running on Mono runtime
        /// </summary>
        public static Boolean IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

        #region Externs

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wp, IntPtr lp);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint GetFullPathName(string lpFileName, uint nBufferLength, StringBuilder lpBuffer, IntPtr mustBeNull);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

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

