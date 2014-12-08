using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using PluginCore.Localization;
using ProjectManager.Controls.TreeView;
using System.Drawing.Drawing2D;
using System.Drawing;
using PluginCore.Helpers;
using PluginCore;

namespace ProjectManager.Controls
{
    /// <summary>
    /// Tree view top toolbar
    /// </summary>
    public class TreeBar : ToolStrip
    {
        public ToolStripButton ShowHidden;
        public ToolStripButton RefreshSelected;
        public ToolStripButton ProjectProperties;
        public ToolStripButton ProjectTypes;
        public ToolStripButton Synchronize;
        public ToolStripButton SynchronizeMain;
        public ToolStripSeparator Separator;

        private ProjectContextMenu treeMenu;

        public TreeBar(FDMenus menus, ProjectContextMenu treeMenu)
        {
            this.treeMenu = treeMenu;
            this.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            this.Size = new Size(200, 26);
            this.Renderer = new DockPanelStripRenderer();
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.Padding = new Padding(2, 1, 2, 1);
            this.CanOverflow = false;

            RefreshSelected = new ToolStripButton(Icons.Refresh.Img);
            RefreshSelected.ToolTipText = TextHelper.GetString("ToolTip.Refresh");
            RefreshSelected.Padding = new Padding(0);

            ShowHidden = new ToolStripButton(Icons.HiddenItems.Img);
            ShowHidden.ToolTipText = TextHelper.GetString("ToolTip.ShowHiddenItems");
            ShowHidden.Padding = new Padding(0);

            ProjectProperties = new ToolStripButton(Icons.Options.Img);
            ProjectProperties.ToolTipText = TextHelper.GetString("ToolTip.ProjectProperties");
            ProjectProperties.Padding = new Padding(0);
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.Properties", ProjectProperties);

            Synchronize = new ToolStripButton(Icons.SyncToFile.Img);
            Synchronize.ToolTipText = TextHelper.GetString("ToolTip.Synchronize");
            Synchronize.Padding = new Padding(0);
            PluginBase.MainForm.RegisterShortcutItem("ProjectTree.LocateActiveFile", Keys.Shift | Keys.Alt | Keys.L);

            SynchronizeMain = new ToolStripButton(Icons.ActionScriptCompile.Img);
            SynchronizeMain.ToolTipText = TextHelper.GetString("ToolTip.SynchronizeMain");
            SynchronizeMain.Padding = new Padding(0);

            ProjectTypes = new ToolStripButton(Icons.AllClasses.Img);
            ProjectTypes.ToolTipText = TextHelper.GetString("ToolTip.ProjectTypes");
            ProjectTypes.Alignment = ToolStripItemAlignment.Right;
            ProjectTypes.Padding = new Padding(0);
            PluginBase.MainForm.RegisterSecondaryItem("FlashToolsMenu.TypeExplorer", ProjectTypes);
            
            Separator = new ToolStripSeparator();
            Separator.Margin = new Padding(0, 0, 1, 0);

            Items.Add(ShowHidden);
            Items.Add(Synchronize);
            Items.Add(SynchronizeMain);
            Items.Add(RefreshSelected);
            Items.Add(Separator);
            Items.Add(ProjectProperties);
            Items.Add(ProjectTypes);
        }

        public bool ProjectHasIssues
        {
            set
            {
                ProjectProperties.Image = value ? Icons.OptionsWithIssues.Img : Icons.Options.Img;
            }
        }
    }    

}
