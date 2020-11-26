using System.IO;

namespace SourceControl.Sources.Git
{
    internal class ResetCommand : BaseCommand
    {
        readonly string args;
        readonly string dir;

        public ResetCommand(string[] paths)
        {
            args = "reset ";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            dir = Path.GetDirectoryName(paths[0]);
        }

        public override void Run() => Run(args, dir);
    }
}
