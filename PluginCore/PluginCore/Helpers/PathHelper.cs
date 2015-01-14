using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Security;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
    public class PathHelper
    {
        /// <summary>
        /// Path to the current application directory
        /// </summary>
        public static String BaseDir
        {
            get
            {
                if (PluginBase.MainForm.StandaloneMode) return AppDir;
                else return UserAppDir;
            }
        }

        /// <summary>
        /// Path to the main application directory
        /// </summary>
        public static String AppDir
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath);
            }
        }

        /// <summary>
        /// Path to the user's application directory
        /// </summary>
        public static String UserAppDir
        {
            get
            {
                String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(userAppDir, "FlashDevelop");
            }
        }

        /// <summary>
        /// Path to the docs directory
        /// </summary>
        public static String DocDir
        {
            get
            {
                return Path.Combine(AppDir, "Docs");
            }
        }

        /// <summary>
        /// Path to the data directory
        /// </summary>
        public static String DataDir
        {
            get
            {
                return Path.Combine(BaseDir, "Data");
            }
        }

        /// <summary>
        /// Path to the snippets directory
        /// </summary>
        public static String SnippetDir
        {
            get
            {
                String custom = PluginBase.Settings.CustomSnippetDir;
                if (!String.IsNullOrEmpty(custom) && Directory.Exists(custom)) return custom;
                else return Path.Combine(BaseDir, "Snippets");
            }
        }

        /// <summary>
        /// Path to the templates directory
        /// </summary>
        public static String TemplateDir
        {
            get
            {
                String custom = PluginBase.Settings.CustomTemplateDir;
                if (!String.IsNullOrEmpty(custom) && Directory.Exists(custom)) return custom;
                else return Path.Combine(BaseDir, "Templates");
            }
        }

        /// <summary>
        /// Path to the project templates directory
        /// </summary>
        public static String ProjectsDir
        {
            get
            {
                String custom = PluginBase.Settings.CustomProjectsDir;
                if (!String.IsNullOrEmpty(custom) && Directory.Exists(custom)) return custom;
                else return Path.Combine(AppDir, "Projects");
            }
        }

        /// <summary>
        /// Path to the settings directory
        /// </summary>
        public static String SettingDir
        {
            get
            {
                return Path.Combine(BaseDir, "Settings");
            }
        }

        /// <summary>
        /// Path to the themes directory
        /// </summary>
        public static String ThemesDir
        {
            get
            {
                return Path.Combine(SettingDir, "Themes");
            }
        }

        /// <summary>
        /// Path to the user project templates directory
        /// </summary>
        public static String UserProjectsDir
        {
            get
            {
                return Path.Combine(UserAppDir, "Projects");
            }
        }

        /// <summary>
        /// Path to the user lirbrary directory
        /// </summary>
        public static String UserLibraryDir
        {
            get
            {
                return Path.Combine(UserAppDir, "Library");
            }
        }

        /// <summary>
        /// Path to the library directory
        /// </summary>
        public static String LibraryDir
        {
            get
            {
                return Path.Combine(AppDir, "Library");
            }
        }

        /// <summary>
        /// Path to the plugin directory
        /// </summary>
        public static String PluginDir
        {
            get
            {
                return Path.Combine(AppDir, "Plugins");
            }
        }

        /// <summary>
        /// Path to the users plugin directory
        /// </summary>
        public static String UserPluginDir
        {
            get
            {
                return Path.Combine(UserAppDir, "Plugins");
            }
        }

        /// <summary>
        /// Path to the tools directory
        /// </summary>
        public static String ToolDir
        {
            get
            {
                return Path.Combine(AppDir, "Tools");
            }
        }

        /// <summary>
        /// Resolve the path to the mm.cfg file
        /// </summary>
        public static String ResolveMMConfig()
        {
            String homePath = Environment.GetEnvironmentVariable("HOMEPATH");
            String homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
            if (!String.IsNullOrEmpty(homeDrive) && homePath != null)
            {
                try
                {
                    String tempPath = homeDrive + homePath;
                    DirectorySecurity security = Directory.GetAccessControl(tempPath);
                    AuthorizationRuleCollection rules = security.GetAccessRules(true, true, typeof(SecurityIdentifier));
                    WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                    foreach (FileSystemAccessRule rule in rules)
                    {
                        if (currentUser.User.Equals(rule.IdentityReference))
                        {
                            if (rule.AccessControlType.Equals(AccessControlType.Allow))
                            {
                                return Path.Combine(tempPath, "mm.cfg");
                            }
                        }
                    }
                }
                catch {} // Not working...
            }
            String userProfile = Environment.GetEnvironmentVariable(Win32.IsRunningOnWindows() ? "USERPROFILE" : "HOME");
            return Path.Combine(userProfile, "mm.cfg");
        }

        /// <summary>
        /// Resolve a path which may be:
        /// - absolute or
        /// - relative to base path
        /// </summary>
        public static String ResolvePath(String path)
        {
            return ResolvePath(path, null);
        }

        /// <summary>
        /// Resolve a path which may be:
        /// - absolute or
        /// - relative to a specified path, or 
        /// - relative to base path
        /// </summary>
        public static String ResolvePath(String path, String relativeTo)
        {
            if (path == null || path.Length == 0) return null;
            Boolean isPathNetworked = path.StartsWith("\\\\") || path.StartsWith("//");
            Boolean isPathAbsSlashed = (path.StartsWith("\\") || path.StartsWith("/")) && !isPathNetworked;
            if (isPathAbsSlashed) path = Path.GetPathRoot(AppDir) + path.Substring(1);
            if (Path.IsPathRooted(path) || isPathNetworked) return path;
            String resolvedPath;
            if (relativeTo != null)
            {
                resolvedPath = Path.Combine(relativeTo, path);
                if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            }
            if (!PluginBase.MainForm.StandaloneMode)
            {
                resolvedPath = Path.Combine(UserAppDir, path);
                if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            }
            resolvedPath = Path.Combine(AppDir, path);
            if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            return null;
        }

        /// <summary>
        /// Converts a long path to a short representative one using ellipsis if necessary
        /// </summary>
        public static String GetCompactPath(String path)
        {
            try
            {
                if (Win32.ShouldUseWin32())
                {
                    Int32 max = 64;
                    StringBuilder sb = new StringBuilder(max);
                    Win32.PathCompactPathEx(sb, path, max, 0);
                    return sb.ToString();
                }
                else // For other platforms
                {
                    const String pattern = @"^(w+:|)([^]+[^]+).*([^]+[^]+)$";
                    const String replacement = "$1$2...$3";
                    if (Regex.IsMatch(path, pattern)) return Regex.Replace(path, pattern, replacement);
                    else return path;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return path;
            }
        }

        /// <summary>
        /// Converts a long filename to a short one
        /// </summary>
        public static String GetShortPathName(String longName)
        {
            try
            {
                if (Win32.ShouldUseWin32())
                {
                    Int32 max = longName.Length + 1;
                    StringBuilder sb = new StringBuilder(max);
                    Win32.GetShortPathName(longName, sb, max);
                    return sb.ToString();
                }
                else return longName; // For other platforms
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return longName;
            }
        }

        /// <summary>
        /// Converts a short filename to a long one
        /// </summary>
        public static String GetLongPathName(String shortName)
        {
            try
            {
                if (Win32.ShouldUseWin32())
                {
                    StringBuilder longNameBuffer = new StringBuilder(256);
                    Win32.GetLongPathName(shortName, longNameBuffer, longNameBuffer.Capacity);
                    return longNameBuffer.ToString();
                }
                else return shortName; // For other platforms
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return shortName;
            }
        }

        /// <summary>
        /// Gets the correct physical path from the file system
        /// </summary>
        public static String GetPhysicalPathName(String path)
        {
            try
            {
                if (Win32.ShouldUseWin32())
                {
                    uint r;
                    IntPtr ppidl;
                    int rgflnOut = 0;
                    r = Win32.SHILCreateFromPath(path, out ppidl, ref rgflnOut);
                    if (r == 0)
                    {
                        StringBuilder sb = new StringBuilder(260);
                        if (Win32.SHGetPathFromIDList(ppidl, sb))
                        {
                            Char sep = Path.DirectorySeparatorChar;
                            Char alt = Path.AltDirectorySeparatorChar;
                            return sb.ToString().Replace(alt, sep);
                        }
                    }
                }
                return path;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return path;
            }
        }

        /// <summary>
        /// Gets the 32-bit Java install path
        /// </summary>
        public static String GetJavaInstallPath()
        {
            String javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(javaKey))
            {
                String currentVersion = rk.GetValue("CurrentVersion").ToString();
                using (RegistryKey key = rk.OpenSubKey(currentVersion))
                {
                    return key.GetValue("JavaHome").ToString();
                }
            }
        }

    }

}
