using System;
using System.IO;
using System.Net;
using System.Data;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using AppMan.Utilities;

namespace AppMan
{
    public partial class MainForm : Form
    {
        private String curFile;
        private String tempFile;
        private Boolean isLoading;
        private DepEntry curEntry;
        private String entriesFile;
        private WebClient webClient;
        private DepEntries depEntries;
        private DepEntries instEntries;
        private BackgroundWorker bgWorker;
        private Dictionary<String, String> entryStates;
        private Dictionary<String, ListViewGroup> appGroups;
        private Queue<DepEntry> downloadQueue;
        private Queue<String> fileQueue;
        private String[] notifyPaths;
        private Boolean shouldNotify;

        public MainForm()
        {
            this.isLoading = false;
            this.shouldNotify = false;
            this.InitializeSettings();
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeContextMenu();
            this.Load += new EventHandler(this.MainFormLoad);
            this.HelpRequested += new HelpEventHandler(this.MainFormHelpRequested);
            this.HelpButtonClicked += new CancelEventHandler(this.MainFormHelpButtonClicked);
            this.FormClosed += new FormClosedEventHandler(this.MainFormClosed);
            this.Font = SystemFonts.MenuFont;
        }

        #region Handlers & Methods

        /// <summary>
        /// Initializes the graphics of the app.
        /// </summary>
        private void InitializeGraphics()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            this.cancelButton.Image = Image.FromStream(assembly.GetManifestResourceStream("AppMan.Resources.Cancel.png"));
            this.Icon = new Icon(assembly.GetManifestResourceStream("AppMan.Resources.AppMan.ico"));
        }

        /// <summary>
        /// Initializes the web client used for item downloads.
        /// </summary>
        private void InitializeWebClient()
        {
            this.webClient = new WebClient();
            this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgressChanged);
            this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.DownloadFileCompleted);
        }

        /// <summary>
        /// Initializes the settings of the app.
        /// </summary>
        private void InitializeSettings()
        {
            try
            {
                Settings settings = new Settings();
                String file = Path.Combine(PathHelper.GetExeDirectory(), "Config.xml");
                if (File.Exists(file))
                {
                    settings = ObjectSerializer.Deserialize(file, settings) as Settings;
                    PathHelper.ARCHIVE_DIR = ArgProcessor.ProcessArguments(settings.Archive);
                    PathHelper.CONFIG_ADR = ArgProcessor.ProcessArguments(settings.Config);
                    PathHelper.HELP_ADR = ArgProcessor.ProcessArguments(settings.Help);
                    this.notifyPaths = settings.Paths;
                }
                else /* Defaults for FlashDevelop */
                {
                    String local = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\.local");
                    local = Path.GetFullPath(local); /* Fix weird path */
                    if (!File.Exists(local))
                    {
                        String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        String fdUserPath = Path.Combine(userAppDir, "FlashDevelop");
                        String appManDataDir = Path.Combine(fdUserPath, @"Data\AppMan");
                        PathHelper.ARCHIVE_DIR = Path.Combine(appManDataDir, "Archive");
                        PathHelper.LOG_DIR = appManDataDir;
                        this.notifyPaths = new String[1] { fdUserPath };
                    }
                    else
                    {
                        String fdPath = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\");
                        fdPath = Path.GetFullPath(fdPath); /* Fix weird path */
                        PathHelper.ARCHIVE_DIR = Path.Combine(fdPath, @"Data\AppMan\Archive");
                        PathHelper.LOG_DIR = Path.Combine(fdPath, @"Data\AppMan");
                        this.notifyPaths = new String[1] { fdPath };
                    }
                }
                if (!Directory.Exists(PathHelper.LOG_DIR))
                {
                    Directory.CreateDirectory(PathHelper.LOG_DIR);
                }
                if (!Directory.Exists(PathHelper.ARCHIVE_DIR))
                {
                    Directory.CreateDirectory(PathHelper.ARCHIVE_DIR);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Initializes the list view context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("View Info", null, new EventHandler(this.onViewInfoClick));
            cms.Opening += new CancelEventHandler(this.ContextMenuOpening);
            this.listView.ContextMenuStrip = cms;
        }

        /// <summary>
        /// Cancel opening if an item is not selected.
        /// </summary>
        private void ContextMenuOpening(Object sender, CancelEventArgs e)
        {
            Point point = this.listView.PointToClient(Cursor.Position);
            ListViewItem item = this.listView.GetItemAt(point.X, point.Y);
            if (item == null || item.Tag == null || String.IsNullOrEmpty(((DepEntry)item.Tag).Info))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Open info file or url when clicked.
        /// </summary>
        private void onViewInfoClick(Object sender, EventArgs e)
        {
            Point point = this.listView.PointToClient(Cursor.Position);
            ListViewItem item = this.listView.GetItemAt(point.X, point.Y);
            if (item != null)
            {
                DepEntry entry = item.Tag as DepEntry;
                if (entry != null && !String.IsNullOrEmpty(entry.Info))
                {
                    Process.Start(entry.Info);
                }
            }
        }

        /// <summary>
        /// On MainForm show, initializes the UI and the props.
        /// </summary>
        private void MainFormLoad(Object sender, EventArgs e)
        {
            this.InitializeWebClient();
            this.depEntries = new DepEntries();
            this.entryStates = new Dictionary<String, String>();
            this.appGroups = new Dictionary<String, ListViewGroup>();
            this.downloadQueue = new Queue<DepEntry>();
            this.TryDeleteOldTempFiles();
            this.listView.Items.Clear();
            this.LoadInstalledEntries();
            this.LoadEntriesFile();
        }

        /// <summary>
        /// Opens the help when pressing help button or F1.
        /// </summary>
        private void MainFormHelpRequested(Object sender, HelpEventArgs e)
        {
            try 
            { 
                Process.Start(PathHelper.HELP_ADR); 
            }
            catch (Exception ex) 
            { 
                DialogHelper.ShowError(ex.ToString()); 
            }
        }
        private void MainFormHelpButtonClicked(Object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.MainFormHelpRequested(null, null);
        }

        /// <summary>
        /// Closes the application when pressing Escape.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys k)
        {
            if (k == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, k);
        }

        /// <summary>
        /// Save notification files to the notify paths
        /// </summary>
        private void MainFormClosed(Object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!this.shouldNotify || this.notifyPaths == null) return;
                foreach (String nPath in this.notifyPaths)
                {
                    try
                    {
                        String path = Path.GetDirectoryName(ArgProcessor.ProcessArguments(nPath));
                        if (Directory.Exists(path))
                        {
                            String amFile = Path.Combine(path, ".appman");
                            File.WriteAllText(amFile, "");
                        }
                    }
                    catch { /* NO ERRORS */ }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Cancels the item download process.
        /// </summary>
        private void CancelButtonClick(Object sender, EventArgs e)
        {
            try
            {
                this.webClient.CancelAsync();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Starts the download queue based on the user selections.
        /// </summary>
        private void InstallButtonClick(Object sender, EventArgs e)
        {
            this.isLoading = true;
            this.cancelButton.Enabled = true;
            this.installButton.Enabled = false;
            this.deleteButton.Enabled = false;
            this.AddEntriesToQueue();
            this.DownloadNextFromQueue();
        }

        /// <summary>
        /// Deletes the selected items from the archive.
        /// </summary>
        private void DeleteButtonClick(Object sender, EventArgs e)
        {
            try
            {
                String title = "Confirm";
                String message = "Are you sure to delete all versions of the selected items?";
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in this.listView.CheckedItems)
                    {
                        DepEntry entry = item.Tag as DepEntry;
                        String state = this.entryStates[entry.Id];
                        if (state == "Installed" || state == "Update")
                        {
                            Directory.Delete(Path.Combine(PathHelper.ARCHIVE_DIR, entry.Id), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                Thread.Sleep(100); // Wait for files...
                this.NoneLinkLabelLinkClicked(null, null);
                this.LoadInstalledEntries();
                this.UpdateEntryStates();
                this.UpdateButtonLabels();
            }
        }

        /// <summary>
        /// Browses the archive with windows explorer.
        /// </summary>
        private void ExploreButtonClick(Object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", PathHelper.ARCHIVE_DIR);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On All link click, selects the all items.
        /// </summary>
        private void AllLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.isLoading) return;
            this.listView.BeginUpdate();
            foreach (ListViewItem item in this.listView.Items)
            {
                item.Checked = true;
            }
            this.listView.EndUpdate();
        }

        /// <summary>
        /// On None link click, deselects all items.
        /// </summary>
        private void NoneLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.isLoading) return;
            this.listView.BeginUpdate();
            foreach (ListViewItem item in this.listView.Items)
            {
                item.Checked = false;
            }
            this.listView.EndUpdate();
        }

        /// <summary>
        /// On New link click, selects all new items.
        /// </summary>
        private void NewLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (this.isLoading) return;
                this.listView.BeginUpdate();
                foreach (ListViewItem item in this.listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    String state = this.entryStates[entry.Id];
                    if (state == "New") item.Checked = true;
                    else item.Checked = false;
                }
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On Installed link click, selects all installed items.
        /// </summary>
        private void InstLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (this.isLoading) return;
                this.listView.BeginUpdate();
                foreach (ListViewItem item in this.listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    String state = this.entryStates[entry.Id];
                    if (state == "Installed") item.Checked = true;
                    else item.Checked = false;
                }
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On Updates link click, selects all updatable items.
        /// </summary>
        private void UpdatesLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (this.isLoading) return;
                this.listView.BeginUpdate();
                foreach (ListViewItem item in this.listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    String state = this.entryStates[entry.Id];
                    if (state == "Update") item.Checked = true;
                    else item.Checked = false;
                }
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Disables the item checking when downloading.
        /// </summary>
        private void ListViewItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (this.isLoading) e.NewValue = e.CurrentValue;
        }

        /// <summary>
        /// Updates the button labels when item is checked.
        /// </summary>
        private void ListViewItemChecked(Object sender, ItemCheckedEventArgs e)
        {
            if (this.isLoading) return;
            this.UpdateButtonLabels();
        }

        /// <summary>
        /// Disables the visual selection of items.
        /// </summary>
        private void ListViewSelectionChanged(Object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected) e.Item.Selected = false;
        }

        /// <summary>
        /// Updates the buttons labels.
        /// </summary>
        private void UpdateButtonLabels()
        {
            try
            {
                Int32 inst = 0;
                Int32 dele = 0;
                if (this.isLoading) return;
                foreach (ListViewItem item in this.listView.CheckedItems)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    if (this.entryStates.ContainsKey(entry.Id))
                    {
                        String state = this.entryStates[entry.Id];
                        if (state == "Installed" || state == "Update") dele++;
                        if (state == "New" || state == "Update") inst++;
                    }
                }
                this.installButton.Text = String.Format("Install {0} items.", inst);
                this.deleteButton.Text = String.Format("Delete {0} items.", dele);
                this.deleteButton.Enabled = dele > 0;
                this.installButton.Enabled = inst > 0;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Populates the list view with current entries.
        /// </summary>
        private void PopulateListView()
        {
            try
            {
                this.listView.BeginUpdate();
                this.pathTextBox.Text = PathHelper.ARCHIVE_DIR;
                foreach (DepEntry entry in this.depEntries)
                {
                    ListViewItem item = new ListViewItem(entry.Name);
                    item.Tag = entry; /* Store for later */
                    item.SubItems.Add(entry.Version);
                    item.SubItems.Add(entry.Desc);
                    item.SubItems.Add("New");
                    item.SubItems.Add(this.IsExecutable(entry) ? "Executable" : "Archive");
                    this.listView.Items.Add(item);
                    this.AddToGroup(item);
                }
                if (this.appGroups.Count > 1) this.listView.ShowGroups = true;
                else this.listView.ShowGroups = false;
                this.UpdateEntryStates();
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Adds the entry into a new or existing group.
        /// </summary>
        private void AddToGroup(ListViewItem item)
        {
            try
            {
                DepEntry entry = item.Tag as DepEntry;
                if (this.appGroups.ContainsKey(entry.Group))
                {
                    ListViewGroup lvg = this.appGroups[entry.Group];
                    item.Group = lvg;
                }
                else
                {
                    ListViewGroup lvg = new ListViewGroup(entry.Group);
                    this.appGroups[entry.Group] = lvg;
                    this.listView.Groups.Add(lvg);
                    item.Group = lvg;
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Creates a temporary file with the given extension.
        /// </summary>
        private String GetTempFileName(String file, Boolean unique)
        {
            try
            {
                Int32 counter = 0;
                String tempDir = Path.GetTempPath();
                String fileName = Path.GetFileName(file);
                String tempFile = Path.Combine(tempDir, "appman_" + fileName);
                if (!unique) return tempFile;
                while (File.Exists(tempFile))
                {
                    counter++;
                    tempFile = Path.Combine(tempDir, "appman_" + counter + "_" + fileName);
                }
                return tempFile;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Tries to delete old temp files.
        /// </summary>
        private void TryDeleteOldTempFiles()
        {
            String path = Path.GetTempPath();
            String[] oldFiles = Directory.GetFiles(path, "appman_*.*");
            foreach (String file in oldFiles)
            {
                try { File.Delete(file); }
                catch { /* NO ERRORS */ }
            }
        }

        /// <summary>
        /// Runs an executable process.
        /// </summary>
        private void RunExecutableProcess(String file)
        {
            try { Process.Start(file); }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Checks if entry is an executable.
        /// </summary>
        private Boolean IsExecutable(DepEntry entry)
        {
            return entry.Type == "Executable";
        }

        #endregion

        #region Entry Management

        /// <summary>
        /// Downloads the entry config file.
        /// </summary>
        private void LoadEntriesFile()
        {
            try
            {
                if (PathHelper.CONFIG_ADR.StartsWith("http"))
                {
                    WebClient client = new WebClient();
                    this.entriesFile = Path.GetTempFileName();
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.EntriesDownloadCompleted);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgressChanged);
                    client.DownloadFileAsync(new Uri(PathHelper.CONFIG_ADR), this.entriesFile);
                    this.statusLabel.Text = "Downloading item list...";
                }
                else
                {
                    this.entriesFile = PathHelper.CONFIG_ADR;
                    Object data = ObjectSerializer.Deserialize(this.entriesFile, this.depEntries);
                    this.statusLabel.Text = "Item list read from file.";
                    this.depEntries = data as DepEntries;
                    this.PopulateListView();
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// When entry config is loaded, populates the list view.
        /// </summary>
        private void EntriesDownloadCompleted(Object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Boolean fileExists = File.Exists(this.entriesFile);
                Boolean fileIsValid = File.ReadAllText(this.entriesFile).Length > 0;
                if (e.Error == null && fileExists && fileIsValid)
                {
                    this.statusLabel.Text = "Item list downloaded.";
                    Object data = ObjectSerializer.Deserialize(this.entriesFile, this.depEntries);
                    this.depEntries = data as DepEntries;
                    this.PopulateListView();
                }
                else this.statusLabel.Text = "Item list could not be downloaded.";
                this.progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            finally /* Try to delete temp file */
            {
                try { File.Delete(this.entriesFile); }
                catch { /* NO ERRORS*/ }
            }
        }

        /// <summary>
        /// Adds the currently selected entries to download queue.
        /// </summary>
        private void AddEntriesToQueue()
        {
            try
            {
                this.downloadQueue.Clear();
                foreach (ListViewItem item in this.listView.CheckedItems)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    String state = this.entryStates[entry.Id];
                    if (state == "New" || state == "Update")
                    {
                        this.downloadQueue.Enqueue(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Downloads next item from the queue.
        /// </summary>
        private void DownloadNextFromQueue()
        {
            try
            {
                this.fileQueue = new Queue<String>();
                this.curEntry = this.downloadQueue.Dequeue();
                foreach (String file in this.curEntry.Urls)
                {
                    this.fileQueue.Enqueue(file);
                }
                this.curFile = this.fileQueue.Dequeue();
                this.tempFile = this.GetTempFileName(this.curFile, false);
                this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                if (File.Exists(this.tempFile)) // Use already downloaded temp...
                {
                    String idPath = Path.Combine(PathHelper.ARCHIVE_DIR, this.curEntry.Id);
                    String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                    this.ExtractFile(this.tempFile, vnPath);
                    return;
                }
                this.tempFile = this.GetTempFileName(this.curFile, true);
                this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                this.webClient.DownloadFileAsync(new Uri(this.curFile), this.tempFile);
                this.statusLabel.Text = "Downloading: " + this.curFile;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Updates the progress bar for individual downloads.
        /// </summary>
        private void DownloadProgressChanged(Object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// When file is downloaded, check for errors and extract the file.
        /// </summary>
        private void DownloadFileCompleted(Object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isLoading = false;
                    this.cancelButton.Enabled = false;
                    this.statusLabel.Text = "Item downloading cancelled.";
                    this.TryDeleteOldTempFiles();
                    this.progressBar.Value = 0;
                    this.UpdateButtonLabels();
                }
                else if (e.Error == null)
                {
                    String idPath = Path.Combine(PathHelper.ARCHIVE_DIR, this.curEntry.Id);
                    String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                    this.ExtractFile(this.tempFile, vnPath);
                }
                else
                {
                    String message = "Error while downloading file: " + this.curFile + ".";
                    if (this.downloadQueue.Count > 1) message += "Trying to continue with the next item.";
                    DialogHelper.ShowError(message);
                    this.DownloadNextFromQueue();
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Starts the extraction work in a background thread.
        /// </summary>
        private void ExtractFile(String file, String path)
        {
            try
            {
                this.bgWorker = new BackgroundWorker();
                this.bgWorker.DoWork += new DoWorkEventHandler(this.WorkerDoWork);
                this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerDoCompleted);
                this.bgWorker.RunWorkerAsync(new BgArg(file, path));
                this.statusLabel.Text = "Extracting: " + this.curFile;
                this.progressBar.Style = ProgressBarStyle.Marquee;
            }
            catch
            {
                String message = "Error while extracting file: " + this.curFile + ".";
                if (this.downloadQueue.Count > 1) message += "Trying to continue with the next item.";
                DialogHelper.ShowError(message);
                this.DownloadNextFromQueue();
            }
        }

        /// <summary>
        /// Completes the actual extraction or file manipulation.
        /// </summary>
        private void WorkerDoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                BgArg args = e.Argument as BgArg;
                String url = new Uri(this.curFile).LocalPath;
                Boolean shouldExecute = this.IsExecutable(this.curEntry);
                if (!Directory.Exists(args.Path) && !shouldExecute) Directory.CreateDirectory(args.Path);
                if (Path.GetExtension(url) == ".zip") ZipHelper.ExtractZip(args.File, args.Path);
                else if (!shouldExecute)
                {
                    String fileName = Path.GetFileName(url);
                    File.Copy(this.tempFile, Path.Combine(args.Path, fileName), true);
                }
            }
            catch
            {
                DialogHelper.ShowError("Error while extracting file: " + this.curFile + ".\nTrying to continue with the next item.");
                this.DownloadNextFromQueue();
            }
        }

        /// <summary>
        /// When file hasd been handled, continues to next file or download next item.
        /// </summary>
        private void WorkerDoCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.progressBar.Style = ProgressBarStyle.Continuous;
                if (this.fileQueue.Count > 0)
                {
                    this.curFile = this.fileQueue.Dequeue();
                    this.tempFile = this.GetTempFileName(this.curFile, false);
                    this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                    if (File.Exists(this.tempFile)) // Use downloaded temp...
                    {
                        String idPath = Path.Combine(PathHelper.ARCHIVE_DIR, this.curEntry.Id);
                        String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                        this.ExtractFile(this.tempFile, vnPath);
                        this.bgWorker.Dispose();
                        return;
                    }
                    this.tempFile = this.GetTempFileName(this.curFile, true);
                    this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                    this.webClient.DownloadFileAsync(new Uri(this.curFile), this.tempFile);
                    this.statusLabel.Text = "Downloading: " + this.curFile;
                }
                else
                {
                    if (!this.IsExecutable(this.curEntry))
                    {
                        String idPath = Path.Combine(PathHelper.ARCHIVE_DIR, this.curEntry.Id);
                        this.RunEntrySetup(idPath, this.curEntry);
                        this.SaveEntryInfo(idPath, this.curEntry);
                        Thread.Sleep(100); // Wait for files...
                        this.LoadInstalledEntries();
                        this.shouldNotify = true;
                        this.UpdateEntryStates();
                    }
                    else this.RunExecutableProcess(this.tempFile);
                    if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                    else
                    {
                        this.isLoading = false;
                        this.progressBar.Value = 0;
                        this.cancelButton.Enabled = false;
                        this.statusLabel.Text = "All selected items completed.";
                        this.NoneLinkLabelLinkClicked(null, null);
                        this.UpdateButtonLabels();
                    }
                }
                this.bgWorker.Dispose();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Saves the entry info into a xml file.
        /// </summary>
        private void SaveEntryInfo(String path, DepEntry entry)
        {
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                ObjectSerializer.Serialize(Path.Combine(path, entry.Version + ".xml"), entry);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Reads the xml entry files from the archive.
        /// </summary>
        private void LoadInstalledEntries()
        {
            try
            {
                this.instEntries = new DepEntries();
                List<String> entryFiles = new List<string>();
                String[] entryDirs = Directory.GetDirectories(PathHelper.ARCHIVE_DIR);
                foreach (String dir in entryDirs)
                {
                    entryFiles.AddRange(Directory.GetFiles(dir, "*.xml"));
                }
                foreach (String file in entryFiles)
                {
                    Object data = ObjectSerializer.Deserialize(file, new DepEntry());
                    this.instEntries.Add(data as DepEntry);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Updates the entry states of the all items.
        /// </summary>
        private void UpdateEntryStates()
        {
            try
            {
                this.listView.BeginUpdate();
                foreach (ListViewItem item in this.listView.Items)
                {
                    DepEntry dep = item.Tag as DepEntry;
                    item.SubItems[3].ForeColor = SystemColors.ControlText;
                    this.entryStates[dep.Id] = "New";
                    item.SubItems[3].Text = "New";
                    foreach (DepEntry inst in this.instEntries)
                    {
                        item.UseItemStyleForSubItems = false;
                        if (dep.Id == inst.Id)
                        {
                            Color color = Color.Green;
                            String text = "Installed";
                            if (dep.Version != inst.Version || (dep.Version == inst.Version && dep.Build != inst.Build))
                            {
                                color = Color.Orange;
                                text = "Update";
                            }
                            this.entryStates[inst.Id] = text;
                            item.SubItems[3].ForeColor = color;
                            item.SubItems[3].Text = text;
                            break;
                        }
                    }
                }
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Executes the next setup command from queue.
        /// </summary>
        private void RunEntrySetup(String path, DepEntry entry)
        {
            try
            {
                if (!String.IsNullOrEmpty(entry.Cmd))
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    String data = ArgProcessor.ProcessArguments(entry.Cmd);
                    for (var i = 0; i < entry.Urls.Length; i++)
                    {
                        String url = entry.Urls[i];
                        if (entry.Temps.ContainsKey(url))
                        {
                            String index = i.ToString();
                            String temp = entry.Temps[url];
                            data = data.Replace("$URL{" + index + "}", url);
                            data = data.Replace("$TMP{" + index + "}", temp);
                        }
                    }
                    String cmd = Path.Combine(path, entry.Version + ".cmd");
                    File.WriteAllText(cmd, data);
                    Process process = new Process();
                    process.StartInfo.FileName = cmd;
                    process.EnableRaisingEvents = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(cmd);
                    process.Exited += delegate(Object sender, EventArgs e)
                    {
                        try { File.Delete(cmd); }
                        catch { /* NO ERRORS */ };
                    };
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        #endregion

    }

    #region Data Items

    [Serializable]
    [XmlRoot("Entries")]
    public class DepEntries : List<DepEntry>
    {
        public DepEntries(){}
    }

    [Serializable]
    [XmlType("Entry")]
    public class DepEntry
    {
        public String Id = "";
        public String Name = "";
        public String Desc = "";
        public String Group = "";
        public String Version = "";
        public String Build = "";
        public String Type = "";
        public String Info = "";
        public String Cmd = "";

        [XmlArrayItem("Url")]
        public String[] Urls = new String[0];

        [XmlIgnore]
        public Dictionary<String, String> Temps;

        public DepEntry()
        {
            this.Type = "Archive";
            this.Temps = new Dictionary<String, String>();
        }
        public DepEntry(String Id, String Name, String Desc, String Group, String Version, String Build, String Type, String Info, String Cmd, String[] Urls)
        {
            this.Id = Id;
            this.Name = Name;
            this.Desc = Desc;
            this.Group = Group;
            this.Version = Version;
            this.Build = Build;
            this.Temps = new Dictionary<String, String>();
            if (!String.IsNullOrEmpty(Type)) this.Type = Type;
            else this.Type = "Archive";
            this.Info = Info;
            this.Cmd = Cmd;
            this.Urls = Urls;
        }
    }

    [Serializable]
    public class Settings
    {
        public String Help = "";
        public String Config = "";
        public String Archive = "";

        [XmlArrayItem("Path")]
        public String[] Paths = new String[0];

        public Settings() { }
        public Settings(String Config, String Archive, String[] Paths, String Help)
        {
            this.Paths = Paths;
            this.Config = Config;
            this.Archive = Archive;
            this.Help = Help;
        }

    }

    public class BgArg
    {
        public String File = "";
        public String Path = "";

        public BgArg(String File, String Path)
        {
            this.File = File;
            this.Path = Path;
        }
    }

    #endregion

}
