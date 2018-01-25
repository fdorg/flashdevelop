using System.IO;

namespace SourceControl.Sources.Git
{
    class AddCommand : BaseCommand
    {
        private readonly string args;
        private readonly string directory;

        public AddCommand(string path)
        {
            directory = Path.GetDirectoryName(path);
            args = $"add \"{Path.GetFileName(path)}\"";
        }

        public override void Run() => Run(args, directory);
    }
}
