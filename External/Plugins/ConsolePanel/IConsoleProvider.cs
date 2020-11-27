namespace ConsolePanel
{
    public interface IConsoleProvider
    {
        IConsole GetConsole();
        IConsole GetConsole(string workingDirectory);
    }
}