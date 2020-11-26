using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace SourceControl.Sources.Mercurial
{
    internal class Status : BaseCommand
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
            ignores = new Ignores(path, ".hgignore");
        }

        public StatusNode Get(string path)
        {
            var found = root.FindPath(path);
            if (found is null)
            {
                foreach (var ignore in ignores)
                {
                    if ((ignore.path == "" || path.StartsWithOrdinal(ignore.path)) && ignore.regex.IsMatch(path))
                    {
                        found = root.MapPath(path.Substring(ignore.path.Length), VCItemStatus.Ignored);
                        return found;
                    }
                }
                found = new StatusNode(Path.GetFileName(path), VCItemStatus.Unknown);
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

            Run("status -A", updatingPath);
        }

        public bool SetPathDirty(string path)
        {
            if (path is null) return false;
            if (string.IsNullOrEmpty(dirty))
            {
                dirty = path;
                return true;
            }

            var sep = Path.DirectorySeparatorChar;
            var p1 = dirty.Split(sep);
            var p2 = path.Split(sep);

            var len = Math.Min(p1.Length, p2.Length);
            path = "";
            for (var i = 0; i < len; i++)
            {
                if (p1[i] == p2[i]) path += sep + p1[i];
                else break;
            }
            dirty = path.Length > 0 ? path.Substring(1) : null;
            return true;
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            runner = null;
            if (exitCode != 0)
            {
                var label = TextHelper.GetString("SourceControl.Label.UnableToGetRepoStatus");
                TraceManager.AddAsync(label + " (" + exitCode + ")");
            }

            if (updatingPath == RootPath) root = temp;
            if (OnResult != null) OnResult(this);
        }

        protected override void Runner_Output(object sender, string line)
        {
            var fileIndex = 0;
            if (line.Length < 3) return;
            var c0 = line[0];
            var c1 = line[1];

            var s = VCItemStatus.Added;
            if (c0 == '?') s = VCItemStatus.Unknown;
            else if (c0 == '!') s = VCItemStatus.Missing;
            else if (c0 == 'A') s = VCItemStatus.Added;
            else if (c0 == 'C') s = VCItemStatus.UpToDate; // clean == UpToDate
            else if (c0 == 'I') s = VCItemStatus.Ignored;
            else if (c0 == 'M' || c1 == 'M') s = VCItemStatus.Modified;
            else if (c0 == 'R' || c1 == 'R') s = VCItemStatus.Deleted;

            if (s == VCItemStatus.Unknown) return;
            var p = line.IndexOfOrdinal(" ");
            if (p > 0) line = line.Substring(p + 1);
            else line = line.Substring(fileIndex);
            temp.MapPath(line, s);
        }
    }

    internal delegate void StatusResult(Status sender);

    internal class StatusNode
    {
        public static StatusNode UNKNOWN = new StatusNode("*", VCItemStatus.Unknown);

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

            var p = path.IndexOf(Path.DirectorySeparatorChar);
            var childName = p < 0 ? path : path.Substring(0, p);
            if (HasChildren && Children.ContainsKey(childName))
            {
                var child = Children[childName];
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
            var p = path.IndexOf(Path.DirectorySeparatorChar);
            if (p < 0) return AddChild(path, status, true);
            else if (p == path.Length - 1) return AddChild(path.Substring(0, path.Length - 1), status, true);
            else return AddChild(path.Substring(0, p), status, false)
                .MapPath(path.Substring(p + 1), status);
        }

        StatusNode AddChild(string name, VCItemStatus status, bool isLeaf)
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

            var node = new StatusNode(name, status);
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
