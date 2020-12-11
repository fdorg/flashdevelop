// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AppMan.Utilities
{
    public class ZipHelper
    {
        /// <summary>
        /// Extracts a zip file to specified directory.
        /// </summary>
        public static void ExtractZip(string file, string path)
        {
            // Save current directory
            var curDir = Environment.CurrentDirectory;
            try
            {
                ZipEntry entry;
                var zis = new ZipInputStream(new FileStream(file, FileMode.Open, FileAccess.Read));
                Environment.CurrentDirectory = path; // Used to work around long path issue
                while ((entry = zis.GetNextEntry()) != null)
                {
                    var data = new byte[4096];
                    if (entry.IsFile)
                    {
                        var dirPath = Path.GetDirectoryName(entry.Name);
                        if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        var fullPath = Path.Combine(path, entry.Name);
                        var extracted = Win32.IsRunningOnMono
                            ? File.Open(fullPath, FileMode.Create, FileAccess.Write)
                            : Win32.LongPathFileOpen(fullPath);
                        while (true)
                        {
                            var size = zis.Read(data, 0, data.Length);
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
    }
}