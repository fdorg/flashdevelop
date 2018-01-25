﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using SourceControl.Actions;

namespace SourceControl.Sources.Mercurial
{
    abstract class BaseCommand : IVCCommand
    {
        static private string resolvedCmd;
        static private string qualifiedCmd;

        protected ProcessRunner runner;
        protected List<string> errors = new List<string>();
        protected IVCCommand nextCommand; //using Func for lazy evaluation

        public void ContinueWith(IVCCommand command)
        {
            nextCommand = command;
        }

        public abstract void Run();

        protected virtual void Run(string args, string workingDirectory)
        {
            try
            {
                if (!args.StartsWithOrdinal("status")) TraceManager.AddAsync("hg " + args);

                string cmd = GetHGCmd();
                runner = new ProcessRunner();
                runner.WorkingDirectory = workingDirectory;
                runner.Run(cmd, args, !File.Exists(cmd));
                runner.Output += new LineOutputHandler(Runner_Output);
                runner.Error += new LineOutputHandler(Runner_Error);
                runner.ProcessEnded += new ProcessEndedHandler(Runner_ProcessEnded);
            }
            catch (Exception ex)
            {
                runner = null;
                String label = TextHelper.GetString("SourceControl.Info.UnableToStartCommand");
                TraceManager.AddAsync(label + "\n" + ex.Message);
            }
        }

        protected virtual string GetHGCmd()
        {
            string cmd = PluginMain.SCSettings.HGPath;
            if (cmd == null) cmd = "hg";
            string resolve = PathHelper.ResolvePath(cmd);
            return resolve ?? ResolveHGPath(cmd);
        }

        static private string ResolveHGPath(string cmd)
        {
            if (resolvedCmd == cmd || Path.IsPathRooted(cmd))
                return qualifiedCmd;
            
            resolvedCmd = cmd;
            qualifiedCmd = cmd;
            string cp = Environment.GetEnvironmentVariable("PATH");
            foreach (string path in cp.Split(';'))
            {
                if (path.IndexOf("hg", StringComparison.OrdinalIgnoreCase) > 0 && Directory.Exists(path))
                {
                    string test = Path.Combine(path, cmd + ".cmd");
                    if (File.Exists(test)) { qualifiedCmd = test; break; }
                    test = Path.Combine(path, cmd + ".exe");
                    if (File.Exists(test)) { qualifiedCmd = test; break; }
                }
            }
            return qualifiedCmd;
        }

        protected virtual void Runner_ProcessEnded(object sender, int exitCode)
        {
            runner = null;
            DisplayErrors();

            nextCommand?.Run();

            ProjectWatcher.ForceRefresh();
        }

        protected virtual void DisplayErrors()
        {
            if (errors.Count > 0)
            {
                (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker)delegate
                {
                    ErrorManager.ShowInfo(String.Join("\n", errors.ToArray()));
                });
            }
        }

        protected virtual void Runner_Error(object sender, string line)
        {
            errors.Add(line.StartsWithOrdinal("hg: ") ? line.Substring(5) : line);
        }

        protected virtual void Runner_Output(object sender, string line)
        {
        }
    }
}
