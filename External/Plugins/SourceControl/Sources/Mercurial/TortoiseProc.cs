using System;
using System.Diagnostics;
using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Mercurial
{
    internal static class TortoiseProc
    {
        static string resolvedCmd;
        static string qualifiedCmd;

        public static void Execute(string command, string path)
        {
            string args = $"{command} \"{path}\"";
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

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
            string args = $"{command} \"{path1}\" \"{path2}\"";
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var proc = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            proc.Exited += (sender, eventArgs) => ProjectWatcher.ForceRefresh();
            proc.Start();
        }

        static string GetTortoiseProc()
        {
            var cmd = PluginMain.SCSettings.TortoiseHGProcPath;
            if (File.Exists(cmd)) return cmd;
            if (string.IsNullOrEmpty(cmd)) cmd = "thgw.exe";
            return ResolveTortoiseProcPath(cmd);
        }

        static string ResolveTortoiseProcPath(string cmd)
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
                    string test = Path.Combine(path, cmd);
                    if (File.Exists(test)) { qualifiedCmd = test; break; }
                }
            }
            return qualifiedCmd;
        }
    }
}
