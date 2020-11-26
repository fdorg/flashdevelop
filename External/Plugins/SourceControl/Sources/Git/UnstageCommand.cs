using System.IO;

namespace SourceControl.Sources.Git
{
    internal class UnstageCommand : BaseCommand
    {
        readonly string path;
        public UnstageCommand(string path)
        {
            this.path = path;
        }

        public override void Run() => Run($"reset HEAD \"{Path.GetFileName(path)}\"", Path.GetDirectoryName(path));
    }
}
