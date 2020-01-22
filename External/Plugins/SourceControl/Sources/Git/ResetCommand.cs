// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Git
{
    class ResetCommand : BaseCommand
    {
        public ResetCommand(string[] paths)
        {
            string args = "reset ";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            Run(args, Path.GetDirectoryName(paths[0]));
        }
    }
}
