using System;
using System.IO;
using System.Text;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using FlashDevelop.Utilities;
using FlashDevelop.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class SnippetDialog : SmartForm
    {
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label insertLabel;
        private System.Windows.Forms.Label snippetsLabel;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.Button revertButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.ListView snippetListView;
        private System.Windows.Forms.TextBox contentsTextBox;
        private System.Windows.Forms.ComboBox languageDropDown;
        private System.Windows.Forms.TextBox snippetNameTextBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Collections.Generic.Dictionary<String, String[]> snippets;
        private Ookii.Dialogs.VistaFolderBrowserDialog browseDialog;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ComboBox insertComboBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addButton;
        private System.String currentSyntax;
        private System.Int32 folderCount;
        private System.Int32 eolMode;

        public SnippetDialog()
        {
            this.eolMode = 0;
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "38535b88-d4b2-4db5-a6f5-40cc0ce3cb01";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
            this.PopulateControls();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.contentsTextBox = new System.Windows.Forms.TextBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.revertButton = new System.Windows.Forms.Button();
            this.snippetNameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.languageLabel = new System.Windows.Forms.Label();
            this.snippetListView = new System.Windows.Forms.ListView();
            this.snippetsLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.insertLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.browseDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            this.languageDropDown = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.insertComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // saveFileDialog
            //
            this.saveFileDialog.AddExtension = true;
            this.saveFileDialog.DefaultExt = "fdz";
            this.saveFileDialog.Filter = "FlashDevelop Zip Files|*.fdz";
            // 
            // contentsTextBox
            //
            this.contentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.contentsTextBox.AcceptsTab = true;
            this.contentsTextBox.AcceptsReturn = true;
            this.contentsTextBox.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentsTextBox.Location = new System.Drawing.Point(151, 53);
            this.contentsTextBox.ScrollBars = ScrollBars.Vertical;
            this.contentsTextBox.Multiline = true;
            this.contentsTextBox.Name = "contentsTextBox";
            this.contentsTextBox.Size = new System.Drawing.Size(453, 299);
            this.contentsTextBox.TabIndex = 8;
            this.contentsTextBox.WordWrap = false;
            this.contentsTextBox.TextChanged += new System.EventHandler(this.ToggleCreate);
            // 
            // addButton
            //
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addButton.Location = new System.Drawing.Point(254, 358);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(80, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "&Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // deleteButton
            //
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteButton.Location = new System.Drawing.Point(343, 358);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(80, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // snippetNameTextBox
            //
            this.snippetNameTextBox.Location = new System.Drawing.Point(151, 26);
            this.snippetNameTextBox.Name = "snippetNameTextBox";
            this.snippetNameTextBox.Size = new System.Drawing.Size(140, 19);
            this.snippetNameTextBox.TabIndex = 6;
            this.snippetNameTextBox.TextChanged += new System.EventHandler(this.ToggleCreate);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nameLabel.Location = new System.Drawing.Point(151, 8);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(85, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Snippet name:";
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.languageLabel.Location = new System.Drawing.Point(12, 8);
            this.languageLabel.Name = "nameLabel";
            this.languageLabel.Size = new System.Drawing.Size(85, 13);
            this.languageLabel.TabIndex = 0;
            this.languageLabel.Text = "Language:";
            // 
            // snippetListView
            //
            this.snippetListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.snippetListView.MultiSelect = false;
            this.snippetListView.HideSelection = false;
            this.snippetListView.Columns.Add(this.columnHeader);
            this.snippetListView.View = System.Windows.Forms.View.Details;
            this.snippetListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.snippetListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.snippetListView.Location = new System.Drawing.Point(12, 53);
            this.snippetListView.Name = "snippetListBox";
            this.snippetListView.Size = new System.Drawing.Size(130, 329);
            this.snippetListView.TabIndex = 5;
            this.snippetListView.SelectedIndexChanged += new System.EventHandler(this.SnippetListViewSelectedIndexChanged);
            // 
            // saveButton
            //
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveButton.Location = new System.Drawing.Point(431, 358);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(80, 23);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "&Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonClick);
            // 
            // exportButton
            //
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportButton.Name = "exportButton";
            this.exportButton.TabIndex = 9;
            this.exportButton.Size = new System.Drawing.Size(30, 23);
            this.exportButton.Location = new System.Drawing.Point(150, 358);
            this.exportButton.Click += new System.EventHandler(this.ExportButtonClick);
            // 
            // insertLabel
            // 
            this.insertLabel.AutoSize = true;
            this.insertLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.insertLabel.Location = new System.Drawing.Point(300, 8);
            this.insertLabel.Name = "insertLabel";
            this.insertLabel.Size = new System.Drawing.Size(93, 13);
            this.insertLabel.TabIndex = 0;
            this.insertLabel.Text = "Insert instruction:";
            // 
            // languageDropDown
            // 
            this.languageDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageDropDown.MaxLength = 200;
            this.languageDropDown.Name = "languageDropDown";
            this.languageDropDown.TabIndex = 4;
            this.languageDropDown.Location = new System.Drawing.Point(12, 25);
            this.languageDropDown.Size = new System.Drawing.Size(130, 23);
            this.languageDropDown.SelectedIndexChanged += new System.EventHandler(this.LanguagesSelectedIndexChanged);
            // 
            // closeButton
            //
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(519, 358);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(85, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // revertButton
            //
            this.revertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.revertButton.Location = new System.Drawing.Point(188, 358);
            this.revertButton.Name = "revertButton";
            this.revertButton.Size = new System.Drawing.Size(30, 23);
            this.revertButton.TabIndex = 10;
            this.revertButton.UseVisualStyleBackColor = true;
            this.revertButton.Click += new System.EventHandler(this.RevertButtonClick);
            // 
            // insertComboBox
            //
            this.insertComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.insertComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.insertComboBox.FormattingEnabled = true;
            this.insertComboBox.Location = new System.Drawing.Point(299, 25);
            this.insertComboBox.MaxDropDownItems = 15;
            this.insertComboBox.Name = "insertComboBox";
            this.insertComboBox.Size = new System.Drawing.Size(305, 21);
            this.insertComboBox.TabIndex = 7;
            this.insertComboBox.SelectedIndexChanged += new System.EventHandler(this.InsertComboBoxSelectedIndexChanged);
            // 
            // SnippetDialog
            //
            this.ShowIcon = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.AcceptButton = this.closeButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 393);
            this.MinimumSize = new System.Drawing.Size(615, 393);
            this.Controls.Add(this.languageDropDown);
            this.Controls.Add(this.insertComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.snippetNameTextBox);
            this.Controls.Add(this.snippetListView);
            this.Controls.Add(this.contentsTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.revertButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.insertLabel);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.nameLabel);
            this.Name = "SnippetDialog";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = " Snippet Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Gets the path to the language directory
        /// </summary>
        private String SnippetDir
        {
            get { return PathHelper.SnippetDir; }
        }

        /// <summary>
        /// Initializes the graphics
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("341", false));
            imageList.Images.Add(PluginBase.MainForm.FindImage("342|24|3|3", false)); // revert
            imageList.Images.Add(PluginBase.MainForm.FindImage("342|9|3|3", false)); // export
            this.snippetListView.SmallImageList = imageList;
            this.snippetListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.revertButton.ImageList = imageList;
            this.exportButton.ImageList = imageList;
            this.revertButton.ImageIndex = 1;
            this.exportButton.ImageIndex = 2;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            this.contentsTextBox.Font = Globals.Settings.ConsoleFont;
            this.insertComboBox.FlatStyle = Globals.Settings.ComboBoxFlatStyle;
            this.languageDropDown.FlatStyle = Globals.Settings.ComboBoxFlatStyle;
            tooltip.SetToolTip(this.exportButton, TextHelper.GetString("Label.ExportFiles"));
            tooltip.SetToolTip(this.revertButton, TextHelper.GetString("Label.RevertFiles"));
            this.saveFileDialog.Filter = TextHelper.GetString("Info.ZipFilter");
            this.Text = " " + TextHelper.GetString("Title.SnippetDialog");
            this.insertLabel.Text = TextHelper.GetString("Info.InsertInstruction");
            this.snippetsLabel.Text = TextHelper.GetString("Info.Snippets");
            this.nameLabel.Text = TextHelper.GetString("Info.SnippetName");
            this.languageLabel.Text = TextHelper.GetString("Info.Language");
            this.deleteButton.Text = TextHelper.GetString("Label.Delete");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.saveButton.Text = TextHelper.GetString("Label.Save");
            this.addButton.Text = TextHelper.GetString("Label.Add");
            if (Globals.MainForm.StandaloneMode)
            {
                this.revertButton.Enabled = false;
            }
        }

        /// <summary>
        /// Populates the controls from scratch
        /// </summary>
        private void PopulateControls()
        {
            this.snippets = new Dictionary<String, String[]>();
            this.ListSnippetFolders();
            this.UpdateSnippetList();
            this.PopulateInsertComboBox();
            Boolean foundSyntax = false;
            String curSyntax = ArgsProcessor.GetCurSyntax();
            foreach (Object item in this.languageDropDown.Items)
            {
                if (item.ToString().ToLower() == curSyntax)
                {
                    this.languageDropDown.SelectedItem = item;
                    foundSyntax = true;
                    break;
                }
            }
            if (!foundSyntax && this.languageDropDown.Items.Count > 0)
            {
                this.languageDropDown.SelectedIndex = 0;
            }
            this.columnHeader.Width = -2;
        }

        /// <summary>
        /// Updates the view based on the syntax selected
        /// </summary>
        private void LanguagesSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.saveButton.Enabled) this.PromptToSaveSnippet();
            this.currentSyntax = this.languageDropDown.Text.ToLower();
            this.snippetNameTextBox.Text = "";
            this.contentsTextBox.Text = "";
            this.saveButton.Enabled = false;
            this.UpdateSnippetList();
        }

        /// <summary>
        /// Saves the snippet with the selected name
        /// </summary>
        private void SaveButtonClick(Object sender, EventArgs e)
        {
            try
            {
                this.saveButton.Enabled = false;
                this.WriteFile(this.snippetNameTextBox.Text, this.contentsTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Closes the snippet manager dialog
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            if (this.saveButton.Enabled) this.PromptToSaveSnippet();
            this.Close();
        }

        /// <summary>
        /// Updates the view based on the selected snippet name
        /// </summary>
        private void ToggleCreate(Object sender, EventArgs e)
        {
            if (this.contentsTextBox.Text.Length > 0 && this.snippetNameTextBox.Text.Length > 0)
            {
                this.deleteButton.Enabled = false;
                foreach (ListViewItem item in this.snippetListView.Items)
                {
                    if (item.Text == this.snippetNameTextBox.Text)
                    {
                        this.deleteButton.Enabled = true;
                        break;
                    }
                }
            }
            if (this.snippetNameTextBox.Text.Length > 0)
            {
                this.saveButton.Enabled = true;
            }
            else this.saveButton.Enabled = false;
        }

        /// <summary>
        /// Shows the activated snippet's contents
        /// </summary>
        private void SnippetListViewSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.snippetListView.SelectedItems.Count == 0) return;
            if (this.saveButton.Enabled) this.PromptToSaveSnippet();
            String name = this.snippetListView.SelectedItems[0].Text;
            String path = Path.Combine(this.SnippetDir, this.currentSyntax);
            path = Path.Combine(path, name + ".fds");
            String content = File.ReadAllText(path);
            // Convert eols to windows and save current eol mode
            this.eolMode = LineEndDetector.DetectNewLineMarker(content, 0);
            content = content.Replace(LineEndDetector.GetNewLineMarker(this.eolMode), "\r\n");
            this.snippetNameTextBox.Text = name;
            this.contentsTextBox.Text = content;
            this.saveButton.Enabled = false;
        }

        /// <summary>
        /// Deletes the currently selected snippet
        /// </summary>
        private void DeleteButtonClick(Object sender, EventArgs e)
        {
            String caption = TextHelper.GetString("Title.ConfirmDialog");
            String message = TextHelper.GetString("Info.ConfirmSnippetDelete");
            if (MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            String path = Path.Combine(this.SnippetDir, this.currentSyntax);
            path = Path.Combine(path, this.snippetNameTextBox.Text + ".fds");
            if (!FileHelper.Recycle(path))
            {
                String error = TextHelper.GetString("Info.CouldNotBeRecycled");
                throw new Exception(error + " " + path);
            }
            this.UpdateSnippetList();
        }

        /// <summary>
        /// Lists all found snippets for the selected syntax
        /// </summary>
        private void ListSnippetFolders()
        {
            String[] folders = Directory.GetDirectories(this.SnippetDir);
            this.folderCount = folders.Length;
            this.languageDropDown.Items.Clear();
            foreach (String folderPath in folders)
            {
                DirectoryInfo info = new DirectoryInfo(folderPath);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                String folderName = Path.GetFileNameWithoutExtension(folderPath);
                String[] files = Directory.GetFiles(folderPath);
                this.snippets.Add(folderName, files);
                this.languageDropDown.Items.Add(folderName.ToUpper());
            }
            this.currentSyntax = this.languageDropDown.Text.ToLower();
        }

        /// <summary>
        /// Updates the snippet list based on the found files
        /// </summary>
        private void UpdateSnippetList()
        {
            this.UpdateSnippetList(null);
        }
        private void UpdateSnippetList(String toSelect)
        {
            try
            {
                Int32 selectedIndex = 0;
                if (this.snippetListView.SelectedIndices.Count > 0)
                {
                    selectedIndex = this.snippetListView.SelectedIndices[0];
                }
                this.snippetListView.BeginUpdate();
                this.snippetListView.Items.Clear();
                String path = Path.Combine(this.SnippetDir, this.currentSyntax);
                this.snippets[this.currentSyntax] = Directory.GetFiles(path);
                foreach (String file in this.snippets[this.currentSyntax])
                {
                    String snippet = Path.GetFileNameWithoutExtension(file);
                    ListViewItem lvi = new ListViewItem(snippet, 0);
                    this.snippetListView.Items.Add(lvi);
                    if (!String.IsNullOrEmpty(toSelect) && snippet == toSelect)
                    {
                        lvi.Selected = true;
                    }
                }
                this.snippetListView.EndUpdate();
                if (this.snippetListView.Items.Count > 0 && String.IsNullOrEmpty(toSelect))
                {
                    try { this.snippetListView.Items[selectedIndex].Selected = true; }
                    catch 
                    { 
                        Int32 last = this.snippetListView.Items.Count - 1;
                        this.snippetListView.Items[last].Selected = true;
                    }
                }
                else this.snippetListView.Select();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Populates the insert combobox
        /// </summary>
        private void PopulateInsertComboBox()
        {
            try
            {
                String locale = Globals.Settings.LocaleVersion.ToString();
                Stream stream = ResourceHelper.GetStream(String.Format("SnippetVars.{0}.txt", locale));
                String contents = new StreamReader(stream).ReadToEnd();
                if (DistroConfig.DISTRIBUTION_NAME != "FlashDevelop")
                {
                    #pragma warning disable CS0162 // Unreachable code detected
                    contents = contents.Replace("FlashDevelop", DistroConfig.DISTRIBUTION_NAME);
                    #pragma warning restore CS0162 // Unreachable code detected
                }
                String[] varLines = contents.Split(new Char[1]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in varLines)
                {
                    this.insertComboBox.Items.Add(line.Trim());
                }
                this.insertComboBox.SelectedIndex = 0;
                stream.Close(); stream.Dispose();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Handles the inserting of an instruction
        /// </summary>
        private void InsertComboBoxSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.insertComboBox.SelectedItem != null && this.insertComboBox.SelectedIndex != 0)
            {
                this.contentsTextBox.Focus();
                String data = this.insertComboBox.SelectedItem.ToString();
                if (!data.StartsWith('-'))
                {
                    Int32 variableEnd = data.IndexOfOrdinal(")") + 1;
                    String variable = data.Substring(0, variableEnd);
                    this.InsertText(this.contentsTextBox, variable);
                }
                this.insertComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Asks the user to save the changes
        /// </summary>
        private void PromptToSaveSnippet()
        {
            if (this.snippetNameTextBox.Text.Length == 0) return;
            String message = TextHelper.GetString("Info.SaveCurrentSnippet");
            String caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.saveButton.Enabled = false;
                this.WriteFile(this.snippetNameTextBox.Text, this.contentsTextBox.Text);
            }
        }

        /// <summary>
        /// Clears the texts to present a creation of a new item
        /// </summary>
        private void AddButtonClick(Object sender, EventArgs e)
        {
            this.contentsTextBox.Text = "";
            this.snippetNameTextBox.Text = "";
            this.deleteButton.Enabled = false;
            this.saveButton.Enabled = false;
        }

        /// <summary>
        /// Opens the revert settings dialog
        /// </summary>
        private void RevertButtonClick(Object sender, EventArgs e)
        {
            String caption = TextHelper.GetString("Title.ConfirmDialog");
            String message = TextHelper.GetString("Info.RevertSnippetFiles");
            String appSnippetDir = Path.Combine(PathHelper.AppDir, "Snippets");
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Enabled = false;
                FolderHelper.CopyFolder(appSnippetDir, this.SnippetDir);
                this.PopulateControls();
                this.Enabled = true;
            }
        }

        /// <summary>
        /// Asks to export the snippet files
        /// </summary>
        private void ExportButtonClick(Object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ZipFile zipFile = ZipFile.Create(this.saveFileDialog.FileName);
                String[] snippetFiles = Directory.GetFiles(this.SnippetDir, "*.fds", SearchOption.AllDirectories);
                zipFile.BeginUpdate();
                foreach (String snippetFile in snippetFiles)
                {
                    Int32 index = snippetFile.IndexOfOrdinal("\\Snippets\\");
                    zipFile.Add(snippetFile, "$(BaseDir)" + snippetFile.Substring(index));
                }
                zipFile.CommitUpdate();
                zipFile.Close();
            }
        }

        /// <summary>
        /// Writes the snippet to the specified file
        /// </summary>
        private void WriteFile(String name, String content)
        {
            StreamWriter file;
            // Restore previous eol mode
            content = content.Replace("\r\n", LineEndDetector.GetNewLineMarker(this.eolMode));
            String path = Path.Combine(this.SnippetDir, this.currentSyntax);
            path = Path.Combine(path, name + ".fds");
            file = File.CreateText(path);
            file.Write(content);
            file.Close();
            this.UpdateSnippetList(name);
        }

        /// <summary>
        /// Inserts text to the current location or selection
        /// </summary>
        private void InsertText(TextBox target, String text)
        {
            target.SelectedText = text;
            target.Focus();
        }

        /// <summary>
        /// Shows the snippets dialog
        /// </summary>
        public static new void Show()
        {
            SnippetDialog snippetDialog = new SnippetDialog();
            snippetDialog.CenterToParent();
            snippetDialog.Show(Globals.MainForm);
        }

        #endregion

    }

}