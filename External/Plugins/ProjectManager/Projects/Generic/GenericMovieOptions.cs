namespace ProjectManager.Projects.Generic
{
    public class GenericMovieOptions : MovieOptions
    {
        public const string DEFAULT = "Default";

        public override string[] TargetPlatforms
        {
            get { return new string[] { DEFAULT }; }
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
            get { return new OutputType[] { OutputType.Website, OutputType.CustomBuild }; }
        }

        public override OutputType DefaultOutput(string platform)
        {
            return OutputType.Website;
        }

        public override bool IsGraphical(string platform)
        {
            return false;
        }

        public override bool DebuggerSupported(string targetBuild)
        {
            return false;
        }
    }
}
