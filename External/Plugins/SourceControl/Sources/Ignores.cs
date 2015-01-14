using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SourceControl.Sources
{
    class Ignores: List<IgnoreEntry>
    {
        string root;
        string ignoreFile;
        DateTime lastWrite;

        public Ignores(string path, string name)
        {
            root = path;
            ignoreFile = name;
        }

        public void Update()
        {
            // TODO ignore files should be explored in each sub directory
            FileInfo info = new FileInfo(Path.Combine(root, ignoreFile));
            if (!info.Exists)
            {
                Clear();
                return;
            }
            if (info.LastWriteTime == lastWrite)
                return;
            lastWrite = info.LastWriteTime;
            Clear();

            try
            {
                string[] lines = File.ReadAllLines(info.FullName);
                foreach (string line in lines)
                {
                    string entry = line.Trim();
                    if (entry.StartsWith("#") || entry.Length == 0) continue;
                    entry = Regex.Escape(entry);
                    if (entry.StartsWith("/")) entry = "^" + entry.Substring(1);
                    entry = entry.Replace("\\*", ".*");
                    entry = entry.Replace("/", "\\");
                    Add(new IgnoreEntry("", new Regex(entry)));
                }
            }
            catch { }
        }
    }

    class IgnoreEntry
    {
        public string path;
        public Regex regex;

        public IgnoreEntry(string path, Regex regex)
        {
            this.path = path;
            this.regex = regex;
        }
    }
}
