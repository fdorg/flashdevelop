using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using FlashDevelop.Utilities;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Dialogs
{
    public class SettingDialog : SmartForm
    {
        private System.String helpUrl;
        private System.Windows.Forms.ListView itemListView;
        private System.Windows.Forms.PictureBox infoPictureBox;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private FlashDevelop.Controls.FilteredGrid itemPropertyGrid;
        private System.Windows.Forms.ListViewGroup pluginsGroup;
        private System.Windows.Forms.ListViewGroup mainGroup;
        private System.Windows.Forms.CheckBox disableCheckBox;
        private System.Windows.Forms.Button clearFilterButton;
        private System.Windows.Forms.LinkLabel helpLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox filterText;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label descLabel;
        private String itemFilter = String.Empty;
        private String itemName = String.Empty;
        private static Int32 lastItemIndex = 0;
        private InstalledSDKContext sdkContext;
        private static Hashtable requireRestart = new Hashtable();

        public SettingDialog(String name, String filter)
        {
            this.itemName = name;
            this.itemFilter = filter;
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "48a75ac0-479a-49b9-8ec0-5db7c8d36388";
            this.InitializeComponent();
            this.InitializeGraphics(); 
            this.InitializeItemGroups();
            this.InitializeContextMenu();
            this.ApplyLocalizedTexts();
            this.UpdateInfo();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.itemListView = new System.Windows.Forms.ListViewEx();
            this.filterText = new System.Windows.Forms.TextBoxEx();
            this.itemPropertyGrid = new FlashDevelop.Controls.FilteredGrid();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.nameLabel = new System.Windows.Forms.Label();
            this.infoPictureBox = new System.Windows.Forms.PictureBox();
            this.filterLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.descLabel = new System.Windows.Forms.Label();
            this.disableCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.helpLabel = new System.Windows.Forms.LinkLabel();
            this.clearFilterButton = new System.Windows.Forms.ButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).BeginInit();
            this.SuspendLayout();
            //
            // columnHeader
            //
            this.columnHeader.Width = -1;
            // 
            // itemListView
            //
            this.itemListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.itemListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.itemListView.HideSelection = false;
            this.itemListView.Location = new System.Drawing.Point(12, 12);
            this.itemListView.MultiSelect = false;
            this.itemListView.Name = "itemListView";
            this.itemListView.Size = new System.Drawing.Size(159, 428);
            this.itemListView.TabIndex = 1;
            this.itemListView.UseCompatibleStateImageBehavior = false;
            this.itemListView.View = System.Windows.Forms.View.Details;
            this.itemListView.Alignment = ListViewAlignment.Left;
            this.itemListView.Columns.Add(this.columnHeader);
            this.itemListView.SelectedIndexChanged += new System.EventHandler(this.ItemListViewSelectedIndexChanged);
            // 
            // itemPropertyGrid
            // 
            this.itemPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.itemPropertyGrid.Location = new System.Drawing.Point(183, 54);
            this.itemPropertyGrid.Name = "itemPropertyGrid";
            this.itemPropertyGrid.Size = new System.Drawing.Size(502, 386);
            this.itemPropertyGrid.TabIndex = 3;
            this.itemPropertyGrid.ToolbarVisible = false;
            this.itemPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.itemPropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.PropertyValueChanged);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(586, 447);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.nameLabel.AutoSize = true;
            this.nameLabel.Enabled = false;
            this.nameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nameLabel.Location = new System.Drawing.Point(185, 11);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(111, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "(no item selected)";
            // 
            // infoPictureBox
            //
            this.infoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.infoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoPictureBox.Location = new System.Drawing.Point(13, 451);
            this.infoPictureBox.Name = "infoPictureBox";
            this.infoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.infoPictureBox.TabIndex = 5;
            this.infoPictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoLabel.AutoSize = true;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.Location = new System.Drawing.Point(34, 452);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(501, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Settings will take effect as soon as you edit them successfully but some may require a program restart.";
            // 
            // descLabel
            // 
            this.descLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.descLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.descLabel.Location = new System.Drawing.Point(185, 31);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(350, 13);
            this.descLabel.TabIndex = 6;
            this.descLabel.Text = "Adds a plugin panel to FlashDevelop.";
            // 
            // disableCheckBox
            // 
            this.disableCheckBox.AutoSize = true;
            this.disableCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.disableCheckBox.Location = new System.Drawing.Point(305, 10);
            this.disableCheckBox.Name = "disableCheckBox";
            this.disableCheckBox.Size = new System.Drawing.Size(69, 18);
            this.disableCheckBox.TabIndex = 7;
            this.disableCheckBox.Text = " Disable";
            this.disableCheckBox.UseVisualStyleBackColor = true;
            this.disableCheckBox.Click += new System.EventHandler(this.DisableCheckBoxCheck);
            // 
            // helpLabel
            //
            this.helpLabel.AutoSize = true;
            this.helpLabel.Location = new System.Drawing.Point(369, 11);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(28, 13);
            this.helpLabel.TabIndex = 9;
            this.helpLabel.TabStop = true;
            this.helpLabel.Text = "Help";
            this.helpLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HelpLabelClick);
            // 
            // filterText
            //
            this.filterText.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.filterText.Location = new System.Drawing.Point(537, 26);
            this.filterText.Name = "FilterText";
            this.filterText.Size = new System.Drawing.Size(120, 20);
            this.filterText.TabIndex = 10;
            this.filterText.TextChanged += new System.EventHandler(this.FilterTextTextChanged);
            // 
            // clearFilterButton
            //
            this.clearFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.clearFilterButton.Location = new System.Drawing.Point(661, 24);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(26, 23);
            this.clearFilterButton.TabIndex = 11;
            this.clearFilterButton.UseVisualStyleBackColor = true;
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButtonClick);
            // 
            // filterLabel
            // 
            this.filterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.filterLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.filterLabel.Location = new System.Drawing.Point(538, 10);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(100, 13);
            this.filterLabel.TabIndex = 12;
            this.filterLabel.Text = "Filter settings:";
            // 
            // SettingDialog
            // 
            this.AcceptButton = this.closeButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 482);
            this.Controls.Add(this.clearFilterButton);
            this.Controls.Add(this.filterText);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.helpLabel);
            this.Controls.Add(this.disableCheckBox);
            this.Controls.Add(this.descLabel);
            this.Controls.Add(this.infoPictureBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.itemPropertyGrid);
            this.Controls.Add(this.itemListView);
            this.Controls.Add(this.infoLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(490, 350);
            this.Name = "SettingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Settings";
            this.Load += new System.EventHandler(this.DialogLoad);
            this.Shown += new System.EventHandler(this.DialogShown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogClosed);
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(Globals.MainForm.FindImage("341", false));
            imageList.Images.Add(Globals.MainForm.FindImage("342", false));
            imageList.Images.Add(Globals.MainForm.FindImage("50", false));
            imageList.Images.Add(Globals.MainForm.FindImage("153", false)); // clear
            this.itemListView.SmallImageList = imageList;
            this.clearFilterButton.ImageList = imageList;
            this.clearFilterButton.ImageIndex = 3;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.helpLabel.Text = TextHelper.GetString("Info.Help");
            this.Text = " " + TextHelper.GetString("Title.SettingDialog");
            this.disableCheckBox.Text = " " + TextHelper.GetString("Info.Disable");
            this.filterLabel.Text = TextHelper.GetString("Info.FilterSettings");
            this.nameLabel.Text = TextHelper.GetString("Info.NoItemSelected");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.nameLabel.Font = new Font(this.Font, FontStyle.Bold);
        }

        /// <summary>
        /// Initializes the setting object groups
        /// </summary>
        private void InitializeItemGroups()
        {
            String mainHeader = TextHelper.GetString("Group.Main");
            String pluginsHeader = TextHelper.GetString("Group.Plugins");
            this.mainGroup = new ListViewGroup(mainHeader, HorizontalAlignment.Left);
            this.pluginsGroup = new ListViewGroup(pluginsHeader, HorizontalAlignment.Left);
            this.itemListView.Groups.Add(this.mainGroup);
            this.itemListView.Groups.Add(this.pluginsGroup);
            this.columnHeader.Width = ScaleHelper.Scale(this.columnHeader.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem collapseAll = new ToolStripMenuItem(TextHelper.GetString("Label.CollapseAll"));
            collapseAll.ShortcutKeys = PluginBase.MainForm.GetShortcutItemKeys("ViewMenu.CollapseAll");
            collapseAll.Click += delegate { this.itemPropertyGrid.CollapseAllGridItems(); };
            contextMenu.Items.Add(collapseAll);
            ToolStripMenuItem expandAll = new ToolStripMenuItem(TextHelper.GetString("Label.ExpandAll"));
            expandAll.ShortcutKeys = PluginBase.MainForm.GetShortcutItemKeys("ViewMenu.ExpandAll");
            expandAll.Click += delegate { this.itemPropertyGrid.ExpandAllGridItems(); };
            contextMenu.Items.Add(expandAll);
            this.itemPropertyGrid.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Populates the plugin list
        /// </summary>
        private void PopulatePluginList()
        {
            this.itemListView.Items.Clear();
            Int32 count = PluginServices.AvailablePlugins.Count;
            ListViewItem main = new ListViewItem(DistroConfig.DISTRIBUTION_NAME, 2);
            this.itemListView.BeginUpdate();
            this.itemListView.Items.Add(main);
            this.mainGroup.Items.Add(main);
            for (Int32 i = 0; i < count; i++)
            {
                AvailablePlugin plugin = PluginServices.AvailablePlugins[i];
                ListViewItem item = new ListViewItem(plugin.Instance.Name, 0);
                item.Tag = plugin.Instance;
                if (Globals.Settings.DisabledPlugins.Contains(plugin.Instance.Guid))
                {
                    item.ImageIndex = 1;
                }
                if (!String.IsNullOrEmpty(this.itemFilter)) // Set default filter...
                {
                    this.filterText.TextChanged -= new EventHandler(this.FilterTextTextChanged);
                    this.filterText.Text = this.itemFilter;
                    this.filterText.TextChanged += new EventHandler(this.FilterTextTextChanged);
                }
                if (this.filterText.Text.Length > 0)
                {
                    if (this.CheckIfExist(plugin.Instance, this.filterText.Text))
                    {
                        this.itemListView.Items.Add(item);
                        this.pluginsGroup.Items.Add(item);
                    }
                }
                else
                {
                    this.itemListView.Items.Add(item);
                    this.pluginsGroup.Items.Add(item);
                }
            }
            this.itemListView.EndUpdate();
            this.SelectCorrectItem(itemName);
        }

        /// <summary>
        /// Selects the correct setting item
        /// </summary>
        private void SelectCorrectItem(String itemName)
        {
            for (Int32 i = 0; i < this.itemListView.Items.Count; i++)
            {
                ListViewItem item = this.itemListView.Items[i];
                if (item.Text == itemName)
                {
                    item.Selected = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Shows the selected plugin in the prop grid
        /// </summary>
        private void ItemListViewSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.itemListView.SelectedIndices.Count > 0)
            {
                Int32 selectedIndex = this.itemListView.SelectedIndices[0];
                if (selectedIndex == 0)
                {
                    sdkContext = new InstalledSDKContext(null);
                    this.itemPropertyGrid.Enabled = true;
                    this.itemPropertyGrid.SelectedObject = Globals.Settings;
                    this.descLabel.Text = TextHelper.GetString("Info.AppDescription");
                    this.helpUrl = DistroConfig.DISTRIBUTION_HELP;
                    this.nameLabel.Text = DistroConfig.DISTRIBUTION_NAME;
                    this.nameLabel.Enabled = true;
                    this.ShowInfoControls(false);
                    this.FilterPropertySheet();
                }
                else
                {
                    IPlugin plugin = (IPlugin)itemListView.SelectedItems[0].Tag;
                    sdkContext = new InstalledSDKContext(plugin as InstalledSDKOwner);
                    this.disableCheckBox.Checked = Globals.Settings.DisabledPlugins.Contains(plugin.Guid);
                    this.itemPropertyGrid.SelectedObject = plugin.Settings;
                    this.itemPropertyGrid.Enabled = plugin.Settings != null;
                    this.descLabel.Text = plugin.Description;
                    this.nameLabel.Text = plugin.Name;
                    this.nameLabel.Enabled = true;
                    this.helpUrl = plugin.Help;
                    this.FilterPropertySheet();
                    this.ShowInfoControls(true);
                    this.MoveInfoControls();
                }
            }
            else
            {
                this.nameLabel.Enabled = false;
                this.descLabel.Text = String.Empty;
                this.nameLabel.Text = TextHelper.GetString("Info.NoItemSelected");
                this.itemPropertyGrid.SelectedObject = null;
                this.itemPropertyGrid.Enabled = false;
                this.ShowInfoControls(false);
            }
        }

        /// <summary>
        /// Filter the currently selected property sheet according to the text on the filter box
        /// </summary>
        private void FilterPropertySheet()
        {
            if (PlatformHelper.IsRunningOnMono()) return;
            object settingsObj = this.itemPropertyGrid.SelectedObject;
            string text = this.filterText.Text;
            if (settingsObj != null)
            {
                int i = 0;
                string[] browsables = { "" };
                foreach (PropertyInfo prop in settingsObj.GetType().GetProperties())
                {
                    if (PropertyMatches(prop, text))
                    {
                        Array.Resize(ref browsables, i + 1);
                        browsables[i++] = prop.Name;
                    }
                }
                this.itemPropertyGrid.BrowsableProperties = browsables;
                this.itemPropertyGrid.SelectedObject = settingsObj;
                this.itemPropertyGrid.Refresh();
            }
        }

        /// <summary>
        /// Checks if a propery exist in a plugin
        /// </summary>
        private Boolean CheckIfExist(IPlugin plugin, String text)
        {
            if (plugin.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }
            object settingsObj = plugin.Settings;
            if (settingsObj != null)
            {
                foreach (PropertyInfo prop in settingsObj.GetType().GetProperties())
                {
                    if (PropertyMatches(prop, text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the property matches in any property infos
        /// </summary>
        private Boolean PropertyMatches(PropertyInfo property, String text)
        {
            if (property.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }
            foreach (DisplayNameAttribute attribute in property.GetCustomAttributes(typeof(DisplayNameAttribute), true))
            {
                if (attribute.DisplayName.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            foreach (CategoryAttribute attribute in property.GetCustomAttributes(typeof(CategoryAttribute), true))
            {
                if (attribute.Category.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            foreach (DescriptionAttribute attribute in property.GetCustomAttributes(typeof(DescriptionAttribute), true))
            {
                if (attribute.Description.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Test whether the name of the candidate member contains the specified partial name.
        /// </summary>
        public Boolean PartialName(MemberInfo candidate, Object part)
        {
            return candidate.Name.Contains(part.ToString());
        }

        /// <summary>
        /// When setting value has changed, dispatch event
        /// </summary>
        private void PropertyValueChanged(Object sender, PropertyValueChangedEventArgs e)
        {
            if (this.itemListView.SelectedIndices.Count > 0)
            {
                GridItem changedItem = e.ChangedItem;
                String settingId = this.nameLabel.Text + "." + changedItem.Label.Replace(" ", "");
                TextEvent te = new TextEvent(EventType.SettingChanged, settingId);
                EventManager.DispatchEvent(Globals.MainForm, te);

                if (changedItem.PropertyDescriptor.Attributes.Matches(new RequiresRestartAttribute()))
                {
                    UpdateRestartRequired(settingId, e.OldValue, changedItem.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateRestartRequired(string key, object oldValue, object newValue)
        {
            bool previous = requireRestart.Count > 0;
            if (requireRestart.Contains(key))
            {
                if (requireRestart[key].Equals(newValue))
                {
                    requireRestart.Remove(key);
                }
            }
            else requireRestart.Add(key, oldValue);
            if (requireRestart.Count > 0 != previous) UpdateInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateInfo()
        {
            if (requireRestart.Count > 0)
            {
                this.infoLabel.Text = TextHelper.GetString("Info.RequiresRestart");
                this.infoPictureBox.Image = Globals.MainForm.FindImage("196", false);
            }
            else
            {
                this.infoLabel.Text = TextHelper.GetString("Info.SettingsTakeEffect");
                this.infoPictureBox.Image = Globals.MainForm.FindImage("229", false);
            }
        }

        /// <summary>
        /// Moves the info controls to correct position
        /// </summary>
        private void MoveInfoControls()
        {
            Int32 dcbY = this.disableCheckBox.Location.Y;
            Int32 dcbX = this.nameLabel.Location.X + this.nameLabel.Width + 13;
            Int32 hlX = dcbX + this.disableCheckBox.Width;
            Int32 hlY = this.helpLabel.Location.Y;
            this.disableCheckBox.Location = new Point(dcbX, dcbY);
            this.helpLabel.Location = new Point(hlX, hlY);
        }

        /// <summary>
        /// Shows or hides the info controls
        /// </summary>
        private void ShowInfoControls(Boolean value)
        {
            this.disableCheckBox.Visible = value;
            this.helpLabel.Visible = value;
        }

        /// <summary>
        /// Toggles the enabled state and saves it to settings
        /// </summary>
        private void DisableCheckBoxCheck(Object sender, EventArgs e)
        {
            Int32 selectedIndex = this.itemListView.SelectedIndices[0];
            if (selectedIndex != 0)
            {
                IPlugin plugin = PluginServices.AvailablePlugins[selectedIndex - 1].Instance;
                bool disabled = this.disableCheckBox.Checked;
                if (disabled)
                {
                    this.itemListView.Items[selectedIndex].ImageIndex = 1;
                    Globals.Settings.DisabledPlugins.Add(plugin.Guid);
                }
                else
                {
                    this.itemListView.Items[selectedIndex].ImageIndex = 0;
                    Globals.Settings.DisabledPlugins.Remove(plugin.Guid);
                }
                UpdateRestartRequired(nameLabel.Text, !disabled, disabled);
            }
        }

        /// <summary>
        /// Populate plugin list before dialog is shown
        /// </summary>
        private void DialogLoad(Object sender, EventArgs e)
        {
            this.PopulatePluginList();
        }

        /// <summary>
        /// Restore the selected index - only if an item id hasn't been provided
        /// </summary>
        private void DialogShown(Object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.itemName) || this.itemName == DistroConfig.DISTRIBUTION_NAME)
            {
                this.itemListView.SelectedIndices.Add(lastItemIndex);
                this.itemListView.EnsureVisible(lastItemIndex);
            }
            this.filterText.Focus();
        }

        /// <summary>
        /// Save the last selected index to a static var
        /// </summary>
        private void DialogClosing(Object sender, FormClosingEventArgs e)
        {
            lastItemIndex = itemListView.SelectedIndices.Count > 0 ? itemListView.SelectedIndices[0] : 0;
        }

        /// <summary>
        /// When the form is closed applies settings
        /// </summary>
        private void DialogClosed(Object sender, FormClosedEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;  // Enough for our sync code
            if (sdkContext != null) sdkContext.Dispose();
            Globals.MainForm.ApplyAllSettings();
            Globals.MainForm.SaveSettings();
            if (requireRestart.Count > 0 && !Globals.MainForm.RequiresRestart) Globals.MainForm.RestartRequired();
        }

        /// <summary>
        /// Event for changing the filter text
        /// </summary>
        private void FilterTextTextChanged(Object sender, EventArgs e)
        {
            this.PopulatePluginList();
            this.FilterPropertySheet();
        }

        /// <summary>
        /// Event for pressing the filter clear button
        /// </summary>
        private void ClearFilterButtonClick(Object sender, EventArgs e)
        {
            this.filterText.Text = "";
        }

        /// <summary>
        /// Browses to the specified help url
        /// </summary>
        private void HelpLabelClick(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.helpUrl != null)
            {
                Globals.MainForm.CallCommand("Browse", this.helpUrl);
            }
        }

        /// <summary>
        /// Closes the setting dialog
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Shows the settings dialog
        /// </summary>
        public static void Show(String itemName, String filter)
        {
            using (SettingDialog settingDialog = new SettingDialog(itemName, filter))
            {
                settingDialog.closeButton.Select();
                settingDialog.ShowDialog();
            }
        }

        #endregion

    }

}