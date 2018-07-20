using System;
using System.Collections.Generic;
using System.IO;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
    public class FolderHelper
    {
        /// <summary>
        /// List of illegal directory names
        /// </summary>
        public static List<String> IllegalFolderNames = new List<String>(){ "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        /// <summary>
        /// Checks if a directory is empty
        /// </summary>
        public static Boolean IsDirectoryEmpty(String path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return Directory.GetFileSystemEntries(path).Length == 0;
                }
                else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Checks if the directory name is illegal
        /// </summary>
        public static Boolean IsIllegalFolderName(String name)
        {
            return IllegalFolderNames.Contains(name.ToUpper());
        }

        /// <summary>
        /// Gets a name of the folder 
        /// </summary> 
        public static String GetFolderName(String path)
        {
            try
            {
                String dir = Path.GetFullPath(path);
                Char separator = Path.DirectorySeparatorChar;
                String[] chunks = dir.Split(separator);
                return chunks[chunks.Length - 1];
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Ensures that the folder name is unique by adding a number to it
        /// </summary>
        public static String EnsureUniquePath(String original)
        {
            try
            {
                Int32 counter = 0;
                String result = original;
                String folder = Path.GetDirectoryName(result);
                String folderName = GetFolderName(result);
                while (Directory.Exists(result))
                {
                    counter++;
                    result = Path.Combine(folder, folderName + " (" + counter + ")");
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Copies the folder structure recursively
        /// </summary> 
        public static void CopyFolder(String source, String destination)
        {
            try
            {
                String[] files = Directory.GetFileSystemEntries(source);
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
                foreach (String file in files)
                {
                    if (Directory.Exists(file)) CopyFolder(file, Path.Combine(destination, Path.GetFileName(file)));
                    else File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

}
