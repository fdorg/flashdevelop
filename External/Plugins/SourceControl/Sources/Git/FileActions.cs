namespace SourceControl.Sources.Git
{
    internal class FileActions : IVCFileActions
    {
        public bool FileBeforeRename(string path)
        {
            return false; // allow in tree edition
        }

        public bool FileRename(string path, string newName)
        {
            return false;
            /*new RenameCommand(path, newName).Run();
            return true; // operation handled*/
        }

        public bool FileDelete(string[] paths, bool confirm)
        {
            if (confirm)
            {
                new DeleteCommand(paths).Run();
                return true; // operation handled
            }

            return false; // let cut/paste files
        }

        public bool FileMove(string fromPath, string toPath)
        {
            return false;
            /*new MoveCommand(fromPath, toPath).Run();
            return true;*/
        }

        public void FileAfterMove(string fromPath, VCItemStatus status, string toPath)
        {
            if (status == VCItemStatus.Added)
                new UnstageCommand(fromPath).ContinueWith(new AddCommand(toPath)).Run();
        }

        public bool FileNew(string path)
        {
            new AddCommand(path).Run();
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
