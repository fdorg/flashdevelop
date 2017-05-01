using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Controls;
using System.ComponentModel;
using ResultsPanel.Helpers;
using ScintillaNet.Enums;
using Keys = System.Windows.Forms.Keys;

namespace ResultsPanel
{
    public class PluginUI : DockPanelControl
    {
        private ListViewEx entriesView;
        private ColumnHeader entryFile;
        private ColumnHeader entryDesc;
        private ColumnHeader entryLine;
        private ColumnHeader entryPath;
        private ColumnHeader entryType;
        private IDictionary<String, Boolean> ignoredEntries;
        private List<ListViewItem> allListViewItems = new List<ListViewItem>();
        private ToolStripButton toolStripButtonError;
        private ToolStripButton toolStripButtonWarning;
        private ToolStripButton toolStripButtonInfo;
        private ToolStripButton toolStripButtonLock;
        private ToolStripSpringTextBox toolStripTextBoxFilter;
        private ToolStripLabel toolStripLabelFilter;
        private ToolStripButton clearFilterButton;
        private ToolStrip toolStripFilters;
        private Int32 errorCount = 0;
        private Int32 warningCount = 0;
        private Int32 messageCount = 0;
        private PluginMain pluginMain;
        private Int32 logCount;
        private Timer autoShow;
        private SortOrder sortOrder = SortOrder.Ascending;
        private int lastColumn = -1;
        private GroupingMethod groupingMethod;
        private int buttonsWidth;
        private Container components;
        private Dictionary<GroupingMethod, ColumnHeader> groupingMap;

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
            this.ignoredEntries = new Dictionary<String, Boolean>();
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.InitializeTexts();
            this.InitializeLayout();
            ScrollBarEx.Attach(entriesView);

            this.entryFile.Tag = GroupingMethod.File;
            this.entryDesc.Tag = GroupingMethod.Description;
            this.entryType.Tag = GroupingMethod.Type;
            this.entryPath.Tag = GroupingMethod.Path;

            groupingMap = new Dictionary<GroupingMethod, ColumnHeader>()
            {
                [(GroupingMethod) this.entryFile.Tag] = this.entryFile,
                [(GroupingMethod) this.entryDesc.Tag] = this.entryDesc,
                [(GroupingMethod) this.entryType.Tag] = this.entryType,
                [(GroupingMethod) this.entryPath.Tag] = this.entryPath
            };

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
        public DockContent ParentPanel { get; set; }

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
            this.entriesView = new ListViewEx();
            this.entryType = new System.Windows.Forms.ColumnHeader();
            this.entryLine = new System.Windows.Forms.ColumnHeader();
            this.entryDesc = new System.Windows.Forms.ColumnHeader();
            this.entryFile = new System.Windows.Forms.ColumnHeader();
            this.entryPath = new System.Windows.Forms.ColumnHeader();
            this.toolStripFilters = new PluginCore.Controls.ToolStripEx();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonError = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWarning = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInfo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLock = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripSpringTextBox();
            this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            this.autoShow = new System.Windows.Forms.Timer(this.components);
            this.toolStripFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // clearFilterButton
            //
            this.clearFilterButton.Enabled = false;
            this.clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(23, 26);
            this.clearFilterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButton_Click);
            // 
            // entriesView
            // 
            this.entriesView.Dock = DockStyle.Fill;
            this.entriesView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.entriesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.entryType, this.entryLine, this.entryDesc, this.entryFile, this.entryPath });
            this.entriesView.FullRowSelect = true;
            this.entriesView.GridLines = true;
            this.entriesView.Location = new System.Drawing.Point(0, 28);
            this.entriesView.Name = "entriesView";
            this.entriesView.ShowItemToolTips = true;
            this.entriesView.Size = new System.Drawing.Size(710, 218);
            this.entriesView.TabIndex = 1;
            this.entriesView.UseCompatibleStateImageBehavior = false;
            this.entriesView.View = System.Windows.Forms.View.Details;
            this.entriesView.DoubleClick += new System.EventHandler(this.EntriesView_DoubleClick);
            this.entriesView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EntriesView_KeyDown);
            this.entriesView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.EntriesView_ColumnClick);
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
            this.toolStripFilters.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.toolStripFilters.CanOverflow = false;
            this.toolStripFilters.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripFilters.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStripFilters.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelFilter,
            this.toolStripTextBoxFilter,
            this.clearFilterButton});
            this.toolStripFilters.Name = "toolStripFilters";
            this.toolStripFilters.Location = new System.Drawing.Point(1, 0);
            this.toolStripFilters.Size = new System.Drawing.Size(710, 25);
            this.toolStripFilters.TabIndex = 0;
            this.toolStripFilters.Text = "toolStripFilters";
            // 
            // toolStripButtonError
            // 
            this.toolStripButtonError.Checked = true;
            this.toolStripButtonError.CheckOnClick = true;
            this.toolStripButtonError.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonError.Name = "toolStripButtonError";
            this.toolStripButtonError.Size = new System.Drawing.Size(36, 22);
            this.toolStripButtonError.Text = "Error";
            this.toolStripButtonError.CheckedChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // toolStripButtonWarning
            // 
            this.toolStripButtonWarning.Checked = true;
            this.toolStripButtonWarning.CheckOnClick = true;
            this.toolStripButtonWarning.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonWarning.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWarning.Name = "toolStripButtonWarning";
            this.toolStripButtonWarning.Size = new System.Drawing.Size(56, 22);
            this.toolStripButtonWarning.Text = "Warning";
            this.toolStripButtonWarning.CheckedChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
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
            // toolStripButtonLock
            // 
            this.toolStripButtonLock.Checked = false;
            this.toolStripButtonLock.CheckOnClick = true;
            this.toolStripButtonLock.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.toolStripButtonLock.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.toolStripButtonLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLock.Name = "toolStripButtonLock";
            this.toolStripButtonLock.Size = new System.Drawing.Size(74, 22);
            this.toolStripButtonLock.Text = "Keep Results";
            // 
            // toolStripTextBoxFilter
            //
            this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxFilter.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.ToolStripButton_CheckedChanged);
            // 
            // toolStripLabelFilter
            //
            this.toolStripLabelFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.toolStripLabelFilter.Name = "toolStripLabelFilter";
            this.toolStripLabelFilter.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabelFilter.Text = "Filter:";
            // 
            // autoShow
            // 
            this.autoShow.Interval = 300;
            this.autoShow.Tick += new System.EventHandler(this.AutoShow_Tick);
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Resize += this.PluginUI_Resize;
            this.Controls.Add(this.entriesView);
            this.Controls.Add(this.toolStripFilters);
            this.Size = new System.Drawing.Size(712, 246);
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
            this.toolStripFilters.Renderer = new DockPanelStripRenderer();
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

            this.toolStripFilters.ImageList = imageList;
            this.entriesView.SmallImageList = imageList;
            this.clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            this.toolStripButtonInfo.Image = PluginBase.MainForm.FindImage("131");
            this.toolStripButtonError.Image = PluginBase.MainForm.FindImage("197");
            this.toolStripButtonWarning.Image = PluginBase.MainForm.FindImage("196");
            this.toolStripButtonLock.Image = PluginBase.MainForm.FindImage("246");
            this.entriesView.AddArrowImages();
        }

        /// <summary>
        /// Applies the localized texts to the control
        /// </summary>
        private void InitializeTexts()
        {
            this.entryFile.Text = TextHelper.GetString("Header.File");
            this.entryDesc.Text = TextHelper.GetString("Header.Description");
            this.entryLine.Text = TextHelper.GetString("Header.Line");
            this.entryPath.Text = TextHelper.GetString("Header.Path");
            this.toolStripButtonError.Text = "0 " + TextHelper.GetString("Filters.Errors");
            this.toolStripButtonWarning.Text = "0 " + TextHelper.GetString("Filters.Warnings");
            this.toolStripButtonInfo.Text = "0 " + TextHelper.GetString("Filters.Informations");
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

        private static void ImageList_Populate(object sender, EventArgs e)
        {
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("131")); // info
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("197")); // error
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("196")); // warning
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("495")); // up arrow
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("493")); // down arrow
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the settings to the UI.
        /// </summary>
        public void ApplySettings(bool invalidate = true)
        {
            bool useGrouping = PluginBase.Settings.UseListViewGrouping; // Legacy setting - value is now stored in theme
            entriesView.ShowGroups = useGrouping;
            entriesView.GridLines = !useGrouping;

            groupingMethod = Settings.DefaultGrouping;
            lastColumn = groupingMap[groupingMethod].Index;

            if (invalidate)
            {
                FilterResults();
            }

            ClearSquiggles();
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard.
        /// </summary>
        public bool CopyTextShortcut()
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
        public bool IgnoreEntryShortcut()
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
        public bool ClearOutput()
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
        public void CopyText()
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
        public void IgnoreEntry()
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
        public bool ClearIgnoredEntries()
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

        /// <summary>
        /// Adds the log entries to the list view
        /// </summary>
        public void AddLogEntries()
        {
            int count = TraceManager.TraceLog.Count;
            if (count <= this.logCount)
            {
                this.logCount = count;
                return;
            }
            bool newResult = false;
            TraceItem entry; Match match; String description;
            string fileTest; bool inExec; int icon; int state;
            IProject project = PluginBase.CurrentProject;
            string projectDir = project != null ? Path.GetDirectoryName(project.ProjectPath) : "";
            int limit = Math.Min(count, this.logCount + 1000);
            for (int i = this.logCount; i < limit; i++)
            {
                entry = TraceManager.TraceLog[i];
                if (entry.GroupData != this.GroupData)
                {
                    continue;
                }
                if (entry.Message != null && entry.Message.Length > 7 && entry.Message.IndexOf(':') > 0)
                {
                    fileTest = entry.Message.TrimStart();
                    inExec = false;
                    if (fileTest.StartsWithOrdinal("[mxmlc]") || fileTest.StartsWithOrdinal("[compc]") || fileTest.StartsWithOrdinal("[exec]") || fileTest.StartsWithOrdinal("[haxe") || fileTest.StartsWithOrdinal("[java]"))
                    {
                        inExec = true;
                        fileTest = fileTest.Substring(fileTest.IndexOf(']') + 1).TrimStart();
                    }
                    // relative to project root (Haxe)
                    if (fileTest.StartsWithOrdinal("~/")) fileTest = fileTest.Substring(2);
                    match = fileEntry.Match(fileTest);
                    if (!match.Success) match = fileEntry2.Match(fileTest);
                    if (match.Success && !this.ignoredEntries.ContainsKey(match.Value))
                    {
                        string filename = match.Groups["filename"].Value;
                        if (filename.Length < 3 || badCharacters.IsMatch(filename)) continue;
                        if (project != null && filename[0] != '/' && filename[1] != ':') // relative to project root
                        {
                            filename = PathHelper.ResolvePath(filename, projectDir);
                            if (filename == null) continue;
                        }
                        else if (!File.Exists(filename)) continue;
                        FileInfo fileInfo = new FileInfo(filename);
                        description = match.Groups["description"].Value.Trim();
                        state = (inExec) ? -3 : entry.State;
                        // automatic state from message
                        // ie. "2:message" -> state = 2
                        if (state == 1 && description.Length > 2)
                        {
                            if (description[1] == ':' && Char.IsDigit(description[0]))
                            {
                                if (int.TryParse(description[0].ToString(), out state))
                                {
                                    description = description.Substring(2);
                                }
                            }
                        }
                        if (state > 2) icon = 1;
                        else if (state == 2) icon = 2;
                        else if (state == -3) icon = (description.IndexOfOrdinal("Warning") >= 0) ? 2 : 1;
                        else if (description.StartsWith("error", StringComparison.OrdinalIgnoreCase)) icon = 1;
                        else icon = 0;
                        ListViewItem item = new ListViewItem("", icon);
                        item.Tag = match; // Save for later...
                        String matchLine = match.Groups["line"].Value;
                        if (matchLine.IndexOf(',') > 0)
                        {
                            Match split = Regex.Match(matchLine, "([0-9]+),\\s*([0-9]+)");
                            if (!split.Success) continue; // invalid line
                            matchLine = split.Groups[1].Value;
                            description = "col: " + split.Groups[2].Value + " " + description;
                        }
                        item.SubItems.Add(matchLine);
                        item.SubItems.Add(description);
                        item.SubItems.Add(fileInfo.Name);
                        item.SubItems.Add(fileInfo.Directory.ToString());
                        newResult = true;
                        if (icon == 0) this.messageCount++;
                        else if (icon == 1) this.errorCount++;
                        else if (icon == 2) this.warningCount++;
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
        public void DisplayOutput()
        {
            this.autoShow.Stop();
            this.autoShow.Start();
        }

        /// <summary>
        /// Panel is hidden.
        /// </summary>
        public void OnPanelHidden()
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
        public void OnPanelActivated()
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

                if (this.lastColumn != e.Column)
                {
                    this.sortOrder = SortOrder.None;
                }

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

                this.lastColumn = header.Index;
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
            if (this.entriesView.SelectedItems.Count < 1) return;
            ListViewItem item = this.entriesView.SelectedItems[0];
            if (item == null) return;
            String file = item.SubItems[4].Text + "\\" + item.SubItems[3].Text;
            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            file = PathHelper.GetLongPathName(file);
            if (File.Exists(file))
            {
                PluginBase.MainForm.OpenEditableDocument(file, false);
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (!PluginBase.MainForm.CurrentDocument.IsEditable) return;
                Int32 line = Convert.ToInt32(item.SubItems[1].Text) - 1;
                String description = item.SubItems[2].Text;
                Match mcaret = this.errorCharacters.Match(description);
                Match mcaret2 = this.errorCharacter.Match(description);
                Match mcaret3 = this.errorCharacters2.Match(description);
                Match mcaret4 = this.lookupRange.Match(description);
                if (mcaret.Success)
                {
                    Int32 start = Convert.ToInt32(mcaret.Groups["start"].Value);
                    Int32 end = Convert.ToInt32(mcaret.Groups["end"].Value);
                    // An error (!=0) with this pattern is most likely a MTASC error (not multibyte)
                    if (item.ImageIndex == 0)
                    {
                        // start & end columns are multibyte lengths
                        start = this.MBSafeColumn(sci, line, start);
                        end = this.MBSafeColumn(sci, line, end);
                    }
                    Int32 startPosition = sci.PositionFromLine(line) + start;
                    Int32 endPosition = sci.PositionFromLine(line) + end;
                    this.SetSelAndFocus(sci, line, startPosition, endPosition);
                }
                else if (mcaret2.Success)
                {
                    Int32 start = Convert.ToInt32(mcaret2.Groups["start"].Value);
                    // column is a multibyte length
                    start = this.MBSafeColumn(sci, line, start);
                    Int32 position = sci.PositionFromLine(line) + start;
                    this.SetSelAndFocus(sci, line, position, position);
                }
                else if (mcaret3.Success)
                {
                    Int32 start = Convert.ToInt32(mcaret3.Groups["start"].Value);
                    // column is a multibyte length
                    start = this.MBSafeColumn(sci, line, start);
                    Int32 position = sci.PositionFromLine(line) + start;
                    this.SetSelAndFocus(sci, line, position, position);
                }
                else if (mcaret4.Success)
                {
                    // expected: both multibyte lengths
                    Int32 start = Convert.ToInt32(mcaret4.Groups["start"].Value);
                    Int32 end = Convert.ToInt32(mcaret4.Groups["end"].Value);
                    this.MBSafeSetSelAndFocus(sci, line, start, end);
                }
                else
                {
                    Int32 position = sci.PositionFromLine(line);
                    this.SetSelAndFocus(sci, line, position, position);
                }
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
                    string groupTitle = TextHelper.GetString("FlashDevelop.Group.Other");
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
                    }
                    this.AddToGroup(item, groupId, groupTitle);
                    this.entriesView.Items.Add(item);
                }
            }

            if (!this.entriesView.GridLines) // if (PluginBase.Settings.UseListViewGrouping)
            {
                this.entriesView.ShowGroups = this.sortOrder != SortOrder.None;
            }

            this.entriesView.SortGroups(this.entriesView.Columns[lastColumn], this.sortOrder, (x, y) => string.CompareOrdinal(x.Name, y.Name));

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
        private void AddToGroup(ListViewItem item, string groupId, string title)
        {
            foreach (ListViewGroup lvg in entriesView.Groups)
            {
                if (lvg.Name == groupId)
                {
                    if (!lvg.Items.Contains(item))
                    {
                        lvg.Items.Add(item);
                    }
                    return;
                }
            }
            var group = new ListViewGroup(groupId, title);
            group.Items.Add(item);
            entriesView.Groups.Add(group);
        }

        /// <summary>
        /// Add all squiggles
        /// </summary>
        public void AddSquiggles()
        {
            foreach (ListViewItem item in entriesView.Items)
            {
                AddSquiggle(item);
            }
        }

        /// <summary>
        /// Squiggle open file
        /// </summary>
        public void AddSquiggles(string filename)
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
        /// Squiggle one result
        /// </summary>
        private void AddSquiggle(ListViewItem item)
        {
            bool fixIndexes = true;
            Match match = errorCharacters.Match(item.SubItems[2].Text);
            if (match.Success)
            {
                // An error with this pattern is most likely a MTASC error (not multibyte)
                if (item.ImageIndex != 0) fixIndexes = false;
            }
            else match = errorCharacter.Match(item.SubItems[2].Text);
            if (!match.Success) match = errorCharacters2.Match(item.SubItems[2].Text);
            if (match.Success)
            {
                string fileName = GetFileName(item);
                int line = Convert.ToInt32(item.SubItems[1].Text) - 1;
                int style = (int) ((item.ImageIndex == 0) ? IndicatorStyle.RoundBox : IndicatorStyle.Squiggle);
                int indicator = (item.ImageIndex == 0) ? 0 : 2;
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    if (document.IsEditable && fileName == document.FileName)
                    {
                        ScintillaControl sci = document.SciControl;
                        int fore = (item.ImageIndex == 0) ? PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage).editorstyle.HighlightBackColor : 0x000000ff;
                        int end;
                        int start = Convert.ToInt32(match.Groups["start"].Value);
                        // start column is (probably) a multibyte length
                        if (fixIndexes) start = this.MBSafeColumn(sci, line, start);
                        if (match.Groups["end"] != null && match.Groups["end"].Success)
                        {
                            end = Convert.ToInt32(match.Groups["end"].Value);
                            // end column is (probably) a multibyte length
                            if (fixIndexes) end = this.MBSafeColumn(sci, line, end);
                        }
                        else
                        {
                            start = Math.Max(1, Math.Min(sci.LineLength(line) - 1, start));
                            end = start--;
                        }
                        if ((start >= 0) && (end > start) && (end < sci.TextLength))
                        {
                            int position = sci.PositionFromLine(line) + start;
                            sci.AddHighlight(indicator, style, fore, position, end - start);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary> 
        /// Clear all squiggles
        /// </summary>
        private void ClearSquiggles()
        {
            var cleared = new HashSet<string>();
            foreach (ListViewItem item in this.entriesView.Items)
            {
                string fileName = GetFileName(item);
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    var sci = document.SciControl;
                    if (fileName == document.FileName && !cleared.Contains(fileName))
                    {
                        sci.RemoveHighlights((item.ImageIndex == 0) ? 0 : 2);
                        cleared.Add(fileName);
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
        private Regex badCharacters = new Regex("[" + Regex.Escape(new String(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);

        /**
        * Match standard file entry -- filename:line:description
        * i.e. C:/path/to/src/com/Class.as:15: description
        */
        private Regex fileEntry = new Regex("^(?<filename>([_A-Za-z]:)?[^:*?]+):(?<line>[0-9]+):(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match MXMLC style errors
        * i.e. C:\path\to\src\Class.as(9): description
        * Match TypeScript style errors
        * i.e. C:\path\to\src\Class.as(9,20): description
        */
        private Regex fileEntry2 = new Regex(@"^(?<filename>[^(]*)\((?<line>[0-9,]+)\).?:(?<description>.*)$", RegexOptions.Compiled);

        /**
        * Match find in files style ranges
        */
        private Regex lookupRange = new Regex("lookup range[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);

        /**
        * Extract error caret position
        */
        private Regex errorCharacter = new Regex("(character|char)[\\s]+[^0-9]*(?<start>[0-9]+)", RegexOptions.Compiled);
        private Regex errorCharacters = new Regex("(characters|chars)[\\s]+[^0-9]*(?<start>[0-9]+)-(?<end>[0-9]+)", RegexOptions.Compiled);
        private Regex errorCharacters2 = new Regex(@"col: (?<start>[0-9]+)\s*", RegexOptions.Compiled);

        #endregion

        #region Entries Navigation

        private int entryIndex = -1;

        /// <summary>
        /// Goes to the next entry in the result list.
        /// </summary>
        public bool NextEntry()
        {
            if (this.entriesView.Items.Count == 0) return false;
            if (this.entryIndex >= 0 && this.entryIndex < this.entriesView.Items.Count)
            {
                this.entriesView.Items[this.entryIndex].ForeColor = this.entriesView.ForeColor;
            }
            this.entryIndex = (this.entryIndex + 1) % this.entriesView.Items.Count;
            this.entriesView.SelectedItems.Clear();
            this.entriesView.Items[this.entryIndex].Selected = true;
            this.entriesView.Items[this.entryIndex].ForeColor = PluginBase.MainForm.GetThemeColor("ListView.Highlight", SystemColors.Highlight);
            this.entriesView.EnsureVisible(this.entryIndex);
            this.EntriesView_DoubleClick(null, null);
            return true;
        }

        /// <summary>
        /// Goes to the previous entry in the result list.
        /// </summary>
        public bool PreviousEntry()
        {
            if (this.entriesView.Items.Count == 0) return false;
            if (this.entryIndex >= 0 && this.entryIndex < this.entriesView.Items.Count)
            {
                this.entriesView.Items[this.entryIndex].ForeColor = this.entriesView.ForeColor;
            }
            if (--this.entryIndex < 0) this.entryIndex = this.entriesView.Items.Count - 1;
            this.entriesView.SelectedItems.Clear();
            this.entriesView.Items[this.entryIndex].Selected = true;
            this.entriesView.Items[this.entryIndex].ForeColor = PluginBase.MainForm.GetThemeColor("ListView.Highlight", SystemColors.Highlight);
            this.entriesView.EnsureVisible(this.entryIndex);
            this.EntriesView_DoubleClick(null, null);
            return true;
        }

        #endregion

    }

    public enum GroupingMethod
    {
        File,
        Type,
        Description,
        Path
    }
}
