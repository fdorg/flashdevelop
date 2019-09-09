// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Diagnostics;
using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Mercurial
{
    static class TortoiseProc
    {
        private static string resolvedCmd;
        private static string qualifiedCmd;

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

        private static string GetTortoiseProc()
        {
            string cmd = PluginMain.SCSettings.TortoiseHGProcPath;
            if (cmd != null && File.Exists(cmd)) return cmd;
            if (string.IsNullOrEmpty(cmd)) cmd = "thgw.exe";
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
