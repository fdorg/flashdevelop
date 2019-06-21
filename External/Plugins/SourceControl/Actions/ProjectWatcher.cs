using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;
using SourceControl.Managers;
using SourceControl.Sources;
using SourceControl.Dialogs;

namespace SourceControl.Actions
{
    public static class ProjectWatcher
    {
        private static bool initialized = false;

        internal static readonly List<IVCManager> VCManagers = new List<IVCManager>();
        static VCManager vcManager;
        static FSWatchers fsWatchers;
        static OverlayManager ovManager;
        static Project currentProject;
        static HashSet<string> addBuffer = new HashSet<string>();

        public static bool Initialized => initialized;
        public static Image Skin { get; set; }
        public static Project CurrentProject => currentProject;
        public static VCManager VCManager => vcManager;

        public static void Init()
        {
            if (initialized)
                return;

            if (Skin == null)
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Skin = new Bitmap(assembly.GetManifestResourceStream(ScaleHelper.GetScale() > 1.5 ? "SourceControl.Resources.icons32.png" : "SourceControl.Resources.icons.png"));
                }
                catch
                {
                    Skin = ScaleHelper.GetScale() > 1.5 ? new Bitmap(320, 32) : new Bitmap(160, 16);
                }
            }
            
            fsWatchers = new FSWatchers();
            ovManager = new OverlayManager(fsWatchers);
            vcManager = new VCManager(ovManager);

            SetProject(PluginBase.CurrentProject as Project);

            initialized = true;
        }

        internal static void Dispose()
        {
            if (vcManager != null)
            {
                vcManager.Dispose();
                fsWatchers.Dispose();
                ovManager.Dispose();
                currentProject = null;
            }
        }

        internal static void SetProject(Project project)
        {
            currentProject = project;

            fsWatchers.SetProject(project);
            ovManager.Reset();

            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
                if (document.IsEditable) HandleFileReload(document.FileName);
        }

        internal static void SelectionChanged()
        {
            ovManager.SelectionChanged();
        }

        internal static void ForceRefresh()
        {
            fsWatchers.ForceRefresh();
        }


        #region file actions

        internal static bool HandleFileBeforeRename(string path)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileBeforeRename(path);
        }

        internal static bool HandleFileRename(string[] paths)
        {
            var result = fsWatchers.ResolveVC(paths[0], true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileRename(paths[0], paths[1]);
        }

        internal static bool HandleFileDelete(string[] paths, bool confirm)
        {
            if (paths == null || paths.Length == 0) return false;
            WatcherVCResult result = fsWatchers.ResolveVC(Path.GetDirectoryName(paths[0]));
            if (result == null) return false;

            List<string> svnRemove = new List<string>();
            List<string> regularRemove = new List<string>();
            List<string> hasModification = new List<string>();
            List<string> hasUnknown = new List<string>();
            try
            {
                foreach (string path in paths)
                {
                    result = fsWatchers.ResolveVC(path, true);
                    if (result == null || result.Status == VCItemStatus.Unknown || result.Status == VCItemStatus.Ignored)
                    {
                        regularRemove.Add(path);
                    }
                    else
                    {
                        IVCManager manager = result.Manager;
                        string root = result.Watcher.Path;
                        int p = root.Length + 1;

                        if (Directory.Exists(path))
                        {
                            List<string> files = new List<string>();
                            GetAllFiles(path, files);
                            foreach (string file in files)
                            {
                                VCItemStatus status = manager.GetOverlay(file, root);
                                if (status == VCItemStatus.Unknown || status == VCItemStatus.Ignored)
                                    hasUnknown.Add(file.Substring(p));
                                else if (status > VCItemStatus.UpToDate)
                                    hasModification.Add(file.Substring(p));
                            }
                        }
                        else if (result.Status > VCItemStatus.UpToDate)
                            hasModification.Add(path);

                        if (svnRemove.Count > 0)
                        {
                            if (Path.GetDirectoryName(svnRemove[0]) != Path.GetDirectoryName(path))
                                throw new UnsafeOperationException(TextHelper.GetString("SourceControl.Info.ElementsLocatedInDiffDirs"));
                        }
                        svnRemove.Add(path);
                    }
                }
                if (regularRemove.Count > 0 && svnRemove.Count > 0)
                    throw new UnsafeOperationException(TextHelper.GetString("SourceControl.Info.MixedSelectionOfElements"));

                if (svnRemove.Count == 0 && regularRemove.Count > 0)
                    return false; // regular deletion
            }
            catch (UnsafeOperationException upex)
            {
                MessageBox.Show(upex.Message, TextHelper.GetString("SourceControl.Info.UnsafeDeleteOperation"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return true; // prevent regular deletion
            }

            if (hasUnknown.Count > 0 && confirm) //this never happens (at least on git), because it is always handled by the "regular deletion" part above
            {
                string title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string msg = TextHelper.GetString("SourceControl.Info.ConfirmUnversionedDelete") + "\n\n" + GetSomeFiles(hasUnknown);
                if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
            }

            if (hasModification.Count > 0 && confirm)
            {
                string title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string msg = TextHelper.GetString("SourceControl.Info.ConfirmLocalModsDelete") + "\n\n" + GetSomeFiles(hasModification);
                if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
            }
            else if (svnRemove.Count > 0) //there are versioned files
            {
                if (confirm)
                {
                    var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                    var msg = TextHelper.GetString("Info.RemoveFiles") + "\n\n" + GetSomeFiles(hasModification);
                    if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
                }

                return result.Manager.FileActions.FileDelete(svnRemove.ToArray(), confirm);
            }

            return false;
        }

        public static void HandleFilesDeleted(string[] files)
        {
            var result = fsWatchers.ResolveVC(files[0], true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return;

            var msg = "Deleted";
            foreach (var file in files)
            {
                msg += " " + GetRelativeFile(file);
            }

            var message = AskForCommit(msg);

            if (message != null)
                result.Manager.Commit(files, message);
                
        }

        private static string GetSomeFiles(List<string> list)
        {
            if (list.Count < 10) return string.Join("\n", list.ToArray());
            return string.Join("\n", list.GetRange(0, 9).ToArray()) + "\n(...)\n" + list[list.Count - 1];
        }

        private static void GetAllFiles(string path, List<string> files)
        {
            string[] search = Directory.GetFiles(path);
            foreach (string file in search)
            {
                string name = Path.GetFileName(file);
                if (name[0] == '.') continue;
                files.Add(file);
            }

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string name = Path.GetFileName(dir);
                if (name[0] == '.') continue;
                FileInfo info = new FileInfo(dir);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                GetAllFiles(dir, files);
            }
        }

        internal static bool HandleFileMove(string[] paths)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(paths[0], true);
            WatcherVCResult result2 = fsWatchers.ResolveVC(paths[1], true);

            var fromVCed = result != null && result.Status >= VCItemStatus.UpToDate && result.Status != VCItemStatus.Added;
            var toVCed = result2 != null && result2.Status >= VCItemStatus.UpToDate && result2.Status != VCItemStatus.Added;

            if (!fromVCed || !toVCed) // origin or target not under VC, ignore
                return false;

            return result.Manager.FileActions.FileMove(paths[0], paths[1]);
        }

        /// <summary>
        /// Called after a file was sucessfully moved.
        /// </summary>
        internal static void HandleFileMoved(string fromFile, string toFile)
        {
            var fResult = fsWatchers.ResolveVC(fromFile, true);
            var result = fsWatchers.ResolveVC(toFile, true);

            var fromVCed = fResult != null && fResult.Status >= VCItemStatus.UpToDate && fResult.Status != VCItemStatus.Added;
            var toVCed = result != null && result.Status >= VCItemStatus.UpToDate && result.Status != VCItemStatus.Added;

            if (fromVCed && toVCed)
                AskWithTwoFiles(result.Manager, "Moved", fromFile, toFile, true);
            else if (fromVCed) //counts as delete
                HandleFilesDeleted(new[] { fromFile });
        }

        internal static bool HandleBuildProject()
        {
            WatcherVCResult result = fsWatchers.ResolveVC(currentProject.OutputPathAbsolute, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.BuildProject();
        }

        internal static bool HandleTestProject()
        {
            WatcherVCResult result = fsWatchers.ResolveVC(currentProject.OutputPathAbsolute, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.TestProject();
        }

        internal static bool HandleSaveProject(string fileName)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(fileName, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.SaveProject();
        }

        internal static bool HandleFileNew(string path)
        {
            if (!initialized)
                return false;

            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown || result.Status == VCItemStatus.Ignored)
                return false;

            addBuffer.Add(path); //at this point there is not yet an ITabbedDocument for the file

            return false;
        }

        internal static bool HandleFileOpen(string path)
        {
            if (!initialized)
                return false;

            var result = fsWatchers.ResolveVC(path, true);
            if (result == null)
                return false;

            if (addBuffer.Remove(path) || result.Status == VCItemStatus.Unknown)
            {
                var yes = TextHelper.GetString("Label.Yes");
                var no = TextHelper.GetString("Label.No");

                MessageBar.ShowQuestion(TextHelper.GetString("Info.AddFile"), new[] { yes, no },
                    s =>
                    {
                        if (s == yes)
                        {
                            result.Manager.FileActions.FileNew(path);
                            ForceRefresh();
                        }

                        TraceManager.Add(s);
                    });
            }
            
            if (result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileOpen(path);
        }

        internal static void HandleFileCopied(string fromFile, string toFile)
        {
            var fResult = fsWatchers.ResolveVC(fromFile, true);
            var result = fsWatchers.ResolveVC(toFile, true);

            var fromVCed = fResult != null && fResult.Status >= VCItemStatus.UpToDate && fResult.Status != VCItemStatus.Added;
            var toVCed = result != null && result.Status >= VCItemStatus.UpToDate && result.Status != VCItemStatus.Added;

            if (fromVCed && toVCed)
            {
                AskWithTwoFiles(result.Manager, "Copied", fromFile, toFile, false);
            }
            else if (toVCed)
            {
                var toFileRelative = GetRelativeFile(toFile);
                var message = AskForCommit($"Created {toFileRelative}"); //note: we do not know if it actually is created (could be replaced, etc.)

                if (message != null)
                    result.Manager.Commit(new[] {toFile}, message);
            }
        }

        internal static bool HandleFileReload(string path)
        {
            if (!initialized)
                return false;

            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileReload(path);
        }

        internal static bool HandleFileModifyRO(string path)
        {
            if (!initialized)
                return false;

            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileModifyRO(path);
        }

        #endregion

        static void AskWithTwoFiles(IVCManager manager, string verb, string from, string to, bool commitBoth)
        {
            var fromFileRelative = GetRelativeFile(from);
            var toFileRelative = GetRelativeFile(to);

            var message = AskForCommit($"{verb} {fromFileRelative} to {toFileRelative}");

            if (message != null)
                manager.Commit(commitBoth ? new[] { from, to } : new []{ to }, message);
        }

        static string GetRelativeFile(string file)
        {
            return PluginBase.CurrentProject != null ? PluginBase.CurrentProject.GetRelativePath(file) : file;
        }

        static string AskForCommit(string message)
        {
            if (PluginMain.SCSettings.NeverCommit)
                return null;

            var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            var msg = TextHelper.GetString("Info.CreateCommit");

            using (LineEntryDialog led = new LineEntryDialog(title, msg, message))
            {
                var result = led.ShowDialog();
                if (result == DialogResult.Cancel) //Never
                {
                    PluginMain.SCSettings.NeverCommit = true;
                    return null;
                }
                if (result != DialogResult.Yes || led.Line == "")
                    return null;

                return led.Line;
            }
        }
    }
    
    class UnsafeOperationException:Exception
    {
        public UnsafeOperationException(string message)
            : base(message)
        {
        }
    }

}
