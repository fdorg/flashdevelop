using System;
using System.Collections;
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
using PluginCore.Helpers;
using PluginCore.Controls;

namespace ASCompletion
{
    public partial class ModelsExplorer : DockPanelControl
    {
        #region Docking

        static readonly string panelGuid = "078c7c1a-c667-4f54-9e47-d45c0e835c4f";
        static DockContent panelCtrl;

        public static void CreatePanel()
        {
            Image panelIcon = PluginBase.MainForm.FindImage("202");
            panelCtrl = PluginBase.MainForm.CreateDockablePanel(Instance, panelGuid, panelIcon, DockState.Hidden);
            panelCtrl.VisibleState = DockState.Float;
        }
        public static void Open()
        {
            panelCtrl.Show();
            Instance.filterTextBox.Focus();
        }

        #endregion

        #region Custom structures/nodes

        class ClasspathTreeNode : TreeNode
        {
            public readonly string Path;
            public ClasspathTreeNode(string path, int count)
                : base(path + " (" + count + ")", PluginUI.ICON_FOLDER_CLOSED, PluginUI.ICON_FOLDER_OPEN)
                => Path = path;
        }

        class ExploreTreeNode : TreeNode
        {
        }

        class PackageTreeNode : TreeNode
        {
            public PackageTreeNode(string name) : base(name, PluginUI.ICON_PACKAGE, PluginUI.ICON_PACKAGE) { }
        }

        class TypeTreeNode : TreeNode
        {
            public TypeTreeNode(MemberModel model) : base(model.FullName)
            {
                Tag = $"{model.InFile.FileName}@{model.Name}";
                if ((model.Flags & FlagType.Interface) > 0) ImageIndex = SelectedImageIndex = PluginUI.ICON_INTERFACE;
                else ImageIndex = SelectedImageIndex = PluginUI.ICON_TYPE;
            }
        }

        struct ResolvedPath
        {
            public string package;
            public PathModel model;
        }

        class NodesComparer : IComparer, IComparer<TreeNode>
        {
            public int Compare(object x, object y) => Compare((TreeNode) x, (TreeNode) y);

            public int Compare(TreeNode x, TreeNode y)
            {
                return x.ImageIndex == y.ImageIndex
                    ? string.Compare(x.Text, y.Text)
                    : x.ImageIndex - y.ImageIndex;
            }
        }

        #endregion

        #region Initialization

        public static bool HasFocus => instance != null && instance.filterTextBox.Focused;

        public static ModelsExplorer Instance => instance ?? new ModelsExplorer();

        static ModelsExplorer instance;
        IASContext current;
        Dictionary<string, TreeNodeCollection> packages;
        TreeNodeCollection rootNodes;
        List<TreeNode> allTypes;
        int typeIndex;
        TreeNode lastMatch;

        public ModelsExplorer()
        {
            instance = this;
            InitializeComponent();
            InitializeLocalization();
            toolStrip.Font = PluginBase.Settings.DefaultFont;
            toolStrip.Renderer = new DockPanelStripRenderer();
            outlineContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            outlineContextMenuStrip.Renderer = new DockPanelStripRenderer();
            searchButton.Image = PluginBase.MainForm.FindImage("251");
            refreshButton.Image = PluginBase.MainForm.FindImage("24");
            rebuildButton.Image = PluginBase.MainForm.FindImage("153");
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            ScrollBarEx.Attach(outlineTreeView);
        }

        void outlineContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var node = outlineTreeView.GetNodeAt(outlineTreeView.PointToClient(MousePosition));
            if (node is null) e.Cancel = true;
            else
            {
                outlineTreeView.SelectedNode = node;
                switch (node)
                {
                    case ClasspathTreeNode:
                    case PackageTreeNode:
                        editToolStripMenuItem.Visible = false;
                        exploreToolStripMenuItem.Visible = true;
                        convertToolStripMenuItem.Visible = true;
                        break;
                    case TypeTreeNode:
                        editToolStripMenuItem.Visible = true;
                        exploreToolStripMenuItem.Visible = false;
                        convertToolStripMenuItem.Visible = true;
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        void InitializeLocalization()
        {
            filterLabel.Text = TextHelper.GetString("Info.FindType");
            searchButton.ToolTipText = TextHelper.GetString("ToolTip.Search");
            refreshButton.ToolTipText = TextHelper.GetString("ToolTip.Refresh");
            rebuildButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.RebuildClasspathCache");
            editToolStripMenuItem.Text = TextHelper.GetString("Label.ModelEdit");
            exploreToolStripMenuItem.Text = TextHelper.GetString("Label.ModelExplore");
            convertToolStripMenuItem.Text = TextHelper.GetString("Label.ModelConvert");
            Text = TextHelper.GetString("Title.ModelExplorer");
        }

        #endregion

        #region Nodes population

        void DetectContext()
        {
            current = ASContext.Context;
            if (PluginBase.CurrentProject != null)
                current = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
        }

        /// <summary>
        /// Display types in each classpath
        /// </summary>
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
                if (current is null || current.Classpath.IsNullOrEmpty()) return;

                foreach (PathModel path in current.Classpath)
                {
                    var node = new ClasspathTreeNode(path.Path, path.FilesCount);
                    outlineTreeView.Nodes.Add(node);
                    rootNodes = node.Nodes;

                    packages = new Dictionary<string, TreeNodeCollection>();
                    path.ForeachFile(model =>
                    {
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
        TreeNodeCollection FindPackage(string package)
        {
            if (package.IsNullOrEmpty()) return rootNodes;
            if (packages.ContainsKey(package)) return packages[package];
            var p = package.LastIndexOf('.');
            var newPackage = p < 0 ? package : package.Substring(p+1);
            var parentPackage = p < 0 ? "" : package.Substring(0, p);
            var nodes = FindPackage(parentPackage);
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
        void AddModel(TreeNodeCollection nodes, FileModel model)
        {
            if (model.Members is not null) PluginUI.AddMembers(nodes, model.Members);
            if (model.Classes is null) return;
            foreach (var aClass in model.Classes)
                if (aClass.IndexType is null)
                {
                    var node = new TypeTreeNode(aClass);
                    AddExplore(node);
                    allTypes.Add(node);
                    nodes.Add(node);
                }
        }

        /// <summary>
        /// Create a subnode to a type node to be populated later
        /// </summary>
        /// <param name="node"></param>
        static void AddExplore(TreeNode node) => node.Nodes.Add(new ExploreTreeNode());

        /// <summary>
        /// Describe types on user selection
        /// </summary>
        void outlineTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count != 1 || e.Node.Nodes[0] is not ExploreTreeNode) return;
            outlineTreeView.BeginUpdate();
            try
            {
                e.Node.Nodes.Clear();
                var @class = ResolveClass(e.Node);
                if (@class.IsVoid()) return;
                PluginUI.AddMembers(e.Node.Nodes, SelectMembers(@class.Members, FlagType.Variable));
                PluginUI.AddMembers(e.Node.Nodes, SelectMembers(@class.Members, FlagType.Getter | FlagType.Setter));
                PluginUI.AddMembers(e.Node.Nodes, SelectMembers(@class.Members, FlagType.Function));
            }
            finally
            {
                outlineTreeView.EndUpdate();
            }
        }

        static MemberList SelectMembers(MemberList list, FlagType mask)
        {
            var result = list.MultipleSearch(mask, Visibility.Public | Visibility.Internal);
            result.Sort();
            return result;
        }

        void outlineTreeView_Click(object sender, EventArgs e)
        {
            if (outlineTreeView.SelectedNode is null) return;
            switch (outlineTreeView.SelectedNode)
            {
                case TypeTreeNode node:
                {
                    var filename = ((string) node.Tag).Split('@')[0];
                    var model = OpenFile(filename);
                    if (model is null) return;
                    var theClass = model.GetClassByName(node.Text);
                    if (theClass.IsVoid()) return;
                    var line = theClass.LineFrom;
                    var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                    if (sci != null && !theClass.IsVoid() && line > 0 && line < sci.LineCount)
                        sci.GotoLineIndent(line);
                    break;
                }
                case MemberTreeNode _ when outlineTreeView.SelectedNode.Parent is TypeTreeNode node:
                {
                    var filename = ((string) node.Tag).Split('@')[0];
                    var model = OpenFile(filename);
                    if (model is null) return;
                    var theClass = model.GetClassByName(node.Text);
                    var memberName = ((string) outlineTreeView.SelectedNode.Tag).Split('@')[0];
                    var member = theClass.Members.Search(memberName);
                    if (member is null) return;
                    var line = member.LineFrom;
                    var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                    if (sci != null && line > 0 && line < sci.LineCount)
                        sci.GotoLineIndent(line);
                    break;
                }
            }
        }

        /// <summary>
        /// Open a file in the classpath (physical or virtual) or the current context
        /// </summary>
        public FileModel OpenFile(string filename)
        {
            if (current is null) DetectContext();
            return OpenFile(filename, current);
        }

        /// <summary>
        /// Open a file in the classpath (physical or virtual) or a specific context
        /// </summary>
        public FileModel OpenFile(string filename, IASContext context)
        {
            if (context?.Classpath is null)
                return null;
            FileModel model = null;
            foreach (PathModel aPath in context.Classpath)
                if (aPath.TryGetFile(filename, out model))
                {
                    break;
                }
            if (model != null)
            {
                if (File.Exists(model.FileName))
                    PluginBase.MainForm.OpenEditableDocument(model.FileName, false);
                else
                {
                    ASComplete.OpenVirtualFile(model);
                    model = ASContext.Context.CurrentModel;
                }
            }
            return model;
        }

        ClassModel ResolveClass(TreeNode node)
        {
            if (!(node is TypeTreeNode) || node.Tag is null) return ClassModel.VoidClass;
            var resolved = ResolvePath(node);
            if (resolved.model is null) return ClassModel.VoidClass;
            var info = ((string) node.Tag).Split('@');
            var model = resolved.model.GetFile(info[0]);
            return model.GetClassByName(info[1]);
        }

        #endregion

        #region UI events

        void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Start();
        }

        void filterTextBox_Leave(object sender, EventArgs e) => SetMatch(null);

        void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Stop();
            typeIndex = 0;
            FindNextMatch(filterTextBox.Text);
        }

        void FindPrevMatch(string search)
        {
            if (!string.IsNullOrEmpty(search) && allTypes != null)
            {
                typeIndex--;
                if (typeIndex <= 0) typeIndex = allTypes.Count;
                while (typeIndex > 0)
                {
                    var node = allTypes[--typeIndex];
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

        void FindNextMatch(string search)
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

        void SetMatch(TreeNode node)
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
            switch (keys)
            {
                case Keys.F3:
                    FindNextMatch(filterTextBox.Text);
                    return true;
                case Keys.Shift | Keys.F3:
                    FindPrevMatch(filterTextBox.Text);
                    return true;
                case Keys.F5:
                case Keys.Control | Keys.J:
                    UpdateTree();
                    return true;
                case Keys.Escape:
                    if (panelCtrl.DockState == DockState.Float) panelCtrl.Hide();
                    if (PluginBase.MainForm.CurrentDocument is {SciControl: { } sci})
                        sci.Focus();
                    break;
            }
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    OnShortcut(keyData);
                    return true;
                default: return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        void ModelsExplorer_KeyDown(object sender, KeyEventArgs e)
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

        void SearchButton_Click(object sender, EventArgs e) => FindNextMatch(filterTextBox.Text);

        void RefreshButton_Click(object sender, EventArgs e) => UpdateTree();

        void RebuildButton_Click(object sender, EventArgs e)
        {
            outlineTreeView.Nodes.Clear();
            ASContext.RebuildClasspath();
        }
        #endregion

        #region Context menu

        void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outlineTreeView.SelectedNode is {} node && GetPathFromNode(node) is {} path)
                PluginBase.MainForm.CallCommand("RunProcess", $"explorer.exe;/e,\"{path}\"");
        }

        void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outlineTreeView.SelectedNode is not null) outlineTreeView_Click(null, null);
        }

        void ConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = outlineTreeView.SelectedNode;
            if (node is null || current?.Classpath is null) return;

            ResolvedPath resolved = ResolvePath(node);
            string package = resolved.package;
            PathModel thePath = resolved.model;
            if (thePath is null) return;

            if (node is TypeTreeNode)
            {
                string filename = ((string) node.Tag).Split('@')[0];
                FileModel theModel = thePath.GetFile(filename);
                if (theModel is null) return;

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

                        thePath.ForeachFile(aModel =>
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

        ResolvedPath ResolvePath(TreeNode node)
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
            if (cp is null) return result;

            string path = (cp as ClasspathTreeNode).Path;
            foreach (PathModel aPath in current.Classpath)
                if (aPath.Path == path)
                {
                    result.model = aPath;
                    break;
                }
            return result;
        }

        static void WriteIntrinsic(FileModel theModel, string filename)
        {
            if (filename.EndsWithOrdinal("$.as")) filename = filename.Replace("$.as", ".as"); // SWC virtual models
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, theModel.GenerateIntrinsic(false), Encoding.UTF8);
        }

        static string GetPathFromNode(TreeNode node)
        {
            string path = null;
            switch (node)
            {
                case ClasspathTreeNode treeNode:
                    path = treeNode.Path;
                    break;
                case PackageTreeNode:
                    path = node.Text;
                    node = node.Parent;
                    while (node != null && !(node is ClasspathTreeNode))
                    {
                        path = Path.Combine(node.Text, path);
                        node = node.Parent;
                    }
                    if (node != null) path = Path.Combine(((ClasspathTreeNode) node).Path, path);
                    break;
                case TypeTreeNode:
                    return ((string) node.Tag).Split('@')[0];
            }

            while (path!.Length > 2 && !Directory.Exists(path)) path = Path.GetDirectoryName(path);
            return path;
        }

        #endregion

    }
}