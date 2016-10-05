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

        /// <summary>
        /// Runs a command in a new console window.
        /// </summary>
        /// <param name="command">the command to run, uses standard cmd input syntax</param>
        public static void RunCommand(string command)
        {
            if (main != null)
            {
                while(commandList.Count > 0)
                {
                    main.CreateConsolePanel().SendString(commandList[0]);
                    commandList.RemoveAt(0);
                }
                main.CreateConsolePanel().SendString(command);
            }
            else
            {
                commandList.Add(command);
            }
        }

        public static void Init(PluginMain plugin)
        {
            main = plugin;
        }
    }
}
