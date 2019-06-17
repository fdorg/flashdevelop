using System;
using System.Diagnostics;
using System.IO;
using SourceControl.Actions;

namespace SourceControl.Sources.Mercurial
{
    static class TortoiseProc
    {
        static private string resolvedCmd;
        static private string qualifiedCmd;

        static public void Execute(string command, string path)
        {
            string args = string.Format("{0} \"{1}\"", command, path);
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

        static public void Execute(string command, string path1, string path2)
        {
            string args = string.Format("{0} \"{1}\" \"{2}\"", command, path1, path2);
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

        static private string GetTortoiseProc()
        {
            string cmd = PluginMain.SCSettings.TortoiseHGProcPath;
            if (cmd != null && File.Exists(cmd)) return cmd;
            if (string.IsNullOrEmpty(cmd)) cmd = "thgw.exe";
            return ResolveTortoiseProcPath(cmd);
        }

        static private string ResolveTortoiseProcPath(string cmd)
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
