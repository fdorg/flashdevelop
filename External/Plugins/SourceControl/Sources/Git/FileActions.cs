namespace SourceControl.Sources.Git
{
    class FileActions : IVCFileActions
    {
        public bool FileBeforeRename(string path)
        {
            return false; // allow in tree edition
        }

        public bool FileRename(string path, string newName)
        {
            return false;
            /*new RenameCommand(path, newName);
            return true; // operation handled*/
        }

        public bool FileDelete(string[] paths, bool confirm)
        {
            if (confirm)
            {
                new DeleteCommand(paths);
                return true; // operation handled
            }

            return false; // let cut/paste files
        }

        public bool FileMove(string fromPath, string toPath)
        {
            return false;
            /*new MoveCommand(fromPath, toPath);
            return true;*/
        }

        public bool FileNew(string path)
        {
            new AddCommand(path);
            return false;
        }
        public bool FileOpen(string path) => false;
        public bool FileReload(string path) => false;
        public bool FileModifyRO(string path) => false;

        public bool BuildProject() => false;
        public bool TestProject() => false;
        public bool SaveProject() => false;
    }
}
