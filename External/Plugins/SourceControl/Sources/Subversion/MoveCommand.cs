// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class MoveCommand : BaseCommand
    {
        public MoveCommand(string fromPath, string toPath)
        {
            string args = $"move \"{Path.GetFileName(fromPath)}\" \"{toPath}\"";

            Run(args, Path.GetDirectoryName(fromPath));
        }
    }
}
