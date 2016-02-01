using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    class ThemeManager
    {
        /// <summary>
        /// Dictionary containing the loaded theme values
        /// </summary>
        private static Dictionary<String, String> valueMap = new Dictionary<String, String>();

        /// <summary>
        /// Gets a value entry from the config.
        /// </summary>
        public static String GetThemeValue(String id)
        {
            String result;
            if (valueMap.TryGetValue(id, out result)) return result;
            else return null;
        }

        /// <summary>
        /// Gets a color entry from the config.
        /// </summary>
        public static Color GetThemeColor(String id)
        {
            try { return ColorTranslator.FromHtml(GetThemeValue(id)); }
            catch { return Color.Empty; }
        }

        /// <summary>
        /// Loads and applies the theme to MainForm.
        /// </summary>
        public static void LoadTheme(String file)
        {
            try
            {
                if (File.Exists(file))
                {
                    valueMap.Clear();
                    String[] lines = File.ReadAllLines(file);
                    foreach (String rawLine in lines)
                    {
                        String line = rawLine.Trim();
                        if (line.Length < 2 || line.StartsWith('#')) continue;
                        String[] entry = line.Split(new Char[] { '=' }, 2);
                        if (entry.Length < 2) continue;
                        valueMap[entry[0]] = entry[1];
                    }
                    String currentFile = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                    if (file != currentFile) File.Copy(file, currentFile, true);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Walks the control tree down and themes all controls.
        /// </summary>
        public static void WalkControls(Object obj)
        {
            try
            {
                if (obj is ListView)
                {
                    ListView parent = obj as ListView;
                    foreach (ListViewItem item in parent.Items)
                    {
                        WalkControls(item);
                    }
                }
                else if (obj is TreeView)
                {
                    TreeView parent = obj as TreeView;
                    foreach (TreeNode item in parent.Nodes)
                    {
                        WalkControls(item);
                    }
                }
                else if (obj is MenuStrip)
                {
                    MenuStrip parent = obj as MenuStrip;
                    foreach (ToolStripItem item in parent.Items)
                    {
                        WalkControls(item);
                    }
                }
                else if (obj is ToolStripMenuItem)
                {
                    ToolStripMenuItem parent = obj as ToolStripMenuItem;
                    foreach (ToolStripItem item in parent.DropDownItems)
                    {
                        WalkControls(item);
                    }
                }
                else if (obj is Control)
                {
                    Control parent = obj as Control;
                    foreach (Control control in parent.Controls)
                    {
                        WalkControls(control);
                    }
                }
                ThemeControl(obj);
                if (obj is MainForm)
                {
                    NotifyEvent ne = new NotifyEvent(EventType.ApplyTheme);
                    EventManager.DispatchEvent(Globals.MainForm, ne);
                    Globals.MainForm.AdjustAllImages();
                    Globals.MainForm.Refresh();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Applies the theme colors to the control.
        /// </summary>
        public static void ThemeControl(Object obj)
        {
            ThemeControl(obj, obj.GetType());
        }

        /// <summary>
        /// Applies theme colors to the control based on type.
        /// </summary>
        private static void ThemeControl(Object obj, Type type)
        {
            try
            {
                // Apply colors of base type before applying for this type
                Boolean useIn = GetThemeValue("ThemeManager.UseInheritance") == "True";
                if (useIn && type.BaseType != null) ThemeControl(obj, type.BaseType);
                // Handle type with full name, with or without suffix 'Ex'
                String name = type.Name.EndsWithOrdinal("Ex") ? type.Name.Remove(type.Name.Length - 2) : type.Name;
                PropertyInfo ground = type.GetProperty("BackgroundColor");
                PropertyInfo alink = type.GetProperty("ActiveLinkColor");
                PropertyInfo dlink = type.GetProperty("DisabledLinkColor");
                PropertyInfo dborder = type.GetProperty("DisabledBorderColor");
                PropertyInfo curpos = type.GetProperty("CurrentPositionColor");
                PropertyInfo dback = type.GetProperty("DisabledBackColor");
                PropertyInfo afore = type.GetProperty("ActiveForeColor");
                PropertyInfo border = type.GetProperty("BorderColor");
                PropertyInfo hfore = type.GetProperty("HotForeColor");
                PropertyInfo harrow = type.GetProperty("HotArrowColor");
                PropertyInfo aarrow = type.GetProperty("ActiveArrowColor");
                PropertyInfo arrow = type.GetProperty("ArrowColor");
                PropertyInfo link = type.GetProperty("LinkColor");
                PropertyInfo back = type.GetProperty("BackColor");
                PropertyInfo fore = type.GetProperty("ForeColor");
                if (back != null)
                {
                    String key = name + ".BackColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        back.SetValue(obj, color, null);
                    }
                }
                if (fore != null)
                {
                    String key = name + ".ForeColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        fore.SetValue(obj, color, null);
                    }
                }
                if (ground != null)
                {
                    String key = name + ".BackgroundColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        ground.SetValue(obj, color, null);
                    }
                }
                if (alink != null)
                {
                    String key = name + ".ActiveLinkColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        alink.SetValue(obj, color, null);
                    }
                }
                if (dlink != null)
                {
                    String key = name + ".DisabledLinkColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        dlink.SetValue(obj, color, null);
                    }
                }
                if (link != null)
                {
                    String key = name + ".LinkColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        link.SetValue(obj, color, null);
                    }
                }
                if (border != null)
                {
                    String key = name + ".BorderColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        border.SetValue(obj, color, null);
                    }
                }
                if (afore != null)
                {
                    String key = name + ".ActiveForeColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        afore.SetValue(obj, color, null);
                    }
                }
                if (dborder != null)
                {
                    String key = name + ".DisabledBorderColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        dborder.SetValue(obj, color, null);
                    }
                }
                if (curpos != null)
                {
                    String key = name + ".CurrentPositionColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        curpos.SetValue(obj, color, null);
                    }
                }
                if (dback != null)
                {
                    String key = name + ".DisabledBackColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        dback.SetValue(obj, color, null);
                    }
                }
                if (hfore != null)
                {
                    String key = name + ".HotForeColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        hfore.SetValue(obj, color, null);
                    }
                }
                if (harrow != null)
                {
                    String key = name + ".HotArrowColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        harrow.SetValue(obj, color, null);
                    }
                }
                if (aarrow != null)
                {
                    String key = name + ".ActiveArrowColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        aarrow.SetValue(obj, color, null);
                    }
                }
                if (arrow != null)
                {
                    String key = name + ".ArrowColor";
                    Color color = GetThemeColor(key);
                    if (color != Color.Empty)
                    {
                        arrow.SetValue(obj, color, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

}

