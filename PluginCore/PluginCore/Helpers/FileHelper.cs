using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace PluginCore.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Deletes files/directories by sending them to recycle bin
        /// </summary>
        public static Boolean Recycle(String path)
        {
            if (Win32.ShouldUseWin32())
            {
                InteropSHFileOperation fo = new InteropSHFileOperation();
                fo.wFunc = InteropSHFileOperation.FO_Func.FO_DELETE;
                fo.fFlags.FOF_ALLOWUNDO = true;
                fo.fFlags.FOF_NOCONFIRMATION = true;
                fo.fFlags.FOF_NOERRORUI = true;
                fo.fFlags.FOF_SILENT = true;
                fo.pFrom = path;
                return fo.Execute();
            }
            else // Delete directly on other platforms
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Reads the file and returns its contents (autodetects encoding and fallback codepage)
        /// </summary>
        public static String ReadFile(String file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.Contents;
        }

        /// <summary>
        /// Reads the file and returns its contents
        /// </summary>
        public static String ReadFile(String file, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(file, encoding))
            {
                String src = sr.ReadToEnd();
                sr.Close();
                return src;
            }
        }
        
        /// <summary>
        /// Writes a text file to the specified location (if Unicode, with BOM)
        /// </summary>
        public static void WriteFile(string file, string text, Encoding encoding) => WriteFile(file, text, encoding, true);

        /// <summary>
        /// Writes a text file to the specified location (if Unicode, with or without BOM)
        /// </summary>
        public static void WriteFile(string file, string text, Encoding encoding, bool saveBOM)
        {
            var useSkipBomWriter = (encoding == Encoding.UTF8 && !saveBOM);
            if (encoding == Encoding.UTF7) encoding = new UTF7EncodingFixed();
            if (!File.Exists(file) && Path.GetDirectoryName(file) is var dir && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var fs = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.CreateNew))
            {
                using (var sw = useSkipBomWriter ? new StreamWriter(fs) : new StreamWriter(fs, encoding))
                {
                    sw.Write(text);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Adds text to the end of the specified file
        /// </summary>
        public static void AddToFile(String file, String text, Encoding encoding)
        {
            using (StreamWriter sw = new StreamWriter(file, true, encoding))
            {
                sw.Write(text);
                sw.Close();
            }
        }

        /// <summary>
        /// Ensures that a file has been updated after zip extraction
        /// </summary>
        public static void EnsureUpdatedFile(String file)
        {
            try
            {
                String newFile = file + ".new";
                String delFile = file + ".del";
                if (File.Exists(newFile))
                {
                    String oldFile = newFile.Substring(0, newFile.Length - 4);
                    if (File.Exists(oldFile))
                    {
                        File.Copy(newFile, oldFile, true);
                        File.Delete(newFile);
                    }
                    else File.Move(newFile, oldFile);
                }
                if (File.Exists(delFile))
                {
                    File.Delete(file);
                    File.Delete(delFile);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Can't update files.", ex);
            }
        }

        /// <summary>
        /// Ensures that the file name is unique by adding a number to it
        /// </summary>
        public static String EnsureUniquePath(String original)
        {
            Int32 counter = 0;
            String result = original;
            String folder = Path.GetDirectoryName(original);
            String filename = Path.GetFileNameWithoutExtension(original);
            String extension = Path.GetExtension(original);
            while (File.Exists(result))
            {
                counter++;
                String fullname = filename + " (" + counter + ")" + extension;
                result = Path.Combine(folder, fullname);
            }
            return result;
        }

        /// <summary>
        /// Checks that if the file is read only
        /// </summary>
        public static Boolean FileIsReadOnly(String file)
        {
            try
            {
                if (!File.Exists(file)) return false;
                FileAttributes fileAttr = File.GetAttributes(file);
                return (fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Moves a file, overwrites the file at the new location if there is one already.
        /// </summary>
        public static void ForceMove(String oldPath, String newPath)
        {
            if (File.Exists(newPath)) File.Delete(newPath);
            File.Move(oldPath, newPath);
        }

        /// <summary>
        /// Moves a folder, overwriting the files at the new location if there are matches.
        /// </summary>
        public static void CopyDirectory(String oldPath, String newPath, Boolean overwrite)
        {
            Stack<String> stack = new Stack<String>();
            stack.Push(String.Empty);
            String sep = Path.DirectorySeparatorChar.ToString();
            String alt = Path.AltDirectorySeparatorChar.ToString();
            Int32 length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                String subPath = stack.Pop();
                String sourcePath = Path.Combine(oldPath, subPath);
                String targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (String file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), overwrite);
                }
                foreach (String folder in Directory.GetDirectories(sourcePath))
                {
                    stack.Push(folder.Substring(length));
                }
            }
        }

        /// <summary>
        /// Moves a folder, overwriting the files at the new location if there are matches.
        /// </summary>
        public static void ForceMoveDirectory(String oldPath, String newPath)
        {
            Stack<String> stack = new Stack<String>();
            stack.Push(String.Empty);
            String sep = Path.DirectorySeparatorChar.ToString();
            String alt = Path.AltDirectorySeparatorChar.ToString();
            Int32 length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                String subPath = stack.Pop();
                String sourcePath = Path.Combine(oldPath, subPath);
                String targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (String file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    ForceMove(file, Path.Combine(targetPath, Path.GetFileName(file)));
                }
                foreach (String folder in Directory.GetDirectories(sourcePath))
                {
                    stack.Push(folder.Substring(length));
                }
            }
            Directory.Delete(oldPath, true);
        }

        /// <summary>
        /// If the path already exists, the user is asked to confirm
        /// </summary>
        public static Boolean ConfirmOverwrite(String path)
        {
            String name = Path.GetFileName(path);
            if (Directory.Exists(path))
            {
                String title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                String message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFolder");
                DialogResult result = MessageBox.Show(PluginBase.MainForm, String.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return result == DialogResult.Yes;
            }
            else if (File.Exists(path))
            {
                String title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                String message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFile");
                DialogResult result = MessageBox.Show(PluginBase.MainForm, String.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return result == DialogResult.Yes;
            }
            else return true;
        }

        /// <summary>
        /// Checks if a file name matches a search filter mask, eg: filename.jpg matches f*.jpg
        /// </summary>
        /// <param name="fileName">The name of the file to check</param>
        /// <param name="filterMask">The search filter to apply. You can use multiple masks by using ;</param>
        public static Boolean FileMatchesSearchFilter(String fileName, String filterMask)
        {
            foreach (String mask in filterMask.Split(';'))
            {
                String convertedMask = "^" + Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                Regex regexMask = new Regex(convertedMask, RegexOptions.IgnoreCase);
                if (regexMask.IsMatch(fileName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the bytes contains invalid UTF-8 bytes
        /// </summary>
        public static Boolean ContainsInvalidUTF8Bytes(Byte[] bytes)
        {
            Int32 bits = 0;
            Int32 i = 0, c = 0, b = 0;
            Int32 length = bytes.Length;
            for (i = 0; i < length; i++)
            {
                c = bytes[i];
                if (c > 128)
                {
                    if ((c >= 254)) return true;
                    else if (c >= 252) bits = 6;
                    else if (c >= 248) bits = 5;
                    else if (c >= 240) bits = 4;
                    else if (c >= 224) bits = 3;
                    else if (c >= 192) bits = 2;
                    else return true;
                    if ((i + bits) > length) return true;
                    while (bits > 1)
                    {
                        i++;
                        b = bytes[i];
                        if (b < 128 || b > 191) return true;
                        bits--;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Reads the file codepage from the file data
        /// </summary>
        public static Int32 GetFileCodepage(String file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.CodePage;
        }

        /// <summary>
        /// Checks if the file contains BOM
        /// </summary>
        public static Boolean ContainsBOM(String file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.ContainsBOM;
        }

        /// <summary>
        /// Acquires encoding related info on one read.
        /// </summary>
        public static EncodingFileInfo GetEncodingFileInfo(String file)
        {
            Int32 startIndex = 0;
            EncodingFileInfo info = new EncodingFileInfo();
            try
            {
                if (File.Exists(file))
                {
                    Byte[] bytes = File.ReadAllBytes(file);
                    if (bytes.Length > 2 && (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf))
                    {
                        startIndex = 3;
                        info.BomLength = 3;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.UTF8.WebName;
                        info.CodePage = Encoding.UTF8.CodePage;
                    }
                    else if (bytes.Length > 3 && (bytes[0] == 0xff && bytes[1] == 0xfe && bytes[2] == 0x00 && bytes[3] == 0x00))
                    {
                        startIndex = 4;
                        info.BomLength = 4;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.UTF32.WebName;
                        info.CodePage = Encoding.UTF32.CodePage;
                    }
                    else if (bytes.Length > 4 && ((bytes[0] == 0x2b && bytes[1] == 0x2f && bytes[2] == 0x76) && (bytes[3] == 0x38 || bytes[3] == 0x39 || bytes[3] == 0x2b || bytes[3] == 0x2f) && bytes[4] == 0x2D))
                    {
                        startIndex = 5;
                        info.BomLength = 5;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.UTF7.WebName;
                        info.CodePage = Encoding.UTF7.CodePage;
                    }
                    else if (bytes.Length > 3 && ((bytes[0] == 0x2b && bytes[1] == 0x2f && bytes[2] == 0x76) && (bytes[3] == 0x38 || bytes[3] == 0x39 || bytes[3] == 0x2b || bytes[3] == 0x2f)))
                    {
                        startIndex = 4;
                        info.BomLength = 4;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.UTF7.WebName;
                        info.CodePage = Encoding.UTF7.CodePage;
                    }
                    else if (bytes.Length > 1 && (bytes[0] == 0xff && bytes[1] == 0xfe))
                    {
                        startIndex = 2;
                        info.BomLength = 2;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.Unicode.WebName;
                        info.CodePage = Encoding.Unicode.CodePage;
                    }
                    else if (bytes.Length > 1 && (bytes[0] == 0xfe && bytes[1] == 0xff))
                    {
                        startIndex = 2;
                        info.BomLength = 2;
                        info.ContainsBOM = true;
                        info.Charset = Encoding.BigEndianUnicode.WebName;
                        info.CodePage = Encoding.BigEndianUnicode.CodePage;
                    }
                    else
                    {
                        if (!ContainsInvalidUTF8Bytes(bytes))
                        {
                            info.Charset = Encoding.UTF8.WebName;
                            info.CodePage = Encoding.UTF8.CodePage;
                        }
                        else // Try detecting using Ude...
                        {
                            Ude.CharsetDetector detector = new Ude.CharsetDetector();
                            detector.Feed(bytes, 0, bytes.Length); detector.DataEnd();
                            if (detector.Charset != null)
                            {
                                Encoding encoding = Encoding.GetEncoding(detector.Charset);
                                info.Charset = encoding.WebName;
                                info.CodePage = encoding.CodePage;
                            }
                            else
                            {
                                info.Charset = Encoding.Default.WebName;
                                info.CodePage = Encoding.Default.CodePage;
                            }
                        }
                    }
                    Int32 contentLength = bytes.Length - startIndex;
                    if (bytes.Length > 0 && bytes.Length > startIndex)
                    {
                        Encoding encoding = Encoding.GetEncoding(info.CodePage);
                        info.Contents = encoding.GetString(bytes, startIndex, contentLength);
                    }
                }
            }
            catch (Exception)
            {
                info = new EncodingFileInfo();
            }
            return info;
        }

        /// <summary>
        /// Filters a list of paths so that only those meeting the File.Exists() condition remain.
        /// </summary>
        public static List<String> FilterByExisting(List<String> paths, Boolean logicalDrivesOnly)
        {
            List<String> toCheck = new List<String>(paths);
            if (logicalDrivesOnly)
            {
                DriveInfo[] driveInfo = DriveInfo.GetDrives();
                toCheck = new List<String>(paths);
                toCheck.RemoveAll(delegate(String path)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (path.StartsWithOrdinal(drive.RootDirectory.ToString())) return false;
                    }
                    return true;
                });
            }
            toCheck.RemoveAll(path => !File.Exists(path));
            paths.Clear();
            paths.AddRange(toCheck);
            return paths;
        }
    }

    /// <summary>
    /// UTF-7 encoding with correct BOM
    /// </summary>
    class UTF7EncodingFixed : UTF7Encoding
    {
        public override byte[] GetPreamble()
        {
            return new byte[] { 0x2B, 0x2F, 0x76, 0x38, 0x2D };
        }
    }

    /// <summary>
    /// Container for encoding file info
    /// </summary>
    public class EncodingFileInfo
    {
        public Int32 CodePage = -1;
        public String Charset = String.Empty;
        public String Contents = String.Empty;
        public Boolean ContainsBOM = false;
        public Int32 BomLength = 0;
    }

}
