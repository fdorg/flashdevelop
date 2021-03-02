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
            => this.Any(it => it == path || path.StartsWith(it + Path.DirectorySeparatorChar, StringComparison.Ordinal));

        public bool IsHiddenIgnoreCase(string path)
            => this.Any(it => it.Equals(path, StringComparison.OrdinalIgnoreCase) || path.StartsWith(it + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));

        public string[] ToArray() => Enumerable.ToArray(this);
    }
}