// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    internal class AddCommand : BaseCommand
    {
        readonly string path;

        public AddCommand(string path) => this.path = path;

        public override void Run()
        {
            var args = $"add \"{Path.GetFileName(path)}\"";
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
