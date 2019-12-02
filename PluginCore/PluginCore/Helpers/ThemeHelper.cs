﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;

namespace PluginCore.Helpers
{
    public class ThemeHelper
    {
        /// <summary>
        /// Get type name with or without suffix 'Ex' and special cases.
        /// </summary>
        public static string GetFilteredTypeName(Type type)
        {
            string name = type.Name;
            name = name.EndsWithOrdinal("Ex") ? name.Remove(name.Length - 2) : name;
            return name == nameof(CheckedListBox) ? nameof(ListBox) : name;
        }
    }
}
