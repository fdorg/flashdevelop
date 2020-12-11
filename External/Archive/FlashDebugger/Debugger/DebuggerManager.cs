// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flash.Tools.Debugger;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;
using ProjectManager.Projects.AS3;
using ScintillaNet;
using PluginCore;

namespace FlashDebugger
{
    public delegate void StateChangedEventHandler(object sender, DebuggerState state);

	public enum DebuggerState
	{
		Initializing,
		Stopped,
		Starting,
		Running,
		Pausing,
		PauseHalt,
		BreakHalt,
		ExceptionHalt
	}

	public class DebuggerManager
    {
        public event StateChangedEventHandler StateChangedEvent;

        internal Project currentProject;
		private BackgroundWorker bgWorker;
        private FlashInterface m_FlashInterface;
		private Location m_CurrentLocation = null;
		private Dictionary<String, String> m_PathMap = new Dictionary<String, String>();
        private Int32 m_CurrentFrame = 0;
        private Boolean useAny = false;

        public DebuggerManager()
        {
			m_FlashInterface = new FlashInterface();
			m_FlashInterface.m_BreakPointManager = PluginMain.breakPointManager;
			m_FlashInterface.StartedEvent += new DebuggerEventHandler(flashInterface_StartedEvent);
			m_FlashInterface.DisconnectedEvent += new DebuggerEventHandler(flashInterface_DisconnectedEvent);
			m_FlashInterface.BreakpointEvent += new DebuggerEventHandler(flashInterface_BreakpointEvent);
			m_FlashInterface.FaultEvent += new DebuggerEventHandler(flashInterface_FaultEvent);
			m_FlashInterface.PauseEvent += new DebuggerEventHandler(flashInterface_PauseEvent);
			m_FlashInterface.StepEvent += new DebuggerEventHandler(flashInterface_StepEvent);
			m_FlashInterface.ScriptLoadedEvent += new DebuggerEventHandler(flashInterface_ScriptLoadedEvent);
			m_FlashInterface.WatchpointEvent += new DebuggerEventHandler(flashInterface_WatchpointEvent);
			m_FlashInterface.UnknownHaltEvent += new DebuggerEventHandler(flashInterface_UnknownHaltEvent);
            m_FlashInterface.ProgressEvent += new DebuggerProgressEventHandler(flashInterface_ProgressEvent);
        }

        #region Startup

        public bool Start(Boolean useAny)
        {
            this.useAny = useAny;
            if (!CheckCurrent()) return false;
            PluginMain.debugBuildStart = true;
            UpdateMenuState(DebuggerState.Starting);
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(currentProject.ProjectPath), currentProject.OutputPath)))
            {
                if ((currentProject.OutputType == OutputType.Application)
                    || currentProject.TestMovieBehavior == TestMovieBehavior.Custom 
                    || currentProject.TestMovieBehavior == TestMovieBehavior.OpenDocument)
                {
                    // Wait for a SWF to connect...
                    Start(null);
                }
                else ErrorManager.ShowWarning(TextHelper.GetString("Info.CannotFindOutputFile"), null);
            }
            else Start(currentProject.OutputPathAbsolute);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CheckCurrent()
        {
            try
            {
                IProject project = PluginBase.CurrentProject;
                if (project == null || !project.EnableInteractiveDebugger) return false;
                ProjectReader reader = new ProjectReader(project.ProjectPath, new AS3Project(project.ProjectPath));
                currentProject = reader.ReadProject();
                // Give a console warning for non external player...
                if (currentProject.TestMovieBehavior == TestMovieBehavior.NewTab || currentProject.TestMovieBehavior == TestMovieBehavior.NewWindow)
                {
                    TraceManager.Add(TextHelper.GetString("Info.CannotDebugActiveXPlayer"));
					return false;
                }
            }
            catch (Exception e) 
            { 
                ErrorManager.ShowError(e);
                return false;
            }
			return true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Start(String filename)
        {
            PluginMain.debugBuildStart = false;
			m_FlashInterface.currentProject = currentProject;
			m_FlashInterface.outputFileFullPath = filename;
            PluginBase.MainForm.ProgressBar.Visible = true;
            PluginBase.MainForm.ProgressLabel.Visible = true;
            PluginBase.MainForm.ProgressLabel.Text = TextHelper.GetString("Info.WaitingForPlayer");
            if (bgWorker == null || !bgWorker.IsBusy)
            {
                // only run a debugger if one is not already runnin - need to redesign core to support multiple debugging instances
                // other option: detach old worker, wait for it to exit and start new one
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                m_FlashInterface.Start(this.useAny);
            }
            catch (Exception ex)
            {
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					ErrorManager.ShowError("Internal Debugger Exception", ex);
				});
            }
			m_PathMap.Clear();
        }

        #endregion

        #region Properties

        public FlashInterface FlashInterface
        {
            get { return m_FlashInterface; }
        }

        public int CurrentFrame
        {
            get { return m_CurrentFrame; }
            set
            {
                if (m_CurrentFrame != value)
                {
                    m_CurrentFrame = value;
                    UpdateLocalsUI();
                }
            }
        }

        public Location CurrentLocation
        {
            get { return m_CurrentLocation; }
            set
            {
                if (m_CurrentLocation != value)
                {
                    if (m_CurrentLocation != null)
                    {
                        ResetCurrentLocation();
                    }
                    m_CurrentLocation = value;
                    if (m_CurrentLocation != null)
                    {
                        GotoCurrentLocation(true);
                    }
                }
            }
        }

        #endregion

        #region PathHelpers

        /// <summary>
        /// 
        /// </summary>
		public String GetLocalPath(SourceFile file)
		{
			if (File.Exists(file.FullPath))
			{
				return file.FullPath;
			}
			if (m_PathMap.ContainsKey(file.FullPath))
			{
				return m_PathMap[file.FullPath];
			}
			Char pathSeparator = Path.DirectorySeparatorChar;
			foreach (Folder folder in PluginMain.settingObject.SourcePaths)
			{
				String pathFromPackage = file.getPackageName().Replace('/', pathSeparator);
                String localPath = folder.Path + pathSeparator + pathFromPackage + pathSeparator + file.Name;
				if (File.Exists(localPath))
				{
					m_PathMap[file.FullPath] = localPath;
					return localPath;
				}
			}
			return null;
        }

        #endregion

        #region FlashInterface Control

        /// <summary>
        /// 
        /// </summary>
        public void Cleanup()
        {
			m_PathMap.Clear();
			m_FlashInterface.Cleanup();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateMenuState(DebuggerState state)
        {
            if (StateChangedEvent != null) StateChangedEvent(this, state);
        }

        #endregion

        #region FlashInterface Events
        
        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_StartedEvent(object sender)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					flashInterface_StartedEvent(sender);
				});
				return;
			}
			UpdateMenuState(DebuggerState.Running);
            PluginBase.MainForm.ProgressBar.Visible = false;
            PluginBase.MainForm.ProgressLabel.Visible = false;
			if (!PluginMain.settingObject.DisablePanelsAutoshow)
			{
                PanelsHelper.watchPanel.Show();
                PanelsHelper.stackframePanel.Show();
                PanelsHelper.pluginPanel.Show();
				PanelsHelper.breakPointPanel.Show();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_DisconnectedEvent(object sender)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					flashInterface_DisconnectedEvent(sender);
				});
				return;
			}
			CurrentLocation = null;
			UpdateMenuState(DebuggerState.Stopped);
            if (!PluginMain.settingObject.DisablePanelsAutoshow)
            {
                PanelsHelper.pluginPanel.Hide();
                PanelsHelper.breakPointPanel.Hide();
                PanelsHelper.stackframePanel.Hide();
                PanelsHelper.watchPanel.Hide();
            }
			PanelsHelper.pluginUI.TreeControl.Nodes.Clear();
			PanelsHelper.stackframeUI.ClearItem();
			PanelsHelper.watchUI.Clear();
			PluginMain.breakPointManager.ResetAll();
            PluginBase.MainForm.ProgressBar.Visible = false;
            PluginBase.MainForm.ProgressLabel.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_BreakpointEvent(object sender)
		{
			Location loc = FlashInterface.getCurrentLocation();
			if (PluginMain.breakPointManager.ShouldBreak(loc.File, loc.Line))
			{
				UpdateUI(DebuggerState.BreakHalt);
			}
			else
			{
				FlashInterface.StepResume();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_FaultEvent(object sender)
		{
			UpdateUI(DebuggerState.ExceptionHalt);
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_StepEvent(object sender)
		{
			UpdateUI(DebuggerState.BreakHalt);
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_ScriptLoadedEvent(object sender)
		{
			m_FlashInterface.UpdateBreakpoints(PluginMain.breakPointManager.GetBreakPointUpdates());
			m_FlashInterface.Continue();
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_WatchpointEvent(object sender)
		{
			UpdateUI(DebuggerState.BreakHalt);
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_UnknownHaltEvent(object sender)
		{
			UpdateUI(DebuggerState.ExceptionHalt);
		}

		/// <summary>
		/// 
		/// </summary>
		private void flashInterface_PauseEvent(Object sender)
		{
			UpdateUI(DebuggerState.PauseHalt);
		}

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUI(DebuggerState state)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					UpdateUI(state);
				});
				return;
			}
			CurrentLocation = FlashInterface.getCurrentLocation();
			UpdateStackUI();
			UpdateLocalsUI();
			UpdateMenuState(state);
			(PluginBase.MainForm as Form).Activate();
		}

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStackUI()
		{
			m_CurrentFrame = 0;
			Frame[] frames = m_FlashInterface.GetFrames();
			PanelsHelper.stackframeUI.AddFrames(frames);
		}

        /// <summary>
        /// 
        /// </summary>
		private void UpdateLocalsUI()
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					UpdateLocalsUI();
				});
				return;
			}
			Frame[] frames = m_FlashInterface.GetFrames();
            if (frames != null && m_CurrentFrame < frames.Length)
			{
				Variable thisValue = m_FlashInterface.GetThis(m_CurrentFrame);
				Variable[] args = m_FlashInterface.GetArgs(m_CurrentFrame);
				Variable[] locals = m_FlashInterface.GetLocals(m_CurrentFrame);
				PanelsHelper.pluginUI.Clear();
				if (thisValue != null)
				{
					PanelsHelper.pluginUI.SetData(new Variable[] { thisValue });
				}
				if (args != null && args.Length > 0)
				{
					PanelsHelper.pluginUI.SetData(args);
				}
				if (locals != null && locals.Length > 0)
				{
					PanelsHelper.pluginUI.SetData(locals);
				}
				CurrentLocation = frames[m_CurrentFrame].Location;
				PanelsHelper.watchUI.UpdateElements();
			}
			else CurrentLocation = null;
		}

        /// <summary>
        /// 
        /// </summary>
        private void ResetCurrentLocation()
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					ResetCurrentLocation();
				});
				return;
			}
			if (CurrentLocation.File != null)
			{
				ScintillaControl sci;
				String localPath = GetLocalPath(CurrentLocation.File);
				if (localPath != null)
				{
					sci = ScintillaHelper.GetScintillaControl(localPath);
					if (sci != null)
					{
						Int32 i = ScintillaHelper.GetScintillaControlIndex(sci);
						if (i != -1)
						{
							Int32 line = CurrentLocation.Line - 1;
							sci.MarkerDelete(line, ScintillaHelper.markerCurrentLine);
						}
					}
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        private void GotoCurrentLocation(bool bSetMarker)
		{
			if ((PluginBase.MainForm as Form).InvokeRequired)
			{
				(PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
				{
					GotoCurrentLocation(bSetMarker);
				});
				return;
			}
			if (CurrentLocation != null && CurrentLocation.File != null)
			{
				ScintillaControl sci;
				String localPath = GetLocalPath(CurrentLocation.File);
				if (localPath != null)
				{
					sci = ScintillaHelper.GetScintillaControl(localPath);
					if (sci == null)
					{
						PluginBase.MainForm.OpenEditableDocument(localPath);
						sci = ScintillaHelper.GetScintillaControl(localPath);
					}
					if (sci != null)
					{
						Int32 i = ScintillaHelper.GetScintillaControlIndex(sci);
						if (i != -1)
						{
							PluginBase.MainForm.Documents[i].Activate();
							Int32 line = CurrentLocation.Line - 1;
							sci.GotoLine(line);
							if (bSetMarker)
							{
								sci.MarkerAdd(line, ScintillaHelper.markerCurrentLine);
							}
						}
					}
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_PauseNotRespondEvent(object sender)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
                {
                    flashInterface_PauseNotRespondEvent(sender);
                });
                return;
            }
            DialogResult res = MessageBox.Show(PluginBase.MainForm, TextHelper.GetString("Title.CloseProcess"), TextHelper.GetString("Info.ProcessNotResponding"), MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            if (res == DialogResult.OK)
            {
				m_FlashInterface.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_TraceEvent(Object sender, String trace)
        {
            TraceManager.AddAsync(trace, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_ProgressEvent(object sender, int current, int total)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate()
                {
                    flashInterface_ProgressEvent(sender, current, total);
                });
                return;
            }
            PluginBase.MainForm.ProgressBar.Maximum = total;
            PluginBase.MainForm.ProgressBar.Value = current;
        }

        #endregion

        #region MenuItem Event Handling

        /// <summary>
        /// 
        /// </summary>
        internal void Stop_Click(Object sender, EventArgs e)
        {
            PluginMain.liveDataTip.Hide();
			CurrentLocation = null;
			m_FlashInterface.Stop();
        }

		/// <summary>
		/// 
		/// </summary>
		internal void Current_Click(Object sender, EventArgs e)
		{
			if (m_FlashInterface.isDebuggerStarted && m_FlashInterface.isDebuggerSuspended)
			{
				GotoCurrentLocation(false);
			}
		}

		/// <summary>
        /// 
        /// </summary>
        internal void Next_Click(Object sender, EventArgs e)
        {
			CurrentLocation = null;
			m_FlashInterface.Next();
			UpdateMenuState(DebuggerState.Running);
		}

        /// <summary>
        /// 
        /// </summary>
        internal void Step_Click(Object sender, EventArgs e)
        {
			CurrentLocation = null;
			m_FlashInterface.Step();
			UpdateMenuState(DebuggerState.Running);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Continue_Click(Object sender, EventArgs e)
        {
            try
            {
				CurrentLocation = null;
				m_FlashInterface.UpdateBreakpoints(PluginMain.breakPointManager.GetBreakPointUpdates());
				m_FlashInterface.Continue();
				UpdateMenuState(DebuggerState.Running);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Pause_Click(Object sender, EventArgs e)
        {
			CurrentLocation = null;
			m_FlashInterface.Pause();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Finish_Click(Object sender, EventArgs e)
        {
			CurrentLocation = null;
			m_FlashInterface.Finish();
			UpdateMenuState(DebuggerState.Running);
		}

        #endregion

    }

}
