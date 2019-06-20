using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Subversion
{
    class DeleteCommand : BaseCommand
    {
        string[] paths;

        public DeleteCommand(string[] paths)
        {
            string args = "delete --force";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            this.paths = paths;

            Run(args, Path.GetDirectoryName(paths[0]));
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);

            ProjectWatcher.HandleFilesDeleted(paths);
        }
    }
}
