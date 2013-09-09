using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeMovieOptions : MovieOptions
    {
        public const string FLASHPLAYER_PLATFORM = "Flash Player";
        public const string AIR_PLATFORM = "AIR";
        public const string AIR_MOBILE_PLATFORM = "AIR Mobile";
        public const string CUSTOM_PLATFORM = "Custom";
        public const string JAVASCRIPT_PLATFORM = "JavaScript";
        public const string NEKO_PLATFORM = "Neko";
        public const string PHP_PLATFORM = "PHP";
        public const string CPP_PLATFORM = "C++";
        public const string NME_PLATFORM = "NME";
        public const string CSHARP_PLATFORM = "C#";
        public const string JAVA_PLATFORM = "Java";
        public static string[] NME_TARGETS = new string[] { 
            "flash", "html5", "windows", "neko", "android", "webos", "blackberry" };
        
        public HaxeMovieOptions()
        {
            MajorVersion = 10;
            Platform = TargetPlatforms[0];
        }

        public override bool DebuggerSupported
        {
            get
            {
                return (Platform == FLASHPLAYER_PLATFORM && MajorVersion >= 9)
                    || Platform == AIR_PLATFORM || Platform == AIR_MOBILE_PLATFORM
                    || Platform == NME_PLATFORM;
            }
        }

        public override string[] TargetPlatforms
        {
            get 
            { 
                return new string[] { 
                        FLASHPLAYER_PLATFORM, AIR_PLATFORM, AIR_MOBILE_PLATFORM, NME_PLATFORM, 
                        JAVASCRIPT_PLATFORM, NEKO_PLATFORM, PHP_PLATFORM, CPP_PLATFORM, 
                        CSHARP_PLATFORM, JAVA_PLATFORM
                    }; 
            }
        }

        public override string[] TargetVersions(string platform)
        {
            switch (platform)
            {
                case AIR_PLATFORM: return new string[] { "1.5", "2.0", "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8" };
                case AIR_MOBILE_PLATFORM: return new string[] { "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8" };
                case FLASHPLAYER_PLATFORM: return new string[] { "6.0", "7.0", "8.0", "9.0", "10.0", "10.1", "10.2", "10.3", "11.0", "11.1", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8" };
                case NME_PLATFORM: return new string[] { "3.0" };
                default: return new string[] { "1.0" };
            }
        }

        public override string DefaultVersion(string platform)
        {
            switch (platform)
            {
                case AIR_PLATFORM: return "3.7";
                case AIR_MOBILE_PLATFORM: return "3.7";
                case FLASHPLAYER_PLATFORM: return "11.0";
                case NME_PLATFORM: return "3.0";
                default: return "1.0";
            }
        }

        public override OutputType[] OutputTypes
        {
            get
            {
                return new OutputType[] { OutputType.CustomBuild, OutputType.Application };
            }
        }

        public override OutputType DefaultOutput(string platform)
        {
            return OutputType.Application;
        }

        public override bool IsGraphical(string platform)
        {
            return platform == AIR_PLATFORM || platform == FLASHPLAYER_PLATFORM;
        }
    }
}
