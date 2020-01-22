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
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label findLabel;
        private System.Windows.Forms.Label lookLabel;
        private System.Windows.Forms.Label replaceLabel;
        private System.Windows.Forms.CheckBox useRegexCheckBox;
        private System.Windows.Forms.CheckBox escapedCheckBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.CheckBox wholeWordCheckBox;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.PictureBox infoPictureBox;
        private System.Windows.Forms.ComboBox replaceComboBox;
        private System.Windows.Forms.ComboBox lookComboBox;
        private System.Windows.Forms.ComboBox findComboBox;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Button bookmarkAllButton;
        private System.Windows.Forms.Button findPrevButton;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button closeButton;
        private bool lookupIsDirty = false;
        private SearchMatch currentMatch = null;

        public FRInDocDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "24910809-a60a-4b7c-8d2a-d53a363f595f";
            this.InitializeComponent();
            this.InitializeProperties();
            this.ApplyLocalizedTexts();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.replaceButton = new System.Windows.Forms.ButtonEx();
            this.findNextButton = new System.Windows.Forms.ButtonEx();
            this.wholeWordCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.findPrevButton = new System.Windows.Forms.ButtonEx();
            this.findComboBox = new System.Windows.Forms.FlatCombo();
            this.findLabel = new System.Windows.Forms.Label();
            this.escapedCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.useRegexCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.infoPictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.optionsGroupBox = new System.Windows.Forms.GroupBoxEx();
            this.replaceComboBox = new System.Windows.Forms.FlatCombo();
            this.replaceLabel = new System.Windows.Forms.Label();
            this.replaceAllButton = new System.Windows.Forms.ButtonEx();
            this.lookComboBox = new System.Windows.Forms.FlatCombo();
            this.lookLabel = new System.Windows.Forms.Label();
            this.bookmarkAllButton = new System.Windows.Forms.ButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).BeginInit();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // replaceButton
            //
            this.replaceButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.replaceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceButton.Location = new System.Drawing.Point(277, 112);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(95, 23);
            this.replaceButton.TabIndex = 8;
            this.replaceButton.Text = "&Replace";
            this.replaceButton.Click += this.ReplaceButtonClick;
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.findNextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findNextButton.Location = new System.Drawing.Point(277, 22);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(95, 23);
            this.findNextButton.TabIndex = 5;
            this.findNextButton.Text = "Find &Next";
            this.findNextButton.Click += this.FindNextButtonClick;
            // 
            // wholeWordCheckBox
            // 
            this.wholeWordCheckBox.AutoSize = true;
            this.wholeWordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.wholeWordCheckBox.Location = new System.Drawing.Point(12, 18);
            this.wholeWordCheckBox.Name = "wholeWordCheckBox";
            this.wholeWordCheckBox.Size = new System.Drawing.Size(92, 18);
            this.wholeWordCheckBox.TabIndex = 1;
            this.wholeWordCheckBox.Text = " &Whole word";
            this.wholeWordCheckBox.CheckedChanged += this.LookupChanged;
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(12, 40);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(89, 18);
            this.matchCaseCheckBox.TabIndex = 2;
            this.matchCaseCheckBox.Text = " Match &case";
            this.matchCaseCheckBox.CheckedChanged += this.LookupChanged;
            // 
            // closeButton
            //
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(277, 172);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "&Close";
            this.closeButton.Click += this.CloseButtonClick;
            // 
            // findPrevButton
            // 
            this.findPrevButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.findPrevButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findPrevButton.Location = new System.Drawing.Point(277, 52);
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Size = new System.Drawing.Size(95, 23);
            this.findPrevButton.TabIndex = 6;
            this.findPrevButton.Text = "Find &Previous";
            this.findPrevButton.Click += this.FindPrevButtonClick;
            // 
            // findComboBox
            //
            this.findComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.findComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findComboBox.Location = new System.Drawing.Point(13, 23);
            this.findComboBox.Name = "findComboBox";
            this.findComboBox.Size = new System.Drawing.Size(252, 21);
            this.findComboBox.TabIndex = 1;
            this.findComboBox.TextChanged += this.LookupChanged;
            this.findComboBox.TextChanged += this.FindComboBoxTextChanged;
            // 
            // findLabel
            // 
            this.findLabel.AutoSize = true;
            this.findLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findLabel.Location = new System.Drawing.Point(14, 8);
            this.findLabel.Name = "findLabel";
            this.findLabel.Size = new System.Drawing.Size(58, 13);
            this.findLabel.TabIndex = 0;
            this.findLabel.Text = "Find what:";
            // 
            // escapedCheckBox
            // 
            this.escapedCheckBox.AutoSize = true;
            this.escapedCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.escapedCheckBox.Location = new System.Drawing.Point(115, 40);
            this.escapedCheckBox.Name = "escapedCheckBox";
            this.escapedCheckBox.Size = new System.Drawing.Size(129, 18);
            this.escapedCheckBox.TabIndex = 4;
            this.escapedCheckBox.Text = " &Escaped characters";
            this.escapedCheckBox.CheckedChanged += this.LookupChanged;
            // 
            // useRegexCheckBox
            // 
            this.useRegexCheckBox.AutoSize = true;
            this.useRegexCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.useRegexCheckBox.Location = new System.Drawing.Point(115, 18);
            this.useRegexCheckBox.Name = "useRegexCheckBox";
            this.useRegexCheckBox.Size = new System.Drawing.Size(132, 18);
            this.useRegexCheckBox.TabIndex = 3;
            this.useRegexCheckBox.Text = " &Regular expressions";
            this.useRegexCheckBox.CheckedChanged += this.LookupChanged;
            // 
            // infoPictureBox
            //
            this.infoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.infoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.infoPictureBox.Location = new System.Drawing.Point(14, 202);
            this.infoPictureBox.Name = "infoPictureBox";
            this.infoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.infoPictureBox.TabIndex = 12;
            this.infoPictureBox.TabStop = false;
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.infoLabel.AutoSize = true;
            this.infoLabel.BackColor = System.Drawing.SystemColors.Control;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.infoLabel.Location = new System.Drawing.Point(36, 203);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(130, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "No suitable results found.";
            // 
            // optionsGroupBox
            //
            this.optionsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.optionsGroupBox.Controls.Add(this.useRegexCheckBox);
            this.optionsGroupBox.Controls.Add(this.escapedCheckBox);
            this.optionsGroupBox.Controls.Add(this.wholeWordCheckBox);
            this.optionsGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.optionsGroupBox.Location = new System.Drawing.Point(13, 128);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(252, 67);
            this.optionsGroupBox.TabIndex = 4;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = " Options";
            // 
            // replaceComboBox
            //
            this.replaceComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.replaceComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceComboBox.Location = new System.Drawing.Point(13, 62);
            this.replaceComboBox.Name = "replaceComboBox";
            this.replaceComboBox.Size = new System.Drawing.Size(252, 21);
            this.replaceComboBox.TabIndex = 2;
            this.replaceComboBox.TextChanged += this.LookupChanged;
            // 
            // replaceLabel
            // 
            this.replaceLabel.AutoSize = true;
            this.replaceLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceLabel.Location = new System.Drawing.Point(14, 47);
            this.replaceLabel.Name = "replaceLabel";
            this.replaceLabel.Size = new System.Drawing.Size(72, 13);
            this.replaceLabel.TabIndex = 16;
            this.replaceLabel.Text = "Replace with:";
            // 
            // replaceAllButton
            //
            this.replaceAllButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.replaceAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceAllButton.Location = new System.Drawing.Point(277, 142);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(95, 23);
            this.replaceAllButton.TabIndex = 9;
            this.replaceAllButton.Text = "Replace &All";
            this.replaceAllButton.Click += this.ReplaceAllButtonClick;
            // 
            // lookComboBox
            //
            this.lookComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.lookComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lookComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lookComboBox.Items.AddRange(new object[] {
            "Full source code",
            "Current selection",
            "Code and strings",
            "Comments only",
            "Strings only"});
            this.lookComboBox.Location = new System.Drawing.Point(13, 101);
            this.lookComboBox.Name = "lookComboBox";
            this.lookComboBox.Size = new System.Drawing.Size(252, 21);
            this.lookComboBox.TabIndex = 3;
            this.lookComboBox.TextChanged += this.LookupChanged;
            // 
            // lookLabel
            // 
            this.lookLabel.AutoSize = true;
            this.lookLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lookLabel.Location = new System.Drawing.Point(14, 86);
            this.lookLabel.Name = "lookLabel";
            this.lookLabel.Size = new System.Drawing.Size(44, 13);
            this.lookLabel.TabIndex = 19;
            this.lookLabel.Text = "Look in:";
            // 
            // bookmarkAllButton
            //
            this.bookmarkAllButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.bookmarkAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bookmarkAllButton.Location = new System.Drawing.Point(277, 82);
            this.bookmarkAllButton.Name = "bookmarkAllButton";
            this.bookmarkAllButton.Size = new System.Drawing.Size(95, 23);
            this.bookmarkAllButton.TabIndex = 7;
            this.bookmarkAllButton.Text = "&Bookmark All";
            this.bookmarkAllButton.Click += this.BookmarkAllButtonClick;
            // 
            // FRInDocDialog
            // 
            this.AcceptButton = this.findNextButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 228);
            this.MinimumSize = new System.Drawing.Size(384, 268);
            //this.MaximumSize = new System.Drawing.Size(1000, 268);
            this.Controls.Add(this.bookmarkAllButton);
            this.Controls.Add(this.replaceComboBox);
            this.Controls.Add(this.lookComboBox);
            this.Controls.Add(this.lookLabel);
            this.Controls.Add(this.replaceAllButton);
            this.Controls.Add(this.replaceLabel);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.findComboBox);
            this.Controls.Add(this.infoPictureBox);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.findPrevButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.findLabel);
            this.Controls.Add(this.findNextButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FRInDocDialog";
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Find And Replace";
            this.Load += this.DialogLoad;
            this.VisibleChanged += this.VisibleChange;
            this.Closing += this.DialogClosing;
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).EndInit();
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion
        
        #region Methods And Event Handlers
        
        /// <summary>
        /// Inits the default properties
        /// </summary> 
        private void InitializeProperties()
        {
            this.lookComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Applies the localized texts
        /// </summary> 
        private void ApplyLocalizedTexts()
        {
            this.lookLabel.Text = TextHelper.GetString("Info.LookIn");
            this.findLabel.Text = TextHelper.GetString("Info.FindWhat");
            this.replaceLabel.Text = TextHelper.GetString("Info.ReplaceWith");
            this.findNextButton.Text = TextHelper.GetString("Label.FindNext");
            this.findPrevButton.Text = TextHelper.GetString("Label.FindPrevious");
            this.bookmarkAllButton.Text = TextHelper.GetString("Label.BookmarkAll");
            this.replaceAllButton.Text = TextHelper.GetString("Label.ReplaceAll");
            this.replaceButton.Text = TextHelper.GetStringWithoutEllipsis("Label.Replace");
            this.closeButton.Text = TextHelper.GetStringWithoutMnemonics("Label.Close");
            this.lookComboBox.Items[0] = TextHelper.GetString("Info.FullSourceCode");
            this.lookComboBox.Items[1] = TextHelper.GetString("Info.CurrentSelection");
            this.lookComboBox.Items[2] = TextHelper.GetString("Info.CodeAndStrings");
            this.lookComboBox.Items[3] = TextHelper.GetString("Info.CommentsOnly");
            this.lookComboBox.Items[4] = TextHelper.GetString("Info.StringsOnly");
            this.optionsGroupBox.Text = " " + TextHelper.GetString("Label.Options");
            this.matchCaseCheckBox.Text = " " + TextHelper.GetString("Label.MatchCase");
            this.wholeWordCheckBox.Text = " " + TextHelper.GetString("Label.WholeWord");
            this.escapedCheckBox.Text = " " + TextHelper.GetString("Label.EscapedCharacters");
            this.useRegexCheckBox.Text = " " + TextHelper.GetString("Label.RegularExpressions");
            this.Text = " " + TextHelper.GetString("Title.FindAndReplaceDialog");
            this.lookComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
        }

        /// <summary>
        /// Set the case of the text to search
        /// </summary>
        public void SetMatchCase(bool matchCase)
        {
            this.matchCaseCheckBox.CheckedChanged -= this.LookupChanged;
            this.matchCaseCheckBox.Checked = matchCase; // Change the value...
            this.matchCaseCheckBox.CheckedChanged += this.LookupChanged;
        }

        /// <summary>
        /// Set the whole word prop of the text to search
        /// </summary>
        public void SetWholeWord(bool wholeWord)
        {
            this.wholeWordCheckBox.CheckedChanged -= this.LookupChanged;
            this.wholeWordCheckBox.Checked = wholeWord; // Change the value...
            this.wholeWordCheckBox.CheckedChanged += this.LookupChanged;
        }

        /// <summary>
        /// Set the text to search
        /// </summary>
        public void SetFindText(string text)
        {
            this.findComboBox.TextChanged -= this.FindComboBoxTextChanged;
            this.findComboBox.Text = text; // Change the value...
            this.findComboBox.TextChanged += this.FindComboBoxTextChanged;
        }

        /// <summary>
        /// When the text changes, synchronizes the find controls
        /// </summary>
        private void FindComboBoxTextChanged(object sender, EventArgs e)
        {
            Globals.MainForm.SetFindText(this, this.findComboBox.Text);
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box
        /// </summary>
        private void UpdateFindText()
        {
            if (this.useRegexCheckBox.Checked) return;
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci != null && sci.SelText.Length > 0 && !this.lookupIsDirty)
            {
                this.findComboBox.Text = sci.SelText;
                this.lookupIsDirty = false;
            }
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box. This will always update the text regardless of any settings.
        /// </summary>
        public void InitializeFindText()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci != null && sci.SelText.Length > 0)
            {
                this.findComboBox.Text = sci.SelText;
                this.lookupIsDirty = false;
            }
        }

        public void FindNext(bool forward) => FindNext(forward, true);

        public void FindNext(bool forward, bool update) => FindNext(forward, update, false);

        public void FindNext(bool forward, bool update, bool simple) => FindNext(forward, update, simple, false);

        /// <summary>
        /// Finds the next result based on direction
        /// </summary>
        public void FindNext(bool forward, bool update, bool simple, bool fixedPosition)
        {
            this.currentMatch = null;
            if (update) this.UpdateFindText();
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            var matches = this.GetResults(sci, simple);
            if (!matches.IsNullOrEmpty())
            {
                FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                var match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, forward, fixedPosition);
                if (match != null)
                {
                    this.currentMatch = match;
                    FRDialogGenerics.SelectMatch(sci, match);
                    this.lookupIsDirty = false;
                }
                if (this.Visible)
                {
                    int index = FRDialogGenerics.GetMatchIndex(match, matches);
                    string message = TextHelper.GetString("Info.ShowingResult");
                    string formatted = string.Format(message, index, matches.Count);
                    this.ShowMessage(formatted, 0);
                }
            }
            else
            {
                if (this.Visible)
                {
                    string message = TextHelper.GetString("Info.NoMatchesFound");
                    this.ShowMessage(message, 0);
                }
            }
            this.SelectText();
        }

        /// <summary>
        /// Finds the next result specified by user input
        /// </summary>
        private void FindNextButtonClick(object sender, EventArgs e) => FindNext(true, false);

        /// <summary>
        /// Finds the previous result specified by user input
        /// </summary>
        private void FindPrevButtonClick(object sender, EventArgs e) => FindNext(false, false);

        /// <summary>
        /// Bookmarks all results
        /// </summary>
        private void BookmarkAllButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            List<SearchMatch> matches = this.GetResults(sci);
            if (matches != null && this.lookComboBox.SelectedIndex == 1 && sci.SelText.Length > 0)
            {
                int end = sci.MBSafeCharPosition(sci.SelectionEnd);
                int start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (!matches.IsNullOrEmpty())
            {
                FRDialogGenerics.BookmarkMatches(sci, matches);
                string message = TextHelper.GetString("Info.MatchesBookmarked");
                this.ShowMessage(message, 0);
            }
            else
            {
                string message = TextHelper.GetString("Info.NothingToBookmark");
                this.ShowMessage(message, 0);
            }
        }

        /// <summary>
        /// Replaces the next result specified by user input
        /// </summary>
        private void ReplaceButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            if (sci.SelText.Length == 0)
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
            var message = TextHelper.GetString("Info.SelectedReplaced");
            ShowMessage(message, 0);
            lookupIsDirty = false;
            FindNext(true);
        }

        /// <summary>
        /// Replaces all results specified by user input
        /// </summary>
        private void ReplaceAllButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            List<SearchMatch> matches = this.GetResults(sci);
            bool selectionOnly = this.lookComboBox.SelectedIndex == 1 && sci.SelText.Length > 0;
            if (matches != null && selectionOnly)
            {
                int end = sci.MBSafeCharPosition(sci.SelectionEnd);
                int start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (matches != null)
            {
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
                FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                FRDialogGenerics.UpdateComboBoxItems(this.replaceComboBox);
                string message = TextHelper.GetString("Info.ReplacedMatches");
                string formatted = string.Format(message, matches.Count);
                this.ShowMessage(formatted, 0);
                this.lookupIsDirty = false;
            }
        }

        /// <summary>
        /// Closes the find and replace dialog
        /// </summary>
        private void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Marks the lookup as dirty
        /// </summary>
        private void LookupChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.lookupIsDirty = true;
                this.currentMatch = null;
            }
            if (sender == this.matchCaseCheckBox && !PluginBase.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetMatchCase(this, this.matchCaseCheckBox.Checked);
            }
            if (sender == this.wholeWordCheckBox && !PluginBase.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetWholeWord(this, this.wholeWordCheckBox.Checked);
            }
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        private void VisibleChange(object sender, EventArgs e)
        {
            if (this.Visible) this.SelectText();
            else this.lookupIsDirty = false;
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        private void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            PluginBase.MainForm.CurrentDocument.Activate();
            this.Hide();
        }

        /// <summary>
        /// Setups the dialog on load
        /// </summary>
        private void DialogLoad(object sender, EventArgs e)
        {
            string message = TextHelper.GetString("Info.NoMatches");
            this.ShowMessage(message, 0);
            this.CenterToParent();
        }

        /// <summary>
        /// Process shortcuts
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                this.findComboBox.Focus();
                return true;
            }

            if ((keyData & Keys.KeyCode) == Keys.Enter && (keyData & Keys.Shift) > 0)
            {
                this.FindNext(false, false);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Gets the replacement text and escapes it if needed
        /// </summary>
        private string GetReplaceText(SearchMatch match)
        {
            string replace = this.replaceComboBox.Text;
            if (this.escapedCheckBox.Checked) replace = FRSearch.Unescape(replace);
            if (this.useRegexCheckBox.Checked) replace = FRSearch.ExpandGroups(replace, match);
            return replace;
        }

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        private List<SearchMatch> GetResults(ScintillaControl sci, bool simple)
        {
            if (IsValidPattern())
            {
                string pattern = this.findComboBox.Text;
                FRSearch search = new FRSearch(pattern);
                search.NoCase = !this.matchCaseCheckBox.Checked;
                search.Filter = SearchFilter.None;
                search.SourceFile = sci.FileName;
                if (!simple)
                {
                    search.IsRegex = this.useRegexCheckBox.Checked;
                    search.IsEscaped = this.escapedCheckBox.Checked;
                    search.WholeWord = this.wholeWordCheckBox.Checked;
                    if (this.lookComboBox.SelectedIndex == 2)
                    {
                        search.Filter = SearchFilter.OutsideCodeComments;
                    }
                    else if (this.lookComboBox.SelectedIndex == 3)
                    {
                        search.Filter = SearchFilter.InCodeComments | SearchFilter.OutsideStringLiterals;
                    }
                    else if (this.lookComboBox.SelectedIndex == 4)
                    {
                        search.Filter = SearchFilter.InStringLiterals | SearchFilter.OutsideCodeComments;
                    }
                }
                return search.Matches(sci.Text);
            }
            return null;
        }
        private List<SearchMatch> GetResults(ScintillaControl sci)
        {
            return this.GetResults(sci, false);
        }

        /// <summary>
        /// Control user pattern
        /// </summary>
        private bool IsValidPattern()
        {
            string pattern = this.findComboBox.Text;
            if (pattern.Length == 0) return false;
            if (this.useRegexCheckBox.Checked)
            {
                try
                {
                    new Regex(pattern);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowInfo(ex.Message);
                    this.Select();
                    this.findComboBox.SelectAll();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shows the info message in dialog
        /// </summary>
        private void ShowMessage(string msg, int img)
        {
            Image image = FRDialogGenerics.GetImage(img);
            this.infoPictureBox.Image = image;
            this.infoLabel.Text = msg;
        }

        /// <summary>
        /// Selects the text in the textfield.
        /// </summary>
        private void SelectText()
        {
            if (this.findComboBox.Text.Length == 0)
            {
                this.findComboBox.Select();
                this.findComboBox.SelectAll();
            }
            else
            {
                this.replaceComboBox.Select();
                this.replaceComboBox.SelectAll();
            }
        }

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            base.Show();
            this.lookupIsDirty = false;
            this.InitializeFindText();
        }

        #endregion
    }
}