// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using PluginCore.Managers;
using PluginCore;
using ASCompletion.Context;
using System.IO;
using FlashDevelop.Utilities;
using System.Timers;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PluginCore.Helpers;
using PluginCore.Localization;

namespace AS3Context
{
    class SyntaxChecking
    {
        #region Singleton
        static public SyntaxChecking Instance
        {
            get
            {
                if (instance == null) instance = new SyntaxChecking();
                return instance;
            }
        }
        static private SyntaxChecking instance;
        #endregion

        public void CheckAS3(string filename)
        {
            if (!File.Exists(filename)) return;
            
            string ascPath = null;

            // Try Flex2SDK
            string flex2Path = ProjectManager.PluginMain.Settings.Flex2SdkPath;
            if (flex2Path != null && Directory.Exists(flex2Path)) 
                ascPath = Path.Combine(flex2Path, "lib\\asc.jar");
            // Try Flash CS3
            if (ascPath == null) ascPath = FindAscAuthoring();

            if (ascPath == null)
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.SetFlex2OrCS3Path"));
                return;
            }
            
            try
            {
                EventManager.DispatchEvent(this, new NotifyEvent(EventType.ProcessStart));
                // direct execution instead of the shell
                ASContext.SetStatusText("Asc Running");
                notificationSent = false;
                WatchFile(filename);
                StartAscRunner(ascPath, filename);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        private string FindAscAuthoring()
        {
            string flashPath = ASContext.CommonSettings.PathToFlashIDE;
            if (flashPath == null) return null;
            string configPath = PluginMain.FindCS3ConfigurationPath(flashPath);
            if (configPath == null) return null;
            string ascJar = Path.Combine(configPath, "ActionScript 3.0\\asc_authoring.jar");
            if (File.Exists(ascJar)) return ascJar;
            else return null;
        }

        #region .p files cleanup
        private FileSystemWatcher watcher;
        private string watchedFile;
        private string fullWatchedPath;
        private System.Timers.Timer timer;

        /// <summary>
        /// Set watcher to remove the .p file
        /// </summary>
        private void WatchFile(string filename)
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher();
                watcher.EnableRaisingEvents = false;
                watcher.Filter = "*.p";
                watcher.Created += new FileSystemEventHandler(onCreateFile);

                timer = new System.Timers.Timer();
                timer.Enabled = false;
                timer.AutoReset = false;
                timer.Interval = 300;
                timer.Elapsed += new ElapsedEventHandler(onTimedDelete);
            }

            string folder = Path.GetDirectoryName(filename);
            watchedFile = Path.GetFileNameWithoutExtension(filename).ToLower() + ".p";
            fullWatchedPath = Path.Combine(folder, watchedFile);
            if (File.Exists(fullWatchedPath)) File.Delete(fullWatchedPath);
            watcher.Path = folder;
            watcher.EnableRaisingEvents = true;
        }

        private void onCreateFile(object source, FileSystemEventArgs e)
        {
            //if (e.Name.ToLower() == watchedFile)
            if (Path.GetExtension(e.Name).ToLower() == ".p")
            {
                watcher.EnableRaisingEvents = false;
                timer.Enabled = true;
            }
        }

        private void onTimedDelete(object source, ElapsedEventArgs e)
        {
            Control ctrl = PluginBase.MainForm.CurrentDocument as Control;
            if (ctrl != null && ctrl.InvokeRequired) ctrl.BeginInvoke(new ElapsedEventHandler(onTimedDelete), new object[] { source, e });
            else
            {
                if (File.Exists(fullWatchedPath))
                {
                    File.Delete(fullWatchedPath);
                    TraceManager.Add("Done(0)", -2);
                    EventManager.DispatchEvent(this, new TextEvent(EventType.ProcessEnd, "Done(0)"));
                    ASContext.SetStatusText("Asc Done");
                }
            }
        }
        #endregion

        #region Background process
        private ProcessRunner ascRunner;

        /// <summary>
        /// Stop background processes
        /// </summary>
        public void Stop()
        {
            if (ascRunner != null && ascRunner.IsRunning) ascRunner.KillProcess();
            ascRunner = null;
        }

        /// <summary>
        /// Start background process
        /// </summary>
        private void StartAscRunner(string ascPath, string fileName)
        {
            string cmd = "-jar \"" + ascPath + "\" -p \"" + fileName + "\"";
            TraceManager.Add("Running: java " + cmd, -1);
            // run asc shell
            ascRunner = new ProcessRunner();
            ascRunner.Run("java", cmd, true);
            ascRunner.Output += new LineOutputHandler(ascRunner_Output);
            ascRunner.Error += new LineOutputHandler(ascRunner_Error);
            errorState = 0;
        }
        #endregion

        #region process output capture
        private int errorState;
        private string errorDesc;
        private bool notificationSent;

        private void ascRunner_Error(object sender, string line)
        {
            if (line.StartsWith("[Compiler] Error"))
            {
                errorState = 1;
                errorDesc = line.Substring(10);
            }
            else if (errorState == 1)
            {
                line = line.Trim();
                Match mErr = Regex.Match(line, @"(?<file>[^,]+), Ln (?<line>[0-9]+), Col (?<col>[0-9]+)");
                if (mErr.Success)
                {
                    string filename = mErr.Groups["file"].Value;
                    try
                    {
                        if (File.Exists(filename))
                            filename = PathHelper.GetLongPathName(filename);
                    }
                    catch { }
                    errorDesc = String.Format("{0}:{1}: {2}", filename, mErr.Groups["line"].Value, errorDesc);
                    ascRunner_OutputError(sender, errorDesc);
                }
                errorState++;
            }
            else if (errorState > 0)
            {
                if (line.IndexOf("error found") > 0) errorState = 0;
            }
            else if (line.Trim().Length > 0) ascRunner_OutputError(sender, line);
        }

        private void ascRunner_OutputError(object sender, string line)
        {
            Control ctrl = PluginBase.MainForm.CurrentDocument as Control;
            if (ctrl != null && ctrl.InvokeRequired) ctrl.BeginInvoke(new ProcessOutputHandler(ascRunner_OutputError), new object[] { sender, line });
            else
            {
                TraceManager.Add(line, -3);
                if (!notificationSent)
                {
                    notificationSent = true;
                    ASContext.SetStatusText("Asc Done");
                    TraceManager.Add("Done(1)", -2);
                    EventManager.DispatchEvent(this, new TextEvent(EventType.ProcessEnd, "Done(1)"));
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "ResultsPanel.ShowResults", null));
                }
            }
        }

        private void ascRunner_Output(object sender, string line)
        {
            if (line.StartsWith("(ash)")) return;
            Control ctrl = PluginBase.MainForm.CurrentDocument as Control;
            if (ctrl != null && ctrl.InvokeRequired) ctrl.BeginInvoke(new ProcessOutputHandler(ascRunner_Output), new object[] { sender, line });
            else TraceManager.Add(line, 0);
        }
        #endregion
    }
}
