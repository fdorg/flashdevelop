using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ProjectManager.Actions;
using ProjectManager.Controls;
using ProjectManager.Controls.TreeView;
using ProjectManager.Projects;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;

namespace ProjectManager
{
    public class PluginUI : DockPanelControl
    {
        public FDMenus menus;
        Project project;
        readonly LinkLabel help;

        public event EventHandler NewProject;
        public event EventHandler OpenFolder;
        public event EventHandler OpenProject;
        public event EventHandler ImportProject;
        public event RenameEventHandler Rename;
        
        public PluginUI(FDMenus menus, FileActions fileActions, ProjectActions projectActions)
        {
            this.menus = menus;
            AutoKeyHandling = true;
            Text = TextHelper.GetString("Title.PluginPanel");
            
            #region Build TreeView and Toolbar

            Menu = new ProjectContextMenu();
            Menu.Rename.Click += RenameNode;

            TreeBar = new TreeBar();

            Tree = new ProjectTreeView();
            Tree.BorderStyle = BorderStyle.None;
            Tree.Dock = DockStyle.Fill;
            Tree.ImageIndex = 0;
            Tree.ImageList = Icons.ImageList;
            Tree.LabelEdit = true;
            Tree.SelectedImageIndex = 0;
            Tree.ShowRootLines = false;
            Tree.HideSelection = false;
            Tree.ContextMenuStrip = Menu;
            Tree.AfterLabelEdit += tree_AfterLabelEdit;
            Tree.BeforeLabelEdit += tree_BeforeLabelEdit;
            Tree.BeforeSelect += tree_BeforeSelect;
            Tree.AfterSelect += tree_AfterSelect;

            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(Tree);
            panel.Controls.Add(TreeBar);

            Menu.ProjectTree = Tree;
            ScrollBarEx.Attach(Tree);

            #endregion

            #region Instructions

            help = new LinkLabel();
            string[] helpParts = string.Format(TextHelper.GetString("Info.NoProjectsOpenLink"), "\n").Split('|');
            string[] helpActions = { "folder", "create", "open", "import|FlashBuilder", "import|hxml" };
            var helpActionsLength = helpActions.Length;
            int[] linkStart = new int[helpActionsLength];
            int[] linkLength = new int[helpActionsLength];
            string helpText = "";
            for (int i = 0; i < helpActionsLength; i++)
            {
                var linkIndex = i * 2;
                if (helpParts.Length > linkIndex)
                {
                    helpText += helpParts[linkIndex];
                    linkStart[i] = helpText.Length;
                    helpText += helpParts[linkIndex + 1];
                    linkLength[i] = helpParts[linkIndex + 1].Length;
                }
            }
            help.Text = helpText + helpParts[helpParts.Length - 1];
            for (int i = 0; i < helpActionsLength; i++)
            {
                help.Links.Add(linkStart[i], linkLength[i], helpActions[i]);
            }
            help.LinkClicked += link_LinkClicked;
            help.Dock = DockStyle.Fill;
            help.TextAlign = ContentAlignment.MiddleCenter;

            #endregion

            Controls.Add(help);
            Controls.Add(panel);

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

        internal void NotifyIssues() => TreeBar.ProjectHasIssues = BuildActions.LatestSDKMatchQuality > 0;

        public void SetProject(Project project)
        {
            if (this.project == project) return;
            this.project = project;
            var projects = Tree.Projects;
            projects.Clear(); // only one project active
            if (project != null) projects.Add(project);
            Tree.Projects = projects;
            Tree.Project = project;
            tree_AfterSelect(Tree, null);
            help.Visible = (project is null);
            if (project != null)
            {
                TreeBar.ShowHidden.Checked = project.ShowHiddenPaths;
                IsTraceDisabled = !project.TraceEnabled;
            }
        }

        #region Public Properties

        public ProjectTreeView Tree { get; }
        public ProjectContextMenu Menu { get; }
        public TreeBar TreeBar { get; }

        public bool IsTraceDisabled
        {
            get => menus.ConfigurationSelector.SelectedIndex == 1;
            set
            {
                menus.ConfigurationSelector.SelectedIndex = (value) ? 1 : 0;
                PluginMain.Settings.GetPrefs(project).DebugMode = !value;
            }
        }

        /// <summary>
        /// A label of the project tree is currently beeing edited
        /// </summary> 
        public bool IsEditingLabel { get; set; }

        #endregion

        #region Event Handling

        /// <summary>
        /// Instructions panel link clicked
        /// </summary>
        void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string action = e.Link.LinkData as string;
            if (action == "create" && NewProject != null) NewProject(sender, e);
            else if (action == "open" && OpenProject != null) OpenProject(sender, e);
            else if (action == "folder" && OpenFolder != null) OpenFolder(sender, e);
            else if (action != null && action.StartsWith("import|")) ImportProject?.Invoke(sender, e);
        }

        /// <summary>
        /// Look for a parent node of the given path (if it exists) and 
        /// ask it to refresh.  This is necessary because filesystemwatcher 
        /// doesn't always work over network shares.
        /// </summary>
        public void WatchParentOf(string path)
        {
            try
            {
                string parent = Path.GetDirectoryName(path);
                WatcherNode node = Tree.NodeMap[parent] as WatcherNode;
                node?.UpdateLater();
            }
            catch { }
        }

        /// <summary>
        /// We don't want to trigger these while editing
        /// </summary>
        void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!e.CancelEdit)
            {
                DataEvent de = new DataEvent(EventType.Command, ProjectFileActionsEvents.FileBeforeRename, Tree.SelectedNode.BackingPath);
                EventManager.DispatchEvent(this, de);
                if (de.Handled) e.CancelEdit = true;
                else IsEditingLabel = true;
            }
        }

        /// <summary>
        /// Happens if you back out
        /// </summary>
        void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string languageDisplayName = "(" + project.LanguageDisplayName + ")";
            if (!string.IsNullOrEmpty(e.Label) && Rename != null)
            {
                var rename = Rename;
                if (e.Node is ProjectNode)
                {
                    var oldName = project.ProjectPath;
                    string label = e.Label;
                    int index = label.IndexOf(languageDisplayName);
                    if (index != -1) label = label.Remove(index).Trim();
                    string newName;
                    try
                    {
                        newName = Path.Combine(project.Directory, label);
                        newName = Path.ChangeExtension(newName, Path.GetExtension(oldName));
                    }
                    catch (Exception)
                    {
                        e.CancelEdit = true;
                        IsEditingLabel = false;
                        return;
                    }
                    if (rename(oldName, newName))
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
                else if (!rename(((GenericNode) e.Node).BackingPath, e.Label))
                    e.CancelEdit = true;
            }
            else e.CancelEdit = true;
            if (e.Node is ProjectNode && !e.Node.Text.Contains(languageDisplayName))
                e.Node.Text += " " + languageDisplayName;
            IsEditingLabel = false;
        }

        /// <summary>
        /// Don't allow non-generic nodes to be selected
        /// </summary>
        void tree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!(e.Node is GenericNode)) e.Cancel = true;
            IsEditingLabel = false;
        }

        /// <summary>
        /// Customize the context menu
        /// </summary>
        void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (Tree.SelectedNodes.Count == 0) return;
            Project project = Tree.ProjectOf(Tree.SelectedNodes[0] as GenericNode);
            Menu.Configure(Tree.SelectedNodes, project);
            // notify other plugins of tree nodes selection - ourben@fdc
            DataEvent de = new DataEvent(EventType.Command, ProjectManagerEvents.TreeSelectionChanged, Tree.SelectedNodes);
            EventManager.DispatchEvent(Tree, de); 
        }

        /// <summary>
        /// A new file was created and we want it to be selected after
        /// the filesystemwatcher finds it and makes us refresh
        /// </summary>
        void NewFileCreated(string path)
        {
            Tree.PathToSelect = path;
            WatchParentOf(path);
        }

        /// <summary>
        /// 
        /// </summary>
        void RenameNode(object sender, EventArgs e)
        {
            if (Tree.SelectedNode is ProjectNode)
            {
                string label = Tree.SelectedNode.Text;
                int index = label.IndexOfOrdinal("(" + project.LanguageDisplayName + ")");
                if (index != -1) Tree.SelectedNode.Text = label.Remove(index).Trim();
            }
            Tree.ForceLabelEdit();
        }

        /// <summary>
        /// The project has changed, so refresh the tree
        /// </summary>
        void ProjectModified(string[] paths)
        {
            Tree.RefreshTree(paths);
        }

        #endregion

    }

    /// <summary>
    ///  Event delegates of the class
    /// </summary>
    public delegate bool RenameEventHandler(string path, string newName);

}
