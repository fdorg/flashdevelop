// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using PluginCore.Localization;
using FlashDevelop.Utilities;
using FlashDevelop.Helpers;
using FlashDevelop.Docking;
using PluginCore.FRService;
using PluginCore.Managers;
using PluginCore.Controls;
using PluginCore.Helpers;
using ScintillaNet.Configuration;
using ScintillaNet;
using PluginCore;

namespace FlashDevelop.Controls
{
    public class QuickFind : ToolStripEx, IEventHandler
    {
        private Color backColor;
        private Timer highlightTimer;
        private CheckBox wholeWordCheckBox;
        private CheckBox highlightCheckBox;
        private CheckBox matchCaseCheckBox;
        private ToolStripButton nextButton;
        private ToolStripButton closeButton;
        private ToolStripControlHost wholeWordHost;
        private ToolStripControlHost matchCaseHost;
        private ToolStripControlHost highlightHost;
        private ToolStripButton previousButton;
        private ToolStripButton moreButton;
        private EscapeTextBox findTextBox;
        private ToolStripLabel findLabel;
        private ToolStripLabel infoLabel;
        private Timer typingTimer;

        public QuickFind()
        {
            Font = Globals.Settings.DefaultFont;
            InitializeComponent();
            InitializeGraphics();
            InitializeEvents();
            InitializeTimers();
        }

        #region Internal Events

        /// <summary>
        /// Initializes the internals events
        /// </summary>
        private void InitializeEvents() => EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.ApplyTheme);

        /// <summary>
        /// Handles the internal events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.FileSwitch)
            {
                ApplyFixedDocumentPadding();
            }
            else if (e.Type == EventType.ApplyTheme)
            {
                InitializeGraphics();
            }
        }

        #endregion

        #region Initialize Component

        public void InitializeComponent()
        {
            ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            highlightTimer = new Timer();
            wholeWordCheckBox = new CheckBoxEx();
            matchCaseCheckBox = new CheckBoxEx();
            highlightCheckBox = new CheckBoxEx();
            nextButton = new ToolStripButton();
            closeButton = new ToolStripButton();
            moreButton = new ToolStripButton();
            highlightHost = new ToolStripControlHost(highlightCheckBox);
            matchCaseHost = new ToolStripControlHost(matchCaseCheckBox);
            wholeWordHost = new ToolStripControlHost(wholeWordCheckBox);
            previousButton = new ToolStripButton();
            findTextBox = new EscapeTextBox();
            findLabel = new ToolStripLabel();
            infoLabel = new ToolStripLabel();
            SuspendLayout();
            //
            // highlightTimer
            //
            highlightTimer = new Timer();
            highlightTimer.Interval = 500;
            highlightTimer.Enabled = false;
            highlightTimer.Tick += delegate { HighlightTimerTick(); };
            //
            // findLabel
            //
            findLabel.BackColor = Color.Transparent;
            findLabel.Text = TextHelper.GetString("Info.Find");
            findLabel.Margin = new Padding(0, 0, 0, 3);
            //
            // infoLabel
            //
            infoLabel.BackColor = Color.Transparent;
            infoLabel.ForeColor = SystemColors.GrayText;
            infoLabel.Text = TextHelper.GetString("Info.NoMatches");
            infoLabel.Margin = new Padding(0, 0, 0, 1);
            //
            // highlightCheckBox
            //
            highlightHost.Margin = new Padding(0, 2, 6, 1);
            highlightCheckBox.Text = TextHelper.GetString("Label.HighlightAll");
            highlightCheckBox.BackColor = Color.Transparent;
            highlightCheckBox.Click += HighlightAllCheckBoxClick;
            //
            // matchCaseCheckBox
            //
            matchCaseHost.Margin = new Padding(0, 2, 6, 1);
            matchCaseCheckBox.Text = TextHelper.GetString("Label.MatchCase");
            matchCaseCheckBox.BackColor = Color.Transparent;
            matchCaseCheckBox.CheckedChanged += MatchCaseCheckBoxCheckedChanged;
            //
            // wholeWordCheckBox
            //
            wholeWordHost.Margin = new Padding(0, 2, 6, 1);
            wholeWordCheckBox.Text = TextHelper.GetString("Label.WholeWord");
            wholeWordCheckBox.BackColor = Color.Transparent;
            wholeWordCheckBox.CheckedChanged += WholeWordCheckBoxCheckedChanged;
            //
            // nextButton
            //
            nextButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            nextButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindNext.png"));
            nextButton.Click += FindNextButtonClick;
            nextButton.Text = TextHelper.GetString("Label.Next");
            nextButton.Margin = new Padding(0, 1, 2, 2);
            //
            // previousButton
            //
            previousButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            previousButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindPrev.png"));
            previousButton.Click += FindPrevButtonClick;
            previousButton.Text = TextHelper.GetString("Label.Previous");
            previousButton.Margin = new Padding(0, 1, 7, 2);
            //
            // closeButton
            //
            closeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            closeButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindClose.png"));
            closeButton.Click += CloseButtonClick;
            closeButton.Margin = new Padding(0, 1, 5, 2);
            //
            // findTextBox
            //
            findTextBox.Size = new Size(150, 21);
            findTextBox.KeyPress += FindTextBoxKeyPress;
            findTextBox.TextChanged += FindTextBoxTextChanged;
            findTextBox.OnKeyEscape += FindTextBoxOnKeyEscape;
            findTextBox.Margin = new Padding(0, 1, 7, 2);
            //
            // moreButton
            //
            moreButton.Click += MoreButtonClick;
            moreButton.Text = TextHelper.GetString("Label.More");
            moreButton.Alignment = ToolStripItemAlignment.Right;
            moreButton.Margin = new Padding(0, 1, 5, 2);
            //
            // QuickFind
            //
            Items.Add(closeButton);
            Items.Add(findLabel);
            Items.Add(findTextBox);
            Items.Add(nextButton);
            Items.Add(previousButton);
            Items.Add(matchCaseHost);
            Items.Add(wholeWordHost);
            Items.Add(highlightHost);
            Items.Add(infoLabel);
            Items.Add(moreButton);
            GripStyle = ToolStripGripStyle.Hidden;
            Renderer = new QuickFindRenderer();
            Padding = new Padding(4, 4, 0, 3);
            Dock = DockStyle.Bottom;
            CanOverflow = false;
            Visible = false;
            ResumeLayout(false);
        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// The document that contains this control
        /// </summary>
        public ITabbedDocument Document => ((ITabbedDocument)Parent);

        /// <summary>
        /// Enables or disables controls
        /// </summary>
        public bool CanSearch
        {
            get => findTextBox.Enabled;
            set
            {
                nextButton.Enabled = value;
                previousButton.Enabled = value;
                matchCaseCheckBox.Enabled = value;
                highlightCheckBox.Enabled = value;
                wholeWordCheckBox.Enabled = value;
                findTextBox.Enabled = value;
            }
        }

        /// <summary>
        /// Initializes the graphics used in the control.
        /// </summary>
        private void InitializeGraphics()
        {
            Color text = Globals.MainForm.GetThemeColor("QuickFind.ForeColor");
            Color fore = Globals.MainForm.GetThemeColor("ToolStripTextBoxControl.ForeColor");
            Color back = Globals.MainForm.GetThemeColor("ToolStripTextBoxControl.BackColor");
            bool useTheme = Globals.MainForm.GetThemeColor("QuickFind.BackColor") != Color.Empty;
            if (back != Color.Empty) backColor = findTextBox.BackColor = back;
            if (text != Color.Empty) infoLabel.ForeColor = text;
            if (fore != Color.Empty) findTextBox.ForeColor = fore;
            if (ScaleHelper.GetScale() >= 1.5)
            {
                nextButton.Image = Globals.MainForm.FindImage("67");
                previousButton.Image = Globals.MainForm.FindImage("63");
                closeButton.Image = Globals.MainForm.FindImage("111");
            }
            Padding pad = new Padding(0, 2, 6, useTheme ? 3 : 1);
            highlightHost.Margin = matchCaseHost.Margin = wholeWordHost.Margin = pad;
            PluginBase.MainForm.SetUseTheme(highlightCheckBox, useTheme);
            PluginBase.MainForm.SetUseTheme(matchCaseCheckBox, useTheme);
            PluginBase.MainForm.SetUseTheme(wholeWordCheckBox, useTheme);
            PluginBase.MainForm.ThemeControls(highlightCheckBox);
            PluginBase.MainForm.ThemeControls(matchCaseCheckBox);
            PluginBase.MainForm.ThemeControls(wholeWordCheckBox);
        }

        /// <summary>
        /// Initializes the timers used in the control.
        /// </summary>
        private void InitializeTimers()
        {
            typingTimer = new Timer();
            typingTimer.Tick += TypingTimerTick;
            typingTimer.Interval = 250;
        }

        /// <summary>
        /// Set the case of the text to search
        /// </summary>
        public void SetMatchCase(bool matchCase)
        {
            matchCaseCheckBox.CheckedChanged -= MatchCaseCheckBoxCheckedChanged;
            matchCaseCheckBox.Checked = matchCase; // Change the value...
            matchCaseCheckBox.CheckedChanged += MatchCaseCheckBoxCheckedChanged;
        }

        /// <summary>
        /// Set the whole word prop of the text to search
        /// </summary>
        public void SetWholeWord(bool wholeWord)
        {
            wholeWordCheckBox.CheckedChanged -= WholeWordCheckBoxCheckedChanged;
            wholeWordCheckBox.Checked = wholeWord; // Change the value...
            wholeWordCheckBox.CheckedChanged += WholeWordCheckBoxCheckedChanged;
        }

        /// <summary>
        /// Set the text to search
        /// </summary>
        public void SetFindText(string text)
        {
            findTextBox.TextChanged -= FindTextBoxTextChanged;
            findTextBox.Text = text; // Change the value...
            findTextBox.TextChanged += FindTextBoxTextChanged;
        }

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        public void ShowControl()
        {
            Show();
            UpdateFindText();
            ApplyFixedDocumentPadding();
            findTextBox.Focus();
            findTextBox.SelectAll();
        }

        /// <summary>
        /// Executes the search for next match
        /// </summary>
        public void FindNextButtonClick(object sender, EventArgs e)
        {
            if (findTextBox.Text.Length > 0)
            {
                FindNext(findTextBox.Text, false);
            }
        }

        /// <summary>
        /// Executes the search for previous match
        /// </summary>
        public void FindPrevButtonClick(object sender, EventArgs e)
        {
            if (findTextBox.Text.Length > 0)
            {
                FindPrev(findTextBox.Text, false);
            }
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box
        /// </summary>
        private void UpdateFindText()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci != null && sci.SelText.Length > 0)
            {
                findTextBox.Text = sci.SelText;
            }
        }

        /// <summary>
        /// Update the match case globally if it's changed
        /// </summary>
        private void MatchCaseCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetMatchCase(this, matchCaseCheckBox.Checked);
            }
        }

        /// <summary>
        /// Update the whole word globally if it's changed
        /// </summary>
        private void WholeWordCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetWholeWord(this, wholeWordCheckBox.Checked);
            }
        }

        /// <summary>
        /// Text into the main search textbox has changed, then 
        /// process with find next occurrence of the word
        /// </summary>
        private void FindTextBoxTextChanged(object sender, EventArgs e)
        {
            if (PluginBase.MainForm.CurrentDocument.SciControl.TextLength > 30000)
            {
                typingTimer.Stop();
                typingTimer.Start();
            }
            else TypingTimerTick(null, null);
        }

        /// <summary>
        /// When the typing timer ticks update the search
        /// </summary>
        private void TypingTimerTick(object sender, EventArgs e)
        {
            typingTimer.Stop();
            if (findTextBox.Text.Length > 0)
            {
                FindCorrect(findTextBox.Text, highlightCheckBox.Checked);
            }
            else
            {
                infoLabel.Text = "";
                findTextBox.BackColor = backColor;
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                sci.RemoveHighlights();
            }
            Globals.MainForm.SetFindText(this, findTextBox.Text);
        }

        /// <summary>
        /// Escape key has been pressed into the toolstriptextbox then 
        /// assign the current focus to the current scintilla control
        /// </summary>
        private void FindTextBoxOnKeyEscape()
        {
            PluginBase.MainForm.CurrentDocument.Activate();
            CloseButtonClick(null, null);
        }

        /// <summary>
        /// Pressed key on the main textbox
        /// </summary>
        private void FindTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return && findTextBox.Text.Length > 0)
            {
                e.Handled = true;
                if ((ModifierKeys & Keys.Shift) == Keys.Shift) FindPrev(findTextBox.Text, false);
                else FindNext(findTextBox.Text, false);
            }
        }

        /// <summary>
        /// Timed highlight of all current items
        /// </summary>
        private void HighlightTimerTick()
        {
            highlightTimer.Stop();
            if (highlightTimer.Tag is Hashtable hashtable)
            {
                try
                {
                    var sci = hashtable["sci"] as ScintillaControl;
                    var matches = hashtable["matches"] as List<SearchMatch>;
                    AddHighlights(sci, matches);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// Highlights or removes highlights for all results
        /// </summary>
        private void HighlightAllCheckBoxClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (highlightCheckBox.Checked)
            {
                if (findTextBox.Text.Length == 0) return;
                var matches = GetResults(sci, findTextBox.Text);
                if (matches.Count != 0)
                {
                    sci.RemoveHighlights();
                    if (highlightTimer.Enabled) highlightTimer.Stop();
                    if (highlightCheckBox.Checked) AddHighlights(sci, matches);
                }
            }
            else sci.RemoveHighlights();
        }

        /// <summary>
        /// Finds the correct match based on the current position
        /// </summary>
        private void FindCorrect(string text, bool refreshHighlights)
        {
            if (string.IsNullOrEmpty(text)) return;
            findTextBox.BackColor = backColor;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var matches = GetResults(sci, text);
            if (matches.Count != 0)
            {
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, true, true);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) RefreshHighlights(sci, matches);
                string message = TextHelper.GetString("Info.ShowingResult");
                int index = FRDialogGenerics.GetMatchIndex(match, matches);
                string formatted = string.Format(message, index, matches.Count);
                infoLabel.Text = formatted;
            }
            else
            {
                findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                string message = TextHelper.GetString("Info.NoMatchesFound");
                infoLabel.Text = message;
            }
        }

        /// <summary>
        /// Finds the next match based on the current position
        /// </summary>
        private void FindNext(string text, bool refreshHighlights)
        {
            if (text == "") return;
            findTextBox.BackColor = backColor;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var matches = GetResults(sci, text);
            if (matches.Count != 0)
            {
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, true, false);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) RefreshHighlights(sci, matches);
                string message = TextHelper.GetString("Info.ShowingResult");
                int index = FRDialogGenerics.GetMatchIndex(match, matches);
                string formatted = string.Format(message, index, matches.Count);
                infoLabel.Text = formatted;
            }
            else
            {
                findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                string message = TextHelper.GetString("Info.NoMatchesFound");
                infoLabel.Text = message;
            }
        }

        /// <summary>
        /// Finds the previous match based on the current position
        /// </summary>
        private void FindPrev(string text, bool refreshHighlights)
        {
            if (text == "") return;
            findTextBox.BackColor = backColor;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var matches = GetResults(sci, text);
            if (matches.Count != 0)
            {
                var match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, false, false);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) RefreshHighlights(sci, matches);
                var message = TextHelper.GetString("Info.ShowingResult");
                var index = FRDialogGenerics.GetMatchIndex(match, matches);
                var formatted = string.Format(message, index, matches.Count);
                infoLabel.Text = formatted;
            }
            else
            {
                findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                string message = TextHelper.GetString("Info.NoMatchesFound");
                infoLabel.Text = message;
            }
        }

        /// <summary>
        /// Fix the padding of documents when quick find is visible
        /// </summary>
        public void ApplyFixedDocumentPadding()
        {
            foreach (ITabbedDocument castable in Globals.MainForm.Documents)
            {
                TabbedDocument document = castable as TabbedDocument;
                if (document.IsEditable)
                {
                    Rectangle find = RectangleToScreen(ClientRectangle);
                    Rectangle doc = document.RectangleToScreen(document.ClientRectangle);
                    if (Visible && doc.IntersectsWith(find)) document.Padding = new Padding(0, 0, 0, Height - 1);
                    else document.Padding = new Padding(0);
                }
            }
        }

        /// <summary>
        /// Remove the status strip elements
        /// </summary>
        private void CloseButtonClick(object sender, EventArgs e)
        {
            Hide();
            ApplyFixedDocumentPadding();
        }

        /// <summary>
        /// Open the Find And Replace dialog
        /// </summary>
        private void MoreButtonClick(object sender, EventArgs e)
        {
            CloseButtonClick(null, null);
            PluginBase.MainForm.CallCommand("FindAndReplace", null);
        }

        /// <summary>
        /// Adds highlights to the correct sci control
        /// </summary>
        private void AddHighlights(ScintillaControl sci, List<SearchMatch> matches)
        {
            Language language = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
            sci.AddHighlights(matches, language.editorstyle.HighlightBackColor);
        }

        /// <summary>
        /// Refreshes the highlights
        /// </summary>
        private void RefreshHighlights(ScintillaControl sci, List<SearchMatch> matches)
        {
            sci.RemoveHighlights();
            if (highlightTimer.Enabled) highlightTimer.Stop();
            Hashtable table = new Hashtable();
            table["sci"] = sci;
            table["matches"] = matches;
            highlightTimer.Tag = table;
            highlightTimer.Start();
        }

        /// Gets search results for a sci control
        /// </summary>
        private List<SearchMatch> GetResults(ScintillaControl sci, string text)
        {
            string pattern = text;
            FRSearch search = new FRSearch(pattern);
            search.Filter = SearchFilter.None;
            search.NoCase = !matchCaseCheckBox.Checked;
            search.WholeWord = wholeWordCheckBox.Checked;
            search.SourceFile = sci.FileName;
            return search.Matches(sci.Text);
        }

        #endregion

        #region QuickFind Renderer

        public class QuickFindRenderer : ToolStripRenderer
        {
            private ToolStrip toolStrip;
            private readonly ToolStripRenderer renderer;

            public QuickFindRenderer()
            {
                UiRenderMode renderMode = Globals.Settings.RenderMode;
                if (renderMode == UiRenderMode.System) renderer = new ToolStripSystemRenderer();
                else renderer = new DockPanelStripRenderer();
            }

            protected override void Initialize(ToolStrip toolStrip)
            {
                this.toolStrip = toolStrip;
                this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
                this.toolStrip.Paint += OnToolStripPaint;
                base.Initialize(toolStrip);
            }

            protected override void InitializeItem(ToolStripItem item)
            {
                base.InitializeItem(item);
                if (item is ToolStripButton)
                {
                    double scale = ScaleHelper.GetScale();
                    if (scale >= 1.5)
                    {
                        item.Padding = new Padding(4, 2, 4, 2);
                    }
                    else if (scale >= 1.2)
                    {
                        item.Padding = new Padding(2, 1, 2, 1);
                    }
                    else if (renderer is ToolStripSystemRenderer && PlatformHelper.IsRunningOnWindows())
                    {
                        item.Padding = new Padding(2, 2, 2, 2);
                    }
                }
                else if (item is ToolStripTextBox)
                {
                    var textBox = item as ToolStripTextBox;
                    Color border = Globals.MainForm.GetThemeColor("ToolStripTextBoxControl.BorderColor");
                    if (border != Color.Empty) // Are we theming?
                    {
                        textBox.Margin = new Padding(2, 1, 2, 1);
                        textBox.BorderStyle = BorderStyle.None;
                    }
                }
            }

            private void OnToolStripPaint(object sender, PaintEventArgs e)
            {
                Color tborder = Globals.MainForm.GetThemeColor("ToolStripTextBoxControl.BorderColor");
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    if (item is ToolStripTextBox && tborder != Color.Empty)
                    {
                        var textBox = item as ToolStripTextBox;
                        var size = textBox.TextBox.Size;
                        var location = textBox.TextBox.Location;
                        e.Graphics.FillRectangle(new SolidBrush(item.BackColor), location.X - 2, location.Y - 3, size.Width + 2, size.Height + 6);
                        e.Graphics.DrawRectangle(new Pen(tborder), location.X - 2, location.Y - 3, size.Width + 2, size.Height + 6);
                    }
                }
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                Rectangle r = e.AffectedBounds;
                Color back = Globals.MainForm.GetThemeColor("ToolStrip.3dDarkColor");
                Color fore = Globals.MainForm.GetThemeColor("ToolStrip.3dLightColor");
                e.Graphics.DrawLine(fore == Color.Empty ? SystemPens.ControlLightLight : new Pen(fore), r.Left, r.Top + 1, r.Right, r.Top + 1);
                e.Graphics.DrawLine(back == Color.Empty ? SystemPens.ControlDark : new Pen(back), r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
                e.Graphics.DrawLine(back == Color.Empty ? SystemPens.ControlDark : new Pen(back), r.Right - 1, r.Top, r.Right - 1, r.Bottom);
                e.Graphics.DrawLine(back == Color.Empty ? SystemPens.ControlDark : new Pen(back), r.Left, r.Top, r.Left, r.Bottom);
                e.Graphics.DrawLine(back == Color.Empty ? SystemPens.ControlDark : new Pen(back), r.Left, r.Top, r.Right, r.Top);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (renderer is ToolStripProfessionalRenderer)
                {
                    bool isOver = false;
                    Color back = Globals.MainForm.GetThemeColor("ToolStripItem.BackColor");
                    Color border = Globals.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                    Color active = Globals.MainForm.GetThemeColor("ToolStripMenu.DropDownBorderColor");
                    if (e.Item is ToolStripButton)
                    {
                        ToolStripButton button = e.Item as ToolStripButton;
                        Rectangle bBounds = button.Owner.RectangleToScreen(button.Bounds);
                        isOver = bBounds.Contains(MousePosition);
                    }
                    if (e.Item.Selected || ((ToolStripButton)e.Item).Checked || (isOver && e.Item.Enabled))
                    {
                        Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                        Rectangle rect2 = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                        LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                        e.Graphics.FillRectangle(b, rect);
                        Rectangle rect3 = new Rectangle(rect2.Left - 1, rect2.Top - 1, rect2.Width + 1, rect2.Height + 1);
                        Rectangle rect4 = new Rectangle(rect3.Left + 1, rect3.Top + 1, rect3.Width - 2, rect3.Height - 2);
                        e.Graphics.DrawRectangle(new Pen(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border), rect3);
                        e.Graphics.DrawRectangle(new Pen(back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back), rect4);
                    }
                    if (e.Item.Pressed)
                    {
                        Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                        LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                        e.Graphics.FillRectangle(b, rect);
                        Rectangle rect2 = new Rectangle(rect.Left - 1, rect.Top - 1, rect.Width + 1, rect.Height + 1);
                        e.Graphics.DrawRectangle(new Pen(active == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : active), rect2);
                    }
                }
                else renderer.DrawButtonBackground(e);
            }

            #region Reuse Some Renderer Stuff

            protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
            {
                renderer.DrawGrip(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                renderer.DrawSeparator(e);
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                renderer.DrawToolStripBackground(e);
            }

            protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawDropDownButtonBackground(e);
            }

            protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawItemBackground(e);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                renderer.DrawItemCheck(e);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                renderer.DrawItemText(e);
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                renderer.DrawItemImage(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                renderer.DrawArrow(e);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                renderer.DrawImageMargin(e);
            }

            protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawLabelBackground(e);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawMenuItemBackground(e);
            }

            protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawOverflowButtonBackground(e);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawSplitButton(e);
            }

            protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
            {
                renderer.DrawStatusStripSizingGrip(e);
            }

            protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
            {
                renderer.DrawToolStripContentPanelBackground(e);
            }

            protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
            {
                renderer.DrawToolStripPanelBackground(e);
            }

            protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
            {
                renderer.DrawToolStripStatusLabelBackground(e);
            }

            #endregion

        }

        #endregion

        #region Custom Controls

        public delegate void KeyEscapeEvent();

        public class EscapeTextBox : ToolStripTextBox
        {
            public event KeyEscapeEvent OnKeyEscape;

            public EscapeTextBox()
            {
                Control.PreviewKeyDown += OnPreviewKeyDown;
            }

            protected override bool ProcessCmdKey(ref Message m, Keys keyData)
            {
                if (keyData == Keys.Escape) OnPressEscapeKey();
                return false;
            }

            private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
            {
                Keys ctrlAlt = Keys.Control | Keys.Alt;
                if ((e.KeyData & ctrlAlt) == ctrlAlt) e.IsInputKey = true;
            }

            protected void OnPressEscapeKey() => OnKeyEscape?.Invoke();
        }

        #endregion

    }
}