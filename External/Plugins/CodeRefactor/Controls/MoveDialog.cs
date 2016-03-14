using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;

namespace CodeRefactor.Controls
{
    public partial class MoveDialog : Form
    {
        static IEnumerable<string> GetClasspaths(string path)
        {
            return GetClasspaths(path, null);
        }
        static IEnumerable<string> GetClasspaths(string path, string projectDirName)
        {
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            List<string> result = new List<string> {GetClasspath(path, projectDirName)};
            result.AddRange(directories.Select(it => GetClasspath(it, projectDirName)));
            return result;
        }

        static string GetClasspath(string path, string projectDirName)
        {
            path = projectDirName == null ? path : path.Replace(projectDirName, string.Empty);
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
            tree.ItemHeight = tree.Font.Height;
            InitializeClasspaths();
            InitializeInput();
            RefreshTree();
        }

        public List<string> MovingFiles { get; private set; }

        public string SelectedDirectory
        {
            get
            {
                string projectDir = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                return Path.Combine(projectDir, tree.SelectedItem.ToString());
            }
        }

        public bool FixPackages { get { return fixPackages.Checked; }}

        void InitializeClasspaths()
        {
            IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            if (context == null) return;
            string projectDir = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
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
            List<string> classpaths = new List<string>(projectClasspaths);
            if (showExternalClasspaths.Checked) classpaths.AddRange(externalClasspaths);
            string search = input.Text.Trim();
            if (search.Length > 0)
            {
                char separator = Path.DirectorySeparatorChar;
                string searchWord = search.Replace('\\', separator).Replace('/', separator).Replace('.', separator);
                int searchLength = search.Length;
                classpaths = classpaths.FindAll(path =>
                {
                    int score = PluginCore.Controls.CompletionList.SmartMatch(path, searchWord, searchLength);
                    return score > 0 && score < 6;
                });
            }
            if (classpaths.Count > 0) tree.Items.AddRange(classpaths.ToArray());
            if (tree.Items.Count > 0) tree.SelectedIndex = 0;
        }

        bool GetCanProcess()
        {
            object selectedItem = tree.SelectedItem;
            if (selectedItem == null) return false;
            string selectedPath = selectedItem.ToString();
            if (!Path.IsPathRooted(selectedPath)) selectedPath = Path.GetFullPath(selectedPath);
            return MovingFiles.All(file => Path.GetDirectoryName(file) != selectedPath);
        }

        void OnShowExternalClasspathsCheckStateChanged(object sender, EventArgs e)
        {
            RefreshTree();
        }

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

        void OnInputTextChanged(object sender, EventArgs eventArgs)
        {
            RefreshTree();
        }

        void OnTreeMouseDoubleClick(object sender, MouseEventArgs e)
        {
            int indexFromPoint = tree.IndexFromPoint(e.Location);
            if (indexFromPoint == -1) return;
            tree.SelectedItem = tree.Items[indexFromPoint];
            if (GetCanProcess()) DialogResult = DialogResult.OK;
        }

        void OnTreeSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            processButton.Enabled = GetCanProcess();
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