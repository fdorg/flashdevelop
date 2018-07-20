// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FlashDebugger.Properties;
using PluginCore.Localization;
using PluginCore;

namespace FlashDebugger
{
    internal class MenusHelper
    {
        static public ImageList imageList;
        private ToolStripItem[] m_ToolStripButtons;
        private ToolStripSeparator m_ToolStripSeparator;
		private ToolStripButton StartContinueButton, PauseButton, StopButton, CurrentButton, RunToCursorButton, StepButton, NextButton, FinishButton;
        private ToolStripMenuItem StartContinueMenu, PauseMenu, StopMenu, CurrentMenu, RunToCursorMenu, StepMenu, NextMenu, FinishMenu, ToggleBreakPointMenu, ToggleBreakPointEnableMenu, DeleteAllBreakPointsMenu, DisableAllBreakPointsMenu, EnableAllBreakPointsMenu, StartRemoteDebuggingMenu;
        private DebuggerState CurrentState = DebuggerState.Initializing;
        private List<ToolStripItem> debugItems;
        private Settings settingObject;

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public MenusHelper(Image pluginImage, DebuggerManager debugManager, Settings settings)
        {
            settingObject = settings;

            imageList = new ImageList();
			imageList.Images.Add("StartContinue", Resource.StartContinue);
			imageList.Images.Add("Pause", Resource.Pause);
			imageList.Images.Add("Stop", Resource.Stop);
			imageList.Images.Add("Current", Resource.Current);
			imageList.Images.Add("RunToCursor", Resource.RunToCursor);
			imageList.Images.Add("Step", Resource.Step);
			imageList.Images.Add("Next", Resource.Next);
			imageList.Images.Add("Finish", Resource.Finish);

            ToolStripMenuItem tempItem;
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewBreakpointsPanel"), pluginImage, new EventHandler(this.OpenBreakPointPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowBreakpoints", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewLocalVariablesPanel"), pluginImage, new EventHandler(this.OpenLocalVariablesPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowLocalVariables", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewStackframePanel"), pluginImage, new EventHandler(this.OpenStackframePanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowStackframe", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewWatchPanel"), pluginImage, new EventHandler(this.OpenWatchPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowWatch", tempItem);
            viewMenu.DropDownItems.Add(tempItem);

            // Menu           
            ToolStripMenuItem debugMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("DebugMenu");
            if (debugMenu == null)
            {
                debugMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Debug"));
                ToolStripMenuItem insertMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("InsertMenu");
                Int32 idx = PluginBase.MainForm.MenuStrip.Items.IndexOf(insertMenu);
                if (idx < 0) idx = PluginBase.MainForm.MenuStrip.Items.Count - 1;
                PluginBase.MainForm.MenuStrip.Items.Insert(idx, debugMenu);
            }

			StartContinueMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], new EventHandler(StartContinue_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Start", StartContinueMenu);
            PauseMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], new EventHandler(debugManager.Pause_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Pause", PauseMenu);
            StopMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], new EventHandler(debugManager.Stop_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Stop", StopMenu);
            CurrentMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Current"), imageList.Images["Current"], new EventHandler(debugManager.Current_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Current", CurrentMenu);
            RunToCursorMenu = new ToolStripMenuItem(TextHelper.GetString("Label.RunToCursor"), imageList.Images["RunToCursor"], new EventHandler(ScintillaHelper.RunToCursor_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.RunToCursor", RunToCursorMenu);
            StepMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Step"), imageList.Images["Step"], new EventHandler(debugManager.Step_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepInto", StepMenu);
            NextMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Next"), imageList.Images["Next"], new EventHandler(debugManager.Next_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepOver", NextMenu);
            FinishMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Finish"), imageList.Images["Finish"], new EventHandler(debugManager.Finish_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Finish", FinishMenu);

            ToggleBreakPointMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpoint"), null, new EventHandler(ScintillaHelper.ToggleBreakPoint_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpoint", ToggleBreakPointMenu);
            DeleteAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DeleteAllBreakpoints"), null, new EventHandler(ScintillaHelper.DeleteAllBreakPoints_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DeleteAllBreakpoints", DeleteAllBreakPointsMenu);
            ToggleBreakPointEnableMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpointEnabled"), null, new EventHandler(ScintillaHelper.ToggleBreakPointEnable_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpointEnabled", ToggleBreakPointEnableMenu);
            DisableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DisableAllBreakpoints"), null, new EventHandler(ScintillaHelper.DisableAllBreakPoints_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DisableAllBreakpoints", DisableAllBreakPointsMenu);
            EnableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.EnableAllBreakpoints"), null, new EventHandler(ScintillaHelper.EnableAllBreakPoints_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.EnableAllBreakpoints", EnableAllBreakPointsMenu);

            StartRemoteDebuggingMenu = new ToolStripMenuItem(TextHelper.GetString("Label.StartRemoteDebugging"), null, new EventHandler(StartRemote_Click), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StartRemoteDebugging", StartRemoteDebuggingMenu);

            debugItems = new List<ToolStripItem>(new ToolStripItem[]
			{
				StartContinueMenu, PauseMenu, StopMenu, new ToolStripSeparator(),
				CurrentMenu, RunToCursorMenu, StepMenu, NextMenu, FinishMenu, new ToolStripSeparator(),
				ToggleBreakPointMenu, DeleteAllBreakPointsMenu, ToggleBreakPointEnableMenu ,DisableAllBreakPointsMenu, EnableAllBreakPointsMenu, new ToolStripSeparator(),
				StartRemoteDebuggingMenu
            });

            debugMenu.DropDownItems.AddRange(debugItems.ToArray());

            // ToolStrip
            m_ToolStripSeparator = new ToolStripSeparator();
            m_ToolStripSeparator.Margin = new Padding(1, 0, 0, 0);
            StartContinueButton = new ToolStripButton(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], new EventHandler(StartContinue_Click));
			StartContinueButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			PauseButton = new ToolStripButton(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], new EventHandler(debugManager.Pause_Click));
			PauseButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			StopButton = new ToolStripButton(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], new EventHandler(debugManager.Stop_Click));
			StopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			CurrentButton = new ToolStripButton(TextHelper.GetString("Label.Current"), imageList.Images["Current"], new EventHandler(debugManager.Current_Click));
			CurrentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			RunToCursorButton = new ToolStripButton(TextHelper.GetString("Label.RunToCursor"), imageList.Images["RunToCursor"], new EventHandler(ScintillaHelper.RunToCursor_Click));
			RunToCursorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			StepButton = new ToolStripButton(TextHelper.GetString("Label.Step"), imageList.Images["Step"], new EventHandler(debugManager.Step_Click));
            StepButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			NextButton = new ToolStripButton(TextHelper.GetString("Label.Next"), imageList.Images["Next"], new EventHandler(debugManager.Next_Click));
            NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FinishButton = new ToolStripButton(TextHelper.GetString("Label.Finish"), imageList.Images["Finish"], new EventHandler(debugManager.Finish_Click));
            FinishButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            m_ToolStripButtons = new ToolStripItem[] { m_ToolStripSeparator, StartContinueButton, PauseButton, StopButton, new ToolStripSeparator(), CurrentButton, RunToCursorButton, StepButton, NextButton, FinishButton };
            
            // Events
            PluginMain.debugManager.StateChangedEvent += UpdateMenuState;
        }

        public void AddToolStripItems()
        {
            ToolStrip toolStrip = PluginBase.MainForm.ToolStrip;
            toolStrip.Items.AddRange(m_ToolStripButtons);
        }

        public void OpenLocalVariablesPanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.pluginPanel.Show();
        }

        public void OpenBreakPointPanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.breakPointPanel.Show();
        }

        public void OpenStackframePanel(Object sender, System.EventArgs e)
        {
            PanelsHelper.stackframePanel.Show();
        }

		public void OpenWatchPanel(Object sender, System.EventArgs e)
		{
			PanelsHelper.watchPanel.Show();
		}

		/// <summary>
        /// 
        /// </summary>
        void StartContinue_Click(Object sender, EventArgs e)
        {
            if (PluginMain.debugManager.FlashInterface.isDebuggerStarted)
            {
                PluginMain.debugManager.Continue_Click(sender, e);
            }
            else PluginMain.debugManager.Start(false);
		}

        /// <summary>
        /// 
        /// </summary>
        void StartRemote_Click(Object sender, EventArgs e)
        {
            if (PluginMain.debugManager.FlashInterface.isDebuggerStarted)
            {
                PluginMain.debugManager.Continue_Click(sender, e);
            }
            else PluginMain.debugManager.Start(true);
        }

		#region Menus State Management

		/// <summary>
        /// 
        /// </summary>
        public void UpdateMenuState(object sender)
        {
            UpdateMenuState(sender, CurrentState);
        }
        public void UpdateMenuState(object sender, DebuggerState state)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
                {
                    UpdateMenuState(sender, state);
                });
                return;
            }
            CurrentState = state; // Set current now...
			if (state == DebuggerState.Initializing || state == DebuggerState.Stopped)
			{
                if (PluginMain.settingObject.StartDebuggerOnTestMovie) StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
                else StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Start");
			}
			else StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
            //
			StopButton.Enabled = StopMenu.Enabled = (state != DebuggerState.Initializing && state != DebuggerState.Stopped);
            PauseButton.Enabled = PauseMenu.Enabled = (state == DebuggerState.Running);
            //
			if (state == DebuggerState.Initializing || state == DebuggerState.Stopped)
			{
                if (PluginMain.settingObject.StartDebuggerOnTestMovie) StartContinueButton.Enabled = StartContinueMenu.Enabled = false;
                else StartContinueButton.Enabled = StartContinueMenu.Enabled = true;
			}
            else if (state == DebuggerState.BreakHalt || state == DebuggerState.ExceptionHalt || state == DebuggerState.PauseHalt)
            {
                StartContinueButton.Enabled = StartContinueMenu.Enabled = true;
            }
			else StartContinueButton.Enabled = StartContinueMenu.Enabled = false;
            //
            Boolean enabled = (state == DebuggerState.BreakHalt || state == DebuggerState.PauseHalt);
            CurrentButton.Enabled = CurrentMenu.Enabled = RunToCursorButton.Enabled = enabled;
            NextButton.Enabled = NextMenu.Enabled = FinishButton.Enabled = FinishMenu.Enabled = enabled;
            RunToCursorMenu.Enabled = StepButton.Enabled = StepMenu.Enabled = enabled;
			if (state == DebuggerState.Running)
			{
				PanelsHelper.pluginUI.TreeControl.Clear();
				PanelsHelper.stackframeUI.ClearItem();
			}
            enabled = (state != DebuggerState.Running);
            ToggleBreakPointMenu.Enabled = ToggleBreakPointEnableMenu.Enabled = DeleteAllBreakPointsMenu.Enabled = enabled;
            DisableAllBreakPointsMenu.Enabled = EnableAllBreakPointsMenu.Enabled = PanelsHelper.breakPointUI.Enabled = enabled;
			StartRemoteDebuggingMenu.Enabled = (state == DebuggerState.Initializing || state == DebuggerState.Stopped);
			PluginBase.MainForm.RefreshUI();
        }

        internal void OnBuildFailed()
        {
            StartContinueMenu.Enabled = true;
        }

        internal void OnBuildComplete()
        {
			StartContinueMenu.Enabled = true;
        }

        #endregion

    }

}
