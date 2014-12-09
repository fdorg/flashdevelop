﻿using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FlashDebugger.Properties;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Managers;

namespace FlashDebugger
{
    internal class MenusHelper
    {
        static public ImageList imageList;
        private ToolStripItem[] m_ToolStripButtons;
        private ToolStripSeparator m_ToolStripSeparator;
		private ToolStripButton StartContinueButton, PauseButton, StopButton, CurrentButton, RunToCursorButton, StepButton, NextButton, FinishButton;
        private ToolStripMenuItem StartContinueMenu, PauseMenu, StopMenu, CurrentMenu, RunToCursorMenu, StepMenu, NextMenu, FinishMenu, ToggleBreakPointMenu, ToggleBreakPointEnableMenu, DeleteAllBreakPointsMenu, DisableAllBreakPointsMenu, EnableAllBreakPointsMenu, StartRemoteDebuggingMenu;
        private ToolStripMenuItem BreakOnAllMenu;
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
            imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.Images.Add("StartContinue", PluginBase.MainForm.ImageSetAdjust(Resource.StartContinue));
			imageList.Images.Add("Pause", PluginBase.MainForm.ImageSetAdjust(Resource.Pause));
			imageList.Images.Add("Stop", PluginBase.MainForm.ImageSetAdjust(Resource.Stop));
			imageList.Images.Add("Current", PluginBase.MainForm.ImageSetAdjust(Resource.Current));
			imageList.Images.Add("RunToCursor", PluginBase.MainForm.ImageSetAdjust(Resource.RunToCursor));
			imageList.Images.Add("Step", PluginBase.MainForm.ImageSetAdjust(Resource.Step));
			imageList.Images.Add("Next", PluginBase.MainForm.ImageSetAdjust(Resource.Next));
            imageList.Images.Add("Finish", PluginBase.MainForm.ImageSetAdjust(Resource.Finish));

            ToolStripMenuItem tempItem;
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewBreakpointsPanel"), pluginImage, OpenBreakPointPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowBreakpoints", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewLocalVariablesPanel"), pluginImage, OpenLocalVariablesPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowLocalVariables", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewStackframePanel"), pluginImage, OpenStackframePanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowStackframe", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewWatchPanel"), pluginImage, OpenWatchPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowWatch", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
            tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewImmediatePanel"), pluginImage, OpenImmediatePanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowImmediate", tempItem);
            viewMenu.DropDownItems.Add(tempItem);
			tempItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewThreadsPanel"), pluginImage, OpenThreadsPanel);
			PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowThreads", tempItem);
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

			StartContinueMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], StartContinue_Click, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Start", StartContinueMenu);
            PauseMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], debugManager.Pause_Click, Keys.Control | Keys.Shift | Keys.F5);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Pause", PauseMenu);
            StopMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], debugManager.Stop_Click, Keys.Shift | Keys.F5);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Stop", StopMenu);
            BreakOnAllMenu = new ToolStripMenuItem(TextHelper.GetString("Label.BreakOnAllErrors"), null, BreakOnAll_Click, Keys.Control | Keys.Alt | Keys.E);
            BreakOnAllMenu.Checked = PluginMain.settingObject.BreakOnThrow;
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.BreakOnAllErrors", BreakOnAllMenu);
            CurrentMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Current"), imageList.Images["Current"], debugManager.Current_Click, Keys.Shift | Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.Current", CurrentMenu);
            RunToCursorMenu = new ToolStripMenuItem(TextHelper.GetString("Label.RunToCursor"), imageList.Images["RunToCursor"], ScintillaHelper.RunToCursor_Click, Keys.Control | Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.RunToCursor", RunToCursorMenu);
            StepMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Step"), imageList.Images["Step"], debugManager.Step_Click, Keys.F11);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepInto", StepMenu);
            NextMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Next"), imageList.Images["Next"], debugManager.Next_Click, Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepOver", NextMenu);
            FinishMenu = new ToolStripMenuItem(TextHelper.GetString("Label.Finish"), imageList.Images["Finish"], debugManager.Finish_Click, Keys.Shift | Keys.F11);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StepOut", FinishMenu);

            ToggleBreakPointMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpoint"), null, ScintillaHelper.ToggleBreakPoint_Click, Keys.F9);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpoint", ToggleBreakPointMenu);
            DeleteAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DeleteAllBreakpoints"), null, ScintillaHelper.DeleteAllBreakPoints_Click, Keys.Control | Keys.Shift | Keys.F9);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DeleteAllBreakpoints", DeleteAllBreakPointsMenu);
            ToggleBreakPointEnableMenu = new ToolStripMenuItem(TextHelper.GetString("Label.ToggleBreakpointEnabled"), null, ScintillaHelper.ToggleBreakPointEnable_Click, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.ToggleBreakpointEnabled", ToggleBreakPointEnableMenu);
            DisableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.DisableAllBreakpoints"), null, ScintillaHelper.DisableAllBreakPoints_Click, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.DisableAllBreakpoints", DisableAllBreakPointsMenu);
            EnableAllBreakPointsMenu = new ToolStripMenuItem(TextHelper.GetString("Label.EnableAllBreakpoints"), null, ScintillaHelper.EnableAllBreakPoints_Click, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.EnableAllBreakpoints", EnableAllBreakPointsMenu);

            StartRemoteDebuggingMenu = new ToolStripMenuItem(TextHelper.GetString("Label.StartRemoteDebugging"), null, StartRemote_Click, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("DebugMenu.StartRemoteDebugging", StartRemoteDebuggingMenu);

            debugItems = new List<ToolStripItem>(new ToolStripItem[]
			{
				StartContinueMenu, PauseMenu, StopMenu, BreakOnAllMenu, new ToolStripSeparator(),
				CurrentMenu, RunToCursorMenu, StepMenu, NextMenu, FinishMenu, new ToolStripSeparator(),
				ToggleBreakPointMenu, DeleteAllBreakPointsMenu, ToggleBreakPointEnableMenu ,DisableAllBreakPointsMenu, EnableAllBreakPointsMenu, new ToolStripSeparator(),
				StartRemoteDebuggingMenu
            });

            debugMenu.DropDownItems.AddRange(debugItems.ToArray());

            // ToolStrip
            m_ToolStripSeparator = new ToolStripSeparator();
            m_ToolStripSeparator.Margin = new Padding(1, 0, 0, 0);
            StartContinueButton = new ToolStripButton(TextHelper.GetString("Label.Start"), imageList.Images["StartContinue"], StartContinue_Click);
			StartContinueButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.Start", StartContinueButton);
			PauseButton = new ToolStripButton(TextHelper.GetString("Label.Pause"), imageList.Images["Pause"], debugManager.Pause_Click);
			PauseButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.Pause", PauseButton);
			StopButton = new ToolStripButton(TextHelper.GetString("Label.Stop"), imageList.Images["Stop"], debugManager.Stop_Click);
			StopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.Stop", StopButton);
			CurrentButton = new ToolStripButton(TextHelper.GetString("Label.Current"), imageList.Images["Current"], debugManager.Current_Click);
			CurrentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.Current", CurrentButton);
			RunToCursorButton = new ToolStripButton(TextHelper.GetString("Label.RunToCursor"), imageList.Images["RunToCursor"], ScintillaHelper.RunToCursor_Click);
			RunToCursorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.RunToCursor", RunToCursorButton);
			StepButton = new ToolStripButton(TextHelper.GetString("Label.Step"), imageList.Images["Step"], debugManager.Step_Click);
            StepButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.StepInto", StepButton);
			NextButton = new ToolStripButton(TextHelper.GetString("Label.Next"), imageList.Images["Next"], debugManager.Next_Click);
            NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.StepOver", NextButton);
            FinishButton = new ToolStripButton(TextHelper.GetString("Label.Finish"), imageList.Images["Finish"], debugManager.Finish_Click);
            FinishButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginBase.MainForm.RegisterSecondaryItem("DebugMenu.StepOut", FinishButton);
            m_ToolStripButtons = new ToolStripItem[] { m_ToolStripSeparator, StartContinueButton, PauseButton, StopButton, new ToolStripSeparator(), CurrentButton, RunToCursorButton, StepButton, NextButton, FinishButton };
            
            // Events
            PluginMain.debugManager.StateChangedEvent += UpdateMenuState;
            PluginMain.settingObject.BreakOnThrowChanged += BreakOnThrowChanged;
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

		public void OpenImmediatePanel(Object sender, System.EventArgs e)
		{
			PanelsHelper.immediatePanel.Show();
		}

		public void OpenThreadsPanel(Object sender, System.EventArgs e)
		{
			PanelsHelper.threadsPanel.Show();
		}

        private void BreakOnAll_Click(Object sender, EventArgs e)
        {
            PluginMain.settingObject.BreakOnThrow = !BreakOnAllMenu.Checked;
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
            else PluginMain.debugManager.Start(/*false*/);
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

        private void BreakOnThrowChanged(object sender, EventArgs e)
        {
            BreakOnAllMenu.Checked = PluginMain.settingObject.BreakOnThrow;
        }

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
            Boolean hasChanged = CurrentState != state;
            CurrentState = state; // Set current now...
			if (state == DebuggerState.Initializing || state == DebuggerState.Stopped)
			{
                if (PluginMain.settingObject.StartDebuggerOnTestMovie) StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
                else StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Start");
			}
			else StartContinueButton.Text = StartContinueMenu.Text = TextHelper.GetString("Label.Continue");
            PluginBase.MainForm.ApplySecondaryShortcut(StartContinueButton);
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
            enabled = /*(state != DebuggerState.Running) &&*/ GetLanguageIsValid();
            ToggleBreakPointMenu.Enabled = ToggleBreakPointEnableMenu.Enabled = enabled;
            DeleteAllBreakPointsMenu.Enabled = DisableAllBreakPointsMenu.Enabled = enabled;
            EnableAllBreakPointsMenu.Enabled = PanelsHelper.breakPointUI.Enabled = enabled;
			StartRemoteDebuggingMenu.Enabled = (state == DebuggerState.Initializing || state == DebuggerState.Stopped);
            // Notify plugins of main states when state changes...
            if (hasChanged && (state == DebuggerState.Running || state == DebuggerState.Stopped))
            {
                DataEvent de = new DataEvent(EventType.Command, "FlashDebugger." + state.ToString(), null);
                EventManager.DispatchEvent(this, de);
            }
            PluginBase.MainForm.RefreshUI();
        }

        /// <summary>
        /// Gets if the language is valid for debugging
        /// </summary>
        private Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                String ext = Path.GetExtension(document.FileName);
                String lang = document.SciControl.ConfigurationLanguage;
                return (lang == "as3" || lang == "haxe" || ext == ".mxml");
            }
            else return false;
        }

        #endregion

    }

}
