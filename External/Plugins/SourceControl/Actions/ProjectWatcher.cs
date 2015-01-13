using System;
using System.Collections.Generic;
using PluginCore;
using System.IO;
using SourceControl.Sources;
using SourceControl.Managers;
using ProjectManager.Projects;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using PluginCore.Localization;

namespace SourceControl.Actions
{
    public static class ProjectWatcher
    {
        private static bool initialized = false;

        static internal readonly List<IVCManager> VCManagers = new List<IVCManager>();
        static VCManager vcManager;
        static FSWatchers fsWatchers;
        static OverlayManager ovManager;
        static Project currentProject;

        public static bool Initialized { get { return initialized; } }
        public static Image Skin { get; set; }
        public static Project CurrentProject { get { return currentProject; } }
        public static VCManager VCManager { get { return vcManager; } }

        public static void Init()
        {
            if (initialized)
                return;

            if (Skin == null)
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Skin = new Bitmap(assembly.GetManifestResourceStream("SourceControl.Resources.icons.png"));
                }
                catch
                {
                    Skin = new Bitmap(160, 16);
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
            WatcherVCResult result = fsWatchers.ResolveVC(paths[0], true);
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

            if (hasUnknown.Count > 0 && confirm)
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

            return result.Manager.FileActions.FileDelete(paths, confirm);
        }

        private static string GetSomeFiles(List<string> list)
        {
            if (list.Count < 10) return String.Join("\n", list.ToArray());
            return String.Join("\n", list.GetRange(0, 9).ToArray()) + "\n(...)\n" + list[list.Count - 1];
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
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false; // origin not under VC, ignore
            WatcherVCResult result2 = fsWatchers.ResolveVC(paths[1], true);
            if (result2 == null || result2.Status == VCItemStatus.Unknown)
                return false; // target dir not under VC, ignore

            return result.Manager.FileActions.FileMove(paths[0], paths[1]);
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
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileNew(path);
        }

        internal static bool HandleFileOpen(string path)
        {
            if (!initialized)
                return false;

            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileOpen(path);
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

    }
    
    class UnsafeOperationException:Exception
    {
        public UnsafeOperationException(string message)
            : base(message)
        {
        }
    }

}
