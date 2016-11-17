using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsolePanel
{
    /// <summary>
    /// Interface to implement a different way of embedding a console window
    /// </summary>
    public interface IConsole
    {
        event EventHandler Exited;

        /// <summary>
        /// The Control that is added to the FlashDevelop form
        /// </summary>
        Control ConsoleControl
        {
            get;
        }

        string WorkingDirectory
        {
            //get;
            set;
        }

        void Clear();

        void Cancel();

        /// <summary>
        /// Sends a string to the command line
        /// </summary>
        /// <param name="str"></param>
        void SendString(string str);
    }
}
