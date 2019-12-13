using System;
using System.IO;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
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
        Label nameLabel;
        Label insertLabel;
        Label snippetsLabel;
        Label languageLabel;
        Button revertButton;
        Button exportButton;
        ListView snippetListView;
        TextBox contentsTextBox;
        ComboBox languageDropDown;
        TextBox snippetNameTextBox;
        SaveFileDialog saveFileDialog;
        Dictionary<string, string[]> snippets;
        Ookii.Dialogs.VistaFolderBrowserDialog browseDialog;
        ColumnHeader columnHeader;
        ComboBox insertComboBox;
        Button deleteButton;
        Button closeButton;
        Button saveButton;
        Button addButton;
        string currentSyntax;
        int eolMode;

        public SnippetDialog()
        {
            eolMode = 0;
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "38535b88-d4b2-4db5-a6f5-40cc0ce3cb01";
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeGraphics();
            PopulateControls();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            addButton = new ButtonEx();
            exportButton = new ButtonEx();
            columnHeader = new ColumnHeader();
            contentsTextBox = new TextBoxEx();
            deleteButton = new ButtonEx();
            revertButton = new ButtonEx();
            snippetNameTextBox = new TextBoxEx();
            nameLabel = new Label();
            languageLabel = new Label();
            snippetListView = new ListViewEx();
            snippetsLabel = new Label();
            saveButton = new ButtonEx();
            insertLabel = new Label();
            saveFileDialog = new SaveFileDialog();
            browseDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            languageDropDown = new FlatCombo();
            closeButton = new ButtonEx();
            insertComboBox = new FlatCombo();
            SuspendLayout();
            // 
            // saveFileDialog
            //
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = "fdz";
            saveFileDialog.Filter = "FlashDevelop Zip Files|*.fdz";
            // 
            // contentsTextBox
            //
            contentsTextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            contentsTextBox.AcceptsTab = true;
            contentsTextBox.AcceptsReturn = true;
            contentsTextBox.Font = new Font("Courier New", 8.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            contentsTextBox.Location = new Point(151, 53);
            contentsTextBox.ScrollBars = ScrollBars.Vertical;
            contentsTextBox.Multiline = true;
            contentsTextBox.Name = "contentsTextBox";
            contentsTextBox.Size = new Size(453, 299);
            contentsTextBox.TabIndex = 8;
            contentsTextBox.WordWrap = false;
            contentsTextBox.TextChanged += ToggleCreate;
            // 
            // addButton
            //
            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.FlatStyle = FlatStyle.System;
            addButton.Location = new Point(254, 358);
            addButton.Name = "addButton";
            addButton.Size = new Size(80, 23);
            addButton.TabIndex = 3;
            addButton.Text = "&Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButtonClick;
            // 
            // deleteButton
            //
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            deleteButton.FlatStyle = FlatStyle.System;
            deleteButton.Location = new Point(343, 358);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(80, 23);
            deleteButton.TabIndex = 3;
            deleteButton.Text = "&Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += DeleteButtonClick;
            // 
            // snippetNameTextBox
            //
            snippetNameTextBox.Location = new Point(151, 25);
            snippetNameTextBox.Name = "snippetNameTextBox";
            snippetNameTextBox.Size = new Size(140, 19);
            snippetNameTextBox.TabIndex = 6;
            snippetNameTextBox.TextChanged += ToggleCreate;
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.FlatStyle = FlatStyle.System;
            nameLabel.Location = new Point(151, 8);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(85, 13);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Snippet name:";
            // 
            // languageLabel
            // 
            languageLabel.AutoSize = true;
            languageLabel.FlatStyle = FlatStyle.System;
            languageLabel.Location = new Point(12, 8);
            languageLabel.Name = "nameLabel";
            languageLabel.Size = new Size(85, 13);
            languageLabel.TabIndex = 0;
            languageLabel.Text = "Language:";
            // 
            // snippetListView
            //
            snippetListView.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left;
            snippetListView.MultiSelect = false;
            snippetListView.HideSelection = false;
            snippetListView.Columns.Add(columnHeader);
            snippetListView.View = View.Details;
            snippetListView.Alignment = ListViewAlignment.Left;
            snippetListView.HeaderStyle = ColumnHeaderStyle.None;
            snippetListView.Location = new Point(12, 53);
            snippetListView.Name = "snippetListBox";
            snippetListView.Size = new Size(130, 329);
            snippetListView.TabIndex = 5;
            snippetListView.SelectedIndexChanged += SnippetListViewSelectedIndexChanged;
            // 
            // saveButton
            //
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.FlatStyle = FlatStyle.System;
            saveButton.Location = new Point(431, 358);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(80, 23);
            saveButton.TabIndex = 2;
            saveButton.Text = "&Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButtonClick;
            // 
            // exportButton
            //
            exportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            exportButton.Name = "exportButton";
            exportButton.TabIndex = 9;
            exportButton.Size = new Size(30, 23);
            exportButton.Location = new Point(150, 358);
            exportButton.Click += ExportButtonClick;
            // 
            // insertLabel
            // 
            insertLabel.AutoSize = true;
            insertLabel.FlatStyle = FlatStyle.System;
            insertLabel.Location = new Point(300, 8);
            insertLabel.Name = "insertLabel";
            insertLabel.Size = new Size(93, 13);
            insertLabel.TabIndex = 0;
            insertLabel.Text = "Insert instruction:";
            // 
            // languageDropDown
            // 
            languageDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            languageDropDown.MaxLength = 200;
            languageDropDown.Name = "languageDropDown";
            languageDropDown.TabIndex = 4;
            languageDropDown.Location = new Point(12, 25);
            languageDropDown.Size = new Size(130, 23);
            languageDropDown.SelectedIndexChanged += LanguagesSelectedIndexChanged;
            // 
            // closeButton
            //
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(519, 358);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(85, 23);
            closeButton.TabIndex = 1;
            closeButton.Text = "&Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButtonClick;
            // 
            // revertButton
            //
            revertButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            revertButton.Location = new Point(188, 358);
            revertButton.Name = "revertButton";
            revertButton.Size = new Size(30, 23);
            revertButton.TabIndex = 10;
            revertButton.UseVisualStyleBackColor = true;
            revertButton.Click += RevertButtonClick;
            // 
            // insertComboBox
            //
            insertComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            insertComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            insertComboBox.FormattingEnabled = true;
            insertComboBox.Location = new Point(299, 25);
            insertComboBox.MaxDropDownItems = 15;
            insertComboBox.Name = "insertComboBox";
            insertComboBox.Size = new Size(305, 21);
            insertComboBox.TabIndex = 7;
            insertComboBox.SelectedIndexChanged += InsertComboBoxSelectedIndexChanged;
            // 
            // SnippetDialog
            //
            ShowIcon = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            AcceptButton = closeButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 394);
            MinimumSize = new Size(615, 393);
            Controls.Add(languageDropDown);
            Controls.Add(insertComboBox);
            Controls.Add(closeButton);
            Controls.Add(snippetNameTextBox);
            Controls.Add(snippetListView);
            Controls.Add(contentsTextBox);
            Controls.Add(saveButton);
            Controls.Add(addButton);
            Controls.Add(revertButton);
            Controls.Add(exportButton);
            Controls.Add(deleteButton);
            Controls.Add(insertLabel);
            Controls.Add(languageLabel);
            Controls.Add(nameLabel);
            Name = "SnippetDialog";
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterParent;
            SizeGripStyle = SizeGripStyle.Show;
            Text = " Snippet Editor";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the graphics
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("341", false));
            imageList.Images.Add(PluginBase.MainForm.FindImage("342|24|3|3", false)); // revert
            imageList.Images.Add(PluginBase.MainForm.FindImage("342|9|3|3", false)); // export
            snippetListView.SmallImageList = imageList;
            snippetListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            revertButton.ImageList = imageList;
            exportButton.ImageList = imageList;
            revertButton.ImageIndex = 1;
            exportButton.ImageIndex = 2;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            contentsTextBox.Font = PluginBase.Settings.ConsoleFont;
            insertComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            languageDropDown.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            tooltip.SetToolTip(exportButton, TextHelper.GetString("Label.ExportFiles"));
            tooltip.SetToolTip(revertButton, TextHelper.GetString("Label.RevertFiles"));
            saveFileDialog.Filter = TextHelper.GetString("Info.ZipFilter");
            Text = " " + TextHelper.GetString("Title.SnippetDialog");
            insertLabel.Text = TextHelper.GetString("Info.InsertInstruction");
            snippetsLabel.Text = TextHelper.GetString("Info.Snippets");
            nameLabel.Text = TextHelper.GetString("Info.SnippetName");
            languageLabel.Text = TextHelper.GetString("Info.Language");
            deleteButton.Text = TextHelper.GetString("Label.Delete");
            closeButton.Text = TextHelper.GetString("Label.Close");
            saveButton.Text = TextHelper.GetString("Label.Save");
            addButton.Text = TextHelper.GetString("Label.Add");
            if (PluginBase.MainForm.StandaloneMode)
            {
                revertButton.Enabled = false;
            }
        }

        /// <summary>
        /// Populates the controls from scratch
        /// </summary>
        void PopulateControls()
        {
            snippets = new Dictionary<string, string[]>();
            ListSnippetFolders();
            UpdateSnippetList();
            PopulateInsertComboBox();
            bool foundSyntax = false;
            string curSyntax = ArgsProcessor.GetCurSyntax();
            foreach (object item in languageDropDown.Items)
            {
                if (item.ToString().ToLower() == curSyntax)
                {
                    languageDropDown.SelectedItem = item;
                    foundSyntax = true;
                    break;
                }
            }
            if (!foundSyntax && languageDropDown.Items.Count > 0)
            {
                languageDropDown.SelectedIndex = 0;
            }
            columnHeader.Width = -2;
        }

        /// <summary>
        /// Updates the view based on the syntax selected
        /// </summary>
        void LanguagesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (saveButton.Enabled) PromptToSaveSnippet();
            currentSyntax = languageDropDown.Text.ToLower();
            snippetNameTextBox.Text = "";
            contentsTextBox.Text = "";
            saveButton.Enabled = false;
            UpdateSnippetList();
        }

        /// <summary>
        /// Saves the snippet with the selected name
        /// </summary>
        void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                saveButton.Enabled = false;
                WriteFile(snippetNameTextBox.Text, contentsTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Closes the snippet manager dialog
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e)
        {
            if (saveButton.Enabled) PromptToSaveSnippet();
            Close();
        }

        /// <summary>
        /// Updates the view based on the selected snippet name
        /// </summary>
        void ToggleCreate(object sender, EventArgs e)
        {
            if (contentsTextBox.Text.Length > 0 && snippetNameTextBox.Text.Length > 0)
            {
                deleteButton.Enabled = false;
                foreach (ListViewItem item in snippetListView.Items)
                {
                    if (item.Text == snippetNameTextBox.Text)
                    {
                        deleteButton.Enabled = true;
                        break;
                    }
                }
            }
            saveButton.Enabled = snippetNameTextBox.Text.Length > 0;
        }

        /// <summary>
        /// Shows the activated snippet's contents
        /// </summary>
        void SnippetListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (snippetListView.SelectedItems.Count == 0) return;
            if (saveButton.Enabled) PromptToSaveSnippet();
            string name = snippetListView.SelectedItems[0].Text;
            var path = Path.Combine(PathHelper.SnippetDir, currentSyntax, name + ".fds");
            string content = File.ReadAllText(path);
            // Convert eols to windows and save current eol mode
            eolMode = LineEndDetector.DetectNewLineMarker(content, 0);
            content = content.Replace(LineEndDetector.GetNewLineMarker(eolMode), "\r\n");
            snippetNameTextBox.Text = name;
            contentsTextBox.Text = content;
            saveButton.Enabled = false;
        }

        /// <summary>
        /// Deletes the currently selected snippet
        /// </summary>
        void DeleteButtonClick(object sender, EventArgs e)
        {
            string caption = TextHelper.GetString("Title.ConfirmDialog");
            string message = TextHelper.GetString("Info.ConfirmSnippetDelete");
            if (MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            var path = Path.Combine(PathHelper.SnippetDir, currentSyntax, snippetNameTextBox.Text + ".fds");
            if (!FileHelper.Recycle(path))
            {
                string error = TextHelper.GetString("Info.CouldNotBeRecycled");
                throw new Exception(error + " " + path);
            }
            UpdateSnippetList();
        }

        /// <summary>
        /// Lists all found snippets for the selected syntax
        /// </summary>
        void ListSnippetFolders()
        {
            string[] folders = Directory.GetDirectories(PathHelper.SnippetDir);
            languageDropDown.Items.Clear();
            foreach (string folderPath in folders)
            {
                DirectoryInfo info = new DirectoryInfo(folderPath);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                string folderName = Path.GetFileNameWithoutExtension(folderPath);
                string[] files = Directory.GetFiles(folderPath);
                snippets.Add(folderName, files);
                languageDropDown.Items.Add(folderName.ToUpper());
            }
            currentSyntax = languageDropDown.Text.ToLower();
        }

        /// <summary>
        /// Updates the snippet list based on the found files
        /// </summary>
        void UpdateSnippetList() => UpdateSnippetList(null);

        void UpdateSnippetList(string toSelect)
        {
            try
            {
                int selectedIndex = 0;
                if (snippetListView.SelectedIndices.Count > 0)
                {
                    selectedIndex = snippetListView.SelectedIndices[0];
                }
                snippetListView.BeginUpdate();
                snippetListView.Items.Clear();
                string path = Path.Combine(PathHelper.SnippetDir, currentSyntax);
                snippets[currentSyntax] = Directory.GetFiles(path);
                foreach (string file in snippets[currentSyntax])
                {
                    string snippet = Path.GetFileNameWithoutExtension(file);
                    ListViewItem lvi = new ListViewItem(snippet, 0);
                    snippetListView.Items.Add(lvi);
                    if (!string.IsNullOrEmpty(toSelect) && snippet == toSelect)
                    {
                        lvi.Selected = true;
                    }
                }
                snippetListView.EndUpdate();
                if (snippetListView.Items.Count > 0 && string.IsNullOrEmpty(toSelect))
                {
                    try { snippetListView.Items[selectedIndex].Selected = true; }
                    catch 
                    { 
                        int last = snippetListView.Items.Count - 1;
                        snippetListView.Items[last].Selected = true;
                    }
                }
                else snippetListView.Select();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Populates the insert combobox
        /// </summary>
        void PopulateInsertComboBox()
        {
            try
            {
                string locale = PluginBase.Settings.LocaleVersion.ToString();
                Stream stream = ResourceHelper.GetStream($"SnippetVars.{locale}.txt");
                string contents = new StreamReader(stream).ReadToEnd();
                if (DistroConfig.DISTRIBUTION_NAME != "FlashDevelop")
                {
                    #pragma warning disable CS0162 // Unreachable code detected
                    contents = contents.Replace("FlashDevelop", DistroConfig.DISTRIBUTION_NAME);
                    #pragma warning restore CS0162 // Unreachable code detected
                }
                var varLines = contents.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in varLines)
                {
                    insertComboBox.Items.Add(line.Trim());
                }
                insertComboBox.SelectedIndex = 0;
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
        void InsertComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (insertComboBox.SelectedItem != null && insertComboBox.SelectedIndex != 0)
            {
                contentsTextBox.Focus();
                var data = insertComboBox.SelectedItem.ToString();
                if (!data.StartsWith('-'))
                {
                    var variableEnd = data.IndexOf(')') + 1;
                    var variable = data.Substring(0, variableEnd);
                    InsertText(contentsTextBox, variable);
                }
                insertComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Asks the user to save the changes
        /// </summary>
        void PromptToSaveSnippet()
        {
            if (snippetNameTextBox.Text.Length == 0) return;
            string message = TextHelper.GetString("Info.SaveCurrentSnippet");
            string caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                saveButton.Enabled = false;
                WriteFile(snippetNameTextBox.Text, contentsTextBox.Text);
            }
        }

        /// <summary>
        /// Clears the texts to present a creation of a new item
        /// </summary>
        void AddButtonClick(object sender, EventArgs e)
        {
            contentsTextBox.Text = "";
            snippetNameTextBox.Text = "";
            deleteButton.Enabled = false;
            saveButton.Enabled = false;
        }

        /// <summary>
        /// Opens the revert settings dialog
        /// </summary>
        void RevertButtonClick(object sender, EventArgs e)
        {
            string caption = TextHelper.GetString("Title.ConfirmDialog");
            string message = TextHelper.GetString("Info.RevertSnippetFiles");
            string appSnippetDir = Path.Combine(PathHelper.AppDir, "Snippets");
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Enabled = false;
                FolderHelper.CopyFolder(appSnippetDir, PathHelper.SnippetDir);
                PopulateControls();
                Enabled = true;
            }
        }

        /// <summary>
        /// Asks to export the snippet files
        /// </summary>
        void ExportButtonClick(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            var zipFile = ZipFile.Create(saveFileDialog.FileName);
            var snippetFiles = Directory.GetFiles(PathHelper.SnippetDir, "*.fds", SearchOption.AllDirectories);
            zipFile.BeginUpdate();
            foreach (string snippetFile in snippetFiles)
            {
                int index = snippetFile.IndexOfOrdinal("\\Snippets\\");
                zipFile.Add(snippetFile, "$(BaseDir)" + snippetFile.Substring(index));
            }
            zipFile.CommitUpdate();
            zipFile.Close();
        }

        /// <summary>
        /// Writes the snippet to the specified file
        /// </summary>
        void WriteFile(string name, string content)
        {
            // Restore previous eol mode
            content = content.Replace("\r\n", LineEndDetector.GetNewLineMarker(eolMode));
            var path = Path.Combine(PathHelper.SnippetDir, currentSyntax, name + ".fds");
            var file = File.CreateText(path);
            file.Write(content);
            file.Close();
            UpdateSnippetList(name);
        }

        /// <summary>
        /// Inserts text to the current location or selection
        /// </summary>
        static void InsertText(TextBoxBase target, string text)
        {
            target.SelectedText = text;
            target.Focus();
        }

        /// <summary>
        /// Shows the snippets dialog
        /// </summary>
        public new static void Show()
        {
            /*using*/ var dialog = new SnippetDialog();
            dialog.CenterToParent();
            dialog.Show(PluginBase.MainForm);
        }

        #endregion
    }
}