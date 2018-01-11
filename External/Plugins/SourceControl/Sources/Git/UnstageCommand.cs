using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceControl.Sources.Git
{
    class UnstageCommand : BaseCommand
    {
        public UnstageCommand(string path)
        {
            Run($"reset HEAD \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
        }
    }
}
