using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using LitJson;
using LoomContext.Projects;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Managers;
using ProjectManager.Projects;
using Timer = System.Timers.Timer;

namespace LoomContext
{
    public class LoomHelper
    {
        static string loomFolder;
        static string configPath;
        static WatcherEx watcher;
        static LoomProject proj;
        static Timer updater;
        static string lastProject;

        public static void Build(LoomProject project)
        {
            string loomPath = ResolveLoom();
            string loom = Path.Combine(loomPath, "loom.bat");
            if (!File.Exists(loom))
            {
                ShowMissingLoomBatError();
                return;
            }
            string oldWD = PluginBase.MainForm.WorkingDirectory;
            PluginBase.MainForm.WorkingDirectory = project.Directory;
            PluginBase.MainForm.CallCommand("RunProcessCaptured", loom + ";compile");
            PluginBase.MainForm.WorkingDirectory = oldWD;
        }

        public static void Run(LoomProject project)
        {
            if (Process.GetProcessesByName("LoomDemo").Length > 0)
            {
                TraceManager.AddAsync("LoomDemo is already running.");
                return;
            }

            string loom = Path.Combine(ResolveLoom(), "loom.bat");
            if (!File.Exists(loom))
            {
                ShowMissingLoomBatError();
                return;
            }
            ProcessStartInfo info = new ProcessStartInfo(loom, "run");
            Process process = new Process();
            process.StartInfo = info;
            process.Start();
        }

        private static void ShowMissingLoomBatError()
        {
            ErrorManager.ShowInfo("Could not find loom.bat. Is the Loom SDK installed?");
        }

        private static string ResolveLoom()
        {
            if (loomFolder != null) return loomFolder;

            var loomPath = Environment.ExpandEnvironmentVariables("%LoomPath%");
            if (Path.IsPathRooted(loomPath) && Directory.Exists(loomPath))
            {
                loomFolder = Path.Combine(loomPath, "bin");
                return loomFolder;
            }

            string[] path = Environment.ExpandEnvironmentVariables("%PATH%").Split(';');
            foreach (string dir in path)
            {
                if (File.Exists(Path.Combine(dir, "loom.bat")))
                {
                    loomFolder = dir;
                    return loomFolder;
                }
            }

            var programFiles = Environment.ExpandEnvironmentVariables("%ProgramFiles%");
            if (Path.IsPathRooted(programFiles) && Directory.Exists(programFiles))
            {
                loomFolder = programFiles;
                return loomFolder;
            }
            programFiles = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
            if (Path.IsPathRooted(programFiles) && Directory.Exists(programFiles))
            {
                loomFolder = programFiles;
                return loomFolder;
            }
            return "";
        }


        /// <summary>
        /// Watch Loom projects to update the configuration
        /// </summary>
        /// <param name="project"></param>
        static public void Monitor(IProject project)
        {
            if (updater == null && project != null)
            {
                updater = new Timer();
                updater.Interval = 200;
                updater.SynchronizingObject = PluginBase.MainForm as Form;
                updater.Elapsed += updater_Elapsed;
                updater.AutoReset = false;
            }

            proj = null;
            StopWatcher();
            if (project is LoomProject)
            {
                proj = project as LoomProject;
                proj.ProjectUpdating += new ProjectUpdatingHandler(proj_ProjectUpdating);
                proj_ProjectUpdating(proj);
                if (lastProject != proj.ProjectPath)
                {
                    lastProject = proj.ProjectPath;
                    AutoInit(proj);
                }
            }
            else lastProject = null;
        }

        /// <summary>
        /// Scaffold Loom project
        /// </summary>
        /// <param name="project"></param>
        private static void AutoInit(LoomProject project)
        {
            string[] files = Directory.GetFiles(project.Directory);
            if (!(files.Length == 1 && Path.GetExtension(files[0]) == ".lsproj"
                && Directory.GetDirectories(project.Directory).Length == 0)) return;

            if (!project.Storage.ContainsKey("package"))
                return;

            string pkg = project.Storage["package"];
            project.Storage.Remove("package");
            project.Save();
            
            string loomPath = ResolveLoom();
            string loom = Path.Combine(loomPath, "loom.bat");
            if (!File.Exists(loom))
            {
                ShowMissingLoomBatError();
                return;
            }

            string oldWD = PluginBase.MainForm.WorkingDirectory;
            string cmd = loom + ";new --force";
            if (!string.IsNullOrEmpty(pkg)) cmd += " --app-id " + pkg;
            cmd += " \"" + Path.GetFileName(project.Directory) + "\"";

            PluginBase.MainForm.WorkingDirectory = Path.GetDirectoryName(project.Directory);
            PluginBase.MainForm.CallCommand("RunProcessCaptured", cmd);
            PluginBase.MainForm.WorkingDirectory = oldWD;

            // force re-exploring the project
            updater.Enabled = true;
        }

        internal static void StopWatcher()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
                configPath = null;
            }
        }

        static void proj_ProjectUpdating(Project project)
        {
            string newConfig = Path.Combine(proj.Directory, "loom.config");
            if (configPath != newConfig)
            {
                configPath = newConfig;
                StopWatcher();
                if (File.Exists(configPath))
                {
                    watcher = new WatcherEx(Path.GetDirectoryName(configPath), Path.GetFileName(configPath));
                    watcher.Changed += watcher_Changed;
                    watcher.EnableRaisingEvents = true;
                    UpdateProject();
                }
            }
            else UpdateProject();
        }

        static void updater_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateProject();
            proj.PropertiesChanged();
            proj.OnClasspathChanged();
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            updater.Enabled = false;
            updater.Enabled = true;
        }

        // READ PROJECT'S LOOM.CONFIG

        private static void UpdateProject()
        {
            try
            {
                string config = File.ReadAllText(configPath);
                JsonReader reader = new JsonReader(config);
                if (reader.Read() && reader.Token == JsonToken.ObjectStart)
                    ReadConfig(reader);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync("Unable to read Loom config '" + configPath + "':\n" + ex.Message);
            }
        }

        private static void ReadConfig(JsonReader reader)
        {
            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "sdk_version":
                            if (val == "latest") val = GetLatestSDK();
                            proj.PreferredSDK = val + ";;"; break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    if (prop == "display") ReadDisplay(reader);
                    else reader.SkipObject();
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    reader.SkipArray();
                }
            }
        }

        private static void ReadDisplay(JsonReader reader)
        {
            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "width": int.TryParse(val, out proj.MovieOptions.Width); break;
                        case "height": int.TryParse(val, out proj.MovieOptions.Height); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    reader.SkipObject();
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    reader.SkipArray();
                }
            }
        }


        // LOOKUP DEFAULT SDK

        private static string GetLatestSDK()
        {
            string path = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.loom\\loom.config");
            try
            {
                string config = File.ReadAllText(path);
                JsonReader reader = new JsonReader(config);
                if (reader.Read() && reader.Token == JsonToken.ObjectStart)
                    return ReadDefaults(reader);
            }
            catch { }
            return "";
        }

        private static string ReadDefaults(JsonReader reader)
        {
            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "last_latest": return val; 
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    if (prop == "display") ReadDisplay(reader);
                    else reader.SkipObject();
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    reader.SkipArray();
                }
            }
            return "";
        }

    }
}
