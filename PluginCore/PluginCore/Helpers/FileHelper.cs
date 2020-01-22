// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public static bool Recycle(string path)
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

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the file and returns its contents (autodetects encoding and fallback codepage)
        /// </summary>
        public static string ReadFile(string file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.Contents;
        }

        /// <summary>
        /// Reads the file and returns its contents
        /// </summary>
        public static string ReadFile(string file, Encoding encoding)
        {
            using var reader = new StreamReader(file, encoding);
            var result = reader.ReadToEnd();
            reader.Close();
            return result;
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
            using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.CreateNew);
            using var writer = useSkipBomWriter ? new StreamWriter(stream) : new StreamWriter(stream, encoding);
            writer.Write(text);
            writer.Close();
        }

        /// <summary>
        /// Adds text to the end of the specified file
        /// </summary>
        public static void AddToFile(string file, string text, Encoding encoding)
        {
            using var writer = new StreamWriter(file, true, encoding);
            writer.Write(text);
            writer.Close();
        }

        /// <summary>
        /// Ensures that a file has been updated after zip extraction
        /// </summary>
        public static void EnsureUpdatedFile(string file)
        {
            try
            {
                var newFile = file + ".new";
                if (File.Exists(newFile))
                {
                    var oldFile = newFile.Substring(0, newFile.Length - 4);
                    if (File.Exists(oldFile))
                    {
                        File.Copy(newFile, oldFile, true);
                        File.Delete(newFile);
                    }
                    else File.Move(newFile, oldFile);
                }
                var delFile = file + ".del";
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
        public static string EnsureUniquePath(string original)
        {
            int counter = 0;
            string result = original;
            string folder = Path.GetDirectoryName(original);
            string filename = Path.GetFileNameWithoutExtension(original);
            string extension = Path.GetExtension(original);
            while (File.Exists(result))
            {
                counter++;
                string fullname = filename + " (" + counter + ")" + extension;
                result = Path.Combine(folder, fullname);
            }
            return result;
        }

        /// <summary>
        /// Checks that if the file is read only
        /// </summary>
        public static bool FileIsReadOnly(string file)
        {
            try
            {
                if (!File.Exists(file)) return false;
                var attributes = File.GetAttributes(file);
                return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Moves a file, overwrites the file at the new location if there is one already.
        /// </summary>
        public static void ForceMove(string oldPath, string newPath)
        {
            if (File.Exists(newPath)) File.Delete(newPath);
            File.Move(oldPath, newPath);
        }

        /// <summary>
        /// Moves a folder, overwriting the files at the new location if there are matches.
        /// </summary>
        public static void CopyDirectory(string oldPath, string newPath, bool overwrite)
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(string.Empty);
            string sep = Path.DirectorySeparatorChar.ToString();
            string alt = Path.AltDirectorySeparatorChar.ToString();
            int length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                string subPath = stack.Pop();
                string sourcePath = Path.Combine(oldPath, subPath);
                string targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (string file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), overwrite);
                }
                foreach (string folder in Directory.GetDirectories(sourcePath))
                {
                    stack.Push(folder.Substring(length));
                }
            }
        }

        /// <summary>
        /// Moves a folder, overwriting the files at the new location if there are matches.
        /// </summary>
        public static void ForceMoveDirectory(string oldPath, string newPath)
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(string.Empty);
            string sep = Path.DirectorySeparatorChar.ToString();
            string alt = Path.AltDirectorySeparatorChar.ToString();
            int length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                string subPath = stack.Pop();
                string sourcePath = Path.Combine(oldPath, subPath);
                string targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (string file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    ForceMove(file, Path.Combine(targetPath, Path.GetFileName(file)));
                }
                foreach (string folder in Directory.GetDirectories(sourcePath))
                {
                    stack.Push(folder.Substring(length));
                }
            }
            Directory.Delete(oldPath, true);
        }

        /// <summary>
        /// If the path already exists, the user is asked to confirm
        /// </summary>
        public static bool ConfirmOverwrite(string path)
        {
            string name = Path.GetFileName(path);
            if (Directory.Exists(path))
            {
                string title = $" {TextHelper.GetString("FlashDevelop.Title.ConfirmDialog")}";
                string message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFolder");
                DialogResult result = MessageBox.Show(PluginBase.MainForm, string.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return result == DialogResult.Yes;
            }

            if (File.Exists(path))
            {
                string title = $" {TextHelper.GetString("FlashDevelop.Title.ConfirmDialog")}";
                string message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFile");
                DialogResult result = MessageBox.Show(PluginBase.MainForm, string.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return result == DialogResult.Yes;
            }
            return true;
        }

        /// <summary>
        /// Checks if a file name matches a search filter mask, eg: filename.jpg matches f*.jpg
        /// </summary>
        /// <param name="fileName">The name of the file to check</param>
        /// <param name="filterMask">The search filter to apply. You can use multiple masks by using ;</param>
        public static bool FileMatchesSearchFilter(string fileName, string filterMask)
        {
            foreach (string mask in filterMask.Split(';'))
            {
                string convertedMask = "^" + Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                Regex regexMask = new Regex(convertedMask, RegexOptions.IgnoreCase);
                if (regexMask.IsMatch(fileName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the bytes contains invalid UTF-8 bytes
        /// </summary>
        public static bool ContainsInvalidUTF8Bytes(byte[] bytes)
        {
            int i;
            int length = bytes.Length;
            for (i = 0; i < length; i++)
            {
                int c = bytes[i];
                if (c > 128)
                {
                    if ((c >= 254)) return true;
                    int bits;
                    if (c >= 252) bits = 6;
                    else if (c >= 248) bits = 5;
                    else if (c >= 240) bits = 4;
                    else if (c >= 224) bits = 3;
                    else if (c >= 192) bits = 2;
                    else return true;
                    if ((i + bits) > length) return true;
                    while (bits > 1)
                    {
                        i++;
                        int b = bytes[i];
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
        public static int GetFileCodepage(string file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.CodePage;
        }

        /// <summary>
        /// Checks if the file contains BOM
        /// </summary>
        public static bool ContainsBOM(string file)
        {
            EncodingFileInfo info = GetEncodingFileInfo(file);
            return info.ContainsBOM;
        }

        /// <summary>
        /// Acquires encoding related info on one read.
        /// </summary>
        public static EncodingFileInfo GetEncodingFileInfo(string file)
        {
            int startIndex = 0;
            EncodingFileInfo info = new EncodingFileInfo();
            try
            {
                if (File.Exists(file))
                {
                    byte[] bytes = File.ReadAllBytes(file);
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
                    int contentLength = bytes.Length - startIndex;
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
        public static List<string> FilterByExisting(List<string> paths, bool logicalDrivesOnly)
        {
            List<string> toCheck = new List<string>(paths);
            if (logicalDrivesOnly)
            {
                DriveInfo[] driveInfo = DriveInfo.GetDrives();
                toCheck = new List<string>(paths);
                toCheck.RemoveAll(delegate(string path)
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
        public int CodePage = -1;
        public string Charset = string.Empty;
        public string Contents = string.Empty;
        public bool ContainsBOM;
        public int BomLength = 0;
    }
}