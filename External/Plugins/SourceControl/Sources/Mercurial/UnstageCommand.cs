using System;
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    class UnstageCommand : BaseCommand
    {
        public UnstageCommand(string path)
        {
            Run($"forget \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
        }
    }
}
