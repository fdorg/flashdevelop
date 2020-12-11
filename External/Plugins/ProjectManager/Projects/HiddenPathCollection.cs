// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ProjectManager.Projects
{
    public class HiddenPathCollection : Collection<string>, IAddPaths
    {
        public new void Add(string path)
        {
            // remove any now-redundant paths underneath this one
            for (int i = 0; i < Count; i++)
            {
                var hiddenPath = this[i];
                if (hiddenPath.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                    || hiddenPath == path)
                {
                    RemoveAt(i--); // search this index again
                }
            }
            base.Add(path);
        }

        public new void Remove(string path)
        {
            // unhide this path and any parent paths
            for (int i = 0; i < Count; i++)
            {
                var hiddenPath = this[i];
                if (hiddenPath == path
                    || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                {
                    RemoveAt(i--); // search this index again
                }
            }
        }

        public bool IsHidden(string path)
            => this.Any(hiddenPath => hiddenPath == path || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.Ordinal));

        public bool IsHiddenIgnoreCase(string path)
            => this.Any(hiddenPath => hiddenPath.Equals(path, StringComparison.OrdinalIgnoreCase) || path.StartsWith(hiddenPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));

        public string[] ToArray() => Enumerable.ToArray(this);
    }
}