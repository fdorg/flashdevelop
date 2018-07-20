using System;
using System.IO;

namespace SourceControl.Sources.Git
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
