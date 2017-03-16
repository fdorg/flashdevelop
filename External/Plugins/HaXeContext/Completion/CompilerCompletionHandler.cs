// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Diagnostics;

namespace HaXeContext
{
    public class CompilerCompletionHandler : IHaxeCompletionHandler
    {
        private Process haxeProcess;

        public CompilerCompletionHandler(Process haxeProcess)
        {
            this.haxeProcess = haxeProcess;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "0");
        }

        public string GetCompletion(string[] args)
        {
            return GetCompletion(args, null);
        }
        public string GetCompletion(string[] args, string fileContent)
        {
            if (args == null || haxeProcess == null)
                return string.Empty;
            try
            {
                haxeProcess.StartInfo.Arguments = String.Join(" ", args);
                haxeProcess.Start();
                var lines = haxeProcess.StandardError.ReadToEnd();
                haxeProcess.Close();
                return lines;
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
