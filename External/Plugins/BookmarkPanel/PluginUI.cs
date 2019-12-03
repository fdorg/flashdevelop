// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
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
        ListViewEx listView;
        ToolStrip toolStrip;
        ColumnHeader columnLine;
        ColumnHeader columnText;
        StatusStrip statusStrip;
        ToolStripButton searchButton;
        ToolStripSpringComboBox searchBox;
        ToolStripStatusLabel statusLabel;
        ToolStripMenuItem removeBookmarksItem;
        ContextMenuStrip contextMenuStrip;
        ImageListManager imageList;
        Timer updateTimer;
        TimeoutManager timeoutManager;

        public PluginUI()
        {
            AutoKeyHandling = true;
            InitializeComponent();
            InitializeTimers();
            InitializeGraphics();
            InitializeLayout();
            InitializeTexts();
            UpdateSettings();
            ScrollBarEx.Attach(listView);
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent() 
        {
            listView = new ListViewEx();
            columnLine = new ColumnHeader();
            columnText = new ColumnHeader();
            contextMenuStrip = new ContextMenuStrip();
            removeBookmarksItem = new ToolStripMenuItem();
            toolStrip = new ToolStripEx();
            searchButton = new ToolStripButton();
            searchBox = new ToolStripSpringComboBox();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            contextMenuStrip.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // listView
            // 
            listView.BorderStyle = BorderStyle.None;
            listView.Columns.AddRange(new[] {
            columnLine,
            columnText});
            listView.LabelWrap = false;
            listView.GridLines = true;
            listView.ShowItemToolTips = true;
            listView.ContextMenuStrip = contextMenuStrip;
            listView.Dock = DockStyle.Fill;
            listView.FullRowSelect = true;
            listView.HeaderStyle = ColumnHeaderStyle.Clickable;
            listView.HideSelection = false;
            listView.Name = "listView";
            listView.Size = new Size(298, 324);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.DoubleClick += ListViewDoubleClick;
            listView.KeyUp += ListViewKeyUp;
            // 
            // columnLine
            // 
            columnLine.Text = "Line";
            columnLine.Width = 55;
            // 
            // columnText
            // 
            columnText.Text = "Text";
            columnText.Width = 250;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            removeBookmarksItem});
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(176, 26);
            contextMenuStrip.Opening += ContextMenuStripOpening;
            // 
            // removeBookmarksItem
            //
            removeBookmarksItem.Name = "removeBookmarksItem";
            removeBookmarksItem.Size = new Size(175, 22);
            removeBookmarksItem.Text = "Remove Bookmarks";
            removeBookmarksItem.Click += RemoveBookmarksItemClick;
            // 
            // toolStrip
            // 
            toolStrip.CanOverflow = false;
            toolStrip.Dock = DockStyle.Top;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] {
            searchButton,
            searchBox});
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(1, 1, 2, 2);
            toolStrip.Size = new Size(298, 26);
            toolStrip.Stretch = true;
            toolStrip.TabIndex = 1;
            // 
            // searchButton
            //
            searchButton.Margin = new Padding(0, 1, 0, 1);
            searchButton.Alignment = ToolStripItemAlignment.Right;
            searchButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(23, 22);
            searchButton.ToolTipText = "Search And Add Bookmarks";
            searchButton.Click += SearchButtonClick;
            // 
            // searchBox
            //
            searchBox.FlatCombo.MaxLength = 200;
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(200, 22);
            searchBox.Padding = new Padding(0, 0, 1, 0);
            searchBox.KeyUp += SearchBoxKeyUp;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = SystemColors.Info;
            statusStrip.Dock = DockStyle.Top;
            statusStrip.Items.AddRange(new ToolStripItem[] {
            statusLabel});
            statusStrip.Location = new Point(0, 25);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(300, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 2;
            statusStrip.Visible = false;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 17);
            statusLabel.Padding = new Padding(0, 2, 0, 0);
            // 
            // PluginUI
            //
            Name = "PluginUI";
            Controls.Add(listView);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Size = new Size(300, 350);
            contextMenuStrip.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for MainForm
        /// </summary>
        [Obsolete("Use PluginBase.MainForm")]
        public static IMainForm MainForm => PluginBase.MainForm;

        /// <summary>
        /// Initializes the timers
        /// </summary>
        void InitializeTimers()
        {
            timeoutManager = new TimeoutManager();
            updateTimer = new Timer();
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimerTick;
            UITools.Manager.OnTextChanged += ManagerOnTextChanged;
            UITools.Manager.OnMarkerChanged += ManagerOnMarkerChanged;
        }

        /// <summary>
        /// Initializes the localized texts
        /// </summary>
        void InitializeTexts()
        {
            columnLine.Text = TextHelper.GetString("ColumnHeader.Line");
            columnText.Text = TextHelper.GetString("ColumnHeader.Text");
            searchButton.ToolTipText = TextHelper.GetString("ToolTip.SearchBookmarks");
            contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            statusLabel.Font = PluginBase.Settings.DefaultFont;
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            imageList = new ImageListManager();
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Initialize(ImageList_Populate);
            listView.SmallImageList = imageList;
            removeBookmarksItem.Image = PluginBase.MainForm.FindImage("402|4|4|4");
            searchButton.Image = PluginBase.MainForm.FindImage("484|26|-4|4");
        }

        void ImageList_Populate(object sender, EventArgs e)
        {
            imageList.Images.Add("Bookmark", PluginBase.MainForm.FindImageAndSetAdjust("559|26|0|1"));
            imageList.Images.Add("Info", PluginBase.MainForm.FindImageAndSetAdjust("229"));
            imageList.Images.Add("Error", PluginBase.MainForm.FindImageAndSetAdjust("197"));
        }

        /// <summary>
        /// Updates the UI with the settings
        /// </summary>
        public void UpdateSettings()
        {
            bool useGrouping = PluginBase.Settings.UseListViewGrouping;
            listView.ShowGroups = useGrouping;
            listView.GridLines = !useGrouping;
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        void InitializeLayout()
        {
            searchBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            toolStrip.Font = PluginBase.Settings.DefaultFont;
            toolStrip.Renderer = new DockPanelStripRenderer();
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            statusStrip.Font = PluginBase.Settings.DefaultFont;
            statusStrip.Renderer = new DockPanelStripRenderer();
            statusStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            contextMenuStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        /// <summary>
        /// Removes bookmarks on context menu item clicking
        /// </summary>
        void RemoveBookmarksItemClick(object sender, EventArgs e) => DeleteMarkers(false);

        /// <summary>
        /// Starts bookmarks searching on search button clicking
        /// </summary>
        void SearchButtonClick(object sender, EventArgs e) => SearchBookmarks();

        /// <summary>
        /// Removes bookmarks on Delete key
        /// </summary>
        void ListViewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteMarkers(true);
            }
        }

        /// <summary>
        /// Starts bookmarks searching on Enter key
        /// </summary>
        void SearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchBookmarks();
            }
        }

        /// <summary>
        /// Double click on an item in the list view
        /// </summary>
        void ListViewDoubleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <= 0) return;
            var item = listView.SelectedItems[0];
            var filename = item.Group.Name;
            var line = (int)item.Tag;
            var document = DocumentManager.FindDocument(filename);
            if (document != null && document.SciControl is { } sci)
            {
                document.Activate();
                sci.GotoLineIndent(line);
            }
        }

        /// <summary>
        /// Updates context menu on opening
        /// </summary>
        void ContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            int count = listView.SelectedItems.Count;
            if (count > 0) removeBookmarksItem.Text = TextHelper.GetString((count > 1) ? "Label.RemoveBookmarks" : "Label.RemoveBookmark");
            else e.Cancel = true;
        }

        /// <summary>
        /// Searches bookmarks by pattern and shows status
        /// </summary>
        void SearchBookmarks()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            var matches = GetResults(sci);
            if (!matches.IsNullOrEmpty())
            {
                BookmarkMatches(sci, matches);
                UITools.Manager.MarkerChanged(sci, -1);
                SetStatus(null);
            }
            else
            {
                var message = TextHelper.GetString("Info.NothingToBookmark");
                SetStatus(message);
            }
        }

        /// <summary>
        /// Shows or hides status strip message
        /// </summary>
        void SetStatus(string message)
        {
            if (message != null)
            {
                statusStrip.Visible = true;
                statusLabel.Text = message;
                statusLabel.Image = imageList.Images["Info"];
                timeoutManager.SetTimeout(ClearStatusTimeout, null, 5000);
            }
            else
            {
                statusLabel.Image = null;
                statusStrip.Visible = false;
                statusLabel.Text = string.Empty;
            }
        }

        /// <summary>
        /// Clear status on timeout
        /// </summary>
        void ClearStatusTimeout(object tag) => SetStatus(null);

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        List<SearchMatch> GetResults(ScintillaControl sci)
        {
            if (searchBox.Text.Length == 0) return null;
            var search = new FRSearch(searchBox.Text);
            search.IsEscaped = false;
            search.WholeWord = false;
            search.NoCase = true;
            search.IsRegex = true;
            search.Filter = SearchFilter.None;
            search.SourceFile = sci.FileName;
            return search.Matches(sci.Text);
        }

        /// <summary>
        /// Bookmarks a search match
        /// </summary>
        static void BookmarkMatches(ScintillaControl sci, IEnumerable<SearchMatch> matches)
        {
            foreach (var it in matches)
            {
                int line = it.Line - 1;
                sci.EnsureVisible(line);
                sci.MarkerAdd(line, 0);
            }
        }

        #endregion

        #region Bookmark List Management

        /// <summary>
        /// Document text changed
        /// </summary>
        void ManagerOnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            ListViewGroup group = FindGroup(sender.FileName);
            if (group is null) return;
            group.Tag = null; // bookmarks list may be dirty
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Document markers changed
        /// </summary>
        void ManagerOnMarkerChanged(ScintillaControl sender, int line)
        {
            ListViewGroup group = FindGroup(sender.FileName);
            if (group is null) return;
            group.Tag = null; // bookmarks list may be dirty
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Check all documents markers
        /// </summary>
        void UpdateTimerTick(object sender, EventArgs e)
        {
            updateTimer.Stop();
            List<ListViewGroup> groups = new List<ListViewGroup>();
            foreach (ListViewGroup group in listView.Groups)
            {
                if (group.Tag is null) groups.Add(group);
            }
            foreach (ListViewGroup group in groups)
            {
                UpdateMarkers(group.Name);
            }
        }

        /// <summary>
        /// Update document bookmarks
        /// </summary>
        void UpdateMarkers(string filename)
        {
            var sci = DocumentManager.FindDocument(filename).SciControl;
            if (sci is null) return;
            var group = FindGroup(sci.FileName);
            if (group is null) return;
            var markers = GetMarkers(sci);
            if (!NeedRefresh(sci, markers, group.Items)) return;
            var index = 0;
            listView.BeginUpdate();
            RemoveItemsFromGroup(group);
            var items = new ListViewItem[markers.Count];
            foreach (int marker in markers)
            {
                var item = new ListViewItem(new[]{(marker + 1).ToString(), sci.GetLine(marker).Trim()}, 0);
                item.ToolTipText = sci.GetLine(marker).Trim();
                item.Name = group.Name;
                item.Group = group;
                item.Tag = marker;
                items[index] = item;
                index++;
            }
            listView.Items.AddRange(items);
            group.Tag = markers;
            columnText.Width = -2; // Extend last column
            listView.EndUpdate();
        }

        /// <summary>
        /// Checks if bookmark list view needs updating
        /// </summary>
        static bool NeedRefresh(ScintillaControl sci, ICollection<int> markers, ICollection items)
        {
            if (items.Count != markers.Count) return true;
            foreach (ListViewItem item in items)
            {
                var marker = (int)item.Tag;
                if (!markers.Contains(marker)) return true;
                if (sci.GetLine(marker).Trim() != item.SubItems[1].Text) return true;
            }
            return false;
        }

        /// <summary>
        /// Return all the bookmark markers from a scintilla document
        /// </summary>
        static List<int> GetMarkers(ScintillaControl sci)
        {
            var line = -1;
            var result = new List<int>();
            while (line < sci.LineCount)
            {
                line = sci.MarkerNext(line + 1, 1);
                if (line < 0) break;
                result.Add(line);
            }
            return result;
        }

        /// <summary>
        /// Remove from the ListView all the items contained in a ListViewGroup
        /// </summary>
        static void RemoveItemsFromGroup(ListViewGroup group)
        {
            var items = new ListViewItem[group.Items.Count];
            group.Items.CopyTo(items, 0);
            foreach (var item in items) item.Remove();
        }

        /// <summary>
        /// Remove selected bookmarks from opened documents
        /// </summary>
        void DeleteMarkers(bool confirm)
        {
            var message = TextHelper.GetString("Info.RemoveBookmarks");
            var title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (confirm && MessageBox.Show(title, message, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            var deleteItems = new List<KeyValuePair<string, int>>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                deleteItems.Add(new KeyValuePair<string,int>(item.Group.Name, (int)item.Tag));
                item.Group.Tag = null; // dirty
            }
            foreach (var entry in deleteItems)
            {
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    if (document.SciControl is { } sci && sci.FileName == entry.Key)
                    {
                        sci.MarkerDelete(entry.Value, 0);
                    }
                }
            }
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Create a new ListViewGroup and assign to the current listview
        /// </summary>
        public void CreateDocument(string filename)
        {
            var group = new ListViewGroup();
            group.Header = Path.GetFileName(filename);
            group.Name = filename;
            listView.BeginUpdate();
            listView.Groups.Add(group);
            listView.EndUpdate();
            timeoutManager.SetTimeout(UpdateMarkers, filename);
        }

        /// <summary>
        /// Remove the group and all associated subitems
        /// </summary>
        public void CloseDocument(string filename)
        {
            var group = FindGroup(filename);
            if (group is null) return;
            listView.BeginUpdate();
            RemoveItemsFromGroup(group);
            listView.Groups.Remove(group);
            listView.EndUpdate();
        }

        /// <summary>
        /// Find a group from a given ITabbedDocument
        /// </summary>
        public ListViewGroup FindGroup(string filename)
        {
            foreach (ListViewGroup group in listView.Groups)
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
            listView.BeginUpdate();
            listView.Groups.Clear();
            listView.Items.Clear();
            listView.EndUpdate();
        }

        #endregion
    }

    #region TimeoutManager

    public class TimeoutManager
    {
        /// <summary>
        /// Method to call on timeout
        /// </summary>
        public delegate void TimeoutDelegate(string tag);

        /// <summary>
        /// Sets the specified timeout
        /// </summary>
        public void SetTimeout(TimeoutDelegate timeoutHandler, string tag) => SetTimeout(timeoutHandler, tag, 200);

        /// <summary>
        /// Waits for timeout and calls method
        /// </summary>
        public void SetTimeout(TimeoutDelegate timeoutHandler, string tag, int timeout)
        {
            TagTimer timer = new TagTimer();
            timer.Interval = timeout;
            timer.Tick += TimerElapsed;
            timer.Tag = tag;
            timer.TimeoutHandler = timeoutHandler;
            timer.Start();
        }

        /// <summary>
        /// Handles the elapsed event
        /// </summary>
        void TimerElapsed(object sender, EventArgs e)
        {
            TagTimer timer = ((TagTimer)sender);
            timer.Enabled = false;
            timer.Stop();
            timer.TimeoutHandler(timer.Tag as string);
        }

        class TagTimer : Timer
        {
            public TimeoutDelegate TimeoutHandler;
        }

    }

    #endregion
}