using System;
using System.IO;

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
