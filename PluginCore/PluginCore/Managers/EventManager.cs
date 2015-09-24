using System;
using System.Collections.Generic;

namespace PluginCore.Managers
{
    public class EventManager
    {
        /// <summary>
        /// Properties of the class
        /// </summary>
        private static List<EventObject> highObjects;
        private static List<EventObject> normalObjects;
        private static List<EventObject> lowObjects;

        /// <summary>
        /// Static constructor of the class
        /// </summary>
        static EventManager()
        {
            highObjects = new List<EventObject>();
            normalObjects = new List<EventObject>();
            lowObjects = new List<EventObject>();
        }

        /// <summary>
        /// Gets the event lists as an array
        /// </summary>
        private static List<EventObject>[] GetObjectListCollection()
        {
            List<EventObject>[] collection = new List<EventObject>[3];
            collection.SetValue(highObjects, 0);
            collection.SetValue(normalObjects, 1);
            collection.SetValue(lowObjects, 2);
            return collection;
        }

        /// <summary>
        /// Adds a new event handler with a specific HandlingPriority
        /// </summary>
        public static void AddEventHandler(IEventHandler handler, EventType mask, HandlingPriority priority)
        {
            try
            {
                EventObject eo = new EventObject(handler, mask, priority);
                switch (priority)
                {
                    case HandlingPriority.High:
                        highObjects.Add(eo);
                        break;
                    case HandlingPriority.Normal:
                        normalObjects.Add(eo);
                        break;
                    case HandlingPriority.Low:
                        lowObjects.Add(eo);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Adds a new event handler with default HandlingPriority
        /// </summary>
        public static void AddEventHandler(IEventHandler handler, EventType mask)
        {
            AddEventHandler(handler, mask, HandlingPriority.Normal);
        }

        /// <summary>
        /// Removes the specified event handler
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler, EventType mask, HandlingPriority priority)
        {
            try
            {
                EventObject eo = new EventObject(handler, mask, priority);
                switch (priority)
                {
                    case HandlingPriority.High:
                        highObjects.Remove(eo);
                        break;
                    case HandlingPriority.Normal:
                        normalObjects.Remove(eo);
                        break;
                    case HandlingPriority.Low:
                        lowObjects.Remove(eo);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Removes all registered event handler instances
        /// </summary>
        public static void RemoveEventHandler(IEventHandler handler)
        {
            try 
            {
                List<EventObject>[] objectList = GetObjectListCollection();
                for (Int32 i = 0; i < objectList.Length; i++)
                {
                    List<EventObject> subObjects = objectList[i];
                    for (Int32 j = 0; j < subObjects.Count; j++)
                    {
                        if (subObjects[j].Handler == handler)
                        {
                            objectList[i].Remove(subObjects[j]);
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Dispatches an event to the registered event handlers
        /// </summary>
        public static void DispatchEvent(Object sender, NotifyEvent e)
        {
            try 
            {
                List<EventObject>[] objectList = GetObjectListCollection();
                for (Int32 i = 0; i < objectList.Length; i++)
                {
                    List<EventObject> subObjects = objectList[i];
                    for (Int32 j = 0; j < subObjects.Count; j++)
                    {
                        EventObject obj = subObjects[j];
                        if ((obj.Mask & e.Type) > 0)
                        {
                            obj.Handler.HandleEvent(sender, e, obj.Priority);
                            if (e.Handled) return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        class EventObject
        {
            /// <summary>
            /// Properties of the class
            /// </summary>
            public IEventHandler Handler;
            public HandlingPriority Priority;
            public EventType Mask;

            /// <summary>
            /// Constructor of the class
            /// </summary>
            public EventObject(IEventHandler handler, EventType mask, HandlingPriority priority)
            {
                this.Handler = handler;
                this.Priority = priority;
                this.Mask = mask;
            }
        }

    }

}
