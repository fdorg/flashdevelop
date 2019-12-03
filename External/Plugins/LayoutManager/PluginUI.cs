using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;

namespace LayoutManager
{
    public class PluginUI : DockPanelControl
    {
        private ToolStrip toolStrip;
        private ListViewEx layoutsListView;
        private ListViewItem infoListViewItem;
        private ToolStripMenuItem menuLoadButton;
        private ToolStripMenuItem menuSaveButton;
        private ToolStripMenuItem menuDeleteButton;
        private ToolStripMenuItem menuSettingButton;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripButton settingStripButton;
        private ToolStripButton deleteStripButton;
        private ToolStripButton saveStripButton;
        private ToolStripButton loadStripButton;
        private PluginMain pluginMain;
        private ImageListManager imageList;
        
        public PluginUI(PluginMain pluginMain)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeGraphics();
            this.InitializeTexts();
            ScrollBarEx.Attach(layoutsListView);
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.layoutsListView = new System.Windows.Forms.ListViewEx();
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.loadStripButton = new System.Windows.Forms.ToolStripButton();
            this.deleteStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveStripButton = new System.Windows.Forms.ToolStripButton();
            this.settingStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutsListView
            // 
            this.layoutsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.layoutsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.layoutsListView.MultiSelect = false;
            this.layoutsListView.Name = "layoutsListView";
            this.layoutsListView.Size = new System.Drawing.Size(297, 256);
            this.layoutsListView.TabIndex = 11;
            this.layoutsListView.UseCompatibleStateImageBehavior = false;
            this.layoutsListView.View = System.Windows.Forms.View.List;
            this.layoutsListView.DoubleClick += this.LayoutsListViewDoubleClick;
            this.layoutsListView.KeyDown += this.LayoutsListViewKeyDown;
            this.layoutsListView.SelectedIndexChanged += this.LayoutsListViewSelectedIndexChanged;
            // 
            // toolStrip
            //
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadStripButton,
            this.deleteStripButton,
            this.saveStripButton,
            this.toolStripSeparator,
            this.settingStripButton});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(297, 25);
            this.toolStrip.TabIndex = 13;
            // 
            // loadStripButton
            //
            this.loadStripButton.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.loadStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadStripButton.Name = "loadStripButton";
            this.loadStripButton.Size = new System.Drawing.Size(23, 22);
            this.loadStripButton.ToolTipText = "Load Layout";
            this.loadStripButton.Click += this.LoadButtonClick;
            // 
            // deleteStripButton
            //
            this.deleteStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.deleteStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteStripButton.Name = "deleteStripButton";
            this.deleteStripButton.Size = new System.Drawing.Size(23, 22);
            this.deleteStripButton.ToolTipText = "Delete Layout";
            this.deleteStripButton.Click += this.DeleteButtonClick;
            // 
            // saveStripButton
            //
            this.saveStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.saveStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveStripButton.Name = "saveStripButton";
            this.saveStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveStripButton.ToolTipText = "Save Current...";
            this.saveStripButton.Click += this.SaveButtonClick;
            // 
            // settingStripButton
            //
            this.settingStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.settingStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingStripButton.Name = "settingsStripButton";
            this.settingStripButton.Size = new System.Drawing.Size(23, 22);
            this.settingStripButton.ToolTipText = "Show Settings...";
            this.settingStripButton.Click += this.SettingsButtonClick;
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            this.toolStripSeparator.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            // 
            // PluginUI
            //
            this.Name = "PluginUI";
            this.Controls.Add(this.layoutsListView);
            this.Controls.Add(this.toolStrip);
            this.Size = new System.Drawing.Size(299, 283);
            this.Load += this.FormLoaded;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the used graphics
        /// </summary>
        private void InitializeGraphics()
        {
            this.imageList = new ImageListManager();
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.TransparentColor = Color.Transparent;
            this.imageList.Initialize(ImageList_Populate);
            this.layoutsListView.SmallImageList = this.imageList;
            this.menuLoadButton.Image = PluginBase.MainForm.FindImage("42|24|3|2");
            this.loadStripButton.Image = PluginBase.MainForm.FindImage("42|24|3|2");
            this.menuDeleteButton.Image = PluginBase.MainForm.FindImage("153");
            this.deleteStripButton.Image = PluginBase.MainForm.FindImage("153");
            this.menuSaveButton.Image = PluginBase.MainForm.FindImage("168");
            this.saveStripButton.Image = PluginBase.MainForm.FindImage("168");
            this.menuSettingButton.Image = PluginBase.MainForm.FindImage("54");
            this.settingStripButton.Image = PluginBase.MainForm.FindImage("54");
            this.toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
        }

        private void ImageList_Populate(object sender, EventArgs e)
        {
            this.imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("48"));
            this.imageList.Images.Add(PluginBase.MainForm.FindImageAndSetAdjust("229"));
        }

        /// <summary>
        /// Creates and attaches the context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ImageScalingSize = PluginCore.Helpers.ScaleHelper.Scale(new Size(16, 16));
            this.menuLoadButton = new ToolStripMenuItem(TextHelper.GetString("Label.LoadLayout"), null, this.LoadButtonClick);
            this.menuDeleteButton = new ToolStripMenuItem(TextHelper.GetString("Label.DeleteLayout"), null, this.DeleteButtonClick);
            this.menuSaveButton = new ToolStripMenuItem(TextHelper.GetString("Label.SaveCurrent"), null, this.SaveButtonClick);
            this.menuSettingButton = new ToolStripMenuItem(TextHelper.GetString("Label.ShowSettings"), null, this.SettingsButtonClick);
            menu.Items.AddRange(new ToolStripMenuItem[4] { this.menuLoadButton, this.menuDeleteButton, this.menuSaveButton, this.menuSettingButton});
            menu.Items.Insert(3, new ToolStripSeparator());
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            this.layoutsListView.ContextMenuStrip = menu;
        }

        /// <summary>
        /// Applies the localized texts to the control
        /// </summary>
        private void InitializeTexts()
        {
            this.loadStripButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.LoadLayout");
            this.deleteStripButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.DeleteLayout");
            this.settingStripButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.ShowSettings");
            this.saveStripButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.SaveCurrent");
        }

        /// <summary>
        /// Populates the list view on load
        /// </summary>
        private void FormLoaded(object sender, EventArgs e)
        {
            this.toolStrip.Renderer = new DockPanelStripRenderer();
            this.toolStrip.ImageScalingSize = PluginCore.Helpers.ScaleHelper.Scale(new Size(16, 16));
            this.infoListViewItem = new ListViewItem(TextHelper.GetString("Info.NoLayoutsFound"), 1);
            string file = Path.Combine(this.GetLayoutsDir(), "DefaultLayout.fdl");
            if (!File.Exists(file)) WriteDefaultLayout(file);
            this.PopulateLayoutsListView();
        }

        /// <summary>
        /// Updates the ui elements
        /// </summary>
        private void UpdatePluginUI()
        {
            if (this.layoutsListView.Items.Count == 0)
            {
                this.menuLoadButton.Enabled = false;
                this.menuDeleteButton.Enabled = false;
                this.layoutsListView.Items.Add(this.infoListViewItem);
                this.deleteStripButton.Enabled = false;
                this.loadStripButton.Enabled = false;
            }
            else if (this.layoutsListView.SelectedItems.Count == 0)
            {
                this.loadStripButton.Enabled = false;
                this.deleteStripButton.Enabled = false;
                this.menuDeleteButton.Enabled = false;
                this.menuLoadButton.Enabled = false;
            }
            else
            {
                if (this.GetSelectedItem().ImageIndex != 1)
                {
                    this.loadStripButton.Enabled = true;
                    this.deleteStripButton.Enabled = true;
                    this.menuDeleteButton.Enabled = true;
                    this.menuLoadButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Populates the list view with workspaces
        /// </summary>
        private void PopulateLayoutsListView()
        {
            this.layoutsListView.Items.Clear();
            string[] layouts = Directory.GetFiles(this.GetLayoutsDir(), "*.fdl");
            foreach (var layout in layouts)
            {
                var label = Path.GetFileNameWithoutExtension(layout);
                var item = new ListViewItem(label, 0);
                item.Tag = layout; // Store full path
                this.layoutsListView.Items.Add(item);
            }
            this.UpdatePluginUI();
        }

        /// <summary>
        /// Shows the plugin settings
        /// </summary>
        private void SettingsButtonClick(object sender, EventArgs e)
        {
            try
            {
                PluginBase.MainForm.ShowSettingsDialog("LayoutManager");
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Loads the selected layout
        /// </summary>
        private void LoadButtonClick(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = this.GetSelectedItem();
                PluginBase.MainForm.CallCommand("RestoreLayout", item.Tag.ToString());
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Deletes the selected layout
        /// </summary>
        private void DeleteButtonClick(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = GetSelectedItem();
                if (!FileHelper.Recycle(item.Tag.ToString()))
                {
                    string message = TextHelper.GetString("FlashDevelop.Info.CouldNotBeRecycled");
                    throw new Exception(message + " " + item.Tag);
                }
                this.PopulateLayoutsListView();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the selected layout
        /// </summary>
        private void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                using SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = TextHelper.GetString("Info.OpenFileFilter");
                sfd.InitialDirectory = this.GetLayoutsDir();
                sfd.DefaultExt = "fdl"; sfd.FileName = "";
                if (sfd.ShowDialog(this) == DialogResult.OK && sfd.FileName.Length != 0)
                {
                    PluginBase.MainForm.DockPanel.SaveAsXml(sfd.FileName);
                    this.PopulateLayoutsListView();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Updates the ui when on index change
        /// </summary>
        private void LayoutsListViewSelectedIndexChanged(object sender, EventArgs e) => UpdatePluginUI();

        /// <summary>
        /// Loads the selected layout on double click
        /// </summary>
        private void LayoutsListViewDoubleClick(object sender, EventArgs e)
        {
            var item = this.GetSelectedItem();
            if (item != null && item.ImageIndex != 1)
            {
                PluginBase.MainForm.CallCommand("RestoreLayout", item.Tag.ToString());
            }
        }

        /// <summary>
        /// If the user presses Enter, dispatch double click
        /// </summary>
        void LayoutsListViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            LayoutsListViewDoubleClick(null, null);
            e.Handled = true;
        }

        /// <summary>
        /// Gets the selected list view item
        /// </summary>
        private ListViewItem GetSelectedItem()
        {
            try
            {
                return this.layoutsListView.SelectedItems[0];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the layouts directory
        /// </summary>
        private string GetLayoutsDir()
        {
            var userPath = Settings.Instance.CustomLayoutPath;
            if (Directory.Exists(userPath)) return userPath;
            var path = Path.Combine(Path.Combine(PathHelper.DataDir, nameof(LayoutManager)), "Layouts");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Copies the default layout file to disk
        /// </summary> 
        private void WriteDefaultLayout(string file)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var src = assembly.GetManifestResourceStream("LayoutManager.Resources.Default.fdl");
                using var reader = new StreamReader(src, true);
                var content = reader.ReadToEnd();
                reader.Close();
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                FileHelper.WriteFile(file, content, Encoding.Unicode);
            }
            catch {}
        }

        #endregion
    }
}