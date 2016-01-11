using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using AppMan.Utilities;

namespace AppMan
{
    public partial class MainForm : Form, IMessageFilter
    {
        private String curFile;
        private String tempFile;
        private String localeId;
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
        private LocaleData localeData;
        private Boolean localeOverride;
        private String[] notifyPaths;
        private Boolean haveUpdates;
        private Boolean checkOnly;

        /**
        * Static link label margin constant
        */
        public static Int32 LINK_MARGIN = 4;

        /**
        * Static constant for current distribution
        */
        public static String DISTRO_NAME = "FlashDevelop";

        /**
        * Static constant for exposed config groups (separated with ,)
        */
        public static String EXPOSED_GROUPS = "FD5";

        /**
        * Static type and state constants
        */ 
        public static String TYPE_LINK = "Link";
        public static String TYPE_EXECUTABLE = "Executable";
        public static String TYPE_ARCHIVE = "Archive";
        public static String STATE_INSTALLED = "Installed";
        public static String STATE_UPDATE = "Updated";
        public static String STATE_NEW = "New";

        public MainForm(String[] args)
        {
            this.CheckArgs(args);
            this.isLoading = false;
            this.haveUpdates = false;
            this.InitializeSettings();
            this.InitializeLocalization();
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeContextMenu();
            this.InitializeFormScaling();
            this.ApplyLocalizationStrings();
            this.Font = SystemFonts.MenuFont;
            if (!Win32.IsRunningOnMono) Application.AddMessageFilter(this);
        }

        #region Instancing

        /// <summary>
        /// Handle the instance message
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_SHOWME) this.RestoreWindow();
            base.WndProc(ref m);
        }

        /// <summary>
        /// Restore the window of the first instance
        /// </summary>
        private void RestoreWindow()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            Boolean top = this.TopMost;
            this.TopMost = true;
            this.TopMost = top;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Processes command line args.
        /// </summary>
        private void CheckArgs(String[] args)
        {
            this.checkOnly = false;
            this.localeId = "en_US";
            this.localeOverride = false;
            foreach (String arg in args)
            {
                // Handle minimized mode
                if (arg.Trim() == "-minimized")
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.checkOnly = true;
                }
                // Handle locale id values
                if (arg.Trim().Contains("-locale="))
                {
                    this.localeId = arg.Trim().Substring("-locale=".Length);
                    this.localeOverride = true;
                }
            }
        }

        /// <summary>
        /// Initialize the scaling of the form.
        /// </summary>
        private void InitializeFormScaling()
        {
            if (this.GetScale() > 1)
            {
                this.descHeader.Width = this.ScaleValue(319);
                this.nameHeader.Width = this.ScaleValue(160);
                this.versionHeader.Width = this.ScaleValue(90);
                this.statusHeader.Width = this.ScaleValue(70);
                this.typeHeader.Width = this.ScaleValue(75);
                this.infoHeader.Width = this.ScaleValue(30);
                Int32 width = Convert.ToInt32(this.Width * 1.06);
                this.Size = new Size(width, this.Height);
            }
        }

        /// <summary>
        /// Initializes the graphics of the app.
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            Assembly assembly = Assembly.GetExecutingAssembly();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(this.ScaleValue(24), this.ScaleValue(24));
            imageList.Images.Add(Image.FromStream(assembly.GetManifestResourceStream("AppMan.Resources.Cancel.png")));
            this.Icon = new Icon(assembly.GetManifestResourceStream("AppMan.Resources.AppMan.ico"));
            this.cancelButton.ImageList = imageList;
            this.cancelButton.ImageIndex = 0;
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
        /// Initializes the localization of the app.
        /// </summary>
        private void InitializeLocalization()
        {
            this.localeData = new LocaleData();
            String localeDir = Path.Combine(PathHelper.GetExeDirectory(), "Locales");
            String localeFile = Path.Combine(localeDir, this.localeId + ".xml");
            if (File.Exists(localeFile))
            {
                this.localeData = ObjectSerializer.Deserialize(localeFile, this.localeData) as LocaleData;
            }
        }

        /// <summary>
        /// Applies the localization string to controls.
        /// </summary>
        private void ApplyLocalizationStrings()
        {
            this.Text = this.localeData.MainFormTitle;
            this.exploreButton.Text = this.localeData.ExploreLabel;
            this.nameHeader.Text = this.localeData.NameHeader;
            this.descHeader.Text = this.localeData.DescHeader;
            this.statusHeader.Text = this.localeData.StatusHeader;
            this.versionHeader.Text = this.localeData.VersionHeader;
            this.typeHeader.Text = this.localeData.TypeHeader;
            this.allLinkLabel.Text = this.localeData.LinkAll;
            this.newLinkLabel.Text = this.localeData.LinkNew;
            this.noneLinkLabel.Text = this.localeData.LinkNone;
            this.instLinkLabel.Text = this.localeData.LinkInstalled;
            this.updateLinkLabel.Text = this.localeData.LinkUpdates;
            this.statusLabel.Text = this.localeData.NoItemsSelected;
            this.pathLabel.Text = this.localeData.InstallPathLabel;
            this.selectLabel.Text = this.localeData.SelectLabel;
            this.installButton.Text = String.Format(this.localeData.InstallSelectedLabel, "0");
            this.deleteButton.Text = String.Format(this.localeData.DeleteSelectedLabel, "0");
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
                #if FLASHDEVELOP
                // Use the customized config file if present next to normal config file...
                if (File.Exists(file.Replace(".xml", ".local.xml"))) file = file.Replace(".xml", ".local.xml");
                #endif
                if (File.Exists(file))
                {
                    settings = ObjectSerializer.Deserialize(file, settings) as Settings;
                    if (!String.IsNullOrEmpty(settings.Name)) MainForm.DISTRO_NAME = settings.Name;
                    if (!String.IsNullOrEmpty(settings.Groups)) MainForm.EXPOSED_GROUPS = settings.Groups;
                    PathHelper.APPS_DIR = ArgProcessor.ProcessArguments(settings.Archive);
                    PathHelper.CONFIG_ADR = ArgProcessor.ProcessArguments(settings.Config);
                    PathHelper.HELP_ADR = ArgProcessor.ProcessArguments(settings.Help);
                    PathHelper.LOG_DIR = ArgProcessor.ProcessArguments(settings.Logs);
                    if (!this.localeOverride) this.localeId = settings.Locale;
                    this.notifyPaths = settings.Paths;
                }
                if (!Directory.Exists(PathHelper.LOG_DIR))
                {
                    Directory.CreateDirectory(PathHelper.LOG_DIR);
                }
                if (!Directory.Exists(PathHelper.APPS_DIR))
                {
                    Directory.CreateDirectory(PathHelper.APPS_DIR);
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
            cms.Items.Add(this.localeData.ShowInfoLabel, null, new EventHandler(this.OnViewInfoClick));
            cms.Items.Add(this.localeData.ToggleCheckedLabel, null, new EventHandler(this.OnCheckToggleClick));
            this.listView.ContextMenuStrip = cms;
        }

        #endregion

        #region Key Handling

        /// <summary>
        /// Closes the application when pressing Escape.
        /// </summary>
        protected override Boolean ProcessCmdKey(ref Message msg, Keys k)
        {
            if (k == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, k);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the mouse wheel on hover
        /// </summary>
        public Boolean PreFilterMessage(ref Message m)
        {
            if (!Win32.IsRunningOnMono && m.Msg == 0x20a) // WM_MOUSEWHEEL
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = Win32.WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero)
                {
                    if (Control.FromHandle(hWnd) != null)
                    {
                        Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                    else if (this.listView != null && hWnd == this.listView.Handle)
                    {
                        Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                }
            }
            return false;
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

        /// <summary>
        /// Opens the help when pressing help button or F1.
        /// </summary>
        private void MainFormHelpButtonClicked(Object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.MainFormHelpRequested(null, null);
        }

        /// <summary>
        /// Save notification files to the notify paths.
        /// </summary>
        private void NotifyPaths()
        {
            try
            {
                if (this.notifyPaths == null) return;
                foreach (String nPath in this.notifyPaths)
                {
                    try
                    {
                        String path = Path.GetFullPath(ArgProcessor.ProcessArguments(nPath));
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
        /// Open info file or url when clicked.
        /// </summary>
        private void OnViewInfoClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                ListViewItem item = this.listView.SelectedItems[0];
                if (item != null)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    if (entry != null && !String.IsNullOrEmpty(entry.Info))
                    {
                        this.RunExecutableProcess(entry.Info);
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the check state of the item.
        /// </summary>
        private void OnCheckToggleClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                ListViewItem item = this.listView.SelectedItems[0];
                if (item != null) item.Checked = !item.Checked;
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
                String title = this.localeData.ConfirmTitle;
                String message = this.localeData.DeleteSelectedConfirm;
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in this.listView.CheckedItems)
                    {
                        DepEntry entry = item.Tag as DepEntry;
                        String state = this.entryStates[entry.Id];
                        if (state == STATE_INSTALLED || state == STATE_UPDATE)
                        {
                            #if FLASHDEVELOP
                            if (entry.Urls[0].ToLower().EndsWith(".fdz"))
                            {
                                String fileName = Path.GetFileName(entry.Urls[0]);
                                String delFile = Path.ChangeExtension(fileName, ".delete.fdz");
                                String tempFile = this.GetTempFileName(delFile, true);
                                String entryDir = Path.Combine(PathHelper.APPS_DIR, entry.Id);
                                String versionDir = Path.Combine(entryDir, entry.Version.ToLower());
                                String entryFile = Path.Combine(versionDir, fileName);
                                File.Copy(entryFile, tempFile, true);
                                this.RunExecutableProcess(tempFile);
                            }
                            #endif
                            this.TryDeleteEntryDir(entry);
                        }
                    }
                    this.NotifyPaths();
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
                Process.Start("explorer.exe", PathHelper.APPS_DIR);
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
                    if (state == STATE_NEW) item.Checked = true;
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
                    if (state == STATE_INSTALLED) item.Checked = true;
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
                    if (state == STATE_UPDATE) item.Checked = true;
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
        /// On bundle link click, selects all bundled items.
        /// </summary>
        private void BundleLinkLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (this.isLoading) return;
                this.listView.BeginUpdate();
                Boolean is64bit = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) != "x86";
                foreach (ListViewItem item in this.listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    if (Array.IndexOf(entry.Bundles, e.Link.LinkData.ToString()) != -1)
                    {
                        if (!entry.Name.Contains("(x86)") && !entry.Name.Contains("(x64)")) item.Checked = true;
                        else
                        {
                            if (!is64bit && entry.Name.Contains("(x86)")) item.Checked = true;
                            else if (is64bit && entry.Name.Contains("(x64)")) item.Checked = true;
                            else item.Checked = false;
                        }
                    }
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
        /// Handles the clicking of the info item.
        /// </summary>
        private void ListViewClick(Object sender, EventArgs e)
        {
            Point point = this.listView.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = this.listView.HitTest(point);
            Int32 columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
            if (columnIndex == 2)
            {
                DepEntry entry = hitTest.Item.Tag as DepEntry;
                this.RunExecutableProcess(entry.Info);
            }
        }

        /// <summary>
        /// Change cursor when hovering info sub item.
        /// </summary>
        private void ListViewMouseMove(Object sender, MouseEventArgs e)
        {
            Point point = this.listView.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = this.listView.HitTest(point);
            if (hitTest.Item != null)
            {
                Int32 columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
                if (columnIndex == 2) this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the drawing of the info image.
        /// </summary>
        private Image InfoImage = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AppMan.Resources.Information.png"));
        private void ListViewDrawSubItem(Object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Header == this.infoHeader)
            {
                if (!e.Item.Selected && (e.ItemState & ListViewItemStates.Selected) == 0)
                {
                    e.DrawBackground();
                }
                else if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                }
                Int32 posOffsetX = (e.Bounds.Width - e.Bounds.Height) / 2;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(InfoImage, new Rectangle(e.Bounds.X + posOffsetX, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
            }
            else e.DrawDefault = true;
        }
        private void ListViewDrawItem(Object sender, DrawListViewItemEventArgs e)
        {
            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            else e.DrawDefault = true; 
        }
        private void ListViewDrawColumnHeader(Object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Completes the minimized update process.
        /// </summary>
        private void CompleteMinimizedProcess()
        {
            if (this.checkOnly)
            {
                if (this.haveUpdates)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
                else Application.Exit();
            }
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
                        if (state == STATE_INSTALLED || state == STATE_UPDATE) dele++;
                        if (state == STATE_NEW || state == STATE_UPDATE) inst++;
                    }
                }
                this.installButton.Text = String.Format(this.localeData.InstallSelectedLabel, inst);
                this.deleteButton.Text = String.Format(this.localeData.DeleteSelectedLabel, dele);
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
                this.pathTextBox.Text = PathHelper.APPS_DIR;
                foreach (DepEntry entry in this.depEntries)
                {
                    ListViewItem item = new ListViewItem(entry.Name);
                    item.Tag = entry; /* Store for later */
                    item.SubItems.Add(entry.Version);
                    item.SubItems.Add(entry.Info);
                    item.SubItems.Add(entry.Desc);
                    item.SubItems.Add(this.GetLocaleState(STATE_NEW));
                    item.SubItems.Add(this.GetLocaleType(entry.Type));
                    this.listView.Items.Add(item);
                    this.AddToGroup(item);
                }
                if (this.appGroups.Count > 1) this.listView.ShowGroups = true;
                else this.listView.ShowGroups = false;
                this.UpdateEntryStates();
                this.UpdateLinkPositions();
                this.GenerateBundleLinks();
                this.listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Update the link label positions for example if the font is different size.
        /// </summary>
        private void UpdateLinkPositions()
        {
            this.allLinkLabel.Location = new Point(this.selectLabel.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
            this.noneLinkLabel.Location = new Point(this.allLinkLabel.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
            this.newLinkLabel.Location = new Point(this.noneLinkLabel.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
            this.instLinkLabel.Location = new Point(this.newLinkLabel.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
            this.updateLinkLabel.Location = new Point(this.instLinkLabel.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
        }

        /// <summary>
        /// Generates the bundle selection links.
        /// </summary>
        private void GenerateBundleLinks()
        {
            LinkLabel prevLink = this.updateLinkLabel;
            List<String> bundleLinks = new List<String>();
            foreach (DepEntry entry in this.depEntries)
            {
                foreach (String bundle in entry.Bundles)
                {
                    if (!bundleLinks.Contains(bundle))
                    {
                        LinkLabel linkLabel = new LinkLabel();
                        linkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                        linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.BundleLinkLabelLinkClicked);
                        linkLabel.Location = new Point(prevLink.Bounds.Right + LINK_MARGIN, this.allLinkLabel.Location.Y);
                        linkLabel.Links[0].LinkData = bundle;
                        linkLabel.LinkColor = Color.Green;
                        linkLabel.AutoSize = true;
                        linkLabel.Text = bundle;
                        bundleLinks.Add(bundle);
                        this.Controls.Add(linkLabel);
                        prevLink = linkLabel;
                    }
                }
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
        /// Try to delete old entry directory
        /// </summary>
        private void TryDeleteEntryDir(DepEntry entry)
        {
            String folder = Path.Combine(PathHelper.APPS_DIR, entry.Id);
            // Sometimes we might get "dir not empty" error, try 10 times...
            for (Int32 attempts = 0; attempts < 10; attempts++)
            {
                try
                {
                    if (Directory.Exists(folder)) Directory.Delete(folder, true);
                    return;
                }
                catch (IOException) { Thread.Sleep(50); }
            }
            throw new Exception(this.localeData.DeleteDirError + folder);
        }

        /// <summary>
        /// Runs an executable process.
        /// </summary>
        private void RunExecutableProcess(String file)
        {
            try 
            {
                #if FLASHDEVELOP
                if (file.ToLower().EndsWith(".fdz"))
                {
                    String fd = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\" + DISTRO_NAME + ".exe");
                    Boolean wait = Process.GetProcessesByName(DISTRO_NAME).Length == 0;
                    if (File.Exists(fd))
                    {
                        Process.Start(Path.GetFullPath(fd), file + " -silent -reuse");
                        // If FD was not running, give it a little time to start...
                        if (wait) Thread.Sleep(500);
                        return;
                    }
                }
                #endif
                Process.Start(file);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Gets the locale string for state
        /// </summary>
        private String GetLocaleState(String state)
        {
            if (state == STATE_INSTALLED) return this.localeData.StateInstalled;
            else if (state == STATE_UPDATE) return this.localeData.StateUpdate;
            else return this.localeData.StateNew;
        }

        /// <summary>
        /// Gets the locale string for type
        /// </summary>
        private String GetLocaleType(String type)
        {
            if (type == TYPE_LINK) return this.localeData.TypeLink;
            else if (type == TYPE_EXECUTABLE) return this.localeData.TypeExecutable;
            else return this.localeData.TypeArchive;
        }

        /// <summary>
        /// Checks if entry is an executable.
        /// </summary>
        private Boolean IsExecutable(DepEntry entry)
        {
            return entry.Type == TYPE_EXECUTABLE;
        }

        /// <summary>
        /// Checks if entry is an executable.
        /// </summary>
        private Boolean IsLink(DepEntry entry)
        {
            return entry.Type == TYPE_LINK;
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
                    this.statusLabel.Text = this.localeData.DownloadingItemList;
                }
                else
                {
                    this.entriesFile = PathHelper.CONFIG_ADR;
                    Object data = ObjectSerializer.Deserialize(this.entriesFile, this.depEntries, MainForm.EXPOSED_GROUPS);
                    this.statusLabel.Text = this.localeData.ItemListOpened;
                    this.depEntries = data as DepEntries;
                    this.PopulateListView();
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                this.CompleteMinimizedProcess();
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
                    this.statusLabel.Text = this.localeData.DownloadedItemList;
                    Object data = ObjectSerializer.Deserialize(this.entriesFile, this.depEntries, MainForm.EXPOSED_GROUPS);
                    this.depEntries = data as DepEntries;
                    this.PopulateListView();
                }
                else this.statusLabel.Text = this.localeData.ItemListDownloadFailed;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                this.progressBar.Value = 0;
            }
            catch (Exception ex)
            { 
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                this.CompleteMinimizedProcess();
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
                    if (state == STATE_NEW || state == STATE_UPDATE)
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
                    if (this.IsLink(this.curEntry)) this.RunExecutableProcess(file);
                    else this.fileQueue.Enqueue(file);
                }
                if (this.IsLink(this.curEntry))
                {
                    if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                    else
                    {
                        this.isLoading = false;
                        this.progressBar.Value = 0;
                        this.cancelButton.Enabled = false;
                        TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        this.statusLabel.Text = this.localeData.AllItemsCompleted;
                        this.NoneLinkLabelLinkClicked(null, null);
                        this.UpdateButtonLabels();
                    }
                    return;
                }
                this.curFile = this.fileQueue.Dequeue();
                this.tempFile = this.GetTempFileName(this.curFile, false);
                this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                if (File.Exists(this.tempFile)) // Use already downloaded temp...
                {
                    String idPath = Path.Combine(PathHelper.APPS_DIR, this.curEntry.Id);
                    String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                    this.ExtractFile(this.tempFile, vnPath);
                    return;
                }
                this.tempFile = this.GetTempFileName(this.curFile, true);
                this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                this.webClient.DownloadFileAsync(new Uri(this.curFile), this.tempFile);
                this.statusLabel.Text = this.localeData.DownloadingFile + this.curFile;
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
            TaskbarProgress.SetValue(this.Handle, e.ProgressPercentage, 100);
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
                    this.statusLabel.Text = this.localeData.ItemListDownloadCancelled;
                    TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    this.TryDeleteOldTempFiles();
                    this.progressBar.Value = 0;
                    this.UpdateButtonLabels();
                }
                else if (e.Error == null)
                {
                    // Verify checksum of the file if specified
                    if (!String.IsNullOrEmpty(this.curEntry.Checksum) && !this.VerifyFile(this.curEntry.Checksum, this.tempFile))
                    {
                        String message = this.localeData.ChecksumVerifyError + this.curFile + ".\n";
                        if (this.downloadQueue.Count > 0) message += this.localeData.ContinueWithNextItem;
                        DialogHelper.ShowError(message); // Show message first...
                        if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                        else
                        {
                            this.isLoading = false;
                            this.progressBar.Value = 0;
                            this.cancelButton.Enabled = false;
                            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                            this.statusLabel.Text = this.localeData.AllItemsCompleted;
                            this.NoneLinkLabelLinkClicked(null, null);
                            this.TryDeleteEntryDir(this.curEntry);
                            this.TryDeleteOldTempFiles();
                            this.UpdateButtonLabels();
                        }
                    }
                    else
                    {
                        String idPath = Path.Combine(PathHelper.APPS_DIR, this.curEntry.Id);
                        String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                        this.ExtractFile(this.tempFile, vnPath);
                    }
                }
                else
                {
                    String message = this.localeData.DownloadingError + this.curFile + ".\n";
                    if (this.downloadQueue.Count > 0) message += this.localeData.ContinueWithNextItem;
                    DialogHelper.ShowError(message); // Show message first...
                    if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                    else
                    {
                        this.isLoading = false;
                        this.progressBar.Value = 0;
                        this.cancelButton.Enabled = false;
                        TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        this.statusLabel.Text = this.localeData.AllItemsCompleted;
                        this.NoneLinkLabelLinkClicked(null, null);
                        this.TryDeleteEntryDir(this.curEntry);
                        this.TryDeleteOldTempFiles();
                        this.UpdateButtonLabels();
                    }
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
                this.statusLabel.Text = this.localeData.ExtractingFile + this.curFile;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Indeterminate);
                this.progressBar.Style = ProgressBarStyle.Marquee;
            }
            catch
            {
                String message = this.localeData.ExtractingError + this.curFile + ".\n";
                if (this.downloadQueue.Count > 0) message += this.localeData.ContinueWithNextItem;
                DialogHelper.ShowError(message); // Show message first...
                if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                else
                {
                    this.isLoading = false;
                    this.progressBar.Value = 0;
                    this.cancelButton.Enabled = false;
                    TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    this.statusLabel.Text = this.localeData.AllItemsCompleted;
                    this.NoneLinkLabelLinkClicked(null, null);
                    this.TryDeleteEntryDir(this.curEntry);
                    this.TryDeleteOldTempFiles();
                    this.UpdateButtonLabels();
                }
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
                String message = this.localeData.ExtractingError + this.curFile + ".\n";
                if (this.downloadQueue.Count > 0) message += this.localeData.ContinueWithNextItem;
                DialogHelper.ShowError(message); // Show message first...
                if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                else
                {
                    this.isLoading = false;
                    this.progressBar.Value = 0;
                    this.cancelButton.Enabled = false;
                    TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    this.statusLabel.Text = this.localeData.AllItemsCompleted;
                    this.NoneLinkLabelLinkClicked(null, null);
                    this.TryDeleteEntryDir(this.curEntry);
                    this.TryDeleteOldTempFiles();
                    this.UpdateButtonLabels();
                }
            }
        }

        /// <summary>
        /// When file has been handled, continues to next file or download next item.
        /// </summary>
        private void WorkerDoCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                
                this.progressBar.Style = ProgressBarStyle.Continuous;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                if (this.fileQueue.Count > 0)
                {
                    this.curFile = this.fileQueue.Dequeue();
                    this.tempFile = this.GetTempFileName(this.curFile, false);
                    this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                    if (File.Exists(this.tempFile)) // Use downloaded temp...
                    {
                        String idPath = Path.Combine(PathHelper.APPS_DIR, this.curEntry.Id);
                        String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                        this.ExtractFile(this.tempFile, vnPath);
                        this.bgWorker.Dispose();
                        return;
                    }
                    this.tempFile = this.GetTempFileName(this.curFile, true);
                    this.curEntry.Temps[this.curFile] = this.tempFile; // Save for cmd
                    this.webClient.DownloadFileAsync(new Uri(this.curFile), this.tempFile);
                    this.statusLabel.Text = this.localeData.DownloadingFile + this.curFile;
                }
                else
                {
                    if (!this.IsExecutable(this.curEntry))
                    {
                        String idPath = Path.Combine(PathHelper.APPS_DIR, this.curEntry.Id);
                        this.RunEntrySetup(idPath, this.curEntry);
                        this.SaveEntryInfo(idPath, this.curEntry);
                        #if FLASHDEVELOP
                        if (this.curEntry.Urls[0].ToLower().EndsWith(".fdz"))
                        {
                            String vnPath = Path.Combine(idPath, this.curEntry.Version.ToLower());
                            String fileName = Path.GetFileName(this.curEntry.Urls[0]);
                            String filePath = Path.Combine(vnPath, fileName);
                            this.RunExecutableProcess(filePath);
                        }
                        #endif
                        Thread.Sleep(100); // Wait for files...
                        this.LoadInstalledEntries();
                        this.UpdateEntryStates();
                        this.NotifyPaths();
                    }
                    else this.RunExecutableProcess(this.tempFile);
                    if (this.downloadQueue.Count > 0) this.DownloadNextFromQueue();
                    else
                    {
                        this.isLoading = false;
                        this.progressBar.Value = 0;
                        this.cancelButton.Enabled = false;
                        TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        this.statusLabel.Text = this.localeData.AllItemsCompleted;
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
                List<String> entryFiles = new List<String>();
                String[] entryDirs = Directory.GetDirectories(PathHelper.APPS_DIR);
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
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[4].ForeColor = SystemColors.ControlText;
                    item.SubItems[4].Text = this.GetLocaleState(STATE_NEW);
                    this.entryStates[dep.Id] = STATE_NEW;
                    foreach (DepEntry inst in this.instEntries)
                    {
                        if (dep.Id == inst.Id)
                        {
                            Color color = Color.Green;
                            String state = STATE_INSTALLED;
                            String text = this.GetLocaleState(STATE_INSTALLED);
                            if (this.CustomCompare(dep, inst) > 0 || (dep.Version == inst.Version && dep.Build != inst.Build))
                            {
                                this.haveUpdates = true;
                                text = this.GetLocaleState(STATE_UPDATE);
                                state = STATE_UPDATE;
                                color = Color.Orange;
                            }
                            this.entryStates[inst.Id] = state;
                            item.SubItems[4].ForeColor = color;
                            item.SubItems[4].Text = text;
                            // If we get an exact match, we don't need to compare more...
                            if (dep.Version == inst.Version && dep.Build == inst.Build)
                            {
                                break;
                            }
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
        /// Compare version numbers
        /// </summary>
        private Int32 CustomCompare(DepEntry dep, DepEntry inst)
        {
            try
            {
                String[] v1 = dep.Version.Replace("+", ".").Split('.');
                String[] v2 = inst.Version.Replace("+", ".").Split('.');
                for (Int32 i = 0; i < v1.Length; i++)
                {
                    try
                    {
                        Int32 t1 = Convert.ToInt32(v1[i]);
                        Int32 t2 = Convert.ToInt32(v2[i]);
                        Int32 comp = t1.CompareTo(t2);
                        if (comp > 0) return 1;
                        else if (comp < 0) return -1;
                    }
                    catch {};
                }
                return 0;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// Verifies the checksum (MD5 in hex) of the file.
        /// </summary>
        private Boolean VerifyFile(String checksum, String file)
        {
            try 
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (Stream stream = File.OpenRead(file))
                    {
                        String hex = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                        return hex.ToLower() == checksum.ToLower();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
                return false;
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
                    for (Int32 i = 0; i < entry.Urls.Length; i++)
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

        #region Scaling Helpers

        /// <summary>
        /// Current scale of the form.
        /// </summary>
        private Double curScale = Double.MinValue;

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public Int32 ScaleValue(Int32 value)
        {
            return (Int32)(value * GetScale());
        }

        /// <summary>
        /// Gets the current display scale.
        /// </summary>
        public Double GetScale()
        {
            if (curScale != Double.MinValue) return curScale;
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                curScale = g.DpiX / 96f;
            }
            return curScale;
        }

        #endregion

    }

    #region Data Items

    public class BgArg
    {
        public String File = "";
        public String Path = "";

        public BgArg(String file, String path)
        {
            this.File = file;
            this.Path = path;
        }
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
        public String Checksum = "";
        public String Build = "";
        public String Type = "";
        public String Info = "";
        public String Cmd = "";

        [XmlArrayItem("Url")]
        public String[] Urls = new String[0];

        [XmlArrayItem("Bundle")]
        public String[] Bundles = new String[0];

        [XmlIgnore]
        public Dictionary<String, String> Temps;

        public DepEntry()
        {
            this.Type = MainForm.TYPE_ARCHIVE;
            this.Temps = new Dictionary<String, String>();
        }
        public DepEntry(String id, String name, String desc, String group, String version, String build, String type, String info, String cmd, String[] urls, String[] bundles, String checksum)
        {
            this.Id = id;
            this.Name = name;
            this.Desc = desc;
            this.Group = group;
            this.Build = build;
            this.Version = version;
            this.Bundles = bundles;
            this.Checksum = checksum;
            this.Temps = new Dictionary<String, String>();
            if (!String.IsNullOrEmpty(type)) this.Type = type;
            else this.Type = MainForm.TYPE_ARCHIVE;
            this.Info = info;
            this.Urls = urls;
            this.Cmd = cmd;
        }
    }

    [Serializable]
    [XmlRoot("Entries")]
    public class DepEntries : List<DepEntry>
    {
        public DepEntries() {}
    }

    [Serializable]
    public class Settings
    {
        public String Help = "";
        public String Logs = "";
        public String Config = "";
        public String Archive = "";
        public String Locale = "en_US";
        public String Name = "FlashDevelop";
        public String Groups = "FD5";

        [XmlArrayItem("Path")]
        public String[] Paths = new String[0];

        public Settings() {}
        public Settings(String config, String archive, String[] paths, String locale, String help, String logs, String name, String groups)
        {
            this.Logs = logs;
            this.Paths = paths;
            this.Config = config;
            this.Archive = archive;
            this.Locale = locale;
            this.Groups = groups;
            this.Name = name;
            this.Help = help;
        }

    }

    [Serializable]
    [XmlRoot("Locale")]
    public class LocaleData
    {
        public LocaleData() {}
        public String NameHeader = "Name";
        public String VersionHeader = "Version";
        public String DescHeader = "Description";
        public String StatusHeader = "Status";
        public String TypeHeader = "Type";
        public String StateNew = "New";
        public String StateUpdate = "Update";
        public String StateInstalled = "Installed";
        public String ExtractingFile = "Extracting: ";
        public String DownloadingFile = "Downloading: ";
        public String DownloadingItemList = "Downloading item list...";
        public String NoItemsSelected = "No items selected.";
        public String ItemListOpened = "Item list read from file.";
        public String DownloadedItemList = "Item list downloaded.";
        public String AllItemsCompleted = "All selected items completed.";
        public String ItemListDownloadCancelled = "Item list download cancelled.";
        public String ItemListDownloadFailed = "Item list could not be downloaded.";
        public String DeleteSelectedConfirm = "Are you sure to delete all versions of the selected items?";
        public String ContinueWithNextItem = "Trying to continue with the next item.";
        public String ChecksumVerifyError = "The specified checksum did not match the file: ";
        public String DownloadingError = "Error while downloading file: ";
        public String ExtractingError = "Error while extracting file: ";
        public String DeleteDirError = "Error while deleting directory: ";
        public String MainFormTitle = "AppMan";
        public String ConfirmTitle = "Confirm";
        public String LinkAll = "All";
        public String LinkNone = "None";
        public String LinkInstalled = "Installed";
        public String LinkUpdates = "Updates";
        public String LinkNew = "New";
        public String SelectLabel = "Select:";
        public String ExploreLabel = "Explore...";
        public String InstallPathLabel = "Install path:";
        public String DeleteSelectedLabel = "Delete {0} items.";
        public String InstallSelectedLabel = "Install {0} items.";
        public String ToggleCheckedLabel = "Toggle Checked";
        public String ShowInfoLabel = "Show Info...";
        public String TypeExecutable = "Executable";
        public String TypeArchive = "Archive";
        public String TypeLink = "Link";
    }

    #endregion

}
