// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Git
{
    internal class AddCommand : BaseCommand
    {
        readonly string args;
        readonly string directory;

        public AddCommand(string path)
        {
            directory = Path.GetDirectoryName(path);
            args = $"add \"{Path.GetFileName(path)}\"";
        }

        public override void Run() => Run(args, directory);
    }
}
