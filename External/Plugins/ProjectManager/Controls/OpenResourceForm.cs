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
        public double DirScore;
        public string Value;
    }

    public class SearchUtil
    {
        // Note: These values may need more tuning to get best results.
        private const double FileScoreWeightFactor = 1.0;
        // Put more weight on directory scores, since file scores tend to have more variation due to their dependency on file name length.
        private const double DirScoreWeightFactor = 4.0;

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
                if (searchDir == null) searchDir = "";
                if (searchFile == "" && searchDir == "") return new List<string>();
            }
            catch (ArgumentException)
            {
                return new List<string>();
            }

            // If query contains any upper-case letters, consider case significant and match
            bool fileCaseMatters = !string.Equals(searchFile.ToLowerInvariant(), searchFile);
            bool dirCaseMatters = !string.Equals(searchDir.ToLowerInvariant(), searchDir);
            double fileScoreWeight = FileScoreWeightFactor * searchFile.Length;
            double dirScoreWeight = DirScoreWeightFactor * String.Join("", searchDir.Split(pathSeparator)).Length;

            double score;
            double dirScore;
            double maxScore = 0;
            double maxDirScore = 0;

            foreach (var item in source)
            {
                var file = Path.GetFileName(item);
                var dir = Path.GetDirectoryName(item);

                if (fileScoreWeight != 0)
                {
                    score = ScoreFileName(file, searchFile, fileCaseMatters);
                    if (score <= 0) continue;
                    if (score > maxScore) maxScore = score;
                }
                else
                {
                    score = 0;
                }

                if (dirScoreWeight != 0)
                {
                    dirScore = ScoreDirName(dir, searchDir, dirCaseMatters, pathSeparator);
                    if (dirScore <= 0) continue;
                    if (dirScore > maxDirScore) maxDirScore = dirScore;
                }
                else
                {
                    dirScore = 0;
                }

                var result = new SearchResult
                {
                    Score = score,
                    DirScore = dirScore,
                    Value = item
                };
                matchedItems.Add(result);
            }

            if (maxScore == 0) maxScore = 1;
            if (maxDirScore == 0) maxDirScore = 1;

            // File and directory (path) score are first normalized (division by maxScore/maxDirScore).
            // Final score is a weighted sum of file and directory (path) scores.
            // The weights are respectively the number of file and path query characters (excluding path separator characters),
            // multiplied by tunable constant values (FileScoreWeightFactor and DirScoreWeightFactor).
            var sortedMatches = matchedItems
                .OrderByDescending(r => fileScoreWeight * r.Score / maxScore + dirScoreWeight * r.DirScore / maxDirScore)
                .ThenBy(r => r.Value.Length)
                .ThenBy(r => r.Value);

            var results = new List<string>();
            foreach (var r in sortedMatches)
            {
                if (limit > 0 && i++ >= limit) break;
                results.Add(r.Value);
            }

            return results;
        }

        static double ScoreFileName(string str, string query, bool caseMatters)
        {
            // Prefer shorter results. The effect is less pronounced when query length is increased.
            return Score(str, query, caseMatters) / (str.Length + query.Length);
        }

        static double ScoreDirName(string str, string query, bool caseMatters, char pathSeparator)
        {
            return Score(str, query, caseMatters, pathSeparator);
        }

        /**
         * Based on: https://github.com/atom/fuzzaldrin/
         */
        static double Score(string str, string query, bool caseMatters, char? pathSeparator = null)
        {
            if (str.StartsWith(query, caseMatters ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                return query.Length + 1; // Exact match at the beginning - highest score

            if (pathSeparator == null && (caseMatters ? str.Contains(query) : str.ToLower().Contains(query.ToLower())))
                return query.Length; // Exact match somewhere - second highest score

            double score = 0;
            int strIndex = 0;

            for (int i = 0; i < query.Length; i++)
            {
                var queryChar = query[i];

                // Use case-sensitive matching if caseMatters is true. Should provide results similar to the superseded `AdvancedSearchMatch`.
                var index = str.IndexOf(queryChar.ToString(), strIndex, caseMatters ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                if (index == -1) return 0;

                double charScore;

                if (queryChar != pathSeparator)
                {
                    charScore = 0.1;

                    if (str[index] == queryChar)
                        charScore += 0.1; // Exact character match (same case if character is a letter)

                    if (pathSeparator != null && index > 0)
                    {
                        // Score a directory (path). Prefer when each path component in the query matches a single path component in the directory.
                        if (index == strIndex || str.IndexOf(pathSeparator.Value, strIndex, index - strIndex) == -1)
                            charScore += 0.5; // Consecutive path component

                        if (str[index - 1] == pathSeparator)
                            charScore += 0.3; // Position at the beginning of path component
                        else if ((!Char.IsLetter(str[index - 1]) && Char.IsLetter(queryChar)) || (Char.IsLower(str[index - 1]) && Char.IsUpper(str[index])))
                            charScore += 0.3; // Position at the beginning of a word boundary (non-word to word or camelCase lower to upper case)
                        else if (index == i)
                            charScore += 0.2; // Consecutive position at the beginning of string
                        else if (index == strIndex)
                            charScore += 0.1; // Consecutive position relative to previous matched character
                    }
                    else
                    {
                        // Score a file. Prefer first character to match at the beginning of string, next characters at word boundaries.
                        if (index == 0)
                            charScore += 0.8; // Position at the beginning of string
                        else if ((!Char.IsLetter(str[index - 1]) && Char.IsLetter(queryChar)) || (Char.IsLower(str[index - 1]) && Char.IsUpper(str[index])))
                            charScore += 0.7; // Position at the beginning of a word boundary (non-word to word or camelCase lower to upper case)
                        else if (index == i)
                            charScore += 0.5; // Consecutive position at the beginning of string
                        else if (index == strIndex)
                            charScore += 0.3; // Consecutive position relative to previous matched character
                    }
                }
                else
                {
                    charScore = 0.2;

                    if (index == i)
                        charScore += 0.5; // Consecutive position at the beginning of string
                    else if (index == strIndex)
                        charScore += 0.3; // Consecutive position relative to previous matched character
                }

                score += charScore;
                strIndex = index + 1;
            }

            return score;
        }

    }

    #endregion

}
