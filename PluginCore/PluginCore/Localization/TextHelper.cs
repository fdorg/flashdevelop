using System;
using System.IO;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

namespace PluginCore.Localization
{
    public class TextHelper
    {
        private static ResourceManager resourceManager = null;
        private static LocaleVersion storedLocale = LocaleVersion.en_US;

        /// <summary>
        /// Gets the specified localized string
        /// </summary>
        public static String GetString(String key)
        {
            String result; String prefix;
            if (PluginBase.MainForm == null || PluginBase.MainForm.Settings == null) return key;
            LocaleVersion localeSetting = PluginBase.MainForm.Settings.LocaleVersion;
            if (resourceManager == null || localeSetting != storedLocale)
            {
                storedLocale = localeSetting;
                String path = "PluginCore.PluginCore.Resources." + storedLocale.ToString();
                resourceManager = new ResourceManager(path, Assembly.GetExecutingAssembly());
            }
            prefix = Assembly.GetCallingAssembly().GetName().Name;
            result = resourceManager.GetString(prefix + "." + key);
            if (result == null) result = resourceManager.GetString(key);
            if (result == null)
            {
                TraceManager.Add("No localized string found: " + key);
                result = String.Empty;
            }
            return result;
        }

    }

}
