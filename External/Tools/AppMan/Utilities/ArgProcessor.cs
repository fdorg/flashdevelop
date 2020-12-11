using System;
using System.IO;

namespace AppMan.Utilities
{
    public class ArgProcessor
    {
        /// <summary>
        /// Processes the internal arguments in a string.
        /// </summary>
        public static string ProcessArguments(string data)
        {
            data = data.Replace("$(Quote)", "\"");
            data = data.Replace("$(AppDir)", PathHelper.GetExeDirectory());
            #if FLASHDEVELOP
            var local = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\.local");
            local = Path.GetFullPath(local); /* Fix weird path */
            if (!File.Exists(local)) 
            {
                var userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fdUserPath = Path.Combine(userAppDir, MainForm.DISTRO_NAME);
                data = data.Replace("$(BaseDir)", fdUserPath);
            }
            else data = data.Replace("$(BaseDir)", Path.GetDirectoryName(local));
            #endif
            data = Environment.ExpandEnvironmentVariables(data);
            return data;
        }

    }

}

