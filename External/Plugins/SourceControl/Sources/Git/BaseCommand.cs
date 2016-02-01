using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace SourceControl.Sources.Git
{
    class BaseCommand
    {
        static private string resolvedCmd;
        static private string qualifiedCmd;

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

        protected virtual string GetGitCmd()
        {
            string cmd = PluginMain.SCSettings.GITPath;
            if (cmd == null) cmd = "git";
            string resolve = PathHelper.ResolvePath(cmd);
            return resolve ?? ResolveGitPath(cmd);
        }

        static private string ResolveGitPath(string cmd)
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
            errors.Add(line.StartsWithOrdinal("git: ") ? line.Substring(5) : line);
        }

        protected virtual void Runner_Output(object sender, string line)
        {
        }
    }
}
