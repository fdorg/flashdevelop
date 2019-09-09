using System.IO;

namespace SourceControl.Sources.Git
{
    class ResetCommand : BaseCommand
    {
        public ResetCommand(string[] paths)
        {
            string args = "reset ";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            Run(args, Path.GetDirectoryName(paths[0]));
        }
    }
}
