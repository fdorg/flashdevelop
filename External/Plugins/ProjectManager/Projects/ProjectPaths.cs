// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ProjectManager.Projects
{
    public partial class ProjectPaths
    {
        public static string GetRelativePath(string baseDirectory, string path)
        {
            if (!Path.IsPathRooted(path))
                throw new ArgumentException("The path is already relative.");

            char slash = Path.DirectorySeparatorChar;
            path = path.TrimEnd(slash);
            baseDirectory = baseDirectory.TrimEnd(slash);

            // trivial cases
            if (path == baseDirectory) 
                return "";
            if (path[1] == ':' && path[0] != baseDirectory[0]) // drive
                return path; 
            if (path.Length > baseDirectory.Length && path.StartsWith(baseDirectory + slash, StringComparison.Ordinal))
                return path.Substring(baseDirectory.Length + 1);

            // resolve relative path
            string[] a = baseDirectory.Split(slash);
            string[] b = path.Split(slash);

            int i = 0;

            // skip equal parts
            for (i = 0; i < a.Length && i < b.Length; i++)
            {
                if (string.Compare(a[i],b[i],true) != 0)
                    break;
            }

            // only common drive letter, consider not relative
            if (i <= 1) return path;

            // at this point, i is the index of the first diverging element of the two paths
            var relPath = new List<string>();
            int backtracks = a.Length - i;
            for (int j = 0; j < backtracks; j++)
                relPath.Add("..");

            for (int j = i; j < b.Length; j++)
                relPath.Add(b[j]);

            string relativePath = string.Join(slash.ToString(), relPath);
            string special = (relativePath.Length > 0) ? relativePath : "."; // special case

            if (special.StartsWith("..", StringComparison.Ordinal) && special.Contains(":")) // invalid relative path...
            {
                special = Path.GetFullPath(path);
            }
            return special;
        }

        public static string GetAbsolutePath(string baseDirectory, string path)
        {
            if (Path.IsPathRooted(path)) return path;
            string combinedPath = Path.Combine(baseDirectory, path);
            return Path.GetFullPath(combinedPath);
        }

        public static string ApplicationDirectory => applicationDirectory ??= Path.GetDirectoryName(GetAssemblyPath(Assembly.GetEntryAssembly()));

        static string applicationDirectory;

        public static string DefaultProjectsDirectory => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        /// <summary>
        /// Path to the main application directory
        /// </summary>
        public static string AppDir => appDir ??= Path.GetDirectoryName(GetAssemblyPath(Assembly.GetExecutingAssembly()));

        static string appDir;

        public static string GetAssemblyPath(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            if (!codeBase.ToLower().StartsWith(Uri.UriSchemeFile)) return assembly.Location;
            // Skip over the file:// part
            var start = Uri.UriSchemeFile.Length + Uri.SchemeDelimiter.Length;
            if (codeBase[start] == '/') // third slash means a local path
            {
                // Handle Windows Drive specifications
                if (codeBase[start + 2] == ':')
                    ++start;
                // else leave the last slash so path is absolute
            }
            else // It's either a Windows Drive spec or a share
            {
                if (codeBase[start + 1] != ':')
                    start -= 2; // Back up to include two slashes
            }
            return codeBase.Substring(start);
        }
    }
}
