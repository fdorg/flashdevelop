// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.IO;

namespace SourceControl.Sources
{
    public class VCHelper
    {
        public static string EscapeCommandLine(string cmd)
        {
            return cmd.Replace("\"", "\\\"");
        }

        public static string GetRelativePath(string file, string folder)
        {
            var dir = Path.GetFullPath(folder);
            var fullFile = Path.GetFullPath(file);

            return fullFile.StartsWith(dir) ? fullFile.Substring(dir.Length + 1) : null;
        }
    }
}