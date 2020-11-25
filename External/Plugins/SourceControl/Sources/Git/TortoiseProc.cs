using System;
using System.Diagnostics;
using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Git
{
    static class TortoiseProc
    {
        private static string resolvedCmd;
        private static string qualifiedCmd;

        public static void Execute(string command, string path)
        {
            string args = $"/command:{command} /path:\"{path}\"";
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = true;

            var proc = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            proc.Exited += (sender, eventArgs) => ProjectWatcher.ForceRefresh();
            proc.Start();
        }

        public static void Execute(string command, string path1, string path2)
        {
            string args = $"/command:{command} /path:\"{path1}\" /path2:\"{path2}\"";
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = true;

            var proc = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            proc.Exited += (sender, eventArgs) => ProjectWatcher.ForceRefresh();
            proc.Start();
        }

        public static void ExecuteCustom(string command, string arguments)
        {
            string args = "/command:" + command + " " + arguments;
            var info = new ProcessStartInfo(GetTortoiseProc(), args) { UseShellExecute = true };

            var proc = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            proc.Exited += (sender, eventArgs) => ProjectWatcher.ForceRefresh();
            proc.Start();
        }

        private static string GetTortoiseProc()
        {
            var cmd = PluginMain.SCSettings.TortoiseGITProcPath;
            if (File.Exists(cmd)) return cmd;
            if (string.IsNullOrEmpty(cmd)) cmd = "TortoiseGitProc.exe";
            return ResolveTortoiseProcPath(cmd);
        }

        private static string ResolveTortoiseProcPath(string cmd)
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
                    string test = Path.Combine(path, cmd);
                    if (File.Exists(test)) { qualifiedCmd = test; break; }
                }
            }
            return qualifiedCmd;
        }
    }
}
