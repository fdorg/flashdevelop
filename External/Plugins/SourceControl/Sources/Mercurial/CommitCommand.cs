
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    internal class CommitCommand : BaseCommand
    {
        readonly string workingDirectory;
        readonly string message;
        readonly string[] files;
        string commitArgs;

        public CommitCommand(string[] files, string message, string workingDir)
        {
            this.files = files;
            this.message = message;
            workingDirectory = workingDir;
        }

        public CommitCommand(string[] paths, string message) : this(null, message, Path.GetDirectoryName(SafeGet(paths, 0)))
        {
        }

        public override void Run()
        {
            if (workingDirectory is null) return;

            //add the files first to make sure untracked files can be committed
            var fileArgs = "";
            if (files != null)
                foreach (var file in files)
                    if (File.Exists(file) || Directory.Exists(file))
                        fileArgs += " \"" + VCHelper.GetRelativePath(file, workingDirectory) + "\"";

            commitArgs = "commit" + fileArgs + " -m \"" + VCHelper.EscapeCommandLine(message) + "\"";
            if (!string.IsNullOrEmpty(fileArgs)) Run("add" + fileArgs, workingDirectory);
            else Run(commitArgs, workingDirectory);
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);

            if (commitArgs is null) return;

            //now do the commit
            Run(commitArgs, workingDirectory);
            commitArgs = null;
        }

        static T SafeGet<T>(T[] a, int i) where T : class
        {
            return a != null && a.Length > i ? a[i] : null;
        }
    }
}
