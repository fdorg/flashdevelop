
using System.IO;

namespace SourceControl.Sources.Subversion
{
    class CommitCommand : BaseCommand
    {
        public CommitCommand(string[] files, string message, string workingDir)
        {
            if (workingDir is null) return;

            var args = "commit";

            if (files != null)
                foreach (var file in files)
                    args += " \"" + VCHelper.GetRelativePath(file, workingDir) + "\"";

            args += " -m \"" + VCHelper.EscapeCommandLine(message) + "\"";

            Run(args, workingDir);
        }

        public CommitCommand(string[] paths, string message) : this(null, message, Path.GetDirectoryName(SafeGet(paths, 0)))
        {
        }

        static T SafeGet<T>(T[] a, int i) where T : class
        {
            return a != null && a.Length > i ? a[i] : null;
        }
    }
}
