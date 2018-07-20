// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
