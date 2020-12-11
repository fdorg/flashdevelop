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
        string curFile;
        string tempFile;
        string localeId;
        bool isLoading;
        DepEntry curEntry;
        string entriesFile;
        WebClient webClient;
        DepEntries depEntries;
        DepEntries instEntries;
        BackgroundWorker bgWorker;
        Dictionary<string, string> entryStates;
        Dictionary<string, ListViewGroup> appGroups;
        Queue<DepEntry> downloadQueue;
        Queue<string> fileQueue;
        LocaleData localeData;
        bool localeOverride;
        bool configOverride;
        string[] notifyPaths;
        bool haveUpdates;
        bool checkOnly;
        string argsConfig;

        /**
        * Static link label margin constant
        */
        public static int LINK_MARGIN = 4;

        /**
        * Static constant for current distribution
        */
        public static string DISTRO_NAME = "FlashDevelop";

        /**
        * Static constant for exposed config groups (separated with ,)
        */
        public static string EXPOSED_GROUPS = "FD5";

        /**
        * Static type and state constants
        */ 
        public static string TYPE_LINK = "Link";
        public static string TYPE_EXECUTABLE = "Executable";
        public static string TYPE_ARCHIVE = "Archive";
        public static string STATE_INSTALLED = "Installed";
        public static string STATE_UPDATE = "Updated";
        public static string STATE_NEW = "New";

        public MainForm(string[] args)
        {
            CheckArgs(args);
            isLoading = false;
            haveUpdates = false;
            InitializeSettings();
            InitializeLocalization();
            InitializeComponent();
            InitializeGraphics();
            InitializeContextMenu();
            InitializeFormScaling();
            ApplyLocalizationStrings();
            Font = SystemFonts.MenuFont;
            if (!Win32.IsRunningOnMono) Application.AddMessageFilter(this);
        }

        #region Instancing

        /// <summary>
        /// Handle the instance message
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_SHOWME) RestoreWindow();
            base.WndProc(ref m);
        }

        /// <summary>
        /// Restore the window of the first instance
        /// </summary>
        void RestoreWindow()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            bool top = TopMost;
            TopMost = true;
            TopMost = top;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Processes command line args.
        /// </summary>
        void CheckArgs(string[] args)
        {
            checkOnly = false;
            localeId = "en_US";
            localeOverride = false;
            foreach (string arg in args)
            {
                // Handle minimized mode
                if (arg.Trim() == "-minimized")
                {
                    WindowState = FormWindowState.Minimized;
                    checkOnly = true;
                }
                // Handle locale id values
                if (arg.Trim().Contains("-locale="))
                {
                    localeId = arg.Trim().Substring("-locale=".Length);
                    localeOverride = true;
                }
                // Handle config values
                if (arg.Trim().Contains("-config="))
                {
                    argsConfig = arg.Trim().Substring("-config=".Length);
                    configOverride = true;
                }
            }
        }

        /// <summary>
        /// Initialize the scaling of the form.
        /// </summary>
        void InitializeFormScaling()
        {
            if (GetScale() > 1)
            {
                descHeader.Width = ScaleValue(319);
                nameHeader.Width = ScaleValue(160);
                versionHeader.Width = ScaleValue(90);
                statusHeader.Width = ScaleValue(70);
                typeHeader.Width = ScaleValue(75);
                infoHeader.Width = ScaleValue(30);
                int width = Convert.ToInt32(Width * 1.06);
                Size = new Size(width, Height);
            }
        }

        /// <summary>
        /// Initializes the graphics of the app.
        /// </summary>
        void InitializeGraphics()
        {
            var imageList = new ImageList();
            var assembly = Assembly.GetExecutingAssembly();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(ScaleValue(24), ScaleValue(24));
            imageList.Images.Add(Image.FromStream(assembly.GetManifestResourceStream("AppMan.Resources.Cancel.png")));
            Icon = new Icon(assembly.GetManifestResourceStream("AppMan.Resources.AppMan.ico"));
            cancelButton.ImageList = imageList;
            cancelButton.ImageIndex = 0;
        }

        /// <summary>
        /// Initializes the web client used for item downloads.
        /// </summary>
        void InitializeWebClient()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xC0 | 0x300 | 0xC00); // SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12
            webClient = new WebClient();
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadFileCompleted += DownloadFileCompleted;
        }

        /// <summary>
        /// Initializes the localization of the app.
        /// </summary>
        void InitializeLocalization()
        {
            localeData = new LocaleData();
            var localeDir = Path.Combine(PathHelper.GetExeDirectory(), "Locales");
            var localeFile = Path.Combine(localeDir, localeId + ".xml");
            if (File.Exists(localeFile)) localeData = ObjectSerializer.Deserialize(localeFile, localeData) as LocaleData;
        }

        /// <summary>
        /// Applies the localization string to controls.
        /// </summary>
        void ApplyLocalizationStrings()
        {
            Text = localeData.MainFormTitle;
            exploreButton.Text = localeData.ExploreLabel;
            nameHeader.Text = localeData.NameHeader;
            descHeader.Text = localeData.DescHeader;
            statusHeader.Text = localeData.StatusHeader;
            versionHeader.Text = localeData.VersionHeader;
            typeHeader.Text = localeData.TypeHeader;
            allLinkLabel.Text = localeData.LinkAll;
            newLinkLabel.Text = localeData.LinkNew;
            noneLinkLabel.Text = localeData.LinkNone;
            instLinkLabel.Text = localeData.LinkInstalled;
            updateLinkLabel.Text = localeData.LinkUpdates;
            statusLabel.Text = localeData.NoItemsSelected;
            pathLabel.Text = localeData.InstallPathLabel;
            selectLabel.Text = localeData.SelectLabel;
            installButton.Text = string.Format(localeData.InstallSelectedLabel, "0");
            deleteButton.Text = string.Format(localeData.DeleteSelectedLabel, "0");
        }

        /// <summary>
        /// Initializes the settings of the app.
        /// </summary>
        void InitializeSettings()
        {
            try
            {
                Settings settings = new Settings();
                string file = Path.Combine(PathHelper.GetExeDirectory(), "Config.xml");
                #if FLASHDEVELOP
                // Use the customized config file if present next to normal config file...
                if (File.Exists(file.Replace(".xml", ".local.xml"))) file = file.Replace(".xml", ".local.xml");
                #endif
                if (File.Exists(file))
                {
                    settings = ObjectSerializer.Deserialize(file, settings) as Settings;
                    if (!string.IsNullOrEmpty(settings.Name)) DISTRO_NAME = settings.Name;
                    if (!string.IsNullOrEmpty(settings.Groups)) EXPOSED_GROUPS = settings.Groups;
                    PathHelper.APPS_DIR = ArgProcessor.ProcessArguments(settings.Archive);
                    PathHelper.CONFIG_ADR = ArgProcessor.ProcessArguments(settings.Config);
                    PathHelper.HELP_ADR = ArgProcessor.ProcessArguments(settings.Help);
                    PathHelper.LOG_DIR = ArgProcessor.ProcessArguments(settings.Logs);
                    if (!localeOverride) localeId = settings.Locale;
                    notifyPaths = settings.Paths;
                }
                if (configOverride)
                {
                    PathHelper.CONFIG_ADR = argsConfig;
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
        void InitializeContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add(localeData.ShowInfoLabel, null, OnViewInfoClick);
            cms.Items.Add(localeData.ToggleCheckedLabel, null, OnCheckToggleClick);
            listView.ContextMenuStrip = cms;
        }

        #endregion

        #region Key Handling

        /// <summary>
        /// Closes the application when pressing Escape.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys k)
        {
            if (k == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, k);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the mouse wheel on hover
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (!Win32.IsRunningOnMono && m.Msg == 0x20a) // WM_MOUSEWHEEL
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = Win32.WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero)
                {
                    if (FromHandle(hWnd) != null)
                    {
                        Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                    else if (listView != null && hWnd == listView.Handle)
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
        void MainFormLoad(object sender, EventArgs e)
        {
            InitializeWebClient();
            depEntries = new DepEntries();
            entryStates = new Dictionary<string, string>();
            appGroups = new Dictionary<string, ListViewGroup>();
            downloadQueue = new Queue<DepEntry>();
            TryDeleteOldTempFiles();
            listView.Items.Clear();
            LoadInstalledEntries();
            LoadEntriesFile();
        }

        /// <summary>
        /// Opens the help when pressing help button or F1.
        /// </summary>
        void MainFormHelpRequested(object sender, HelpEventArgs e)
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
        void MainFormHelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            MainFormHelpRequested(null, null);
        }

        /// <summary>
        /// Save notification files to the notify paths.
        /// </summary>
        void NotifyPaths(bool restart)
        {
            try
            {
                if (notifyPaths == null) return;
                foreach (string nPath in notifyPaths)
                {
                    try
                    {
                        string path = Path.GetFullPath(ArgProcessor.ProcessArguments(nPath));
                        if (Directory.Exists(path))
                        {
                            string amFile = Path.Combine(path, ".appman");
                            if (restart) File.WriteAllText(amFile, "restart");
                            else File.WriteAllText(amFile, "");
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
        void OnViewInfoClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var item = listView.SelectedItems[0];
                var entry = item?.Tag as DepEntry;
                if (entry != null && !string.IsNullOrEmpty(entry.Info))
                {
                    RunExecutableProcess(entry.Info);
                }
            }
        }

        /// <summary>
        /// Toggles the check state of the item.
        /// </summary>
        void OnCheckToggleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem item = listView.SelectedItems[0];
                if (item != null) item.Checked = !item.Checked;
            }
        }

        /// <summary>
        /// Cancels the item download process.
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e)
        {
            try
            {
                webClient.CancelAsync();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Starts the download queue based on the user selections.
        /// </summary>
        void InstallButtonClick(object sender, EventArgs e)
        {
            isLoading = true;
            cancelButton.Enabled = true;
            installButton.Enabled = false;
            deleteButton.Enabled = false;
            AddEntriesToQueue();
            DownloadNextFromQueue();
        }

        /// <summary>
        /// Deletes the selected items from the archive.
        /// </summary>
        void DeleteButtonClick(object sender, EventArgs e)
        {
            try
            {
                string title = localeData.ConfirmTitle;
                string message = localeData.DeleteSelectedConfirm;
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in listView.CheckedItems)
                    {
                        DepEntry entry = item.Tag as DepEntry;
                        string state = entryStates[entry.Id];
                        if (state == STATE_INSTALLED || state == STATE_UPDATE)
                        {
                            #if FLASHDEVELOP
                            if (entry.Urls[0].ToLower().EndsWith(".fdz"))
                            {
                                string fileName = Path.GetFileName(entry.Urls[0]);
                                string delFile = Path.ChangeExtension(fileName, ".delete.fdz");
                                string tempFile = GetTempFileName(delFile, true);
                                string entryDir = Path.Combine(PathHelper.APPS_DIR, entry.Id);
                                string versionDir = Path.Combine(entryDir, entry.Version.ToLower());
                                string entryFile = Path.Combine(versionDir, fileName);
                                File.Copy(entryFile, tempFile, true);
                                RunExecutableProcess(tempFile);
                            }
                            #endif
                            TryDeleteEntryDir(entry);
                        }
                    }
                    NotifyPaths(false);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                Thread.Sleep(100); // Wait for files...
                NoneLinkLabelLinkClicked(null, null);
                LoadInstalledEntries();
                UpdateEntryStates();
                UpdateButtonLabels();
            }
        }

        /// <summary>
        /// Browses the archive with windows explorer.
        /// </summary>
        void ExploreButtonClick(object sender, EventArgs e)
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
        void AllLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isLoading) return;
            listView.BeginUpdate();
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = true;
            }
            listView.EndUpdate();
        }

        /// <summary>
        /// On None link click, deselects all items.
        /// </summary>
        void NoneLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isLoading) return;
            listView.BeginUpdate();
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = false;
            }
            listView.EndUpdate();
        }

        /// <summary>
        /// On New link click, selects all new items.
        /// </summary>
        void NewLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (isLoading) return;
                listView.BeginUpdate();
                foreach (ListViewItem item in listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    string state = entryStates[entry.Id];
                    if (state == STATE_NEW) item.Checked = true;
                    else item.Checked = false;
                }
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On Installed link click, selects all installed items.
        /// </summary>
        void InstLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (isLoading) return;
                listView.BeginUpdate();
                foreach (ListViewItem item in listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    string state = entryStates[entry.Id];
                    if (state == STATE_INSTALLED) item.Checked = true;
                    else item.Checked = false;
                }
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On Updates link click, selects all updatable items.
        /// </summary>
        void UpdatesLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (isLoading) return;
                listView.BeginUpdate();
                foreach (ListViewItem item in listView.Items)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    string state = entryStates[entry.Id];
                    if (state == STATE_UPDATE) item.Checked = true;
                    else item.Checked = false;
                }
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// On bundle link click, selects all bundled items.
        /// </summary>
        void BundleLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (isLoading) return;
                listView.BeginUpdate();
                bool is64bit = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) != "x86";
                foreach (ListViewItem item in listView.Items)
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
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Disables the item checking when downloading.
        /// </summary>
        void ListViewItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isLoading) e.NewValue = e.CurrentValue;
        }

        /// <summary>
        /// Updates the button labels when item is checked.
        /// </summary>
        void ListViewItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (isLoading) return;
            UpdateButtonLabels();
        }

        /// <summary>
        /// Handles the clicking of the info item.
        /// </summary>
        void ListViewClick(object sender, EventArgs e)
        {
            Point point = listView.PointToClient(MousePosition);
            ListViewHitTestInfo hitTest = listView.HitTest(point);
            int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
            if (columnIndex == 2)
            {
                DepEntry entry = hitTest.Item.Tag as DepEntry;
                RunExecutableProcess(entry.Info);
            }
        }

        /// <summary>
        /// Change cursor when hovering info sub item.
        /// </summary>
        void ListViewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = listView.PointToClient(MousePosition);
            ListViewHitTestInfo hitTest = listView.HitTest(point);
            if (hitTest.Item != null)
            {
                int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
                if (columnIndex == 2) Cursor = Cursors.Hand;
                else Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the drawing of the info image.
        /// </summary>
        readonly Image InfoImage = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AppMan.Resources.Information.png"));

        void ListViewDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Header == infoHeader)
            {
                if (!e.Item.Selected && (e.ItemState & ListViewItemStates.Selected) == 0)
                {
                    e.DrawBackground();
                }
                else if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                }
                int posOffsetX = (e.Bounds.Width - e.Bounds.Height) / 2;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(InfoImage, new Rectangle(e.Bounds.X + posOffsetX, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
            }
            else e.DrawDefault = true;
        }

        void ListViewDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            else e.DrawDefault = true; 
        }

        void ListViewDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Completes the minimized update process.
        /// </summary>
        void CompleteMinimizedProcess()
        {
            if (checkOnly)
            {
                if (haveUpdates)
                {
                    WindowState = FormWindowState.Normal;
                    Activate();
                }
                else Application.Exit();
            }
        }

        /// <summary>
        /// Updates the buttons labels.
        /// </summary>
        void UpdateButtonLabels()
        {
            try
            {
                int inst = 0;
                int dele = 0;
                if (isLoading) return;
                foreach (ListViewItem item in listView.CheckedItems)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    if (entryStates.ContainsKey(entry.Id))
                    {
                        string state = entryStates[entry.Id];
                        if (state == STATE_INSTALLED || state == STATE_UPDATE) dele++;
                        if (state == STATE_NEW || state == STATE_UPDATE) inst++;
                    }
                }
                installButton.Text = string.Format(localeData.InstallSelectedLabel, inst);
                deleteButton.Text = string.Format(localeData.DeleteSelectedLabel, dele);
                deleteButton.Enabled = dele > 0;
                installButton.Enabled = inst > 0;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Populates the list view with current entries.
        /// </summary>
        void PopulateListView()
        {
            try
            {
                listView.BeginUpdate();
                pathTextBox.Text = PathHelper.APPS_DIR;
                foreach (DepEntry entry in depEntries)
                {
                    ListViewItem item = new ListViewItem(entry.Name);
                    item.Tag = entry; /* Store for later */
                    item.SubItems.Add(entry.Version);
                    item.SubItems.Add(entry.Info);
                    item.SubItems.Add(entry.Desc);
                    item.SubItems.Add(GetLocaleState(STATE_NEW));
                    item.SubItems.Add(GetLocaleType(entry.Type));
                    listView.Items.Add(item);
                    AddToGroup(item);
                }
                if (appGroups.Count > 1) listView.ShowGroups = true;
                else listView.ShowGroups = false;
                UpdateEntryStates();
                UpdateLinkPositions();
                GenerateBundleLinks();
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Update the link label positions for example if the font is different size.
        /// </summary>
        void UpdateLinkPositions()
        {
            allLinkLabel.Location = new Point(selectLabel.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
            noneLinkLabel.Location = new Point(allLinkLabel.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
            newLinkLabel.Location = new Point(noneLinkLabel.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
            instLinkLabel.Location = new Point(newLinkLabel.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
            updateLinkLabel.Location = new Point(instLinkLabel.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
        }

        /// <summary>
        /// Generates the bundle selection links.
        /// </summary>
        void GenerateBundleLinks()
        {
            LinkLabel prevLink = updateLinkLabel;
            List<string> bundleLinks = new List<string>();
            foreach (DepEntry entry in depEntries)
            {
                foreach (string bundle in entry.Bundles)
                {
                    if (!bundleLinks.Contains(bundle))
                    {
                        LinkLabel linkLabel = new LinkLabel();
                        linkLabel.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
                        linkLabel.LinkClicked += BundleLinkLabelLinkClicked;
                        linkLabel.Location = new Point(prevLink.Bounds.Right + LINK_MARGIN, allLinkLabel.Location.Y);
                        linkLabel.Links[0].LinkData = bundle;
                        linkLabel.LinkColor = Color.Green;
                        linkLabel.AutoSize = true;
                        linkLabel.Text = bundle;
                        bundleLinks.Add(bundle);
                        Controls.Add(linkLabel);
                        prevLink = linkLabel;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the entry into a new or existing group.
        /// </summary>
        void AddToGroup(ListViewItem item)
        {
            try
            {
                DepEntry entry = item.Tag as DepEntry;
                if (appGroups.ContainsKey(entry.Group))
                {
                    ListViewGroup lvg = appGroups[entry.Group];
                    item.Group = lvg;
                }
                else
                {
                    ListViewGroup lvg = new ListViewGroup(entry.Group);
                    appGroups[entry.Group] = lvg;
                    listView.Groups.Add(lvg);
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
        string GetTempFileName(string file, bool unique)
        {
            try
            {
                int counter = 0;
                string tempDir = Path.GetTempPath();
                string fileName = Path.GetFileName(file);
                string tempFile = Path.Combine(tempDir, "appman_" + fileName);
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
        void TryDeleteOldTempFiles()
        {
            string path = Path.GetTempPath();
            string[] oldFiles = Directory.GetFiles(path, "appman_*.*");
            foreach (string file in oldFiles)
            {
                try { File.Delete(file); }
                catch { /* NO ERRORS */ }
            }
        }

        /// <summary>
        /// Try to delete old entry directory
        /// </summary>
        void TryDeleteEntryDir(DepEntry entry)
        {
            string folder = Path.Combine(PathHelper.APPS_DIR, entry.Id);
            // Sometimes we might get "dir not empty" error, try 10 times...
            for (int attempts = 0; attempts < 10; attempts++)
            {
                try
                {
                    if (Directory.Exists(folder)) Directory.Delete(folder, true);
                    return;
                }
                catch (IOException) { Thread.Sleep(50); }
            }
            throw new Exception(localeData.DeleteDirError + folder);
        }

        /// <summary>
        /// Runs an executable process.
        /// </summary>
        void RunExecutableProcess(string file, bool wait)
        {
            try 
            {
                #if FLASHDEVELOP
                if (file.ToLower().EndsWith(".fdz"))
                {
                    string fd = Path.Combine(PathHelper.GetExeDirectory(), @"..\..\" + DISTRO_NAME + ".exe");
                    bool waitfd = Process.GetProcessesByName(DISTRO_NAME).Length == 0;
                    if (File.Exists(fd))
                    {
                        Process.Start(Path.GetFullPath(fd), "\"" + file + "\" -silent -reuse");
                        // If FD was not running, give it a little time to start...
                        if (waitfd) Thread.Sleep(500);
                        return;
                    }
                }
                #endif
                Process process = new Process();
                process.StartInfo.FileName = file;
                process.Start();
                if (wait)
                {
                    process.WaitForExit();
                    NotifyPaths(true);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        void RunExecutableProcess(string file)
        {
            RunExecutableProcess(file, false);
        }

        /// <summary>
        /// Gets the locale string for state
        /// </summary>
        string GetLocaleState(string state)
        {
            if (state == STATE_INSTALLED) return localeData.StateInstalled;
            else if (state == STATE_UPDATE) return localeData.StateUpdate;
            else return localeData.StateNew;
        }

        /// <summary>
        /// Gets the locale string for type
        /// </summary>
        string GetLocaleType(string type)
        {
            if (type == TYPE_LINK) return localeData.TypeLink;
            else if (type == TYPE_EXECUTABLE) return localeData.TypeExecutable;
            else return localeData.TypeArchive;
        }

        /// <summary>
        /// Checks if entry is an executable.
        /// </summary>
        bool IsExecutable(DepEntry entry)
        {
            return entry.Type == TYPE_EXECUTABLE;
        }

        /// <summary>
        /// Checks if entry is an executable.
        /// </summary>
        bool IsLink(DepEntry entry)
        {
            return entry.Type == TYPE_LINK;
        }

        #endregion

        #region Entry Management

        /// <summary>
        /// Downloads the entry config file.
        /// </summary>
        void LoadEntriesFile()
        {
            try
            {
                if (PathHelper.CONFIG_ADR.StartsWith("http"))
                {
                    WebClient client = new WebClient();
                    entriesFile = Path.GetTempFileName();
                    client.DownloadFileCompleted += EntriesDownloadCompleted;
                    client.DownloadProgressChanged += DownloadProgressChanged;
                    client.DownloadFileAsync(new Uri(PathHelper.CONFIG_ADR), entriesFile);
                    statusLabel.Text = localeData.DownloadingItemList;
                }
                else
                {
                    entriesFile = PathHelper.CONFIG_ADR;
                    object data = ObjectSerializer.Deserialize(entriesFile, depEntries, EXPOSED_GROUPS);
                    statusLabel.Text = localeData.ItemListOpened;
                    depEntries = data as DepEntries;
                    PopulateListView();
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                CompleteMinimizedProcess();
            }
        }

        /// <summary>
        /// When entry config is loaded, populates the list view.
        /// </summary>
        void EntriesDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                bool fileExists = File.Exists(entriesFile);
                bool fileIsValid = File.ReadAllText(entriesFile).Length > 0;
                if (e.Error == null && fileExists && fileIsValid)
                {
                    statusLabel.Text = localeData.DownloadedItemList;
                    object data = ObjectSerializer.Deserialize(entriesFile, depEntries, EXPOSED_GROUPS);
                    depEntries = data as DepEntries;
                    PopulateListView();
                }
                else statusLabel.Text = localeData.ItemListDownloadFailed;
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                progressBar.Value = 0;
            }
            catch (Exception ex)
            { 
                DialogHelper.ShowError(ex.ToString());
            }
            finally
            {
                CompleteMinimizedProcess();
                try { File.Delete(entriesFile); }
                catch { /* NO ERRORS*/ }
            }
        }

        /// <summary>
        /// Adds the currently selected entries to download queue.
        /// </summary>
        void AddEntriesToQueue()
        {
            try
            {
                downloadQueue.Clear();
                foreach (ListViewItem item in listView.CheckedItems)
                {
                    DepEntry entry = item.Tag as DepEntry;
                    string state = entryStates[entry.Id];
                    if (state == STATE_NEW || state == STATE_UPDATE)
                    {
                        downloadQueue.Enqueue(entry);
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
        void DownloadNextFromQueue()
        {
            try
            {
                fileQueue = new Queue<string>();
                curEntry = downloadQueue.Dequeue();
                foreach (string file in curEntry.Urls)
                {
                    if (IsLink(curEntry)) RunExecutableProcess(file);
                    else fileQueue.Enqueue(file);
                }
                if (IsLink(curEntry))
                {
                    if (downloadQueue.Count > 0) DownloadNextFromQueue();
                    else
                    {
                        isLoading = false;
                        progressBar.Value = 0;
                        cancelButton.Enabled = false;
                        TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        statusLabel.Text = localeData.AllItemsCompleted;
                        NoneLinkLabelLinkClicked(null, null);
                        UpdateButtonLabels();
                    }
                    return;
                }
                curFile = fileQueue.Dequeue();
                tempFile = GetTempFileName(curFile, false);
                curEntry.Temps[curFile] = tempFile; // Save for cmd
                if (File.Exists(tempFile)) // Use already downloaded temp...
                {
                    string idPath = Path.Combine(PathHelper.APPS_DIR, curEntry.Id);
                    string vnPath = Path.Combine(idPath, curEntry.Version.ToLower());
                    ExtractFile(tempFile, vnPath);
                    return;
                }
                tempFile = GetTempFileName(curFile, true);
                curEntry.Temps[curFile] = tempFile; // Save for cmd
                webClient.DownloadFileAsync(new Uri(curFile), tempFile);
                statusLabel.Text = localeData.DownloadingFile + curFile;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Updates the progress bar for individual downloads.
        /// </summary>
        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            TaskbarProgress.SetValue(Handle, e.ProgressPercentage, 100);
        }

        /// <summary>
        /// When file is downloaded, check for errors and extract the file.
        /// </summary>
        void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    isLoading = false;
                    cancelButton.Enabled = false;
                    statusLabel.Text = localeData.ItemListDownloadCancelled;
                    TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    TryDeleteOldTempFiles();
                    progressBar.Value = 0;
                    UpdateButtonLabels();
                }
                else if (e.Error == null)
                {
                    // Verify checksum of the file if specified
                    if (!string.IsNullOrEmpty(curEntry.Checksum) && !VerifyFile(curEntry.Checksum, tempFile))
                    {
                        string message = localeData.ChecksumVerifyError + curFile + ".\n";
                        if (downloadQueue.Count > 0) message += localeData.ContinueWithNextItem;
                        DialogHelper.ShowError(message); // Show message first...
                        if (downloadQueue.Count > 0) DownloadNextFromQueue();
                        else
                        {
                            isLoading = false;
                            progressBar.Value = 0;
                            cancelButton.Enabled = false;
                            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                            statusLabel.Text = localeData.AllItemsCompleted;
                            NoneLinkLabelLinkClicked(null, null);
                            TryDeleteEntryDir(curEntry);
                            TryDeleteOldTempFiles();
                            UpdateButtonLabels();
                        }
                    }
                    else
                    {
                        string idPath = Path.Combine(PathHelper.APPS_DIR, curEntry.Id);
                        string vnPath = Path.Combine(idPath, curEntry.Version.ToLower());
                        ExtractFile(tempFile, vnPath);
                    }
                }
                else
                {
                    string message = localeData.DownloadingError + curFile + ".\n";
                    if (downloadQueue.Count > 0) message += localeData.ContinueWithNextItem;
                    DialogHelper.ShowError(message); // Show message first...
                    if (downloadQueue.Count > 0) DownloadNextFromQueue();
                    else
                    {
                        isLoading = false;
                        progressBar.Value = 0;
                        cancelButton.Enabled = false;
                        TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        statusLabel.Text = localeData.AllItemsCompleted;
                        NoneLinkLabelLinkClicked(null, null);
                        TryDeleteEntryDir(curEntry);
                        TryDeleteOldTempFiles();
                        UpdateButtonLabels();
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
        void ExtractFile(string file, string path)
        {
            try
            {
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += WorkerDoWork;
                bgWorker.RunWorkerCompleted += WorkerDoCompleted;
                bgWorker.RunWorkerAsync(new BgArg(file, path));
                statusLabel.Text = localeData.ExtractingFile + curFile;
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            catch
            {
                string message = localeData.ExtractingError + curFile + ".\n";
                if (downloadQueue.Count > 0) message += localeData.ContinueWithNextItem;
                DialogHelper.ShowError(message); // Show message first...
                if (downloadQueue.Count > 0) DownloadNextFromQueue();
                else
                {
                    isLoading = false;
                    progressBar.Value = 0;
                    cancelButton.Enabled = false;
                    TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    statusLabel.Text = localeData.AllItemsCompleted;
                    NoneLinkLabelLinkClicked(null, null);
                    TryDeleteEntryDir(curEntry);
                    TryDeleteOldTempFiles();
                    UpdateButtonLabels();
                }
            }
        }

        /// <summary>
        /// Completes the actual extraction or file manipulation.
        /// </summary>
        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BgArg args = e.Argument as BgArg;
                string url = new Uri(curFile).LocalPath;
                bool shouldExecute = IsExecutable(curEntry);
                if (!Directory.Exists(args.Path) && !shouldExecute) Directory.CreateDirectory(args.Path);
                if (Path.GetExtension(url) == ".zip") ZipHelper.ExtractZip(args.File, args.Path);
                else if (!shouldExecute)
                {
                    string fileName = Path.GetFileName(url);
                    File.Copy(tempFile, Path.Combine(args.Path, fileName), true);
                }
            }
            catch
            {
                string message = localeData.ExtractingError + curFile + ".\n";
                if (downloadQueue.Count > 0) message += localeData.ContinueWithNextItem;
                DialogHelper.ShowError(message); // Show message first...
                if (downloadQueue.Count > 0) DownloadNextFromQueue();
                else
                {
                    isLoading = false;
                    progressBar.Value = 0;
                    cancelButton.Enabled = false;
                    TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                    statusLabel.Text = localeData.AllItemsCompleted;
                    NoneLinkLabelLinkClicked(null, null);
                    TryDeleteEntryDir(curEntry);
                    TryDeleteOldTempFiles();
                    UpdateButtonLabels();
                }
            }
        }

        /// <summary>
        /// When file has been handled, continues to next file or download next item.
        /// </summary>
        void WorkerDoCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
                if (fileQueue.Count > 0)
                {
                    curFile = fileQueue.Dequeue();
                    tempFile = GetTempFileName(curFile, false);
                    curEntry.Temps[curFile] = tempFile; // Save for cmd
                    if (File.Exists(tempFile)) // Use downloaded temp...
                    {
                        string idPath = Path.Combine(PathHelper.APPS_DIR, curEntry.Id);
                        string vnPath = Path.Combine(idPath, curEntry.Version.ToLower());
                        ExtractFile(tempFile, vnPath);
                        bgWorker.Dispose();
                        return;
                    }
                    tempFile = GetTempFileName(curFile, true);
                    curEntry.Temps[curFile] = tempFile; // Save for cmd
                    webClient.DownloadFileAsync(new Uri(curFile), tempFile);
                    statusLabel.Text = localeData.DownloadingFile + curFile;
                }
                else
                {
                    if (!IsExecutable(curEntry))
                    {
                        string idPath = Path.Combine(PathHelper.APPS_DIR, curEntry.Id);
                        RunEntrySetup(idPath, curEntry);
                        SaveEntryInfo(idPath, curEntry);
                        #if FLASHDEVELOP
                        if (curEntry.Urls[0].ToLower().EndsWith(".fdz"))
                        {
                            string vnPath = Path.Combine(idPath, curEntry.Version.ToLower());
                            string fileName = Path.GetFileName(curEntry.Urls[0]);
                            string filePath = Path.Combine(vnPath, fileName);
                            RunExecutableProcess(filePath);
                        }
                        #endif
                        Thread.Sleep(100); // Wait for files...
                        LoadInstalledEntries();
                        UpdateEntryStates();
                        NotifyPaths(false);
                    }
                    else RunExecutableProcess(tempFile, true);
                    if (downloadQueue.Count > 0) DownloadNextFromQueue();
                    else
                    {
                        isLoading = false;
                        progressBar.Value = 0;
                        cancelButton.Enabled = false;
                        TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
                        statusLabel.Text = localeData.AllItemsCompleted;
                        NoneLinkLabelLinkClicked(null, null);
                        UpdateButtonLabels();
                    }
                }
                bgWorker.Dispose();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Saves the entry info into a xml file.
        /// </summary>
        void SaveEntryInfo(string path, DepEntry entry)
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
        void LoadInstalledEntries()
        {
            try
            {
                instEntries = new DepEntries();
                List<string> entryFiles = new List<string>();
                string[] entryDirs = Directory.GetDirectories(PathHelper.APPS_DIR);
                foreach (string dir in entryDirs)
                {
                    entryFiles.AddRange(Directory.GetFiles(dir, "*.xml"));
                }
                foreach (string file in entryFiles)
                {
                    object data = ObjectSerializer.Deserialize(file, new DepEntry());
                    instEntries.Add(data as DepEntry);
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
        void UpdateEntryStates()
        {
            try
            {
                listView.BeginUpdate();
                foreach (ListViewItem item in listView.Items)
                {
                    DepEntry dep = item.Tag as DepEntry;
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[4].ForeColor = SystemColors.ControlText;
                    item.SubItems[4].Text = GetLocaleState(STATE_NEW);
                    entryStates[dep.Id] = STATE_NEW;
                    foreach (DepEntry inst in instEntries)
                    {
                        if (dep.Id == inst.Id)
                        {
                            Color color = Color.Green;
                            string state = STATE_INSTALLED;
                            string text = GetLocaleState(STATE_INSTALLED);
                            if (CustomCompare(dep, inst) > 0 || (dep.Version == inst.Version && dep.Build != inst.Build))
                            {
                                haveUpdates = true;
                                text = GetLocaleState(STATE_UPDATE);
                                state = STATE_UPDATE;
                                color = Color.Orange;
                            }
                            entryStates[inst.Id] = state;
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
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Compare version numbers
        /// </summary>
        int CustomCompare(DepEntry dep, DepEntry inst)
        {
            try
            {
                string[] v1 = dep.Version.Replace("+", ".").Split('.');
                string[] v2 = inst.Version.Replace("+", ".").Split('.');
                for (int i = 0; i < v1.Length; i++)
                {
                    try
                    {
                        int t1 = Convert.ToInt32(v1[i]);
                        int t2 = Convert.ToInt32(v2[i]);
                        int comp = t1.CompareTo(t2);
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
        bool VerifyFile(string checksum, string file)
        {
            try 
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (Stream stream = File.OpenRead(file))
                    {
                        string hex = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
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
        void RunEntrySetup(string path, DepEntry entry)
        {
            try
            {
                if (!string.IsNullOrEmpty(entry.Cmd))
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    string data = ArgProcessor.ProcessArguments(entry.Cmd);
                    for (int i = 0; i < entry.Urls.Length; i++)
                    {
                        string url = entry.Urls[i];
                        if (entry.Temps.ContainsKey(url))
                        {
                            string index = i.ToString();
                            string temp = entry.Temps[url];
                            data = data.Replace("$URL{" + index + "}", url);
                            data = data.Replace("$TMP{" + index + "}", temp);
                        }
                    }
                    string cmd = Path.Combine(path, entry.Version + ".cmd");
                    File.WriteAllText(cmd, data);
                    Process process = new Process();
                    process.StartInfo.FileName = cmd;
                    process.EnableRaisingEvents = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(cmd);
                    process.Exited += delegate(object sender, EventArgs e)
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
        double curScale = double.MinValue;

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public int ScaleValue(int value)
        {
            return (int)(value * GetScale());
        }

        /// <summary>
        /// Gets the current display scale.
        /// </summary>
        public double GetScale()
        {
            if (curScale != double.MinValue) return curScale;
            using (Graphics g = Graphics.FromHwnd(Handle))
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
        public string File;
        public string Path;

        public BgArg(string file, string path)
        {
            File = file;
            Path = path;
        }
    }

    [Serializable]
    [XmlType("Entry")]
    public class DepEntry
    {
        public string Id = "";
        public string Name = "";
        public string Desc = "";
        public string Group = "";
        public string Version = "";
        public string Checksum = "";
        public string Build = "";
        public string Type = "";
        public string Info = "";
        public string Cmd = "";

        [XmlArrayItem("Url")]
        public string[] Urls = new string[0];

        [XmlArrayItem("Bundle")]
        public string[] Bundles = new string[0];

        [XmlIgnore]
        public Dictionary<string, string> Temps;

        public DepEntry()
        {
            Type = MainForm.TYPE_ARCHIVE;
            Temps = new Dictionary<string, string>();
        }
        public DepEntry(string id, string name, string desc, string group, string version, string build, string type, string info, string cmd, string[] urls, string[] bundles, string checksum)
        {
            Id = id;
            Name = name;
            Desc = desc;
            Group = group;
            Build = build;
            Version = version;
            Bundles = bundles;
            Checksum = checksum;
            Temps = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(type)) Type = type;
            else Type = MainForm.TYPE_ARCHIVE;
            Info = info;
            Urls = urls;
            Cmd = cmd;
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
        public string Help = "";
        public string Logs = "";
        public string Config = "";
        public string Archive = "";
        public string Locale = "en_US";
        public string Name = "FlashDevelop";
        public string Groups = "FD5";

        [XmlArrayItem("Path")]
        public string[] Paths = new string[0];

        public Settings() {}
        public Settings(string config, string archive, string[] paths, string locale, string help, string logs, string name, string groups)
        {
            Logs = logs;
            Paths = paths;
            Config = config;
            Archive = archive;
            Locale = locale;
            Groups = groups;
            Name = name;
            Help = help;
        }

    }

    [Serializable]
    [XmlRoot("Locale")]
    public class LocaleData
    {
        public LocaleData() {}
        public string NameHeader = "Name";
        public string VersionHeader = "Version";
        public string DescHeader = "Description";
        public string StatusHeader = "Status";
        public string TypeHeader = "Type";
        public string StateNew = "New";
        public string StateUpdate = "Update";
        public string StateInstalled = "Installed";
        public string ExtractingFile = "Extracting: ";
        public string DownloadingFile = "Downloading: ";
        public string DownloadingItemList = "Downloading item list...";
        public string NoItemsSelected = "No items selected.";
        public string ItemListOpened = "Item list read from file.";
        public string DownloadedItemList = "Item list downloaded.";
        public string AllItemsCompleted = "All selected items completed.";
        public string ItemListDownloadCancelled = "Item list download cancelled.";
        public string ItemListDownloadFailed = "Item list could not be downloaded.";
        public string DeleteSelectedConfirm = "Are you sure to delete all versions of the selected items?";
        public string ContinueWithNextItem = "Trying to continue with the next item.";
        public string ChecksumVerifyError = "The specified checksum did not match the file: ";
        public string DownloadingError = "Error while downloading file: ";
        public string ExtractingError = "Error while extracting file: ";
        public string DeleteDirError = "Error while deleting directory: ";
        public string MainFormTitle = "AppMan";
        public string ConfirmTitle = "Confirm";
        public string LinkAll = "All";
        public string LinkNone = "None";
        public string LinkInstalled = "Installed";
        public string LinkUpdates = "Updates";
        public string LinkNew = "New";
        public string SelectLabel = "Select:";
        public string ExploreLabel = "Explore...";
        public string InstallPathLabel = "Install path:";
        public string DeleteSelectedLabel = "Delete {0} items.";
        public string InstallSelectedLabel = "Install {0} items.";
        public string ToggleCheckedLabel = "Toggle Checked";
        public string ShowInfoLabel = "Show Info...";
        public string TypeExecutable = "Executable";
        public string TypeArchive = "Archive";
        public string TypeLink = "Link";
    }

    #endregion
}