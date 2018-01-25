using System;
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class MoveCommand : BaseCommand
    {
        private readonly string args;
        private readonly string directory;

        public MoveCommand(string fromPath, string toPath)
        {
            args = $"move \"{Path.GetFileName(fromPath)}\" \"{toPath}\"";
            directory = Path.GetDirectoryName(fromPath);
        }

        public override void Run() => Run(args, directory);
    }
}
