// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using FlashDevelop.Utilities;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Managers;
using PluginCore.Controls;
using ScintillaNet;

namespace FlashDevelop.Dialogs
{
    public class FRInDocDialog : SmartForm
    {
        Label infoLabel;
        Label findLabel;
        Label lookLabel;
        Label replaceLabel;
        CheckBox useRegexCheckBox;
        CheckBox escapedCheckBox;
        CheckBox matchCaseCheckBox;
        CheckBox wholeWordCheckBox;
        GroupBox optionsGroupBox;
        PictureBox infoPictureBox;
        ComboBox replaceComboBox;
        ComboBox lookComboBox;
        ComboBox findComboBox;
        Button replaceAllButton;
        Button bookmarkAllButton;
        Button findPrevButton;
        Button findNextButton;
        Button replaceButton;
        Button closeButton;
        bool lookupIsDirty;
        SearchMatch currentMatch;

        public FRInDocDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "24910809-a60a-4b7c-8d2a-d53a363f595f";
            InitializeComponent();
            InitializeProperties();
            ApplyLocalizedTexts();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent() 
        {
            replaceButton = new ButtonEx();
            findNextButton = new ButtonEx();
            wholeWordCheckBox = new CheckBoxEx();
            matchCaseCheckBox = new CheckBoxEx();
            closeButton = new ButtonEx();
            findPrevButton = new ButtonEx();
            findComboBox = new FlatCombo();
            findLabel = new Label();
            escapedCheckBox = new CheckBoxEx();
            useRegexCheckBox = new CheckBoxEx();
            infoPictureBox = new PictureBox();
            infoLabel = new Label();
            optionsGroupBox = new GroupBoxEx();
            replaceComboBox = new FlatCombo();
            replaceLabel = new Label();
            replaceAllButton = new ButtonEx();
            lookComboBox = new FlatCombo();
            lookLabel = new Label();
            bookmarkAllButton = new ButtonEx();
            ((System.ComponentModel.ISupportInitialize)(infoPictureBox)).BeginInit();
            optionsGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // replaceButton
            //
            replaceButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            replaceButton.FlatStyle = FlatStyle.System;
            replaceButton.Location = new Point(277, 112);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new Size(95, 23);
            replaceButton.TabIndex = 8;
            replaceButton.Text = "&Replace";
            replaceButton.Click += ReplaceButtonClick;
            // 
            // findNextButton
            // 
            findNextButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            findNextButton.FlatStyle = FlatStyle.System;
            findNextButton.Location = new Point(277, 22);
            findNextButton.Name = "findNextButton";
            findNextButton.Size = new Size(95, 23);
            findNextButton.TabIndex = 5;
            findNextButton.Text = "Find &Next";
            findNextButton.Click += FindNextButtonClick;
            // 
            // wholeWordCheckBox
            // 
            wholeWordCheckBox.AutoSize = true;
            wholeWordCheckBox.FlatStyle = FlatStyle.System;
            wholeWordCheckBox.Location = new Point(12, 18);
            wholeWordCheckBox.Name = "wholeWordCheckBox";
            wholeWordCheckBox.Size = new Size(92, 18);
            wholeWordCheckBox.TabIndex = 1;
            wholeWordCheckBox.Text = " &Whole word";
            wholeWordCheckBox.CheckedChanged += LookupChanged;
            // 
            // matchCaseCheckBox
            // 
            matchCaseCheckBox.AutoSize = true;
            matchCaseCheckBox.FlatStyle = FlatStyle.System;
            matchCaseCheckBox.Location = new Point(12, 40);
            matchCaseCheckBox.Name = "matchCaseCheckBox";
            matchCaseCheckBox.Size = new Size(89, 18);
            matchCaseCheckBox.TabIndex = 2;
            matchCaseCheckBox.Text = " Match &case";
            matchCaseCheckBox.CheckedChanged += LookupChanged;
            // 
            // closeButton
            //
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(277, 172);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(95, 23);
            closeButton.TabIndex = 10;
            closeButton.Text = "&Close";
            closeButton.Click += CloseButtonClick;
            // 
            // findPrevButton
            // 
            findPrevButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            findPrevButton.FlatStyle = FlatStyle.System;
            findPrevButton.Location = new Point(277, 52);
            findPrevButton.Name = "findPrevButton";
            findPrevButton.Size = new Size(95, 23);
            findPrevButton.TabIndex = 6;
            findPrevButton.Text = "Find &Previous";
            findPrevButton.Click += FindPrevButtonClick;
            // 
            // findComboBox
            //
            findComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            findComboBox.FlatStyle = FlatStyle.System;
            findComboBox.Location = new Point(13, 23);
            findComboBox.Name = "findComboBox";
            findComboBox.Size = new Size(252, 21);
            findComboBox.TabIndex = 1;
            findComboBox.TextChanged += LookupChanged;
            findComboBox.TextChanged += FindComboBoxTextChanged;
            // 
            // findLabel
            // 
            findLabel.AutoSize = true;
            findLabel.FlatStyle = FlatStyle.System;
            findLabel.Location = new Point(14, 8);
            findLabel.Name = "findLabel";
            findLabel.Size = new Size(58, 13);
            findLabel.TabIndex = 0;
            findLabel.Text = "Find what:";
            // 
            // escapedCheckBox
            // 
            escapedCheckBox.AutoSize = true;
            escapedCheckBox.FlatStyle = FlatStyle.System;
            escapedCheckBox.Location = new Point(115, 40);
            escapedCheckBox.Name = "escapedCheckBox";
            escapedCheckBox.Size = new Size(129, 18);
            escapedCheckBox.TabIndex = 4;
            escapedCheckBox.Text = " &Escaped characters";
            escapedCheckBox.CheckedChanged += LookupChanged;
            // 
            // useRegexCheckBox
            // 
            useRegexCheckBox.AutoSize = true;
            useRegexCheckBox.FlatStyle = FlatStyle.System;
            useRegexCheckBox.Location = new Point(115, 18);
            useRegexCheckBox.Name = "useRegexCheckBox";
            useRegexCheckBox.Size = new Size(132, 18);
            useRegexCheckBox.TabIndex = 3;
            useRegexCheckBox.Text = " &Regular expressions";
            useRegexCheckBox.CheckedChanged += LookupChanged;
            // 
            // infoPictureBox
            //
            infoPictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            infoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            infoPictureBox.Location = new Point(14, 202);
            infoPictureBox.Name = "infoPictureBox";
            infoPictureBox.Size = new Size(16, 16);
            infoPictureBox.TabIndex = 12;
            infoPictureBox.TabStop = false;
            // 
            // infoLabel
            //
            infoLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            infoLabel.AutoSize = true;
            infoLabel.BackColor = SystemColors.Control;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.ForeColor = SystemColors.ControlText;
            infoLabel.Location = new Point(36, 203);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(130, 13);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "No suitable results found.";
            // 
            // optionsGroupBox
            //
            optionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            optionsGroupBox.Controls.Add(useRegexCheckBox);
            optionsGroupBox.Controls.Add(escapedCheckBox);
            optionsGroupBox.Controls.Add(wholeWordCheckBox);
            optionsGroupBox.Controls.Add(matchCaseCheckBox);
            optionsGroupBox.FlatStyle = FlatStyle.System;
            optionsGroupBox.Location = new Point(13, 128);
            optionsGroupBox.Name = "optionsGroupBox";
            optionsGroupBox.Size = new Size(252, 67);
            optionsGroupBox.TabIndex = 4;
            optionsGroupBox.TabStop = false;
            optionsGroupBox.Text = " Options";
            // 
            // replaceComboBox
            //
            replaceComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            replaceComboBox.FlatStyle = FlatStyle.System;
            replaceComboBox.Location = new Point(13, 62);
            replaceComboBox.Name = "replaceComboBox";
            replaceComboBox.Size = new Size(252, 21);
            replaceComboBox.TabIndex = 2;
            replaceComboBox.TextChanged += LookupChanged;
            // 
            // replaceLabel
            // 
            replaceLabel.AutoSize = true;
            replaceLabel.FlatStyle = FlatStyle.System;
            replaceLabel.Location = new Point(14, 47);
            replaceLabel.Name = "replaceLabel";
            replaceLabel.Size = new Size(72, 13);
            replaceLabel.TabIndex = 16;
            replaceLabel.Text = "Replace with:";
            // 
            // replaceAllButton
            //
            replaceAllButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            replaceAllButton.FlatStyle = FlatStyle.System;
            replaceAllButton.Location = new Point(277, 142);
            replaceAllButton.Name = "replaceAllButton";
            replaceAllButton.Size = new Size(95, 23);
            replaceAllButton.TabIndex = 9;
            replaceAllButton.Text = "Replace &All";
            replaceAllButton.Click += ReplaceAllButtonClick;
            // 
            // lookComboBox
            //
            lookComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lookComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            lookComboBox.FlatStyle = FlatStyle.System;
            lookComboBox.Items.AddRange(new object[] {
            "Full source code",
            "Current selection",
            "Code and strings",
            "Comments only",
            "Strings only"});
            lookComboBox.Location = new Point(13, 101);
            lookComboBox.Name = "lookComboBox";
            lookComboBox.Size = new Size(252, 21);
            lookComboBox.TabIndex = 3;
            lookComboBox.TextChanged += LookupChanged;
            // 
            // lookLabel
            // 
            lookLabel.AutoSize = true;
            lookLabel.FlatStyle = FlatStyle.System;
            lookLabel.Location = new Point(14, 86);
            lookLabel.Name = "lookLabel";
            lookLabel.Size = new Size(44, 13);
            lookLabel.TabIndex = 19;
            lookLabel.Text = "Look in:";
            // 
            // bookmarkAllButton
            //
            bookmarkAllButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bookmarkAllButton.FlatStyle = FlatStyle.System;
            bookmarkAllButton.Location = new Point(277, 82);
            bookmarkAllButton.Name = "bookmarkAllButton";
            bookmarkAllButton.Size = new Size(95, 23);
            bookmarkAllButton.TabIndex = 7;
            bookmarkAllButton.Text = "&Bookmark All";
            bookmarkAllButton.Click += BookmarkAllButtonClick;
            // 
            // FRInDocDialog
            // 
            AcceptButton = findNextButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 228);
            MinimumSize = new Size(384, 268);
            //this.MaximumSize = new System.Drawing.Size(1000, 268);
            Controls.Add(bookmarkAllButton);
            Controls.Add(replaceComboBox);
            Controls.Add(lookComboBox);
            Controls.Add(lookLabel);
            Controls.Add(replaceAllButton);
            Controls.Add(replaceLabel);
            Controls.Add(optionsGroupBox);
            Controls.Add(findComboBox);
            Controls.Add(infoPictureBox);
            Controls.Add(infoLabel);
            Controls.Add(findPrevButton);
            Controls.Add(replaceButton);
            Controls.Add(closeButton);
            Controls.Add(findLabel);
            Controls.Add(findNextButton);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FRInDocDialog";
            ShowInTaskbar = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Find And Replace";
            Load += DialogLoad;
            VisibleChanged += VisibleChange;
            Closing += DialogClosing;
            ((System.ComponentModel.ISupportInitialize)(infoPictureBox)).EndInit();
            optionsGroupBox.ResumeLayout(false);
            optionsGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
        
        #endregion
        
        #region Methods And Event Handlers
        
        /// <summary>
        /// Inits the default properties
        /// </summary> 
        void InitializeProperties() => lookComboBox.SelectedIndex = 0;

        /// <summary>
        /// Applies the localized texts
        /// </summary> 
        void ApplyLocalizedTexts()
        {
            lookLabel.Text = TextHelper.GetString("Info.LookIn");
            findLabel.Text = TextHelper.GetString("Info.FindWhat");
            replaceLabel.Text = TextHelper.GetString("Info.ReplaceWith");
            findNextButton.Text = TextHelper.GetString("Label.FindNext");
            findPrevButton.Text = TextHelper.GetString("Label.FindPrevious");
            bookmarkAllButton.Text = TextHelper.GetString("Label.BookmarkAll");
            replaceAllButton.Text = TextHelper.GetString("Label.ReplaceAll");
            replaceButton.Text = TextHelper.GetStringWithoutEllipsis("Label.Replace");
            closeButton.Text = TextHelper.GetStringWithoutMnemonics("Label.Close");
            lookComboBox.Items[0] = TextHelper.GetString("Info.FullSourceCode");
            lookComboBox.Items[1] = TextHelper.GetString("Info.CurrentSelection");
            lookComboBox.Items[2] = TextHelper.GetString("Info.CodeAndStrings");
            lookComboBox.Items[3] = TextHelper.GetString("Info.CommentsOnly");
            lookComboBox.Items[4] = TextHelper.GetString("Info.StringsOnly");
            optionsGroupBox.Text = " " + TextHelper.GetString("Label.Options");
            matchCaseCheckBox.Text = " " + TextHelper.GetString("Label.MatchCase");
            wholeWordCheckBox.Text = " " + TextHelper.GetString("Label.WholeWord");
            escapedCheckBox.Text = " " + TextHelper.GetString("Label.EscapedCharacters");
            useRegexCheckBox.Text = " " + TextHelper.GetString("Label.RegularExpressions");
            Text = " " + TextHelper.GetString("Title.FindAndReplaceDialog");
            lookComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
        }

        /// <summary>
        /// Set the case of the text to search
        /// </summary>
        public void SetMatchCase(bool matchCase)
        {
            matchCaseCheckBox.CheckedChanged -= LookupChanged;
            matchCaseCheckBox.Checked = matchCase; // Change the value...
            matchCaseCheckBox.CheckedChanged += LookupChanged;
        }

        /// <summary>
        /// Set the whole word prop of the text to search
        /// </summary>
        public void SetWholeWord(bool wholeWord)
        {
            wholeWordCheckBox.CheckedChanged -= LookupChanged;
            wholeWordCheckBox.Checked = wholeWord; // Change the value...
            wholeWordCheckBox.CheckedChanged += LookupChanged;
        }

        /// <summary>
        /// Set the text to search
        /// </summary>
        public void SetFindText(string text)
        {
            findComboBox.TextChanged -= FindComboBoxTextChanged;
            findComboBox.Text = text; // Change the value...
            findComboBox.TextChanged += FindComboBoxTextChanged;
        }

        /// <summary>
        /// When the text changes, synchronizes the find controls
        /// </summary>
        void FindComboBoxTextChanged(object sender, EventArgs e) => Globals.MainForm.SetFindText(this, findComboBox.Text);

        /// <summary>
        /// If there is a word selected, insert it to the find box
        /// </summary>
        void UpdateFindText()
        {
            if (useRegexCheckBox.Checked || lookupIsDirty) return;
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null || sci.SelTextSize == 0) return;
            findComboBox.Text = sci.SelText;
            lookupIsDirty = false;
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box. This will always update the text regardless of any settings.
        /// </summary>
        public void InitializeFindText()
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null || sci.SelTextSize == 0) return;
            findComboBox.Text = sci.SelText;
            lookupIsDirty = false;
        }

        public void FindNext(bool forward) => FindNext(forward, true);

        public void FindNext(bool forward, bool update) => FindNext(forward, update, false);

        public void FindNext(bool forward, bool update, bool simple) => FindNext(forward, update, simple, false);

        /// <summary>
        /// Finds the next result based on direction
        /// </summary>
        public void FindNext(bool forward, bool update, bool simple, bool fixedPosition)
        {
            currentMatch = null;
            if (update) UpdateFindText();
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            var matches = GetResults(sci, simple);
            if (!matches.IsNullOrEmpty())
            {
                FRDialogGenerics.UpdateComboBoxItems(findComboBox);
                var match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, forward, fixedPosition);
                if (match != null)
                {
                    currentMatch = match;
                    FRDialogGenerics.SelectMatch(sci, match);
                    lookupIsDirty = false;
                }
                if (Visible)
                {
                    var index = FRDialogGenerics.GetMatchIndex(match, matches);
                    var message = TextHelper.GetString("Info.ShowingResult");
                    var formatted = string.Format(message, index, matches.Count);
                    ShowMessage(formatted, 0);
                }
            }
            else if (Visible)
            {
                var message = TextHelper.GetString("Info.NoMatchesFound");
                ShowMessage(message, 0);
            }
            SelectText();
        }

        /// <summary>
        /// Finds the next result specified by user input
        /// </summary>
        void FindNextButtonClick(object sender, EventArgs e) => FindNext(true, false);

        /// <summary>
        /// Finds the previous result specified by user input
        /// </summary>
        void FindPrevButtonClick(object sender, EventArgs e) => FindNext(false, false);

        /// <summary>
        /// Bookmarks all results
        /// </summary>
        void BookmarkAllButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            var matches = GetResults(sci);
            if (matches != null && lookComboBox.SelectedIndex == 1 && sci.SelTextSize > 0)
            {
                var end = sci.MBSafeCharPosition(sci.SelectionEnd);
                var start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (!matches.IsNullOrEmpty())
            {
                FRDialogGenerics.BookmarkMatches(sci, matches);
                ShowMessage(TextHelper.GetString("Info.MatchesBookmarked"), 0);
            }
            else ShowMessage(TextHelper.GetString("Info.NothingToBookmark"), 0);
        }

        /// <summary>
        /// Replaces the next result specified by user input
        /// </summary>
        void ReplaceButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            if (sci.SelTextSize == 0)
            {
                FindNext(true);
                return;
            }
            if (useRegexCheckBox.Enabled && currentMatch is null)
            {
                FindNext(true, false, false, true);
                if (currentMatch is null) return;
            }
            var replaceWith = GetReplaceText(currentMatch);
            sci.ReplaceSel(replaceWith);
            FRDialogGenerics.UpdateComboBoxItems(findComboBox);
            FRDialogGenerics.UpdateComboBoxItems(replaceComboBox);
            ShowMessage(TextHelper.GetString("Info.SelectedReplaced"), 0);
            lookupIsDirty = false;
            FindNext(true);
        }

        /// <summary>
        /// Replaces all results specified by user input
        /// </summary>
        void ReplaceAllButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            var matches = GetResults(sci);
            var selectionOnly = lookComboBox.SelectedIndex == 1 && sci.SelTextSize > 0;
            if (matches != null && selectionOnly)
            {
                var end = sci.MBSafeCharPosition(sci.SelectionEnd);
                var start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (matches is null) return;
            sci.BeginUndoAction();
            try
            {
                var firstVisibleLine = sci.FirstVisibleLine;
                var pos = sci.CurrentPos;
                for (int i = 0, count = matches.Count; i < count; i++)
                {
                    var match = matches[i];
                    var replacement = GetReplaceText(match);
                    var replacementLength = sci.MBSafeTextLength(replacement);
                    if (sci.MBSafePosition(match.Index) < pos) pos += replacementLength - sci.MBSafeTextLength(match.Value);
                    if (selectionOnly) FRDialogGenerics.SelectMatchInTarget(sci, match);
                    else FRDialogGenerics.SelectMatch(sci, match);
                    FRSearch.PadIndexes(matches, i, match.Value, replacement);
                    sci.EnsureVisible(sci.CurrentLine);
                    if (selectionOnly) sci.ReplaceTarget(replacementLength, replacement);
                    else sci.ReplaceSel(replacement);
                }
                sci.FirstVisibleLine = firstVisibleLine;
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.EndUndoAction();
            }
            FRDialogGenerics.UpdateComboBoxItems(findComboBox);
            FRDialogGenerics.UpdateComboBoxItems(replaceComboBox);
            var message = TextHelper.GetString("Info.ReplacedMatches");
            var formatted = string.Format(message, matches.Count);
            ShowMessage(formatted, 0);
            lookupIsDirty = false;
        }

        /// <summary>
        /// Closes the find and replace dialog
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Marks the lookup as dirty
        /// </summary>
        void LookupChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                lookupIsDirty = true;
                currentMatch = null;
            }
            if (sender == matchCaseCheckBox && !PluginBase.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetMatchCase(this, matchCaseCheckBox.Checked);
            }
            if (sender == wholeWordCheckBox && !PluginBase.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetWholeWord(this, wholeWordCheckBox.Checked);
            }
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        void VisibleChange(object sender, EventArgs e)
        {
            if (Visible) SelectText();
            else lookupIsDirty = false;
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            PluginBase.MainForm.CurrentDocument?.Activate();
            Hide();
        }

        /// <summary>
        /// Setups the dialog on load
        /// </summary>
        void DialogLoad(object sender, EventArgs e)
        {
            ShowMessage(TextHelper.GetString("Info.NoMatches"), 0);
            CenterToParent();
        }

        /// <summary>
        /// Process shortcuts
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                findComboBox.Focus();
                return true;
            }
            if ((keyData & Keys.KeyCode) == Keys.Enter && (keyData & Keys.Shift) > 0)
            {
                FindNext(false, false);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Gets the replacement text and escapes it if needed
        /// </summary>
        string GetReplaceText(SearchMatch match)
        {
            var replace = replaceComboBox.Text;
            if (escapedCheckBox.Checked) replace = FRSearch.Unescape(replace);
            if (useRegexCheckBox.Checked) replace = FRSearch.ExpandGroups(replace, match);
            return replace;
        }

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        List<SearchMatch> GetResults(ScintillaControl sci, bool simple)
        {
            if (!IsValidPattern()) return null;
            var pattern = findComboBox.Text;
            var search = new FRSearch(pattern)
            {
                NoCase = !matchCaseCheckBox.Checked,
                Filter = SearchFilter.None,
                SourceFile = sci.FileName
            };
            if (!simple)
            {
                search.IsRegex = useRegexCheckBox.Checked;
                search.IsEscaped = escapedCheckBox.Checked;
                search.WholeWord = wholeWordCheckBox.Checked;
                search.Filter = lookComboBox.SelectedIndex switch
                {
                    2 => SearchFilter.OutsideCodeComments,
                    3 => SearchFilter.InCodeComments | SearchFilter.OutsideStringLiterals,
                    4 => SearchFilter.InStringLiterals | SearchFilter.OutsideCodeComments,
                    _ => search.Filter
                };
            }
            return search.Matches(sci.Text);
        }

        List<SearchMatch> GetResults(ScintillaControl sci) => GetResults(sci, false);

        /// <summary>
        /// Control user pattern
        /// </summary>
        bool IsValidPattern()
        {
            var pattern = findComboBox.Text;
            if (pattern.Length == 0) return false;
            if (useRegexCheckBox.Checked)
            {
                try
                {
                    new Regex(pattern);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowInfo(ex.Message);
                    Select();
                    findComboBox.SelectAll();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shows the info message in dialog
        /// </summary>
        void ShowMessage(string msg, int img)
        {
            infoPictureBox.Image = FRDialogGenerics.GetImage(img);
            infoLabel.Text = msg;
        }

        /// <summary>
        /// Selects the text in the textfield.
        /// </summary>
        void SelectText()
        {
            if (findComboBox.Text.Length == 0)
            {
                findComboBox.Select();
                findComboBox.SelectAll();
            }
            else
            {
                replaceComboBox.Select();
                replaceComboBox.SelectAll();
            }
        }

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            base.Show();
            lookupIsDirty = false;
            InitializeFindText();
        }

        #endregion
    }
}