// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Reflection;
using PluginCore;
using PluginCore.Managers;
using ScintillaNet.Enums;

namespace FlashDevelop.Settings
{
    public partial class SettingObject : ISettings
    {
        /// <summary>
        /// Sets a value of a setting
        /// </summary>
        public void SetValue(string name, object value)
        {
            try
            {
                Type type = this.GetType();
                PropertyInfo info = type.GetProperty(name);
                info.SetValue(this, value, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Gets a value of a setting as an object
        /// </summary>
        public object GetValue(string name)
        {
            try
            {
                Type type = this.GetType();
                PropertyInfo info = type.GetProperty(name);
                return info.GetValue(this, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Ensures that the values of the settings are not null
        /// </summary>
        public static void EnsureValidity(SettingObject settings)
        {
            try
            {
                SettingObject defaults = GetDefaultSettings();
                PropertyInfo[] properties = settings.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    object current = settings.GetValue(property.Name);
                    if (current is null || (current is Color && (Color)current == Color.Empty))
                    {
                        object value = defaults.GetValue(property.Name);
                        settings.SetValue(property.Name, value);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Gets a fresh copy of the settings
        /// </summary>
        public static SettingObject GetDefaultSettings()
        {
            SettingObject settings = new SettingObject();
            settings.IndentView = IndentView.Real;
            settings.DefaultFont = SystemFonts.MenuFont;
            return settings;
        }

    }

}
