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
            data = Environment.ExpandEnvironmentVariables(data);
            return data;
        }

    }

}

