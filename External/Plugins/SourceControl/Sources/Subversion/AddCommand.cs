using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceControl.Sources.Subversion
{
    class AddCommand : BaseCommand
    {
        public AddCommand(string path)
        {
            string args = String.Format("add \"{0}\"", Path.GetFileName(path));
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
