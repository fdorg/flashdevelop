using System;
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
    public class ExternalToolchain
    {
        static string projectPath;
        static WatcherEx watcher;
        static HaxeProject hxproj;
        static System.Timers.Timer updater;

        static internal bool HandleProject(IProject project)
        {
            HaxeProject hxproj = project as HaxeProject;
            if (hxproj == null) return false;
            if (!hxproj.MovieOptions.HasPlatformSupport) return false;
            return hxproj.MovieOptions.PlatformSupport.ExternalToolchain == "haxelib";
        }

        /// <summary>
        /// Run project (after build)
        /// </summary>
        /// <param name="command">Project's custom run command</param>
        /// <returns>Execution handled</returns>
        static internal bool Run(string command)
        {
            if (!string.IsNullOrEmpty(command)) // project has custom run command
                return false;

            HaxeProject hxproj = PluginBase.CurrentProject as HaxeProject;
            if (!HandleProject(hxproj)) return false;

            string args = GetCommand(hxproj, "run");
            if (args == null) return false;

            string config = hxproj.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";
            else if (config.IndexOf("android") >= 0) CheckADB();
            
            string haxelib = GetHaxelib(hxproj);

            if (config.StartsWith("html5") && ProjectManager.Actions.Webserver.Enabled && hxproj.RawHXML != null) // webserver
            {
                foreach (string line in hxproj.RawHXML)
                {
                    if (line.StartsWith("-js "))
                    {
                        string path = line.Substring(4);
                        path = path.Substring(0, path.LastIndexOf("/"));
                        ProjectManager.Actions.Webserver.StartServer(hxproj.GetAbsolutePath(path));
                        return true;
                    }
                }
            }

            TraceManager.Add("haxelib " + args);

            if (hxproj.TraceEnabled && hxproj.EnableInteractiveDebugger) // debugger
            {
                DataEvent de;
                if (config.StartsWith("flash"))
                {
                    de = new DataEvent(EventType.Command, "AS3Context.StartProfiler", null);
                    EventManager.DispatchEvent(hxproj, de);
                }
                de = new DataEvent(EventType.Command, "AS3Context.StartDebugger", null);
                EventManager.DispatchEvent(hxproj, de);
            }

            if (config.StartsWith("flash") || config.StartsWith("html5")) // no capture
            {
                var infos = new ProcessStartInfo(haxelib, args);
                infos.WorkingDirectory = hxproj.Directory;
                infos.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(infos);
            }
            else
            {
                string oldWD = PluginBase.MainForm.WorkingDirectory;
                PluginBase.MainForm.WorkingDirectory = hxproj.Directory;
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
            try
            {
                DataEvent de = new DataEvent(EventType.Command, "FlashViewer.GetFlashPlayer", null);
                EventManager.DispatchEvent(null, de);
                if (de.Handled && !String.IsNullOrEmpty((string)de.Data) && File.Exists((string)de.Data))
                    return " -DSWF_PLAYER=\"" + de.Data + "\"";
            }
            catch { }
            return "";
        }

        static internal bool Clean(IProject project)
        {
            if (!HandleProject(project)) return false;
            HaxeProject hxproj = project as HaxeProject;

            string args = GetCommand(hxproj, "clean");
            if (args == null) return false;
            
            string haxelib = GetHaxelib(hxproj);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = haxelib;
            pi.Arguments = args;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath);
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(pi);
            p.WaitForExit(5000);
            p.Close();
            return true;
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
                projectPath = null;
            }
        }

        static void hxproj_ProjectUpdating(Project project)
        {
            if (!HandleProject(project))
            {
                StopWatcher();
                return;
            }

            string projectFile = hxproj.OutputPathAbsolute;
            if (projectPath != projectFile)
            {
                projectPath = projectFile;
                StopWatcher();
                if (File.Exists(projectPath))
                {
                    watcher = new WatcherEx(Path.GetDirectoryName(projectPath), Path.GetFileName(projectPath));
                    watcher.Changed += watcher_Changed;
                    watcher.EnableRaisingEvents = true;
                    UpdateProject();
                }
            }
            else UpdateProject();
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
            var form = PluginBase.MainForm as System.Windows.Forms.Form;
            if (form.InvokeRequired)
            {
                form.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate {
                    UpdateProject();
                });
                return;
            }

            string haxelib = GetHaxelib(hxproj);
            if (haxelib == "haxelib")
            {
                TraceManager.Add("Haxelib not found", -3);
                return;
            }

            string args = GetCommand(hxproj, "display");
            if (args == null)
            {
                var msg = String.Format("No external 'display' command found for platform '{0}'", hxproj.MovieOptions.Platform);
                TraceManager.Add(msg, -3);
                return;
            }

            string config = hxproj.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = haxelib;
            pi.Arguments = args;
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
                TraceManager.Add(err, -3);
                hxproj.RawHXML = null;
            }
            else if (hxml.IndexOf("not installed") > 0)
            {
                TraceManager.Add(hxml, -3);
                hxproj.RawHXML = null;
            }
            else
            {
                hxml = hxml.Replace("--macro keep", "#--macro keep"); // TODO remove this hack

                hxproj.RawHXML = Regex.Split(hxml, "[\r\n]+");

                args = GetCommand(hxproj, "build", false);
                if (args == null)
                {
                    var msg = String.Format("No external 'build' command found for platform '{0}'", hxproj.MovieOptions.Platform);
                    TraceManager.Add(msg, -3);
                }
                else hxproj.PreBuildEvent = "\"$(CompilerPath)/haxelib\" " + args;

                hxproj.OutputType = OutputType.CustomBuild;
                hxproj.TestMovieBehavior = TestMovieBehavior.Custom;
                hxproj.TestMovieCommand = "";
                hxproj.Save();
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

        /// <summary>
        /// Get build/run/clean commands
        /// </summary>
        static string GetCommand(HaxeProject project, string name)
        {
            return GetCommand(project, name, true);
        }

        static string GetCommand(HaxeProject project, string name, bool processArguments)
        {
            var platform = project.MovieOptions.PlatformSupport;
            var version = platform.GetVersion(project.MovieOptions.Version);
            if (version.Commands == null)
            {
                throw new Exception(String.Format("No external commands found for target {0} and version {1}",
                    project.MovieOptions.Platform, project.MovieOptions.Version));
            }
            if (version.Commands.ContainsKey(name))
            {
                var cmd = "run " + version.Commands[name].Value;
                if (!processArguments) return cmd;
                else return PluginBase.MainForm.ProcessArgString(cmd);
            }
            else return null;
        }
    }
}
