using System;
using System.Collections;
using System.IO;
using System.Reflection;
using PluginCore;

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

            ArrayList relPath = new ArrayList();
            int i = 0;

            // skip equal parts
            for (i = 0; i < a.Length && i < b.Length; i++)
            {
                if (string.Compare(a[i],b[i],true) != 0)
                    break;
            }

            // only common drive letter, consider not relative
            if (i <= 1)
                return path; 

            // at this point, i is the index of the first diverging element of the two paths
            int backtracks = a.Length - i;
            for (int j = 0; j < backtracks; j++)
                relPath.Add("..");

            for (int j = i; j < b.Length; j++)
                relPath.Add(b[j]);

            string relativePath = string.Join(slash.ToString(), relPath.ToArray(typeof(string)) as string[]);
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

        public static string ApplicationDirectory
        {
            get
            {
                string url = Assembly.GetEntryAssembly().GetName().CodeBase;
                Uri uri = new Uri(url);
                return Path.GetDirectoryName(uri.LocalPath);
            }
        }

        public static string DefaultProjectsDirectory 
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
        }
    }
}
