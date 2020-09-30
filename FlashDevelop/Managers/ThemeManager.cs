using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FlashDevelop.Managers
{
    internal class ThemeManager
    {
        /// <summary>
        /// Dictionary containing the loaded theme values
        /// </summary>
        static readonly Dictionary<string, string> valueMap = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value entry from the config.
        /// </summary>
        public static string GetThemeValue(string id) => valueMap.TryGetValue(id, out var result) ? result : null;

        /// <summary>
        /// Gets a color entry from the config.
        /// </summary>
        public static Color GetThemeColor(string id)
        {
            try { return ColorTranslator.FromHtml(GetThemeValue(id)); }
            catch { return Color.Empty; }
        }

        /// <summary>
        /// Loads and applies the theme to MainForm.
        /// </summary>
        public static void LoadTheme(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                valueMap.Clear();
                var lines = File.ReadAllLines(file);
                foreach (var rawLine in lines)
                {
                    var line = rawLine.Trim();
                    if (line.Length < 2 || line.StartsWith('#')) continue;
                    var entry = line.Split(new[] { '=' }, 2);
                    if (entry.Length < 2) continue;
                    valueMap[entry[0]] = entry[1];
                }
                var currentFile = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                if (file != currentFile) File.Copy(file, currentFile, true);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Sets the use theme setting also to children
        /// </summary>
        public static void SetUseTheme(object obj, bool use)
        {
            try
            {
                switch (obj)
                {
                    case ListView view:
                        foreach (ListViewItem item in view.Items)
                        {
                            SetUseTheme(item, use);
                        }
                        break;
                    case TreeView view:
                        foreach (TreeNode item in view.Nodes)
                        {
                            SetUseTheme(item, use);
                        }
                        break;
                    case MenuStrip view:
                        foreach (ToolStripItem item in view.Items)
                        {
                            SetUseTheme(item, use);
                        }
                        break;
                    case ToolStripMenuItem view:
                        foreach (ToolStripItem item in view.DropDownItems)
                        {
                            SetUseTheme(item, use);
                        }
                        break;
                    case Control view:
                        foreach (Control item in view.Controls)
                        {
                            SetUseTheme(item, use);
                        }
                        break;
                }
                var info = obj.GetType().GetProperty("UseTheme");
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
        public static void WalkControls(object obj)
        {
            try
            {
                switch (obj)
                {
                    case ListView view:
                        foreach (ListViewItem item in view.Items)
                        {
                            WalkControls(item);
                        }
                        break;
                    case TreeView view:
                        foreach (TreeNode item in view.Nodes)
                        {
                            WalkControls(item);
                        }
                        break;
                    case MenuStrip view:
                        foreach (ToolStripItem item in view.Items)
                        {
                            WalkControls(item);
                        }
                        break;
                    case ToolStripMenuItem view:
                        foreach (ToolStripItem item in view.DropDownItems)
                        {
                            WalkControls(item);
                        }
                        break;
                    case Control view:
                        foreach (Control item in view.Controls)
                        {
                            WalkControls(item);
                        }
                        break;
                }
                ThemeControl(obj);
                switch (obj)
                {
                    case IThemeHandler th:
                        th.AfterTheming();
                        break;
                    case MainForm _:
                        var ne = new NotifyEvent(EventType.ApplyTheme);
                        EventManager.DispatchEvent(PluginBase.MainForm, ne);
                        Globals.MainForm.AdjustAllImages();
                        Globals.MainForm.Refresh();
                        break;
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
        public static void ThemeControl(object obj) => ThemeControl(obj, obj.GetType());

        /// <summary>
        /// Applies theme colors to the control based on type.
        /// </summary>
        static void ThemeControl(object obj, Type type)
        {
            try
            {
                // Apply colors of base type before applying for this type
                var useIn = GetThemeValue("ThemeManager.UseInheritance") == "True";
                if (useIn && type.BaseType != null) ThemeControl(obj, type.BaseType);
                var name = ThemeHelper.GetFilteredTypeName(type);
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
                var bstyle = type.GetProperty(nameof(BorderStyle));
                var force = GetThemeValue("ThemeManager.ForceBorderStyle") == "True";
                if (bstyle != null && bstyle.CanWrite && (force || (BorderStyle)bstyle.GetValue(obj) != BorderStyle.None))
                {
                    var key = name + ".BorderStyle";
                    var style = GetThemeValue(key);
                    switch (style)
                    {
                        case nameof(BorderStyle.None):
                            bstyle.SetValue(obj, BorderStyle.None, null);
                            break;
                        case nameof(BorderStyle.Fixed3D):
                            bstyle.SetValue(obj, BorderStyle.Fixed3D, null);
                            break;
                        case nameof(BorderStyle.FixedSingle):
                            bstyle.SetValue(obj, BorderStyle.FixedSingle, null);
                            break;
                    }
                }
                // Set flat style from flat style key
                var fstyle = type.GetProperty(nameof(FlatStyle));
                if (fstyle != null && fstyle.CanWrite)
                {
                    var style = GetThemeValue(name + ".FlatStyle");
                    switch (style)
                    {
                        case nameof(FlatStyle.Flat):
                            fstyle.SetValue(obj, FlatStyle.Flat, null);
                            break;
                        case nameof(FlatStyle.Popup):
                            fstyle.SetValue(obj, FlatStyle.Popup, null);
                            break;
                        case nameof(FlatStyle.System):
                            fstyle.SetValue(obj, FlatStyle.System, null);
                            break;
                        case nameof(FlatStyle.Standard):
                            fstyle.SetValue(obj, FlatStyle.Standard, null);
                            break;
                    }
                }
                switch (obj)
                {
                    // Control specific style assignments
                    case Button control:
                        if (GetThemeValue("Button.FlatStyle") == "Flat")
                        {
                            var color = GetThemeColor("Button.BorderColor");
                            if (color != Color.Empty) control.FlatAppearance.BorderColor = color;
                            color = GetThemeColor("Button.CheckedBackColor");
                            if (color != Color.Empty) control.FlatAppearance.CheckedBackColor = color;
                            color = GetThemeColor("Button.MouseDownBackColor");
                            if (color != Color.Empty) control.FlatAppearance.MouseDownBackColor = color;
                            color = GetThemeColor("Button.MouseOverBackColor");
                            if (color != Color.Empty) control.FlatAppearance.MouseOverBackColor = color;
                        }
                        break;
                    case CheckBox control:
                        if (GetThemeValue("CheckBox.FlatStyle") == "Flat")
                        {
                            var color = GetThemeColor("CheckBox.BorderColor");
                            if (color != Color.Empty) control.FlatAppearance.BorderColor = color;
                            color = GetThemeColor("CheckBox.CheckedBackColor");
                            if (color != Color.Empty) control.FlatAppearance.CheckedBackColor = color;
                            color = GetThemeColor("CheckBox.MouseDownBackColor");
                            if (color != Color.Empty) control.FlatAppearance.MouseDownBackColor = color;
                            color = GetThemeColor("CheckBox.MouseOverBackColor");
                            if (color != Color.Empty) control.FlatAppearance.MouseOverBackColor = color;
                        }
                        break;
                    case PropertyGrid control:
                        ApplyPropColor(control, "PropertyGrid.ViewBackColor");
                        ApplyPropColor(control, "PropertyGrid.ViewForeColor");
                        ApplyPropColor(control, "PropertyGrid.ViewBorderColor");
                        ApplyPropColor(control, "PropertyGrid.HelpBackColor");
                        ApplyPropColor(control, "PropertyGrid.HelpForeColor");
                        ApplyPropColor(control, "PropertyGrid.HelpBorderColor");
                        ApplyPropColor(control, "PropertyGrid.CategoryForeColor");
                        ApplyPropColor(control, "PropertyGrid.CategorySplitterColor");
                        ApplyPropColor(control, "PropertyGrid.CommandsBackColor");
                        ApplyPropColor(control, "PropertyGrid.CommandsActiveLinkColor");
                        ApplyPropColor(control, "PropertyGrid.CommandsDisabledLinkColor");
                        ApplyPropColor(control, "PropertyGrid.CommandsForeColor");
                        ApplyPropColor(control, "PropertyGrid.CommandsLinkColor");
                        ApplyPropColor(control, "PropertyGrid.LineColor");
                        break;
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
        static void ApplyPropColor(object obj, string propId)
        {
            var color = GetThemeColor(propId);
            var prop = obj.GetType().GetProperty(propId.Split('.')[1]);
            if (prop != null && prop.CanWrite && color != Color.Empty)
            {
                prop.SetValue(obj, color, null);
            }
        }
    }
}