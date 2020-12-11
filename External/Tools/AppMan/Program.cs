// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using System.Threading;

namespace AppMan
{
    internal static class Program
    {
        /// <summary>
        /// SIA Mutex entry
        /// </summary>
        static readonly Mutex mutex = new Mutex(true, "{2A46BA9B-F8DA-40AA-904F-4C1630BA4428}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm(args));
                    mutex.ReleaseMutex();
                }
                else if (Array.IndexOf(args, "-minimized") == -1)
                {
                    if (Win32.IsRunningOnMono) MessageBox.Show("AppMan is already running.");
                    else Win32.PostMessage((IntPtr)Win32.HWND_BROADCAST, Win32.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while starting AppMan:\n" + ex.ToString());
            }
        }
    }
}