using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using FlashDevelop.Settings;
using FlashDevelop.Helpers;
using PluginCore.Utilities;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class ArgumentDialog : Form
    {
        private static List<Argument> arguments;
        private System.Windows.Forms.Label keyLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.TextBox keyTextBox;
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.ListView argsListView;
        private System.Windows.Forms.ToolStripItem exportItem;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ListViewGroup argumentGroup;
        private System.Windows.Forms.PictureBox infoPictureBox;
        private System.Windows.Forms.GroupBox detailsGroupBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button addButton;

        static ArgumentDialog()
        {
            arguments = new List<Argument>();
        }

        public ArgumentDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.InitializeItemGroups();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.ApplyLocalizedTexts();
            ScaleHelper.AdjustForHighDPI(this);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.keyTextBox = new System.Windows.Forms.TextBox();
            this.argsListView = new System.Windows.Forms.ListView();
            this.valueLabel = new System.Windows.Forms.Label();
            this.detailsGroupBox = new System.Windows.Forms.GroupBox();
            this.keyLabel = new System.Windows.Forms.Label();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.infoPictureBox = new System.Windows.Forms.PictureBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.detailsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).BeginInit();
            this.SuspendLayout();
            //
            // columnHeader
            //
            this.columnHeader.Width = 182;
            // 
            // keyTextBox
            // 
            this.keyTextBox.Location = new System.Drawing.Point(14, 35);
            this.keyTextBox.Name = "keyTextBox";
            this.keyTextBox.Size = new System.Drawing.Size(308, 21);
            this.keyTextBox.TabIndex = 2;
            this.keyTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChange);
            // 
            // argsListView
            // 
            this.argsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.argsListView.HideSelection = false;
            this.argsListView.Location = new System.Drawing.Point(12, 12);
            this.argsListView.MultiSelect = true;
            this.argsListView.Name = "argsListView";
            this.argsListView.Size = new System.Drawing.Size(182, 277);
            this.argsListView.TabIndex = 1;
            this.argsListView.UseCompatibleStateImageBehavior = false;
            this.argsListView.View = System.Windows.Forms.View.Details;
            this.argsListView.Alignment = ListViewAlignment.Left;
            this.argsListView.Columns.Add(this.columnHeader);
            this.argsListView.SelectedIndexChanged += new System.EventHandler(this.ArgsListViewSelectedIndexChanged);
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.valueLabel.Location = new System.Drawing.Point(14, 60);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(37, 13);
            this.valueLabel.TabIndex = 3;
            this.valueLabel.Text = "Value:";
            // 
            // detailsGroupBox
            //
            this.detailsGroupBox.Controls.Add(this.keyLabel);
            this.detailsGroupBox.Controls.Add(this.valueTextBox);
            this.detailsGroupBox.Controls.Add(this.keyTextBox);
            this.detailsGroupBox.Controls.Add(this.valueLabel);
            this.detailsGroupBox.Location = new System.Drawing.Point(207, 6);
            this.detailsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.detailsGroupBox.Name = "detailsGroupBox";
            this.detailsGroupBox.Size = new System.Drawing.Size(335, 313);
            this.detailsGroupBox.TabIndex = 4;
            this.detailsGroupBox.TabStop = false;
            this.detailsGroupBox.Text = " Details";
            // 
            // keyLabel
            // 
            this.keyLabel.AutoSize = true;
            this.keyLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.keyLabel.Location = new System.Drawing.Point(14, 17);
            this.keyLabel.Name = "keyLabel";
            this.keyLabel.Size = new System.Drawing.Size(29, 13);
            this.keyLabel.TabIndex = 1;
            this.keyLabel.Text = "Key:";
            // 
            // valueTextBox 
            // Font needs to be set here so that controls resize correctly in high-dpi
            //
            this.valueTextBox.AcceptsTab = true;
            this.valueTextBox.AcceptsReturn = true;
            this.valueTextBox.Font = Globals.Settings.ConsoleFont;
            this.valueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.valueTextBox.Location = new System.Drawing.Point(14, 77);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(308, 223);
            this.valueTextBox.TabIndex = 4;
            this.valueTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChange);
            // 
            // infoLabel
            //
            this.infoLabel.AutoSize = true;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.Location = new System.Drawing.Point(34, 331);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(357, 13);
            this.infoLabel.TabIndex = 5;
            this.infoLabel.Text = "Custom arguments will take effect as soon as you edit them successfully.";
            // 
            // closeButton
            //
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(443, 326);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // infoPictureBox
            //
            this.infoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.infoPictureBox.Location = new System.Drawing.Point(13, 330);
            this.infoPictureBox.Name = "infoPictureBox";
            this.infoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.infoPictureBox.TabIndex = 7;
            this.infoPictureBox.TabStop = false;
            // 
            // addButton
            //
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addButton.Location = new System.Drawing.Point(11, 296);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(87, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "&Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // deleteButton
            //
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteButton.Location = new System.Drawing.Point(108, 296);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(87, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // ArgumentDialog
            //
            this.CancelButton = this.closeButton;
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 360);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.infoPictureBox);
            this.Controls.Add(this.detailsGroupBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.argsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArgumentDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Custom Arguments";
            this.Load += new System.EventHandler(this.DialogLoad);
            this.detailsGroupBox.ResumeLayout(false);
            this.detailsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// List of all custom arguments
        /// </summary>
        public static List<Argument> CustomArguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(Globals.MainForm.FindImage("242", false));
            this.infoPictureBox.Image = Globals.MainForm.FindImage("229", false);
            this.argsListView.SmallImageList = imageList;
            this.argsListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.columnHeader.Width = ScaleHelper.Scale(this.columnHeader.Width);
        }

        /// <summary>
        /// Initializes the import/export context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Font = PluginBase.Settings.DefaultFont;
            contextMenu.Renderer = new DockPanelStripRenderer(false, false);
            contextMenu.Opening += new CancelEventHandler(this.ContextMenuOpening);
            contextMenu.Items.Add(TextHelper.GetString("Label.ImportArguments"), null, this.ImportArguments);
            this.exportItem = new ToolStripMenuItem(TextHelper.GetString("Label.ExportArguments"), null, this.ExportArguments);
            contextMenu.Items.Add(this.exportItem); // Add export item
            this.argsListView.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Hides the export item if there are no items selected
        /// </summary>
        private void ContextMenuOpening(Object sender, CancelEventArgs e)
        {
            if (this.argsListView.SelectedItems.Count == 0) this.exportItem.Visible = false;
            else this.exportItem.Visible = true;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.Text = " " + TextHelper.GetString("Title.ArgumentDialog");
            this.infoLabel.Text = TextHelper.GetString("Info.ArgumentsTakeEffect");
            this.detailsGroupBox.Text = TextHelper.GetString("Info.Details");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.deleteButton.Text = TextHelper.GetString("Label.Delete");
            this.addButton.Text = TextHelper.GetString("Label.Add");
            this.valueLabel.Text = TextHelper.GetString("Info.Value");
            this.keyLabel.Text = TextHelper.GetString("Info.Key");
        }

        /// <summary>
        /// Initializes the list view item groups
        /// </summary>
        private void InitializeItemGroups()
        {
            String argumentHeader = TextHelper.GetString("Group.Arguments");
            this.argumentGroup = new ListViewGroup(argumentHeader, HorizontalAlignment.Left);
            this.argsListView.Groups.Add(this.argumentGroup);
        }

        /// <summary>
        /// Populates the argument list
        /// </summary>
        private void PopulateArgumentList(List<Argument> arguments)
        {
            this.argsListView.BeginUpdate();
            this.argsListView.Items.Clear();
            String message = TextHelper.GetString("Info.Argument");
            foreach (Argument argument in arguments)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = 0; item.Tag = argument;
                item.Text = message + " $(" + argument.Key + ")";
                this.argsListView.Items.Add(item);
                this.argumentGroup.Items.Add(item);
            }
            this.argsListView.EndUpdate();
            if (this.argsListView.Items.Count > 0)
            {
                ListViewItem item = this.argsListView.Items[0];
                item.Selected = true;
            }
        }

        /// <summary>
        /// Adds a new empty argument
        /// </summary>
        private void AddButtonClick(Object sender, EventArgs e)
        {
            Argument argument = new Argument();
            ListViewItem item = new ListViewItem();
            String message = TextHelper.GetString("Info.Argument");
            String undefined = TextHelper.GetString("Info.Undefined");
            item.ImageIndex = 0; argument.Key = undefined;
            item.Text = message + " $(" + undefined + ")";
            this.argsListView.Items.Add(item);
            this.argumentGroup.Items.Add(item);
            foreach (ListViewItem other in this.argsListView.Items)
            {
                other.Selected = false;
            }
            item.Tag = argument; 
            item.Selected = true;
            arguments.Add(argument);
        }

        /// <summary>
        /// Removes the selected arguments
        /// </summary>
        private void DeleteButtonClick(Object sender, EventArgs e)
        {
            Int32 selectedIndex = 0;
            if (this.argsListView.SelectedIndices.Count > 0)
            {
                selectedIndex = this.argsListView.SelectedIndices[0];
            }
            this.argsListView.BeginUpdate();
            foreach (ListViewItem item in this.argsListView.SelectedItems)
            {
                this.argsListView.Items.Remove(item);
                Argument argument = item.Tag as Argument;
                arguments.Remove(argument);
            }
            this.argsListView.EndUpdate();
            if (this.argsListView.Items.Count > 0)
            {
                try { this.argsListView.Items[selectedIndex].Selected = true; }
                catch
                {
                    Int32 last = this.argsListView.Items.Count - 1;
                    this.argsListView.Items[last].Selected = true;
                }
            }
            else this.argsListView.Select();
        }

        /// <summary>
        /// Closes the dialog saving the arguments
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Selected item has changed, updates the state
        /// </summary>
        private void ArgsListViewSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.argsListView.SelectedItems.Count == 1)
            {
                this.keyTextBox.Enabled = true;
                this.valueTextBox.Enabled = true;
                ListViewItem item = this.argsListView.SelectedItems[0];
                Argument argument = item.Tag as Argument;
                this.valueTextBox.Text = argument.Value;
                this.keyTextBox.Text = argument.Key;
                if (argument.Key == "DefaultUser") this.keyTextBox.ReadOnly = true;
                else this.keyTextBox.ReadOnly = false;
            }
            else
            {
                this.keyTextBox.Text = "";
                this.valueTextBox.Text = "";
                this.valueTextBox.Enabled = false;
                this.keyTextBox.Enabled = false;
            }
            if (this.argsListView.SelectedItems.Count == 0) this.deleteButton.Enabled = false;
            else this.deleteButton.Enabled = true;
        }

        /// <summary>
        /// Updates the argument when text changes
        /// </summary>
        private void TextBoxTextChange(Object sender, EventArgs e)
        {
            if (this.keyTextBox.Text == "") return;
            String message = TextHelper.GetString("Info.Argument");
            if (this.argsListView.SelectedItems.Count == 1)
            {
                ListViewItem item = this.argsListView.SelectedItems[0];
                Argument argument = item.Tag as Argument;
                argument.Value = this.valueTextBox.Text;
                argument.Key = this.keyTextBox.Text;
                item.Text = message + " $(" + argument.Key + ")";
            }
        }

        /// <summary>
        /// Exports the current argument list into a file
        /// </summary>
        private void ExportArguments(Object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda";
            sfd.InitialDirectory = Globals.MainForm.WorkingDirectory;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                List<Argument> args = new List<Argument>();
                foreach (ListViewItem item in this.argsListView.SelectedItems)
                {
                    args.Add((Argument)item.Tag);
                }
                ObjectSerializer.Serialize(sfd.FileName, args);
            }
        }

        /// <summary>
        /// Imports an argument list from a file
        /// </summary>
        private void ImportArguments(Object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda";
            ofd.InitialDirectory = Globals.MainForm.WorkingDirectory;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                List<Argument> args = new List<Argument>();
                args = (List<Argument>)ObjectSerializer.Deserialize(ofd.FileName, args, false);
                arguments.AddRange(args); // Append imported
                this.PopulateArgumentList(arguments);
            }
        }

        /// <summary>
        /// Loads the arguments from the settings
        /// </summary>
        private void DialogLoad(Object sender, EventArgs e)
        {
            this.PopulateArgumentList(arguments);
        }

        /// <summary>
        /// Shows the argument dialog
        /// </summary>
        public static new void Show()
        {
            ArgumentDialog argumentDialog = new ArgumentDialog();
            argumentDialog.ShowDialog();
        }

        #endregion

        #region User Argument Management

        /// <summary>
        /// Loads the argument list from file
        /// </summary>
        public static void LoadCustomArguments()
        {
            String file = FileNameHelper.UserArgData;
            if (File.Exists(file))
            {
                Object data = ObjectSerializer.Deserialize(file, arguments, false);
                arguments = (List<Argument>)data;
            }
            if (!File.Exists(file) || arguments.Count == 0)
            {
                arguments.Add(new Argument("DefaultUser", "..."));
            }
        }

        /// <summary>
        /// Saves the argument list to file
        /// </summary>
        public static void SaveCustomArguments()
        {
            String file = FileNameHelper.UserArgData;
            ObjectSerializer.Serialize(file, arguments);
        }

        #endregion

    }

}
