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
        private const int PriorityCount = 3;
        private static List<EventObject>[] eventObjects;

        static EventManager()
        {
            eventObjects = new List<EventObject>[PriorityCount]
            {
                new List<EventObject>(),
                new List<EventObject>(),
                new List<EventObject>(),
            };
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
            GetObjects(priority).Add(new EventObject(handler, mask, priority));
        }

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler)
        {
            for (int i = 0; i < PriorityCount; i++)
            {
                var objects = eventObjects[i];
                for (int j = 0; j < objects.Count; j++)
                {
                    if (objects[j].Handler == handler)
                    {
                        objects.RemoveAt(j);
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
            var objects = GetObjects(priority);
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                if (obj.Handler == handler)
                {
                    obj.Mask &= ~mask;
                    if (obj.Mask == 0)
                    {
                        objects.RemoveAt(i);
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
            for (int i = 0; i < PriorityCount; i++)
            {
                var objects = eventObjects[i];
                for (int j = 0; j < objects.Count; j++)
                {
                    var obj = objects[j];
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
                        if (e.Handled) return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list of event objects with the specified priority.
        /// </summary>
        private static List<EventObject> GetObjects(HandlingPriority priority)
        {
            switch (priority)
            {
                case HandlingPriority.High:
                case HandlingPriority.Normal:
                case HandlingPriority.Low:
                    return eventObjects[(int) priority];
                default:
                    throw new InvalidEnumArgumentException(nameof(priority), (int) priority, typeof(HandlingPriority));
            }
        }

        private class EventObject
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
