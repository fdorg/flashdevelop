using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Globalization;

namespace AppMan.Utilities
{
    public class ZipHelper
    {
        /// <summary>
        /// Extracts a zip file to specified directory.
        /// </summary>
        public static void ExtractZip(String file, String path)
        {
            // Save current directory
            String curDir = Environment.CurrentDirectory;
            try
            {
                ZipEntry entry = null;
                ZipInputStream zis = new ZipInputStream(new FileStream(file, FileMode.Open, FileAccess.Read));
                Environment.CurrentDirectory = path; // Used to work around long path issue
                while ((entry = zis.GetNextEntry()) != null)
                {
                    Int32 size = 4096;
                    Byte[] data = new Byte[4096];
                    if (entry.IsFile)
                    {
                        String ext = Path.GetExtension(entry.Name);
                        String dirPath = Path.GetDirectoryName(entry.Name);
                        if (!String.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        String fullPath = Path.Combine(path, entry.Name);
                        #if WIN32
                        FileStream extracted = LongPathFileOpen(fullPath);
                        #else
                        FileStream extracted = File.Open(fullPath, FileMode.Create, FileAccess.Write);
                        #endif
                        while (true)
                        {
                            size = zis.Read(data, 0, data.Length);
                            if (size > 0) extracted.Write(data, 0, size);
                            else break;
                        }
                        extracted.Close();
                        extracted.Dispose();
                    }
                    else if (!Directory.Exists(entry.Name))
                    {
                        Directory.CreateDirectory(entry.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            // Restore current directory
            Environment.CurrentDirectory = curDir;
        }

        #region LongPath

        // See: http://bcl.codeplex.com/wikipage?title=Long%20Path&referringTitle=Home

        #if WIN32

        private const string LongPathPrefix = @"\\?\";

        [Flags]
        internal enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint GetFullPathName(string lpFileName, uint nBufferLength, StringBuilder lpBuffer, IntPtr mustBeNull);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        public static FileStream LongPathFileOpen(string path)
        {
            string normalizedPath = NormalizeLongPath(path);
            SafeFileHandle handle = CreateFile(normalizedPath, EFileAccess.GenericWrite, (uint)FileShare.None, IntPtr.Zero, (uint)FileMode.Create, (uint)FileOptions.None, IntPtr.Zero);
            if (handle.IsInvalid) throw new Exception("Error while resolving path.");
            return new FileStream(handle, FileAccess.Write, 1024, false);
        }

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

        #endif

        #endregion

    }

}
