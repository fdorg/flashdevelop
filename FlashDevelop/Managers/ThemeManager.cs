using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Managers;
using System.Collections;
using PluginCore.Helpers;

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
                        if (line.Length < 2 || line.StartsWith("#")) continue;
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
            try
            {
                Type type = obj.GetType();
                String full = type.Name;
                String name = full.EndsWith("Ex") ? full.Remove(full.Length - 2) : full;
                PropertyInfo ground = type.GetProperty("BackgroundColor");
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
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

}

