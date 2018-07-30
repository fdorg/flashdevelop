using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using flash.tools.debugger;
using net.sf.jni4net;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;
using ScintillaNet;

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
        private static bool jvm_up = false;

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
            m_FlashInterface.ThreadsEvent += new DebuggerEventHandler(m_FlashInterface_ThreadsEvent);
        }

        #region Startup

        /// <summary>
        /// 
        /// </summary>
        private bool CheckCurrent()
        {
            try
            {
                IProject project = PluginBase.CurrentProject;
                if (project == null || !project.EnableInteractiveDebugger) return false;
                currentProject = project as Project;

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
        internal bool Start()
        {
            return Start(false);
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool Start(bool alwaysStart)
        {
            if (!alwaysStart && !CheckCurrent()) return false;
            UpdateMenuState(DebuggerState.Starting);

            // load JVM.. only once
            if (!jvm_up)
            {
                try
                {
                    var bridgeSetup = new BridgeSetup(false);

                    string flexSDKPath = null;
                    if (currentProject != null) flexSDKPath = currentProject.CurrentSDK;
                    else flexSDKPath = PathHelper.ResolvePath(PluginBase.MainForm.ProcessArgString("$(FlexSDK)"));

                    if (flexSDKPath != null && Directory.Exists(flexSDKPath))
                    {
                        Dictionary<string, string> jvmConfig = JvmConfigHelper.ReadConfig(flexSDKPath);
                        String javaHome = JvmConfigHelper.GetJavaHome(jvmConfig, flexSDKPath);
                        if (!String.IsNullOrEmpty(javaHome)) bridgeSetup.JavaHome = javaHome;
                    }

                    bridgeSetup.AddAllJarsClassPath(PathHelper.PluginDir);
                    bridgeSetup.AddAllJarsClassPath(Path.Combine(PathHelper.ToolDir, @"flexlibs\lib"));
                    Bridge.CreateJVM(bridgeSetup);
                    Bridge.RegisterAssembly(typeof(IProgress).Assembly); // ??
                    Bridge.RegisterAssembly(typeof(Bootstrap).Assembly);
                    jvm_up = true;
                }
                catch (Exception ex)
                {
                    String msg = "Debugger startup error. For troubleshooting see: http://www.flashdevelop.org/wikidocs/index.php?title=F.A.Q\n";
                    TraceManager.Add(msg + "Error details: " + ex); 
                    return false;
                }
            }

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
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                m_FlashInterface.Start();
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

        #region LayoutHelpers

        /// <summary>
        /// Check if old layout is sotred and restores it. It also deletes this temporary layout file.
        /// </summary>
        public void RestoreOldLayout()
        {
            String oldLayoutFile = Path.Combine(PathHelper.DataDir, "FlashDebugger", "oldlayout.fdl");
            if (File.Exists(oldLayoutFile))
            {
                PluginBase.MainForm.CallCommand("RestoreLayout", oldLayoutFile);
                File.Delete(oldLayoutFile);
            }
        }

        #endregion

        #region PathHelpers

        /// <summary>
        /// 
        /// </summary>
        public String GetLocalPath(SourceFile file)
        {
            if (file == null) return null;
            String fileFullPath = file.getFullPath();
            if (m_PathMap.ContainsKey(fileFullPath))
            {
                return m_PathMap[fileFullPath];
            }
            if (File.Exists(fileFullPath))
            {
                m_PathMap[fileFullPath] = fileFullPath;
                return fileFullPath;
            }
            Char pathSeparator = Path.DirectorySeparatorChar;
            String pathFromPackage = file.getPackageName().ToString().Replace('/', pathSeparator);
            String fileName = file.getName();
            foreach (Folder folder in PluginMain.settingObject.SourcePaths)
            {
                StringBuilder localPathBuilder = new StringBuilder(260/*Windows max path length*/);
                localPathBuilder.Append(folder.Path);
                localPathBuilder.Append(pathSeparator);
                localPathBuilder.Append(pathFromPackage);
                localPathBuilder.Append(pathSeparator);
                localPathBuilder.Append(fileName);
                String localPath = localPathBuilder.ToString();
                if (File.Exists(localPath))
                {
                    m_PathMap[fileFullPath] = localPath;
                    return localPath;
                }
            }
            IProject project = PluginBase.CurrentProject;
            if (project != null)
            {
                var basePaths = project.SourcePaths.Length == 0 ? new[] { Path.GetDirectoryName(project.ProjectPath) } : project.SourcePaths;
                var lookupPaths = basePaths.
                                    Concat(ProjectManager.PluginMain.Settings.GetGlobalClasspaths(project.Language)).
                                    Select(project.GetAbsolutePath).Distinct();
                foreach (string cp in lookupPaths)
                {
                    StringBuilder localPathBuilder = new StringBuilder(260/*Windows max path length*/);
                    localPathBuilder.Append(cp);
                    localPathBuilder.Append(pathSeparator);
                    localPathBuilder.Append(pathFromPackage);
                    localPathBuilder.Append(pathSeparator);
                    localPathBuilder.Append(fileName);
                    String localPath = localPathBuilder.ToString();
                    if (File.Exists(localPath))
                    {
                        m_PathMap[fileFullPath] = localPath;
                        return localPath;
                    }
                }
            }
            m_PathMap[fileFullPath] = null;
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
            if (PluginMain.settingObject.SwitchToLayout != null)
            {
                // save current state
                String oldLayoutFile = Path.Combine(PathHelper.DataDir, "FlashDebugger", "oldlayout.fdl");
                PluginBase.MainForm.DockPanel.SaveAsXml(oldLayoutFile);
                PluginBase.MainForm.CallCommand("RestoreLayout", PluginMain.settingObject.SwitchToLayout);
            }
            else if (!PluginMain.settingObject.DisablePanelsAutoshow)
            {
                PanelsHelper.watchPanel.Show();
                PanelsHelper.localsPanel.Show();
                PanelsHelper.threadsPanel.Show();
                PanelsHelper.immediatePanel.Show();
                PanelsHelper.breakPointPanel.Show();
                PanelsHelper.stackframePanel.Show();
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
            if (PluginMain.settingObject.SwitchToLayout != null)
            {
                RestoreOldLayout();
            }
            else if (!PluginMain.settingObject.DisablePanelsAutoshow)
            {
                PanelsHelper.watchPanel.Hide();
                PanelsHelper.localsPanel.Hide();
                PanelsHelper.threadsPanel.Hide();
                PanelsHelper.immediatePanel.Hide();
                PanelsHelper.breakPointPanel.Hide();
                PanelsHelper.stackframePanel.Hide();
            }
            PanelsHelper.localsUI.TreeControl.Nodes.Clear();
            PanelsHelper.stackframeUI.ClearItem();
            PanelsHelper.watchUI.UpdateElements();
            PanelsHelper.threadsUI.ClearItem();
            PluginMain.breakPointManager.ResetAll();
            PluginBase.MainForm.ProgressBar.Visible = false;
            PluginBase.MainForm.ProgressLabel.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void flashInterface_BreakpointEvent(object sender)
        {
            UpdateUI(DebuggerState.BreakHalt);
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
            // this was moved directly into flashInterface
            // force all breakpoints update after new as code loaded into debug movie 
            //PluginMain.breakPointManager.ForceBreakPointUpdates();
            //m_FlashInterface.UpdateBreakpoints(PluginMain.breakPointManager.GetBreakPointUpdates());
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
        void m_FlashInterface_ThreadsEvent(object sender)
        {
            if (FlashInterface.isDebuggerSuspended)
            {
                // TODO there will be redunandt calls
                UpdateUI(DebuggerState.BreakHalt);
            }
            else
            {
                // TODO should get a signal that thread has changed, keep local number...
                UpdateThreadsUI();
                UpdateMenuState(DebuggerState.Running);
            }
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
            try
            {
                CurrentLocation = FlashInterface.getCurrentLocation();
                UpdateStackUI();
                UpdateLocalsUI();
                UpdateMenuState(state);
                UpdateThreadsUI();
                (PluginBase.MainForm as Form).Activate();
            }
            catch (PlayerDebugException ex)
            {
                ErrorManager.ShowError("Internal Debugger Exception", ex);
            }
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
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)UpdateLocalsUI);
                return;
            }
            Frame[] frames = m_FlashInterface.GetFrames();
            if (frames != null && m_CurrentFrame < frames.Length)
            {
                Variable thisValue = m_FlashInterface.GetThis(m_CurrentFrame);
                Variable[] args = m_FlashInterface.GetArgs(m_CurrentFrame);
                Variable[] locals = m_FlashInterface.GetLocals(m_CurrentFrame);
                if (PanelsHelper.localsUI.TreeControl.Nodes.Count > 0) PanelsHelper.localsUI.TreeControl.SaveState();
                PanelsHelper.localsUI.Clear();
                if (thisValue != null)
                {
                    PanelsHelper.localsUI.SetData(new Variable[] { thisValue });
                }
                if (args != null && args.Length > 0)
                {
                    PanelsHelper.localsUI.SetData(args);
                }
                if (locals != null && locals.Length > 0)
                {
                    PanelsHelper.localsUI.SetData(locals);
                }
                PanelsHelper.localsUI.TreeControl.RestoreState();
                CurrentLocation = frames[m_CurrentFrame].getLocation();
                PanelsHelper.watchUI.UpdateElements();
            }
            else CurrentLocation = null;
        }

        private void UpdateThreadsUI()
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)UpdateThreadsUI);
                return;
            }
            PanelsHelper.threadsUI.SetThreads(m_FlashInterface.IsolateSessions);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetCurrentLocation()
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)ResetCurrentLocation);
                return;
            }
            if (CurrentLocation.getFile() != null)
            {
                ScintillaControl sci;
                String localPath = GetLocalPath(CurrentLocation.getFile());
                if (localPath != null)
                {
                    sci = ScintillaHelper.GetScintillaControl(localPath);
                    if (sci != null)
                    {
                        Int32 i = ScintillaHelper.GetScintillaControlIndex(sci);
                        if (i != -1)
                        {
                            Int32 line = CurrentLocation.getLine() - 1;
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
                (PluginBase.MainForm as Form).BeginInvoke(new Action<bool>(GotoCurrentLocation), bSetMarker);
                return;
            }
            if (CurrentLocation != null && CurrentLocation.getFile() != null)
            {
                String localPath = GetLocalPath(CurrentLocation.getFile());
                if (localPath != null)
                {
                    int line = CurrentLocation.getLine() - 1;
                    ScintillaControl sci = ScintillaHelper.ActivateDocument(localPath, line, false);
                    if (sci == null) return;
                    if (bSetMarker)
                    {
                        sci.MarkerAdd(line, ScintillaHelper.markerCurrentLine);
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
                // this should not be needed, as we update breakpoints right away
                //m_FlashInterface.UpdateBreakpoints(PluginMain.breakPointManager.BreakPoints);
                m_FlashInterface.Continue();
                UpdateMenuState(DebuggerState.Running);
                UpdateThreadsUI();
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
