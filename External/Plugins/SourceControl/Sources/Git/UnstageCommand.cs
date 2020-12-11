// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Git
{
    internal class UnstageCommand : BaseCommand
    {
        readonly string path;
        public UnstageCommand(string path) => this.path = path;

        public override void Run() => Run($"reset HEAD \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
    }
}
