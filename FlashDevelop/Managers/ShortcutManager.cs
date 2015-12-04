using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    static class ShortcutManager
    {
        public static readonly List<Keys> AllShortcuts;
        public static readonly List<ToolStripItem> SecondaryItems;
        public static readonly Dictionary<string, ShortcutItem> RegisteredItems;

        static ShortcutManager()
        {
            AllShortcuts = new List<Keys>();
            SecondaryItems = new List<ToolStripItem>();
            RegisteredItems = new Dictionary<string, ShortcutItem>();
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(string key, Keys keys)
        {
            RegisteredItems.Add(key, new ShortcutItem(key, keys));
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(string key, ToolStripMenuItem item)
        {
            RegisteredItems.Add(key, new ShortcutItem(key, item.ShortcutKeys));
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        public static void RegisterSecondaryItem(ToolStripItem item)
        {
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Registers a secondary item.
        /// </summary>
        public static void RegisterSecondaryItem(string id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ShortcutItem GetRegisteredItem(string id)
        {
            ShortcutItem item;
            return RegisteredItems.TryGetValue(id, out item) ? item : null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ShortcutItem GetRegisteredItem(Keys keys)
        {
            if (keys == Keys.None) return null;
            foreach (var item in RegisteredItems.Values)
            {
                if (item.Custom == keys) return item;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ToolStripItem GetSecondaryItem(string id)
        {
            foreach (var item in SecondaryItems)
            {
                string temp = string.Empty;
                string[] ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length == 2 && string.IsNullOrEmpty(ids[1]))
                {
                    temp = StripBarManager.GetMenuItemId(item);
                }
                else if (ids.Length == 2) temp = ids[1];
                if (!string.IsNullOrEmpty(temp) && temp == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the list of all shortcuts.
        /// </summary>
        public static void UpdateAllShortcuts()
        {
            foreach (var item in RegisteredItems.Values)
            {
                if (AllShortcuts.Contains(item.Custom)) continue;
                AllShortcuts.Add(item.Custom);
            }
        }

        /// <summary>
        /// Applies all shortcuts to the items.
        /// </summary>
        public static void ApplyAllShortcuts()
        {
            UpdateAllShortcuts();
            foreach (var item in RegisteredItems.Values)
            {
                if (item.Item != null)
                {
                    item.Item.ShortcutKeys = item.Custom;
                }
                else if (item.Default != item.Custom)
                {
                    ScintillaControl.UpdateShortcut(item.Id, item.Custom);
                    EventManager.DispatchEvent(Globals.MainForm, new DataEvent(EventType.Shortcut, item.Id, item.Custom));
                }
            }
            foreach (var button in SecondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id.
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            bool view = Globals.Settings.ViewShortcuts;
            if (item == null || item.Tag == null) return;
            string id;
            string[] ids = ((ItemData) item.Tag).Id.Split(';');
            if (ids.Length == 2 && string.IsNullOrEmpty(ids[1]))
            {
                id = StripBarManager.GetMenuItemId(item);
            }
            else if (ids.Length == 2) id = ids[1];
            else return; // No work for us here...
            var keys = Globals.MainForm.GetShortcutItemKeys(id);
            if (keys == Keys.None) return;
            if (item is ToolStripMenuItem)
            {
                var casted = item as ToolStripMenuItem;
                if (casted.ShortcutKeys != Keys.None) return;
                string keytext = DataConverter.KeysToString(keys);
                casted.ShortcutKeyDisplayString = view ? keytext : string.Empty;
            }
            else
            {
                int end = item.ToolTipText.IndexOf(" (", StringComparison.Ordinal);
                string keytext = view ? " (" + DataConverter.KeysToString(keys) + ")" : string.Empty;
                if (end != -1) item.ToolTipText = item.ToolTipText.Substring(0, end) + keytext;
                else item.ToolTipText = item.ToolTipText + keytext;
            }
        }

        /// <summary>
        /// Loads the custom shorcuts from a file.
        /// </summary>
        public static void LoadCustomShortcuts()
        {
            ScintillaControl.InitShortcuts();
            string file = FileNameHelper.ShortcutData;
            if (!File.Exists(file)) return;
            var shortcuts = new List<Argument>();
            shortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, shortcuts, false);
            foreach (var arg in shortcuts)
            {
                var item = GetRegisteredItem(arg.Key);
                if (item != null) item.Custom = (Keys) Enum.Parse(typeof(Keys), arg.Value);
            }
        }

        /// <summary>
        /// Saves the custom shorcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts()
        {
            var shortcuts = new List<Argument>();
            foreach (var item in RegisteredItems.Values)
            {
                if (item.Custom != item.Default)
                {
                    shortcuts.Add(new Argument(item.Id, item.Custom.ToString()));
                }
            }
            string file = FileNameHelper.ShortcutData;
            ObjectSerializer.Serialize(file, shortcuts);
        }

    }

    #region Helper Classes

    /// <summary>
    /// Represents a shortcut item.
    /// </summary>
    public class ShortcutItem
    {
        public readonly string Id;
        public Keys Custom;
        public Keys Default;
        public ToolStripMenuItem Item;

        public ShortcutItem(string id, Keys keys)
        {
            Id = id;
            Default = Custom = keys;
        }

        public override string ToString()
        {
            return Id;
        }
    }

    #endregion

}
