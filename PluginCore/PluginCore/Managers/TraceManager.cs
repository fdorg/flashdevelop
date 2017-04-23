using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PluginCore.Managers
{
    public class TraceManager
    {
        private static Boolean synchronizing;
        private static Int32 MAX_QUEUE = 1000;
        private static List<TraceItem> traceLog;
        private static List<TraceItem> asyncQueue;
        private static Dictionary<string, TraceGroup> traceGroups;
        private static Timer asyncTimer;

        static TraceManager()
        {
            traceLog = new List<TraceItem>();
            asyncQueue = new List<TraceItem>();
            traceGroups = new Dictionary<string, TraceGroup>();
            asyncTimer = new Timer();
            asyncTimer.Interval = 200;
            asyncTimer.AutoReset = false;
            asyncTimer.Elapsed += new ElapsedEventHandler(AsyncTimer_Elapsed);
        }

        /// <summary>
        /// Adds a new entry to the log
        /// </summary>
        public static void Add(String message)
        {
            Add(message, 0);
        }

        /// <summary>
        /// Adds a new entry to the log
        /// </summary>
        public static void Add(String message, Int32 state)
        {
            Add(new TraceItem(message, state));
        }

        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="groupId">The id of the trace group to output to.</param>
        public static void Add(string message, int state, string groupId)
        {
            Add(new TraceItem(message, state, groupId));
        }

        /// <summary>
        /// Adds a new entry to the log in an unsafe threading context
        /// </summary>
        public static void AddAsync(String message)
        {
            AddAsync(message, 0);
        }

        /// <summary>
        /// Adds a new entry to the log (no overflow check for sync traces)
        /// </summary>
        public static void Add(TraceItem traceItem)
        {
            lock (asyncQueue)
            {
                asyncQueue.Add(traceItem);
                asyncTimer.Start();
            }
        }

        /// <summary>
        /// Adds a new entry to the log in an unsafe threading context
        /// </summary>
        public static void AddAsync(String message, Int32 state)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                lock (asyncQueue)
                {
                    int count = asyncQueue.Count;
                    if (count < MAX_QUEUE)
                    {
                        asyncQueue.Add(new TraceItem(message, state));
                        asyncTimer.Start();
                    }
                    else if (count == MAX_QUEUE)
                    {
                        asyncQueue.Add(new TraceItem(DistroConfig.DISTRIBUTION_NAME + ": Trace overflow", 4));
                        asyncTimer.Stop();
                        asyncTimer.Start();
                    }
                }
                return;
            }
            Add(new TraceItem(message, state));
        }

        /// <summary>
        /// Registers a trace group with a title and an icon.
        /// </summary>
        /// <param name="groupId">The id used to determine the trace group.</param>
        /// <param name="title">The title to be displayed in the panel.</param>
        /// <param name="icon">The icon to be displayed in the panel. If this is <see langword="null"/>, the default icon is used.</param>
        public static void RegisterTraceGroup(string groupId, string title, Image icon = null)
        {
            if (groupId == null) throw new ArgumentNullException(nameof(groupId));
            traceGroups.Add(groupId, new TraceGroup(groupId, title, icon));
        }

        /// <summary>
        /// Returns the <see cref="TraceGroup"/> with the specified group id, or <see langword="null"/>.
        /// </summary>
        /// <param name="groupId">The id of the trace group.</param>
        public static TraceGroup GetTraceGroup(string groupId)
        {
            if (groupId == null) throw new ArgumentNullException(nameof(groupId));
            TraceGroup value;
            return traceGroups.TryGetValue(groupId, out value) ? value : null;
        }

        /// <summary>
        /// After a delay, synchronizes the traces
        /// </summary>
        private static void AsyncTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            lock (asyncQueue)
            {
                if (PluginBase.MainForm.ClosingEntirely) return;
                if (!synchronizing)
                {
                    synchronizing = true;
                    try
                    {
                        (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate { ProcessQueue(); });
                    }
                    catch (Exception)
                    {
                        synchronizing = false;
                    }

                }
            }
        }

        /// <summary>
        /// Processes the trace queue
        /// </summary>
        private static void ProcessQueue()
        {
            lock (asyncQueue)
            {
                if (PluginBase.MainForm.ClosingEntirely) return;
                traceLog.AddRange(asyncQueue);
                asyncQueue.Clear();
                synchronizing = false;
            }
            NotifyEvent ne = new NotifyEvent(EventType.Trace);
            EventManager.DispatchEvent(null, ne);
        }

        /// <summary>
        /// Gets a read only list from trace log
        /// </summary>
        public static IList<TraceItem> TraceLog
        {
            get { return traceLog.AsReadOnly(); }
        }

    }

    public class TraceGroup
    {
        public string Id { get; }
        public string Title { get; }
        public Image Icon { get; }

        public TraceGroup(string id, string title, Image icon)
        {
            Id = id;
            Title = title;
            Icon = icon;
        }
    }

    public class TraceItem
    {
        private Int32 state = 0;
        private DateTime timestamp;
        private String message;

        public string Group { get; }

        public TraceItem(string message, int state, string group) : this(message, state)
        {
            Group = group;
        }

        public TraceItem(String message, Int32 state)
        {
            this.timestamp = DateTime.Now;
            this.message = message;
            this.state = state;
        }

        /// <summary>
        /// Gets the state (TraceType enum)
        /// </summary>
        public Int32 State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets the logged trace message
        /// </summary>
        public String Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the timestamp of the trace
        /// </summary>
        public DateTime Timestamp
        {
            get { return this.timestamp; }
        }

    }

}
