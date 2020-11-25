using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Automation;
using ConsoleControl;

namespace ConsolePanel.Implementation.CmdProcess
{
    public partial class ConsoleControl : UserControl
    {
        IntPtr cmdHandle;
        AutomationElement window;
        Size realSize;

        ConsoleColor backColor = ConsoleColor.Black;
        ConsoleColor foreColor = ConsoleColor.White;
        readonly List<string> commandsToDo = new List<string>();
        string lastWorkingDir;

        public event EventHandler Exited;

        public string WorkingDirectory
        {
            set
            {
                lastWorkingDir = value;
                if (Process == null) Create();
                else
                {
                    SendString("cd \"" + value + "\"");
                    SendString("cls");
                }
            }
        }

        public ConsoleColor ConsoleBackColor
        {
            get => backColor;
            set
            {
                if (value == backColor) return;
                backColor = value;
                var trimmedFore = foreColor.ToString("X").TrimStart('0');
                var trimmedBack = backColor.ToString("X").TrimStart('0');
                if (trimmedFore == "") trimmedFore = "0";
                if (trimmedBack == "") trimmedBack = "0";
                SendString("color " + trimmedBack + trimmedFore);
                SendString("cls");
            }
        }

        public ConsoleColor ConsoleForeColor
        {
            get => foreColor;
            set
            {
                if (value == foreColor) return;
                foreColor = value;
                var trimmedFore = foreColor.ToString("X").TrimStart('0');
                var trimmedBack = backColor.ToString("X").TrimStart('0');
                if (trimmedFore == "") trimmedFore = "0";
                if (trimmedBack == "") trimmedBack = "0";
                SendString("color " + trimmedBack + trimmedFore);
                SendString("cls");
            }
        }

        /// <summary>
        /// Returns the actual size of the console window's client area
        /// (the width of a console window only changes by specific values)
        /// </summary>
        public Size ActualSize => realSize;

        public Process Process { get; set; }

        /// <summary>
        /// Creates a new ConsoleControl
        /// </summary>
        /// <param name="init"></param>
        /// <param name="workingDirectory"></param>
        public ConsoleControl(bool init = true, string workingDirectory = null)
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, true);
            lastWorkingDir = workingDirectory;
            if (init) Create();
        }

        /// <summary>
        /// Cancel the currently running process
        /// </summary>
        public void Cancel()
        {
            if (Process is null) return;
            if (!Process.HasExited)
            {
                SendString("^(c)", false);
                try
                {
                    Process.Kill();
                }
                catch { }
                    
            }
            Process = null;
        }

        /// <summary>
        /// Creates the console window if it was closed / not created yet
        /// </summary>
        public void Create()
        {
            if (Process != null && !Process.HasExited) return;
            try
            {
                Process = new Process();
                Process.StartInfo.FileName = "cmd";
                if (lastWorkingDir != null) Process.StartInfo.WorkingDirectory = lastWorkingDir;
                Process.StartInfo.UseShellExecute = false;
                Process.EnableRaisingEvents = true;
                Process.Exited += Process_Exited;
                Process.Start();

                //Wait for cmd window
                while (Process.MainWindowHandle == IntPtr.Zero)
                {
                    Process.Refresh();
                }
                cmdHandle = Process.MainWindowHandle;
                window = AutomationElement.FromHandle(cmdHandle);
                WinApi.SetParent(cmdHandle, pnlClipping.Handle);
                SendString(" ",false);
                ResizeConsole();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends a String to the embedded console window
        /// </summary>
        /// <param name="str">The string to send</param>
        /// <param name="execute">if true, a "\r" is appended to the given string</param>
        public void SendString(string str, bool execute = true)
        {
            if (execute) str += "\r";
            ProcessCommandCache();
            try
            {
                RunCommandWithoutCache(str);
            }
            catch
            {
                commandsToDo.Add(str);
            }
        }

        void ProcessCommandCache()
        {
            while (commandsToDo.Count > 0)
            {
                var toDo = commandsToDo[0];
                try
                {
                    RunCommandWithoutCache(toDo);
                    commandsToDo.RemoveAt(0);
                }
                catch
                {
                    break;
                }
            }
        }

        void RunCommandWithoutCache(string cmd)
        {
            window.SetFocus();
            //TODO: prevent user from clicking around while doing this
            SendKeys.SendWait(cmd);
            Focus();
        }

        void ResizeConsole()
        {
            WinApi.ShowWindow(cmdHandle, WinApi.SW_SHOWMAXIMIZED);
            WinApi.ResizeClientRectTo(cmdHandle, new Rectangle(new Point(0, 0), Size));
            SetRealConsoleSize();
            var tooWide = realSize.Width > Width;
            var tooHigh = realSize.Height > Height;
            if (tooWide || tooHigh)
            {
                int newWidth = realSize.Width;
                int newHeight = realSize.Height;
                if (tooWide) newWidth -= 8;
                if (tooHigh) newHeight -= 12;
                WinApi.ResizeClientRectTo(cmdHandle, new Rectangle(0, 0, newWidth, newHeight));
                SetRealConsoleSize();
            }
            pnlClipping.Size = realSize;
        }

        void SetRealConsoleSize()
        {
            WinApi.GetWindowInfo(cmdHandle, out var info);
            var leftBorder = info.rcClient.Left - info.rcWindow.Left;
            var topBorder = info.rcClient.Top - info.rcWindow.Top;
            var rightBorder = (int)info.cxWindowBorders;
            var bottomBorder = (int)info.cyWindowBorders;
            var width = info.rcWindow.Right - info.rcWindow.Left;
            var height = info.rcWindow.Bottom - info.rcWindow.Top;

            realSize.Width = width - leftBorder - rightBorder;
            realSize.Height = height - topBorder - bottomBorder;
        }

        void CmdPanel_Resize(object sender, EventArgs e) => ResizeConsole();

        void Process_Exited(object sender, EventArgs e)
        {
            cmdHandle = IntPtr.Zero;
            Exited?.Invoke(sender, e);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            Cancel();
            base.Dispose(disposing);
        }

        void CmdPanel_Enter(object sender, EventArgs e)
        {
            try
            {
                window.SetFocus();
            }
            catch { }
            ProcessCommandCache();
        }

        void ConsoleControl_Paint(object sender, PaintEventArgs e) => ResizeConsole();
    }
}
