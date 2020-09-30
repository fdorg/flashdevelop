using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using PluginCore.Managers;

namespace PluginCore.Utilities
{
    public class ProcessRunner
    {
        StreamReader outputReader;
        StreamReader errorReader;
        int tasksFinished;
        
        public event LineOutputHandler Output;
        public event LineOutputHandler Error;
        public event ProcessEndedHandler ProcessEnded;

        public string WorkingDirectory;
        public Process HostedProcess { get; set; }
        public bool IsRunning { get; set; }
        public bool RedirectInput;
        NextTask nextTask;

        public void Run(string fileName, string arguments) => Run(fileName, arguments, false);

        public void Run(string fileName, string arguments, bool shellCommand)
        {
            if (IsRunning)
            {
                // kill process and queue Run command
                nextTask = () => Run(fileName, arguments, shellCommand);
                KillProcess();
                return;
            }

            if (!shellCommand && !File.Exists(fileName))
                throw new FileNotFoundException("The program '" + fileName + "' was not found.", fileName);

            IsRunning = true;
            HostedProcess = new Process();
            HostedProcess.StartInfo.UseShellExecute = false;
            HostedProcess.StartInfo.RedirectStandardInput = RedirectInput;
            HostedProcess.StartInfo.RedirectStandardOutput = true;
            HostedProcess.StartInfo.RedirectStandardError = true;
            HostedProcess.StartInfo.StandardOutputEncoding = Encoding.Default;
            HostedProcess.StartInfo.StandardErrorEncoding = Encoding.Default;
            HostedProcess.StartInfo.CreateNoWindow = true;
            HostedProcess.StartInfo.FileName = fileName;
            HostedProcess.StartInfo.Arguments = arguments;
            HostedProcess.StartInfo.WorkingDirectory = WorkingDirectory ?? PluginBase.MainForm.WorkingDirectory;
            HostedProcess.Start();
            
            outputReader = HostedProcess.StandardOutput;
            errorReader = HostedProcess.StandardError;
            
            // we need to wait for all 3 threadpool operations 
            // to finish (processexit, readoutput, readerror)
            tasksFinished = 0;
            
            ThreadStart waitForExitDel = HostedProcess.WaitForExit;
            waitForExitDel.BeginInvoke(TaskFinished, null);
            
            ThreadStart readOutputDel = ReadOutput;
            ThreadStart readErrorDel = ReadError;
            
            readOutputDel.BeginInvoke(TaskFinished, null);
            readErrorDel.BeginInvoke(TaskFinished, null);
        }
        
        public void KillProcess()
        {
            if (HostedProcess is null) return;
            try
            {
                if (IsRunning) TraceManager.AddAsync("Kill active process...", -3);
                IsRunning = false;
                // recursive kill (parent and children)
                var process = new Process();
                process.StartInfo.FileName = "taskkill.exe";
                process.StartInfo.Arguments = "/PID " + HostedProcess.Id + " /T /F";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        void ReadOutput()
        {
            while (true)
            {
                var line = outputReader.ReadLine();
                if (line is null) break;
                Output?.Invoke(this, line);
            }
        }

        void ReadError()
        {
            while (true)
            {
                var line = errorReader.ReadLine();
                if (line is null) break;
                Error?.Invoke(this, line);
            }
        }

        void TaskFinished(IAsyncResult result)
        {
            lock (this) 
            {
                if (++tasksFinished >= 3) 
                {
                    IsRunning = false;

                    if (nextTask != null)
                    {
                        nextTask();
                        nextTask = null;
                        // do not call ProcessEnd if another process was queued after the kill
                    }
                    else if (HostedProcess != null) 
                        ProcessEnded?.Invoke(this, HostedProcess.ExitCode);
                }
            }
        }

    }
    
    public delegate void LineOutputHandler(object sender, string line);
    public delegate void ProcessEndedHandler(object sender, int exitCode);
    internal delegate void NextTask();
}