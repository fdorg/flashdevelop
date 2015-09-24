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
    class ShortcutManager
    {
        public static List<Keys> AllShortcuts = new List<Keys>();
        public static List<ToolStripItem> SecondaryItems = new List<ToolStripItem>();
        public static List<ShortcutItem> RegisteredItems = new List<ShortcutItem>();

        /// <summary>
        /// Registers a shortcut item
        /// </summary>
        public static void RegisterItem(String key, Keys keys)
        {
            ShortcutItem registered = new ShortcutItem(key, keys);
            RegisteredItems.Add(registered);
        }

        /// <summary>
        /// Registers a shortcut item
        /// </summary>
        public static void RegisterItem(String key, ToolStripMenuItem item)
        {
            ShortcutItem registered = new ShortcutItem(key, item);
            RegisteredItems.Add(registered);
        }

        /// <summary>
        /// Registers a secondary item
        /// </summary>
        public static void RegisterSecondaryItem(ToolStripItem item)
        {
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Registers a secondary item
        /// </summary>
        public static void RegisterSecondaryItem(String id, ToolStripItem item)
        {
            item.Tag = new ItemData("none;" + id, null, null);
            SecondaryItems.Add(item);
        }

        /// <summary>
        /// Gets the specified registered shortcut item
        /// </summary>
        public static ShortcutItem GetRegisteredItem(String id)
        {
            foreach (ShortcutItem item in RegisteredItems)
            {
                if (item.Id == id) return item;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item
        /// </summary>
        public static ToolStripItem GetSecondaryItem(String id)
        {
            foreach (ToolStripItem item in SecondaryItems)
            {
                String temp = String.Empty;
                String[] ids = ((ItemData)item.Tag).Id.Split(';');
                if (ids.Length == 2 && String.IsNullOrEmpty(ids[1]))
                {
                    temp = StripBarManager.GetMenuItemId(item);
                }
                else if (ids.Length == 2) temp = ids[1];
                if (!String.IsNullOrEmpty(temp) && temp == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the list of all shortcuts
        /// </summary>
        public static void UpdateAllShortcuts()
        {
            foreach (ShortcutItem item in RegisteredItems)
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
            foreach (ShortcutItem item in RegisteredItems)
            {
                if (item.Item != null)
                {
                    item.Item.ShortcutKeys = Keys.None;
                    item.Item.ShortcutKeys = item.Custom;
                }
                else if (item.Default != item.Custom)
                {
                    ScintillaControl.UpdateShortcut(item.Id, item.Custom);
                    DataEvent de = new DataEvent(EventType.Shortcut, item.Id, item.Custom);
                    EventManager.DispatchEvent(Globals.MainForm, de);
                }
            }
            foreach (ToolStripItem button in SecondaryItems)
            {
                ApplySecondaryShortcut(button);
            }
        }

        /// <summary>
        /// Applies the shortcut display string to items with keyid or same id
        /// </summary>
        public static void ApplySecondaryShortcut(ToolStripItem item)
        {
            Boolean view = Globals.Settings.ViewShortcuts;
            if (item != null && item.Tag != null)
            {
                String id = String.Empty;
                String[] ids = ((ItemData)item.Tag).Id.Split(';');
                if (ids.Length == 2 && String.IsNullOrEmpty(ids[1]))
                {
                    id = StripBarManager.GetMenuItemId(item);
                }
                else if (ids.Length == 2) id = ids[1];
                else return; // No work for us here...
                Keys keys = Globals.MainForm.GetShortcutItemKeys(id);
                if (keys != Keys.None)
                {
                    if (item is ToolStripMenuItem)
                    {
                        var casted = item as ToolStripMenuItem;
                        if (casted.ShortcutKeys == Keys.None)
                        {
                            String keytext = DataConverter.KeysToString(keys);
                            casted.ShortcutKeyDisplayString = view ? keytext : "";
                        }
                    }
                    else
                    {
                        Int32 end = item.ToolTipText.IndexOf(" (");
                        String keytext = view ? " (" + DataConverter.KeysToString(keys) + ")" : "";
                        if (end != -1) item.ToolTipText = item.ToolTipText.Substring(0, end) + keytext;
                        else item.ToolTipText = item.ToolTipText + keytext;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the custom shorcuts from a file
        /// </summary>
        public static void LoadCustomShortcuts()
        {
            ScintillaControl.InitShortcuts();
            String file = FileNameHelper.ShortcutData;
            if (File.Exists(file))
            {
                List<Argument> shortcuts = new List<Argument>();
                shortcuts = (List<Argument>)ObjectSerializer.Deserialize(file, shortcuts, false);
                foreach (Argument arg in shortcuts)
                {
                    ShortcutItem item = GetRegisteredItem(arg.Key);
                    if (item != null) item.Custom = (Keys)Enum.Parse(typeof(Keys), arg.Value);
                }
            }
        }

        /// <summary>
        /// Saves the custom shorcuts to a file
        /// </summary>
        public static void SaveCustomShortcuts()
        {
            List<Argument> shortcuts = new List<Argument>();
            foreach (ShortcutItem item in RegisteredItems)
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

    public class ShortcutItem
    {
        public Keys Custom = Keys.None;
        public Keys Default = Keys.None;
        public ToolStripMenuItem Item = null;
        public String Id = String.Empty;

        public ShortcutItem(String id, Keys keys)
        {
            this.Id = id;
            this.Default = this.Custom = keys;
        }

        public ShortcutItem(String id, ToolStripMenuItem item)
        {
            this.Id = id;
            this.Item = item;
            this.Default = this.Custom = item.ShortcutKeys;
        }

        public override string ToString()
        {
            return Id;
        }
    }

    #endregion

}
