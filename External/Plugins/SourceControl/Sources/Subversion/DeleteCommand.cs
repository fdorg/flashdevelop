using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Subversion
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
            var args = "delete --force";
            foreach (var path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            Run(args, Path.GetDirectoryName(paths[0]));
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);

            ProjectWatcher.HandleFilesDeleted(paths);
        }
    }
}
