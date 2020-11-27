using System.Collections.Generic;

namespace ConsolePanel.Managers
{
    public class ConsoleManager
    {
        static PluginMain main;

        static readonly List<string> CommandList = new List<string>();
        static IConsoleProvider cachedProvider;

        /// <summary>
        /// Creates a new console panel and sends the given string to it.
        /// </summary>
        /// <param name="command">the command to send</param>
        public static void CreateConsole(string command)
        {
            if (main != null)
            {
                ProcessCommandList();
                main.CreateConsolePanel().SendString(command);
            }
            else
            {
                CommandList.Add(command);
            }
        }

        public static void SetConsoleProvider(IConsoleProvider provider)
        {
            if (main != null)
            {
                main.ConsoleProvider = provider;
            }
            else
            {
                cachedProvider = provider;
            }
        }

        public static void Init(PluginMain plugin)
        {
            main = plugin;
            ProcessCachedProvider();
            ProcessCommandList();
        }

        static void ProcessCommandList()
        {
            while (CommandList.Count > 0)
            {
                main.CreateConsolePanel().SendString(CommandList[0]);
                CommandList.RemoveAt(0);
            }
        }

        static void ProcessCachedProvider()
        {
            if (cachedProvider is null) return;
            main.ConsoleProvider = cachedProvider;
        }
    }
}
