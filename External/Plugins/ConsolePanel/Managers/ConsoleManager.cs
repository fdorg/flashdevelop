using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolePanel.Managers
{
    public delegate void OnExited();

    public class ConsoleManager
    {
        static PluginMain main;

        static List<string> commandList = new List<string>();
        static IConsoleProvider cachedProvider;

        /// <summary>
        /// Creates a new console panel and sends the given string to it.
        /// </summary>
        /// <param name="command">the command to send</param>
        public static void CreateConsole(string command)
        {
            if (main != null)
            {
                processCommandList();
                main.CreateConsolePanel().SendString(command);
            }
            else
            {
                commandList.Add(command);
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
            processCachedProvider();
            processCommandList();
        }

        static void processCommandList()
        {
            while (commandList.Count > 0)
            {
                main.CreateConsolePanel().SendString(commandList[0]);
                commandList.RemoveAt(0);
            }
        }

        static void processCachedProvider()
        {
            if (cachedProvider != null)
            {
                main.ConsoleProvider = cachedProvider;
            }
        }
    }
}
