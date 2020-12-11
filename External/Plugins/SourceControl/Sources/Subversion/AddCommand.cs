using System.IO;

namespace SourceControl.Sources.Subversion
{
    internal class AddCommand : BaseCommand
    {
        readonly string path;

        public AddCommand(string path) => this.path = path;

        public override void Run()
        {
            var args = $"add \"{Path.GetFileName(path)}\"";
            Run(args, Path.GetDirectoryName(path));
        }
    }
}
