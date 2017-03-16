// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace SourceControl.Sources.Git
{
    class CommitCommand : BaseCommand
    {
        public CommitCommand(string[] paths, string message)
        {
            if (paths.Length == 0) return;

            string args = "commit -m \"" + message.Replace("\"", "\\\"") + "\" .";

            Run(args, paths[0]);
        }
    }
}
