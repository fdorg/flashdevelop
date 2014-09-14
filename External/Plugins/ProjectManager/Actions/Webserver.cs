using System;
using System.Collections.Generic;
using System.Text;
using ProjectManager.Projects;
using System.Diagnostics;
using PluginCore;
using System.IO;
using PluginCore.Managers;
using PluginCore.Helpers;

namespace ProjectManager.Actions
{
    public class Webserver
    {
        static string pathServed;
        static string fileServed;
        static Process process;
        static int portServed = 2000;

        public static bool Enabled
        {
            get { return portServed > 0; }
        }

        public static int Port
        {
            get { return portServed; }
            set { portServed = value; }
        }
                
        public static void StartServer(string path)
        {
            KillServer();

            if (portServed < 80) // invalid port
            {
                PluginBase.MainForm.CallCommand("RunProcess", path);
                return;
            }

            if (File.Exists(path))
            {
                fileServed = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
            }
            else fileServed = "";
            pathServed = path;

            TraceManager.Add("Web Server starting with root: " + path);
            var server = Path.Combine(PathHelper.ToolDir, "webserver\\server.cmd");
            var infos = new ProcessStartInfo(server, portServed.ToString());
            infos.Arguments = "" + portServed;
            infos.WorkingDirectory = pathServed;
            infos.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                process = Process.Start(infos);
            }
            catch (Exception ex)
            {
                TraceManager.Add("Unable to start the webserver: " + ex.Message, 3);
                return;
            }

            PluginBase.MainForm.CallCommand("RunProcess", "http://localhost:" + portServed + "/" + fileServed);
        }

        public static void KillServer()
        {
            if (process != null)
            {
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
}
