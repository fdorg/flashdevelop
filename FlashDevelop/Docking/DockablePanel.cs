using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Utilities;

namespace FlashDevelop.Docking
{
    public class DockablePanel : DockContent
    {
        private Image image;
        private String pluginGuid;
        
        public DockablePanel(Control ctrl, String pluginGuid)
        {
            this.Text = ctrl.Text;
            ctrl.Dock = DockStyle.Fill;
            this.DockPanel = Globals.MainForm.DockPanel;
            if (ctrl.Tag != null) this.TabText = ctrl.Tag.ToString();
            this.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Float;
            this.Font = Globals.Settings.DefaultFont;
            this.pluginGuid = pluginGuid;
            this.HideOnClose = true;
            this.Controls.Add(ctrl);
            Globals.MainForm.ThemeControls(this);
            this.Show();
        }

        /// <summary>
        /// Gets or sets the icon for the <see cref="DockablePanel"/>
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                RefreshIcon();
            }
        }

        /// <summary>
        /// Refreshes the icon.
        /// </summary>
        public void RefreshIcon()
        {
            if (image != null) Icon = ImageKonverter.ImageToIcon(image);
        }

        /// <summary>
        /// Retrieves the guid of the document
        /// </summary>
        public override String GetPersistString()
        {
            return this.pluginGuid;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (this.IsAutoHidden)
                {
                    this.DockPanel.ActiveAutoHideContent = null;
                    return true;
                }
                else if (this.IsFloat)
                {
                    if (this.HideOnClose)
                    {
                        this.Hide();
                    }
                    else
                    {
                        this.Close();
                    }
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private bool IsAutoHidden
        {
            get
            {
                switch (this.DockState)
                {
                    case DockState.DockTopAutoHide:
                    case DockState.DockLeftAutoHide:
                    case DockState.DockRightAutoHide:
                    case DockState.DockBottomAutoHide:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }

}