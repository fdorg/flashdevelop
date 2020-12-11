// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        internal static readonly List<IVCManager> VCManagers = new List<IVCManager>();
        static FSWatchers fsWatchers;
        static OverlayManager ovManager;
        static readonly HashSet<string> addBuffer = new HashSet<string>();

        public static bool Initialized { get; private set; }

        public static Image Skin { get; set; }
        public static Project CurrentProject { get; private set; }
        public static VCManager VCManager { get; private set; }

        public static void Init()
        {
            if (Initialized) return;
            if (Skin is null)
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    Skin = new Bitmap(assembly.GetManifestResourceStream(ScaleHelper.GetScale() > 1.5 ? "SourceControl.Resources.icons32.png" : "SourceControl.Resources.icons.png"));
                }
                catch
                {
                    Skin = ScaleHelper.GetScale() > 1.5 ? new Bitmap(320, 32) : new Bitmap(160, 16);
                }
            }
            
            fsWatchers = new FSWatchers();
            ovManager = new OverlayManager(fsWatchers);
            VCManager = new VCManager(ovManager);

            SetProject(PluginBase.CurrentProject as Project);

            Initialized = true;
        }

        internal static void Dispose()
        {
            if (VCManager is null) return;
            VCManager.Dispose();
            fsWatchers.Dispose();
            ovManager.Dispose();
            CurrentProject = null;
        }

        internal static void SetProject(Project project)
        {
            CurrentProject = project;

            fsWatchers.SetProject(project);
            ovManager.Reset();

            foreach (var document in PluginBase.MainForm.Documents)
                if (document.SciControl is { } sci) 
                    HandleFileReload(sci.FileName);
        }

        internal static void SelectionChanged() => ovManager.SelectionChanged();

        internal static void ForceRefresh() => fsWatchers.ForceRefresh();


        #region file actions

        internal static bool HandleFileBeforeRename(string path)
        {
            return fsWatchers.ResolveVC(path, true) is { } result
                   && result.Status != VCItemStatus.Unknown
                   && result.Manager.FileActions.FileBeforeRename(path);
        }

        internal static bool HandleFileRename(string[] paths)
        {
            return fsWatchers.ResolveVC(paths[0], true) is { } result
                   && result.Status != VCItemStatus.Unknown
                   && result.Manager.FileActions.FileRename(paths[0], paths[1]);
        }

        internal static bool HandleFileDelete(string[] paths, bool confirm)
        {
            if (paths.IsNullOrEmpty()) return false;
            var result = fsWatchers.ResolveVC(Path.GetDirectoryName(paths[0]));
            if (result is null) return false;

            var svnRemove = new List<string>();
            var regularRemove = new List<string>();
            var hasModification = new List<string>();
            var hasUnknown = new List<string>();
            try
            {
                foreach (var path in paths)
                {
                    result = fsWatchers.ResolveVC(path, true);
                    if (result is null || result.Status == VCItemStatus.Unknown || result.Status == VCItemStatus.Ignored)
                    {
                        regularRemove.Add(path);
                    }
                    else
                    {
                        var manager = result.Manager;
                        var root = result.Watcher.Path;
                        var p = root.Length + 1;

                        if (Directory.Exists(path))
                        {
                            var files = new List<string>();
                            GetAllFiles(path, files);
                            foreach (var file in files)
                            {
                                var status = manager.GetOverlay(file, root);
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
            catch (UnsafeOperationException ex)
            {
                MessageBox.Show(ex.Message, TextHelper.GetString("SourceControl.Info.UnsafeDeleteOperation"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return true; // prevent regular deletion
            }

            if (hasUnknown.Count > 0 && confirm) //this never happens (at least on git), because it is always handled by the "regular deletion" part above
            {
                var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                var msg = TextHelper.GetString("SourceControl.Info.ConfirmUnversionedDelete") + "\n\n" + GetSomeFiles(hasUnknown);
                if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
            }

            if (hasModification.Count > 0 && confirm)
            {
                var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                var msg = TextHelper.GetString("SourceControl.Info.ConfirmLocalModsDelete") + "\n\n" + GetSomeFiles(hasModification);
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
            if (result is null || result.Status == VCItemStatus.Unknown)
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

        static string GetSomeFiles(List<string> list)
        {
            if (list.Count < 10) return string.Join("\n", list);
            return string.Join("\n", list.GetRange(0, 9)) + "\n(...)\n" + list[list.Count - 1];
        }

        static void GetAllFiles(string path, ICollection<string> files)
        {
            var search = Directory.GetFiles(path);
            foreach (var file in search)
            {
                var name = Path.GetFileName(file);
                if (name[0] == '.') continue;
                files.Add(file);
            }

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                var name = Path.GetFileName(dir);
                if (name[0] == '.') continue;
                var info = new FileInfo(dir);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                GetAllFiles(dir, files);
            }
        }

        internal static bool HandleFileMove(string[] paths)
        {
            var result = fsWatchers.ResolveVC(paths[0], true);
            var result2 = fsWatchers.ResolveVC(paths[1], true);

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
            if (fResult is null) return;
            var result = fsWatchers.ResolveVC(toFile, true);
            fResult.Manager.FileActions.FileAfterMove(fromFile, fResult.Status, toFile);
            ForceRefresh();
            if (fResult.Status < VCItemStatus.UpToDate || fResult.Status == VCItemStatus.Added) return;
            if (result != null && result.Status >= VCItemStatus.UpToDate && result.Status != VCItemStatus.Added)
                AskWithTwoFiles(result.Manager, "Moved", fromFile, toFile, true);
            else //counts as delete
                HandleFilesDeleted(new[] { fromFile });
        }

        internal static bool HandleBuildProject()
        {
            return fsWatchers.ResolveVC(CurrentProject.OutputPathAbsolute, true) is { } result
                   && result.Status != VCItemStatus.Unknown
                   && result.Manager.FileActions.BuildProject();
        }

        internal static bool HandleTestProject()
        {
            return fsWatchers.ResolveVC(CurrentProject.OutputPathAbsolute, true) is { } result
                   && result.Status != VCItemStatus.Unknown
                   && result.Manager.FileActions.TestProject();
        }

        internal static bool HandleSaveProject(string fileName)
        {
            return fsWatchers.ResolveVC(fileName, true) is { } result
                   && result.Status != VCItemStatus.Unknown
                   && result.Manager.FileActions.SaveProject();
        }

        internal static bool HandleFileNew(string path)
        {
            if (!Initialized) return false;
            var result = fsWatchers.ResolveVC(path, true);
            if (result is null || result.Status == VCItemStatus.Unknown || result.Status == VCItemStatus.Ignored)
                return false;

            addBuffer.Add(path); //at this point there is not yet an ITabbedDocument for the file
            return false;
        }

        internal static bool HandleFileOpen(string path)
        {
            if (!Initialized) return false;
            var result = fsWatchers.ResolveVC(path, true);
            if (result is null) return false;
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
            
            return result.Status != VCItemStatus.Unknown && result.Manager.FileActions.FileOpen(path);
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
            if (!Initialized) return false;

            var result = fsWatchers.ResolveVC(path, true);
            if (result is null || result.Status == VCItemStatus.Unknown)
                return false;
            return result.Manager.FileActions.FileReload(path);
        }

        internal static bool HandleFileModifyRO(string path)
        {
            if (!Initialized) return false;
            var result = fsWatchers.ResolveVC(path, true);
            if (result is null || result.Status == VCItemStatus.Unknown)
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

        static string GetRelativeFile(string file) => PluginBase.CurrentProject != null
            ? PluginBase.CurrentProject.GetRelativePath(file)
            : file;

        static string AskForCommit(string message)
        {
            if (PluginMain.SCSettings.NeverCommit) return null;

            var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            var msg = TextHelper.GetString("Info.CreateCommit");

            using var dialog = new LineEntryDialog(title, msg, message);
            if (dialog.ShowDialog() == DialogResult.Cancel) //Never
            {
                PluginMain.SCSettings.NeverCommit = true;
                return null;
            }
            if (dialog.ShowDialog() != DialogResult.Yes || dialog.Line == "") return null;
            return dialog.Line;
        }
    }

    internal class UnsafeOperationException : Exception
    {
        public UnsafeOperationException(string message)
            : base(message)
        {
        }
    }
}