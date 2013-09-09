using System;
using System.Collections.Generic;
using System.Text;
using PluginCore.Utilities;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Subversion
{
    class MoveCommand : BaseCommand
    {
        public MoveCommand(string fromPath, string toPath)
        {
            string args = String.Format("move \"{0}\" \"{1}\"", Path.GetFileName(fromPath), toPath);

            Run(args, Path.GetDirectoryName(fromPath));
        }
    }
}
