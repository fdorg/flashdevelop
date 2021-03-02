using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SourceControl.Sources.Subversion
{
    internal class SubversionManager : IVCManager
    {
        public event VCManagerStatusChange OnChange;

        readonly Dictionary<string, Status> statusCache = new Dictionary<string, Status>();
        readonly IVCMenuItems menuItems = new MenuItems();
        readonly IVCFileActions fileActions = new FileActions();
        readonly Regex reIgnore = new Regex("[/\\\\][._]svn[/\\\\]");
        bool ignoreDirty = false;

        public IVCMenuItems MenuItems => menuItems;
        public IVCFileActions FileActions => fileActions;

        public SubversionManager()
        {
        }

        public bool IsPathUnderVC(string path)
        {
            if (Directory.Exists(Path.Combine(path, ".svn"))) return true;
            if (Directory.Exists(Path.Combine(path, "_svn"))) return true;
            return false;
        }

        public VCItemStatus GetOverlay(string path, string rootPath)
        {
            var snode = FindNode(path, rootPath);
            if (snode != null) return snode.Status;
            return VCItemStatus.Unknown;
        }

        StatusNode? FindNode(string path, string rootPath)
        {
            if (!statusCache.TryGetValue(rootPath, out var status)) return null;
            var len = path.Length;
            var rlen = rootPath.Length + 1;
            if (len < rlen) path = ".";
            else path = path.Substring(rlen);
            return status.Get(path);
        }

        public List<VCStatusReport>? GetAllOverlays(string path, string rootPath)
        {
            var root = FindNode(path, rootPath);
            if (root is null) return null;
            var children = new List<StatusNode>();
            GetChildren(root, children);
            return children
                .Select(it => new VCStatusReport(GetNodePath(it, rootPath), it.Status))
                .ToList();
        }

        static string GetNodePath(StatusNode child, string rootPath)
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

        static void GetChildren(StatusNode node, ICollection<StatusNode> result)
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
            if (statusCache.TryGetValue(rootPath, out var status))
            {
                if (reIgnore.IsMatch(path)) return false;
                return status.SetPathDirty(path);
            }
            return false;
        }

        public VCCommand Commit(string[] files, string message) => new CommitCommand(files, message, Path.GetDirectoryName(files[0]));

        public VCCommand Unstage(string file) => null; //Not sure what to do here, ignore for now
    }
}
