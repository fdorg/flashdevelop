using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var fo = new InteropSHFileOperation
                {
                    wFunc = InteropSHFileOperation.FO_Func.FO_DELETE,
                    fFlags =
                    {
                        FOF_ALLOWUNDO = true, FOF_NOCONFIRMATION = true, FOF_NOERRORUI = true, FOF_SILENT = true
                    },
                    pFrom = path
                };
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
            var info = GetEncodingFileInfo(file);
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
            var useSkipBomWriter = encoding == Encoding.UTF8 && !saveBOM;
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
            var result = original;
            var counter = 0;
            var folder = Path.GetDirectoryName(original);
            var filename = Path.GetFileNameWithoutExtension(original);
            var extension = Path.GetExtension(original);
            while (File.Exists(result))
            {
                counter++;
                var fullname = filename + " (" + counter + ")" + extension;
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
            catch
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
            var stack = new Stack<string>();
            stack.Push(string.Empty);
            var sep = Path.DirectorySeparatorChar.ToString();
            var alt = Path.AltDirectorySeparatorChar.ToString();
            var length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                var subPath = stack.Pop();
                var sourcePath = Path.Combine(oldPath, subPath);
                var targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (var file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), overwrite);
                }
                foreach (var folder in Directory.GetDirectories(sourcePath))
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
            var stack = new Stack<string>();
            stack.Push(string.Empty);
            var sep = Path.DirectorySeparatorChar.ToString();
            var alt = Path.AltDirectorySeparatorChar.ToString();
            var length = oldPath.EndsWithOrdinal(sep) || oldPath.EndsWithOrdinal(alt) ? oldPath.Length : oldPath.Length + 1;
            while (stack.Count > 0)
            {
                var subPath = stack.Pop();
                var sourcePath = Path.Combine(oldPath, subPath);
                var targetPath = Path.Combine(newPath, subPath);
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                foreach (var file in Directory.GetFiles(sourcePath, "*.*"))
                {
                    ForceMove(file, Path.Combine(targetPath, Path.GetFileName(file)));
                }
                foreach (var folder in Directory.GetDirectories(sourcePath))
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
            var name = Path.GetFileName(path);
            if (Directory.Exists(path))
            {
                var title = $" {TextHelper.GetString("FlashDevelop.Title.ConfirmDialog")}";
                var message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFolder");
                var result = MessageBox.Show(PluginBase.MainForm, string.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return result == DialogResult.Yes;
            }

            if (File.Exists(path))
            {
                var title = $" {TextHelper.GetString("FlashDevelop.Title.ConfirmDialog")}";
                var message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFile");
                var result = MessageBox.Show(PluginBase.MainForm, string.Format(message, name, "\n"), title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
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
            foreach (var mask in filterMask.Split(';'))
            {
                var convertedMask = "^" + Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                var regexMask = new Regex(convertedMask, RegexOptions.IgnoreCase);
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
            var info = GetEncodingFileInfo(file);
            return info.CodePage;
        }

        /// <summary>
        /// Checks if the file contains BOM
        /// </summary>
        public static bool ContainsBOM(string file)
        {
            var info = GetEncodingFileInfo(file);
            return info.ContainsBOM;
        }

        /// <summary>
        /// Acquires encoding related info on one read.
        /// </summary>
        public static EncodingFileInfo GetEncodingFileInfo(string file)
        {
            var startIndex = 0;
            var info = new EncodingFileInfo();
            try
            {
                if (File.Exists(file))
                {
                    var bytes = File.ReadAllBytes(file);
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
                            var detector = new Ude.CharsetDetector();
                            detector.Feed(bytes, 0, bytes.Length); detector.DataEnd();
                            if (detector.Charset != null)
                            {
                                var encoding = Encoding.GetEncoding(detector.Charset);
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
                    if (bytes.Length > 0 && bytes.Length > startIndex)
                    {
                        var contentLength = bytes.Length - startIndex;
                        var encoding = Encoding.GetEncoding(info.CodePage);
                        info.Contents = encoding.GetString(bytes, startIndex, contentLength);
                    }
                }
            }
            catch
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
            var toCheck = new List<string>(paths);
            if (logicalDrivesOnly)
            {
                var driveInfo = DriveInfo.GetDrives();
                toCheck = new List<string>(paths);
                toCheck.RemoveAll(path => driveInfo.All(drive => !path.StartsWithOrdinal(drive.RootDirectory.ToString())));
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
        public override byte[] GetPreamble() => new byte[] { 0x2B, 0x2F, 0x76, 0x38, 0x2D };
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
        public int BomLength;
    }
}