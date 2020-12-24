using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore;
using Ookii.Dialogs;

namespace ProjectManager.Controls
{
    public class ClasspathControl : UserControl
    {
        string language;

        public event EventHandler Changed;

        #region Component Designer

        Button btnUp;
        Button btnDown;
        ListBox listBox;
        Button btnNewClasspath;
        Button btnRemove;
        ToolTip toolTip;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listBox = new ListBoxEx();
            btnNewClasspath = new ButtonEx();
            btnRemove = new ButtonEx();
            toolTip = new ToolTip(components);
            btnUp = new ButtonEx();
            btnDown = new ButtonEx();
            LanguageBox = new FlatCombo();
            SuspendLayout();
            // 
            // listBox
            // 
            listBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            listBox.Location = new Point(1, 1);
            listBox.Name = "listBox";
            listBox.Size = new Size(271, 133);
            listBox.TabIndex = 0;
            listBox.DoubleClick += listBox_DoubleClick;
            listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;
            listBox.MouseMove += listBox_MouseMove;
            listBox.IntegralHeight = false;
            // 
            // btnNewClasspath
            // 
            btnNewClasspath.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnNewClasspath.FlatStyle = FlatStyle.System;
            btnNewClasspath.Location = new Point(0, 147);
            btnNewClasspath.Name = "btnNewClasspath";
            btnNewClasspath.Size = new Size(107, 21);
            btnNewClasspath.TabIndex = 1;
            btnNewClasspath.Text = "&Add Classpath...";
            btnNewClasspath.Click += btnNewClasspath_Click;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemove.FlatStyle = FlatStyle.System;
            btnRemove.Location = new Point(111, 147);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(69, 21);
            btnRemove.TabIndex = 2;
            btnRemove.Text = "&Remove";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnUp
            // 
            btnUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUp.Location = new Point(278, 0);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(24, 24);
            btnUp.TabIndex = 3;
            btnUp.Click += btnUp_Click;
            // 
            // btnDown
            // 
            btnDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDown.Location = new Point(278, 24);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(24, 24);
            btnDown.TabIndex = 4;
            btnDown.Click += btnDown_Click;
            // 
            // langComboBox
            // 
            LanguageBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LanguageBox.DropDownStyle = ComboBoxStyle.DropDownList;
            LanguageBox.FlatStyle = FlatStyle.System;
            LanguageBox.FormattingEnabled = true;
            LanguageBox.Items.AddRange(new object[] {
            "AS2",
            "AS3",
            "Haxe"});
            LanguageBox.Location = new Point(187, 150);
            LanguageBox.Name = "LanguageBox";
            LanguageBox.Size = new Size(88, 21);
            LanguageBox.TabIndex = 5;
            // 
            // ClasspathControl
            // 
            Controls.Add(LanguageBox);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(btnRemove);
            Controls.Add(btnNewClasspath);
            Controls.Add(listBox);
            Name = "ClasspathControl";
            Size = new Size(302, 170);
            ResumeLayout(false);

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

        public Project Project { get; set; }

        public ComboBox LanguageBox { get; set; }

        public string Language
        {
            get => language;
            set 
            {
                if (value is null) return;
                for (int i = 0; i < LanguageBox.Items.Count; i++)
                    if (value.Equals(LanguageBox.Items[i] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        LanguageBox.SelectedIndex = i;
                        break;
                    }
                language = value; 
            }
        }

        public string[] Classpaths
        {
            get
            {
                var classpaths = new List<string>();
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

        void InitializeLocalization()
        {
            btnRemove.Text = TextHelper.GetString("Label.Remove");
            btnNewClasspath.Text = TextHelper.GetString("Label.AddClasspath");
        }

        void OnChanged() => Changed?.Invoke(this, new EventArgs());

        void SetButtons()
        {
            btnRemove.Enabled = (listBox.SelectedIndex > -1);
            btnUp.Enabled = (listBox.SelectedIndex > 0);
            btnDown.Enabled = (listBox.SelectedIndex < listBox.Items.Count -1);
        }

        string lastBrowserPath;

        void btnNewClasspath_Click(object sender, EventArgs e)
        {
            using var dialog = new VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                UseDescriptionForTitle = true,
                Description = TextHelper.GetString("Info.SelectClasspathDirectory")
            };
            if (Project != null) dialog.SelectedPath = Project.Directory;
            if (Directory.Exists(lastBrowserPath)) dialog.SelectedPath = lastBrowserPath;
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            var path = dialog.SelectedPath;
            if (Project != null)
            {
                if (CanBeRelative(path))
                {
                    path = Project.GetRelativePath(path);
                    // remove default classpath if you add a subfolder in the classpath
                    if (!path.StartsWithOrdinal("..") && listBox.Items.Count == 1 
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

        bool CanBeRelative(string path)
        {
            return Path.GetPathRoot(path).ToLower() == Path.GetPathRoot(Project.ProjectPath).ToLower();
        }

        void btnRemove_Click(object sender, EventArgs e)
        {
            listBox.Items.RemoveAt(listBox.SelectedIndex);
            OnChanged();
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e) => SetButtons();

        void listBox_DoubleClick(object sender, EventArgs e)
        {
            var entry = listBox.SelectedItem as ClasspathEntry;
            if (entry is null) return; // you could have double-clicked on whitespace
            using var dialog = new VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                UseDescriptionForTitle = true,
                Description = TextHelper.GetString("Info.SelectClasspathDirectory")
            };
            if (Project != null)
            {
                dialog.SelectedPath = Project.GetAbsolutePath(entry.Classpath);
                if (!Directory.Exists(dialog.SelectedPath)) dialog.SelectedPath = Project.Directory;
            }
            else dialog.SelectedPath = entry.Classpath;
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            var selectedPath = dialog.SelectedPath;
            if (Project != null)
            {
                if (CanBeRelative(selectedPath)) 
                    selectedPath = Project.GetRelativePath(selectedPath);
            }
            if (selectedPath == entry.Classpath) return; // nothing to do!
            listBox.Items[listBox.SelectedIndex] = new ClasspathEntry(selectedPath);
            OnChanged();
        }

        Point prevPoint = new Point(0, 0); // blocks too frequent updates

        void listBox_MouseMove(object sender, MouseEventArgs e)
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

        void btnUp_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object temp = listBox.Items[index-1];
            listBox.Items[index-1] = listBox.Items[index];
            listBox.Items[index] = temp;
            listBox.SelectedIndex = index-1;
            OnChanged();
        }

        void btnDown_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object temp = listBox.Items[index+1];
            listBox.Items[index+1] = listBox.Items[index];
            listBox.Items[index] = temp;
            listBox.SelectedIndex = index+1;
            OnChanged();
        }

        #region WarnConflictingPath

        bool WarnConflictingPath(string path)
        {
            var sep = Path.DirectorySeparatorChar;
            if (Project != null) path = Project.GetAbsolutePath(path);
            foreach (ClasspathEntry entry in listBox.Items)
            {
                string cp = entry.Classpath;
                if (Project != null) cp = Project.GetAbsolutePath(cp);
                if (path.StartsWithOrdinal(cp + sep) || cp.StartsWithOrdinal(path + sep))
                {
                    var info = TextHelper.GetString("Info.PathConflict");
                    var message = string.Format(info, cp);
                    var title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                    var result = MessageBox.Show(this, message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel) return false;
                }
            }
            return true;
        }

        #endregion

        #region ClasspathEntry

        class ClasspathEntry
        {
            public readonly string Classpath;

            public ClasspathEntry(string classpath) => Classpath = classpath;

            public override string ToString()
            {
                string projPath = TextHelper.GetString("Info.ProjectDirectory");
                return (Classpath == ".") ? projPath : Classpath;
            }

            public override bool Equals(object obj)
            {
                if (obj is ClasspathEntry entry) return entry.Classpath == Classpath;
                return base.Equals(obj);
            }

            public override int GetHashCode() => base.GetHashCode();
        }

        #endregion
    }
}