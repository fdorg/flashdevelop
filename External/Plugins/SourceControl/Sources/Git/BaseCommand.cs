// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using SourceControl.Actions;

namespace SourceControl.Sources.Git
{
    internal class BaseCommand
    {
        private static string resolvedCmd;
        private static string qualifiedCmd;

        protected ProcessRunner runner;
        protected List<string> errors = new List<string>();

        protected virtual void Run(string args, string workingDirectory)
        {
            try
            {
                if (!args.StartsWithOrdinal("status")) TraceManager.AddAsync("git " + args);

                string cmd = GetGitCmd();
                runner = new ProcessRunner();
                runner.WorkingDirectory = workingDirectory;
                runner.Run(cmd, args, !File.Exists(cmd));
                runner.Output += Runner_Output;
                runner.Error += Runner_Error;
                runner.ProcessEnded += Runner_ProcessEnded;
            }
            catch (Exception ex)
            {
                runner = null;
                string label = TextHelper.GetString("SourceControl.Info.UnableToStartCommand");
                TraceManager.AddAsync(label + "\n" + ex.Message);
            }
        }

        protected virtual string GetGitCmd()
        {
            string cmd = PluginMain.SCSettings.GITPath ?? "git";
            string resolve = PathHelper.ResolvePath(cmd);
            return resolve ?? ResolveGitPath(cmd);
        }

        private static string ResolveGitPath(string cmd)
        {
            if (resolvedCmd == cmd || Path.IsPathRooted(cmd))
                return qualifiedCmd;
            
            resolvedCmd = cmd;
            qualifiedCmd = cmd;
            string cp = Environment.GetEnvironmentVariable("PATH");
            foreach (string path in cp.Split(';'))
            {
                if (path.IndexOf("git", StringComparison.OrdinalIgnoreCase) > 0 && Directory.Exists(path))
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

            ProjectWatcher.ForceRefresh();
        }

        protected virtual void DisplayErrors()
        {
            if (errors.Count > 0)
            {
                ((Form) PluginBase.MainForm).BeginInvoke((MethodInvoker)(() =>
                    ErrorManager.ShowInfo(string.Join("\n", errors.ToArray()))));
            }
        }

        protected virtual void Runner_Error(object sender, string line)
        {
            errors.Add(line.StartsWithOrdinal("git: ") ? line.Substring(5) : line);
        }

        protected virtual void Runner_Output(object sender, string line)
        {
        }
    }
}
