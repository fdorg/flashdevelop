using System;
using System.IO;
using System.Timers;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI;
using PluginCore.FRService;
using PluginCore.Controls;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;
using PluginCore.Helpers;

namespace BookmarkPanel
{
    public class PluginUI : DockPanelControl
    {
        private System.Windows.Forms.ListViewEx listView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ColumnHeader columnLine;
        private System.Windows.Forms.ColumnHeader columnText;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.ToolStripSpringComboBox searchBox;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem removeBookmarksItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private ImageListManager imageList;
        private System.Windows.Forms.Timer updateTimer;
        private TimeoutManager timeoutManager;
        private PluginMain pluginMain;
        
        public PluginUI(PluginMain pluginMain)
        {
            this.AutoKeyHandling = true;
            this.InitializeComponent();
            this.pluginMain = pluginMain;
            this.InitializeTimers();
            this.InitializeGraphics();
            this.InitializeLayout();
            this.InitializeTexts();
            this.UpdateSettings();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.listView = new System.Windows.Forms.ListViewEx();
            this.columnLine = new System.Windows.Forms.ColumnHeader();
            this.columnText = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.removeBookmarksItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.searchBox = new System.Windows.Forms.ToolStripSpringComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLine,
            this.columnText});
            this.listView.LabelWrap = false;
            this.listView.GridLines = true;
            this.listView.ShowItemToolTips = true;
            this.listView.ContextMenuStrip = this.contextMenuStrip;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.listView.HideSelection = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(298, 324);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.ListViewDoubleClick);
            this.listView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ListViewKeyUp);
            // 
            // columnLine
            // 
            this.columnLine.Text = "Line";
            this.columnLine.Width = 55;
            // 
            // columnText
            // 
            this.columnText.Text = "Text";
            this.columnText.Width = 250;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeBookmarksItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(176, 26);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripOpening);
            // 
            // removeBookmarksItem
            //
            this.removeBookmarksItem.Name = "removeBookmarksItem";
            this.removeBookmarksItem.Size = new System.Drawing.Size(175, 22);
            this.removeBookmarksItem.Text = "Remove Bookmarks";
            this.removeBookmarksItem.Click += new System.EventHandler(this.RemoveBookmarksItemClick);
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchButton,
            this.searchBox});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(298, 26);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 1;
            // 
            // searchButton
            //
            this.searchButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.searchButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(23, 22);
            this.searchButton.ToolTipText = "Search And Add Bookmarks";
            this.searchButton.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // searchBox
            //
            this.searchBox.FlatCombo.MaxLength = 200;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(200, 22);
            this.searchBox.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.searchBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchBoxKeyUp);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.Info;
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 25);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(300, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            this.statusLabel.Padding = new Padding(0, 2, 0, 0);
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Controls.Add(this.listView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Size = new System.Drawing.Size(300, 350);
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for MainForm
        /// </summary>
        public static IMainForm MainForm
        {
            get { return PluginBase.MainForm; }
        }

        /// <summary>
        /// Initializes the timers
        /// </summary>
        private void InitializeTimers()
        {
            this.timeoutManager = new TimeoutManager();
            this.updateTimer = new System.Windows.Forms.Timer();
            this.updateTimer.Interval = 500;
            this.updateTimer.Tick += new EventHandler(this.UpdateTimerTick);
            UITools.Manager.OnTextChanged += new UITools.TextChangedHandler(this.ManagerOnTextChanged);
            UITools.Manager.OnMarkerChanged += new UITools.LineEventHandler(this.ManagerOnMarkerChanged);
        }

        /// <summary>
        /// Initializes the localized texts
        /// </summary>
        private void InitializeTexts()
        {
            this.columnLine.Text = TextHelper.GetString("ColumnHeader.Line");
            this.columnText.Text = TextHelper.GetString("ColumnHeader.Text");
            this.searchButton.ToolTipText = TextHelper.GetString("ToolTip.SearchBookmarks");
            this.contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            this.statusLabel.Font = PluginBase.Settings.DefaultFont;
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList = new ImageListManager();
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.Initialize(ImageList_Populate);
            this.listView.SmallImageList = this.imageList;
            this.removeBookmarksItem.Image = PluginBase.MainForm.FindImage("402|4|4|4");
            this.searchButton.Image = PluginBase.MainForm.FindImage("484|26|-4|4");
        }

        private void ImageList_Populate(object sender, EventArgs e)
        {
            this.imageList.Images.Add("Bookmark", PluginBase.MainForm.FindImageAndSetAdjust("559|26|0|1"));
            this.imageList.Images.Add("Info", PluginBase.MainForm.FindImageAndSetAdjust("229"));
            this.imageList.Images.Add("Error", PluginBase.MainForm.FindImageAndSetAdjust("197"));
        }

        /// <summary>
        /// Updates the UI with the settings
        /// </summary>
        public void UpdateSettings()
        {
            Boolean useGrouping = PluginBase.Settings.UseListViewGrouping;
            this.listView.ShowGroups = useGrouping;
            this.listView.GridLines = !useGrouping;
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            this.searchBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.toolStrip.Font = PluginBase.Settings.DefaultFont;
            this.toolStrip.Renderer = new DockPanelStripRenderer();
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.statusStrip.Font = PluginBase.Settings.DefaultFont;
            this.statusStrip.Renderer = new DockPanelStripRenderer();
            this.statusStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            this.contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            this.contextMenuStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        /// <summary>
        /// Removes bookmarks on context menu item clicking
        /// </summary>
        private void RemoveBookmarksItemClick(Object sender, EventArgs e)
        {
            this.DeleteMarkers(false);
        }

        /// <summary>
        /// Starts bookmarks searching on search button ckicking
        /// </summary>
        private void SearchButtonClick(Object sender, EventArgs e)
        {
            this.SearchBookmarks();
        }

        /// <summary>
        /// Removes bookmarks on Delete key
        /// </summary>
        private void ListViewKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.DeleteMarkers(true);
            }
        }

        /// <summary>
        /// Starts bookmarks searching on Enter key
        /// </summary>
        private void SearchBoxKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SearchBookmarks();
            }
        }

        /// <summary>
        /// Double click on an item in the list view
        /// </summary>
        private void ListViewDoubleClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                ListViewItem item = this.listView.SelectedItems[0];
                String filename = item.Group.Name;
                Int32 line = (Int32)item.Tag;
                ITabbedDocument document = DocumentManager.FindDocument(filename);
                if (document != null && document.IsEditable)
                {
                    document.Activate();
                    document.SciControl.GotoLineIndent(line);
                }
            }
        }

        /// <summary>
        /// Updates context menu on opening
        /// </summary>
        private void ContextMenuStripOpening(Object sender, CancelEventArgs e)
        {
            Int32 count = this.listView.SelectedItems.Count;
            if (count > 0) this.removeBookmarksItem.Text = TextHelper.GetString((count > 1) ? "Label.RemoveBookmarks" : "Label.RemoveBookmark");
            else e.Cancel = true;
        }

        /// <summary>
        /// Searches bookmarks by pattern and shows status
        /// </summary>
        private void SearchBookmarks()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                ScintillaControl sci = document.SciControl;
                List<SearchMatch> matches = this.GetResults(sci);
                if (matches != null && matches.Count != 0)
                {
                    this.BookmarkMatches(sci, matches);
                    UITools.Manager.MarkerChanged(sci, -1);
                    this.SetStatus(null);
                }
                else
                {
                    String message = TextHelper.GetString("Info.NothingToBookmark");
                    this.SetStatus(message);
                }
            }
        }

        /// <summary>
        /// Shows or hides status strip message
        /// </summary>
        private void SetStatus(String message)
        {
            if (message != null)
            {
                this.statusStrip.Visible = true;
                this.statusLabel.Text = message;
                this.statusLabel.Image = this.imageList.Images["Info"];
                this.timeoutManager.SetTimeout(this.ClearStatusTimeout, null, 5000);
            }
            else
            {
                this.statusLabel.Image = null;
                this.statusStrip.Visible = false;
                this.statusLabel.Text = String.Empty;
            }
        }

        /// <summary>
        /// Clear status on timeout
        /// </summary>
        private void ClearStatusTimeout(Object tag)
        {
            this.SetStatus(null);
        }

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        private List<SearchMatch> GetResults(ScintillaControl sci)
        {
            if (this.searchBox.Text != String.Empty)
            {
                String pattern = this.searchBox.Text;
                FRSearch search = new FRSearch(pattern);
                search.IsEscaped = false;
                search.WholeWord = false;
                search.NoCase = true;
                search.IsRegex = true;
                search.Filter = SearchFilter.None;
                search.SourceFile = sci.FileName;
                return search.Matches(sci.Text);
            }
            return null;
        }

        /// <summary>
        /// Bookmarks a search match
        /// </summary>
        private void BookmarkMatches(ScintillaControl sci, List<SearchMatch> matches)
        {
            for (Int32 i = 0; i < matches.Count; i++)
            {
                Int32 line = matches[i].Line - 1;
                sci.EnsureVisible(line);
                sci.MarkerAdd(line, 0);
            }
        }

        #endregion

        #region Bookmark List Management

        /// <summary>
        /// Document text changed
        /// </summary>
        private void ManagerOnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            ListViewGroup group = this.FindGroup(sender.FileName);
            if (group == null) return;
            group.Tag = null; // bookmarks list may be dirty
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Document markers changed
        /// </summary>
        private void ManagerOnMarkerChanged(ScintillaControl sender, int line)
        {
            ListViewGroup group = this.FindGroup(sender.FileName);
            if (group == null) return;
            group.Tag = null; // bookmarks list may be dirty
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Check all documents markers
        /// </summary>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            updateTimer.Stop();
            List<ListViewGroup> groups = new List<ListViewGroup>();
            foreach (ListViewGroup group in this.listView.Groups)
            {
                if (group.Tag == null) groups.Add(group);
            }
            foreach (ListViewGroup group in groups)
            {
                UpdateMarkers(group.Name);
            }
        }

        /// <summary>
        /// Update document bookmarks
        /// </summary>
        private void UpdateMarkers(String filename)
        {
            ITabbedDocument document = DocumentManager.FindDocument(filename);
            if (document == null || !document.IsEditable) return;
            ScintillaControl sci = document.SciControl;
            ListViewGroup group = this.FindGroup(document.FileName);
            if (group == null) return;
            List<Int32> markers = this.GetMarkers(document.SciControl);
            if (this.NeedRefresh(document.SciControl, markers, group.Items))
            {
                Int32 index = 0;
                ListViewItem item;
                this.listView.BeginUpdate();
                this.RemoveItemsFromGroup(group);
                ListViewItem[] items = new ListViewItem[markers.Count];
                foreach (Int32 marker in markers)
                {
                    item = new ListViewItem(new String[]{(marker + 1).ToString(), sci.GetLine(marker).Trim()}, 0);
                    item.ToolTipText = sci.GetLine(marker).Trim();
                    item.Name = group.Name;
                    item.Group = group;
                    item.Tag = marker;
                    items[index] = item;
                    index++;
                }
                this.listView.Items.AddRange(items);
                group.Tag = markers;
                this.columnText.Width = -2; // Extend last column
                this.listView.EndUpdate();
            }
        }

        /// <summary>
        /// Checks if bookmark list view needs updating
        /// </summary>
        private Boolean NeedRefresh(ScintillaControl sci, List<Int32> markers, ListView.ListViewItemCollection items)
        {
            if (items.Count != markers.Count) return true;
            foreach (ListViewItem item in items)
            {
                Int32 marker = (Int32)item.Tag;
                if (!markers.Contains(marker)) return true;
                if (sci.GetLine(marker).Trim() != item.SubItems[1].Text) return true;
            }
            return false;
        }

        /// <summary>
        /// Return all the bookmark markers from a scintilla document
        /// </summary>
        private List<Int32> GetMarkers(ScintillaControl sci)
        {
            Int32 line = -1;
            List<Int32> markerLines = new List<Int32>();
            while (line < sci.LineCount)
            {
                line = sci.MarkerNext(line + 1, 1);
                if (line < 0) break;
                markerLines.Add(line);
            }
            return markerLines;
        }

        /// <summary>
        /// Remove from the ListView all the items contained in a ListViewGroup
        /// </summary>
        private void RemoveItemsFromGroup(ListViewGroup group)
        {
            ListViewItem[] items = new ListViewItem[group.Items.Count];
            group.Items.CopyTo(items, 0);
            foreach (ListViewItem item in items) item.Remove();
        }

        /// <summary>
        /// Remove selected bookmarks from opened documents
        /// </summary>
        private void DeleteMarkers(Boolean confirm)
        {
            String message = TextHelper.GetString("Info.RemoveBookmarks");
            String title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (confirm && (MessageBox.Show(title, message, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK))
            {
                return;
            }
            ArrayList deleteItems = new ArrayList();
            foreach (ListViewItem item in this.listView.SelectedItems)
            {
                deleteItems.Add(new KeyValuePair<String,Int32>(item.Group.Name, (Int32)item.Tag));
                item.Group.Tag = null; // dirty
            }
            foreach (KeyValuePair<String,Int32> entry in deleteItems)
            {
                foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
                {
                    if (document.IsEditable && document.FileName == entry.Key)
                    {
                        document.SciControl.MarkerDelete(entry.Value, 0);
                    }
                }
            }
            this.updateTimer.Stop();
            this.updateTimer.Start();
        }

        /// <summary>
        /// Create a new ListViewGroup and assign to the current listview
        /// </summary>
        public void CreateDocument(String filename)
        {
            ListViewGroup group = new ListViewGroup();
            group.Header = Path.GetFileName(filename);
            group.Name = filename;
            this.listView.BeginUpdate();
            this.listView.Groups.Add(group);
            this.listView.EndUpdate();
            this.timeoutManager.SetTimeout(UpdateMarkers, filename);
        }

        /// <summary>
        /// Remove the group and all associated subitems
        /// </summary>
        public void CloseDocument(String filename)
        {
            ListViewGroup group = FindGroup(filename);
            if (group != null)
            {
                this.listView.BeginUpdate();
                this.RemoveItemsFromGroup(group);
                this.listView.Groups.Remove(group);
                this.listView.EndUpdate();
            }
        }

        /// <summary>
        /// Find a group from a given ITabbedDocument
        /// </summary>
        public ListViewGroup FindGroup(String filename)
        {
            foreach (ListViewGroup group in this.listView.Groups)
            {
                if (group.Name == filename) return group;
            }
            return null;
        }

        /// <summary>
        /// Close All active documents/groups
        /// </summary>
        public void CloseAll()
        {
            this.listView.BeginUpdate();
            this.listView.Groups.Clear();
            this.listView.Items.Clear();
            this.listView.EndUpdate();
        }

        #endregion
    }

    #region TimeoutManager

    public class TimeoutManager
    {
        /// <summary>
        /// Method to call on timeout
        /// </summary>
        public delegate void TimeoutDelegate(String tag);

        /// <summary>
        /// Sets the specified timeout
        /// </summary>
        public void SetTimeout(TimeoutDelegate timeoutHandler, String tag)
        {
            this.SetTimeout(timeoutHandler, tag, 200);
        }

        /// <summary>
        /// Waits for timeout and calls method
        /// </summary>
        public void SetTimeout(TimeoutDelegate timeoutHandler, String tag, Int32 timeout)
        {
            TagTimer timer = new TagTimer();
            timer.Interval = timeout;
            timer.Tick += this.TimerElapsed;
            timer.Tag = tag;
            timer.TimeoutHandler = timeoutHandler;
            timer.Start();
        }

        /// <summary>
        /// Handles the elapsed event
        /// </summary>
        private void TimerElapsed(Object sender, EventArgs e)
        {
            TagTimer timer = ((TagTimer)sender);
            timer.Enabled = false;
            timer.Stop();
            timer.TimeoutHandler(timer.Tag as String);
        }

        private class TagTimer : System.Windows.Forms.Timer
        {
            public TimeoutDelegate TimeoutHandler;
        }

    }

    #endregion

}
