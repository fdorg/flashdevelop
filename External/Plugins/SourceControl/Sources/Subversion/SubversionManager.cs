using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SourceControl.Sources.Subversion
{
    class SubversionManager : IVCManager
    {
        public event VCManagerStatusChange OnChange;

        Dictionary<string, Status> statusCache = new Dictionary<string, Status>();
        IVCMenuItems menuItems = new MenuItems();
        IVCFileActions fileActions = new FileActions();
        Regex reIgnore = new Regex("[/\\\\][._]svn[/\\\\]");
        bool ignoreDirty = false;

        public IVCMenuItems MenuItems { get { return menuItems; } }
        public IVCFileActions FileActions { get { return fileActions; } }

        public SubversionManager()
        {
        }

        public bool IsPathUnderVC(string path)
        {
            if (Directory.Exists(Path.Combine(path, ".svn"))) return true;
            else if (Directory.Exists(Path.Combine(path, "_svn"))) return true;
            else return false;
        }

        public VCItemStatus GetOverlay(string path, string rootPath)
        {
            StatusNode snode = FindNode(path, rootPath);
            if (snode != null) return snode.Status;
            else return VCItemStatus.Unknown;
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
            if (root == null) return null;

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
            if (node.Children == null) return;
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
                status.OnResult += new StatusResult(Status_OnResult);
                statusCache[rootPath] = status;
            }
            else status = statusCache[rootPath];
            ignoreDirty = true;
            status.Update();
        }

        void Status_OnResult(Status status)
        {
            ignoreDirty = false;
            if (OnChange != null) OnChange(this);
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
    }
}
