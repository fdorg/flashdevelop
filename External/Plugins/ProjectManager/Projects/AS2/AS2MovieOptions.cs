// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace ProjectManager.Projects.AS2
{
    public class AS2MovieOptions: MovieOptions
    {
        public AS2MovieOptions()
        {
            Language = "as2";
            MajorVersion = 9;
            Platform = TargetPlatforms[0];
        }

        public override OutputType[] OutputTypes =>
            new[] { OutputType.OtherIDE, OutputType.CustomBuild, OutputType.Application };

        public override OutputType DefaultOutput(string platform) => OutputType.Application;
    }
}