using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using flash.tools.debugger;
using flash.tools.debugger.events;
using java.net;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDebugger
{
    public delegate void TraceEventHandler(object sender, string trace);
    public delegate void DebuggerEventHandler(object sender);
    public delegate void DebuggerProgressEventHandler(object sender, int current, int total);

    public class FlashInterface : IProgress
    {
        // default metadata retry count 8 attempts per waitForMetadata() call * 5 calls
        public const int METADATA_RETRIES = 8 * 5;
        public event TraceEventHandler  TraceEvent;
        public event DebuggerEventHandler DisconnectedEvent;
        public event DebuggerEventHandler PauseFailedEvent;
        public event DebuggerEventHandler StartedEvent;
        public event DebuggerEventHandler BreakpointEvent;
        public event DebuggerEventHandler FaultEvent;
        public event DebuggerEventHandler PauseEvent;
        public event DebuggerEventHandler StepEvent;
        public event DebuggerEventHandler ScriptLoadedEvent;
        public event DebuggerEventHandler WatchpointEvent;
        public event DebuggerEventHandler UnknownHaltEvent;
        public event DebuggerProgressEventHandler ProgressEvent;
        public event DebuggerEventHandler ThreadsEvent;

        #region Public Properties

        public BreakPointManager m_BreakPointManager = null;

        public bool isDebuggerStarted => Session != null && m_CurrentState != DebuggerState.Initializing && m_CurrentState != DebuggerState.Stopped;

        public bool isDebuggerSuspended => ActiveSession == 1 ? Session.isSuspended() : IsolateSessions[ActiveSession].i_Session.isSuspended();

        public int suspendReason => Session.suspendReason();

        public Session Session { get; private set; }

        public Dictionary<int, IsolateInfo> IsolateSessions { get; set; }

        public int ActiveSession
        {
            get => m_activeSession;
            set
            {
                // todo
                m_activeSession = value;
                ThreadsEvent?.Invoke(this);
            }
        }

        #endregion

        #region Private Properties

        DebuggerState m_CurrentState = DebuggerState.Initializing;
        bool m_RequestPause;
        bool m_RequestResume;
        bool m_RequestStop;
        bool m_RequestDetach;
        bool m_StepResume;
        readonly EventWaitHandle m_SuspendWait = new EventWaitHandle(false, EventResetMode.ManualReset);
        bool m_SuspendWaiting;
        int m_activeSession; // 1 is m_session else lookup runningIsolates

        // Isolates 

        IsolateInfo AddRunningIsolate(int iId)
        {
            RemoveRunningIsolate(iId);
            IsolateSessions[iId] = new IsolateInfo();
            return IsolateSessions[iId];
        }

        void RemoveRunningIsolate(int iId)
        {
            if (IsolateSessions.ContainsKey(iId)) IsolateSessions.Remove(iId);
            if (ActiveSession == iId) ActiveSession = 1;
        }

        // Probably more info needed :)
        public class IsolateInfo
        {
            public IsolateSession i_Session;
            public Dictionary<BreakPointInfo, Location> breakpointLocations = new Dictionary<BreakPointInfo, Location>();
            public bool requestPause;
        }

        // breakpoint mappings
        readonly Dictionary<BreakPointInfo, Location> breakpointLocations = new Dictionary<BreakPointInfo, Location>();

        // Global Properties
        readonly int m_HaltTimeout;
        readonly int m_UpdateDelay;

        // Session Properties
        int m_MetadataAttemptsPeriod;   // 1/4s per attempt
        int m_MetadataAttempts;

        #endregion

        public FlashInterface()
        {
            m_HaltTimeout = 7000;
            m_UpdateDelay = 25;
            m_CurrentState = DebuggerState.Stopped;
        }

        /// <summary>
        /// Main loop
        /// </summary>
        public void Start()
        {
            m_CurrentState = DebuggerState.Starting;
            m_activeSession = 1;
            var mgr = Bootstrap.sessionManager();
            //mgr.setDebuggerCallbacks(new FlashDebuggerCallbacks()); // obsoleted
            mgr.setPreference(SessionManager_.PREF_GETVAR_RESPONSE_TIMEOUT, 5000);
            m_RequestDetach = false;
            mgr.startListening();
            try
            {
                Session = mgr.accept(this);
                if (mgr.isListening()) mgr.stopListening();
                TraceManager.AddAsync("[Starting debug session with FDB]", -1);
                if (Session is null)
                {
                    m_CurrentState = DebuggerState.Stopped;
                    throw new Exception(TextHelper.GetString("Info.UnableToStartDebugger"));
                }
                InitSession();
                m_CurrentState = DebuggerState.Running;
                StartedEvent?.Invoke(this);
                try
                {
                    WaitTilHalted();
                }
                catch {}
                try
                {
                    WaitForMetaData();
                }
                catch {}
                m_CurrentState = DebuggerState.Running;
                Session.breakOnCaughtExceptions(PluginMain.settingObject.BreakOnThrow);
                // now poke to see if the player is good enough
                try
                {
                    if (Session.getPreference(SessionManager_.PLAYER_SUPPORTS_GET) == 0)
                    {
                        TraceManager.AddAsync(TextHelper.GetString("Info.WarningNotAllCommandsSupported"));
                    }
                }
                catch {}
                m_SuspendWaiting = false;
                bool stop = false;
                while (!stop)
                {
                    if (Session != null && Session.getEventCount() > 0) m_SuspendWaiting = false;

                    ProcessEvents();
                    // not there, not connected
                    if (m_RequestStop || m_RequestDetach || !HaveConnection())
                    {
                        stop = true;
                        if (m_RequestResume)
                        {
                            // resume before we disconnect, this is in case of ExceptionHalt
                            Session.resume(); // just throw for now
                            ThreadsEvent?.Invoke(this);
                        }
                        continue;
                    }
                    if (m_RequestResume)
                    {
                        // resume execution (request fulfilled)
                        try
                        {
                            if (m_StepResume)
                            {
                                Session.stepContinue();
                            }
                            else
                            {
                                Session.resume();
                            }
                            ThreadsEvent?.Invoke(this);
                        }
                        catch (NotSuspendedException)
                        {
                            TraceManager.AddAsync(TextHelper.GetString("Info.PlayerAlreadyRunning"));
                        }
                        catch (NoResponseException)
                        {
                            TraceManager.AddAsync(TextHelper.GetString("Info.ProcessNotResponding"));
                        }
                        m_RequestResume = false;
                        m_RequestPause = false;
                        m_StepResume = false;
                        m_CurrentState = DebuggerState.Running;
                        continue;
                    }

                    if (Session.isSuspended())
                    {
                        /*
                        * We have stopped for some reason.
                        * Now before we do this see, if we have a valid break reason, since
                        * we could be still receiving incoming messages, even though we have halted.
                        * This is definately the case with loading of multiple SWFs.  After the load
                        * we get info on the swf.
                        */

                        int tries = 3;
                        while (tries-- > 0 && Session.suspendReason() == SuspendReason_.Unknown)
                        {
                            try
                            {
                                Thread.Sleep(100);
                                ProcessEvents();
                            }
                            catch (ThreadInterruptedException){}
                        }
                        m_SuspendWait.Reset();

                        switch (m_SuspendWaiting ? -1 : suspendReason)
                        {
                            case 1://SuspendReason_.Breakpoint:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (BreakpointEvent is { } breakpointEvent)
                                {
                                    m_RequestPause = true;
                                    // trigger only for main worker here (if we are main worker, or if we are not and are not suspended)
                                    if (ActiveSession == 1 || !isDebuggerSuspended)
                                    {
                                        var frames = Session.getFrames();
                                        Location loc = null;
                                        if (frames.Length > 0)
                                        {
                                            loc = frames[0].getLocation();
                                        }
                                        if (loc != null && m_BreakPointManager.ShouldBreak(loc.getFile(), loc.getLine(), frames[0]))
                                        {
                                            ActiveSession = 1;
                                            breakpointEvent(this);
                                        }
                                        else
                                        {
                                            m_RequestResume = true;
                                            m_StepResume = true;
                                        }
                                    }
                                    else
                                    {
                                        ThreadsEvent?.Invoke(this);
                                    }
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 3://SuspendReason_.Fault:
                                m_CurrentState = DebuggerState.ExceptionHalt;
                                if (FaultEvent is { } faultEvent)
                                {
                                    faultEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 7://SuspendReason_.ScriptLoaded:
                                try
                                {
                                     WaitForMetaData();
                                } 
                                catch (InProgressException) {}
                                m_CurrentState = DebuggerState.PauseHalt;

                                // refresh breakpoints
                                ClearBreakpoints();
                                UpdateBreakpoints(m_BreakPointManager.BreakPoints);

                                if (ScriptLoadedEvent is { } scriptLoadedEvent)
                                {
                                    scriptLoadedEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 5://SuspendReason_.Step:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (StepEvent is { } stepEvent)
                                {
                                    stepEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 4://SuspendReason_.StopRequest:
                                m_CurrentState = DebuggerState.PauseHalt;
                                if (PauseEvent is { } pauseEvent)
                                {
                                    pauseEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 2://SuspendReason_.Watch:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (WatchpointEvent is { } watchPointEvent)
                                {
                                    watchPointEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case -1:
                                // waiting for susped to end, do nothing
                                break;

                            default:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (UnknownHaltEvent is { } unknownHaltEvent)
                                {
                                    unknownHaltEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;
                        }

                        if (!(m_RequestResume || m_RequestDetach))
                        {
                            m_SuspendWaiting = !m_SuspendWait.WaitOne(500, false);
                        }
                    }
                    else
                    {
                        if (m_RequestPause)
                        {
                            try
                            {
                                m_CurrentState = DebuggerState.Pausing;
                                try
                                {
                                    Session.suspend();
                                }
                                catch { } // No crash here please...
                                if (!HaveConnection()) // no connection => dump state and end
                                {
                                    stop = true;
                                    continue;
                                }
                                if (!Session.isSuspended())
                                {
                                    m_RequestPause = false;
                                    m_CurrentState = DebuggerState.Running;
                                    TraceManager.AddAsync(TextHelper.GetString("Info.CouldNotHalt"));
                                    PauseFailedEvent?.Invoke(this);
                                }
                            }
                            catch (ArgumentException)
                            {
                                TraceManager.AddAsync(TextHelper.GetString("Info.EscapingFromDebuggerPendingLoop"));
                                stop = true;
                            }
                            catch (IOException io)
                            {
                                IDictionary args = new Hashtable();
                                args["error"] = io.Message; //$NON-NLS-1$
                                TraceManager.AddAsync(ReplaceInlineReferences(TextHelper.GetString("Info.ContinuingDueToError"), args));
                            }
                            catch (SuspendedException)
                            {
                                // lucky us, already paused
                            }
                        }
                    }
                    // sleep for a bit, then process our events.

                    // process isolates
                    foreach (var kv in IsolateSessions)
                    {
                        if (kv.Value.requestPause)
                        {
                            try
                            {
                                kv.Value.i_Session.suspend();
                            }
                            catch (Exception e)
                            {
                                TraceManager.AddAsync("Isolate suspend exception: " + e);
                            }
                        }
                        kv.Value.requestPause = false;
                    }

                    try
                    {
                        Thread.Sleep(m_UpdateDelay);
                    }
                    catch {}
                }
            }
            catch (SocketException ex)
            {
                // No errors if requested
                if (!m_RequestStop) throw ex;
            }
            catch (SocketTimeoutException ex)
            {
                if (m_CurrentState != DebuggerState.Starting) throw ex;
                TraceManager.AddAsync("[No debug Flash player connection request]", -1);
            }
            finally
            {
                DisconnectedEvent?.Invoke(this);
                ExitSession();
            }
        }

        void settingObject_BreakOnThrowChanged(object sender, EventArgs e)
        {
            if (m_CurrentState != DebuggerState.Starting &&
                m_CurrentState != DebuggerState.Stopped)
            {
                Session.breakOnCaughtExceptions(PluginMain.settingObject.BreakOnThrow);
            }
        }

        internal virtual void InitSession()
        {
            try
            {
                Session.bind();
            }
            catch (VersionException)
            {
            }
            // reset session properties
            m_MetadataAttemptsPeriod = 250;                 // 1/4s per attempt
            m_MetadataAttempts = METADATA_RETRIES;
            m_RequestStop = false;
            m_RequestPause = false;
            m_RequestResume = false;
            m_StepResume = false;

            IsolateSessions = new Dictionary<int, IsolateInfo>();
            PluginMain.settingObject.BreakOnThrowChanged += settingObject_BreakOnThrowChanged;
        }

        /// <summary> If we still have a socket try to send an exit message
        /// Doesn't seem to work ?!?
        /// </summary>
        internal virtual void ExitSession()
        {
            PluginMain.settingObject.BreakOnThrowChanged -= settingObject_BreakOnThrowChanged;
            // clear out our watchpoint list and displays
            // keep breakpoints around so that we can try to reapply them if we reconnect
            if (Session != null)
            {
                if (m_RequestDetach) Session.unbind();
                else Session.terminate();
                Session = null;
            }
            IsolateSessions?.Clear();
            var mgr = Bootstrap.sessionManager();
            if (mgr != null && mgr.isListening()) mgr.stopListening();
            m_CurrentState = DebuggerState.Stopped;
            ClearBreakpoints();
        }

        internal virtual void Detach()
        {
            m_RequestDetach = true;
            m_SuspendWait.Set();
        }

        bool HaveConnection() => Session != null && Session.isConnected();

        void WaitForMetaData()
        {
            // perform a query to see if our metadata has loaded
            int metadatatries = m_MetadataAttempts;
            const int maxPerCall = 8; // cap on how many attempt we make per call
            int tries = Math.Min(maxPerCall, metadatatries);
            if (tries > 0)
            {
                int remain = metadatatries - tries; // assume all get used up
                // perform the call and then update our remaining number of attempts
                try
                {
                    tries = WaitForMetaData(tries);
                    remain = metadatatries - tries; // update our used count
                }
                catch (InProgressException ipe)
                {
                    m_MetadataAttempts = remain;
                    throw ipe;
                }
            }
        }

        /// <summary> Wait for the API to load function names, which
        /// exist in the form of external meta-data.
        /// 
        /// Only do this tries times, then give up
        /// 
        /// We wait period * attempts
        /// </summary>
        public virtual int WaitForMetaData(int attempts)
        {
            int start = attempts;
            int period = m_MetadataAttemptsPeriod;
            while (attempts > 0)
            {
                // are we done yet?
                if (MetaDataAvailable) break;
                try
                {
                    attempts--;
                    Thread.Sleep(period);
                }
                catch (ThreadInterruptedException){}
            }
            // throw exception if still not ready
            if (!MetaDataAvailable) throw new InProgressException();
            return start - attempts; // remaining number of tries
        }

        /// <summary> Ask each swf if metadata processing is complete</summary>
        public virtual bool MetaDataAvailable
        {
            get
            {
                bool allLoaded = true;
                try
                {
                    // we need to ask the session since our fileinfocache will hide the exception
                    var swfs = Session.getSwfs();
                    foreach (var swf in swfs)
                    {
                        if (swf != null && !swf.isProcessingComplete())
                        {
                            allLoaded = false;
                            break;
                        }
                    }
                }
                catch (NoResponseException)
                {
                    // ok we still need to wait for player to read the swd in
                    allLoaded = false;
                }
                // count the number of times we checked and it wasn't there
                if (!allLoaded)
                {
                }
                else
                {
                    // success so we reset our attempt counter
                    m_MetadataAttempts = METADATA_RETRIES;
                }
                return allLoaded;
            }
        }

        /// <summary> Process the incoming debug event queue</summary>
        internal virtual void ProcessEvents()
        {
            while (Session != null && Session.getEventCount() > 0)
            {
                var e = Session.nextEvent();
                switch (e)
                {
                    case TraceEvent _:
                        DumpTraceLine(e.information);
                        TraceEvent?.Invoke(this, e.information);
                        break;
                    case SwfLoadedEvent @event:
                        if (PluginMain.settingObject.VerboseOutput) DumpSwfLoadedEvent(@event);
                        break;
                    case SwfUnloadedEvent @event:
                        if (PluginMain.settingObject.VerboseOutput) DumpSwfUnloadedEvent(@event);
                        break;
                    case IsolateCreateEvent @event:
                        TraceManager.AddAsync("Created Worker " + @event.isolate.getId());
                        DumpIsolateCreateEvent(@event);
                        break;
                    case IsolateExitEvent @event:
                        TraceManager.AddAsync("Worker " + @event.isolate.getId() + " Exited");
                        DumpIsolateExitEvent(@event);
                        break;
                    case BreakEvent @event:
                        // handle only worker events
                        if (PluginMain.settingObject.VerboseOutput)
                            TraceManager.AddAsync("Worker " + @event.isolateId + " BreakEvent");
                        if (@event.isolateId > 1 && IsolateSessions.TryGetValue(@event.isolateId, out var ii))
                        {
                            if (!ii.i_Session.isSuspended()) continue;
                            var frames = ii.i_Session.getFrames();
                            Location loc = null;
                            if (frames.Length > 0) loc = frames[0].getLocation();
                            if (ii.i_Session.suspendReason() == 1) // SuspendReason_.Breakpoint
                            {
                                if (loc != null &&
                                    m_BreakPointManager.ShouldBreak(loc.getFile(), loc.getLine(), frames[0]))
                                {
                                    // switch and break only if we are not dealing with some other thread
                                    if (ActiveSession == @event.isolateId || !isDebuggerSuspended)
                                    {
                                        ActiveSession = @event.isolateId;
                                        BreakpointEvent?.Invoke(this);
                                    }
                                    else ThreadsEvent?.Invoke(this);
                                }
                                else ii.i_Session.stepContinue();
                            }
                            else if (ii.i_Session.suspendReason() == 3) //SuspendReason_.Fault
                            {
                                if (ActiveSession == @event.isolateId || !isDebuggerSuspended)
                                {
                                    ActiveSession = @event.isolateId;
                                    if (FaultEvent is { } faultEvent) faultEvent(this);
                                    else ii.i_Session.resume();
                                }
                                else ThreadsEvent?.Invoke(this);
                            }
                            else if (ii.i_Session.suspendReason() == 5) //SuspendReason_.Step
                            {
                                if (ActiveSession == @event.isolateId || !isDebuggerSuspended)
                                {
                                    ActiveSession = @event.isolateId;
                                    if (StepEvent is { } stepEvent) stepEvent(this);
                                    else ii.i_Session.resume();
                                }
                                else ThreadsEvent?.Invoke(this);
                            }
                            else if (ii.i_Session.suspendReason() == 4) //SuspendReason_.StopRequest
                            {
                                if (ActiveSession == @event.isolateId || !isDebuggerSuspended)
                                {
                                    ActiveSession = @event.isolateId;
                                    if (PauseEvent is { } pauseEvent) pauseEvent(this);
                                    else ii.i_Session.resume();
                                }
                                else ThreadsEvent?.Invoke(this);
                            }
                            else if (ii.i_Session.suspendReason() == 2) //SuspendReason_.Watch
                            {
                                if (ActiveSession == @event.isolateId || !isDebuggerSuspended)
                                {
                                    ActiveSession = @event.isolateId;
                                    if (WatchpointEvent is { } watchpointEvent) watchpointEvent(this);
                                    else ii.i_Session.resume();
                                }
                                else ThreadsEvent?.Invoke(this);
                            }
                        }

                        break;
                    case FileListModifiedEvent _:
                        // we ignore this
                        break;
                    case FunctionMetaDataAvailableEvent _:
                        // we ignore this
                        break;
                    case FaultEvent @event:
                        DumpFaultLine(@event);
                        break;
                    default:
                        if (PluginMain.settingObject.VerboseOutput)
                        {
                            IDictionary args = new Hashtable();
                            args["type"] = e; //$NON-NLS-1$
                            args["info"] = e.information; //$NON-NLS-1$
                            TraceManager.AddAsync(ReplaceInlineReferences(TextHelper.GetString("Info.UnknownEvent"), args));
                        }
                        break;
                }
            }
        }

        // wait a little bit of time until the player halts, if not throw an exception!
        internal virtual void WaitTilHalted()
        {
            if (!HaveConnection()) throw new InvalidOperationException();
            // spin for a while waiting for a halt; updating trace messages as we get them
            WaitForSuspend(m_HaltTimeout, m_UpdateDelay);
            if (!Session.isSuspended()) throw new SynchronizationLockException();
        }

        /// <summary> We spin in this spot until the player reaches the
        /// requested suspend state, either true or false.
        /// 
        /// During this time we wake up every period milliseconds
        /// and update the display and our state with information
        /// received from the debug event queue.
        /// </summary>
        internal virtual void WaitForSuspend(int timeout, int period)
        {
            while (timeout > 0)
            {
                // dump our events to the console while we are waiting.
                ProcessEvents();
                if (Session.isSuspended()) break;
                try
                {
                    Thread.Sleep(period);
                }
                catch (ThreadInterruptedException){}
                timeout -= period;
            }
        }

        // pretty print a trace statement to the console
        internal virtual void DumpTraceLine(string s) => TraceManager.AddAsync(s, 1);

        // pretty print a fault statement to the console
        internal virtual void DumpFaultLine(FaultEvent e)
        {
            var sb = new StringBuilder();
            // use a slightly different format for ConsoleErrorFaults
            if (e is ConsoleErrorFault)
            {
                sb.Append(TextHelper.GetString("Info.LinePrefixWhenDisplayingConsoleError")); //$NON-NLS-1$
                sb.Append(' ');
                sb.Append(e.information);
            }
            else
            {
                string name = e.name();
                sb.Append(TextHelper.GetString("Info.LinePrefixWhenDisplayingFault")); //$NON-NLS-1$
                sb.Append(' ');
                sb.Append(name);
                if ((string)e.information != null && e.information.length() > 0)
                {
                    sb.Append(TextHelper.GetString("Info.InformationAboutFault")); //$NON-NLS-1$
                    sb.Append(e.information);
                }
                if (PluginMain.settingObject.VerboseOutput)
                {
                    sb.AppendLine();
                    sb.Append(e.stackTrace());
                }
            }

            if (e.isolateId == 1) TraceManager.AddAsync(sb.ToString(), 3);
            else TraceManager.AddAsync("Worker " + e.isolateId + ": " + sb, 3);
        }

        /// <summary> Called when a swf has been loaded by the player</summary>
        /// <param name="e">event documenting the load
        /// </param>
        internal virtual void DumpSwfLoadedEvent(SwfLoadedEvent e)
        {
            // now rip off any trailing ? options
            var at = e.path.lastIndexOf('?');
            string name = (at > -1) ? e.path.substring(0, (at) - (0)) : e.path;
            var sb = new StringBuilder();
            sb.Append(TextHelper.GetString("Info.LinePrefixWhenSwfLoaded"));
            sb.Append(' ');
            sb.Append(name);
            sb.Append(" - "); //$NON-NLS-1$
            IDictionary args = new Hashtable();
            args["size"] = e.swfSize.ToString("N0"); //$NON-NLS-1$
            sb.Append(ReplaceInlineReferences(TextHelper.GetString("Info.SizeAfterDecompression"), args));
            TraceManager.AddAsync(sb.ToString());
        }

        /// <summary> Perform the tasks need for when a swf is unloaded
        /// the player
        /// </summary>
        internal virtual void DumpSwfUnloadedEvent(SwfUnloadedEvent e)
        {
            // now rip off any trailing ? options
            var at = e.path.lastIndexOf('?');
            string name = (at > -1) ? e.path.substring(0, (at) - (0)) : e.path;
            var sb = new StringBuilder();
            sb.Append(TextHelper.GetString("Info.LinePrefixWhenSwfUnloaded")); //$NON-NLS-1$
            sb.Append(' ');
            sb.Append(name);
            TraceManager.AddAsync(sb.ToString());
        }

        /// <summary> Perform the tasks need for when an isolate is created
        /// </summary>
        internal virtual void DumpIsolateCreateEvent(IsolateCreateEvent e)
        {
            var i_Session = Session.getWorkerSession(e.isolate.getId());
            i_Session.breakOnCaughtExceptions(true);
            var ii = AddRunningIsolate(e.isolate.getId());
            ii.i_Session = i_Session;
            UpdateIsolateBreakpoints(m_BreakPointManager.BreakPoints, ii);
            i_Session.resume();
            ThreadsEvent?.Invoke(this);
        }

        /// <summary> Perform the tasks need for when an isolate has exited
        /// </summary>
        internal virtual void DumpIsolateExitEvent(IsolateExitEvent e)
        {
            RemoveRunningIsolate(e.isolate.getId());
            ThreadsEvent?.Invoke(this);
        }

        internal virtual Location GetCurrentLocation()
        {
            return ActiveSession == 1
                ? GetCurrentMainLocation()
                : GetCurrentIsolateLocation(ActiveSession);
        }

        internal virtual Location GetCurrentMainLocation()
        {
            Location where = null;
            try
            {
                var frames = Session.getFrames();
                where = frames.Length > 0 ? frames[0].getLocation() : null;
            }
            catch (PlayerDebugException)
            {
                // where is null
            }
            return where;
        }

        internal virtual Location GetCurrentIsolateLocation(int iId)
        {
            var ii = IsolateSessions[iId];
            Location where = null;
            try
            {
                var frames = ii.i_Session.getFrames();
                where = frames.Length > 0 ? frames[0].getLocation() : null;
            }
            catch (PlayerDebugException)
            {
                // where is null
            }
            return where;
        }

        public void Stop()
        {
            m_RequestStop = true;
            switch (m_CurrentState)
            {
                case DebuggerState.Starting:
                    var mgr = Bootstrap.sessionManager();
                    mgr.stopListening();
                    return;
                case DebuggerState.ExceptionHalt:
                    m_RequestResume = true;
                    break;
            }
            m_SuspendWait.Set();
        }

        public void Cleanup()
        {
            if (isDebuggerStarted)
            {
                Stop();
            }
        }

        public void Next()
        {
            if (ActiveSession > 1)
            {
                if (IsolateSessions[ActiveSession].i_Session.isSuspended())
                {
                    IsolateSessions[ActiveSession].i_Session.stepOver();
                }
                return;
            }
            if (Session.isSuspended())
            {
                Session.stepOver();
                m_SuspendWait.Set();
            }
        }

        public void Step()
        {
            if (ActiveSession > 1)
            {
                if (IsolateSessions[ActiveSession].i_Session.isSuspended())
                {
                    IsolateSessions[ActiveSession].i_Session.stepInto();
                }
                return;
            }
            if (Session.isSuspended())
            {
                Session.stepInto();
                m_SuspendWait.Set();
            }
        }

        public void Continue()
        {
            // fix me, needs to move to thread
            if (ActiveSession > 1)
            {
                if (IsolateSessions[ActiveSession].i_Session.isSuspended())
                {
                    IsolateSessions[ActiveSession].i_Session.resume();
                }
                return;
            }
            if (Session.isSuspended())
            {
                m_RequestResume = true;
                m_SuspendWait.Set();
            }
        }

        public void Pause()
        {
            // fix me, needs to move to thread
            if (ActiveSession > 1)
            {
                if (!IsolateSessions[ActiveSession].i_Session.isSuspended())
                {
                    IsolateSessions[ActiveSession].requestPause = true;
                    m_SuspendWait.Set();
                }
                return;
            }
            if (!Session.isSuspended())
            {
                m_RequestPause = true;
                m_SuspendWait.Set();
            }
        }

        public void Finish()
        {
            if (ActiveSession > 1)
            {
                if (IsolateSessions[ActiveSession].i_Session.isSuspended())
                {
                    IsolateSessions[ActiveSession].i_Session.stepOut();
                }
                return;
            }
            if (Session.isSuspended())
            {
                Session.stepOut();
                m_SuspendWait.Set();
            }
        }

        public Frame[] GetFrames()
        {
            return ActiveSession == 1
                ? Session.getFrames()
                : IsolateSessions[ActiveSession].i_Session.getFrames();
        }

        public Variable[] GetArgs(int frameNumber) => GetFrames()[frameNumber].getArguments(Session);

        public Variable GetThis(int frameNumber) => GetFrames()[frameNumber].getThis(Session);

        public Variable[] GetLocals(int frameNumber) => GetFrames()[frameNumber].getLocals(Session);

        public void UpdateBreakpoints(List<BreakPointInfo> breakpoints)
        {
            CommonUpdateBreakpoints(breakpoints, breakpointLocations, null);
            foreach (var ii in IsolateSessions.Values)
            {
                UpdateIsolateBreakpoints(breakpoints, ii);
            }
        }

        void UpdateIsolateBreakpoints(IReadOnlyCollection<BreakPointInfo> breakpoints, IsolateInfo ii) => CommonUpdateBreakpoints(breakpoints, ii.breakpointLocations, ii.i_Session);

        void CommonUpdateBreakpoints(IReadOnlyCollection<BreakPointInfo> breakpoints, IDictionary<BreakPointInfo, Location> breakpointLocations, IsolateSession iSession)
        {
            var files = new Dictionary<string, int>();
            foreach (BreakPointInfo bp in breakpoints)
            {
                if (breakpointLocations.ContainsKey(bp) || bp.IsDeleted || !bp.IsEnabled) continue;
                if (files.ContainsKey(bp.FileFullPath)) continue;
                files.Add(bp.FileFullPath, int.MaxValue);
            }
            if (files.Count > 0)
            {
                // reverse loop to take latest loaded swf first, and ignore old swf.
                var swfInfo = (iSession != null) ? iSession.getSwfs() : Session.getSwfs();
                for (int swfC = swfInfo.Length - 1; swfC >= 0; swfC--) 
                {
                    var swf = swfInfo[swfC];
                    if (swf is null) continue;
                    try
                    {
                        foreach (var src in swf.getSourceList(Session))
                        {
                            var localPath = PluginMain.debugManager.GetLocalPath(src);
                            if (localPath != null && files.TryGetValue(localPath, out var value) && value > src.getId())
                            {
                                files[localPath] = src.getId();
                            }
                        }
                    }
                    catch (InProgressException) { }
                }
            }

            foreach (BreakPointInfo bp in breakpoints)
            {
                if (!breakpointLocations.ContainsKey(bp))
                {
                    if (bp.IsEnabled && !bp.IsDeleted && files.TryGetValue(bp.FileFullPath, out var value) && value != int.MaxValue)
                    {
                        var l = iSession != null
                            ? iSession.setBreakpoint(value, bp.Line + 1)
                            : Session.setBreakpoint(value, bp.Line + 1);
                        if (l != null)
                        {
                            breakpointLocations.Add(bp, l);
                        }
                    }
                }
                else if (bp.IsDeleted || !bp.IsEnabled)
                {
                    // todo, i_Session does not have a clearBreakpoint method, m_Session clears them all. optimize out extra loops
                    Session.clearBreakpoint(breakpointLocations[bp]);
                    breakpointLocations.Remove(bp);
                }
            }
        }

        void ClearBreakpoints()
        {
            if (isDebuggerStarted)
            {
                foreach (Location l in Session.getBreakpointList())
                {
                    Session.clearBreakpoint(l);
                }
            }

            breakpointLocations?.Clear();
            if (IsolateSessions != null)
                foreach (IsolateInfo ii in IsolateSessions.Values)
                {
                    ii.breakpointLocations.Clear();
                }
        }

        static string ReplaceInlineReferences(string text, IDictionary parameters)
        {
            if (parameters is null) return text;
            int depth = 100;
            while (depth-- > 0)
            {
                int o = text.IndexOfOrdinal("${");
                if (o == -1) break;
                if ((o >= 1) && (text[o - 1] == '$'))
                {
                    o = text.IndexOfOrdinal("${", o + 2);
                    if (o == -1) break;
                }
                int c = text.IndexOf('}', o);
                if (c == -1)
                {
                    return null; // FIXME
                }
                string name = text.Substring(o + 2, (c) - (o + 2));
                string value = null;
                if (parameters.Contains(name) && (parameters[name] != null))
                {
                    value = parameters[name].ToString();
                }
                value ??= "";
                text = text.Substring(0, (o) - (0)) + value + text.Substring(c + 1);
            }
            return text.Replace("$${", "${");
        }

        public void setProgress(int current, int total) => ProgressEvent?.Invoke(this, current, total);
    }
}