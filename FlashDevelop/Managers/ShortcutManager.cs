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
        public static readonly Dictionary<String, ShortcutItem> RegisteredItems;

        static ShortcutManager()
        {
            AllShortcuts = new List<Keys>();
            SecondaryItems = new List<ToolStripItem>();
            RegisteredItems = new Dictionary<String, ShortcutItem>();
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(String key, Keys keys)
        {
            RegisteredItems.Add(key, new ShortcutItem(key, keys));
        }

        /// <summary>
        /// Registers a shortcut item.
        /// </summary>
        public static void RegisterItem(String key, ToolStripMenuItem item)
        {
            RegisteredItems.Add(key, new ShortcutItem(key, item));
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
        public static void RegisterSecondaryItem(String id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ShortcutItem GetRegisteredItem(String id)
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
            foreach (ShortcutItem item in RegisteredItems.Values)
            {
                if (item.Custom == keys) return item;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        public static ToolStripItem GetSecondaryItem(String id)
        {
            foreach (ToolStripItem item in SecondaryItems)
            {
                String[] ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length == 2)
                {
                    String temp = String.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
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
            foreach (ShortcutItem item in RegisteredItems.Values)
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
            foreach (ShortcutItem item in RegisteredItems.Values)
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
            foreach (ToolStripItem button in SecondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id.
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            if (item == null || item.Tag == null) return;

            String id;
            String[] ids = ((ItemData) item.Tag).Id.Split(';');
            if (ids.Length == 2 && String.IsNullOrEmpty(ids[1]))
            {
                id = StripBarManager.GetMenuItemId(item);
            }
            else if (ids.Length == 2) id = ids[1];
            else return; // No work for us here...

            Keys keys = Globals.MainForm.GetShortcutItemKeys(id);
            if (keys == Keys.None) return;

            Boolean view = Globals.Settings.ViewShortcuts;
            if (item is ToolStripMenuItem)
            {
                ToolStripMenuItem casted = item as ToolStripMenuItem;
                if (casted.ShortcutKeys != Keys.None) return;
                String keytext = DataConverter.KeysToString(keys);
                casted.ShortcutKeyDisplayString = view ? keytext : String.Empty;
            }
            else
            {
                Int32 end = item.ToolTipText.IndexOf(" (", StringComparison.Ordinal);
                String keytext = view ? " (" + DataConverter.KeysToString(keys) + ")" : String.Empty;
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
            String file = FileNameHelper.ShortcutData;
            if (!File.Exists(file)) return;
            List<Argument> shortcuts = new List<Argument>();
            shortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, shortcuts, false);
            foreach (Argument arg in shortcuts)
            {
                ShortcutItem item = GetRegisteredItem(arg.Key);
                if (item != null) item.Custom = (Keys) Enum.Parse(typeof(Keys), arg.Value);
            }
        }

        /// <summary>
        /// Saves the custom shorcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts()
        {
            List<Argument> shortcuts = new List<Argument>();
            foreach (ShortcutItem item in RegisteredItems.Values)
            {
                if (item.Custom != item.Default)
                {
                    shortcuts.Add(new Argument(item.Id, item.Custom.ToString()));
                }
            }
            String file = FileNameHelper.ShortcutData;
            ObjectSerializer.Serialize(file, shortcuts);
        }

    }

    #region Helper Classes

    /// <summary>
    /// Represents a shortcut item.
    /// </summary>
    public class ShortcutItem
    {
        public readonly String Id;
        public Keys Custom;
        public Keys Default;
        public ToolStripMenuItem Item;

        public ShortcutItem(String id, Keys keys)
        {
            Id = id;
            Default = Custom = keys;
        }

        public ShortcutItem(String id, ToolStripMenuItem item)
        {
            Id = id;
            Item = item;
            Default = Custom = item.ShortcutKeys;
        }

        public override String ToString()
        {
            return Id;
        }
    }

    #endregion

}
