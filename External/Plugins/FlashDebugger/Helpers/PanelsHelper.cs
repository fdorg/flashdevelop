using System.Drawing;
using FlashDebugger.Controls;
using PluginCore;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace FlashDebugger
{
    class PanelsHelper
    {
        public static string localsGuid = "f9d8faf1-31f7-45ca-9c14-2cad27d7a19e";
        public static DockContent localsPanel;
        public static LocalsUI localsUI;
        public static string breakPointGuid = "6ee0f809-a3f7-4365-96c7-3bdf89f3aaa4";
        public static DockContent breakPointPanel;
        public static BreakPointUI breakPointUI;

        public static string stackframeGuid = "49eb0b7e-f601-4860-a190-ae48e122a661";
        public static DockContent stackframePanel;
        public static StackframeUI stackframeUI;

        public static string watchGuid = "c4a0c90a-236c-4a41-8ece-85486a23960f";
        public static DockContent watchPanel;
        public static WatchUI watchUI;

        public static string immediateGuid = "7ce31ab1-5a40-42d5-ad32-5df567707afc";
        public static DockContent immediatePanel;
        public static ImmediateUI immediateUI;

        public static string threadsGuid = "49eb0b7e-f601-4860-a190-ae48e122a662";
        public static DockContent threadsPanel;
        public static ThreadsUI threadsUI;

        public PanelsHelper(Image pluginImage)
        {
            localsUI = new LocalsUI {Text = TextHelper.GetString("Title.LocalVariables")};
            localsPanel = PluginBase.MainForm.CreateDockablePanel(localsUI, localsGuid, pluginImage, DockState.DockLeft);
            localsPanel.Hide();

            stackframeUI = new StackframeUI(MenusHelper.imageList) {Text = TextHelper.GetString("Title.StackTrace")};
            stackframePanel = PluginBase.MainForm.CreateDockablePanel(stackframeUI, stackframeGuid, pluginImage, DockState.DockLeft);
            stackframePanel.Hide();

            watchUI = new WatchUI(PluginMain.watchManager) {Text = TextHelper.GetString("Title.Watch")};
            watchPanel = PluginBase.MainForm.CreateDockablePanel(watchUI, watchGuid, pluginImage, DockState.DockLeft);
            watchPanel.Hide();

            breakPointUI = new BreakPointUI(PluginMain.breakPointManager)
            {
                Text = TextHelper.GetString("Title.Breakpoints")
            };
            breakPointPanel = PluginBase.MainForm.CreateDockablePanel(breakPointUI, breakPointGuid, pluginImage, DockState.DockLeft);
            breakPointPanel.Hide();

            immediateUI = new ImmediateUI {Text = TextHelper.GetString("Title.Immediate")};
            immediatePanel = PluginBase.MainForm.CreateDockablePanel(immediateUI, immediateGuid, pluginImage, DockState.DockLeft);
            immediatePanel.Hide();

            threadsUI = new ThreadsUI(MenusHelper.imageList) {Text = TextHelper.GetString("Title.Threads")};
            threadsPanel = PluginBase.MainForm.CreateDockablePanel(threadsUI, threadsGuid, pluginImage, DockState.DockLeft);
            threadsPanel.Hide();
        }

        /// <summary>
        /// Docks all panels into a group
        /// </summary>
        public void DockTogether()
        {
            if (watchPanel?.Pane is null) return;
            localsPanel.DockTo(watchPanel.Pane, DockStyle.Fill, -1);
            stackframePanel.DockTo(watchPanel.Pane, DockStyle.Fill, -1);
            immediatePanel.DockTo(watchPanel.Pane, DockStyle.Fill, -1);
            threadsPanel.DockTo(watchPanel.Pane, DockStyle.Fill, -1);
            breakPointPanel.DockTo(watchPanel.Pane, DockStyle.Fill, -1);
        }
    }
}