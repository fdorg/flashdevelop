using System;
using System.IO;
using System.Windows.Forms;
using PluginCore.Managers;

namespace PluginCore
{
    public class PluginBase
    {
        private static IProject project;
        private static IProject solution;
        private static InstalledSDK sdk;
        private static IMainForm instance;

        // Distribution info
        public const String DISTRIBUTION_NAME = "FlashDevelop";
        public const String DISTRIBUTION_DESC = "FlashDevelop is an open source script editor.";
        public const String DISTRIBUTION_HELP = "http://www.flashdevelop.org/wikidocs/";
        public const String DISTRIBUTION_ABOUT = "FlashDevelop logo, domain and the name are copyright of Mika Palmu.\r\nDevelopment: Mika Palmu, Philippe Elsass and all helpful contributors.";
        public const String DISTRIBUTION_COPYRIGHT = "FlashDevelop.org 2005-2015";
        public const String DISTRIBUTION_COMPANY = "FlashDevelop.org";

        /// <summary>
        /// Activates if the sender is MainForm
        /// </summary>
        public static void Initialize(IMainForm sender)
        {
            if (sender.GetType().ToString() == "FlashDevelop.MainForm")
            {
                instance = sender;
            }
        }

        /// <summary>
        /// Gets the instance of the Settings
        /// </summary>
        public static ISettings Settings
        {
            get { return instance.Settings; }
        }

        /// <summary>
        /// Gets the instance of the MainForm
        /// </summary>
        public static IMainForm MainForm
        {
            get { return instance; }
        }

        /// <summary>
        /// Sets and gets the current project
        /// </summary>
        public static IProject CurrentProject
        {
            get { return project; }
            set { project = value; }
        }

        /// <summary>
        /// Sets and gets the current solution
        /// </summary>
        public static IProject CurrentSolution
        {
            get { return solution; }
            set { solution = value; }
        }

        /// <summary>
        /// Sets and gets the current project's SDK
        /// </summary>
        public static InstalledSDK CurrentSDK
        {
            get { return sdk; }
            set { sdk = value; }
        }

        /// <summary>
        /// Run action on UI thread
        /// </summary>
        /// <param name="action"></param>
        public static void RunAsync(MethodInvoker action)
        {
            Form ui = MainForm as Form;
            if (ui != null && ui.InvokeRequired) ui.BeginInvoke(action);
            else action.Invoke();
        }
    }

}
