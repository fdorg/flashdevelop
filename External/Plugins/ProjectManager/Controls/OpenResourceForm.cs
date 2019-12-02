// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;
using System.Linq;

namespace ProjectManager.Controls
{
    public class OpenResourceForm : SmartForm
    {
        readonly PluginMain plugin;
        readonly int MAX_ITEMS = 100;
        List<string> openedFiles;
        List<string> projectFiles;
        const string ITEM_SPACER = "-----------------";
        TextBox textBox;
        ListBox listBox;
        CheckBox cbInClasspathsOnly;
        CheckBox checkBox;
        Button refreshButton;
        static string previousSearch;

        public OpenResourceForm(PluginMain plugin)
        {
            this.plugin = plugin;
            InitializeComponent();
            InitializeGraphics();
            InitializeLocalization();
            Font = PluginBase.Settings.DefaultFont;
            listBox.ItemHeight = listBox.Font.Height;
            FormGuid = "8e4e0a95-0aff-422c-b8f5-ad9bc8affabb";
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            textBox = new TextBoxEx();
            listBox = new ListBoxEx();
            cbInClasspathsOnly = new CheckBoxEx();
            checkBox = new CheckBoxEx();
            refreshButton = new ButtonEx();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            textBox.Location = new Point(12, 32);
            textBox.Name = "textBox";
            textBox.Size = new Size(466, 22);
            textBox.TabIndex = 1;
            textBox.TextChanged += TextBoxTextChanged;
            textBox.KeyDown += TextBoxKeyDown;
            // 
            // refreshButton
            //
            refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            refreshButton.Location = new Point(485, 30);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(26, 23);
            refreshButton.TabIndex = 4;
            refreshButton.Click += RefreshButtonClick;
            // 
            // cbInClasspathsOnly
            // 
            cbInClasspathsOnly.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbInClasspathsOnly.Location = new Point(380, 9);
            cbInClasspathsOnly.Size = new Size(26, 24);
            cbInClasspathsOnly.Text = "In Classpaths only";
            cbInClasspathsOnly.Name = "cbInClasspathsOnly";
            cbInClasspathsOnly.AutoSize = true;
            cbInClasspathsOnly.TabIndex = 2;
            cbInClasspathsOnly.Checked = false;
            cbInClasspathsOnly.CheckedChanged += CbInClasspathsOnlyCheckedChanged;
            // 
            // checkBox
            //
            checkBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox.Location = new Point(480, 9);
            checkBox.Size = new Size(26, 24);
            checkBox.Text = "Code files only";
            checkBox.Name = "checkBox";
            checkBox.AutoSize = true;
            checkBox.TabIndex = 3;
            checkBox.Checked = false;
            checkBox.CheckedChanged += CheckBoxCheckedChanged;
            // 
            // listBox
            // 
            listBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.IntegralHeight = false;
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(12, 62);
            listBox.Name = "listBox";
            listBox.Size = new Size(498, 264);
            listBox.TabIndex = 5;
            listBox.DrawItem += ListBoxDrawItem;
            listBox.Resize += ListBoxResize;
            listBox.DoubleClick += ListBoxDoubleClick;
            // 
            // OpenResourceForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(522, 340);
            Controls.Add(cbInClasspathsOnly);
            Controls.Add(listBox);
            Controls.Add(textBox);
            Controls.Add(refreshButton);
            Controls.Add(checkBox);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 300);
            Name = "OpenResourceForm";
            ShowIcon = false;
            KeyPreview = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Open Resource";
            KeyDown += OpenResourceKeyDown;
            Load += OpenResourceFormLoad;
            Activated += OpenResourceFormActivated;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(PluginBase.MainForm.FindImage("-1|24|0|0", false));
            refreshButton.ImageList = imageList;
            refreshButton.ImageIndex = 0;
        }

        void InitializeLocalization()
        {
            cbInClasspathsOnly.Text = TextHelper.GetString("Label.InClasspathsOnly");
            checkBox.Text = TextHelper.GetString("Label.CodeFilesOnly");
            Text = " " + TextHelper.GetString("Title.OpenResource");
        }

        void RefreshButtonClick(object sender, EventArgs e)
        {
            CreateFileList();
            RefreshListBox();
        }

        void CbInClasspathsOnlyCheckedChanged(object sender, EventArgs e)
        {
            CreateFileList();
            RefreshListBox();
        }

        void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            CreateFileList();
            RefreshListBox();
        }

        void OpenResourceFormLoad(object sender, EventArgs e)
        {
            CreateFileList();
            RefreshListBox();
        }

        void OpenResourceFormActivated(object sender, EventArgs e)
        {
            textBox.Focus();
            textBox.SelectAll();
            if (previousSearch != null)
            {
                previousSearch = null;
                UpdateOpenFiles();
                textBox.Focus();
            }
        }

        void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            if (e.Index >= 0)
            {
                var fullName = (string)listBox.Items[e.Index];
                if (fullName == ITEM_SPACER)
                {
                    e.Graphics.FillRectangle(new SolidBrush(listBox.BackColor), e.Bounds);
                    int y = (e.Bounds.Top + e.Bounds.Bottom)/2;
                    e.Graphics.DrawLine(Pens.Gray, e.Bounds.Left, y, e.Bounds.Right, y);
                }
                else if (!selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(listBox.BackColor), e.Bounds);
                    int slashIndex = fullName.LastIndexOf(Path.DirectorySeparatorChar);
                    string path = fullName.Substring(0, slashIndex + 1);
                    string name = fullName.Substring(slashIndex + 1);
                    int pathSize = DrawHelper.MeasureDisplayStringWidth(e.Graphics, path, e.Font) - 2;
                    if (pathSize < 0) pathSize = 0; // No negative padding...
                    e.Graphics.DrawString(path, e.Font, Brushes.Gray, e.Bounds.Left, e.Bounds.Top, StringFormat.GenericDefault);
                    e.Graphics.DrawString(name, e.Font, Brushes.Black, e.Bounds.Left + pathSize, e.Bounds.Top, StringFormat.GenericDefault);
                    e.DrawFocusRectangle();
                }
                else
                {
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                    e.Graphics.DrawString(fullName, e.Font, Brushes.White, e.Bounds.Left, e.Bounds.Top, StringFormat.GenericDefault);
                }
            }
        }

        void RefreshListBox()
        {
            listBox.BeginUpdate();
            listBox.Items.Clear();
            FillListBox();
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = 0;
            }
            listBox.EndUpdate();
        }

        void FillListBox()
        {
            List<string> matchedFiles;
            if (textBox.Text.Length > 0)
            {
                string searchText = textBox.Text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                matchedFiles = SearchUtil.getMatchedItems(openedFiles, searchText, Path.DirectorySeparatorChar, 0);
                if (matchedFiles.Capacity > 0) matchedFiles.Add(ITEM_SPACER);
                matchedFiles.AddRange(SearchUtil.getMatchedItems(projectFiles, searchText, Path.DirectorySeparatorChar, MAX_ITEMS));
            }
            else matchedFiles = openedFiles;
            foreach (string file in matchedFiles)
            {
               listBox.Items.Add(file);
            }
        }

        /// <summary>
        /// Check open files and update collections accordingly
        /// </summary>
        void UpdateOpenFiles()
        {
            List<string> open = GetOpenFiles();
            List<string> folders = GetProjectFolders();
            List<string> prevOpen = openedFiles;
            openedFiles = new List<string>();
            foreach (string file in open)
            {
                foreach (string folder in folders)
                {
                    if (file.StartsWithOrdinal(folder))
                    {
                        openedFiles.Add(PluginBase.CurrentProject.GetRelativePath(file));
                        break;
                    }
                }
            }
            foreach (string file in prevOpen)
            {
                if (!openedFiles.Contains(file)) projectFiles.Add(file);
            }
            foreach (string file in openedFiles)
            {
                if (projectFiles.Contains(file)) projectFiles.Remove(file);
            }
        }

        void CreateFileList()
        {
            var open = GetOpenFiles();
            openedFiles = new List<string>();
            projectFiles = new List<string>();
            var allFiles = GetProjectFiles();
            foreach (string file in allFiles)
            {
                if (open.Contains(file)) openedFiles.Add(PluginBase.CurrentProject.GetRelativePath(file));
                else projectFiles.Add(PluginBase.CurrentProject.GetRelativePath(file));
            }
        }

        /// <summary>
        /// Open documents paths
        /// </summary>
        /// <returns></returns>
        static List<string> GetOpenFiles()
        {
            var result = new List<string>();
            foreach (var doc in PluginBase.MainForm.Documents)
            {
                if (doc.SciControl is { } sci && !doc.IsUntitled) 
                {
                    var ext = Path.GetExtension(sci.FileName);
                    if (!PluginMain.Settings.ExcludedFileTypes.Contains(ext))
                    {
                        result.Add(sci.FileName);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a list of project related files
        /// </summary>
        public List<string> GetProjectFiles()
        {
            List<string> files = new List<string>();
            List<string> folders = GetProjectFolders();
            foreach (string folder in folders)
            {
                AddFilesInFolder(files, folder);
            }
            return files;
        }

        /// <summary>
        /// Gather files in depth avoiding hidden directories
        /// </summary>
        void AddFilesInFolder(List<string> files, string folder)
        {
            if (Directory.Exists(folder) && !isFolderHidden(folder))
            {
                try
                {
                    var searchFilters = PluginBase.CurrentProject.DefaultSearchFilter.Split(';');
                    var temp = Directory.GetFiles(folder, "*.*");
                    foreach (var file in temp)
                    {
                        var extension = Path.GetExtension(file);
                        var ignored = PluginMain.Settings.ExcludedFileTypes.Contains(extension);
                        if (ignored || (checkBox.Checked && !searchFilters.Contains("*" + extension))) continue;
                        files.Add(file);
                    }
                    foreach (var sub in Directory.GetDirectories(folder))
                    {
                        AddFilesInFolder(files, sub);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Sometimes after a directory is deleted it still "exists" for some time, but any operation on it results in "Access denied".
                }
                catch (PathTooLongException)
                {
                    // Catch this error to avoid crashing the IDE.  There isn't really a graceful way to handle this.
                }
            }
        }

        /// <summary>
        /// Gets a list of project related folders
        /// </summary>
        public List<string> GetProjectFolders()
        {
            string projectFolder = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            List<string> folders = new List<string>();
            if (!cbInClasspathsOnly.Checked) folders.Add(projectFolder);
            if (!PluginMain.Settings.SearchExternalClassPath) return folders;
            foreach (string path in PluginBase.CurrentProject.SourcePaths)
            {
                if (Path.IsPathRooted(path)) folders.Add(path);
                else
                {
                    string folder = Path.GetFullPath(Path.Combine(projectFolder, path));
                    if (cbInClasspathsOnly.Checked || !folder.StartsWithOrdinal(projectFolder)) folders.Add(folder);
                }
            }
            return folders;
        }

        /// <summary>
        /// Filter out hidden/VCS directories
        /// </summary>
        bool isFolderHidden(string folder)
        {
            string name = Path.GetFileName(folder);
            if (name.Length == 0 || !char.IsLetterOrDigit(name[0])) return true;
            foreach (string dir in PluginMain.Settings.ExcludedDirectories)
                if (dir == name) return true;
            FileInfo info = new FileInfo(folder);
            return (info.Attributes & FileAttributes.Hidden) > 0;
        }

        void Navigate()
        {
            if (listBox.SelectedItem != null)
            {
                string file = PluginBase.CurrentProject.GetAbsolutePath((string)listBox.SelectedItem);
                ((Form)PluginBase.MainForm).BeginInvoke((MethodInvoker)delegate
                {
                    plugin.OpenFile(file);
                });
                Close();
            }
        }

        void OpenResourceKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Navigate();
            }
            else if (e.KeyData == (Keys.Control | Keys.R))
            {
                e.SuppressKeyPress = true;
                CreateFileList();
                RefreshListBox();
            }
        }

        void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && listBox.SelectedIndex < listBox.Items.Count - 1)
            {
                listBox.SelectedIndex++;
                if (listBox.SelectedItem.ToString() == ITEM_SPACER)
                {
                    try { listBox.SelectedIndex++; }
                    catch { }
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && listBox.SelectedIndex > 0)
            {
                listBox.SelectedIndex--;
                if (listBox.SelectedItem.ToString() == ITEM_SPACER)
                {
                    try { listBox.SelectedIndex--; }
                    catch { }
                }
                e.Handled = true;
            }
        }

        void TextBoxTextChanged(object sender, EventArgs e)
        {
            RefreshListBox();
        }

        void ListBoxDoubleClick(object sender, EventArgs e)
        {
            Navigate();
        }

        void ListBoxResize(object sender, EventArgs e)
        {
            listBox.Refresh();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            previousSearch = textBox.Text;
        }

        #endregion

    }

    #region Helpers

    internal struct SearchResult
    {
        public double Score;
        public double FolderScore;
        public string Value;
    }

    public class SearchUtil
    {
        public static List<string> getMatchedItems(List<string> source, string searchText, string pathSeparator, int limit)
        {
            return getMatchedItems(source, searchText, pathSeparator[0], limit);
        }

        public static List<string> getMatchedItems(List<string> source, string searchText, char pathSeparator, int limit)
        {
            var i = 0;
            var matchedItems = new List<SearchResult>();
            string searchFile;
            string searchDir;
            try
            {
                searchFile = Path.GetFileName(searchText);
                searchDir = Path.GetDirectoryName(searchText);
            }
            catch (ArgumentException)
            {
                return new List<string>();
            }

            foreach (var item in source)
            {
                double score;
                var file = Path.GetFileName(item);
                var dir = Path.GetDirectoryName(item);

                //score file name
                if (AdvancedSearchMatch(file, searchFile))
                    score = 1000.0;
                else
                    score = Score(file, searchFile, pathSeparator);

                //score /= file.Length; //divide by length to prefer shorter results

                if (score <= 0) continue;

                //score folder path
                var folderScore = 0.0;
                if (!string.IsNullOrEmpty(searchDir))
                    folderScore = ScoreWithoutNormalize(dir, searchDir, pathSeparator); //do not divide by length here, because short folders should not be favoured too much

                var result = new SearchResult
                {
                    Score = score,
                    FolderScore = folderScore,
                    Value = item
                };
                matchedItems.Add(result);
            }

            //sort results in following priority: folderScore, score, length (folderScore being the most important one)
            var sortedMatches = matchedItems.OrderByDescending(r => r.FolderScore).ThenByDescending(r => r.Score).ThenBy(r => r.Value.Length);

            var results = new List<string>();
            foreach (var r in sortedMatches)
            {
                if (limit > 0 && i++ >= limit) break;
                results.Add(r.Value);
            }

            return results;
        }

        static bool AdvancedSearchMatch(string file, string searchText)
        {
            if (!string.Equals(searchText.ToUpperInvariant(), searchText)) return false;

            int i = 0; int j = 0;
            if (file.Length < searchText.Length) return false;
            var text = file.ToCharArray();
            var pattern = searchText.ToCharArray();
            while (i < pattern.Length)
            {
                while (i < pattern.Length && j < text.Length && pattern[i] == text[j])
                {
                    i++;
                    j++;
                }
                if (i == pattern.Length) return true;
                if (char.IsLower(pattern[i])) return false;
                while (j < text.Length && char.IsLower(text[j]))
                {
                    j++;
                }
                if (j == text.Length) return false;
                if (pattern[i] != text[j]) return false;
            }
            return i == pattern.Length;
        }

        static double Score(string str, string query, char pathSeparator)
        {
            var score = ScoreWithoutNormalize(str, query, pathSeparator);

            return (score / str.Length + score / query.Length) / 2;
        }

        /**
         * Ported from: https://github.com/atom/fuzzaldrin/
         */
        static double ScoreWithoutNormalize(string str, string query, char pathSeparator)
        {
            double score = 0;

            if (str.StartsWith(query, StringComparison.OrdinalIgnoreCase)) //Starts with bonus
                return query.Length + 1;

            if (str.ToLower().Contains(query.ToLower())) //Contains bonus
                return query.Length;

            int strIndex = 0;

            for (int i = 0; i < query.Length; i++)
            {
                var character = query[i].ToString();

                var index = str.IndexOf(character, strIndex, StringComparison.OrdinalIgnoreCase);

                if (index == -1)
                    return 0;

                var charScore = 0.1;

                if (str[index] == query[i]) //same case bonus
                    charScore += 0.1;

                if (index == 0 || str[index - 1] == pathSeparator) //start of string bonus
                    charScore += 0.8;
                else if (i == index) //equivalent position bonus
                    charScore += 0.5;

                score += charScore;
                strIndex = index + 1;
            }

            return score;
        }

    }

    #endregion

}
