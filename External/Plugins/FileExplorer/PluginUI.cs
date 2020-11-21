using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
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
        ListViewEx fileView;
        ToolStrip toolStrip;
        ContextMenuStrip menu;
        ToolStripMenuItem runButton;
        ToolStripMenuItem editButton;
        ToolStripMenuItem renameButton;
        ToolStripMenuItem deleteButton;
        ToolStripMenuItem shellButton;
        ToolStripMenuItem pasteButton;
        ToolStripMenuItem copyButton;
        ToolStripSeparator separator;
        ToolStripSpringComboBox selectedPath;
        ToolStripButton browseButton;
        ToolStripButton syncronizeButton;
        ColumnHeader fileHeader;
        ColumnHeader sizeHeader;
        ColumnHeader typeHeader;
        ColumnHeader modifiedHeader;
        VistaFolderBrowserDialog folderBrowserDialog;
        ListViewItem highlightedItem;
        ImageListManager imageList;
        bool updateInProgress;
        string previousItemLabel;
        string autoSelectItem;
        long lastUpdateTimeStamp;
        int prevColumnClick;
        readonly ListViewSorter listViewSorter;
        FileSystemWatcher watcher;
        readonly PluginMain pluginMain;
        
        public PluginUI(PluginMain pluginMain)
        {
            AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            listViewSorter = new ListViewSorter();
            InitializeComponent();
            InitializeGraphics();
            InitializeContextMenu();
            InitializeLayout();
            InitializeTexts();
            ScrollBarEx.Attach(fileView);
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent() 
        {
            watcher = new FileSystemWatcher();
            modifiedHeader = new ColumnHeader();
            typeHeader = new ColumnHeader();
            fileView = new ListViewEx();
            fileHeader = new ColumnHeader();
            sizeHeader = new ColumnHeader();
            folderBrowserDialog = new VistaFolderBrowserDialog();
            toolStrip = new ToolStripEx();
            selectedPath = new ToolStripSpringComboBox();
            syncronizeButton = new ToolStripButton();
            browseButton = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(watcher)).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // watcher
            // 
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.SynchronizingObject = this;
            watcher.Renamed += WatcherRenamed;
            watcher.Deleted += WatcherChanged;
            watcher.Created += WatcherChanged;
            watcher.Changed += WatcherChanged;
            // 
            // modifiedHeader
            // 
            modifiedHeader.Text = "Modified";
            modifiedHeader.Width = 120;
            // 
            // typeHeader
            // 
            typeHeader.Text = "Type";
            // 
            // fileView
            // 
            fileView.AllowDrop = true;
            fileView.BorderStyle = BorderStyle.None;
            fileView.Columns.AddRange(new[] {
            fileHeader,
            sizeHeader,
            typeHeader,
            modifiedHeader});
            fileView.Dock = DockStyle.Fill;
            fileView.LabelEdit = true;
            fileView.Name = "fileView";
            fileView.Size = new Size(278, 327);
            fileView.TabIndex = 5;
            fileView.FullRowSelect = true;
            fileView.UseCompatibleStateImageBehavior = false;
            fileView.View = View.Details;
            fileView.ItemActivate += FileViewItemActivate;
            fileView.AfterLabelEdit += FileViewAfterLabelEdit;
            fileView.MouseUp += FileViewMouseUp;
            fileView.DragDrop += FileViewDragDrop;
            fileView.ColumnClick += FileViewColumnClick;
            fileView.KeyUp += FileViewKeyUp;
            fileView.BeforeLabelEdit += FileViewBeforeLabelEdit;
            fileView.ItemDrag += FileViewDragItems;
            fileView.DragOver += FileViewDragOver;
            // 
            // fileHeader
            // 
            fileHeader.Text = "Files";
            fileHeader.Width = 190;
            // 
            // sizeHeader
            // 
            sizeHeader.Text = "Size";
            sizeHeader.Width = 55;
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.Description = "Open a folder to list the files in the folder";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            // 
            // toolStrip
            //
            toolStrip.CanOverflow = false;
            toolStrip.Dock = DockStyle.Top;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] {
            selectedPath,
            syncronizeButton,
            browseButton});
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(1, 1, 2, 2);
            toolStrip.Size = new Size(278, 26);
            toolStrip.Stretch = true;
            toolStrip.TabIndex = 6;
            // 
            // selectedPath
            //
            selectedPath.Name = "selectedPath";
            selectedPath.Size = new Size(200, 22);
            selectedPath.Padding = new Padding(0, 0, 1, 0);
            selectedPath.FlatCombo.SelectedIndexChanged += SelectedPathSelectedIndexChanged;
            selectedPath.KeyDown += SelectedPathKeyDown;
            // 
            // syncronizeButton
            //
            syncronizeButton.Margin = new Padding(0, 1, 0, 1);
            syncronizeButton.Alignment = ToolStripItemAlignment.Right;
            syncronizeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            syncronizeButton.Name = "syncronizeButton";
            syncronizeButton.Size = new Size(23, 22);
            syncronizeButton.Text = "Synchronize";
            syncronizeButton.Click += SynchronizeView;
            // 
            // browseButton
            //
            browseButton.Margin = new Padding(0, 1, 0, 1);
            browseButton.Alignment = ToolStripItemAlignment.Right;
            browseButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(23, 22);
            browseButton.Text = "Browse";
            browseButton.Click += BrowseButtonClick;
            // 
            // PluginUI
            //
            Name = "PluginUI";
            Controls.Add(fileView);
            Controls.Add(toolStrip);
            Size = new Size(280, 352);
            ((System.ComponentModel.ISupportInitialize)(watcher)).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Shows the explorer shell menu
        /// </summary>
        void ShowShellMenu(object sender, EventArgs e)
        {
            int count = fileView.SelectedItems.Count;
            FileInfo[] selectedPathsAndFiles = new FileInfo[count];
            ShellContextMenu scm = new ShellContextMenu();
            for (int i = 0; i < count; i++)
            {
                string path = fileView.SelectedItems[i].Tag.ToString();
                selectedPathsAndFiles[i] = new FileInfo(path);
            }
            if (count == 0)
            {
                string path = selectedPath.Text;
                if (!Directory.Exists(path)) return;
                selectedPathsAndFiles = new FileInfo[1];
                selectedPathsAndFiles[0] = new FileInfo(path);
            }
            menu.Hide(); /* Hide default menu */
            Point location = new Point(menu.Bounds.Left, menu.Bounds.Top);
            scm.ShowContextMenu(selectedPathsAndFiles, location);
        }

        /// <summary>
        /// Creates and attaches the context menu
        /// </summary>
        void InitializeContextMenu()
        {
            menu = new ContextMenuStrip();
            menu.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.RefreshView"), null, RefreshFileView));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.SynchronizeView"), null, SynchronizeView));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CreateFileHere"), null, CreateFileHere));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CreateFolderHere"), null, CreateFolderHere));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ExploreHere"), null, ExploreHere));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.FindHere"), null, FindHere));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CommandPromptHere"), null, CommandPromptHere));
            if (Win32.ShouldUseWin32())
            {
                shellButton = new ToolStripMenuItem(TextHelper.GetString("Label.ShellMenu"), null, ShowShellMenu);
                menu.Items.Add(shellButton);
            }
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.TrustHere"), null, TrustHere));
            separator = new ToolStripSeparator();
            runButton = new ToolStripMenuItem(TextHelper.GetString("Label.Run"), null, OpenItem);
            editButton = new ToolStripMenuItem(TextHelper.GetString("Label.Edit"), null, EditItems);
            copyButton = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, CopyItems);
            pasteButton = new ToolStripMenuItem(TextHelper.GetString("Label.Paste"), null, PasteItems);
            renameButton = new ToolStripMenuItem(TextHelper.GetString("Label.Rename"), null, RenameItem);
            deleteButton = new ToolStripMenuItem(TextHelper.GetString("Label.Delete"), null, DeleteItems);
            menu.Items.Add(separator);
            menu.Items.AddRange(new ToolStripItem[]{runButton, editButton, copyButton, pasteButton, renameButton, deleteButton});
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            fileView.ContextMenuStrip = menu;
            // Set default key strings
            if (PluginBase.Settings.ViewShortcuts)
            {
                editButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Enter);
                renameButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.F2);
                copyButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.C);
                pasteButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.P);
                deleteButton.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Delete);
            }
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            imageList = new ImageListManager();
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Populate += RefreshFileView;
            AddNonWin32Images();
            syncronizeButton.Image = PluginBase.MainForm.FindImage("203|9|-3|-3");
            browseButton.Image = PluginBase.MainForm.FindImage("203");
            fileView.SmallImageList = imageList;
        }

        /// <summary>
        /// Applies localized texts to the control
        /// </summary>
        public void InitializeTexts()
        {
            fileHeader.Text = TextHelper.GetString("Header.Files");
            modifiedHeader.Text = TextHelper.GetString("Header.Modified");
            folderBrowserDialog.UseDescriptionForTitle = true;
            folderBrowserDialog.Description = TextHelper.GetString("Info.BrowseDescription");
            syncronizeButton.ToolTipText = TextHelper.GetString("ToolTip.Synchronize");
            browseButton.ToolTipText = TextHelper.GetString("ToolTip.Browse");
            typeHeader.Text = TextHelper.GetString("Header.Type");
            sizeHeader.Text = TextHelper.GetString("Header.Size");
        }

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        void InitializeLayout()
        {
            selectedPath.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            toolStrip.Renderer = new DockPanelStripRenderer();
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            foreach (ColumnHeader column in fileView.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
        }

        /// <summary>
        /// Browses to the selected path
        /// </summary>
        public void BrowseTo(string path) => PopulateFileView(path);

        /// <summary>
        /// Gets the reference to the context menu
        /// </summary>
        public ContextMenuStrip GetContextMenu() => menu;

        /// <summary>
        /// Add the path to the combo box
        /// </summary>
        public void AddToMRU(string path)
        {
            if (Directory.Exists(path) && !selectedPath.Items.Contains(path))
            {
                selectedPath.Items.Add(path);
            }
        }

        /// <summary>
        /// List last open path on load
        /// </summary>
        public void Initialize(object sender, EventArgs e)
        {
            string path = PathHelper.AppDir;
            string pathToCheck = pluginMain.Settings.FilePath;
            if (Directory.Exists(pathToCheck)) path = pathToCheck;
            listViewSorter.SortColumn = pluginMain.Settings.SortColumn;
            if (pluginMain.Settings.SortOrder == 0) listViewSorter.Order = SortOrder.Ascending;
            else listViewSorter.Order = SortOrder.Descending;
            watcher.Path = path; watcher.EnableRaisingEvents = true;
            fileView.ListViewItemSorter = listViewSorter;
            PopulateFileView(path);
        }

        /// <summary>
        /// Update files listview. If the path is invalid, use the last valid path
        /// </summary>
        void PopulateFileView(string path)
        {
            // Prevent double call caused by AddToMRU
            if (updateInProgress) return;
            updateInProgress = true;
            fileView.ContextMenuStrip = null;
            syncronizeButton.Enabled = false;
            browseButton.Enabled = false;
            selectedPath.Enabled = false;
            Cursor = Cursors.WaitCursor;
            AddToMRU(path);
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
                    UpdateUI(path, dir, infos); 
                });
            }, null);
        }

        /// <summary>
        /// Updates the UI in the GUI thread
        /// </summary>
        void UpdateUI(string path, DirectoryInfo directory, FileSystemInfo[] infos)
        {
            ClearImageList();
            pluginMain.Settings.FilePath = path;
            selectedPath.Text = path;
            fileView.BeginUpdate();
            fileView.ListViewItemSorter = null;
            fileView.Items.Clear();
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
                    fileView.Items.Add(item);
                }
                foreach (FileSystemInfo info in infos)
                {
                    if (info is DirectoryInfo subDir && (subDir.Attributes & FileAttributes.Hidden) == 0)
                    {
                        item = new ListViewItem(subDir.Name, ExtractIconIfNecessary(subDir.FullName, false));
                        item.Tag = subDir.FullName;
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add(subDir.LastWriteTime.ToString());
                        fileView.Items.Add(item);
                    }
                }
                foreach (FileSystemInfo info in infos)
                {
                    if (info is FileInfo file && (file.Attributes & FileAttributes.Hidden) == 0)
                    {
                        string kbs = TextHelper.GetString("Info.Kilobytes");
                        item = new ListViewItem(file.Name, ExtractIconIfNecessary(file.FullName, true));
                        item.Tag = file.FullName;
                        if (file.Length / 1024 < 1) item.SubItems.Add("1 " + kbs);
                        else item.SubItems.Add((file.Length / 1024) + " " + kbs);
                        item.SubItems.Add(file.Extension.ToUpper().Replace(".", ""));
                        item.SubItems.Add(file.LastWriteTime.ToString());
                        fileView.Items.Add(item);
                    }
                }
                watcher.Path = path;
                fileView.ListViewItemSorter = listViewSorter;
            }
            finally
            {
                // Select the possible created item
                if (autoSelectItem != null)
                {
                    foreach (ListViewItem item in fileView.Items)
                    {
                        if (item.Text == autoSelectItem)
                        {
                            fileView.Focus();
                            item.BeginEdit();
                            break;
                        }
                    }
                    autoSelectItem = null;
                }
                Cursor = Cursors.Default;
                fileView.ContextMenuStrip = menu;
                syncronizeButton.Enabled = true;
                selectedPath.Enabled = true;
                browseButton.Enabled = true;
                updateInProgress = false;
                fileView.EndUpdate();
            }
        }

        /// <summary>
        /// Open the folder browser dialog
        /// </summary>
        void BrowseButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(selectedPath.Text))
                {
                    folderBrowserDialog.SelectedPath = selectedPath.Text;
                }
                if (folderBrowserDialog.ShowDialog((Form)PluginBase.MainForm) == DialogResult.OK)
                {
                    PopulateFileView(folderBrowserDialog.SelectedPath);
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
        void SelectedPathSelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedPath.SelectedIndex != -1)
            {
                string path = selectedPath.SelectedItem.ToString();
                if (Directory.Exists(path)) PopulateFileView(path);
            }
        }

        /// <summary>
        /// Key pressed while editing the selected path
        /// </summary>
        void SelectedPathKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                string path = selectedPath.Text;
                if (Directory.Exists(path))
                {
                    PopulateFileView(path);
                }
            }
        }

        /// <summary>
        /// Gets a list of currectly selected files
        /// </summary>
        string[] GetSelectedFiles()
        {
            int i = 0;
            if (fileView.SelectedItems.Count == 0) return null;
            string[] files = new string[fileView.SelectedItems.Count];
            foreach (ListViewItem item in fileView.SelectedItems)
            {
                files[i++] = item.Tag.ToString();
            }
            return files;
        }

        /// <summary>
        /// Starts the dragging operation
        /// </summary>
        void FileViewDragItems(object sender, ItemDragEventArgs e)
        {
            string[] files = GetSelectedFiles();
            if (files != null && e.Button == MouseButtons.Left)
            {
                DataObject data = new DataObject(DataFormats.FileDrop, files);
                fileView.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Checks if the path list contains only files
        /// </summary> 
        bool ContainsOnlyFiles(IEnumerable<string> files) => files.All(it => !Directory.Exists(it));

        /// <summary>
        /// Handles the event when the drag is over the control
        /// </summary>
        void FileViewDragOver(object sender, DragEventArgs e)
        {
            Point cp = fileView.PointToClient(new Point(e.X, e.Y));
            ListViewItem whereToMove = fileView.GetItemAt(cp.X, cp.Y);
            if (whereToMove != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = whereToMove.Tag.ToString();
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(path) && IsValidDropTarget(path, data))
                {
                    if (ModifierKeys == Keys.Control)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else e.Effect = DragDropEffects.Move;
                    HighlightSelectedItem(whereToMove);
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    UnhighlightSelectedItem();
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
                UnhighlightSelectedItem();
            }
        }

        /// <summary>
        /// Checks whether the user is trying to drop something inside itself
        /// </summary>
        static bool IsValidDropTarget(string path, IEnumerable<string> paths)
        {
            var original = PathHelper.GetLongPathName(path);
            return paths.All(it => PathHelper.GetLongPathName(it) != original);
        }

        /// <summary>
        /// If the files are dropped over the file view, moves the files
        /// </summary>
        void FileViewDragDrop(object sender, DragEventArgs e)
        {
            UnhighlightSelectedItem();
            try
            {
                Point cp = fileView.PointToClient(new Point(e.X, e.Y));
                ListViewItem whereToMove = fileView.GetItemAt(cp.X, cp.Y);
                if (whereToMove is null) return; // Item is dropped on nothing
                string targetDirectory = whereToMove.Tag.ToString();
                if (whereToMove.Text.StartsWith('[') || Directory.Exists(targetDirectory))
                {
                    for (int i = 0; i < fileView.SelectedItems.Count; i++)
                    {
                        string path = fileView.SelectedItems[i].Tag.ToString();
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
        void FileViewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                DeleteItems(null, null);
            }
            else if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                if (fileView.SelectedItems.Count > 0)
                {
                    RenameItem(null, null);
                }
            }
        }

        /// <summary>
        /// A file/directory item could be renamed
        /// </summary>
        void FileViewBeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                previousItemLabel = fileView.Items[e.Item].Text;
                if (previousItemLabel.StartsWith('[')) e.CancelEdit = true;
            }
            catch
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// A file/directory item has been renamed
        /// </summary>
        void FileViewAfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem item = null;
            try
            {
                if (e.CancelEdit || string.IsNullOrEmpty(e.Label) || e.Label == previousItemLabel)
                {
                    e.CancelEdit = true;
                    return;
                }
                item = fileView.Items[e.Item];
                string file = item.Tag.ToString();
                FileInfo info = new FileInfo(file);
                string path = info.Directory + Path.DirectorySeparatorChar.ToString();
                if (File.Exists(file))
                {
                    File.Move(path + previousItemLabel, path + e.Label);
                    DocumentManager.MoveDocuments(path + previousItemLabel, path + e.Label);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Move(path + previousItemLabel, path + e.Label);
                    DocumentManager.MoveDocuments(path + previousItemLabel, path + e.Label);
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
        void FileViewItemActivate(object sender, EventArgs e)
        {
            if (fileView.SelectedItems.Count == 0) return;
            string file = fileView.SelectedItems[0].Tag.ToString();
            if (ModifierKeys == Keys.Shift) OpenItem(null, null);
            else
            {
                if (File.Exists(file)) PluginBase.MainForm.OpenEditableDocument(file);
                else PopulateFileView(file);
            }
        }

        /// <summary>
        /// Creates the context menu on right button click
        /// </summary>
        void FileViewMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) 
            {
                UpdateMenuItemVisibility();
            }
        }

        /// <summary>
        /// Updates the context menu item visibility
        /// </summary>
        void UpdateMenuItemVisibility()
        {
            bool notFirstItem = true;
            bool targetIsDirectory = false;
            bool onlyFiles = SelectedItemsAreOnlyFiles();
            int selectedItems = fileView.SelectedItems.Count;
            if (selectedItems > 0) notFirstItem = !fileView.SelectedItems[0].Text.StartsWith('[');
            if (selectedItems == 1) targetIsDirectory = Directory.Exists(fileView.SelectedItems[0].Tag.ToString());
            if (!targetIsDirectory) targetIsDirectory = Directory.Exists(selectedPath.Text);
            var canPaste = (targetIsDirectory && notFirstItem && Clipboard.ContainsFileDropList());
            renameButton.Visible = (selectedItems == 1 && notFirstItem);
            runButton.Visible = (selectedItems == 1 && notFirstItem);
            deleteButton.Visible = (selectedItems > 0 && notFirstItem);
            copyButton.Visible = (selectedItems > 0 && notFirstItem);
            editButton.Visible = (selectedItems > 0 && notFirstItem && onlyFiles);
            separator.Visible = (selectedItems > 0 && notFirstItem) || (selectedItems == 0 && canPaste);
            pasteButton.Visible = canPaste;
        }

        /// <summary>
        /// Check whether the selected items are only files
        /// </summary> 
        bool SelectedItemsAreOnlyFiles()
        {
            for (int i = 0; i < fileView.SelectedItems.Count; i++)
            {
                string path = fileView.SelectedItems[i].Tag.ToString();
                if (Directory.Exists(path)) return false;
            }
            return true;
        }

        /// <summary>
        /// Refreshes the file view
        /// </summary>
        void RefreshFileView(object sender, EventArgs e)
        {
            string path = selectedPath.Text;
            if (!string.IsNullOrEmpty(path)) PopulateFileView(path);
        }

        /// <summary>
        /// Browses to the current file's path
        /// If file is in a project, browse to project root
        /// </summary>
        void SynchronizeView(object sender, EventArgs e)
        {
            string path = null;
            var document = PluginBase.MainForm.CurrentDocument;
            if (PluginBase.CurrentProject != null && pluginMain.Settings.SynchronizeToProject)
            {
                path = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                if (document.SciControl is { } sci
                    && !document.IsUntitled
                    && !sci.FileName.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    path = Path.GetDirectoryName(sci.FileName);
                }
            }
            else if (document.SciControl is { } sci && !document.IsUntitled)
            {
                path = Path.GetDirectoryName(sci.FileName);
            }
            if (Directory.Exists(path)) PopulateFileView(path);
        }

        /// <summary>
        /// Add directory to trust files
        /// </summary>
        void TrustHere(object sender, EventArgs e)
        {
            string path;
            // add selected file
            if ((fileView.SelectedItems.Count != 0) && (fileView.SelectedIndices[0] > 0))
            {
                var file = fileView.SelectedItems[0].Tag.ToString();
                if (File.Exists(file)) file = Path.GetDirectoryName(file);
                if (!Directory.Exists(file)) return;
                var info = new DirectoryInfo(file);
                path = info.FullName;
            }
            // add current folder
            else
            {
                var info = new FileInfo(selectedPath.Text);
                path = info.FullName;
            }
            var trustFile = path.Replace('\\', '_').Remove(1, 1);
            while (trustFile.Length > 100 && trustFile.Contains('_', out var p) && p > 0) trustFile = trustFile.Substring(p);
            var trustParams = "FlashDevelop_" + trustFile + ".cfg;" + path;
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
        void ExploreHere(object sender, EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.Explore", selectedPath.Text);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Opens the find and replace in files popup in the current path
        /// </summary>
        void FindHere(object sender, EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.FindHere", GetSelectedFiles());
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Opens the command prompt in the current path
        /// </summary>
        void CommandPromptHere(object sender, EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "FileExplorer.PromptHere", selectedPath.Text);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Sorts items on user column click
        /// </summary>
        void FileViewColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (prevColumnClick == e.Column)
            {
                listViewSorter.Order = (listViewSorter.Order == SortOrder.Descending) ? SortOrder.Ascending : SortOrder.Descending;
            } 
            else listViewSorter.Order = SortOrder.Ascending;
            if (listViewSorter.Order == SortOrder.Ascending)
            {
                pluginMain.Settings.SortOrder = 0;
            } 
            else pluginMain.Settings.SortOrder = 1;
            prevColumnClick = e.Column;
            pluginMain.Settings.SortColumn = e.Column;
            listViewSorter.SortColumn = e.Column;
            fileView.Sort();
        }

        /// <summary>
        /// Creates a new file to the current folder
        /// </summary>
        void CreateFileHere(object sender, EventArgs e)
        {
            try
            {
                string filename = TextHelper.GetString("Info.NewFileName");
                int codepage = (int)PluginBase.Settings.DefaultCodePage;
                string extension = PluginBase.Settings.DefaultFileExtension;
                string file = Path.Combine(selectedPath.Text, filename) + "." + extension;
                string unique = FileHelper.EnsureUniquePath(file);
                FileHelper.WriteFile(unique, "", Encoding.GetEncoding(codepage), PluginBase.Settings.SaveUnicodeWithBOM);
                autoSelectItem = Path.GetFileName(unique);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Creates a new folder to the current folder
        /// </summary>
        void CreateFolderHere(object sender, EventArgs e)
        {
            try
            {
                string folderName = TextHelper.GetString("Info.NewFolderName");
                string target = Path.Combine(selectedPath.Text, folderName);
                string unique = FolderHelper.EnsureUniquePath(target);
                Directory.CreateDirectory(unique);
                autoSelectItem = folderName;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Copies the selected files to the clipboard
        /// </summary>
        void CopyItems(object sender, EventArgs e)
        {
            var items = new StringCollection();
            for (var i = 0; i < fileView.SelectedItems.Count; i++)
            {
                items.Add(fileView.SelectedItems[i].Tag.ToString());
            }
            Clipboard.SetFileDropList(items);
        }

        /// <summary>
        /// Pastes the selected files from clipboard
        /// </summary>
        void PasteItems(object sender, EventArgs e)
        {
            var target = fileView.SelectedItems.Count == 0
                ? selectedPath.Text
                : fileView.SelectedItems[0].Tag.ToString();
            var items = Clipboard.GetFileDropList();
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
        void EditItems(object sender, EventArgs e)
        {
            for (int i = 0; i < fileView.SelectedItems.Count; i++)
            {
                string file = fileView.SelectedItems[i].Tag.ToString();
                if (File.Exists(file)) PluginBase.MainForm.OpenEditableDocument(file);
            }
        }

        /// <summary>
        /// Deletes the selected items
        /// </summary>
        void DeleteItems(object sender, EventArgs e)
        {
            try
            {
                string message = TextHelper.GetString("Info.ConfirmDelete");
                string confirm = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                DialogResult result = MessageBox.Show(message, " " + confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < fileView.SelectedItems.Count; i++)
                    {
                        string path = fileView.SelectedItems[i].Tag.ToString();
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
        void RenameItem(object sender, EventArgs e) => fileView.SelectedItems[0].BeginEdit();

        /// <summary>
        /// Opens the current file or directory with associated program 
        /// </summary>
        void OpenItem(object sender, EventArgs e)
        {
            try
            {
                string file = fileView.SelectedItems[0].Tag.ToString();
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
        void HighlightSelectedItem(ListViewItem item)
        {
            if (item != highlightedItem)
            {
                UnhighlightSelectedItem();
                highlightedItem = item;
                highlightedItem.Selected = true;
            }
        }

        /// <summary>
        /// Unhighlights the selected item
        /// </summary>
        void UnhighlightSelectedItem()
        {
            if (highlightedItem != null)
            {
                highlightedItem.Selected = false;
                highlightedItem = null;
            }
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                long timestamp = DateTime.Now.Ticks;
                if (timestamp - lastUpdateTimeStamp < 500) return;
                lastUpdateTimeStamp = timestamp; // Store timestamp
                PopulateFileView(selectedPath.Text);
            });
        }

        /// <summary>
        /// The directory we're watching has changed - refresh!
        /// </summary>
        void WatcherRenamed(object sender, RenamedEventArgs e) => WatcherChanged(sender, null);

        #endregion

        #region Icon Management

        /// <summary>
        /// Ask the shell to feed us the appropriate icon for the given file, but
        /// first try looking in our cache to see if we've already loaded it.
        /// </summary>
        int ExtractIconIfNecessary(string path, bool isFile)
        {
            try
            {
                if (Win32.ShouldUseWin32())
                {
                    var icon = isFile
                        ? IconExtractor.GetFileIcon(path, false, true)
                        : IconExtractor.GetFolderIcon(path, false, true);
                    var size = ScaleHelper.Scale(new Size(16, 16));
                    var image = ImageKonverter.ImageResize(icon.ToBitmap(), size.Width, size.Height);
                    image = PluginBase.MainForm.ImageSetAdjust(image);
                    icon.Dispose();
                    imageList.Images.Add(image);
                    return imageList.Images.Count - 1;
                }
            }
            catch {} // No errors please...
            return isFile ? 0 : 1;
        }

        /// <summary>
        /// Dispose all images entirely.
        /// </summary>
        void ClearImageList()
        {
            imageList.Images.Clear();
            AddNonWin32Images();
        }

        /// <summary>
        /// 
        /// </summary>
        void AddNonWin32Images()
        {
            if (Win32.ShouldUseWin32()) return;
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("526"));
            imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("203"));
        }

        #endregion
    }
}