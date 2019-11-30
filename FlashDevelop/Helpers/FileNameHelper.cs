using System.IO;
using PluginCore;
using PluginCore.Helpers;

namespace FlashDevelop.Helpers
{
    class FileNameHelper
    {
        /// <summary>
        /// Path to the system image file
        /// </summary>
        public static string Images => GetSettingFile("Images.png");

        /// <summary>
        /// Path to the large system image file
        /// </summary>
        public static string Images32 => GetSettingFile("Images32.png");

        /// <summary>
        /// Path to the toolbar file
        /// </summary>
        public static string ToolBar => GetSettingFile("ToolBar.xml");

        /// <summary>
        /// Path to the main menu file
        /// </summary>
        public static string MainMenu => GetSettingFile("MainMenu.xml");

        /// <summary>
        /// Path to the scintilla menu file
        /// </summary>
        public static string ScintillaMenu => GetSettingFile("ScintillaMenu.xml");

        /// <summary>
        /// Path to the tab menu file
        /// </summary>
        public static string TabMenu => GetSettingFile("TabMenu.xml");

        /// <summary>
        /// Path to the session file
        /// </summary>
        public static string SessionData => Path.Combine(PathHelper.SettingDir, "SessionData.fdb");

        /// <summary>
        /// Path to the panel layout file
        /// </summary>
        public static string LayoutData => Path.Combine(PathHelper.SettingDir, "LayoutData.fdl");

        /// <summary>
        /// Path to the setting file
        /// </summary>
        public static string SettingData => Path.Combine(PathHelper.SettingDir, "SettingData.fdb");

        /// <summary>
        /// Path to the shortcut file
        /// </summary>
        public static string ShortcutData => Path.Combine(PathHelper.SettingDir, "ShortcutData.fda");

        /// <summary>
        /// Path to the argument file
        /// </summary>
        public static string UserArgData => Path.Combine(PathHelper.SettingDir, "UserArgData.fda");

        /// <summary>
        /// Path to the recovery directory
        /// </summary>
        public static string RecoveryDir => Path.Combine(PathHelper.SettingDir, "Recovery");

        /// <summary>
        /// Path to the file state directory
        /// </summary>
        public static string FileStateDir => Path.Combine(PathHelper.SettingDir, "FileStates");

        /// <summary>
        /// Selects correct setting file from user dir or app dir.
        /// </summary>
        public static string GetSettingFile(string file)
        {
            var standalone = PluginBase.MainForm.StandaloneMode;
            var appDirSettingFile = Path.Combine(PathHelper.AppDir, "Settings", file);
            var userDirSettingFile = Path.Combine(PathHelper.UserAppDir, "Settings", file);
            if (!standalone && File.Exists(userDirSettingFile)) return userDirSettingFile;
            return appDirSettingFile;
        }

    }
}