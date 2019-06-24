using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace AS3Context.Compiler
{
    class FlexDebugger
    {
        #region Public interface

        public static bool Start(string projectPath, string flex2Path, DataEvent message)
        {
            if (ignoreMessage) return false;
            try
            {
                debugger?.Cleanup();
                startMessage = message;
                debugger = new FdbWrapper();
                debugger.OnStarted += debugger_OnStarted;
                debugger.OnTrace += debugger_OnTrace;
                debugger.OnError += debugger_OnError;
                if (PluginMain.Settings.VerboseFDB)
                    debugger.OnOutput += debugger_OnOutput;
                debugger.Run(projectPath, flex2Path);
                TraceManager.AddAsync(TextHelper.GetString("Info.CapturingTracesWithFDB"));
                return true;
            }
            catch
            {
                TraceManager.AddAsync(TextHelper.GetString("Info.FailedToLaunchFBD"), 3);
            }
            return false;
        }

        public static void Stop()
        {
            if (debugger != null)
            {
                debugger.Cleanup();
                debugger = null;
                startMessage = null;
                ignoreMessage = false;
            }
        }

        #endregion

        private static bool ignoreMessage;
        private static FdbWrapper debugger;
        private static DataEvent startMessage;

        static void debugger_OnStarted(string line)
        {
            if (startMessage == null) return;
            PluginBase.RunAsync(delegate
            {
                // send message again
                ignoreMessage = true;
                EventManager.DispatchEvent(null, startMessage);
                startMessage = null;
                ignoreMessage = false;
            });
        }

        static void debugger_OnError(string line) => TraceManager.AddAsync(line, 3);

        static void debugger_OnOutput(string line) => TraceManager.AddAsync(line, 1);

        static void debugger_OnTrace(string line) => TraceManager.AddAsync(line, 1);
    }
}
