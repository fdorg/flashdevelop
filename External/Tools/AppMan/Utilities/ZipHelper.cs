using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace AppMan.Utilities
{
    public class ZipHelper
    {
        /// <summary>
        /// Extracts a zip file to specified directory.
        /// </summary>
        public static void ExtractZip(String file, String path)
        {
            try
            {
                ZipEntry entry = null;
                ZipInputStream zis = new ZipInputStream(new FileStream(file, FileMode.Open, FileAccess.Read));
                while ((entry = zis.GetNextEntry()) != null)
                {
                    Int32 size = 4096;
                    Byte[] data = new Byte[4096];
                    String full = Path.Combine(path, entry.Name);
                    if (entry.IsFile)
                    {
                        String ext = Path.GetExtension(full);
                        String dirPath = Path.GetDirectoryName(full);
                        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                        FileStream extracted = new FileStream(full, FileMode.Create);
                        while (true)
                        {
                            size = zis.Read(data, 0, data.Length);
                            if (size > 0) extracted.Write(data, 0, size);
                            else break;
                        }
                        extracted.Close();
                        extracted.Dispose();
                    }
                    else Directory.CreateDirectory(full);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }

        }

    }

}
