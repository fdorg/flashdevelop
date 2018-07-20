// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
