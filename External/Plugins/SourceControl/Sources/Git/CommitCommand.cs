// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;

namespace SourceControl.Sources.Git
{
    class CommitCommand : BaseCommand
    {
        readonly string workingDirectory;
        string commitArgs;

        public CommitCommand(string[] files, string message, string workingDir)
        {
            if (workingDir is null) return;
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

        public CommitCommand(IList<string> paths, string message) : this(null, message, Path.GetDirectoryName(SafeGet(paths, 0)))
        {
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            base.Runner_ProcessEnded(sender, exitCode);

            if (commitArgs is null) return;

            //now do the commit
            Run(commitArgs, workingDirectory);
            commitArgs = null;
        }

        static T SafeGet<T>(IList<T> a, int i) where T : class
        {
            return a != null && a.Count > i ? a[i] : null;
        }
    }
}
