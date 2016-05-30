using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    /// <summary>
    /// A manager class for shortcuts.
    /// </summary>
    internal static class ShortcutManager
    {
        private static readonly List<ShortcutKeys> allShortcuts;
        private static readonly List<ToolStripItem> secondaryItems;
        private static readonly List</*Dictionary<string, */ShortcutItem> registeredItems;
        private static readonly Dictionary<ShortcutKeys, ShortcutItem> cachedItems;

        static ShortcutManager()
        {
            allShortcuts = new List<ShortcutKeys>();
            secondaryItems = new List<ToolStripItem>();
            registeredItems = new List</*Dictionary<string, */ShortcutItem>();
            cachedItems = new Dictionary<ShortcutKeys, ShortcutItem>();
        }

        /// <summary>
        /// Gets a list of all shortcut keys.
        /// </summary>
        public static List<ShortcutKeys> AllShortcuts
        {
            get { return allShortcuts; }
        }

        /// <summary>
        /// Gets a collection of all registered shortcut items.
        /// </summary>
        public static ICollection<ShortcutItem> RegisteredItems
        {
            get { return registeredItems/*.Values*/; }
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(string key, ShortcutKeys keys, bool supportsExtended)
        {
            registeredItems.Add(/*key, */new ShortcutItem(key, keys, supportsExtended));
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(string key, ToolStripMenuItem item)
        {
            registeredItems.Add(/*key, */new ShortcutItem(key, item));
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        public static void RegisterSecondaryItem(ToolStripItem item)
        {
            secondaryItems.Add(item);
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        public static void RegisterSecondaryItem(string id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            secondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ShortcutItem GetRegisteredItem(string id)
        {
            //ShortcutItem item;
            //return RegisteredItems.TryGetValue(id, out item) ? item : null;
            foreach (ShortcutItem item in registeredItems)
            {
                if (item.Id == id) return item;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ShortcutItem GetRegisteredItem(ShortcutKeys keys)
        {
            ShortcutItem item;
            return !keys.IsNone && cachedItems.TryGetValue(keys, out item) ? item : null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ToolStripItem GetSecondaryItem(string id)
        {
            foreach (ToolStripItem item in secondaryItems)
            {
                string[] ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length == 2)
                {
                    string temp = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
                    if (temp == id) return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the list of all shortcuts.
        /// </summary>
        public static void UpdateAllShortcuts()
        {
            cachedItems.Clear();
            foreach (ShortcutItem item in registeredItems/*.Values*/)
            {
                var keys = item.Custom;
                if (!keys.IsNone)
                {
                    if (!allShortcuts.Contains(item.Custom))
                    {
                        allShortcuts.Add(item.Custom);
                    }
                    cachedItems.Add(item.Custom, item);
                }
            }
        }

        /// <summary>
        /// Applies all shortcuts to the items.
        /// </summary>
        public static void ApplyAllShortcuts()
        {
            UpdateAllShortcuts();
            foreach (ShortcutItem item in registeredItems/*.Values*/)
            {
                if (item.ItemEx != null)
                {
                    item.ItemEx.ShortcutKeys = item.Custom;
                }
                if (item.Item != null)
                {
                    item.Item.ShortcutKeys = (Keys) item.Custom;
                }
                else if (item.Default != item.Custom)
                {
                    ScintillaControl.UpdateShortcut(item.Id, item.Custom);
                    DataEvent de = new DataEvent(EventType.Shortcut, item.Id, item.Custom);
                    EventManager.DispatchEvent(Globals.MainForm, de);
                }
            }
            foreach (ToolStripItem button in secondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id.
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            Boolean view = Globals.Settings.ViewShortcuts;
            if (item != null && item.Tag != null)
            {
                string id = string.Empty;
                string[] ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length == 2)
                {
                    id = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
                }
                else return; // No work for us here...
                ShortcutKeys keys = Globals.MainForm.GetShortcutItemKeys(id);
                if (!keys.IsNone)
                {
                    if (item is ToolStripMenuItemEx)
                    {
                        var casted = item as ToolStripMenuItemEx;
                        if (casted.ShortcutKeys.IsNone)
                        {
                            casted.ShortcutKeyDisplayString = view ? keys.ToString() : "";
                        }
                    }
                    if (item is ToolStripMenuItem)
                    {
                        var casted = item as ToolStripMenuItem;
                        if (casted.ShortcutKeys == Keys.None)
                        {
                            casted.ShortcutKeyDisplayString = view ? keys.ToString() : "";
                        }
                    }
                    else
                    {
                        int end = item.ToolTipText.IndexOfOrdinal(" (");
                        string keytext = view ? " (" + keys + ")" : "";
                        if (end != -1) item.ToolTipText = item.ToolTipText.Substring(0, end) + keytext;
                        else item.ToolTipText += keytext;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the custom shortcuts from a file.
        /// </summary>
        public static void LoadCustomShortcuts()
        {
            ScintillaControl.InitShortcuts();
            string file = FileNameHelper.ShortcutData;
            if (File.Exists(file))
            {
                List<Argument> shortcuts = new List<Argument>();
                shortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, shortcuts, false);
                foreach (Argument arg in shortcuts)
                {
                    ShortcutItem item = GetRegisteredItem(arg.Key);
                    if (item != null)
                    {
                        try
                        {
                            item.Custom = ShortcutKeys.Parse(arg.Value);
                        }
                        catch
                        {
                            item.Custom = (Keys) Enum.Parse(typeof(Keys), arg.Value); // for backward compatibility
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the custom shortcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts()
        {
            List<Argument> shortcuts = new List<Argument>();
            foreach (ShortcutItem item in registeredItems/*.Values*/)
            {
                if (item.Custom != item.Default)
                {
                    shortcuts.Add(new Argument(item.Id, item.Custom.ToString()));
                }
            }
            string file = FileNameHelper.ShortcutData;
            ObjectSerializer.Serialize(file, shortcuts);
        }

        /// <summary>
        /// Loads the custom shortcuts from a file to a list.
        /// </summary>
        public static void LoadCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            if (File.Exists(file))
            {
                try
                {
                    List<Argument> customShortcuts = new List<Argument>();
                    customShortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, customShortcuts, false);
                    int count = customShortcuts.Count;

                    foreach (IShortcutItem item in items)
                    {
                        ShortcutKeys newShortcut = item.Default;
                        for (int i = 0; i < count; i++)
                        {
                            Argument arg = customShortcuts[i];
                            if (arg.Key == item.Id)
                            {
                                try
                                {
                                    newShortcut = ShortcutKeys.Parse(arg.Value);
                                }
                                catch
                                {
                                    newShortcut = (Keys) Enum.Parse(typeof(Keys), arg.Value); // for backward compatibility
                                }
                                customShortcuts.RemoveAt(i);
                                count--;
                                break;
                            }
                        }
                        item.Custom = newShortcut;
                    }
                }
                catch (Exception e)
                {
                    ErrorManager.ShowError(e);
                }
            }
        }

        /// <summary>
        /// Saves the list of custom shortcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            try
            {
                List<Argument> shortcuts = new List<Argument>();
                foreach (IShortcutItem item in items)
                {
                    if (item.IsModified) shortcuts.Add(new Argument(item.Id, item.Custom.ToString()));
                }
                ObjectSerializer.Serialize(file, shortcuts);
            }
            catch (Exception e)
            {
                ErrorManager.ShowError(e);
            }
        }

    }

    #region Helper Classes

    internal class ShortcutItem
    {
        public string Id;
        public bool SupportsExtended;
        public ShortcutKeys Default;
        public ShortcutKeys Custom;
        public ToolStripMenuItem Item;
        public ToolStripMenuItemEx ItemEx;

        public ShortcutItem(string id, ShortcutKeys keys, bool supportsExtended)
        {
            Id = id;
            Default = Custom = keys;
            SupportsExtended = supportsExtended;
        }

        public ShortcutItem(string id, ToolStripMenuItem item)
        {
            Id = id;
            Item = item;
            ItemEx = item as ToolStripMenuItemEx;
            SupportsExtended = ItemEx != null;
            Default = Custom = SupportsExtended ? ItemEx.ShortcutKeys : (ShortcutKeys) Item.ShortcutKeys;
        }

        public override string ToString()
        {
            return Id;
        }
    }

    // This interface is completely (maybe not completely but still) irrelevant to the ShortcutItem class above.
    // Just noting it down since I got pretty confused when I came back to it after a while.
    // This class is for the ShortcutDialog control.
    internal interface IShortcutItem
    {
        string Id { get; }
        ShortcutKeys Default { get; }
        ShortcutKeys Custom { get; set; }
        bool IsModified { get; }
    }

    #endregion

}
