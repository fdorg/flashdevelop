// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Diagnostics;
using System.Threading;
using HaXeContext.Helpers;

namespace HaXeContext
{
    public class CompilerCompletionHandler : IHaxeCompletionHandler
    {
        readonly ThreadLocal<ProcessStartInfo> haxeProcessStartInfo;

        public CompilerCompletionHandler(ProcessStartInfo haxeProcessStartInfo)
        {
            this.haxeProcessStartInfo = haxeProcessStartInfo != null ? new ThreadLocal<ProcessStartInfo>(haxeProcessStartInfo.Clone) : null;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "0");
        }

        public string GetCompletion(string[] args) => GetCompletion(args, null);

        public string GetCompletion(string[] args, string fileContent)
        {
            if (args is null || haxeProcessStartInfo is null) return string.Empty;
            try
            {
                using var process = new Process();
                process.StartInfo = haxeProcessStartInfo.Value;
                process.StartInfo.Arguments = string.Join(" ", args);
                process.EnableRaisingEvents = true;
                process.Start();
                return process.StandardError.ReadToEnd();
            }
            catch 
            { 
                return string.Empty;
            }
        }

        public void Stop()
        {
        }
    }
}
