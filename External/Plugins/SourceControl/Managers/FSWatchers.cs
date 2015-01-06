﻿using System;
using System.Collections.Generic;
using SourceControl.Sources;
using System.Windows.Forms;
using System.IO;
using SourceControl.Actions;
using ProjectManager.Projects;

namespace SourceControl.Managers
{
    public class FSWatchers
    {
        private Dictionary<FileSystemWatcher, IVCManager> watchers = new Dictionary<FileSystemWatcher, IVCManager>();
        private List<IVCManager> dirtyVC = new List<IVCManager>();
        private System.Timers.Timer updateTimer;
        private string lastDirtyPath;
        private bool disposing;

        public FSWatchers()
        {
            updateTimer = new System.Timers.Timer();
            updateTimer.SynchronizingObject = PluginCore.PluginBase.MainForm as Form;
            updateTimer.Interval = 4000;
            updateTimer.Elapsed += UpdateTimer_Tick;
            updateTimer.Start();
        }

        internal void Dispose()
        {
            disposing = true;
            Clear();
        }

        private void Clear()
        {
            try
            {
                if (updateTimer != null) updateTimer.Stop();
                lock (watchers)
                {
                    foreach (FileSystemWatcher watcher in watchers.Keys)
                    {
                        watcher.EnableRaisingEvents = false;
                        watcher.Dispose();
                    }
                    watchers.Clear();
                }
            }
            catch (Exception) { }
        }

        internal WatcherVCResult ResolveVC(string path, bool andStatus)
        {
            WatcherVCResult result = ResolveVC(path);
            if (result != null && andStatus) result.Status = result.Manager.GetOverlay(path, result.Watcher.Path);
            return result;
        }

        internal WatcherVCResult ResolveVC(string path)
        {
            foreach (FileSystemWatcher watcher in watchers.Keys)
                if (path.StartsWith(watcher.Path))
                    return new WatcherVCResult(watcher, watchers[watcher]);
            return null;
        }

        internal void SetProject(Project project)
        {
            Clear();
            if (project == null) return;
            CreateWatchers(project.Directory);
            if (project.Classpaths != null)
                foreach (string path in project.AbsoluteClasspaths)
                    CreateWatchers(path);
            if (ProjectManager.PluginMain.Settings.ShowGlobalClasspaths)
                foreach (string path in ProjectManager.PluginMain.Settings.GlobalClasspaths)
                    CreateWatchers(path);
            updateTimer.Start();
        }

        internal void ForceRefresh()
        {
            lock (dirtyVC)
            {
                foreach (FileSystemWatcher watcher in watchers.Keys)
                {
                    IVCManager manager = watchers[watcher];
                    if (!dirtyVC.Contains(manager)) dirtyVC.Add(manager);
                }
            }
            updateTimer.Stop();
            updateTimer.Start();
        }

        private void CreateWatchers(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;
                foreach (FileSystemWatcher watcher in watchers.Keys)
                    if (path.StartsWith(watcher.Path, StringComparison.OrdinalIgnoreCase))
                        return;
                ExploreDirectory(path, true, 0);
            }
            catch { }
        }

        private void ExploreDirectory(string path, bool rootDir, int depth)
        {
            foreach (IVCManager manager in ProjectWatcher.VCManagers)
            {
                if (manager.IsPathUnderVC(path))
                {
                    CreateWatcher(path, manager);
                    return;
                }
            }
            if (rootDir && ParentDirUnderVC(path)) return;
            if (depth >= 3) return;
            foreach (string dir in Directory.GetDirectories(path))
            {
                FileInfo info = new FileInfo(dir);
                if ((info.Attributes & FileAttributes.Hidden) == 0) ExploreDirectory(dir, false, depth++);
            }
        }

        private void CreateWatcher(string path, IVCManager manager)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.Attributes;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher, manager);
            dirtyVC.Add(manager);
        }

        private bool ParentDirUnderVC(string path)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                do
                {
                    info = info.Parent;
                    foreach (FileSystemWatcher watcher in watchers.Keys)
                        if (info.FullName.StartsWith(watcher.Path, StringComparison.OrdinalIgnoreCase))
                            return true;
                    foreach (IVCManager manager in ProjectWatcher.VCManagers)
                        if (manager.IsPathUnderVC(info.FullName))
                        {
                            CreateWatcher(path, manager);
                            return true;
                        }
                }
                while (info.Parent != null);
            }
            catch { }
            return false;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (lastDirtyPath != null && e.FullPath.StartsWith(lastDirtyPath)) return;
            lastDirtyPath = e.FullPath;
            FileSystemWatcher watcher = (FileSystemWatcher)sender;
            IVCManager manager = watchers[watcher];
            if (manager.SetPathDirty(e.FullPath, watcher.Path)) Changed(manager);
        }

        private void Changed(IVCManager manager)
        {
            if (disposing || updateTimer == null) return;
            lock (dirtyVC)
            {
                if (!dirtyVC.Contains(manager)) dirtyVC.Add(manager);
            }
            updateTimer.Stop();
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateTimer.Stop();
            lastDirtyPath = null;
            if (disposing) return;
            lock (dirtyVC)
            {
                foreach (FileSystemWatcher watcher in watchers.Keys)
                {
                    IVCManager manager = watchers[watcher];
                    if (dirtyVC.Contains(manager)) manager.GetStatus(watcher.Path);
                }
                dirtyVC.Clear();
            }
        }
    }

    class WatcherVCResult
    {
        public FileSystemWatcher Watcher;
        public IVCManager Manager;
        public VCItemStatus Status;

        public WatcherVCResult(FileSystemWatcher watcher, IVCManager manager)
        {
            Watcher = watcher;
            Manager = manager;
        }
    }
}