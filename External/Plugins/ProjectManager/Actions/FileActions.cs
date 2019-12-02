// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;
using ProjectManager.Projects;

namespace ProjectManager.Actions
{
    public delegate void FileNameHandler(string path);
    public delegate void FileMovedHandler(string fromPath, string toPath);

    public static class ProjectFileActionsEvents
    {
        public const string FileBeforeRename = "ProjectManager.FileActions.BeforeRename";
        public const string FileRename = "ProjectManager.FileActions.Rename";
        public const string FileMove = "ProjectManager.FileActions.Move";
        public const string FileDelete = "ProjectManager.FileActions.Delete";
        public const string FileDeleteSilent = "ProjectManager.FileActions.DeleteSilent";
        public const string FileCopy = "ProjectManager.FileActions.Copy";
        public const string FileCut = "ProjectManager.FileActions.Cut";
        public const string FilePaste = "ProjectManager.FileActions.Paste";
        public const string FileEnableWatchers = "ProjectManager.FileActions.EnableWatchers";
        public const string FileDisableWatchers = "ProjectManager.FileActions.DisableWatchers";
    }

    /// <summary>
    /// Provides methods for creating new files and working with existing files in projects.
    /// </summary>
    public class FileActions
    {
        readonly IMainForm mainForm;
        string storedDirectory;
        string lastFileFromTemplate;

        public event FileNameHandler FileCreated;
        public event ProjectModifiedHandler ProjectModified;
        public event FileNameHandler OpenFile;
        public event FileNameHandler FileDeleted;
        public event FileMovedHandler FileMoved;
        public event FileMovedHandler FileCopied;

        public FileActions(IMainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        void PushCurrentDirectory()
        {
            storedDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Application.StartupPath;
        }

        void PopCurrentDirectory()
        {
            try { Environment.CurrentDirectory = storedDirectory; }
            catch { }
        }

        #region Add File From Template

        public void AddFileFromTemplate(Project project, string inDirectory, string templatePath, bool noName)
        {
            try
            {
                // the template could be named something like "MXML.fdt", or maybe "Class.as.fdt"
                string extension;
                string fileName = Path.GetFileNameWithoutExtension(templatePath);
                string caption = TextHelper.GetString("Label.AddNew") + " ";

                if (fileName.Contains('.'))
                {
                    // it's something like Class.as.fdt
                    extension = Path.GetExtension(fileName); // .as
                    if (noName)
                    {
                        caption += extension.Substring(1).ToUpper() + " " + TextHelper.GetString("Label.File");
                        fileName = TextHelper.GetString("Label.NewFile");
                    }
                    else
                    {
                        caption += Path.GetFileNameWithoutExtension(fileName);
                        fileName = TextHelper.GetString("Label.New") + Path.GetFileNameWithoutExtension(fileName).Replace(" ", ""); // just Class
                    }
                }
                else
                {
                    // something like MXML.fdt
                    extension = "." + fileName.ToLower();
                    caption += fileName + " " + TextHelper.GetString("Label.File");
                    fileName = TextHelper.GetString("Label.NewFile");
                }

                // let plugins handle the file creation
                var info = new Hashtable {["templatePath"] = templatePath, ["inDirectory"] = inDirectory};
                var de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
                EventManager.DispatchEvent(this, de);
                if (de.Handled) return;

                using var dialog = new LineEntryDialog(caption, TextHelper.GetString("Label.FileName"), fileName + extension);
                dialog.SelectRange(0, fileName.Length);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FlashDevelopActions.CheckAuthorName();

                    string newFilePath = Path.Combine(inDirectory, dialog.Line);
                    if (!Path.HasExtension(newFilePath) && extension != ".ext")
                        newFilePath = Path.ChangeExtension(newFilePath, extension);

                    if (!FileHelper.ConfirmOverwrite(newFilePath)) return;

                    // save this so when we are asked to process args, we know what file it's talking about
                    lastFileFromTemplate = newFilePath;

                    mainForm.FileFromTemplate(templatePath, newFilePath);
                }
            }
            catch (UserCancelException) { }
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
            }
        }

        public string ProcessArgs(Project project, string args)
        {
            if (lastFileFromTemplate != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);

                args = args.Replace("$(FileName)", fileName);

                if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
                {
                    string package = "";
                    string path = lastFileFromTemplate;

                    // Find closest parent
                    string classpath = project.AbsoluteClasspaths.GetClosestParent(path);

                    // Can't find parent, look in global classpaths
                    if (classpath is null)
                    {
                        PathCollection globalPaths = new PathCollection();
                        foreach (string cp in PluginMain.Settings.GlobalClasspaths)
                            globalPaths.Add(cp);
                        classpath = globalPaths.GetClosestParent(path);
                    }
                    if (classpath != null)
                    {
                        // Parse package name from path
                        package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, path));
                        package = package.Replace(Path.DirectorySeparatorChar, '.');
                    }

                    if (package == "") args = args.Replace(" $(Package)", "");
                    args = args.Replace("$(Package)", package);

                    if (package != "")
                        args = args.Replace("$(FileNameWithPackage)", package + "." + fileName);
                    else
                        args = args.Replace("$(FileNameWithPackage)", fileName);
                }
            }
            return args;
        }

        #endregion

        #region Adding Other Items

        public void AddLibraryAsset(Project project, string inDirectory)
        {
            using var dialog = new OpenFileDialog();
            dialog.Title = TextHelper.GetString("Label.AddLibraryAsset");
            dialog.Filter = TextHelper.GetString("Info.FileFilter");
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = CopyFile(dialog.FileName, inDirectory);

                // null means the user cancelled
                if (filePath is null) return;

                // add as an asset
                project.SetLibraryAsset(filePath, true);

                if (!FileInspector.IsSwc(filePath))
                {
                    // ask if you want to keep this file updated
                    string caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                    string message = TextHelper.GetString("Info.ConfirmFileUpdate");

                    DialogResult result = MessageBox.Show(mainForm, message, caption,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        LibraryAsset asset = project.GetAsset(filePath);
                        asset.UpdatePath = project.GetRelativePath(dialog.FileName);
                    }
                }

                project.Save();
                OnProjectModified(new[] { filePath });
            }
        }

        public void AddExistingFile(string inDirectory)
        {
            using var dialog = new OpenFileDialog();
            dialog.Title = TextHelper.GetString("Label.AddExistingFile");
            dialog.Filter = TextHelper.GetString("Info.FileFilter");
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
                CopyFile(dialog.FileName, inDirectory);
        }

        public void AddFolder(string inDirectory)
        {
            try
            {
                string label = TextHelper.GetStringWithoutMnemonicsOrEllipsis("Label.NewFolder");
                string path = Path.Combine(inDirectory, label);

                int i = 2;
                while (Directory.Exists(path))
                    path = Path.Combine(inDirectory, label + " (" + (i++) + ")");

                // this will set off a interesting chain of events that will cause
                // the newly created "New Folder" node to begin label editing.
                OnFileCreated(path);

                Directory.CreateDirectory(path);
            }
            catch (Exception exception)
            {
                string msg = TextHelper.GetString("Info.CouldNotAddFolder");
                ErrorManager.ShowInfo(msg + " " + exception.Message);
            }
        }

        #endregion

        #region Working With Existing Files

        bool cut;

        /// <summary>
        /// Notify of file action and allow plugins to handle the operation
        /// </summary>
        bool CancelAction(string name, object context)
        {
            var e = new DataEvent(EventType.Command, name, context);
            EventManager.DispatchEvent(this, e);
            return e.Handled;
        }

        public void CutToClipboard(string[] paths)
        {
            if (CancelAction(ProjectFileActionsEvents.FileCut, paths)) return;

            CopyToClipboard(paths);
            cut = true;
        }
        public void CopyToClipboard(string[] paths)
        {
            if (CancelAction(ProjectFileActionsEvents.FileCopy, paths)) return;

            cut = false;
            var o = new DataObject(DataFormats.FileDrop, paths);
            Clipboard.SetDataObject(o);
        }

        public void PasteFromClipboard(string toPath)
        {
            if (CancelAction(ProjectFileActionsEvents.FilePaste, toPath)) return;

            if (File.Exists(toPath)) 
                toPath = Path.GetDirectoryName(toPath);

            var o = (DataObject) Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.FileDrop))
            {
                // the data is in the form of an array of paths
                var aFiledrop = (Array)o.GetData(DataFormats.FileDrop);
                try
                {
                    foreach (string path in aFiledrop)
                    {
                        Copy(path, toPath);
                        if (cut) Delete(path, false);
                    }
                }
                catch (UserCancelException) { }
            }
            cut = false;
        }

        public void Delete(string path, bool confirm)
        {
            if (confirm && CancelAction(ProjectFileActionsEvents.FileDelete, new[] { path })) return;
            if (!confirm && CancelAction(ProjectFileActionsEvents.FileDeleteSilent, new[] { path })) return;

            try
            {
                PushCurrentDirectory();

                bool isDirectory = Directory.Exists(path);

                string name = Path.GetFileName(path);
                string caption = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.WillMoveToRecycleBin");

                if (isDirectory)
                {
                    string msg = TextHelper.GetString("Info.AndAllItsContents");
                    message = string.Format("\"{0}\" " + msg + " {1}", name, message);
                }
                else message = $"\"{name}\" {message}";

                var result = DialogResult.OK;
                if (confirm) result = MessageBox.Show(mainForm, message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    DisableWatchers();
                    if (!FileHelper.Recycle(path))
                    {
                        string error = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                        throw new Exception(error + " " + path);
                    }

                    OnFileDeleted(path);
                }
            }
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
            }
            finally
            {
                EnableWatchers();
                PopCurrentDirectory();
            }
        }

        public void Delete(string[] paths)
        {
            if (paths.Length == 1) Delete(paths[0], true);
            else
            {
                if (CancelAction(ProjectFileActionsEvents.FileDelete, paths)) return;

                try
                {
                    PushCurrentDirectory();

                    string caption = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                    string message = TextHelper.GetString("Info.ItemsWillBeMovedToRecycleBin");

                    DialogResult result = MessageBox.Show(mainForm, message, caption,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.OK)
                    {
                        DisableWatchers();

                        foreach (string path in paths)
                        {
                            // Once a directory is deleted we need to ignore all remaining files/sub-directories still in the paths
                            // array since they are already gone.
                            if (File.Exists(path) || Directory.Exists(path))
                            {
                                if (!FileHelper.Recycle(path))
                                {
                                    string error = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                                    throw new Exception(error + " " + path);
                                }
                            }
                            OnFileDeleted(path);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorManager.ShowError(exception);
                }
                finally
                {
                    EnableWatchers();
                    PopCurrentDirectory();
                }
            }
        }

        public bool Rename(string oldPath, string newName)
        {
            if (FolderHelper.IsIllegalFolderName(newName)) // is name illegal (ie. CON, AUX etc..)
            {
                string message = TextHelper.GetString("Info.ReservedDirName");
                ErrorManager.ShowInfo(string.Format(message, newName));
                return false;
            }

            bool isDirectory = Directory.Exists(oldPath);

            if (!isDirectory)
            {
                string oldExt = Path.GetExtension(oldPath);
                string newExt = Path.GetExtension(newName);

                string caption = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.ExtensionChangeWarning");

                if (oldExt.ToUpperInvariant() != newExt.ToUpperInvariant() &&
                    MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            }

            if (CancelAction(ProjectFileActionsEvents.FileRename, new[] { oldPath, newName })) return false;

            try
            {
                DisableWatchers();
                PushCurrentDirectory();

                string oldDir = Path.GetDirectoryName(oldPath);
                string newPath = Path.Combine(oldDir, newName);
                string originalOld = oldPath;

                OnFileCreated(newPath);

                if (isDirectory)
                {
                    // this is required for renaming directories, don't ask me why
                    string oldPathFixed = (oldPath.EndsWith('\\')) ? oldPath : oldPath + "\\";
                    string newPathFixed = (newPath.EndsWith('\\')) ? newPath : newPath + "\\";
                    if (oldPathFixed.Equals(newPathFixed, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        string tmpPath = newPathFixed.Substring(0, newPathFixed.Length - 1) + "$renaming$\\";
                        Directory.Move(oldPathFixed, tmpPath);
                        oldPathFixed = tmpPath;
                    }
                    if (FileHelper.ConfirmOverwrite(newPath))
                    {
                        FileHelper.ForceMoveDirectory(oldPathFixed, newPathFixed);
                    }
                    else return false;
                }
                else
                {
                    string oldName = Path.GetFileName(oldPath);
                    if (oldName.Equals(newName, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        string tmpPath = newPath + "$renaming$";
                        File.Move(oldPath, tmpPath);
                        oldPath = tmpPath;
                    }

                    if (FileHelper.ConfirmOverwrite(newPath))
                    {
                        FileHelper.ForceMove(oldPath, newPath);
                    }
                    else return false;
                }
                OnFileMoved(originalOld, newPath);
            }
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
                return false;
            }
            finally
            {
                EnableWatchers();
                PopCurrentDirectory();
            }
            return true;
        }

        public void Move(string fromPath, string toPath)
        {
            if (CancelAction(ProjectFileActionsEvents.FileMove, new[] { fromPath, toPath })) return;

            try
            {
                DisableWatchers();
                PushCurrentDirectory();

                // try to fix toPath if it's a filename
                if (File.Exists(toPath))
                    toPath = Path.GetDirectoryName(toPath);

                toPath = Path.Combine(toPath, Path.GetFileName(fromPath));

                if (!FileHelper.ConfirmOverwrite(toPath)) return;

                if (File.Exists(toPath))
                {
                    if (!FileHelper.Recycle(toPath))
                    {
                        string message = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                        throw new Exception(message + " " + toPath);
                    }
                }

                OnFileCreated(toPath);

                if (Directory.Exists(fromPath)) FileHelper.ForceMoveDirectory(fromPath, toPath);
                else File.Move(fromPath, toPath);

                OnFileMoved(fromPath, toPath);
            }
            catch (UserCancelException){}
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
            }
            finally
            {
                EnableWatchers();
                PopCurrentDirectory();
            }
        }

        public void Copy(string fromPath, string toPath)
        {
            if (CancelAction(ProjectFileActionsEvents.FileCopy, new[] { fromPath, toPath })) return;

            try
            {
                // try to fix toPath if it's a filename
                if (File.Exists(toPath))
                    toPath = Path.GetDirectoryName(toPath);

                // avoid recursive copy
                if (Directory.Exists(fromPath) && toPath.StartsWith(fromPath, StringComparison.OrdinalIgnoreCase))
                    throw new IOException(TextHelper.GetString("Info.RecursiveCopyDetected"));

                toPath = Path.Combine(toPath, Path.GetFileName(fromPath));

                // create copies of a file
                if (toPath == fromPath)
                {
                    string copyPath = Path.Combine(
                        Path.GetDirectoryName(toPath),
                        string.Format(TextHelper.GetString("Label.CopyOf"), Path.GetFileNameWithoutExtension(fromPath))
                        ) + Path.GetExtension(fromPath);

                    int copies = 1;
                    while (File.Exists(copyPath))
                    {
                        copies++;
                        copyPath = Path.Combine(
                            Path.GetDirectoryName(toPath),
                            string.Format(TextHelper.GetString("Label.CopyOf") + " ({1})", Path.GetFileNameWithoutExtension(fromPath), copies)
                            ) + Path.GetExtension(fromPath);
                    }

                    // offer to choose the new name
                    var label = TextHelper.GetString("Info.NewDuplicateName");
                    var title = string.Format(TextHelper.GetString("Info.DuplicatingFile"), Path.GetFileName(toPath));
                    var suggestion = Path.GetFileName(copyPath);
                    using var dialog = new LineEntryDialog(title, label, suggestion);
                    dialog.SelectRange(0, Path.GetFileNameWithoutExtension(copyPath).Length);
                    var choice = dialog.ShowDialog();
                    if (choice == DialogResult.OK && dialog.Line.Trim().Length > 0)
                    {
                        copyPath = Path.Combine(Path.GetDirectoryName(toPath), dialog.Line.Trim());
                    }
                    else throw new UserCancelException();

                    toPath = copyPath;
                }

                if (!FileHelper.ConfirmOverwrite(toPath)) return;

                OnFileCreated(toPath);

                if (Directory.Exists(fromPath)) FileHelper.CopyDirectory(fromPath, toPath, true);
                else File.Copy(fromPath, toPath, true);

                OnFilePasted(fromPath, toPath);
            }
            catch (UserCancelException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
            }
        }

        string CopyFile(string file, string toDirectory)
        {
            var fileName = Path.GetFileName(file);
            var filePath = Path.Combine(toDirectory, fileName);

            if (File.Exists(filePath))
            {
                var caption = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                var message = TextHelper.GetString("Info.FileAlreadyExistsInProject");

                var result = MessageBox.Show(mainForm, string.Format(message, filePath), 
                   caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return null;
            }
            File.Copy(file, filePath, true);
            return filePath;
        }

        void DisableWatchers()
        {
            var e = new DataEvent(EventType.Command, ProjectFileActionsEvents.FileDisableWatchers, null);
            EventManager.DispatchEvent(this, e);
        }

        void EnableWatchers()
        {
            var e = new DataEvent(EventType.Command, ProjectFileActionsEvents.FileEnableWatchers, null);
            EventManager.DispatchEvent(this, e);
        }

        #endregion

        #region Event Helpers

        void OnFileCreated(string path) => FileCreated?.Invoke(path);

        void OnFileMoved(string fromPath, string toPath) => FileMoved?.Invoke(fromPath, toPath);

        void OnFilePasted(string fromPath, string toPath) => FileCopied?.Invoke(fromPath, toPath);

        void OnFileDeleted(string path) => FileDeleted?.Invoke(path);

        void OnProjectModified(string[] paths) => ProjectModified?.Invoke(paths);

        void OnOpenFile(string path) => OpenFile?.Invoke(path);

        #endregion

        /// <summary>
        /// Thrown when user cancel a file operation
        /// </summary>
        class UserCancelException : Exception
        {
        }
    }
}
