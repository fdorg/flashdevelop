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
        Timer updateTimer;
        ToolStripMenuItem removeShortcut;
        ToolStripMenuItem revertToDefault;
        ToolStripMenuItem revertAllToDefault;
        ShortcutListItem[] shortcutListItems;
        Label infoLabel;
        Label searchLabel;
        ListView listView;
        PictureBox pictureBox;
        ColumnHeader idHeader;
        ColumnHeader keyHeader;
        TextBox filterTextBox;
        Button clearButton;
        Button closeButton;
        Button importButton;
        Button exportButton;
        const char ViewConflictsKey = '?';
        const char ViewCustomKey = '*';

        public ShortcutDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            InitializeComponent();
            InitializeContextMenu();
            ApplyLocalizedTexts();
            SetupUpdateTimer();
            InitializeGraphics();
            InitializeShortcutListItems();
            PopulateListView(string.Empty);
            ApplyScaling();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            searchLabel = new Label();
            filterTextBox = new TextBoxEx();
            clearButton = new ButtonEx();
            idHeader = new ColumnHeader();
            keyHeader = new ColumnHeader();
            listView = new ListViewEx();
            pictureBox = new PictureBox();
            infoLabel = new Label();
            importButton = new ButtonEx();
            exportButton = new ButtonEx();
            closeButton = new ButtonEx();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            searchLabel.Location = new Point(10, 10);
            searchLabel.Name = "searchLabel";
            // 
            // filterTextBox
            // 
            filterTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filterTextBox.Location = new Point(12, 32);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new Size(561, 20);
            filterTextBox.TabIndex = 0;
            filterTextBox.ForeColor = SystemColors.GrayText;
            filterTextBox.TextChanged += FilterTextChanged;
            // 
            // clearButton
            // 
            clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            clearButton.Location = new Point(579, 30);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(26, 23);
            clearButton.TabIndex = 1;
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearFilterClick;
            // 
            // idHeader
            // 
            idHeader.Width = 350;
            // 
            // keyHeader
            // 
            keyHeader.Width = 239;
            // 
            // listView
            // 
            listView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView.Columns.AddRange(new[] { idHeader, keyHeader });
            listView.GridLines = true;
            listView.FullRowSelect = true;
            listView.Location = new Point(12, 62);
            listView.MultiSelect = false;
            listView.Name = "listView222";
            listView.Size = new Size(592, 312);
            listView.TabIndex = 2;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.KeyDown += ListViewKeyDown;
            // 
            // pictureBox
            // 
            pictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox.Location = new Point(12, 386);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(16, 16);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            infoLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            infoLabel.Location = new Point(33, 378);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(420, 32);
            infoLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // importButton
            // 
            importButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            importButton.Location = new Point(454, 382);
            importButton.Name = "openButton";
            importButton.Size = new Size(25, 23);
            importButton.TabIndex = 3;
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += SelectCustomShortcut;
            // 
            // exportButton
            // 
            exportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            exportButton.Location = new Point(484, 382);
            exportButton.Name = "saveButton";
            exportButton.Size = new Size(25, 23);
            exportButton.TabIndex = 4;
            exportButton.UseVisualStyleBackColor = true;
            exportButton.Click += SaveCustomShortcut;
            // 
            // closeButton
            // 
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(515, 382);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(90, 23);
            closeButton.TabIndex = 5;
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButtonClick;
            // 
            // ShortcutDialog
            // 
            ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Name = "ShortcutDialog";
            AcceptButton = closeButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(616, 418);
            MinimumSize = new Size(400, 250);
            Controls.Add(searchLabel);
            Controls.Add(filterTextBox);
            Controls.Add(clearButton);
            Controls.Add(listView);
            Controls.Add(pictureBox);
            Controls.Add(importButton);
            Controls.Add(exportButton);
            Controls.Add(infoLabel);
            Controls.Add(closeButton);
            FormClosing += DialogClosing;
            FormClosed += DialogClosed;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        void InitializeGraphics()
        {
            pictureBox.Image = PluginBase.MainForm.FindImage16("229", false);
            clearButton.Image = PluginBase.MainForm.FindImage16("153", false);
            exportButton.Image = PluginBase.MainForm.FindImage16("55|9|3|3", false);
            importButton.Image = PluginBase.MainForm.FindImage16("55|1|3|3", false);
        }

        /// <summary>
        /// Initializes the <see cref="ListView"/> context menu.
        /// </summary>
        void InitializeContextMenu()
        {
            var cms = new ContextMenuStrip();
            cms.Font = PluginBase.Settings.DefaultFont;
            cms.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            cms.Renderer = new DockPanelStripRenderer(false, false);
            removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, RemoveShortcutClick);
            revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, RevertToDefaultClick);
            revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, RevertAllToDefaultClick);
            removeShortcut.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Delete);
            removeShortcut.Image = PluginBase.MainForm.FindImage("153", false);
            revertToDefault.Image = PluginBase.MainForm.FindImage("69", false);
            revertAllToDefault.Image = PluginBase.MainForm.FindImage("224", false);
            cms.Items.Add(removeShortcut);
            cms.Items.Add(revertToDefault);
            cms.Items.Add(revertAllToDefault);
            cms.Opening += ContextMenuOpening;
            listView.ContextMenuStrip = cms;
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            idHeader.Text = TextHelper.GetString("Label.Command");
            keyHeader.Text = TextHelper.GetString("Label.Shortcut");
            infoLabel.Text = TextHelper.GetString("Info.ShortcutEditInfo");
            closeButton.Text = TextHelper.GetString("Label.Close");
            tooltip.SetToolTip(importButton, TextHelper.GetStringWithoutMnemonics("Label.Import"));
            tooltip.SetToolTip(exportButton, TextHelper.GetStringWithoutMnemonics("Label.Export"));
            searchLabel.Text = TextHelper.GetString("Label.ShortcutSearch");
            Text = " " + TextHelper.GetString("Title.Shortcuts");
        }

        /// <summary>
        /// Applies additional scaling to controls in order to support HDPI.
        /// </summary>
        void ApplyScaling()
        {
            idHeader.Width = ScaleHelper.Scale(idHeader.Width);
            keyHeader.Width = ScaleHelper.Scale(keyHeader.Width);
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
            item.Font = new Font(PluginBase.Settings.DefaultFont, item.IsModified ? FontStyle.Bold : 0);
            item.UseItemStyleForSubItems = item.IsModified;
        }

        /// <summary>
        /// Initialize the full shortcut list.
        /// </summary>
        void InitializeShortcutListItems()
        {
            var collection = ShortcutManager.RegisteredItems.Values;
            shortcutListItems = new ShortcutListItem[collection.Count];
            int counter = 0;
            foreach (var item in collection)
            {
                shortcutListItems[counter++] = new ShortcutListItem(item);
            }
            Array.Sort(shortcutListItems, new ShortcutListItemComparer());
            UpdateAllShortcutsConflicts();
        }

        /// <summary>
        /// Update conflicts statuses of all shortcut items.
        /// </summary>
        bool UpdateAllShortcutsConflicts()
        {
            bool conflicts = false;
            foreach (var item in shortcutListItems)
            {
                if (item.HasConflicts)
                {
                    conflicts = true;
                }
                else
                {
                    GetConflictItems(item);
                    conflicts = conflicts || item.HasConflicts;
                }
            }
            return conflicts;
        }

        /// <summary>
        /// Display a warning message to show conflicts.
        /// </summary>
        bool ShowConflictsPresent()
        {
            string text = TextHelper.GetString("Info.ShortcutConflictsPresent");
            string caption = TextHelper.GetString("Title.WarningDialog");
            if (MessageBox.Show(this, text, " " + caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                filterTextBox.Text = ViewConflictsKey.ToString();
                filterTextBox.SelectAll();
                filterTextBox.Focus();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates the shortcut list view.
        /// </summary>
        void PopulateListView(string filter)
        {
            bool viewCustom = false, viewConflicts = false;
            filter = ExtractFilterKeywords(filter, ref viewCustom, ref viewConflicts);
            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var item in shortcutListItems)
            {
                if (string.IsNullOrEmpty(filter) ||
                    item.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.KeysString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (viewCustom && !item.IsModified) continue;
                    if (viewConflicts && !item.HasConflicts) continue;
                    listView.Items.Add(item);
                }
            }
            keyHeader.Width = -2;
            listView.EndUpdate();
            if (listView.Items.Count > 0) listView.Items[0].Selected = true;
        }

        /// <summary>
        /// Reads and removes filter keywords from the start of the filter.
        /// The order of the keywords is irrelevant.
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
            if (listView.SelectedItems.Count > 0)
            {
                var item = (ShortcutListItem) listView.SelectedItems[0];
                removeShortcut.Enabled = item.Custom != 0;
                revertToDefault.Enabled = revertAllToDefault.Enabled = item.IsModified;
                if (revertAllToDefault.Enabled) return;
            }
            else removeShortcut.Enabled = revertToDefault.Enabled = false;
            foreach (ShortcutListItem item in listView.Items)
            {
                if (item.IsModified)
                {
                    revertAllToDefault.Enabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Assign a new valid shortcut when keys are pressed.
        /// </summary>
        void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;
            var item = (ShortcutListItem) listView.SelectedItems[0];
            AssignNewShortcut(item, e.KeyData);
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
        void AssignNewShortcut(ShortcutListItem item, Keys shortcut)
        {
            if (shortcut == 0 || shortcut == Keys.Delete) shortcut = 0;
            else if (!ToolStripManager.IsValidShortcut(shortcut)) return;
            if (item.Custom == shortcut) return;
            listView.BeginUpdate();
            var oldShortcut = item.Custom;
            item.Custom = shortcut;
            item.Selected = true;
            GetConflictItems(item);
            listView.EndUpdate();
            if (item.HasConflicts)
            {
                string text = TextHelper.GetString("Info.ShortcutIsAlreadyUsed");
                string caption = TextHelper.GetString("Title.WarningDialog");
                switch (MessageBox.Show(PluginBase.MainForm, text, " " + caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                {
                    case DialogResult.Abort:
                        listView.BeginUpdate();
                        item.Custom = oldShortcut;
                        GetConflictItems(item);
                        listView.EndUpdate();
                        break;
                    case DialogResult.Retry:
                        filterTextBox.Text = ViewConflictsKey + item.KeysString;
                        filterTextBox.SelectAll();
                        filterTextBox.Focus(); // Set focus to filter...
                        break;
                }
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
            List<ShortcutListItem> conflicts = null;
            foreach (var item in shortcutListItems)
            {
                if (item.Custom != keys || item == target) continue;
                if (conflicts is null) conflicts = new List<ShortcutListItem> { target };
                conflicts.Add(item);
                item.Conflicts = conflicts;
            }
            target.Conflicts = conflicts;
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        void RevertToDefaultClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                RevertToDefault((ShortcutListItem) listView.SelectedItems[0]);
            }
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        void RevertAllToDefaultClick(object sender, EventArgs e)
        {
            foreach (ShortcutListItem item in listView.Items) RevertToDefault(item);
        }

        /// <summary>
        /// Revert the selected items shortcut to default.
        /// </summary>
        void RevertToDefault(ShortcutListItem item)
        {
            AssignNewShortcut(item, item.Default);
        }

        /// <summary>
        /// Removes the shortcut by setting it to <see cref="Keys.None"/>.
        /// </summary>
        void RemoveShortcutClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                AssignNewShortcut((ShortcutListItem) listView.SelectedItems[0], 0);
            }
        }

        /// <summary>
        /// Clears the filter text field.
        /// </summary>
        void ClearFilterClick(object sender, EventArgs e)
        {
            filterTextBox.Text = string.Empty;
            filterTextBox.Select();
        }

        /// <summary>
        /// Set up the timer for delayed list update with filters.
        /// </summary>
        void SetupUpdateTimer()
        {
            updateTimer = new Timer();
            updateTimer.Enabled = false;
            updateTimer.Interval = 100;
            updateTimer.Tick += UpdateTimer_Tick;
        }

        /// <summary>
        /// Update the list with filter.
        /// </summary>
        void UpdateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;
            PopulateListView(filterTextBox.Text);
        }

        /// <summary>
        /// Restart the timer for updating the list.
        /// </summary>
        void FilterTextChanged(object sender, EventArgs e)
        {
            updateTimer.Stop();
            updateTimer.Start();
        }

        /// <summary>
        /// Switch to a custom shortcut set.
        /// </summary>
        void SelectCustomShortcut(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                listView.BeginUpdate();
                ShortcutManager.LoadCustomShortcuts(dialog.FileName, shortcutListItems);
                bool conflicts = UpdateAllShortcutsConflicts();
                listView.EndUpdate();
                if (conflicts) ShowConflictsPresent(); // Make sure the warning message shows up after the listview is rendered
            }
        }

        /// <summary>
        /// Save the current shortcut set to a file.
        /// </summary>
        void SaveCustomShortcut(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
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
                ShortcutManager.SaveCustomShortcuts(dialog.FileName, shortcutListItems);
            }
        }

        /// <summary>
        /// Closes the shortcut dialog.
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        void DialogClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var it in shortcutListItems)
            {
                if (it.HasConflicts)
                {
                    e.Cancel = ShowConflictsPresent();
                    break;
                }
            }
        }

        /// <summary>
        /// When the form is closed, applies shortcuts.
        /// </summary>
        void DialogClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var it in shortcutListItems)
                it.ApplyChanges();

            Globals.MainForm.ApplyAllSettings();
            ShortcutManager.SaveCustomShortcuts();
        }

        /// <summary>
        /// Shows the shortcut dialog.
        /// </summary>
        public new static void Show()
        {
            /*using*/ var dialog = new ShortcutDialog();
            dialog.CenterToParent();
            dialog.Show(PluginBase.MainForm);
            dialog.filterTextBox.Focus();
        }

        #endregion

        #region ListViewComparer

        /// <summary>
        /// Defines a method that compares two <see cref="ShortcutListItem"/> objects.
        /// </summary>
        class ShortcutListItemComparer : IComparer<ShortcutListItem>
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
            List<ShortcutListItem> conflicts;
            Keys custom;

            /// <summary>
            /// Gets the associated <see cref="ShortcutItem"/> object.
            /// </summary>
            public ShortcutItem Item { get; }

            /// <summary>
            /// Gets whether this <see cref="ShortcutListItem"/> has other conflicting <see cref="ShortcutListItem"/> objects.
            /// </summary>
            public bool HasConflicts => Conflicts != null;

            /// <summary>
            /// Gets or sets a collection of <see cref="ShortcutListItem"/> objects that have conflicting keys with this instance.
            /// </summary>
            public List<ShortcutListItem> Conflicts
            {
                get => conflicts;
                set
                {
                    if (conflicts == value) return;
                    conflicts = value;
                    UpdateItemHighlightFont(this);
                }
            }

            /// <summary>
            /// Gets the ID of the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public string Id => Item.Id;

            /// <summary>
            /// Gets the default shortcut keys.
            /// </summary>
            public Keys Default => Item.Default;

            /// <summary>
            /// Gets or sets the custom shortcut keys.
            /// </summary>
            public Keys Custom
            {
                get => custom;
                set
                {
                    if (custom == value) return;
                    custom = value;
                    KeysString = DataConverter.KeysToString(custom);
                    SubItems[1].Text = KeysString;
                    ResetConflicts(this);
                    UpdateItemHighlightFont(this);
                }
            }

            /// <summary>
            /// Gets the string representation of the custom shortcut keys.
            /// </summary>
            public string KeysString { get; private set; }

            /// <summary>
            /// Gets the modification status of the shortcut.
            /// </summary>
            public bool IsModified => Custom != Default;

            /// <summary>
            /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
            /// </summary>
            public ShortcutListItem(ShortcutItem shortcutItem)
            {
                Item = shortcutItem;
                conflicts = null;
                custom = Item.Custom;
                KeysString = DataConverter.KeysToString(Custom);
                Name = Text = Id;
                SubItems.Add(KeysString);
                UpdateItemHighlightFont(this);
            }

            /// <summary>
            /// Apply changes made to this instance to the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public void ApplyChanges() => Item.Custom = Custom;
        }

        #endregion
    }
}