// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    internal class RecoveryManager
    {
        /// <summary>
        /// Removes the saved backup file from the restoration directory
        /// </summary>
        public static void RemoveTemporaryFile(string file)
        {
            try
            {
                string name = ConvertToFileName(file) + ".bak";
                string path = Path.Combine(FileNameHelper.RecoveryDir, name);
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
        public static void SaveTemporaryFile(string file, string text, Encoding encoding)
        {
            try
            {
                string name = ConvertToFileName(file) + ".bak";
                string recoveryDir = FileNameHelper.RecoveryDir;
                if (!Directory.Exists(recoveryDir)) Directory.CreateDirectory(recoveryDir);
                string path = Path.Combine(recoveryDir, name); // Create full file path
                int lineEndMode = LineEndDetector.DetectNewLineMarker(text, (int)PluginBase.MainForm.Settings.EOLMode);
                string lineEndMarker = LineEndDetector.GetNewLineMarker(lineEndMode);
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
        static string ConvertToFileName(string path)
        {
            try
            {
                return HashCalculator.CalculateSHA1(path);
            }
            catch
            {
                var separator = Path.DirectorySeparatorChar;
                var filename = path.Replace(separator, '.');
                filename = filename.Replace(' ', '_');
                return filename;
            }
        }
    }
}