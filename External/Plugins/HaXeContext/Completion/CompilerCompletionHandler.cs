using System;
using System.Diagnostics;
using System.Threading;
using HaXeContext.Helpers;

namespace HaXeContext
{
    public class CompilerCompletionHandler : IHaxeCompletionHandler
    {
        private ThreadLocal<ProcessStartInfo> haxeProcessStartInfo;

        public CompilerCompletionHandler(ProcessStartInfo haxeProcessStartInfo)
        {
            this.haxeProcessStartInfo = haxeProcessStartInfo != null ? new ThreadLocal<ProcessStartInfo>(haxeProcessStartInfo.Clone) : null;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "0");
        }

        public string GetCompletion(string[] args)
        {
            return GetCompletion(args, null);
        }
        public string GetCompletion(string[] args, string fileContent)
        {
            if (args == null || haxeProcessStartInfo == null)
                return string.Empty;
            try
            {
                using (var haxeProcess = new Process())
                {
                    haxeProcess.StartInfo = haxeProcessStartInfo.Value;
                    haxeProcess.StartInfo.Arguments = string.Join(" ", args);
                    haxeProcess.EnableRaisingEvents = true;
                    haxeProcess.Start();
                    return haxeProcess.StandardError.ReadToEnd();
                }
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
