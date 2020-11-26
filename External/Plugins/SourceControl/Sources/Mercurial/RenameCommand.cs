using System;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Mercurial
{
    internal class RenameCommand : BaseCommand
    {
        readonly string path;
        readonly string newName;

        public RenameCommand(string path, string newName)
        {
            this.path = path;
            this.newName = newName;
        }

        public override void Run()
        {
            // directly rename empty dirs
            if (Directory.Exists(path) && Directory.GetFiles(path).Length == 0)
            {
                string newPath = Path.Combine(Path.GetDirectoryName(path), newName);
                if (Directory.Exists(newPath)) return;
                try { Directory.Move(path, newPath); }
                catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                return;
            }

            var args = $"mv \"{Path.GetFileName(path)}\" \"{newName}\"";
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
