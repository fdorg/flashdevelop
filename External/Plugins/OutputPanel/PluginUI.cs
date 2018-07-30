using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Controls;

namespace OutputPanel
{
    public class PluginUI : DockPanelControl
    {
        private Int32 logCount;
        private RichTextBoxEx textLog;
        private PluginMain pluginMain;
        private String searchInvitation;
        private System.Timers.Timer scrollTimer;
        private ToolStripMenuItem wrapTextItem;
        private ToolStripSpringTextBox findTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toggleButton;
        private ToolStripButton clearButton;
        private ToolStrip toolStrip;
        private Timer typingTimer;
        private Boolean scrolling;
        private Timer autoShow;
        private Boolean muted;
        private Image toggleButtonImagePause, toggleButtonImagePlay, toggleButtonImagePlayNew;

        public PluginUI(PluginMain pluginMain)
        {
            this.InitializeTimers();
            this.scrolling = false;
            this.pluginMain = pluginMain;
            this.logCount = TraceManager.TraceLog.Count;
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeLayout();
            this.toggleButtonImagePause = PluginBase.MainForm.FindImage("146");
            this.toggleButtonImagePlay = PluginBase.MainForm.FindImage("147");
            this.toggleButtonImagePlayNew = PluginBase.MainForm.FindImage("147|17|5|4");
            this.ToggleButtonClick(this, new EventArgs());
            ScrollBarEx.Attach(textLog);
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.scrollTimer = new System.Timers.Timer();
            this.textLog = new System.Windows.Forms.RichTextBoxEx();
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.toggleButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.findTextBox = new System.Windows.Forms.ToolStripSpringTextBox();
            this.clearButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.scrollTimer)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollTimer
            // 
            this.scrollTimer.Interval = 50;
            this.scrollTimer.SynchronizingObject = this;
            this.scrollTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.ScrollTimerElapsed);
            // 
            // textLog
            // 
            this.textLog.BackColor = System.Drawing.SystemColors.Window;
            this.textLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textLog.Location = new System.Drawing.Point(1, 26);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(278, 326);
            this.textLog.TabIndex = 1;
            this.textLog.Text = "";
            this.textLog.WordWrap = false;
            this.textLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PluginUIKeyDown);
            this.textLog.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextLogMouseUp);
            this.textLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.LinkClicked);
            this.textLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextLogMouseDown);
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleButton,
            this.toolStripSeparator1,
            this.findTextBox,
            this.clearButton});
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(278, 26);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 1;
            // 
            // toggleButton
            // 
            this.toggleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleButton.Name = "toggleButton";
            this.toggleButton.Size = new System.Drawing.Size(23, 20);
            this.toggleButton.Text = "toolStripButton1";
            this.toggleButton.Click += new System.EventHandler(this.ToggleButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // findTextBox
            //
            this.findTextBox.Name = "FindTextBox";
            this.findTextBox.Size = new System.Drawing.Size(190, 23);
            this.findTextBox.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.findTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.findTextBox.TextChanged += new System.EventHandler(this.FindTextBoxTextChanged);
            this.findTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PluginUIKeyDown);
            this.findTextBox.Leave += new System.EventHandler(this.FindTextBoxLeave);
            this.findTextBox.Enter += new System.EventHandler(this.FindTextBoxEnter);
            // 
            // clearButton
            //
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(23, 21);
            this.clearButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearButton.Click += new System.EventHandler(this.ClearButtonClick);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.toolStrip);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            ((System.ComponentModel.ISupportInitialize)(this.scrollTimer)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods and Event Handlers

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        private void InitializeLayout()
        {
            this.toolStrip.Renderer = new DockPanelStripRenderer();
        }

        /// <summary>
        /// Initializes the timers used in the control.
        /// </summary>
        private void InitializeTimers()
        {
            this.autoShow = new Timer();
            this.autoShow.Interval = 300;
            this.autoShow.Tick += new EventHandler(this.AutoShowPanel);
            this.typingTimer = new Timer();
            this.typingTimer.Tick += new EventHandler(this.TypingTimerTick);
            this.typingTimer.Interval = 250;
        }

        /// <summary>
        /// Initializes the context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer();
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ClearOutput"), null, new EventHandler(this.ClearOutput)));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CopyOutput"), null, new EventHandler(this.CopyOutput)));
            menu.Items.Add(new ToolStripSeparator());
            wrapTextItem = new ToolStripMenuItem(TextHelper.GetString("Label.WrapText"), null, new EventHandler(this.WrapText));
            menu.Items.Add(wrapTextItem);
            this.searchInvitation = TextHelper.GetString("Label.SearchInvitation");
            this.clearButton.ToolTipText = TextHelper.GetString("Label.ClearSearchText");
            this.clearButton.Image = PluginBase.MainForm.FindImage("153");
            this.textLog.Font = PluginBase.Settings.ConsoleFont;
            this.findTextBox.Text = this.searchInvitation;
            this.textLog.ContextMenuStrip = menu;
            this.ApplyWrapText();
        }

        /// <summary>
        /// Opens the clicked link
        /// </summary>
        private void LinkClicked(Object sender, LinkClickedEventArgs e)
        {
            PluginBase.MainForm.CallCommand("Browse", e.LinkText);
        }

        /// <summary>
        /// Handle the internal key down event
        /// </summary>
        private void PluginUIKeyDown(Object sender, KeyEventArgs e)
        {
            this.OnShortcut(e.KeyData);
        }

        /// <summary>
        /// Changes the wrapping in the control
        /// </summary>
        private void WrapText(Object sender, EventArgs e)
        {
            this.pluginMain.PluginSettings.WrapOutput = !this.pluginMain.PluginSettings.WrapOutput;
            this.pluginMain.SaveSettings();
            this.ApplyWrapText();
        }

        /// <summary>
        /// Applies the wrapping in the control
        /// </summary>
        public void ApplyWrapText()
        {
            if (this.pluginMain.PluginPanel == null) return;
            if (this.pluginMain.PluginPanel.InvokeRequired)
            {
                this.pluginMain.PluginPanel.BeginInvoke((MethodInvoker)this.ApplyWrapText);
                return;
            }
            this.wrapTextItem.Checked = this.pluginMain.PluginSettings.WrapOutput;
            this.textLog.WordWrap = this.pluginMain.PluginSettings.WrapOutput;
        }

        /// <summary>
        /// Copies the text to clipboard
        /// </summary>
        private void CopyOutput(Object sender, EventArgs e)
        {
            if (this.textLog.SelectedText.Length > 0) this.textLog.Copy();
            else if (!String.IsNullOrEmpty(this.textLog.Text))
            {
                Clipboard.SetText(this.textLog.Text);
                PluginBase.MainForm.RefreshUI();
            }
        }

        /// <summary>
        /// Update colors on start after theme engine
        /// </summary>
        public void UpdateAfterTheme()
        {
            this.findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
        }

        /// <summary>
        /// Clears the output
        /// </summary>
        public void ClearOutput(Object sender, EventArgs e)
        {
            this.textLog.Clear();
        }

        /// <summary>
        /// Flashes the panel to the user
        /// </summary>
        public void DisplayOutput()
        {
            this.autoShow.Stop();
            this.autoShow.Start();
        }

        /// <summary>
        /// Shows the panel
        /// </summary>
        private void AutoShowPanel(Object sender, EventArgs e)
        {
            this.autoShow.Stop();
            if (this.textLog.TextLength > 0)
            {
                DockContent panel = this.Parent as DockContent;
                DockState ds = panel.VisibleState;
                if (!panel.Visible || ds.ToString().EndsWithOrdinal("AutoHide"))
                {
                    panel.Show();
                    if (ds.ToString().EndsWithOrdinal("AutoHide")) panel.Activate();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // We use custom resizing because when the owner DockPanel hides, textLog.Height = 0, and ScrollToCaret() fails
            if (this.Height != 0)
            {
                var bounds = Rectangle.FromLTRB(Padding.Left, toolStrip.Bottom, ClientSize.Width - Padding.Right, ClientSize.Height - Padding.Bottom);
                textLog.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                
                // Recreate handle and restore scrollbars position, built-in behavior is flawed (eg.: go to bottom of scroll and resize)
                if (Win32.ShouldUseWin32())
                {
                    int vPos = Win32.GetScrollPos(textLog.Handle, Win32.SB_VERT);
                    int hPos = Win32.GetScrollPos(textLog.Handle, Win32.SB_HORZ);
                    textLog.Recreate();
                    int wParam = Win32.SB_THUMBPOSITION | vPos << 16;
                    Win32.SendMessage(textLog.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
                    wParam = Win32.SB_THUMBPOSITION | hPos << 16;
                    Win32.SendMessage(textLog.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        /// Handles the shortcut
        /// </summary>
        public Boolean OnShortcut(Keys keys)
        {
            if (ContainsFocus)
            {
                if (keys == Keys.F3)
                {
                    this.FindNextMatch(true);
                    return true;
                }
                else if (keys == (Keys.Shift | Keys.F3))
                {
                    this.FindNextMatch(false);
                    return true;
                }
                else if (keys == Keys.Escape)
                {
                    ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                    if (doc != null && doc.IsEditable) doc.SciControl.Focus();
                }
                else if (keys == (Keys.Control | Keys.F))
                {
                    findTextBox.Focus();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds entries to the output if new entries are found
        /// </summary>
        public void AddTraces()
        {
            if (this.muted) return;
            if (!this.scrolling)
            {
                this.toggleButton.Image = this.toggleButtonImagePlayNew;
                return;
            }
            IList<TraceItem> log = TraceManager.TraceLog;
            Int32 newCount = log.Count;
            if (newCount <= this.logCount)
            {
                this.logCount = newCount;
                return;
            }
            Int32 state;
            String message;
            TraceItem entry;
            Color newColor = Color.Red;
            Color currentColor = Color.Red;
            int oldSelectionStart = this.textLog.SelectionStart;
            int oldSelectionLength = this.textLog.SelectionLength;
            List<HighlightMarker> markers = this.pluginMain.PluginSettings.HighlightMarkers;
            Boolean fastMode = (newCount - this.logCount) > 1000;
            StringBuilder newText = new StringBuilder();
            for (Int32 i = this.logCount; i < newCount; i++)
            {
                entry = log[i];
                state = entry.State;
                if (entry.Message == null) message = "";
                else message = entry.Message;
                if (!fastMode)
                {
                    // Automatic state from message, legacy format, ie. "2:message" -> state = 2
                    if (this.pluginMain.PluginSettings.UseLegacyColoring && state == 1 && message.Length > 2 && message[1] == ':' && Char.IsDigit(message[0]))
                    {
                        if (int.TryParse(message[0].ToString(), out state))
                        {
                            message = message.Substring(2);
                        }
                    }
                    // Automatic state from message: New format with customizable markers
                    if (state == 1 && markers != null && markers.Count > 0)
                    {
                        foreach (HighlightMarker marker in markers)
                        {
                            if (message.Contains(marker.Marker))
                            {
                                state = (int)marker.Level;
                                break;
                            }
                        }
                    }
                    switch (state)
                    {
                        case 0: // Info
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.InfoColor", Color.Gray);
                            break;
                        case 1: // Debug
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.DebugColor", this.ForeColor);
                            break;
                        case 2: // Warning
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.WarningColor", Color.Orange);
                            break;
                        case 3: // Error
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.ErrorColor", Color.Red);
                            break;
                        case 4: // Fatal
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.FatalColor", Color.Magenta);
                            break;
                        case -1: // ProcessStart
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.ProcessStartColor", Color.Blue);
                            break;
                        case -2: // ProcessEnd
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.ProcessEndColor", Color.Blue);
                            break;
                        case -3: // ProcessError
                            if (message.Contains("Warning")) newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.WarningColor", Color.Orange);
                            else newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.ErrorColor", Color.Red);
                            break;
                    }
                    if (newColor != currentColor)
                    {
                        if (newText.Length > 0)
                        {
                            this.textLog.Select(this.textLog.TextLength, 0);
                            this.textLog.SelectionColor = currentColor;
                            this.textLog.AppendText(newText.ToString());
                            newText.Remove(0, newText.Length);
                        }
                        currentColor = newColor;
                    }
                }
                newText.Append(message + "\n");
            }
            if (newText.Length > 0)
            {
                this.ClearCurrentSelection();
                this.textLog.Select(this.textLog.TextLength, 0);
                this.textLog.SelectionColor = currentColor;
                this.textLog.AppendText(newText.ToString());
            }
            if (oldSelectionLength != 0) this.textLog.Select(oldSelectionStart, oldSelectionLength);
            else if (scrolling) this.textLog.Select(this.textLog.TextLength, 0);
            this.logCount = newCount;
            this.scrollTimer.Enabled = true;
            this.TypingTimerTick(null, null);
        }

        /// <summary>
        /// Scrolling fix on RichTextBox
        /// </summary> 
        private void ScrollTimerElapsed(Object sender, System.Timers.ElapsedEventArgs e)
        {
            this.scrollTimer.Enabled = false;
            if (this.pluginMain.PluginSettings.ShowOnProcessEnd)
            {
                this.DisplayOutput();
            }

            try
            {
                this.textLog.Select(this.textLog.TextLength, 0);
                this.textLog.ScrollToCaret();
            }
            catch { /* WineMod: not supported */ }
        }

        /// <summary>
        /// Filters the output by search text
        /// </summary>
        private void FilterOutput(String findText)
        {
            this.textLog.Select(0, this.textLog.TextLength);
            this.textLog.SelectionBackColor = this.textLog.BackColor;
            if (findText.Trim() != "")
            {
                findText = Regex.Escape(findText);
                MatchCollection results = Regex.Matches(this.textLog.Text, findText, RegexOptions.IgnoreCase);
                for (Int32 i = 0; i < results.Count; i++)
                {
                    Match match = results[i];
                    this.textLog.SelectionStart = match.Index;
                    this.textLog.SelectionLength = match.Length;
                    this.textLog.SelectionBackColor = PluginBase.MainForm.GetThemeColor("OutputPanel.HighlightColor", SystemColors.Highlight);
                }
            }
        }

        /// <summary>
        /// Handles the text change event
        /// </summary>
        private void FindTextBoxTextChanged(Object sender, EventArgs e)
        {
            if (this.textLog.TextLength > 10000)
            {
                this.typingTimer.Stop();
                this.typingTimer.Start();
            }
            else this.TypingTimerTick(null, null);
        }

        /// <summary>
        /// When the typing timer ticks update the search
        /// </summary>
        private void TypingTimerTick(Object sender, EventArgs e)
        {
            this.typingTimer.Stop();
            String searchText = this.findTextBox.Text;
            if (searchText == searchInvitation) searchText = "";
            if (searchText.Trim() != "") this.FilterOutput(searchText);
            else this.ClearCurrentSelection();
            this.clearButton.Enabled = searchText.Length > 0;
        }

        /// <summary>
        /// When user enters control, handle it
        /// </summary>
        private void FindTextBoxEnter(Object sender, EventArgs e)
        {
            if (this.findTextBox.Text == searchInvitation)
            {
                this.findTextBox.Text = "";
                this.findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.ForeColor", SystemColors.WindowText);
            }
        }

        /// <summary>
        /// When user leaves control, handle it
        /// </summary>
        private void FindTextBoxLeave(Object sender, EventArgs e)
        {
            if (this.findTextBox.Text == "")
            {
                this.clearButton.Enabled = false;
                this.findTextBox.Text = searchInvitation;
                this.findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
            }
        }

        /// <summary>
        /// Clears the search text from the control
        /// </summary>
        private void ClearButtonClick(Object sender, EventArgs e)
        {
            this.findTextBox.Text = "";
            this.ClearCurrentSelection();
            this.findTextBox.Focus();
        }

        /// <summary>
        /// Finds the next match and selects it
        /// </summary>
        private void FindNextMatch(Boolean forward)
        {
            try
            {
                String searchText = this.findTextBox.Text;
                if (searchText == searchInvitation) searchText = "";
                if (searchText.Trim() != "")
                {
                    Int32 curPos = this.textLog.SelectionStart + this.textLog.SelectionLength;
                    MatchCollection results = Regex.Matches(this.textLog.Text, searchText, RegexOptions.IgnoreCase);
                    Match nearestMatch = results[0];
                    for (Int32 i = 0; i < results.Count; i++)
                    {
                        if (forward)
                        {
                            if (curPos > results[results.Count - 1].Index)
                            {
                                nearestMatch = results[0];
                                break;
                            }
                            if (results[i].Index >= curPos)
                            {
                                nearestMatch = results[i];
                                break;
                            }
                        }
                        else
                        {
                            if (this.textLog.SelectedText.Length > 0 && curPos <= results[0].Index + results[0].Length)
                            {
                                nearestMatch = results[results.Count - 1];
                                break;
                            }
                            if (curPos < results[0].Index + results[0].Length)
                            {
                                nearestMatch = results[results.Count - 1];
                                break;
                            }
                            if (this.textLog.SelectedText.Length == 0 && curPos == results[i].Index + results[i].Length)
                            {
                                nearestMatch = results[i];
                                break;
                            }
                            if (results[i].Index > nearestMatch.Index && results[i].Index + results[i].Length < curPos)
                            {
                                nearestMatch = results[i];
                            }
                        }
                    }
                    this.textLog.Focus();
                    this.textLog.Select(nearestMatch.Index, nearestMatch.Length);
                    try { this.textLog.ScrollToCaret(); }
                    catch { /* WineMod: not supported */ }
                }
            }
            catch { }
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        private void ClearCurrentSelection()
        {
            int oldSelectionStart = this.textLog.SelectionStart;
            int oldSelectionLength = this.textLog.SelectionLength;
            this.textLog.Select(0, this.textLog.TextLength);
            this.textLog.SelectionBackColor = this.textLog.BackColor;
            this.textLog.Select(oldSelectionStart, oldSelectionLength);
        }

        /// <summary>
        /// Toggle the scrolling enabled
        /// </summary>
        private void ToggleButtonClick(object sender, EventArgs e)
        {
            this.scrolling = !this.scrolling;
            this.toggleButton.Image = this.scrolling ? toggleButtonImagePause : toggleButtonImagePlay;
            this.toggleButton.ToolTipText = (this.scrolling ? TextHelper.GetString("ToolTip.StopScrolling") : TextHelper.GetString("ToolTip.StartScrolling"));
            if (this.scrolling) this.AddTraces();
        }

        /// <summary>
        /// Handle the muting of the traces
        /// </summary>
        private void TextLogMouseDown(object sender, MouseEventArgs e)
        {
            this.muted = true;
        }

        /// <summary>
        /// Handle the muting of the traces
        /// </summary>
        private void TextLogMouseUp(object sender, MouseEventArgs e)
        {
            this.muted = false;
            this.AddTraces();
        }

        #endregion

    }

}
