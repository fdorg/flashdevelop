using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;
using ProjectManager.Helpers;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore;
using System.Collections.Generic;
using PluginCore.Controls;
using Ookii.Dialogs;

namespace ProjectManager.Controls
{
    public class NewProjectDialog : SmartForm, IThemeHandler
    {
        static string lastTemplate;
        string defaultProjectImage;

        #region Windows Form Designer

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.ListView projectListView;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.CheckBox createDirectoryBox;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox packageTextBox;
        private System.ComponentModel.IContainer components;
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.okButton = new System.Windows.Forms.ButtonEx();
            this.previewBox = new System.Windows.Forms.PictureBoxEx();
            this.projectListView = new System.Windows.Forms.ListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.locationTextBox = new System.Windows.Forms.TextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.ButtonEx();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBoxEx();
            this.createDirectoryBox = new System.Windows.Forms.CheckBoxEx();
            this.statusBar = new System.Windows.Forms.StatusBarEx();
            this.label3 = new System.Windows.Forms.Label();
            this.packageTextBox = new System.Windows.Forms.TextBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(573, 388);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "&Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Enabled = false;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(475, 388);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(90, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // previewBox
            // 
            this.previewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
            this.previewBox.BackColor = System.Drawing.Color.White;
            this.previewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewBox.Location = new System.Drawing.Point(469, 12);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(192, 246);
            this.previewBox.TabIndex = 5;
            this.previewBox.TabStop = false;
            // 
            // projectListView
            // 
            this.projectListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.projectListView.BackColor = System.Drawing.SystemColors.Window;
            this.projectListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.columnHeader1});
            this.projectListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.projectListView.HideSelection = false;
            this.projectListView.LargeImageList = this.imageList;
            this.projectListView.Location = new System.Drawing.Point(11, 12);
            this.projectListView.MultiSelect = false;
            this.projectListView.Name = "projectListView";
            this.projectListView.Size = new System.Drawing.Size(459, 246);
            this.projectListView.SmallImageList = this.imageList;
            this.projectListView.TabIndex = 0;
            this.projectListView.TileSize = new System.Drawing.Size(170, 22);
            this.projectListView.UseCompatibleStateImageBehavior = false;
            this.projectListView.View = System.Windows.Forms.View.Tile;
            this.projectListView.SelectedIndexChanged += new System.EventHandler(this.projectListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 183;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // locationTextBox
            // 
            this.locationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.locationTextBox.Location = new System.Drawing.Point(77, 328);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(485, 21);
            this.locationTextBox.TabIndex = 5;
            this.locationTextBox.Text = "C:\\Documents and Settings\\Nick\\My Documents";
            this.locationTextBox.TextChanged += new System.EventHandler(this.locationTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(13, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "&Location:";
            this.label1.AutoSize = true;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.descriptionLabel.Location = new System.Drawing.Point(11, 265);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(650, 21);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Project description";
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseButton.Location = new System.Drawing.Point(573, 326);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(90, 23);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "&Browse...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(13, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Name:";
            this.label2.AutoSize = true;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(77, 297);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(585, 21);
            this.nameTextBox.TabIndex = 3;
            this.nameTextBox.Text = "New Project";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // createDirectoryBox
            // 
            this.createDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.createDirectoryBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.createDirectoryBox.Location = new System.Drawing.Point(78, 389);
            this.createDirectoryBox.Name = "createDirectoryBox";
            this.createDirectoryBox.Size = new System.Drawing.Size(249, 16);
            this.createDirectoryBox.TabIndex = 9;
            this.createDirectoryBox.Text = " Create &directory for project";
            this.createDirectoryBox.CheckedChanged += new System.EventHandler(this.createDirectoryBox_CheckedChanged);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 423);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(673, 21);
            this.statusBar.TabIndex = 9;
            this.statusBar.Text = "  Will create:  C:\\Documents and Settings\\Nick\\My Documents\\New Project.fdp";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(10, 361);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "&Package:";
            this.label3.AutoSize = true;
            // 
            // packageTextBox
            // 
            this.packageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.packageTextBox.Location = new System.Drawing.Point(77, 359);
            this.packageTextBox.Name = "packageTextBox";
            this.packageTextBox.Size = new System.Drawing.Size(585, 21);
            this.packageTextBox.TabIndex = 8;
            this.packageTextBox.TextChanged += new System.EventHandler(this.textPackage_TextChanged);
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(673, 444);
            this.Controls.Add(this.projectListView);
            this.Controls.Add(this.packageTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.locationTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.createDirectoryBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        public NewProjectDialog()
        {
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "128470dc-9372-46cd-ad32-e5ca27e3c366";
            this.InitializeComponent();
            this.InitializeLocalization();

            imageList.Images.Add(Icons.Project.Img);
            imageList.ImageSize = PluginCore.Helpers.ScaleHelper.Scale(new Size(16, 16));
            defaultProjectImage = Path.Combine(ProjectPaths.ProjectTemplatesDirectory, "Default.png");

            projectListView.Items.Clear();
            projectListView.TileSize = PluginCore.Helpers.ScaleHelper.Scale(new Size(170, 22));
            projectListView.ShowGroups = PluginBase.Settings.UseListViewGrouping;

            if (PlatformHelper.isRunningOnWine())
            {
                projectListView.View = View.SmallIcon;
                projectListView.GridLines = !projectListView.ShowGroups;
                columnHeader1.Width = -2;
            }

            if (!Directory.Exists(ProjectPaths.ProjectTemplatesDirectory))
            {
                string info = TextHelper.GetString("Info.TemplateDirNotFound");
                ErrorManager.ShowWarning(info, null);
                return;
            }

            ListViewGroup group = null;
            List<String> templateDirs = ProjectPaths.GetAllProjectDirs();
            templateDirs.Sort(CompareFolderNames);
            ListViewItem lastItem = null;
            String lastTemplate = null;

            foreach (string templateDir in templateDirs)
            {
                // skip hidden folders (read: version control)
                if ((File.GetAttributes(templateDir) & FileAttributes.Hidden) != 0) continue;

                string templateName = Path.GetFileName(templateDir).Substring(3);
                if (!templateName.Contains('-')) templateName = "-" + templateName;
                string[] parts = templateName.Split('-');

                ListViewItem item = new ListViewItem(" " + parts[1].Trim());
                item.ImageIndex = 0;
                item.Tag = templateDir;

                if (parts[0].Length > 0)
                {
                    if (group == null || group.Header != parts[0])
                    {
                        group = new ListViewGroup(parts[0]);
                        projectListView.Groups.Add(group);
                    }
                    item.Group = group;
                }

                if (lastItem != null && lastTemplate == templateName) // remove duplicates (keep last)
                    projectListView.Items.Remove(lastItem);
                lastItem = item;
                lastTemplate = templateName;
                projectListView.Items.Add(item);
            }
            this.Load += new EventHandler(NewProjectDialog_Load);
        }

        int CompareFolderNames(string pathA, string pathB)
        {
            return Path.GetFileName(pathA).CompareTo(Path.GetFileName(pathB));
        }

        void NewProjectDialog_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(lastTemplate))
            {
                foreach (ListViewItem item in projectListView.Items)
                {
                    if ((string)item.Tag == lastTemplate)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            else if (projectListView.Items.Count > 0) projectListView.Items[0].Selected = true;
            else
            {
                string info = TextHelper.GetString("Info.NoTemplatesFound");
                ErrorManager.ShowWarning(info, null);
            }
            nameTextBox.Text = TextHelper.GetString("Info.NewProject");
            createDirectoryBox.Checked = PluginMain.Settings.CreateProjectDirectory;

            string locationDir = PluginMain.Settings.NewProjectDefaultDirectory;
            if (!string.IsNullOrEmpty(locationDir) && Directory.Exists(locationDir))
                locationTextBox.Text = locationDir;
            else locationTextBox.Text = ProjectPaths.DefaultProjectsDirectory;
            locationTextBox.SelectionStart = locationTextBox.Text.Length;
        }

        #region Public Properties

        public string ProjectName
        {
            get { return nameTextBox.Text; }
            set { nameTextBox.Text = value; }
        }

        public string PackageName
        {
            get { return packageTextBox.Text; }

            set { packageTextBox.Text = value; }
        }

        public string ProjectExt
        {
            get
            {
                if (TemplateDirectory != null)
                {
                    string templateFile = ProjectCreator.FindProjectTemplate(TemplateDirectory);
                    if (templateFile != null)
                        return Path.GetExtension(templateFile);
                }
                return null;
            }
        }

        public string ProjectLocation
        {
            get
            {
                if (createDirectoryBox.Checked)
                    return Path.Combine(locationTextBox.Text,ProjectName);
                else
                    return locationTextBox.Text;
            }
            set { locationTextBox.Text = value; }
        }

        public string TemplateDirectory
        {
            get
            {
                if (projectListView.SelectedItems.Count > 0)
                    return projectListView.SelectedItems[0].Tag as string;
                else
                    return null;
            }
        }

        public string TemplateName
        {
            get
            {
                if (projectListView.SelectedItems.Count > 0)
                    return projectListView.SelectedItems[0].Text;
                else
                    return null;
            }
        }

        #endregion

        private void InitializeLocalization()
        {
            this.okButton.Text = TextHelper.GetString("Label.OK");
            this.label2.Text = TextHelper.GetString("Label.Name");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.browseButton.Text = TextHelper.GetString("Label.Browse");
            this.createDirectoryBox.Text = TextHelper.GetString("Info.CreateDirForProject");
            //this.groupBox2.Text = TextHelper.GetString("Label.InstalledTemplates");
            this.nameTextBox.Text = TextHelper.GetString("Info.NewProject");
            this.label1.Text = TextHelper.GetString("Label.Location");
            this.label3.Text = TextHelper.GetString("Label.Package");
            //this.label4.Text = TextHelper.GetString("Info.AboutPackages");
            this.Text = " " + TextHelper.GetString("Info.NewProject");
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            // we want to create a project directory with the same name as the
            // project file, underneath the selected location.
            string projectName = Path.GetFileNameWithoutExtension(ProjectName);
            string projectPath = Path.Combine(ProjectLocation,projectName+ProjectExt);

            // does this directory exist and is NOT empty?
            if (Directory.Exists(ProjectLocation) && Directory.GetFileSystemEntries(ProjectLocation).Length > 0)
            {
                string empty = TextHelper.GetString("Info.EmptyProject");
                if (TemplateName == empty && !createDirectoryBox.Checked)
                {} // don't show the dialog in this case
                else
                {
                    string msg = TextHelper.GetString("Info.TargetDirNotEmpty");
                    string title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                    DialogResult result = MessageBox.Show(this, msg, title, MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
                    if (result != DialogResult.OK) return;
                }
            }

            // does this project file already exist?
            if (File.Exists(projectPath))
            {
                string msg = TextHelper.GetString("Info.ProjectFileAlreadyExists");
                string title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                DialogResult result = MessageBox.Show(this, msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK) return;
            }

            PluginMain.Settings.CreateProjectDirectory = createDirectoryBox.Checked;
            if (createDirectoryBox.Checked) PluginMain.Settings.NewProjectDefaultDirectory = locationTextBox.Text;
            else PluginMain.Settings.NewProjectDefaultDirectory = Path.GetDirectoryName(locationTextBox.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void projectListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (projectListView.SelectedIndices.Count > 0)
            {
                lastTemplate = TemplateDirectory;

                if (previewBox.Image != null) previewBox.Image.Dispose();

                string projectImage = Path.Combine(TemplateDirectory,"Project.png");
                if (File.Exists(projectImage)) SetProjectImage(projectImage);
                else if (File.Exists(defaultProjectImage)) SetProjectImage(defaultProjectImage);
                else previewBox.Image = null;

                string projectDescription = Path.Combine(TemplateDirectory,"Project.txt");
                if (File.Exists(projectDescription))
                {
                    using (StreamReader reader = File.OpenText(projectDescription))
                    {
                        descriptionLabel.Text = reader.ReadToEnd();
                    }
                }
                else descriptionLabel.Text = "";
                okButton.Enabled = true;
            }
            else okButton.Enabled = false;
            UpdateStatusBar();
        }

        private void SetProjectImage(String projectImage)
        {
            Image image = Image.FromFile(projectImage);
            Bitmap empty = new Bitmap(this.previewBox.Width, this.previewBox.Height);
            Graphics graphics = Graphics.FromImage(empty);
            graphics.DrawImage(image, new Rectangle(empty.Width / 2 - image.Width / 2, empty.Height / 2 - image.Height / 2, image.Width, image.Height));
            previewBox.Image = empty;
            graphics.Dispose();
            image.Dispose();
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            using (VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.UseDescriptionForTitle = true;
                dialog.Description = TextHelper.GetString("Info.SelectProjectDirectory");

                string selectedPath = locationTextBox.Text;
                // try to get as close as we can to the directory you typed in
                try
                {
                    while (!Directory.Exists(selectedPath))
                    {
                        selectedPath = Path.GetDirectoryName(selectedPath);
                    }
                }
                catch
                {
                    selectedPath = ProjectPaths.DefaultProjectsDirectory;
                }
                dialog.SelectedPath = selectedPath;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    locationTextBox.Text = dialog.SelectedPath;
                    locationTextBox.SelectionStart = locationTextBox.Text.Length;

                    // smart project naming
                    if (!this.createDirectoryBox.Checked
                        && this.nameTextBox.Text == TextHelper.GetString("Info.NewProject"))
                    {
                        string name = Path.GetFileName(dialog.SelectedPath);
                        if (name.Length > 5)
                        {
                            this.nameTextBox.Text = name.Substring(0, 1).ToUpper() + name.Substring(1);
                        }
                    }
                }
            }
        }

        private void UpdateStatusBar()
        {
            string status = string.Empty;
            string ext = ProjectExt;
            if (ext != null)
            {
                char separator = Path.DirectorySeparatorChar;
                string name = nameTextBox.Text;
                status = "  " + TextHelper.GetString("Info.WillCreate") + " ";
                status += locationTextBox.Text.TrimEnd('\\', '/') + separator + name;
                if (createDirectoryBox.Checked) status += separator + name;
                status += ext;
                status = status.Replace('\\', separator).Replace('/', separator);
            }
            statusBar.Text = status;
        }

        private void locationTextBox_TextChanged(object sender, System.EventArgs e) { UpdateStatusBar(); }
        private void nameTextBox_TextChanged(object sender, System.EventArgs e) { UpdateStatusBar(); }
        private void createDirectoryBox_CheckedChanged(object sender, System.EventArgs e) { UpdateStatusBar(); }

        private void textPackage_TextChanged(object sender, EventArgs e)
        {
            //package name invalid
            if (!Regex.IsMatch(PackageName, "^[_a-zA-Z]([_a-zA-Z0-9])*([\\.][_a-zA-Z]([_a-zA-Z0-9])*)*$") && packageTextBox.Text.Length > 0)
            {
                okButton.Enabled = false;
                packageTextBox.BackColor = System.Drawing.Color.Salmon;
            }
            else
            {
                okButton.Enabled = true;
                packageTextBox.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        /// <summary>
        /// Make sure previewBox background remains white
        /// </summary>
        public void AfterTheming()
        {
            this.previewBox.BackColor = Color.White;
        }
    }

}
