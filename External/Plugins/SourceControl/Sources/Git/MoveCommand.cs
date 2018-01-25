using System;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Git
{
    class MoveCommand : BaseCommand
    {
        private readonly string from;
        private readonly string to;

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
                toPath = Path.Combine(to, Path.GetFileName(from));
                if (Directory.Exists(toPath)) return;
                try { Directory.Move(from, toPath); }
                catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                return;
            }

            string args = $"mv \"{Path.GetFileName(from)}\" \"{toPath}\"";

            Run(args, Path.GetDirectoryName(from));
        }
    }
}
