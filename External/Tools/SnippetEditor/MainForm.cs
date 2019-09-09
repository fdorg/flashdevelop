using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace SnippetEditor
{
    public class MainForm : Form
    {
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tabPage;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label insertLabel;
        private System.Windows.Forms.Label snippetsLabel;
        private System.Windows.Forms.TextBox snippetNameTextBox;
        private System.Windows.Forms.TextBox contentsTextBox;
        private System.Windows.Forms.ListBox snippetListBox;
        private System.Windows.Forms.FolderBrowserDialog browseDialog;
        private System.Windows.Forms.ComboBox insertComboBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private Dictionary<String, String[]> snippets;
        private System.String snippetFolder;
        private System.String currentSyntax;
        private System.Int32 folderCount;
        private System.String[] args;

        public MainForm(String[] arguments)
        {
            this.args = arguments;
            this.Font = SystemFonts.MenuFont;
            this.InitializeComponent();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentsTextBox = new System.Windows.Forms.TextBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.snippetNameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.snippetListBox = new System.Windows.Forms.ListBox();
            this.snippetsLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.insertLabel = new System.Windows.Forms.Label();
            this.browseDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPage = new System.Windows.Forms.TabPage();
            this.tab = new System.Windows.Forms.TabControl();
            this.closeButton = new System.Windows.Forms.Button();
            this.insertComboBox = new System.Windows.Forms.ComboBox();
            this.tab.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentsTextBox
            // 
            this.contentsTextBox.AcceptsTab = true;
            this.contentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.contentsTextBox.Enabled = false;
            this.contentsTextBox.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentsTextBox.Location = new System.Drawing.Point(121, 90);
            this.contentsTextBox.Multiline = true;
            this.contentsTextBox.Name = "contentsTextBox";
            this.contentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.contentsTextBox.Size = new System.Drawing.Size(483, 289);
            this.contentsTextBox.TabIndex = 5;
            this.contentsTextBox.WordWrap = false;
            this.contentsTextBox.TextChanged += new System.EventHandler(this.ToggleCreate);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Enabled = false;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteButton.Location = new System.Drawing.Point(381, 386);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(70, 23);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // snippetNameTextBox
            // 
            this.snippetNameTextBox.Enabled = false;
            this.snippetNameTextBox.Location = new System.Drawing.Point(121, 63);
            this.snippetNameTextBox.Name = "snippetNameTextBox";
            this.snippetNameTextBox.Size = new System.Drawing.Size(147, 21);
            this.snippetNameTextBox.TabIndex = 3;
            this.snippetNameTextBox.TextChanged += new System.EventHandler(this.ToggleCreate);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nameLabel.Location = new System.Drawing.Point(123, 45);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(76, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Snippet name:";
            // 
            // snippetListBox
            // 
            this.snippetListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.snippetListBox.BackColor = System.Drawing.SystemColors.Window;
            this.snippetListBox.Enabled = false;
            this.snippetListBox.FormattingEnabled = true;
            this.snippetListBox.Location = new System.Drawing.Point(15, 63);
            this.snippetListBox.Name = "snippetListBox";
            this.snippetListBox.Size = new System.Drawing.Size(100, 316);
            this.snippetListBox.TabIndex = 2;
            this.snippetListBox.SelectedIndexChanged += new System.EventHandler(this.SnippetListBoxSelectedIndexChanged);
            // 
            // snippetsLabel
            // 
            this.snippetsLabel.AutoSize = true;
            this.snippetsLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.snippetsLabel.Location = new System.Drawing.Point(15, 45);
            this.snippetsLabel.Name = "snippetsLabel";
            this.snippetsLabel.Size = new System.Drawing.Size(52, 13);
            this.snippetsLabel.TabIndex = 0;
            this.snippetsLabel.Text = "Snippets:";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveButton.Location = new System.Drawing.Point(458, 386);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(70, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "&Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonClick);
            // 
            // insertLabel
            // 
            this.insertLabel.AutoSize = true;
            this.insertLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.insertLabel.Location = new System.Drawing.Point(285, 45);
            this.insertLabel.Name = "insertLabel";
            this.insertLabel.Size = new System.Drawing.Size(93, 13);
            this.insertLabel.TabIndex = 0;
            this.insertLabel.Text = "Insert instruction:";
            // 
            // tabPage
            // 
            this.tabPage.Location = new System.Drawing.Point(4, 22);
            this.tabPage.Name = "tabPage";
            this.tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage.Size = new System.Drawing.Size(588, 1);
            this.tabPage.TabIndex = 0;
            this.tabPage.UseVisualStyleBackColor = true;
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tabPage);
            this.tab.Location = new System.Drawing.Point(10, 12);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(596, 27);
            this.tab.TabIndex = 1;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(535, 386);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(70, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // insertComboBox
            // 
            this.insertComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.insertComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.insertComboBox.FormattingEnabled = true;
            this.insertComboBox.Location = new System.Drawing.Point(284, 63);
            this.insertComboBox.MaxDropDownItems = 15;
            this.insertComboBox.Name = "insertComboBox";
            this.insertComboBox.Size = new System.Drawing.Size(320, 21);
            this.insertComboBox.TabIndex = 4;
            this.insertComboBox.SelectedIndexChanged += new System.EventHandler(this.InsertComboBoxSelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(618, 424);
            this.Controls.Add(this.insertComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.snippetNameTextBox);
            this.Controls.Add(this.snippetListBox);
            this.Controls.Add(this.contentsTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.snippetsLabel);
            this.Controls.Add(this.insertLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.tab);
            this.MinimumSize = new System.Drawing.Size(626, 451);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Snippet Editor";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.tab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Updates the view based on the syntax selected
        /// </summary>
        private void TabSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.currentSyntax = this.tab.SelectedTab.Text;
            this.UpdateSnippetList();
            this.EnableAllControls();
        }

        /// <summary>
        /// Saves the snippet with the selected name
        /// </summary>
        private void SaveButtonClick(Object sender, EventArgs e)
        {
            try
            {
                if (this.snippetNameTextBox.Text.Length == 0) return;
                else this.WriteFile(this.snippetNameTextBox.Text, this.contentsTextBox.Text);
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
            this.Close();
        }

        /// <summary>
        /// Updates the view based on the selected snippet name
        /// </summary>
        private void ToggleCreate(Object sender, EventArgs e)
        {
            if (this.contentsTextBox.Text.Length > 0 && this.snippetNameTextBox.Text.Length > 0)
            {
                this.saveButton.Enabled = true;
                this.deleteButton.Enabled = this.snippetListBox.Items.Contains(this.snippetNameTextBox.Text);
            }
            else this.saveButton.Enabled = false;
        }

        /// <summary>
        /// Shows the activated snippet's contents
        /// </summary>
        private void SnippetListBoxSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.snippetListBox.SelectedItem == null) return;
            String name = this.snippetListBox.SelectedItem.ToString();
            String path = Path.Combine(this.snippetFolder, this.currentSyntax);
            path = Path.Combine(path, name + ".fds");
            String content = File.ReadAllText(path);
            this.snippetNameTextBox.Text = name;
            this.contentsTextBox.Text = content;
        }

        /// <summary>
        /// Deletes the currently selected snippet
        /// </summary>
        private void DeleteButtonClick(Object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this snippet?", " Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return;
            String path = Path.Combine(this.snippetFolder, this.currentSyntax);
            path = Path.Combine(path, this.snippetNameTextBox.Text + ".fds");
            if (!File.Exists(path)) return;
            else
            {
                File.Delete(path);
                this.UpdateSnippetList();
                if (this.snippetListBox.Items.Count > 0)
                {
                    this.snippetListBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Sets up the form on load
        /// </summary>
        private void MainFormLoad(Object sender, EventArgs e)
        {
            this.snippets = new Dictionary<String, String[]>();
            this.snippetNameTextBox.SelectAll();
            String exePath = Path.GetDirectoryName(Application.ExecutablePath);
            this.snippetFolder = Path.Combine(exePath, @"..\..\Snippets");
            if (!File.Exists(Path.Combine(exePath, @"..\..\.local")))
            {
                String appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                String userSnippets = Path.Combine(appData, @"FlashDevelop\Snippets");
                if (Directory.Exists(userSnippets)) this.snippetFolder = userSnippets;
            }
            this.GetSnippetFolder();
            if (this.snippetFolder != null && Directory.Exists(this.snippetFolder))
            {
                this.ListSnippetFolder();
                this.UpdateSnippetList();
                this.tab.SelectedIndexChanged += new EventHandler(this.TabSelectedIndexChanged);
                this.currentSyntax = this.tab.SelectedTab.Text;
                this.PopulateInsertComboBox();
                this.UpdateSnippetList();
                this.EnableAllControls();
                if (this.args.Length > 0)
                {
                    foreach (TabPage page in this.tab.TabPages)
                    {
                        if (page.Text == this.args[0])
                        {
                            this.tab.SelectedTab = page;
                            break;
                        }
                    }
                }
            }
            else this.Close();
        }

        /// <summary>
        /// Tries to locate the snippet folder based on the current location
        /// </summary>
        private Boolean GetSnippetFolder()
        {
            if (!Directory.Exists(this.snippetFolder))
            {
                this.browseDialog.Description = "Select your snippets directory location...";
                if (this.browseDialog.ShowDialog() == DialogResult.OK)
                {
                    this.snippetFolder = this.browseDialog.SelectedPath;
                    return true;
                }
                else return false;
            }
            return true;
        }

        /// <summary>
        /// Lists all found snippets for the selected syntax
        /// </summary>
        private void ListSnippetFolder()
        {
            Boolean foundSome = false;
            String[] folders = Directory.GetDirectories(this.snippetFolder);
            this.folderCount = folders.Length;
            this.tab.Controls.Clear();
            foreach (String folderPath in folders)
            {
                DirectoryInfo info = new DirectoryInfo(folderPath);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                foundSome = true;
                String folderName = Path.GetFileNameWithoutExtension(folderPath);
                String[] files = Directory.GetFiles(folderPath);
                Int32 fileCount = files.Length;
                this.snippets.Add(folderName, files);
                TabPage tabPage = new TabPage();
                tabPage.Text = folderName;
                tab.Controls.Add(tabPage);
            }
            if (!foundSome)
            {
                DialogResult result = MessageBox.Show("No snippet found in this location. Do you want to select another location?", "No snippet found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    this.snippetFolder = null;
                    if (this.GetSnippetFolder()) this.ListSnippetFolder();
                }
            }
        }

        /// <summary>
        /// Writes the snippet to the specified file
        /// </summary>
        private void WriteFile(String name, String content)
        {
            StreamWriter file;
            String path = Path.Combine(this.snippetFolder, this.currentSyntax);
            path = Path.Combine(path, name + ".fds");
            file = File.CreateText(path);
            file.WriteLine(content);
            file.Close();
            this.UpdateSnippetList();
        }

        /// <summary>
        /// Enables all controls of the form
        /// </summary>
        private void EnableAllControls()
        {
            this.snippetNameTextBox.Text = "";
            this.contentsTextBox.Text = "";
            this.contentsTextBox.Enabled = true;
            this.saveButton.Enabled = false;
            this.deleteButton.Enabled = false;
            this.snippetNameTextBox.Enabled = true;
            this.snippetListBox.Enabled = true;
        }

        /// <summary>
        /// Updates the snippet list based on the found files
        /// </summary>
        private void UpdateSnippetList()
        {
            if (this.currentSyntax == null) return;
            this.snippetListBox.BeginUpdate();
            try
            {
                this.snippetListBox.Items.Clear();
                String path = Path.Combine(this.snippetFolder, this.currentSyntax);
                this.snippets[this.currentSyntax] = Directory.GetFiles(path);
                foreach (String file in this.snippets[this.currentSyntax])
                {
                    String snippet = Path.GetFileNameWithoutExtension(file);
                    this.snippetListBox.Items.Add(snippet);
                }
            }
            finally
            {
                this.snippetListBox.EndUpdate();
            }
        }

        /// <summary>
        /// Populates the insert combobox
        /// </summary>
        private void PopulateInsertComboBox()
        {
            this.insertComboBox.Items.Add("Select instruction...");
            this.insertComboBox.Items.Add("- MAIN -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(EntryPoint) - Selection's start position");
            this.insertComboBox.Items.Add("$(ExitPoint) - Selection's end position.");
            this.insertComboBox.Items.Add("$(Boundary) - Keep all text between boundaries");
            this.insertComboBox.Items.Add("$(CSLB) - Coding style line break");
            this.insertComboBox.Items.Add("$(CBI) - Comment block indent");
            this.insertComboBox.Items.Add("- TEXT -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(SelText) - Selected text");
            this.insertComboBox.Items.Add("$(CurWord) - Word at cursor position");
            this.insertComboBox.Items.Add("$(CurSyntax) - Currently active syntax");
            this.insertComboBox.Items.Add("$(Clipboard) - Clipboard content");
            this.insertComboBox.Items.Add("- FILE -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(CurFile) - Current file");
            this.insertComboBox.Items.Add("$(CurFilename) - Current file's name");
            this.insertComboBox.Items.Add("$(CurDir) - Current file's directory");
            this.insertComboBox.Items.Add("- DIRECTORY -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(DesktopDir) - User's desktop directory");
            this.insertComboBox.Items.Add("$(SystemDir) - Windows system directory");
            this.insertComboBox.Items.Add("$(ProgramsDir) - Program Files directory");
            this.insertComboBox.Items.Add("$(PersonalDir) - User's personal files directory");
            this.insertComboBox.Items.Add("$(WorkingDir) - Current working directory");
            this.insertComboBox.Items.Add("$(AppDir) - FlashDevelop program directory");
            this.insertComboBox.Items.Add("$(BaseDir) - FlashDevelop files directory");
            this.insertComboBox.Items.Add("$(UserAppDir) - FlashDevelop user directory");
            this.insertComboBox.Items.Add("- TYPE -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(TypPkg) - File package");
            this.insertComboBox.Items.Add("$(TypName) - Current type name");
            this.insertComboBox.Items.Add("$(TypPkgName) - Current package + type name");
            this.insertComboBox.Items.Add("$(TypKind) - Type kind (interface, class)");
            this.insertComboBox.Items.Add("- MEMBER -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(MbrName) - Current member (declaration at line, ie. current method");
            this.insertComboBox.Items.Add("$(MbrKind) - Member kind (const, var, function)");
            this.insertComboBox.Items.Add("$(MbrTypPkg) - Member's type package");
            this.insertComboBox.Items.Add("$(MbrTypName) - Members's type name");
            this.insertComboBox.Items.Add("$(MbrTypePkgName) - Members's type qualified name (package + type name)");
            this.insertComboBox.Items.Add("$(MbrTypKind) - Members's type kind (interface, class)");
            this.insertComboBox.Items.Add("- ITEM -------------------------------------------------------------------------");
            this.insertComboBox.Items.Add("$(ItmTypName) - Current item name (object resolved at cursor position)");
            this.insertComboBox.Items.Add("$(ItmFile) - File where the item is declared");
            this.insertComboBox.Items.Add("$(ItmKind) - Item kind (const, var, function)");
            this.insertComboBox.Items.Add("$(ItmTypPkg) - Item's type package");
            this.insertComboBox.Items.Add("$(ItmTypName) - Item's type name");
            this.insertComboBox.Items.Add("$(ItmTypePkgName) - Item's type qualified name (package + type name)");
            this.insertComboBox.Items.Add("$(ItmTypKind) - Item's type kind (interface, class)");
            this.insertComboBox.SelectedIndex = 0;
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
                if (!data.StartsWith("-"))
                {
                    Int32 variableEnd = data.IndexOf(")") + 1;
                    String variable = data.Substring(0, variableEnd);
                    this.InsertText(this.contentsTextBox, variable);
                }
                this.insertComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Inserts text to the current location or selection
        /// </summary>
        private void InsertText(TextBox target, String text)
        {
            target.SelectedText = text;
            target.Focus();
        }

        #endregion

    }

}