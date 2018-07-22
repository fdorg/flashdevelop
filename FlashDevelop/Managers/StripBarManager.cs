using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    static class StripBarManager
    {
        public static List<ToolStripItem> Items = new List<ToolStripItem>();

        /// <summary>
        /// Finds the tool or menu strip item by name or text
        /// </summary>
        public static ToolStripItem FindMenuItem(String name)
        {
            for (Int32 i = 0; i < Items.Count; i++)
            {
                ToolStripItem item = Items[i];
                if (item.Name == name) return item;
            }
            ShortcutItem item2 = ShortcutManager.GetRegisteredItem(name);
            if (item2 != null) return item2.Item;
            ToolStripItem item3 = ShortcutManager.GetSecondaryItem(name);
            return item3;
        }

        /// <summary>
        /// Finds the tool or menu strip items by name or text
        /// </summary>
        public static List<ToolStripItem> FindMenuItems(String name)
        {
            List<ToolStripItem> found = new List<ToolStripItem>();
            for (Int32 i = 0; i < Items.Count; i++)
            {
                ToolStripItem item = Items[i];
                if (item.Name == name) found.Add(item);
            }
            ShortcutItem item2 = ShortcutManager.GetRegisteredItem(name);
            if (item2 != null) found.Add(item2.Item);
            ToolStripItem item3 = ShortcutManager.GetSecondaryItem(name);
            if (item3 != null) found.Add(item3);
            return found;
        }

        /// <summary>
        /// Gets a tool strip from the specified xml file
        /// </summary>
        public static ToolStrip GetToolStrip(String file)
        {
            ToolStripEx toolStrip = new ToolStripEx();            
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            XmlNode rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode subNode in rootNode.ChildNodes)
            {
                FillToolItems(toolStrip.Items, subNode);
            }
            return toolStrip;
        }

        /// <summary>
        /// Gets a context menu strip from the specified xml file
        /// </summary>
        public static ContextMenuStrip GetContextMenu(String file)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            XmlNode rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode subNode in rootNode.ChildNodes)
            {
                FillMenuItems(contextMenu.Items, subNode);
            }
            return contextMenu;
        }

        /// <summary>
        /// Gets a menu strip from the specified xml file
        /// </summary>
        public static MenuStrip GetMenuStrip(String file)
        {
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            XmlNode rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode subNode in rootNode.ChildNodes)
            {
                FillMenuItems(menuStrip.Items, subNode);
            }
            return menuStrip;
        }

        /// <summary>
        /// Fills items to the specified tool strip item collection
        /// </summary>
        public static void FillMenuItems(ToolStripItemCollection items, XmlNode node)
        {
            switch (node.Name)
            {
                case "menu" :
                    String name = XmlHelper.GetAttribute(node, "name");
                    if (name == "SyntaxMenu") node.InnerXml = GetSyntaxMenuXml();
                    items.Add(GetMenu(node));
                    break;
                case "separator" :
                    items.Add(GetSeparator(node));
                    break;
                case "button" :
                    ToolStripMenuItem menu = GetMenuItem(node);
                    items.Add(menu); // Add menu first to get the id correct
                    String id = GetMenuItemId(menu);
                    if (id.Contains('.') && ShortcutManager.GetRegisteredItem(id) == null)
                    {
                        ShortcutManager.RegisterItem(id, menu);
                    }
                    else ShortcutManager.RegisterSecondaryItem(menu);
                    break;
            }
        }

        /// <summary>
        /// Fills items to the specified tool strip item collection
        /// </summary>
        public static void FillToolItems(ToolStripItemCollection items, XmlNode node)
        {
            switch (node.Name)
            {
                case "separator":
                    items.Add(GetSeparator(node));
                    break;
                case "button":
                    ToolStripItem button = GetButtonItem(node);
                    items.Add(button); // Add button first to get the id correct
                    ShortcutManager.RegisterSecondaryItem(button);
                    break;
            }
        }

        /// <summary>
        /// Gets a menu from the specified xml node
        /// </summary>
        public static ToolStripMenuItem GetMenu(XmlNode node)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            String name = XmlHelper.GetAttribute(node, "name");
            String image = XmlHelper.GetAttribute(node, "image");
            String label = XmlHelper.GetAttribute(node, "label");
            String click = XmlHelper.GetAttribute(node, "click");
            String flags = XmlHelper.GetAttribute(node, "flags");
            String enabled = XmlHelper.GetAttribute(node, "enabled");
            String tag = XmlHelper.GetAttribute(node, "tag");
            menu.Tag = new ItemData(label, tag, flags);
            menu.Text = GetLocalizedString(label);
            if (name != null) menu.Name = name; // Use the given name
            else menu.Name = label.Replace("Label.", ""); // Assign from id
            if (enabled != null) menu.Enabled = Convert.ToBoolean(enabled);
            if (image != null) menu.Image = Globals.MainForm.FindImage(image);
            if (click != null) menu.Click += GetEventHandler(click);
            foreach (XmlNode subNode in node.ChildNodes)
            {
                FillMenuItems(menu.DropDownItems, subNode);
            }
            Items.Add(menu);
            return menu;
        }

        /// <summary>
        /// Gets a button item from the specified xml node
        /// </summary>
        public static ToolStripItem GetButtonItem(XmlNode node)
        {
            ToolStripButton button = new ToolStripButton();
            String name = XmlHelper.GetAttribute(node, "name");
            String image = XmlHelper.GetAttribute(node, "image");
            String label = XmlHelper.GetAttribute(node, "label");
            String click = XmlHelper.GetAttribute(node, "click");
            String flags = XmlHelper.GetAttribute(node, "flags");
            String enabled = XmlHelper.GetAttribute(node, "enabled");
            String keyId = XmlHelper.GetAttribute(node, "keyid");
            String tag = XmlHelper.GetAttribute(node, "tag");
            button.Tag = new ItemData(label + ";" + keyId, tag, flags);
            if (name != null) button.Name = name; // Use the given name
            else button.Name = label.Replace("Label.", ""); // Assign from id
            String stripped = GetStrippedString(GetLocalizedString(label), false);
            if (image != null) button.ToolTipText = stripped;
            else button.Text = stripped; // Use text instead...
            if (enabled != null) button.Enabled = Convert.ToBoolean(enabled);
            if (image != null) button.Image = Globals.MainForm.FindImage(image);
            if (click != null) button.Click += GetEventHandler(click);
            Items.Add(button);
            return button;
        }

        /// <summary>
        /// Get a menu item from the specified xml node
        /// </summary>
        public static ToolStripMenuItem GetMenuItem(XmlNode node)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            String name = XmlHelper.GetAttribute(node, "name");
            String image = XmlHelper.GetAttribute(node, "image");
            String label = XmlHelper.GetAttribute(node, "label");
            String click = XmlHelper.GetAttribute(node, "click");
            String enabled = XmlHelper.GetAttribute(node, "enabled");
            String shortcut = XmlHelper.GetAttribute(node, "shortcut");
            String keytext = XmlHelper.GetAttribute(node, "keytext");
            String keyId = XmlHelper.GetAttribute(node, "keyid");
            String flags = XmlHelper.GetAttribute(node, "flags");
            String tag = XmlHelper.GetAttribute(node, "tag");
            menu.Tag = new ItemData(label + ";" + keyId, tag, flags);
            menu.Text = GetLocalizedString(label);
            if (name != null) menu.Name = name; // Use the given name
            else menu.Name = label.Replace("Label.", ""); // Assign from id
            if (image != null) menu.Image = Globals.MainForm.FindImage(image);
            if (enabled != null) menu.Enabled = Convert.ToBoolean(enabled);
            if (keytext != null) menu.ShortcutKeyDisplayString = GetKeyText(keytext);
            if (click != null) menu.Click += GetEventHandler(click);
            if (shortcut != null) menu.ShortcutKeys = GetKeys(shortcut);
            Items.Add(menu);
            return menu;
        }

        /// <summary>
        /// Gets a new tool strip separetor item
        /// </summary>
        public static ToolStripSeparator GetSeparator(XmlNode node)
        {
            return new ToolStripSeparator();
        }

        /// <summary>
        /// Gets the dynamic syntax menu xml (easy integration :)
        /// </summary>
        private static String GetSyntaxMenuXml()
        {
            String syntaxXml = "";
            String[] syntaxFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            String xmlTmpl = "<button label=\"{0}\" click=\"ChangeSyntax\" tag=\"{1}\" image=\"559\" flags=\"Enable:IsEditable+Check:IsEditable|IsActiveSyntax\" />";
            for (Int32 i = 0; i < syntaxFiles.Length; i++)
            {
                String fileName = Path.GetFileNameWithoutExtension(syntaxFiles[i]);
                syntaxXml += String.Format(xmlTmpl, fileName, fileName.ToLower());
            }
            return syntaxXml;
        }

        /// <summary>
        /// Strips normal label characters from the string
        /// </summary>
        private static String GetStrippedString(String text, Boolean removeWhite)
        {
            text = TextHelper.RemoveMnemonicsAndEllipsis(text);
            if (removeWhite)
            {
                text = text.Replace(" ", "");
                text = text.Replace("\t", "");
            }
            return text;
        }

        /// <summary>
        /// Gets a localized string if available
        /// </summary>
        private static String GetLocalizedString(String key)
        {
            try
            {
                if (!key.StartsWithOrdinal("Label.")) return key;
                else return TextHelper.GetString(key);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the id of the menu item from owner tree
        /// </summary>
        public static String GetMenuItemId(ToolStripItem menu)
        {
            if (menu.OwnerItem != null)
            {
                ToolStripItem parent = menu.OwnerItem;
                return GetMenuItemId(parent) + "." + GetStrippedString(menu.Name, true);
            }
            else return GetStrippedString(menu.Name, true);
        }

        /// <summary>
        /// Gets a shortcut key string from a string
        /// </summary>
        private static String GetKeyText(String data)
        {
            data = data.Replace("|", "+");
            data = data.Replace("Control", "Ctrl");
            return data;
        }

        /// <summary>
        /// Gets a shortcut keys from a string
        /// </summary>
        private static Keys GetKeys(String data)
        {
            try
            {
                Keys shortcut = Keys.None;
                String[] keys = data.Split('|');
                for (Int32 i = 0; i < keys.Length; i++) shortcut = shortcut | (Keys)Enum.Parse(typeof(Keys), keys[i]);
                return shortcut;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return Keys.None;
            }
        }

        /// <summary>
        /// Gets a click handler from a string
        /// </summary>
        private static EventHandler GetEventHandler(String method)
        {
            try
            {
                return (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), Globals.MainForm, method);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

    }

}
