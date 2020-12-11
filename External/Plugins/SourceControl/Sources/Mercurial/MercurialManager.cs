using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SourceControl.Sources.Mercurial
{
    internal class MercurialManager : IVCManager
    {
        public event VCManagerStatusChange OnChange;

        readonly Dictionary<string, Status> statusCache = new Dictionary<string, Status>();
        readonly Regex reIgnore = new Regex("([/\\\\]\\.hg[/\\\\]|hg-checkexec)");
        bool ignoreDirty = false;

        public IVCMenuItems MenuItems { get; } = new MenuItems();

        public IVCFileActions FileActions { get; } = new FileActions();

        public MercurialManager()
        {
        }

        public bool IsPathUnderVC(string path) => Directory.Exists(Path.Combine(path, ".hg"));

        public VCItemStatus GetOverlay(string path, string rootPath)
        {
            var snode = FindNode(path, rootPath);
            return snode?.Status ?? VCItemStatus.Unknown;
        }

        StatusNode? FindNode(string path, string rootPath)
        {
            if (statusCache.ContainsKey(rootPath))
            {
                var status = statusCache[rootPath];
                var len = path.Length;
                var rlen = rootPath.Length + 1;
                if (len < rlen) path = ".";
                else path = path.Substring(rlen);

                return status.Get(path);
            }
            return null;
        }

        public List<VCStatusReport>? GetAllOverlays(string path, string rootPath)
        {
            var root = FindNode(path, rootPath);
            if (root is null) return null;

            var children = new List<StatusNode>();
            GetChildren(root, children);
            var result = new List<VCStatusReport>();
            foreach (var child in children)
                result.Add(new VCStatusReport(GetNodePath(child, rootPath), child.Status));
            return result;
        }

        string GetNodePath(StatusNode child, string rootPath)
        {
            var S = Path.DirectorySeparatorChar;
            var path = "";
            while (child != null && child.Name != ".")
            {
                path = S + child.Name + path;
                child = child.Parent;
            }
            return rootPath + S + path;
        }

        void GetChildren(StatusNode node, List<StatusNode> result)
        {
            if (node.Children is null) return;
            foreach (var child in node.Children.Values)
            {
                result.Add(child);
                GetChildren(child, result);
            }
        }

        public void GetStatus(string rootPath)
        {
            Status status;
            if (!statusCache.ContainsKey(rootPath))
            {
                status = new Status(rootPath);
                status.OnResult += Status_OnResult;
                status.Run();
                statusCache[rootPath] = status;
            }
            else status = statusCache[rootPath];
            ignoreDirty = true;
            status.Update();
        }

        void Status_OnResult(Status status)
        {
            ignoreDirty = false;
            OnChange?.Invoke(this);
        }

        public bool SetPathDirty(string path, string rootPath)
        {
            if (ignoreDirty) return false;
            if (statusCache.ContainsKey(rootPath))
            {
                if (reIgnore.IsMatch(path)) return false;
                return statusCache[rootPath].SetPathDirty(path);
            }
            return false;
        }

        public VCCommand Commit(string[] files, string message) => new CommitCommand(files, message, Path.GetDirectoryName(files[0]));

        public VCCommand Unstage(string file) => new UnstageCommand(file);
    }
}
