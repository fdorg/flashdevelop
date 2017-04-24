using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private const string VersionKey = "version";
        private const int CurrentShortcutFileVersion = 1;
        private const int ExtendedShortcutMinimumFileVersion = 1;

        private static readonly HashSet<ShortcutKeys> allShortcuts;
        private static readonly HashSet<Keys> altFirstKeys;
        private static readonly List<ShortcutKeys> ignoredKeys;
        private static readonly List<ToolStripItem> secondaryItems;
        private static readonly List</*Dictionary<string, */ShortcutItem> registeredItems;
        private static readonly Dictionary<ShortcutKeys, ShortcutItem> cachedItems;

        static ShortcutManager()
        {
            allShortcuts = new HashSet<ShortcutKeys>();
            altFirstKeys = new HashSet<Keys>();
            ignoredKeys = new List<ShortcutKeys>();
            secondaryItems = new List<ToolStripItem>();
            registeredItems = new List</*Dictionary<string, */ShortcutItem>();
            cachedItems = new Dictionary<ShortcutKeys, ShortcutItem>();
        }

        /// <summary>
        /// Gets a collection of all shortcut keys.
        /// </summary>
        internal static HashSet<ShortcutKeys> AllShortcuts
        {
            get { return allShortcuts; }
        }

        /// <summary>
        /// Gets a collection of the first parts of all extended shortcuts with only <see cref="Keys.Alt"/> as their modifiers in the first parts.
        /// </summary>
        internal static HashSet<Keys> AltFirstKeys
        {
            get { return altFirstKeys; }
        }

        /// <summary>
        /// Gets a list of ignored keys.
        /// </summary>
        internal static List<ShortcutKeys> IgnoredKeys
        {
            get { return ignoredKeys; }
        }

        /// <summary>
        /// Gets a collection of all registered shortcut items.
        /// </summary>
        internal static ICollection<ShortcutItem> RegisteredItems
        {
            get { return registeredItems/*.Values*/; }
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        internal static void RegisterItem(string key, ShortcutKeys keys, bool supportsExtended)
        {
            registeredItems.Add(/*key, */new ShortcutItem(key, keys, supportsExtended));
        }

        /// <summary>
        /// Registers a shortcut item.
        /// <para/>
        /// [deprecated] Use the <see cref="RegisterItem(string, ToolStripMenuItemEx)"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void RegisterItem(string key, ToolStripMenuItem item)
        {
            if (item is ToolStripMenuItemEx)
            {
                registeredItems.Add(/*key, */new ShortcutItem(key, (ToolStripMenuItemEx) item));
            }
            else
            {
                registeredItems.Add(/*key, */new ShortcutItem(key, item));
            }
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        internal static void RegisterItem(string key, ToolStripMenuItemEx item)
        {
            registeredItems.Add(/*key, */new ShortcutItem(key, item));
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        internal static void RegisterSecondaryItem(ToolStripItem item)
        {
            secondaryItems.Add(item);
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        internal static void RegisterSecondaryItem(string id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            secondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        internal static ShortcutItem GetRegisteredItem(string id)
        {
            //ShortcutItem item;
            //return registeredItems.TryGetValue(id, out item) ? item : null;
            foreach (var item in registeredItems)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        internal static ShortcutItem GetRegisteredItem(ShortcutKeys keys)
        {
            ShortcutItem item;
            return !keys.IsNone && cachedItems.TryGetValue(keys, out item) ? item : null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        internal static ToolStripItem GetSecondaryItem(string id)
        {
            foreach (var item in secondaryItems)
            {
                if (item != null && item.Tag is ItemData)
                {
                    string[] ids = ((ItemData) item.Tag).Id.Split(';');
                    if (ids.Length == 2)
                    {
                        string itemId = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
                        if (itemId == id)
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }
        
        /// <summary>
        /// Applies all shortcuts to the items.
        /// </summary>
        internal static void ApplyAllShortcuts()
        {
            allShortcuts.Clear();
            altFirstKeys.Clear();
            cachedItems.Clear();
            foreach (var item in registeredItems/*.Values*/)
            {
                var keys = item.Custom;
                if (!keys.IsNone)
                {
                    allShortcuts.Add(keys);
                    cachedItems.Add(keys, item);
                    if (keys.IsExtended && (keys.First & Keys.Modifiers) == Keys.Alt)
                    {
                        altFirstKeys.Add(keys.First);
                    }
                }

                if (item.ItemEx != null)
                {
                    item.ItemEx.ShortcutKeys = keys;
                }
                else if (item.Item != null)
                {
                    item.Item.ShortcutKeys = (Keys) keys;
                }
                else if (item.Default != keys)
                {
                    if (!ScintillaControl.UpdateShortcut(item.Id, keys))
                    {
                        var de = new DataEvent(EventType.Shortcut, item.Id, keys);
                        EventManager.DispatchEvent(Globals.MainForm, de);
                    }
                }
            }
            foreach (var item in ignoredKeys)
            {
                allShortcuts.Add(item);
            }
            foreach (var button in secondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id.
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            bool viewShortcuts = Globals.Settings.ViewShortcuts;
            if (item != null && item.Tag is ItemData)
            {
                string[] ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length == 2)
                {
                    string itemId = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
                    var keys = Globals.MainForm.GetShortcutKeys(itemId);
                    bool showShortcuts = viewShortcuts && !keys.IsNone;
                    if (item is ToolStripMenuItemEx)
                    {
                        var casted = item as ToolStripMenuItemEx;
                        if (casted.ShortcutKeys.IsNone)
                        {
                            casted.ShortcutKeyDisplayString = showShortcuts ? keys.ToString() : null;
                        }
                    }
                    else if (item is ToolStripMenuItem)
                    {
                        var casted = item as ToolStripMenuItem;
                        if (casted.ShortcutKeys == Keys.None)
                        {
                            casted.ShortcutKeyDisplayString = showShortcuts ? keys.ToString() : null;
                        }
                    }
                    else
                    {
                        string shortcutText = showShortcuts ? " (" + keys + ")" : null;
                        int shortcutTextIndex = item.ToolTipText.LastIndexOfOrdinal(" (");
                        if (shortcutTextIndex >= 0)
                        {
                            item.ToolTipText = item.ToolTipText.Substring(0, shortcutTextIndex) + shortcutText;
                        }
                        else
                        {
                            item.ToolTipText += shortcutText;
                        }
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
            if (!File.Exists(file))
            {
                if (File.Exists(FileNameHelper.ShortcutDataOld))
                {
                    File.Move(FileNameHelper.ShortcutDataOld, file);
                }
                else return;
            }
            var shortcuts = new List<Argument>();
            shortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, shortcuts, false);

            int version = 0;
            int i = 0;
            int count = shortcuts.Count;

            if (count > 0 && shortcuts[0].Key == VersionKey)
            {
                version = int.Parse(shortcuts[0].Value);
                i = 1;
            }

            for (; i < count; i++)
            {
                var arg = shortcuts[i];
                var item = GetRegisteredItem(arg.Key);
                if (item != null)
                {
                    if (version >= ExtendedShortcutMinimumFileVersion)
                    {
                        item.Custom = ShortcutKeys.Parse(arg.Value);
                    }
                    else
                    {
                        item.Custom = (Keys) Enum.Parse(typeof(Keys), arg.Value); // for backward compatibility
                    }
                }
            }
        }

        /// <summary>
        /// Saves the custom shortcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts()
        {
            var shortcuts = new List<Argument>();
            shortcuts.Add(new Argument(VersionKey, CurrentShortcutFileVersion.ToString()));
            foreach (var item in registeredItems/*.Values*/)
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
                    var customShortcuts = new List<Argument>();
                    customShortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, customShortcuts, false);

                    int version = 0;
                    int start = 0;
                    int count = customShortcuts.Count;

                    if (count > 0 && customShortcuts[0].Key == VersionKey)
                    {
                        version = int.Parse(customShortcuts[0].Value);
                        start = 1;
                    }

                    foreach (var item in items)
                    {
                        var newShortcut = item.Default;
                        for (int i = start; i < count; i++)
                        {
                            var arg = customShortcuts[i];
                            if (arg.Key == item.Id)
                            {
                                if (version >= ExtendedShortcutMinimumFileVersion)
                                {
                                    newShortcut = ShortcutKeys.Parse(arg.Value);
                                }
                                else
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
                var shortcuts = new List<Argument>();
                shortcuts.Add(new Argument(VersionKey, CurrentShortcutFileVersion.ToString()));
                foreach (var item in items)
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
        internal readonly string Id;
        internal readonly bool SupportsExtended;
        internal readonly ToolStripMenuItem Item;
        internal readonly ToolStripMenuItemEx ItemEx;
        internal readonly ShortcutKeys Default;
        internal ShortcutKeys Custom;

        internal ShortcutItem(string id, ShortcutKeys keys, bool supportsExtended)
        {
            Id = id;
            Item = null;
            ItemEx = null;
            Default = Custom = keys;
            SupportsExtended = supportsExtended;
        }

        /// <summary>
        /// [deprecated] Use the <see cref="ShortcutItem(string, ToolStripMenuItemEx)"/> constructor instead.
        /// </summary>
        [Obsolete("This constructor has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal ShortcutItem(string id, ToolStripMenuItem item)
        {
            Id = id;
            Item = item;
            ItemEx = null;
            Default = Custom = (ShortcutKeys) Item.ShortcutKeys;
            SupportsExtended = false;
        }

        internal ShortcutItem(string id, ToolStripMenuItemEx item)
        {
            Id = id;
            Item = item;
            ItemEx = item;
            Default = Custom = ItemEx.ShortcutKeys;
            SupportsExtended = true;
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
