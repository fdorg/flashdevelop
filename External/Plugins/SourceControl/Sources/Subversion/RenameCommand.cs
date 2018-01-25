using System;
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class RenameCommand : BaseCommand
    {
        private readonly string args;
        private readonly string path;

        public RenameCommand(string path, string newName)
        {
            args = $"rename \"{Path.GetFileName(path)}\" \"{newName}\"";
            this.path = path;
        }

        public override void Run() => Run(args, Path.GetDirectoryName(path));
    }
}
