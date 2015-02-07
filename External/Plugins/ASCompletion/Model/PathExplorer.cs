using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PluginCore;
using ASCompletion.Context;
using System.Windows.Forms;
using System.Diagnostics;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Helpers;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace ASCompletion.Model
{
	/// <summary>
	/// Deep classpath exploration & parsing
	/// </summary>
	public class PathExplorer
	{
        public delegate void ExplorationProgressHandler(string state, int value, int max);
		public delegate void ExplorationDoneHandler(string path);

        static public bool IsWorking
        {
            get { return explorerThread != null; }
        }

        static private bool uistarted;
        static private Queue<PathExplorer> waiting = new Queue<PathExplorer>();
        static private volatile Thread explorerThread;
        static private volatile bool stopExploration;
        static private volatile int toWait = 1000; // initial delay before exploring the filesystem

        static public void OnUIStarted()
        {
            if (!uistarted)
            {
                uistarted = true;
                lock (waiting)
                {
                    StartBackgroundThread();
                }
            }
        }

        static public void StopBackgroundExploration()
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

        static public void ClearAll()
        {
            lock (waiting) { waiting.Clear(); }
        }

        public event ExplorationProgressHandler OnExplorationProgress;
		public event ExplorationDoneHandler OnExplorationDone;
        public bool UseCache;

        private IASContext context;
        private PathModel pathModel;
		private List<string> foundFiles;
        private List<string> explored;
        private string hashName;
        private char hiddenPackagePrefix;

        public PathExplorer(IASContext context, PathModel pathModel)
        {
            this.context = context;
            this.pathModel = pathModel;
            hashName = pathModel.Path;
            hiddenPackagePrefix = context.Features.hiddenPackagePrefix;
            foundFiles = new List<string>();
            explored = new List<string>();
        }

        public void HideDirectories(IEnumerable<String> dirs)
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
                foreach (PathExplorer exp in waiting)
                    if (exp.pathModel == pathModel) return;
                waiting.Enqueue(this);

                StartBackgroundThread();
            }
        }

        private static void StartBackgroundThread()
        {
            if (uistarted && explorerThread == null)
            {
                explorerThread = new Thread(ExploreInBackground);
                explorerThread.Name = "ExplorerThread";
                explorerThread.Priority = ThreadPriority.Lowest;
                explorerThread.Start();
            }
		}

        private static void ExploreInBackground()
        {
            Thread.Sleep(toWait);
            toWait = 10;

            PathExplorer last = null;

            while (!stopExploration)
            {
                PathExplorer next = null;

                lock (waiting)
                {
                    if (waiting.Count > 0) next = waiting.Dequeue();
                    else explorerThread = null;

                    // we want to call these notifications after we've processed the above
                    // logic, so that the last PathExplorer's NotifyDone gets called after
                    // explorerThread is null so that IsWorking is false.
                    if (last != null && last.OnExplorationDone != null)
                    {
                        last.NotifyProgress(null, 0, 0);
                        last.NotifyDone(last.pathModel.Path);
                    }
                }

                if (next != null)
                    next.BackgroundRun();
                else
                    break;

                last = next;
            }
        }

        /// <summary>
        /// Background search
        /// </summary>
        private void BackgroundRun()
		{
            pathModel.Updating = true;
            try
            {
                if (pathModel.IsVirtual)
                {
                    string ext = Path.GetExtension(pathModel.Path).ToLower();
                    if (ext == ".jar" || ext == ".zip")
                    {
                        pathModel.WasExplored = true;
                        ExtractFilesFromArchive();
                    }

                    // let the context explore packaged libraries
                    else if (pathModel.Owner != null)
                        try
                        {
                            NotifyProgress(String.Format(TextHelper.GetString("Info.Parsing"), 1), 0, 1);
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
                        string cacheDir = Path.GetDirectoryName(cacheFileName);
                        if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
                        else if (File.Exists(cacheFileName)) File.Delete(cacheFileName);

                        if (pathModel.FilesCount > 0)
                            pathModel.Serialize(cacheFileName);
                    }
                    catch { }
                }
            }
            finally { pathModel.Updating = false; }
		}

        private void ExtractFilesFromArchive()
        {
            string[] masks = context.GetExplorerMask();
            for (int i = 0; i < masks.Length; i++)
                masks[i] = masks[i].Substring(masks[i].IndexOf('*') + 1);

            Stream fileStream = File.OpenRead(pathModel.Path);
            ZipFile zipFile = new ZipFile(fileStream);
            ASFileParser parser = new ASFileParser();
            Dictionary<string, FileModel> models = new Dictionary<string, FileModel>();

            foreach (ZipEntry entry in zipFile)
            {
                string ext = Path.GetExtension(entry.Name).ToLower();
                foreach (string mask in masks)
                    if (ext == mask)
                    {
                        string src = UnzipFile(zipFile, entry);
                        FileModel model = new FileModel(Path.Combine(pathModel.Path, entry.Name));
                        model.Context = pathModel.Owner;
                        parser.ParseSrc(model, src);
                        models.Add(model.FileName, model);
                    }
            }
            zipFile.Close();
            fileStream.Close();

            pathModel.SetFiles(models);
        }

        private static string UnzipFile(ZipFile zfile, ZipEntry entry)
        {
            Stream stream = zfile.GetInputStream(entry);
            byte[] data = new byte[entry.Size];
            int length = stream.Read(data, 0, (int)entry.Size);
            if (length != entry.Size)
                throw new Exception("Corrupted archive");

            MemoryStream ms = new MemoryStream(data);
            return new StreamReader(ms).ReadToEnd();
        }

        private bool ParseFoundFiles()
        {
            bool writeCache = false;

            // parse files
            int n = foundFiles.Count;
            NotifyProgress(String.Format(TextHelper.GetString("Info.Parsing"), n), 0, n);
            FileModel aFile = null;
            int cpt = 0;
            string filename;
            for (int i = 0; i < n; i++)
            {
                if (stopExploration) return writeCache;
                // parse
                filename = foundFiles[i] as string;
                if (!File.Exists(filename))
                    continue;
                if (pathModel.HasFile(filename))
                {
                    FileModel cachedModel = pathModel.GetFile(filename);
                    if (cachedModel.OutOfDate)
                    {
                        cachedModel.Check();
                        writeCache = true;
                    }
                    continue;
                }
                else writeCache = true;

                aFile = GetFileModel(filename);

                if (aFile == null || pathModel.HasFile(filename)) continue;
                // store model
                if (aFile.Context.IsModelValid(aFile, pathModel))
                    pathModel.AddFile(aFile);
                aFile = null;
                cpt++;
                // update status
                if (stopExploration) return writeCache;
                if (i % 10 == 0) NotifyProgress(String.Format(TextHelper.GetString("Info.Parsing"), n), i, n);
                Thread.Sleep(1);
            }
            return writeCache;
        }

        private string GetCacheFileName(string path)
        {
            string pluginDir = Path.Combine(PathHelper.DataDir, "ASCompletion");
            string cacheDir = Path.Combine(pluginDir, "FileCache");
            string hashFileName = HashCalculator.CalculateSHA1(path);
            return Path.Combine(cacheDir, hashFileName + "." + context.Settings.LanguageId.ToLower() + ".bin");
        }

        private void NotifyProgress(string state, int value, int max)
        {
            ExplorationProgressHandler handler = OnExplorationProgress;
            if (handler != null)
                handler(state, value, max);
        }

        private void NotifyDone(string path)
        {
            ExplorationDoneHandler handler = OnExplorationDone;
            if (handler != null)
                handler(path);
        }

        private FileModel GetFileModel(string filename)
        {
            // Going to try just doing this operation on our background thread - if there
            // are any strange exceptions, this should be synchronized
            IASContext ctx = context;
            return (ctx != null) ? ctx.GetFileModel(filename) : null;
        }

		private void ExploreFolder(string path, string[] masks)
		{
            if (stopExploration || !Directory.Exists(path)) return;
			explored.Add(path);
            Thread.Sleep(5);

            // The following try/catch is used to handle "There are no more files" IOException.
            // For some undocumented reason, on a networks share, and when using a mask, 
            //  Directory.GetFiles() can throw an IOException instead of returning an empty array.
            try
            {
                foreach (string mask in masks)
                {
                    string[] files = Directory.GetFiles(path, mask);
                    if (files != null)
                        foreach (string file in files) foundFiles.Add(file);
                    Thread.Sleep(5);
                }
            }
            catch { }

            try
            {
                // explore subfolders
                string[] dirs = Directory.GetDirectories(path);
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

        private bool IgnoreDirectory(string dir)
        {
            var name = Path.GetFileName(dir);
            if (name[0] == '.') return true;
            if (hiddenPackagePrefix != 0 && name[0] == hiddenPackagePrefix) return true;
            return false;
        }
	}
}
