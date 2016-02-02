using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Localization;
using PluginCore;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Managers;
using ASCompletion.Completion;
using System.Collections;
using PluginCore.Helpers;

namespace ASCompletion
{
    public partial class ModelsExplorer : DockPanelControl
    {
        #region Docking

        static private string panelGuid = "078c7c1a-c667-4f54-9e47-d45c0e835c4f";
        static private DockContent panelCtrl;

        static public void CreatePanel()
        {
            Image panelIcon = PluginBase.MainForm.FindImage("202");
            panelCtrl = PluginBase.MainForm.CreateDockablePanel(Instance, panelGuid, panelIcon, DockState.Hidden);
            panelCtrl.VisibleState = DockState.Float;
        }
        static public void Open()
        {
            panelCtrl.Show();
            Instance.filterTextBox.Focus();
        }

        #endregion

        #region Custom structures/nodes

        class ClasspathTreeNode : TreeNode
        {
            public string Path;
            public ClasspathTreeNode(string path, int count)
                : base(path + " (" + count + ")", PluginUI.ICON_FOLDER_CLOSED, PluginUI.ICON_FOLDER_OPEN) 
            {
                Path = path;
            }
        }

        class ExploreTreeNode : TreeNode
        {
            public ExploreTreeNode() : base() { }
        }

        class PackageTreeNode : TreeNode
        {
            public PackageTreeNode(string name) : base(name, PluginUI.ICON_PACKAGE, PluginUI.ICON_PACKAGE) { }
        }

        class TypeTreeNode : TreeNode
        {
            public TypeTreeNode(ClassModel model) : base(model.FullName)
            {
                Tag = model.InFile.FileName + "@" + model.Name;
                if ((model.Flags & FlagType.Interface) > 0) ImageIndex = SelectedImageIndex = PluginUI.ICON_INTERFACE;
                else ImageIndex = SelectedImageIndex = PluginUI.ICON_TYPE;
            }
        }

        struct ResolvedPath
        {
            public string package;
            public PathModel model;
        }

        class NodesComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode a = (TreeNode)x;
                TreeNode b = (TreeNode)y;
                if (a.ImageIndex == b.ImageIndex)
                    return string.Compare(a.Text, b.Text);
                else return a.ImageIndex - b.ImageIndex;
            }
        }

        #endregion

        #region Initialization

        static public bool HasFocus
        {
            get { return instance != null && instance.filterTextBox.Focused; }
        }

        static public ModelsExplorer Instance
        {
            get { return instance ?? new ModelsExplorer(); }
        }

        static private ModelsExplorer instance;
        private IASContext current;
        private Dictionary<string, TreeNodeCollection> packages;
        private TreeNodeCollection rootNodes;
        private List<TreeNode> allTypes;
        private int typeIndex;
        private TreeNode lastMatch;

        public ModelsExplorer()
        {
            instance = this;
            InitializeComponent();
            InitializeLocalization();
            this.toolStrip.Font = PluginBase.Settings.DefaultFont;
            this.toolStrip.Renderer = new DockPanelStripRenderer();
            this.outlineContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            this.outlineContextMenuStrip.Renderer = new DockPanelStripRenderer();
            searchButton.Image = PluginBase.MainForm.FindImage("251");
            refreshButton.Image = PluginBase.MainForm.FindImage("24");
            rebuildButton.Image = PluginBase.MainForm.FindImage("153");
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
        }

        private void outlineContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = outlineTreeView.GetNodeAt(outlineTreeView.PointToClient(Control.MousePosition));
            if (node == null) e.Cancel = true;
            else
            {
                outlineTreeView.SelectedNode = node;
                if (node is ClasspathTreeNode || node is PackageTreeNode)
                {
                    editToolStripMenuItem.Visible = false;
                    exploreToolStripMenuItem.Visible = true;
                    convertToolStripMenuItem.Visible = true;
                }
                else if (node is TypeTreeNode)
                {
                    editToolStripMenuItem.Visible = true;
                    exploreToolStripMenuItem.Visible = false;
                    convertToolStripMenuItem.Visible = true;
                }
                else e.Cancel = true;
            }
        }

        private void InitializeLocalization()
        {
            this.filterLabel.Text = TextHelper.GetString("Info.FindType");
            this.searchButton.ToolTipText = TextHelper.GetString("ToolTip.Search");
            this.refreshButton.ToolTipText = TextHelper.GetString("ToolTip.Refresh");
            this.rebuildButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.RebuildClasspathCache");
            this.editToolStripMenuItem.Text = TextHelper.GetString("Label.ModelEdit");
            this.exploreToolStripMenuItem.Text = TextHelper.GetString("Label.ModelExplore");
            this.convertToolStripMenuItem.Text = TextHelper.GetString("Label.ModelConvert");
            this.Text = TextHelper.GetString("Title.ModelExplorer");
        }

        #endregion

        #region Nodes population
        private void DetectContext()
        {
            current = ASContext.Context;
            if (PluginBase.CurrentProject != null)
                current = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
        }

        /// <summary>
        /// Display types in each classpath
        /// </summary>
        /// <param name="context">Language context to explore</param>
        public void UpdateTree()
        {
            DetectContext();
            filterTextBox.Text = "";

            outlineTreeView.BeginUpdate();
            try
            {
                outlineTreeView.TreeViewNodeSorter = null;
                outlineTreeView.Nodes.Clear();
                outlineTreeView.ImageList = ASContext.Panel.TreeIcons;

                allTypes = new List<TreeNode>();
                if (current == null || current.Classpath == null || current.Classpath.Count == 0) return;

                foreach (PathModel path in current.Classpath)
                {
                    ClasspathTreeNode node = new ClasspathTreeNode(path.Path, path.FilesCount);
                    outlineTreeView.Nodes.Add(node);
                    rootNodes = node.Nodes;

                    packages = new Dictionary<string, TreeNodeCollection>();
                    path.ForeachFile((model) => {
                        AddModel(FindPackage(model.Package), model);
                        return true;
                    });
                }
            }
            finally
            {
                outlineTreeView.TreeViewNodeSorter = new NodesComparer();
                outlineTreeView.EndUpdate();
                filterTextBox.Focus();
            }
        }

        /// <summary>
        /// Find or create the package where to create the model nodes
        /// </summary>
        /// <param name="package">Package path</param>
        /// <returns>Nodes collection</returns>
        private TreeNodeCollection FindPackage(string package)
        {
            if (package == "") return rootNodes;
            else if (packages.ContainsKey(package)) return packages[package];

            int p = package.LastIndexOf('.');
            string newPackage = (p < 0) ? package : package.Substring(p+1);
            string parentPackage = (p < 0) ? "" : package.Substring(0, p);

            TreeNodeCollection nodes = FindPackage(parentPackage);
            TreeNode node = new PackageTreeNode(newPackage);
            node.Expand();
            nodes.Add(node);
            packages[package] = node.Nodes;
            return node.Nodes;
        }

        /// <summary>
        /// Create node for a type (class or interface)
        /// - defer subnodes population to user selection
        /// </summary>
        /// <param name="nodes">In package</param>
        /// <param name="model">Model information</param>
        private void AddModel(TreeNodeCollection nodes, FileModel model)
        {
            if (model.Members != null)
                PluginUI.AddMembers(nodes, model.Members);

            if (model.Classes != null)
                foreach (ClassModel aClass in model.Classes) if (aClass.IndexType == null)
                {
                    TypeTreeNode node = new TypeTreeNode(aClass);
                    AddExplore(node);
                    allTypes.Add(node);
                    nodes.Add(node);
                }

            
        }

        /// <summary>
        /// Create a subnode to a type node to be populated later
        /// </summary>
        /// <param name="node"></param>
        private void AddExplore(TypeTreeNode node)
        {
            node.Nodes.Add(new ExploreTreeNode());
        }

        /// <summary>
        /// Describe types on user selection
        /// </summary>
        /// <param name="node"></param>
        private void outlineTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0] is ExploreTreeNode)
            {
                outlineTreeView.BeginUpdate();
                try
                {
                    e.Node.Nodes.Clear();
                    ClassModel theClass = ResolveClass(e.Node);
                    if (theClass.IsVoid())
                        return;
                    PluginUI.AddMembers(e.Node.Nodes, SelectMembers(theClass.Members, FlagType.Variable));
                    PluginUI.AddMembers(e.Node.Nodes, SelectMembers(theClass.Members, FlagType.Getter|FlagType.Setter));
                    PluginUI.AddMembers(e.Node.Nodes, SelectMembers(theClass.Members, FlagType.Function));
                }
                finally
                {
                    outlineTreeView.EndUpdate();
                }
            }
        }

        private MemberList SelectMembers(MemberList list, FlagType mask)
        {
            MemberList filtered = new MemberList();
            Visibility acc = Visibility.Public | Visibility.Internal;
            //MemberModel lastAdded = null;
            foreach (MemberModel item in list)
                if ((item.Access & acc) > 0 && (item.Flags & mask) > 0)
                {
                    //if (lastAdded != null && lastAdded.Name == item.Name) continue;
                    //lastAdded = item;
                    filtered.Add(item);
                }
            filtered.Sort();
            return filtered;
        }

        private void outlineTreeView_Click(object sender, EventArgs e)
        {
            if (outlineTreeView.SelectedNode == null)
                return;
            TreeNode node = outlineTreeView.SelectedNode;
            TypeTreeNode tnode;
            if (node is TypeTreeNode)
            {
                tnode = node as TypeTreeNode;
                string filename = (tnode.Tag as string).Split('@')[0];

                FileModel model = OpenFile(filename);
                if (model != null)
                {
                    ClassModel theClass = model.GetClassByName(tnode.Text);
                    if (!theClass.IsVoid())
                    {
                        int line = theClass.LineFrom;
                        ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                        if (sci != null && !theClass.IsVoid() && line > 0 && line < sci.LineCount)
                            sci.GotoLineIndent(line);
                    }
                }
            }
            else if (node is MemberTreeNode && node.Parent is TypeTreeNode)
            {
                tnode = node.Parent as TypeTreeNode;
                string filename = (tnode.Tag as string).Split('@')[0];

                FileModel model = OpenFile(filename);
                if (model != null)
                {
                    ClassModel theClass = model.GetClassByName(tnode.Text);
                    string memberName = (node.Tag as String).Split('@')[0];
                    MemberModel member = theClass.Members.Search(memberName, 0, 0);
                    if (member == null) return;
                    int line = member.LineFrom;
                    ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    if (sci != null && line > 0 && line < sci.LineCount)
                        sci.GotoLineIndent(line);
                }
            }
        }

        /// <summary>
        /// Open a file in the classpath (physical or virtual) or the current context
        /// </summary>
        public FileModel OpenFile(string filename)
        {
            if (current == null) DetectContext();
            return OpenFile(filename, current);
        }

        /// <summary>
        /// Open a file in the classpath (physical or virtual) or a specific context
        /// </summary>
        public FileModel OpenFile(string filename, IASContext context)
        {
            if (context == null || context.Classpath == null)
                return null;
            FileModel model = null;
            foreach (PathModel aPath in context.Classpath)
                if (aPath.HasFile(filename))
                {
                    model = aPath.GetFile(filename);
                    break;
                }
            if (model != null)
            {
                if (File.Exists(model.FileName))
                    ASContext.MainForm.OpenEditableDocument(model.FileName, false);
                else
                {
                    ASComplete.OpenVirtualFile(model);
                    model = ASContext.Context.CurrentModel;
                }
            }
            return model;
        }

        private ClassModel ResolveClass(TreeNode node)
        {
            if (!(node is TypeTreeNode) || node.Tag == null)
                return ClassModel.VoidClass;

            ResolvedPath resolved = ResolvePath(node);
            if (resolved.model == null) return ClassModel.VoidClass;

            string[] info = (node.Tag as string).Split('@');
            FileModel model = resolved.model.GetFile(info[0]);
            return model.GetClassByName(info[1]);
        }

        private int ResolveMemberLine(TreeNode node)
        {
            if (!(node is MemberTreeNode) || node.Tag == null)
                return 0;
            string[] info = (node.Tag as string).Split('@');
            int line;
            if (int.TryParse(info[1], out line)) return line;
            else return 0;
        }

        #endregion

        #region UI events
        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Start();
        }

        private void filterTextBox_Leave(object sender, EventArgs e)
        {
            SetMatch(null);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Stop();
            typeIndex = 0;
            FindNextMatch(filterTextBox.Text);
        }

        private void FindPrevMatch(string search)
        {
            if (!string.IsNullOrEmpty(search) && allTypes != null)
            {
                typeIndex--;
                if (typeIndex <= 0) typeIndex = allTypes.Count;
                while (typeIndex > 0)
                {
                    TreeNode node = allTypes[--typeIndex];
                    if (node.Text.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SetMatch(node);
                        typeIndex++;
                        return;
                    }
                }
            }
            typeIndex = 0;
        }

        private void FindNextMatch(string search)
        {
            if (!string.IsNullOrEmpty(search) && allTypes != null)
            {
                while (typeIndex < allTypes.Count)
                {
                    TreeNode node = allTypes[typeIndex++];
                    if (node.Text.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SetMatch(node);
                        return;
                    }
                }
            }
            typeIndex = 0;
        }

        private void SetMatch(TreeNode node)
        {
            if (lastMatch != null)
            {
                lastMatch.BackColor = PluginBase.MainForm.GetThemeColor("TreeView.BackColor", SystemColors.Window);
                lastMatch.ForeColor = PluginBase.MainForm.GetThemeColor("TreeView.ForeColor", SystemColors.WindowText);

            }
            lastMatch = node;
            if (lastMatch != null)
            {
                lastMatch.BackColor = PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);
                lastMatch.ForeColor = PluginBase.MainForm.GetThemeColor("TreeView.HighlightText", SystemColors.HighlightText);
                outlineTreeView.SelectedNode = node;
            }
        }

        internal bool OnShortcut(Keys keys)
        {
            if (keys == Keys.F3)
            {
                FindNextMatch(filterTextBox.Text);
                return true;
            }
            else if (keys == (Keys.Shift | Keys.F3))
            {
                FindPrevMatch(filterTextBox.Text);
                return true;
            }
            else if (keys == Keys.F5 || keys == (Keys.Control | Keys.J))
            {
                UpdateTree();
                return true;
            }
            else if (keys == Keys.Escape)
            {
                if (panelCtrl.DockState == DockState.Float) panelCtrl.Hide();
                if (PluginBase.MainForm.CurrentDocument.IsEditable)
                    PluginBase.MainForm.CurrentDocument.SciControl.Focus();
            }
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                OnShortcut(keyData);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ModelsExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (outlineTreeView.SelectedNode is TypeTreeNode)
                {
                    if (panelCtrl.DockState == DockState.Float) panelCtrl.Hide();
                    outlineTreeView_Click(null, null);
                }
            }
            else OnShortcut(e.KeyData);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            FindNextMatch(filterTextBox.Text);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void RebuildButton_Click(object sender, EventArgs e)
        {
            outlineTreeView.Nodes.Clear();
            ASContext.RebuildClasspath();
        }
        #endregion

        #region Context menu

        private void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = outlineTreeView.SelectedNode;
            if (node == null) return;

            string path = GetPathFromNode(node);
            if (path != null)
                PluginBase.MainForm.CallCommand("RunProcess", String.Format("explorer.exe;/e,\"{0}\"", path));
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = outlineTreeView.SelectedNode;
            if (node == null) return;
            outlineTreeView_Click(null, null);
        }

        private void ConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = outlineTreeView.SelectedNode;
            if (node == null || current == null || current.Classpath == null) return;

            ResolvedPath resolved = ResolvePath(node);
            string package = resolved.package;
            PathModel thePath = resolved.model;
            if (thePath == null) 
                return;

            if (node is TypeTreeNode)
            {
                string filename = (node.Tag as string).Split('@')[0];
                FileModel theModel = thePath.GetFile(filename);
                if (theModel == null)
                    return;

                saveFileDialog.Title = TextHelper.GetString("Title.SaveIntrinsicAs");
                saveFileDialog.FileName = Path.GetFileName(filename);
                saveFileDialog.DefaultExt = Path.GetExtension(filename);
                if (PluginBase.CurrentProject != null)
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        WriteIntrinsic(theModel, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }
            }
            else
            {
                folderBrowserDialog.ShowNewFolderButton = true;
                folderBrowserDialog.UseDescriptionForTitle = true;
                folderBrowserDialog.Description = TextHelper.GetString("Title.SelectIntrinsicTargetFolder");
                if (PluginBase.CurrentProject != null)
                    folderBrowserDialog.SelectedPath = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sourcePath = Path.Combine(thePath.Path, package.Replace('.', Path.DirectorySeparatorChar));
                        string targetPath = folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar;
                        string packagep = (package.Length > 0) ? package + "." : "";

                        thePath.ForeachFile((aModel) =>
                        {
                            if (aModel.Package == package || aModel.Package.StartsWithOrdinal(packagep))
                            {
                                if (aModel.FileName.StartsWithOrdinal(sourcePath))
                                    WriteIntrinsic(aModel, aModel.FileName.Replace(sourcePath, targetPath));
                            }
                            return true;
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }
            }
        }

        private ResolvedPath ResolvePath(TreeNode node)
        {
            ResolvedPath result = new ResolvedPath();
            TreeNode cp = node;
            string package = "";
            while (cp != null && !(cp is ClasspathTreeNode))
            {
                if (cp is PackageTreeNode) package = (package.Length > 0) ? cp.Text + "." + package : cp.Text;
                cp = cp.Parent;
            }
            result.package = package;
            if (cp == null) return result;

            string path = (cp as ClasspathTreeNode).Path;
            foreach (PathModel aPath in current.Classpath)
                if (aPath.Path == path)
                {
                    result.model = aPath;
                    break;
                }
            return result;
        }

        private void WriteIntrinsic(FileModel theModel, string filename)
        {
            if (filename.EndsWithOrdinal("$.as")) filename = filename.Replace("$.as", ".as"); // SWC virtual models
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, theModel.GenerateIntrinsic(false), Encoding.UTF8);
        }

        private string GetPathFromNode(TreeNode node)
        {
            string path = null;
            if (node is ClasspathTreeNode)
            {
                path = (node as ClasspathTreeNode).Path;
            }
            else if (node is PackageTreeNode)
            {
                path = node.Text;
                node = node.Parent;
                while (node != null && !(node is ClasspathTreeNode))
                {
                    path = Path.Combine(node.Text, path);
                    node = node.Parent;
                }
                if (node != null) path = Path.Combine((node as ClasspathTreeNode).Path, path);
            }
            else if (node is TypeTreeNode)
            {
                return (node.Tag as string).Split('@')[0];
            }

            while (path.Length > 2 && !Directory.Exists(path)) path = Path.GetDirectoryName(path);
            return path;
        }

        #endregion

    }
}