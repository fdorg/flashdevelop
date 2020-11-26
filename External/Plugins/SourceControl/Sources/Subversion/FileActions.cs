namespace SourceControl.Sources.Subversion
{
    internal class FileActions:IVCFileActions
    {
        public bool FileBeforeRename(string path)
        {
            return false; // allow in tree edition
        }

        public bool FileRename(string path, string newName)
        {
            new RenameCommand(path, newName).Run();
            return true; // operation handled
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
            new MoveCommand(fromPath, toPath).Run();
            return true;
        }

        public void FileAfterMove(string fromPath, VCItemStatus status, string toPath)
        {
            new AddCommand(toPath).ContinueWith(new DeleteCommand(new []{fromPath})).Run();
        }

        public bool FileNew(string path)
        {
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
