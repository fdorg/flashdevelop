// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using Ookii.Dialogs;

namespace FileExplorer
{
    public class PluginUI : DockPanelControl
    {
        private ListViewEx fileView;
        private ToolStrip toolStrip;
        private ContextMenuStrip menu;
        private ToolStripMenuItem runButton;
        private ToolStripMenuItem editButton;
        private ToolStripMenuItem renameButton;
        private ToolStripMenuItem deleteButton;
        private ToolStripMenuItem shellButton;
        private ToolStripMenuItem pasteButton;
        private ToolStripMenuItem copyButton;
        private ToolStripSeparator separator;
        private ToolStripSpringComboBox selectedPath;
        private ToolStripButton browseButton;
        private ToolStripButton syncronizeButton;
        private ColumnHeader fileHeader;
        private ColumnHeader sizeHeader;
        private ColumnHeader typeHeader;
        private ColumnHeader modifiedHeader;
        private VistaFolderBrowserDialog folderBrowserDialog;
        private ListViewItem highlightedItem;
        private ImageListManager imageList;
        private bool updateInProgress;
        private string previousItemLabel;
        private string autoSelectItem;
        private long lastUpdateTimeStamp;
        private int prevColumnClick;
        private readonly ListViewSorter listViewSorter;
        private FileSystemWatcher watcher;
        private readonly PluginMain pluginMain;
        
        public PluginUI(PluginMain pluginMain)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            this.listViewSorter = new ListViewSorter();
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeContextMenu();
            this.InitializeLayout();
            this.InitializeTexts();
            ScrollBarEx.Attach(fileView);
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.watcher = new System.IO.FileSystemWatcher();
            this.modifiedHeader = new System.Windows.Forms.ColumnHeader();
            this.typeHeader = new System.Windows.Forms.ColumnHeader();
            this.fileView = new System.Windows.Forms.ListViewEx();
            this.fileHeader = new System.Windows.Forms.ColumnHeader();
            this.sizeHeader = new System.Windows.Forms.ColumnHeader();
            this.folderBrowserDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.selectedPath = new System.Windows.Forms.ToolStripSpringComboBox();
            this.syncronizeButton = new System.Windows.Forms.ToolStripButton();
            this.browseButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // watcher
            // 
            this.watcher.EnableRaisingEvents = true;
            this.watcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName;
            this.watcher.SynchronizingObject = this;
            this.watcher.Renamed += this.WatcherRenamed;
            this.watcher.Deleted += this.WatcherChanged;
            this.watcher.Created += this.WatcherChanged;
            this.watcher.Changed += this.WatcherChanged;
            // 
            // modifiedHeader
            // 
            this.modifiedHeader.Text = "Modified";
            this.modifiedHeader.Width = 120;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Type";
            // 
            // fileView
            // 
            this.fileView.AllowDrop = true;
            this.fileView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileView.Columns.AddRange(new[] {
            this.fileHeader,
            this.sizeHeader,
            this.typeHeader,
            this.modifiedHeader});
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.LabelEdit = true;
            this.fileView.Name = "fileView";
            this.fileView.Size = new System.Drawing.Size(278, 327);
            this.fileView.TabIndex = 5;
            this.fileView.FullRowSelect = true;
            this.fileView.UseCompatibleStateImageBehavior = false;
            this.fileView.View = System.Windows.Forms.View.Details;
            this.fileView.ItemActivate += this.FileViewItemActivate;
            this.fileView.AfterLabelEdit += this.FileViewAfterLabelEdit;
            this.fileView.MouseUp += this.FileViewMouseUp;
            this.fileView.DragDrop += this.FileViewDragDrop;
            this.fileView.ColumnClick += this.FileViewColumnClick;
            this.fileView.KeyUp += this.FileViewKeyUp;
            this.fileView.BeforeLabelEdit += this.FileViewBeforeLabelEdit;
            this.fileView.ItemDrag += this.FileViewDragItems;
            this.fileView.DragOver += this.FileViewDragOver;
            // 
            // fileHeader
            // 
            this.fileHeader.Text = "Files";
            this.fileHeader.Width = 190;
            // 
            // sizeHeader
            // 
            this.sizeHeader.Text = "Size";
            this.sizeHeader.Width = 55;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Open a folder to list the files in the folder";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // toolStrip
            //
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedPath,
            this.syncronizeButton,
            this.browseButton});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(278, 26);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 6;
            // 
            // selectedPath
            //
            this.selectedPath.Name = "selectedPath";
            this.selectedPath.Size = new System.Drawing.Size(200, 22);
            this.selectedPath.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.selectedPath.FlatCombo.SelectedIndexChanged += this.SelectedPathSelectedIndexChanged;
            this.selectedPath.KeyDown += this.SelectedPathKeyDown;
            // 
            // syncronizeButton
            //
            this.syncronizeButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.syncronizeButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.syncronizeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.syncronizeButton.Name = "syncronizeButton";
            this.syncronizeButton.Size = new System.Drawing.Size(23, 22);
            this.syncronizeButton.Text = "Synchronize";
            this.syncronizeButton.Click += this.SynchronizeView;
            // 
            // browseButton
            //
            this.browseButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.browseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.browseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(23, 22);
            this.browseButton.Text = "Browse";
            this.browseButton.Click += this.BrowseButtonClick;
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Controls.Add(this.fileView);
            this.Controls.Add(this.toolStrip);
            this.Size = new System.Drawing.Size(280, 352);
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Shows the explorer shell menu
        /// </summary>
        private void ShowShellMenu(object sender, EventArgs e)
        {
            int count = this.fileView.SelectedItems.Count;
            FileInfo[] selectedPathsAndFiles = new FileInfo[count];
            ShellContextMenu scm = new ShellContextMenu();
            for (int i = 0; i < count; i++)
            {
                string path = this.fileView.SelectedItems[i].Tag.ToString();
                selectedPathsAndFiles[i] = new FileInfo(path);
            }
            if (count == 0)
            {
                string path = this.selectedPath.Text;
                if (!Directory.Exists(path)) return;
                selectedPathsAndFiles = new FileInfo[1];
                selectedPathsAndFiles[0] = new FileInfo(path);
            }
            this.menu.Hide(); /* Hide default menu */
            Point location = new Point(this.menu.Bounds.Left, this.menu.Bounds.Top);
            scm.ShowContextMenu(selectedPathsAndFiles, location);
        }

        /// <summary>
        /// Creates and attaches the context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            this.menu = new ContextMenuStrip();
            this.menu.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.RefreshView"), null, this.RefreshFileView));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.SynchronizeView"), null, this.SynchronizeView));
            this.menu.Items.Add(new ToolStripSeparator());
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CreateFileHere"), null, this.CreateFileHere));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CreateFolderHere"), null, this.CreateFolderHere));
            this.menu.Items.Add(new ToolStripSeparator());
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ExploreHere"), null, this.ExploreHere));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.FindHere"), null, this.FindHere));
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CommandPromptHere"), null, this.CommandPromptHere));
            if (Win32.ShouldUseWin32())
            {
                this.shellButton = new ToolStripMenuItem(TextHelper.GetString("Label.ShellMenu"), null, this.ShowShellMenu);
                this.menu.Items.Add(this.shellButton);
            }
            this.menu.Items.Add(new ToolStripSeparator());
            this.menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.TrustHere"), null, this.TrustHere));
            this.separator = new ToolStripSeparator();
            this.runButton = new ToolStripMenuItem(TextHelper.GetString("Label.Run"), null, this.OpenItem);
            this.editButton = new ToolStripMenuItem(TextHelper.GetString("Label.Edit"), null, this.EditItems);
            this.copyButton = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, this.CopyItems);
            this.pasteButton = new ToolStripMenuItem(TextHelper.GetString("Label.Paste"), null, this.PasteItems);
            this.renameButton = new ToolStripMenuItem(TextHelper.GetString("Label.Rename"), null, this.RenameItem);
            this.deleteButton = new ToolStripMenuItem(TextHelper.GetString("Label.Delete"), null, this.DeleteItems);
            this.menu.Items.Add(this.separator);
            this.menu.Items.AddRange(new ToolStripMenuItem[6]{this.runButton, this.editButton, this.copyButton, this.pasteButton, this.renameButton, this.deleteButton});
            this.menu.Font = PluginBase.Settings.DefaultFont;
            this.menu.Renderer = new DockPanelStripRenderer(false);
            this.fileView.ContextMenuStrip = this.menu;
            // Set default key strings
            if (PluginBase.Settings.ViewShortcuts)
            {
                this.editButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Enter);
                this.renameButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.F2);
                this.copyButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.C);
                this.pasteButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.P);
                this.deleteButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Delete);
            }
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList = new ImageListManager();
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.Populate += RefreshFileView;
            this.AddNonWin32Images();
            this.syncronizeButton.Image = PluginBase.MainForm.FindImage("203|9|-3|-3");
            this.browseButton.Image = PluginBase.MainForm.FindImage("203");
            this.fileView.SmallImageList = this.imageList;
        }

        /// <summary>
        /// Applies localized texts to the control
        /// </summary>
        public void InitializeTexts()
        {
            this.fileHeader.Text = TextHelper.GetString("Header.Files");
            this.modifiedHeader.Text = TextHelper.GetString("Header.Modified");
            this.folderBrowserDialog.UseDescriptionForTitle = true;
            this.folderBrowserDialog.Description = TextHelper.GetString("Info.BrowseDescription");
            this.syncronizeButton.ToolTipText = TextHelper.GetString("ToolTip.Synchronize");
            this.browseButton.ToolTipText = TextHelper.GetString("ToolTip.Browse");
            this.typeHeader.Text = TextHelper.GetString("Header.Type");
            this.sizeHeader.Text = TextHelper.GetString("Header.Size");
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            this.selectedPath.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.toolStrip.Renderer = new DockPanelStripRenderer();
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            foreach (ColumnHeader column in fileView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        /// <summary>
        /// Browses to the selected path
        /// </summary>
        public void BrowseTo(string path)
        {
            this.PopulateFileView(path);
        }

        /// <summary>
        /// Gets the reference to the context menu
        /// </summary>
        public ContextMenuStrip GetContextMenu()
        {
            return this.menu;
        }

        /// <summary>
        /// Add the path to the combo box
        /// </summary>
        public void AddToMRU(string path)
        {
            if (Directory.Exists(path) && !this.selectedPath.Items.Contains(path))
            {
                this.selectedPath.Items.Add(path);
            }
        }

        /// <summary>
        /// List last open path on load
        /// </summary>
        public void Initialize(object sender, System.EventArgs e)
        {
            string path = PathHelper.AppDir;
            string pathToCheck = this.pluginMain.Settings.FilePath;
            if (Directory.Exists(pathToCheck)) path = pathToCheck;
            this.listViewSorter.SortColumn = this.pluginMain.Settings.SortColumn;
            if (this.pluginMain.Settings.SortOrder == 0) this.listViewSorter.Order = SortOrder.Ascending;
            else this.listViewSorter.Order = SortOrder.Descending;
            this.watcher.Path = path; this.watcher.EnableRaisingEvents = true;
            this.fileView.ListViewItemSorter = this.listViewSorter;
            this.PopulateFileView(path);
        }

        /// <summary>
        /// Update files listview. If the path is invalid, use the last valid path
        /// </summary>
        private void PopulateFileView(string path)
        {
            // Prevent double call caused by AddToMRU
            if (this.updateInProgress) return;
            this.updateInProgress = true;
            this.fileView.ContextMenu = null;
            this.syncronizeButton.Enabled = false;
            this.browseButton.Enabled = false;
            this.selectedPath.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            this.AddToMRU(path);
            DirectoryInfo dir = null;
            FileSystemInfo[] infos = null;
            // Check the specified path
            path = PathHelper.GetPhysicalPathName(path);
            // Do the actual filesystem querying in the background
            MethodInvoker backgroundMethod = delegate
            {
                dir = new DirectoryInfo(path);
                infos = dir.GetFileSystemInfos();
            };
            backgroundMethod.BeginInvoke(delegate(IAsyncResult result)
            {
                backgroundMethod.EndInvoke(result);
                // marshal back to the UI thread
                BeginInvoke((MethodInvoker)delegate
                { 
                    this.UpdateUI(path, dir, infos); 
                });
            }, null);
        }

        /// <summary>
        /// Updates the UI in the GUI thread
        /// </summary>
        private void UpdateUI(string path, DirectoryInfo directory, FileSystemInfo[] infos)
        {
            this.ClearImageList();
            this.pluginMain.Settings.FilePath = path;
            this.selectedPath.Text = path;
            this.fileView.BeginUpdate();
            this.fileView.ListViewItemSorter = null;
            this.fileView.Items.Clear();
            try
            {
                ListViewItem item;
                if (directory.Parent != null)
                {
                    item = new ListViewItem("[..]", ExtractIconIfNecessary("/Folder/", false));
                    item.Tag = directory.Parent.FullName;
                    item.SubItems.Add("-");
                    item.SubItems.Add("-");
                    item.SubItems.Add("-");
                    this.fileView.Items.Add(item);
                }
                foreach (FileSystemInfo info in infos)
                {
                    DirectoryInfo subDir = info as DirectoryInfo;
                    if (subDir != null && (subDir.Attributes & FileAttributes.Hidden) == 0)
                    {
                        item = new ListViewItem(subDir.Name, ExtractIconIfNecessary(subDir.FullName, false));
                        item.Tag = subDir.FullName;
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add(subDir.LastWriteTime.ToString());
                        this.fileView.Items.Add(item);
                    }
                }
                foreach (FileSystemInfo info in infos)
                {
                    FileInfo file = info as FileInfo;
                    if (file != null && (file.Attributes & FileAttributes.Hidden) == 0)
                    {
                        string kbs = TextHelper.GetString("Info.Kilobytes");
                        item = new ListViewItem(file.Name, ExtractIconIfNecessary(file.FullName, true));
                        item.Tag = file.FullName;
                        if (file.Length / 1024 < 1) item.SubItems.Add("1 " + kbs);
                        else item.SubItems.Add((file.Length / 1024) + " " + kbs);
                        item.SubItems.Add(file.Extension.ToUpper().Replace(".", ""));
                        item.SubItems.Add(file.LastWriteTime.ToString());
                        this.fileView.Items.Add(item);
                    }
                }
                this.watcher.Path = path;
                this.fileView.ListViewItemSorter = this.listViewSorter;
            }
            finally
            {
                // Select the possible created item
                if (this.autoSelectItem != null)
                {
                    foreach (ListViewItem item in this.fileView.Items)
                    {
                        if (item.Text == this.autoSelectItem)
                        {
                            this.fileView.Focus();
                            item.BeginEdit();
                            break;
                        }
                    }
                    this.autoSelectItem = null;
                }
                this.Cursor = Cursors.Default;
                this.fileView.ContextMenuStrip = this.menu;
                this.syncronizeButton.Enabled = true;
                this.selectedPath.Enabled = true;
                this.browseButton.Enabled = true;
                this.updateInProgress = false;
                this.fileView.EndUpdate();
            }
        }

        /// <summary>
        /// Open the folder browser dialog
        /// </summary>
        private void BrowseButtonClick(object sender, System.EventArgs e)
        {
            try
            {
                if (Directory.Exists(this.selectedPath.Text))
                {
                    this.folderBrowserDialog.SelectedPath = this.selectedPath.Text;
                }
                if (this.folderBrowserDialog.ShowDialog((Form)PluginBase.MainForm) == DialogResult.OK)
                {
                    this.PopulateFileView(this.folderBrowserDialog.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Repopulate when user changes the path from the combo box
        /// </summary>
        private void SelectedPathSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.selectedPath.SelectedIndex != -1)
            {
                string path = this.selectedPath.SelectedItem.ToString();
                if (Directory.Exists(path)) this.PopulateFileView(path);
            }
        }

        /// <summary>
        /// Key pressed while editing the selected path
        /// </summary>
        private void SelectedPathKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                string path = this.selectedPath.Text;
                if (Directory.Exists(path))
                {
                    this.PopulateFileView(path);
                }
            }
        }

        /// <summary>
        /// Gets a list of currectly selected files
        /// </summary>
        private string[] GetSelectedFiles()
        {
            int i = 0;
            if (this.fileView.SelectedItems.Count == 0) return null;
            string[] files = new string[this.fileView.SelectedItems.Count];
            foreach (ListViewItem item in this.fileView.SelectedItems)
            {
                files[i++] = item.Tag.ToString();
            }
            return files;
        }

        /// <summary>
        /// Starts the dragging operation
        /// </summary>
        private void FileViewDragItems(object sender, ItemDragEventArgs e)
        {
            string[] files = this.GetSelectedFiles();
            if (files != null && e.Button == MouseButtons.Left)
            {
                DataObject data = new DataObject(DataFormats.FileDrop, files);
                this.fileView.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Checks if the path list contains only files
        /// </summary> 
        private bool ContainsOnlyFiles(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (Directory.Exists(files[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Handles the event when the drag is over the control
        /// </summary>
        private void FileViewDragOver(object sender, DragEventArgs e)
        {
            Point cp = this.fileView.PointToClient(new Point(e.X, e.Y));
            ListViewItem whereToMove = this.fileView.GetItemAt(cp.X, cp.Y);
            if (whereToMove != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = whereToMove.Tag.ToString();
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(path) && this.IsValidDropTarget(path, data))
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else e.Effect = DragDropEffects.Move;
                    this.HighlightSelectedItem(whereToMove);
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    this.UnhighlightSelectedItem();
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
                this.UnhighlightSelectedItem();
            }
        }

        /// <summary>
        /// Checks whether the user is trying to drop something inside itself
        /// </summary>
        private bool IsValidDropTarget(string path, string[] paths)
        {
            string original = PathHelper.GetLongPathName(path);
            for (int i = 0; i < paths.Length; i++)
            {
                string current = PathHelper.GetLongPathName(paths[i]);
                if (original == current) return false;
            }
            return true;
        }

        /// <summary>
        /// If the files are dropped over the file view, moves the files
        /// </summary>
        private void FileViewDragDrop(object sender, DragEventArgs e)
        {
            this.UnhighlightSelectedItem();
            try
            {
                Point cp = this.fileView.PointToClient(new Point(e.X, e.Y));
                ListViewItem whereToMove = this.fileView.GetItemAt(cp.X, cp.Y);
                if (whereToMove is null) return; // Item is dropped on nothing
                string targetDirectory = whereToMove.Tag.ToString();
                if (whereToMove.Text.StartsWith('[') || Directory.Exists(targetDirectory))
                {
                    for (int i = 0; i < this.fileView.SelectedItems.Count; i++)
                    {
                        string path = this.fileView.SelectedItems[i].Tag.ToString();
                        if (File.Exists(path))
                        {
                            string name = Path.GetFileName(path);
                            string target = Path.Combine(targetDirectory, name);
                            if (e.Effect == DragDropEffects.Move)
                            {
                                File.Move(path, target);
                                DocumentManager.MoveDocuments(path, target);
                            }
                            else File.Copy(path, target, true);
                        }
                        else if (Directory.Exists(path))
                        {
                            string name = FolderHelper.GetFolderName(path);
                            string target = Path.Combine(targetDirectory, name);
                            if (e.Effect == DragDropEffects.Move)
                            {
                                Directory.Move(path, target);
                                DocumentManager.MoveDocuments(path, target);
                            }
                            else FolderHelper.CopyFolder(path, target);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Handles the pressed keys from the fileView
        /// </summary> 
        private void FileViewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                this.DeleteItems(null, null);
            }
            else if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                if (this.fileView.SelectedItems.Count > 0)
                {
                    this.RenameItem(null, null);
                }
            }
        }

        /// <summary>
        /// A file/directory item could be renamed
        /// </summary>
        private void FileViewBeforeLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            try
            {
                this.previousItemLabel = this.fileView.Items[e.Item].Text;
                if (this.previousItemLabel.StartsWith('[')) e.CancelEdit = true;
            }
            catch
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// A file/directory item has been renamed
        /// </summary>
        private void FileViewAfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            ListViewItem item = null;
            try
            {
                if (e.CancelEdit || string.IsNullOrEmpty(e.Label) || e.Label == this.previousItemLabel)
                {
                    e.CancelEdit = true;
                    return;
                }
                item = this.fileView.Items[e.Item];
                string file = item.Tag.ToString();
                FileInfo info = new FileInfo(file);
                string path = info.Directory + Path.DirectorySeparatorChar.ToString();
                if (File.Exists(file))
                {
                    File.Move(path + this.previousItemLabel, path + e.Label);
                    DocumentManager.MoveDocuments(path + this.previousItemLabel, path + e.Label);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Move(path + this.previousItemLabel, path + e.Label);
                    DocumentManager.MoveDocuments(path + this.previousItemLabel, path + e.Label);
                }
            }
            catch (Exception ex)
            {
                if (item != null) ErrorManager.ShowError(ex);
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// Opens the selected file or browses to a path
        /// </summary>
        private void FileViewItemActivate(object sender, System.EventArgs e)
        {
            if (this.fileView.SelectedItems.Count == 0) return;
            string file = this.fileView.SelectedItems[0].Tag.ToString();
            if (Control.ModifierKeys == Keys.Shift) this.OpenItem(null, null);
            else
            {
                if (File.Exists(file)) PluginBase.MainForm.OpenEditableDocument(file);
                else this.PopulateFileView(file);
            }
        }

        /// <summary>
        /// Creates the context menu on right button click
        /// </summary>
        private void FileViewMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) 
            {
                this.UpdateMenuItemVisibility();
            }
        }

        /// <summary>
        /// Updates the context menu item visibility
        /// </summary>
        private void UpdateMenuItemVisibility()
        {
            bool canPaste = false;
            bool notFirstItem = true;
            bool targetIsDirectory = false;
            bool onlyFiles = this.SelectedItemsAreOnlyFiles();
            int selectedItems = this.fileView.SelectedItems.Count;
            if (selectedItems > 0) notFirstItem = !this.fileView.SelectedItems[0].Text.StartsWith('[');
            if (selectedItems == 1) targetIsDirectory = Directory.Exists(this.fileView.SelectedItems[0].Tag.ToString());
            if (!targetIsDirectory) targetIsDirectory = Directory.Exists(this.selectedPath.Text);
            canPaste = (targetIsDirectory && notFirstItem && Clipboard.ContainsFileDropList());
            this.renameButton.Visible = (selectedItems == 1 && notFirstItem);
            this.runButton.Visible = (selectedItems == 1 && notFirstItem);
            this.deleteButton.Visible = (selectedItems > 0 && notFirstItem);
            this.copyButton.Visible = (selectedItems > 0 && notFirstItem);
            this.editButton.Visible = (selectedItems > 0 && notFirstItem && onlyFiles);
            this.separator.Visible = (selectedItems > 0 && notFirstItem);
            if (selectedItems == 0 && canPaste) this.separator.Visible = true;
            this.pasteButton.Visible = canPaste;
        }

        /// <summary>
        /// Check whether the selected items are only files
        /// </summary> 
        private bool SelectedItemsAreOnlyFiles()
        {
            for (int i = 0; i < this.fileView.SelectedItems.Count; i++)
            {
                string path = this.fileView.SelectedItems[i].Tag.ToString();
                if (Directory.Exists(path)) return false;
            }
            return true;
        }

        /// <summary>
        /// Refreshes the file view
        /// </summary>
        private void RefreshFileView(object sender, System.EventArgs e)
        {
            string path = this.selectedPath.Text;
            if (!string.IsNullOrEmpty(path)) this.PopulateFileView(path);
        }

        /// <summary>
        /// Browses to the current file's path
        /// If file is in a project, browse to project root
        /// </summary>
        private void SynchronizeView(object sender, System.EventArgs e)
        {
            string path = null;
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (PluginBase.CurrentProject != null && this.pluginMain.Settings.SynchronizeToProject)
            {
                path = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                if (document.IsEditable && !document.IsUntitled && !document.FileName.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    path = Path.GetDirectoryName(document.FileName);
                }
            }
            else if (document.IsEditable && !document.IsUntitled)
            {
                path = Path.GetDirectoryName(document.FileName);
            }
            if (path != null && Directory.Exists(path))
            {
                this.PopulateFileView(path);
            }
        }

        /// <summary>
        /// Add directory to trust files
        /// </summary>
        private void TrustHere(object sender, System.EventArgs e)
        {
            string path;
            string trustFile;
            string trustParams;
            // add selected file
            if ((this.fileView.SelectedItems.Count != 0) && (this.fileView.SelectedIndices[0] > 0))
            {
                string file = this.fileView.SelectedItems[0].Tag.ToString();
                if (File.Exists(file)) file = Path.GetDirectoryName(file);
                if (!Directory.Exists(file)) return;
                DirectoryInfo info = new DirectoryInfo(file);
                path = info.FullName;
                trustFile = path.Replace('\\', '_').Remove(1, 1);
                while ((trustFile.Length > 100) && (trustFile.IndexOf('_') is var p && p > 0)) trustFile = trustFile.Substring(p);
                trustParams = "FlashDevelop_" + trustFile + ".cfg;" + path;
            }
            // add current folder
            else
            {
                FileInfo info = new FileInfo(this.selectedPath.Text);
                path = info.FullName;
                trustFile = path.Replace('\\', '_').Remove(1, 1);
                while ((trustFile.Length > 100) && (trustFile.IndexOf('_') is var p && p > 0)) trustFile = trustFile.Substring(p);
                trustParams = "FlashDevelop_" + trustFile + ".cfg;" + path;
            }
            // add to trusted files
            DataEvent deTrust = new DataEvent(EventType.Command, "ASCompletion.CreateTrustFile", trustParams);
            EventManager.DispatchEvent(this, deTrust);
            if (deTrust.Handled)
            {
                string message = TextHelper.GetString("Info.PathTrusted");
                ErrorManager.ShowInfo("\"" + path + "\"\n" + message);
            }
        }

        /// <summary>
        /// Opens Windows explorer in the current path
        /// </summary>
        private void ExploreHere(object sender, System.EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.Explore", this.selectedPath.Text);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Opens the find and replace in files popup in the current path
        /// </summary>
        private void FindHere(object sender, System.EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.FindHere", this.GetSelectedFiles());
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Opens the command prompt in the current path
        /// </summary>
        private void CommandPromptHere(object sender, System.EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.PromptHere", this.selectedPath.Text);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Sorts items on user column click
        /// </summary>
        private void FileViewColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            if (this.prevColumnClick == e.Column)
            {
                this.listViewSorter.Order = (this.listViewSorter.Order == SortOrder.Descending) ? SortOrder.Ascending : SortOrder.Descending;
            } 
            else this.listViewSorter.Order = SortOrder.Ascending;
            if (this.listViewSorter.Order == SortOrder.Ascending)
            {
                this.pluginMain.Settings.SortOrder = 0;
            } 
            else this.pluginMain.Settings.SortOrder = 1;
            this.prevColumnClick = e.Column;
            this.pluginMain.Settings.SortColumn = e.Column;
            this.listViewSorter.SortColumn = e.Column;
            this.fileView.Sort();
        }

        /// <summary>
        /// Creates a new file to the current folder
        /// </summary>
        private void CreateFileHere(object sender, System.EventArgs e)
        {
            try
            {
                string filename = TextHelper.GetString("Info.NewFileName");
                int codepage = (int)PluginBase.MainForm.Settings.DefaultCodePage;
                string extension = PluginBase.MainForm.Settings.DefaultFileExtension;
                string file = Path.Combine(this.selectedPath.Text, filename) + "." + extension;
                string unique = FileHelper.EnsureUniquePath(file);
                FileHelper.WriteFile(unique, "", Encoding.GetEncoding(codepage), PluginBase.Settings.SaveUnicodeWithBOM);
                this.autoSelectItem = Path.GetFileName(unique);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Creates a new folder to the current folder
        /// </summary>
        private void CreateFolderHere(object sender, System.EventArgs e)
        {
            try
            {
                string folderName = TextHelper.GetString("Info.NewFolderName");
                string target = Path.Combine(this.selectedPath.Text, folderName);
                string unique = FolderHelper.EnsureUniquePath(target);
                Directory.CreateDirectory(unique);
                this.autoSelectItem = folderName;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Copies the selected files to the clipboard
        /// </summary>
        private void CopyItems(object sender, System.EventArgs e)
        {
            StringCollection items = new StringCollection();
            for (int i = 0; i < this.fileView.SelectedItems.Count; i++)
            {
                items.Add(fileView.SelectedItems[i].Tag.ToString());
            }
            Clipboard.SetFileDropList(items);
        }

        /// <summary>
        /// Pastes the selected files from clipboard
        /// </summary>
        private void PasteItems(object sender, System.EventArgs e)
        {
            string target = string.Empty;
            if (this.fileView.SelectedItems.Count == 0) target = this.selectedPath.Text;
            else target = this.fileView.SelectedItems[0].Tag.ToString();
            StringCollection items = Clipboard.GetFileDropList();
            foreach (var it in items)
            {
                if (File.Exists(it))
                {
                    string copy = Path.Combine(target, Path.GetFileName(it));
                    string file = FileHelper.EnsureUniquePath(copy);
                    File.Copy(it, file, false);
                }
                else
                {
                    string folder = FolderHelper.EnsureUniquePath(target);
                    FolderHelper.CopyFolder(it, folder);
                }
            }
        }

        /// <summary>
        /// Edits the selected items in FlashDevelop
        /// </summary>
        private void EditItems(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.fileView.SelectedItems.Count; i++)
            {
                string file = this.fileView.SelectedItems[i].Tag.ToString();
                if (File.Exists(file)) PluginBase.MainForm.OpenEditableDocument(file);
            }
        }

        /// <summary>
        /// Deletes the selected items
        /// </summary>
        private void DeleteItems(object sender, System.EventArgs e)
        {
            try
            {
                string message = TextHelper.GetString("Info.ConfirmDelete");
                string confirm = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                DialogResult result = MessageBox.Show(message, " " + confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < this.fileView.SelectedItems.Count; i++)
                    {
                        string path = this.fileView.SelectedItems[i].Tag.ToString();
                        if (!FileHelper.Recycle(path))
                        {
                            string error = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                            throw new Exception(error + " " + path);
                        }
                        DocumentManager.CloseDocuments(path);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Renames current file or directory
        /// </summary>
        private void RenameItem(object sender, System.EventArgs e)
        {
            this.fileView.SelectedItems[0].BeginEdit();
        }

        /// <summary>
        /// Opens the current file or directory with associated program 
        /// </summary>
        private void OpenItem(object sender, System.EventArgs e)
        {
            try
            {
                string file = this.fileView.SelectedItems[0].Tag.ToString();
                ProcessHelper.StartAsync(file);
            }
            catch (Exception ex) 
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Highlights the selected item
        /// </summary>
        private void HighlightSelectedItem(ListViewItem item)
        {
            if (item != this.highlightedItem)
            {
                this.UnhighlightSelectedItem();
                this.highlightedItem = item;
                this.highlightedItem.Selected = true;
            }
        }

        /// <summary>
        /// Unhighlights the selected item
        /// </summary>
        private void UnhighlightSelectedItem()
        {
            if (this.highlightedItem != null)
            {
                this.highlightedItem.Selected = false;
                this.highlightedItem = null;
            }
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                long timestamp = DateTime.Now.Ticks;
                if (timestamp - this.lastUpdateTimeStamp < 500) return;
                this.lastUpdateTimeStamp = timestamp; // Store timestamp
                this.PopulateFileView(this.selectedPath.Text);
            });
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            this.WatcherChanged(sender, null);
        }

        #endregion

        #region Icon Management

        /// <summary>
        /// Ask the shell to feed us the appropriate icon for the given file, but
        /// first try looking in our cache to see if we've already loaded it.
        /// </summary>
        private int ExtractIconIfNecessary(string path, bool isFile)
        {
            try
            {
                Icon icon; Image image;
                Size size = ScaleHelper.Scale(new Size(16, 16));
                if (Win32.ShouldUseWin32())
                {
                    if (isFile) icon = IconExtractor.GetFileIcon(path, false, true);
                    else icon = IconExtractor.GetFolderIcon(path, false, true);
                    image = ImageKonverter.ImageResize(icon.ToBitmap(), size.Width, size.Height);
                    image = PluginBase.MainForm.ImageSetAdjust(image);
                    icon.Dispose();
                    this.imageList.Images.Add(image);
                    return this.imageList.Images.Count - 1;
                }
            }
            catch {} // No errors please...
            return isFile ? 0 : 1;
        }

        /// <summary>
        /// Dispose all images entirely.
        /// </summary>
        private void ClearImageList()
        {
            this.imageList.Images.Clear();
            AddNonWin32Images();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddNonWin32Images()
        {
            if (Win32.ShouldUseWin32()) return;
            this.imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("526"));
            this.imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("203"));
        }

        #endregion

    }

}
