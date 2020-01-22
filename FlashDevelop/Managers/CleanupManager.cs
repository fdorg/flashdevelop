﻿using System;
using System.IO;
using System.Text;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    internal class CleanupManager
    {
        const string coloringStart = "<!-- COLORING_START -->";
        const string coloringEnd = "<!-- COLORING_END -->";

        /// <summary>
        /// Reverts the language config files fully or keeping the coloring.
        /// </summary>
        public static void RevertConfiguration(bool everything)
        {
            string appLanguageDir = Path.Combine(PathHelper.AppDir, "Settings", "Languages");
            string userLanguageDir = Path.Combine(PathHelper.SettingDir, "Languages");
            if (everything) FolderHelper.CopyFolder(appLanguageDir, userLanguageDir);
            else
            {
                string[] userFiles = Directory.GetFiles(userLanguageDir);
                foreach (string userFile in userFiles)
                {
                    string appFile = Path.Combine(appLanguageDir, Path.GetFileName(userFile));
                    if (File.Exists(appFile))
                    {
                        try
                        {
                            string appFileContents = FileHelper.ReadFile(appFile);
                            string userFileContents = FileHelper.ReadFile(userFile);
                            int appFileColoringStart = appFileContents.IndexOfOrdinal(coloringStart);
                            int appFileColoringEnd = appFileContents.IndexOfOrdinal(coloringEnd);
                            int userFileColoringStart = userFileContents.IndexOfOrdinal(coloringStart);
                            int userFileColoringEnd = userFileContents.IndexOfOrdinal(coloringEnd);
                            string replaceTarget = appFileContents.Substring(appFileColoringStart, appFileColoringEnd - appFileColoringStart + coloringEnd.Length);
                            string replaceContent = userFileContents.Substring(userFileColoringStart, userFileColoringEnd - userFileColoringStart + coloringEnd.Length);
                            string finalContent = appFileContents.Replace(replaceTarget, replaceContent);
                            FileHelper.WriteFile(userFile, finalContent, Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            string desc = TextHelper.GetString("Info.ColoringTagsMissing");
                            ErrorManager.ShowError(desc, ex);
                        }
                    }
                }
            }
        }
    }
}
