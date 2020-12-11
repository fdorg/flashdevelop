// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using PluginCore.Helpers;

namespace AS3Context.Compiler
{
    public delegate void LineEvent(string line);

    public class FdbWrapper
    {
        public event LineEvent OnStarted;
        public event LineEvent OnOutput;
        public event LineEvent OnError;
        public event LineEvent OnTrace;

        public void Run(string projectPath, string flexSDKPath)
        {
            flexSDKPath = PathHelper.ResolvePath(flexSDKPath, projectPath);
            if (Directory.Exists(flexSDKPath))
            {
                if (flexSDKPath.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
                    flexSDKPath = Path.GetDirectoryName(flexSDKPath);
            }
            else return;

            if (process is null || process.HasExited)
            {
                var jvmConfig = JvmConfigHelper.ReadConfig(flexSDKPath);
                Initialize(flexSDKPath, jvmConfig, projectPath);
            }
        }

        public void PushCommand(string cmd)
        {
            lock (cmdQueue)
            {
                cmdQueue.Enqueue(cmd);
            }
        }

        public void SendCommand(string cmd)
        {
            lock (process)
            {
                WriteToPrompt(cmd);
            }
        }

        #region Process management
        Process process;
        string workingDir;
        Thread fdbThread;
        Thread errorThread;

        bool running;
        bool interactive;
        bool connected;

        bool Initialize(string flexSDKPath, Dictionary<string, string> jvmConfig, string projectPath)
        {
            cmdQueue = new Queue<string>();

            var fdbPath = Path.Combine(flexSDKPath, "lib\\fdb.jar");
            if (!File.Exists(fdbPath)) fdbPath = Path.Combine(flexSDKPath, "lib\\legacy\\fdb.jar");
            if (!File.Exists(fdbPath))
            {
                process = null;
                return false;
            }

            workingDir = projectPath;
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = JvmConfigHelper.GetJavaEXE(jvmConfig);
            process.StartInfo.Arguments = jvmConfig["java.args"] + " -Dfile.encoding=UTF-8 -Dapplication.home=\"" + flexSDKPath + "\" -jar \"" + fdbPath + "\"";
            process.StartInfo.WorkingDirectory = workingDir;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();

            process.Exited += process_Exited;

            errorThread = new Thread(ReadErrors);
            errorThread.Start();

            fdbThread = new Thread(FdbRun);
            fdbThread.Start();

            Thread.Sleep(100);
            return true;
        }

        void process_Exited(object sender, EventArgs e)
        {
            keepAlive = false;
            Cleanup();
            OnOutput?.Invoke("[FDB halted]");
        }

        public void Cleanup()
        {
            // this will free up our error-reading thread as well.
            if (process != null && !process.HasExited && keepAlive)
            {
                keepAlive = false;
                OnOutput?.Invoke("[Shutting down FDB]");
                try
                {
                    if (!process.HasExited) process.Kill();
                }
                catch { }
                process = null;
            }
            running = false;
        }
        #endregion

        #region Output reading/writing

        bool keepAlive;
        Queue<string> cmdQueue;
        ManualResetEvent connectedEvent;

        void FdbRun()
        {
            keepAlive = true;
            connectedEvent = new ManualResetEvent(false);

            while (keepAlive && !process.HasExited)
            {
                if (!interactive) ReadUntilPrompt();
                lock (cmdQueue)
                {
                    // no session
                    if (!running)
                    {
                        cmdQueue.Clear();
                        running = true;
                        WriteToPrompt("run");
                        Thread.Sleep(200);
                        OnStarted?.Invoke(null);
                    }
                    // send commands queue
                    else if (cmdQueue.Count > 0) WriteToPrompt(cmdQueue.Dequeue());
                    // default behavior
                    else if (connected) WriteToPrompt("continue");
                    else
                    { 
                        connectedEvent.WaitOne(250, false); 
                        connectedEvent.Reset();
                    }
                }
            }
            if (process != null && !process.HasExited)
            {
                OnOutput?.Invoke("[Shutting down FDB]");
                try
                {
                    if (!process.HasExited) process.Kill();
                }
                catch { }
            }
        }

        void ReadErrors()
        {
            while (keepAlive && !process.StandardError.EndOfStream)
            {
                string line = process.StandardError.ReadLine();
                OnError?.Invoke(line);
            }
        }

        void WriteToPrompt(string line)
        {
            interactive = false;
            process?.StandardInput.WriteLine(line);
            OnOutput?.Invoke("(fdb) "+line);
        }

        /// <summary>
        /// Read until fdb is in idle state, displaying its (fdb) prompt
        /// </summary>
        /// <returns>Debugger is interactive</returns>
        void ReadUntilPrompt()
        {
            ReadUntilToken("(fdb)");
            interactive = true;
        }

        void ReadUntilToken(string token)
        {
            var output = new StringBuilder();
            var queue = new Queue<char>();
            var keepProcessing = true;
            while (keepProcessing && keepAlive)
            {
                if (process is null || process.HasExited) keepProcessing = false;
                else
                {
                    char c = (char)process.StandardOutput.Read();
                    output.Append(c);
                    if (c == '\n')
                    {
                        MatchLine(output.ToString().Trim());
                        output = new StringBuilder();
                    }

                    queue.Enqueue(c);
                    if (queue.Count > token.Length)
                        queue.Dequeue();

                    if (new string(queue.ToArray()).Equals(token))
                    {
                        interactive = keepProcessing;
                        return;
                    }
                }
            }
        }
        #endregion

        #region Understanding

        static readonly Regex reTrace = new Regex(@"^?\[trace\]", RegexOptions.Compiled);
        static readonly Regex reFault = new Regex(@"^(\[Fault\]|Fault,) ", RegexOptions.Compiled);
        static readonly Regex reLoad = new Regex(@"^\[SWF\] ", RegexOptions.Compiled);
        static readonly Regex reUnload = new Regex(@"^\[UnloadSWF\] ", RegexOptions.Compiled);
        static readonly Regex reDisconnect = new Regex(@"^Player session terminated", RegexOptions.Compiled);
        static readonly Regex reContinue = new Regex(@"'continue'", RegexOptions.Compiled);

        void MatchLine(string line)
        {
            if (!connected)
            {
                // [SWF] C:\path\to\Project.swf
                if (reLoad.IsMatch(line)) connected = true;
                if (reContinue.IsMatch(line)) connected = true;
                connectedEvent.Set();
            }

            // [trace] Hello World!
            else if (reTrace.IsMatch(line))
            {
                if (OnTrace is { } onTrace)
                {
                    var trace = line.Substring(line.IndexOf(']') + 1);
                    if (trace.Length > 0 && char.IsWhiteSpace(trace[0])) onTrace(trace.Substring(1));
                    else onTrace(trace);
                    return;
                }
            }
            // [Fault] exception, information=TypeError: Error #1009: Il est impossible d'accéder à la propriété ou à la méthode d'une référence d'objet nul.
            // Fault, Main$iinit() at Main.as:20
            // 20 <code extract>
            else if (reFault.IsMatch(line))
            {
                if (OnError is { } onError)
                {
                    onError(line);
                    return;
                }
            }
            else if (reLoad.IsMatch(line))
            {
                //return;
            }
            // [UnloadSWF] C:\path\to\Project.swf
            else if (reUnload.IsMatch(line))
            {
                return;
            }
            // Player session terminated
            else if (reDisconnect.IsMatch(line)
                     || line == "Command not valid without a session.")
            {
                connected = false;
                cmdQueue.Clear();
                keepAlive = false;
            }

            OnOutput?.Invoke(line);
        }
        #endregion
    }
}
