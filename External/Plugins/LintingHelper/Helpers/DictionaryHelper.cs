using System.Collections.Generic;

namespace LintingHelper.Helpers
{
    static class DictionaryHelper
    {
        /// <summary>
        /// Helper method for a dictionary containing lists,
        /// Returns either the list of the given <paramref name="key"/> or a new list, if the key does not exist.
        /// </summary>
        public static List<T> GetOrCreate<S, T>(this Dictionary<S, List<T>> dict, S key)
        {
            List<T> list;
            if (!dict.TryGetValue(key, out list))
            {
                list = new List<T>();
                dict[key] = list;
            }

            return list;
        }
    }
}
