using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static readonly List<Keys> AllShortcuts = new List<Keys>();
        public static readonly List<ToolStripItem> SecondaryItems = new List<ToolStripItem>();
        public static readonly Dictionary<string, ShortcutItem> RegisteredItems = new Dictionary<string, ShortcutItem>();

        /// <summary>
        /// Registers a shortcut item
        /// </summary>
        public static void RegisterItem(string key, Keys keys) => RegisteredItems.Add(key, new ShortcutItem(key, keys));

        /// <summary>
        /// Registers a shortcut item
        /// </summary>
        public static void RegisterItem(string key, ToolStripMenuItem item) => RegisteredItems.Add(key, new ShortcutItem(key, item));

        /// <summary>
        /// Registers a secondary item
        /// </summary>
        public static void RegisterSecondaryItem(ToolStripItem item) => SecondaryItems.Add(item);

        /// <summary>
        /// Registers a secondary item
        /// </summary>
        public static void RegisterSecondaryItem(string id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item
        /// </summary>
        public static ShortcutItem? GetRegisteredItem(string id) => RegisteredItems.TryGetValue(id, out var item) ? item : null;

        /// <summary>
        /// Gets the specified registered shortcut item
        /// </summary>
        public static ShortcutItem? GetRegisteredItem(Keys keys)
        {
            return keys != Keys.None
                ? RegisteredItems.Values.FirstOrDefault(item => item.Custom == keys)
                : null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item
        /// </summary>
        public static ToolStripItem? GetSecondaryItem(string id)
        {
            foreach (var item in SecondaryItems)
            {
                var ids = ((ItemData) item.Tag).Id.Split(';');
                if (ids.Length != 2) continue;
                var temp = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
                if (temp == id) return item;
            }
            return null;
        }

        /// <summary>
        /// Updates the list of all shortcuts
        /// </summary>
        public static void UpdateAllShortcuts()
        {
            foreach (var item in RegisteredItems.Values)
            {
                if (!AllShortcuts.Contains(item.Custom))
                {
                    AllShortcuts.Add(item.Custom);
                }
            }
        }

        /// <summary>
        /// Applies all shortcuts to the items
        /// </summary>
        public static void ApplyAllShortcuts()
        {
            UpdateAllShortcuts();
            foreach (var item in RegisteredItems.Values)
            {
                if (item.Item != null)
                {
                    item.Item.ShortcutKeys = Keys.None;
                    item.Item.ShortcutKeys = item.Custom;
                }
                else if (item.Default != item.Custom)
                {
                    ScintillaControl.UpdateShortcut(item.Id, item.Custom);
                    var de = new DataEvent(EventType.Shortcut, item.Id, item.Custom);
                    EventManager.DispatchEvent(PluginBase.MainForm, de);
                }
            }
            foreach (var button in SecondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            if (item?.Tag is null) return;
            string id;
            var ids = ((ItemData) item.Tag).Id.Split(';');
            if (ids.Length == 2) id = string.IsNullOrEmpty(ids[1]) ? StripBarManager.GetMenuItemId(item) : ids[1];
            else return; // No work for us here...
            var keys = PluginBase.MainForm.GetShortcutItemKeys(id);
            if (keys == Keys.None) return;
            var view = PluginBase.Settings.ViewShortcuts;
            if (item is ToolStripMenuItem casted)
            {
                if (casted.ShortcutKeys == Keys.None)
                {
                    string keytext = DataConverter.KeysToString(keys);
                    casted.ShortcutKeyDisplayString = view ? keytext : "";
                }
            }
            else
            {
                int end = item.ToolTipText.IndexOfOrdinal(" (");
                string keytext = view ? " (" + DataConverter.KeysToString(keys) + ")" : "";
                if (end != -1) item.ToolTipText = item.ToolTipText.Substring(0, end) + keytext;
                else item.ToolTipText += keytext;
            }
        }

        /// <summary>
        /// Loads the custom shortcuts from a file
        /// </summary>
        public static void LoadCustomShortcuts()
        {
            ScintillaControl.InitShortcuts();
            var file = FileNameHelper.ShortcutData;
            if (!File.Exists(file)) return;
            var shortcuts = ObjectSerializer.Deserialize(file, new List<Argument>(), false);
            foreach (var arg in shortcuts)
            {
                var item = GetRegisteredItem(arg.Key);
                if (item != null) item.Custom = (Keys) Enum.Parse(typeof(Keys), arg.Value);
            }
        }

        /// <summary>
        /// Saves the custom shortcuts to a file
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
            ObjectSerializer.Serialize(FileNameHelper.ShortcutData, shortcuts);
        }

        /// <summary>
        /// Loads the custom shortcuts from a file to a list.
        /// </summary>
        public static void LoadCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            if (!File.Exists(file)) return;
            try
            {
                var shortcuts = ObjectSerializer.Deserialize(file, new List<Argument>(), false);
                var count = shortcuts.Count;
                foreach (var item in items)
                {
                    var newShortcut = item.Default;
                    for (int i = 0; i < count; i++)
                    {
                        var arg = shortcuts[i];
                        if (arg.Key == item.Id)
                        {
                            newShortcut = (Keys) Enum.Parse(typeof(Keys), arg.Value);
                            shortcuts.RemoveAt(i);
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

        /// <summary>
        /// Saves the list of custom shortcuts to a file.
        /// </summary>
        public static void SaveCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            try
            {
                var shortcuts = new List<Argument>();
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

    public class ShortcutItem
    {
        public string Id;
        public Keys Default;
        public Keys Custom;
        public ToolStripMenuItem Item;

        public ShortcutItem(string id, Keys keys)
        {
            Id = id;
            Default = Custom = keys;
        }

        public ShortcutItem(string id, ToolStripMenuItem item)
        {
            Id = id;
            Item = item;
            Default = Custom = item.ShortcutKeys;
        }

        public override string ToString() => Id;
    }

    public interface IShortcutItem
    {
        string Id { get; }
        Keys Default { get; }
        Keys Custom { get; set; }
        bool IsModified { get; }
    }

    #endregion
}