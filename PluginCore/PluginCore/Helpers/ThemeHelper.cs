// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;

namespace PluginCore.Helpers
{
    public class ThemeHelper
    {
        /// <summary>
        /// Get type name with or without suffix 'Ex' and special cases.
        /// </summary>
        public static String GetFilteredTypeName(Type type)
        {
            String name = type.Name;
            name = name.EndsWithOrdinal("Ex") ? name.Remove(name.Length - 2) : name;
            if (name == "CheckedListBox") return "ListBox";
            else return name;
        }
    }
}
