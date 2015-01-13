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
