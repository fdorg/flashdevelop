using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;

namespace ProjectManager.Projects
{
    public interface IAddPaths
    {
        void Add(string path);
    }

    public class PathCollection : List<string>, IAddPaths
    {
        public PathCollection() : base() { }
        public PathCollection(IEnumerable<string> paths) : base(paths) { }

        /// <summary>
        /// Removes any paths equal to or below the given path.
        /// </summary>
        public void RemoveAtOrBelow(string path)
        {
            if (Contains(path))
                Remove(path);
            RemoveBelow(path);
        }

        /// <summary>
        /// Removes any paths below the given path.
        /// </summary>
        public void RemoveBelow(string path)
        {
            for (int i = 0; i < Count; i++)
            {
                string p = this[i];

                if (p.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                    p == path)
                {
                    RemoveAt(i--); // search this index again
                }
            }
        }

        /// <summary>
        /// Returns the closest parent path in this collection to the given path.
        /// </summary>
        public string GetClosestParent(string path)
        {
            string closest = "";
            foreach (string classpath in this)
                if ((path.StartsWith(classpath, StringComparison.Ordinal) || classpath == ".") && classpath.Length > closest.Length)
                    closest = classpath;
            return (closest.Length > 0) ? closest : null;
        }
    }
}
