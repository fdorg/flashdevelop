using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;

namespace ProjectManager.Actions
{
    public class Webserver
    {
        static string pathServed;
        static string fileServed;
        static Process process;

        public static bool Enabled => Port > 0;

        public static int Port { get; set; } = 2000;

        public static void StartServer(string path)
        {
            KillServer();
            if (Port < 80) // invalid port
            {
                PluginBase.MainForm.CallCommand("RunProcess", path);
                return;
            }

            ValidatePath(path);
            CreateServer();

            // open browser
            PluginBase.MainForm.CallCommand("RunProcess", "http://localhost:" + Port + "/" + fileServed);
        }

        static void ValidatePath(string path)
        {
            if (File.Exists(path))
            {
                fileServed = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
            }
            else fileServed = "";
            pathServed = path.TrimEnd('/', '\\');
        }

        static void CreateServer()
        {
            var ToolsWebserver = Path.Combine(PathHelper.ToolDir, "webserver");
            var config = GetServerConfig(Path.Combine(ToolsWebserver, "server.ini"));
            if (config is null) return;

            var server = Path.Combine(ToolsWebserver, config["executable"]);
            var arguments = config["arguments"].Replace("{doc}", pathServed).Replace("{port}", Port.ToString());

            TraceManager.Add("Web Server starting with root: " + pathServed);
            if (config.ContainsKey("verbose") && config["verbose"].ToLower() == "true")
                TraceManager.Add("Server process: " + server + " " + arguments);

            StartProcess(server, arguments);
        }

        static void StartProcess(string executable, string arguments)
        {
            var infos = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                WorkingDirectory = pathServed,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                process = Process.Start(infos);
            }
            catch (Exception ex)
            {
                TraceManager.Add("Unable to start the webserver: " + ex.Message, 3);
            }
        }

        static Dictionary<string, string> GetServerConfig(string configPath)
        {
            var ini = ConfigHelper.Parse(configPath, true);
            if (!ini.ContainsKey("Default"))
            {
                TraceManager.Add("Missing " + configPath, 3);
                return null;
            }
            var config = ini["Default"];
            if (!config.ContainsKey("executable"))
            {
                TraceManager.Add("Missing 'executable' entry in in " + configPath, 3);
                return null;
            }
            if (config.ContainsKey("arguments")) return config;
            TraceManager.Add("Missing 'arguments' entry in in " + configPath, 3);
            return null;
        }

        public static void KillServer()
        {
            if (process is null) return;
            try
            {
                if (!process.HasExited) process.Kill();
            }
            catch { }
            finally
            {
                pathServed = null;
                process = null;
            }
        }
    }
}