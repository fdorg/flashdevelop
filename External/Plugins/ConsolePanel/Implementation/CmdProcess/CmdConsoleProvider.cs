namespace ConsolePanel.Implementation.CmdProcess
{
    class CmdConsoleProvider : IConsoleProvider
    {
        public IConsole GetConsole() => GetConsole(null);
        public IConsole GetConsole(string workingDirectory) => new CmdConsole(workingDirectory);
    }
}
