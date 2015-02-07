using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace PluginCore
{
    // When updating, update also AirProperties plugin!

    // This class must not have any dependencies

    public class PlatformData
    {
        public static string CUSTOM_PLATFORM = "Custom";
        public static string FLASHPLAYER_PLATFORM = "Flash Player";
        public static string JAVASCRIPT_PLATFORM = "JavaScript";

        public static Dictionary<String, SupportedLanguage> SupportedLanguages;

        public static string ResolveFlashPlayerVersion(string lang, string platformName, string version)
        {
            if (!SupportedLanguages.ContainsKey(lang)) return "14.0";
            var platforms = SupportedLanguages[lang.ToLower()].Platforms;
            if (!platforms.ContainsKey(FLASHPLAYER_PLATFORM)) return "14.0";
            var flashPlatform = platforms[FLASHPLAYER_PLATFORM];
            if (platforms.ContainsKey(platformName))
            {
                var otherTarget = platforms[platformName];
                foreach (var otherVersion in otherTarget.Versions)
                    if (otherVersion.Value == version)
                    {
                        foreach (var flashVersion in flashPlatform.Versions)
                            if (flashVersion.SwfVersion == otherVersion.SwfVersion) return flashVersion.Value;
                    }
            }
            // default to last FP
            return flashPlatform.LastVersion.Value;
        }

        public static string ResolveSwfVersion(string lang, string platformName, string version)
        {
            if (!SupportedLanguages.ContainsKey(lang)) return "25";
            var platforms = SupportedLanguages[lang.ToLower()].Platforms;
            if (!platforms.ContainsKey(FLASHPLAYER_PLATFORM) || !platforms.ContainsKey(platformName)) return "25";
            var platform = platforms[platformName];
            foreach (var platformVersion in platform.Versions)
                if (platformVersion.Value == version)
                    return platformVersion.SwfVersion;
            // default to last FP
            return platform.LastVersion.SwfVersion;
        }

        #region platform config loading

        public static void Load(string path)
        {
            SupportedLanguages = new Dictionary<String, SupportedLanguage>();
            if (!Directory.Exists(path)) return;

            // walk AS2, AS3, Haxe...
            foreach (string langDir in Directory.GetDirectories(path))
            {
                try
                {
                    SupportedLanguage lang = new SupportedLanguage
                    {
                        Name = Path.GetFileNameWithoutExtension(langDir),
                        Platforms = LoadPlatforms(langDir)
                    };
                    lang.PlatformNames = new string[lang.Platforms.Count];
                    lang.Platforms.Keys.CopyTo(lang.PlatformNames, 0);
                    SupportedLanguages.Add(lang.Name.ToLower(), lang);
                }
                catch { }
            }
        }

        private static Dictionary<string, LanguagePlatform> LoadPlatforms(string langDir)
        {
            // walk flashplayer.xml, openfl.xml,... for one language (ie. Haxe)
            var platforms = new Dictionary<string, LanguagePlatform>();
            foreach (string platformFile in Directory.GetFiles(langDir, "*.xml"))
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(platformFile);
                    if (doc.FirstChild.Name == "platform")
                    {
                        var platform = ParsePlatform(doc.FirstChild);
                        platforms.Add(platform.Name, platform);
                    }
                }
                catch { }
            }
            return platforms;
        }

        private static LanguagePlatform ParsePlatform(XmlNode node)
        {
            // parse platform file, ie. flashplayer.xml
            List<PlatformVersion> versions = null;
            Dictionary<string, PlatformCommand> defaultCommands = null;
            foreach (XmlNode sub in node.ChildNodes)
            {
                switch (sub.Name)
                {
                    case "defaults": defaultCommands = ParseCommands(sub, null); break;
                    case "versions": versions = ParseVersions(sub, defaultCommands); break;
                }
            }

            var platform = new LanguagePlatform
            {
                Name = node.Attributes["name"].Value,
                Targets = GetList(node, "targets"),
                Versions = versions,
                LastVersion = versions[versions.Count - 1],
                IsFlashPlatform = GetBool(node, "flash"),
                IsGraphical = GetBool(node, "graphical"),
                ExternalToolchain = GetAttribute(node, "external"),
                ExternalToolchainCapture = GetList(node, "external-capture"),
                DefaultProjectFile = GetList(node, "default-project"),
                HaxeTarget = GetAttribute(node, "haxe-target"),
                DebuggerSupported = GetList(node, "debugger"),
                RawData = node
            };
            platform.VersionNames = new string[platform.Versions.Count];
            for (int i = 0; i < platform.Versions.Count; i++)
                platform.VersionNames[i] = platform.Versions[i].Value;
            return platform;
        }

        private static bool GetBool(XmlNode node, string attribute)
        {
            return (GetAttribute(node, attribute) ?? "false").ToLower() == "true";
        }

        private static string GetAttribute(XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr != null) return attr.Value;
            return null;
        }

        private static string[] GetList(XmlNode node, string attribute)
        {
            // build targets, ie. html5, flash, android for openfl
            var attr = node.Attributes[attribute];
            if (attr == null) return null;
            else return attr.Value.Split(',');
        }

        private static List<PlatformVersion> ParseVersions(XmlNode language, Dictionary<string, PlatformCommand> defaultCommands)
        {
            if (!language.HasChildNodes) return new List<PlatformVersion>
            {
                new PlatformVersion { Value = "1.0", Commands = defaultCommands }
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
                        Commands = ParseCommands(node, defaultCommands),
                        RawData = node
                    });
                }
            }
            return versions;
        }

        private static Dictionary<string, PlatformCommand> ParseCommands(XmlNode version, Dictionary<string, PlatformCommand> defaultCommands)
        {
            // custom display/build/run/clean commands, ie. for openfl
            var commands = defaultCommands == null 
                ? new Dictionary<string, PlatformCommand>()
                : new Dictionary<string, PlatformCommand>(defaultCommands);

            foreach (XmlNode node in version.ChildNodes)
            {
                if (node.Name == "command")
                {
                    var command = new PlatformCommand
                    {
                        Name = node.Attributes["name"].Value,
                        Value = node.Attributes["value"].Value,
                        RawData = node
                    };
                    commands.Add(command.Name.ToLower(), command);
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

        #endregion
    }

    public class SupportedLanguage
    {
        public string Name;
        public string[] PlatformNames;
        public Dictionary<string, LanguagePlatform> Platforms;
    }

    public class LanguagePlatform
    {
        public string Name;
        public string[] Targets;
        public string[] VersionNames;
        public List<PlatformVersion> Versions;
        public PlatformVersion LastVersion;
        public bool IsFlashPlatform;
        public bool IsGraphical;
        public string ExternalToolchain;
        public string[] ExternalToolchainCapture;
        public string[] DefaultProjectFile;
        public string HaxeTarget;
        public string[] DebuggerSupported;
        public XmlNode RawData;

        public PlatformVersion GetVersion(string value)
        {
            foreach (var version in Versions)
            {
                if (version.Value == value) return version;
            }
            return LastVersion;
        }

        public string GetProjectTemplate(string target)
        {
            var templateNode = RawData.SelectSingleNode("templates/template[@target='" + target + "']/@value");
                
            if (templateNode != null) return templateNode.Value;

            return null;
        }
    }

    public class PlatformVersion
    {
        public string Value;
        public string SwfVersion;
        public Dictionary<string, PlatformCommand> Commands;
        public XmlNode RawData;
    }

    public class PlatformCommand
    {
        public string Name;
        public string Value;
        public XmlNode RawData;
    }
}

