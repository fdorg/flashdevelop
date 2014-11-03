using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore.Localization;
using System.Text.RegularExpressions;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashLogViewer
{
	public class PluginUI : DockPanelControl
    {
        private Form popupForm;
        private Boolean tracking;
        private Timer refreshTimer;
        private ToolStrip toolStrip;
        private RichTextBox logTextBox;
        private ToolStripLabel viewLabel;
        private ToolStripLabel filterLabel;
        private ToolStripButton toggleButton;
        private ToolStripButton topMostButton;
        private ToolStripButton clearFilterButton;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripSpringComboBox filterComboBox;
        private ToolStripComboBox logComboBox;
        private DateTime policyLogWrited;
        private DateTime flashLogWrited;
        private PluginMain pluginMain;
        private ImageList imageList;
        private String curLogFile;
        private Regex reWarning;
        private Regex reFilter;
        private Regex reError;
        private long lastPosition;
        
		public PluginUI(PluginMain pluginMain)
		{
            this.Font = PluginBase.Settings.DefaultFont;
            this.pluginMain = pluginMain;
            this.InitializeSettings();
			this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.InitializeControls();
            this.UpdateMainRegexes();
		}

		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.toggleButton = new System.Windows.Forms.ToolStripButton();
            this.topMostButton = new System.Windows.Forms.ToolStripButton();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.viewLabel = new System.Windows.Forms.ToolStripLabel();
            this.logComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.filterLabel = new System.Windows.Forms.ToolStripLabel();
            this.filterComboBox = new System.Windows.Forms.ToolStripSpringComboBox();
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toggleButton, this.topMostButton, this.toolStripSeparator, this.viewLabel, this.logComboBox, this.filterLabel, this.filterComboBox, this.clearFilterButton });
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(683, 30);
            this.toolStrip.TabIndex = 1;
            // 
            // toggleButton
            // 
            this.toggleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleButton.Margin = new System.Windows.Forms.Padding(1);
            this.toggleButton.Name = "toggleButton";
            this.toggleButton.Size = new System.Drawing.Size(23, 26);
            this.toggleButton.Click += new System.EventHandler(this.ToggleButtonClick);
            // 
            // clearFilterButton
            //
            this.clearFilterButton.Enabled = true;
            this.clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(23, 26);
            this.clearFilterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButtonClick);
            // 
            // topMostButton
            // 
            this.topMostButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.topMostButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.topMostButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.topMostButton.Name = "topMostButton";
            this.topMostButton.Size = new System.Drawing.Size(23, 26);
            this.topMostButton.Click += new System.EventHandler(this.TopMostButtonClick);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 28);
            // 
            // viewLabel
            // 
            this.viewLabel.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.viewLabel.Name = "viewLabel";
            this.viewLabel.Size = new System.Drawing.Size(44, 25);
            this.viewLabel.Text = "View:";
            // 
            // logComboBox
            //
            this.logComboBox.Enabled = false;
            this.logComboBox.Items.AddRange(new Object[] { TextHelper.GetString("Label.FlashLog"), TextHelper.GetString("Label.PolicyLog") });
            this.logComboBox.Name = "logComboBox";
            this.logComboBox.Size = new System.Drawing.Size(90, 28);
            this.logComboBox.SelectedIndex = 0;
            this.logComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.logComboBox.SelectedIndexChanged += new System.EventHandler(this.LogComboBoxIndexChanged);
            // 
            // filterLabel
            // 
            this.filterLabel.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(45, 25);
            this.filterLabel.Text = "Filter:";
            // 
            // filterComboBox
            //
            this.filterComboBox.Enabled = true;
            this.filterComboBox.Name = "filterComboBox";
            this.filterComboBox.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.filterComboBox.Size = new System.Drawing.Size(50, 28);
            this.filterComboBox.TextChanged += new System.EventHandler(this.FilterTextBoxTextChanged);
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logTextBox.DetectUrls = false;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.Location = new System.Drawing.Point(1, 30);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(683, 322);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.Text = "";
            this.logTextBox.WordWrap = false;
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.toolStrip);
            this.Size = new System.Drawing.Size(685, 352);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for the setting from the PluginMain
        /// </summary>
        public Settings Settings
        {
            get { return (Settings)this.pluginMain.Settings; }
        }

        /// <summary>
        /// Ensures the update interval is valid and gets it
        /// </summary>
        public Int32 GetUpdateInterval()
        {
            Int32 interval = this.Settings.UpdateInterval;
            if (interval == 0) this.Settings.UpdateInterval = interval = 100;
            return interval;
        }

        /// <summary>
        /// Initializes the settings and checks the config files
        /// </summary>
        private void InitializeSettings()
        {
            String mmConfigFile = PathHelper.ResolveMMConfig();
            String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String macromediaDir = Path.Combine(userAppDir, "Macromedia");
            String flashPlayerDir = Path.Combine(macromediaDir, "Flash Player");
            String flashLogDir = Path.Combine(flashPlayerDir, "Logs");
            try
            {
                if (!File.Exists(this.Settings.FlashLogFile))
                {
                    if (!Directory.Exists(flashLogDir)) Directory.CreateDirectory(flashLogDir);
                    if (string.IsNullOrEmpty(this.Settings.FlashLogFile) || !File.Exists(this.Settings.FlashLogFile))
                    {
                        this.Settings.FlashLogFile = Path.Combine(flashLogDir, "flashlog.txt");
                    }
                    File.WriteAllText(this.Settings.FlashLogFile, "", Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            try
            {
                if (!File.Exists(this.Settings.PolicyLogFile))
                {
                    if (!Directory.Exists(flashLogDir)) Directory.CreateDirectory(flashLogDir);
                    if (string.IsNullOrEmpty(this.Settings.PolicyLogFile) || !File.Exists(this.Settings.PolicyLogFile))
                    {
                        this.Settings.PolicyLogFile = Path.Combine(flashLogDir, "policyfiles.txt");
                    }
                    File.WriteAllText(this.Settings.PolicyLogFile, "", Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            try
            {
                if (!File.Exists(mmConfigFile))
                {
                    String contents = "PolicyFileLog=1\r\nPolicyFileLogAppend=0\r\nErrorReportingEnable=1\r\nTraceOutputFileEnable=1\r\n";
                    File.WriteAllText(mmConfigFile, contents, Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            this.curLogFile = this.Settings.FlashLogFile;
        }

        /// <summary>
        /// Initializes the graphics after settings
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList = new ImageList();
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.TransparentColor = Color.Transparent;
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.Images.Add(PluginBase.MainForm.FindImage("151"));
            this.imageList.Images.Add(PluginBase.MainForm.FindImage("147"));
            this.imageList.Images.Add(PluginBase.MainForm.FindImage("56|8|2|4"));
            this.imageList.Images.Add(PluginBase.MainForm.FindImage("153"));
            this.clearFilterButton.Image = this.imageList.Images[3];
            this.topMostButton.Image = this.imageList.Images[2];
            this.toggleButton.Image = this.imageList.Images[1];
        }

        /// <summary>
        /// Initializes the context menu after settings
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ClearLog"), null, new EventHandler(this.ClearOutput)));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CopyOutput"), null, new EventHandler(this.CopyOutput)));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.WrapText"), null, new EventHandler(this.WrapText)));
            this.clearFilterButton.ToolTipText = TextHelper.GetString("ToolTip.ClearFilterText");
            this.topMostButton.ToolTipText = TextHelper.GetString("ToolTip.PopupToTopMost");
            this.toggleButton.ToolTipText = TextHelper.GetString("ToolTip.StartTracking");
            this.filterLabel.Text = TextHelper.GetString("Label.Filter");
            this.viewLabel.Text = TextHelper.GetString("Label.View");
            this.Text = TextHelper.GetString("ToolTip.StartTracking");
            this.logTextBox.ContextMenuStrip = menu;
        }

        /// <summary>
        /// Initializes the controls after settings
        /// </summary>
        private void InitializeControls()
        {
            this.tracking = false;
            this.viewLabel.Font = PluginBase.Settings.DefaultFont;
            this.logTextBox.Font = PluginBase.Settings.ConsoleFont;
            this.filterLabel.Font = PluginBase.Settings.DefaultFont;
            this.logComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.logComboBox.Width = ScaleHelper.Scale(this.logComboBox.Width);
            this.filterComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.toolStrip.Renderer = new DockPanelStripRenderer();
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.refreshTimer = new Timer();
            this.refreshTimer.Interval = this.GetUpdateInterval();
            this.refreshTimer.Tick += new EventHandler(this.RefreshTimerTick);
            this.refreshTimer.Start();
            this.refreshTimer.Enabled = false;
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButtonClick(Object sender, System.EventArgs e)
        {
            this.lastPosition = 0;
            this.filterComboBox.Text = "";
            if (this.tracking) this.RefreshDisplay(true);
        }

        /// <summary>
        /// Clears the output control text
        /// </summary>
        private void ClearOutput(Object sender, System.EventArgs e)
        {
            this.logTextBox.Clear();
            try
            {
                StreamWriter sw = new StreamWriter(this.curLogFile);
                sw.Write("");
                sw.Close();
            }
            catch {}
        }

        /// <summary>
        /// Wraps the output texts in the control
        /// </summary>
        private void WrapText(Object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            this.logTextBox.WordWrap = item.Checked;
        }

        /// <summary>
        /// Copies the output to clipboard
        /// </summary>
        private void CopyOutput(Object sender, System.EventArgs e)
        {
            this.logTextBox.Copy();
        }

        /// <summary>
        /// Activates the plugin panel
        /// </summary> 
        public void DisplayOutput()
        {
            this.pluginMain.OpenPanel(null, null);
        }

        /// <summary>
        /// Refreshes the output window and filters it if needed
        /// </summary>
        public void RefreshDisplay(Boolean forceScroll)
        {
            using (StreamReader s = new StreamReader(File.Open(this.curLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8))
            {
                if (s.BaseStream.Length > lastPosition)
                {
                    s.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                }
                RichTextBox log = this.logTextBox;
                bool colorize = this.Settings.ColourWarnings;
                Color currentColor = Color.White; // undefined
                while (!s.EndOfStream)
                {
                    string line = s.ReadLine();
                    if (!this.PassesFilter(line)) continue;
                    Color newColor = Color.Black;
                    if (colorize)
                    {
                        if (reWarning.IsMatch(line)) newColor = Color.Orange;
                        else if (reError.IsMatch(line)) newColor = Color.Red;
                    }
                    if (newColor != currentColor)
                    {
                        log.Select(log.TextLength, 0);
                        log.SelectionColor = currentColor = newColor;
                    }
                    log.AppendText(line);
                    log.AppendText("\n");
                }
                lastPosition = s.BaseStream.Length;
                s.Close();
            }
            if (forceScroll) this.logTextBox.ScrollToCaret();
        }

        /// <summary>
        /// Enables or disables the tracking of the log file
        /// </summary>
        public void EnableTracking(Boolean enable)
        {
            this.tracking = enable;
            this.refreshTimer.Enabled = this.tracking;
            this.refreshTimer.Interval = this.GetUpdateInterval();
            this.toggleButton.Image = this.imageList.Images[(enable ? 0 : 1)];
            this.toggleButton.ToolTipText = (enable ? TextHelper.GetString("ToolTip.StopTracking") : TextHelper.GetString("ToolTip.StartTracking"));
            this.logComboBox.Enabled = enable;
            if (enable)
            {
                this.lastPosition = 0;
                this.logTextBox.Clear();
                this.UpdateMainRegexes();
                this.RefreshDisplay(true);
            }
        }

        /// <summary>
        /// Makes the plugin panel top most form of FD
        /// </summary>
        private void TopMostButtonClick(Object sender, EventArgs e)
        {
            this.topMostButton.Checked = !this.topMostButton.Checked;
            if (this.topMostButton.Checked)
            {
                this.ClosePluginPanel();
                this.popupForm = new Form();
                this.popupForm.Controls.Add(this);
                this.popupForm.MinimumSize = new Size(350, 120);
                this.popupForm.Text = TextHelper.GetString("Title.PluginPanel");
                this.popupForm.FormClosed += new FormClosedEventHandler(this.PopupFormClosed);
                this.popupForm.Icon = ImageKonverter.ImageToIcon(PluginBase.MainForm.FindImage("412"));
                if (this.Settings.KeepPopupTopMost) this.popupForm.TopMost = true;
                this.popupForm.Show();
            }
            else this.popupForm.Close();
        }

        /// <summary>
        /// Closes the panel and start listening visible changed event
        /// </summary>
        private void ClosePluginPanel()
        {
            this.pluginMain.PluginPanel.Hide();
            this.pluginMain.PluginPanel.VisibleChanged += new EventHandler(this.PluginPanelVisibleChanged);
        }

        /// <summary>
        /// If the user shows the plugin panel from view menu, restore
        /// </summary>
        private void PluginPanelVisibleChanged(Object sender, EventArgs e)
        {
            this.pluginMain.PluginPanel.VisibleChanged -= new EventHandler(this.PluginPanelVisibleChanged);
            this.popupForm.Close();
        }

        /// <summary>
        /// After the popup form has been closed, show plugin panel
        /// </summary>
        private void PopupFormClosed(Object sender, FormClosedEventArgs e)
        {
            this.topMostButton.Checked = false;
            this.pluginMain.PluginPanel.Controls.Add(this);
            this.pluginMain.PluginPanel.Show();
        }

        /// <summary>
        /// Enables or disables the tracking of the log file
        /// </summary>
        private void ToggleButtonClick(Object sender, EventArgs e)
        {
            this.tracking = !this.tracking;
            this.EnableTracking(this.tracking);
        }

        /// <summary>
        /// Polls for changes in the current log file
        /// </summary>
        private void RefreshTimerTick(Object sender, EventArgs e)
        {
            DateTime writeTime = DateTime.Now;
            try
            {
                writeTime = File.GetLastWriteTimeUtc(this.curLogFile);
            }
            catch {}
            if (this.curLogFile == this.Settings.FlashLogFile)
            {
                if (this.flashLogWrited != writeTime)
                {
                    this.flashLogWrited = writeTime;
                    if (this.tracking) this.RefreshDisplay(true);
                }
            }
            else if (this.curLogFile == this.Settings.PolicyLogFile)
            {
                if (this.policyLogWrited != writeTime)
                {
                    this.policyLogWrited = writeTime;
                    if (this.tracking) this.RefreshDisplay(true);
                }
            }
        }

        /// <summary>
        /// If log file has been changed, refreshes the display
        /// </summary>
        private void LogComboBoxIndexChanged(Object sender, EventArgs e)
        {
            this.lastPosition = 0;
            this.logTextBox.Clear();
            if (this.logComboBox.SelectedIndex == 0) this.curLogFile = this.Settings.FlashLogFile;
            else this.curLogFile = this.Settings.PolicyLogFile;
            if (this.policyLogWrited != DateTime.MinValue)
            {
                this.RefreshDisplay(true);
            }
        }

        /// <summary>
        /// If the filter text has been changed, refreshes the display
        /// </summary>
        private void FilterTextBoxTextChanged(Object sender, EventArgs e)
        {
            if (!this.tracking) return;
            if (this.filterComboBox.Text.Length == 0) this.reFilter = null;
            else
            {
                try
                {
                    this.reFilter = new Regex(filterComboBox.Text, RegexOptions.IgnoreCase);
                    this.filterComboBox.ForeColor = SystemColors.ControlText;
                }
                catch { this.filterComboBox.ForeColor = Color.Red; }
            }
            this.lastPosition = 0;
            this.logTextBox.Clear();
            this.RefreshDisplay(false);
        }

        /// <summary>
        /// Updates the regexes from setting and ensures their validity
        /// </summary>
        private void UpdateMainRegexes()
        {
            try
            {
                this.reError = new Regex(this.Settings.RegexError, RegexOptions.IgnoreCase);
            }
            catch 
            { 
                this.Settings.RegexError = "Error #";
                this.reError = new Regex(this.Settings.RegexError, RegexOptions.IgnoreCase);
            }
            try
            {
                this.reWarning = new Regex(this.Settings.RegexWarning, RegexOptions.IgnoreCase);
            }
            catch 
            { 
                this.Settings.RegexError = "Warning: ";
                this.reWarning = new Regex(this.Settings.RegexWarning, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Filters the contents of the output
        /// </summary>
        private Boolean PassesFilter(String logLine)
        {
            if (reFilter == null) return true;
            else return reFilter.IsMatch(logLine);
        }

        #endregion

    }

}
