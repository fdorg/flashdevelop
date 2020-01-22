// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Controls;

namespace FlashLogViewer
{
    public class PluginUI : DockPanelControl
    {
        private Form popupForm;
        private bool tracking;
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
        private ToolStripComboBoxEx logComboBox;
        private DateTime policyLogWrited;
        private DateTime flashLogWrited;
        private readonly PluginMain pluginMain;
        private string curLogFile;
        private Regex reWarning;
        private Regex reFilter;
        private Regex reError;
        private long lastPosition;
        private Image toggleButtonImagePlay;
        private Image toggleButtonImageStop;
        
        public PluginUI(PluginMain pluginMain)
        {
            AutoKeyHandling = true;
            Font = PluginBase.Settings.DefaultFont;
            this.pluginMain = pluginMain;
            InitializeSettings();
            InitializeComponent();
            InitializeContextMenu();
            InitializeGraphics();
            InitializeControls();
            UpdateMainRegexes();
            ScrollBarEx.Attach(logTextBox);
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            toolStrip = new ToolStripEx();
            toggleButton = new ToolStripButton();
            topMostButton = new ToolStripButton();
            clearFilterButton = new ToolStripButton();
            toolStripSeparator = new ToolStripSeparator();
            viewLabel = new ToolStripLabel();
            logComboBox = new ToolStripComboBoxEx();
            filterLabel = new ToolStripLabel();
            filterComboBox = new ToolStripSpringComboBox();
            logTextBox = new RichTextBoxEx();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.CanOverflow = false;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { toggleButton, topMostButton, toolStripSeparator, viewLabel, logComboBox, filterLabel, filterComboBox, clearFilterButton });
            toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip.Location = new Point(1, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(1, 1, 2, 2);
            toolStrip.Size = new Size(683, 30);
            toolStrip.TabIndex = 1;
            // 
            // toggleButton
            // 
            toggleButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toggleButton.ImageTransparentColor = Color.Magenta;
            toggleButton.Margin = new Padding(1);
            toggleButton.Name = "toggleButton";
            toggleButton.Size = new Size(23, 26);
            toggleButton.Click += ToggleButtonClick;
            // 
            // clearFilterButton
            //
            clearFilterButton.Enabled = true;
            clearFilterButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clearFilterButton.ImageTransparentColor = Color.Magenta;
            clearFilterButton.Margin = new Padding(0, 1, 0, 1);
            clearFilterButton.Name = "clearFilterButton";
            clearFilterButton.Size = new Size(23, 26);
            clearFilterButton.Alignment = ToolStripItemAlignment.Right;
            clearFilterButton.Click += ClearFilterButtonClick;
            // 
            // topMostButton
            // 
            topMostButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            topMostButton.ImageTransparentColor = Color.Magenta;
            topMostButton.Margin = new Padding(0, 1, 0, 1);
            topMostButton.Name = "topMostButton";
            topMostButton.Size = new Size(23, 26);
            topMostButton.Click += TopMostButtonClick;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(6, 28);
            // 
            // viewLabel
            // 
            viewLabel.Margin = new Padding(2, 1, 0, 1);
            viewLabel.Name = "viewLabel";
            viewLabel.Size = new Size(44, 25);
            viewLabel.Text = "View:";
            // 
            // logComboBox
            //
            logComboBox.Enabled = false;
            logComboBox.Items.AddRange(new object[] { TextHelper.GetString("Label.FlashLog"), TextHelper.GetString("Label.PolicyLog") });
            logComboBox.Name = "logComboBox";
            logComboBox.Size = new Size(120, 28);
            logComboBox.SelectedIndex = 0;
            logComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            logComboBox.FlatCombo.SelectedIndexChanged += LogComboBoxIndexChanged;
            // 
            // filterLabel
            // 
            filterLabel.Margin = new Padding(2, 1, 0, 1);
            filterLabel.Name = "filterLabel";
            filterLabel.Size = new Size(45, 25);
            filterLabel.Text = "Filter:";
            // 
            // filterComboBox
            //
            filterComboBox.Enabled = true;
            filterComboBox.Name = "filterComboBox";
            filterComboBox.Padding = new Padding(0, 0, 1, 0);
            filterComboBox.Size = new Size(50, 28);
            filterComboBox.TextChanged += FilterTextBoxTextChanged;
            // 
            // logTextBox
            // 
            logTextBox.BackColor = SystemColors.Window;
            logTextBox.BorderStyle = BorderStyle.None;
            logTextBox.DetectUrls = false;
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Font = new Font("Courier New", 8.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            logTextBox.Location = new Point(1, 30);
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.Size = new Size(683, 322);
            logTextBox.TabIndex = 0;
            logTextBox.Text = "";
            logTextBox.WordWrap = false;
            // 
            // PluginUI
            //
            Name = "PluginUI";
            Controls.Add(logTextBox);
            Controls.Add(toolStrip);
            Size = new Size(685, 352);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for the setting from the PluginMain
        /// </summary>
        public Settings Settings => (Settings)pluginMain.Settings;

        /// <summary>
        /// Ensures the update interval is valid and gets it
        /// </summary>
        public int GetUpdateInterval()
        {
            int interval = Settings.UpdateInterval;
            if (interval == 0) Settings.UpdateInterval = interval = 100;
            return interval;
        }

        /// <summary>
        /// Initializes the settings and checks the config files
        /// </summary>
        private void InitializeSettings()
        {
            string mmConfigFile = PathHelper.ResolveMMConfig();
            string userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string flashLogDir = Path.Combine(userAppDir, "Macromedia", "Flash Player", "Logs");
            try
            {
                if (!File.Exists(Settings.FlashLogFile))
                {
                    if (!Directory.Exists(flashLogDir)) Directory.CreateDirectory(flashLogDir);
                    if (string.IsNullOrEmpty(Settings.FlashLogFile) || !File.Exists(Settings.FlashLogFile))
                    {
                        Settings.FlashLogFile = Path.Combine(flashLogDir, "flashlog.txt");
                    }
                    File.WriteAllText(Settings.FlashLogFile, "", Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            try
            {
                if (!File.Exists(Settings.PolicyLogFile))
                {
                    if (!Directory.Exists(flashLogDir)) Directory.CreateDirectory(flashLogDir);
                    if (string.IsNullOrEmpty(Settings.PolicyLogFile) || !File.Exists(Settings.PolicyLogFile))
                    {
                        Settings.PolicyLogFile = Path.Combine(flashLogDir, "policyfiles.txt");
                    }
                    File.WriteAllText(Settings.PolicyLogFile, "", Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            try
            {
                if (!File.Exists(mmConfigFile))
                {
                    string contents = "PolicyFileLog=1\r\nPolicyFileLogAppend=0\r\nErrorReportingEnable=1\r\nTraceOutputFileEnable=1\r\n";
                    File.WriteAllText(mmConfigFile, contents, Encoding.UTF8);
                }
            }
            catch {} // No errors please...
            curLogFile = Settings.FlashLogFile;
        }

        /// <summary>
        /// Initializes the graphics after settings
        /// </summary>
        private void InitializeGraphics()
        {
            toggleButtonImageStop = PluginBase.MainForm.FindImage("151");
            toggleButtonImagePlay = PluginBase.MainForm.FindImage("147");
            clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            topMostButton.Image = PluginBase.MainForm.FindImage("56|8|2|4");
            toggleButton.Image = toggleButtonImagePlay;
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
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ClearLog"), null, ClearOutput));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CopyOutput"), null, CopyOutput));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.WrapText"), null, WrapText));
            clearFilterButton.ToolTipText = TextHelper.GetString("ToolTip.ClearFilterText");
            topMostButton.ToolTipText = TextHelper.GetString("ToolTip.PopupToTopMost");
            toggleButton.ToolTipText = TextHelper.GetString("ToolTip.StartTracking");
            filterLabel.Text = TextHelper.GetString("Label.Filter");
            viewLabel.Text = TextHelper.GetString("Label.View");
            Text = TextHelper.GetString("ToolTip.StartTracking");
            logTextBox.ContextMenuStrip = menu;
        }

        /// <summary>
        /// Initializes the controls after settings
        /// </summary>
        private void InitializeControls()
        {
            tracking = false;
            viewLabel.Font = PluginBase.Settings.DefaultFont;
            logTextBox.Font = PluginBase.Settings.ConsoleFont;
            filterLabel.Font = PluginBase.Settings.DefaultFont;
            logComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            logComboBox.Width = ScaleHelper.Scale(logComboBox.Width);
            filterComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            toolStrip.Renderer = new DockPanelStripRenderer();
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            refreshTimer = new Timer();
            refreshTimer.Interval = GetUpdateInterval();
            refreshTimer.Tick += RefreshTimerTick;
            refreshTimer.Start();
            refreshTimer.Enabled = false;
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButtonClick(object sender, EventArgs e)
        {
            lastPosition = 0;
            filterComboBox.Text = "";
            if (tracking) RefreshDisplay(true);
        }

        /// <summary>
        /// Clears the output control text
        /// </summary>
        private void ClearOutput(object sender, EventArgs e)
        {
            logTextBox.Clear();
            try
            {
                StreamWriter sw = new StreamWriter(curLogFile);
                sw.Write("");
                sw.Close();
            }
            catch {}
        }

        /// <summary>
        /// Wraps the output texts in the control
        /// </summary>
        private void WrapText(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            logTextBox.WordWrap = item.Checked;
        }

        /// <summary>
        /// Copies the output to clipboard
        /// </summary>
        private void CopyOutput(object sender, EventArgs e)
        {
            logTextBox.Copy();
        }

        /// <summary>
        /// Activates the plugin panel
        /// </summary> 
        public void DisplayOutput()
        {
            pluginMain.OpenPanel(null, null);
        }

        /// <summary>
        /// Refreshes the output window and filters it if needed
        /// </summary>
        public void RefreshDisplay(bool forceScroll)
        {
            using var reader = new StreamReader(File.Open(curLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8);
            if (reader.BaseStream.Length > lastPosition)
            {
                reader.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
            }
            RichTextBox log = logTextBox;
            bool colorize = Settings.ColourWarnings;
            Color currentColor = Color.White; // undefined
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!PassesFilter(line)) continue;
                Color newColor = PluginBase.MainForm.GetThemeColor("FlashLogViewer.DebugColor", Color.Black);
                if (colorize)
                {
                    if (reWarning.IsMatch(line)) newColor = PluginBase.MainForm.GetThemeColor("FlashLogViewer.WarningColor", Color.Orange);
                    else if (reError.IsMatch(line)) newColor = PluginBase.MainForm.GetThemeColor("FlashLogViewer.ErrorColor", Color.Red);
                }
                if (newColor != currentColor)
                {
                    log.Select(log.TextLength, 0);
                    log.SelectionColor = currentColor = newColor;
                }
                log.AppendText(line);
                log.AppendText("\n");
            }
            lastPosition = reader.BaseStream.Length;
            reader.Close();
            if (forceScroll)
            {
                try
                {
                    logTextBox.Select(logTextBox.TextLength, 0);
                    logTextBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
        }

        /// <summary>
        /// Enables or disables the tracking of the log file
        /// </summary>
        public void EnableTracking(bool enable)
        {
            tracking = enable;
            refreshTimer.Enabled = tracking;
            refreshTimer.Interval = GetUpdateInterval();
            toggleButton.Image = enable ? toggleButtonImageStop : toggleButtonImagePlay;
            toggleButton.ToolTipText = (enable ? TextHelper.GetString("ToolTip.StopTracking") : TextHelper.GetString("ToolTip.StartTracking"));
            logComboBox.Enabled = enable;
            if (enable)
            {
                lastPosition = 0;
                logTextBox.Clear();
                UpdateMainRegexes();
                RefreshDisplay(true);
            }
        }

        /// <summary>
        /// Makes the plugin panel top most form of FD
        /// </summary>
        private void TopMostButtonClick(object sender, EventArgs e)
        {
            topMostButton.Checked = !topMostButton.Checked;
            if (topMostButton.Checked)
            {
                ClosePluginPanel();
                popupForm = new Form();
                popupForm.Controls.Add(this);
                popupForm.MinimumSize = new Size(350, 120);
                popupForm.Text = TextHelper.GetString("Title.PluginPanel");
                popupForm.FormClosed += PopupFormClosed;
                popupForm.Icon = ImageKonverter.ImageToIcon(PluginBase.MainForm.FindImage("412", false));
                if (Settings.KeepPopupTopMost) popupForm.TopMost = true;
                popupForm.Show();
            }
            else popupForm.Close();
        }

        /// <summary>
        /// Closes the panel and start listening visible changed event
        /// </summary>
        private void ClosePluginPanel()
        {
            pluginMain.PluginPanel.Hide();
            pluginMain.PluginPanel.VisibleChanged += PluginPanelVisibleChanged;
        }

        /// <summary>
        /// If the user shows the plugin panel from view menu, restore
        /// </summary>
        private void PluginPanelVisibleChanged(object sender, EventArgs e)
        {
            pluginMain.PluginPanel.VisibleChanged -= PluginPanelVisibleChanged;
            popupForm.Close();
        }

        /// <summary>
        /// After the popup form has been closed, show plugin panel
        /// </summary>
        private void PopupFormClosed(object sender, FormClosedEventArgs e)
        {
            topMostButton.Checked = false;
            pluginMain.PluginPanel.Controls.Add(this);
            pluginMain.PluginPanel.Show();
        }

        /// <summary>
        /// Enables or disables the tracking of the log file
        /// </summary>
        private void ToggleButtonClick(object sender, EventArgs e)
        {
            tracking = !tracking;
            EnableTracking(tracking);
        }

        /// <summary>
        /// Polls for changes in the current log file
        /// </summary>
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            DateTime writeTime = DateTime.Now;
            try
            {
                writeTime = File.GetLastWriteTimeUtc(curLogFile);
            }
            catch {}
            if (curLogFile == Settings.FlashLogFile)
            {
                if (flashLogWrited != writeTime)
                {
                    flashLogWrited = writeTime;
                    if (tracking) RefreshDisplay(true);
                }
            }
            else if (curLogFile == Settings.PolicyLogFile)
            {
                if (policyLogWrited != writeTime)
                {
                    policyLogWrited = writeTime;
                    if (tracking) RefreshDisplay(true);
                }
            }
        }

        /// <summary>
        /// If log file has been changed, refreshes the display
        /// </summary>
        private void LogComboBoxIndexChanged(object sender, EventArgs e)
        {
            lastPosition = 0;
            logTextBox.Clear();
            if (logComboBox.SelectedIndex == 0) curLogFile = Settings.FlashLogFile;
            else curLogFile = Settings.PolicyLogFile;
            if (policyLogWrited != DateTime.MinValue)
            {
                RefreshDisplay(true);
            }
        }

        /// <summary>
        /// If the filter text has been changed, refreshes the display
        /// </summary>
        private void FilterTextBoxTextChanged(object sender, EventArgs e)
        {
            if (!tracking) return;
            if (filterComboBox.Text.Length == 0) reFilter = null;
            else
            {
                try
                {
                    reFilter = new Regex(filterComboBox.Text, RegexOptions.IgnoreCase);
                    filterComboBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor", SystemColors.WindowText);
                }
                catch {}
            }
            lastPosition = 0;
            logTextBox.Clear();
            RefreshDisplay(false);
        }

        /// <summary>
        /// Updates the regexes from setting and ensures their validity
        /// </summary>
        private void UpdateMainRegexes()
        {
            try
            {
                reError = new Regex(Settings.RegexError, RegexOptions.IgnoreCase);
            }
            catch 
            { 
                Settings.RegexError = "Error: ";
                reError = new Regex(Settings.RegexError, RegexOptions.IgnoreCase);
            }
            try
            {
                reWarning = new Regex(Settings.RegexWarning, RegexOptions.IgnoreCase);
            }
            catch 
            { 
                Settings.RegexError = "Warning: ";
                reWarning = new Regex(Settings.RegexWarning, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Filters the contents of the output
        /// </summary>
        private bool PassesFilter(string logLine)
        {
            if (reFilter is null) return true;
            return reFilter.IsMatch(logLine);
        }

        #endregion

    }

}
