// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;

namespace CodeRefactor.Controls
{
    public partial class MoveDialog : SmartForm
    {
        IEnumerable<string> GetClasspaths(string path) => GetClasspaths(path, null);

        IEnumerable<string> GetClasspaths(string path, string projectDirName)
        {
            var directories = new List<string> {path};
            directories.AddRange(Directory.GetDirectories(path, "*", SearchOption.AllDirectories));
            directories.RemoveAll(it =>
            {
                foreach (var movingFile in MovingFiles)
                {
                    if (it.StartsWith(movingFile) || Path.GetDirectoryName(movingFile) == it) return true;
                }
                return false;
            });
            var result = directories.Select(it => GetClasspath(it, projectDirName));
            return result;
        }

        static string GetClasspath(string path, string projectDirName)
        {
            path = projectDirName is null ? path : path.Replace(projectDirName, string.Empty);
            return path.Trim(Path.DirectorySeparatorChar);
        }

        readonly List<string> projectClasspaths = new List<string>();
        readonly List<string> externalClasspaths = new List<string>();

        public MoveDialog(string file):this(new List<string> { file })
        {
        }

        public MoveDialog(List<string> files)
        {
            MovingFiles = files;
            InitializeComponent();
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "2823102d-d712-4ce6-aa36-58e0bb4bf61d";
            tree.ItemHeight = tree.Font.Height;
            InitializeClasspaths();
            InitializeInput();
            RefreshTree();
        }

        public List<string> MovingFiles { get; }

        public string SelectedDirectory
        {
            get
            {
                if (tree.SelectedItem is null) return null;
                var path = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                return Path.Combine(path, tree.SelectedItem.ToString());
            }
        }

        public bool FixPackages => fixPackages.Checked;

        void InitializeClasspaths()
        {
            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            if (context is null) return;
            var projectDir = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            foreach (PathModel classpath in context.Classpath)
            {
                if (classpath.IsVirtual) continue;
                string path = classpath.Path;
                string fullPath = path;
                if (!Path.IsPathRooted(path)) fullPath = Path.GetFullPath(path);
                if (fullPath.StartsWith(projectDir))
                    projectClasspaths.AddRange(GetClasspaths(path, projectDir));
                else externalClasspaths.AddRange(GetClasspaths(path));
            }
        }

        void InitializeInput()
        {
            if (MovingFiles.Count == 1)
            {
                input.KeyDown += OnInputKeyDown;
                input.TextChanged += OnInputTextChanged;
            }
            else input.Enabled = false;
        }

        void RefreshTree()
        {
            tree.BeginUpdate();
            tree.Items.Clear();
            FillTree();
            tree.EndUpdate();
        }

        void FillTree()
        {
            var classpaths = new List<string>(projectClasspaths);
            if (showExternalClasspaths.Checked) classpaths.AddRange(externalClasspaths);
            var search = input.Text.Trim();
            if (search.Length > 0)
            {
                var separator = Path.DirectorySeparatorChar;
                var searchWord = search.Replace('\\', separator).Replace('/', separator).Replace('.', separator);
                var searchLength = search.Length;
                classpaths = classpaths.FindAll(path =>
                {
                    int score = CompletionList.SmartMatch(path, searchWord, searchLength);
                    return score > 0 && score < 6;
                });
            }
            if (classpaths.Count > 0)
            {
                tree.Items.AddRange(classpaths.ToArray());
                tree.SelectedIndex = 0;
                processButton.Enabled = true;
            }
            else processButton.Enabled = false;
        }

        void OnShowExternalClasspathsCheckStateChanged(object sender, EventArgs e) => RefreshTree();

        void OnInputKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.E:
                case Keys.I:
                    e.Handled = e.Control;
                    break;
                case Keys.Down:
                    if (tree.SelectedIndex < tree.Items.Count - 1) tree.SelectedIndex++;
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (tree.SelectedIndex > 0) tree.SelectedIndex--;
                    e.Handled = true;
                    break;
            }
        }

        void OnInputTextChanged(object sender, EventArgs eventArgs) => RefreshTree();

        void OnTreeMouseDoubleClick(object sender, MouseEventArgs e)
        {
            int indexFromPoint = tree.IndexFromPoint(e.Location);
            if (indexFromPoint == -1) return;
            tree.SelectedItem = tree.Items[indexFromPoint];
            DialogResult = DialogResult.OK;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.E:
                    if (e.Control) showExternalClasspaths.Checked = !showExternalClasspaths.Checked;
                    break;
                case Keys.I:
                    if (e.Control) fixPackages.Checked = !fixPackages.Checked;
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }
    }
}