using System;
using System.Collections.Generic;
using System.Text;
using ProjectManager.Projects;

namespace UnityContext
{
    public class UnityMovieOptions : MovieOptions
    {
        public const string UNITY3D_PLATFORM = "Unity3D";

        public UnityMovieOptions()
        {
            MajorVersion = 10;
            Platform = TargetPlatforms[0];
        }

        public override bool DebuggerSupported
        {
            get { return false; }
        }

        public override string[] TargetPlatforms
        {
            get { return new string[] { UNITY3D_PLATFORM }; }
        }

        public override string[] TargetVersions(string platform)
        {
            return new string[] { "1.0" };
        }

        public override string DefaultVersion(string platform)
        {
            return "1.0";
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
            return false;
        }
    }
}
