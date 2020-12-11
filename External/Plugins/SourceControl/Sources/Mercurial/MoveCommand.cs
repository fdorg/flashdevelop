// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Mercurial
{
    internal class MoveCommand : BaseCommand
    {
        readonly string from;
        readonly string to;

        public MoveCommand(string fromPath, string toPath)
        {
            from = fromPath;
            to = toPath;
        }

        public override void Run()
        {
            var toPath = to;
            // directly move empty dirs
            if (Directory.Exists(from) && Directory.GetFiles(from).Length == 0)
            {
                toPath = Path.Combine(toPath, Path.GetFileName(from));
                if (Directory.Exists(toPath)) return;
                try { Directory.Move(from, toPath); }
                catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                return;
            }

            var args = $"mv \"{Path.GetFileName(@from)}\" \"{toPath}\"";

            Run(args, Path.GetDirectoryName(from));
        }
    }
}
