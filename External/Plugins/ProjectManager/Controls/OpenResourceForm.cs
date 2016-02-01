using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore.Controls;
using ProjectManager;
using PluginCore;

namespace ProjectManager.Controls
{
    public class OpenResourceForm : SmartForm
    {
        private PluginMain plugin;
        private Int32 MAX_ITEMS = 100;
        private List<String> openedFiles;
        private List<String> projectFiles;
        private const String ITEM_SPACER = "-----------------";
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.CheckBox cbInClasspathsOnly;
        private System.Windows.Forms.CheckBox checkBox;
        private System.Windows.Forms.Button refreshButton;

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
            this.infoLabel = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.cbInClasspathsOnly = new System.Windows.Forms.CheckBox();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(10, 10);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(273, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Search: (UPPERCASE for search by abbreviation)";
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
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 62);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(498, 276);
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
            this.Controls.Add(this.infoLabel);
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
            this.Activated += new EventHandler(OpenResourceFormActivated);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// 
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(PluginBase.MainForm.FindImage("-1|24|0|0", false));
            this.refreshButton.ImageList = imageList;
            this.refreshButton.ImageIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeLocalization()
        {
            this.infoLabel.Text = TextHelper.GetString("Label.SearchString");
            this.cbInClasspathsOnly.Text = TextHelper.GetString("Label.InClasspathsOnly");
            this.checkBox.Text = TextHelper.GetString("Label.CodeFilesOnly");
            this.Text = " " + TextHelper.GetString("Title.OpenResource");
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshButtonClick(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CbInClasspathsOnlyCheckedChanged(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            this.CreateFileList();
            this.RefreshListBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenResourceFormActivated(Object sender, EventArgs e)
        {
            if (openedFiles == null) this.CreateFileList();
            else
            {
                this.textBox.Text = "";
                this.UpdateOpenFiles();
                this.textBox.Focus();
            }
            this.RefreshListBox();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void FillListBox()
        {
            List<String> matchedFiles;
            if (this.textBox.Text.Length > 0)
            {
                matchedFiles = SearchUtil.getMatchedItems(this.openedFiles, this.textBox.Text, "\\", 0);
                if (matchedFiles.Capacity > 0) matchedFiles.Add(ITEM_SPACER);
                matchedFiles.AddRange(SearchUtil.getMatchedItems(this.projectFiles, this.textBox.Text, "\\", this.MAX_ITEMS));
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void OpenResourceKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.Navigate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void TextBoxTextChanged(Object sender, EventArgs e)
        {
            this.RefreshListBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListBoxDoubleClick(Object sender, EventArgs e)
        {
            this.Navigate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListBoxResize(Object sender, EventArgs e)
        {
            this.listBox.Refresh();
        }

        #endregion

    }

    #region Helpers

    class SearchUtil
    {
        public delegate Boolean Comparer(String value1, String value2, String value3);

        public static List<String> getMatchedItems(List<String> source, String searchText, String pathSeparator, Int32 limit)
        {
            Int32 i = 0;
            List<String> matchedItems = new List<String>();
            String firstChar = searchText.Substring(0, 1);
            Comparer searchMatch = (firstChar == firstChar.ToUpper()) ? new Comparer(AdvancedSearchMatch) : new Comparer(SimpleSearchMatch);
            foreach (String item in source)
            {
                if (searchMatch(item, searchText, pathSeparator))
                {
                    matchedItems.Add(item);
                    if (limit > 0 && i++ > limit) break;
                }
            }
            return matchedItems;
        }

        static private bool AdvancedSearchMatch(String file, String searchText, String pathSeparator)
        {
            int i = 0; int j = 0;
            if (file.Length < searchText.Length) return false;
            Char[] text = Path.GetFileName(file).ToCharArray();
            Char[] pattern = searchText.ToCharArray();
            while (i < pattern.Length)
            {
                while (i < pattern.Length && j < text.Length && pattern[i] == text[j])
                {
                    i++;
                    j++;
                }
                if (i == pattern.Length) return true;
                if (Char.IsLower(pattern[i])) return false;
                while (j < text.Length && Char.IsLower(text[j]))
                {
                    j++;
                }
                if (j == text.Length) return false;
                if (pattern[i] != text[j]) return false;
            }
            return (i == pattern.Length);
        }

        private static Boolean SimpleSearchMatch(String file, String searchText, String pathSeparator)
        {
            String fileName = Path.GetFileName(file).ToLower();
            return fileName.IndexOfOrdinal(searchText.ToLower()) > -1;
        }

    }

    #endregion

}
