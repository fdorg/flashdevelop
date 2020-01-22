using System;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore.Managers;

namespace PluginCore.Bridge
{
    public class WatcherEx
    {
        string path;
        readonly string filter;
        bool enabled;
        FileSystemWatcher watcher;
        BridgeClient bridge;

        public bool IsRemote { get; }

        /// <summary>
        /// Either watch a single file (if specified) or an entire directory tree.
        /// </summary>
        public WatcherEx(string path)
            : this(path, null)
        {
        }

        public WatcherEx(string path, string file)
        {
            this.path = path;
            this.filter = file;
            IsRemote = BridgeManager.Active && path.ToUpper().StartsWithOrdinal(BridgeManager.Settings.SharedDrive);
            if (!IsRemote) SetupRegularWatcher();
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }
        }

        #region Tracing

        static bool errorDone;
        public void TraceError()
        {
            if (errorDone) return;
            errorDone = true;
            TraceManager.AddAsync("Unable to connect to FlashDevelop Bridge.");
        }

        static bool okDone;
        public void TraceOk()
        {
            if (okDone) return;
            okDone = true;
            TraceManager.AddAsync("Connected successfully to FlashDevelop Bridge.");
        }

        #endregion

        #region FSW emulation

        public event FileSystemEventHandler Created;
        public event FileSystemEventHandler Changed;
        public event FileSystemEventHandler Deleted;
        public event RenamedEventHandler Renamed;

        public bool EnableRaisingEvents
        {
            get => enabled;
            set
            {
                enabled = value;
                if (watcher != null) watcher.EnableRaisingEvents = value;
                else if (enabled)
                {
                    bridge = new BridgeClient();
                    if (!bridge.Connected)
                    {
                        enabled = false;
                        bridge = null;
                        TraceError();
                    }
                    else
                    {
                        if (Directory.Exists(path) && !path.EndsWith('\\')) path += "\\";
                        bridge.DataReceived += bridge_DataReceived;
                        if (filter is null) bridge.Send("watch:" + path);
                        else bridge.Send("watch:" + Path.Combine(path, filter));
                        TraceOk();
                    }
                }
                else if (bridge != null)
                {
                    if (bridge.Connected)
                    {
                        //bridge.Send("unwatch:");
                        try
                        {
                            bridge.Disconnect();
                        }
                        catch { }
                    }
                    bridge = null;
                }
            }
        }

        void bridge_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string fullPath = e.Text;
            if (fullPath.StartsWithOrdinal("BRIDGE:"))
            {
                // Lets expose bridge location...
                Environment.SetEnvironmentVariable("FDBRIDGE", fullPath.Replace("BRIDGE:", ""));
                return;
            }
            if (!fullPath.EndsWith('\\')) fullPath += '\\';
            if (fullPath.Length < 3) return;
            string folder = Path.GetDirectoryName(fullPath);
            string name = Path.GetFileName(fullPath);
            Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, folder, name));
        }

        #endregion

        #region regular watcher implementation

        static readonly Regex reIgnore = new Regex("[\\\\/][._]svn", RegexOptions.Compiled | RegexOptions.RightToLeft);

        void SetupRegularWatcher()
        {
            watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            if (filter != null)
            {
                watcher.IncludeSubdirectories = false;
                watcher.Filter = filter;
            }
            else
            {
                watcher.IncludeSubdirectories = true;
                watcher.InternalBufferSize = 4096 * 8;
            }
            watcher.Created += watcher_Created;
            watcher.Changed += watcher_Changed;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (reIgnore.IsMatch(e.FullPath)) return;
            Created?.Invoke(this, e);
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (reIgnore.IsMatch(e.FullPath)) return;
            Changed?.Invoke(this, e);
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (reIgnore.IsMatch(e.FullPath)) return;
            Deleted?.Invoke(this, e);
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (reIgnore.IsMatch(e.FullPath)) return;
            Renamed?.Invoke(this, e);
        }

        #endregion

    }

}
