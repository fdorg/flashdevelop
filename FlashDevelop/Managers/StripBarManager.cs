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
        public static ToolStripItem FindMenuItem(string name)
        {
            foreach (var item in Items)
            {
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
        public static List<ToolStripItem> FindMenuItems(string name)
        {
            var found = new List<ToolStripItem>();
            foreach (var item in Items)
            {
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
        public static ToolStrip GetToolStrip(string file)
        {
            var toolStrip = new ToolStripEx {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode subNode in rootNode.ChildNodes)
            {
                FillToolItems(toolStrip.Items, subNode);
            }
            return toolStrip;
        }

        /// <summary>
        /// Gets a context menu strip from the specified xml file
        /// </summary>
        public static ContextMenuStrip GetContextMenu(string file)
        {
            var contextMenu = new ContextMenuStrip {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode subNode in rootNode.ChildNodes)
            {
                FillMenuItems(contextMenu.Items, subNode);
            }
            return contextMenu;
        }

        /// <summary>
        /// Gets a menu strip from the specified xml file
        /// </summary>
        public static MenuStrip GetMenuStrip(string file)
        {
            var menuStrip = new MenuStrip {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
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
                    string name = XmlHelper.GetAttribute(node, "name");
                    if (name == "SyntaxMenu") node.InnerXml = GetSyntaxMenuXml();
                    items.Add(GetMenu(node));
                    break;
                case "separator" :
                    items.Add(GetSeparator(node));
                    break;
                case "button" :
                    ToolStripMenuItem menu = GetMenuItem(node);
                    items.Add(menu); // Add menu first to get the id correct
                    string id = GetMenuItemId(menu);
                    if (id.Contains('.') && ShortcutManager.GetRegisteredItem(id) is null)
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
            string name = XmlHelper.GetAttribute(node, "name");
            string image = XmlHelper.GetAttribute(node, "image");
            string label = XmlHelper.GetAttribute(node, "label");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string tag = XmlHelper.GetAttribute(node, "tag");
            menu.Tag = new ItemData(label, tag, flags);
            menu.Text = GetLocalizedString(label);
            menu.Name = name ?? label.Replace("Label.", "");
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
            string name = XmlHelper.GetAttribute(node, "name");
            string image = XmlHelper.GetAttribute(node, "image");
            string label = XmlHelper.GetAttribute(node, "label");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string keyId = XmlHelper.GetAttribute(node, "keyid");
            string tag = XmlHelper.GetAttribute(node, "tag");
            button.Tag = new ItemData(label + ";" + keyId, tag, flags);
            button.Name = name ?? label.Replace("Label.", "");
            string stripped = GetStrippedString(GetLocalizedString(label), false);
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
            string name = XmlHelper.GetAttribute(node, "name");
            string image = XmlHelper.GetAttribute(node, "image");
            string label = XmlHelper.GetAttribute(node, "label");
            string click = XmlHelper.GetAttribute(node, "click");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string shortcut = XmlHelper.GetAttribute(node, "shortcut");
            string keytext = XmlHelper.GetAttribute(node, "keytext");
            string keyId = XmlHelper.GetAttribute(node, "keyid");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string tag = XmlHelper.GetAttribute(node, "tag");
            menu.Tag = new ItemData(label + ";" + keyId, tag, flags);
            menu.Text = GetLocalizedString(label);
            menu.Name = name ?? label.Replace("Label.", "");
            if (image != null) menu.Image = Globals.MainForm.FindImage(image);
            if (enabled != null) menu.Enabled = Convert.ToBoolean(enabled);
            if (keytext != null) menu.ShortcutKeyDisplayString = GetKeyText(keytext);
            if (click != null) menu.Click += GetEventHandler(click);
            if (shortcut != null) menu.ShortcutKeys = GetKeys(shortcut);
            Items.Add(menu);
            return menu;
        }

        /// <summary>
        /// Gets a new tool strip separator item
        /// </summary>
        public static ToolStripSeparator GetSeparator(XmlNode node) => new ToolStripSeparator();

        /// <summary>
        /// Gets the dynamic syntax menu xml (easy integration :)
        /// </summary>
        private static string GetSyntaxMenuXml()
        {
            string syntaxXml = "";
            string[] syntaxFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            string xmlTmpl = "<button label=\"{0}\" click=\"ChangeSyntax\" tag=\"{1}\" image=\"559\" flags=\"Enable:IsEditable+Check:IsEditable|IsActiveSyntax\" />";
            foreach (var syntaxFile in syntaxFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(syntaxFile);
                syntaxXml += string.Format(xmlTmpl, fileName, fileName.ToLower());
            }
            return syntaxXml;
        }

        /// <summary>
        /// Strips normal label characters from the string
        /// </summary>
        private static string GetStrippedString(string text, bool removeWhite)
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
        private static string GetLocalizedString(string key)
        {
            try
            {
                if (!key.StartsWithOrdinal("Label.")) return key;
                return TextHelper.GetString(key);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the id of the menu item from owner tree
        /// </summary>
        public static string GetMenuItemId(ToolStripItem menu)
        {
            if (menu.OwnerItem != null)
            {
                ToolStripItem parent = menu.OwnerItem;
                return GetMenuItemId(parent) + "." + GetStrippedString(menu.Name, true);
            }

            return GetStrippedString(menu.Name, true);
        }

        /// <summary>
        /// Gets a shortcut key string from a string
        /// </summary>
        private static string GetKeyText(string data)
        {
            data = data.Replace("|", "+");
            data = data.Replace("Control", "Ctrl");
            return data;
        }

        /// <summary>
        /// Gets a shortcut keys from a string
        /// </summary>
        private static Keys GetKeys(string data)
        {
            try
            {
                var shortcut = Keys.None;
                var keys = data.Split('|');
                foreach (var key in keys)
                    shortcut |= (Keys)Enum.Parse(typeof(Keys), key);

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
        private static EventHandler GetEventHandler(string method)
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
