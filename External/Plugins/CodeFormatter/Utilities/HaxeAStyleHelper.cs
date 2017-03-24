using CodeFormatter.Preferences;
using PluginCore.Helpers;
using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeFormatter.Utilities
{
    public class HaxeAStyleHelper
    {
        static string configFile;

        static HaxeAStyleHelper()
        {
            String dataDir = Path.Combine(PathHelper.DataDir, "CodeFormatter");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            configFile = Path.Combine(dataDir, "HaxeAStyleConfig.fdb");
        }

        public static HaxeAStyleOptions LoadOptions()
        {
            HaxeAStyleOptions options = new HaxeAStyleOptions();
            if (File.Exists(configFile))
            {
                Object obj = ObjectSerializer.Deserialize(configFile, options);
                options = new HaxeAStyleOptions((HaxeAStyleOption[])obj);
            }

            AddDefaultOptions(options);

            return options;
        }

        public static void SaveOptions(HaxeAStyleOptions options)
        {
            ObjectSerializer.Serialize(configFile, options.ToArray());
        }

        public static string GetAStyleArguments()
        {
            HaxeAStyleOptions options = LoadOptions();
            return String.Join(" ", options.ToStringArray());
        }

        public static void AddDefaultOptions(HaxeAStyleOptions options)
        {
            if (!options.Exists("--mode"))
            {
                options.Add(new HaxeAStyleOption("--mode", "cs"));
            }
            
            //Not supported by old version of AStyle
            //if (!options.Exists("--options"))
            //{
            //    options.Add(new HaxeAStyleOption("--options", "none"));
            //}

        }
    }
}
