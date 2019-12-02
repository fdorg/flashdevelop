// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore.Helpers;
using PluginCore.Utilities;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Docking
{
    public class DockablePanel : DockContent
    {
        Image image;
        readonly string pluginGuid;

        public DockablePanel(Control ctrl, string pluginGuid)
        {
            Text = ctrl.Text;
            ctrl.Dock = DockStyle.Fill;
            DockPanel = PluginBase.MainForm.DockPanel;
            if (ctrl.Tag != null) TabText = ctrl.Tag.ToString();
            DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Float;
            // Restrict docking on Wine/CrossOver as you can't dock panels back if undocked...
            DockPanel.AllowEndUserFloatChange = !PlatformHelper.isRunningOnWine();
            DockPanel.AllowEndUserDocking = !PlatformHelper.isRunningOnWine();
            Font = PluginBase.MainForm.Settings.DefaultFont;
            this.pluginGuid = pluginGuid;
            HideOnClose = true;
            Controls.Add(ctrl);
            PluginBase.MainForm.ThemeControls(this);
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
        public override string GetPersistString() => pluginGuid;

        internal class Template : DockContent
        {
            readonly string persistString;

            internal Template(string persistString)
            {
                this.persistString = persistString;
            }

            public override string GetPersistString() => persistString;
        }
    }
}