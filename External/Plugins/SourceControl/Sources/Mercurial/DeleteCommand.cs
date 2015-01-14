using System;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Mercurial
{
    class DeleteCommand : BaseCommand
    {
        public DeleteCommand(string[] paths)
        {
            string args = "rm -f";
            int count = 0;
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    // directly delete empty dirs
                    if (Directory.GetFiles(path).Length == 0)
                    {
                        try { Directory.Delete(path); }
                        catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                        continue;
                    }
                    args += " -r";
                }
                args += " \"" + Path.GetFileName(path) + "\"";
                count++;
            }

            if (count > 0) Run(args, Path.GetDirectoryName(paths[0]));
        }
    }
}
