using System;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace ASCompletion.Commands
{
    public class CallFlashIDE
    {
        private delegate void RunBackgroundInvoker(string exe, string args);

        static readonly private string[] FLASHIDE_PATH = {
            @"C:\Program Files\Adobe\Adobe Animate CC 2016\Animate.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Animate CC 2016\Animate.exe",
            @"C:\Program Files\Adobe\Adobe Animate CC 2015\Animate.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Animate CC 2015\Animate.exe",
            @"C:\Program Files\Adobe\Adobe Flash CS5.5\Flash.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Flash CS5.5\Flash.exe",
            @"C:\Program Files\Adobe\Adobe Flash CS5\Flash.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Flash CS5\Flash.exe",
            @"C:\Program Files\Adobe\Adobe Flash CS4\Flash.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Flash CS4\Flash.exe",
            @"C:\Program Files\Adobe\Adobe Flash CS3\Flash.exe",
            @"C:\Program Files (x86)\Adobe\Adobe Flash CS3\Flash.exe",
            @"C:\Program Files\Macromedia\Flash 8\Flash.exe",
            @"C:\Program Files (x86)\Macromedia\Flash 8\Flash.exe",
            @"C:\Program Files\Macromedia\Flash MX 2004\Flash.exe",
            @"C:\Program Files (x86)\Macromedia\Flash MX 2004\Flash.exe"
        };
        static private DateTime lastRun;

        /// <summary>
        /// Return the path to the most recent Flash.exe 
        /// </summary>
        /// <returns></returns>
        static public string FindFlashIDE()
        {
            return FindFlashIDE(false);
        }

        /// <summary>
        /// Return the path to the most recent Flash.exe 
        /// </summary>
        /// <param name="AS3CapableOnly">Only AS3-capable authoring</param>
        /// <returns></returns>
        static public string FindFlashIDE(bool AS3CapableOnly)
        {
            string found = null;
            foreach (string flashexe in FLASHIDE_PATH)
            {
                if (File.Exists(flashexe)
                    && (!AS3CapableOnly || found.IndexOfOrdinal("Flash CS") > 0 || found.IndexOfOrdinal("Animate") > 0))
                {
                    found = flashexe;
                    break;
                }
            }
            return Path.GetDirectoryName(found);
        }

        /// <summary>
        /// Run the Flash IDE with the additional parameters provided
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Operation successful</returns>
        static public bool Run(string pathToIDE, string cmdData)
        {
            if (BridgeManager.Active) pathToIDE = "Flash";
            else
            {
                if (pathToIDE != null && Directory.Exists(pathToIDE))
                {
                    var exe = Path.Combine(pathToIDE, "Animate.exe");
                    if (!File.Exists(exe)) exe = Path.Combine(pathToIDE, "Flash.exe");
                    pathToIDE = exe;
                }
                if (pathToIDE == null || !File.Exists(pathToIDE))
                {
                    string msg = TextHelper.GetString("Info.ConfigureFlashPath");
                    string title = TextHelper.GetString("Info.ConfigurationRequired");
                    DialogResult result = MessageBox.Show(msg, title, MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        PluginBase.MainForm.ShowSettingsDialog("ASCompletion", "Flash");
                    }
                    return false;
                }
            }
            
            TimeSpan diff = DateTime.Now.Subtract(lastRun);
            if (diff.Seconds < 1) return false;
            lastRun = DateTime.Now;

            string args = null;
            if (cmdData != null)
            {
                args = PluginBase.MainForm.ProcessArgString(cmdData);
                if (args.IndexOf('"') < 0) args = '"' + args + '"';
            }

            // execution
            ASContext.SetStatusText(TextHelper.GetString("Info.CallingFlashIDE"));
            PluginBase.MainForm.CallCommand("SaveAllModified", null);
            EventManager.DispatchEvent(null, new NotifyEvent(EventType.ProcessStart));

            try
            {
                string file = args.StartsWith('\"') ? args.Substring(1, args.Length-2) : args;
                if (BridgeManager.Active && BridgeManager.Settings.TargetRemoteIDE 
                    && File.Exists(file) && Path.GetExtension(file) == ".jsfl" && file[0] <= 'H')
                {
                    string folder = Path.Combine(BridgeManager.Settings.SharedDrive, ".FlashDevelop\\flashide");
                    string[] logs = Directory.GetFiles(folder, "*.log");
                    foreach (string log in logs)
                        File.Delete(log);

                    string shared = Path.Combine(folder, Path.GetFileName(file));
                    File.Copy(file, shared, true);
                    BridgeManager.RemoteOpen(shared);
                    return true;
                }
            }
            catch { }

            if (args != null) ProcessHelper.StartAsync(pathToIDE, args);
            else ProcessHelper.StartAsync(pathToIDE);
            return true;
        }
    }
}
