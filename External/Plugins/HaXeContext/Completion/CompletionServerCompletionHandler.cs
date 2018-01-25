using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using PluginCore;
using ProjectManager.Projects.Haxe;
using PluginCore.Managers;
using System;
using System.Text.RegularExpressions;

namespace HaXeContext
{
    public delegate void FallbackNeededHandler(bool notSupported);

    public class CompletionServerCompletionHandler : IHaxeCompletionHandler
    {
        public event FallbackNeededHandler FallbackNeeded;

        private readonly Process haxeProcess;
        private readonly int port;
        private bool listening;
        private bool failure;

        public CompletionServerCompletionHandler(ProcessStartInfo haxeProcessStartInfo, int port)
        {
            this.haxeProcess = new Process {StartInfo = haxeProcessStartInfo, EnableRaisingEvents = true};
            this.port = port;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "" + port);
        }

        public bool IsRunning()
        {
            try { return !haxeProcess.HasExited; } 
            catch { return false; }
        }

        ~CompletionServerCompletionHandler()
        {
            Stop();
        }

        public string GetCompletion(string[] args)
        {
            return GetCompletion(args, null);
        }
        public string GetCompletion(string[] args, string fileContent)
        {
            if (args == null || haxeProcess == null)
                return string.Empty;
            if (!IsRunning()) StartServer();
            try
            {
                var client = new TcpClient("127.0.0.1", port);
                var writer = new StreamWriter(client.GetStream());
                writer.WriteLine("--cwd " + (PluginBase.CurrentProject as HaxeProject).Directory);
                foreach (var arg in args)
                    writer.WriteLine(arg);
                if (fileContent != null)
                {
                    writer.Write("\x01");
                    writer.Write(fileContent);
                }
                writer.Write("\0");
                writer.Flush();
                var reader = new StreamReader(client.GetStream());
                var lines = reader.ReadToEnd();
                client.Close();
                return lines;
            }
            catch(Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
                if (!failure && FallbackNeeded != null)
                    FallbackNeeded(false);
                failure = true;
                return string.Empty;
            }
        }

        public void StartServer()
        {
            if (haxeProcess == null || IsRunning()) return;
            haxeProcess.Start();
            if (!listening)
            {
                listening = true;
                haxeProcess.BeginOutputReadLine();
                haxeProcess.BeginErrorReadLine();
                haxeProcess.OutputDataReceived += new DataReceivedEventHandler(haxeProcess_OutputDataReceived);
                haxeProcess.ErrorDataReceived += new DataReceivedEventHandler(haxeProcess_ErrorDataReceived);
            }
        }

        void haxeProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            TraceManager.AddAsync(e.Data, 2);
        }

        void haxeProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            if (Regex.IsMatch(e.Data, "Error.*--wait"))
            {
                if (!failure && FallbackNeeded != null) 
                    FallbackNeeded(true);
                failure = true;
            }
        }

        public void Stop()
        {
            if (IsRunning())
                haxeProcess.Kill();
        }
    }
}
