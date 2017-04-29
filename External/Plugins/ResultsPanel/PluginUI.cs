using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Controls;

namespace ResultsPanel
{
    public class PluginUI : DockPanelControl
    {
        public ToolStripMenuItem clearEntriesContextMenuItem;
        public ToolStripMenuItem copyEntryContextMenuItem;
        public ToolStripMenuItem ignoreEntryContextMenuItem;
        public ToolStripMenuItem clearIgnoredEntriesContextMenuItem;
        public ToolStripMenuItem nextEntryContextMenuItem;
        public ToolStripMenuItem previousEntryContextMenuItem;

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
        private ToolStripSpringTextBox toolStripTextBoxFilter;
        private ToolStripButton toolStripButtonInfo;
        private ToolStripLabel toolStripLabelFilter;
        private ToolStripButton clearFilterButton;
        private ToolStrip toolStripFilters;
        private Int32 errorCount = 0;
        private Int32 warningCount = 0;
        private Int32 messageCount = 0;
        private PluginMain pluginMain;
        private Int32 logCount;
        private Timer autoShow;
        private ImageListManager imageList;
        private SortOrder sortOrder = SortOrder.Ascending;
        private int lastColumn = -1;

        private Dictionary<ColumnHeader, GroupingMethod> groupingMap;
        private Dictionary<GroupingMethod, ColumnHeader> reverseGroupingMap;
        private static Dictionary<GroupingMethod, IComparer<ListViewGroup>> groupingComparer;
        private static Dictionary<int, String> levelMap;

        public string Group { get; set; }

        static PluginUI()
        {
            levelMap = new Dictionary<int, String>()
            {
                { 0, TextHelper.GetString("Filters.Informations") },
                { 1, TextHelper.GetString("Filters.Errors") },
                { 2, TextHelper.GetString("Filters.Warnings") }
            };
            groupingComparer = new Dictionary<GroupingMethod, IComparer<ListViewGroup>>()
            {
                { GroupingMethod.File, new FileComparer() },
                { GroupingMethod.Description, new DescriptionComparer() },
                { GroupingMethod.Path, new PathComparer() },
                { GroupingMethod.Type, new TypeComparer() },
            };
        }

        public PluginUI(PluginMain pluginMain, string group) : this(pluginMain)
        {
            this.Group = group;
        }

        public PluginUI(PluginMain pluginMain)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            this.autoShow = new Timer();
            this.autoShow.Interval = 300;
            this.autoShow.Tick += this.AutoShowPanel;
            //this.logCount = TraceManager.TraceLog.Count;
            this.ignoredEntries = new Dictionary<String, Boolean>();
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.InitializeTexts();
            this.InitializeLayout();
            this.ApplySettings();
            ScrollBarEx.Attach(entriesView);

            groupingMap = new Dictionary<ColumnHeader, GroupingMethod>()
            {
                { this.entryFile, GroupingMethod.File },
                { this.entryDesc, GroupingMethod.Description },
                { this.entryType, GroupingMethod.Type },
                { this.entryPath, GroupingMethod.Path }
            };
            reverseGroupingMap = new Dictionary<GroupingMethod, ColumnHeader>();
            foreach (ColumnHeader h in groupingMap.Keys)
            {
                GroupingMethod m = groupingMap[h];
                reverseGroupingMap[m] = h;
            }
        }
        
        #region Windows Forms Designer Generated Code
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
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
            this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripSpringTextBox();
            this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
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
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButtonClick);
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
            this.entriesView.DoubleClick += new System.EventHandler(this.EntriesViewDoubleClick);
            this.entriesView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EntriesViewKeyDown);
            this.entriesView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.EntriesViewColumnClick);
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
            this.toolStripButtonError,
            new ToolStripSeparator(),
            this.toolStripButtonWarning,
            new ToolStripSeparator(),
            this.toolStripButtonInfo,
            new ToolStripSeparator(),
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
            this.toolStripButtonError.CheckedChanged += new System.EventHandler(this.ToolStripButtonErrorCheckedChanged);
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
            this.toolStripButtonWarning.CheckedChanged += new System.EventHandler(this.ToolStripButtonErrorCheckedChanged);
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
            this.toolStripButtonInfo.CheckedChanged += new System.EventHandler(this.ToolStripButtonErrorCheckedChanged);
            // 
            // toolStripTextBoxFilter
            //
            this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxFilter.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.ToolStripButtonErrorCheckedChanged);
            // 
            // toolStripLabelFilter
            //
            this.toolStripLabelFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.toolStripLabelFilter.Name = "toolStripLabelFilter";
            this.toolStripLabelFilter.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabelFilter.Text = "Filter:";
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Resize += this.PluginUIResize;
            this.Controls.Add(this.entriesView);
            this.Controls.Add(this.toolStripFilters);
            this.Size = new System.Drawing.Size(712, 246);
            this.toolStripFilters.ResumeLayout(false);
            this.toolStripFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        #region Methods And Event Handlers

        /// <summary>
        /// Accessor for settings
        /// </summary>
        private Settings Settings
        {
            get { return (Settings)this.pluginMain.Settings; }
        }

        /// <summary>
        /// Initializes the image list for entriesView
        /// </summary>
        public void InitializeGraphics()
        {
            imageList = new ImageListManager();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.TransparentColor = Color.Transparent;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));

            this.toolStripFilters.ImageList = imageList;
            this.entriesView.SmallImageList = imageList;
            this.clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            this.toolStripButtonInfo.Image = PluginBase.MainForm.FindImage("131");
            this.toolStripButtonError.Image = PluginBase.MainForm.FindImage("197");
            this.toolStripButtonWarning.Image = PluginBase.MainForm.FindImage("196");

            imageList.Initialize(ImageList_Populate);
        }

        private void ImageList_Populate(object sender, EventArgs e)
        {
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("131")); // info
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("197")); // error
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("196")); // warning

            this.entriesView.AddArrowImages();
        }

        /// <summary>
        /// Initializes the context menu for entriesView
        /// </summary>
        public void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            this.clearEntriesContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ClearEntries"), null, new EventHandler(this.ClearOutputClick));
            this.copyEntryContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyEntry"), null, new EventHandler(this.CopyTextClick));
            this.ignoreEntryContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.IgnoreEntry"), null, new EventHandler(this.IgnoreEntryClick));
            this.clearIgnoredEntriesContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ClearIgnoredEntries"), null, new EventHandler(this.ClearIgnoredEntriesClick));
            this.nextEntryContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.NextEntry"), null, new EventHandler(this.NextEntryClick));
            this.previousEntryContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.PreviousEntry"), null, new EventHandler(this.PreviousEntryClick));
            
            this.copyEntryContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(PluginMain.CopyEntryKeys);
            this.ignoreEntryContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(PluginMain.IgnoreEntryKeys);
            Keys keys = PluginBase.MainForm.GetShortcutItemKeys("ResultsPanel.ShowNextResult");
            if (keys != Keys.None) this.nextEntryContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(keys);
            keys = PluginBase.MainForm.GetShortcutItemKeys("ResultsPanel.ShowPrevResult");
            if (keys != Keys.None) this.previousEntryContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(keys);

            menu.Items.Add(this.clearEntriesContextMenuItem);
            menu.Items.Add(this.copyEntryContextMenuItem);
            menu.Items.Add(this.ignoreEntryContextMenuItem);
            menu.Items.Add(this.clearIgnoredEntriesContextMenuItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(this.nextEntryContextMenuItem);
            menu.Items.Add(this.previousEntryContextMenuItem);

            this.entriesView.ContextMenuStrip = menu;
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            this.toolStripFilters.Renderer = new DockPanelStripRenderer();
            this.DisableContextMenuItems();
            menu.Opening += ContextMenuOpening;
        }

        /// <summary>
        /// Applies the localized texts to the control
        /// </summary>
        public void InitializeTexts()
        {
            this.entryFile.Text = TextHelper.GetString("Header.File");
            this.entryDesc.Text = TextHelper.GetString("Header.Description");
            this.entryLine.Text = TextHelper.GetString("Header.Line");
            this.entryPath.Text = TextHelper.GetString("Header.Path");
            this.toolStripButtonError.Text = "0 " + TextHelper.GetString("Filters.Errors");
            this.toolStripButtonWarning.Text = "0 " + TextHelper.GetString("Filters.Warnings");
            this.toolStripButtonInfo.Text = "0 " + TextHelper.GetString("Filters.Informations");
            this.toolStripLabelFilter.Text = TextHelper.GetString("Filters.Filter");
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        public void InitializeLayout()
        {
            foreach (ColumnHeader column in entriesView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        /// <summary>
        /// Applies the settings to the UI
        /// </summary>
        public void ApplySettings()
        {
            Boolean useGrouping = PluginBase.Settings.UseListViewGrouping;
            this.entriesView.ShowGroups = useGrouping;
            this.entriesView.GridLines = !useGrouping;
        }

        /// <summary>
        /// Filter the result on check change
        /// </summary>
        private void ToolStripButtonErrorCheckedChanged(Object sender, EventArgs e)
        {
            this.clearFilterButton.Enabled = this.toolStripTextBoxFilter.Text.Trim().Length > 0;
            this.FilterResults(false);
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButtonClick(Object sender, System.EventArgs e)
        {
            this.clearFilterButton.Enabled = false;
            this.toolStripTextBoxFilter.Text = "";
        }

        /// <summary>
        /// Clears the output on click
        /// </summary>
        public void ClearOutputClick(Object sender, System.EventArgs e)
        {
            this.ClearOutput();
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard
        /// </summary>
        public bool CopyTextShortcut()
        {
            if (!ContainsFocus || !entriesView.Focused) return false;
            CopyTextClick(null, null);
            return true;
        }

        /// <summary>
        /// Copies the selected items or all items to clipboard
        /// </summary>
        public void CopyTextClick(Object sender, System.EventArgs e)
        {
            var selectedItems = entriesView.SelectedItems;
            if (selectedItems.Count > 0)
            {
                var copy = string.Empty;
                for (var i = 0; i < selectedItems.Count; i++)
                {
                    var it = selectedItems[i];
                    var m = (Match) it.Tag;
                    copy += m.Value + "\n";
                }
                Clipboard.SetDataObject(copy);
            }
            else
            {
                String copy = String.Empty;
                foreach (ListViewItem item in this.entriesView.Items)
                {
                    Match match = (Match)item.Tag;
                    copy += match.Value + "\n";
                }
                Clipboard.SetDataObject(copy);
            }
        }

        /// <summary>
        /// Clears any result entries that are ignored. Invoked from the context menu.
        /// </summary>
        public void ClearIgnoredEntriesClick(Object sender, System.EventArgs e)
        {
            ClearIgnoredEntries();
        }

        public Boolean ClearIgnoredEntries()
        {
            if (this.ignoredEntries.Count == 0) return false;
            this.ignoredEntries.Clear();
            this.FilterResults(false);
            return true;
        }

        /// <summary>
        /// Ignore entry via shortcut
        /// </summary>
        public Boolean IgnoreEntryShortcut()
        {
            if (!this.ContainsFocus || !this.entriesView.Focused) return false;
            this.IgnoreEntryClick(null, null);
            return true;
        }

        /// <summary>
        /// Ignores the currently selected entries.
        /// </summary>
        public void IgnoreEntryClick(Object sender, System.EventArgs e)
        {
            List<ListViewItem> newIgnoredEntries = new List<ListViewItem>();
            foreach (ListViewItem item in this.entriesView.SelectedItems)
            {
                Match match = (Match)item.Tag;
                string entryValue = match.Value;
                if (!this.ignoredEntries.ContainsKey(entryValue))
                {
                    this.ignoredEntries.Add(entryValue, false);
                    newIgnoredEntries.Add(item);
                }
            }
            foreach (ListViewItem item in newIgnoredEntries)
            {
                this.entriesView.Items.Remove(item);
            }
        }

        /// <summary>
        /// If the user presses Enter, dispatch double click
        /// </summary> 
        private void EntriesViewKeyDown(Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.EntriesViewDoubleClick(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// When the user clicks on a column, group using that column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntriesViewColumnClick(Object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            ColumnHeader h = this.entriesView.Columns[e.Column];
            if (groupingMap.ContainsKey(h))
            {
                Settings.DefaultGrouping = groupingMap[h];

                if (lastColumn != e.Column)
                {
                    sortOrder = SortOrder.None;
                }
				
                switch (sortOrder)
                {
                    case SortOrder.None:
                        sortOrder = SortOrder.Ascending;
                        break;
                    case SortOrder.Ascending:
                        sortOrder = SortOrder.Descending;
                        break;
                    case SortOrder.Descending:
                        sortOrder = SortOrder.None;
                        break;
                }

                lastColumn = e.Column;
                this.FilterResults(false);
                //this.entriesView.SortGroups(h, sortOrder, groupingComparer[Settings.DefaultGrouping]);
            }
        }

        /// <summary>
        /// Update the buttons when the panel resizes
        /// </summary>
        private void PluginUIResize(object sender, EventArgs e)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// When context menu opens, update button enabled states
        /// </summary>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.nextEntryContextMenuItem.Enabled = this.previousEntryContextMenuItem.Enabled = this.clearEntriesContextMenuItem.Enabled = this.entriesView.Items.Count > 0;
            this.ignoreEntryContextMenuItem.Enabled = this.copyEntryContextMenuItem.Enabled = this.entriesView.SelectedItems.Count > 0;
            this.clearIgnoredEntriesContextMenuItem.Enabled = this.ignoredEntries.Count > 0;
        }

        /// <summary>
        /// Opens the file and goes to the match
        /// </summary>
        private void EntriesViewDoubleClick(Object sender, System.EventArgs e)
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
        private void SetSelAndFocus(ScintillaControl sci, Int32 line, Int32 startPosition, Int32 endPosition)
        {
            sci.SetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Goes to the match and ensures that correct fold is opened
        /// </summary>
        private void MBSafeSetSelAndFocus(ScintillaControl sci, Int32 line, Int32 startPosition, Int32 endPosition)
        {
            sci.MBSafeSetSel(startPosition, endPosition);
            sci.EnsureVisibleEnforcePolicy(line);
        }

        /// <summary>
        /// Clears the output
        /// </summary>
        public Boolean ClearOutput()
        {
            if (this.allListViewItems.Count == 0) return false;
            this.ClearSquiggles();
            this.allListViewItems.Clear();
            this.toolStripTextBoxFilter.Text = "";
            this.errorCount = this.messageCount = this.warningCount = 0;
            this.entriesView.Items.Clear();
            this.DisableContextMenuItems();
            this.entryIndex = -1;
            this.UpdateButtons();
            return true;
        }

        /// <summary>
        /// Disables all context menu items
        /// </summary>
        private void DisableContextMenuItems()
        {
            foreach (ToolStripItem item in this.entriesView.ContextMenuStrip.Items)
            {
                item.Enabled = false;
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
        /// Shows the panel
        /// </summary>
        private void AutoShowPanel(Object sender, System.EventArgs e)
        {
            this.autoShow.Stop();
            if (this.entriesView.Items.Count > 0)
            {
                DockContent panel = this.Parent as DockContent;
                DockState ds = panel.VisibleState;
                if (!panel.Visible || ds.ToString().EndsWithOrdinal("AutoHide"))
                {
                    panel.Show();
                    if (ds.ToString().EndsWithOrdinal("AutoHide")) panel.Activate();
                }
            }
        }

        /// <summary>
        /// Adds the log entries to the list view
        /// </summary>
        public void AddLogEntries()
        {
            Int32 count = TraceManager.TraceLog.Count;
            if (count <= this.logCount)
            {
                this.logCount = count;
                return;
            }
            Int32 newResult = -1;
            TraceItem entry; Match match; String description;
            String fileTest; Boolean inExec; Int32 icon; Int32 state;
            IProject project = PluginBase.CurrentProject;
            String projectDir = project != null ? Path.GetDirectoryName(project.ProjectPath) : "";
            Boolean limitMode = (count - this.logCount) > 1000;
            this.entriesView.BeginUpdate();
            for (Int32 i = this.logCount; i < (limitMode ? this.logCount + 1000 : count); i++)
            {
                entry = TraceManager.TraceLog[i];
                if (entry.Group != this.Group)
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
                        if (newResult < 0) newResult = this.entriesView.Items.Count;
                        if (icon == 0) this.messageCount++;
                        else if (icon == 1) this.errorCount++;
                        else if (icon == 2) this.warningCount++;
                        allListViewItems.Add(item);
                    }
                }
            }
            this.logCount = count;
            if (newResult >= 0)
            {
                this.UpdateButtons();
                this.FilterResults(true);
                for (Int32 i = newResult; i < this.entriesView.Items.Count; i++)
                {
                    this.AddSquiggle(this.entriesView.Items[i]);
                }
            }
            this.entriesView.EndUpdate();
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        private void FilterResults(bool locked)
        {
            if (!locked) this.entriesView.BeginUpdate();
            String filterText = this.toolStripTextBoxFilter.Text.ToLower();
            Boolean matchInfo = this.toolStripButtonInfo.Checked;
            Boolean matchWarnings = this.toolStripButtonWarning.Checked;
            Boolean matchErrors = this.toolStripButtonError.Checked;
            this.entriesView.Items.Clear();
            this.entriesView.Groups.Clear();
            foreach (ListViewItem it in this.allListViewItems)
            {
                // Is checked?
                Int32 img = it.ImageIndex;
                if (((matchInfo && img == 0) || (matchWarnings && img == 2) || (matchErrors && img == 1))
                    // Contains filter?
                    && (filterText == "" || ((Match)it.Tag).Value.ToLower().Contains(filterText)))
                {
                    if (PluginBase.Settings.UseListViewGrouping)
                    {
                        string groupTitle = TextHelper.GetString("FlashDevelop.Group.Other");
                        string groupId = "";
                        switch (Settings.DefaultGrouping)
                        {
                            case GroupingMethod.File:
                                String filename = it.SubItems[3].Text;
                                String fullPath = Path.Combine(it.SubItems[4].Text, filename);
                                if (File.Exists(fullPath)) groupTitle = filename;

                                groupId = fullPath;
                                break;
                            case GroupingMethod.Description:
                                String desc = it.SubItems[2].Text;
                                groupId = desc;
                                //Remove character position and other additional information for better grouping
                                String[] split = desc.Split(new string[] { " : " }, StringSplitOptions.None);
                                if (split.Length >= 2)
                                {
                                    groupId = split[1];
                                }
                                groupTitle = groupId;
                                break;
                            case GroupingMethod.Path:
                                String path = it.SubItems[4].Text;
                                String dirname = new DirectoryInfo(path).Name;

                                groupId = path;
                                groupTitle = dirname;
                                break;
                            case GroupingMethod.Type:
                                int type = it.ImageIndex;
                                groupId = type.ToString();
                                groupTitle = levelMap[type];
                                break;
                        }
                        this.AddToGroup(it, groupId, groupTitle);
                    }
                    this.entriesView.Items.Add(it);
                }
            }

            if (sortOrder == SortOrder.None)
            {
                this.entriesView.ShowGroups = false;
            }
            else
            {
                lastColumn = reverseGroupingMap[Settings.DefaultGrouping].Index;
                this.entriesView.ShowGroups = true;
            }

            if (lastColumn != -1)
            {
                this.entriesView.SortGroups(this.entriesView.Columns[lastColumn], sortOrder, groupingComparer[Settings.DefaultGrouping]);
            }

            if (this.entriesView.Items.Count > 0)
            {
                if (this.Settings.ScrollToBottom)
                {
                    Int32 last = this.entriesView.Items.Count - 1;
                    this.entriesView.EnsureVisible(last);
                }
                else this.entriesView.EnsureVisible(0);
            }
            if (!locked) this.entriesView.EndUpdate();
        }

        /// <summary>
        /// Updates the filter buttons
        /// </summary>
        private void UpdateButtons()
        {
            this.toolStripButtonError.Text = errorCount.ToString();
            this.toolStripButtonWarning.Text = warningCount.ToString();
            this.toolStripButtonInfo.Text = messageCount.ToString();
            if (this.Width >= 800)
            {
                this.toolStripButtonError.Text += " " + TextHelper.GetString("Filters.Errors");
                this.toolStripButtonWarning.Text += " " + TextHelper.GetString("Filters.Warnings");
                this.toolStripButtonInfo.Text += " " + TextHelper.GetString("Filters.Informations");
            }
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        private void AddToGroup(ListViewItem item, String id, String title)
        {
            Boolean found = false;
            ListViewGroup gp = null;

            foreach (ListViewGroup lvg in this.entriesView.Groups)
            {
                if (lvg.Tag.ToString() == id)
                {
                    found = true;
                    gp = lvg;
                    break;
                }
            }
            if (found)
            {
                if (!gp.Items.Contains(item))
                {
                    gp.Items.Add(item);
                }
            }
            else
            {
                gp = new ListViewGroup();
                gp.Tag = id;
                gp.Header = title;
                this.entriesView.Groups.Add(gp);
                gp.Items.Add(item);
            }
        }

        /// <summary>
        /// Squiggle open file
        /// </summary>
        public void AddSquiggles(String filename)
        {
            String fname;
            if (this.entriesView.Items.Count > 0)
            foreach(ListViewItem item in this.entriesView.Items)
            {
                fname = (item.SubItems[4].Text + "\\" + item.SubItems[3].Text).Replace('/','\\');
                if (fname == filename) AddSquiggle(item);
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
                String fname = (item.SubItems[4].Text + "\\" + item.SubItems[3].Text).Replace('/','\\').Trim();
                Int32 line = Convert.ToInt32(item.SubItems[1].Text) - 1;
                ITabbedDocument[] documents = PluginBase.MainForm.Documents;
                foreach (ITabbedDocument document in documents)
                {
                    if (!document.IsEditable) continue;
                    ScintillaControl sci = document.SciControl;
                    Language language = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
                    Int32 style = (item.ImageIndex == 0) ? (Int32)ScintillaNet.Enums.IndicatorStyle.RoundBox : (Int32)ScintillaNet.Enums.IndicatorStyle.Squiggle;
                    Int32 fore = (item.ImageIndex == 0) ? language.editorstyle.HighlightBackColor : 0x000000ff;
                    Int32 indic = (item.ImageIndex == 0) ? 0 : 2;
                    if (fname == document.FileName)
                    {
                        Int32 end;
                        Int32 start = Convert.ToInt32(match.Groups["start"].Value);
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
                            Int32 position = sci.PositionFromLine(line) + start;
                            sci.AddHighlight(indic, style, fore, position, end - start);
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
            String fname; ScintillaControl sci;
            ArrayList cleared = new ArrayList();
            ITabbedDocument[] documents = PluginBase.MainForm.Documents;
            foreach (ITabbedDocument document in documents)
            {
                foreach (ListViewItem item in this.entriesView.Items)
                {
                    sci = document.SciControl;
                    fname = (item.SubItems[4].Text + "\\" + item.SubItems[3].Text).Replace('/','\\');
                    if (fname == document.FileName && !cleared.Contains(fname))
                    {
                        Int32 indic = (item.ImageIndex == 0) ? 0 : 2;
                        sci.RemoveHighlights(indic);
                        cleared.Add(fname);
                        break;
                    }
                }
            }
        }
        
        #endregion

        #region Regular Expressions

        /**
        * Finds if a string contains invalid characters for a path
        */ 
        private Regex badCharacters = new Regex("[" + Regex.Escape(new String(System.IO.Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);

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

        private Int32 entryIndex = -1;

        /// <summary>
        /// Goes to the next entry in the result list.
        /// </summary>
        public void NextEntryClick(Object sender, System.EventArgs e)
        {
            NextEntry();
        }

        /// <summary>
        /// Goes to the next entry in the result list.
        /// </summary>
        public Boolean NextEntry()
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
            this.EntriesViewDoubleClick(null, null);
            return true;
        }

        /// <summary>
        /// Goes to the previous entry in the result list.
        /// </summary>
        public void PreviousEntryClick(Object sender, System.EventArgs e)
        {
            PreviousEntry();
        }

        /// <summary>
        /// Goes to the previous entry in the result list.
        /// </summary>
        public Boolean PreviousEntry()
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
            this.EntriesViewDoubleClick(null, null);
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
