using System;
using System.IO;
using PluginCore.Helpers;

namespace FlashDevelop.Helpers
{
    class FileNameHelper
    {
        /// <summary>
        /// Path to the system image file
        /// </summary>
        public static string Images
        {
            get
            {
                return GetSettingFile("Images.png");
            }
        }

        /// <summary>
        /// Path to the large system image file
        /// </summary>
        public static string Images32
        {
            get
            {
                return GetSettingFile("Images32.png");
            }
        }

        /// <summary>
        /// Path to the toolbar file
        /// </summary>
        public static string ToolBar
        {
            get
            {
                return GetSettingFile("ToolBar.xml");
            }
        }

        /// <summary>
        /// Path to the main menu file
        /// </summary>
        public static string MainMenu
        {
            get 
            {
                return GetSettingFile("MainMenu.xml");
            }
        }

        /// <summary>
        /// Path to the scintilla menu file
        /// </summary>
        public static string ScintillaMenu
        {
            get
            {
                return GetSettingFile("ScintillaMenu.xml");
            }
        }

        /// <summary>
        /// Path to the tab menu file
        /// </summary>
        public static string TabMenu
        {
            get
            {
                return GetSettingFile("TabMenu.xml");
            }
        }

        /// <summary>
        /// Path to the session file
        /// </summary>
        public static string SessionData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "SessionData.fdb");
            }
        }

        /// <summary>
        /// Path to the panel layout file
        /// </summary>
        public static string LayoutData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "LayoutData.fdl");
            }
        }

        /// <summary>
        /// Path to the setting file
        /// </summary>
        public static string SettingData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "SettingData.fdb");
            }
        }

        /// <summary>
        /// Path to the shortcut file
        /// </summary>
        public static string ShortcutData
        {
            get
            {
                //Alternative: Path.Combine(PathHelper.ShortcutsDir, "CURRENT");
                return Path.Combine(PathHelper.SettingDir, "ShortcutData.fda");
            }
        }

        /// <summary>
        /// Path to the argument file
        /// </summary>
        public static string UserArgData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "UserArgData.fda");
            }
        }

        /// <summary>
        /// Path to the recovery directory
        /// </summary>
        public static string RecoveryDir
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "Recovery");
            }
        }

        /// <summary>
        /// Path to the file state directory
        /// </summary>
        public static string FileStateDir
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "FileStates");
            }
        }
        
        /// <summary>
        /// Selects correct setting file from user dir or app dir.
        /// </summary>
        public static string GetSettingFile(string file)
        {
            var standalone = Globals.MainForm.StandaloneMode;
            var appDirSettingFile = Path.Combine(PathHelper.AppDir, "Settings", file);
            var userDirSettingFile = Path.Combine(PathHelper.UserAppDir, "Settings", file);
            if (!standalone && File.Exists(userDirSettingFile)) return userDirSettingFile;
            return appDirSettingFile;
        }

    }

}
