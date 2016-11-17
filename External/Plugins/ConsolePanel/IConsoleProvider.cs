using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolePanel
{
    public interface IConsoleProvider
    {
        IConsole GetConsole();
    }
}
