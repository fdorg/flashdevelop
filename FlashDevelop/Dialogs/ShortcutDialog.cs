using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Managers;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashDevelop.Dialogs
{
    public class ShortcutDialog : SmartForm
    {
        const string ViewConflictsKey = "?"; // TODO: Change the type to char after #969 is merged
        const string ViewCustomKey = "*"; // TODO: Change the type to char after #969 is merged
        
        Timer updateTimer;
        ToolStripMenuItem removeShortcut;
        ToolStripMenuItem revertToDefault;
        ToolStripMenuItem revertAllToDefault;
        ShortcutListItem[] shortcutListItems;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.ColumnHeader keyHeader;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button saveButton;

        ShortcutDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.ApplyLocalizedTexts();
            this.SetupUpdateTimer();
            this.InitializeGraphics();
            this.InitializeShortcutListItems();
            this.PopulateListView(string.Empty);
            this.ApplyScaling();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.idHeader = new System.Windows.Forms.ColumnHeader();
            this.keyHeader = new System.Windows.Forms.ColumnHeader();
            this.closeButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.searchLabel = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // idHeader
            // 
            this.idHeader.Text = "Command";
            this.idHeader.Width = 350;
            // 
            // keyHeader
            // 
            this.keyHeader.Text = "Shortcut";
            this.keyHeader.Width = 208;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.idHeader, this.keyHeader});
            this.listView.GridLines = true;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(12, 70);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(562, 304);
            this.listView.TabIndex = 4;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.KeyDown += new KeyEventHandler(this.ListViewKeyDown);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(485, 381);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(90, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox.Location = new System.Drawing.Point(12, 385);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(16, 16);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.Location = new System.Drawing.Point(33, 386);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(446, 16);
            this.infoLabel.TabIndex = 6;
            this.infoLabel.Text = "Shortcuts can be edited by selecting an item and pressing valid menu item shortcut keys.";
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.searchLabel.Location = new System.Drawing.Point(10, 25);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search:";
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(549, 39);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(26, 23);
            this.clearButton.TabIndex = 2;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearFilterClick);
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(12, 41);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(531, 20);
            this.filterTextBox.TabIndex = 1;
            this.filterTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextChanged);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(369, 12);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(100, 23);
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(SelectCustomShortcut);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(475, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 23);
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(SaveCustomShortcut);
            // 
            // ShortcutDialog
            // 
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Text = " Shortcuts";
            this.Name = "ShortcutDialog";
            this.AcceptButton = this.closeButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 416);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.searchLabel);
            this.FormClosing += new FormClosingEventHandler(this.DialogClosing);
            this.FormClosed += new FormClosedEventHandler(this.DialogClosed);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        void InitializeGraphics()
        {
            this.pictureBox.Image = Globals.MainForm.FindImage("229");
            this.clearButton.Image = Globals.MainForm.FindImage("153");
        }

        /// <summary>
        /// Initializes the <see cref="ListView"/> context menu.
        /// </summary>
        void InitializeContextMenu()
        {
            var cms = new ContextMenuStrip();
            cms.Font = Globals.Settings.DefaultFont;
            cms.Renderer = new DockPanelStripRenderer(false, false);
            this.removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, this.RemoveShortcutClick);
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefaultClick);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefaultClick);
            this.removeShortcut.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Delete);
            this.removeShortcut.Image = Globals.MainForm.FindImage("153");
            this.revertToDefault.Image = Globals.MainForm.FindImage("69");
            this.revertAllToDefault.Image = Globals.MainForm.FindImage("224");
            cms.Items.Add(this.removeShortcut);
            cms.Items.Add(this.revertToDefault);
            cms.Items.Add(this.revertAllToDefault);
            cms.Opening += this.ContextMenuOpening;
            this.listView.ContextMenuStrip = cms;
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        void ApplyLocalizedTexts()
        {
            this.idHeader.Text = TextHelper.GetString("Label.Command");
            this.keyHeader.Text = TextHelper.GetString("Label.Shortcut");
            this.infoLabel.Text = TextHelper.GetString("Info.ShortcutEditInfo");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.openButton.Text = TextHelper.GetString("Label.Open");
            this.saveButton.Text = TextHelper.GetString("Label.SaveAs");
            this.searchLabel.Text = TextHelper.GetString("Label.Search").Replace("&", "") + ":";
            this.Text = " " + TextHelper.GetString("Title.Shortcuts");
        }

        /// <summary>
        /// Applies additional scaling to controls in order to support HDPI.
        /// </summary>
        void ApplyScaling()
        {
            this.idHeader.Width = ScaleHelper.Scale(this.idHeader.Width);
            this.keyHeader.Width = ScaleHelper.Scale(this.keyHeader.Width);
        }
        
        /// <summary>
        /// Updates the font highlight of the item.
        /// </summary>
        static void UpdateItemHighlightFont(ShortcutListItem item)
        {
            if (item.HasConflicts)
            {
                item.ForeColor = Color.DarkRed;
                item.SubItems[1].ForeColor = Color.DarkRed;
            }
            else
            {
                item.ForeColor = SystemColors.ControlText;
                item.SubItems[1].ForeColor = item.Custom == 0 ? SystemColors.GrayText : SystemColors.ControlText;
            }
            item.Font = new Font(Globals.Settings.DefaultFont, item.IsModified ? FontStyle.Bold : 0);
            item.UseItemStyleForSubItems = item.IsModified;
        }

        /// <summary>
        /// Initialize the full shortcut list.
        /// </summary>
        void InitializeShortcutListItems()
        {
            var collection = ShortcutManager.RegisteredItems.Values;
            this.shortcutListItems = new ShortcutListItem[collection.Count];
            int counter = 0;
            foreach (var item in collection)
            {
                this.shortcutListItems[counter++] = new ShortcutListItem(item);
            }
            Array.Sort(this.shortcutListItems, new ShorcutListItemComparer());

            bool conflicts = false;
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                this.GetConflictItems(item);
                conflicts = conflicts || item.HasConflicts;
            }
            if (conflicts)
            {
                ErrorManager.ShowWarning(TextHelper.GetString("Info.ShortcutConflictsPresent"), null);
                this.filterTextBox.Text = ViewConflictsKey.ToString();
            }
        }

        /// <summary>
        /// Populates the shortcut list view.
        /// </summary>
        void PopulateListView(string filter)
        {
            bool viewCustom = false, viewConflicts = false;
            filter = ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);

            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (string.IsNullOrEmpty(filter) ||
                    item.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || 
                    item.KeysString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (viewCustom && !item.IsModified) continue;
                    if (viewConflicts && !item.HasConflicts) continue;
                    this.listView.Items.Add(item);
                }
            }
            this.keyHeader.Width = -2;
            this.listView.EndUpdate();

            if (this.listView.Items.Count > 0) this.listView.Items[0].Selected = true;
        }

        /// <summary>
        /// Reads and removes filter keywords from the start of the filter.
        /// Order of the keywords is irrelevant.
        /// </summary>
        static string ExtractFilterKeywords(string filter, ref bool viewCustom, ref bool viewConflicts)
        {
            if (!viewCustom && filter.StartsWith(ViewCustomKey))
            {
                filter = filter.Substring(1);
                viewCustom = true;
                return ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);
            }
            if (!viewConflicts && filter.StartsWith(ViewConflictsKey))
            {
                filter = filter.Substring(1);
                viewConflicts = true;
                return ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);
            }
            return filter.Trim();
        }

        /// <summary>
        /// Raised when the context menu for the list view is opening.
        /// </summary>
        void ContextMenuOpening(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                var item = (ShortcutListItem) this.listView.SelectedItems[0];
                this.removeShortcut.Enabled = item.Custom != 0;
                this.revertToDefault.Enabled = this.revertAllToDefault.Enabled = item.IsModified;
                if (this.revertAllToDefault.Enabled) return;
            }
            else this.removeShortcut.Enabled = this.revertToDefault.Enabled = false;

            foreach (ShortcutListItem item in this.listView.Items)
            {
                if (item.IsModified)
                {
                    this.revertAllToDefault.Enabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Assign a new valid shortcut when keys are pressed.
        /// </summary>
        void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.listView.SelectedItems.Count == 0) return;

            var item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.AssignNewShortcut(item, e.KeyData);

            // Don't trigger list view default shortcuts like Ctrl+Add
            if (e.KeyData != Keys.Up && e.KeyData != Keys.Down) e.Handled = true;
        }

        /// <summary>
        /// Assign the new shortcut.
        /// </summary>
        void AssignNewShortcut(ShortcutListItem item, Keys shortcut, bool suppressWarning = false)
        {
            if (shortcut == 0 || shortcut == Keys.Delete) shortcut = 0;
            else if (!ToolStripManager.IsValidShortcut(shortcut)) return;

            if (item.Custom == shortcut) return;
            item.Custom = shortcut;
            item.Selected = true;
            ResetConflicts(item);
            this.GetConflictItems(item);
            if (item.HasConflicts)
            {
                if (suppressWarning) return;
                ErrorManager.ShowWarning(TextHelper.GetString("Info.ShortcutIsAlreadyUsed"), null);
                this.filterTextBox.Focus(); // Set focus to filter...
                this.filterTextBox.Text = ViewConflictsKey + item.KeysString;
                this.filterTextBox.SelectAll();
            }
        }
        
        /// <summary>
        /// Resets the conflicts status of an item.
        /// </summary>
        static void ResetConflicts(ShortcutListItem item)
        {
            if (!item.HasConflicts) return;
            var conflicts = item.Conflicts;
            conflicts.Remove(item);
            item.Conflicts = null;

            if (conflicts.Count == 1)
            {
                item = conflicts[0];
                conflicts.RemoveAt(0);
                item.Conflicts = null; // empty conflicts list will be garbage collected
            }
        }
        
        /// <summary>
        /// Gets a list of all conflicting entries.
        /// </summary>
        void GetConflictItems(ShortcutListItem target)
        {
            var keys = target.Custom;
            if (keys == 0) return;

            var conflicts = new List<ShortcutListItem> { target };

            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];

                if (item == target) continue;
                if (item.Custom == keys)
                {
                    conflicts.Add(item);
                    item.Conflicts = conflicts; 
                }
            }

            if (conflicts.Count > 1) target.Conflicts = conflicts;
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        void RevertToDefaultClick(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
                this.RevertToDefault((ShortcutListItem) this.listView.SelectedItems[0]);
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        void RevertAllToDefaultClick(object sender, EventArgs e)
        {
            foreach (ShortcutListItem item in this.listView.Items) this.RevertToDefault(item);
        }

        /// <summary>
        /// Revert the selected items shortcut to default.
        /// </summary>
        void RevertToDefault(ShortcutListItem item)
        {
            this.AssignNewShortcut(item, item.Default);
        }

        /// <summary>
        /// Removes the shortcut by setting it to <see cref="Keys.None"/>.
        /// </summary>
        void RemoveShortcutClick(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
                this.AssignNewShortcut((ShortcutListItem) this.listView.SelectedItems[0], 0);
        }

        /// <summary>
        /// Clears the filter text field.
        /// </summary>
        void ClearFilterClick(object sender, EventArgs e)
        {
            this.filterTextBox.Text = string.Empty;
            this.filterTextBox.Select();
        }

        /// <summary>
        /// Set up the timer for delayed list update with filters.
        /// </summary>
        void SetupUpdateTimer()
        {
            this.updateTimer = new Timer();
            this.updateTimer.Enabled = false;
            this.updateTimer.Interval = 100;
            this.updateTimer.Tick += this.UpdateTimer_Tick;
        }

        /// <summary>
        /// Update the list with filter.
        /// </summary>
        void UpdateTimer_Tick(object sender, EventArgs e)
        {
            this.updateTimer.Enabled = false;
            this.PopulateListView(this.filterTextBox.Text);
        }

        /// <summary>
        /// Restart the timer for updating the list.
        /// </summary>
        void FilterTextChanged(object sender, EventArgs e)
        {
            this.updateTimer.Stop();
            this.updateTimer.Start();
        }

        /// <summary>
        /// Switch to a custom shortcut set.
        /// </summary>
        void SelectCustomShortcut(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string extension = Path.GetExtension(dialog.FileName);
                if (extension.Equals(".fda", StringComparison.OrdinalIgnoreCase))
                {
                    var shortcuts = ShortcutManager.LoadCustomShortcuts(dialog.FileName, this.shortcutListItems);
                    if (shortcuts != null)
                    {
                        this.listView.BeginUpdate();
                        for (int i = 0; i < shortcuts.Length; i++)
                        {
                            this.AssignNewShortcut(this.shortcutListItems[i], shortcuts[i], true);
                        }
                        this.listView.EndUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Save the current shortcut set to a file.
        /// </summary>
        void SaveCustomShortcut(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".fda",
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                OverwritePrompt = true,
                Title = " " + TextHelper.GetString("Title.SaveFileDialog")
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShortcutManager.SaveCustomShortcuts(dialog.FileName, this.shortcutListItems);
            }
        }

        /// <summary>
        /// Closes the shortcut dialog.
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        void DialogClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                if (this.shortcutListItems[i].HasConflicts)
                {
                    ErrorManager.ShowError(TextHelper.GetString("Info.ShortcutConflictsPresent"), null);
                    this.filterTextBox.Text = ViewConflictsKey.ToString();
                    e.Cancel = true;
                    break;
                }
            }
        }

        /// <summary>
        /// When the form is closed, applies shortcuts.
        /// </summary>
        void DialogClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++) this.shortcutListItems[i].ApplyChanges();
            Globals.MainForm.ApplyAllSettings();
        }

        /// <summary>
        /// Shows the shortcut dialog.
        /// </summary>
        public static new void Show()
        {
            var shortcutDialog = new ShortcutDialog();
            shortcutDialog.CenterToParent();
            shortcutDialog.Show(Globals.MainForm);
            shortcutDialog.filterTextBox.Focus();
        }

        #endregion

        
        #region ListViewComparer

        /// <summary>
        /// Defines a method that compares two <see cref="ShortcutListItem"/> objects.
        /// </summary>
        class ShorcutListItemComparer : IComparer<ShortcutListItem>
        {
            /// <summary>
            /// Compares two <see cref="ShortcutListItem"/> objects.
            /// </summary>
            int IComparer<ShortcutListItem>.Compare(ShortcutListItem x, ShortcutListItem y)
            {
                return StringComparer.Ordinal.Compare(x.Text, y.Text);
            }
        }

        #endregion

        /// <summary>
        /// Represents a visual representation of a <see cref="ShortcutItem"/> object.
        /// </summary>
        class ShortcutListItem : ListViewItem, IShortcutItem
        {
            readonly ShortcutItem item;
            List<ShortcutListItem> conflicts;
            Keys custom;

            /// <summary>
            /// Gets the associated <see cref="ShortcutItem"/> object.
            /// </summary>
            public ShortcutItem Item
            {
                get { return this.item; }
            }

            /// <summary>
            /// Gets whether this <see cref="ShortcutListItem"/> has other conflicting <see cref="ShortcutListItem"/> objects.
            /// </summary>
            public bool HasConflicts
            {
                get { return this.Conflicts != null; }
            }

            /// <summary>
            /// Gets or sets a collection of <see cref="ShortcutListItem"/> objects that have conflicting keys with this instance.
            /// </summary>
            public List<ShortcutListItem> Conflicts
            {
                get { return this.conflicts; }
                set
                {
                    if (this.conflicts == value) return;
                    this.conflicts = value;
                    UpdateItemHighlightFont(this);
                }
            }

            /// <summary>
            /// Gets the ID of the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public string Id
            {
                get { return this.Item.Id; }
            }

            /// <summary>
            /// Gets the default shortcut keys.
            /// </summary>
            public Keys Default
            {
                get { return this.Item.Default; }
            }

            /// <summary>
            /// Gets or sets the custom shortcut keys.
            /// </summary>
            public Keys Custom
            {
                get { return this.custom; }
                set
                {
                    if (this.custom == value) return;
                    this.custom = value;
                    this.KeysString = DataConverter.KeysToString(this.custom);
                    this.SubItems[1].Text = this.KeysString;
                    UpdateItemHighlightFont(this);
                }
            }

            /// <summary>
            /// Gets the string representation of the custom shortcut keys.
            /// </summary>
            public string KeysString
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the modification status of the shortcut.
            /// </summary>
            public bool IsModified
            {
                get { return this.Custom != this.Default; }
            }

            /// <summary>
            /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
            /// </summary>
            public ShortcutListItem(ShortcutItem shortcutItem)
            {
                this.SubItems.Add(string.Empty);
                this.Name = this.Text = shortcutItem.Id;
                this.item = shortcutItem;
                this.conflicts = null;
                this.Custom = shortcutItem.Custom;
            }

            /// <summary>
            /// Apply changes made to this instance to the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public void ApplyChanges()
            {
                this.Item.Custom = this.Custom;
            }
        }
    }
}
