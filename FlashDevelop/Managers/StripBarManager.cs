using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    internal static class StripBarManager
    {
        public static List<ToolStripItem> Items = new List<ToolStripItem>();

        /// <summary>
        /// Finds the tool or menu strip item by name or text
        /// </summary>
        public static ToolStripItem FindMenuItem(string name)
        {
            return Items.FirstOrDefault(it => it.Name == name)
                ?? ShortcutManager.GetRegisteredItem(name)?.Item
                ?? ShortcutManager.GetSecondaryItem(name);
        }

        /// <summary>
        /// Finds the tool or menu strip items by name or text
        /// </summary>
        public static List<ToolStripItem> FindMenuItems(string name)
        {
            var result = new List<ToolStripItem>();
            foreach (var item in Items)
            {
                if (item.Name == name) result.Add(item);
            }
            var item2 = ShortcutManager.GetRegisteredItem(name);
            if (item2 != null) result.Add(item2.Item);
            var item3 = ShortcutManager.GetSecondaryItem(name);
            if (item3 != null) result.Add(item3);
            return result;
        }

        /// <summary>
        /// Gets a tool strip from the specified xml file
        /// </summary>
        public static ToolStrip GetToolStrip(string file)
        {
            var result = new ToolStripEx {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                FillToolItems(result.Items, node);
            }
            return result;
        }

        /// <summary>
        /// Gets a context menu strip from the specified xml file
        /// </summary>
        public static ContextMenuStrip GetContextMenu(string file)
        {
            var result = new ContextMenuStrip {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                FillMenuItems(result.Items, node);
            }
            return result;
        }

        /// <summary>
        /// Gets a menu strip from the specified xml file
        /// </summary>
        public static MenuStrip GetMenuStrip(string file)
        {
            var result = new MenuStrip {ImageScalingSize = ScaleHelper.Scale(new Size(16, 16))};
            var rootNode = XmlHelper.LoadXmlDocument(file);
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                FillMenuItems(result.Items, node);
            }
            return result;
        }

        /// <summary>
        /// Fills items to the specified tool strip item collection
        /// </summary>
        public static void FillMenuItems(ToolStripItemCollection items, XmlNode node)
        {
            switch (node.Name)
            {
                case "menu" :
                    var name = XmlHelper.GetAttribute(node, "name");
                    if (name == "SyntaxMenu") node.InnerXml = GetSyntaxMenuXml();
                    items.Add(GetMenu(node));
                    break;
                case "separator" :
                    items.Add(GetSeparator(node));
                    break;
                case "button" :
                    var menu = GetMenuItem(node);
                    items.Add(menu); // Add menu first to get the id correct
                    var id = GetMenuItemId(menu);
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
                    var button = GetButtonItem(node);
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
            var result = new ToolStripMenuItem();
            string name = XmlHelper.GetAttribute(node, "name");
            string image = XmlHelper.GetAttribute(node, "image");
            string label = XmlHelper.GetAttribute(node, "label");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string tag = XmlHelper.GetAttribute(node, "tag");
            result.Tag = new ItemData(label, tag, flags);
            result.Text = GetLocalizedString(label);
            result.Name = name ?? label.Replace("Label.", "");
            if (enabled != null) result.Enabled = Convert.ToBoolean(enabled);
            if (image != null) result.Image = PluginBase.MainForm.FindImage(image);
            if (click != null) result.Click += GetEventHandler(click);
            foreach (XmlNode it in node.ChildNodes)
            {
                FillMenuItems(result.DropDownItems, it);
            }
            Items.Add(result);
            return result;
        }

        /// <summary>
        /// Gets a button item from the specified xml node
        /// </summary>
        public static ToolStripItem GetButtonItem(XmlNode node)
        {
            var result = new ToolStripButton();
            string name = XmlHelper.GetAttribute(node, "name");
            string image = XmlHelper.GetAttribute(node, "image");
            string label = XmlHelper.GetAttribute(node, "label");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string keyId = XmlHelper.GetAttribute(node, "keyid");
            string tag = XmlHelper.GetAttribute(node, "tag");
            result.Tag = new ItemData(label + ";" + keyId, tag, flags);
            result.Name = name ?? label.Replace("Label.", "");
            string stripped = GetStrippedString(GetLocalizedString(label), false);
            if (image != null) result.ToolTipText = stripped;
            else result.Text = stripped; // Use text instead...
            if (enabled != null) result.Enabled = Convert.ToBoolean(enabled);
            if (image != null) result.Image = PluginBase.MainForm.FindImage(image);
            if (click != null) result.Click += GetEventHandler(click);
            Items.Add(result);
            return result;
        }

        /// <summary>
        /// Get a menu item from the specified xml node
        /// </summary>
        public static ToolStripMenuItem GetMenuItem(XmlNode node)
        {
            var result = new ToolStripMenuItem();
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
            result.Tag = new ItemData(label + ";" + keyId, tag, flags);
            result.Text = GetLocalizedString(label);
            result.Name = name ?? label.Replace("Label.", "");
            if (image != null) result.Image = PluginBase.MainForm.FindImage(image);
            if (enabled != null) result.Enabled = Convert.ToBoolean(enabled);
            if (keytext != null) result.ShortcutKeyDisplayString = GetKeyText(keytext);
            if (click != null) result.Click += GetEventHandler(click);
            if (shortcut != null) result.ShortcutKeys = GetKeys(shortcut);
            Items.Add(result);
            return result;
        }

        /// <summary>
        /// Gets a new tool strip separator item
        /// </summary>
        public static ToolStripSeparator GetSeparator(XmlNode node) => new ToolStripSeparator();

        /// <summary>
        /// Gets the dynamic syntax menu xml (easy integration :)
        /// </summary>
        static string GetSyntaxMenuXml()
        {
            var result = string.Empty;
            var syntaxFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            var xmlTmpl = "<button label=\"{0}\" click=\"ChangeSyntax\" tag=\"{1}\" image=\"559\" flags=\"Enable:IsEditable+Check:IsEditable|IsActiveSyntax\" />";
            foreach (var syntaxFile in syntaxFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(syntaxFile);
                result += string.Format(xmlTmpl, fileName, fileName.ToLower());
            }
            return result;
        }

        /// <summary>
        /// Strips normal label characters from the string
        /// </summary>
        static string GetStrippedString(string text, bool removeWhite)
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
        static string GetLocalizedString(string key)
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
            return menu.OwnerItem != null
                ? GetMenuItemId(menu.OwnerItem) + "." + GetStrippedString(menu.Name, true)
                : GetStrippedString(menu.Name, true);
        }

        /// <summary>
        /// Gets a shortcut key string from a string
        /// </summary>
        static string GetKeyText(string data)
        {
            data = data.Replace("|", "+");
            data = data.Replace("Control", "Ctrl");
            return data;
        }

        /// <summary>
        /// Gets a shortcut keys from a string
        /// </summary>
        static Keys GetKeys(string data)
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
        static EventHandler GetEventHandler(string method)
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