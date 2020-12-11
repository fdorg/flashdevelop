// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using ProjectManager.Projects.Haxe;
using PluginCore;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            return project is HaxeProject hxproj
                   && hxproj.MovieOptions.HasPlatformSupport
                   && hxproj.MovieOptions.PlatformSupport.ExternalToolchain != null;
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

            var hxproj = PluginBase.CurrentProject as HaxeProject;
            if (!HandleProject(hxproj)) return false;

            var platform = hxproj.MovieOptions.PlatformSupport;
            var toolchain = platform.ExternalToolchain;
            var exe = GetExecutable(toolchain);
            if (exe is null) return false;

            var args = GetCommand(hxproj, "run");
            if (args is null) return false;

            var config = hxproj.TargetBuild;
            if (string.IsNullOrEmpty(config)) config = "flash";
            else if (config.Contains("android")) CheckADB();
            
            if (config.StartsWithOrdinal("html5") && ProjectManager.Actions.Webserver.Enabled && hxproj.RawHXML != null) // webserver
            {
                foreach (var line in hxproj.RawHXML)
                {
                    if (!line.StartsWithOrdinal("-js ")) continue;
                    var p = line.LastIndexOf('/');
                    if (p == -1) break;// for example: -js _
                    var path = line.Substring(3, p - 3).Trim();
                    path = hxproj.GetAbsolutePath(path);
                    ProjectManager.Actions.Webserver.StartServer(path);
                    return true;
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
                var oldWD = PluginBase.MainForm.WorkingDirectory;
                PluginBase.MainForm.WorkingDirectory = hxproj.Directory;
                PluginBase.MainForm.CallCommand("RunProcessCaptured", exe + ";" + args);
                PluginBase.MainForm.WorkingDirectory = oldWD;
            }
            else
            {
                var infos = new ProcessStartInfo(exe, args)
                {
                    WorkingDirectory = hxproj.Directory,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(infos);
            }
            return true;
        }

        static bool ShouldCapture(string[] targets, string config) => targets != null && targets.Any(config.StartsWithOrdinal);

        /// <summary>
        /// Start Android ADB server in the background
        /// </summary>
        static void CheckADB()
        {
            if (Process.GetProcessesByName("adb").Length > 0)
                return;

            var adb = Environment.ExpandEnvironmentVariables("%ANDROID_SDK%/platform-tools");
            if (adb.StartsWith('%') || !Directory.Exists(adb))
                adb = Path.Combine(PathHelper.ToolDir, "android/platform-tools");
            if (!Directory.Exists(adb)) return;
            adb = Path.Combine(adb, "adb.exe");
            var p = new ProcessStartInfo(adb, "get-state");
            p.UseShellExecute = true;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(p);
        }

        internal static bool Clean(IProject project)
        {
            if (!HandleProject(project)) return false;
            var hxproj = (HaxeProject) project;

            var toolchain = hxproj.MovieOptions.PlatformSupport.ExternalToolchain;
            var exe = GetExecutable(toolchain);
            if (exe is null) return false;

            var args = GetCommand(hxproj, "clean");
            if (args is null) return false;

            TraceManager.Add(toolchain + " " + args);

            var pi = new ProcessStartInfo();
            pi.FileName = Environment.ExpandEnvironmentVariables(exe);
            pi.Arguments = args;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath);
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            var p = Process.Start(pi);
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
            if (updater is null)
            {
                updater = new System.Timers.Timer();
                updater.Interval = 200;
                updater.SynchronizingObject = (System.Windows.Forms.Form) PluginBase.MainForm;
                updater.Elapsed += updater_Elapsed;
                updater.AutoReset = false;
            }

            hxproj = null;
            StopWatcher();

            if (project is HaxeProject haxeProject)
            {
                hxproj = haxeProject;
                hxproj.ProjectUpdating += hxproj_ProjectUpdating;
                hxproj_ProjectUpdating(hxproj);
            }
        }

        internal static void StopWatcher()
        {
            if (watcher is null) return;
            watcher.Dispose();
            watcher = null;
            projectPath = null;
        }

        static void hxproj_ProjectUpdating(Project project)
        {
            if (!HandleProject(project))
            {
                StopWatcher();
                return;
            }

            var projectFile = hxproj.OutputPathAbsolute;
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

        static void UpdateProject()
        {
            var form = (System.Windows.Forms.Form) PluginBase.MainForm;
            if (form.InvokeRequired)
            {
                form.BeginInvoke((Action)UpdateProject);
                return;
            }
            if (hxproj.MovieOptions.Platform == "Lime" && string.IsNullOrEmpty(hxproj.TargetBuild)) return;

            var exe = GetExecutable(hxproj.MovieOptions.PlatformSupport.ExternalToolchain);
            if (exe is null) return;

            var args = GetCommand(hxproj, "display");
            if (args is null)
            {
                TraceManager.Add($"No external 'display' command found for platform '{hxproj.MovieOptions.Platform}'", -3);
                return;
            }

            var pi = new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(exe),
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(hxproj.ProjectPath),
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var p = Process.Start(pi);
            p.WaitForExit(5000);

            var hxml = p.StandardOutput.ReadToEnd();
            var err = p.StandardError.ReadToEnd();
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
                if (args is null) TraceManager.Add($"No external 'build' command found for platform '{hxproj.MovieOptions.Platform}'", -3);
                else if (string.IsNullOrEmpty(hxproj.PreBuildEvent))
                {
                    hxproj.PreBuildEvent = hxproj.MovieOptions.PlatformSupport.ExternalToolchain switch
                    {
                        "haxelib" => "\"$(CompilerPath)/haxelib\" " + args,
                        "cmd" => "cmd " + args,
                        _ => "\"" + exe + "\" " + args
                    };
                }

                var run = GetCommand(hxproj, "run");
                if (run is not null)
                {
                    hxproj.OutputType = OutputType.CustomBuild;
                    if (hxproj.TestMovieBehavior == TestMovieBehavior.Default)
                    {
                        hxproj.TestMovieBehavior = TestMovieBehavior.Custom;
                        hxproj.TestMovieCommand = string.Empty;
                    }
                }
                hxproj.Save();
                hxproj.OnClasspathChanged();
            }
        }

        static string? GetExecutable(string toolchain)
        {
            switch (toolchain)
            {
                case "haxelib":
                    var haxelib = GetHaxelib(hxproj);
                    if (haxelib != "haxelib") return haxelib;
                    TraceManager.Add("haxelib.exe not found in SDK path", -3);
                    return null;
                case "cmd": return "%SystemRoot%\\system32\\cmd.exe";
            }
            return File.Exists(toolchain) ? toolchain : null;
        }

        static string GetHaxelib(IProject project)
        {
            var haxelib = project.CurrentSDK;
            if (haxelib is null) return "haxelib";
            haxelib = Directory.Exists(haxelib)
                ? Path.Combine(haxelib, "haxelib.exe")
                : haxelib.Replace("haxe.exe", "haxelib.exe");

            if (!File.Exists(haxelib)) return "haxelib";

            // fix environment for command line tools
            var sdk = Path.GetDirectoryName(haxelib);
            var de = new DataEvent(EventType.Command, "Context.SetHaxeEnvironment", sdk);
            EventManager.DispatchEvent(null, de);
            
            return haxelib;
        }

        /// <summary>
        /// Get build/run/clean commands
        /// </summary>
        static string GetCommand(Project project, string name) => GetCommand(project, name, true);

        static string GetCommand(Project project, string name, bool processArguments)
        {
            var platform = project.MovieOptions.PlatformSupport;
            var version = platform.GetVersion(project.MovieOptions.Version);
            if (version.Commands is null) throw new Exception($"No external commands found for target {project.MovieOptions.Platform} and version {project.MovieOptions.Version}");
            if (!version.Commands.ContainsKey(name)) return null;
            var cmd = version.Commands[name].Value;
            cmd = platform.ExternalToolchain switch
            {
                "haxelib" => "run " + cmd,
                "cmd" => "/c " + cmd,
                _ => cmd
            };
            return !processArguments ? cmd : PluginBase.MainForm.ProcessArgString(cmd);
        }
    }
}
