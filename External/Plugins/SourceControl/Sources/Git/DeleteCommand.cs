using System;
using System.IO;
using PluginCore.Managers;
using SourceControl.Actions;

namespace SourceControl.Sources.Git
{
    internal class DeleteCommand : BaseCommand
    {
        readonly string[] paths;

        public DeleteCommand(string[] paths)
        {
            this.paths = paths;
        }
        
        public override void Run()
        {
            var args = "rm -f";
            var count = 0;
            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                {
                    // directly delete empty dirs
                    if (Directory.GetFiles(path).Length == 0)
                    {
                        try { Directory.Delete(path, Directory.GetDirectories(path).Length > 0); }
                        catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                        continue;
                    }
                    args += " -r";
                }
                args += " \"" + Path.GetFileName(path) + "\"";
                count++;
            }

            if (count > 0) Run(args, Path.GetDirectoryName(paths[0]));
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);
            ProjectWatcher.HandleFilesDeleted(paths);
        }
    }
}
