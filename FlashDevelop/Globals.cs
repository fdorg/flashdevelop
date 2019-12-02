// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using FlashDevelop.Settings;
using PluginCore;
using ScintillaNet;

namespace FlashDevelop
{
    public class Globals
    {
        /// <summary>
        /// Quick reference to MainForm 
        /// </summary>
        public static MainForm MainForm { get; internal set; }

        /// <summary>
        /// Quick reference to CurrentDocument 
        /// </summary>
        [Obsolete("Use PluginBase.MainForm.CurrentDocument")]
        public static ITabbedDocument CurrentDocument => PluginBase.MainForm.CurrentDocument;

        /// <summary>
        /// Quick reference to SciControl 
        /// </summary>
        [Obsolete("Use PluginBase.MainForm.CurrentDocument?.SciControl")]
        public static ScintillaControl SciControl => CurrentDocument.SciControl;

        /// <summary>
        /// Quick reference to PreviousDocuments 
        /// </summary>
        [Obsolete("Use PluginBase.MainForm.Settings.PreviousDocuments")]
        public static List<string> PreviousDocuments => Settings.PreviousDocuments;

        /// <summary>
        /// Quick reference to Settings 
        /// </summary>
        public static SettingObject Settings => (SettingObject)PluginBase.MainForm.Settings;
    }
}