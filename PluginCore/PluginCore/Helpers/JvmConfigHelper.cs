using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PluginCore.Helpers
{
    public class JvmConfigHelper
    {
        static public Dictionary<string, Dictionary<string, string>> Cache = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Read a jvm.config file and returns its variables as a Dictionnary.
        /// </summary>
        public static Dictionary<string, string> ReadConfig(string configPath)
        {
            if (configPath == null) configPath = "";
            string hash = configPath;
            if (Cache.ContainsKey(hash)) return Cache[hash];
            if (Directory.Exists(configPath))
            {
                if (File.Exists(Path.Combine(configPath, "bin\\jvm.config")))
                    configPath = Path.Combine(configPath, "bin\\jvm.config");
                else if (File.Exists(Path.Combine(configPath, "build.properties")))
                    configPath = Path.Combine(configPath, "build.properties");
            }

            Dictionary<string, string> config = ConfigHelper.Parse(configPath, false).Flatten();
            Cache[hash] = config;

            // default values
            if (!config.ContainsKey("java.home")) config["java.home"] = "";
            else config["java.home"] = config["java.home"].Trim(new char[] { '"', '\'', ' ', '\t' });

            string args = "-Dsun.io.useCanonCaches=false -Xms32m -Xmx512m";
            if (config.ContainsKey("java.args")) args = config["java.args"];
            else if (config.ContainsKey("jvm.args")) args = config["jvm.args"];

            args = ExpandArguments(args, config, 0);

            // add language if not specified
            if (args.IndexOf("-Duser.language") < 0)
            {
                args += " -Duser.language=en -Duser.region=US";
            }

            // flex needs old Java 6 sort
            if (args.IndexOf("-Djava.util.Arrays.useLegacyMergeSort") < 0)
            {
                args += " -Djava.util.Arrays.useLegacyMergeSort=true";
            }

            config["java.args"] = args;
            return config;
        }

        private static string ExpandArguments(string value, Dictionary<string, string> config, int depth)
        {
            while (value.IndexOf("${") >= 0)
            {
                int start = value.IndexOf("${");
                int end = value.IndexOf("}", start);
                if (end < start) return value;
                string key = value.Substring(start + 2, end - start - 2).Trim();
                string eval = config.ContainsKey(key) ? config[key] : "";
                if (!string.IsNullOrEmpty(eval) && depth < 10) eval = ExpandArguments(eval, config, depth + 1);
                value = value.Substring(0, start) + eval + value.Substring(end + 1);
            }
            return value;
        }

        public static string GetJavaEXE()
        {
            return GetJavaEXE(null, null);
        }
        public static string GetJavaEXE(Dictionary<string, string> jvmConfig)
        {
            return GetJavaEXE(jvmConfig, null);
        }
        public static string GetJavaEXE(Dictionary<string, string> jvmConfig, string flexSdkPath)
        {
            string defaultExe = "java.exe";
            string home = GetJavaHome(jvmConfig, flexSdkPath);
            if (!String.IsNullOrEmpty(home) && !home.StartsWith("%"))
            {
                return Path.Combine(home, "bin\\java.exe");
            }
            return defaultExe;
        }

        public static string GetJavaHome(Dictionary<string, string> jvmConfig, string flexSdkPath)
        {
            string home = null;
            if (jvmConfig != null && jvmConfig.ContainsKey("java.home"))
            {
                home = ResolvePath(jvmConfig["java.home"], flexSdkPath, true);
            }
            if (home == null)
            {
                home = Environment.ExpandEnvironmentVariables("%JAVA_HOME%");
                if (home.StartsWith("%")) home = null;
            }
            return home;
        }

        // Duplicated from 'PluginCore.PathHelper.ResolvePath()'
        // because JvmConfigHelper is used in external tool 'FDBuild'
        private static string ResolvePath(String path, String relativeTo, Boolean checkResolvedPathExisting)
        {
            if (string.IsNullOrEmpty(path)) return null;
            Boolean isPathNetworked = path.StartsWith("\\\\") || path.StartsWith("//");
            Boolean isPathAbsSlashed = (path.StartsWith("\\") || path.StartsWith("/")) && !isPathNetworked;
            if (isPathAbsSlashed) path = Path.GetPathRoot(AppDir) + path.Substring(1);
            if (Path.IsPathRooted(path) || isPathNetworked) return path;
            String resolvedPath;
            if (relativeTo != null)
            {
                resolvedPath = Path.Combine(relativeTo, path);
                if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            }
            resolvedPath = Path.Combine(AppDir, path);
            if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            return null;
        }

        private static String AppDir
        {
            get
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

    }

}
