// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class DeleteCommand : BaseCommand
    {
        public DeleteCommand(string[] paths)
        {
            string args = "delete --force";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            Run(args, Path.GetDirectoryName(paths[0]));
        }
    }
}
