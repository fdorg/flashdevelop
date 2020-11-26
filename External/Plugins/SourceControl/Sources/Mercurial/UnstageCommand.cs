using System;
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    internal class UnstageCommand : BaseCommand
    {
        readonly string path;

        public UnstageCommand(string path)
        {
            this.path = path;
        }

        public override void Run()
        {
            Run($"forget \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
        }
    }
}
