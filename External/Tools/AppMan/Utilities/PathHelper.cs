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
        /// Default path to the data dir.
        /// </summary>
        public static String LOG_DIR = GetExeDirectory();

        /// <summary>
        /// Default path to the app help.
        /// </summary>
        public static String HELP_ADR = "http://www.flashdevelop.org/wikidocs/";

        /// <summary>
        /// Default path to the config xml file.
        /// </summary>
        public static String CONFIG_ADR = "http://www.flashdevelop.org/appman.xml";

        /// <summary>
        /// Default path to the item archive.
        /// </summary>
        public static String ARCHIVE_DIR = Path.Combine(GetExeDirectory(), "Archive");

        /// <summary>
        /// Gets the directory of the app.
        /// </summary>
        public static String GetExeDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

    }

}
