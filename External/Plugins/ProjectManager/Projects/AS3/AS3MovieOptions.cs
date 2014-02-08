using System;
using System.Collections.Generic;
using System.Text;
using PluginCore;

namespace ProjectManager.Projects.AS3
{
    public class AS3MovieOptions : MovieOptions
    {
        public const string FLASHPLAYER_PLATFORM = "Flash Player";
        public const string AIR_PLATFORM = "AIR";
        public const string AIR_MOBILE_PLATFORM = "AIR Mobile";
        public const string CUSTOM_PLATFORM = "Custom";

        public AS3MovieOptions()
        {
            MajorVersion = 10;
            Platform = TargetPlatforms[0];
        }

        public override bool DebuggerSupported
        {
            get { return true; }
        }

        public override string[] TargetPlatforms
        {
            get { return new string[] { FLASHPLAYER_PLATFORM, AIR_PLATFORM, AIR_MOBILE_PLATFORM, CUSTOM_PLATFORM }; }
        }

        public override string[] TargetVersions(string platform)
        {
            switch (platform)
            {
                case CUSTOM_PLATFORM: return new string[] { "0.0" };
                case AIR_MOBILE_PLATFORM: return PlatformData.AIR_MOBILE_VERSIONS;
                case AIR_PLATFORM: return PlatformData.AIR_VERSIONS;
                default: return PlatformData.FLASH_VERSIONS;
            }
        }

        public override string DefaultVersion(string platform)
        {
            switch (platform)
            {
                case CUSTOM_PLATFORM: return "0.0";
                case AIR_PLATFORM: return PlatformData.DEFAULT_AIR_VERSION;
                case AIR_MOBILE_PLATFORM: return PlatformData.DEFAULT_AIR_MOBILE_VERSION;
                default: return PlatformData.DEFAULT_FLASH_VERSION;
            }
        }

        public override OutputType[] OutputTypes
        {
            get
            {
                return new OutputType[] { 
                    OutputType.OtherIDE, OutputType.CustomBuild, OutputType.Application/*, OutputType.Library*/ };
            }
        }

        public override OutputType DefaultOutput(string platform)
        {
            return platform == CUSTOM_PLATFORM ? OutputType.CustomBuild : OutputType.Application;
        }

        public override bool IsGraphical(string platform)
        {
            return platform != CUSTOM_PLATFORM;
        }

        public string GetSWFVersion()
        {
            if (Platform != FLASHPLAYER_PLATFORM) return null;
            return GetSWFVersion(Version);
        }

        public string GetSWFVersion(string version)
        {
            int index = Array.IndexOf(TargetVersions(FLASHPLAYER_PLATFORM), version);
            if (index < 0) return null;
            return PlatformData.SWF_VERSIONS[index];
        }

    }

}

