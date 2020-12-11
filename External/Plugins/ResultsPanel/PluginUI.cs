// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ResultsPanel.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;
using WeifenLuo.WinFormsUI.Docking;
using Keys = System.Windows.Forms.Keys;

namespace ResultsPanel
{
    public class PluginUI : DockPanelControl
    {
        ColumnHeader entryType;
        ColumnHeader entryLine;
        ColumnHeader entryDesc;
        ColumnHeader entryFile;
        ColumnHeader entryPath;
        readonly List<ListViewItem> allListViewItems;
        ToolStripButton toolStripButtonError;
        ToolStripButton toolStripButtonWarning;
        ToolStripButton toolStripButtonInfo;
        ToolStripButton toolStripButtonLock;
        ToolStripSpringTextBox toolStripTextBoxFilter;
        ToolStripLabel toolStripLabelFilter;
        ToolStripButton clearFilterButton;
        ToolStrip toolStripFilters;
        int errorCount;
        int warningCount;
        int messageCount;
        readonly PluginMain pluginMain;
        int logCount;
        Timer autoShow;
        SortOrder sortOrder;
        int lastColumn;
        GroupingMethod groupingMethod;
        readonly int buttonsWidth;
        Container components;
        static ImageListManager imageList;

        #region Constructors

        public PluginUI(PluginMain pluginMain) : this(pluginMain, null, null, true, false)
        {
        }

        internal PluginUI(PluginMain pluginMain, string groupData, string groupId, bool showFilterButtons, bool allowMultiplePanels)
        {
            AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            //this.logCount = TraceManager.TraceLog.Count;
            logCount = 0;
            allListViewItems = new List<ListViewItem>();
            IgnoredEntries = new Dictionary<string, bool>();
            errorCount = 0;
            warningCount = 0;
            messageCount = 0;
            sortOrder = SortOrder.Ascending;
            InitializeComponent();
            InitializeContextMenu();
            InitializeGraphics();
            InitializeTexts();
            InitializeLayout();
            ScrollBarEx.Attach(EntriesView);

            GroupData = groupData;
            GroupId = groupId;

            buttonsWidth = 0;
            if (allowMultiplePanels)
            {
                buttonsWidth = 200;
                toolStripButtonLock.Checked = Settings.KeepResultsByDefault;
                toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                toolStripFilters.Items.Insert(0, toolStripButtonLock);
            }
            if (showFilterButtons)
            {
                buttonsWidth = 800;
                toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                toolStripFilters.Items.Insert(0, toolStripButtonError);
                toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                toolStripFilters.Items.Insert(0, toolStripButtonWarning);
                toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                toolStripFilters.Items.Insert(0, toolStripButtonInfo);
            }

            entryFile.Tag = GroupingMethod.File;
            entryType.Tag = GroupingMethod.Type;
            entryDesc.Tag = GroupingMethod.Description;
            entryPath.Tag = GroupingMethod.Path;
            entryLine.Tag = GroupingMethod.Line;
            groupingMethod = Settings.DefaultGrouping;
            lastColumn = new Dictionary<GroupingMethod, ColumnHeader>()
            {
                [(GroupingMethod) entryFile.Tag] = entryFile,
                [(GroupingMethod) entryType.Tag] = entryType,
                [(GroupingMethod) entryDesc.Tag] = entryDesc,
                [(GroupingMethod) entryPath.Tag] = entryPath,
                [(GroupingMethod) entryLine.Tag] = entryLine
            }[groupingMethod].Index;
            ApplySettings(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trace group id and arguments associated with this panel.
        /// </summary>
        public string GroupData { get; internal set; }

        /// <summary>
        /// Gets the trace group id associated with this panel.
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// Gets whether the Keep Results button is toggled.
        /// </summary>
        public bool Locked => toolStripButtonLock.Checked;

        /// <summary>
        /// Gets the parent <see cref="DockContent"/>.
        /// </summary>
        public DockContent ParentPanel { get; internal set; }

        /// <summary>
        /// Accessor for settings
        /// </summary>
        internal Settings Settings => (Settings) pluginMain.Settings;

        internal ListViewEx EntriesView { get; private set; }

        internal IDictionary<string, bool> IgnoredEntries { get; }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent()
        {
            components = new Container();
            EntriesView = new ListViewEx();
            entryType = new ColumnHeader();
            entryLine = new ColumnHeader();
            entryDesc = new ColumnHeader();
            entryFile = new ColumnHeader();
            entryPath = new ColumnHeader();
            toolStripFilters = new ToolStripEx();
            toolStripButtonInfo = new ToolStripButton();
            toolStripButtonWarning = new ToolStripButton();
            toolStripButtonError = new ToolStripButton();
            toolStripButtonLock = new ToolStripButton();
            toolStripLabelFilter = new ToolStripLabel();
            toolStripTextBoxFilter = new ToolStripSpringTextBox();
            clearFilterButton = new ToolStripButton();
            autoShow = new Timer(components);
            toolStripFilters.SuspendLayout();
            SuspendLayout();
            // 
            // entriesView
            // 
            EntriesView.BorderStyle = BorderStyle.None;
            EntriesView.Columns.AddRange(new[] {
            entryType,
            entryLine,
            entryDesc,
            entryFile,
            entryPath});
            EntriesView.Dock = DockStyle.Fill;
            EntriesView.FullRowSelect = true;
            EntriesView.GridLines = false;
            EntriesView.Location = new Point(0, 28);
            EntriesView.Name = "entriesView";
            EntriesView.ShowGroups = true;
            EntriesView.ShowItemToolTips = true;
            EntriesView.Size = new Size(710, 218);
            EntriesView.TabIndex = 1;
            EntriesView.UseCompatibleStateImageBehavior = false;
            EntriesView.View = View.Details;
            EntriesView.ColumnClick += EntriesView_ColumnClick;
            EntriesView.DoubleClick += EntriesView_DoubleClick;
            EntriesView.KeyDown += EntriesView_KeyDown;
            // 
            // entryType
            // 
            entryType.Text = "!";
            entryType.Width = 23;
            // 
            // entryLine
            // 
            entryLine.Text = "Line";
            entryLine.Width = 55;
            // 
            // entryDesc
            // 
            entryDesc.Text = "Description";
            entryDesc.Width = 350;
            // 
            // entryFile
            // 
            entryFile.Text = "File";
            entryFile.Width = 150;
            // 
            // entryPath
            // 
            entryPath.Text = "Path";
            entryPath.Width = 400;
            // 
            // toolStripFilters
            // 
            toolStripFilters.CanOverflow = false;
            toolStripFilters.GripStyle = ToolStripGripStyle.Hidden;
            toolStripFilters.Items.AddRange(new ToolStripItem[] {
            toolStripLabelFilter,
            toolStripTextBoxFilter,
            clearFilterButton});
            toolStripFilters.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStripFilters.Location = new Point(1, 0);
            toolStripFilters.Name = "toolStripFilters";
            toolStripFilters.Padding = new Padding(1, 1, 2, 2);
            toolStripFilters.Size = new Size(710, 25);
            toolStripFilters.TabIndex = 0;
            toolStripFilters.Text = "toolStripFilters";
            // 
            // toolStripButtonInfo
            // 
            toolStripButtonInfo.Checked = true;
            toolStripButtonInfo.CheckOnClick = true;
            toolStripButtonInfo.Margin = new Padding(1, 1, 0, 1);
            toolStripButtonInfo.CheckState = CheckState.Checked;
            toolStripButtonInfo.ImageTransparentColor = Color.Magenta;
            toolStripButtonInfo.Name = "toolStripButtonInfo";
            toolStripButtonInfo.Size = new Size(74, 22);
            toolStripButtonInfo.Text = "Information";
            toolStripButtonInfo.CheckedChanged += ToolStripButton_CheckedChanged;
            // 
            // toolStripButtonWarning
            // 
            toolStripButtonWarning.Checked = true;
            toolStripButtonWarning.CheckOnClick = true;
            toolStripButtonWarning.CheckState = CheckState.Checked;
            toolStripButtonWarning.ImageTransparentColor = Color.Magenta;
            toolStripButtonWarning.Margin = new Padding(1, 1, 0, 1);
            toolStripButtonWarning.Name = "toolStripButtonWarning";
            toolStripButtonWarning.Size = new Size(56, 22);
            toolStripButtonWarning.Text = "Warning";
            toolStripButtonWarning.CheckedChanged += ToolStripButton_CheckedChanged;
            // 
            // toolStripButtonError
            // 
            toolStripButtonError.Checked = true;
            toolStripButtonError.CheckOnClick = true;
            toolStripButtonError.CheckState = CheckState.Checked;
            toolStripButtonError.ImageTransparentColor = Color.Magenta;
            toolStripButtonError.Margin = new Padding(1, 1, 0, 1);
            toolStripButtonError.Name = "toolStripButtonError";
            toolStripButtonError.Size = new Size(36, 22);
            toolStripButtonError.Text = "Error";
            toolStripButtonError.CheckedChanged += ToolStripButton_CheckedChanged;
            // 
            // toolStripButtonLock
            // 
            toolStripButtonLock.Checked = false;
            toolStripButtonLock.CheckOnClick = true;
            toolStripButtonLock.CheckState = CheckState.Unchecked;
            toolStripButtonLock.ImageTransparentColor = Color.Magenta;
            toolStripButtonLock.Margin = new Padding(1, 1, 0, 1);
            toolStripButtonLock.Name = "toolStripButtonLock";
            toolStripButtonLock.Size = new Size(74, 22);
            toolStripButtonLock.Text = "Keep Results";
            // 
            // toolStripLabelFilter
            //
            toolStripLabelFilter.Margin = new Padding(2, 1, 0, 1);
            toolStripLabelFilter.Name = "toolStripLabelFilter";
            toolStripLabelFilter.Size = new Size(36, 22);
            toolStripLabelFilter.Text = "Filter:";
            // 
            // toolStripTextBoxFilter
            //
            toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            toolStripTextBoxFilter.Size = new Size(100, 25);
            toolStripTextBoxFilter.Padding = new Padding(0, 0, 1, 0);
            toolStripTextBoxFilter.TextChanged += ToolStripButton_CheckedChanged;
            // 
            // clearFilterButton
            //
            clearFilterButton.Alignment = ToolStripItemAlignment.Right;
            clearFilterButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clearFilterButton.Enabled = false;
            clearFilterButton.ImageTransparentColor = Color.Magenta;
            clearFilterButton.Margin = new Padding(0, 1, 0, 1);
            clearFilterButton.Name = "clearFilterButton";
            clearFilterButton.Size = new Size(23, 26);
            clearFilterButton.Click += ClearFilterButton_Click;
            // 
            // autoShow
            // 
            autoShow.Interval = 300;
            autoShow.Tick += AutoShow_Tick;
            // 
            // PluginUI
            //
            Controls.Add(EntriesView);
            Controls.Add(toolStripFilters);
            Name = "PluginUI";
            Size = new Size(712, 246);
            Resize += PluginUI_Resize;
            toolStripFilters.ResumeLayout(false);
            toolStripFilters.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the context menu for entriesView
        /// </summary>
        void InitializeContextMenu() => EntriesView.ContextMenuStrip = pluginMain.contextMenuStrip;

        /// <summary>
        /// Initializes the image list for entriesView
        /// </summary>
        void InitializeGraphics()
        {
            if (imageList is null)
            {
                imageList = new ImageListManager();
                imageList.ColorDepth = ColorDepth.Depth32Bit;
                imageList.TransparentColor = Color.Transparent;
                imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
                imageList.Initialize(ImageList_Populate);
            }

            toolStripFilters.Renderer = new DockPanelStripRenderer();
            toolStripFilters.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            toolStripFilters.ImageList = imageList;
            EntriesView.SmallImageList = imageList;
            clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            toolStripButtonInfo.Image = PluginBase.MainForm.FindImage("131");
            toolStripButtonWarning.Image = PluginBase.MainForm.FindImage("196");
            toolStripButtonError.Image = PluginBase.MainForm.FindImage("197");
            toolStripButtonLock.Image = PluginBase.MainForm.FindImage("246");
            EntriesView.AddArrowImages();
        }

        /// <summary>
        /// Applies the localized texts to the control
        /// </summary>
        void InitializeTexts()
        {
            entryLine.Text = TextHelper.GetString("Header.Line");
            entryDesc.Text = TextHelper.GetString("Header.Description");
            entryFile.Text = TextHelper.GetString("Header.File");
            entryPath.Text = TextHelper.GetString("Header.Path");
            toolStripButtonInfo.Text = "0 " + TextHelper.GetString("Filters.Informations");
            toolStripButtonWarning.Text = "0 " + TextHelper.GetString("Filters.Warnings");
            toolStripButtonError.Text = "0 " + TextHelper.GetString("Filters.Errors");
            toolStripButtonLock.Text = TextHelper.GetString("Label.KeepResults");
            toolStripLabelFilter.Text = TextHelper.GetString("Filters.Filter");
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        void InitializeLayout()
        {
            foreach (ColumnHeader column in EntriesView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the settings to the UI.
        /// </summary>
        internal void ApplySettings(bool invalidate = true)
        {
            bool showGroups = PluginBase.Settings.UseListViewGrouping; // Legacy setting - value is now stored in theme
            if (EntriesView.ShowGroups != showGroups)
            {
                EntriesView.ShowGroups = showGroups;
                EntriesView.GridLines = !showGroups;
            }
            else invalidate = false;
            if (invalidate) FilterResults();
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard.
        /// </summary>
        internal bool CopyTextShortcut()
        {
            if (ContainsFocus && EntriesView.Focused)
            {
                CopyText();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ignore entry via shortcut.
        /// </summary>
        internal bool IgnoreEntryShortcut()
        {
            if (ContainsFocus && EntriesView.Focused)
            {
                IgnoreEntry();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the output.
        /// </summary>
        internal bool ClearOutput()
        {
            if (allListViewItems.Count > 0)
            {
                ClearSquiggles();
                allListViewItems.Clear();
                toolStripTextBoxFilter.Text = "";
                errorCount = messageCount = warningCount = 0;
                EntriesView.Items.Clear();
                entryIndex = -1;
                UpdateButtons();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard.
        /// </summary>
        internal void CopyText()
        {
            var selectedItems = EntriesView.SelectedItems;
            if (selectedItems.Count > 0)
            {
                string copy = string.Empty;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var it = selectedItems[i];
                    var m = (Match) it.Tag;
                    copy += m.Value + "\n";
                }
                Clipboard.SetDataObject(copy);
            }
            else
            {
                string copy = string.Empty;
                foreach (ListViewItem item in EntriesView.Items)
                {
                    Match match = (Match) item.Tag;
                    copy += match.Value + "\n";
                }
                Clipboard.SetDataObject(copy);
            }
        }

        /// <summary>
        /// Ignores the currently selected entries.
        /// </summary>
        internal void IgnoreEntry()
        {
            var newIgnoredEntries = new List<ListViewItem>();
            foreach (ListViewItem item in EntriesView.SelectedItems)
            {
                var match = (Match) item.Tag;
                string entryValue = match.Value;
                if (!IgnoredEntries.ContainsKey(entryValue))
                {
                    IgnoredEntries.Add(entryValue, false);
                    newIgnoredEntries.Add(item);
                }
            }
            foreach (ListViewItem item in newIgnoredEntries)
            {
                EntriesView.Items.Remove(item);
            }
            if (newIgnoredEntries.Count > 0)
            {
                RefreshSquiggles();
            }
        }

        /// <summary>
        /// Clears any result entries that are ignored.
        /// </summary>
        internal bool ClearIgnoredEntries()
        {
            if (IgnoredEntries.Count > 0)
            {
                IgnoredEntries.Clear();
                FilterResults();
                RefreshSquiggles();
                return true;
            }
            return false;
        }

        static readonly Regex re_lineColumn = new Regex("([0-9]+),\\s*([0-9]+)", RegexOptions.Compiled);

        /// <summary>
        /// Adds the log entries to the list view
        /// </summary>
        internal void AddLogEntries()
        {
            var traceLog = TraceManager.TraceLog;
            var count = traceLog.Count;
            if (count <= logCount)
            {
                logCount = count;
                return;
            }
            var newResult = false;
            var project = PluginBase.CurrentProject;
            var projectDir = project != null ? Path.GetDirectoryName(project.ProjectPath) : "";
            var limit = Math.Min(count, logCount + 1000);
            for (var i = logCount; i < limit; i++)
            {
                var entry = traceLog[i];
                if (entry.GroupData != GroupData) continue;
                if (entry.Message != null && entry.Message.Length > 7 && entry.Message.IndexOf(':') > 0)
                {
                    var fileTest = entry.Message.TrimStart();
                    var inExec = false;
                    if (fileTest.StartsWithOrdinal("[mxmlc]") || fileTest.StartsWithOrdinal("[compc]") || fileTest.StartsWithOrdinal("[exec]") || fileTest.StartsWithOrdinal("[haxe") || fileTest.StartsWithOrdinal("[java]"))
                    {
                        inExec = true;
                        fileTest = fileTest.Substring(fileTest.IndexOf(']') + 1).TrimStart();
                    }
                    // relative to project root (Haxe)
                    if (fileTest.StartsWithOrdinal("~/")) fileTest = fileTest.Substring(2);
                    var match = fileEntry.Match(fileTest);
                    if (!match.Success) match = fileEntry2.Match(fileTest);
                    if (match.Success && !IgnoredEntries.ContainsKey(match.Value))
                    {
                        var filename = match.Groups["filename"].Value;
                        if (filename.Length < 3 || badCharacters.IsMatch(filename)) continue;
                        if (project != null && filename[0] != '/' && filename[1] != ':') // relative to project root
                        {
                            filename = PathHelper.ResolvePath(filename, projectDir);
                            if (filename is null) continue;
                        }
                        else if (!File.Exists(filename)) continue;
                        var fileInfo = new FileInfo(filename);
                        var description = match.Groups["description"].Value.Trim();
                        var state = inExec ? -3 : entry.State;
                        // automatic state from message
                        // ie. "2:message" -> state = 2
                        if (state == 1 && description.Length > 2
                            && description[1] == ':' && char.IsDigit(description[0])
                            && int.TryParse(description[0].ToString(), out state))
                        {
                            description = description.Substring(2);
                        }

                        int icon;
                        if (state > 2) icon = 1;
                        else if (state == 2) icon = 2;
                        else if (state == -3) icon = description.Contains("Warning") ? 2 : 1;
                        else if (description.StartsWith("error", StringComparison.OrdinalIgnoreCase)) icon = 1;
                        else icon = 0;
                        var item = new ListViewItem("", icon);
                        item.Tag = match; // Save for later...
                        var matchLine = match.Groups["line"].Value;
                        if (matchLine.IndexOf(',') > 0)
                        {
                            var split = re_lineColumn.Match(matchLine);
                            if (!split.Success) continue; // invalid line
                            matchLine = split.Groups[1].Value;
                            description = "col: " + split.Groups[2].Value + " " + description;
                        }
                        item.SubItems.Add(matchLine);
                        item.SubItems.Add(description);
                        item.SubItems.Add(fileInfo.Name);
                        item.SubItems.Add(fileInfo.Directory.ToString());
                        newResult = true;
                        if (icon == 0) messageCount++;
                        else if (icon == 1) errorCount++;
                        else if (icon == 2) warningCount++;
                        allListViewItems.Add(item);
                    }
                }
            }
            logCount = count;
            if (newResult)
            {
                int startIndex = EntriesView.Items.Count;
                UpdateButtons();
                FilterResults();
                for (int i = startIndex; i < EntriesView.Items.Count; i++)
                {
                    AddSquiggle(EntriesView.Items[i]);
                }
            }
        }

        /// <summary>
        /// Flashes the panel to the user
        /// </summary>
        internal void DisplayOutput()
        {
            autoShow.Stop();
            autoShow.Start();
        }

        /// <summary>
        /// Panel is hidden.
        /// </summary>
        internal void OnPanelHidden()
        {
            if (Settings.HighlightOnlyActivePanelEntries)
            {
                if (ResultsPanelHelper.ActiveUI == this)
                {
                    pluginMain.pluginUI.OnPanelActivated();
                }
            }
            else
            {
                ClearSquiggles();
                pluginMain.pluginUI.AddSquiggles();
                foreach (var pluginUI in ResultsPanelHelper.PluginUIs)
                {
                    if (pluginUI != this && !pluginUI.ParentPanel.IsHidden)
                    {
                        pluginUI.AddSquiggles();
                    }
                }
            }
        }

        /// <summary>
        /// Panel is activated.
        /// </summary>
        internal void OnPanelActivated()
        {
            if (Settings.HighlightOnlyActivePanelEntries)
            {
                if (ResultsPanelHelper.ActiveUI != this)
                {
                    if (ResultsPanelHelper.ActiveUI.GroupData != null)
                    {
                        ResultsPanelHelper.ActiveUI.ClearSquiggles();
                        pluginMain.pluginUI.ClearSquiggles();
                        pluginMain.pluginUI.AddSquiggles();
                    }
                    ResultsPanelHelper.ActiveUI = this;
                    if (GroupData != null)
                    {
                        AddSquiggles();
                    }
                }
            }
            else
            {
                ResultsPanelHelper.ActiveUI = this;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Re-populate image list when the theme changes.
        /// </summary>
        static void ImageList_Populate(object sender, EventArgs e)
        {
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("131")); // info
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("197")); // error
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("196")); // warning
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("495")); // up arrow
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("493")); // down arrow
        }

        /// <summary>
        /// Filter the result on check change
        /// </summary>
        void ToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            clearFilterButton.Enabled = toolStripTextBoxFilter.Text.Length > 0;
            FilterResults();
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        void ClearFilterButton_Click(object sender, EventArgs e) => toolStripTextBoxFilter.Text = "";

        /// <summary>
        /// If the user presses Enter, dispatch double click
        /// </summary> 
        void EntriesView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EntriesView_DoubleClick(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// When the user clicks on a column, group using that column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EntriesView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader header = EntriesView.Columns[e.Column];
            if (header.Tag is GroupingMethod tag)
            {
                groupingMethod = tag;

                if (lastColumn == e.Column)
                {
                    sortOrder = sortOrder switch
                    {
                        SortOrder.None => SortOrder.Ascending,
                        SortOrder.Ascending => SortOrder.Descending,
                        SortOrder.Descending => SortOrder.None,
                        _ => sortOrder,
                    };
                }
                else
                {
                    lastColumn = e.Column;
                    sortOrder = SortOrder.Ascending;
                }

                FilterResults();
            }
        }

        /// <summary>
        /// Update the buttons when the panel resizes
        /// </summary>
        void PluginUI_Resize(object sender, EventArgs e) => UpdateButtons();

        /// <summary>
        /// Opens the file and goes to the match
        /// </summary>
        void EntriesView_DoubleClick(object sender, EventArgs e)
        {
            if (EntriesView.SelectedIndices.Count > 0)
            {
                SelectItem(EntriesView.SelectedIndices[0]);
                NavigateToSelectedItem();
            }
        }

        /// <summary>
        /// Shows the panel
        /// </summary>
        void AutoShow_Tick(object sender, EventArgs e)
        {
            autoShow.Stop();
            if (EntriesView.Items.Count > 0)
            {
                bool autoHide = ParentPanel.VisibleState.ToString().EndsWithOrdinal("AutoHide");
                if (!ParentPanel.Visible || autoHide)
                {
                    ParentPanel.Show();
                    if (autoHide) ParentPanel.Activate();
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Convert multibyte column to byte length
        /// </summary>
        int MBSafeColumn(ScintillaControl sci, int line, int length)
        {
            string text = sci.GetLine(line) ?? "";
            length = Math.Min(length, text.Length);
            return sci.MBSafeTextLength(text.Substring(0, length));
        }

        /// <summary>
        /// Goes to the match and ensures that correct fold is opened
        /// </summary>
        void SetSelAndFocus(ScintillaControl sci, int line, int startPosition, int endPosition)
        {
            sci.SetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Goes to the match and ensures that correct fold is opened
        /// </summary>
        void MBSafeSetSelAndFocus(ScintillaControl sci, int line, int startPosition, int endPosition)
        {
            sci.MBSafeSetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        void FilterResults()
        {
            string defaultGroupTitle = TextHelper.GetString("FlashDevelop.Group.Other");
            string filterText = toolStripTextBoxFilter.Text;
            bool matchInfo = toolStripButtonInfo.Checked;
            bool matchWarnings = toolStripButtonWarning.Checked;
            bool matchErrors = toolStripButtonError.Checked;
            EntriesView.BeginUpdate();
            EntriesView.Items.Clear();
            EntriesView.Groups.Clear();
            foreach (var item in allListViewItems)
            {
                // Is checked?
                int img = item.ImageIndex;
                if (((matchInfo && img == 0) || (matchWarnings && img == 2) || (matchErrors && img == 1))
                    // Contains filter?
                    && (string.IsNullOrEmpty(filterText) || ((Match) item.Tag).Value.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    string groupId = "";
                    string groupTitle = defaultGroupTitle;
                    switch (groupingMethod)
                    {
                        case GroupingMethod.File:
                            string filename = item.SubItems[3].Text;
                            //string fullPath = Path.Combine(item.SubItems[4].Text, filename);
                            groupId = filename;
                            groupTitle = filename;
                            break;

                        case GroupingMethod.Description:
                            string desc = item.SubItems[2].Text;
                            groupId = desc;
                            //Remove character position and other additional information for better grouping
                            string[] split = desc.Split(new[] { " : " }, StringSplitOptions.None);
                            if (split.Length >= 2)
                            {
                                groupId = split[1];
                            }
                            groupTitle = groupId;
                            break;

                        case GroupingMethod.Path:
                            string path = item.SubItems[4].Text;
                            groupId = path;
                            groupTitle = path;
                            break;

                        case GroupingMethod.Type:
                            int type = item.ImageIndex;
                            groupId = type.ToString();
                            switch (type)
                            {
                                case 0:
                                    groupTitle = TextHelper.GetString("Filters.Informations");
                                    break;
                                case 1:
                                    groupTitle = TextHelper.GetString("Filters.Errors");
                                    break;
                                case 2:
                                    groupTitle = TextHelper.GetString("Filters.Warnings");
                                    break;
                            }
                            break;

                        case GroupingMethod.Line:
                            groupId = item.SubItems[1].Text;
                            GroupData = groupId;
                            break;
                    }
                    AddToGroup(item, groupId, groupTitle);
                    EntriesView.Items.Add(item);
                }
            }

            if (!EntriesView.GridLines) // if (PluginBase.Settings.UseListViewGrouping)
            {
                EntriesView.ShowGroups = sortOrder != SortOrder.None
                    && groupingMethod != GroupingMethod.Description && groupingMethod != GroupingMethod.Line; // Do not group by description or line
            }

            if (EntriesView.ShowGroups)
            {
                EntriesView.SortGroups(EntriesView.Columns[lastColumn], sortOrder, (x, y) => string.CompareOrdinal(x.Name, y.Name));
            }
            else
            {
                EntriesView.SortItems(EntriesView.Columns[lastColumn], sortOrder, (x, y) =>
                {
                    var xName = x.Group.Name;
                    var yName = y.Group.Name;
                    if (int.TryParse(xName, out var xInt) && int.TryParse(yName, out var yInt))
                        return xInt.CompareTo(yInt);
                    return string.CompareOrdinal(xName, yName);
                });
            }

            if (EntriesView.Items.Count > 0)
            {
                if (Settings.ScrollToBottom)
                {
                    int last = EntriesView.Items.Count - 1;
                    EntriesView.EnsureVisible(last);
                }
                else EntriesView.EnsureVisible(0);
            }

            EntriesView.EndUpdate();
        }

        /// <summary>
        /// Updates the filter buttons
        /// </summary>
        void UpdateButtons()
        {
            if (buttonsWidth == 0) return;
            if (Width >= buttonsWidth)
            {
                toolStripButtonError.Text = errorCount + " " + TextHelper.GetString("Filters.Errors");
                toolStripButtonWarning.Text = warningCount + " " + TextHelper.GetString("Filters.Warnings");
                toolStripButtonInfo.Text = messageCount + " " + TextHelper.GetString("Filters.Informations");
                toolStripButtonLock.Text = TextHelper.GetString("Label.KeepResults");
            }
            else
            {
                toolStripButtonError.Text = errorCount.ToString();
                toolStripButtonWarning.Text = warningCount.ToString();
                toolStripButtonInfo.Text = messageCount.ToString();
                toolStripButtonLock.Text = "";
            }
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        void AddToGroup(ListViewItem item, string name, string header)
        {
            foreach (ListViewGroup lvg in EntriesView.Groups)
            {
                if (lvg.Name == name)
                {
                    if (!lvg.Items.Contains(item))
                    {
                        lvg.Items.Add(item);
                    }
                    return;
                }
            }
            var group = new ListViewGroup(name, header);
            group.Items.Add(item);
            EntriesView.Groups.Add(group);
        }

        /// <summary>
        /// Add all squiggles
        /// </summary>
        internal void AddSquiggles()
        {
            foreach (ListViewItem item in EntriesView.Items)
            {
                AddSquiggle(item);
            }
        }

        /// <summary>
        /// Squiggle open file
        /// </summary>
        internal void AddSquiggles(string filename)
        {
            foreach (ListViewItem item in EntriesView.Items)
            {
                if (GetFileName(item) == filename)
                {
                    AddSquiggle(item);
                }
            }
        }

        /// <summary>
        /// Adds the results of this ResultsPanel at the specified character position to the given list.
        /// </summary>
        internal void GetResultsAt(List<string> results, ITabbedDocument document, int position) => GetResultsAt(results, document.SciControl, position);

        /// <summary>
        /// Adds the results of this ResultsPanel at the specified character position to the given list.
        /// </summary>
        internal void GetResultsAt(List<string> results, ScintillaControl sci, int position)
        {
            foreach (ListViewItem item in EntriesView.Items)
            {
                var fullPath = Path.Combine(item.SubItems[4].Text, item.SubItems[3].Text);
                if (fullPath != sci.FileName) continue; //item is about different file

                if (item.ImageIndex == 0) continue; //item is only information

                var hasPos = GetPosition(sci, item, out var start, out var end);
                if (!hasPos) continue; //item has no position

                var line = Convert.ToInt32(item.SubItems[1].Text) - 1;
                var lineStart = sci.PositionFromLine(line);
                start += lineStart;
                end += lineStart;

                if (start > position || end < position) continue; //item is not at position
                //suitable result
                var description = item.SubItems[2].Text;

                //remove character positions
                var split = description.Split(new[] { " : " }, StringSplitOptions.None);
                if (split.Length >= 2) description = split[1];

                results.Add(description);
            }
        }

        bool GetPosition(ScintillaControl sci, ListViewItem item, out int start, out int end)
        {
            var line = Convert.ToInt32(item.SubItems[1].Text) - 1;
            var description = item.SubItems[2].Text;

            Match match;
            if ((match = errorCharacters.Match(description)).Success) // "chars {start}-{end}"
            {
                start = Convert.ToInt32(match.Groups["start"].Value);
                end = Convert.ToInt32(match.Groups["end"].Value);
                // An error (!=0) with this pattern is most likely a MTASC error (not multibyte)
                if (item.ImageIndex == 0)
                {
                    // start & end columns are multibyte lengths
                    start = MBSafeColumn(sci, line, start);
                    end = MBSafeColumn(sci, line, end);
                }
            }
            else if ((match = errorCharacter.Match(description)).Success // "char {start}"
                     || (match = errorCharacters2.Match(description)).Success) // "col: {start}"
            {
                start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = MBSafeColumn(sci, line, start);
                end = start + 1;
            }
            else
            {
                start = end = -1;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Squiggle one result
        /// </summary>
        void AddSquiggle(ListViewItem item)
        {
            var sci = DocumentManager.FindDocument(GetFileName(item))?.SciControl;
            if (sci is null)
            {
                return;
            }

            int line = Convert.ToInt32(item.SubItems[1].Text) - 1;

            var hasPos = GetPosition(sci, item, out var start, out var end);
            if (!hasPos) return;

            if (0 <= start && start < end && end <= sci.TextLength)
            {
                int indicator;
                int style;
                int color;
                switch (item.ImageIndex)
                {
                    case 0:
                        indicator = (int) TraceType.Info;
                        style = (int) IndicatorStyle.RoundBox;
                        color = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage).editorstyle.HighlightBackColor;
                        break;
                    case 1:
                        indicator = (int) TraceType.Error;
                        style = (int) IndicatorStyle.Squiggle;
                        color = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage).editorstyle.ErrorLineBack;
                        break;
                    case 2:
                        indicator = (int) TraceType.Warning;
                        style = (int) IndicatorStyle.Squiggle;
                        color = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage).editorstyle.DebugLineBack;
                        break;
                    default:
                        return;
                }
                int position = sci.PositionFromLine(line) + start;
                sci.AddHighlight(indicator, style, color, position, end - start);
            }
        }

        /// <summary> 
        /// Clear all squiggles
        /// </summary>
        internal void ClearSquiggles()
        {
            var cleared = new HashSet<string>();
            foreach (ListViewItem item in EntriesView.Items)
            {
                var fileName = GetFileName(item);
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    var sci = document.SciControl;
                    if (sci is null) continue;
                    if (sci.FileName != fileName) continue;
                    if (!cleared.Contains(fileName))
                    {
                        sci.RemoveHighlights((int) TraceType.Info);
                        sci.RemoveHighlights((int) TraceType.Error);
                        sci.RemoveHighlights((int) TraceType.Warning);
                        cleared.Add(fileName);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Clear and add all squiggles.
        /// </summary>
        void RefreshSquiggles()
        {
            pluginMain.pluginUI.ClearSquiggles();
            if (GroupData != null)
            {
                if (Settings.HighlightOnlyActivePanelEntries)
                {
                    ClearSquiggles();
                    AddSquiggles();
                }
                else
                {
                    foreach (var pluginUI in ResultsPanelHelper.PluginUIs)
                    {
                        if (!pluginUI.ParentPanel.IsHidden)
                        {
                            pluginUI.ClearSquiggles();
                        }
                    }
                    foreach (var pluginUI in ResultsPanelHelper.PluginUIs)
                    {
                        if (!pluginUI.ParentPanel.IsHidden)
                        {
                            pluginUI.AddSquiggles();
                        }
                    }
                }
            }
            pluginMain.pluginUI.AddSquiggles();
        }

        /// <summary>
        /// Get file name from a list view item
        /// </summary>
        static string GetFileName(ListViewItem item) => (item.SubItems[4].Text + "\\" + item.SubItems[3].Text).Replace('/', '\\');

        #endregion

        #region Regular Expressions

        /**
        * Finds if a string contains invalid characters for a path
        */
        static readonly Regex badCharacters = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);

        /**
        * Match standard file entry -- filename:line:description
        * i.e. C:/path/to/src/com/Class.as:15: description
        */
        static readonly Regex fileEntry = new Regex("^(?<filename>([_A-Za-z]:)?[^:*?]+):(?<line>[0-9]+):(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match MXMLC style errors
        * i.e. C:\path\to\src\Class.as(9): description
        * Match TypeScript style errors
        * i.e. C:\path\to\src\Class.as(9,20): description
        */
        static readonly Regex fileEntry2 = new Regex(@"^(?<filename>.*)\((?<line>[0-9,]+)\).?:(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match find in files style ranges
        */
        static readonly Regex lookupRange = new Regex("lookup range[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);

        /**
        * Extract error caret position
        */
        static readonly Regex errorCharacter = new Regex("(character|char)[\\s]+[^0-9]*(?<start>[0-9]+)", RegexOptions.Compiled);
        static readonly Regex errorCharacters = new Regex("(characters|chars)[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);
        static readonly Regex errorCharacters2 = new Regex("col: (?<start>[0-9]+)\\s*", RegexOptions.Compiled);

        #endregion

        #region Entries Navigation

        int entryIndex = -1;

        /// <summary>
        /// Goes to the next entry in the result list.
        /// </summary>
        public bool NextEntry()
        {
            if (EntriesView.Items.Count > 0)
            {
                SelectItem(entryIndex == EntriesView.Items.Count - 1 ? 0 : entryIndex + 1);
                NavigateToSelectedItem();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Goes to the previous entry in the result list.
        /// </summary>
        public bool PreviousEntry()
        {
            if (EntriesView.Items.Count > 0)
            {
                SelectItem(entryIndex <= 0 ? EntriesView.Items.Count - 1 : entryIndex - 1);
                NavigateToSelectedItem();
                return true;
            }
            return false;
        }

        void SelectItem(int index)
        {
            if (0 <= entryIndex && entryIndex < EntriesView.Items.Count)
            {
                EntriesView.Items[entryIndex].ForeColor = EntriesView.ForeColor;
            }
            entryIndex = index;
            EntriesView.SelectedItems.Clear();
            EntriesView.Items[entryIndex].Selected = true;
            EntriesView.Items[entryIndex].ForeColor = PluginBase.MainForm.GetThemeColor("ListView.Highlight", SystemColors.Highlight);
            EntriesView.EnsureVisible(entryIndex);
        }

        void NavigateToSelectedItem()
        {
            if (EntriesView.SelectedItems.Count == 0) return;
            var item = EntriesView.SelectedItems[0];
            if (item is null) return;
            var file = item.SubItems[4].Text + "\\" + item.SubItems[3].Text;
            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            file = PathHelper.GetLongPathName(file);
            if (!File.Exists(file)) return;
            PluginBase.MainForm.OpenEditableDocument(file, false);
            var document = PluginBase.MainForm.CurrentDocument;
            if (document is null || !document.IsEditable) return;
            var sci = document.SciControl;
            if (sci is null) return;
            var line = Convert.ToInt32(item.SubItems[1].Text) - 1;
            var description = item.SubItems[2].Text;
            Match match;
            if ((match = errorCharacters.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                int end = Convert.ToInt32(match.Groups["end"].Value);
                // An error (!=0) with this pattern is most likely a MTASC error (not multibyte)
                if (item.ImageIndex == 0)
                {
                    // start & end columns are multibyte lengths
                    start = MBSafeColumn(sci, line, start);
                    end = MBSafeColumn(sci, line, end);
                }
                int startPosition = sci.PositionFromLine(line) + start;
                int endPosition = sci.PositionFromLine(line) + end;
                SetSelAndFocus(sci, line, startPosition, endPosition);
            }
            else if ((match = errorCharacter.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = MBSafeColumn(sci, line, start);
                int position = sci.PositionFromLine(line) + start;
                SetSelAndFocus(sci, line, position, position);
            }
            else if ((match = errorCharacters2.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = MBSafeColumn(sci, line, start);
                int position = sci.PositionFromLine(line) + start;
                SetSelAndFocus(sci, line, position, position);
            }
            else if ((match = lookupRange.Match(description)).Success)
            {
                // expected: both multibyte lengths
                int start = Convert.ToInt32(match.Groups["start"].Value);
                int end = Convert.ToInt32(match.Groups["end"].Value);
                MBSafeSetSelAndFocus(sci, line, start, end);
            }
            else
            {
                int position = sci.PositionFromLine(line);
                SetSelAndFocus(sci, line, position, position);
            }
        }

        #endregion

    }

    public enum GroupingMethod
    {
        File,
        Type,
        Description,
        Path,
        Line
    }
}
