using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Docking
{
    public class DockablePanel : DockContent
    {
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
        /// Retrieves the guid of the document
        /// </summary>
        public override String GetPersistString()
        {
            return this.pluginGuid;
        }

    }

}