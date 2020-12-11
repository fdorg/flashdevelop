// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;

namespace PluginCore
{
    public class PluginBase
    {
        /// <summary>
        /// Activates if the sender is MainForm
        /// </summary>
        public static void Initialize(IMainForm sender)
        {
            if (sender.GetType().ToString() == "FlashDevelop.MainForm")
            {
                MainForm = sender;
            }
        }

        /// <summary>
        /// Gets the instance of the Settings
        /// </summary>
        public static ISettings Settings => MainForm.Settings;

        /// <summary>
        /// Gets the instance of the MainForm
        /// </summary>
        public static IMainForm MainForm { get; set; }

        /// <summary>
        /// Sets and gets the current project
        /// </summary>
        public static IProject CurrentProject { get; set; }

        /// <summary>
        /// Sets and gets the current solution
        /// </summary>
        public static IProject CurrentSolution { get; set; }

        /// <summary>
        /// Sets and gets the current project's SDK
        /// </summary>
        public static InstalledSDK CurrentSDK { get; set; }

        /// <summary>
        /// Run action on UI thread
        /// </summary>
        /// <param name="action"></param>
        public static void RunAsync(Action action)
        {
            if (MainForm is Form ui && ui.InvokeRequired) ui.BeginInvoke(action);
            else action.Invoke();
        }
    }
}