using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Helpers;
using Microsoft.Win32;
using System.IO;
using PluginCore;

namespace FlashDevelop.Controls
{
    public class Browser : UserControl
    {
        ToolStrip toolStrip;
        ToolStripButton goButton;
        ToolStripButton backButton;
        ToolStripButton forwardButton;
        ToolStripButton refreshButton;
        ToolStripSpringComboBox addressComboBox;
        WebBrowserEx webBrowser;

        static Browser()
        {
            try
            {
                // Sets a key in registry so that latest .NET browser control is used
                string valueName = Path.GetFileName(Application.ExecutablePath);
                string subKey = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION\\";
                RegistryKey emu = Registry.CurrentUser.OpenSubKey(subKey, true);
                {
                    var value = emu.GetValue(valueName);
                    if (value is null) emu.SetValue(valueName, 0, RegistryValueKind.DWord);
                }
            }
            catch { } // No errors please...
        }

        public Browser()
        {
            Font = PluginBase.MainForm.Settings.DefaultFont;
            InitializeComponent();
            InitializeLocalization();
            InitializeInterface();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Browser));
            toolStrip = new PluginCore.Controls.ToolStripEx();
            backButton = new ToolStripButton();
            forwardButton = new ToolStripButton();
            refreshButton = new ToolStripButton();
            addressComboBox = new ToolStripSpringComboBox();
            goButton = new ToolStripButton();
            webBrowser = new WebBrowserEx();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.CanOverflow = false;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] {backButton, forwardButton, refreshButton, addressComboBox, goButton});
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(2, 1, 2, 2);
            toolStrip.Size = new Size(620, 25);
            toolStrip.TabIndex = 3;
            // 
            // backButton
            //
            backButton.Margin = new Padding(0, 1, 0, 1);
            backButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            backButton.Enabled = false;
            backButton.Image = ((Image)(resources.GetObject("backButton.Image")));
            backButton.ImageTransparentColor = Color.Magenta;
            backButton.Name = "backButton";
            backButton.Size = new Size(23, 22);
            backButton.Text = "Back";
            backButton.Click += BackButtonClick;
            // 
            // forwardButton
            //
            forwardButton.Margin = new Padding(0, 1, 0, 1);
            forwardButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            forwardButton.Enabled = false;
            forwardButton.Image = ((Image)(resources.GetObject("forwardButton.Image")));
            forwardButton.ImageTransparentColor = Color.Magenta;
            forwardButton.Name = "forwardButton";
            forwardButton.Size = new Size(23, 22);
            forwardButton.Text = "Forward";
            forwardButton.Click += ForwardButtonClick;
            // 
            // refreshButton
            //
            refreshButton.Margin = new Padding(0, 1, 1, 1);
            refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            refreshButton.Image = ((Image)(resources.GetObject("refreshButton.Image")));
            refreshButton.ImageTransparentColor = Color.Magenta;
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(23, 22);
            refreshButton.Text = "Refresh";
            refreshButton.Click += RefreshButtonClick;
            // 
            // addressComboBox
            //
            addressComboBox.Name = "addressComboBox";
            addressComboBox.Size = new Size(450, 21);
            addressComboBox.Padding = new Padding(0, 0, 1, 0);
            addressComboBox.KeyPress += AddressComboBoxKeyPress;
            addressComboBox.FlatCombo.SelectedIndexChanged += AddressComboBoxSelectedIndexChanged;
            // 
            // goButton
            //
            goButton.Margin = new Padding(0, 1, 0, 1);
            goButton.Alignment = ToolStripItemAlignment.Right;
            goButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            goButton.Image = ((Image)(resources.GetObject("goButton.Image")));
            goButton.ImageTransparentColor = Color.Magenta;
            goButton.Name = "goButton";
            goButton.Size = new Size(23, 22);
            goButton.Text = "Go";
            goButton.Click += BrowseButtonClick;
            // 
            // webBrowser
            //
            webBrowser.AllowWebBrowserDrop = true;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.WebBrowserShortcutsEnabled = false;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Location = new Point(0, 25);
            webBrowser.Name = "webBrowser";
            webBrowser.Size = new Size(620, 375);
            webBrowser.TabIndex = 2;
            webBrowser.CanGoForwardChanged += WebBrowserPropertyUpdated;
            webBrowser.CanGoBackChanged += WebBrowserPropertyUpdated;
            webBrowser.Navigated += WebBrowserNavigated;
            webBrowser.DocumentTitleChanged += WebBrowserDocumentTitleChanged;
            webBrowser.NewWindow += WebBrowserNewWindow;
            // 
            // Browser
            // 
            Controls.Add(webBrowser);
            Controls.Add(toolStrip);
            Name = "Browser";
            Size = new Size(620, 400);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for the webBrowser
        /// </summary>
        public WebBrowser WebBrowser => webBrowser;

        /// <summary>
        /// Accessor for the addressComboBox
        /// </summary>
        public ToolStripComboBoxEx AddressBox => addressComboBox;

        /// <summary>
        /// Initializes localized texts to the controls
        /// </summary>
        void InitializeLocalization()
        {
            goButton.Text = TextHelper.GetString("Label.Go");
            backButton.Text = TextHelper.GetString("Label.Back");
            forwardButton.Text = TextHelper.GetString("Label.Forward");
            refreshButton.Text = TextHelper.GetString("Label.Refresh");
        }

        /// <summary>
        /// Initializes the ui based on settings
        /// </summary>
        void InitializeInterface()
        {
            addressComboBox.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            toolStrip.Renderer = new DockPanelStripRenderer(true, true);
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            if (ScaleHelper.GetScale() >= 1.5)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Browser));
                goButton.Image = ((Image)(resources.GetObject("goButton.Image32")));
                forwardButton.Image = ((Image)(resources.GetObject("forwardButton.Image32")));
                refreshButton.Image = ((Image)(resources.GetObject("refreshButton.Image32")));
                backButton.Image = ((Image)(resources.GetObject("backButton.Image32")));
                Refresh();
            }
        }

        /// <summary>
        /// If the page tries to open a new window use a fd tab instead
        /// </summary>
        void WebBrowserNewWindow(object sender, CancelEventArgs e)
        {
            Globals.MainForm.CallCommand("Browse", webBrowser.StatusText);
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the web browser property changed event
        /// </summary>
        void WebBrowserPropertyUpdated(object sender, EventArgs e)
        {
            backButton.Enabled = webBrowser.CanGoBack;
            forwardButton.Enabled = webBrowser.CanGoForward;
        }

        /// <summary>
        /// Handles the web browser navigated event
        /// </summary>
        void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            addressComboBox.Text = webBrowser.Url.ToString();
            Globals.MainForm.OnScintillaControlUpdateControl(PluginBase.MainForm.CurrentDocument.SciControl);
        }

        /// <summary>
        /// Handles the web browser title changed event
        /// </summary>
        void WebBrowserDocumentTitleChanged(object sender, EventArgs e)
        {
            if (webBrowser.DocumentTitle.Trim().Length == 0)
            {
                string domain = webBrowser.Document.Domain.Trim();
                if (!string.IsNullOrEmpty(domain)) Parent.Text = domain;
                else Parent.Text = TextHelper.GetString("Info.UntitledFileStart");
            }
            else Parent.Text = webBrowser.DocumentTitle;
        }

        /// <summary>
        /// Handles the combo box index changed event
        /// </summary>
        void AddressComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (addressComboBox.SelectedItem != null)
            {
                string url = addressComboBox.SelectedItem.ToString();
                webBrowser.Navigate(url);
            }
        }

        /// <summary>
        /// Browses to the previous page in history
        /// </summary>
        void BackButtonClick(object sender, EventArgs e) => webBrowser.GoBack();

        /// <summary>
        /// Browses to the next page in history
        /// </summary>
        void ForwardButtonClick(object sender, EventArgs e) => webBrowser.GoForward();

        /// <summary>
        /// Reloads the current pages contents
        /// </summary>
        void RefreshButtonClick(object sender, EventArgs e) => webBrowser.Refresh();

        /// <summary>
        /// Browses to the specified url on click
        /// </summary>
        void BrowseButtonClick(object sender, EventArgs e)
        {
            string url = addressComboBox.Text;
            if (!addressComboBox.Items.Contains(url))
            {
                addressComboBox.Items.Insert(0, url);
            }
            webBrowser.Navigate(url);
        }

        /// <summary>
        /// Handles the combo box key press event
        /// </summary>
        void AddressComboBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string url = addressComboBox.Text;
                if (!addressComboBox.Items.Contains(url))
                {
                    addressComboBox.Items.Insert(0, url);
                }
                webBrowser.Navigate(url);
            }
        }

        #endregion

    }

    #region WebBrowserEx

    internal class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// Redirect events to MainForm.
        /// </summary>
        public override bool PreProcessMessage(ref Message msg)
        {
            return Globals.MainForm.PreProcessMessage(ref msg);
        }

    }

    #endregion

}