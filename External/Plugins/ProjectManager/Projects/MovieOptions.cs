using System;
using System.Drawing;
using PluginCore;

namespace ProjectManager.Projects
{
    public abstract class MovieOptions
    {
        public string Language;
        public int Fps;
        public int Width;
        public int Height;
        public int MajorVersion;
        public int MinorVersion;

        string platform;
        public string Platform
        {
            get => platform;
            set
            {
                if (value == "NME") return;
                platform = value;
            }
        }

        public string Background;
        public string[] TargetBuildTypes;
        public string[] DefaultBuildTargets;

        public MovieOptions()
        {
            Platform = "Custom";
            Language = "unknown";
            Fps = 30;
            Width = 800;
            Height = 600;
            Background = "#FFFFFF";
        }

        public Color BackgroundColor
        {
            get => ColorTranslator.FromHtml(Background);
            set => Background = $"#{(value.R << 16) + (value.G << 8) + value.B:X6}";
        }

        public int BackgroundColorInt
        {
            get
            {
                Color c = BackgroundColor;
                return (c.R << 16) + (c.G << 8) + c.B;
            }
        }

        public virtual bool HasSupport => PlatformData.SupportedLanguages.ContainsKey(Language);

        public virtual SupportedLanguage LanguageSupport => PlatformData.SupportedLanguages[Language];

        public virtual bool HasPlatformSupport => HasSupport && Platform != null && LanguageSupport.Platforms.ContainsKey(Platform);

        public virtual LanguagePlatform PlatformSupport => LanguageSupport.Platforms[Platform];

        public virtual string[] TargetPlatforms 
        {
            get
            {
                if (HasSupport) return LanguageSupport.PlatformNames;
                return new[] { "Custom" };
            }
        }

        public virtual string[] TargetVersions(string platform)
        {
            if (HasSupport)
            {
                var platforms = LanguageSupport.Platforms;
                if (platform != null && platforms.ContainsKey(platform)) return platforms[platform].VersionNames;
            }
            return new[] { "0.0" };
        }

        public virtual string DefaultVersion(string platform)
        {
            if (HasSupport)
            {
                var platforms = LanguageSupport.Platforms;
                if (platform != null && platforms.ContainsKey(platform)) return platforms[platform].LastVersion.Value;
            }
            return "0.0";
        }

        public abstract OutputType[] OutputTypes { get; }
        public abstract OutputType DefaultOutput(string platform);

        public virtual bool HasOutput(OutputType output)
        {
            return (output == OutputType.Application || output == OutputType.Library);
        }

        public virtual string Version 
        { 
            get => MajorVersion + "." + MinorVersion;
            set
            {
                string[] p = value.Split('.');
                MajorVersion = p[0].Length > 0 ? int.Parse(p[0]) : 0;
                if (p.Length > 1 && p[1].Length > 0) MinorVersion = int.Parse(p[1]); else MinorVersion = 0;
            }
        }

        public virtual bool IsGraphical(string platform)
        {
            if (HasSupport)
            {
                var platforms = LanguageSupport.Platforms;
                if (platform != null && platforms.ContainsKey(platform)) return platforms[platform].IsGraphical;
            }
            return false;
        }
        
        public virtual bool DebuggerSupported(string targetBuild)
        {
            if (HasPlatformSupport)
            {
                var debugger = PlatformSupport.DebuggerSupported;
                if (debugger is null) return false;
                
                if (string.IsNullOrEmpty(targetBuild)) 
                    return debugger.Length > 0 && debugger[0] == "*";

                foreach (string target in debugger)
                {
                    if (target == "*") return true;
                    if (targetBuild.StartsWith(target, StringComparison.Ordinal)) return true;
                }
            }
            return false;
        }

    }
}
