// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public static List<string> IllegalFolderNames = new List<string> { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        /// <summary>
        /// Checks if a directory is empty
        /// </summary>
        public static bool IsDirectoryEmpty(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return Directory.GetFileSystemEntries(path).Length == 0;
                }

                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Checks if the directory name is illegal
        /// </summary>
        public static bool IsIllegalFolderName(string name)
        {
            return IllegalFolderNames.Contains(name.ToUpper());
        }

        /// <summary>
        /// Gets a name of the folder 
        /// </summary> 
        public static string GetFolderName(string path)
        {
            try
            {
                string dir = Path.GetFullPath(path);
                char separator = Path.DirectorySeparatorChar;
                string[] chunks = dir.Split(separator);
                return chunks[chunks.Length - 1];
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Ensures that the folder name is unique by adding a number to it
        /// </summary>
        public static string EnsureUniquePath(string original)
        {
            try
            {
                int counter = 0;
                string result = original;
                string folder = Path.GetDirectoryName(result);
                string folderName = GetFolderName(result);
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
                return string.Empty;
            }
        }

        /// <summary>
        /// Copies the folder structure recursively
        /// </summary> 
        public static void CopyFolder(string source, string destination)
        {
            try
            {
                string[] files = Directory.GetFileSystemEntries(source);
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
                foreach (string file in files)
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
