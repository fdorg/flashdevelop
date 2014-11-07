using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;

namespace FlashDevelop.Controls
{
    public class Browser : UserControl
    {
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton goButton;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripSpringComboBox addressComboBox;
        private FlashDevelop.Controls.WebBrowserEx webBrowser;

        public Browser()
        {
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.InitializeLocalization();
            this.InitializeInterface();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Browser));
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.addressComboBox = new System.Windows.Forms.ToolStripSpringComboBox();
            this.goButton = new System.Windows.Forms.ToolStripButton();
            this.webBrowser = new FlashDevelop.Controls.WebBrowserEx();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.backButton, this.forwardButton, this.refreshButton, this.addressComboBox, this.goButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(620, 25);
            this.toolStrip.TabIndex = 3;
            // 
            // backButton
            //
            this.backButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backButton.Enabled = false;
            this.backButton.Image = ((System.Drawing.Image)(resources.GetObject("backButton.Image")));
            this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 22);
            this.backButton.Text = "Back";
            this.backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // forwardButton
            //
            this.forwardButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.Image = ((System.Drawing.Image)(resources.GetObject("forwardButton.Image")));
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(23, 22);
            this.forwardButton.Text = "Forward";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
            // 
            // refreshButton
            //
            this.refreshButton.Margin = new System.Windows.Forms.Padding(0, 1, 1, 1);
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshButton.Image")));
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // addressComboBox
            //
            this.addressComboBox.Name = "addressComboBox";
            this.addressComboBox.Size = new System.Drawing.Size(450, 21);
            this.addressComboBox.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.addressComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddressComboBoxKeyPress);
            this.addressComboBox.SelectedIndexChanged += new System.EventHandler(this.AddressComboBoxSelectedIndexChanged);
            // 
            // goButton
            //
            this.goButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.goButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.goButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goButton.Image = ((System.Drawing.Image)(resources.GetObject("goButton.Image")));
            this.goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(23, 22);
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // webBrowser
            //
            this.webBrowser.AllowWebBrowserDrop = true;
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 25);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(620, 375);
            this.webBrowser.TabIndex = 2;
            this.webBrowser.CanGoForwardChanged += new System.EventHandler(this.WebBrowserPropertyUpdated);
            this.webBrowser.CanGoBackChanged += new System.EventHandler(this.WebBrowserPropertyUpdated);
            this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
            this.webBrowser.DocumentTitleChanged += new System.EventHandler(this.WebBrowserDocumentTitleChanged);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.WebBrowserNewWindow);
            // 
            // Browser
            // 
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.toolStrip);
            this.Name = "Browser";
            this.Size = new System.Drawing.Size(620, 400);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for the webBrowser
        /// </summary>
        public WebBrowser WebBrowser
        {
            get { return this.webBrowser; }
        }

        /// <summary>
        /// Accessor for the addressComboBox
        /// </summary>
        public ToolStripComboBox AddressBox
        {
            get { return this.addressComboBox; }
        }

        /// <summary>
        /// Initializes localized texts to the controls
        /// </summary>
        private void InitializeLocalization()
        {
            this.goButton.Text = TextHelper.GetString("Label.Go");
            this.backButton.Text = TextHelper.GetString("Label.Back");
            this.forwardButton.Text = TextHelper.GetString("Label.Forward");
            this.refreshButton.Text = TextHelper.GetString("Label.Refresh");
        }

        /// <summary>
        /// Initializes the ui based on settings
        /// </summary>
        private void InitializeInterface()
        {
            this.toolStrip.Renderer = new DockPanelStripRenderer(true);
            this.addressComboBox.FlatStyle = Globals.Settings.ComboBoxFlatStyle;
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            if (ScaleHelper.GetScale() >= 1.5)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Browser));
                this.goButton.Image = ((System.Drawing.Image)(resources.GetObject("goButton.Image32")));
                this.forwardButton.Image = ((System.Drawing.Image)(resources.GetObject("forwardButton.Image32")));
                this.refreshButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshButton.Image32")));
                this.backButton.Image = ((System.Drawing.Image)(resources.GetObject("backButton.Image32")));
                this.Refresh();
            }
        }

        /// <summary>
        /// If the page tries to open a new window use a fd tab instead
        /// </summary>
        private void WebBrowserNewWindow(Object sender, CancelEventArgs e)
        {
            Globals.MainForm.CallCommand("Browse", this.webBrowser.StatusText);
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the web browser property changed event
        /// </summary>
        private void WebBrowserPropertyUpdated(Object sender, EventArgs e)
        {
            this.backButton.Enabled = this.webBrowser.CanGoBack;
            this.forwardButton.Enabled = this.webBrowser.CanGoForward;
        }

        /// <summary>
        /// Handles the web browser navigated event
        /// </summary>
        private void WebBrowserNavigated(Object sender, WebBrowserNavigatedEventArgs e)
        {
            this.addressComboBox.Text = this.webBrowser.Url.ToString();
            Globals.MainForm.OnScintillaControlUpdateControl(Globals.SciControl);
        }

        /// <summary>
        /// Handles the web browser title changed event
        /// </summary>
        private void WebBrowserDocumentTitleChanged(Object sender, EventArgs e)
        {
            if (this.webBrowser.DocumentTitle.Trim() == "")
            {
                String domain = this.webBrowser.Document.Domain.Trim();
                if (!String.IsNullOrEmpty(domain)) this.Parent.Text = domain;
                else this.Parent.Text = TextHelper.GetString("Info.UntitledFileStart");
            }
            else this.Parent.Text = this.webBrowser.DocumentTitle;
        }

        /// <summary>
        /// Handles the combo box index changed event
        /// </summary>
        private void AddressComboBoxSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.addressComboBox.SelectedItem != null)
            {
                String url = this.addressComboBox.SelectedItem.ToString();
                this.webBrowser.Navigate(url);
            }
        }

        /// <summary>
        /// Browses to the previous page in history
        /// </summary>
        private void BackButtonClick(Object sender, EventArgs e)
        {
            this.webBrowser.GoBack();
        }

        /// <summary>
        /// Browses to the next page in history
        /// </summary>
        private void ForwardButtonClick(Object sender, EventArgs e)
        {
            this.webBrowser.GoForward();
        }

        /// <summary>
        /// Reloads the current pages contents
        /// </summary>
        private void RefreshButtonClick(Object sender, EventArgs e)
        {
            this.webBrowser.Refresh();
        }

        /// <summary>
        /// Browses to the specified url on click
        /// </summary>
        private void BrowseButtonClick(Object sender, EventArgs e)
        {
            String url = this.addressComboBox.Text;
            if (!this.addressComboBox.Items.Contains(url))
            {
                this.addressComboBox.Items.Insert(0, url);
            }
            this.webBrowser.Navigate(url);
        }

        /// <summary>
        /// Handles the combo box key press event
        /// </summary>
        private void AddressComboBoxKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                String url = this.addressComboBox.Text;
                if (!this.addressComboBox.Items.Contains(url))
                {
                    this.addressComboBox.Items.Insert(0, url);
                }
                this.webBrowser.Navigate(url);
            }
        }

        #endregion

    }

    #region WebBrowserEx

    class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// Redirect events to MainForm.
        /// </summary>
        public override Boolean PreProcessMessage(ref Message msg)
        {
            return Globals.MainForm.PreProcessMessage(ref msg);
        }

    }

    #endregion

}
