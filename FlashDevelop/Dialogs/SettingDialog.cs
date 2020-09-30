using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
        string helpUrl;
        ListView itemListView;
        PictureBox infoPictureBox;
        ColumnHeader columnHeader;
        Controls.FilteredGrid itemPropertyGrid;
        ListViewGroup pluginsGroup;
        ListViewGroup mainGroup;
        CheckBox disableCheckBox;
        Button clearFilterButton;
        LinkLabel helpLabel;
        Button closeButton;
        TextBox filterText;
        Label filterLabel;
        Label nameLabel;
        Label infoLabel;
        Label descLabel;
        readonly string itemFilter = string.Empty;
        readonly string itemName = string.Empty;
        static int lastItemIndex = 0;
        InstalledSDKContext sdkContext;
        static readonly Hashtable requireRestart = new Hashtable();

        public SettingDialog(string name, string filter)
        {
            itemName = name;
            itemFilter = filter;
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "48a75ac0-479a-49b9-8ec0-5db7c8d36388";
            InitializeComponent();
            InitializeGraphics(); 
            InitializeItemGroups();
            InitializeContextMenu();
            ApplyLocalizedTexts();
            UpdateInfo();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            itemListView = new ListViewEx();
            filterText = new TextBoxEx();
            itemPropertyGrid = new Controls.FilteredGrid();
            columnHeader = new ColumnHeader();
            closeButton = new ButtonEx();
            nameLabel = new Label();
            infoPictureBox = new PictureBox();
            filterLabel = new Label();
            infoLabel = new Label();
            descLabel = new Label();
            disableCheckBox = new CheckBoxEx();
            helpLabel = new LinkLabel();
            clearFilterButton = new ButtonEx();
            ((ISupportInitialize)(infoPictureBox)).BeginInit();
            SuspendLayout();
            //
            // columnHeader
            //
            columnHeader.Width = -1;
            // 
            // itemListView
            //
            itemListView.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left;
            itemListView.HeaderStyle = ColumnHeaderStyle.None;
            itemListView.HideSelection = false;
            itemListView.Location = new Point(12, 12);
            itemListView.MultiSelect = false;
            itemListView.Name = "itemListView";
            itemListView.Size = new Size(159, 428);
            itemListView.TabIndex = 1;
            itemListView.UseCompatibleStateImageBehavior = false;
            itemListView.View = View.Details;
            itemListView.Alignment = ListViewAlignment.Left;
            itemListView.Columns.Add(columnHeader);
            itemListView.SelectedIndexChanged += ItemListViewSelectedIndexChanged;
            // 
            // itemPropertyGrid
            // 
            itemPropertyGrid.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            itemPropertyGrid.Location = new Point(183, 54);
            itemPropertyGrid.Name = "itemPropertyGrid";
            itemPropertyGrid.Size = new Size(502, 386);
            itemPropertyGrid.TabIndex = 3;
            itemPropertyGrid.ToolbarVisible = false;
            itemPropertyGrid.PropertySort = PropertySort.Categorized;
            itemPropertyGrid.PropertyValueChanged += PropertyValueChanged;
            // 
            // closeButton
            // 
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(586, 447);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(100, 23);
            closeButton.TabIndex = 4;
            closeButton.Text = "&Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButtonClick;
            // 
            // nameLabel
            // 
            nameLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            nameLabel.AutoSize = true;
            nameLabel.Enabled = false;
            nameLabel.FlatStyle = FlatStyle.System;
            nameLabel.Location = new Point(185, 11);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(111, 13);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "(no item selected)";
            // 
            // infoPictureBox
            //
            infoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            infoPictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            infoPictureBox.Location = new Point(13, 451);
            infoPictureBox.Name = "infoPictureBox";
            infoPictureBox.Size = new Size(16, 16);
            infoPictureBox.TabIndex = 5;
            infoPictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            infoLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            infoLabel.AutoSize = true;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.Location = new Point(34, 452);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(501, 13);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "Settings will take effect as soon as you edit them successfully but some may require a program restart.";
            // 
            // descLabel
            // 
            descLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            descLabel.FlatStyle = FlatStyle.System;
            descLabel.Location = new Point(185, 31);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(350, 13);
            descLabel.TabIndex = 6;
            descLabel.Text = "Adds a plugin panel to FlashDevelop.";
            // 
            // disableCheckBox
            // 
            disableCheckBox.AutoSize = true;
            disableCheckBox.FlatStyle = FlatStyle.System;
            disableCheckBox.Location = new Point(305, 10);
            disableCheckBox.Name = "disableCheckBox";
            disableCheckBox.Size = new Size(69, 18);
            disableCheckBox.TabIndex = 7;
            disableCheckBox.Text = " Disable";
            disableCheckBox.UseVisualStyleBackColor = true;
            disableCheckBox.Click += DisableCheckBoxCheck;
            // 
            // helpLabel
            //
            helpLabel.AutoSize = true;
            helpLabel.Location = new Point(369, 11);
            helpLabel.Name = "helpLabel";
            helpLabel.Size = new Size(28, 13);
            helpLabel.TabIndex = 9;
            helpLabel.TabStop = true;
            helpLabel.Text = "Help";
            helpLabel.LinkClicked += HelpLabelClick;
            // 
            // filterText
            //
            filterText.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            filterText.Location = new Point(537, 26);
            filterText.Name = "FilterText";
            filterText.Size = new Size(120, 20);
            filterText.TabIndex = 10;
            filterText.TextChanged += FilterTextTextChanged;
            // 
            // clearFilterButton
            //
            clearFilterButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            clearFilterButton.Location = new Point(661, 24);
            clearFilterButton.Name = "clearFilterButton";
            clearFilterButton.Size = new Size(26, 23);
            clearFilterButton.TabIndex = 11;
            clearFilterButton.UseVisualStyleBackColor = true;
            clearFilterButton.Click += ClearFilterButtonClick;
            // 
            // filterLabel
            // 
            filterLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            filterLabel.FlatStyle = FlatStyle.System;
            filterLabel.Location = new Point(538, 10);
            filterLabel.Name = "filterLabel";
            filterLabel.Size = new Size(100, 13);
            filterLabel.TabIndex = 12;
            filterLabel.Text = "Filter settings:";
            // 
            // SettingDialog
            // 
            AcceptButton = closeButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(697, 482);
            Controls.Add(clearFilterButton);
            Controls.Add(filterText);
            Controls.Add(filterLabel);
            Controls.Add(helpLabel);
            Controls.Add(disableCheckBox);
            Controls.Add(descLabel);
            Controls.Add(infoPictureBox);
            Controls.Add(nameLabel);
            Controls.Add(closeButton);
            Controls.Add(itemPropertyGrid);
            Controls.Add(itemListView);
            Controls.Add(infoLabel);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(490, 350);
            Name = "SettingDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Settings";
            Load += DialogLoad;
            Shown += DialogShown;
            FormClosing += DialogClosing;
            FormClosed += DialogClosed;
            ((ISupportInitialize)(infoPictureBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(PluginBase.MainForm.FindImage("341", false));
            imageList.Images.Add(PluginBase.MainForm.FindImage("342", false));
            imageList.Images.Add(PluginBase.MainForm.FindImage("50", false));
            imageList.Images.Add(PluginBase.MainForm.FindImage("153", false)); // clear
            itemListView.SmallImageList = imageList;
            clearFilterButton.ImageList = imageList;
            clearFilterButton.ImageIndex = 3;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            helpLabel.Text = TextHelper.GetString("Info.Help");
            Text = " " + TextHelper.GetString("Title.SettingDialog");
            disableCheckBox.Text = " " + TextHelper.GetString("Info.Disable");
            filterLabel.Text = TextHelper.GetString("Info.FilterSettings");
            nameLabel.Text = TextHelper.GetString("Info.NoItemSelected");
            closeButton.Text = TextHelper.GetString("Label.Close");
            nameLabel.Font = new Font(Font, FontStyle.Bold);
        }

        /// <summary>
        /// Initializes the setting object groups
        /// </summary>
        void InitializeItemGroups()
        {
            string mainHeader = TextHelper.GetString("Group.Main");
            string pluginsHeader = TextHelper.GetString("Group.Plugins");
            mainGroup = new ListViewGroup(mainHeader, HorizontalAlignment.Left);
            pluginsGroup = new ListViewGroup(pluginsHeader, HorizontalAlignment.Left);
            itemListView.Groups.Add(mainGroup);
            itemListView.Groups.Add(pluginsGroup);
            columnHeader.Width = ScaleHelper.Scale(columnHeader.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        void InitializeContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var collapseAll = new ToolStripMenuItem(TextHelper.GetString("Label.CollapseAll"));
            collapseAll.ShortcutKeys = PluginBase.MainForm.GetShortcutItemKeys("ViewMenu.CollapseAll");
            collapseAll.Click += (sender, args) => itemPropertyGrid.CollapseAllGridItems();
            contextMenu.Items.Add(collapseAll);
            var expandAll = new ToolStripMenuItem(TextHelper.GetString("Label.ExpandAll"));
            expandAll.ShortcutKeys = PluginBase.MainForm.GetShortcutItemKeys("ViewMenu.ExpandAll");
            expandAll.Click += (sender, args) => itemPropertyGrid.ExpandAllGridItems();
            contextMenu.Items.Add(expandAll);
            itemPropertyGrid.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Populates the plugin list
        /// </summary>
        void PopulatePluginList()
        {
            itemListView.Items.Clear();
            int count = PluginServices.AvailablePlugins.Count;
            ListViewItem main = new ListViewItem(DistroConfig.DISTRIBUTION_NAME, 2);
            itemListView.BeginUpdate();
            itemListView.Items.Add(main);
            mainGroup.Items.Add(main);
            for (int i = 0; i < count; i++)
            {
                var plugin = PluginServices.AvailablePlugins[i];
                var item = new ListViewItem(plugin.Instance.Name, 0) {Tag = plugin.Instance};
                if (PluginBase.Settings.DisabledPlugins.Contains(plugin.Instance.Guid))
                {
                    item.ImageIndex = 1;
                }
                if (!string.IsNullOrEmpty(itemFilter)) // Set default filter...
                {
                    filterText.TextChanged -= FilterTextTextChanged;
                    filterText.Text = itemFilter;
                    filterText.TextChanged += FilterTextTextChanged;
                }
                if (filterText.Text.Length > 0)
                {
                    if (CheckIfExist(plugin.Instance, filterText.Text))
                    {
                        itemListView.Items.Add(item);
                        pluginsGroup.Items.Add(item);
                    }
                }
                else
                {
                    itemListView.Items.Add(item);
                    pluginsGroup.Items.Add(item);
                }
            }
            itemListView.EndUpdate();
            SelectCorrectItem(itemName);
        }

        /// <summary>
        /// Selects the correct setting item
        /// </summary>
        void SelectCorrectItem(string itemName)
        {
            for (var i = 0; i < itemListView.Items.Count; i++)
            {
                var item = itemListView.Items[i];
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
        void ItemListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemListView.SelectedIndices.Count > 0)
            {
                var selectedIndex = itemListView.SelectedIndices[0];
                if (selectedIndex == 0)
                {
                    sdkContext = new InstalledSDKContext(null);
                    itemPropertyGrid.Enabled = true;
                    itemPropertyGrid.SelectedObject = Globals.Settings;
                    descLabel.Text = TextHelper.GetString("Info.AppDescription");
                    helpUrl = DistroConfig.DISTRIBUTION_HELP;
                    nameLabel.Text = DistroConfig.DISTRIBUTION_NAME;
                    nameLabel.Enabled = true;
                    ShowInfoControls(false);
                    FilterPropertySheet();
                }
                else
                {
                    var plugin = (IPlugin)itemListView.SelectedItems[0].Tag;
                    sdkContext = new InstalledSDKContext(plugin as InstalledSDKOwner);
                    disableCheckBox.Checked = PluginBase.Settings.DisabledPlugins.Contains(plugin.Guid);
                    itemPropertyGrid.SelectedObject = plugin.Settings;
                    itemPropertyGrid.Enabled = plugin.Settings != null;
                    descLabel.Text = plugin.Description;
                    nameLabel.Text = plugin.Name;
                    nameLabel.Enabled = true;
                    helpUrl = plugin.Help;
                    FilterPropertySheet();
                    ShowInfoControls(true);
                    MoveInfoControls();
                }
            }
            else
            {
                nameLabel.Enabled = false;
                descLabel.Text = string.Empty;
                nameLabel.Text = TextHelper.GetString("Info.NoItemSelected");
                itemPropertyGrid.SelectedObject = null;
                itemPropertyGrid.Enabled = false;
                ShowInfoControls(false);
            }
        }

        /// <summary>
        /// Filter the currently selected property sheet according to the text on the filter box
        /// </summary>
        void FilterPropertySheet()
        {
            if (PlatformHelper.IsRunningOnMono()) return;
            var settingsObj = itemPropertyGrid.SelectedObject;
            if (settingsObj is null) return;
            var text = filterText.Text;
            var i = 0;
            string[] browsables = { "" };
            foreach (var prop in settingsObj.GetType().GetProperties())
            {
                if (PropertyMatches(prop, text))
                {
                    Array.Resize(ref browsables, i + 1);
                    browsables[i++] = prop.Name;
                }
            }
            itemPropertyGrid.BrowsableProperties = browsables;
            itemPropertyGrid.SelectedObject = settingsObj;
            itemPropertyGrid.Refresh();
        }

        /// <summary>
        /// Checks if a property exist in a plugin
        /// </summary>
        static bool CheckIfExist(IPlugin plugin, string text)
        {
            if (plugin.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }
            return plugin.Settings is { } settingsObj
                   && settingsObj.GetType()
                       .GetProperties()
                       .Any(it => PropertyMatches(it, text));
        }

        /// <summary>
        /// Checks if the property matches in any property infos
        /// </summary>
        static bool PropertyMatches(PropertyInfo property, string text)
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
        public bool PartialName(MemberInfo candidate, object part) => candidate.Name.Contains(part.ToString());

        /// <summary>
        /// When setting value has changed, dispatch event
        /// </summary>
        void PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (itemListView.SelectedIndices.Count == 0) return;
            var changedItem = e.ChangedItem;
            var settingId = nameLabel.Text + "." + changedItem.Label.Replace(" ", "");
            var te = new TextEvent(EventType.SettingChanged, settingId);
            EventManager.DispatchEvent(PluginBase.MainForm, te);
            if (changedItem.PropertyDescriptor.Attributes.Matches(new RequiresRestartAttribute()))
            {
                UpdateRestartRequired(settingId, e.OldValue, changedItem.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateRestartRequired(string key, object oldValue, object newValue)
        {
            var previous = requireRestart.Count > 0;
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
        void UpdateInfo()
        {
            if (requireRestart.Count > 0)
            {
                infoLabel.Text = TextHelper.GetString("Info.RequiresRestart");
                infoPictureBox.Image = PluginBase.MainForm.FindImage("196", false);
            }
            else
            {
                infoLabel.Text = TextHelper.GetString("Info.SettingsTakeEffect");
                infoPictureBox.Image = PluginBase.MainForm.FindImage("229", false);
            }
        }

        /// <summary>
        /// Moves the info controls to correct position
        /// </summary>
        void MoveInfoControls()
        {
            int dcbY = disableCheckBox.Location.Y;
            int dcbX = nameLabel.Location.X + nameLabel.Width + 13;
            int hlX = dcbX + disableCheckBox.Width;
            int hlY = helpLabel.Location.Y;
            disableCheckBox.Location = new Point(dcbX, dcbY);
            helpLabel.Location = new Point(hlX, hlY);
        }

        /// <summary>
        /// Shows or hides the info controls
        /// </summary>
        void ShowInfoControls(bool value)
        {
            disableCheckBox.Visible = value;
            helpLabel.Visible = value;
        }

        /// <summary>
        /// Toggles the enabled state and saves it to settings
        /// </summary>
        void DisableCheckBoxCheck(object sender, EventArgs e)
        {
            var selectedIndex = itemListView.SelectedIndices[0];
            if (selectedIndex == 0) return;
            var plugin = PluginServices.AvailablePlugins[selectedIndex - 1].Instance;
            var disabled = disableCheckBox.Checked;
            if (disabled)
            {
                itemListView.Items[selectedIndex].ImageIndex = 1;
                PluginBase.Settings.DisabledPlugins.Add(plugin.Guid);
            }
            else
            {
                itemListView.Items[selectedIndex].ImageIndex = 0;
                PluginBase.Settings.DisabledPlugins.Remove(plugin.Guid);
            }
            UpdateRestartRequired(nameLabel.Text, !disabled, disabled);
        }

        /// <summary>
        /// Populate plugin list before dialog is shown
        /// </summary>
        void DialogLoad(object sender, EventArgs e) => PopulatePluginList();

        /// <summary>
        /// Restore the selected index - only if an item id hasn't been provided
        /// </summary>
        void DialogShown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(itemName) || itemName == DistroConfig.DISTRIBUTION_NAME)
            {
                itemListView.SelectedIndices.Add(lastItemIndex);
                itemListView.EnsureVisible(lastItemIndex);
            }
            filterText.Focus();
        }

        /// <summary>
        /// Save the last selected index to a static var
        /// </summary>
        void DialogClosing(object sender, FormClosingEventArgs e)
            => lastItemIndex = itemListView.SelectedIndices.Count > 0
                ? itemListView.SelectedIndices[0]
                : 0;

        /// <summary>
        /// When the form is closed applies settings
        /// </summary>
        void DialogClosed(object sender, FormClosedEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;  // Enough for our sync code
            sdkContext?.Dispose();
            Globals.MainForm.ApplyAllSettings();
            Globals.MainForm.SaveSettings();
            if (requireRestart.Count > 0 && !PluginBase.MainForm.RequiresRestart) Globals.MainForm.RestartRequired();
        }

        /// <summary>
        /// Event for changing the filter text
        /// </summary>
        void FilterTextTextChanged(object sender, EventArgs e)
        {
            PopulatePluginList();
            FilterPropertySheet();
        }

        /// <summary>
        /// Event for pressing the filter clear button
        /// </summary>
        void ClearFilterButtonClick(object sender, EventArgs e) => filterText.Text = "";

        /// <summary>
        /// Browses to the specified help url
        /// </summary>
        void HelpLabelClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (helpUrl != null) PluginBase.MainForm.CallCommand("Browse", helpUrl);
        }

        /// <summary>
        /// Closes the setting dialog
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Shows the settings dialog
        /// </summary>
        public static void Show(string itemName, string filter)
        {
            using var dialog = new SettingDialog(itemName, filter);
            dialog.closeButton.Select();
            dialog.ShowDialog();
        }

        #endregion
    }
}