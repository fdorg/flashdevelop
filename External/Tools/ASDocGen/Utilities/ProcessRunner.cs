// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace ASDocGen.Utilities
{
    public class ProcessRunner
    {
        private Process process;
        public event EventHandler Exited;
        public event DataReceivedEventHandler DataReceived;

        /// <summary>
        /// Runs a process with the specified file.
        /// </summary>
        public void Run(String file, String dir)
        {
            try
            {
                this.process = new Process();
                this.process.EnableRaisingEvents = true;
                this.process.StartInfo.FileName = file;
                this.process.StartInfo.WorkingDirectory = dir;
                this.process.StartInfo.CreateNoWindow = true;
                this.process.StartInfo.UseShellExecute = false;
                this.process.StartInfo.RedirectStandardError = true;
                this.process.StartInfo.RedirectStandardOutput = true;
                this.process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                this.process.OutputDataReceived += new DataReceivedEventHandler(this.ProcessDataReceived);
                this.process.ErrorDataReceived += new DataReceivedEventHandler(this.ProcessDataReceived);
                this.process.Exited += new EventHandler(this.ProcessExited);
                this.process.Start();
                this.process.BeginOutputReadLine();
                this.process.BeginErrorReadLine();
            }
            catch(Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// When the process recieves data, dispatches an event for it.
        /// </summary>
        private void ProcessDataReceived(Object sender, DataReceivedEventArgs e)
        {
            if (this.DataReceived != null) this.DataReceived(sender, e);
        }

        /// <summary>
        /// When the process finishes, closes it and dispatches an event for it.
        /// </summary>
        private void ProcessExited(Object sender, EventArgs e)
        {
            if (this.Exited != null) this.Exited(sender, e);
            this.process.Close();
        }

    }

}
