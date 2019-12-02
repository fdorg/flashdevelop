// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Reflection;
using System.Resources;
using PluginCore.Managers;

namespace PluginCore.Localization
{
    /// <summary>
    /// Provides methods to retrieve and modify localized resource strings.
    /// </summary>
    public static class TextHelper
    {
        static ResourceManager resourceManager;
        static LocaleVersion storedLocale = LocaleVersion.Invalid;

        /// <summary>
        /// Gets the specified localized string.
        /// <para/>
        /// The calling assembly's name will be used as the default prefix for the specified key.
        /// If the key with the default prefix does not exist, the key without the default prefix
        /// will be used to retrieve the localized string.
        /// <see cref="String.Empty"/> is returned if the key does not exist.
        /// </summary>
        /// <param name="key">The key used to retrieve the localized string.</param>
        public static string GetString(string key) => GetStringInternal(key, Assembly.GetCallingAssembly());

        /// <summary>
        /// Gets the specified localized string with ampersand (&amp;) characters removed.
        /// <para/>
        /// Internally calls <see cref="TextHelper.RemoveMnemonics(String)"/> on the string
        /// returned from <see cref="TextHelper.GetString(String)"/>.
        /// </summary>
        /// <param name="key">The key used to retrieve the localized string.</param>
        public static string GetStringWithoutMnemonics(string key)
        {
            return RemoveMnemonics(GetStringInternal(key, Assembly.GetCallingAssembly()));
        }

        /// <summary>
        /// Gets the specified localized string with trailing triple dots (...) removed.
        /// <para/>
        /// Internally calls <see cref="TextHelper.RemoveEllipsis(String)"/> on the string returned
        /// from <see cref="TextHelper.GetString(String)"/>.
        /// </summary>
        /// <param name="key">The key used to retrieve the localized string.</param>
        public static string GetStringWithoutEllipsis(string key)
        {
            return RemoveEllipsis(GetStringInternal(key, Assembly.GetCallingAssembly()));
        }

        /// <summary>
        /// Gets the specified localized string with mnemonics and ellipsis removed.
        /// <para/>
        /// Internally calls <see cref="TextHelper.RemoveMnemonicsAndEllipsis(String)"/> on the
        /// string returned from <see cref="TextHelper.GetString(String)"/>.
        /// </summary>
        /// <param name="key">The key used to retrieve the localized string.</param>
        public static string GetStringWithoutMnemonicsOrEllipsis(string key)
        {
            return RemoveMnemonicsAndEllipsis(GetStringInternal(key, Assembly.GetCallingAssembly()));
        }

        /// <summary>
        /// Removes mnemonics from the specified string.
        /// <para/>
        /// The string can be in two forms: <code>"&amp;Close"</code> or <code>"Close (&amp;C)"</code>.
        /// In both cases this method will return the string <code>"Close"</code>.
        /// </summary>
        /// <param name="text">A <see cref="String"/> instance to remove mnemonics from.</param>
        public static string RemoveMnemonics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            int index = text.Length;
            if (index > 4 &&
                text[--index].Equals(')') &&
                char.IsUpper(text[--index]) &&
                text[--index].Equals('&') &&
                text[--index].Equals('('))
            {
                if (!text[--index].Equals(' ')) index++;
                return text.Remove(index);
            }
            return text.Replace("&", string.Empty);
        }

        /// <summary>
        /// Removes trailing ellipsis (...) from the specified string.
        /// </summary>
        /// <param name="text">A <see cref="String"/> instance to remove ellipsis from.</param>
        public static string RemoveEllipsis(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var index = text.LastIndexOfOrdinal("...");
            return index == -1 ? text : text.Remove(index, 3);
        }

        /// <summary>
        /// Removes mnemonics and ellipsis from the specified string.
        /// <para/>
        /// Internally calls <see cref="TextHelper.RemoveMnemonics(String)"/> on the string
        /// returned from  <see cref="TextHelper.RemoveEllipsis(String)"/>.
        /// </summary>
        /// <param name="text">A <see cref="String"/> instance to remove mnemonics and ellipsis from.</param>
        public static string RemoveMnemonicsAndEllipsis(string text)
        {
            return RemoveMnemonics(RemoveEllipsis(text));
        }

        /// <summary>
        /// Gets the specified localized string with the specified assembly's name as the default prefix.
        /// </summary>
        static string GetStringInternal(string key, Assembly assembly)
        {
            if (!ValidateStoredLocale()) return key ?? string.Empty;
            string prefix = assembly.GetName().Name;
            // On different distro we need to use FlashDevelop prefix
            if (prefix == DistroConfig.DISTRIBUTION_NAME) prefix = "FlashDevelop";
            string result = resourceManager.GetString(prefix + "." + key);
            if (result is null)
            {
                result = resourceManager.GetString(key);
                if (result is null)
                {
                    TraceManager.Add("No localized string found: " + key);
                    return string.Empty;
                }
            }
            // Replace FlashDevelop with distro name if needed
            if (DistroConfig.DISTRIBUTION_NAME != "FlashDevelop")
            {
                #pragma warning disable CS0162 // Unreachable code detected
                result = result.Replace("FlashDevelop", DistroConfig.DISTRIBUTION_NAME);
                #pragma warning restore CS0162 // Unreachable code detected
            }
            return result;
        }

        /// <summary>
        /// Validates the stored locale and resource manager if necessary.
        /// Returns whether the operation succeeded (<code>true</code>) or failed (<code>false</code>).
        /// </summary>
        static bool ValidateStoredLocale()
        {
            if (storedLocale == LocaleVersion.Invalid)
            {
                if (PluginBase.MainForm?.Settings is null) return false;
                storedLocale = PluginBase.MainForm.Settings.LocaleVersion;
                string path = "PluginCore.PluginCore.Resources." + storedLocale;
                resourceManager = new ResourceManager(path, Assembly.GetExecutingAssembly());
            }
            return true;
        }
    }

}
