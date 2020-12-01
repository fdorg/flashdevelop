using System.IO;

namespace SourceControl.Sources.Git
{
    internal class AddCommand : BaseCommand
    {
        readonly string args;
        readonly string directory;

        public AddCommand(string path)
        {
            directory = Path.GetDirectoryName(path);
            args = $"add \"{Path.GetFileName(path)}\"";
        }

        public override void Run() => Run(args, directory);
    }
}
