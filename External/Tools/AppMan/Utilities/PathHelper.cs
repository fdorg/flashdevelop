// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AppMan.Utilities
{
    public class PathHelper
    {
        /// <summary>
        /// Default path to the log dir.
        /// </summary>
        public static String LOG_DIR = ArgProcessor.ProcessArguments("$(AppDir)");

        /// <summary>
        /// Default path to the apps directory.
        /// </summary>
        public static String APPS_DIR = ArgProcessor.ProcessArguments("$(AppDir)/Apps");

        /// <summary>
        /// Default path to the app help.
        /// </summary>
        public static String HELP_ADR = ArgProcessor.ProcessArguments("$(AppDir)/Help.html");

        /// <summary>
        /// Default path to the config xml file.
        /// </summary>
        public static String CONFIG_ADR = ArgProcessor.ProcessArguments("$(AppDir)/Config.xml");

        /// <summary>
        /// Gets the directory of the app.
        /// </summary>
        public static String GetExeDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

    }

}
