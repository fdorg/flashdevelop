using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore.Helpers;
using ProjectManager.Controls.TreeView;

namespace ProjectManager.Controls.AS3
{
    internal static class MxmlFileMapping
    {
        static readonly Regex reSource = new Regex("\\ssource=\"(?<file>[^\"]+)\"", RegexOptions.Compiled);

        public static void AddMxmlMapping(FileMappingRequest request)
        {
            foreach (string file in request.Files)
                if (FileInspector.IsMxml(Path.GetExtension(file).ToLower()))
                {
                    foreach (string includedFile in GetIncludedFiles(file))
                        request.Mapping.Map(includedFile, file);
                }
        }

        static string[] GetIncludedFiles(string mxmlFile)
        {
            var dir = Path.GetDirectoryName(mxmlFile);
            var included = new List<string>();
            try
            {
                var src = File.ReadAllText(mxmlFile);
                var matches = reSource.Matches(src);
                if (matches.Count > 0)
                    foreach (Match match in matches)
                        included.Add(Path.Combine(dir, match.Groups["file"].Value));
            }
            // this is just a convenience feature, no big deal if it fails
            catch { }
            return included.ToArray();
        }
    }
}