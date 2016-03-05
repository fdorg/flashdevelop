using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore.Controls;

namespace TaskListPanel
{
    public class PluginUI : DockPanelControl, IEventHandler
    {  
        private Int32 totalFiles;
        private Int32 currentPos;
        private List<String> groups;
        private Int32 processedFiles;
        private PluginMain pluginMain;
        private String currentFileName;
        private List<String> extensions;
        private Regex todoParser = null;
        private Boolean isEnabled = false;
        private Boolean refreshEnabled = false;
        private System.Windows.Forms.Timer parseTimer;
        private Boolean firstExecutionCompleted = false;
        private Dictionary<String, DateTime> filesCache;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem refreshButton;
        private ToolStripLabel toolStripLabel;
        private ListViewSorter columnSorter;
        private StatusStrip statusStrip;
        private ColumnHeader columnPos;
        private ColumnHeader columnType;
        private ColumnHeader columnIcon;
        private ColumnHeader columnText;
        private ColumnHeader columnName;
        private ColumnHeader columnPath;
        private BackgroundWorker bgWork;
        private ListViewEx listView;
        private ImageListManager imageList;

        // Regex
        static private Regex reClean = new Regex(@"(\*)?\*/.*", RegexOptions.Compiled);

        public PluginUI(PluginMain pluginMain)
        {
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeLocalization();
            this.InitializeLayout();
            this.pluginMain = pluginMain;
            this.groups = new List<String>();
            this.columnSorter = new ListViewSorter();
            this.listView.ListViewItemSorter = this.columnSorter;
            Settings settings = (Settings)pluginMain.Settings;
            this.filesCache = new Dictionary<String, DateTime>();
            EventManager.AddEventHandler(this, EventType.Keys); // Listen Esc
            try
            {
                if (settings.GroupValues.Length > 0)
                {
                    this.groups.AddRange(settings.GroupValues);
                    this.todoParser = BuildRegex(String.Join("|", settings.GroupValues));
                    this.isEnabled = true;
                    this.InitGraphics();
                }
                this.listView.ShowGroups = PluginBase.Settings.UseListViewGrouping;
                this.listView.GridLines = !PluginBase.Settings.UseListViewGrouping;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                this.isEnabled = false;
            }
            this.parseTimer = new System.Windows.Forms.Timer();
            this.parseTimer.Interval = 2000;
            this.parseTimer.Tick += delegate { this.ParseNextFile(); };
            this.parseTimer.Enabled = false;
            this.parseTimer.Tag = null;
            ScrollBarEx.Attach(listView);
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
            this.columnIcon = new System.Windows.Forms.ColumnHeader();
            this.columnPos = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnText = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPath = new System.Windows.Forms.ColumnHeader();
            this.toolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIcon,
            this.columnPos,
            this.columnType,
            this.columnText,
            this.columnName,
            this.columnPath});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.LabelWrap = false;
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(278, 330);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.ListViewDoubleClick);
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewColumnClick);
            this.listView.KeyPress += new KeyPressEventHandler(this.ListViewKeyPress);
            // 
            // columnIcon
            // 
            this.columnIcon.Text = "!";
            this.columnIcon.Width = 23;
            // 
            // columnPos
            // 
            this.columnPos.Text = "Position";
            this.columnPos.Width = 75;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 70;
            // 
            // columnText
            // 
            this.columnText.Text = "Description";
            this.columnText.Width = 350;
            // 
            // columnName
            // 
            this.columnName.Text = "File";
            this.columnName.Width = 150;
            // 
            // columnPath
            // 
            this.columnPath.Text = "Path";
            this.columnPath.Width = 400;
            // 
            // toolStripLabel
            // 
            this.toolStripLabel.Name = "toolStripLabel";
            this.toolStripLabel.Size = new System.Drawing.Size(0, 20);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.toolStripLabel});
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip.Renderer = new DockPanelStripRenderer(false);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(278, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Controls.Add(this.listView);
            this.Controls.Add(this.statusStrip);
            this.Size = new System.Drawing.Size(280, 352);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            foreach (ColumnHeader column in listView.Columns)
                column.Width = ScaleHelper.Scale(column.Width);
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// 
        /// </summary>
        private Regex BuildRegex(String pattern)
        {
            return new Regex(@"(//|\*)[\t ]*(" + pattern + @")[:\t ]+(.*)", RegexOptions.Multiline);
        }

        /// <summary>
        /// Initialises the list view's context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            this.contextMenu = new ContextMenuStrip();
            this.contextMenu.Renderer = new DockPanelStripRenderer(false);
            this.contextMenu.Font = PluginBase.Settings.DefaultFont;
            this.statusStrip.Font = PluginBase.Settings.DefaultFont;
            Image image = PluginBase.MainForm.FindImage("66");
            String label = TextHelper.GetString("FlashDevelop.Label.Refresh");
            this.refreshButton = new ToolStripMenuItem(label, image, new EventHandler(this.RefreshButtonClick));
            this.contextMenu.Items.Add(this.refreshButton);
            this.listView.ContextMenuStrip = this.contextMenu;
        }

        /// <summary>
        /// Initializes the localized texts
        /// </summary>
        private void InitializeLocalization()
        {
            this.columnType.Text = TextHelper.GetString("Column.Type");
            this.columnPos.Text = TextHelper.GetString("Column.Position");
            this.columnText.Text = TextHelper.GetString("Column.Description");
            this.columnName.Text = TextHelper.GetString("Column.File");
            this.columnPath.Text = TextHelper.GetString("Column.Path");
        }

        /// <summary>
        /// Update extensions and search pattern
        /// </summary>
        public void UpdateSettings()
        {
            Settings settings = ((Settings)pluginMain.Settings);
            this.groups.Clear();
            this.isEnabled = false;
            try
            {
                if (settings.GroupValues.Length > 0)
                {
                    this.groups.AddRange(settings.GroupValues);
                    this.todoParser = BuildRegex(String.Join("|", settings.GroupValues));
                    this.isEnabled = true;
                    this.InitGraphics();
                }
                else this.isEnabled = false;
                this.listView.ShowGroups = PluginBase.Settings.UseListViewGrouping;
                this.listView.GridLines = !PluginBase.Settings.UseListViewGrouping;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                this.isEnabled = false;
            }
        }

        /// <summary>
        /// While parsing project files we need to disable the refresh button
        /// </summary>
        public Boolean RefreshEnabled
        {
            get { return this.refreshEnabled; }
            set
            {
                this.refreshEnabled = value;
                this.refreshButton.Enabled = value;
            }
        }

        /// <summary>
        /// Stops the parse timer if not enabled.
        /// </summary>
        public void Terminate()
        {
            if (this.parseTimer.Enabled) this.parseTimer.Stop();
        }

        /// <summary>
        /// Get all available files with extension matches, filters out hidden paths.
        /// </summary>
        private List<String> GetFiles(String path, ExplorationContext context)
        {
            List<String> files = new List<String>();
            foreach (String extension in this.extensions)
            {
                String[] allFiles = Directory.GetFiles(path, "*" + extension);
                files.AddRange(allFiles);
                foreach (String file in allFiles)
                {
                    foreach (String hidden in context.HiddenPaths)
                    {
                        if (file.StartsWith(hidden, StringComparison.OrdinalIgnoreCase))
                        {
                            files.Remove(file);
                        }
                    }
                }
            }
            foreach (String dir in Directory.GetDirectories(path))
            {
                if (context.Worker.CancellationPending) return new List<String>();
                Thread.Sleep(5);
                if (this.ShouldBeScanned(dir, context.ExcludedPaths))
                {
                    files.AddRange(GetFiles(dir, context));
                }
            }
            return files;
        }

        /// <summary>
        /// Get all available files with extension match
        /// </summary>
        private List<String> GetFiles(ExplorationContext context)
        {
            List<String> files = new List<String>();
            foreach (String path in context.Directories)
            {
                if (context.Worker.CancellationPending) return new List<String>();
                Thread.Sleep(5);
                try
                {
                    if (this.ShouldBeScanned(path, context.ExcludedPaths))
                    {
                        files.AddRange(this.GetFiles(path, context));
                    }
                }
                catch {}
            }
            return files;
        }

        /// <summary>
        /// Checks if the path should be scanned for tasks
        /// </summary>
        private Boolean ShouldBeScanned(String path, string[] excludedPaths)
        {
            String name = Path.GetFileName(path);
            if ("._- ".IndexOf(name[0]) >= 0) return false;
            foreach (String exclude in excludedPaths)
            {
                if (!Directory.Exists(path) || path.StartsWith(exclude, StringComparison.OrdinalIgnoreCase)) return false;
            }
            return true;
        }

        /// <summary>
        /// When a new project is opened recreate the ui
        /// </summary>
        public void InitProject()
        {
            this.currentPos = -1;
            this.currentFileName = null;
            if (!this.isEnabled) return;
            this.listView.Items.Clear();
            this.filesCache.Clear();
            Settings settings = (Settings)pluginMain.Settings;
            if (PluginBase.CurrentProject != null && settings.ExploringMode == ExploringMode.Complete)
            {
                this.RefreshProject();
            }
            else
            {
                this.parseTimer.Enabled = false;
                this.parseTimer.Stop();
                if (!this.InvokeRequired) this.toolStripLabel.Text = "";
            }
        }

        /// <summary>
        /// Refresh the current project parsing all files
        /// </summary>
        private void RefreshProject()
        {
            this.currentPos = -1;
            this.currentFileName = null;
            if (this.isEnabled && PluginBase.CurrentProject != null)
            {
                this.RefreshEnabled = false;
                // Stop current exploration
                if (this.parseTimer.Enabled) this.parseTimer.Stop();
                this.parseTimer.Tag = null;
                if (bgWork != null && bgWork.IsBusy) bgWork.CancelAsync();
                IProject project = PluginBase.CurrentProject;
                ExplorationContext context = new ExplorationContext();
                Settings settings = (Settings)this.pluginMain.Settings;
                context.ExcludedPaths = (String[])settings.ExcludedPaths.Clone();
                context.Directories = (String[])project.SourcePaths.Clone();
                for (Int32 i = 0; i < context.Directories.Length; i++)
                {
                    context.Directories[i] = project.GetAbsolutePath(context.Directories[i]);
                }
                context.HiddenPaths = project.GetHiddenPaths();
                for (Int32 i = 0; i < context.HiddenPaths.Length; i++)
                {
                    context.HiddenPaths[i] = project.GetAbsolutePath(context.HiddenPaths[i]);
                }
                GetExtensions();
                bgWork = new BackgroundWorker();
                context.Worker = bgWork;
                bgWork.WorkerSupportsCancellation = true;
                bgWork.DoWork += new DoWorkEventHandler(bgWork_DoWork);
                bgWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWork_RunWorkerCompleted);
                bgWork.RunWorkerAsync(context);
                String message = TextHelper.GetString("Info.Refreshing");
                this.toolStripLabel.Text = message;
            }
        }

        private void GetExtensions()
        {
            Settings settings = (Settings)pluginMain.Settings;
            this.extensions = new List<String>();
            foreach (String ext in settings.FileExtensions)
            {
                if (!String.IsNullOrEmpty(ext))
                {
                    if (!ext.StartsWith('*')) this.extensions.Add("*" + ext);
                    else this.extensions.Add(ext);
                }
            }
            String[] addExt = ASContext.Context.GetExplorerMask();
            if (addExt != null && addExt.Length > 0) extensions.AddRange(addExt);
        }

        /// <summary>
        /// 
        /// </summary>
        private void bgWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;
            ExplorationContext context = e.Result as ExplorationContext;
            this.parseTimer.Tag = context;
            this.parseTimer.Interval = 2000;
            this.parseTimer.Enabled = true;
            this.parseTimer.Start();
            this.totalFiles = context.Files.Count;
            this.processedFiles = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void bgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            ExplorationContext context = e.Argument as ExplorationContext;
            context.Files = this.GetFiles(context);
            if (context.Worker.CancellationPending) e.Cancel = true;
            else e.Result = context;
        }

        /// <summary>
        /// At startup parse all opened files
        /// </summary>
        private void ParseNextFile()
        {
            String path;
            Int32 status;
            ExplorationContext context;
            if (this.parseTimer.Tag is ExplorationContext)
            {
                context = (ExplorationContext)this.parseTimer.Tag;
                status = context.Status;
                if (status == 0)
                {
                    context.Status = 1;
                    this.parseTimer.Interval = 10;
                }
                else if (status == 1)
                {
                    List<string> files = context.Files;
                    if (files != null && files.Count > 0)
                    {
                        Boolean parseFile = false;
                        path = files[0];
                        DateTime lastWriteTime = new FileInfo(path).LastWriteTime;
                        if (!this.filesCache.ContainsKey(path))
                        {
                            this.filesCache[path] = lastWriteTime;
                            parseFile = true;
                        }
                        else
                        {
                            if (this.filesCache[path] != lastWriteTime) parseFile = true;
                        }
                        files.RemoveAt(0);
                        if (parseFile) this.ParseFile(path);
                        this.processedFiles++;
                        String message = TextHelper.GetString("Info.Processing");
                        this.toolStripLabel.Text = String.Format(message, processedFiles, totalFiles);
                        this.refreshButton.Enabled = false;
                    }
                    else context.Status = 2;
                }
                else this.ParseTimerCompleted();
            }
            else this.toolStripLabel.Text = "";
        }

        /// <summary>
        /// Parse timer completed parsing all files
        /// </summary>
        private void ParseTimerCompleted()
        {
            this.parseTimer.Enabled = false;
            this.parseTimer.Stop();
            this.RefreshEnabled = true;
            this.toolStripLabel.Text = "";
            if (!this.firstExecutionCompleted)
            {
                EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.FileSave);
            }
            this.firstExecutionCompleted = true;
        }

        /// <summary>
        /// Parse a file adding all the found Matches into the listView
        /// </summary>
        private void ParseFile(String path)
        {
            if (!File.Exists(path)) return;
            Hashtable itemTag; 
            ListViewItem item;
            EncodingFileInfo info = FileHelper.GetEncodingFileInfo(path);
            if (info.CodePage == -1) return; // If the file is locked, stop.
            MatchCollection matches = this.todoParser.Matches(info.Contents);
            this.RemoveItemsByPath(path);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    itemTag = new Hashtable();
                    itemTag["FullPath"] = path;
                    itemTag["LastWriteTime"] = new FileInfo(path).LastWriteTime;
                    itemTag["Position"] = match.Groups[2].Index;
                    item = new ListViewItem(new String[] {
                        "",
                        match.Groups[2].Index.ToString(),
                        match.Groups[2].Value,
                        CleanMatch(match.Groups[3].Value), 
                        Path.GetFileName(path),
                        Path.GetDirectoryName(path)
                    }, FindImageIndex(match.Groups[2].Value));
                    item.Tag = itemTag;
                    item.Name = path;
                    item.ToolTipText = match.Groups[2].Value;
                    this.listView.Items.Add(item);
                    this.AddToGroup(item, path);
                }
            }
        }

        /// <summary>
        /// Clean match from dirt
        /// </summary>
        private string CleanMatch(string value)
        {
            return reClean.Replace(value, "").Trim();
        }

        /// <summary>
        /// Parse a string
        /// </summary>
        private void ParseFile(String text, String path)
        {
            ListViewItem item;
            Hashtable itemTag;
            MatchCollection matches = this.todoParser.Matches(text);
            this.RemoveItemsByPath(path);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    itemTag = new Hashtable();
                    itemTag["FullPath"] = path;
                    itemTag["LastWriteTime"] = new FileInfo(path).LastWriteTime;
                    itemTag["Position"] = match.Groups[2].Index;
                    item = new ListViewItem(new String[] {
                        "",
                        match.Groups[2].Index.ToString(),
                        match.Groups[2].Value,
                        match.Groups[3].Value.Trim(), 
                        Path.GetFileName(path),
                        Path.GetDirectoryName(path)
                    }, FindImageIndex(match.Groups[2].Value));
                    item.Tag = itemTag;
                    item.Name = path;
                    this.listView.Items.Add(item);
                    this.AddToGroup(item, path);
                }
            }
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        private void AddToGroup(ListViewItem item, String path)
        {
            String gpname;
            Boolean found = false;
            ListViewGroup gp = null;
            if (File.Exists(path)) gpname = Path.GetFileName(path);
            else gpname = TextHelper.GetString("FlashDevelop.Group.Other");
            foreach (ListViewGroup lvg in this.listView.Groups)
            {
                if (lvg.Tag.ToString() == path)
                {
                    found = true;
                    gp = lvg;
                    break;
                }
            }
            if (found) gp.Items.Add(item);
            else
            {
                gp = new ListViewGroup();
                gp.Tag = path;
                gp.Header = gpname;
                this.listView.Groups.Add(gp);
                gp.Items.Add(item);
            }
        }

        /// <summary>
        /// Find the corresponding image index
        /// </summary>
        private Int32 FindImageIndex(String p)
        {
            if (this.groups.Contains(p)) return this.groups.IndexOf(p);
            else return -1;
        }

        /// <summary>
        /// Initialize the imagelist for the listView
        /// </summary>
        private void InitGraphics()
        {
            imageList = new ImageListManager();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Initialize(ImageList_Populate);
            this.listView.SmallImageList = imageList;
        }

        private void ImageList_Populate(object sender, EventArgs e)
        {
            Settings settings = (Settings) this.pluginMain.Settings;
            if (settings != null && settings.ImageIndexes != null)
            {
                foreach (Int32 index in settings.ImageIndexes)
                {
                    imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust(index.ToString()));
                }
            }
        }

        /// <summary>
        /// Remove all items by filename
        /// </summary>
        private void RemoveItemsByPath(String path)
        {
            ListViewItem[] items = this.listView.Items.Find(path, false);
            foreach (ListViewItem item in items) item.Remove();
        }

        /// <summary>
        /// Remove invalid items. Check if the item file exists
        /// </summary>
        private void RemoveInvalidItems()
        {
            this.listView.BeginUpdate();
            foreach (ListViewItem item in this.listView.Items)
            {
                if (!File.Exists(item.Name)) item.Remove();
            }
            this.listView.EndUpdate();
        }

        /// <summary>
        /// Move the document position
        /// </summary>
        private void MoveToPosition(ScintillaControl sci, Int32 position)
        {
            try
            {
                position = sci.MBSafePosition(position); // scintilla indexes are in 8bits
                Int32 line = sci.LineFromPosition(position);
                sci.EnsureVisible(line);
                sci.GotoPos(position);
                sci.SetSel(position, sci.LineEndPosition(line));
                sci.Focus();
            }
            catch 
            {
                String message = TextHelper.GetString("Info.InvalidItem");
                ErrorManager.ShowInfo(message);
                this.RemoveInvalidItems();
                this.RefreshProject();
            }
        }

        /// <summary>
        /// Clicked on "Refresh" project button. This will refresh all the project files
        /// </summary>
        private void RefreshButtonClick(Object sender, EventArgs e)
        {
            if (!this.isEnabled)
            {
                String message = TextHelper.GetString("Info.SettingError");
                this.toolStripLabel.Text = message;
            }
            else
            {
                this.RemoveInvalidItems();
                this.RefreshProject();
            }
        }

        /// <summary>
        /// Parse again the current file occasionally
        /// </summary>
        private void RefreshCurrentFile(ScintillaControl sci)
        {
            if (!this.isEnabled) return;
            try
            {
                if (this.filesCache.ContainsKey(sci.FileName))
                {
                    this.filesCache.Remove(sci.FileName);
                }
                this.ParseFile(sci.Text, sci.FileName);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Double click on an element, open the file and move to the correct position
        /// </summary>
        private void ListViewDoubleClick(Object sender, EventArgs e)
        {
            if (!this.isEnabled) return;
            ListView.SelectedListViewItemCollection selected = this.listView.SelectedItems;
            this.currentFileName = null; this.currentPos = -1;
            if (selected.Count > 0)
            {
                ListViewItem firstSelected = selected[0];
                String path = firstSelected.Name;
                this.currentFileName = path;
                this.currentPos = (Int32)((Hashtable)firstSelected.Tag)["Position"];
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (document.IsEditable)
                {
                    if (document.FileName.ToUpper() == path.ToUpper())
                    {
                        MoveToPosition(document.SciControl, currentPos);
                        currentFileName = null;
                        currentPos = -1;
                        return;
                    }
                }
                if (!File.Exists(path))
                {
                    String message = TextHelper.GetString("Info.InvalidFile");
                    ErrorManager.ShowInfo(message);
                    this.RemoveInvalidItems();
                    this.RefreshProject();
                }
                else PluginBase.MainForm.OpenEditableDocument(path, false);
            }
        }

        /// <summary>
        /// Handles the internal events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (!this.isEnabled) return;
            ITabbedDocument document;
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    document = PluginBase.MainForm.CurrentDocument;
                    if (document.IsEditable)
                    {
                        if (this.currentFileName != null && this.currentPos > -1)
                        {
                            if (this.currentFileName.ToUpper() == document.FileName.ToUpper())
                            {
                                this.MoveToPosition(document.SciControl, currentPos);
                            }
                        }
                        else RefreshCurrentFile(document.SciControl);
                    }
                    this.currentFileName = null;
                    this.currentPos = -1;
                    break;
                case EventType.FileSave:
                    document = PluginBase.MainForm.CurrentDocument;
                    if (document.IsEditable) RefreshCurrentFile(document.SciControl);
                    break;
                case EventType.Keys:
                    Keys keys = (e as KeyEvent).Value;
                    if (this.ContainsFocus && keys == Keys.Escape)
                    {
                        ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                        if (doc != null && doc.IsEditable)
                        {
                            doc.SciControl.Focus();
                            e.Handled = true;
                        }
                    }
                    break;
            }
        }


        /// <summary>
        /// Click on a listview header column, then sort the view
        /// </summary>
        private void ListViewColumnClick(Object sender, ColumnClickEventArgs e)
        {
            if (!this.isEnabled) return;
            if (e.Column == this.columnSorter.SortColumn)
            {
                if (this.columnSorter.Order == SortOrder.Ascending)
                {
                    this.columnSorter.Order = SortOrder.Descending;
                }
                else this.columnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                this.columnSorter.SortColumn = e.Column;
                this.columnSorter.Order = SortOrder.Ascending;
            }
            this.listView.Sort();
        }

        /// <summary>
        /// On enter, go to the selected item
        /// </summary>
        private void ListViewKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                this.ListViewDoubleClick(null, null);
            }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    class ExplorationContext
    {
        public int Status = 0;
        public List<String> Files;
        public BackgroundWorker Worker;
        public string[] Directories;
        public string[] ExcludedPaths;
        public string[] HiddenPaths;
    }

}
