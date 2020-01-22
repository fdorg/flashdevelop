// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using FlashDevelop.Helpers;
using PluginCore.Utilities;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class ArgumentDialog : SmartForm
    {
        Label keyLabel;
        Label infoLabel;
        Label valueLabel;
        TextBox keyTextBox;
        TextBox valueTextBox;
        ListView argsListView;
        ToolStripItem exportItem;
        ColumnHeader columnHeader;
        ListViewGroup argumentGroup;
        PictureBox infoPictureBox;
        GroupBox detailsGroupBox;
        Button deleteButton;
        Button closeButton;
        Button addButton;

        static ArgumentDialog()
        {
            CustomArguments = new List<Argument>();
        }

        public ArgumentDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "ea726ad2-ef09-4e4c-bfc6-41cc980be521";
            InitializeComponent();
            InitializeItemGroups();
            InitializeContextMenu();
            InitializeGraphics();
            ApplyLocalizedTexts();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            keyTextBox = new TextBoxEx();
            argsListView = new ListViewEx();
            valueLabel = new Label();
            detailsGroupBox = new GroupBoxEx();
            keyLabel = new Label();
            valueTextBox = new TextBoxEx();
            infoLabel = new Label();
            closeButton = new ButtonEx();
            columnHeader = new ColumnHeader();
            infoPictureBox = new PictureBox();
            addButton = new ButtonEx();
            deleteButton = new ButtonEx();
            detailsGroupBox.SuspendLayout();
            ((ISupportInitialize)(infoPictureBox)).BeginInit();
            SuspendLayout();
            //
            // columnHeader
            //
            columnHeader.Width = 182;
            // 
            // keyTextBox
            // 
            keyTextBox.Location = new Point(14, 35);
            keyTextBox.Name = "keyTextBox";
            keyTextBox.Size = new Size(308, 21);
            keyTextBox.TabIndex = 2;
            keyTextBox.TextChanged += TextBoxTextChange;
            // 
            // argsListView
            // 
            argsListView.HeaderStyle = ColumnHeaderStyle.None;
            argsListView.HideSelection = false;
            argsListView.Location = new Point(12, 12);
            argsListView.MultiSelect = true;
            argsListView.Name = "argsListView";
            argsListView.Size = new Size(182, 277);
            argsListView.TabIndex = 1;
            argsListView.UseCompatibleStateImageBehavior = false;
            argsListView.View = View.Details;
            argsListView.Alignment = ListViewAlignment.Left;
            argsListView.Columns.Add(columnHeader);
            argsListView.SelectedIndexChanged += ArgsListViewSelectedIndexChanged;
            // 
            // valueLabel
            // 
            valueLabel.AutoSize = true;
            valueLabel.FlatStyle = FlatStyle.System;
            valueLabel.Location = new Point(14, 60);
            valueLabel.Name = "valueLabel";
            valueLabel.Size = new Size(37, 13);
            valueLabel.TabIndex = 3;
            valueLabel.Text = "Value:";
            // 
            // detailsGroupBox
            //
            detailsGroupBox.Controls.Add(keyLabel);
            detailsGroupBox.Controls.Add(valueTextBox);
            detailsGroupBox.Controls.Add(keyTextBox);
            detailsGroupBox.Controls.Add(valueLabel);
            detailsGroupBox.Location = new Point(207, 6);
            detailsGroupBox.FlatStyle = FlatStyle.System;
            detailsGroupBox.Name = "detailsGroupBox";
            detailsGroupBox.Size = new Size(335, 313);
            detailsGroupBox.TabIndex = 4;
            detailsGroupBox.TabStop = false;
            detailsGroupBox.Text = " Details";
            // 
            // keyLabel
            // 
            keyLabel.AutoSize = true;
            keyLabel.FlatStyle = FlatStyle.System;
            keyLabel.Location = new Point(14, 17);
            keyLabel.Name = "keyLabel";
            keyLabel.Size = new Size(29, 13);
            keyLabel.TabIndex = 1;
            keyLabel.Text = "Key:";
            // 
            // valueTextBox
            //
            valueTextBox.AcceptsTab = true;
            valueTextBox.AcceptsReturn = true;
            valueTextBox.Font = PluginBase.Settings.ConsoleFont;
            valueTextBox.ScrollBars = ScrollBars.Vertical;
            valueTextBox.Location = new Point(14, 77);
            valueTextBox.Multiline = true;
            valueTextBox.Name = "valueTextBox";
            valueTextBox.Size = new Size(308, 223);
            valueTextBox.TabIndex = 4;
            valueTextBox.TextChanged += TextBoxTextChange;
            // 
            // infoLabel
            //
            infoLabel.AutoSize = true;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.Location = new Point(34, 331);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(357, 13);
            infoLabel.TabIndex = 5;
            infoLabel.Text = "Custom arguments will take effect as soon as you edit them successfully.";
            // 
            // closeButton
            //
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(443, 326);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(100, 23);
            closeButton.TabIndex = 7;
            closeButton.Text = "&Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButtonClick;
            // 
            // infoPictureBox
            //
            infoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            infoPictureBox.Location = new Point(13, 330);
            infoPictureBox.Name = "infoPictureBox";
            infoPictureBox.Size = new Size(16, 16);
            infoPictureBox.TabIndex = 7;
            infoPictureBox.TabStop = false;
            // 
            // addButton
            //
            addButton.FlatStyle = FlatStyle.System;
            addButton.Location = new Point(11, 296);
            addButton.Name = "addButton";
            addButton.Size = new Size(87, 23);
            addButton.TabIndex = 2;
            addButton.Text = "&Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButtonClick;
            // 
            // deleteButton
            //
            deleteButton.FlatStyle = FlatStyle.System;
            deleteButton.Location = new Point(108, 296);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(87, 23);
            deleteButton.TabIndex = 3;
            deleteButton.Text = "&Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += DeleteButtonClick;
            // 
            // ArgumentDialog
            //
            CancelButton = closeButton;
            AcceptButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 360);
            Controls.Add(deleteButton);
            Controls.Add(addButton);
            Controls.Add(infoLabel);
            Controls.Add(infoPictureBox);
            Controls.Add(detailsGroupBox);
            Controls.Add(closeButton);
            Controls.Add(argsListView);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ArgumentDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Custom Arguments";
            Load += DialogLoad;
            detailsGroupBox.ResumeLayout(false);
            detailsGroupBox.PerformLayout();
            ((ISupportInitialize)(infoPictureBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// List of all custom arguments
        /// </summary>
        public static List<Argument> CustomArguments { get; private set; }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("242", false));
            infoPictureBox.Image = PluginBase.MainForm.FindImage("229", false);
            argsListView.SmallImageList = imageList;
            argsListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            columnHeader.Width = ScaleHelper.Scale(columnHeader.Width);
        }

        /// <summary>
        /// Initializes the import/export context menu
        /// </summary>
        void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Font = PluginBase.Settings.DefaultFont;
            contextMenu.Renderer = new DockPanelStripRenderer(false, false);
            contextMenu.Opening += ContextMenuOpening;
            contextMenu.Items.Add(TextHelper.GetString("Label.ImportArguments"), null, ImportArguments);
            exportItem = new ToolStripMenuItem(TextHelper.GetString("Label.ExportArguments"), null, ExportArguments);
            contextMenu.Items.Add(exportItem); // Add export item
            argsListView.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Hides the export item if there are no items selected
        /// </summary>
        void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            if (argsListView.SelectedItems.Count == 0) exportItem.Visible = false;
            else exportItem.Visible = true;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            Text = " " + TextHelper.GetString("Title.ArgumentDialog");
            infoLabel.Text = TextHelper.GetString("Info.ArgumentsTakeEffect");
            detailsGroupBox.Text = TextHelper.GetString("Info.Details");
            closeButton.Text = TextHelper.GetString("Label.Close");
            deleteButton.Text = TextHelper.GetString("Label.Delete");
            addButton.Text = TextHelper.GetString("Label.Add");
            valueLabel.Text = TextHelper.GetString("Info.Value");
            keyLabel.Text = TextHelper.GetString("Info.Key");
        }

        /// <summary>
        /// Initializes the list view item groups
        /// </summary>
        void InitializeItemGroups()
        {
            string argumentHeader = TextHelper.GetString("Group.Arguments");
            argumentGroup = new ListViewGroup(argumentHeader, HorizontalAlignment.Left);
            argsListView.Groups.Add(argumentGroup);
        }

        /// <summary>
        /// Populates the argument list
        /// </summary>
        void PopulateArgumentList(List<Argument> arguments)
        {
            argsListView.BeginUpdate();
            argsListView.Items.Clear();
            string message = TextHelper.GetString("Info.Argument");
            foreach (Argument argument in arguments)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = 0; item.Tag = argument;
                item.Text = message + " $(" + argument.Key + ")";
                argsListView.Items.Add(item);
                argumentGroup.Items.Add(item);
            }
            argsListView.EndUpdate();
            if (argsListView.Items.Count > 0)
            {
                ListViewItem item = argsListView.Items[0];
                item.Selected = true;
            }
        }

        /// <summary>
        /// Adds a new empty argument
        /// </summary>
        void AddButtonClick(object sender, EventArgs e)
        {
            Argument argument = new Argument();
            ListViewItem item = new ListViewItem();
            string message = TextHelper.GetString("Info.Argument");
            string undefined = TextHelper.GetString("Info.Undefined");
            item.ImageIndex = 0; argument.Key = undefined;
            item.Text = message + " $(" + undefined + ")";
            argsListView.Items.Add(item);
            argumentGroup.Items.Add(item);
            foreach (ListViewItem other in argsListView.Items)
            {
                other.Selected = false;
            }
            item.Tag = argument; 
            item.Selected = true;
            CustomArguments.Add(argument);
        }

        /// <summary>
        /// Removes the selected arguments
        /// </summary>
        void DeleteButtonClick(object sender, EventArgs e)
        {
            int selectedIndex = 0;
            if (argsListView.SelectedIndices.Count > 0)
            {
                selectedIndex = argsListView.SelectedIndices[0];
            }
            argsListView.BeginUpdate();
            foreach (ListViewItem item in argsListView.SelectedItems)
            {
                argsListView.Items.Remove(item);
                Argument argument = item.Tag as Argument;
                CustomArguments.Remove(argument);
            }
            argsListView.EndUpdate();
            if (argsListView.Items.Count > 0)
            {
                try { argsListView.Items[selectedIndex].Selected = true; }
                catch
                {
                    int last = argsListView.Items.Count - 1;
                    argsListView.Items[last].Selected = true;
                }
            }
            else argsListView.Select();
        }

        /// <summary>
        /// Closes the dialog saving the arguments
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Selected item has changed, updates the state
        /// </summary>
        void ArgsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (argsListView.SelectedItems.Count == 1)
            {
                keyTextBox.Enabled = true;
                valueTextBox.Enabled = true;
                ListViewItem item = argsListView.SelectedItems[0];
                Argument argument = item.Tag as Argument;
                valueTextBox.Text = argument.Value;
                keyTextBox.Text = argument.Key;
                if (argument.Key == "DefaultUser") keyTextBox.ReadOnly = true;
                else keyTextBox.ReadOnly = false;
            }
            else
            {
                keyTextBox.Text = "";
                valueTextBox.Text = "";
                valueTextBox.Enabled = false;
                keyTextBox.Enabled = false;
            }
            deleteButton.Enabled = argsListView.SelectedItems.Count != 0;
        }

        /// <summary>
        /// Updates the argument when text changes
        /// </summary>
        void TextBoxTextChange(object sender, EventArgs e)
        {
            if (keyTextBox.Text.Length == 0) return;
            string message = TextHelper.GetString("Info.Argument");
            if (argsListView.SelectedItems.Count == 1)
            {
                ListViewItem item = argsListView.SelectedItems[0];
                Argument argument = item.Tag as Argument;
                argument.Value = valueTextBox.Text;
                argument.Key = keyTextBox.Text;
                item.Text = message + " $(" + argument.Key + ")";
            }
        }

        /// <summary>
        /// Exports the current argument list into a file
        /// </summary>
        void ExportArguments(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda";
            dialog.InitialDirectory = PluginBase.MainForm.WorkingDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<Argument> args = new List<Argument>();
                foreach (ListViewItem item in argsListView.SelectedItems)
                {
                    args.Add((Argument)item.Tag);
                }
                ObjectSerializer.Serialize(dialog.FileName, args);
            }
        }

        /// <summary>
        /// Imports an argument list from a file
        /// </summary>
        void ImportArguments(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda";
            dialog.InitialDirectory = PluginBase.MainForm.WorkingDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<Argument> args = new List<Argument>();
                args = (List<Argument>)ObjectSerializer.Deserialize(dialog.FileName, args, false);
                CustomArguments.AddRange(args); // Append imported
                PopulateArgumentList(CustomArguments);
            }
        }

        /// <summary>
        /// Loads the arguments from the settings
        /// </summary>
        void DialogLoad(object sender, EventArgs e) => PopulateArgumentList(CustomArguments);

        /// <summary>
        /// Shows the argument dialog
        /// </summary>
        public new static void Show()
        {
            using var dialog = new ArgumentDialog();
            dialog.ShowDialog();
        }

        #endregion

        #region User Argument Management

        /// <summary>
        /// Loads the argument list from file
        /// </summary>
        public static void LoadCustomArguments()
        {
            string file = FileNameHelper.UserArgData;
            if (File.Exists(file))
            {
                object data = ObjectSerializer.Deserialize(file, CustomArguments, false);
                CustomArguments = (List<Argument>)data;
            }
            if (!File.Exists(file) || CustomArguments.Count == 0)
            {
                CustomArguments.Add(new Argument("DefaultUser", "..."));
            }
        }

        /// <summary>
        /// Saves the argument list to file
        /// </summary>
        public static void SaveCustomArguments()
        {
            string file = FileNameHelper.UserArgData;
            ObjectSerializer.Serialize(file, CustomArguments);
        }

        #endregion

    }
}