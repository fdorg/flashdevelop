using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace PluginCore
{
    // When updating, update also AirProperties plugin!

    // This class must not have any dependencies

    public class PlatformData
    {
        public static String DEFAULT_NME_VERSION = "3.0";
        public static String DEFAULT_AIR_VERSION = "14.0";
        public static String DEFAULT_AIR_MOBILE_VERSION = "14.0";
        public static String DEFAULT_FLASH_VERSION = "14.0";
        public static String[] AIR_VERSIONS = new String[] { "1.5", "2.0", "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.9", "4.0", "13.0", "14.0" };
        public static String[] AIR_MOBILE_VERSIONS = new String[] { "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.9", "4.0", "13.0", "14.0" };
        public static String[] FLASH_LEGACY_VERSIONS = new String[] { "6.0", "7.0", "8.0", "9.0", "10.0", "10.1", "10.2", "10.3", "11.0", "11.1", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8", "11.9", "12.0", "13.0", "14.0" };
        public static String[] FLASH_VERSIONS = new String[] { "9.0", "10.0", "10.1", "10.2", "10.3", "11.0", "11.1", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8", "11.9", "12.0", "13.0", "14.0" };
        public static String[] SWF_VERSIONS = new String[] { "9", "10", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" };
        public static String[] NME_TARGETS = new String[] { "flash", "html5", "windows", "neko", "android", "webos", "blackberry" };
        public static String[] NME_VERSIONS = new String[] { "3.0" };

        public static Dictionary<String, PlatformTarget> PlatformTargets;
        public static Dictionary<String, PlatformLanguage> PlatformLanguages;

        public static string ResolveFlashPlayerVersion(string otherPlatform, string version)
        {
            var flashTarget = PluginCore.PlatformData.PlatformTargets["Flash Player"];
            if (PlatformTargets.ContainsKey(otherPlatform))
            {
                var otherTarget = PluginCore.PlatformData.PlatformTargets[otherPlatform];
                foreach (var otherVersion in otherTarget.Versions)
                    if (otherVersion.Value == version)
                    {
                        foreach (var flashVersion in flashTarget.Versions)
                            if (flashVersion.SwfVersion == otherVersion.SwfVersion) return flashVersion.Value;
                    }
            }
            // default to last FP
            return flashTarget.LastVersion.Value;
        }

        public static string ResolveSwfVersion(string platformName, string version)
        {
            var platform = PluginCore.PlatformData.PlatformTargets[platformName];
            foreach (var platformVersion in platform.Versions)
                if (platformVersion.Value == version)
                    return platformVersion.SwfVersion;
            // default to last FP
            return platform.LastVersion.SwfVersion;
        }

        #region platform config loading

        public static void Load(string path)
        {
            if (!System.IO.File.Exists(path)) return;

            var xml = new XmlDocument();
            xml.Load(path);
            foreach (XmlNode node in xml.ChildNodes)
            {
                if (node.Name == "data")
                {
                    ParseData(node);
                    break;
                }
            }
        }

        private static void ParseData(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name == "platforms") ParsePlatforms(node);
                if (node.Name == "languages") ParseLanguages(node);
            }
        }

        private static void ParseLanguages(XmlNode languages)
        {
            PlatformLanguages = new Dictionary<String, PlatformLanguage>();
            if (PlatformTargets == null || PlatformTargets.Count == 0)
            {
                var defaultVersion = new PlatformVersion { Value = "0.0" };
                var custom = new PlatformTarget {
                    Name = "Custom",
                    Versions = new List<PlatformVersion> { defaultVersion },
                    LastVersion = defaultVersion
                };
                PlatformTargets.Add(custom.Name, custom);
                return;
            }

            foreach (XmlNode node in languages.ChildNodes)
            {
                if (node.Name == "language")
                {
                    var lang = new PlatformLanguage {
                        Name = node.Attributes["name"].Value,
                        Targets = ParseTargets(node),
                        RawData = node
                    };
                    PlatformLanguages.Add(lang.Name, lang);
                }
            }
        }

        private static List<PlatformTarget> ParseTargets(XmlNode language)
        {
            var targets = new List<PlatformTarget>();
            if (language.HasChildNodes)
                foreach (XmlNode node in language.ChildNodes)
                {
                    if (node.Name == "target")
                    {
                        FindTargets(node, targets);
                    }
                }

            if (targets.Count == 0)
            {
                targets.Add(PlatformTargets["Custom"]);
            }
            return targets;
        }

        private static void FindTargets(XmlNode node, List<PlatformTarget> targets)
        {
            var p = node.Attributes["platform"];
            if (p == null) return;
            var names = p.Value.Split('|');
            foreach (var name in names)
            {
                if (PlatformTargets.ContainsKey(name))
                    targets.Add(PlatformTargets[name]);
            }
        }

        private static List<PlatformVersion> ParseVersions(XmlNode language)
        {
            if (!language.HasChildNodes) return new List<PlatformVersion>
            {
                new PlatformVersion { Value = "1.0" }
            };

            var versions = new List<PlatformVersion>();
            foreach (XmlNode node in language.ChildNodes)
            {
                if (node.Name == "version")
                {
                    versions.Add(new PlatformVersion
                    {
                        Value = node.Attributes["value"].Value,
                        SwfVersion = ParseSwfVersion(node),
                        Commands = ParseCommands(node),
                        RawData = node
                    });
                }
            }
            return versions;
        }

        private static List<PlatformCommand> ParseCommands(XmlNode version)
        {
            var commands = new List<PlatformCommand>();
            foreach (XmlNode node in version.ChildNodes)
            {
                if (node.Name == "command")
                {
                    commands.Add(new PlatformCommand
                    {
                        Name = node.Attributes["name"].Value,
                        Value = node.Attributes["value"].Value,
                        RawData = node
                    });
                }
            }
            if (commands.Count > 0) return commands;
            return null;
        }

        private static string ParseSwfVersion(XmlNode node)
        {
            var attr = node.Attributes["swf-version"];
            return attr != null ? attr.Value : null;
        }

        private static void ParsePlatforms(XmlNode platforms)
        {
            PlatformTargets = new Dictionary<String, PlatformTarget>();
            foreach (XmlNode node in platforms.ChildNodes)
            {
                var versions = ParseVersions(node);
                var target = new PlatformTarget
                {
                    Name = node.Attributes["name"].Value,
                    Versions = versions,
                    LastVersion = versions[versions.Count - 1],
                    RawData = node
                };
                PlatformTargets.Add(target.Name, target);
            }
        }

        #endregion
    }

    public class PlatformLanguage
    {
        public string Name;
        public List<PlatformTarget> Targets;
        public XmlNode RawData;
    }

    public class PlatformTarget
    {
        public string Name;
        public List<PlatformVersion> Versions;
        public PlatformVersion LastVersion;
        public XmlNode RawData;
    }

    public class PlatformVersion
    {
        public string Value;
        public string SwfVersion;
        public List<PlatformCommand> Commands;
        public XmlNode RawData;
    }

    public class PlatformCommand
    {
        public string Name;
        public string Value;
        public XmlNode RawData;
    }

}

