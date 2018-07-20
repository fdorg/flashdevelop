using Microsoft.Win32;
using System;

namespace PluginCore.Helpers
{
    public static class PlatformHelper
    {
        // INIT
        private static Boolean isRunningOnWindows = (Environment.OSVersion.Platform != PlatformID.Unix) && (Environment.OSVersion.Platform != PlatformID.MacOSX);
        private static Boolean isRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Checks if we are running on Windows
        /// </summary>
        public static Boolean IsRunningOnWindows()
        {
            return isRunningOnWindows;
        }

        /// <summary>
        ///  Checks if we are running on Wine
        /// </summary>
        public static Boolean isRunningOnWine()
        {
            return isRunningOnWindows && (Registry.LocalMachine.OpenSubKey(@"Software\Wine\") != null);
        }

        /// <summary>
        /// Checks if we are running on Mono
        /// </summary>
        public static Boolean IsRunningOnMono()
        {
            return isRunningOnMono;
        }
    }
}