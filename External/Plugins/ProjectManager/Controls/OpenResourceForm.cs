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
        private PluginMain plugin;
        private Int32 MAX_ITEMS = 100;
        private List<String> openedFiles;
        private List<String> projectFiles;
        private const String ITEM_SPACER = "-----------------";
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.CheckBox cbInClasspathsOnly;
        private System.Windows.Forms.CheckBox checkBox;
        private System.Windows.Forms.Button refreshButton;
        private static string previousSearch;

        public OpenResourceForm(PluginMain plugin)
        {
            this.plugin = plugin;
            this.InitializeComponent();
            this.InitializeGraphics();
            this.InitializeLocalization();
            this.Font = PluginBase.Settings.DefaultFont;
            this.listBox.ItemHeight = this.listBox.Font.Height;
            this.FormGuid = "8e4e0a95-0aff-422c-b8f5-ad9bc8affabb";
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBoxEx();
            this.listBox = new System.Windows.Forms.ListBoxEx();
            this.cbInClasspathsOnly = new System.Windows.Forms.CheckBoxEx();
            this.checkBox = new System.Windows.Forms.CheckBoxEx();
            this.refreshButton = new System.Windows.Forms.ButtonEx();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Location = new System.Drawing.Point(12, 32);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(466, 22);
            this.textBox.TabIndex = 1;
            this.textBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            // 
            // refreshButton
            //
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.Location = new System.Drawing.Point(485, 30);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(26, 23);
            this.refreshButton.TabIndex = 4;
            this.refreshButton.Click += new EventHandler(RefreshButtonClick);
            // 
            // cbInClasspathsOnly
            // 
            this.cbInClasspathsOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbInClasspathsOnly.Location = new System.Drawing.Point(380, 9);
            this.cbInClasspathsOnly.Size = new System.Drawing.Size(26, 24);
            this.cbInClasspathsOnly.Text = "In Classpaths only";
            this.cbInClasspathsOnly.Name = "cbInClasspathsOnly";
            this.cbInClasspathsOnly.AutoSize = true;
            this.cbInClasspathsOnly.TabIndex = 2;
            this.cbInClasspathsOnly.Checked = false;
            this.cbInClasspathsOnly.CheckedChanged += new System.EventHandler(this.CbInClasspathsOnlyCheckedChanged);
            // 
            // checkBox
            //
            this.checkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox.Location = new System.Drawing.Point(480, 9);
            this.checkBox.Size = new System.Drawing.Size(26, 24);
            this.checkBox.Text = "Code files only";
            this.checkBox.Name = "checkBox";
            this.checkBox.AutoSize = true;
            this.checkBox.TabIndex = 3;
            this.checkBox.Checked = false;
            this.checkBox.CheckedChanged += new EventHandler(this.CheckBoxCheckedChanged);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox.IntegralHeight = false;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 62);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(498, 264);
            this.listBox.TabIndex = 5;
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxDrawItem);
            this.listBox.Resize += new System.EventHandler(this.ListBoxResize);
            this.listBox.DoubleClick += new System.EventHandler(this.ListBoxDoubleClick);
            // 
            // OpenResourceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 340);
            this.Controls.Add(this.cbInClasspathsOnly);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.checkBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 300);
            this.Name = "OpenResourceForm";
            this.ShowIcon = false;
            this.KeyPreview = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Resource";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpenResourceKeyDown);
            this.Load += new EventHandler(OpenResourceFormLoad);
            this.Activated += new EventHandler(OpenResourceFormActivated);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(PluginBase.MainForm.FindImage("-1|24|0|0", false));
            this.refreshButton.ImageList = imageList;
            this.refreshButton.ImageIndex = 0;
        }

        private void InitializeLocalization()
        {
            this.cbInClasspathsOnly.Text = TextHelper.GetString("Label.InClasspathsOnly");
            this.checkBox.Text = TextHelper.GetString("Label.CodeFilesOnly");
            this.Text = " " + TextHelper.GetString("Title.OpenResource");
        }

        private void RefreshButtonClick(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        private void CbInClasspathsOnlyCheckedChanged(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        private void CheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        private void OpenResourceFormLoad(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        private void OpenResourceFormActivated(Object sender, EventArgs e)
        {
            this.textBox.Focus();
            this.textBox.SelectAll();
            if (previousSearch != null)
            {
                previousSearch = null;
                this.UpdateOpenFiles();
                this.textBox.Focus();
            }
        }

        private void ListBoxDrawItem(Object sender, DrawItemEventArgs e)
        {
            Boolean selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            if (e.Index >= 0)
            {
                var fullName = (String)this.listBox.Items[e.Index];
                if (fullName == ITEM_SPACER)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.listBox.BackColor), e.Bounds);
                    int y = (e.Bounds.Top + e.Bounds.Bottom)/2;
                    e.Graphics.DrawLine(Pens.Gray, e.Bounds.Left, y, e.Bounds.Right, y);
                }
                else if (!selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.listBox.BackColor), e.Bounds);
                    Int32 slashIndex = fullName.LastIndexOf(Path.DirectorySeparatorChar);
                    String path = fullName.Substring(0, slashIndex + 1);
                    String name = fullName.Substring(slashIndex + 1);
                    Int32 pathSize = DrawHelper.MeasureDisplayStringWidth(e.Graphics, path, e.Font) - 2;
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

        private void RefreshListBox()
        {
            this.listBox.BeginUpdate();
            this.listBox.Items.Clear();
            this.FillListBox();
            if (this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }
            this.listBox.EndUpdate();
        }

        private void FillListBox()
        {
            List<String> matchedFiles;
            if (this.textBox.Text.Length > 0)
            {
                String searchText = this.textBox.Text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                matchedFiles = SearchUtil.getMatchedItems(this.openedFiles, searchText, Path.DirectorySeparatorChar, 0);
                if (matchedFiles.Capacity > 0) matchedFiles.Add(ITEM_SPACER);
                matchedFiles.AddRange(SearchUtil.getMatchedItems(this.projectFiles, searchText, Path.DirectorySeparatorChar, this.MAX_ITEMS));
            }
            else matchedFiles = openedFiles;
            foreach (String file in matchedFiles)
            {
               this.listBox.Items.Add(file);
            }
        }

        /// <summary>
        /// Check open files and update collections accordingly
        /// </summary>
        private void UpdateOpenFiles()
        {
            List<String> open = this.GetOpenFiles();
            List<String> folders = this.GetProjectFolders();
            List<String> prevOpen = openedFiles;
            openedFiles = new List<String>();
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

        private void CreateFileList()
        {
            List<String> open = this.GetOpenFiles();
            openedFiles = new List<String>();
            projectFiles = new List<String>();
            List<String> allFiles = this.GetProjectFiles();
            foreach (String file in allFiles)
            {
                if (open.Contains(file)) openedFiles.Add(PluginBase.CurrentProject.GetRelativePath(file));
                else projectFiles.Add(PluginBase.CurrentProject.GetRelativePath(file));
            }
        }

        /// <summary>
        /// Open documents paths
        /// </summary>
        /// <returns></returns>
        private List<string> GetOpenFiles()
        {
            List<String> open = new List<String>();
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                if (doc.IsEditable && !doc.IsUntitled) 
                {
                    String ext = Path.GetExtension(doc.FileName);
                    if (Array.IndexOf(PluginMain.Settings.ExcludedFileTypes, ext) == -1)
                    {
                        open.Add(doc.FileName);
                    }
                }
            }
            return open;
        }

        /// <summary>
        /// Gets a list of project related files
        /// </summary>
        public List<String> GetProjectFiles()
        {
            List<String> files = new List<String>();
            List<String> folders = this.GetProjectFolders();
            foreach (String folder in folders)
            {
                AddFilesInFolder(files, folder);
            }
            return files;
        }

        /// <summary>
        /// Gather files in depth avoiding hidden directories
        /// </summary>
        private void AddFilesInFolder(List<String> files, String folder)
        {
            if (Directory.Exists(folder) && !isFolderHidden(folder))
            {
                try
                {
                    String[] temp = Directory.GetFiles(folder, "*.*");
                    foreach (String file in temp)
                    {
                        String extension = Path.GetExtension(file);
                        String[] filters = PluginBase.CurrentProject.DefaultSearchFilter.Split(';');
                        Boolean ignored = Array.IndexOf(PluginMain.Settings.ExcludedFileTypes, extension) > -1;
                        if (ignored || (this.checkBox.Checked && Array.IndexOf(filters, "*" + extension) == -1)) continue;
                        files.Add(file);
                    }
                    foreach (string sub in Directory.GetDirectories(folder))
                    {
                        AddFilesInFolder(files, sub);
                    }
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
        public List<String> GetProjectFolders()
        {
            String projectFolder = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            List<String> folders = new List<String>();
            if (!cbInClasspathsOnly.Checked) folders.Add(projectFolder);
            if (!PluginMain.Settings.SearchExternalClassPath) return folders;
            foreach (String path in PluginBase.CurrentProject.SourcePaths)
            {
                if (Path.IsPathRooted(path)) folders.Add(path);
                else
                {
                    String folder = Path.GetFullPath(Path.Combine(projectFolder, path));
                    if (cbInClasspathsOnly.Checked || !folder.StartsWithOrdinal(projectFolder)) folders.Add(folder);
                }
            }
            return folders;
        }

        /// <summary>
        /// Filter out hidden/VCS directories
        /// </summary>
        private bool isFolderHidden(string folder)
        {
            String name = Path.GetFileName(folder);
            if (name.Length == 0 || !Char.IsLetterOrDigit(name[0])) return true;
            foreach (string dir in PluginMain.Settings.ExcludedDirectories)
                if (dir == name) return true;
            FileInfo info = new FileInfo(folder);
            return (info.Attributes & FileAttributes.Hidden) > 0;
        }

        private void Navigate()
        {
            if (this.listBox.SelectedItem != null)
            {
                String file = PluginBase.CurrentProject.GetAbsolutePath((string)this.listBox.SelectedItem);
                ((Form)PluginBase.MainForm).BeginInvoke((MethodInvoker)delegate
                {
                    plugin.OpenFile(file);
                });
                this.Close();
            }
        }

        private void OpenResourceKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.Navigate();
            }
            else if (e.KeyData == (Keys.Control | Keys.R))
            {
                e.SuppressKeyPress = true;
                this.CreateFileList();
                this.RefreshListBox();
            }
        }

        private void TextBoxKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && this.listBox.SelectedIndex < this.listBox.Items.Count - 1)
            {
                this.listBox.SelectedIndex++;
                if (this.listBox.SelectedItem.ToString() == ITEM_SPACER)
                {
                    try { this.listBox.SelectedIndex++; }
                    catch { }
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && this.listBox.SelectedIndex > 0)
            {
                this.listBox.SelectedIndex--;
                if (this.listBox.SelectedItem.ToString() == ITEM_SPACER)
                {
                    try { this.listBox.SelectedIndex--; }
                    catch { }
                }
                e.Handled = true;
            }
        }

        private void TextBoxTextChanged(Object sender, EventArgs e)
        {
            this.RefreshListBox();
        }

        private void ListBoxDoubleClick(Object sender, EventArgs e)
        {
            this.Navigate();
        }

        private void ListBoxResize(Object sender, EventArgs e)
        {
            this.listBox.Refresh();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            previousSearch = this.textBox.Text;
        }

        #endregion

    }

    #region Helpers

    struct SearchResult
    {
        public double Score;
        public double DirScore;
        public string Value;
    }

    public class SearchUtil
    {
        private const double FileScoreWeightFactor = 1.0;
        private const double DirScoreWeightFactor = 3.0;

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

            if (pathSeparator == null && (caseMatters ? str.Contains(query) : str.ToLower().Contains(query.ToLower()))) // Contains bonus
                return query.Length; // Exact match - second highest score

            double score = 0;
            int strIndex = 0;

            for (int i = 0; i < query.Length; i++)
            {
                var queryChar = query[i];

                // Require exact (case-sensitive) match for upper-case characters in the query (should provide results similar to the superseded AdvancedSearchMatch).
                var index = str.IndexOf(queryChar.ToString(), strIndex, Char.IsUpper(queryChar) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
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
