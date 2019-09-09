// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore;

namespace ProjectManager.Projects.AS3
{
    public class AS3MovieOptions : MovieOptions
    {
        public AS3MovieOptions()
        {
            Language = "as3";
            MajorVersion = 14;
            Platform = TargetPlatforms[0];
        }

        public override OutputType[] OutputTypes =>
            new[] { OutputType.OtherIDE, OutputType.CustomBuild, OutputType.Application/*, OutputType.Library*/ };

        public override OutputType DefaultOutput(string platform)
        {
            return platform == PlatformData.CUSTOM_PLATFORM ? OutputType.CustomBuild : OutputType.Application;
        }
    }
}