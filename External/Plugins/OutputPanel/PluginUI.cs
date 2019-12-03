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
        int logCount;
        RichTextBoxEx textLog;
        readonly PluginMain pluginMain;
        string searchInvitation;
        System.Timers.Timer scrollTimer;
        ToolStripMenuItem wrapTextItem;
        ToolStripSpringTextBox findTextBox;
        ToolStripSeparator toolStripSeparator1;
        ToolStripButton toggleButton;
        ToolStripButton clearButton;
        ToolStrip toolStrip;
        Timer typingTimer;
        bool scrolling;
        Timer autoShow;
        bool muted;
        readonly Image toggleButtonImagePause;
        readonly Image toggleButtonImagePlay;
        readonly Image toggleButtonImagePlayNew;

        public PluginUI(PluginMain pluginMain)
        {
            InitializeTimers();
            scrolling = false;
            this.pluginMain = pluginMain;
            logCount = TraceManager.TraceLog.Count;
            InitializeComponent();
            InitializeContextMenu();
            InitializeLayout();
            toggleButtonImagePause = PluginBase.MainForm.FindImage("146");
            toggleButtonImagePlay = PluginBase.MainForm.FindImage("147");
            toggleButtonImagePlayNew = PluginBase.MainForm.FindImage("147|17|5|4");
            ToggleButtonClick(this, new EventArgs());
            ScrollBarEx.Attach(textLog);
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent()
        {
            scrollTimer = new System.Timers.Timer();
            textLog = new RichTextBoxEx();
            toolStrip = new ToolStripEx();
            toggleButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            findTextBox = new ToolStripSpringTextBox();
            clearButton = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(scrollTimer)).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // scrollTimer
            // 
            scrollTimer.Interval = 50;
            scrollTimer.SynchronizingObject = this;
            scrollTimer.Elapsed += ScrollTimerElapsed;
            // 
            // textLog
            // 
            textLog.BackColor = SystemColors.Window;
            textLog.BorderStyle = BorderStyle.None;
            textLog.Location = new Point(1, 26);
            textLog.Name = "textLog";
            textLog.ReadOnly = true;
            textLog.Size = new Size(278, 326);
            textLog.TabIndex = 1;
            textLog.Text = "";
            textLog.WordWrap = false;
            textLog.KeyDown += PluginUIKeyDown;
            textLog.MouseUp += TextLogMouseUp;
            textLog.LinkClicked += LinkClicked;
            textLog.MouseDown += TextLogMouseDown;
            // 
            // toolStrip
            // 
            toolStrip.CanOverflow = false;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            toolStrip.Items.AddRange(new ToolStripItem[] {
            toggleButton,
            toolStripSeparator1,
            findTextBox,
            clearButton});
            toolStrip.Location = new Point(1, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(1, 1, 2, 2);
            toolStrip.Size = new Size(278, 26);
            toolStrip.Stretch = true;
            toolStrip.TabIndex = 1;
            // 
            // toggleButton
            // 
            toggleButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toggleButton.ImageTransparentColor = Color.Magenta;
            toggleButton.Name = "toggleButton";
            toggleButton.Size = new Size(23, 20);
            toggleButton.Text = "toolStripButton1";
            toggleButton.Click += ToggleButtonClick;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 23);
            // 
            // findTextBox
            //
            findTextBox.Name = "FindTextBox";
            findTextBox.Size = new Size(190, 23);
            findTextBox.Padding = new Padding(0, 0, 1, 0);
            findTextBox.ForeColor = SystemColors.GrayText;
            findTextBox.TextChanged += FindTextBoxTextChanged;
            findTextBox.KeyDown += PluginUIKeyDown;
            findTextBox.Leave += FindTextBoxLeave;
            findTextBox.Enter += FindTextBoxEnter;
            // 
            // clearButton
            //
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(23, 21);
            clearButton.Margin = new Padding(0, 1, 0, 1);
            clearButton.ImageTransparentColor = Color.Magenta;
            clearButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clearButton.Click += ClearButtonClick;
            // 
            // PluginUI
            // 
            Controls.Add(textLog);
            Controls.Add(toolStrip);
            Name = "PluginUI";
            Size = new Size(280, 352);
            ((System.ComponentModel.ISupportInitialize)(scrollTimer)).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods and Event Handlers

        /// <summary>
        /// Initializes the custom rendering
        /// </summary>
        void InitializeLayout()
        {
            toolStrip.Renderer = new DockPanelStripRenderer();
        }

        /// <summary>
        /// Initializes the timers used in the control.
        /// </summary>
        void InitializeTimers()
        {
            autoShow = new Timer();
            autoShow.Interval = 300;
            autoShow.Tick += AutoShowPanel;
            typingTimer = new Timer();
            typingTimer.Tick += TypingTimerTick;
            typingTimer.Interval = 250;
        }

        /// <summary>
        /// Initializes the context menu
        /// </summary>
        void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer();
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.ClearOutput"), null, ClearOutput));
            menu.Items.Add(new ToolStripMenuItem(TextHelper.GetString("Label.CopyOutput"), null, CopyOutput));
            menu.Items.Add(new ToolStripSeparator());
            wrapTextItem = new ToolStripMenuItem(TextHelper.GetString("Label.WrapText"), null, WrapText);
            menu.Items.Add(wrapTextItem);
            searchInvitation = TextHelper.GetString("Label.SearchInvitation");
            clearButton.ToolTipText = TextHelper.GetString("Label.ClearSearchText");
            clearButton.Image = PluginBase.MainForm.FindImage("153");
            textLog.Font = PluginBase.Settings.ConsoleFont;
            findTextBox.Text = searchInvitation;
            textLog.ContextMenuStrip = menu;
            ApplyWrapText();
        }

        /// <summary>
        /// Opens the clicked link
        /// </summary>
        void LinkClicked(object sender, LinkClickedEventArgs e)
        {
            PluginBase.MainForm.CallCommand("Browse", e.LinkText);
        }

        /// <summary>
        /// Handle the internal key down event
        /// </summary>
        void PluginUIKeyDown(object sender, KeyEventArgs e) => OnShortcut(e.KeyData);

        /// <summary>
        /// Changes the wrapping in the control
        /// </summary>
        void WrapText(object sender, EventArgs e)
        {
            pluginMain.PluginSettings.WrapOutput = !pluginMain.PluginSettings.WrapOutput;
            pluginMain.SaveSettings();
            ApplyWrapText();
        }

        /// <summary>
        /// Applies the wrapping in the control
        /// </summary>
        public void ApplyWrapText()
        {
            if (pluginMain.PluginPanel is null) return;
            if (pluginMain.PluginPanel.InvokeRequired)
            {
                pluginMain.PluginPanel.BeginInvoke((MethodInvoker)ApplyWrapText);
                return;
            }
            wrapTextItem.Checked = pluginMain.PluginSettings.WrapOutput;
            textLog.WordWrap = pluginMain.PluginSettings.WrapOutput;
        }

        /// <summary>
        /// Copies the text to clipboard
        /// </summary>
        void CopyOutput(object sender, EventArgs e)
        {
            if (textLog.SelectedText.Length > 0) textLog.Copy();
            else if (!string.IsNullOrEmpty(textLog.Text))
            {
                Clipboard.SetText(textLog.Text);
                PluginBase.MainForm.RefreshUI();
            }
        }

        /// <summary>
        /// Update colors on start after theme engine
        /// </summary>
        public void UpdateAfterTheme()
        {
            findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
        }

        /// <summary>
        /// Clears the output
        /// </summary>
        public void ClearOutput(object sender, EventArgs e) => textLog.Clear();

        /// <summary>
        /// Flashes the panel to the user
        /// </summary>
        public void DisplayOutput()
        {
            autoShow.Stop();
            autoShow.Start();
        }

        /// <summary>
        /// Shows the panel
        /// </summary>
        void AutoShowPanel(object sender, EventArgs e)
        {
            autoShow.Stop();
            if (textLog.TextLength <= 0) return;
            var panel = (DockContent) Parent;
            var ds = panel.VisibleState;
            if (panel.Visible && !ds.ToString().EndsWithOrdinal("AutoHide")) return;
            panel.Show();
            if (ds.ToString().EndsWithOrdinal("AutoHide")) panel.Activate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // We use custom resizing because when the owner DockPanel hides, textLog.Height = 0, and ScrollToCaret() fails
            if (Height == 0) return;
            var bounds = Rectangle.FromLTRB(Padding.Left, toolStrip.Bottom, ClientSize.Width - Padding.Right, ClientSize.Height - Padding.Bottom);
            textLog.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                
            // Recreate handle and restore scrollbars position, built-in behavior is flawed (eg.: go to bottom of scroll and resize)
            if (!Win32.ShouldUseWin32()) return;
            int vPos = Win32.GetScrollPos(textLog.Handle, Win32.SB_VERT);
            int hPos = Win32.GetScrollPos(textLog.Handle, Win32.SB_HORZ);
            textLog.Recreate();
            int wParam = Win32.SB_THUMBPOSITION | vPos << 16;
            Win32.SendMessage(textLog.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            wParam = Win32.SB_THUMBPOSITION | hPos << 16;
            Win32.SendMessage(textLog.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
        }

        /// <summary>
        /// Handles the shortcut
        /// </summary>
        public bool OnShortcut(Keys keys)
        {
            if (ContainsFocus)
            {
                switch (keys)
                {
                    case Keys.F3:
                        FindNextMatch(true);
                        return true;
                    case Keys.Shift | Keys.F3:
                        FindNextMatch(false);
                        return true;
                    case Keys.Escape:
                    {
                        if (PluginBase.MainForm.CurrentDocument?.SciControl is { } sci) sci.Focus();
                        break;
                    }
                    case Keys.Control | Keys.F:
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
            if (muted) return;
            if (!scrolling)
            {
                toggleButton.Image = toggleButtonImagePlayNew;
                return;
            }
            var log = TraceManager.TraceLog;
            var newCount = log.Count;
            if (newCount <= logCount)
            {
                logCount = newCount;
                return;
            }
            Color newColor = Color.Red;
            Color currentColor = Color.Red;
            int oldSelectionStart = textLog.SelectionStart;
            int oldSelectionLength = textLog.SelectionLength;
            List<HighlightMarker> markers = pluginMain.PluginSettings.HighlightMarkers;
            bool fastMode = (newCount - logCount) > 1000;
            StringBuilder newText = new StringBuilder();
            for (int i = logCount; i < newCount; i++)
            {
                var entry = log[i];
                var state = entry.State;
                var message = entry.Message ?? "";
                if (!fastMode)
                {
                    // Automatic state from message, legacy format, ie. "2:message" -> state = 2
                    if (pluginMain.PluginSettings.UseLegacyColoring && state == 1 && message.Length > 2 && message[1] == ':' && char.IsDigit(message[0]))
                    {
                        if (int.TryParse(message[0].ToString(), out state))
                        {
                            message = message.Substring(2);
                        }
                    }
                    // Automatic state from message: New format with customizable markers
                    if (state == 1 && !markers.IsNullOrEmpty())
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
                            newColor = PluginBase.MainForm.GetThemeColor("OutputPanel.DebugColor", ForeColor);
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
                            textLog.Select(textLog.TextLength, 0);
                            textLog.SelectionColor = currentColor;
                            textLog.AppendText(newText.ToString());
                            newText.Remove(0, newText.Length);
                        }
                        currentColor = newColor;
                    }
                }
                newText.Append(message + "\n");
            }
            if (newText.Length > 0)
            {
                ClearCurrentSelection();
                textLog.Select(textLog.TextLength, 0);
                textLog.SelectionColor = currentColor;
                textLog.AppendText(newText.ToString());
            }
            if (oldSelectionLength != 0) textLog.Select(oldSelectionStart, oldSelectionLength);
            else if (scrolling) textLog.Select(textLog.TextLength, 0);
            logCount = newCount;
            scrollTimer.Enabled = true;
            TypingTimerTick(null, null);
        }

        /// <summary>
        /// Scrolling fix on RichTextBox
        /// </summary> 
        void ScrollTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            scrollTimer.Enabled = false;
            if (pluginMain.PluginSettings.ShowOnProcessEnd)
            {
                DisplayOutput();
            }

            try
            {
                textLog.Select(textLog.TextLength, 0);
                textLog.ScrollToCaret();
            }
            catch { /* WineMod: not supported */ }
        }

        /// <summary>
        /// Filters the output by search text
        /// </summary>
        void FilterOutput(string findText)
        {
            textLog.Select(0, textLog.TextLength);
            textLog.SelectionBackColor = textLog.BackColor;
            if (findText.Trim().Length == 0) return;
            findText = Regex.Escape(findText);
            MatchCollection results = Regex.Matches(textLog.Text, findText, RegexOptions.IgnoreCase);
            for (int i = 0; i < results.Count; i++)
            {
                Match match = results[i];
                textLog.SelectionStart = match.Index;
                textLog.SelectionLength = match.Length;
                textLog.SelectionBackColor = PluginBase.MainForm.GetThemeColor("OutputPanel.HighlightColor", SystemColors.Highlight);
            }
        }

        /// <summary>
        /// Handles the text change event
        /// </summary>
        void FindTextBoxTextChanged(object sender, EventArgs e)
        {
            if (textLog.TextLength > 10000)
            {
                typingTimer.Stop();
                typingTimer.Start();
            }
            else TypingTimerTick(null, null);
        }

        /// <summary>
        /// When the typing timer ticks update the search
        /// </summary>
        void TypingTimerTick(object sender, EventArgs e)
        {
            typingTimer.Stop();
            string searchText = findTextBox.Text;
            if (searchText == searchInvitation) searchText = "";
            if (searchText.Trim().Length > 0) FilterOutput(searchText);
            else ClearCurrentSelection();
            clearButton.Enabled = searchText.Length > 0;
        }

        /// <summary>
        /// When user enters control, handle it
        /// </summary>
        void FindTextBoxEnter(object sender, EventArgs e)
        {
            if (findTextBox.Text != searchInvitation) return;
            findTextBox.Text = "";
            findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.ForeColor", SystemColors.WindowText);
        }

        /// <summary>
        /// When user leaves control, handle it
        /// </summary>
        void FindTextBoxLeave(object sender, EventArgs e)
        {
            if (findTextBox.Text != "") return;
            clearButton.Enabled = false;
            findTextBox.Text = searchInvitation;
            findTextBox.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
        }

        /// <summary>
        /// Clears the search text from the control
        /// </summary>
        void ClearButtonClick(object sender, EventArgs e)
        {
            findTextBox.Text = "";
            ClearCurrentSelection();
            findTextBox.Focus();
        }

        /// <summary>
        /// Finds the next match and selects it
        /// </summary>
        void FindNextMatch(bool forward)
        {
            try
            {
                string searchText = findTextBox.Text;
                if (searchText == searchInvitation) searchText = "";
                if (searchText.Trim().Length > 0)
                {
                    int curPos = textLog.SelectionStart + textLog.SelectionLength;
                    MatchCollection results = Regex.Matches(textLog.Text, searchText, RegexOptions.IgnoreCase);
                    Match nearestMatch = results[0];
                    for (int i = 0; i < results.Count; i++)
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
                            if (textLog.SelectedText.Length > 0 && curPos <= results[0].Index + results[0].Length)
                            {
                                nearestMatch = results[results.Count - 1];
                                break;
                            }
                            if (curPos < results[0].Index + results[0].Length)
                            {
                                nearestMatch = results[results.Count - 1];
                                break;
                            }
                            if (textLog.SelectedText.Length == 0 && curPos == results[i].Index + results[i].Length)
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
                    textLog.Focus();
                    textLog.Select(nearestMatch.Index, nearestMatch.Length);
                    try { textLog.ScrollToCaret(); }
                    catch { /* WineMod: not supported */ }
                }
            }
            catch { }
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        void ClearCurrentSelection()
        {
            int oldSelectionStart = textLog.SelectionStart;
            int oldSelectionLength = textLog.SelectionLength;
            textLog.Select(0, textLog.TextLength);
            textLog.SelectionBackColor = textLog.BackColor;
            textLog.Select(oldSelectionStart, oldSelectionLength);
        }

        /// <summary>
        /// Toggle the scrolling enabled
        /// </summary>
        void ToggleButtonClick(object sender, EventArgs e)
        {
            scrolling = !scrolling;
            toggleButton.Image = scrolling ? toggleButtonImagePause : toggleButtonImagePlay;
            toggleButton.ToolTipText = (scrolling ? TextHelper.GetString("ToolTip.StopScrolling") : TextHelper.GetString("ToolTip.StartScrolling"));
            if (scrolling) AddTraces();
        }

        /// <summary>
        /// Handle the muting of the traces
        /// </summary>
        void TextLogMouseDown(object sender, MouseEventArgs e) => muted = true;

        /// <summary>
        /// Handle the muting of the traces
        /// </summary>
        void TextLogMouseUp(object sender, MouseEventArgs e)
        {
            muted = false;
            AddTraces();
        }

        #endregion
    }
}