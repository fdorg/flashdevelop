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
        int totalFiles;
        int currentPos;
        readonly List<string> groups;
        int processedFiles;
        readonly PluginMain pluginMain;
        string currentFileName;
        List<string> extensions;
        Regex todoParser;
        bool isEnabled;
        bool refreshEnabled;
        readonly System.Windows.Forms.Timer parseTimer;
        bool firstExecutionCompleted;
        readonly Dictionary<string, DateTime> filesCache;
        ContextMenuStrip contextMenu;
        ToolStripMenuItem refreshButton;
        ToolStripLabel toolStripLabel;
        readonly ListViewSorter columnSorter;
        StatusStrip statusStrip;
        ColumnHeader columnPos;
        ColumnHeader columnType;
        ColumnHeader columnIcon;
        ColumnHeader columnText;
        ColumnHeader columnName;
        ColumnHeader columnPath;
        BackgroundWorker bgWork;
        ListViewEx listView;
        ImageListManager imageList;

        // Regex
        static readonly Regex reClean = new Regex(@"(\*)?\*/.*", RegexOptions.Compiled);

        public PluginUI(PluginMain pluginMain)
        {
            InitializeComponent();
            InitializeContextMenu();
            InitializeLocalization();
            InitializeLayout();
            this.pluginMain = pluginMain;
            groups = new List<string>();
            columnSorter = new ListViewSorter();
            listView.ListViewItemSorter = columnSorter;
            Settings settings = (Settings)pluginMain.Settings;
            filesCache = new Dictionary<string, DateTime>();
            EventManager.AddEventHandler(this, EventType.Keys); // Listen Esc
            try
            {
                if (settings.GroupValues.Length > 0)
                {
                    groups.AddRange(settings.GroupValues);
                    todoParser = BuildRegex(string.Join("|", settings.GroupValues));
                    isEnabled = true;
                    InitGraphics();
                }
                listView.ShowGroups = PluginBase.Settings.UseListViewGrouping;
                listView.GridLines = !PluginBase.Settings.UseListViewGrouping;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                isEnabled = false;
            }
            parseTimer = new System.Windows.Forms.Timer();
            parseTimer.Interval = 2000;
            parseTimer.Tick += delegate { ParseNextFile(); };
            parseTimer.Enabled = false;
            parseTimer.Tag = null;
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
            columnIcon = new ColumnHeader();
            columnPos = new ColumnHeader();
            columnType = new ColumnHeader();
            columnText = new ColumnHeader();
            columnName = new ColumnHeader();
            columnPath = new ColumnHeader();
            toolStripLabel = new ToolStripLabel();
            statusStrip = new StatusStrip();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // listView
            // 
            listView.BorderStyle = BorderStyle.None;
            listView.Columns.AddRange(new[] {
            columnIcon,
            columnPos,
            columnType,
            columnText,
            columnName,
            columnPath});
            listView.Dock = DockStyle.Fill;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.LabelWrap = false;
            listView.MultiSelect = false;
            listView.Name = "listView";
            listView.ShowItemToolTips = true;
            listView.Size = new Size(278, 330);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.DoubleClick += ListViewDoubleClick;
            listView.ColumnClick += ListViewColumnClick;
            listView.KeyPress += ListViewKeyPress;
            // 
            // columnIcon
            // 
            columnIcon.Text = "!";
            columnIcon.Width = 23;
            // 
            // columnPos
            // 
            columnPos.Text = "Position";
            columnPos.Width = 75;
            // 
            // columnType
            // 
            columnType.Text = "Type";
            columnType.Width = 70;
            // 
            // columnText
            // 
            columnText.Text = "Description";
            columnText.Width = 350;
            // 
            // columnName
            // 
            columnName.Text = "File";
            columnName.Width = 150;
            // 
            // columnPath
            // 
            columnPath.Text = "Path";
            columnPath.Width = 400;
            // 
            // toolStripLabel
            // 
            toolStripLabel.Name = "toolStripLabel";
            toolStripLabel.Size = new Size(0, 20);
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] {toolStripLabel});
            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Renderer = new DockPanelStripRenderer(false);
            statusStrip.SizingGrip = false;
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(278, 22);
            statusStrip.TabIndex = 0;
            // 
            // PluginUI
            //
            Name = "PluginUI";
            Controls.Add(listView);
            Controls.Add(statusStrip);
            Size = new Size(280, 352);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        void InitializeLayout()
        {
            foreach (ColumnHeader column in listView.Columns)
                column.Width = ScaleHelper.Scale(column.Width);
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// 
        /// </summary>
        Regex BuildRegex(string pattern)
        {
            return new Regex(@"(//|\*)[\t ]*(" + pattern + @")[:\t ]+(.*)", RegexOptions.Multiline);
        }

        /// <summary>
        /// Initialises the list view's context menu
        /// </summary>
        void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            contextMenu.Renderer = new DockPanelStripRenderer(false);
            contextMenu.Font = PluginBase.Settings.DefaultFont;
            statusStrip.Font = PluginBase.Settings.DefaultFont;
            Image image = PluginBase.MainForm.FindImage("66");
            string label = TextHelper.GetString("FlashDevelop.Label.Refresh");
            refreshButton = new ToolStripMenuItem(label, image, RefreshButtonClick);
            contextMenu.Items.Add(refreshButton);
            listView.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Initializes the localized texts
        /// </summary>
        void InitializeLocalization()
        {
            columnType.Text = TextHelper.GetString("Column.Type");
            columnPos.Text = TextHelper.GetString("Column.Position");
            columnText.Text = TextHelper.GetString("Column.Description");
            columnName.Text = TextHelper.GetString("Column.File");
            columnPath.Text = TextHelper.GetString("Column.Path");
        }

        /// <summary>
        /// Update extensions and search pattern
        /// </summary>
        public void UpdateSettings()
        {
            Settings settings = ((Settings)pluginMain.Settings);
            groups.Clear();
            isEnabled = false;
            try
            {
                if (settings.GroupValues.Length > 0)
                {
                    groups.AddRange(settings.GroupValues);
                    todoParser = BuildRegex(string.Join("|", settings.GroupValues));
                    isEnabled = true;
                    InitGraphics();
                }
                else isEnabled = false;
                listView.ShowGroups = PluginBase.Settings.UseListViewGrouping;
                listView.GridLines = !PluginBase.Settings.UseListViewGrouping;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                isEnabled = false;
            }
        }

        /// <summary>
        /// While parsing project files we need to disable the refresh button
        /// </summary>
        public bool RefreshEnabled
        {
            get => refreshEnabled;
            set
            {
                refreshEnabled = value;
                refreshButton.Enabled = value;
            }
        }

        /// <summary>
        /// Stops the parse timer if not enabled.
        /// </summary>
        public void Terminate()
        {
            if (parseTimer.Enabled) parseTimer.Stop();
        }

        /// <summary>
        /// Get all available files with extension matches, filters out hidden paths.
        /// </summary>
        IEnumerable<string> GetFiles(string path, ExplorationContext context)
        {
            var files = new List<string>();
            foreach (string extension in extensions)
            {
                string[] allFiles = Directory.GetFiles(path, "*" + extension);
                files.AddRange(allFiles);
                foreach (string file in allFiles)
                {
                    foreach (string hidden in context.HiddenPaths)
                    {
                        if (file.StartsWith(hidden, StringComparison.OrdinalIgnoreCase))
                        {
                            files.Remove(file);
                        }
                    }
                }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                if (context.Worker.CancellationPending) return new List<string>();
                Thread.Sleep(5);
                if (ShouldBeScanned(dir, context.ExcludedPaths))
                {
                    files.AddRange(GetFiles(dir, context));
                }
            }
            return files;
        }

        /// <summary>
        /// Get all available files with extension match
        /// </summary>
        List<string> GetFiles(ExplorationContext context)
        {
            var files = new List<string>();
            foreach (string path in context.Directories)
            {
                if (context.Worker.CancellationPending) return new List<string>();
                Thread.Sleep(5);
                try
                {
                    if (ShouldBeScanned(path, context.ExcludedPaths))
                    {
                        files.AddRange(GetFiles(path, context));
                    }
                }
                catch {}
            }
            return files;
        }

        /// <summary>
        /// Checks if the path should be scanned for tasks
        /// </summary>
        static bool ShouldBeScanned(string path, string[] excludedPaths)
        {
            string name = Path.GetFileName(path);
            if ("._- ".Contains(name[0])) return false;
            foreach (string exclude in excludedPaths)
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
            currentPos = -1;
            currentFileName = null;
            if (!isEnabled) return;
            listView.Items.Clear();
            filesCache.Clear();
            Settings settings = (Settings)pluginMain.Settings;
            if (PluginBase.CurrentProject != null && settings.ExploringMode == ExploringMode.Complete)
            {
                RefreshProject();
            }
            else
            {
                parseTimer.Enabled = false;
                parseTimer.Stop();
                if (!InvokeRequired) toolStripLabel.Text = "";
            }
        }

        /// <summary>
        /// Refresh the current project parsing all files
        /// </summary>
        void RefreshProject()
        {
            currentPos = -1;
            currentFileName = null;
            if (!isEnabled || PluginBase.CurrentProject is null) return;
            RefreshEnabled = false;
            // Stop current exploration
            if (parseTimer.Enabled) parseTimer.Stop();
            parseTimer.Tag = null;
            if (bgWork != null && bgWork.IsBusy) bgWork.CancelAsync();
            var project = PluginBase.CurrentProject;
            var context = new ExplorationContext();
            var settings = (Settings)pluginMain.Settings;
            context.ExcludedPaths = (string[])settings.ExcludedPaths.Clone();
            context.Directories = (string[])project.SourcePaths.Clone();
            for (int i = 0; i < context.Directories.Length; i++)
            {
                context.Directories[i] = project.GetAbsolutePath(context.Directories[i]);
            }
            context.HiddenPaths = project.GetHiddenPaths();
            for (int i = 0; i < context.HiddenPaths.Length; i++)
            {
                context.HiddenPaths[i] = project.GetAbsolutePath(context.HiddenPaths[i]);
            }
            GetExtensions();
            bgWork = new BackgroundWorker();
            context.Worker = bgWork;
            bgWork.WorkerSupportsCancellation = true;
            bgWork.DoWork += BgWork_DoWork;
            bgWork.RunWorkerCompleted += BgWork_RunWorkerCompleted;
            bgWork.RunWorkerAsync(context);
            string message = TextHelper.GetString("Info.Refreshing");
            toolStripLabel.Text = message;
        }

        void GetExtensions()
        {
            Settings settings = (Settings)pluginMain.Settings;
            extensions = new List<string>();
            foreach (string ext in settings.FileExtensions)
            {
                if (!string.IsNullOrEmpty(ext))
                {
                    if (!ext.StartsWith('*')) extensions.Add("*" + ext);
                    else extensions.Add(ext);
                }
            }
            var addExt = ASContext.Context.GetExplorerMask();
            if (!addExt.IsNullOrEmpty()) extensions.AddRange(addExt);
        }

        /// <summary>
        /// 
        /// </summary>
        void BgWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;
            var context = (ExplorationContext) e.Result;
            parseTimer.Tag = context;
            parseTimer.Interval = 2000;
            parseTimer.Enabled = true;
            parseTimer.Start();
            totalFiles = context.Files.Count;
            processedFiles = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        void BgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            var context = (ExplorationContext) e.Argument;
            context.Files = GetFiles(context);
            if (context.Worker.CancellationPending) e.Cancel = true;
            else e.Result = context;
        }

        /// <summary>
        /// At startup parse all opened files
        /// </summary>
        void ParseNextFile()
        {
            if (parseTimer.Tag is ExplorationContext context)
            {
                var status = context.Status;
                if (status == 0)
                {
                    context.Status = 1;
                    parseTimer.Interval = 10;
                }
                else if (status == 1)
                {
                    var files = context.Files;
                    if (!files.IsNullOrEmpty())
                    {
                        bool parseFile = false;
                        var path = files[0];
                        DateTime lastWriteTime = new FileInfo(path).LastWriteTime;
                        if (!filesCache.ContainsKey(path))
                        {
                            filesCache[path] = lastWriteTime;
                            parseFile = true;
                        }
                        else
                        {
                            if (filesCache[path] != lastWriteTime) parseFile = true;
                        }
                        files.RemoveAt(0);
                        if (parseFile) ParseFile(path);
                        processedFiles++;
                        string message = TextHelper.GetString("Info.Processing");
                        toolStripLabel.Text = string.Format(message, processedFiles, totalFiles);
                        refreshButton.Enabled = false;
                    }
                    else context.Status = 2;
                }
                else ParseTimerCompleted();
            }
            else toolStripLabel.Text = "";
        }

        /// <summary>
        /// Parse timer completed parsing all files
        /// </summary>
        void ParseTimerCompleted()
        {
            parseTimer.Enabled = false;
            parseTimer.Stop();
            RefreshEnabled = true;
            toolStripLabel.Text = "";
            if (!firstExecutionCompleted)
            {
                EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.FileSave);
            }
            firstExecutionCompleted = true;
        }

        /// <summary>
        /// Parse a file adding all the found Matches into the listView
        /// </summary>
        void ParseFile(string path)
        {
            if (!File.Exists(path)) return;
            var info = FileHelper.GetEncodingFileInfo(path);
            if (info.CodePage == -1) return; // If the file is locked, stop.
            var matches = todoParser.Matches(info.Contents);
            RemoveItemsByPath(path);
            if (matches.Count == 0) return;
            foreach (Match match in matches)
            {
                var itemTag = new Hashtable
                {
                    ["FullPath"] = path,
                    ["LastWriteTime"] = new FileInfo(path).LastWriteTime,
                    ["Position"] = match.Groups[2].Index
                };
                var item = new ListViewItem(new[] {
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
                listView.Items.Add(item);
                AddToGroup(item, path);
            }
        }

        /// <summary>
        /// Clean match from dirt
        /// </summary>
        static string CleanMatch(string value) => reClean.Replace(value, "").Trim();

        /// <summary>
        /// Parse a string
        /// </summary>
        void ParseFile(string text, string path)
        {
            var matches = todoParser.Matches(text);
            RemoveItemsByPath(path);
            if (matches.Count == 0) return;
            foreach (Match match in matches)
            {
                var itemTag = new Hashtable
                {
                    ["FullPath"] = path,
                    ["LastWriteTime"] = new FileInfo(path).LastWriteTime,
                    ["Position"] = match.Groups[2].Index
                };
                var item = new ListViewItem(new[] {
                    "",
                    match.Groups[2].Index.ToString(),
                    match.Groups[2].Value,
                    match.Groups[3].Value.Trim(),
                    Path.GetFileName(path),
                    Path.GetDirectoryName(path)
                }, FindImageIndex(match.Groups[2].Value));
                item.Tag = itemTag;
                item.Name = path;
                listView.Items.Add(item);
                AddToGroup(item, path);
            }
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        void AddToGroup(ListViewItem item, string path)
        {
            bool found = false;
            ListViewGroup gp = null;
            var gpname = File.Exists(path)
                ? Path.GetFileName(path)
                : TextHelper.GetString("FlashDevelop.Group.Other");
            foreach (ListViewGroup lvg in listView.Groups)
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
                gp = new ListViewGroup {Tag = path, Header = gpname};
                listView.Groups.Add(gp);
                gp.Items.Add(item);
            }
        }

        /// <summary>
        /// Find the corresponding image index
        /// </summary>
        int FindImageIndex(string p)
        {
            if (groups.Contains(p)) return groups.IndexOf(p);
            return -1;
        }

        /// <summary>
        /// Initialize the imagelist for the listView
        /// </summary>
        void InitGraphics()
        {
            imageList = new ImageListManager {ColorDepth = ColorDepth.Depth32Bit};
            imageList.Initialize(ImageList_Populate);
            listView.SmallImageList = imageList;
        }

        void ImageList_Populate(object sender, EventArgs e)
        {
            var settings = (Settings) pluginMain.Settings;
            if (settings?.ImageIndexes is null) return;
            foreach (int index in settings.ImageIndexes)
            {
                imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust(index.ToString()));
            }
        }

        /// <summary>
        /// Remove all items by filename
        /// </summary>
        void RemoveItemsByPath(string path)
        {
            var items = listView.Items.Find(path, false);
            foreach (var item in items) item.Remove();
        }

        /// <summary>
        /// Remove invalid items. Check if the item file exists
        /// </summary>
        void RemoveInvalidItems()
        {
            listView.BeginUpdate();
            foreach (ListViewItem item in listView.Items)
            {
                if (!File.Exists(item.Name)) item.Remove();
            }
            listView.EndUpdate();
        }

        /// <summary>
        /// Move the document position
        /// </summary>
        void MoveToPosition(ScintillaControl sci, int position)
        {
            try
            {
                position = sci.MBSafePosition(position); // scintilla indexes are in 8bits
                int line = sci.LineFromPosition(position);
                sci.EnsureVisibleEnforcePolicy(line);
                sci.GotoPos(position);
                sci.SetSel(position, sci.LineEndPosition(line));
                sci.Focus();
            }
            catch 
            {
                string message = TextHelper.GetString("Info.InvalidItem");
                ErrorManager.ShowInfo(message);
                RemoveInvalidItems();
                RefreshProject();
            }
        }

        /// <summary>
        /// Clicked on "Refresh" project button. This will refresh all the project files
        /// </summary>
        void RefreshButtonClick(object sender, EventArgs e)
        {
            if (!isEnabled)
            {
                string message = TextHelper.GetString("Info.SettingError");
                toolStripLabel.Text = message;
            }
            else
            {
                RemoveInvalidItems();
                RefreshProject();
            }
        }

        /// <summary>
        /// Parse again the current file occasionally
        /// </summary>
        void RefreshCurrentFile(ScintillaControl sci)
        {
            if (!isEnabled) return;
            try
            {
                if (filesCache.ContainsKey(sci.FileName))
                {
                    filesCache.Remove(sci.FileName);
                }
                ParseFile(sci.Text, sci.FileName);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Double click on an element, open the file and move to the correct position
        /// </summary>
        void ListViewDoubleClick(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            var selected = listView.SelectedItems;
            currentFileName = null; currentPos = -1;
            if (selected.Count == 0) return;
            var firstSelected = selected[0];
            var path = firstSelected.Name;
            currentFileName = path;
            currentPos = (int)((Hashtable)firstSelected.Tag)["Position"];
            if (PluginBase.MainForm.CurrentDocument?.SciControl is { } sci)
            {
                if (sci.FileName.ToUpper() == path.ToUpper())
                {
                    MoveToPosition(sci, currentPos);
                    currentFileName = null;
                    currentPos = -1;
                    return;
                }
            }
            if (!File.Exists(path))
            {
                string message = TextHelper.GetString("Info.InvalidFile");
                ErrorManager.ShowInfo(message);
                RemoveInvalidItems();
                RefreshProject();
            }
            else PluginBase.MainForm.OpenEditableDocument(path, false);
        }

        /// <summary>
        /// Handles the internal events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (!isEnabled) return;
            switch (e.Type)
            {
                case EventType.FileSwitch:
                {
                    var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                    if (sci != null)
                    {
                        if (currentFileName != null && currentPos > -1)
                        {
                            if (currentFileName.ToUpper() == sci.FileName.ToUpper())
                            {
                                MoveToPosition(sci, currentPos);
                            }
                        }
                        else RefreshCurrentFile(sci);
                    }
                    currentFileName = null;
                    currentPos = -1;
                    break;
                }
                case EventType.FileSave:
                {
                    var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                    if (sci != null) RefreshCurrentFile(sci);
                    break;
                }
                case EventType.Keys:
                    var keys = ((KeyEvent) e).Value;
                    if (ContainsFocus && keys == Keys.Escape)
                    {
                        var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                        if (sci != null)
                        {
                            sci.Focus();
                            e.Handled = true;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Click on a listview header column, then sort the view
        /// </summary>
        void ListViewColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!isEnabled) return;
            if (e.Column == columnSorter.SortColumn)
            {
                columnSorter.Order = columnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                columnSorter.SortColumn = e.Column;
                columnSorter.Order = SortOrder.Ascending;
            }
            listView.Sort();
        }

        /// <summary>
        /// On enter, go to the selected item
        /// </summary>
        void ListViewKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ListViewDoubleClick(null, null);
            }
        }

        #endregion

    }

    internal class ExplorationContext
    {
        public int Status;
        public List<string> Files;
        public BackgroundWorker Worker;
        public string[] Directories;
        public string[] ExcludedPaths;
        public string[] HiddenPaths;
    }
}