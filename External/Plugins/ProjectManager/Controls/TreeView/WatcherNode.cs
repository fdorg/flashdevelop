using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Localization;

namespace ProjectManager.Controls.TreeView
{
    /// <summary>
    /// Represents a node that watches for changes to its children using a FileSystemWatcher.
    /// </summary>
    public class WatcherNode : DirectoryNode
    {
        Timer updateTimer;
        WatcherEx watcher;
        List<String> changedPaths;
        String[] excludedFiles;
        String[] excludedDirs;
        Boolean updateNeeded;

        public WatcherNode(string directory) : base(directory)
        {
            isRefreshable = true;
            changedPaths = new List<String>();
            excludedDirs = PluginMain.Settings.ExcludedDirectories.Clone() as String[];
            excludedFiles = PluginMain.Settings.ExcludedFileTypes.Clone() as String[];
            // Use a timer for FileSystemWatcher updates so they don't do lots of redrawing
            updateTimer = new Timer();
            updateTimer.SynchronizingObject = Tree;
            updateTimer.Interval = 500;
            updateTimer.Elapsed += updateTimer_Tick;
            setWatcher();
        }

        private void setWatcher()
        {
            try
            {
                if (Directory.Exists(BackingPath))
                {
                    watcher = new WatcherEx(BackingPath);
                    if (watcher.IsRemote)
                        watcher.Changed += watcher_Changed;
                    else
                    {
                        watcher.Created += watcher_Created;
                        watcher.Deleted += watcher_Deleted;
                        watcher.Renamed += watcher_Renamed;
                    }
                    watcher.EnableRaisingEvents = true;
                }
            }
            catch {}
        }

        public override void Refresh(Boolean recursive)
        {
            if (watcher == null) setWatcher();
            base.Refresh(recursive);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        public void UpdateLater()
        {
            updateNeeded = true;
            updateTimer.Enabled = true;
        }

        private bool AppendPath(FileSystemEventArgs e)
        {
            lock (this.changedPaths)
            {
                try
                {
                    String fullPath = e.FullPath.TrimEnd('\\');
                    String path = Path.GetDirectoryName(fullPath);
                    return AppendToChangedPaths(fullPath, path, e.ChangeType);
                }
                catch 
                {
                    return false;
                }
            }
        }

        private bool AppendToChangedPaths(string fullPath, string path, WatcherChangeTypes changeType)
        {
            if (this.excludedDirs != null) // filter ignored paths
            {
                Char separator = Path.DirectorySeparatorChar;
                foreach (String excludedDir in this.excludedDirs)
                {
                    if (path.IndexOfOrdinal(separator + excludedDir + separator) > 0) return false;
                }
            }
            if (this.excludedFiles != null && File.Exists(fullPath)) // filter ignored filetypes
            {
                String extension = Path.GetExtension(fullPath);
                foreach (String excludedFile in this.excludedFiles)
                {
                    if (extension == excludedFile) return false;
                }
            }
            if (changeType != WatcherChangeTypes.Created && changeType != WatcherChangeTypes.Renamed
                && Directory.Exists(fullPath))
            {
                if (!this.changedPaths.Contains(fullPath))
                    this.changedPaths.Add(fullPath);
            }
            else if (!this.changedPaths.Contains(path) && Directory.Exists(path))
            {
                this.changedPaths.Add(path);
            }
            return true;
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (AppendPath(e))
                Changed();
        }
        private void watcher_Created(object sender, FileSystemEventArgs e) 
        {
            if (AppendPath(e))
                Changed(); 
        }
        private void watcher_Deleted(object sender, FileSystemEventArgs e) 
        {
            if (AppendPath(e))
                Changed(); 
        }
        private void watcher_Renamed(object sender, RenamedEventArgs e) 
        {
            if (AppendPath(e))
                Changed();
        }

        private void Changed()
        {
            // have we been deleted already?
            if (!Directory.Exists(BackingPath)) return;
            updateNeeded = true;
            updateTimer.Enabled = false; // reset timer
            updateTimer.Enabled = true;
        }

        private void Update()
        {
            if (!updateNeeded) return;
            updateTimer.Enabled = false;
            try
            {
                Tree.BeginUpdate();
                String[] paths = this.changedPaths.ToArray();
                this.changedPaths.Clear();
                Tree.RefreshTree(paths);
            }
            catch {}
            finally
            {
                Tree.EndUpdate();
                updateNeeded = false;
                excludedDirs = PluginMain.Settings.ExcludedDirectories.Clone() as String[];
                excludedFiles = PluginMain.Settings.ExcludedFileTypes.Clone() as String[];
            }
            // new folder name edition
            if (Tree.PathToSelect != null && Tree.SelectedNode != null && Tree.SelectedNode is DirectoryNode && (Tree.SelectedNode as DirectoryNode).BackingPath == Tree.PathToSelect)
            {
                DirectoryNode node = Tree.SelectedNode as DirectoryNode;
                Tree.PathToSelect = null;
                node.EnsureVisible();
                // if you created a new folder, then label edit it!
                string label = TextHelper.GetStringWithoutMnemonicsOrEllipsis("Label.NewFolder");
                if (node.Text.StartsWithOrdinal(label))
                {
                    node.BeginEdit();
                }
            }
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;
            Update();
        }

    }

}
