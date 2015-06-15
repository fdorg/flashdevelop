using System;
using System.Drawing;
using System.Windows.Forms;
using FlashDebugger.Controls;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore;
using PluginCore.DockPanelSuite.Helpers;

namespace FlashDebugger
{
    internal class PanelsHelper
    {
        static public String localsGuid = "f9d8faf1-31f7-45ca-9c14-2cad27d7a19e";
        static public DockContent localsPanel;
        static public LocalsUI localsUI;

        static public String breakPointGuid = "6ee0f809-a3f7-4365-96c7-3bdf89f3aaa4";
        static public DockContent breakPointPanel;
        static public BreakPointUI breakPointUI;

        static public String stackframeGuid = "49eb0b7e-f601-4860-a190-ae48e122a661";
        static public DockContent stackframePanel;
        static public StackframeUI stackframeUI;

        static public String watchGuid = "c4a0c90a-236c-4a41-8ece-85486a23960f";
        static public DockContent watchPanel;
        static public WatchUI watchUI;

        static public String immediateGuid = "7ce31ab1-5a40-42d5-ad32-5df567707afc";
        static public DockContent immediatePanel;
        static public ImmediateUI immediateUI;

        static public String threadsGuid = "49eb0b7e-f601-4860-a190-ae48e122a662";
        static public DockContent threadsPanel;
        static public ThreadsUI threadsUI;

        static private PluginMain pluginMain;
        static private bool uisCreated;

        public PanelsHelper(PluginMain pluginMain, Image pluginImage)
        {
            PanelsHelper.pluginMain = pluginMain;   

            var localsStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return localsUI;
            });
            localsStub.Text = TextHelper.GetString("Title.LocalVariables");
            localsPanel = PluginBase.MainForm.CreateDockablePanel(localsStub, localsGuid, pluginImage, DockState.DockLeft);
            localsPanel.Hide();

            var stackframeStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return stackframeUI;
            });
            stackframeStub.Text = TextHelper.GetString("Title.StackTrace");
            stackframePanel = PluginBase.MainForm.CreateDockablePanel(stackframeStub, stackframeGuid, pluginImage, DockState.DockLeft);
            stackframePanel.Hide();

            var watchStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return watchUI;
            });
            watchStub.Text = TextHelper.GetString("Title.Watch");
            watchPanel = PluginBase.MainForm.CreateDockablePanel(watchStub, watchGuid, pluginImage, DockState.DockLeft);
            watchPanel.Hide();

            var breakPointStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return breakPointUI;
            });
            breakPointStub.Text = TextHelper.GetString("Title.Breakpoints");
            breakPointPanel = PluginBase.MainForm.CreateDockablePanel(breakPointStub, breakPointGuid, pluginImage, DockState.DockLeft);
            breakPointPanel.Hide();

            var immediateStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return immediateUI;
            });
            immediateStub.Text = TextHelper.GetString("Title.Immediate");
            immediatePanel = PluginBase.MainForm.CreateDockablePanel(immediateStub, immediateGuid, pluginImage, DockState.DockLeft);
            immediatePanel.Hide();

            var threadsStub = new DelayedDockContent(delegate
            {
                CreateUIs();
                return threadsUI;
            });
            threadsStub.Text = TextHelper.GetString("Title.Threads");
            threadsPanel = PluginBase.MainForm.CreateDockablePanel(threadsStub, threadsGuid, pluginImage, DockState.DockLeft);
            threadsPanel.Hide();
        }

        static internal void CreateUIs()
        {
            if (uisCreated) return;
            uisCreated = true;

            localsUI = new LocalsUI(pluginMain);
            stackframeUI = new StackframeUI(pluginMain, MenusHelper.imageList);
            watchUI = new WatchUI();
            breakPointUI = new BreakPointUI(pluginMain, PluginMain.breakPointManager);
            immediateUI = new ImmediateUI();
            threadsUI = new ThreadsUI(pluginMain, MenusHelper.imageList);
        }

        internal static void ShowAllPanels()
        {
            watchPanel.Show();
            localsPanel.Show();
            threadsPanel.Show();
            immediatePanel.Show();
            breakPointPanel.Show();
            stackframePanel.Show();
        }

        internal static void HideAllPanels()
        {
            localsPanel.Hide();
            breakPointPanel.Hide();
            stackframePanel.Hide();
            watchPanel.Hide();
            immediatePanel.Hide();
            threadsPanel.Hide();
        }

        internal static void ResetUIs()
        {
            if (!uisCreated) return;
            localsUI.TreeControl.Nodes.Clear();
            stackframeUI.ClearItem();
            watchUI.Clear();
            threadsUI.ClearItem();
        }
    }

}
