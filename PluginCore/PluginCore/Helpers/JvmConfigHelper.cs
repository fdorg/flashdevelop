using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PluginCore.Helpers
{
    public class JvmConfigHelper
    {
        public static Dictionary<string, Dictionary<string, string>> Cache = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Read a jvm.config file and returns its variables as a Dictionary.
        /// </summary>
        public static Dictionary<string, string> ReadConfig(string configPath)
        {
            if (configPath is null) configPath = "";
            string hash = configPath;
            if (Cache.ContainsKey(hash)) return Cache[hash];
            if (Directory.Exists(configPath))
            {
                var jvmconfig = Path.Combine(configPath, "bin", "jvm.config");
                var buildproperties = Path.Combine(configPath, "build.properties");

                if (File.Exists(jvmconfig))
                    configPath = jvmconfig;
                else if (File.Exists(buildproperties))
                    configPath = buildproperties;
            }

            Dictionary<string, string> config = ConfigHelper.Parse(configPath, false).Flatten();
            Cache[hash] = config;

            // default values
            if (!config.ContainsKey("java.home")) config["java.home"] = "";
            else config["java.home"] = config["java.home"].Trim('"', '\'', ' ', '\t');

            string args = "-Dsun.io.useCanonCaches=false -Xms32m -Xmx512m";
            if (config.ContainsKey("java.args")) args = config["java.args"];
            else if (config.ContainsKey("jvm.args")) args = config["jvm.args"];

            args = ExpandArguments(args, config, 0);

            // add language if not specified
            if (!args.Contains("-Duser.language"))
            {
                args += " -Duser.language=en -Duser.region=US";
            }

            // flex needs old Java 6 sort
            if (!args.Contains("-Djava.util.Arrays.useLegacyMergeSort"))
            {
                args += " -Djava.util.Arrays.useLegacyMergeSort=true";
            }

            config["java.args"] = args;
            return config;
        }

        private static string ExpandArguments(string value, IDictionary<string, string> config, int depth)
        {
            while (value.IndexOf("${", StringComparison.Ordinal) is { } start && start >= 0)
            {
                int end = value.IndexOf('}', start);
                if (end < start) return value;
                string key = value.Substring(start + 2, end - start - 2).Trim();
                string eval = config.ContainsKey(key) ? config[key] : "";
                if (!string.IsNullOrEmpty(eval) && depth < 10) eval = ExpandArguments(eval, config, depth + 1);
                value = value.Substring(0, start) + eval + value.Substring(end + 1);
            }
            return value;
        }

        public static string GetJavaEXE() => GetJavaEXE(null, null);

        public static string GetJavaEXE(Dictionary<string, string> jvmConfig) => GetJavaEXE(jvmConfig, null);

        public static string GetJavaEXE(Dictionary<string, string> jvmConfig, string flexSdkPath)
        {
            var home = GetJavaHome(jvmConfig, flexSdkPath);
            if (!string.IsNullOrEmpty(home) && !home.StartsWith("%", StringComparison.Ordinal))
            {
                return Path.Combine(home, "bin", "java");
            }
            return "java";
        }

        public static string GetJavaHome(Dictionary<string, string> jvmConfig, string flexSdkPath)
        {
            string home = null;
            if (jvmConfig != null && jvmConfig.ContainsKey("java.home"))
            {
                home = ResolvePath(jvmConfig["java.home"], flexSdkPath);
            }
            if (home is null)
            {
                home = Environment.ExpandEnvironmentVariables("%JAVA_HOME%");
                if (home.StartsWith("%", StringComparison.Ordinal)) home = null;
            }
            return home;
        }

        // Duplicated from 'PluginCore.PathHelper.ResolvePath()'
        // because JvmConfigHelper is used in external tool 'FDBuild'
        private static string ResolvePath(string path, string relativeTo)
        {
            if (string.IsNullOrEmpty(path)) return null;
            bool isPathNetworked = path.StartsWith("\\\\", StringComparison.Ordinal) || path.StartsWith("//", StringComparison.Ordinal);
            bool isPathAbsSlashed = (path.StartsWith("\\", StringComparison.Ordinal) || path.StartsWith("/", StringComparison.Ordinal)) && !isPathNetworked;
            if (isPathAbsSlashed) path = Path.GetPathRoot(AppDir) + path.Substring(1);
            if (Path.IsPathRooted(path) || isPathNetworked) return path;
            string resolvedPath;
            if (relativeTo != null)
            {
                resolvedPath = Path.Combine(relativeTo, path);
                if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            }
            resolvedPath = Path.Combine(AppDir, path);
            if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            return null;
        }

        private static string AppDir => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }

}
