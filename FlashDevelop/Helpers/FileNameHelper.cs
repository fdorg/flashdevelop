using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore.Helpers;

namespace FlashDevelop.Helpers
{
    class FileNameHelper
    {
        /// <summary>
        /// Path to the system image file
        /// </summary>
        public static String Images
        {
            get
            {
                return GetSettingFile("Images.png");
            }
        }

        /// <summary>
        /// Path to the large system image file
        /// </summary>
        public static String Images32
        {
            get
            {
                return GetSettingFile("Images32.png");
            }
        }

        /// <summary>
        /// Path to the toolbar file
        /// </summary>
        public static String ToolBar
        {
            get
            {
                return GetSettingFile("ToolBar.xml");
            }
        }

        /// <summary>
        /// Path to the main menu file
        /// </summary>
        public static String MainMenu
        {
            get 
            {
                return GetSettingFile("MainMenu.xml");
            }
        }

        /// <summary>
        /// Path to the scintilla menu file
        /// </summary>
        public static String ScintillaMenu
        {
            get
            {
                return GetSettingFile("ScintillaMenu.xml");
            }
        }

        /// <summary>
        /// Path to the tab menu file
        /// </summary>
        public static String TabMenu
        {
            get
            {
                return GetSettingFile("TabMenu.xml");
            }
        }

        /// <summary>
        /// Path to the session file
        /// </summary>
        public static String SessionData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "SessionData.fdb");
            }
        }

        /// <summary>
        /// Path to the panel layout file
        /// </summary>
        public static String LayoutData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "LayoutData.fdl");
            }
        }

        /// <summary>
        /// Path to the setting file
        /// </summary>
        public static String SettingData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "SettingData.fdb");
            }
        }

        /// <summary>
        /// Path to the shortcut file
        /// </summary>
        public static String ShortcutData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "ShortcutData.fda");
            }
        }

        /// <summary>
        /// Path to the argument file
        /// </summary>
        public static String UserArgData
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "UserArgData.fda");
            }
        }

        /// <summary>
        /// Path to the recovery directory
        /// </summary>
        public static String RecoveryDir
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "Recovery");
            }
        }

        /// <summary>
        /// Path to the file state directory
        /// </summary>
        public static String FileStateDir
        {
            get
            {
                return Path.Combine(PathHelper.SettingDir, "FileStates");
            }
        }
        
        /// <summary>
        /// Selects correct setting file from user dir or app dir.
        /// </summary>
        public static String GetSettingFile(String file)
        {
            Boolean standalone = Globals.MainForm.StandaloneMode;
            String appDirSettingFile = Path.Combine(Path.Combine(PathHelper.AppDir, "Settings"), file);
            String userDirSettingFile = Path.Combine(Path.Combine(PathHelper.UserAppDir, "Settings"), file);
            if (!standalone && File.Exists(userDirSettingFile)) return userDirSettingFile;
            else return appDirSettingFile;
        }

    }

}
