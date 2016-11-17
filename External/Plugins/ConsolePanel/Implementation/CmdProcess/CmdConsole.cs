using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsolePanel.Implementation.CmdProcess
{
    class CmdConsole : IConsole
    {
        ConsoleControl cmd;

        public Control ConsoleControl
        {
            get
            {
                return cmd;
            }
        }

        public string WorkingDirectory
        {
            set
            {
                cmd.WorkingDirectory = value;
            }
        }

        public event EventHandler Exited;

        public void Clear()
        {
            cmd.SendString("cls");
        }

        public void Cancel()
        {
            cmd.Cancel();
        }

        public void SendString(string str)
        {
            cmd.SendString(str, false);
        }

        public CmdConsole()
        {
            cmd = new ConsoleControl();
            cmd.Text = "cmd";

            cmd.Exited += delegate (object sender, EventArgs e)
            {
                if (Exited != null)
                    Exited(sender, e);
            };
        }
    }
}
