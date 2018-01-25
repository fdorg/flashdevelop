using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceControl.Sources.Mercurial
{
    class AddCommand : BaseCommand
    {
        readonly string path;

        public AddCommand(string path)
        {
            this.path = path;
        }

        public override void Run()
        {
            var args = $"add \"{Path.GetFileName(path)}\"";
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
