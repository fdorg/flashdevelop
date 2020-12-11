// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    internal class MoveCommand : BaseCommand
    {
        readonly string args;
        readonly string directory;

        public MoveCommand(string fromPath, string toPath)
        {
            args = $"move \"{Path.GetFileName(fromPath)}\" \"{toPath}\"";
            directory = Path.GetDirectoryName(fromPath);
        }

        public override void Run() => Run(args, directory);
    }
}
