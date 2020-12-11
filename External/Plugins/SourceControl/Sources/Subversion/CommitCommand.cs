// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources.Subversion
{
    internal class CommitCommand : BaseCommand
    {
        readonly string workingDirectory;
        readonly string message;
        readonly string[] files;

        public CommitCommand(string[] files, string message, string workingDir)
        {
            this.files = files;
            this.message = message;
            workingDirectory = workingDir;
        }

        public CommitCommand(string[] paths, string message) : this(null, message, Path.GetDirectoryName(SafeGet(paths, 0)))
        {
        }

        static T SafeGet<T>(T[] a, int i) where T : class => a != null && a.Length > i ? a[i] : null;

        public override void Run()
        {
            if (workingDirectory is null) return;

            var args = "commit";

            if (files != null)
                foreach (var file in files)
                    args += " \"" + VCHelper.GetRelativePath(file, workingDirectory) + "\"";

            args += " -m \"" + VCHelper.EscapeCommandLine(message) + "\"";

            Run(args, workingDirectory);
        }
    }
}
