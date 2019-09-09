// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore.Helpers;
using PluginCore.Utilities;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Docking
{
    public class DockablePanel : DockContent
    {
        private Image image;
        private readonly string pluginGuid;

        public DockablePanel(Control ctrl, string pluginGuid)
        {
            this.Text = ctrl.Text;
            ctrl.Dock = DockStyle.Fill;
            this.DockPanel = Globals.MainForm.DockPanel;
            if (ctrl.Tag != null) this.TabText = ctrl.Tag.ToString();
            this.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Float;
            // Restrict docking on Wine/CrossOver as you can't dock panels back if undocked...
            this.DockPanel.AllowEndUserFloatChange = !PlatformHelper.isRunningOnWine();
            this.DockPanel.AllowEndUserDocking = !PlatformHelper.isRunningOnWine();
            this.Font = Globals.Settings.DefaultFont;
            this.pluginGuid = pluginGuid;
            this.HideOnClose = true;
            this.Controls.Add(ctrl);
            Globals.MainForm.ThemeControls(this);
        }

        /// <summary>
        /// Gets or sets the icon for the <see cref="DockablePanel"/>
        /// </summary>
        public Image Image
        {
            get => image;
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
        public override string GetPersistString()
        {
            return this.pluginGuid;
        }
        
        /// <summary>
        /// 
        /// </summary>
        internal class Template : DockContent
        {
            private readonly string persistString;

            internal Template(string persistString)
            {
                this.persistString = persistString;
            }

            public override string GetPersistString()
            {
                return persistString;
            }

        }

    }

}