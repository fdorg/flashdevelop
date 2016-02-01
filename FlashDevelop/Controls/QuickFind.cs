using System;
using System.Text;
using System.Media;
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
using PluginCore.Utilities;
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
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeEvents();
            this.InitializeTimers();
        }

        #region Internal Events

        /// <summary>
        /// Initializes the internals events
        /// </summary>
        private void InitializeEvents()
        {
            EventManager.AddEventHandler(this, EventType.FileSwitch);
        }

        /// <summary>
        /// Handles the internal events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            this.ApplyFixedDocumentPadding();
        }

        #endregion

        #region Initialize Component

        public void InitializeComponent()
        {
            this.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.highlightTimer = new Timer();
            this.wholeWordCheckBox = new CheckBox();
            this.matchCaseCheckBox = new CheckBox();
            this.highlightCheckBox = new CheckBox();
            this.nextButton = new ToolStripButton();
            this.closeButton = new ToolStripButton();
            this.moreButton = new ToolStripButton();
            this.highlightHost = new ToolStripControlHost(this.highlightCheckBox);
            this.matchCaseHost = new ToolStripControlHost(this.matchCaseCheckBox);
            this.wholeWordHost = new ToolStripControlHost(this.wholeWordCheckBox);
            this.previousButton = new ToolStripButton();
            this.findTextBox = new EscapeTextBox();
            this.findLabel = new ToolStripLabel();
            this.infoLabel = new ToolStripLabel();
            this.SuspendLayout();
            //
            // highlightTimer
            //
            this.highlightTimer = new Timer();
            this.highlightTimer.Interval = 500;
            this.highlightTimer.Enabled = false;
            this.highlightTimer.Tick += delegate { this.HighlightTimerTick(); };
            //
            // findLabel
            //
            this.findLabel.BackColor = Color.Transparent;
            this.findLabel.Text = TextHelper.GetString("Info.Find");
            this.findLabel.Margin = new Padding(0, 0, 0, 3);
            //
            // infoLabel
            //
            this.infoLabel.BackColor = Color.Transparent;
            this.infoLabel.ForeColor = SystemColors.GrayText;
            this.infoLabel.Text = TextHelper.GetString("Info.NoMatches");
            this.infoLabel.Margin = new Padding(0, 0, 0, 1);
            //
            // highlightCheckBox
            //
            this.highlightHost.Margin = new Padding(0, 2, 6, 1);
            this.highlightCheckBox.Text = TextHelper.GetString("Label.HighlightAll");
            this.highlightCheckBox.BackColor = Color.Transparent;
            this.highlightCheckBox.Click += new EventHandler(this.HighlightAllCheckBoxClick);
            //
            // matchCaseCheckBox
            //
            this.matchCaseHost.Margin = new Padding(0, 2, 6, 1);
            this.matchCaseCheckBox.Text = TextHelper.GetString("Label.MatchCase");
            this.matchCaseCheckBox.BackColor = Color.Transparent;
            this.matchCaseCheckBox.CheckedChanged += new EventHandler(this.MatchCaseCheckBoxCheckedChanged);
            //
            // wholeWordCheckBox
            //
            this.wholeWordHost.Margin = new Padding(0, 2, 6, 1);
            this.wholeWordCheckBox.Text = TextHelper.GetString("Label.WholeWord");
            this.wholeWordCheckBox.BackColor = Color.Transparent;
            this.wholeWordCheckBox.CheckedChanged += new EventHandler(this.WholeWordCheckBoxCheckedChanged);
            //
            // nextButton
            //
            this.nextButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.nextButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindNext.png"));
            this.nextButton.Click += new EventHandler(this.FindNextButtonClick);
            this.nextButton.Text = TextHelper.GetString("Label.Next");
            this.nextButton.Margin = new Padding(0, 1, 2, 2);
            //
            // previousButton
            //
            this.previousButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.previousButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindPrev.png"));
            this.previousButton.Click += new EventHandler(this.FindPrevButtonClick);
            this.previousButton.Text = TextHelper.GetString("Label.Previous");
            this.previousButton.Margin = new Padding(0, 1, 7, 2);
            //
            // closeButton
            //
            this.closeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.closeButton.Image = Image.FromStream(ResourceHelper.GetStream("QuickFindClose.png"));
            this.closeButton.Click += new EventHandler(this.CloseButtonClick);
            this.closeButton.Margin = new Padding(0, 1, 5, 2);
            //
            // findTextBox
            //
            this.findTextBox.Size = new Size(150, 21);
            this.findTextBox.KeyPress += new KeyPressEventHandler(this.FindTextBoxKeyPress);
            this.findTextBox.TextChanged += new EventHandler(this.FindTextBoxTextChanged);
            this.findTextBox.OnKeyEscape += new KeyEscapeEvent(this.FindTextBoxOnKeyEscape);
            this.findTextBox.Margin = new Padding(0, 1, 7, 2);
            //
            // moreButton
            //
            this.moreButton.Click += new EventHandler(this.MoreButtonClick);
            this.moreButton.Text = TextHelper.GetString("Label.More");
            this.moreButton.Alignment = ToolStripItemAlignment.Right;
            this.moreButton.Margin = new Padding(0, 1, 5, 2);
            //
            // QuickFind
            //
            this.Items.Add(this.closeButton);
            this.Items.Add(this.findLabel);
            this.Items.Add(this.findTextBox);
            this.Items.Add(this.nextButton);
            this.Items.Add(this.previousButton);
            this.Items.Add(this.matchCaseHost);
            this.Items.Add(this.wholeWordHost);
            this.Items.Add(this.highlightHost);
            this.Items.Add(this.infoLabel);
            this.Items.Add(this.moreButton);
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.Renderer = new QuickFindRenderer();
            this.Padding = new Padding(4, 4, 0, 3);
            this.Dock = DockStyle.Bottom;
            this.CanOverflow = false;
            this.Visible = false;
            this.ResumeLayout(false);
        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// The document that contains this control
        /// </summary>
        public ITabbedDocument Document
        {
            get { return ((ITabbedDocument)this.Parent); }
        }

        /// <summary>
        /// Enables or disables controls
        /// </summary>
        public Boolean CanSearch
        {
            get { return this.findTextBox.Enabled; }
            set
            {
                this.nextButton.Enabled = value;
                this.previousButton.Enabled = value;
                this.matchCaseCheckBox.Enabled = value;
                this.highlightCheckBox.Enabled = value;
                this.wholeWordCheckBox.Enabled = value;
                this.findTextBox.Enabled = value;
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
            if (back != Color.Empty) this.backColor = this.findTextBox.BackColor = back;
            if (text != Color.Empty) this.infoLabel.ForeColor = text;
            if (fore != Color.Empty) this.findTextBox.ForeColor = fore;
            if (ScaleHelper.GetScale() >= 1.5)
            {
                this.nextButton.Image = Globals.MainForm.FindImage("67");
                this.previousButton.Image = Globals.MainForm.FindImage("63");
                this.closeButton.Image = Globals.MainForm.FindImage("111");
            }
        }

        /// <summary>
        /// Initializes the timers used in the control.
        /// </summary>
        private void InitializeTimers()
        {
            this.typingTimer = new Timer();
            this.typingTimer.Tick += new EventHandler(this.TypingTimerTick);
            this.typingTimer.Interval = 250;
        }

        /// <summary>
        /// Set the case of the text to search
        /// </summary>
        public void SetMatchCase(Boolean matchCase)
        {
            this.matchCaseCheckBox.CheckedChanged -= new EventHandler(this.MatchCaseCheckBoxCheckedChanged);
            this.matchCaseCheckBox.Checked = matchCase; // Change the value...
            this.matchCaseCheckBox.CheckedChanged += new EventHandler(this.MatchCaseCheckBoxCheckedChanged);
        }

        /// <summary>
        /// Set the whole word prop of the text to search
        /// </summary>
        public void SetWholeWord(Boolean wholeWord)
        {
            this.wholeWordCheckBox.CheckedChanged -= new EventHandler(this.WholeWordCheckBoxCheckedChanged);
            this.wholeWordCheckBox.Checked = wholeWord; // Change the value...
            this.wholeWordCheckBox.CheckedChanged += new EventHandler(this.WholeWordCheckBoxCheckedChanged);
        }

        /// <summary>
        /// Set the text to search
        /// </summary>
        public void SetFindText(String text)
        {
            this.findTextBox.TextChanged -= new EventHandler(this.FindTextBoxTextChanged);
            this.findTextBox.Text = text; // Change the value...
            this.findTextBox.TextChanged += new EventHandler(this.FindTextBoxTextChanged);
        }

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        public void ShowControl()
        {
            this.Show();
            this.UpdateFindText();
            this.ApplyFixedDocumentPadding();
            this.findTextBox.Focus();
            this.findTextBox.SelectAll();
        }

        /// <summary>
        /// Executes the search for next match
        /// </summary>
        public void FindNextButtonClick(Object sender, EventArgs e)
        {
            if (this.findTextBox.Text.Trim() != "")
            {
                this.FindNext(this.findTextBox.Text, false);
            }
        }

        /// <summary>
        /// Executes the search for previous match
        /// </summary>
        public void FindPrevButtonClick(Object sender, EventArgs e)
        {
            if (this.findTextBox.Text.Trim() != "")
            {
                this.FindPrev(this.findTextBox.Text, false);
            }
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box
        /// </summary>
        private void UpdateFindText()
        {
            ScintillaControl sci = Globals.SciControl;
            if (sci != null && sci.SelText.Length > 0)
            {
                this.findTextBox.Text = sci.SelText;
            }
        }

        /// <summary>
        /// Update the match case globally if it's changed
        /// </summary>
        private void MatchCaseCheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            if (!Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetMatchCase(this, this.matchCaseCheckBox.Checked);
            }
        }

        /// <summary>
        /// Update the whole word globally if it's changed
        /// </summary>
        private void WholeWordCheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            if (!Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetWholeWord(this, this.wholeWordCheckBox.Checked);
            }
        }

        /// <summary>
        /// Text into the main search textbox has changed, then 
        /// process with find next occurrence of the word
        /// </summary>
        private void FindTextBoxTextChanged(Object sender, EventArgs e)
        {
            if (Globals.SciControl.TextLength > 30000)
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
            if (this.findTextBox.Text.Trim() != "")
            {
                this.FindCorrect(this.findTextBox.Text, this.highlightCheckBox.Checked);
            }
            else
            {
                this.infoLabel.Text = "";
                this.findTextBox.BackColor = this.backColor;
                ScintillaControl sci = Globals.SciControl;
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                sci.RemoveHighlights();
            }
            Globals.MainForm.SetFindText(this, this.findTextBox.Text);
        }

        /// <summary>
        /// Escape key has been pressed into the toolstriptextbox then 
        /// assign the current focus to the current scintilla control
        /// </summary>
        private void FindTextBoxOnKeyEscape()
        {
            Globals.CurrentDocument.Activate();
            this.CloseButtonClick(null, null);
        }

        /// <summary>
        /// Pressed key on the main textbox
        /// </summary>
        private void FindTextBoxKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Return && this.findTextBox.Text.Trim() != "")
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
            this.highlightTimer.Stop();
            if (this.highlightTimer.Tag != null && this.highlightTimer.Tag is Hashtable)
            {
                try
                {
                    ScintillaControl sci = ((Hashtable)this.highlightTimer.Tag)["sci"] as ScintillaControl;
                    List<SearchMatch> matches = ((Hashtable)this.highlightTimer.Tag)["matches"] as List<SearchMatch>;
                    this.AddHighlights(sci, matches);
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
        private void HighlightAllCheckBoxClick(Object sender, EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            if (this.highlightCheckBox.Checked)
            {
                if (this.findTextBox.Text.Trim() == "") return;
                List<SearchMatch> matches = this.GetResults(sci, this.findTextBox.Text);
                if (matches != null && matches.Count != 0)
                {
                    sci.RemoveHighlights();
                    if (this.highlightTimer.Enabled) this.highlightTimer.Stop();
                    if (this.highlightCheckBox.Checked) this.AddHighlights(sci, matches);
                }
            }
            else sci.RemoveHighlights();
        }

        /// <summary>
        /// Finds the correct match based on the current position
        /// </summary>
        private void FindCorrect(String text, Boolean refreshHighlights)
        {
            if (text == "") return;
            ScintillaControl sci = Globals.SciControl;
            this.findTextBox.BackColor = this.backColor;
            List<SearchMatch> matches = this.GetResults(sci, text);
            if (matches != null && matches.Count != 0)
            {
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, true, true);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) this.RefreshHighlights(sci, matches);
                String message = TextHelper.GetString("Info.ShowingResult");
                Int32 index = FRDialogGenerics.GetMatchIndex(match, matches);
                String formatted = String.Format(message, index, matches.Count);
                this.infoLabel.Text = formatted;
            }
            else
            {
                this.findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                String message = TextHelper.GetString("Info.NoMatchesFound");
                this.infoLabel.Text = message;
            }
        }

        /// <summary>
        /// Finds the next match based on the current position
        /// </summary>
        private void FindNext(String text, Boolean refreshHighlights)
        {
            if (text == "") return;
            ScintillaControl sci = Globals.SciControl;
            this.findTextBox.BackColor = this.backColor;
            List<SearchMatch> matches = this.GetResults(sci, text);
            if (matches != null && matches.Count != 0)
            {
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, true, false);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) this.RefreshHighlights(sci, matches);
                String message = TextHelper.GetString("Info.ShowingResult");
                Int32 index = FRDialogGenerics.GetMatchIndex(match, matches);
                String formatted = String.Format(message, index, matches.Count);
                this.infoLabel.Text = formatted;
            }
            else
            {
                this.findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                String message = TextHelper.GetString("Info.NoMatchesFound");
                this.infoLabel.Text = message;
            }
        }

        /// <summary>
        /// Finds the previous match based on the current position
        /// </summary>
        private void FindPrev(String text, Boolean refreshHighlights)
        {
            if (text == "") return;
            ScintillaControl sci = Globals.SciControl;
            this.findTextBox.BackColor = this.backColor;
            List<SearchMatch> matches = this.GetResults(sci, text);
            if (matches != null && matches.Count != 0)
            {
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, false, false);
                if (match != null) FRDialogGenerics.SelectMatch(sci, match);
                if (refreshHighlights) this.RefreshHighlights(sci, matches);
                String message = TextHelper.GetString("Info.ShowingResult");
                Int32 index = FRDialogGenerics.GetMatchIndex(match, matches);
                String formatted = String.Format(message, index, matches.Count);
                this.infoLabel.Text = formatted;
            }
            else
            {
                this.findTextBox.BackColor = Globals.MainForm.GetThemeColor("QuickFind.ErrorBack", Color.Salmon);
                sci.SetSel(sci.SelectionStart, sci.SelectionStart);
                String message = TextHelper.GetString("Info.NoMatchesFound");
                this.infoLabel.Text = message;
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
                    Rectangle find = this.RectangleToScreen(this.ClientRectangle);
                    Rectangle doc = document.RectangleToScreen(document.ClientRectangle);
                    if (this.Visible && doc.IntersectsWith(find)) document.Padding = new Padding(0, 0, 0, this.Height - 1);
                    else document.Padding = new Padding(0);
                }
            }
        }

        /// <summary>
        /// Remove the status strip elements
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Hide();
            this.ApplyFixedDocumentPadding();
        }

        /// <summary>
        /// Open the Find And Replace dialog
        /// </summary>
        private void MoreButtonClick(Object sender, EventArgs e)
        {
            this.CloseButtonClick(null, null);
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
            if (this.highlightTimer.Enabled) this.highlightTimer.Stop();
            Hashtable table = new Hashtable();
            table["sci"] = sci;
            table["matches"] = matches;
            this.highlightTimer.Tag = table;
            this.highlightTimer.Start();
        }

        /// Gets search results for a sci control
        /// </summary>
        private List<SearchMatch> GetResults(ScintillaControl sci, String text)
        {
            String pattern = text;
            FRSearch search = new FRSearch(pattern);
            search.Filter = SearchFilter.None;
            search.NoCase = !this.matchCaseCheckBox.Checked;
            search.WholeWord = this.wholeWordCheckBox.Checked;
            search.SourceFile = sci.FileName;
            return search.Matches(sci.Text);
        }

        #endregion

        #region QuickFind Renderer

        public class QuickFindRenderer : ToolStripRenderer
        {
            private ToolStrip toolStrip;
            private ToolStripRenderer renderer;

            public QuickFindRenderer()
            {
                UiRenderMode renderMode = Globals.Settings.RenderMode;
                if (renderMode == UiRenderMode.System) this.renderer = new ToolStripSystemRenderer();
                else this.renderer = new DockPanelStripRenderer();
            }

            protected override void Initialize(ToolStrip toolStrip)
            {
                this.toolStrip = toolStrip;
                this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
                this.toolStrip.Paint += this.OnToolStripPaint;
                base.Initialize(toolStrip);
            }

            protected override void InitializeItem(ToolStripItem item)
            {
                base.InitializeItem(item);
                if (item is ToolStripButton)
                {
                    Double scale = ScaleHelper.GetScale();
                    if (scale >= 1.5)
                    {
                        item.Padding = new Padding(4, 2, 4, 2);
                    }
                    else if (scale >= 1.2)
                    {
                        item.Padding = new Padding(2, 1, 2, 1);
                    }
                    else if (renderer is ToolStripSystemRenderer && Win32.IsRunningOnWindows())
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

            private void OnToolStripPaint(Object sender, PaintEventArgs e)
            {
                Color tborder = Globals.MainForm.GetThemeColor("ToolStripTextBoxControl.BorderColor");
                foreach (ToolStripItem item in this.toolStrip.Items)
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
                    Boolean isOver = false;
                    Color back = Globals.MainForm.GetThemeColor("ToolStripItem.BackColor");
                    Color border = Globals.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                    Color active = Globals.MainForm.GetThemeColor("ToolStripMenu.DropDownBorderColor");
                    if (e.Item is ToolStripButton)
                    {
                        ToolStripButton button = e.Item as ToolStripButton;
                        Rectangle bBounds = button.Owner.RectangleToScreen(button.Bounds);
                        isOver = bBounds.Contains(Control.MousePosition);
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
                this.renderer.DrawGrip(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                this.renderer.DrawSeparator(e);
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                this.renderer.DrawToolStripBackground(e);
            }

            protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawDropDownButtonBackground(e);
            }

            protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawItemBackground(e);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                this.renderer.DrawItemCheck(e);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                this.renderer.DrawItemText(e);
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                this.renderer.DrawItemImage(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                this.renderer.DrawArrow(e);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                this.renderer.DrawImageMargin(e);
            }

            protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawLabelBackground(e);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawMenuItemBackground(e);
            }

            protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawOverflowButtonBackground(e);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawSplitButton(e);
            }

            protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
            {
                this.renderer.DrawStatusStripSizingGrip(e);
            }

            protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
            {
                this.renderer.DrawToolStripContentPanelBackground(e);
            }

            protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
            {
                this.renderer.DrawToolStripPanelBackground(e);
            }

            protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
            {
                this.renderer.DrawToolStripStatusLabelBackground(e);
            }

            #endregion

        }

        #endregion

        #region Custom Controls

        public delegate void KeyEscapeEvent();

        public class EscapeTextBox : ToolStripTextBox
        {
            public event KeyEscapeEvent OnKeyEscape;

            public EscapeTextBox() : base() 
            {
                this.Control.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            }

            protected override Boolean ProcessCmdKey(ref Message m, Keys keyData)
            {
                if (keyData == Keys.Escape) OnPressEscapeKey();
                return false;
            }

            private void OnPreviewKeyDown(Object sender, PreviewKeyDownEventArgs e)
            {
                Keys ctrlAlt = Keys.Control | Keys.Alt;
                if ((e.KeyData & ctrlAlt) == ctrlAlt) e.IsInputKey = true;
            }

            protected void OnPressEscapeKey()
            {
                if (OnKeyEscape != null) OnKeyEscape();
            }
        }

        #endregion

    }

}
