// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.IO;

namespace ProjectManager.Projects
{
    public class HiddenPathCollection : CollectionBase, IAddPaths
    {
        public void Add(string path)
        {
            // remove any now-redundant paths underneath this one
            for (int i = 0; i < List.Count; i++)
            {
                string hiddenPath = List[i] as string;

                if (hiddenPath.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                    || hiddenPath == path)
                {
                    List.RemoveAt(i--); // search this index again
                }
            }
            List.Add(path);
        }

        public void Remove(string path)
        {
            // unhide this path and any parent paths
            for (int i = 0; i < List.Count; i++)
            {
                string hiddenPath = List[i] as string;

                if (hiddenPath == path
                    || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                {
                    List.RemoveAt(i--); // search this index again
                }
            }
        }

        public bool IsHidden(string path)
        {
            foreach (string hiddenPath in List)
                if (hiddenPath == path || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                    return true;
            return false;
        }

        public bool IsHiddenIgnoreCase(string path)
        {
            foreach (string hiddenPath in List)
            {
                if (hiddenPath.Equals(path, StringComparison.OrdinalIgnoreCase) 
                    || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public string[] ToArray()
        {
            string[] hiddenPaths = new string[List.Count];
            for (int i = 0; i < List.Count; i++)
            {
                hiddenPaths.SetValue(List[i], i);
            }
            return hiddenPaths;
        }
    }
}