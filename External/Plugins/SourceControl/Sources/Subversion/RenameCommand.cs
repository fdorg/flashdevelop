using System;
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class RenameCommand : BaseCommand
    {
        public RenameCommand(string path, string newName)
        {
            string args = String.Format("rename \"{0}\" \"{1}\"", Path.GetFileName(path), newName);

            Run(args, Path.GetDirectoryName(path));
        }
    }
}
