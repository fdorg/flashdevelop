using System;
using System.Collections.Generic;
using System.IO;
using PluginCore.Localization;
using PluginCore.Managers;

namespace SourceControl.Sources.Git
{
    class Status : BaseCommand
    {
        public event StatusResult OnResult;

        public string RootPath;

        StatusNode root = new StatusNode(".", VCItemStatus.Undefined);
        StatusNode temp;
        string dirty;
        string updatingPath;
        Ignores ignores;

        public Status(string path)
        {
            RootPath = path;
            ignores = new Ignores(path, ".gitignore");
        }

        public StatusNode Get(string path)
        {
            StatusNode found = root.FindPath(path);
            if (found == null)
            {
                foreach (IgnoreEntry ignore in ignores)
                {
                    if ((ignore.path == "" || path.StartsWith(ignore.path)) && ignore.regex.IsMatch(path))
                    {
                        found = root.MapPath(path.Substring(ignore.path.Length), VCItemStatus.Ignored);
                        return found;
                    }
                }
                found = new StatusNode(Path.GetFileName(path), VCItemStatus.UpToDate);
            }
            return found;
        }

        public void Update()
        {
            if (runner != null) return;

            temp = new StatusNode(".", VCItemStatus.Undefined);
            updatingPath = RootPath;
            dirty = null;
            ignores.Update();

            Run("status -s -b --porcelain", updatingPath);
        }

        public bool SetPathDirty(string path)
        {
            if (path == null) return false;
            if (string.IsNullOrEmpty(dirty))
            {
                dirty = path;
                return true;
            }

            char sep = Path.DirectorySeparatorChar;
            string[] p1 = dirty.Split(sep);
            string[] p2 = path.Split(sep);

            int len = Math.Min(p1.Length, p2.Length);
            path = "";
            for (int i = 0; i < len; i++)
            {
                if (p1[i] == p2[i]) path += sep + p1[i];
                else break;
            }
            dirty = path.Length > 0 ? path.Substring(1) : null;
            return true;
        }

        override protected void Runner_ProcessEnded(object sender, int exitCode)
        {
            runner = null;
            if (exitCode != 0)
            {
                String label = TextHelper.GetString("SourceControl.Label.UnableToGetRepoStatus");
                TraceManager.AddAsync(label + " (" + exitCode + ")");
            }

            if (updatingPath == RootPath) root = temp;
            if (OnResult != null) OnResult(this);
        }

        override protected void Runner_Output(object sender, string line)
        {
            int fileIndex = 3;
            if (line.Length < fileIndex) return;

            char c0 = line[0];
            char c1 = line[1];
            if (c1 == ':') return;

            VCItemStatus s = VCItemStatus.UpToDate;
            //else if (c0 == 'I') s = VCItemStatus.Ignored; // TODO look into .gitignore
            if (c0 == '?') s = VCItemStatus.Unknown;
            if (c0 == 'U') s = VCItemStatus.Conflicted;
            else if (c0 == 'A') s = VCItemStatus.Added;
            else if (c0 == 'C') s = VCItemStatus.Added; // copied
            else if (c0 == 'R') s = VCItemStatus.Added; // renamed
            else if (c0 == 'M' || c1 == 'M') s = VCItemStatus.Modified;
            else if (c0 == 'D' || c1 == 'D') s = VCItemStatus.Deleted;
            else if (c0 == '#') return;

            int p = line.IndexOf(" -> ");
            if (p > 0) line = line.Substring(p + 4);
            else line = line.Substring(fileIndex);
            temp.MapPath(line, s);
        }
    }

    delegate void StatusResult(Status sender);

    class StatusNode
    {
        static public StatusNode UNKNOWN = new StatusNode("*", VCItemStatus.Unknown);

        public bool HasChildren;
        public string Name;
        public VCItemStatus Status;
        public StatusNode Parent;
        public Dictionary<string, StatusNode> Children;

        public StatusNode(string name, VCItemStatus status)
        {
            Name = name;
            Status = status;
        }

        /// <summary>
        /// Recursively find a path in the tree
        /// </summary>
        /// <returns>Last node of the path</returns>
        public StatusNode FindPath(string path)
        {
            if (path == ".") return this;
            if (Status == VCItemStatus.Unknown) return UNKNOWN;

            int p = path.IndexOf(Path.DirectorySeparatorChar);
            string childName = p < 0 ? path : path.Substring(0, p);
            if (HasChildren && Children.ContainsKey(childName))
            {
                StatusNode child = Children[childName];
                if (p > 0) return child.FindPath(path.Substring(p + 1));
                else return child;
            }
            return null;
        }

        /// <summary>
        /// Recursively create (if needed) nodes to match the path
        /// </summary>
        /// <returns>Last node of the path</returns>
        public StatusNode MapPath(string path, VCItemStatus status)
        {
            int p = path.IndexOf('/');
            if (p < 0) return AddChild(path, status, true);
            else if (p == path.Length - 1) return AddChild(path.Substring(0, path.Length - 1), status, true);
            else return AddChild(path.Substring(0, p), status, false)
                .MapPath(path.Substring(p + 1), status);
        }

        private StatusNode AddChild(string name, VCItemStatus status, bool isLeaf)
        {
            if (name == ".")
            {
                if (status != VCItemStatus.Unknown) Status = status;
                return this;
            }

            // inherit child status
            if (Status < status && status > VCItemStatus.UpToDate)
                Status = status == VCItemStatus.Conflicted ? status : VCItemStatus.Modified;

            // add/retrieve child
            if (!isLeaf)
            {
                if (status > VCItemStatus.UpToDate && status != VCItemStatus.Conflicted)
                    status = VCItemStatus.Modified;
                else status = VCItemStatus.UpToDate;
            }

            StatusNode node = new StatusNode(name, status);
            node.Parent = this;
            if (!HasChildren)
            {
                HasChildren = true;
                Children = new Dictionary<string, StatusNode>();
                Children.Add(name, node);
            }
            else if (Children.ContainsKey(name))
            {
                return Children[name];
            }
            else Children.Add(name, node);
            return node;
        }
    }
}
