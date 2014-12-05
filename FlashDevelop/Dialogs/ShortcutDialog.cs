using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using FlashDevelop.Managers;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore.Utilities;
using System.Collections.Generic;
using PluginCore.Managers;
using PluginCore.Helpers;

namespace FlashDevelop.Dialogs
{
    public class ShortcutDialog : SmartForm
    {
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
        private System.Windows.Forms.CheckBox viewCustom;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button closeButton;

        public ShortcutDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
            this.PopulateListView("", false);
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
            this.viewCustom = new System.Windows.Forms.CheckBox();
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
            this.listView.Location = new System.Drawing.Point(12, 62);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(562, 312);
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
            this.searchLabel.Location = new System.Drawing.Point(10, 10);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search:";
            // 
            // clearButton
            //
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(549, 30);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(26, 23);
            this.clearButton.TabIndex = 2;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearFilterClick);
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(12, 32);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(530, 20);
            this.filterTextBox.TabIndex = 1;
            this.filterTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextChanged);
            // 
            // viewCustom
            //
            this.viewCustom.AutoSize = true;
            this.viewCustom.CheckAlign = ContentAlignment.MiddleRight;
            this.viewCustom.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.viewCustom.Location = new System.Drawing.Point(471, 9);
            this.viewCustom.Click += new System.EventHandler(this.ViewCustomClick);
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
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.viewCustom);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.searchLabel);
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
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(Globals.MainForm.FindImage("153")); // clear
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.pictureBox.Image = Globals.MainForm.FindImage("229");
            this.clearButton.ImageList = imageList;
            this.clearButton.ImageIndex = 0;
        }

        /// <summary>
        /// Initializes the ListView context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Font = Globals.Settings.DefaultFont;
            cms.Renderer = new DockPanelStripRenderer(false);
            this.removeShortcut = new ToolStripMenuItem(TextHelper.GetString("Label.RemoveShortcut"), null, this.RemoveShortcutClick);
            this.removeShortcut.ShortcutKeys = Keys.Delete;
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefaultClick);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefaultClick);
            cms.Items.Add(this.removeShortcut);
            cms.Items.Add(this.revertToDefault);
            cms.Items.Add(this.revertAllToDefault);
            this.listView.ContextMenuStrip = cms;
            this.listView.ContextMenuStrip.Opening += this.ContextMenuOpening;
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
            this.viewCustom.Text = TextHelper.GetString("Label.ViewCustom");
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
        /// Gets the keys as a string
        /// </summary>
        private String GetKeysAsString(Keys keys)
        {
            return DataConverter.KeysToString(keys);
        }

        /// <summary>
        /// Updates the font highlight of the item
        /// </summary>
        private void UpdateItemHighlightFont(ListViewItem lvi, ShortcutItem si)
        {
            Font def = Globals.Settings.DefaultFont;
            ListViewItem.ListViewSubItem lvsi = lvi.SubItems[1];
            if (lvsi.Text == "None") lvsi.ForeColor = SystemColors.GrayText;
            else lvsi.ForeColor = SystemColors.ControlText;
            if (si.Default != si.Custom)
            {
                lvi.Font = new Font(def, FontStyle.Bold);
                lvi.UseItemStyleForSubItems = true;
            }
            else
            {
                lvi.Font = new Font(def, FontStyle.Regular);
                lvi.UseItemStyleForSubItems = false;
            }
        }

        /// <summary>
        /// Populates the shortcut list view
        /// </summary>
        private void PopulateListView(String filter, Boolean viewCustom)
        {
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            this.listView.ListViewItemSorter = new ListViewComparer();
            foreach (ShortcutItem item in ShortcutManager.RegisteredItems)
            {
                if (!this.listView.Items.ContainsKey(item.Id) && 
                    (item.Id.ToLower().Contains(filter.ToLower()) || 
                    GetKeysAsString(item.Custom).ToLower().Contains(filter.ToLower())))
                {
                    if (viewCustom && item.Custom == item.Default) continue;
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = lvi.Name = item.Id; lvi.Tag = item;
                    lvi.SubItems.Add(GetKeysAsString(item.Custom));
                    this.UpdateItemHighlightFont(lvi, item);
                    this.listView.Items.Add(lvi);
                }
            }
            this.listView.Sort();
            this.keyHeader.Width = -2;
            this.listView.EndUpdate();
            if (this.listView.Items.Count > 0)
            {
                ListViewItem item = this.listView.Items[0];
                item.Selected = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ContextMenuOpening(Object sender, EventArgs e)
        {
            Boolean customShortcut = false;
            if (this.listView.SelectedItems.Count > 0)
            {
                ShortcutItem current = this.listView.SelectedItems[0].Tag as ShortcutItem;
                this.removeShortcut.Enabled = current.Custom != Keys.None;
                this.revertToDefault.Enabled = current.Custom != current.Default;
            }
            else this.removeShortcut.Enabled = revertToDefault.Enabled = false;
            foreach (ListViewItem item in listView.Items)
            {
                ShortcutItem current = item.Tag as ShortcutItem;
                if (current.Custom != current.Default)
                {
                    customShortcut = true;
                    break;
                }
            }
            this.revertAllToDefault.Enabled = customShortcut;
        }

        /// <summary>
        /// Assign a new valid shortcut when keys are pressed
        /// </summary>
        private void ListViewKeyDown(Object sender, KeyEventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                if (e.KeyData == Keys.Delete) this.RemoveShortcutClick(null, null);
                else 
                {
                    ListViewItem selected = this.listView.SelectedItems[0];
                    ShortcutItem item = selected.Tag as ShortcutItem;
                    if (item.Custom != e.KeyData && ToolStripManager.IsValidShortcut(e.KeyData))
                    {
                        selected.SubItems[1].Text = GetKeysAsString(e.KeyData);
                        item.Custom = e.KeyData; selected.Selected = true;
                        this.UpdateItemHighlightFont(selected, item);
                        if (this.CountItemsByKey(e.KeyData) > 1)
                        {
                            String message = TextHelper.GetString("Info.ShortcutIsAlreadyUsed");
                            ErrorManager.ShowWarning(message, null);
                            this.filterTextBox.Focus(); // Set focus to filter...
                            this.filterTextBox.Text = GetKeysAsString(e.KeyData);
                            this.filterTextBox.SelectAll();
                        }
                    }
                    // Don't trigger list view default shortcuts like Ctrl+Add
                    if (e.KeyData != Keys.Up && e.KeyData != Keys.Down) e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Filter the list view for custom items
        /// </summary>
        private void ViewCustomClick(Object sender, EventArgs e)
        {
            this.FilterTextChanged(null, null);
        }

        /// <summary>
        /// Gets the count of items with the specified keys
        /// </summary>
        private Int32 CountItemsByKey(Keys keys)
        {
            Int32 counter = 0;
            foreach (ListViewItem item in this.listView.Items)
            {
                ShortcutItem si = item.Tag as ShortcutItem;
                if (si.Custom == keys) counter++;
            }
            return counter;
        }

        /// <summary>
        /// Reverts the shortcut to default value
        /// </summary>
        private void RevertToDefaultClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0) this.RevertToDefault(listView.SelectedItems[0]);
        }

        /// <summary>
        /// Reverts all shortcut to their default value
        /// </summary>
        private void RevertAllToDefaultClick(Object sender, EventArgs e)
        {
            foreach (ListViewItem listItem in listView.Items) this.RevertToDefault(listItem);
        }

        /// <summary>
        /// Revert the selected items shortcut to default
        /// </summary>
        private void RevertToDefault(ListViewItem listItem)
        {
            ShortcutItem item = listItem.Tag as ShortcutItem;
            listItem.SubItems[1].Text = this.GetKeysAsString(item.Default);
            item.Custom = item.Default;
            this.UpdateItemHighlightFont(listItem, item);
        }

        /// <summary>
        /// Removes the shortcut by setting it to Keys.None
        /// </summary>
        private void RemoveShortcutClick(Object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                ListViewItem selected = this.listView.SelectedItems[0];
                ShortcutItem item = selected.Tag as ShortcutItem;
                selected.SubItems[1].Text = GetKeysAsString(Keys.None);
                item.Custom = Keys.None;
                this.UpdateItemHighlightFont(selected, item);
            }
        }

        /// <summary>
        /// Clears the filter text field
        /// </summary>
        private void ClearFilterClick(Object sender, EventArgs e)
        {
            this.viewCustom.Checked = false;
            this.filterTextBox.Text = "";
        }

        /// <summary>
        /// Updated the list with the filter
        /// </summary>
        private void FilterTextChanged(Object sender, EventArgs e)
        {
            String searchText = this.filterTextBox.Text.Trim();
            this.PopulateListView(searchText, viewCustom.Checked);
        }

        /// <summary>
        /// Closes the shortcut dialog
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is closed, applies shortcuts
        /// </summary>
        private void DialogClosed(Object sender, FormClosedEventArgs e)
        {
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

    class ListViewComparer : IComparer
    {
        public Int32 Compare(Object x, Object y)
        {
            ListViewItem castX = x as ListViewItem;
            ListViewItem castY = y as ListViewItem;
            return StringComparer.Ordinal.Compare(castX.Text, castY.Text);
        }
    }

    #endregion

}
