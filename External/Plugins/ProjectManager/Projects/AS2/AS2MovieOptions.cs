using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Projects.AS2
{
    public class AS2MovieOptions: MovieOptions
    {
        public const string FLASHPLAYER_PLATFORM = "Flash Player";

        public AS2MovieOptions()
        {
            MajorVersion = 9;
            Platform = TargetPlatforms[0];
        }

        public override bool DebuggerSupported
        {
            get { return false; }
        }

        public override string[] TargetPlatforms
        {
            get { return new string[] { FLASHPLAYER_PLATFORM }; }
        }

        public override string[] TargetVersions(string platform)
        {
            return new string[] { "6.0", "7.0", "8.0", "9.0" }; 
        }

        public override string DefaultVersion(string platform)
        {
            return "9.0";
        }

        public override OutputType[] OutputTypes
        {
            get
            {
                return new OutputType[] { 
                    OutputType.OtherIDE, OutputType.CustomBuild, OutputType.Application };
            }
        }

        public override OutputType DefaultOutput(string platform)
        {
            return OutputType.Application;
        }

        public override bool IsGraphical(string platform)
        {
            return true;
        }
    }
}
