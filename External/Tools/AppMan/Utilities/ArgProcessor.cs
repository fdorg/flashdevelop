using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AppMan.Utilities
{
    public class ArgProcessor
    {
        /// <summary>
        /// Processes the internal argruments in a string.
        /// </summary>
        public static String ProcessArguments(String data)
        {
            data = data.Replace("$(Quote)", "\"");
            data = data.Replace("$(AppDir)", PathHelper.GetExeDirectory());
            #if FLASHDEVELOP
            String local = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\.local");
            local = Path.GetFullPath(local); /* Fix weird path */
            if (!File.Exists(local)) 
            {
                String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                String fdUserPath = Path.Combine(userAppDir, "FlashDevelop");
                data = data.Replace("$(BaseDir)", fdUserPath);
            }
            else data = data.Replace("$(BaseDir)", Path.GetDirectoryName(local));
            #endif
            data = Environment.ExpandEnvironmentVariables(data);
            return data;
        }

    }

}

