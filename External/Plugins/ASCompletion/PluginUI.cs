/*
 * ASCompletion panel
 * 
 * Contributed by IAP: 
 * - Quick search field allowing to highlight members in the tree (see: region Find declaration)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;

namespace ASCompletion
{
    /// <summary>
    /// Class treeview
    /// </summary>
    public class PluginUI : DockPanelControl
    {
        public const int ICON_FILE = 0;
        public const int ICON_FOLDER_CLOSED = 1;
        public const int ICON_FOLDER_OPEN = 2;
        public const int ICON_CHECK_SYNTAX = 3;
        public const int ICON_QUICK_BUILD = 4;
        public const int ICON_PACKAGE = 5;
        public const int ICON_INTERFACE = 6;
        public const int ICON_INTRINSIC_TYPE = 7;
        public const int ICON_TYPE = 8;
        public const int ICON_VAR = 9;
        public const int ICON_PROTECTED_VAR = 10;
        public const int ICON_PRIVATE_VAR = 11;
        public const int ICON_STATIC_VAR = 12;
        public const int ICON_STATIC_PROTECTED_VAR = 13;
        public const int ICON_STATIC_PRIVATE_VAR = 14;
        public const int ICON_CONST = 15;
        public const int ICON_PROTECTED_CONST = 16;
        public const int ICON_PRIVATE_CONST = 17;
        public const int ICON_STATIC_CONST = 18;
        public const int ICON_STATIC_PROTECTED_CONST = 19;
        public const int ICON_STATIC_PRIVATE_CONST = 20;
        public const int ICON_FUNCTION = 21;
        public const int ICON_PROTECTED_FUNCTION = 22;
        public const int ICON_PRIVATE_FUNCTION = 23;
        public const int ICON_STATIC_FUNCTION = 24;
        public const int ICON_STATIC_PROTECTED_FUNCTION = 25;
        public const int ICON_STATIC_PRIVATE_FUNCTION = 26;
        public const int ICON_PROPERTY = 27;
        public const int ICON_PROTECTED_PROPERTY = 28;
        public const int ICON_PRIVATE_PROPERTY = 29;
        public const int ICON_STATIC_PROPERTY = 30;
        public const int ICON_STATIC_PROTECTED_PROPERTY = 31;
        public const int ICON_STATIC_PRIVATE_PROPERTY = 32;
        public const int ICON_TEMPLATE = 33;
        public const int ICON_DECLARATION = 34;

        public int LookupCount => (lookupLocations != null) ? lookupLocations.Count : 0;
        public FixedTreeView OutlineTree => this.outlineTree;
        public ImageList TreeIcons => treeIcons;

        public ToolStripMenuItem LookupMenuItem;
        private System.ComponentModel.IContainer components;
        public ImageListManager treeIcons;
        private FixedTreeView outlineTree;
        private System.Timers.Timer tempoClick;

        private GeneralSettings settings;
        private string prevChecksum;
        private List<int> prevLines;
        private Stack<LookupLocation> lookupLocations;

        private TreeNode currentHighlight;
        private ToolStrip toolStrip;
        private ToolStripSpringTextBox findTextTxt;
        private ToolStripDropDownButton sortDropDown;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem noneItem;
        private ToolStripMenuItem sortedItem;
        private ToolStripMenuItem sortedByKindItem;
        private ToolStripMenuItem sortedSmartItem;
        private ToolStripMenuItem sortedGroupItem;
        private ToolStripButton clearButton;

        #region initialization
        public PluginUI(PluginMain plugin)
        {
            settings = plugin.PluginSettings;
            SuspendLayout();
            InitializeControls();
            InitializeTexts();
            ResumeLayout();
        }

        private void InitializeControls()
        {
            InitializeComponent();
            treeIcons.ColorDepth = ColorDepth.Depth32Bit;
            treeIcons.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            treeIcons.Initialize(TreeIcons_Populate);

            toolStrip.Renderer = new DockPanelStripRenderer();
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            toolStrip.Padding = new Padding(2, 1, 2, 2);
            sortDropDown.Font = PluginBase.Settings.DefaultFont;
            sortDropDown.Image = PluginBase.MainForm.FindImage("444");
            clearButton.Image = PluginBase.MainForm.FindImage("153");
            clearButton.Alignment = ToolStripItemAlignment.Right;
            clearButton.CheckOnClick = false;

            outlineTree = new FixedTreeView();
            outlineTree.BorderStyle = BorderStyle.None;
            outlineTree.ShowRootLines = false;
            outlineTree.Location = new Point(0, toolStrip.Bottom);
            outlineTree.Size = new Size(198, 300);
            outlineTree.Dock = DockStyle.Fill;
            outlineTree.ImageList = treeIcons;
            outlineTree.HotTracking = true;
            outlineTree.TabIndex = 1;
            outlineTree.NodeClicked += ClassTreeSelect;
            outlineTree.AfterSelect += outlineTree_AfterSelect;
            outlineTree.ShowNodeToolTips = true;
            Controls.Add(outlineTree);
            outlineTree.BringToFront();
            ScrollBarEx.Attach(outlineTree);
        }

        private void TreeIcons_Populate(object sender, EventArgs e)
        {
            treeIcons.Images.AddRange(new[]
            {
                GetImage("FilePlain.png"),
                GetImage("FolderClosed.png"),
                GetImage("FolderOpen.png"),
                GetImage("CheckAS.png"),
                GetImage("QuickBuild.png"),
                GetImage("Package.png"),
                GetImage("Interface.png"),
                GetImage("Intrinsic.png"),
                GetImage("Class.png"),
                GetImage("Variable.png"),
                GetImage("VariableProtected.png"),
                GetImage("VariablePrivate.png"),
                GetImage("VariableStatic.png"),
                GetImage("VariableStaticProtected.png"),
                GetImage("VariableStaticPrivate.png"),
                GetImage("Const.png"),
                GetImage("ConstProtected.png"),
                GetImage("ConstPrivate.png"),
                GetImage("Const.png"),
                GetImage("ConstProtected.png"),
                GetImage("ConstPrivate.png"),
                GetImage("Method.png"),
                GetImage("MethodProtected.png"),
                GetImage("MethodPrivate.png"),
                GetImage("MethodStatic.png"),
                GetImage("MethodStaticProtected.png"),
                GetImage("MethodStaticPrivate.png"),
                GetImage("Property.png"),
                GetImage("PropertyProtected.png"),
                GetImage("PropertyPrivate.png"),
                GetImage("PropertyStatic.png"),
                GetImage("PropertyStaticProtected.png"),
                GetImage("PropertyStaticPrivate.png"),
                GetImage("Template.png"),
                GetImage("Declaration.png")
            });
        }

        public static Image GetImage(String name)
        {
            return PluginBase.MainForm.ImageSetAdjust(Image.FromStream(GetStream(name)));
        }

        public static System.IO.Stream GetStream(String name)
        {
            String prefix = "ASCompletion.Icons.";
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(prefix + name);
        }

        private void InitializeTexts()
        {
            this.noneItem.Text = TextHelper.GetString("Outline.SortNone");
            this.sortedItem.Text = TextHelper.GetString("Outline.SortDefault");
            this.sortedByKindItem.Text = TextHelper.GetString("Outline.SortedByKind");
            this.sortedSmartItem.Text = TextHelper.GetString("Outline.SortedSmart");
            this.sortedGroupItem.Text = TextHelper.GetString("Outline.SortedGroup");
            clearButton.Text = TextHelper.GetString("Outline.ClearSearchText");
            sortDropDown.Text = TextHelper.GetString("Outline.SortingMode");
            searchInvitation = TextHelper.GetString("Outline.Search");
            FindProcTxtLeave(null, null);
        }

        void outlineTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // notify other plugins of tree nodes selection
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.TreeSelectionChanged", e.Node);
            EventManager.DispatchEvent(sender, de); 
        }

        public System.Drawing.Image GetIcon(int index)
        {
            if (treeIcons.Images.Count > 0)
                return treeIcons.Images[Math.Min(index, treeIcons.Images.Count)];
            else return null;
        }

        public static int GetIcon(FlagType flag, Visibility access)
        {
            int rst = 0;
            bool isStatic = (flag & FlagType.Static) > 0;

            if ((flag & FlagType.Constant) > 0)
            {
                rst = ((access & Visibility.Private) > 0) ? ICON_PRIVATE_CONST :
                    ((access & Visibility.Protected) > 0) ? ICON_PROTECTED_CONST : ICON_CONST;
                if (isStatic) rst += 3;
            }
            else if ((flag & FlagType.Variable) > 0)
            {
                rst = ((access & Visibility.Private) > 0) ? ICON_PRIVATE_VAR :
                    ((access & Visibility.Protected) > 0) ? ICON_PROTECTED_VAR : ICON_VAR;
                if (isStatic) rst += 3;
            }
            else if ((flag & (FlagType.Getter | FlagType.Setter)) > 0)
            {
                rst = ((access & Visibility.Private) > 0) ? ICON_PRIVATE_PROPERTY :
                    ((access & Visibility.Protected) > 0) ? ICON_PROTECTED_PROPERTY : ICON_PROPERTY;
                if (isStatic) rst += 3;
            }
            else if ((flag & FlagType.Function) > 0)
            {
                rst = ((access & Visibility.Private) > 0) ? ICON_PRIVATE_FUNCTION :
                    ((access & Visibility.Protected) > 0) ? ICON_PROTECTED_FUNCTION : ICON_FUNCTION;
                if (isStatic) rst += 3;
            }
            else if ((flag & (FlagType.Interface | FlagType.TypeDef)) > 0)
            {
                rst = ICON_INTERFACE;
            }
            else if (flag == FlagType.Package)
                rst = ICON_PACKAGE;
            else if (flag == FlagType.Declaration)
                rst = ICON_DECLARATION;
            else if (flag == FlagType.Template)
                rst = ICON_TEMPLATE;
            else
            {
                rst = ((flag & FlagType.Intrinsic) > 0) ? ICON_INTRINSIC_TYPE :
                    ((flag & FlagType.Interface) > 0) ? ICON_INTERFACE : ICON_TYPE;
            }
            return rst;
        }

        #endregion

        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginUI));
            this.treeIcons = new ImageListManager(this.components);
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.sortDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.noneItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortedByKindItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortedSmartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortedGroupItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.findTextTxt = new System.Windows.Forms.ToolStripSpringTextBox();
            this.clearButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeIcons
            // 
            this.treeIcons.TransparentColor = System.Drawing.Color.Transparent;            
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortDropDown,
            this.toolStripSeparator1,
            this.findTextTxt,
            this.clearButton});
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(266, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // sortDropDown
            // 
            this.sortDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneItem,
            this.sortedItem,
            this.sortedByKindItem,
            this.sortedSmartItem,
            this.sortedGroupItem});
            this.sortDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortDropDown.Name = "sortDropDown";
            this.sortDropDown.Size = new System.Drawing.Size(13, 22);
            this.sortDropDown.Text = "";
            this.sortDropDown.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SortDropDown_DropDownItemClicked);
            this.sortDropDown.DropDownOpening += new System.EventHandler(this.SortDropDown_DropDownOpening);
            this.sortDropDown.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // noneItem
            // 
            this.noneItem.Name = "noneItem";
            this.noneItem.Size = new System.Drawing.Size(152, 22);
            this.noneItem.Text = "None";
            // 
            // sortedItem
            // 
            this.sortedItem.Name = "sortedItem";
            this.sortedItem.Size = new System.Drawing.Size(152, 22);
            this.sortedItem.Text = "Sorted";
            // 
            // sortedByKindItem
            // 
            this.sortedByKindItem.Name = "sortedByKindItem";
            this.sortedByKindItem.Size = new System.Drawing.Size(152, 22);
            this.sortedByKindItem.Text = "SortedByKind";
            // 
            // sortedSmartItem
            // 
            this.sortedSmartItem.Name = "sortedSmartItem";
            this.sortedSmartItem.Size = new System.Drawing.Size(152, 22);
            this.sortedSmartItem.Text = "SortedSmart";
            // 
            // sortedGroupItem
            // 
            this.sortedGroupItem.Name = "sortedGroupItem";
            this.sortedGroupItem.Size = new System.Drawing.Size(152, 22);
            this.sortedGroupItem.Text = "SortedGroup";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // findProcTxt
            // 
            this.findTextTxt.Name = "findTextTxt";
            this.findTextTxt.Size = new System.Drawing.Size(100, 25);
            this.findTextTxt.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.findTextTxt.Leave += new System.EventHandler(this.FindProcTxtLeave);
            this.findTextTxt.Enter += new System.EventHandler(this.FindProcTxtEnter);
            this.findTextTxt.Click += new System.EventHandler(this.FindProcTxtEnter);
            this.findTextTxt.TextChanged += new System.EventHandler(this.FindProcTxtChanged);
            // 
            // clearButton
            //
            this.clearButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(23, 22);
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.toolStrip);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(268, 300);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Status

        private delegate void SetStatusInvoker(string state, int value, int max);

        /// <summary>
        /// Show a status bar
        /// - this method is always thread safe
        /// - if the message is 'null', hides the panel
        /// </summary>
        /// <param name="state">Message</param>
        /// <param name="value">ProgressBar's value</param>
        /// <param name="max">ProgressBar's maximum</param>
        public void SetStatus(string state, int value, int max)
        {
            // thread safe invocation
            if (InvokeRequired)
            {
                BeginInvoke(new SetStatusInvoker(SetStatus), new object[] { state, value, max });
                return;
            }
            if (PluginBase.MainForm.ClosingEntirely) return;
            if (state == null)
            {
                if (PluginBase.MainForm.ProgressLabel != null) PluginBase.MainForm.ProgressLabel.Visible = false;
                if (PluginBase.MainForm.ProgressBar != null) PluginBase.MainForm.ProgressBar.Visible = false;
                return;
            }
            if (PluginBase.MainForm.ProgressLabel != null)
            {
                PluginBase.MainForm.ProgressLabel.Text = state;
                PluginBase.MainForm.ProgressLabel.Visible = true;
            }
            if (PluginBase.MainForm.ProgressBar != null)
            {
                PluginBase.MainForm.ProgressBar.Maximum = max;
                PluginBase.MainForm.ProgressBar.Value = value;
                PluginBase.MainForm.ProgressBar.Visible = true;
            }
        }

        #endregion

        #region class_tree_display

        /// <summary>
        /// Show the current class/member in the current outline
        /// </summary>
        /// <param name="classModel"></param>
        /// <param name="memberModel"></param>
        internal void Highlight(ClassModel classModel, MemberModel memberModel)
        {
            if (outlineTree.Nodes.Count == 0) return;
            string match;
            // class or class member
            if (classModel != null && classModel != ClassModel.VoidClass)
            {
                match = classModel.Name;
                TreeNode found = MatchNodeText(outlineTree.Nodes[0].Nodes, match);
                if (found != null)
                {
                    if (memberModel != null)
                    {
                        match = memberModel.ToString();
                        TreeNode foundSub = MatchNodeText(found.Nodes, match);
                        if (foundSub != null)
                        {
                            SetHighlight(foundSub);
                            return;
                        }
                    }
                    SetHighlight(found);
                    return;
                }
            }
            // file member
            else if (memberModel != null)
            {
                match = memberModel.ToString();
                TreeNode found = MatchNodeText(outlineTree.Nodes[0].Nodes, match);
                if (found != null)
                {
                    SetHighlight(found);
                    return;
                }
            }
            // no match
            SetHighlight(null);
        }

        private TreeNode MatchNodeText(TreeNodeCollection nodes, string match)
        {
            foreach(TreeNode node in nodes)
            {
                if (node.Text == match) return node;
                if (node.Nodes.Count > 0)
                {
                    TreeNode found = MatchNodeText(node.Nodes, match);
                    if (found != null) return found;
                }
            }
            return null;
        }

        private void SetHighlight(TreeNode node)
        {
            if (node == currentHighlight) return;
            if (currentHighlight != null)
            {
                //currentHighlight.BackColor = System.Drawing.SystemColors.Window;
                currentHighlight.ForeColor = outlineTree.ForeColor;
            }
            outlineTree.SelectedNode = currentHighlight = node;
            if (currentHighlight != null)
            {
                if (outlineTree.State != null && currentHighlight.TreeView != null)
                    outlineTree.State.highlight = currentHighlight.FullPath;

                //currentHighlight.BackColor = System.Drawing.Color.LightGray;
                currentHighlight.ForeColor = PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);

            }
        }

        /// <summary>
        /// Update outline view
        /// </summary>
        /// <param name="aFile"></param>
        internal void UpdateView(FileModel aFile)
        {
            try
            {
                // files "checksum"
                StringBuilder sb = new StringBuilder().Append(aFile.FileName).Append(aFile.Version).Append(aFile.Package);
                var lines = new List<int>();
                var names = new List<string>();
                if (aFile != FileModel.Ignore)
                {
                    sb.Append(settings.ShowExtends).Append(settings.ShowImplements).Append(settings.ShowImports).Append(
                        settings.ShowRegions);
                    foreach (MemberModel import in aFile.Imports)
                    {
                        sb.Append(import.Type);
                        lines.Add(import.LineFrom);
                        names.Add(import.Name);
                    }
                    foreach (MemberModel member in aFile.Members)
                    {
                        sb.Append(member.Flags).Append(member);
                        lines.Add(member.LineFrom);
                        names.Add(member.Name);
                    }
                    foreach (var aClass in aFile.Classes)
                    {
                        if (string.IsNullOrEmpty(aClass.ExtendsType))
                            aClass.ResolveExtends();

                        sb.Append(aClass.Flags).Append(aClass.FullName);
                        sb.Append(aClass.ExtendsType);
                        if (aClass.Implements != null)
                            foreach (var implements in aClass.Implements)
                                sb.Append(implements);
                        lines.Add(aClass.LineFrom);
                        names.Add(aClass.Name);
                        foreach (MemberModel member in aClass.Members)
                        {
                            sb.Append(member.Flags).Append(member);
                            lines.Add(member.LineFrom);
                            names.Add(member.Name);
                        }
                    }
                    foreach (MemberModel region in aFile.Regions)
                    {
                        sb.Append(region.Name);
                        lines.Add(region.LineFrom);
                        names.Add(region.Name);
                    }
                }

                string checksum = sb.ToString();
                if (checksum != prevChecksum)
                {
                    prevChecksum = checksum;
                    prevLines = lines;
                    RefreshView(aFile);
                }
                else
                {
                    int prevLinesCount = prevLines.Count;
                    for (int i = 0, count = lines.Count; i < count; i++)
                    {
                        if (i < prevLinesCount && lines[i] == prevLines[i]) continue;
                        UpdateTree(aFile, names, lines);
                        prevLines = lines;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(/*ex.Message,*/ ex);
            }
        }

        private void RefreshView(FileModel aFile)
        {
            //TraceManager.Add("Outline refresh...");
            outlineTree.BeginStatefulUpdate();
            if (prevChecksum.StartsWithOrdinal(aFile.FileName))
                aFile.OutlineState = outlineTree.State;

            try
            {
                currentHighlight = null;
                outlineTree.Nodes.Clear();

                // If text == "" then the field has the focus and it's already empty, no need to dispatch unneeded events
                if (findTextTxt.Text != searchInvitation && findTextTxt.Text != string.Empty)
                {
                    findTextTxt.Clear();
                    FindProcTxtLeave(null, null);
                }

                TreeNode root = new TreeNode(System.IO.Path.GetFileName(aFile.FileName), ICON_FILE, ICON_FILE);
                outlineTree.Nodes.Add(root);
                if (aFile == FileModel.Ignore)
                    return;

                TreeNodeCollection folders = root.Nodes;
                TreeNodeCollection nodes;
                TreeNode node;
                int img;

                // imports
                if (settings.ShowImports && aFile.Imports.Count > 0)
                {
                    node = new TreeNode(TextHelper.GetString("Info.ImportsNode"), ICON_FOLDER_OPEN, ICON_FOLDER_OPEN);
                    folders.Add(node);
                    nodes = node.Nodes;
                    foreach (MemberModel import in aFile.Imports)
                    {
                        if (import.Type.EndsWithOrdinal(".*"))
                            nodes.Add(new TreeNode(import.Type, ICON_PACKAGE, ICON_PACKAGE));
                        else
                        {
                            img = GetIcon(import.Flags, import.Access); 
                            //((import.Flags & FlagType.Intrinsic) > 0) ? ICON_INTRINSIC_TYPE : ICON_TYPE;
                            node = new TreeNode(import.Type, img, img);
                            node.Tag = "import";
                            nodes.Add(node);
                        }
                    }
                }

                // class members
                if (aFile.Members.Count > 0)
                {
                    AddMembersSorted(folders, aFile.Members);
                }

                // regions
                if (settings.ShowRegions)
                {
                    if (aFile.Regions.Count > 0)
                    {
                        node = new TreeNode(TextHelper.GetString("Info.RegionsNode"), ICON_PACKAGE, ICON_PACKAGE);
                        folders.Add(node);
                        //AddRegions(node.Nodes, aFile.Regions);
                        AddRegionsExtended(node.Nodes, aFile);
                    }
                }

                // classes
                if (aFile.Classes.Count > 0)
                {
                    nodes = folders;

                    foreach (ClassModel aClass in aFile.Classes)
                    {
                        img = GetIcon(aClass.Flags, aClass.Access);
                        node = new TreeNode(aClass.FullName, img, img);
                        node.Tag = "class";
                        nodes.Add(node);
                        if (settings.ShowExtends) AddExtend(node.Nodes, aClass);
                        if (settings.ShowImplements) AddImplements(node.Nodes, aClass.Implements);
                        AddMembersSorted(node.Nodes, aClass.Members);
                        node.Expand();
                    }
                }

                root.Expand();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(/*ex.Message,*/ ex);
            }
            finally
            {
                // outline state will be restored/saved from the model data
                if (aFile.OutlineState == null)
                    aFile.OutlineState = new TreeState();
                // restore collapsing state
                outlineTree.EndStatefulUpdate(aFile.OutlineState);
                // restore highlighted item
                if (aFile.OutlineState.highlight != null)
                {
                    TreeNode toHighligh = outlineTree.FindClosestPath(outlineTree.State.highlight);
                    if (toHighligh != null)
                        SetHighlight(toHighligh);
                    else
                        Highlight(ASContext.Context.CurrentClass, ASContext.Context.CurrentMember);
                }
            }
        }

        private void UpdateTree(FileModel aFile, List<string> modelNames, List<int> newLines)
        {
            try
            {
                if (aFile == FileModel.Ignore)
                    return;

                var mapping = new Dictionary<string, string>();
                int prevLinesCount = prevLines.Count;
                for (int i = 0, count = newLines.Count; i < count; i++)
                {
                    string name = modelNames[i];
                    string value = name + "@" + newLines[i];
                    if (i < prevLinesCount) mapping[name + "@" + prevLines[i]] = value;
                    else mapping[value] = value;
                }

                var tree = new Stack<TreeNodeCollection>();
                tree.Push(outlineTree.Nodes);
                while (tree.Count > 0)
                {
                    var nodes = tree.Pop();
                    foreach (TreeNode node in nodes)
                    {
                        var memberNode = node as MemberTreeNode;
                        string newTag;
                        if (memberNode != null && mapping.TryGetValue((string)memberNode.Tag, out newTag))
                            memberNode.Tag = newTag;
                        if (node.Nodes.Count > 0) tree.Push(node.Nodes);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(/*ex.Message,*/ ex);
            }
        }

        private void AddExtend(TreeNodeCollection tree, ClassModel aClass)
        {
            TreeNode folder = new TreeNode(TextHelper.GetString("Info.ExtendsNode"), ICON_FOLDER_CLOSED, ICON_FOLDER_OPEN);

            //if ((aClass.Flags & FlagType.TypeDef) > 0 && aClass.Members.Count == 0)
            //    folder.Text = "Defines"; // TODO need a better word I guess

            while (!string.IsNullOrEmpty(aClass.ExtendsType) && aClass.ExtendsType != aClass.InFile.Context.Features.dynamicKey)
            {
                string extends = aClass.ExtendsType;
                aClass = aClass.Extends;
                if (!aClass.IsVoid()) extends = aClass.QualifiedName;
                if (extends.ToLower() == "void")
                    break;
                TreeNode extNode = new TreeNode(extends, ICON_TYPE, ICON_TYPE);
                extNode.Tag = "import";
                folder.Nodes.Add(extNode);
            }
            if (folder.Nodes.Count > 0) tree.Add(folder);
        }

        private void AddImplements(TreeNodeCollection tree, List<string> implementsTypes)
        {
            if (implementsTypes == null || implementsTypes.Count == 0)
                return;
            TreeNode folder = new TreeNode(TextHelper.GetString("Info.ImplementsNode"), ICON_FOLDER_CLOSED, ICON_FOLDER_OPEN);
            foreach (string implements in implementsTypes)
            {
                TreeNode impNode = new TreeNode(implements, ICON_INTERFACE, ICON_INTERFACE);
                impNode.Tag = "import";
                folder.Nodes.Add(impNode);
            }
            tree.Add(folder);
        }

        private void AddMembersSorted(TreeNodeCollection tree, MemberList members)
        {
            if (settings.SortingMode == OutlineSorting.None)
            {
                AddMembers(tree, members);
            }
            else if (settings.SortingMode == OutlineSorting.SortedGroup)
            {
                AddMembersGrouped(tree, members);
            }
            else
            {
                IComparer<MemberModel> comparer = null;
                if (settings.SortingMode == OutlineSorting.Sorted)
                    comparer = null;
                else if (settings.SortingMode == OutlineSorting.SortedByKind)
                    comparer = new ByKindMemberComparer();
                else if (settings.SortingMode == OutlineSorting.SortedSmart)
                    comparer = new SmartMemberComparer();
                else if (settings.SortingMode == OutlineSorting.SortedGroup)
                    comparer = new ByKindMemberComparer();

                MemberList copy = new MemberList();
                copy.Add(members);
                copy.Sort(comparer);
                AddMembers(tree, copy);
            }
        }

        private void AddRegionsExtended(TreeNodeCollection tree, FileModel aFile)
        {
            int endRegion = 0;
            int index = 0;
            MemberModel region = null;
            MemberList regions = aFile.Regions;
            int count = regions.Count;
            for (index = 0; index < count; ++index)
            {
                region = regions[index];
                MemberTreeNode node = new MemberTreeNode(region, ICON_PACKAGE);
                tree.Add(node);

                endRegion = region.LineTo;
                if (endRegion == 0)
                {
                    endRegion = (index + 1 < count) ? regions[index + 1].LineFrom : int.MaxValue;
                }

                MemberList regionMembers = new MemberList();
                foreach (MemberModel import in aFile.Imports)
                {
                    if (import.LineFrom >= region.LineFrom &&
                        import.LineTo <= endRegion)
                    {
                        regionMembers.Add(import);
                    }
                }

                foreach (MemberModel fileMember in aFile.Members)
                {
                    if (fileMember.LineFrom >= region.LineFrom &&
                        fileMember.LineTo <= endRegion)
                    {
                        regionMembers.Add(fileMember);
                    }
                }

                foreach (ClassModel cls in aFile.Classes)
                {
                    if (cls.LineFrom <= region.LineFrom)
                    {
                        foreach (MemberModel clsMember in cls.Members)
                        {
                            if (clsMember.LineFrom >= region.LineFrom &&
                                clsMember.LineTo <= endRegion)
                            {
                                regionMembers.Add(clsMember);
                            }
                        }
                    }
                    else if (cls.LineFrom >= region.LineFrom &&
                             cls.LineTo <= endRegion)
                    {
                        regionMembers.Add(cls);
                    }
                }
                AddMembers(node.Nodes, regionMembers);
            }
        }

        /// <summary>
        /// Add tree nodes following the user defined members presentation
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="members"></param>
        static public void AddMembers(TreeNodeCollection tree, MemberList members)
        {
            TreeNodeCollection nodes = tree;
            foreach (MemberModel member in members)
            {
                var img = GetIcon(member.Flags, member.Access);
                var node = new MemberTreeNode(member, img);
                nodes.Add(node);
            }
        }

        static public void AddMembersGrouped(TreeNodeCollection tree, MemberList members)
        {
            FlagType[] typePriority = new FlagType[] { FlagType.Constructor, FlagType.Function, FlagType.Getter | FlagType.Setter, FlagType.Variable, FlagType.Constant };
            Dictionary<FlagType, List<MemberModel>> typeGroups = new Dictionary<FlagType, List<MemberModel>>();

            FlagType type;
            List<MemberModel> groupList;
            foreach (MemberModel member in members)
            {
                // member type
                if ((member.Flags & FlagType.Constant) > 0)
                    type = FlagType.Constant;
                else if ((member.Flags & FlagType.Variable) > 0)
                    type = FlagType.Variable;
                else if ((member.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                    type = (FlagType.Getter | FlagType.Setter);
                else if ((member.Flags & FlagType.Constructor) > 0)
                    type = FlagType.Constructor;
                else type = FlagType.Function;

                // group
                if (!typeGroups.TryGetValue(type, out groupList))
                {
                    groupList = new List<MemberModel>();
                    typeGroups.Add(type, groupList);
                }

                groupList.Add(member);
            }

            for (int i = 0, count = typePriority.Length; i < count; i++)
            {
                type = typePriority[i];
                if (typeGroups.TryGetValue(type, out groupList))
                {
                    if (groupList.Count == 0)
                        continue;
                    groupList.Sort();

                    TreeNode groupNode = new TreeNode(type.ToString(), ICON_FOLDER_CLOSED, ICON_FOLDER_OPEN);
                    int img;
                    foreach (MemberModel member in groupList)
                    {
                        img = GetIcon(member.Flags, member.Access);
                        var node = new MemberTreeNode(member, img);
                        groupNode.Nodes.Add(node);
                    }
                    if (type != FlagType.Constructor) groupNode.Expand();
                    tree.Add(groupNode);
                }
            }
        }

        private void SortDropDown_DropDownOpening(object sender, EventArgs e)
        {
            noneItem.Checked = settings.SortingMode == OutlineSorting.None;
            sortedItem.Checked = settings.SortingMode == OutlineSorting.Sorted;
            sortedByKindItem.Checked = settings.SortingMode == OutlineSorting.SortedByKind;
            sortedGroupItem.Checked = settings.SortingMode == OutlineSorting.SortedGroup;
            sortedSmartItem.Checked = settings.SortingMode == OutlineSorting.SortedSmart;
        }

        private void SortDropDown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
            if (item == null || item.Checked) return;
            if (item == noneItem) settings.SortingMode = OutlineSorting.None;
            else if (item == sortedItem) settings.SortingMode = OutlineSorting.Sorted;
            else if (item == sortedByKindItem) settings.SortingMode = OutlineSorting.SortedByKind;
            else if (item == sortedGroupItem) settings.SortingMode = OutlineSorting.SortedGroup;
            else if (item == sortedSmartItem) settings.SortingMode = OutlineSorting.SortedSmart;
            if (ASContext.Context.CurrentModel != null) RefreshView(ASContext.Context.CurrentModel);
        }

        #endregion

        #region tree_items_selection
        /// <summary>
        /// Selection des items de l'arbre
        /// </summary>
        private void ClassTreeSelect(object sender, TreeNode node)
        {
            if (tempoClick == null)
            {
                tempoClick = new System.Timers.Timer();
                tempoClick.Interval = 50;
                tempoClick.SynchronizingObject = this;
                tempoClick.AutoReset = false;
                tempoClick.Elapsed += delayedClassTreeSelect;
            }
            tempoClick.Enabled = true;
        }

        private void delayedClassTreeSelect(Object sender, System.Timers.ElapsedEventArgs e)
        {
            TreeNode node = outlineTree.SelectedNode;
            if (node == null)
                return;
            ASContext.Context.OnSelectOutlineNode(node);
        }

        public void GotoPosAndFocus(ScintillaNet.ScintillaControl sci, int position)
        {
            int pos = sci.MBSafePosition(position);
            int line = sci.LineFromPosition(pos);
            sci.EnsureVisible(line);
            sci.GotoPos(pos);
        }

        // TODO: Refactor, doesn't make a lot of sense to have this feature inside the Panel
        public void SetLastLookupPosition(string file, int line, int column)
        {
            // store location
            if (lookupLocations == null) lookupLocations = new Stack<LookupLocation>();
            lookupLocations.Push(new LookupLocation(file, line, column));
            if (lookupLocations.Count > 100) lookupLocations.TrimExcess();
            // menu item
            if (LookupMenuItem != null) LookupMenuItem.Enabled = true;
        }
        public bool RestoreLastLookupPosition()
        {
            if (!ASContext.Context.IsFileValid || lookupLocations == null || lookupLocations.Count == 0)
                return false;

            LookupLocation location = lookupLocations.Pop();
            // menu item
            if (lookupLocations.Count == 0 && LookupMenuItem != null) LookupMenuItem.Enabled = false;

            PluginBase.MainForm.OpenEditableDocument(location.file, false);
            ScintillaNet.ScintillaControl sci = ASContext.CurSciControl;
            if (sci != null)
            {
                int position = sci.PositionFromLine(location.line) + location.column;
                sci.SetSel(position, position);
                int line = sci.CurrentLine;
                sci.EnsureVisible(line);
                int top = sci.FirstVisibleLine;
                int middle = top + sci.LinesOnScreen / 2;
                sci.LineScroll(0, line - middle);
                return true;
            }
            return false;
        }

        #endregion

        #region Find declaration

        // if hilight is true, shows the node and paint it with color 
        private void ShowAndHilightNode(TreeNode node, bool hilight)
        {
            if (hilight)
            {
                node.EnsureVisible();
                node.BackColor = PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);
                node.ForeColor = PluginBase.MainForm.GetThemeColor("TreeView.HighlightText", SystemColors.HighlightText);
            }
            else
            {
                node.ForeColor = OutlineTree.ForeColor;
                node.BackColor = OutlineTree.BackColor;
            }
        }

        private bool IsMach(string inputText, string searchText)
        {
            if (inputText == null || searchText == "")
            {
                return false;
            }
            return inputText.ToUpper().Contains(searchText);
        }

        private void HighlightAllMachingDeclaration(string text)
        {
            try
            {
                TreeNodeCollection nodes = outlineTree.Nodes;
                HilightDeclarationInGroup(nodes, text);
                if (Win32.ShouldUseWin32()) Win32.ScrollToLeft(outlineTree);
            }
            catch (Exception ex)
            {
                // log error and disable search field
                ErrorManager.ShowError(ex);
                //findProcTxt.Visible = false;
            }
        }

        private void HilightDeclarationInGroup(TreeNodeCollection nodes, string text)
        {
            foreach (TreeNode sub in nodes)
            {
                if (sub.ImageIndex >= ICON_VAR) ShowAndHilightNode(sub, IsMach(sub.Tag as string, text));
                if (sub.Nodes.Count > 0) HilightDeclarationInGroup(sub.Nodes, text);
            }
        }

        void FindProcTxtChanged(object sender, EventArgs e)
        {
            string text = findTextTxt.Text;
            if (text == searchInvitation) return;
            HighlightAllMachingDeclaration(text.ToUpper());
            clearButton.Enabled = text.Length > 0;
        }

        // Display informative text in the search field
        private string searchInvitation = TextHelper.GetString("Info.SearchInvitation");

        void FindProcTxtEnter(object sender, EventArgs e)
        {
            if (findTextTxt.Text == searchInvitation)
            {
                findTextTxt.Text = "";
                findTextTxt.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.ForeColor", SystemColors.WindowText);
            }
        }

        void FindProcTxtLeave(object sender, EventArgs e)
        {
            if (findTextTxt.Text == "")
            {
                findTextTxt.Text = searchInvitation;
                findTextTxt.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
                clearButton.Enabled = false;
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            findTextTxt.Text = "";
            FindProcTxtLeave(null, null);
            outlineTree.Focus();
            if (PluginBase.MainForm.CurrentDocument.IsEditable)
            {
                PluginBase.MainForm.CurrentDocument.SciControl.Focus();
            }
        }

        // Update colors on start after theme engine
        public void UpdateAfterTheme()
        {
            findTextTxt.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
        }

        protected override Boolean ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (outlineTree.Focused)
                {
                    ClassTreeSelect(outlineTree, outlineTree.SelectedNode);
                    return true;
                }
                else
                {
                    if (findTextTxt.Text != searchInvitation)
                    {
                        TreeNode node = FindMatch(outlineTree.Nodes);
                        if (node != null)
                        {
                            outlineTree.SelectedNode = node;
                            delayedClassTreeSelect(null, null);
                        }
                        findTextTxt.Text = "";
                    }
                    return true;
                }
            }
            else if (keyData == Keys.Escape)
            {
                findTextTxt.Text = "";
                FindProcTxtLeave(null, null);
                outlineTree.Focus();
                if (PluginBase.MainForm.CurrentDocument.IsEditable)
                {
                    PluginBase.MainForm.CurrentDocument.SciControl.Focus();
                }
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Find an highlighted item and "click" it
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private TreeNode FindMatch(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.BackColor == PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight)) return node;
                if (node.Nodes.Count > 0)
                {
                    TreeNode subnode = FindMatch(node.Nodes);
                    if (subnode != null) return subnode;
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Handles the shortcut
        /// </summary>
        public bool OnShortcut(Keys keys)
        {
            if (!ContainsFocus) return false;
            if (keys == (Keys.Control | Keys.F))
            {
                findTextTxt.Focus();
                return true;
            }
            return false;
        }
    }

    #region Custom structures
    struct LookupLocation
    {
        public string file;
        public int line;
        public int column;
        public LookupLocation(string file, int line, int column)
        {
            this.file = file;
            this.line = line;
            this.column = column;
        }
    }

    class MemberTreeNode : TreeNode
    {
        public MemberTreeNode(MemberModel member, int imageIndex)
            : base(member.ToString(), imageIndex, imageIndex)
        {
            Tag = member.Name + "@" + member.LineFrom;
        }
    }
    #endregion

}
