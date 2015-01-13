﻿using System;
using System.Diagnostics;
using System.IO;

namespace SourceControl.Sources.Mercurial
{
    static class TortoiseProc
    {
        private static string resolvedCmd;
        private static string qualifiedCmd;

        public static void Execute(string command, string path)
        {
            string args = String.Format("{0} \"{1}\"", command, path);
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            Process.Start(info);
        }

        public static void Execute(string command, string path1, string path2)
        {
            string args = String.Format("{0} \"{1}\" \"{2}\"", command, path1, path2);
            ProcessStartInfo info = new ProcessStartInfo(GetTortoiseProc(), args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            Process.Start(info);
        }

        private static string GetTortoiseProc()
        {
            string cmd = PluginMain.SCSettings.TortoiseHGProcPath;
            if (cmd != null && File.Exists(cmd)) return cmd;
            if (String.IsNullOrEmpty(cmd)) cmd = "hgtk";
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
