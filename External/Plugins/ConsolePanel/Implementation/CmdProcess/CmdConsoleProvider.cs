namespace ConsolePanel.Implementation.CmdProcess
{
    class CmdConsoleProvider : IConsoleProvider
    {
        public IConsole GetConsole() => new CmdConsole();
    }
}
