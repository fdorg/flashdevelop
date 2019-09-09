// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SourceControl.Sources.Subversion
{
    class SubversionManager : IVCManager
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
            StatusNode snode = FindNode(path, rootPath);
            if (snode != null) return snode.Status;
            return VCItemStatus.Unknown;
        }

        private StatusNode FindNode(string path, string rootPath)
        {
            if (statusCache.ContainsKey(rootPath))
            {
                Status status = statusCache[rootPath];
                int len = path.Length;
                int rlen = rootPath.Length + 1;
                if (len < rlen) path = ".";
                else path = path.Substring(rlen);

                return status.Get(path);
            }
            return null;
        }

        public List<VCStatusReport> GetAllOverlays(string path, string rootPath)
        {
            StatusNode root = FindNode(path, rootPath);
            if (root is null) return null;

            List<StatusNode> children = new List<StatusNode>();
            GetChildren(root, children);
            List<VCStatusReport> result = new List<VCStatusReport>();
            foreach (StatusNode child in children)
                result.Add(new VCStatusReport(GetNodePath(child, rootPath), child.Status));
            return result;
        }

        private string GetNodePath(StatusNode child, string rootPath)
        {
            char S = Path.DirectorySeparatorChar;
            string path = "";
            while (child != null && child.Name != ".")
            {
                path = S + child.Name + path;
                child = child.Parent;
            }
            return rootPath + S + path;
        }

        private void GetChildren(StatusNode node, List<StatusNode> result)
        {
            if (node.Children is null) return;
            foreach (StatusNode child in node.Children.Values)
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
            if (statusCache.ContainsKey(rootPath))
            {
                if (reIgnore.IsMatch(path)) return false;
                return statusCache[rootPath].SetPathDirty(path);
            }
            return false;
        }

        public void Commit(string[] files, string message)
        {
            new Git.CommitCommand(files, message, Path.GetDirectoryName(files[0]));
        }
    }
}
