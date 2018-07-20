// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.IO;

namespace SourceControl.Sources.Mercurial
{
    class CommitCommand : BaseCommand
    {
        string workingDirectory;
        string commitArgs;


        public CommitCommand(string[] files, string message, string workingDir)
        {
            if (workingDir == null) return;

            //add the files first to make sure untracked files can be committed
            var addArgs = "add";
            var fileArgs = "";

            if (files != null)
                foreach (var file in files)
                    fileArgs += " \"" + VCHelper.GetRelativePath(file, workingDir) + "\"";

            addArgs += fileArgs;
            commitArgs = "commit" + fileArgs + " -m \"" + VCHelper.EscapeCommandLine(message) + "\"";
            workingDirectory = workingDir;

            if (files != null)
                Run(addArgs, workingDir);
            else
                Run(commitArgs, workingDir);
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
