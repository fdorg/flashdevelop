using System;
using System.Diagnostics;

namespace HaXeContext
{
    class CompilerCompletionHandler : IHaxeCompletionHandler
    {
        private Process haxeProcess;

        public CompilerCompletionHandler(Process haxeProcess)
        {
            this.haxeProcess = haxeProcess;
            Environment.SetEnvironmentVariable("HAXE_SERVER_PORT", "0");
        }

        public string GetCompletion(string[] args)
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
