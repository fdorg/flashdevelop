// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class RenameCommand : BaseCommand
    {
        public RenameCommand(string path, string newName)
        {
            string args = $"rename \"{Path.GetFileName(path)}\" \"{newName}\"";

            Run(args, Path.GetDirectoryName(path));
        }
    }
}
