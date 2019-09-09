// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SnippetEditor
{
    static class Program
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        [STAThread]
        static void Main(String[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(arguments));
        }

    }

}