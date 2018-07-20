// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
    }
}
