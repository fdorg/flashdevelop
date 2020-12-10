using System.IO;
using System.Windows.Forms;

namespace AppMan.Utilities
{
    public class PathHelper
    {
        /// <summary>
        /// Default path to the log dir.
        /// </summary>
        public static string LOG_DIR = ArgProcessor.ProcessArguments("$(AppDir)");

        /// <summary>
        /// Default path to the apps directory.
        /// </summary>
        public static string APPS_DIR = ArgProcessor.ProcessArguments("$(AppDir)/Apps");

        /// <summary>
        /// Default path to the app help.
        /// </summary>
        public static string HELP_ADR = ArgProcessor.ProcessArguments("$(AppDir)/Help.html");

        /// <summary>
        /// Default path to the config xml file.
        /// </summary>
        public static string CONFIG_ADR = ArgProcessor.ProcessArguments("$(AppDir)/Config.xml");

        /// <summary>
        /// Gets the directory of the app.
        /// </summary>
        public static string GetExeDirectory() => Path.GetDirectoryName(Application.ExecutablePath);
    }
}