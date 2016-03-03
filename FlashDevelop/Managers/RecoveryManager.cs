using System;
using System.IO;
using System.Text;
using FlashDevelop.Helpers;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    class RecoveryManager
    {
        /// <summary>
        /// Removes the saved backup file from the restoration directory
        /// </summary>
        public static void RemoveTemporaryFile(String file)
        {
            try
            {
                String name = ConvertToFileName(file) + ".bak";
                String path = Path.Combine(FileNameHelper.RecoveryDir, name);
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves a backup file to the recovery directory
        /// </summary>
        public static void SaveTemporaryFile(String file, String text, Encoding encoding)
        {
            try
            {
                String name = ConvertToFileName(file) + ".bak";
                String recoveryDir = FileNameHelper.RecoveryDir;
                if (!Directory.Exists(recoveryDir)) Directory.CreateDirectory(recoveryDir);
                String path = Path.Combine(recoveryDir, name); // Create full file path
                Int32 lineEndMode = LineEndDetector.DetectNewLineMarker(text, (Int32)Globals.Settings.EOLMode);
                String lineEndMarker = LineEndDetector.GetNewLineMarker(lineEndMode);
                text = file + lineEndMarker + lineEndMarker + text;
                FileHelper.WriteFile(path, text, encoding);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Converts a path to a valid file name
        /// </summary>
        private static String ConvertToFileName(String path)
        {
            try
            {
                return HashCalculator.CalculateSHA1(path);
            }
            catch
            {
                String filename = Path.GetFullPath(path);
                String separator = Path.DirectorySeparatorChar.ToString();
                filename = path.Replace(separator, ".");
                filename = path.Replace(" ", "_");
                return filename;
            }
        }

    }

}
