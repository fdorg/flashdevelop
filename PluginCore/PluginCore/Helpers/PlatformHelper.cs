// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Microsoft.Win32;
using System;

namespace PluginCore.Helpers
{
    public static class PlatformHelper
    {
        // INIT
        static readonly bool isRunningOnWindows = Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX;
        static readonly bool isRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Checks if we are running on Windows
        /// </summary>
        public static bool IsRunningOnWindows() => isRunningOnWindows;

        /// <summary>
        ///  Checks if we are running on Wine
        /// </summary>
        public static bool isRunningOnWine() => isRunningOnWindows && Registry.LocalMachine.OpenSubKey(@"Software\Wine\") != null;

        /// <summary>
        /// Checks if we are running on Mono
        /// </summary>
        public static bool IsRunningOnMono() => isRunningOnMono;
    }
}