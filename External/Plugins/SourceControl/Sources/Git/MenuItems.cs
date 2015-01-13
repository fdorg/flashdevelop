﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using ProjectManager.Controls.TreeView;
using ProjectManager.Helpers;
using PluginCore.Localization;

namespace SourceControl.Sources.Git
{
    class MenuItems : IVCMenuItems
    {
        private TreeNode[] currentNodes;
        private IVCManager currentManager;

        private ToolStripItem update;
        private ToolStripItem commit;
        private ToolStripItem push;
        private ToolStripItem showLog;
        private ToolStripItem midSeparator;
        private ToolStripItem diff;
        private ToolStripItem diffChange;
        private ToolStripItem add;
        private ToolStripItem ignore;
        private ToolStripItem undoAdd;
        private ToolStripItem revert;
        private ToolStripItem editConflict;

        public TreeNode[] CurrentNodes { set { currentNodes = value; } }
        public IVCManager CurrentManager { set { currentManager = value; } }

        public ToolStripItem Update { get { return update; } }
        public ToolStripItem Commit { get { return commit; } }
        public ToolStripItem Push { get { return push; } }
        public ToolStripItem ShowLog { get { return showLog; } }
        public ToolStripItem MidSeparator { get { return midSeparator; } }
        public ToolStripItem Diff { get { return diff; } }
        public ToolStripItem DiffChange { get { return diffChange; } }
        public ToolStripItem Add { get { return add; } }
        public ToolStripItem Ignore { get { return ignore; } }
        public ToolStripItem UndoAdd { get { return undoAdd; } }
        public ToolStripItem Revert { get { return revert; } }
        public ToolStripItem EditConflict { get { return editConflict; } }

        private Dictionary<ToolStripItem, VCMenuItemProperties> items = new Dictionary<ToolStripItem, VCMenuItemProperties>();
        public Dictionary<ToolStripItem, VCMenuItemProperties> Items { get { return items; } }

        public MenuItems()
        {
            update = new ToolStripMenuItem(TextHelper.GetString("Label.Pull"), PluginBase.MainForm.FindImage("159|1|-3|3"), Update_Click);
            commit = new ToolStripMenuItem(TextHelper.GetString("Label.Commit"), PluginBase.MainForm.FindImage("62"), Commit_Click);
            push = new ToolStripMenuItem(TextHelper.GetString("Label.Push"), PluginBase.MainForm.FindImage("159|9|-3|3"), Push_Click);
            showLog = new ToolStripMenuItem(TextHelper.GetString("Label.ShowLog"), PluginBase.MainForm.FindImage("95"), ShowLog_Click);
            midSeparator = new ToolStripSeparator();
            diff = new ToolStripMenuItem(TextHelper.GetString("Label.Diff"), PluginBase.MainForm.FindImage("251"), Diff_Click);
            diffChange = new ToolStripMenuItem(TextHelper.GetString("Label.DiffWithPrevious"), PluginBase.MainForm.FindImage("251"), DiffChange_Click);
            add = new ToolStripMenuItem(TextHelper.GetString("Label.Add"), PluginBase.MainForm.FindImage("33"), Add_Click);
            ignore = new ToolStripMenuItem(TextHelper.GetString("Label.AddToIgnoreList"), PluginBase.MainForm.FindImage("166"), Ignore_Click);
            undoAdd = new ToolStripMenuItem(TextHelper.GetString("Label.UndoAdd"), PluginBase.MainForm.FindImage("73"), UndoAdd_Click);
            revert = new ToolStripMenuItem(TextHelper.GetString("Label.Revert"), PluginBase.MainForm.FindImage("73"), Revert_Click);
            editConflict = new ToolStripMenuItem(TextHelper.GetString("Label.EditConflict"), PluginBase.MainForm.FindImage("196"), EditConflict_Click);
        }

        private string GetPaths()
        {
            return String.Join("*", GetPathsArray());
        }

        private string[] GetPathsArray()
        {
            List<string> paths = new List<string>();
            if (currentNodes != null)
                foreach (TreeNode node in currentNodes)
                    if (node is GenericNode) paths.Add((node as GenericNode).BackingPath);

            return paths.ToArray();
        }

        private void EditConflict_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("conflicteditor", GetPaths());
        }

        private void Revert_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("revert", GetPaths());
        }

        private void UndoAdd_Click(object sender, EventArgs e)
        {
            new ResetCommand(GetPathsArray());
        }

        private void Add_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("add", GetPaths());
        }

        private void Ignore_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("ignore", GetPaths());
        }

        private void DiffChange_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("diff", GetPaths());
        }

        private void Diff_Click(object sender, EventArgs e)
        {
            if (currentNodes == null || currentNodes.Length != 2)
                return;
            string path1 = (currentNodes[0] as GenericNode).BackingPath;
            string path2 = (currentNodes[1] as GenericNode).BackingPath;
            TortoiseProc.Execute("diff", path1, path2);
        }

        private void ShowLog_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("log", GetPaths());
        }

        private void Push_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("push", GetPaths());
        }

        private void Commit_Click(object sender, EventArgs e)
        {
            String title = TextHelper.GetString("Label.Commit");
            String msg = TextHelper.GetString("Info.EnterMessage");
            LineEntryDialog led = new LineEntryDialog(title, msg, "");
            if (led.ShowDialog() != DialogResult.OK || led.Line == "")
                return;

            new CommitCommand(GetPathsArray(), led.Line);
        }

        private void Update_Click(object sender, EventArgs e)
        {
            TortoiseProc.Execute("pull", GetPaths());
        }
    }
}
