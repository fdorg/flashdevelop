using System.Windows.Forms;

namespace PluginCore
{
    public class PluginBase
    {
        private static IProject project;
        private static IProject solution;
        private static InstalledSDK sdk;
        private static IMainForm instance;

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
