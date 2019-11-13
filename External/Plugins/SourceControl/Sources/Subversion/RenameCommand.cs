using System.IO;

namespace SourceControl.Sources.Subversion
{
    class RenameCommand : BaseCommand
    {
        public RenameCommand(string path, string newName)
        {
            string args = $"rename \"{Path.GetFileName(path)}\" \"{newName}\"";

            Run(args, Path.GetDirectoryName(path));
        }
    }
}
