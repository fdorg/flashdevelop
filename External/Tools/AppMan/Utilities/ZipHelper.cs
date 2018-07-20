using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
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
                        FileStream extracted;
                        String fullPath = Path.Combine(path, entry.Name);
                        if (Win32.IsRunningOnMono) extracted = File.Open(fullPath, FileMode.Create, FileAccess.Write);
                        else extracted = Win32.LongPathFileOpen(fullPath);
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

    }

}
