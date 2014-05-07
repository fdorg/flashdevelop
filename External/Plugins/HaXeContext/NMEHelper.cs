using System;
using System.Collections.Generic;
using System.Text;
using ProjectManager.Projects.Haxe;
using PluginCore;
using System.Diagnostics;
using System.IO;
using PluginCore.Managers;
using PluginCore.Bridge;
using ProjectManager.Projects;
using PluginCore.Helpers;
using System.Text.RegularExpressions;

namespace HaXeContext
{
    public class NMEHelper
    {
        static string nmmlPath;
        static WatcherEx watcher;
        static HaxeProject hxproj;
        static System.Timers.Timer updater;

        /// <summary>
        /// Run NME project (after build)
        /// </summary>
        /// <param name="command">Project's custom run command</param>
        /// <returns>Execution handled</returns>
        static public bool Run(string command)
        {
            if (!string.IsNullOrEmpty(command)) // project has custom run command
                return false;

            HaxeProject project = PluginBase.CurrentProject as HaxeProject;
            if (project == null || project.OutputType != OutputType.Application)
                return false;

            string builder = HaxeProject.GetBuilder(project);
            if (builder == null) return true;

            string config = project.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";
            else if (config.IndexOf("android") >= 0) CheckADB();

            if (project.TraceEnabled)
            {
                config += " -debug -Dfdb";
            }
            if (config.StartsWith("flash") && config.IndexOf("-DSWF_PLAYER") < 0)
                config += GetSwfPlayer();

            string args = "run " + builder + " run \"" + project.OutputPathAbsolute + "\" " + config;
            string haxelib = GetHaxelib(project);

            if (config.StartsWith("html5") && ProjectManager.Actions.Webserver.Enabled && project.RawHXML != null) // webserver
            {
                foreach (string line in project.RawHXML)
                {
                    if (line.StartsWith("-js "))
                    {
                        string path = line.Substring(4);
                        path = path.Substring(0, path.LastIndexOf("/"));
                        ProjectManager.Actions.Webserver.StartServer(project.GetAbsolutePath(path));
                        return true;
                    }
                }
            }

            if (config.StartsWith("flash") || config.StartsWith("html5")) // no capture
            {
                if (config.StartsWith("flash") && project.TraceEnabled) // debugger
                {
                    DataEvent de = new DataEvent(EventType.Command, "AS3Context.StartProfiler", null);
                    EventManager.DispatchEvent(project, de);
                    de = new DataEvent(EventType.Command, "AS3Context.StartDebugger", null);
                    EventManager.DispatchEvent(project, de);
                }

                var infos = new ProcessStartInfo(haxelib, args);
                infos.WorkingDirectory = project.Directory;
                infos.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(infos);
            }
            else
            {
                string oldWD = PluginBase.MainForm.WorkingDirectory;
                PluginBase.MainForm.WorkingDirectory = project.Directory;
                PluginBase.MainForm.CallCommand("RunProcessCaptured", haxelib + ";" + args);
                PluginBase.MainForm.WorkingDirectory = oldWD;
            }
            return true;
        }

        /// <summary>
        /// Start Android ADB server in the background
        /// </summary>
        static private void CheckADB()
        {
            if (Process.GetProcessesByName("adb").Length > 0)
                return;

            string adb = Environment.ExpandEnvironmentVariables("%ANDROID_SDK%/platform-tools");
            if (adb.StartsWith("%") || !Directory.Exists(adb))
                adb = Path.Combine(PathHelper.ToolDir, "android/platform-tools");
            if (Directory.Exists(adb))
            {
                adb = Path.Combine(adb, "adb.exe");
                ProcessStartInfo p = new ProcessStartInfo(adb, "get-state");
                p.UseShellExecute = true;
                p.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(p);
            }
        }

        /// <summary>
        /// Provide FD-configured Flash player
        /// </summary>
        static private string GetSwfPlayer()
        {
            DataEvent de = new DataEvent(EventType.Command, "FlashViewer.GetFlashPlayer", null);
            EventManager.DispatchEvent(null, de);
            if (de.Handled && !String.IsNullOrEmpty((string)de.Data)) return " -DSWF_PLAYER=\"" + de.Data + "\"";
            else return "";
        }

        static public void Clean(IProject project)
        {
            if (!(project is HaxeProject))
                return;
            HaxeProject hxproj = project as HaxeProject;
            if (hxproj.MovieOptions.Platform != HaxeMovieOptions.NME_PLATFORM)
                return;
            
            string builder = HaxeProject.GetBuilder(hxproj);
            if (builder == null) return;
            
            string haxelib = GetHaxelib(hxproj);
            string config = hxproj.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = haxelib;
            pi.Arguments = " run " + builder + " clean \"" + hxproj.OutputPathAbsolute + "\" " + config;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath);
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(pi);
            p.WaitForExit(5000);
            p.Close();
        }

        /// <summary>
        /// Watch NME projects to update the configuration & HXML command using 'nme display'
        /// </summary>
        /// <param name="project"></param>
        static public void Monitor(IProject project)
        {
            if (updater == null)
            {
                updater = new System.Timers.Timer();
                updater.Interval = 200;
                updater.SynchronizingObject = PluginCore.PluginBase.MainForm as System.Windows.Forms.Form;
                updater.Elapsed += updater_Elapsed;
                updater.AutoReset = false;
            }

            hxproj = null;
            StopWatcher();

            if (project is HaxeProject)
            {
                hxproj = project as HaxeProject;
                hxproj.ProjectUpdating += new ProjectUpdatingHandler(hxproj_ProjectUpdating);
                hxproj_ProjectUpdating(hxproj);
            }
        }

        internal static void StopWatcher()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
                nmmlPath = null;
            }
        }

        static void hxproj_ProjectUpdating(Project project)
        {
            if (hxproj.MovieOptions.Platform == HaxeMovieOptions.NME_PLATFORM)
            {
                string nmmlProj = hxproj.OutputPathAbsolute;
                if (nmmlPath != nmmlProj)
                {
                    nmmlPath = nmmlProj;
                    StopWatcher();
                    if (File.Exists(nmmlPath))
                    {
                        watcher = new WatcherEx(Path.GetDirectoryName(nmmlPath), Path.GetFileName(nmmlPath));
                        watcher.Changed += watcher_Changed;
                        watcher.EnableRaisingEvents = true;
                        UpdateProject();
                    }
                }
                else UpdateProject();
            }
            else StopWatcher();
        }

        static void updater_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateProject();
            hxproj.PropertiesChanged();
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            updater.Enabled = false;
            updater.Enabled = true;
        }

        private static void UpdateProject()
        {
            string haxelib = GetHaxelib(hxproj);
            if (haxelib == "haxelib")
            {
                TraceManager.AddAsync("Haxelib not found", -3);
                return;
            }

            string builder = HaxeProject.GetBuilder(hxproj);
            if (builder == null)
            {
                TraceManager.AddAsync("Project config not found:\n" + hxproj.OutputPathAbsolute, -3);
                return;
            }

            string config = hxproj.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = haxelib;
            pi.Arguments = "run " + builder + " display \"" + hxproj.GetRelativePath(nmmlPath) + "\" " + config;
            pi.RedirectStandardError = true;
            pi.RedirectStandardOutput = true;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath);
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(pi);
            p.WaitForExit(5000);

            string hxml = p.StandardOutput.ReadToEnd();
            string err = p.StandardError.ReadToEnd();
            p.Close();

            if (string.IsNullOrEmpty(hxml) || (!string.IsNullOrEmpty(err) && err.Trim().Length > 0))
            {
                if (string.IsNullOrEmpty(err)) err = "Haxelib error: no response";
                TraceManager.AddAsync(err, -3);
                hxproj.RawHXML = null;
            }
            else if (hxml.IndexOf("not installed") > 0)
            {
                TraceManager.AddAsync(hxml, -3);
                hxproj.RawHXML = null;
            }
            else
            {
                hxml = hxml.Replace("--macro keep", "#--macro keep"); // TODO remove this hack
                hxproj.RawHXML = Regex.Split(hxml, "[\r\n]+");
            }
        }

        private static string GetHaxelib(IProject project)
        {
            string haxelib = project.CurrentSDK;
            if (haxelib == null) return "haxelib";
            else if (Directory.Exists(haxelib)) haxelib = Path.Combine(haxelib, "haxelib.exe");
            else haxelib = haxelib.Replace("haxe.exe", "haxelib.exe");

            // fix environment for command line tools
            string currentSDK = Path.GetDirectoryName(haxelib);
            Context.SetHaxeEnvironment(currentSDK);
            
            return haxelib;
        }
    }
}
