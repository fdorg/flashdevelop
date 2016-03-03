using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProjectManager.Actions;
using ProjectManager.Controls;
using ProjectManager.Controls.TreeView;
using ProjectManager.Projects;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace ProjectManager
{
    public class PluginUI : DockPanelControl
    {
        public FDMenus menus;
        TreeBar treeBar;
        Project project;
        LinkLabel help;
        ProjectTreeView tree;
        ProjectContextMenu menu;
        Boolean isEditingLabel;

        public event EventHandler NewProject;
        public event EventHandler OpenProject;
        public event EventHandler ImportProject;
        public event RenameEventHandler Rename;
        
        public PluginUI(PluginMain plugin, FDMenus menus, FileActions fileActions, ProjectActions projectActions)
        {
            this.menus = menus;
            this.AutoKeyHandling = true;
            this.Text = TextHelper.GetString("Title.PluginPanel");
            
            #region Build TreeView and Toolbar

            menu = new ProjectContextMenu();
            menu.Rename.Click += RenameNode;

            treeBar = new TreeBar(menus, menu);

            tree = new ProjectTreeView();
            tree.BorderStyle = BorderStyle.None;
            tree.Dock = DockStyle.Fill;
            tree.ImageIndex = 0;
            tree.ImageList = Icons.ImageList;
            tree.LabelEdit = true;
            tree.SelectedImageIndex = 0;
            tree.ShowRootLines = false;
            tree.HideSelection = false;
            tree.ContextMenuStrip = menu;
            tree.AfterLabelEdit += tree_AfterLabelEdit;
            tree.BeforeLabelEdit += tree_BeforeLabelEdit;
            tree.BeforeSelect += tree_BeforeSelect;
            tree.AfterSelect += tree_AfterSelect;

            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(tree);
            panel.Controls.Add(treeBar);

            menu.ProjectTree = tree;

            #endregion

            #region Instructions

            help = new LinkLabel();
            string[] helpParts = String.Format(TextHelper.GetString("Info.NoProjectsOpenLink"), "\n").Split('|');
            string[] helpActions = { "create", "open", "import" };
            string helpText = "";
            int[] linkStart = { 0, 0, 0 };
            int[] linkLength = { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
            {
                if (helpParts.Length > i * 2)
                {
                    helpText += helpParts[i * 2];
                    linkStart[i] = helpText.Length;
                    helpText += helpParts[i * 2 + 1];
                    linkLength[i] = helpParts[i * 2 + 1].Length;
                }
            }
            help.Text = helpText + helpParts[helpParts.Length - 1];
            for (int i = 0; i < 3; i++)
            {
                help.Links.Add(linkStart[i], linkLength[i], helpActions[i]);
            }
            help.LinkClicked += link_LinkClicked;
            help.Dock = DockStyle.Fill;
            help.TextAlign = ContentAlignment.MiddleCenter;
            help.ContextMenu = new ContextMenu();

            #endregion

            this.Controls.Add(help);
            this.Controls.Add(panel);

            #region Events

            fileActions.FileCreated += NewFileCreated;
            fileActions.ProjectModified += ProjectModified;
            projectActions.ProjectModified += ProjectModified;

            #endregion
        }

        public void ShowHiddenPaths(bool show)
        {
            TreeBar.ShowHidden.Checked = show;
            Menu.ShowHidden.Checked = show;
        }

        internal void NotifyIssues()
        {
            treeBar.ProjectHasIssues = BuildActions.LatestSDKMatchQuality > 0;
        }

        public void SetProject(Project project)
        {
            if (this.project == project) return;

            this.project = project;

            List<Project> projects = tree.Projects;
            projects.Clear(); // only one project active
            if (project != null) projects.Add(project);
            tree.Projects = projects;
            tree.Project = project;
            tree_AfterSelect(tree, null);

            help.Visible = (project == null);

            if (project != null)
            {
                TreeBar.ShowHidden.Checked = project.ShowHiddenPaths;
                IsTraceDisabled = !project.TraceEnabled;
            }
        }

        #region Public Properties

        public ProjectTreeView Tree  { get { return this.tree; }  }
        public ProjectContextMenu Menu  { get { return this.menu; }  }
        public TreeBar TreeBar  { get { return this.treeBar; } }

        public bool IsTraceDisabled
        {
            get { return menus.ConfigurationSelector.SelectedIndex == 1; }
            set
            {
                menus.ConfigurationSelector.SelectedIndex = (value) ? 1 : 0;
                PluginMain.Settings.GetPrefs(project).DebugMode = !value;
            }
        }

        /// <summary>
        /// A label of the project tree is currently beeing edited
        /// </summary> 
        public Boolean IsEditingLabel
        {
            get { return this.isEditingLabel; }
            set { this.isEditingLabel = value; }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Instructions panel link clicked
        /// </summary>
        private void link_LinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            string action = e.Link.LinkData as string;
            if (action == "create" && NewProject != null) NewProject(sender, e);
            else if (action == "open" && OpenProject != null) OpenProject(sender, e);
            else if (action == "import" && ImportProject != null) ImportProject(sender, e);
        }

        /// <summary>
        /// Look for a parent node of the given path (if it exists) and 
        /// ask it to refresh.  This is necessary because filesystemwatcher 
        /// doesn't always work over network shares.
        /// </summary>
        public void WatchParentOf(String path)
        {
            try
            {
                String parent = Path.GetDirectoryName(path);
                WatcherNode node = tree.NodeMap[parent] as WatcherNode;
                if (node != null) node.UpdateLater();
            }
            catch { }
        }

        /// <summary>
        /// We don't want to trigger these while editing
        /// </summary>
        private void tree_BeforeLabelEdit(Object sender, NodeLabelEditEventArgs e)
        {
            if (!e.CancelEdit)
            {
                DataEvent de = new DataEvent(EventType.Command, ProjectFileActionsEvents.FileBeforeRename, tree.SelectedNode.BackingPath);
                EventManager.DispatchEvent(this, de);
                if (de.Handled) e.CancelEdit = true;
                else isEditingLabel = true;
            }
        }

        /// <summary>
        /// Happens if you back out
        /// </summary>
        private void tree_AfterLabelEdit(Object sender, NodeLabelEditEventArgs e)
        {
            string languageDisplayName = "(" + project.LanguageDisplayName + ")";
            if (!string.IsNullOrEmpty(e.Label) && Rename != null)
            {
                if (e.Node is ProjectNode)
                {
                    var oldName = project.ProjectPath;
                    string label = e.Label;
                    int index = label.IndexOf(languageDisplayName);
                    if (index != -1) label = label.Remove(index).Trim();
                    string newName = string.Empty;
                    try
                    {
                        newName = Path.Combine(project.Directory, label);
                        newName = Path.ChangeExtension(newName, Path.GetExtension(oldName));
                    }
                    catch (Exception)
                    {
                        e.CancelEdit = true;
                        isEditingLabel = false;
                        return;
                    }
                    if (Rename(oldName, newName))
                    {
                        PluginBase.MainForm.OpenEditableDocument(newName);
                        try
                        {
                            File.Delete(oldName);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    else e.CancelEdit = true;
                }
                else if (!Rename(((GenericNode) e.Node).BackingPath, e.Label))
                    e.CancelEdit = true;
            }
            else e.CancelEdit = true;
            if (e.Node is ProjectNode && !e.Node.Text.Contains(languageDisplayName))
                e.Node.Text += " " + languageDisplayName;
            isEditingLabel = false;
        }

        /// <summary>
        /// Don't allow non-generic nodes to be selected
        /// </summary>
        void tree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!(e.Node is GenericNode)) e.Cancel = true;
            isEditingLabel = false;
        }

        /// <summary>
        /// Customize the context menu
        /// </summary>
        private void tree_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            if (tree.SelectedNodes.Count == 0) return;
            Project project = Tree.ProjectOf(tree.SelectedNodes[0] as GenericNode);
            menu.Configure(tree.SelectedNodes, project);
            // notify other plugins of tree nodes selection - ourben@fdc
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.TreeSelectionChanged, tree.SelectedNodes);
            EventManager.DispatchEvent(tree, de); 
        }

        /// <summary>
        /// A new file was created and we want it to be selected after
        /// the filesystemwatcher finds it and makes us refresh
        /// </summary>
        private void NewFileCreated(String path)
        {
            tree.PathToSelect = path;
            WatchParentOf(path);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenameNode(Object sender, EventArgs e)
        {
            if (tree.SelectedNode is ProjectNode)
            {
                string label = tree.SelectedNode.Text;
                int index = label.IndexOf("(" + project.LanguageDisplayName + ")");
                if (index != -1) tree.SelectedNode.Text = label.Remove(index).Trim();
            }
            tree.ForceLabelEdit();
        }

        /// <summary>
        /// The project has changed, so refresh the tree
        /// </summary>
        private void ProjectModified(String[] paths)
        {
            tree.RefreshTree(paths);
        }

        #endregion

    }

    /// <summary>
    ///  Event delegates of the class
    /// </summary>
    public delegate bool RenameEventHandler(String path, String newName);

}
