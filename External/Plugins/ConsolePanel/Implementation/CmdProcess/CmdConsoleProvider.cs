using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolePanel.Implementation.CmdProcess
{
    class CmdConsoleProvider : IConsoleProvider
    {
        public IConsole GetConsole()
        {
            return new CmdConsole();
        }
    }
}
