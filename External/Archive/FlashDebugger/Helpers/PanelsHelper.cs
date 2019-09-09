// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Windows.Forms;
using FlashDebugger.Controls;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore;

namespace FlashDebugger
{
    internal class PanelsHelper
    {
		static public String pluginGuid = "f9d8faf1-31f7-45ca-9c14-2cad27d7a19e";
        static public DockContent pluginPanel;
        static public PluginUI pluginUI;

		static public String breakPointGuid = "6ee0f809-a3f7-4365-96c7-3bdf89f3aaa4";
        static public DockContent breakPointPanel;
        static public BreakPointUI breakPointUI;

		static public String stackframeGuid = "49eb0b7e-f601-4860-a190-ae48e122a661";
        static public DockContent stackframePanel;
        static public StackframeUI stackframeUI;

		static public String watchGuid = "c4a0c90a-236c-4a41-8ece-85486a23960f";
		static public DockContent watchPanel;
		static public WatchUI watchUI;

        public PanelsHelper(PluginMain pluginMain, Image pluginImage)
        {
            pluginUI = new PluginUI(pluginMain);
            pluginUI.Text = TextHelper.GetString("Title.LocalVariables");
            pluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, pluginGuid, pluginImage, DockState.DockLeft);
            pluginPanel.Hide();
            
            stackframeUI = new StackframeUI(pluginMain, MenusHelper.imageList);
            stackframeUI.Text = TextHelper.GetString("Title.StackTrace");
            stackframePanel = PluginBase.MainForm.CreateDockablePanel(stackframeUI, stackframeGuid, pluginImage, DockState.DockLeft);
            stackframePanel.Hide();

			watchUI = new WatchUI();
			watchUI.Text = TextHelper.GetString("Title.Watch");
            watchPanel = PluginBase.MainForm.CreateDockablePanel(watchUI, watchGuid, pluginImage, DockState.DockLeft);
            watchPanel.Hide();

            breakPointUI = new BreakPointUI(pluginMain, PluginMain.breakPointManager);
            breakPointUI.Text = TextHelper.GetString("Title.Breakpoints");
            breakPointPanel = PluginBase.MainForm.CreateDockablePanel(breakPointUI, breakPointGuid, pluginImage, DockState.DockLeft);
            breakPointPanel.Hide();
        }

	}

}
