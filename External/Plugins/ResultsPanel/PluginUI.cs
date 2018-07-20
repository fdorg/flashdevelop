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
        private ListViewEx entriesView;
        private ColumnHeader entryType;
        private ColumnHeader entryLine;
        private ColumnHeader entryDesc;
        private ColumnHeader entryFile;
        private ColumnHeader entryPath;
        private IDictionary<string, bool> ignoredEntries;
        private List<ListViewItem> allListViewItems;
        private ToolStripButton toolStripButtonError;
        private ToolStripButton toolStripButtonWarning;
        private ToolStripButton toolStripButtonInfo;
        private ToolStripButton toolStripButtonLock;
        private ToolStripSpringTextBox toolStripTextBoxFilter;
        private ToolStripLabel toolStripLabelFilter;
        private ToolStripButton clearFilterButton;
        private ToolStrip toolStripFilters;
        private int errorCount;
        private int warningCount;
        private int messageCount;
        private PluginMain pluginMain;
        private int logCount;
        private Timer autoShow;
        private SortOrder sortOrder;
        private int lastColumn;
        private GroupingMethod groupingMethod;
        private int buttonsWidth;
        private Container components;

        private static ImageListManager imageList;

        #region Constructors

        public PluginUI(PluginMain pluginMain) : this(pluginMain, null, null, true, false)
        {

        }

        internal PluginUI(PluginMain pluginMain, string groupData, string groupId, bool showFilterButtons, bool allowMultiplePanels)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            //this.logCount = TraceManager.TraceLog.Count;
            this.logCount = 0;
            this.allListViewItems = new List<ListViewItem>();
            this.ignoredEntries = new Dictionary<String, Boolean>();
            this.errorCount = 0;
            this.warningCount = 0;
            this.messageCount = 0;
            this.sortOrder = SortOrder.Ascending;
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.InitializeTexts();
            this.InitializeLayout();
            ScrollBarEx.Attach(entriesView);

            GroupData = groupData;
            GroupId = groupId;

            buttonsWidth = 0;
            if (allowMultiplePanels)
            {
                buttonsWidth = 200;
                this.toolStripButtonLock.Checked = this.Settings.KeepResultsByDefault;
                this.toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                this.toolStripFilters.Items.Insert(0, this.toolStripButtonLock);
            }
            if (showFilterButtons)
            {
                buttonsWidth = 800;
                this.toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                this.toolStripFilters.Items.Insert(0, this.toolStripButtonError);
                this.toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                this.toolStripFilters.Items.Insert(0, this.toolStripButtonWarning);
                this.toolStripFilters.Items.Insert(0, new ToolStripSeparator());
                this.toolStripFilters.Items.Insert(0, this.toolStripButtonInfo);
            }

            this.entryFile.Tag = GroupingMethod.File;
            this.entryType.Tag = GroupingMethod.Type;
            this.entryDesc.Tag = GroupingMethod.Description;
            this.entryPath.Tag = GroupingMethod.Path;
            this.entryLine.Tag = GroupingMethod.Line;
            this.groupingMethod = Settings.DefaultGrouping;
            this.lastColumn = new Dictionary<GroupingMethod, ColumnHeader>()
            {
                [(GroupingMethod) this.entryFile.Tag] = this.entryFile,
                [(GroupingMethod) this.entryType.Tag] = this.entryType,
                [(GroupingMethod) this.entryDesc.Tag] = this.entryDesc,
                [(GroupingMethod) this.entryPath.Tag] = this.entryPath,
                [(GroupingMethod) this.entryLine.Tag] = this.entryLine
            }[this.groupingMethod].Index;
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
        public bool Locked
        {
            get { return this.toolStripButtonLock.Checked; }
        }

        /// <summary>
        /// Gets the parent <see cref="DockContent"/>.
        /// </summary>
        public DockContent ParentPanel { get; internal set; }

        /// <summary>
        /// Accessor for settings
        /// </summary>
        internal Settings Settings
        {
            get { return (Settings) this.pluginMain.Settings; }
        }

        internal ListViewEx EntriesView
        {
            get { return this.entriesView; }
        }

        internal IDictionary<string, bool> IgnoredEntries
        {
            get { return this.ignoredEntries; }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.entriesView = new ResultsPanel.ListViewEx();
            this.entryType = new System.Windows.Forms.ColumnHeader();
            this.entryLine = new System.Windows.Forms.ColumnHeader();
            this.entryDesc = new System.Windows.Forms.ColumnHeader();
            this.entryFile = new System.Windows.Forms.ColumnHeader();
            this.entryPath = new System.Windows.Forms.ColumnHeader();
            this.toolStripFilters = new PluginCore.Controls.ToolStripEx();
            this.toolStripButtonInfo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWarning = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonError = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLock = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripSpringTextBox();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.autoShow = new System.Windows.Forms.Timer(this.components);
            this.toolStripFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // entriesView
            // 
            this.entriesView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.entriesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.entryType,
            this.entryLine,
            this.entryDesc,
            this.entryFile,
            this.entryPath});
            this.entriesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entriesView.FullRowSelect = true;
            this.entriesView.GridLines = false;
            this.entriesView.Location = new System.Drawing.Point(0, 28);
            this.entriesView.Name = "entriesView";
            this.entriesView.ShowGroups = true;
            this.entriesView.ShowItemToolTips = true;
            this.entriesView.Size = new System.Drawing.Size(710, 218);
            this.entriesView.TabIndex = 1;
            this.entriesView.UseCompatibleStateImageBehavior = false;
            this.entriesView.View = System.Windows.Forms.View.Details;
            this.entriesView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.EntriesView_ColumnClick);
            this.entriesView.DoubleClick += new System.EventHandler(this.EntriesView_DoubleClick);
            this.entriesView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EntriesView_KeyDown);
            // 
            // entryType
            // 
            this.entryType.Text = "!";
            this.entryType.Width = 23;
            // 
            // entryLine
            // 
            this.entryLine.Text = "Line";
            this.entryLine.Width = 55;
            // 
            // entryDesc
            // 
            this.entryDesc.Text = "Description";
            this.entryDesc.Width = 350;
            // 
            // entryFile
            // 
            this.entryFile.Text = "File";
            this.entryFile.Width = 150;
            // 
            // entryPath
            // 
            this.entryPath.Text = "Path";
            this.entryPath.Width = 400;
            // 
            // toolStripFilters
            // 
            this.toolStripFilters.CanOverflow = false;
            this.toolStripFilters.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelFilter,
            this.toolStripTextBoxFilter,
            this.clearFilterButton});
            this.toolStripFilters.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripFilters.Location = new System.Drawing.Point(1, 0);
            this.toolStripFilters.Name = "toolStripFilters";
            this.toolStripFilters.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStripFilters.Size = new System.Drawing.Size(710, 25);
            this.toolStripFilters.TabIndex = 0;
            this.toolStripFilters.Text = "toolStripFilters";
            // 
            // toolStripButtonInfo
            // 
            this.toolStripButtonInfo.Checked = true;
            this.toolStripButtonInfo.CheckOnClick = true;
            this.toolStripButtonInfo.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInfo.Name = "toolStripButtonInfo";
            this.toolStripButtonInfo.Size = new System.Drawing.Size(74, 22);
            this.toolStripButtonInfo.Text = "Information";
            this.toolStripButtonInfo.CheckedChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // toolStripButtonWarning
            // 
            this.toolStripButtonWarning.Checked = true;
            this.toolStripButtonWarning.CheckOnClick = true;
            this.toolStripButtonWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonWarning.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWarning.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonWarning.Name = "toolStripButtonWarning";
            this.toolStripButtonWarning.Size = new System.Drawing.Size(56, 22);
            this.toolStripButtonWarning.Text = "Warning";
            this.toolStripButtonWarning.CheckedChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // toolStripButtonError
            // 
            this.toolStripButtonError.Checked = true;
            this.toolStripButtonError.CheckOnClick = true;
            this.toolStripButtonError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonError.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonError.Name = "toolStripButtonError";
            this.toolStripButtonError.Size = new System.Drawing.Size(36, 22);
            this.toolStripButtonError.Text = "Error";
            this.toolStripButtonError.CheckedChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // toolStripButtonLock
            // 
            this.toolStripButtonLock.Checked = false;
            this.toolStripButtonLock.CheckOnClick = true;
            this.toolStripButtonLock.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.toolStripButtonLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLock.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonLock.Name = "toolStripButtonLock";
            this.toolStripButtonLock.Size = new System.Drawing.Size(74, 22);
            this.toolStripButtonLock.Text = "Keep Results";
            // 
            // toolStripLabelFilter
            //
            this.toolStripLabelFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.toolStripLabelFilter.Name = "toolStripLabelFilter";
            this.toolStripLabelFilter.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabelFilter.Text = "Filter:";
            // 
            // toolStripTextBoxFilter
            //
            this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxFilter.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // clearFilterButton
            //
            this.clearFilterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilterButton.Enabled = false;
            this.clearFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(23, 26);
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButton_Click);
            // 
            // autoShow
            // 
            this.autoShow.Interval = 300;
            this.autoShow.Tick += new System.EventHandler(this.AutoShow_Tick);
            // 
            // PluginUI
            //
            this.Controls.Add(this.entriesView);
            this.Controls.Add(this.toolStripFilters);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(712, 246);
            this.Resize += new System.EventHandler(this.PluginUI_Resize);
            this.toolStripFilters.ResumeLayout(false);
            this.toolStripFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the context menu for entriesView
        /// </summary>
        private void InitializeContextMenu()
        {
            this.entriesView.ContextMenuStrip = this.pluginMain.contextMenuStrip;
        }

        /// <summary>
        /// Initializes the image list for entriesView
        /// </summary>
        private void InitializeGraphics()
        {
            if (imageList == null)
            {
                imageList = new ImageListManager();
                imageList.ColorDepth = ColorDepth.Depth32Bit;
                imageList.TransparentColor = Color.Transparent;
                imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
                imageList.Initialize(ImageList_Populate);
            }

            this.toolStripFilters.Renderer = new DockPanelStripRenderer();
            this.toolStripFilters.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.toolStripFilters.ImageList = imageList;
            this.entriesView.SmallImageList = imageList;
            this.clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            this.toolStripButtonInfo.Image = PluginBase.MainForm.FindImage("131");
            this.toolStripButtonWarning.Image = PluginBase.MainForm.FindImage("196");
            this.toolStripButtonError.Image = PluginBase.MainForm.FindImage("197");
            this.toolStripButtonLock.Image = PluginBase.MainForm.FindImage("246");
            this.entriesView.AddArrowImages();
        }

        /// <summary>
        /// Applies the localized texts to the control
        /// </summary>
        private void InitializeTexts()
        {
            this.entryLine.Text = TextHelper.GetString("Header.Line");
            this.entryDesc.Text = TextHelper.GetString("Header.Description");
            this.entryFile.Text = TextHelper.GetString("Header.File");
            this.entryPath.Text = TextHelper.GetString("Header.Path");
            this.toolStripButtonInfo.Text = "0 " + TextHelper.GetString("Filters.Informations");
            this.toolStripButtonWarning.Text = "0 " + TextHelper.GetString("Filters.Warnings");
            this.toolStripButtonError.Text = "0 " + TextHelper.GetString("Filters.Errors");
            this.toolStripButtonLock.Text = TextHelper.GetString("Label.KeepResults");
            this.toolStripLabelFilter.Text = TextHelper.GetString("Filters.Filter");
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            foreach (ColumnHeader column in entriesView.Columns)
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

            if (entriesView.ShowGroups != showGroups)
            {
                entriesView.ShowGroups = showGroups;
                entriesView.GridLines = !showGroups;
            }
            else
            {
                invalidate = false;
            }

            if (invalidate)
            {
                FilterResults();
            }
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard.
        /// </summary>
        internal bool CopyTextShortcut()
        {
            if (ContainsFocus && entriesView.Focused)
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
            if (ContainsFocus && entriesView.Focused)
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
                entriesView.Items.Clear();
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
            var selectedItems = entriesView.SelectedItems;
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
                foreach (ListViewItem item in entriesView.Items)
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
            foreach (ListViewItem item in entriesView.SelectedItems)
            {
                var match = (Match) item.Tag;
                string entryValue = match.Value;
                if (!ignoredEntries.ContainsKey(entryValue))
                {
                    ignoredEntries.Add(entryValue, false);
                    newIgnoredEntries.Add(item);
                }
            }
            foreach (ListViewItem item in newIgnoredEntries)
            {
                entriesView.Items.Remove(item);
            }
            if (newIgnoredEntries.Count > 0)
            {
                this.RefreshSquiggles();
            }
        }

        /// <summary>
        /// Clears any result entries that are ignored.
        /// </summary>
        internal bool ClearIgnoredEntries()
        {
            if (this.ignoredEntries.Count > 0)
            {
                this.ignoredEntries.Clear();
                this.FilterResults();
                this.RefreshSquiggles();
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
            int count = TraceManager.TraceLog.Count;
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
                var entry = TraceManager.TraceLog[i];
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
                    if (match.Success && !ignoredEntries.ContainsKey(match.Value))
                    {
                        var filename = match.Groups["filename"].Value;
                        if (filename.Length < 3 || badCharacters.IsMatch(filename)) continue;
                        if (project != null && filename[0] != '/' && filename[1] != ':') // relative to project root
                        {
                            filename = PathHelper.ResolvePath(filename, projectDir);
                            if (filename == null) continue;
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
            this.logCount = count;
            if (newResult)
            {
                int startIndex = this.entriesView.Items.Count;
                this.UpdateButtons();
                this.FilterResults();
                for (int i = startIndex; i < this.entriesView.Items.Count; i++)
                {
                    this.AddSquiggle(this.entriesView.Items[i]);
                }
            }
        }

        /// <summary>
        /// Flashes the panel to the user
        /// </summary>
        internal void DisplayOutput()
        {
            this.autoShow.Stop();
            this.autoShow.Start();
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
        private static void ImageList_Populate(object sender, EventArgs e)
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
        private void ToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            this.clearFilterButton.Enabled = this.toolStripTextBoxFilter.Text.Length > 0;
            this.FilterResults();
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            this.toolStripTextBoxFilter.Text = "";
        }

        /// <summary>
        /// If the user presses Enter, dispatch double click
        /// </summary> 
        private void EntriesView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.EntriesView_DoubleClick(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// When the user clicks on a column, group using that column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntriesView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader header = this.entriesView.Columns[e.Column];
            if (header.Tag is GroupingMethod)
            {
                this.groupingMethod = (GroupingMethod) header.Tag;

                if (this.lastColumn == e.Column)
                {
                    switch (this.sortOrder)
                    {
                        case SortOrder.None:
                            this.sortOrder = SortOrder.Ascending;
                            break;
                        case SortOrder.Ascending:
                            this.sortOrder = SortOrder.Descending;
                            break;
                        case SortOrder.Descending:
                            this.sortOrder = SortOrder.None;
                            break;
                    }
                }
                else
                {
                    this.lastColumn = e.Column;
                    this.sortOrder = SortOrder.Ascending;
                }

                this.FilterResults();
            }
        }

        /// <summary>
        /// Update the buttons when the panel resizes
        /// </summary>
        private void PluginUI_Resize(object sender, EventArgs e)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// Opens the file and goes to the match
        /// </summary>
        private void EntriesView_DoubleClick(object sender, EventArgs e)
        {
            if (this.entriesView.SelectedIndices.Count > 0)
            {
                this.SelectItem(this.entriesView.SelectedIndices[0]);
                this.NavigateToSelectedItem();
            }
        }

        /// <summary>
        /// Shows the panel
        /// </summary>
        private void AutoShow_Tick(object sender, EventArgs e)
        {
            this.autoShow.Stop();
            if (this.entriesView.Items.Count > 0)
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
        private int MBSafeColumn(ScintillaControl sci, int line, int length)
        {
            String text = sci.GetLine(line) ?? "";
            length = Math.Min(length, text.Length);
            return sci.MBSafeTextLength(text.Substring(0, length));
        }

        /// <summary>
        /// Goes to the match and ensures that correct fold is opened
        /// </summary>
        private void SetSelAndFocus(ScintillaControl sci, int line, int startPosition, int endPosition)
        {
            sci.SetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Goes to the match and ensures that correct fold is opened
        /// </summary>
        private void MBSafeSetSelAndFocus(ScintillaControl sci, int line, int startPosition, int endPosition)
        {
            sci.MBSafeSetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        private void FilterResults()
        {
            string defaultGroupTitle = TextHelper.GetString("FlashDevelop.Group.Other");
            string filterText = this.toolStripTextBoxFilter.Text;
            bool matchInfo = this.toolStripButtonInfo.Checked;
            bool matchWarnings = this.toolStripButtonWarning.Checked;
            bool matchErrors = this.toolStripButtonError.Checked;
            this.entriesView.BeginUpdate();
            this.entriesView.Items.Clear();
            this.entriesView.Groups.Clear();
            foreach (var item in this.allListViewItems)
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
                    this.AddToGroup(item, groupId, groupTitle);
                    this.entriesView.Items.Add(item);
                }
            }

            if (!this.entriesView.GridLines) // if (PluginBase.Settings.UseListViewGrouping)
            {
                this.entriesView.ShowGroups = this.sortOrder != SortOrder.None
                    && this.groupingMethod != GroupingMethod.Description && this.groupingMethod != GroupingMethod.Line; // Do not group by description or line
            }

            if (this.entriesView.ShowGroups)
            {
                this.entriesView.SortGroups(this.entriesView.Columns[lastColumn], this.sortOrder, (x, y) => string.CompareOrdinal(x.Name, y.Name));
            }
            else
            {
                this.entriesView.SortItems(this.entriesView.Columns[lastColumn], this.sortOrder, (x, y) => string.CompareOrdinal(x.Group.Name, y.Group.Name));
            }

            if (this.entriesView.Items.Count > 0)
            {
                if (this.Settings.ScrollToBottom)
                {
                    int last = this.entriesView.Items.Count - 1;
                    this.entriesView.EnsureVisible(last);
                }
                else this.entriesView.EnsureVisible(0);
            }

            this.entriesView.EndUpdate();
        }

        /// <summary>
        /// Updates the filter buttons
        /// </summary>
        private void UpdateButtons()
        {
            if (this.buttonsWidth == 0) return;
            if (this.Width >= this.buttonsWidth)
            {
                this.toolStripButtonError.Text = errorCount + " " + TextHelper.GetString("Filters.Errors");
                this.toolStripButtonWarning.Text = warningCount + " " + TextHelper.GetString("Filters.Warnings");
                this.toolStripButtonInfo.Text = messageCount + " " + TextHelper.GetString("Filters.Informations");
                this.toolStripButtonLock.Text = TextHelper.GetString("Label.KeepResults");
            }
            else
            {
                this.toolStripButtonError.Text = errorCount.ToString();
                this.toolStripButtonWarning.Text = warningCount.ToString();
                this.toolStripButtonInfo.Text = messageCount.ToString();
                this.toolStripButtonLock.Text = "";
            }
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        private void AddToGroup(ListViewItem item, string name, string header)
        {
            foreach (ListViewGroup lvg in entriesView.Groups)
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
            entriesView.Groups.Add(group);
        }

        /// <summary>
        /// Add all squiggles
        /// </summary>
        internal void AddSquiggles()
        {
            foreach (ListViewItem item in entriesView.Items)
            {
                AddSquiggle(item);
            }
        }

        /// <summary>
        /// Squiggle open file
        /// </summary>
        internal void AddSquiggles(string filename)
        {
            foreach (ListViewItem item in entriesView.Items)
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
        internal void GetResultsAt(List<string> results, ITabbedDocument document, int position)
        {
            foreach (ListViewItem item in EntriesView.Items)
            {
                var fullPath = Path.Combine(item.SubItems[4].Text, item.SubItems[3].Text);
                if (fullPath != document.FileName) continue; //item is about different file

                if (item.ImageIndex == 0) continue; //item is only information

                int start;
                int end;
                var hasPos = GetPosition(document.SciControl, item, out start, out end);
                if (!hasPos) continue; //item has no position

                var line = Convert.ToInt32(item.SubItems[1].Text) - 1;
                var lineStart = document.SciControl.PositionFromLine(line);
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

        private bool GetPosition(ScintillaControl sci, ListViewItem item, out int start, out int end)
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
                    start = this.MBSafeColumn(sci, line, start);
                    end = this.MBSafeColumn(sci, line, end);
                }
            }
            else if ((match = errorCharacter.Match(description)).Success // "char {start}"
                     || (match = errorCharacters2.Match(description)).Success) // "col: {start}"
            {
                start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = this.MBSafeColumn(sci, line, start);
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
        private void AddSquiggle(ListViewItem item)
        {
            var sci = DocumentManager.FindDocument(GetFileName(item))?.SciControl;
            if (sci == null)
            {
                return;
            }

            int line = Convert.ToInt32(item.SubItems[1].Text) - 1;

            int start;
            int end;
            var hasPos = GetPosition(sci, item, out start, out end);
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
            foreach (ListViewItem item in this.entriesView.Items)
            {
                string fileName = GetFileName(item);
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    if (fileName == document.FileName)
                    {
                        if (!cleared.Contains(fileName))
                        {
                            var sci = document.SciControl;
                            sci.RemoveHighlights((int) TraceType.Info);
                            sci.RemoveHighlights((int) TraceType.Error);
                            sci.RemoveHighlights((int) TraceType.Warning);
                            cleared.Add(fileName);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Clear and add all squiggles.
        /// </summary>
        private void RefreshSquiggles()
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
        private static string GetFileName(ListViewItem item)
        {
            return (item.SubItems[4].Text + "\\" + item.SubItems[3].Text).Replace('/', '\\');
        }

        #endregion

        #region Regular Expressions

        /**
        * Finds if a string contains invalid characters for a path
        */
        private static Regex badCharacters = new Regex("[" + Regex.Escape(new String(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);

        /**
        * Match standard file entry -- filename:line:description
        * i.e. C:/path/to/src/com/Class.as:15: description
        */
        private static Regex fileEntry = new Regex("^(?<filename>([_A-Za-z]:)?[^:*?]+):(?<line>[0-9]+):(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match MXMLC style errors
        * i.e. C:\path\to\src\Class.as(9): description
        * Match TypeScript style errors
        * i.e. C:\path\to\src\Class.as(9,20): description
        */
        private static Regex fileEntry2 = new Regex(@"^(?<filename>.*)\((?<line>[0-9,]+)\).?:(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match find in files style ranges
        */
        private static Regex lookupRange = new Regex("lookup range[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);

        /**
        * Extract error caret position
        */
        private static Regex errorCharacter = new Regex("(character|char)[\\s]+[^0-9]*(?<start>[0-9]+)", RegexOptions.Compiled);
        private static Regex errorCharacters = new Regex("(characters|chars)[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);
        private static Regex errorCharacters2 = new Regex("col: (?<start>[0-9]+)\\s*", RegexOptions.Compiled);

        #endregion

        #region Entries Navigation

        private int entryIndex = -1;

        /// <summary>
        /// Goes to the next entry in the result list.
        /// </summary>
        public bool NextEntry()
        {
            if (this.entriesView.Items.Count > 0)
            {
                this.SelectItem(this.entryIndex == this.entriesView.Items.Count - 1 ? 0 : this.entryIndex + 1);
                this.NavigateToSelectedItem();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Goes to the previous entry in the result list.
        /// </summary>
        public bool PreviousEntry()
        {
            if (this.entriesView.Items.Count > 0)
            {
                this.SelectItem(this.entryIndex <= 0 ? this.entriesView.Items.Count - 1 : this.entryIndex - 1);
                this.NavigateToSelectedItem();
                return true;
            }
            return false;
        }

        private void SelectItem(int index)
        {
            if (0 <= this.entryIndex && this.entryIndex < this.entriesView.Items.Count)
            {
                this.entriesView.Items[this.entryIndex].ForeColor = this.entriesView.ForeColor;
            }
            this.entryIndex = index;
            this.entriesView.SelectedItems.Clear();
            this.entriesView.Items[this.entryIndex].Selected = true;
            this.entriesView.Items[this.entryIndex].ForeColor = PluginBase.MainForm.GetThemeColor("ListView.Highlight", SystemColors.Highlight);
            this.entriesView.EnsureVisible(this.entryIndex);
        }

        private void NavigateToSelectedItem()
        {
            if (this.entriesView.SelectedItems.Count == 0) return;
            var item = this.entriesView.SelectedItems[0];
            if (item == null) return;
            string file = item.SubItems[4].Text + "\\" + item.SubItems[3].Text;
            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            file = PathHelper.GetLongPathName(file);
            if (!File.Exists(file)) return;
            PluginBase.MainForm.OpenEditableDocument(file, false);
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (!PluginBase.MainForm.CurrentDocument.IsEditable) return;
            int line = Convert.ToInt32(item.SubItems[1].Text) - 1;
            string description = item.SubItems[2].Text;
            Match match;
            if ((match = errorCharacters.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                int end = Convert.ToInt32(match.Groups["end"].Value);
                // An error (!=0) with this pattern is most likely a MTASC error (not multibyte)
                if (item.ImageIndex == 0)
                {
                    // start & end columns are multibyte lengths
                    start = this.MBSafeColumn(sci, line, start);
                    end = this.MBSafeColumn(sci, line, end);
                }
                int startPosition = sci.PositionFromLine(line) + start;
                int endPosition = sci.PositionFromLine(line) + end;
                this.SetSelAndFocus(sci, line, startPosition, endPosition);
            }
            else if ((match = errorCharacter.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = this.MBSafeColumn(sci, line, start);
                int position = sci.PositionFromLine(line) + start;
                this.SetSelAndFocus(sci, line, position, position);
            }
            else if ((match = errorCharacters2.Match(description)).Success)
            {
                int start = Convert.ToInt32(match.Groups["start"].Value);
                // column is a multibyte length
                start = this.MBSafeColumn(sci, line, start);
                int position = sci.PositionFromLine(line) + start;
                this.SetSelAndFocus(sci, line, position, position);
            }
            else if ((match = lookupRange.Match(description)).Success)
            {
                // expected: both multibyte lengths
                int start = Convert.ToInt32(match.Groups["start"].Value);
                int end = Convert.ToInt32(match.Groups["end"].Value);
                this.MBSafeSetSelAndFocus(sci, line, start, end);
            }
            else
            {
                int position = sci.PositionFromLine(line);
                this.SetSelAndFocus(sci, line, position, position);
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
