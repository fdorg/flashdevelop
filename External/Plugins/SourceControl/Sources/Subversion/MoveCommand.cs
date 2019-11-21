using System.IO;

namespace SourceControl.Sources.Subversion
{
    class MoveCommand : BaseCommand
    {
        public MoveCommand(string fromPath, string toPath)
        {
            string args = $"move \"{Path.GetFileName(fromPath)}\" \"{toPath}\"";

            Run(args, Path.GetDirectoryName(fromPath));
        }
    }
}
