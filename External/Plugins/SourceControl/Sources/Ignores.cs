// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore;

namespace SourceControl.Sources
{
    internal class Ignores: List<IgnoreEntry>
    {
        readonly string root;
        readonly string ignoreFile;
        DateTime lastWrite;

        public Ignores(string path, string name)
        {
            root = path;
            ignoreFile = name;
        }

        public void Update()
        {
            // TODO ignore files should be explored in each sub directory
            var info = new FileInfo(Path.Combine(root, ignoreFile));
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
                var lines = File.ReadAllLines(info.FullName);
                foreach (var line in lines)
                {
                    var entry = line.Trim();
                    if (entry.StartsWith('#') || entry.Length == 0) continue;
                    entry = Regex.Escape(entry);
                    if (entry.StartsWith('/')) entry = "^" + entry.Substring(1);
                    entry = entry.Replace("\\*", ".*");
                    entry = entry.Replace("/", "\\\\");
                    Add(new IgnoreEntry("", new Regex(entry)));
                }
            }
            catch { }
        }
    }

    internal class IgnoreEntry
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
