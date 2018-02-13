using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

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
        /// Sets the use theme setting also to children
        /// </summary>
        public static void SetUseTheme(Object obj, Boolean use)
        {
            try
            {
                if (obj is ListView)
                {
                    ListView parent = obj as ListView;
                    foreach (ListViewItem item in parent.Items)
                    {
                        SetUseTheme(item, use);
                    }
                }
                else if (obj is TreeView)
                {
                    TreeView parent = obj as TreeView;
                    foreach (TreeNode item in parent.Nodes)
                    {
                        SetUseTheme(item, use);
                    }
                }
                else if (obj is MenuStrip)
                {
                    MenuStrip parent = obj as MenuStrip;
                    foreach (ToolStripItem item in parent.Items)
                    {
                        SetUseTheme(item, use);
                    }
                }
                else if (obj is ToolStripMenuItem)
                {
                    ToolStripMenuItem parent = obj as ToolStripMenuItem;
                    foreach (ToolStripItem item in parent.DropDownItems)
                    {
                        SetUseTheme(item, use);
                    }
                }
                else if (obj is Control)
                {
                    Control parent = obj as Control;
                    foreach (Control item in parent.Controls)
                    {
                        SetUseTheme(item, use);
                    }
                }
                PropertyInfo info = obj.GetType().GetProperty("UseTheme");
                if (info != null && info.CanWrite)
                {
                    info.SetValue(obj, use, null);
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
                    foreach (Control item in parent.Controls)
                    {
                        WalkControls(item);
                    }
                }
                ThemeControl(obj);
                if (obj is IThemeHandler)
                {
                    var th = obj as IThemeHandler;
                    th.AfterTheming();
                }
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
                dynamic cast = obj;
                // Apply colors of base type before applying for this type
                Boolean useIn = GetThemeValue("ThemeManager.UseInheritance") == "True";
                if (useIn && type.BaseType != null) ThemeControl(obj, type.BaseType);
                String name = ThemeHelper.GetFilteredTypeName(type);
                // Apply all basic style settings
                ApplyPropColor(obj, name + ".BackColor");
                ApplyPropColor(obj, name + ".ForeColor");
                ApplyPropColor(obj, name + ".BackgroundColor");
                ApplyPropColor(obj, name + ".ActiveLinkColor");
                ApplyPropColor(obj, name + ".DisabledLinkColor");
                ApplyPropColor(obj, name + ".LinkColor");
                ApplyPropColor(obj, name + ".BorderColor");
                ApplyPropColor(obj, name + ".ActiveForeColor");
                ApplyPropColor(obj, name + ".DisabledTextColor");
                ApplyPropColor(obj, name + ".DisabledBorderColor");
                ApplyPropColor(obj, name + ".CurrentPositionColor");
                ApplyPropColor(obj, name + ".DisabledBackColor");
                ApplyPropColor(obj, name + ".GridLineColor");
                ApplyPropColor(obj, name + ".HotForeColor");
                ApplyPropColor(obj, name + ".HotArrowColor");
                ApplyPropColor(obj, name + ".ActiveArrowColor");
                ApplyPropColor(obj, name + ".ArrowColor");
                // Set border style from border style key
                PropertyInfo bstyle = type.GetProperty("BorderStyle");
                Boolean force = GetThemeValue("ThemeManager.ForceBorderStyle") == "True";
                if (bstyle != null && bstyle.CanWrite && (force || cast.BorderStyle != BorderStyle.None))
                {
                    String key = name + ".BorderStyle";
                    String style = GetThemeValue(key);
                    switch (style)
                    {
                        case "None":
                            bstyle.SetValue(obj, BorderStyle.None, null);
                            break;
                        case "Fixed3D":
                            bstyle.SetValue(obj, BorderStyle.Fixed3D, null);
                            break;
                        case "FixedSingle":
                            bstyle.SetValue(obj, BorderStyle.FixedSingle, null);
                            break;
                        default:
                            break;
                    }
                }
                // Set flat style from flat style key
                PropertyInfo fstyle = type.GetProperty("FlatStyle");
                if (fstyle != null && fstyle.CanWrite)
                {
                    String key = name + ".FlatStyle";
                    String style = GetThemeValue(key);
                    switch (style)
                    {
                        case "Flat":
                            fstyle.SetValue(obj, FlatStyle.Flat, null);
                            break;
                        case "Popup":
                            fstyle.SetValue(obj, FlatStyle.Popup, null);
                            break;
                        case "System":
                            fstyle.SetValue(obj, FlatStyle.System, null);
                            break;
                        case "Standard":
                            fstyle.SetValue(obj, FlatStyle.Standard, null);
                            break;
                        default:
                            break;
                    }
                }
                // Control specific style assignments
                if (obj is Button)
                {
                    Color color = Color.Empty;
                    Button parent = obj as Button;
                    Boolean flat = GetThemeValue("Button.FlatStyle") == "Flat";
                    if (flat)
                    {
                        color = GetThemeColor("Button.BorderColor");
                        if (color != Color.Empty) parent.FlatAppearance.BorderColor = color;
                        color = GetThemeColor("Button.CheckedBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.CheckedBackColor = color;
                        color = GetThemeColor("Button.MouseDownBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.MouseDownBackColor = color;
                        color = GetThemeColor("Button.MouseOverBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.MouseOverBackColor = color;
                    }
                }
                else if (obj is CheckBox)
                {
                    Color color = Color.Empty;
                    CheckBox parent = obj as CheckBox;
                    Boolean flat = GetThemeValue("CheckBox.FlatStyle") == "Flat";
                    if (flat)
                    {
                        color = GetThemeColor("CheckBox.BorderColor");
                        if (color != Color.Empty) parent.FlatAppearance.BorderColor = color;
                        color = GetThemeColor("CheckBox.CheckedBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.CheckedBackColor = color;
                        color = GetThemeColor("CheckBox.MouseDownBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.MouseDownBackColor = color;
                        color = GetThemeColor("CheckBox.MouseOverBackColor");
                        if (color != Color.Empty) parent.FlatAppearance.MouseOverBackColor = color;
                    }
                }
                else if (obj is PropertyGrid)
                {
                    PropertyGrid grid = obj as PropertyGrid;
                    ApplyPropColor(grid, "PropertyGrid.ViewBackColor");
                    ApplyPropColor(grid, "PropertyGrid.ViewForeColor");
                    ApplyPropColor(grid, "PropertyGrid.ViewBorderColor");
                    ApplyPropColor(grid, "PropertyGrid.HelpBackColor");
                    ApplyPropColor(grid, "PropertyGrid.HelpForeColor");
                    ApplyPropColor(grid, "PropertyGrid.HelpBorderColor");
                    ApplyPropColor(grid, "PropertyGrid.CategoryForeColor");
                    ApplyPropColor(grid, "PropertyGrid.CategorySplitterColor");
                    ApplyPropColor(grid, "PropertyGrid.CommandsBackColor");
                    ApplyPropColor(grid, "PropertyGrid.CommandsActiveLinkColor");
                    ApplyPropColor(grid, "PropertyGrid.CommandsDisabledLinkColor");
                    ApplyPropColor(grid, "PropertyGrid.CommandsForeColor");
                    ApplyPropColor(grid, "PropertyGrid.CommandsLinkColor");
                    ApplyPropColor(grid, "PropertyGrid.LineColor");
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Apply property color if defined and property is available
        /// </summary>
        private static void ApplyPropColor(Object targObj, String propId)
        {
            Color color = GetThemeColor(propId);
            PropertyInfo prop = targObj.GetType().GetProperty(propId.Split('.')[1]);
            if (prop != null && prop.CanWrite && color != Color.Empty)
            {
                prop.SetValue(targObj, color, null);
            }
        }

    }

}

