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
using PluginCore.Managers;

namespace FlashDevelop.Dialogs
{
    public class ShortcutDialog : SmartForm
    {
        private const char ViewConflictsKey = '?';
        private const char ViewCustomKey = '*';

        private Timer updateTimer;
        private ToolStripMenuItem removeShortcut;
        private ToolStripMenuItem revertToDefault;
        private ToolStripMenuItem revertAllToDefault;
        private ShortcutListItem[] shortcutListItems;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.ColumnHeader keyHeader;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TextBox filterTextBox;

        private ShortcutDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeLocalization();
            this.InitializeGraphics();
            this.InitializeShortcutListItems();
            this.InitializeUpdateTimer();
            this.ApplyScaling();
            this.PopulateListView(string.Empty);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.searchLabel = new System.Windows.Forms.Label();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.idHeader = new System.Windows.Forms.ColumnHeader();
            this.keyHeader = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList();
            this.listView = new System.Windows.Forms.ListView();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) this.pictureBox).BeginInit();
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
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(579, 30);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(26, 23);
            this.clearButton.TabIndex = 1;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // idHeader
            // 
            this.idHeader.Width = 350;
            // 
            // keyHeader
            // 
            this.keyHeader.Width = 239;
            // 
            // imageList
            // 
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            // 
            // listView
            // 
            this.listView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.idHeader, this.keyHeader });
            this.listView.GridLines = true;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(12, 62);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(592, 312);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ClientSizeChanged += new EventHandler(this.ListView_ClientSizeChanged);
            this.listView.DoubleClick += new EventHandler(this.ListView_DoubleClick);
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
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.exportButton.Location = new System.Drawing.Point(484, 382);
            this.exportButton.Name = "saveButton";
            this.exportButton.Size = new System.Drawing.Size(25, 23);
            this.exportButton.TabIndex = 4;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
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
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
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
            this.FormClosing += new FormClosingEventHandler(this.ShortcutDialog_FormClosing);
            this.FormClosed += new FormClosedEventHandler(this.ShortcutDialog_FormClosed);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize) this.pictureBox).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the <see cref="ListView"/> context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            var cms = new ContextMenuStrip();
            cms.Font = Globals.Settings.DefaultFont;
            cms.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            cms.Renderer = new DockPanelStripRenderer(false, false);
            this.removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, this.RemoveShortcut_Click);
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefault_Click);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefault_Click);
            this.removeShortcut.ShortcutKeys = Keys.Delete;
            cms.Items.Add(this.removeShortcut);
            cms.Items.Add(this.revertToDefault);
            cms.Items.Add(this.revertAllToDefault);
            cms.Opening += this.ContextMenu_Opening;
            this.listView.ContextMenuStrip = cms;
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        private void InitializeLocalization()
        {
            var tooltip = new ToolTip();
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
        /// Initializes the graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.Images.Add(Globals.MainForm.FindImage("545", false));
            this.imageList.Images.Add(Globals.MainForm.FindImage("545|12|5|4", false));
            this.pictureBox.Image = Globals.MainForm.FindImage16("229", false);
            this.clearButton.Image = Globals.MainForm.FindImage16("153", false);
            this.exportButton.Image = Globals.MainForm.FindImage16("55|9|3|3", false);
            this.importButton.Image = Globals.MainForm.FindImage16("55|1|3|3", false);
            this.removeShortcut.Image = Globals.MainForm.FindImage("153", false);
            this.revertToDefault.Image = Globals.MainForm.FindImage("69", false);
            this.revertAllToDefault.Image = Globals.MainForm.FindImage("224", false);
        }

        /// <summary>
        /// Initialize the full shortcut list.
        /// </summary>
        private void InitializeShortcutListItems()
        {
            var collection = ShortcutManager.RegisteredItems;
            this.shortcutListItems = new ShortcutListItem[collection.Count];
            int counter = 0;
            foreach (var item in collection)
            {
                this.shortcutListItems[counter++] = new ShortcutListItem(item) { UseItemStyleForSubItems = false };
            }
            Array.Sort(this.shortcutListItems, new ShorcutListItemComparer());
            this.UpdateAllShortcutsConflicts();
        }

        /// <summary>
        /// Set up the timer for delayed list update with filters.
        /// </summary>
        private void InitializeUpdateTimer()
        {
            this.updateTimer = new Timer();
            this.updateTimer.Enabled = false;
            this.updateTimer.Interval = 100;
            this.updateTimer.Tick += this.UpdateTimer_Tick;
        }

        /// <summary>
        /// Applies additional scaling to controls in order to support HDPI.
        /// </summary>
        private void ApplyScaling()
        {
            this.idHeader.Width = ScaleHelper.Scale(this.idHeader.Width);
            this.keyHeader.Width = ScaleHelper.Scale(this.keyHeader.Width);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the shortcut dialog.
        /// </summary>
        public static new void Show()
        {
            var shortcutDialog = new ShortcutDialog();
            shortcutDialog.Show(Globals.MainForm);
            shortcutDialog.filterTextBox.Focus();
        }

        /// <summary>
        /// Populates the shortcut list view.
        /// </summary>
        private void PopulateListView(string filter)
        {
            bool viewCustom = false;
            bool viewConflicts = false;
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
            this.listView.EndUpdate();
            if (this.listView.Items.Count > 0) this.listView.Items[0].Selected = true;
        }

        /// <summary>
        /// Assign the new shortcut.
        /// </summary>
        private void AssignNewShortcut(ShortcutListItem item, ShortcutKeys shortcut)
        {
            if (!shortcut.IsNone && !ShortcutKeysManager.IsValidShortcut(shortcut)) return;
            if (item.Custom == shortcut) return;
            this.listView.BeginUpdate();
            var oldShortcut = item.Custom;
            item.Custom = shortcut;
            this.GetConflictItems(item);
            item.Selected = true;
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
        /// Remove shortcut from the selected item.
        /// </summary>
        /// <param name="item"></param>
        private void RemoveShortcut(ShortcutListItem item)
        {
            this.AssignNewShortcut(item, ShortcutKeys.None);
        }

        /// <summary>
        /// Revert the selected items shortcut to default.
        /// </summary>
        private void RevertToDefault(ShortcutListItem item)
        {
            this.AssignNewShortcut(item, item.Default);
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
        /// Gets a list of all conflicting entries.
        /// </summary>
        private void GetConflictItems(ShortcutListItem target)
        {
            var keys = target.Custom;
            if (keys.IsNone) return;
            bool isSimple = keys.IsSimple;
            var first = keys.First;
            List<ShortcutListItem> conflicts = null;
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (item == target) continue;
                var itemKeys = item.Custom;
                if (keys == itemKeys || (isSimple || itemKeys.IsSimple) && first == itemKeys.First)
                {
                    if (conflicts == null)
                    {
                        if (!item.HasConflicts)
                        {
                            item.Conflicts = new List<ShortcutListItem>() { item };
                        }
                        conflicts = item.Conflicts;
                        conflicts.Add(target);
                    }
                    else if (!conflicts.Contains(item))
                    {
                        conflicts.Add(item);
                    }
                }
            }
            target.Conflicts = conflicts;
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
        /// Updates the font highlight of the item.
        /// <para/><see cref="FontStyle.Bold"/> - The item is modified.
        /// <para/><see cref="Color.DarkRed"/> - The item has conflicts.
        /// <para/><see cref="SystemColors.GrayText"/> - The item has no shortcut keys.
        /// </summary>
        private static void UpdateItemHighlightFont(ShortcutListItem item)
        {
            var fontStyle = item.IsModified ? FontStyle.Bold : 0;
            item.SubItems[0].Font = new Font(Globals.Settings.DefaultFont, fontStyle);
            item.SubItems[1].Font = new Font(Globals.Settings.DefaultFont, fontStyle);
            if (item.HasConflicts)
            {
                item.SubItems[0].ForeColor = Color.DarkRed;
                item.SubItems[1].ForeColor = Color.DarkRed;
            }
            else
            {
                item.SubItems[0].ForeColor = SystemColors.ControlText;
                item.SubItems[1].ForeColor = item.Custom.IsNone ? SystemColors.GrayText : SystemColors.ControlText;
            }
        }

        /// <summary>
        /// Resets the conflicts status of an item.
        /// </summary>
        private static void ResetConflicts(ShortcutListItem item)
        {
            if (item.HasConflicts)
            {
                var conflicts = item.Conflicts;
                item.Conflicts = null;
                conflicts.Remove(item);
                if (conflicts.Count == 1)
                {
                    item = conflicts[0];
                    item.Conflicts = null; // empty conflicts list will be garbage collected
                    conflicts.Clear();
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Restart the timer for updating the list.
        /// </summary>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            this.updateTimer.Stop();
            this.updateTimer.Start();
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
        /// Clears the filter text field.
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.filterTextBox.TextChanged -= this.FilterTextBox_TextChanged;
            this.filterTextBox.Text = string.Empty;
            this.filterTextBox.TextChanged += this.FilterTextBox_TextChanged;
            this.filterTextBox.Select();
            this.PopulateListView(string.Empty);
        }

        /// <summary>
        /// Raised when the client size of listView changes.
        /// </summary>
        private void ListView_ClientSizeChanged(object sender, EventArgs e)
        {
            this.keyHeader.Width = this.listView.ClientSize.Width - this.idHeader.Width;
        }

        /// <summary>
        /// Assign a new valid shortcut when keys are pressed.
        /// </summary>
        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                var item = this.listView.SelectedItems[0] as ShortcutListItem;
                var dialog = new ShortcutModificationDialog(item.SupportsExtended);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.AssignNewShortcut(item, dialog.NewKeys);
                }
            }
        }

        /// <summary>
        /// Raised when the context menu for the list view is opening.
        /// </summary>
        private void ContextMenu_Opening(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count == 0)
            {
                this.removeShortcut.Enabled = false;
            }
            else
            {
                var item = (ShortcutListItem) this.listView.SelectedItems[0];
                this.removeShortcut.Enabled = !item.Custom.IsNone;
                if (item.IsModified)
                {
                    this.revertToDefault.Enabled = true;
                    this.revertAllToDefault.Enabled = true;
                    return;
                }
            }
            this.revertToDefault.Enabled = false;
            this.revertAllToDefault.Enabled = false;
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
        /// Removes the shortcut by setting it to <see cref="Keys.None"/>.
        /// </summary>
        private void RemoveShortcut_Click(object sender, EventArgs e)
        {
            this.RemoveShortcut((ShortcutListItem) this.listView.SelectedItems[0]);
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        private void RevertToDefault_Click(object sender, EventArgs e)
        {
            this.RevertToDefault((ShortcutListItem) this.listView.SelectedItems[0]);
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        private void RevertAllToDefault_Click(object sender, EventArgs e)
        {
            foreach (ShortcutListItem item in this.listView.Items) RevertToDefault(item);
        }

        /// <summary>
        /// Switch to a custom shortcut set.
        /// </summary>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.listView.BeginUpdate();
                ShortcutManager.LoadCustomShortcuts(dialog.FileName, this.shortcutListItems);
                bool conflicts = this.UpdateAllShortcutsConflicts();
                this.listView.EndUpdate();
                if (conflicts)
                {
                    this.ShowConflictsPresent(); // Make sure the warning message shows up after listView is rendered.
                }
            }
        }

        /// <summary>
        /// Save the current shortcut set to a file.
        /// </summary>
        private void ExportButton_Click(object sender, EventArgs e)
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
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        private void ShortcutDialog_FormClosing(object sender, FormClosingEventArgs e)
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
        private void ShortcutDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                this.shortcutListItems[i].ApplyChanges();
            }
            Globals.MainForm.ApplyAllSettings();
        }

        #endregion

        #region ListViewComparer

        /// <summary>
        /// Defines a method that compares two <see cref="ShortcutListItem"/> objects.
        /// </summary>
        internal class ShorcutListItemComparer : IComparer<ShortcutListItem>
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
        internal class ShortcutListItem : ListViewItem, IShortcutItem
        {
            private ShortcutItem item;
            private List<ShortcutListItem> conflicts;
            private ShortcutKeys custom;

            /// <summary>
            /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
            /// </summary>
            public ShortcutListItem(ShortcutItem shortcutItem)
            {
                this.item = shortcutItem;
                this.conflicts = null;
                this.custom = this.Item.Custom;
                this.KeysString = this.Custom.ToString();
                this.Name = this.Text = this.Id;
                this.SubItems.Add(this.KeysString);
                this.ImageIndex = this.SupportsExtended ? 1 : 0;
                UpdateItemHighlightFont(this);
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
            public ShortcutKeys Default
            {
                get { return this.Item.Default; }
            }

            /// <summary>
            /// Gets or sets the custom shortcut keys.
            /// </summary>
            public ShortcutKeys Custom
            {
                get { return this.custom; }
                set
                {
                    if (this.custom == value) return;
                    this.custom = value;
                    this.KeysString = this.custom.ToString();
                    this.SubItems[1].Text = this.KeysString;
                    ResetConflicts(this);
                    UpdateItemHighlightFont(this);
                }
            }

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
                get { return !this.Custom.Equals(this.Default); }
            }

            /// <summary>
            /// Gets whether this item supports extended shortcut keys.
            /// </summary>
            public bool SupportsExtended
            {
                get { return this.Item.SupportsExtended; }
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
