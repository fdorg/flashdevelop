using System;
using System.Collections.Generic;
using System.IO;

namespace PluginCore.Helpers
{
    public class SimpleIni 
        : Dictionary<string, Dictionary<string, string>>
    {
        public Dictionary<string, string> Flatten()
        {
            var flat = new Dictionary<string, string>();
            foreach (var section in this)
            {
                foreach (var entry in section.Value)
                    flat[entry.Key] = entry.Value;
            }
            return flat;
        }
    }

    public class ConfigHelper
    {
        public static Dictionary<string, SimpleIni> Cache = new Dictionary<string, SimpleIni>();

        /// <summary>
        /// Read a simple config file and returns its variables as a collection of dictionaries.
        /// </summary>
        public static SimpleIni Parse(string configPath, bool cache)
        {
            if (cache && Cache.ContainsKey(configPath)) return Cache[configPath];

            SimpleIni ini = new SimpleIni();
            Dictionary<string, string> config = new Dictionary<string, string>();
            string currentSection = "Default";
            if (File.Exists(configPath))
            {
                string[] lines = File.ReadAllLines(configPath);
                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();
                    if (line.Length < 2 || line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith(";", StringComparison.Ordinal)) continue;
                    if (line.StartsWith("[", StringComparison.Ordinal))
                    {
                        ini.Add(currentSection, config);
                        config = new Dictionary<string, string>();
                        currentSection = line.Substring(1, line.Length - 2);
                    }
                    else
                    {
                        string[] entry = line.Split(new char[] { '=' }, 2);
                        if (entry.Length < 2) continue;
                        config[entry[0].Trim()] = entry[1].Trim();
                    }
                }
            }
            ini.Add(currentSection, config);

            if (cache) Cache[configPath] = ini;
            return ini;
        }
    }
}
