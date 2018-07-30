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

        internal static bool HandleProject(IProject project)
        {
            HaxeProject hxproj = project as HaxeProject;
            if (hxproj == null) return false;
            if (!hxproj.MovieOptions.HasPlatformSupport) return false;
            return hxproj.MovieOptions.PlatformSupport.ExternalToolchain != null;
        }

        /// <summary>
        /// Run project (after build)
        /// </summary>
        /// <param name="command">Project's custom run command</param>
        /// <returns>Execution handled</returns>
        internal static bool Run(string command)
        {
            if (!string.IsNullOrEmpty(command)) // project has custom run command
                return false;

            HaxeProject hxproj = PluginBase.CurrentProject as HaxeProject;
            if (!HandleProject(hxproj)) return false;

            var platform = hxproj.MovieOptions.PlatformSupport;
            var toolchain = platform.ExternalToolchain;
            var exe = GetExecutable(toolchain);
            if (exe == null) return false;

            string args = GetCommand(hxproj, "run");
            if (args == null) return false;

            string config = hxproj.TargetBuild;
            if (String.IsNullOrEmpty(config)) config = "flash";
            else if (config.Contains("android")) CheckADB();
            
            if (config.StartsWithOrdinal("html5") && ProjectManager.Actions.Webserver.Enabled && hxproj.RawHXML != null) // webserver
            {
                foreach (string line in hxproj.RawHXML)
                {
                    if (line.StartsWithOrdinal("-js "))
                    {
                        string path = line.Substring(4);
                        path = path.Substring(0, path.LastIndexOf('/'));
                        ProjectManager.Actions.Webserver.StartServer(hxproj.GetAbsolutePath(path));
                        return true;
                    }
                }
            }

            TraceManager.Add(toolchain + " " + args);

            if (hxproj.TraceEnabled && hxproj.EnableInteractiveDebugger) // debugger
            {
                DataEvent de;
                if (config.StartsWithOrdinal("flash"))
                {
                    de = new DataEvent(EventType.Command, "AS3Context.StartProfiler", null);
                    EventManager.DispatchEvent(hxproj, de);
                }
                de = new DataEvent(EventType.Command, "AS3Context.StartDebugger", null);
                EventManager.DispatchEvent(hxproj, de);
            }

            exe = Environment.ExpandEnvironmentVariables(exe);
            if (ShouldCapture(platform.ExternalToolchainCapture, config))
            {
                string oldWD = PluginBase.MainForm.WorkingDirectory;
                PluginBase.MainForm.WorkingDirectory = hxproj.Directory;
                PluginBase.MainForm.CallCommand("RunProcessCaptured", exe + ";" + args);
                PluginBase.MainForm.WorkingDirectory = oldWD;
            }
            else
            {
                var infos = new ProcessStartInfo(exe, args);
                infos.WorkingDirectory = hxproj.Directory;
                infos.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(infos);
            }
            return true;
        }

        private static bool ShouldCapture(string[] targets, string config)
        {
            if (targets == null) return false;
            foreach (string target in targets)
                if (config.StartsWithOrdinal(target)) return true;
            return false;
        }

        /// <summary>
        /// Start Android ADB server in the background
        /// </summary>
        private static void CheckADB()
        {
            if (Process.GetProcessesByName("adb").Length > 0)
                return;

            string adb = Environment.ExpandEnvironmentVariables("%ANDROID_SDK%/platform-tools");
            if (adb.StartsWith('%') || !Directory.Exists(adb))
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
        private static string GetSwfPlayer()
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

        internal static bool Clean(IProject project)
        {
            if (!HandleProject(project)) return false;
            HaxeProject hxproj = project as HaxeProject;

            var toolchain = hxproj.MovieOptions.PlatformSupport.ExternalToolchain;
            var exe = GetExecutable(toolchain);
            if (exe == null) return false;

            string args = GetCommand(hxproj, "clean");
            if (args == null) return false;

            TraceManager.Add(toolchain + " " + args);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = Environment.ExpandEnvironmentVariables(exe);
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
        public static void Monitor(IProject project)
        {
            if (updater == null)
            {
                updater = new System.Timers.Timer();
                updater.Interval = 200;
                updater.SynchronizingObject = (System.Windows.Forms.Form) PluginBase.MainForm;
                updater.Elapsed += updater_Elapsed;
                updater.AutoReset = false;
            }

            hxproj = null;
            StopWatcher();

            if (project is HaxeProject)
            {
                hxproj = project as HaxeProject;
                hxproj.ProjectUpdating += hxproj_ProjectUpdating;
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
            var form = (System.Windows.Forms.Form) PluginBase.MainForm;
            if (form.InvokeRequired)
            {
                form.BeginInvoke((System.Windows.Forms.MethodInvoker)UpdateProject);
                return;
            }
            if (hxproj.MovieOptions.Platform == "Lime" && string.IsNullOrEmpty(hxproj.TargetBuild)) return;

            var exe = GetExecutable(hxproj.MovieOptions.PlatformSupport.ExternalToolchain);
            if (exe == null) return;

            var args = GetCommand(hxproj, "display");
            if (args == null)
            {
                TraceManager.Add($"No external 'display' command found for platform '{hxproj.MovieOptions.Platform}'", -3);
                return;
            }

            var pi = new ProcessStartInfo();
            pi.FileName = Environment.ExpandEnvironmentVariables(exe);
            pi.Arguments = args;
            pi.RedirectStandardError = true;
            pi.RedirectStandardOutput = true;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath);
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            var p = Process.Start(pi);
            p.WaitForExit(5000);

            string hxml = p.StandardOutput.ReadToEnd();
            string err = p.StandardError.ReadToEnd();
            p.Close();

            if (string.IsNullOrEmpty(hxml) || (!string.IsNullOrEmpty(err) && err.Trim().Length > 0))
            {
                if (string.IsNullOrEmpty(err)) err = "External tool error: no response";
                TraceManager.Add(err, -3);
                hxproj.RawHXML = null;
            }
            else if (hxml.IndexOfOrdinal("not installed") > 0)
            {
                TraceManager.Add(hxml, -3);
                hxproj.RawHXML = null;
            }
            else
            {
                hxml = hxml.Replace("--macro keep", "#--macro keep"); // TODO remove this hack
                hxml = Regex.Replace(hxml, "(-[a-z0-9-]+)\\s*[\r\n]+([^-#])", "$1 $2", RegexOptions.IgnoreCase);
                hxproj.RawHXML = Regex.Split(hxml, "[\r\n]+");

                args = GetCommand(hxproj, "build", false);
                if (args == null)
                {
                    TraceManager.Add($"No external 'build' command found for platform '{hxproj.MovieOptions.Platform}'", -3);
                }
                else if (string.IsNullOrEmpty(hxproj.PreBuildEvent))
                {
                    if (hxproj.MovieOptions.PlatformSupport.ExternalToolchain == "haxelib") hxproj.PreBuildEvent = "\"$(CompilerPath)/haxelib\" " + args;
                    else if (hxproj.MovieOptions.PlatformSupport.ExternalToolchain == "cmd") hxproj.PreBuildEvent = "cmd " + args;
                    else hxproj.PreBuildEvent = "\"" + exe + "\" " + args;
                }

                var run = GetCommand(hxproj, "run");
                if (run != null)
                {
                    hxproj.OutputType = OutputType.CustomBuild;
                    if (hxproj.TestMovieBehavior == TestMovieBehavior.Default)
                    {
                        hxproj.TestMovieBehavior = TestMovieBehavior.Custom;
                        hxproj.TestMovieCommand = "";
                    }
                }
                hxproj.Save();
            }
        }

        private static string GetExecutable(string toolchain)
        {
            if (toolchain == "haxelib")
            {
                var haxelib = GetHaxelib(hxproj);
                if (haxelib == "haxelib")
                {
                    TraceManager.Add("haxelib.exe not found in SDK path", -3);
                    return null;
                }
                return haxelib;
            }
            else if (toolchain == "cmd")
            {
                return "%SystemRoot%\\system32\\cmd.exe";
            }
            else if (File.Exists(toolchain))
            {
                return toolchain;
            }
            else return null;
        }

        private static string GetHaxelib(IProject project)
        {
            string haxelib = project.CurrentSDK;
            if (haxelib == null) return "haxelib";
            if (Directory.Exists(haxelib)) haxelib = Path.Combine(haxelib, "haxelib.exe");
            else haxelib = haxelib.Replace("haxe.exe", "haxelib.exe");

            if (!File.Exists(haxelib)) return "haxelib";

            // fix environment for command line tools
            string currentSDK = Path.GetDirectoryName(haxelib);
            DataEvent de = new DataEvent(EventType.Command, "Context.SetHaxeEnvironment", currentSDK);
            EventManager.DispatchEvent(null, de);
            
            return haxelib;
        }

        /// <summary>
        /// Get build/run/clean commands
        /// </summary>
        static string GetCommand(HaxeProject project, string name) => GetCommand(project, name, true);

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
                var cmd = version.Commands[name].Value;
                if (platform.ExternalToolchain == "haxelib") cmd = "run " + cmd;
                else if (platform.ExternalToolchain == "cmd") cmd = "/c " + cmd;
                
                if (!processArguments) return cmd;
                return PluginBase.MainForm.ProcessArgString(cmd);
            }
            return null;
        }
    }
}
