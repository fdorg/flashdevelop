using System.IO;

namespace SourceControl.Sources.Git
{
    class CommitCommand : BaseCommand
    {
        readonly string workingDirectory;
        string commitArgs;

        public CommitCommand(string[] files, string message, string workingDir)
        {
            if (workingDir == null) return;
            workingDirectory = workingDir;

            //add the files first to make sure untracked files can be committed
            var fileArgs = "";
            if (files != null)
                foreach (var file in files)
                    if(File.Exists(file) || Directory.Exists(file))
                        fileArgs += " \"" + VCHelper.GetRelativePath(file, workingDir) + "\"";

            commitArgs = "commit" + fileArgs + " -m \"" + VCHelper.EscapeCommandLine(message) + "\"";
            if (!string.IsNullOrEmpty(fileArgs)) Run("add" + fileArgs, workingDir);
            else Run(commitArgs, workingDir);
        }

        public CommitCommand(string[] paths, string message) : this(null, message, Path.GetDirectoryName(SafeGet(paths, 0)))
        {
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);

            if (commitArgs == null) return;

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
