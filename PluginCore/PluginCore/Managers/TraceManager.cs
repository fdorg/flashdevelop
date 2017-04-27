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
            asyncTimer.Elapsed += new ElapsedEventHandler(asyncTimerElapsed);
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
        /// <param name="message"></param>
        /// <param name="state"></param>
        /// <param name="group">Used to group the items</param>
        public static void Add(string message, int state, string group)
        {
            Add(new TraceItem(message, state, group));
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
        /// Associates a title and an icon with a trace group
        /// </summary>
        /// <param name="group">The id used to determine the group traces</param>
        /// <param name="title">The title of the group</param>
        /// <param name="icon">An icon for the group. If this is null, the default icon is used</param>
        public static void RegisterTraceGroup(string group, string title, Image icon)
        {
            traceGroups.Add(group, new TraceGroup(group, title, icon));
        }

        public static TraceGroup GetTraceGroup(string group)
        {
            if (group == null) return null;

            TraceGroup value;
            return traceGroups.TryGetValue(group, out value) ? value : null;
        }

        /// <summary>
        /// After a delay, synchronizes the traces
        /// </summary>
        private static void asyncTimerElapsed(Object sender, ElapsedEventArgs e)
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
        public string Title { get; set; }
        public Image Icon { get; set; }

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
