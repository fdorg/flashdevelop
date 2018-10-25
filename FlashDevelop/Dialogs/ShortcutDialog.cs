using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using FlashDevelop.Managers;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class ShortcutDialog : SmartForm
    {
        private Timer updateTimer;
        private ToolStripMenuItem removeShortcut;
        private ToolStripMenuItem revertToDefault;
        private ToolStripMenuItem revertAllToDefault;
        private ShortcutListItem[] shortcutListItems;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.ColumnHeader keyHeader;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private const char ViewConflictsKey = '?';
        private const char ViewCustomKey = '*';

        public ShortcutDialog()
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
            this.searchLabel = new System.Windows.Forms.Label();
            this.filterTextBox = new System.Windows.Forms.TextBoxEx();
            this.clearButton = new System.Windows.Forms.ButtonEx();
            this.idHeader = new System.Windows.Forms.ColumnHeader();
            this.keyHeader = new System.Windows.Forms.ColumnHeader();
            this.listView = new System.Windows.Forms.ListViewEx();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.ButtonEx();
            this.exportButton = new System.Windows.Forms.ButtonEx();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
            this.SuspendLayout();
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.searchLabel.Location = new System.Drawing.Point(10, 10);
            this.searchLabel.Name = "searchLabel";
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.filterTextBox.Location = new System.Drawing.Point(12, 32);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(561, 20);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextChanged);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(579, 30);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(26, 23);
            this.clearButton.TabIndex = 1;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearFilterClick);
            // 
            // idHeader
            // 
            this.idHeader.Width = 350;
            // 
            // keyHeader
            // 
            this.keyHeader.Width = 239;
            // 
            // listView
            // 
            this.listView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.idHeader, this.keyHeader });
            this.listView.GridLines = true;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(12, 62);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView222";
            this.listView.Size = new System.Drawing.Size(592, 312);
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.KeyDown += new KeyEventHandler(this.ListViewKeyDown);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.pictureBox.Location = new System.Drawing.Point(12, 386);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(16, 16);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.infoLabel.Location = new System.Drawing.Point(33, 378);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(420, 32);
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // importButton
            // 
            this.importButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.importButton.Location = new System.Drawing.Point(454, 382);
            this.importButton.Name = "openButton";
            this.importButton.Size = new System.Drawing.Size(25, 23);
            this.importButton.TabIndex = 3;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.SelectCustomShortcut);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.exportButton.Location = new System.Drawing.Point(484, 382);
            this.exportButton.Name = "saveButton";
            this.exportButton.Size = new System.Drawing.Size(25, 23);
            this.exportButton.TabIndex = 4;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.SaveCustomShortcut);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(515, 382);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(90, 23);
            this.closeButton.TabIndex = 5;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // ShortcutDialog
            // 
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Name = "ShortcutDialog";
            this.AcceptButton = this.closeButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 418);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.searchLabel);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.closeButton);
            this.FormClosing += new FormClosingEventHandler(this.DialogClosing);
            this.FormClosed += new FormClosedEventHandler(this.DialogClosed);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            this.pictureBox.Image = Globals.MainForm.FindImage16("229", false);
            this.clearButton.Image = Globals.MainForm.FindImage16("153", false);
            this.exportButton.Image = Globals.MainForm.FindImage16("55|9|3|3", false);
            this.importButton.Image = Globals.MainForm.FindImage16("55|1|3|3", false);
        }

        /// <summary>
        /// Initializes the <see cref="ListView"/> context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            var cms = new ContextMenuStrip();
            cms.Font = Globals.Settings.DefaultFont;
            cms.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            cms.Renderer = new DockPanelStripRenderer(false, false);
            this.removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, this.RemoveShortcutClick);
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefaultClick);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefaultClick);
            this.removeShortcut.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Delete);
            this.removeShortcut.Image = Globals.MainForm.FindImage("153", false);
            this.revertToDefault.Image = Globals.MainForm.FindImage("69", false);
            this.revertAllToDefault.Image = Globals.MainForm.FindImage("224", false);
            cms.Items.Add(this.removeShortcut);
            cms.Items.Add(this.revertToDefault);
            cms.Items.Add(this.revertAllToDefault);
            cms.Opening += this.ContextMenuOpening;
            this.listView.ContextMenuStrip = cms;
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            this.idHeader.Text = TextHelper.GetString("Label.Command");
            this.keyHeader.Text = TextHelper.GetString("Label.Shortcut");
            this.infoLabel.Text = TextHelper.GetString("Info.ShortcutEditInfo");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            tooltip.SetToolTip(this.importButton, TextHelper.GetStringWithoutMnemonics("Label.Import"));
            tooltip.SetToolTip(this.exportButton, TextHelper.GetStringWithoutMnemonics("Label.Export"));
            this.searchLabel.Text = TextHelper.GetString("Label.ShortcutSearch");
            this.Text = " " + TextHelper.GetString("Title.Shortcuts");
        }

        /// <summary>
        /// Applies additional scaling to controls in order to support HDPI.
        /// </summary>
        private void ApplyScaling()
        {
            this.idHeader.Width = ScaleHelper.Scale(this.idHeader.Width);
            this.keyHeader.Width = ScaleHelper.Scale(this.keyHeader.Width);
        }

        /// <summary>
        /// Updates the font highlight of the item.
        /// </summary>
        private static void UpdateItemHighlightFont(ShortcutListItem item)
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
        private void InitializeShortcutListItems()
        {
            var collection = ShortcutManager.RegisteredItems.Values;
            this.shortcutListItems = new ShortcutListItem[collection.Count];
            int counter = 0;
            foreach (var item in collection)
            {
                this.shortcutListItems[counter++] = new ShortcutListItem(item);
            }
            Array.Sort(this.shortcutListItems, new ShorcutListItemComparer());
            this.UpdateAllShortcutsConflicts();
        }

        /// <summary>
        /// Update conflicts statuses of all shortcut items.
        /// </summary>
        private bool UpdateAllShortcutsConflicts()
        {
            bool conflicts = false;
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (item.HasConflicts)
                {
                    conflicts = true;
                }
                else
                {
                    this.GetConflictItems(item);
                    conflicts = conflicts || item.HasConflicts;
                }
            }
            return conflicts;
        }

        /// <summary>
        /// Display a warning message to show conflicts.
        /// </summary>
        private bool ShowConflictsPresent()
        {
            string text = TextHelper.GetString("Info.ShortcutConflictsPresent");
            string caption = TextHelper.GetString("Title.WarningDialog");
            if (MessageBox.Show(this, text, " " + caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                this.filterTextBox.Text = ViewConflictsKey.ToString();
                this.filterTextBox.SelectAll();
                this.filterTextBox.Focus();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates the shortcut list view.
        /// </summary>
        private void PopulateListView(string filter)
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
        /// The order of the keywords is irrelevant.
        /// </summary>
        private static string ExtractFilterKeywords(string filter, ref bool viewCustom, ref bool viewConflicts)
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
        private void ContextMenuOpening(object sender, EventArgs e)
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
        private void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.listView.SelectedItems.Count == 0) return;
            var item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.AssignNewShortcut(item, e.KeyData);
            // Don't trigger list view default shortcuts like Ctrl+Add
            switch (e.KeyData) {
                case Keys.Up:
                case Keys.Down:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Home:
                case Keys.End:
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Assign the new shortcut.
        /// </summary>
        private void AssignNewShortcut(ShortcutListItem item, Keys shortcut)
        {
            if (shortcut == 0 || shortcut == Keys.Delete) shortcut = 0;
            else if (!ToolStripManager.IsValidShortcut(shortcut)) return;
            if (item.Custom == shortcut) return;
            this.listView.BeginUpdate();
            var oldShortcut = item.Custom;
            item.Custom = shortcut;
            item.Selected = true;
            this.GetConflictItems(item);
            this.listView.EndUpdate();
            if (item.HasConflicts)
            {
                string text = TextHelper.GetString("Info.ShortcutIsAlreadyUsed");
                string caption = TextHelper.GetString("Title.WarningDialog");
                switch (MessageBox.Show(Globals.MainForm, text, " " + caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                {
                    case DialogResult.Abort:
                        this.listView.BeginUpdate();
                        item.Custom = oldShortcut;
                        this.GetConflictItems(item);
                        this.listView.EndUpdate();
                        break;
                    case DialogResult.Retry:
                        this.filterTextBox.Text = ViewConflictsKey + item.KeysString;
                        this.filterTextBox.SelectAll();
                        this.filterTextBox.Focus(); // Set focus to filter...
                        break;
                }
            }
        }

        /// <summary>
        /// Resets the conflicts status of an item.
        /// </summary>
        private static void ResetConflicts(ShortcutListItem item)
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
        private void GetConflictItems(ShortcutListItem target)
        {
            var keys = target.Custom;
            if (keys == 0) return;
            List<ShortcutListItem> conflicts = null;
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (item.Custom != keys || item == target) continue;
                if (conflicts == null) conflicts = new List<ShortcutListItem> { target };
                conflicts.Add(item);
                item.Conflicts = conflicts;
            }
            target.Conflicts = conflicts;
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        private void RevertToDefaultClick(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                this.RevertToDefault((ShortcutListItem) this.listView.SelectedItems[0]);
            }
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        private void RevertAllToDefaultClick(object sender, EventArgs e)
        {
            foreach (ShortcutListItem item in this.listView.Items) RevertToDefault(item);
        }

        /// <summary>
        /// Revert the selected items shortcut to default.
        /// </summary>
        private void RevertToDefault(ShortcutListItem item)
        {
            this.AssignNewShortcut(item, item.Default);
        }

        /// <summary>
        /// Removes the shortcut by setting it to <see cref="Keys.None"/>.
        /// </summary>
        private void RemoveShortcutClick(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                this.AssignNewShortcut((ShortcutListItem) this.listView.SelectedItems[0], 0);
            }
        }

        /// <summary>
        /// Clears the filter text field.
        /// </summary>
        private void ClearFilterClick(object sender, EventArgs e)
        {
            this.filterTextBox.Text = string.Empty;
            this.filterTextBox.Select();
        }

        /// <summary>
        /// Set up the timer for delayed list update with filters.
        /// </summary>
        private void SetupUpdateTimer()
        {
            this.updateTimer = new Timer();
            this.updateTimer.Enabled = false;
            this.updateTimer.Interval = 100;
            this.updateTimer.Tick += this.UpdateTimer_Tick;
        }

        /// <summary>
        /// Update the list with filter.
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
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
        private void SelectCustomShortcut(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.listView.BeginUpdate();
                    ShortcutManager.LoadCustomShortcuts(dialog.FileName, this.shortcutListItems);
                    bool conflicts = this.UpdateAllShortcutsConflicts();
                    this.listView.EndUpdate();
                    if (conflicts) this.ShowConflictsPresent(); // Make sure the warning message shows up after the listview is rendered
                }
            }
        }

        /// <summary>
        /// Save the current shortcut set to a file.
        /// </summary>
        private void SaveCustomShortcut(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".fda",
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                OverwritePrompt = true,
                Title = " " + TextHelper.GetString("Title.SaveFileDialog")
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    ShortcutManager.SaveCustomShortcuts(dialog.FileName, this.shortcutListItems);
                }
            }
        }

        /// <summary>
        /// Closes the shortcut dialog.
        /// </summary>
        private void CloseButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        private void DialogClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                if (this.shortcutListItems[i].HasConflicts)
                {
                    e.Cancel = this.ShowConflictsPresent();
                    break;
                }
            }
        }

        /// <summary>
        /// When the form is closed, applies shortcuts.
        /// </summary>
        private void DialogClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++) this.shortcutListItems[i].ApplyChanges(); 
            Globals.MainForm.ApplyAllSettings();
            ShortcutManager.SaveCustomShortcuts();
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

        #region ShortcutListItem

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
                    ResetConflicts(this);
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
                this.item = shortcutItem;
                this.conflicts = null;
                this.custom = this.Item.Custom;
                this.KeysString = DataConverter.KeysToString(this.Custom);
                this.Name = this.Text = this.Id;
                this.SubItems.Add(this.KeysString);
                UpdateItemHighlightFont(this);
            }

            /// <summary>
            /// Apply changes made to this instance to the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public void ApplyChanges()
            {
                this.Item.Custom = this.Custom;
            }
        }

        #endregion

    }

}
