﻿using System.Diagnostics;
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

        readonly Process haxeProcess;
        readonly int port;
        readonly object lockObj = new object();
        bool isRunning;
        bool listening;
        bool failure;

        public CompletionServerCompletionHandler(ProcessStartInfo haxeProcessStartInfo, int port)
        {
            haxeProcess = new Process {StartInfo = haxeProcessStartInfo, EnableRaisingEvents = true};
            this.port = port;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "" + port);
        }

        public bool IsRunning() => isRunning;

        ~CompletionServerCompletionHandler() => Stop();

        public string GetCompletion(string[] args) => GetCompletion(args, null);

        public string GetCompletion(string[] args, string fileContent)
        {
            if (args is null || haxeProcess is null) return string.Empty;
            if (!isRunning) StartServer();
            try
            {
                var client = new TcpClient("127.0.0.1", port);
                using var writer = new StreamWriter(client.GetStream());
                writer.WriteLine("--cwd " + ((HaxeProject) PluginBase.CurrentProject).Directory);
                foreach (var arg in args)
                    writer.WriteLine(arg);
                if (fileContent != null)
                {
                    writer.Write("\x01");
                    writer.Write(fileContent);
                }
                writer.Write("\0");
                writer.Flush();
                using var reader = new StreamReader(client.GetStream());
                var lines = reader.ReadToEnd();
                client.Close();
                return lines;
            }
            catch(Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
                if (!failure)
                    FallbackNeeded?.Invoke(false);
                failure = true;
                return string.Empty;
            }
        }

        public void StartServer()
        {
            if (isRunning) return;
            lock (lockObj)
            {
                if (isRunning) return;
                if (!(isRunning = haxeProcess.Start())) return;
                if (listening) return;
                listening = true;
                haxeProcess.BeginOutputReadLine();
                haxeProcess.BeginErrorReadLine();
                haxeProcess.OutputDataReceived += OnOutputDataReceived;
                haxeProcess.ErrorDataReceived += OnErrorDataReceived;
                haxeProcess.Exited += HaxeProcess_Exited;
            }
        }

        void OnOutputDataReceived(object sender, DataReceivedEventArgs e) => TraceManager.AddAsync(e.Data, 2);

        void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is null) return;
            TraceManager.AddAsync(e.Data, 2);
            if (Regex.IsMatch(e.Data, "Error.*--wait"))
            {
                if (!failure) 
                    FallbackNeeded?.Invoke(true);
                failure = true;
            }
        }

        void HaxeProcess_Exited(object sender, EventArgs e) => isRunning = false;

        public void Stop()
        {
            if (!isRunning) return;
            haxeProcess.Kill();
            isRunning = false;
        }
    }
}