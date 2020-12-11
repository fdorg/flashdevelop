﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using PluginCore;
using ProjectManager.Projects;
using SourceControl.Actions;
using SourceControl.Sources;
using Timer = System.Timers.Timer;

namespace SourceControl.Managers
{
    public class FSWatchers
    {
        readonly Dictionary<FileSystemWatcher, IVCManager> watchers = new Dictionary<FileSystemWatcher, IVCManager>();
        readonly HashSet<IVCManager> dirtyVC = new HashSet<IVCManager>();
        readonly Timer updateTimer = new Timer {SynchronizingObject = PluginBase.MainForm as Form, Interval = 4000};
        string lastDirtyPath;
        bool disposing;

        public FSWatchers()
        {
            updateTimer.Elapsed += UpdateTimer_Tick;
            updateTimer.Start();
        }

        internal void Dispose()
        {
            disposing = true;
            Clear();
        }

        void Clear()
        {
            try
            {
                updateTimer?.Stop();
                lock (watchers)
                {
                    foreach (var watcher in watchers.Keys)
                    {
                        watcher.EnableRaisingEvents = false;
                        watcher.Dispose();
                    }
                    watchers.Clear();
                }
            }
            catch { }
        }

        internal WatcherVCResult? ResolveVC(string path, bool andStatus)
        {
            var result = ResolveVC(path);
            if (result != null && andStatus)
                result.Status = result.Manager.GetOverlay(path, result.Watcher.Path);

            return result;
        }

        internal WatcherVCResult? ResolveVC(string path)
        {
            foreach (var watcher in watchers.Keys)
                if (path.StartsWithOrdinal(watcher.Path))
                    return new WatcherVCResult(watcher, watchers[watcher]);
            return null;
        }

        internal void SetProject(Project project)
        {
            Clear();
            if (project is null) return;

            CreateWatchers(project.Directory);

            if (project.Classpaths != null)
                foreach (var path in project.AbsoluteClasspaths)
                    CreateWatchers(path);

            if (ProjectManager.PluginMain.Settings.ShowGlobalClasspaths)
                foreach (var path in ProjectManager.PluginMain.Settings.GlobalClasspaths)
                    CreateWatchers(path);

            updateTimer.Interval = 4000;
            updateTimer.Start();
        }

        void CreateWatchers(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return;

                foreach (var watcher in watchers.Keys)
                    if (path.StartsWith(watcher.Path, StringComparison.OrdinalIgnoreCase))
                        return;

                ExploreDirectory(path, true, 0);
            }
            catch { }
        }

        void ExploreDirectory(string path, bool rootDir, int depth)
        {
            foreach (var manager in ProjectWatcher.VCManagers)
                if (manager.IsPathUnderVC(path))
                {
                    CreateWatcher(path, manager);
                    return;
                }

            if (rootDir && ParentDirUnderVC(path))
                return;

            if (depth < 3)
                foreach (var dir in Directory.GetDirectories(path))
                {
                    var info = new FileInfo(dir);
                    if ((info.Attributes & FileAttributes.Hidden) == 0)
                        ExploreDirectory(dir, false, depth++);
                }
        }

        void CreateWatcher(string path, IVCManager manager)
        {
            var watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.Attributes;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher, manager);

            dirtyVC.Add(manager);
        }

        bool ParentDirUnderVC(string path)
        {
            try
            {
                var info = new DirectoryInfo(path);
                do
                {
                    info = info.Parent;

                    foreach (var watcher in watchers.Keys)
                        if (info.FullName.StartsWith(watcher.Path, StringComparison.OrdinalIgnoreCase))
                            return true;

                    foreach (var manager in ProjectWatcher.VCManagers)
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

        void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (lastDirtyPath != null && e.FullPath.StartsWithOrdinal(lastDirtyPath))
                return;
            lastDirtyPath = e.FullPath;
            
            var watcher = (FileSystemWatcher)sender;
            var manager = watchers[watcher];
            if (manager.SetPathDirty(e.FullPath, watcher.Path))
                Changed(manager);
        }

        public void Changed(IVCManager manager)
        {
            if (disposing || updateTimer is null) return;

            lock (dirtyVC)
            {
                dirtyVC.Add(manager);
            }

            updateTimer.Stop();
            updateTimer.Start();
        }

        void UpdateTimer_Tick(object sender, ElapsedEventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Interval = 4000;
            lastDirtyPath = null;
            if (disposing)
                return;

            lock (dirtyVC)
            {
                foreach (var watcher in watchers.Keys)
                {
                    var manager = watchers[watcher];
                    if (dirtyVC.Contains(manager))
                        manager.GetStatus(watcher.Path);
                }
                dirtyVC.Clear();
            }
        }

        internal void ForceRefresh()
        {
            lock (dirtyVC)
            {
                foreach (var watcher in watchers.Keys)
                {
                    var manager = watchers[watcher];
                    dirtyVC.Add(manager);
                }
            }

            updateTimer.Stop();
            updateTimer.Interval = 4000;
            updateTimer.Start();
        }
    }

    internal class WatcherVCResult
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
