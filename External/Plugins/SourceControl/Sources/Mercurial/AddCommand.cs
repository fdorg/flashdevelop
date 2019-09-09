// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    class AddCommand : BaseCommand
    {
        public AddCommand(string path)
        {
            string args = $"add \"{Path.GetFileName(path)}\"";
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
