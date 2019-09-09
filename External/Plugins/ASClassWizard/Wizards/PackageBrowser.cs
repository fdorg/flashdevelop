// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Controls;
using ProjectManager.Projects;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Controls;
using ASClassWizard.Controls.TreeView;

namespace ASClassWizard.Wizards
{
    public partial class PackageBrowser : SmartForm
    {
        List<string> classpathList;

        public PackageBrowser()
        {
            Initialize();
            InitializeComponent();
            LocalizeTexts();
            CenterToParent();
            this.FormGuid = "b5a7f1b4-959b-485b-a7d7-9b683191c0cf";
            this.Font = PluginBase.Settings.DefaultFont;
            this.browserView.ImageList = Icons.ImageList;
            this.browserView.BeforeExpand += onBeforeExpandNode;
        }

        private void LocalizeTexts()
        {
            this.inviteLabel.Text = TextHelper.GetString("Wizard.Label.ChosePackage");
            this.okButton.Text = TextHelper.GetString("Wizard.Button.Ok");
            this.cancelButton.Text = TextHelper.GetString("Wizard.Button.Cancel");
            this.Text = TextHelper.GetString("Wizard.Label.PackageSelection");
        }

        public Project Project { get; set; }

        public string Package => ((SimpleDirectoryNode) browserView.SelectedNode)?.directoryPath;

        private void Initialize() => classpathList = new List<string>();

        public void AddClassPath(string value) => classpathList.Add(value);

        private void RefreshTree()
        {
            this.browserView.BeginUpdate();
            this.browserView.Nodes.Clear();

            if (classpathList.Count > 0)
            {
                foreach(string cp in classpathList)
                {
                    if (Directory.Exists(cp))
                        try
                        {
                            foreach (string item in Directory.GetDirectories(cp))
                            {
                                if (!IsDirectoryExcluded(item))
                                {
                                    var node = new SimpleDirectoryNode(item, Path.Combine(cp, item));
                                    node.ImageIndex = Icons.Folder.Index;
                                    node.SelectedImageIndex = Icons.Folder.Index;
                                    this.browserView.Nodes.Add(node);
                                }
                            }
                        }
                        catch { }
                }
            }
            this.browserView.EndUpdate();
        }

        private void PackageBrowser_Load(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void onBeforeExpandNode(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is SimpleDirectoryNode node && node.dirty)
            {
                node.dirty = false;
                node.Nodes.Clear();
                foreach (string item in Directory.GetDirectories(node.directoryPath))
                {
                    if (!IsDirectoryExcluded(item))
                    {
                        var newNode = new SimpleDirectoryNode(item, Path.Combine(node.directoryPath, item));
                        newNode.ImageIndex = Icons.Folder.Index;
                        newNode.SelectedImageIndex = Icons.Folder.Index;
                        node.Nodes.Add(newNode);
                    }
                }
            }
        }

        /// <summary>
        /// Verify if a given directory is hidden
        /// </summary>
        protected bool IsDirectoryExcluded(string path)
        {
            string dirName = Path.GetFileName(path);
            foreach (string excludedDir in ProjectManager.PluginMain.Settings.ExcludedDirectories)
            {
                if (dirName.ToLower() == excludedDir)
                {
                    return true;
                }
            }
            return Project.IsPathHidden(path) && !Project.ShowHiddenPaths;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.browserView.SelectedNode = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
