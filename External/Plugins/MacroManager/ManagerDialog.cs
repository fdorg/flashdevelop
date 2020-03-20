using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;

namespace MacroManager
{
    public class ManagerDialog : SmartForm
    {
        Label infoLabel;
        ListView listView;
        PictureBox pictureBox;
        ToolStripItem exportItem;
        PropertyGrid propertyGrid;
        ColumnHeader columnHeader;
        ListViewGroup macroGroup;
        readonly PluginMain pluginMain;
        Button deleteButton;
        Button closeButton;
        Button addButton;
    
        public ManagerDialog(PluginMain pluginMain)
        {
            this.pluginMain = pluginMain;
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "2c8745c8-8f17-4b79-b663-afacc76abfe5";
            this.InitializeComponent();
            this.InitializeItemGroups();
            this.InitializeContextMenu();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.listView = new System.Windows.Forms.ListViewEx();
            this.addButton = new System.Windows.Forms.ButtonEx();
            this.deleteButton = new System.Windows.Forms.ButtonEx();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.infoLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            this.propertyGrid.HelpVisible = true;
            this.propertyGrid.Location = new System.Drawing.Point(182, 12);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(355, 299);
            this.propertyGrid.TabIndex = 2;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += this.PropertyValueChanged;
            // 
            // listView
            // 
            this.listView.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left;
            this.listView.Location = new System.Drawing.Point(12, 12);
            this.listView.Name = "listView";
            this.listView.MultiSelect = true;
            this.listView.HideSelection = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.Size = new System.Drawing.Size(160, 270);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.SelectedIndexChanged += this.ListViewIndexChanged;
            this.listView.Columns.Add(this.columnHeader);
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.addButton.Location = new System.Drawing.Point(11, 289);
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "&Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += this.AddButtonClick;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteButton.Location = new System.Drawing.Point(97, 289);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += this.DeleteButtonClick;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(438, 319);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += this.CloseButtonClick;
            // 
            // pictureBox
            //
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.pictureBox.Location = new System.Drawing.Point(13, 324);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(16, 16);
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(34, 325);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(358, 20);
            this.infoLabel.TabIndex = 6;
            this.infoLabel.Text = "Macros will take effect soon as you edit them successfully.";
            // 
            // ManagerDialog
            //
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = this.closeButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 353);
            this.MinimumSize = new System.Drawing.Size(449, 253);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.propertyGrid);
            this.Name = "ManagerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += this.DialogLoad;
            this.FormClosed += this.DialogClosed;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Text = "Macros";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the graphics used in the control
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("338|13|0|0", false));
            this.pictureBox.Image = PluginBase.MainForm.FindImage("229", false);
            this.listView.SmallImageList = imageList;
            this.listView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.columnHeader.Width = -2;
        }

        /// <summary>
        /// Initializes the list view item groups
        /// </summary>
        void InitializeItemGroups()
        {
            string macroGroup = TextHelper.GetString("Group.Macros");
            this.macroGroup = new ListViewGroup(macroGroup, HorizontalAlignment.Left);
            this.listView.Groups.Add(this.macroGroup);
        }

        /// <summary>
        /// Initializes the import/export context menu
        /// </summary>
        void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Font = PluginBase.Settings.DefaultFont;
            contextMenu.Renderer = new DockPanelStripRenderer(false, false);
            contextMenu.Opening += this.ContextMenuOpening;
            contextMenu.Items.Add(TextHelper.GetString("Label.ImportMacros"), null, this.ImportMacros);
            this.exportItem = new ToolStripMenuItem(TextHelper.GetString("Label.ExportMacros"), null, this.ExportMacros);
            contextMenu.Items.Add(this.exportItem); // Add export item
            this.listView.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Hides the export item if there are no items selected
        /// </summary>
        void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            exportItem.Visible = listView.SelectedItems.Count != 0;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            this.addButton.Text = TextHelper.GetString("Label.Add");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.deleteButton.Text = TextHelper.GetString("Label.Delete");
            this.infoLabel.Text = TextHelper.GetString("Info.MacrosTakeEffect");
            this.Text = " " + TextHelper.GetString("Title.MacroDialog");
        }

        /// <summary>
        /// Loads the macro list from settings
        /// </summary>
        void LoadUserMacros()
        {
            List<Macro> macros = this.pluginMain.AppSettings.UserMacros;
            this.PopulateMacroList(macros);
        }

        /// <summary>
        /// Saves the macro list to settings
        /// </summary>
        void SaveUserMacros()
        {
            List<Macro> macros = new List<Macro>();
            foreach (ListViewItem item in this.listView.Items)
            {
                Macro macro = item.Tag as Macro;
                macros.Add(macro);
            }
            this.pluginMain.AppSettings.UserMacros = macros;
        }

        /// <summary>
        /// Populates the macro list
        /// </summary>
        void PopulateMacroList(List<Macro> macros)
        {
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            foreach (Macro macro in macros)
            {
                ListViewItem item = new ListViewItem(macro.Label, 0);
                item.Tag = macro;
                this.macroGroup.Items.Add(item);
                this.listView.Items.Add(item);
            }
            this.listView.EndUpdate();
            if (this.listView.Items.Count > 0)
            {
                ListViewItem item = this.listView.Items[0];
                item.Selected = true;
            }
        }

        /// <summary>
        /// Adds a new empty macro to the list
        /// </summary>
        void AddButtonClick(object sender, EventArgs e)
        {
            string untitled = TextHelper.GetString("Info.Untitled");
            ListViewItem item = new ListViewItem(untitled, 0);
            item.Tag = new Macro(untitled, Array.Empty<string>(), string.Empty, Keys.None);
            this.macroGroup.Items.Add(item);
            this.listView.Items.Add(item);
        }

        /// <summary>
        /// Deletes the delected macro[s] from the list
        /// </summary>
        void DeleteButtonClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView.SelectedItems)
            {
                this.listView.Items.Remove(item);
            }
            if (this.listView.Items.Count > 0)
            {
                ListViewItem item = this.listView.Items[0];
                item.Selected = true;
            }
        }

        /// <summary>
        /// Activates correct macros and controls
        /// </summary>
        void ListViewIndexChanged(object sender, EventArgs e)
        {
            if (this.listView.SelectedIndices.Count == 1)
            {
                ListViewItem item = this.listView.SelectedItems[0];
                this.propertyGrid.SelectedObject = item.Tag;
            }
            else this.propertyGrid.SelectedObject = null;
            deleteButton.Enabled = listView.SelectedItems.Count != 0;
        }

        /// <summary>
        /// Updates the label of the selected macro
        /// </summary>
        void PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (this.listView.SelectedIndices.Count == 1)
            {
                ListViewItem item = this.listView.SelectedItems[0];
                item.Text = ((Macro)item.Tag).Label;
            }
        }

        /// <summary>
        /// Closes the macro editoe dialog
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Exports the current macro list into a file
        /// </summary>
        void ExportMacros(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.Filter = TextHelper.GetString("Info.MacroFilter") + "|*.fdm";
            dialog.InitialDirectory = PluginBase.MainForm.WorkingDirectory;
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var macros = new List<Macro>();
            foreach (ListViewItem item in this.listView.SelectedItems)
            {
                macros.Add((Macro)item.Tag);
            }
            ObjectSerializer.Serialize(dialog.FileName, macros);
        }

        /// <summary>
        /// Imports an macro list from a file
        /// </summary>
        void ImportMacros(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Filter = TextHelper.GetString("Info.MacroFilter") + "|*.fdm";
            dialog.InitialDirectory = PluginBase.MainForm.WorkingDirectory;
            if (dialog.ShowDialog() != DialogResult.OK) return;
            this.SaveUserMacros();
            var macros = new List<Macro>();
            macros = (List<Macro>)ObjectSerializer.Deserialize(dialog.FileName, macros, false);
            this.pluginMain.AppSettings.UserMacros.AddRange(macros);
            this.PopulateMacroList(this.pluginMain.AppSettings.UserMacros);
        }

        /// <summary>
        /// Loads the macros from the settings
        /// </summary>
        void DialogLoad(object sender, EventArgs e) => LoadUserMacros();

        /// <summary>
        /// Saves the macros when the dialog is closed
        /// </summary>
        void DialogClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveUserMacros();
            this.pluginMain.RefreshMacroToolBarItems();
            this.pluginMain.RefreshMacroMenuItems();
        }

        /// <summary>
        /// Shows the macro editing dialog
        /// </summary>
        public static void Show(PluginMain pluginMain)
        {
            using var dialog = new ManagerDialog(pluginMain);
            dialog.ShowDialog();
        }

        #endregion
    }
}