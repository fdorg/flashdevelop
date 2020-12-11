using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ASCompletion.Context;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace ASCompletion.Model
{
    /// <summary>
    /// Deep classpath exploration & parsing
    /// </summary>
    public class PathExplorer
    {
        public delegate void ExplorationProgressHandler(string state, int value, int max);
        public delegate void ExplorationDoneHandler(string path);

        public static bool IsWorking => explorerThread != null;

        static bool uistarted;
        static bool contextUpdating;
        static readonly Queue<PathExplorer> waiting = new Queue<PathExplorer>();
        static volatile Thread explorerThread;
        static volatile bool stopExploration;
        static volatile int toWait = 1000; // initial delay before exploring the filesystem

        public static void OnUIStarted()
        {
            if (uistarted) return;
            uistarted = true;
            lock (waiting)
            {
                StartBackgroundThread();
            }
        }

        public static void StopBackgroundExploration()
        {
            // signal to stop cleanly
            stopExploration = true;

            if (explorerThread != null && explorerThread.IsAlive)
            {
                Debug.WriteLine("Signaling to stop exploration.");

                if (!explorerThread.Join(4000))
                {
                    Debug.WriteLine("Aborting exploration.");
                    explorerThread.Abort();
                }

                explorerThread = null;
                Debug.WriteLine("Explorer exited cleanly.");
            }
            lock (waiting) { waiting.Clear(); }
        }

        public static void ClearAll()
        {
            lock (waiting) { waiting.Clear(); }
        }

        public static void BeginUpdate() => contextUpdating = true;

        public static void EndUpdate() => contextUpdating = false;

        public static void ClearPersistentCache()
        {
            string cacheDir = GetCachePath();
            try
            {
                if (Directory.Exists(cacheDir))
                    Directory.Delete(cacheDir, true);
            }
            catch { }
        }

        public event ExplorationProgressHandler OnExplorationProgress;
        public event ExplorationDoneHandler OnExplorationDone;
        public bool UseCache;

        readonly IASContext context;
        readonly PathModel pathModel;
        readonly List<string> foundFiles = new List<string>();
        readonly List<string> explored = new List<string>();
        string hashName;
        readonly char hiddenPackagePrefix;

        public PathExplorer(IASContext context, PathModel pathModel)
        {
            this.context = context;
            this.pathModel = pathModel;
            hashName = pathModel.Path;
            hiddenPackagePrefix = context.Features.hiddenPackagePrefix;
        }

        public void HideDirectories(IEnumerable<string> dirs)
        {
            foreach (string dir in dirs)
            {
                if (Path.IsPathRooted(dir)) explored.Add(dir);
                else explored.Add(Path.Combine(pathModel.Path, dir));
                hashName += ";" + dir;
            }
        }

        public void Run()
        {
            lock (waiting)
            {
                if (waiting.Any(exp => exp.pathModel == pathModel)) return;
                waiting.Enqueue(this);
                StartBackgroundThread();
            }
        }

        static void StartBackgroundThread()
        {
            if (!uistarted || explorerThread != null) return;
            explorerThread = new Thread(ExploreInBackground)
            {
                Name = "ExplorerThread",
                Priority = ThreadPriority.Lowest
            };
            explorerThread.Start();
        }

        static void ExploreInBackground()
        {
            Thread.Sleep(toWait);
            toWait = 10;

            PathExplorer last = null;

            while (!stopExploration)
            {
                PathExplorer next = null;

                if (contextUpdating)
                {
                    Thread.Sleep(100);
                    continue;
                }

                lock (waiting)
                {
                    if (waiting.Count > 0) next = waiting.Dequeue();
                    else explorerThread = null;

                    // we want to call these notifications after we've processed the above
                    // logic, so that the last PathExplorer's NotifyDone gets called after
                    // explorerThread is null so that IsWorking is false.
                    if (last?.OnExplorationDone != null)
                    {
                        last.NotifyProgress(null, 0, 0);
                        last.NotifyDone(last.pathModel.Path);
                    }
                }

                if (next != null) next.BackgroundRun();
                else
                {
                    PluginBase.RunAsync(() => EventManager.DispatchEvent(last, new DataEvent(EventType.Command, "ASCompletion.PathExplorerFinished", null)));
                    break;
                }
                last = next;
            }
        }

        /// <summary>
        /// Background search
        /// </summary>
        void BackgroundRun()
        {
            pathModel.Updating = true;
            try
            {
                if (pathModel.IsVirtual)
                {
                    var ext = Path.GetExtension(pathModel.Path).ToLower();
                    if (ext == ".jar" || ext == ".zip")
                    {
                        pathModel.WasExplored = true;
                        ExtractFilesFromArchive();
                    }
                    // let the context explore packaged libraries
                    else if (pathModel.Owner != null)
                        try
                        {
                            NotifyProgress(string.Format(TextHelper.GetString("Info.Parsing"), 1), 0, 1);
                            pathModel.Owner.ExploreVirtualPath(pathModel);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, TextHelper.GetString("Info.SWCConversionException"));
                        }
                }
                else
                {
                    pathModel.WasExplored = true;
                    bool writeCache = false;
                    string cacheFileName = null;
                    if (UseCache)
                    {
                        cacheFileName = GetCacheFileName(hashName);
                        if (File.Exists(cacheFileName))
                        {
                            NotifyProgress(TextHelper.GetString("Info.ParsingCache"), 0, 1);
                            pathModel.Deserialize(cacheFileName);
                        }
                        else writeCache = true;
                        if (stopExploration || !pathModel.InUse) return;
                    }

                    // explore filesystem (populates foundFiles)
                    ExploreFolder(pathModel.Path, context.GetExplorerMask());
                    if (stopExploration || !pathModel.InUse) return;

                    // create models
                    writeCache |= ParseFoundFiles();

                    // write cache file
                    if (UseCache && writeCache && !stopExploration && pathModel.InUse)
                        try
                        {
                            var path = Path.GetDirectoryName(cacheFileName);
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                            else if (File.Exists(cacheFileName)) File.Delete(cacheFileName);
                            if (pathModel.FilesCount > 0) pathModel.Serialize(cacheFileName);
                        }
                        catch { }
                }
            }
            finally { pathModel.Updating = false; }
        }

        void ExtractFilesFromArchive()
        {
            var masks = context.GetExplorerMask();
            for (var i = 0; i < masks.Length; i++)
                masks[i] = masks[i].Substring(masks[i].IndexOf('*') + 1);

            using var stream = File.OpenRead(pathModel.Path);
            using var zip = new ZipFile(stream);
            var models = new Dictionary<string, FileModel>();
            foreach (ZipEntry entry in zip)
            {
                var ext = Path.GetExtension(entry.Name).ToLower();
                foreach (var mask in masks)
                    if (ext == mask)
                    {
                        var src = UnzipFile(zip, entry);
                        var model = context.CreateFileModel(Path.Combine(pathModel.Path, entry.Name));
                        model.Context = pathModel.Owner;
                        context.GetCodeModel(model, src);
                        models.Add(model.FileName, model);
                    }
            }
            pathModel.SetFiles(models);
        }

        static string UnzipFile(ZipFile file, ZipEntry entry)
        {
            using var stream = file.GetInputStream(entry);
            var data = new byte[entry.Size];
            var length = stream.Read(data, 0, (int)entry.Size);
            if (length != entry.Size) throw new Exception("Corrupted archive");
            using var ms = new MemoryStream(data);
            using var reader = new StreamReader(ms);
            var result = reader.ReadToEnd();
            return result;
        }

        bool ParseFoundFiles()
        {
            bool writeCache = false;

            // parse files
            int n = foundFiles.Count;
            NotifyProgress(string.Format(TextHelper.GetString("Info.Parsing"), n), 0, n);
            for (int i = 0; i < n; i++)
            {
                if (stopExploration) return writeCache;
                // parse
                var filename = foundFiles[i];
                if (!File.Exists(filename)) continue;
                if (pathModel.TryGetFile(filename, out var cachedModel))
                {
                    if (cachedModel.OutOfDate)
                    {
                        cachedModel.Check();
                        writeCache = true;
                    }
                    continue;
                }

                writeCache = true;

                var aFile = GetFileModel(filename);

                if (aFile is null || pathModel.HasFile(filename)) continue;
                // store model
                if (aFile.Context.IsModelValid(aFile, pathModel))
                    pathModel.AddFile(aFile);
                // update status
                if (stopExploration) return writeCache;
                if (i % 10 == 0) NotifyProgress(string.Format(TextHelper.GetString("Info.Parsing"), n), i, n);
                Thread.Sleep(1);
            }
            return writeCache;
        }

        string GetCacheFileName(string path)
        {
            var cacheDir = GetCachePath();
            var hashFileName = HashCalculator.CalculateSHA1(path);
            return Path.Combine(cacheDir, hashFileName + "." + context.Settings.LanguageId.ToLower() + ".bin");
        }

        static string GetCachePath() => Path.Combine(PathHelper.DataDir, nameof(ASCompletion), "FileCache");

        void NotifyProgress(string state, int value, int max) => OnExplorationProgress?.Invoke(state, value, max);

        void NotifyDone(string path) => OnExplorationDone?.Invoke(path);

        FileModel? GetFileModel(string filename)
        {
            // Going to try just doing this operation on our background thread - if there
            // are any strange exceptions, this should be synchronized
            return context?.GetFileModel(filename);
        }

        void ExploreFolder(string path, string[] masks)
        {
            if (stopExploration || !Directory.Exists(path)) return;
            explored.Add(path);
            Thread.Sleep(5);

            // The following try/catch is used to handle "There are no more files" IOException.
            // For some undocumented reason, on a networks share, and when using a mask, 
            //  Directory.GetFiles() can throw an IOException instead of returning an empty array.
            try
            {
                foreach (var mask in masks)
                {
                    var files = Directory.GetFiles(path, mask);
                    foundFiles.AddRange(files);
                    Thread.Sleep(5);
                }
            }
            catch { }

            try
            {
                // explore subfolders
                var dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    if (!explored.Contains(dir) 
                        && (File.GetAttributes(dir) & FileAttributes.Hidden) == 0
                        && !IgnoreDirectory(dir))
                        ExploreFolder(dir, masks);
                }
            }
            catch { }
        }

        bool IgnoreDirectory(string dir)
        {
            return Path.GetFileName(dir) is {} name 
                && (name[0] == '.' || (hiddenPackagePrefix != 0 && name[0] == hiddenPackagePrefix));
        }
    }
}
