// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Reflection;
using PluginCore.Managers;
using ScintillaNet.Enums;

namespace FlashDevelop.Settings
{
    public partial class SettingObject
    {
        /// <summary>
        /// Sets a value of a setting
        /// </summary>
        public void SetValue(string name, object value)
        {
            try
            {
                Type type = GetType();
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
                Type type = GetType();
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
                var defaults = GetDefaultSettings();
                var properties = settings.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var current = settings.GetValue(property.Name);
                    if (current is null || (current is Color color && color == Color.Empty))
                    {
                        var value = defaults.GetValue(property.Name);
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
            => new SettingObject
            {
                IndentView = IndentView.Real,
                DefaultFont = SystemFonts.MenuFont
            };
    }
}