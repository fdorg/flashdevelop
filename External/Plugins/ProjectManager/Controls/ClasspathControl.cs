using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using Ookii.Dialogs;

namespace ProjectManager.Controls
{
    public class ClasspathControl : UserControl
    {
        String language;
        Project project; // if not null, use relative paths

        public event EventHandler Changed;

        #region Component Designer

        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.ComboBox langComboBox;
        private System.Windows.Forms.Button btnNewClasspath;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ToolTip toolTip;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox = new System.Windows.Forms.ListBox();
            this.btnNewClasspath = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.langComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(275, 143);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseMove);
            // 
            // btnNewClasspath
            // 
            this.btnNewClasspath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewClasspath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNewClasspath.Location = new System.Drawing.Point(0, 149);
            this.btnNewClasspath.Name = "btnNewClasspath";
            this.btnNewClasspath.Size = new System.Drawing.Size(107, 21);
            this.btnNewClasspath.TabIndex = 1;
            this.btnNewClasspath.Text = "&Add Classpath...";
            this.btnNewClasspath.Click += new System.EventHandler(this.btnNewClasspath_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(111, 149);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(69, 21);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Location = new System.Drawing.Point(278, 0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(24, 24);
            this.btnUp.TabIndex = 3;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Location = new System.Drawing.Point(278, 24);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(24, 24);
            this.btnDown.TabIndex = 4;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // langComboBox
            // 
            this.langComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.langComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.langComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.langComboBox.FormattingEnabled = true;
            this.langComboBox.Items.AddRange(new object[] {
            "AS2",
            "AS3",
            "Haxe"});
            this.langComboBox.Location = new System.Drawing.Point(187, 150);
            this.langComboBox.Name = "langComboBox";
            this.langComboBox.Size = new System.Drawing.Size(88, 21);
            this.langComboBox.TabIndex = 5;
            // 
            // ClasspathControl
            // 
            this.Controls.Add(this.langComboBox);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnNewClasspath);
            this.Controls.Add(this.listBox);
            this.Name = "ClasspathControl";
            this.Size = new System.Drawing.Size(302, 170);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        public ClasspathControl()
        {
            InitializeComponent();
            InitializeLocalization();
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(Icons.DownArrow.Img);
            imageList.Images.Add(Icons.UpArrow.Img);
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            btnDown.ImageList = imageList;
            btnUp.ImageList = imageList;
            btnDown.ImageIndex = 0;
            btnUp.ImageIndex = 1;
            btnRemove.Enabled = false;
            SetButtons();
        }

        #region Public Properties

        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        public ComboBox LanguageBox
        {
            get { return this.langComboBox; }
            set { this.langComboBox = value; }
        }

        public string Language
        {
            get { return this.language; }
            set 
            {
                if (value == null) return;
                for(int i=0; i<langComboBox.Items.Count; i++)
                    if (value.Equals(langComboBox.Items[i] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        langComboBox.SelectedIndex = i;
                        break;
                    }
                this.language = value; 
            }
        }

        public string[] Classpaths
        {
            get
            {
                List<string> classpaths = new List<string>();
                foreach (ClasspathEntry entry in listBox.Items) classpaths.Add(entry.Classpath);
                return classpaths.ToArray();
            }
            set
            {
                listBox.Items.Clear();
                foreach (string cp in value) listBox.Items.Add(new ClasspathEntry(cp));
            }
        }

        #endregion

        private void InitializeLocalization()
        {
            this.btnRemove.Text = TextHelper.GetString("Label.Remove");
            this.btnNewClasspath.Text = TextHelper.GetString("Label.AddClasspath");
        }

        private void OnChanged()
        {
            if (Changed != null) Changed(this, new EventArgs());
        }

        private void SetButtons()
        {
            btnRemove.Enabled = (listBox.SelectedIndex > -1);
            btnUp.Enabled = (listBox.SelectedIndex > 0);
            btnDown.Enabled = (listBox.SelectedIndex < listBox.Items.Count -1);
        }

        string lastBrowserPath;

        private void btnNewClasspath_Click(object sender, EventArgs e)
        {
            using (VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.UseDescriptionForTitle = true;
                dialog.Description = TextHelper.GetString("Info.SelectClasspathDirectory");

                if (project != null) dialog.SelectedPath = project.Directory;
                if (lastBrowserPath != null && Directory.Exists(lastBrowserPath)) dialog.SelectedPath = lastBrowserPath;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    if (project != null)
                    {
                        if (CanBeRelative(path))
                        {
                            path = project.GetRelativePath(path);
                            // remove default classpath if you add a subfolder in the classpath
                            if (!path.StartsWith("..") && listBox.Items.Count == 1 
                                && (listBox.Items[0] as ClasspathEntry).Classpath == ".")
                                listBox.Items.Clear();
                        }
                    }
                    if (listBox.Items.Count > 0 && !WarnConflictingPath(path)) return;
                    ClasspathEntry entry = new ClasspathEntry(path);
                    if (!listBox.Items.Contains(entry)) listBox.Items.Add(entry);
                    OnChanged();
                    lastBrowserPath = dialog.SelectedPath;
                }
            }
        }

        private bool CanBeRelative(string path)
        {
            if (Path.GetPathRoot(path).ToLower() != Path.GetPathRoot(project.ProjectPath).ToLower())
            {
                return false;
            }
            return true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            listBox.Items.RemoveAt(listBox.SelectedIndex);
            OnChanged();
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void listBox_DoubleClick(object sender, System.EventArgs e)
        {
            ClasspathEntry entry = listBox.SelectedItem as ClasspathEntry;
            if (entry == null) return; // you could have double-clicked on whitespace
            using (VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.UseDescriptionForTitle = true;
                dialog.Description = TextHelper.GetString("Info.SelectClasspathDirectory");
                if (project != null)
                {
                    dialog.SelectedPath = project.GetAbsolutePath(entry.Classpath);
                    if (!Directory.Exists(dialog.SelectedPath)) dialog.SelectedPath = project.Directory;
                }
                else dialog.SelectedPath = entry.Classpath;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    if (project != null)
                    {
                        if (CanBeRelative(selectedPath)) 
                            selectedPath = project.GetRelativePath(selectedPath);
                    }
                    if (selectedPath == entry.Classpath) return; // nothing to do!
                    listBox.Items[listBox.SelectedIndex] = new ClasspathEntry(selectedPath);
                    OnChanged();
                }
            }
        }

        private Point prevPoint = new Point(0, 0); // blocks too frequent updates
        private void listBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (prevPoint.Equals(new Point(e.X, e.Y))) return;
            prevPoint = new Point(e.X, e.Y);
            int selectedIndex = listBox.IndexFromPoint(e.X, e.Y);
            if (selectedIndex > -1)
            {
                string path = listBox.Items[selectedIndex].ToString();
                Graphics g = listBox.CreateGraphics();
                if (g.MeasureString(path, listBox.Font).Width > listBox.ClientRectangle.Width)
                {
                    toolTip.SetToolTip(listBox, path);
                    return;
                }
            }
            toolTip.SetToolTip(listBox, "");
        }

        private void btnUp_Click(object sender, System.EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object temp = listBox.Items[index-1];
            listBox.Items[index-1] = listBox.Items[index];
            listBox.Items[index] = temp;
            listBox.SelectedIndex = index-1;
            OnChanged();
        }

        private void btnDown_Click(object sender, System.EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object temp = listBox.Items[index+1];
            listBox.Items[index+1] = listBox.Items[index];
            listBox.Items[index] = temp;
            listBox.SelectedIndex = index+1;
            OnChanged();
        }

        #region WarnConflictingPath

        private bool WarnConflictingPath(string path)
        {
            char sep = Path.DirectorySeparatorChar;

            if (project != null)
                path = project.GetAbsolutePath(path);

            foreach (ClasspathEntry entry in listBox.Items)
            {
                string cp = entry.Classpath;

                if (project != null) cp = project.GetAbsolutePath(cp);

                if (path.StartsWith(cp + sep) || cp.StartsWith(path + sep))
                {
                    string info = TextHelper.GetString("Info.PathConflict");
                    string message = string.Format(info, cp);

                    string title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                    DialogResult result = MessageBox.Show(this, message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Cancel) return false;
                }
            }
            return true;
        }

        #endregion

        #region ClasspathEntry

        private class ClasspathEntry
        {
            public string Classpath;

            public ClasspathEntry(string classpath)
            {
                this.Classpath = classpath;
            }

            public override string ToString()
            {
                String projPath = TextHelper.GetString("Info.ProjectDirectory");
                return (Classpath == ".") ? projPath : Classpath;
            }

            public override bool Equals(object obj)
            {
                ClasspathEntry entry = obj as ClasspathEntry;
                if (entry != null) return entry.Classpath == Classpath;
                else return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion

    }
}
