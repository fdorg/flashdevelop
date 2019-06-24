using Microsoft.Win32;
using System;

namespace PluginCore.Helpers
{
    public static class PlatformHelper
    {
        // INIT
        private static readonly bool isRunningOnWindows = (Environment.OSVersion.Platform != PlatformID.Unix) && (Environment.OSVersion.Platform != PlatformID.MacOSX);
        private static readonly bool isRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Checks if we are running on Windows
        /// </summary>
        public static bool IsRunningOnWindows() => isRunningOnWindows;

        /// <summary>
        ///  Checks if we are running on Wine
        /// </summary>
        public static bool isRunningOnWine()
        {
            return isRunningOnWindows && (Registry.LocalMachine.OpenSubKey(@"Software\Wine\") != null);
        }

        /// <summary>
        /// Checks if we are running on Mono
        /// </summary>
        public static bool IsRunningOnMono() => isRunningOnMono;
    }
}