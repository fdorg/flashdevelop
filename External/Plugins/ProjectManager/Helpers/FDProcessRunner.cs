using System;
using PluginCore;

namespace ProjectManager.Helpers
{
    public delegate void ProcessEndedHandler(bool success);

    /// <summary>
    /// A helper class with static methods to deal with the unusual way FlashDevelop
    /// handles external processes and process-ended notifications.  The fundamental
    /// problem is that when FD tells us a process is ended, we don't know what process
    /// that was or who started it.
    /// </summary>
    public class FDProcessRunner
    {
        IMainForm mainForm;

        // some fudging to detect if a particular process was started by us and if so, what
        // name to associate with that process.
        string runningProcessName;
        string runningProcessNameRequest;
        ProcessEndedHandler processEndedHandler;
        string savedDirectory; // to save the current directory before running processes

        public FDProcessRunner(IMainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void StartProcess(string process, string arguments, string startupDirectory, ProcessEndedHandler callback)
        {
            // change current directory
            savedDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = startupDirectory;

            // save callback
            processEndedHandler = callback;

            runningProcessNameRequest = process;
            mainForm.CallCommand("RunProcessCaptured", process + ";" + arguments);
            // ProcessStartedEventCaught() is called
            runningProcessNameRequest = null;
        }

        // this is called from PluginMain which catches the ProcessStarted event
        // which is spawned by MainForm
        public void ProcessStartedEventCaught()
        {
            if (runningProcessNameRequest != null)
            {
                runningProcessName = runningProcessNameRequest;
            }
        }

        public void ProcessEndedEventCaught(string result)
        {
            // test if this process was started by us - we are assuming that FD can
            // only run one process at a time.
            if (runningProcessName != null)
            {
                bool success = result.EndsWith("(0)");
                if (processEndedHandler != null)
                {
                    processEndedHandler.DynamicInvoke(new object[] { success });
                }
                // restore current directory
                Environment.CurrentDirectory = savedDirectory;
                processEndedHandler = null;
                runningProcessName = null;
            }
        }

    }

}
