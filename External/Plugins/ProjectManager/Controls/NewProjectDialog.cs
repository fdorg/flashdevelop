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
        readonly string defaultProjectImage;

        #region Windows Form Designer

        Button cancelButton;
        Button okButton;
        Label label1;
        ImageList imageList;
        ColumnHeader columnHeader1;
        PictureBox previewBox;
        ListView projectListView;
        Label descriptionLabel;
        Button browseButton;
        TextBox locationTextBox;
        Label label2;
        TextBox nameTextBox;
        CheckBox createDirectoryBox;
        StatusBar statusBar;
        Label label3;
        TextBox packageTextBox;
        System.ComponentModel.IContainer components;
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cancelButton = new ButtonEx();
            okButton = new ButtonEx();
            previewBox = new PictureBoxEx();
            projectListView = new ListViewEx();
            columnHeader1 = new ColumnHeader();
            imageList = new ImageList(components);
            locationTextBox = new TextBoxEx();
            label1 = new Label();
            descriptionLabel = new Label();
            browseButton = new ButtonEx();
            label2 = new Label();
            nameTextBox = new TextBoxEx();
            createDirectoryBox = new CheckBoxEx();
            statusBar = new StatusBarEx();
            label3 = new Label();
            packageTextBox = new TextBoxEx();
            ((System.ComponentModel.ISupportInitialize)(previewBox)).BeginInit();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new Point(573, 388);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(90, 23);
            cancelButton.TabIndex = 11;
            cancelButton.Text = "&Cancel";
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.Enabled = false;
            okButton.FlatStyle = FlatStyle.System;
            okButton.Location = new Point(475, 388);
            okButton.Name = "okButton";
            okButton.Size = new Size(90, 23);
            okButton.TabIndex = 10;
            okButton.Text = "&OK";
            okButton.Click += okButton_Click;
            // 
            // previewBox
            // 
            previewBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Right;
            previewBox.BackColor = Color.White;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.Location = new Point(469, 12);
            previewBox.Name = "previewBox";
            previewBox.Size = new Size(192, 246);
            previewBox.TabIndex = 5;
            previewBox.TabStop = false;
            // 
            // projectListView
            // 
            projectListView.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            projectListView.BackColor = SystemColors.Window;
            projectListView.BorderStyle = BorderStyle.FixedSingle;
            projectListView.Columns.AddRange(new[] {columnHeader1});
            projectListView.HeaderStyle = ColumnHeaderStyle.None;
            projectListView.HideSelection = false;
            projectListView.LargeImageList = imageList;
            projectListView.Location = new Point(11, 12);
            projectListView.MultiSelect = false;
            projectListView.Name = "projectListView";
            projectListView.Size = new Size(459, 246);
            projectListView.SmallImageList = imageList;
            projectListView.TabIndex = 0;
            projectListView.TileSize = new Size(170, 22);
            projectListView.UseCompatibleStateImageBehavior = false;
            projectListView.View = View.Tile;
            projectListView.SelectedIndexChanged += projectListView_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Width = 183;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.TransparentColor = Color.Transparent;
            // 
            // locationTextBox
            // 
            locationTextBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            locationTextBox.Location = new Point(77, 328);
            locationTextBox.Name = "locationTextBox";
            locationTextBox.Size = new Size(485, 21);
            locationTextBox.TabIndex = 5;
            locationTextBox.Text = "C:\\Documents and Settings\\Nick\\My Documents";
            locationTextBox.TextChanged += locationTextBox_TextChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.FlatStyle = FlatStyle.System;
            label1.Location = new Point(13, 330);
            label1.Name = "label1";
            label1.Size = new Size(68, 16);
            label1.TabIndex = 4;
            label1.Text = "&Location:";
            label1.AutoSize = true;
            // 
            // descriptionLabel
            // 
            descriptionLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            descriptionLabel.BorderStyle = BorderStyle.Fixed3D;
            descriptionLabel.Location = new Point(11, 265);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(650, 21);
            descriptionLabel.TabIndex = 1;
            descriptionLabel.Text = "Project description";
            descriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            browseButton.FlatStyle = FlatStyle.System;
            browseButton.Location = new Point(573, 326);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(90, 23);
            browseButton.TabIndex = 6;
            browseButton.Text = "&Browse...";
            browseButton.Click += browseButton_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.FlatStyle = FlatStyle.System;
            label2.Location = new Point(13, 299);
            label2.Name = "label2";
            label2.Size = new Size(68, 16);
            label2.TabIndex = 2;
            label2.Text = "&Name:";
            label2.AutoSize = true;
            // 
            // nameTextBox
            // 
            nameTextBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            nameTextBox.Location = new Point(77, 297);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(585, 21);
            nameTextBox.TabIndex = 3;
            nameTextBox.Text = "New Project";
            nameTextBox.TextChanged += nameTextBox_TextChanged;
            // 
            // createDirectoryBox
            // 
            createDirectoryBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            createDirectoryBox.FlatStyle = FlatStyle.System;
            createDirectoryBox.Location = new Point(78, 389);
            createDirectoryBox.Name = "createDirectoryBox";
            createDirectoryBox.Size = new Size(249, 16);
            createDirectoryBox.TabIndex = 9;
            createDirectoryBox.Text = " Create &directory for project";
            createDirectoryBox.CheckedChanged += createDirectoryBox_CheckedChanged;
            // 
            // statusBar
            // 
            statusBar.Location = new Point(0, 423);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(673, 21);
            statusBar.TabIndex = 9;
            statusBar.Text = "  Will create:  C:\\Documents and Settings\\Nick\\My Documents\\New Project.fdp";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.Location = new Point(10, 361);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 7;
            label3.Text = "&Package:";
            label3.AutoSize = true;
            // 
            // packageTextBox
            // 
            packageTextBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            packageTextBox.Location = new Point(77, 359);
            packageTextBox.Name = "packageTextBox";
            packageTextBox.Size = new Size(585, 21);
            packageTextBox.TabIndex = 8;
            packageTextBox.TextChanged += textPackage_TextChanged;
            // 
            // NewProjectDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(673, 444);
            Controls.Add(projectListView);
            Controls.Add(packageTextBox);
            Controls.Add(label3);
            Controls.Add(previewBox);
            Controls.Add(statusBar);
            Controls.Add(nameTextBox);
            Controls.Add(locationTextBox);
            Controls.Add(label2);
            Controls.Add(browseButton);
            Controls.Add(descriptionLabel);
            Controls.Add(label1);
            Controls.Add(createDirectoryBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewProjectDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "New Project";
            ((System.ComponentModel.ISupportInitialize)(previewBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        public NewProjectDialog()
        {
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "128470dc-9372-46cd-ad32-e5ca27e3c366";
            InitializeComponent();
            InitializeLocalization();

            imageList.Images.Add(Icons.Project.Img);
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            defaultProjectImage = Path.Combine(ProjectPaths.ProjectTemplatesDirectory, "Default.png");

            projectListView.Items.Clear();
            projectListView.TileSize = ScaleHelper.Scale(new Size(170, 22));
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
            List<string> templateDirs = ProjectPaths.GetAllProjectDirs();
            templateDirs.Sort(CompareFolderNames);
            ListViewItem lastItem = null;
            string lastTemplate = null;

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
                    if (group is null || group.Header != parts[0])
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
            Load += NewProjectDialog_Load;
        }

        static int CompareFolderNames(string pathA, string pathB) => Path.GetFileName(pathA).CompareTo(Path.GetFileName(pathB));

        void NewProjectDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lastTemplate))
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

            var locationDir = PluginMain.Settings.NewProjectDefaultDirectory;
            locationTextBox.Text = Directory.Exists(locationDir)
                ? locationDir
                : ProjectPaths.DefaultProjectsDirectory;
            locationTextBox.SelectionStart = locationTextBox.Text.Length;
        }

        #region Public Properties

        public string ProjectName
        {
            get => nameTextBox.Text;
            set => nameTextBox.Text = value;
        }

        public string PackageName
        {
            get => packageTextBox.Text;
            set => packageTextBox.Text = value;
        }

        public string ProjectExt
        {
            get
            {
                if (TemplateDirectory == null) return null;
                var templateFile = ProjectCreator.FindProjectTemplate(TemplateDirectory);
                return Path.GetExtension(templateFile);
            }
        }

        public string ProjectLocation
        {
            get => createDirectoryBox.Checked ? Path.Combine(locationTextBox.Text,ProjectName) : locationTextBox.Text;
            set => locationTextBox.Text = value;
        }

        public string TemplateDirectory => projectListView.SelectedItems.Count > 0 ? projectListView.SelectedItems[0].Tag as string : null;

        public string TemplateName => projectListView.SelectedItems.Count > 0 ? projectListView.SelectedItems[0].Text : null;

        #endregion

        void InitializeLocalization()
        {
            okButton.Text = TextHelper.GetString("Label.OK");
            label2.Text = TextHelper.GetString("Label.Name");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            browseButton.Text = TextHelper.GetString("Label.Browse");
            createDirectoryBox.Text = TextHelper.GetString("Info.CreateDirForProject");
            nameTextBox.Text = TextHelper.GetString("Info.NewProject");
            label1.Text = TextHelper.GetString("Label.Location");
            label3.Text = TextHelper.GetString("Label.Package");
            Text = " " + TextHelper.GetString("Info.NewProject");
        }

        void okButton_Click(object sender, EventArgs e)
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
                var msg = TextHelper.GetString("Info.ProjectFileAlreadyExists");
                var title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                var result = MessageBox.Show(this, msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK) return;
            }
            PluginMain.Settings.CreateProjectDirectory = createDirectoryBox.Checked;
            PluginMain.Settings.NewProjectDefaultDirectory = createDirectoryBox.Checked
                ? locationTextBox.Text
                : Path.GetDirectoryName(locationTextBox.Text);
            DialogResult = DialogResult.OK;
            Close();
        }

        void projectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (projectListView.SelectedIndices.Count > 0)
            {
                lastTemplate = TemplateDirectory;

                previewBox.Image?.Dispose();

                string projectImage = Path.Combine(TemplateDirectory,"Project.png");
                if (File.Exists(projectImage)) SetProjectImage(projectImage);
                else if (File.Exists(defaultProjectImage)) SetProjectImage(defaultProjectImage);
                else previewBox.Image = null;

                string projectDescription = Path.Combine(TemplateDirectory,"Project.txt");
                if (File.Exists(projectDescription))
                {
                    using StreamReader reader = File.OpenText(projectDescription);
                    descriptionLabel.Text = reader.ReadToEnd();
                }
                else descriptionLabel.Text = "";
                okButton.Enabled = true;
            }
            else okButton.Enabled = false;
            UpdateStatusBar();
        }

        void SetProjectImage(string projectImage)
        {
            using var image = Image.FromFile(projectImage);
            var empty = new Bitmap(previewBox.Width, previewBox.Height);
            using var graphics = Graphics.FromImage(empty);
            graphics.DrawImage(image, new Rectangle(empty.Width / 2 - image.Width / 2, empty.Height / 2 - image.Height / 2, image.Width, image.Height));
            previewBox.Image = empty;
        }

        void browseButton_Click(object sender, EventArgs e)
        {
            using var dialog = new VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                UseDescriptionForTitle = true,
                Description = TextHelper.GetString("Info.SelectProjectDirectory")
            };

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
                if (!createDirectoryBox.Checked
                    && nameTextBox.Text == TextHelper.GetString("Info.NewProject"))
                {
                    string name = Path.GetFileName(dialog.SelectedPath);
                    if (name.Length > 5)
                    {
                        nameTextBox.Text = name.Substring(0, 1).ToUpper() + name.Substring(1);
                    }
                }
            }
        }

        void UpdateStatusBar()
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

        void locationTextBox_TextChanged(object sender, EventArgs e) => UpdateStatusBar();
        
        void nameTextBox_TextChanged(object sender, EventArgs e) => UpdateStatusBar();
        
        void createDirectoryBox_CheckedChanged(object sender, EventArgs e) => UpdateStatusBar();

        void textPackage_TextChanged(object sender, EventArgs e)
        {
            //package name invalid
            if (!Regex.IsMatch(PackageName, "^[_a-zA-Z]([_a-zA-Z0-9])*([\\.][_a-zA-Z]([_a-zA-Z0-9])*)*$") && packageTextBox.Text.Length > 0)
            {
                okButton.Enabled = false;
                packageTextBox.BackColor = Color.Salmon;
            }
            else
            {
                okButton.Enabled = true;
                packageTextBox.BackColor = SystemColors.Window;
            }
        }

        /// <summary>
        /// Make sure previewBox background remains white
        /// </summary>
        public void AfterTheming() => previewBox.BackColor = Color.White;
    }
}