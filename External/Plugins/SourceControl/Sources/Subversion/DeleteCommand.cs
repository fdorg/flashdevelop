using System.IO;

namespace SourceControl.Sources.Subversion
{
    class DeleteCommand : BaseCommand
    {
        public DeleteCommand(string[] paths)
        {
            string args = "delete --force";
            foreach (string path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            Run(args, Path.GetDirectoryName(paths[0]));
        }
    }
}
