using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using FlashDevelop.Utilities;
using FlashDevelop.Helpers;
using PluginCore.FRService;
using PluginCore.Managers;
using PluginCore.Controls;
using PluginCore.Helpers;
using ScintillaNet;
using PluginCore;

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
        private System.Boolean lookupIsDirty = false;
        private SearchMatch currentMatch = null;

        public FRInDocDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "24910809-a60a-4b7c-8d2a-d53a363f595f";
            this.InitializeComponent();
            this.InitializeProperties();
            this.ApplyLocalizedTexts();
            ScaleHelper.AdjustForHighDPI(this);
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.replaceButton = new System.Windows.Forms.Button();
            this.findNextButton = new System.Windows.Forms.Button();
            this.wholeWordCheckBox = new System.Windows.Forms.CheckBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.findPrevButton = new System.Windows.Forms.Button();
            this.findComboBox = new System.Windows.Forms.ComboBox();
            this.findLabel = new System.Windows.Forms.Label();
            this.escapedCheckBox = new System.Windows.Forms.CheckBox();
            this.useRegexCheckBox = new System.Windows.Forms.CheckBox();
            this.infoPictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.replaceComboBox = new System.Windows.Forms.ComboBox();
            this.replaceLabel = new System.Windows.Forms.Label();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.lookComboBox = new System.Windows.Forms.ComboBox();
            this.lookLabel = new System.Windows.Forms.Label();
            this.bookmarkAllButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).BeginInit();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // replaceButton
            //
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceButton.Location = new System.Drawing.Point(277, 112);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(95, 23);
            this.replaceButton.TabIndex = 8;
            this.replaceButton.Text = "&Replace";
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButtonClick);
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findNextButton.Location = new System.Drawing.Point(277, 22);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(95, 23);
            this.findNextButton.TabIndex = 5;
            this.findNextButton.Text = "Find &Next";
            this.findNextButton.Click += new System.EventHandler(this.FindNextButtonClick);
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
            this.wholeWordCheckBox.CheckedChanged += new System.EventHandler(this.LookupChanged);
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
            this.matchCaseCheckBox.CheckedChanged += new System.EventHandler(this.LookupChanged);
            // 
            // closeButton
            //
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(277, 172);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "&Close";
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // findPrevButton
            // 
            this.findPrevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findPrevButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findPrevButton.Location = new System.Drawing.Point(277, 52);
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Size = new System.Drawing.Size(95, 23);
            this.findPrevButton.TabIndex = 6;
            this.findPrevButton.Text = "Find &Previous";
            this.findPrevButton.Click += new System.EventHandler(this.FindPrevButtonClick);
            // 
            // findComboBox
            //
            this.findComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.findComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findComboBox.Location = new System.Drawing.Point(13, 23);
            this.findComboBox.Name = "findComboBox";
            this.findComboBox.Size = new System.Drawing.Size(252, 21);
            this.findComboBox.TabIndex = 1;
            this.findComboBox.TextChanged += new System.EventHandler(this.LookupChanged);
            this.findComboBox.TextChanged += new EventHandler(this.FindComboBoxTextChanged);
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
            this.escapedCheckBox.CheckedChanged += new System.EventHandler(this.LookupChanged);
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
            this.useRegexCheckBox.CheckedChanged += new System.EventHandler(this.LookupChanged);
            // 
            // infoPictureBox
            //
            this.infoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.infoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.infoPictureBox.Location = new System.Drawing.Point(14, 202);
            this.infoPictureBox.Name = "infoPictureBox";
            this.infoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.infoPictureBox.TabIndex = 12;
            this.infoPictureBox.TabStop = false;
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
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
            this.replaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceComboBox.Location = new System.Drawing.Point(13, 62);
            this.replaceComboBox.Name = "replaceComboBox";
            this.replaceComboBox.Size = new System.Drawing.Size(252, 21);
            this.replaceComboBox.TabIndex = 2;
            this.replaceComboBox.TextChanged += new System.EventHandler(this.LookupChanged);
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
            this.replaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceAllButton.Location = new System.Drawing.Point(277, 142);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(95, 23);
            this.replaceAllButton.TabIndex = 9;
            this.replaceAllButton.Text = "Replace &All";
            this.replaceAllButton.Click += new System.EventHandler(this.ReplaceAllButtonClick);
            // 
            // lookComboBox
            //
            this.lookComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
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
            this.lookComboBox.TextChanged += new System.EventHandler(this.LookupChanged);
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
            this.bookmarkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookmarkAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bookmarkAllButton.Location = new System.Drawing.Point(277, 82);
            this.bookmarkAllButton.Name = "bookmarkAllButton";
            this.bookmarkAllButton.Size = new System.Drawing.Size(95, 23);
            this.bookmarkAllButton.TabIndex = 7;
            this.bookmarkAllButton.Text = "&Bookmark All";
            this.bookmarkAllButton.Click += new System.EventHandler(this.BookmarkAllButtonClick);
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
            this.Load += new System.EventHandler(this.DialogLoad);
            this.VisibleChanged += new System.EventHandler(this.VisibleChange);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogClosing);
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
        }

        /// <summary>
        /// Set the case of the text to search
        /// </summary>
        public void SetMatchCase(Boolean matchCase)
        {
            this.matchCaseCheckBox.CheckedChanged -= new EventHandler(this.LookupChanged);
            this.matchCaseCheckBox.Checked = matchCase; // Change the value...
            this.matchCaseCheckBox.CheckedChanged += new EventHandler(this.LookupChanged);
        }

        /// <summary>
        /// Set the whole word prop of the text to search
        /// </summary>
        public void SetWholeWord(Boolean wholeWord)
        {
            this.wholeWordCheckBox.CheckedChanged -= new EventHandler(this.LookupChanged);
            this.wholeWordCheckBox.Checked = wholeWord; // Change the value...
            this.wholeWordCheckBox.CheckedChanged += new EventHandler(this.LookupChanged);
        }

        /// <summary>
        /// Set the text to search
        /// </summary>
        public void SetFindText(String text)
        {
            this.findComboBox.TextChanged -= new EventHandler(this.FindComboBoxTextChanged);
            this.findComboBox.Text = text; // Change the value...
            this.findComboBox.TextChanged += new EventHandler(this.FindComboBoxTextChanged);
        }

        /// <summary>
        /// When the text changes, synchronizes the find controls
        /// </summary>
        private void FindComboBoxTextChanged(Object sender, EventArgs e)
        {
            Globals.MainForm.SetFindText(this, this.findComboBox.Text);
        }

        /// <summary>
        /// If there is a word selected, insert it to the find box
        /// </summary>
        private void UpdateFindText()
        {
            ScintillaControl sci = Globals.SciControl;
            if (this.useRegexCheckBox.Checked) return;
            if (sci != null && sci.SelText.Length > 0 && !this.lookupIsDirty)
            {
                this.findComboBox.Text = sci.SelText;
                this.lookupIsDirty = false;
            }
        }
        
        /// <summary>
        /// Finds the next result based on direction
        /// </summary>
        public void FindNext(Boolean forward, Boolean update, Boolean simple)
        {
            this.currentMatch = null;
            if (update) this.UpdateFindText();
            if (Globals.SciControl == null) return;
            ScintillaControl sci = Globals.SciControl;
            List<SearchMatch> matches = this.GetResults(sci, simple);
            if (matches != null && matches.Count != 0)
            {
                FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                SearchMatch match = FRDialogGenerics.GetNextDocumentMatch(sci, matches, forward, false);
                if (match != null)
                {
                    this.currentMatch = match;
                    FRDialogGenerics.SelectMatch(sci, match);
                    this.lookupIsDirty = false;
                }
                if (this.Visible)
                {
                    Int32 index = FRDialogGenerics.GetMatchIndex(match, matches);
                    String message = TextHelper.GetString("Info.ShowingResult");
                    String formatted = String.Format(message, index, matches.Count);
                    this.ShowMessage(formatted, 0);
                }
            }
            else
            {
                if (this.Visible)
                {
                    String message = TextHelper.GetString("Info.NoMatchesFound");
                    this.ShowMessage(message, 0);
                }
            }
            this.SelectText();
        }
        public void FindNext(Boolean forward, Boolean update)
        {
            this.FindNext(forward, update, false);
        }
        public void FindNext(Boolean forward)
        {
            this.FindNext(forward, true);
        }

        /// <summary>
        /// Finds the next result specified by user input
        /// </summary>
        private void FindNextButtonClick(Object sender, System.EventArgs e)
        {
            this.FindNext(true, false);
        }
        
        /// <summary>
        /// Finds the previous result specified by user input
        /// </summary>
        private void FindPrevButtonClick(Object sender, System.EventArgs e)
        {
            this.FindNext(false, false);
        }

        /// <summary>
        /// Bookmarks all results
        /// </summary>
        private void BookmarkAllButtonClick(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl == null) return;
            ScintillaControl sci = Globals.SciControl;
            List<SearchMatch> matches = this.GetResults(sci);
            if (matches != null && this.lookComboBox.SelectedIndex == 1 && sci.SelText.Length > 0)
            {
                Int32 end = sci.MBSafeCharPosition(sci.SelectionEnd);
                Int32 start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (matches != null && matches.Count != 0)
            {
                FRDialogGenerics.BookmarkMatches(sci, matches);
                String message = TextHelper.GetString("Info.MatchesBookmarked");
                this.ShowMessage(message, 0);
            }
            else
            {
                String message = TextHelper.GetString("Info.NothingToBookmark");
                this.ShowMessage(message, 0);
            }
        }

        /// <summary>
        /// Replaces the next result specified by user input
        /// </summary>
        private void ReplaceButtonClick(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl == null) return;
            ScintillaControl sci = Globals.SciControl;
            if (sci.SelText.Length == 0) this.FindNext(true);
            else
            {
                String replaceWith = this.GetReplaceText(currentMatch);
                sci.ReplaceSel(replaceWith);
                FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                FRDialogGenerics.UpdateComboBoxItems(this.replaceComboBox);
                String message = TextHelper.GetString("Info.SelectedReplaced");
                this.ShowMessage(message, 0);
                this.lookupIsDirty = false;
                this.FindNext(true);
            }
        }

        /// <summary>
        /// Replaces all results specified by user input
        /// </summary>
        private void ReplaceAllButtonClick(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl == null) return;
            ScintillaControl sci = Globals.SciControl;
            List<SearchMatch> matches = this.GetResults(sci);
            if (matches != null && this.lookComboBox.SelectedIndex == 1 && sci.SelText.Length > 0)
            {
                Int32 end = sci.MBSafeCharPosition(sci.SelectionEnd);
                Int32 start = sci.MBSafeCharPosition(sci.SelectionStart);
                matches = FRDialogGenerics.FilterMatches(matches, start, end);
            }
            if (matches != null)
            {
                sci.BeginUndoAction();
                try
                {
                    for (Int32 i = 0; i < matches.Count; i++)
                    {
                        FRDialogGenerics.SelectMatch(sci, matches[i]);
                        String replaceWith = this.GetReplaceText(matches[i]);
                        FRSearch.PadIndexes(matches, i, matches[i].Value, replaceWith);
                        sci.EnsureVisible(sci.CurrentLine);
                        sci.ReplaceSel(replaceWith);
                    }
                }
                finally
                {
                    sci.EndUndoAction();
                }
                FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                FRDialogGenerics.UpdateComboBoxItems(this.replaceComboBox);
                String message = TextHelper.GetString("Info.ReplacedMatches");
                String formatted = String.Format(message, matches.Count);
                this.ShowMessage(formatted, 0);
                this.lookupIsDirty = false;
            }
        }

        /// <summary>
        /// Closes the find and replace dialog
        /// </summary>
        private void CloseButtonClick(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Marks the lookup as dirty
        /// </summary>
        private void LookupChanged(Object sender, System.EventArgs e)
        {
            if (this.Visible)
            {
                this.lookupIsDirty = true;
                this.currentMatch = null;
            }
            if (sender == this.matchCaseCheckBox && !Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetMatchCase(this, this.matchCaseCheckBox.Checked);
            }
            if (sender == this.wholeWordCheckBox && !Globals.Settings.DisableFindOptionSync)
            {
                Globals.MainForm.SetWholeWord(this, this.wholeWordCheckBox.Checked);
            }
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        private void VisibleChange(Object sender, System.EventArgs e)
        {
            if (this.Visible) this.SelectText();
            else this.lookupIsDirty = false;
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        private void DialogClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Globals.CurrentDocument.Activate();
            this.Hide();
        }

        /// <summary>
        /// Setups the dialog on load
        /// </summary>
        private void DialogLoad(Object sender, EventArgs e)
        {
            String message = TextHelper.GetString("Info.NoMatches");
            this.ShowMessage(message, 0);
            this.CenterToParent();
        }

        /// <summary>
        /// Process shortcuts
        /// </summary>
        protected override Boolean ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                this.findComboBox.Focus();
                return true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Enter && (keyData & Keys.Shift) > 0)
            {
                this.FindNext(false, false);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Gets the replacement text and escapes it if needed
        /// </summary>
        private String GetReplaceText(SearchMatch match)
        {
            String replace = this.replaceComboBox.Text;
            if (this.escapedCheckBox.Checked) replace = FRSearch.Unescape(replace);
            if (this.useRegexCheckBox.Checked) replace = FRSearch.ExpandGroups(replace, match);
            return replace;
        }

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        private List<SearchMatch> GetResults(ScintillaControl sci, Boolean simple)
        {
            if (IsValidPattern())
            {
                String pattern = this.findComboBox.Text;
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
            String pattern = this.findComboBox.Text;
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
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shows the info message in dialog
        /// </summary>
        private void ShowMessage(String msg, Int32 img)
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
            this.UpdateFindText();
        }

        #endregion
        
    }
    
}
