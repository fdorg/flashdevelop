using PluginCore;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeMovieOptions : MovieOptions
    {
        public HaxeMovieOptions()
        {
            Language = "haxe";
            MajorVersion = 14;
            Platform = TargetPlatforms[0];
        }

        public override bool DebuggerSupported(string targetBuild)
        {
            var supported = base.DebuggerSupported(targetBuild);
            if (supported && Platform == PlatformData.FLASHPLAYER_PLATFORM && MajorVersion < 9) // not AS2
                return false;
            return supported;
        }

        public override OutputType[] OutputTypes
        {
            get
            {
                return new OutputType[] { OutputType.CustomBuild, OutputType.Application };
            }
        }

        public override bool HasOutput(OutputType output)
        {
            return output == OutputType.Application || output == OutputType.Library 
                || (HasPlatformSupport && PlatformSupport.ExternalToolchain != null);
        }

        public override OutputType DefaultOutput(string platform)
        {
            return OutputType.Application;
        }
    }
}
