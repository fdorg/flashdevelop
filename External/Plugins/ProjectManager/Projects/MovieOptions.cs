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
        public string Platform;
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
            get { return ColorTranslator.FromHtml(Background); }
            set { Background = string.Format("#{0:X6}", (value.R << 16) + (value.G << 8) + value.B); }
        }

        public int BackgroundColorInt
        {
            get
            {
                Color c = BackgroundColor;
                return (c.R << 16) + (c.G << 8) + c.B;
            }
        }

        public virtual bool HasSupport
        {
            get { return PlatformData.SupportedLanguages.ContainsKey(Language); }
        }

        public virtual SupportedLanguage LanguageSupport
        {
            get { return PlatformData.SupportedLanguages[Language]; }
        }

        public virtual bool HasPlatformSupport
        {
            get { return HasSupport && Platform != null && LanguageSupport.Platforms.ContainsKey(Platform); }
        }

        public virtual LanguagePlatform PlatformSupport
        {
            get { return LanguageSupport.Platforms[Platform]; }
        }

        public virtual string[] TargetPlatforms 
        {
            get
            {
                if (HasSupport) return LanguageSupport.PlatformNames;
                return new string[] { "Custom" };
            }
        }

        public virtual string[] TargetVersions(string platform)
        {
            if (HasSupport)
            {
                var platforms = LanguageSupport.Platforms;
                if (platform != null && platforms.ContainsKey(platform)) return platforms[platform].VersionNames;
            }
            return new string[] { "0.0" };
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
            get { return MajorVersion + "." + MinorVersion; }
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
                if (debugger == null) return false;
                
                if (string.IsNullOrEmpty(targetBuild)) 
                    return debugger.Length > 0 && debugger[0] == "*";

                foreach (string target in debugger)
                {
                    if (target == "*") return true;
                    else if (targetBuild.StartsWith(target, StringComparison.Ordinal)) return true;
                }
            }
            return false;
        }

    }
}
