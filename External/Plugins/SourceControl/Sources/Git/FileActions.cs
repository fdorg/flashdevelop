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
            else return false; // let cut/paste files
        }

        public bool FileMove(string fromPath, string toPath)
        {
            return false;
            /*new MoveCommand(fromPath, toPath);
            return true;*/
        }

        public void FileAfterMove(string fromPath, VCItemStatus status, string toPath)
        {
            if (status == VCItemStatus.Added)
            {
                new UnstageCommand(fromPath); //TODO: this will not work, since the commands are started simultaniously
                new AddCommand(toPath);
            }
        }

        public bool FileNew(string path)
        {
            new AddCommand(path);
            return false;
        }
        public bool FileOpen(string path) { return false; }
        public bool FileReload(string path) { return false; }
        public bool FileModifyRO(string path) { return false; }

        public bool BuildProject() { return false; }
        public bool TestProject() { return false; }
        public bool SaveProject() { return false; }
    }
}
