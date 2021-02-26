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
            FormGuid = "b5a7f1b4-959b-485b-a7d7-9b683191c0cf";
            Font = PluginBase.Settings.DefaultFont;
            browserView.ImageList = Icons.ImageList;
            browserView.BeforeExpand += OnBeforeExpandNode;
        }

        void LocalizeTexts()
        {
            inviteLabel.Text = TextHelper.GetString("Wizard.Label.ChosePackage");
            okButton.Text = TextHelper.GetString("Wizard.Button.Ok");
            cancelButton.Text = TextHelper.GetString("Wizard.Button.Cancel");
            Text = TextHelper.GetString("Wizard.Label.PackageSelection");
        }

        public Project Project { get; set; }

        public string Package => ((SimpleDirectoryNode) browserView.SelectedNode)?.directoryPath;

        void Initialize() => classpathList = new List<string>();

        public void AddClassPath(string value) => classpathList.Add(value);

        void RefreshTree()
        {
            browserView.BeginUpdate();
            browserView.Nodes.Clear();

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
                                    var node = new SimpleDirectoryNode(item, Path.Combine(cp, item))
                                    {
                                        ImageIndex = Icons.Folder.Index,
                                        SelectedImageIndex = Icons.Folder.Index
                                    };
                                    browserView.Nodes.Add(node);
                                }
                            }
                        }
                        catch { }
                }
            }
            browserView.EndUpdate();
        }

        void PackageBrowser_Load(object sender, EventArgs e) => RefreshTree();

        void OnBeforeExpandNode(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is SimpleDirectoryNode {dirty: true} node)
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

        void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            browserView.SelectedNode = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
