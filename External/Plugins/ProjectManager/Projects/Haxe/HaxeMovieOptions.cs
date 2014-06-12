using System;
using System.Collections.Generic;
using System.Text;
using PluginCore;

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
                case AIR_PLATFORM: return PlatformData.AIR_VERSIONS;
                case AIR_MOBILE_PLATFORM: return PlatformData.AIR_MOBILE_VERSIONS;
                case FLASHPLAYER_PLATFORM: return PlatformData.FLASH_LEGACY_VERSIONS;
                case NME_PLATFORM: return PlatformData.NME_VERSIONS;
                default: return new string[] { "1.0" };
            }
        }

        public override string DefaultVersion(string platform)
        {
            switch (platform)
            {
                case AIR_PLATFORM: return PlatformData.DEFAULT_AIR_VERSION;
                case AIR_MOBILE_PLATFORM: return PlatformData.DEFAULT_AIR_MOBILE_VERSION;
                case FLASHPLAYER_PLATFORM: return PlatformData.DEFAULT_FLASH_VERSION;
                case NME_PLATFORM: return PlatformData.DEFAULT_NME_VERSION;
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
