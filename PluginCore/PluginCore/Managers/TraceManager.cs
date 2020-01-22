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
        static bool synchronizing;
        static int maxQueue = 1000;
        static readonly List<TraceItem> traceLog;
        static readonly List<TraceItem> asyncQueue;
        static readonly Dictionary<string, TraceGroup> traceGroups;
        static readonly Timer asyncTimer;
        static int uniqueToken;

        static TraceManager()
        {
            traceLog = new List<TraceItem>();
            asyncQueue = new List<TraceItem>();
            traceGroups = new Dictionary<string, TraceGroup>();
            asyncTimer = new Timer();
            asyncTimer.Interval = 200;
            asyncTimer.AutoReset = false;
            asyncTimer.Elapsed += AsyncTimer_Elapsed;
            uniqueToken = 0;
        }

        /// <summary>
        /// Apply the modified settings.
        /// </summary>
        public static void ApplySettings(int maxTraceLines) => maxQueue = maxTraceLines;

        /// <summary>
        /// Adds a new entry to the log
        /// </summary>
        public static void Add(string message) => Add(message, 0);

        /// <summary>
        /// Adds a new entry to the log
        /// </summary>
        public static void Add(string message, int state) => Add(new TraceItem(message, state));

        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="groupData">The id of the trace group to output to, with optional arguments.</param>
        public static void Add(string message, int state, string groupData) => Add(new TraceItem(message, state, groupData));

        /// <summary>
        /// Adds a new entry to the log in an unsafe threading context
        /// </summary>
        public static void AddAsync(string message) => AddAsync(message, 0);

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
        public static void AddAsync(string message, int state)
        {
            if (((Form) PluginBase.MainForm).InvokeRequired)
            {
                lock (asyncQueue)
                {
                    var count = asyncQueue.Count;
                    if (count < maxQueue)
                    {
                        asyncQueue.Add(new TraceItem(message, state));
                        asyncTimer.Start();
                    }
                    else if (count == maxQueue)
                    {
                        asyncQueue.Add(new TraceItem(DistroConfig.DISTRIBUTION_NAME + ": Trace overflow, " +
                            "you can configure \"Max Trace Lines\" in: " +
                            "Tools >> Program Settings >> FlashDevelop >> Features", 4));
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
        /// <param name="showFilterButtons">Whether to show info, warning and error filter buttons.</param>
        /// <param name="allowMultiplePanels">Whether to let users lock certain panels to create multiple panels.</param>
        /// <param name="icon">The icon to be displayed in the panel. If this is <see langword="null"/>, the default icon is used.</param>
        public static void RegisterTraceGroup(string groupId, string title, bool showFilterButtons = true, bool allowMultiplePanels = false, Image icon = null)
        {
            if (groupId is null) throw new ArgumentNullException(nameof(groupId));
            traceGroups.Add(groupId, new TraceGroup(groupId, title, showFilterButtons, allowMultiplePanels, icon));
        }

        /// <summary>
        /// Returns the <see cref="TraceGroup"/> with the specified group id, or <see langword="null"/>.
        /// </summary>
        /// <param name="groupId">The id of the trace group.</param>
        public static TraceGroup GetTraceGroup(string groupId)
        {
            if (groupId is null) throw new ArgumentNullException(nameof(groupId));
            return traceGroups.TryGetValue(groupId, out var value) ? value : null;
        }

        /// <summary>
        /// Creates a string for group data.
        /// <para/>
        /// <paramref name="groupId"/>:arg1,arg2,<paramref name="args"/>
        /// </summary>
        public static string CreateGroupData(string groupId, params string[] args)
        {
            if (args.IsNullOrEmpty()) return groupId;
            return groupId + ":" + string.Join(",", args);
        }

        /// <summary>
        /// Creates a string for group data with a unique id argument at the end. Used for groups with <see cref="TraceGroup.AllowMultiplePanels"/> set to <see langword="true"/>.
        /// <para/>
        /// <paramref name="groupId"/>:arg1,arg2,<paramref name="args"/>,uniqueId
        /// </summary>
        public static string CreateGroupDataUnique(string groupId, params string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                return CreateGroupData(groupId, uniqueToken++.ToString());
            }
            return CreateGroupData(groupId, args) + "," + uniqueToken++;
        }

        /// <summary>
        /// Parses group data into group id and args. Returns <see langword="true"/> if args are present; <see langword="false"/> otherwise.
        /// </summary>
        public static bool ParseGroupData(string groupData, out string groupId, out string[] args)
        {
            var data = groupData.Split(new[] { ':' }, 2);
            if (data.Length > 1)
            {
                groupId = data[0];
                args = data[1].Split(',');
                return true;
            }
            groupId = groupData;
            args = Array.Empty<string>();
            return false;
        }

        /// <summary>
        /// After a delay, synchronizes the traces
        /// </summary>
        static void AsyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (asyncQueue)
            {
                if (PluginBase.MainForm.ClosingEntirely) return;
                if (synchronizing) return;
                synchronizing = true;
                try
                {
                    ((Form) PluginBase.MainForm).BeginInvoke((MethodInvoker) ProcessQueue);
                }
                catch (Exception)
                {
                    synchronizing = false;
                }
            }
        }

        /// <summary>
        /// Processes the trace queue
        /// </summary>
        static void ProcessQueue()
        {
            lock (asyncQueue)
            {
                if (PluginBase.MainForm.ClosingEntirely) return;
                traceLog.AddRange(asyncQueue);
                asyncQueue.Clear();
                synchronizing = false;
            }
            var ne = new NotifyEvent(EventType.Trace);
            EventManager.DispatchEvent(null, ne);
        }

        /// <summary>
        /// Gets a read only list from trace log
        /// </summary>
        public static IList<TraceItem> TraceLog => traceLog.AsReadOnly();
    }

    public class TraceGroup
    {
        public string Id { get; }
        public string Title { get; }
        public bool ShowFilterButtons { get; }
        public bool AllowMultiplePanels { get; }
        public Image Icon { get; }

        public TraceGroup(string id, string title, bool showFilterButtons, bool allowMultiplePanels, Image icon)
        {
            Id = id;
            Title = title;
            ShowFilterButtons = showFilterButtons;
            AllowMultiplePanels = allowMultiplePanels;
            Icon = icon;
        }
    }

    public class TraceItem
    {

        public TraceItem(string message, int state) : this(message, state, null)
        {

        }

        public TraceItem(string message, int state, string groupData)
        {
            Timestamp = DateTime.Now;
            Message = message;
            State = state;
            GroupData = groupData;
        }

        /// <summary>
        /// Gets the state (<see cref="TraceType"/> enum).
        /// </summary>
        public int State { get; }

        /// <summary>
        /// Gets the logged trace message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the timestamp of the trace.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets the group data.
        /// </summary>
        public string GroupData { get; }
    }
}