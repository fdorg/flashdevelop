namespace ProjectManager.Projects.Generic
{
    public class GenericMovieOptions : MovieOptions
    {
        public const string DEFAULT = "Default";

        public override string[] TargetPlatforms => new[] { DEFAULT };

        public override string[] TargetVersions(string platform) => new[] { "1.0" };

        public override string DefaultVersion(string platform) => "1.0";

        public override OutputType[] OutputTypes => new[] { OutputType.Website, OutputType.CustomBuild };

        public override OutputType DefaultOutput(string platform) => OutputType.Website;

        public override bool IsGraphical(string platform) => false;

        public override bool DebuggerSupported(string targetBuild) => false;
    }
}