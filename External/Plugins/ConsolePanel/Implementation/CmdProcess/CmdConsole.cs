using System;
using System.Windows.Forms;

namespace ConsolePanel.Implementation.CmdProcess
{
    class CmdConsole : IConsole
    {
        readonly ConsoleControl cmd;

        public CmdConsole() : this(null)
        {
        }

        public CmdConsole(string workingDirectory)
        {
            cmd = new ConsoleControl(true, workingDirectory) { Text = "cmd" };
            cmd.Exited += (sender, e) => Exited?.Invoke(sender, e);
        }

        public Control ConsoleControl => cmd;

        public string WorkingDirectory
        {
            set => cmd.WorkingDirectory = value;
        }

        public event EventHandler Exited;

        public void Clear() => cmd.SendString("cls");

        public void Cancel() => cmd.Cancel();

        public void SendString(string str) => cmd.SendString(str, false);
    }
}
