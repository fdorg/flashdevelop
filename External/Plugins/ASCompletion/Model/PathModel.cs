using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Managers;
using Timer = System.Timers.Timer;

namespace ASCompletion.Model
{
    public delegate bool ForeachDelegate(FileModel model);

    /// <summary>
    /// Files cache
    /// </summary>
    public class PathModel
    {
        internal static event Action<FileModel> OnFileRemove;
        internal static event Action<FileModel> OnFileAdded;

        //static private readonly bool cacheEnabled = false;
        static Dictionary<string, PathModel> pathes = new Dictionary<string, PathModel>();

        /// <summary>
        /// Delete all models and remove all watchers
        /// </summary>
        public static void ClearAll()
        {
            foreach (PathModel model in pathes.Values)
            {
                model.ReleaseWatcher();
            }
            pathes.Clear();
        }

        /// <summary>
        /// Free models & system watchers
        /// </summary>
        public static void Compact()
        {
            lock (pathes)
            {
                var clean = new Dictionary<string, PathModel>();
                foreach (var key in pathes.Keys)
                {
                    var model = pathes[key];
                    if (model.InUse) clean.Add(key, model);
                    else model.Cleanup();
                }
                pathes = clean;
            }
        }

        /// <summary>
        /// Retrieve a PathModel from the cache or create a new one
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context">Associated language context</param>
        /// <returns></returns>
        public static PathModel GetModel(string path, IASContext context)
        {
            if (context?.Settings is null) return null;

            PathModel result;
            var modelName = context.Settings.LanguageId + "|" + path.ToUpper();
            if (pathes.ContainsKey(modelName))
            {
                result = pathes[modelName];
                if (result.IsTemporaryPath || !result.IsValid || result.FilesCount == 0)
                {
                    pathes[modelName] = result = new PathModel(path, context);
                }
            }
            else pathes[modelName] = result = new PathModel(path, context);
            return result;
        }

        public volatile bool Updating;
        public bool WasExplored;
        public bool IsTemporaryPath;
        public string Path;
        public IASContext Owner;
        public bool IsValid;
        public bool IsVirtual;
        public bool ValidatePackage;
        readonly object lockObject = new object();
        bool inited;
        bool inUse;
        WatcherEx watcher;
        Timer updater;
        string[] masks;
        string basePath;
        Dictionary<string, FileModel> files;
        List<string> toExplore;
        List<string> toRemove;

        public bool InUse
        {
            get => inUse;
            set
            {
                if (!inited) Init();
                inUse = value;
            }
        }

        public int FilesCount
        {
            get
            {
                lock (lockObject) { return files.Count; }
            }
        }

        public PathModel(string path, IASContext context)
        {
            Owner = context;
            Path = path.TrimEnd('\\', '/');

            files = new Dictionary<string, FileModel>();

            if (Owner is null) return;
            if (Directory.Exists(Path)) IsValid = Path.Length > 3 /*no root drive*/;
            else if (System.IO.Path.GetExtension(path).Length > 1) 
            { 
                IsValid = File.Exists(Path); 
                IsVirtual = true; 
            }
        }

        #region init_timers

        void Init()
        {
            if (inited && IsValid) return;
            inited = true;
            updater = new Timer();
            updater.Interval = 500;
            updater.SynchronizingObject = PluginBase.MainForm as Form;
            updater.Elapsed += updater_Tick;
            toExplore = new List<string>();
            toRemove = new List<string>();
            
            // generic models container
            if (IsVirtual)
            {
                try
                {
                    basePath = System.IO.Path.GetDirectoryName(Path);
                    masks = new[] { System.IO.Path.GetFileName(Path) };
                    watcher = new WatcherEx(System.IO.Path.GetDirectoryName(Path), System.IO.Path.GetFileName(Path));
                    watcher.Deleted += watcher_Deleted;
                    watcher.Changed += watcher_Changed;
                    watcher.Renamed += watcher_Renamed;
                    watcher.EnableRaisingEvents = true;
                }
                catch
                {
                    watcher = null;
                    IsValid = false;
                }
            }
            // watched path
            else if (IsValid)
            {
                if (Owner is null) return;
                try
                {
                    basePath = Path;
                    masks = Owner.GetExplorerMask();
                    watcher = new WatcherEx(Path); //System.IO.Path.GetDirectoryName(Path));
                    if (IsTemporaryPath && watcher.IsRemote) return;
                    watcher.Deleted += watcher_Deleted;
                    watcher.Changed += watcher_Changed;
                    watcher.Renamed += watcher_Renamed;
                    watcher.EnableRaisingEvents = true;
                }
                catch
                {
                    watcher = null;
                    IsValid = false;
                }
            }
        }

        /// <summary>
        /// Enable the timer to update the outdated models
        /// </summary>
        void SetTimer()
        {
            updater.Enabled = false;
            updater.Enabled = true;
        }

        void updater_Tick(object sender, EventArgs e)
        {
            if (Updating) return;
            updater.Stop();

            if (IsVirtual)
            {
                WasExplored = false;
                Owner.ExploreVirtualPath(this);
            }
            else
            {
                lock (lockObject)
                {
                    DoScheduledOperations();

                    foreach (FileModel file in files.Values)
                    {
                        file?.Check();
                    }
                }
            }

            Owner?.RefreshContextCache(Path);
        }

        #endregion

        #region Watcher events

        bool MaskMatch(string fileName)
        {
            foreach (string mask in masks)
            {
                if (mask[0] == '*')
                {
                    if (fileName.EndsWithOrdinal(mask.Substring(1))) return true;
                }
                else if (fileName.EndsWithOrdinal(mask)) return true;
            }
            return false;
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // possibly renamed the watched folder
            if (!e.FullPath.StartsWithOrdinal(basePath) && e.FullPath != Path)
                return;
            // folder renamed: flag directory to be removed from models
            if (!MaskMatch(e.FullPath))
            {
                if (Directory.Exists(e.FullPath))
                {
                    lock (lockObject)
                    {
                        string path = e.OldFullPath;
                        // add to known removed paths
                        var newSchedule = new List<string>();
                        foreach (string scheduled in toRemove)
                            if (path.StartsWithOrdinal(scheduled)) return;
                            else if (!scheduled.StartsWithOrdinal(path)) newSchedule.Add(scheduled);
                        newSchedule.Add(path);
                        toRemove = newSchedule;
                    }
                }
                // explore will be added in parent directory's watcher_Changed event
                return;
            }
            // file renamed: remove old and schedule for parsing
            if (IsVirtual)
            {
                SetTimer();
            }
            else
            {
                if (files.ContainsKey(e.OldFullPath.ToUpper()))
                {
                    RemoveFile(e.OldFullPath);
                }
                ParseNewFile(e.FullPath);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWithOrdinal(basePath) && e.FullPath != Path)
                return;
            // directory change: schedule for exploration
            if (!MaskMatch(e.FullPath))
            {
                lock (lockObject)
                {
                    string path = e.FullPath;
                    // add path for exploration if not already scheduled
                    var newSchedule = new List<string>();
                    foreach (string scheduled in toExplore)
                        if (path.StartsWithOrdinal(scheduled)) return;
                        else if (!scheduled.StartsWithOrdinal(path)) newSchedule.Add(scheduled);
                    newSchedule.Add(path);
                    toExplore = newSchedule;
                }
                SetTimer();
                return;
            }
            // file change: schedule for update
            if (IsVirtual)
            {
                SetTimer();
            }
            else
            {
                lock (lockObject)
                {
                    if (files.ContainsKey(e.FullPath.ToUpper()))
                    {
                        files[e.FullPath.ToUpper()].OutOfDate = true;
                        SetTimer();
                    }
                    else ParseNewFile(e.FullPath);
                }
            }
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWithOrdinal(basePath) && e.FullPath != Path)
                return;
            // (possibly) folder deleted
            if (!MaskMatch(e.FullPath))
            {
                lock (lockObject)
                {
                    string path = e.FullPath;
                    // add to known removed paths
                    var newSchedule = new List<string>();
                    foreach (string scheduled in toRemove)
                        if (path.StartsWithOrdinal(scheduled)) return;
                        else if (!scheduled.StartsWithOrdinal(path)) newSchedule.Add(scheduled);
                    newSchedule.Add(path);
                    toRemove = newSchedule;
                }
                return;
            }
            // file deleted
            if (IsVirtual)
            {
                SetTimer();
            }
            else if (files.ContainsKey(e.FullPath.ToUpper()))
            {
                RemoveFile(e.FullPath);
                SetTimer();
            }
        }

        void ParseNewFile(string fileName)
        {
            if (Owner is null || Owner.Settings.LazyClasspathExploration || !File.Exists(fileName)) return;
            var newModel = Owner.CreateFileModel(fileName);
            newModel.OutOfDate = true;
            files[fileName.ToUpper()] = newModel;
            SetTimer();
        }

        void DoScheduledOperations()
        {
            if (toExplore.Count == 0) return;
            var _toExplore = toExplore.ToArray();
            toExplore.Clear();

            var _toRemove = new List<string>(toRemove.Count);
            for (int i = 0; i < _toRemove.Count; i++)
                _toRemove[i] = toRemove[i].ToUpper() + System.IO.Path.DirectorySeparatorChar;
            toRemove.Clear();

            var newFiles = new Dictionary<string, FileModel>();
            // cleanup files
            foreach (string file in files.Keys)
            {
                var drop = _toRemove.Any(remPath => file.StartsWithOrdinal(remPath));
                if (drop) continue;

                FileModel model = files[file];
                if (!File.Exists(model.FileName))
                {
                    var directoryName = System.IO.Path.GetDirectoryName(model.FileName);
                    if (!Directory.Exists(directoryName))
                    {
                        var newRemPath = directoryName.ToUpper() + System.IO.Path.DirectorySeparatorChar;
                        _toRemove.Add(newRemPath);
                    }
                    //TraceManager.Add("drop2: " + files[file].FileName);
                    drop = true;
                }
                if (drop) continue;
                newFiles[file] = model;
            }
            files = newFiles;
            
            // add new files
            foreach (string path in _toExplore)
            {
                AddNewFilesIn(path);
            }
        }

        void AddNewFilesIn(string path)
        {
            if (!Directory.Exists(path) || (File.GetAttributes(path) & FileAttributes.Hidden) != 0) return;
            var explored = new List<string>();
            var foundFiles = new List<string>();
            ExploreFolder(path, masks, explored, foundFiles);
            foreach (string fileName in foundFiles)
                if (!files.ContainsKey(fileName.ToUpper()))
                {
                    //TraceManager.Add("add: " + fileName);
                    var newModel = new FileModel(fileName) {Context = Owner, OutOfDate = true};
                    if (Owner.IsModelValid(newModel, this))
                        files[fileName.ToUpper()] = newModel;
                }
        }

        void ExploreFolder(string path, string[] masks, ICollection<string> explored, List<string> foundFiles)
        {
            if (!Directory.Exists(path)) return;
            explored.Add(path);

            try
            {
                // convert classes
                foreach (var mask in masks)
                {
                    foundFiles.AddRange(Directory.GetFiles(path, mask));
                }

                // explore subfolders
                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    if (!explored.Contains(dir) && (File.GetAttributes(dir) & FileAttributes.Hidden) == 0)
                        ExploreFolder(dir, masks, explored, foundFiles);
                }
            }
            catch { }
        }
        #endregion

        public void ReleaseWatcher()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
                //TraceManager.Add("Release: " + Path);
            }
        }

        /// <summary>
        /// Temporarily disables the watcher to release the lock on directories / files
        /// </summary>
        public void DisableWatcher()
        {
            if (watcher != null)
                watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Should be called after DisableWatcher()
        /// </summary>
        public void EnableWatcher()
        {
            if (watcher != null)
                watcher.EnableRaisingEvents = true;
        }

        public bool HasFile(string fileName)
        {
            if (!IsValid) return false;
            lock (lockObject) 
            {
                return files.ContainsKey(fileName.ToUpper());
            }
        }

        public bool TryGetFile(string fileName, out FileModel value)
        {
            if (!IsValid)
            {
                value = null;
                return false;
            }

            lock (lockObject)
            {
                return files.TryGetValue(fileName.ToUpper(), out value);
            }
        }

        public FileModel GetFile(string fileName)
        {
            if (!IsValid) return new FileModel(fileName) {Context = Owner, OutOfDate = true};
            lock (lockObject)
            {
                return files[fileName.ToUpper()];
            }
        }

        public void AddFile(FileModel aFile)
        {
            if (!IsValid) return;
            lock (lockObject)
            {
                files[aFile.FileName.ToUpper()] = aFile;
                OnFileAdded?.Invoke(aFile);
            }
        }

        public void SetFiles(Dictionary<string, FileModel> newFiles)
        {
            if (!IsValid) return;
            lock (lockObject)
            {
                files.Clear();
                foreach (FileModel model in newFiles.Values)
                    files[model.FileName.ToUpper()] = model;
            }
        }

        public void ForeachFile(ForeachDelegate callback)
        {
            lock (lockObject)
            {
                foreach (FileModel model in files.Values)
                    if (!callback(model)) break;
            }
        }

        public void RemoveFile(string fileName)
        {
            if (!IsValid) return;
            lock (lockObject)
            {
                var fn = fileName.ToUpper();
                OnFileRemove?.Invoke(files[fn]);
                files.Remove(fn);
            }
        }

        public void Cleanup()
        {
            lock (lockObject)
            {
                files.Clear();
            }
            ReleaseWatcher();
        }

        public void Serialize(string path)
        {
            lock (lockObject)
            {
                try
                {
                    using Stream stream = File.Open(path, FileMode.Create);
                    var bin = new BinaryFormatter();
                    bin.Serialize(stream, files);
                }
                catch (Exception)
                {
                    TraceManager.AddAsync($"Failed to serialize: {path}");
                }
            }
        }

        public bool Deserialize(string path)
        {
            try
            {
                using Stream stream = File.Open(path, FileMode.Open);
                var bin = new BinaryFormatter();
                var newFiles = (Dictionary<string, FileModel>)bin.Deserialize(stream);
                lock (lockObject)
                {
                    foreach (var key in newFiles.Keys)
                    {
                        var aFile = newFiles[key];
                        if (!File.Exists(aFile.FileName)) continue;
                        if (File.GetLastWriteTime(aFile.FileName) != aFile.LastWriteTime) aFile.OutOfDate = true;
                        aFile.Context = Owner;
                        files[key] = aFile;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                TraceManager.AddAsync($"Failed to deserialize: {path}");
                return false;
            }
        }
    }
}