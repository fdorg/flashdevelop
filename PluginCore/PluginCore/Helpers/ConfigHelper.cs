// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;

namespace PluginCore.Helpers
{
    public class SimpleIni : Dictionary<string, Dictionary<string, string>>
    {
        public Dictionary<string, string> Flatten()
        {
            var result = new Dictionary<string, string>();
            foreach (var section in this)
            {
                foreach (var entry in section.Value)
                    result[entry.Key] = entry.Value;
            }
            return result;
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

            var result = new SimpleIni();
            var config = new Dictionary<string, string>();
            var currentSection = "Default";
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var rawLine in lines)
                {
                    var line = rawLine.Trim();
                    if (line.Length < 2 || line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith(";", StringComparison.Ordinal)) continue;
                    if (line.StartsWith("[", StringComparison.Ordinal))
                    {
                        result.Add(currentSection, config);
                        config = new Dictionary<string, string>();
                        currentSection = line.Substring(1, line.Length - 2);
                    }
                    else
                    {
                        var entry = line.Split(new[] { '=' }, 2);
                        if (entry.Length < 2) continue;
                        config[entry[0].Trim()] = entry[1].Trim();
                    }
                }
            }
            result.Add(currentSection, config);

            if (cache) Cache[configPath] = result;
            return result;
        }
    }
}