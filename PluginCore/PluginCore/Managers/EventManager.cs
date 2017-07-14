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
        private static List<EventObject> highObjects;
        private static List<EventObject> normalObjects;
        private static List<EventObject> lowObjects;
        private static EventObject[] eventObjectsSnapshot;
        private static bool snapshotInvalid;

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
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            snapshotInvalid = true;
            GetEventObjects(priority).Add(new EventObject(handler, mask, priority));
        }

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler)
        {
            var eventObjectsList = new[] { highObjects, normalObjects, lowObjects };
            for (int i = 0; i < eventObjectsList.Length; i++)
            {
                var eventObjects = eventObjectsList[i];
                for (int j = 0; j < eventObjects.Count; j++)
                {
                    if (eventObjects[j].Handler == handler)
                    {
                        snapshotInvalid = true;
                        eventObjects.RemoveAt(j);
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
            for (int i = 0; i < eventObjects.Count; i++)
            {
                var obj = eventObjects[i];
                if (obj.Handler == handler)
                {
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
            if (snapshotInvalid)
            {
                length = highObjects.Count + normalObjects.Count + lowObjects.Count;
                if (eventObjectsSnapshot.Length != length)
                {
                    eventObjectsSnapshot = new EventObject[length];
                }
                highObjects.CopyTo(eventObjectsSnapshot, 0);
                normalObjects.CopyTo(eventObjectsSnapshot, highObjects.Count);
                lowObjects.CopyTo(eventObjectsSnapshot, highObjects.Count + normalObjects.Count);
                snapshotInvalid = false;
            }
            else
            {
                length = eventObjectsSnapshot.Length;
            }
            
            for (int i = 0; i < length; i++)
            {
                var obj = eventObjectsSnapshot[i];
                if ((obj.Mask & e.Type) == e.Type)
                {
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
        }

        /// <summary>
        /// Gets the list of event objects with the specified priority.
        /// </summary>
        private static List<EventObject> GetEventObjects(HandlingPriority priority)
        {
            switch (priority)
            {
                case HandlingPriority.High:
                    return highObjects;
                case HandlingPriority.Normal:
                    return normalObjects;
                case HandlingPriority.Low:
                    return lowObjects;
                default:
                    throw new InvalidEnumArgumentException(nameof(priority), (int) priority, typeof(HandlingPriority));
            }
        }

        private sealed class EventObject
        {
            internal IEventHandler Handler;
            internal HandlingPriority Priority;
            internal EventType Mask;

            internal EventObject(IEventHandler handler, EventType mask, HandlingPriority priority)
            {
                this.Handler = handler;
                this.Priority = priority;
                this.Mask = mask;
            }
        }
    }
}
