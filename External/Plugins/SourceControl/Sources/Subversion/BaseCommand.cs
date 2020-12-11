// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using SourceControl.Actions;

namespace SourceControl.Sources.Subversion
{
    internal abstract class BaseCommand : VCCommand
    {
        protected ProcessRunner runner;
        protected List<string> errors = new List<string>();

        protected virtual void Run(string args, string workingDirectory)
        {
            try
            {
                if (!args.StartsWithOrdinal("status")) TraceManager.AddAsync("svn " + args);

                var cmd = GetSvnCmd();
                if (cmd == "Tools\\sliksvn\\bin\\svn.exe")
                {
                    TraceManager.AddAsync("SlickSVN is not included anymore in FlashDevelop tools.\nPlease install a SVN client and set Program Settings > SourceControl > SVN Path = 'svn'", -3);
                    return;
                }

                runner = new ProcessRunner();
                runner.WorkingDirectory = workingDirectory;
                runner.Run(cmd, args);
                runner.Output += Runner_Output;
                runner.Error += Runner_Error;
                runner.ProcessEnded += Runner_ProcessEnded;
            }
            catch (Exception ex)
            {
                runner = null;
                var label = TextHelper.GetString("SourceControl.Info.UnableToStartCommand");
                TraceManager.AddAsync(label + "\n" + ex.Message);
            }
        }

        protected virtual string GetSvnCmd()
        {
            var cmd = PluginMain.SCSettings.SVNPath;
            if (cmd == "null") cmd = "svn";
            var resolve = PathHelper.ResolvePath(cmd);
            return resolve ?? cmd;
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
                ((Form) PluginBase.MainForm).BeginInvoke((Action)delegate
                {
                    ErrorManager.ShowInfo(string.Join("\n", errors));
                });
            }
        }

        protected virtual void Runner_Error(object sender, string line)
        {
            errors.Add(line.StartsWithOrdinal("svn: ") ? line.Substring(5) : line);
        }

        protected virtual void Runner_Output(object sender, string line)
        {
        }
    }
}
