using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceControl.Sources.Git
{
    class UnstageCommand : BaseCommand
    {
        readonly string path;
        public UnstageCommand(string path)
        {
            this.path = path;
        }

        public override void Run() => Run($"reset HEAD \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
    }
}
