// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SourceControl.Sources.Git
{
    class GitManager : IVCManager
    {
        public event VCManagerStatusChange OnChange;

        readonly Dictionary<string, Status> statusCache = new Dictionary<string, Status>();
        readonly Regex reIgnore = new Regex("[/\\\\]\\.git([/\\\\]|$)");
        bool ignoreDirty;

        public IVCMenuItems MenuItems { get; } = new MenuItems();

        public IVCFileActions FileActions { get; } = new FileActions();

        public bool IsPathUnderVC(string path)
        {
            return Directory.Exists(Path.Combine(path, ".git"));
        }

        public VCItemStatus GetOverlay(string path, string rootPath)
        {
            var snode = FindNode(path, rootPath);
            return snode?.Status ?? VCItemStatus.Ignored;
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

        private void GetChildren(StatusNode node, ICollection<StatusNode> result)
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
            return !ignoreDirty
                   && !reIgnore.IsMatch(path)
                   && statusCache.ContainsKey(rootPath)
                   && statusCache[rootPath].SetPathDirty(path);
        }

        public void Commit(string[] files, string message)
        {
            new CommitCommand(files, message, Path.GetDirectoryName(files[0]));
        }
    }
}
