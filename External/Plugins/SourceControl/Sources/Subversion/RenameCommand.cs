// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    internal class RenameCommand : BaseCommand
    {
        readonly string args;
        readonly string path;

        public RenameCommand(string path, string newName)
        {
            args = $"rename \"{Path.GetFileName(path)}\" \"{newName}\"";
            this.path = path;
        }

        public override void Run() => Run(args, Path.GetDirectoryName(path));
    }
}
