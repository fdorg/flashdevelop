// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PluginCore.Managers
{
    /// <summary>
    /// Provides an application-wide event management methods.
    /// </summary>
    public static class EventManager
    {
        private static readonly List<EventObject> highObjects;
        private static readonly List<EventObject> normalObjects;
        private static readonly List<EventObject> lowObjects;
        private static EventObject[] eventObjectsSnapshot;
        private static bool snapshotInvalid;

        private static readonly object eventLock = new object();

        static EventManager()
        {
            highObjects = new List<EventObject>();
            normalObjects = new List<EventObject>();
            lowObjects = new List<EventObject>();
            eventObjectsSnapshot = new EventObject[0];
            snapshotInvalid = false;
        }

        /// <summary>
        /// Adds an event handler with <see cref="HandlingPriority.Normal"/>.
        /// </summary>
        public static void AddEventHandler(IEventHandler handler, EventType mask)
        {
            AddEventHandler(handler, mask, HandlingPriority.Normal);
        }

        /// <summary>
        /// Adds an event handler with the specific <see cref="HandlingPriority"/> value.
        /// </summary>
        public static void AddEventHandler(IEventHandler handler, EventType mask, HandlingPriority priority)
        {
            if (handler is null) throw new ArgumentNullException(nameof(handler));
            lock (eventLock)
            {
                snapshotInvalid = true;
                GetEventObjects(priority).Add(new EventObject(handler, mask, priority));
            }
        }

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler)
        {
            var eventObjectsList = new[] {highObjects, normalObjects, lowObjects};
            foreach (var eventObjects in eventObjectsList)
            {
                lock (eventLock)
                {
                    for (var i = 0; i < eventObjects.Count; i++)
                    {
                        if (eventObjects[i].Handler != handler) continue;
                        snapshotInvalid = true;
                        eventObjects.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="EventType"/> mask from the event handler.
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler, EventType mask, HandlingPriority priority)
        {
            var eventObjects = GetEventObjects(priority);
            lock (eventLock)
            {
                for (var i = 0; i < eventObjects.Count; i++)
                {
                    var obj = eventObjects[i];
                    if (obj.Handler != handler) continue;
                    obj.Mask &= ~mask;
                    if (obj.Mask == 0)
                    {
                        snapshotInvalid = true;
                        eventObjects.RemoveAt(i);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Dispatches an event to the registered event handlers.
        /// </summary>
        public static void DispatchEvent(object sender, NotifyEvent e)
        {
            int length;
            EventObject[] eventObjectsCopy;
            if (snapshotInvalid)
            {
                lock (eventLock)
                {
                    if (snapshotInvalid)
                    {
                        length = highObjects.Count + normalObjects.Count + lowObjects.Count;
                        eventObjectsCopy = new EventObject[length];
                        highObjects.CopyTo(eventObjectsCopy, 0);
                        normalObjects.CopyTo(eventObjectsCopy, highObjects.Count);
                        lowObjects.CopyTo(eventObjectsCopy, highObjects.Count + normalObjects.Count);
                        eventObjectsSnapshot = eventObjectsCopy;
                        snapshotInvalid = false;
                    }
                    else
                    {
                        eventObjectsCopy = eventObjectsSnapshot;
                        length = eventObjectsCopy.Length;
                    }
                }
            }
            else
            {
                eventObjectsCopy = eventObjectsSnapshot;
                length = eventObjectsCopy.Length;
            }

            for (var i = 0; i < length; i++)
            {
                var obj = eventObjectsCopy[i];
                if ((obj.Mask & e.Type) == 0) continue;
                try
                {
                    obj.Handler.HandleEvent(sender, e, obj.Priority);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowError(ex);
                }
                if (e.Handled)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the list of event objects with the specified priority.
        /// </summary>
        private static List<EventObject> GetEventObjects(HandlingPriority priority)
        {
            return priority switch
            {
                HandlingPriority.High => highObjects,
                HandlingPriority.Normal => normalObjects,
                HandlingPriority.Low => lowObjects,
                _ => throw new InvalidEnumArgumentException(nameof(priority), (int) priority, typeof(HandlingPriority)),
            };
        }

        private sealed class EventObject
        {
            internal readonly IEventHandler Handler;
            internal readonly HandlingPriority Priority;
            internal EventType Mask;

            internal EventObject(IEventHandler handler, EventType mask, HandlingPriority priority)
            {
                Handler = handler;
                Priority = priority;
                Mask = mask;
            }
        }
    }
}
