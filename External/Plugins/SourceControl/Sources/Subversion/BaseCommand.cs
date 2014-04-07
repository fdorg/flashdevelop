using System;
using System.Text;
using System.Collections.Generic;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using System.Threading;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore;

namespace SourceControl.Sources.Subversion
{
    class BaseCommand
    {
        protected ProcessRunner runner;
        protected List<string> errors = new List<string>();

        protected virtual void Run(string args, string workingDirectory)
        {
            try
            {
                if (!args.StartsWith("status")) TraceManager.AddAsync("svn " + args);

                var cmd = GetSvnCmd();
                if (cmd == "Tools\\sliksvn\\bin\\svn.exe")
                {
                    TraceManager.AddAsync("SlickSVN is not included anymore in FlashDevelop tools.\nPlease install a SVN client and set Program Settings > SourceControl > SVN Path = 'svn'", -3);
                    return;
                }

                runner = new ProcessRunner();
                runner.WorkingDirectory = workingDirectory;
                runner.Run(cmd, args);
                runner.Output += new LineOutputHandler(runner_Output);
                runner.Error += new LineOutputHandler(runner_Error);
                runner.ProcessEnded += new ProcessEndedHandler(runner_ProcessEnded);
            }
            catch (Exception ex)
            {
                runner = null;
                String label = TextHelper.GetString("SourceControl.Info.UnableToStartCommand");
                TraceManager.AddAsync(label + "\n" + ex.Message);
            }
        }

        protected virtual string GetSvnCmd()
        {
            string cmd = PluginMain.SCSettings.SVNPath;
            if (cmd == "null") cmd = "svn";
            string resolve = PathHelper.ResolvePath(cmd);
            return resolve ?? cmd;
        }

        protected virtual void runner_ProcessEnded(object sender, int exitCode)
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

        protected virtual void runner_Error(object sender, string line)
        {
            errors.Add(line.StartsWith("svn: ") ? line.Substring(5) : line);
        }

        protected virtual void runner_Output(object sender, string line)
        {
        }
    }
}
