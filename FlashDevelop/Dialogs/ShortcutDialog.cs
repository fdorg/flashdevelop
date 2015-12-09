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
        const string ViewConflictsKey = "?";
        const string ViewCustomKey = "*";
        
        private Timer updateTimer;
        private ToolStripMenuItem removeShortcut;
        private ToolStripMenuItem revertToDefault;
        private ToolStripMenuItem revertAllToDefault;
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
        private ShortcutListItem[] shortcutListItems;

        public ShortcutDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
            this.InitializeShortcutListItems();
            this.PopulateListView(String.Empty);
            this.ApplyScaling();
            this.SetupUpdateTimer();
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
        /// Initializes the graphics
        /// </summary>
        private void InitializeGraphics()
        {
            this.pictureBox.Image = Globals.MainForm.FindImage("229");
            this.clearButton.Image = Globals.MainForm.FindImage("153");
        }

        /// <summary>
        /// Initializes the ListView context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Font = Globals.Settings.DefaultFont;
            cms.Renderer = new DockPanelStripRenderer(false, false);
            this.removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, this.RemoveShortcutClick, Keys.Delete);
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefaultClick);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefaultClick);
            this.removeShortcut.Image = Globals.MainForm.FindImage("153");
            this.revertToDefault.Image = Globals.MainForm.FindImage("69");
            this.revertAllToDefault.Image = Globals.MainForm.FindImage("224");
            cms.Items.Add(this.removeShortcut);
            cms.Items.Add(this.revertToDefault);
            cms.Items.Add(this.revertAllToDefault);
            this.listView.ContextMenuStrip = cms;
            cms.Opening += this.ContextMenuOpening;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
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
        /// Applies additional scaling to controls in order to support HDPI
        /// </summary>
        private void ApplyScaling()
        {
            this.idHeader.Width = ScaleHelper.Scale(this.idHeader.Width);
            this.keyHeader.Width = ScaleHelper.Scale(this.keyHeader.Width);
        }
        
        /// <summary>
        /// Updates the font highlight of the item
        /// </summary>
        private static void UpdateItemHighlightFont(ShortcutListItem item)
        {
            item.ForeColor = item.Conflicts == null ? SystemColors.ControlText : Color.DarkRed;
            item.SubItems[1].ForeColor = item.KeysString == "None" ? SystemColors.GrayText : SystemColors.ControlText;
            if (item.IsModified)
            {
                item.Font = new Font(Globals.Settings.DefaultFont, FontStyle.Bold);
                item.UseItemStyleForSubItems = true;
            }
            else
            {
                item.Font = new Font(Globals.Settings.DefaultFont, FontStyle.Regular);
                item.UseItemStyleForSubItems = false;
            }
        }

        /// <summary>
        /// Initialize the full shortcut list.
        /// </summary>
        private void InitializeShortcutListItems()
        {
            Dictionary<String, ShortcutItem>.ValueCollection collection = ShortcutManager.RegisteredItems.Values;
            this.shortcutListItems = new ShortcutListItem[collection.Count];
            Int32 counter = 0;
            foreach (ShortcutItem item in collection)
            {
                ShortcutListItem shortcutListItem = new ShortcutListItem(item);
                UpdateItemHighlightFont(shortcutListItem);
                this.shortcutListItems[counter++] = shortcutListItem;
            }
            Array.Sort(this.shortcutListItems, new ShorcutListItemComparer());
        }

        /// <summary>
        /// Populates the shortcut list view
        /// </summary>
        private void PopulateListView(String filter)
        {
            Boolean viewCustom = false, viewConflicts = false;
            filter = ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);

            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            foreach (ShortcutListItem item in this.shortcutListItems)
            {
                if (String.IsNullOrEmpty(filter) ||
                    item.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || 
                    item.KeysString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (viewCustom && !item.IsModified) continue;
                    if (viewConflicts && item.Conflicts == null) continue;
                    this.listView.Items.Add(item);
                }
            }
            this.keyHeader.Width = -2;
            this.listView.EndUpdate();
            if (this.listView.Items.Count > 0)
            {
                this.listView.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Reads and removes filter keywords from the start of the filter.
        /// Order of the keywords is irrelevant.
        /// </summary>
        private static String ExtractFilterKeywords(String filter, ref Boolean viewCustom, ref Boolean viewConflicts)
        {
            if (!viewCustom && filter.StartsWith(ViewCustomKey))
            {
                filter = filter.Substring(ViewCustomKey.Length);
                viewCustom = true;
                return ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);
            }
            if (!viewConflicts && filter.StartsWith(ViewConflictsKey))
            {
                filter = filter.Substring(ViewConflictsKey.Length);
                viewConflicts = true;
                return ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);
            }
            return filter.Trim();
        }

        /// <summary>
        /// Raised when the context menu for the list view is opening.
        /// </summary>
        private void ContextMenuOpening(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                ShortcutListItem item = (ShortcutListItem) this.listView.SelectedItems[0];
                this.removeShortcut.Enabled = item.Custom != Keys.None;
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
        /// Assign a new valid shortcut when keys are pressed
        /// </summary>
        private void ListViewKeyDown(Object sender, KeyEventArgs e)
        {
            if (this.listView.SelectedItems.Count == 0) return;

            ShortcutListItem item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.AssignNewShortcut(item, e.KeyData);

            // Don't trigger list view default shortcuts like Ctrl+Add
            if (e.KeyData != Keys.Up && e.KeyData != Keys.Down) e.Handled = true;
        }

        /// <summary>
        /// Assign the new shortcut.
        /// </summary>
        private void AssignNewShortcut(ShortcutListItem item, Keys shortcut, Boolean suppressWarning = false)
        {
            if (shortcut == Keys.None || shortcut == Keys.Delete) shortcut = 0;
            else if (!ToolStripManager.IsValidShortcut(shortcut)) return;

            if (item.Custom == shortcut) return;
            item.Custom = shortcut;
            item.Selected = true;
            ResetConflicts(item);
            List<ShortcutListItem> conflicts = this.GetConflictItems(shortcut);
            if (conflicts == null) UpdateItemHighlightFont(item);
            else
            {
                foreach (ShortcutListItem i in conflicts)
                {
                    i.Conflicts = conflicts;
                    UpdateItemHighlightFont(i);
                }
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
        private static void ResetConflicts(ShortcutListItem item)
        {
            List<ShortcutListItem> conflicts = item.Conflicts;
            if (conflicts == null) return;
            item.Conflicts = null;
            conflicts.Remove(item);
            if (conflicts.Count == 1)
            {
                item = conflicts[0];
                item.Conflicts = null; // empty conflicts list will be garbage collected
                conflicts.RemoveAt(0);
                UpdateItemHighlightFont(item);
            }
        }
        
        /// <summary>
        /// Gets a list of all conflicting entries. Returns null if the length is 0 or 1.
        /// </summary>
        private List<ShortcutListItem> GetConflictItems(Keys keys)
        {
            if (keys == Keys.None) return null;
            // To prevent creation of unnecessary List<T> objects
            ShortcutListItem first = null;
            List<ShortcutListItem> items = null;
            foreach (ShortcutListItem item in this.shortcutListItems)
            {
                if (item.Custom == keys)
                {
                    if (first == null) first = item;
                    else
                    {
                        if (items == null) items = new List<ShortcutListItem> { first };
                        items.Add(item); 
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// Reverts the shortcut to default value
        /// </summary>
        private void RevertToDefaultClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
                this.RevertToDefault((ShortcutListItem) this.listView.SelectedItems[0]);
        }

        /// <summary>
        /// Reverts all shortcut to their default value
        /// </summary>
        private void RevertAllToDefaultClick(Object sender, EventArgs e)
        {
            foreach (ShortcutListItem item in this.listView.Items) this.RevertToDefault(item);
        }

        /// <summary>
        /// Revert the selected items shortcut to default
        /// </summary>
        private void RevertToDefault(ShortcutListItem item)
        {
            this.AssignNewShortcut(item, item.Default);
        }

        /// <summary>
        /// Removes the shortcut by setting it to Keys.None
        /// </summary>
        private void RemoveShortcutClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count == 0) return;
            ShortcutListItem item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.AssignNewShortcut(item, Keys.None);
        }

        /// <summary>
        /// Clears the filter text field
        /// </summary>
        private void ClearFilterClick(Object sender, EventArgs e)
        {
            this.filterTextBox.Text = "";
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
        private void UpdateTimer_Tick(Object sender, EventArgs e)
        {
            this.updateTimer.Enabled = false;
            this.PopulateListView(this.filterTextBox.Text);
        }

        /// <summary>
        /// Restart the timer for updating the list.
        /// </summary>
        private void FilterTextChanged(Object sender, EventArgs e)
        {
            this.updateTimer.Stop();
            this.updateTimer.Start();
        }

        /// <summary>
        /// Switch to a custom shortcut set.
        /// </summary>
        private void SelectCustomShortcut(Object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                String extension = Path.GetExtension(dialog.FileName);
                if (extension.Equals(".fda", StringComparison.OrdinalIgnoreCase))
                {
                    Keys[] shortcuts = ShortcutManager.LoadCustomShortcuts(dialog.FileName, this.shortcutListItems);
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
        private void SaveCustomShortcut(Object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
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
        /// Closes the shortcut dialog
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is about to close, check for any conflicts.
        /// </summary>
        private void DialogClosing(Object sender, FormClosingEventArgs e)
        {
            foreach (ShortcutListItem item in this.shortcutListItems)
            {
                if (item.Conflicts != null)
                {
                    ErrorManager.ShowError(TextHelper.GetString("Info.ShortcutConflictsPresent"), null);
                    this.filterTextBox.Text = ViewConflictsKey;
                    e.Cancel = true;
                    break;
                }
            }
        }

        /// <summary>
        /// When the form is closed, applies shortcuts
        /// </summary>
        private void DialogClosed(Object sender, FormClosedEventArgs e)
        {
            foreach (ShortcutListItem item in this.shortcutListItems)
            {
                item.ApplyChanges();
            }
            Globals.MainForm.ApplyAllSettings();
        }

        /// <summary>
        /// Shows the shortcut dialog
        /// </summary>
        public static new void Show()
        {
            ShortcutDialog shortcutDialog = new ShortcutDialog();
            shortcutDialog.CenterToParent();
            shortcutDialog.Show(Globals.MainForm);
            shortcutDialog.filterTextBox.Focus();
        }

        #endregion

    }

    #region ListViewComparer

    /// <summary>
    /// Defines a method that compares two <see cref="ShortcutListItem"/> objects.
    /// </summary>
    class ShorcutListItemComparer : IComparer<ShortcutListItem>
    {
        Int32 IComparer<ShortcutListItem>.Compare(ShortcutListItem x, ShortcutListItem y)
        {
            return StringComparer.Ordinal.Compare(x.Text, y.Text);
        }
    }

    #endregion

    /// <summary>
    /// Represents a copy of a <see cref="ShortcutItem"/> as well as a visual component.
    /// </summary>
    class ShortcutListItem : ListViewItem, IShortcutItem
    {
        private Keys custom;

        /// <summary>
        /// Gets the associated <see cref="ShortcutItem"/> object.
        /// </summary>
        public ShortcutItem Item { get; private set; }
        /// <summary>
        /// Gets or sets a list of <see cref="ShortcutListItem"/> objects that have conflicting keys with this instance.
        /// </summary>
        public List<ShortcutListItem> Conflicts { get; set; }
        /// <summary>
        /// Gets the ID of the associated <see cref="ShortcutItem"/>.
        /// </summary>
        public String Id { get { return this.Item.Id; } }
        /// <summary>
        /// Gets the default shortcut keys.
        /// </summary>
        public Keys Default { get { return this.Item.Default; } }
        /// <summary>
        /// Gets or sets the custom shortcut keys.
        /// </summary>
        public Keys Custom
        {
            get { return this.custom; }
            set
            {
                this.custom = value;
                this.KeysString = DataConverter.KeysToString(value);
                this.SubItems[1].Text = this.KeysString;
            }
        }
        /// <summary>
        /// Gets the string representation of the custom shortcut keys.
        /// </summary>
        public String KeysString { get; private set; }
        /// <summary>
        /// Gets the modification status of the shortcut.
        /// </summary>
        public Boolean IsModified { get { return this.Custom != this.Default; } }

        /// <summary>
        /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
        /// </summary>
        public ShortcutListItem(ShortcutItem item)
        {
            this.SubItems.Add(string.Empty);
            this.Name = this.Text = item.Id;
            this.Item = item;
            this.Conflicts = null;
            this.Custom = item.Custom;
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
