using System;
using System.Windows.Forms;
using PluginCore;
using SourceControl.Actions;
using SourceControl.Sources;
using SourceControl.Sources.Git;
using SourceControl.Sources.Mercurial;
using SourceControl.Sources.Subversion;

namespace SourceControl.Managers
{
    public class VCManager
    {
        Timer refreshTimer;
        OverlayManager ovManager;

        public VCManager(OverlayManager ovManager)
        {
            this.ovManager = ovManager;

            if (PluginMain.SCSettings.EnableSVN) AddVCManager(new SubversionManager());
            if (PluginMain.SCSettings.EnableGIT) AddVCManager(new GitManager());
            if (PluginMain.SCSettings.EnableHG) AddVCManager(new MercurialManager());

            refreshTimer = new Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
            refreshTimer.Stop();
        }

        public void Dispose()
        {
            ovManager = null;
            refreshTimer.Stop();
        }

        public void AddVCManager(IVCManager manager)
        {
            ProjectWatcher.VCManagers.Add(manager);
            manager.OnChange += Manager_OnChange;
        }

        void Manager_OnChange(IVCManager sender)
        {
            (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate
            {
                refreshTimer.Stop();
                refreshTimer.Start();
            });
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTimer.Stop();
            ovManager.Refresh();
        }
    }
}
