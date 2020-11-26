using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using ProjectManager.Controls.TreeView;

namespace SourceControl.Sources.Subversion
{
    internal class MenuItems : IVCMenuItems
    {
        TreeNode[] currentNodes;
        IVCManager currentManager;

        readonly ToolStripItem update;
        readonly ToolStripItem commit;
        readonly ToolStripItem push;
        readonly ToolStripItem showLog;
        readonly ToolStripItem midSeparator;
        readonly ToolStripItem annotate;
        readonly ToolStripItem diff;
        readonly ToolStripItem diffChange;
        readonly ToolStripItem add;
        readonly ToolStripItem ignore;
        readonly ToolStripItem undoAdd;
        readonly ToolStripItem revert;
        readonly ToolStripItem editConflict;

        public TreeNode[] CurrentNodes { set => currentNodes = value; }
        public IVCManager CurrentManager { set => currentManager = value; }

        public ToolStripItem Update => update;
        public ToolStripItem Commit => commit;
        public ToolStripItem Push => push;
        public ToolStripItem ShowLog => showLog;
        public ToolStripItem MidSeparator => midSeparator;
        public ToolStripItem Annotate => annotate;
        public ToolStripItem Diff => diff;
        public ToolStripItem DiffChange => diffChange;
        public ToolStripItem Add => add;
        public ToolStripItem Ignore => ignore;
        public ToolStripItem UndoAdd => undoAdd;
        public ToolStripItem Revert => revert;
        public ToolStripItem EditConflict => editConflict;

        readonly Dictionary<ToolStripItem, VCMenuItemProperties> items = new Dictionary<ToolStripItem, VCMenuItemProperties>();
        public Dictionary<ToolStripItem, VCMenuItemProperties> Items => items;

        public MenuItems()
        {
            update = new ToolStripMenuItem(TextHelper.GetString("Label.Update"), PluginBase.MainForm.FindImage("159|1|-3|3"), Update_Click);
            commit = new ToolStripMenuItem(TextHelper.GetString("Label.Commit"), PluginBase.MainForm.FindImage("62"), Commit_Click);
            push = null;
            showLog = new ToolStripMenuItem(TextHelper.GetString("Label.ShowLog"), PluginBase.MainForm.FindImage("95"), ShowLog_Click);
            midSeparator = new ToolStripSeparator();
            annotate = new ToolStripMenuItem("Annotate", PluginBase.MainForm.FindImage("45")) { Enabled = false };
            diff = new ToolStripMenuItem(TextHelper.GetString("Label.Diff"), PluginBase.MainForm.FindImage("251"), Diff_Click);
            diffChange = new ToolStripMenuItem(TextHelper.GetString("Label.DiffWithPrevious"), PluginBase.MainForm.FindImage("251"), DiffChange_Click);
            add = new ToolStripMenuItem(TextHelper.GetString("Label.Add"), PluginBase.MainForm.FindImage("33"), Add_Click);
            ignore = new ToolStripMenuItem(TextHelper.GetString("Label.AddToIgnoreList"), PluginBase.MainForm.FindImage("166"), Ignore_Click);
            undoAdd = new ToolStripMenuItem(TextHelper.GetString("Label.UndoAdd"), PluginBase.MainForm.FindImage("73"), UndoAdd_Click);
            revert = new ToolStripMenuItem(TextHelper.GetString("Label.Revert"), PluginBase.MainForm.FindImage("73"), Revert_Click);
            editConflict = new ToolStripMenuItem(TextHelper.GetString("Label.EditConflict"), PluginBase.MainForm.FindImage("196"), EditConflict_Click);
        }

        string GetPaths()
        {
            var paths = new List<string>();
            if (currentNodes != null)
                foreach (var node in currentNodes)
                {
                    if (node is GenericNode genericNode)
                        paths.Add(genericNode.BackingPath);
                }
            return string.Join("*", paths);
        }

        void EditConflict_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("conflicteditor", GetPaths());
        }

        void Revert_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("revert", GetPaths());
        }

        void UndoAdd_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("revert", GetPaths());
        }

        void Add_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("add", GetPaths());
        }

        void Ignore_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("ignore", GetPaths());
        }

        void DiffChange_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("diff", GetPaths());
        }

        void Diff_Click(object sender, EventArgs e)
        {
            if (currentNodes is null || currentNodes.Length != 2)
                return;
            var path1 = (currentNodes[0] as GenericNode).BackingPath;
            var path2 = (currentNodes[1] as GenericNode).BackingPath;
            TortoiseProc.Execute("diff", path1, path2);
        }

        void ShowLog_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("log", GetPaths());
        }

        void Commit_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("commit", GetPaths());
        }

        void Update_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("update", GetPaths());
        }
    }
}
