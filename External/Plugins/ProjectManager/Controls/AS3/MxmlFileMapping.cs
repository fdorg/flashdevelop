using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore.Helpers;
using ProjectManager.Controls.TreeView;

namespace ProjectManager.Controls.AS3
{
    static class MxmlFileMapping
    {
        private static Regex reSource = new Regex("\\ssource=\"(?<file>[^\"]+)\"", RegexOptions.Compiled);

        public static void AddMxmlMapping(FileMappingRequest request)
        {
            foreach (string file in request.Files)
                if (FileInspector.IsMxml(file, Path.GetExtension(file).ToLower()))
                {
                    foreach (string includedFile in GetIncludedFiles(file))
                        request.Mapping.Map(includedFile, file);
                }
        }

        private static string[] GetIncludedFiles(string mxmlFile)
        {
            string dir = Path.GetDirectoryName(mxmlFile);
            List<string> included = new List<string>();

            try
            {
                string src = File.ReadAllText(mxmlFile);
                MatchCollection matches = reSource.Matches(src);
                if (matches.Count > 0)
                    foreach (Match match in matches)
                        included.Add(Path.Combine(dir, match.Groups["file"].Value));
                /*
                XmlReader reader = XmlReader.Create(mxmlFile);
                while (reader.Read())
                {
                    if (reader.Name == "mx:Script")
                    {
                        string source = reader.GetAttribute("source");
                        if (source != null && source != "")
                            included.Add(Path.Combine(dir, source));
                    }
                }
                reader.Close();*/
            }
            // this is just a convenience feature, no big deal if it fails
            catch { }

            return included.ToArray();
        }
    }
}
