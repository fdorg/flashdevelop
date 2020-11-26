using System;
using System.Diagnostics;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
    public class ProcessHelper
    {
        /// <summary>
        /// Starts a basic process asynchronously
        /// </summary>
        public static void StartAsync(string path)
        {
            StartDelegate1 dlgt = Start;
            dlgt.BeginInvoke(path, ar =>
            {
                try
                {
                    dlgt.EndInvoke(ar);
                }
                catch
                {
                    // Something wrong, handling for possible leaks
                }
            }, null);
        }
        
        /// <summary>
        /// Starts a process with arguments asynchronously
        /// </summary>
        public static void StartAsync(string path, string arguments)
        {
            StartDelegate2 dlgt = Start;
            dlgt.BeginInvoke(path, arguments, ar =>
            {
                try
                {
                    dlgt.EndInvoke(ar);
                }
                catch
                {
                    // Something wrong, handling for possible leaks
                }
            }, null);
        }

        /// <summary>
        /// Starts a process with start info asynchronously
        /// </summary>
        public static void StartAsync(ProcessStartInfo psi)
        {
            StartDelegate3 dlgt = Start;
            dlgt.BeginInvoke(psi, ar =>
                {
                    try
                    {
                        dlgt.EndInvoke(ar);
                    }
                    catch
                    {
                        // Something wrong, handling for possible leaks
                    }
                },
            null);
        }

        /// <summary>
        /// Runs a basic process
        /// </summary>
        static void Start(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Runs a process with arguments
        /// </summary>
        static void Start(string path, string arguments)
        {
            try
            {
                Process.Start(path, arguments);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Runs a process with start info
        /// </summary>
        static void Start(ProcessStartInfo psi)
        {
            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Event delegates of the class
        /// </summary>
        delegate void StartDelegate1(string path);

        delegate void StartDelegate2(string path, string arguments);

        delegate void StartDelegate3(ProcessStartInfo psi);
    }
}