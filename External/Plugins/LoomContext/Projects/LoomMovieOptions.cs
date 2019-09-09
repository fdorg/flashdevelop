// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ProjectManager.Projects;

namespace LoomContext.Projects
{
    public class LoomMovieOptions : MovieOptions
    {
        public const string LOOM_PLATFORM = "Loom";

        public LoomMovieOptions()
        {
            MajorVersion = 1;
            Platform = TargetPlatforms[0];
        }

        public override string[] TargetPlatforms
        {
            get { return new string[] { LOOM_PLATFORM }; }
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
                return new OutputType[] { 
                    OutputType.CustomBuild, OutputType.Application };
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

        public override bool HasOutput(OutputType output)
        {
            return false;
        }

    }
}
