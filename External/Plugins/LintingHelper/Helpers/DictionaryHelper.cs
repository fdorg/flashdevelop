using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LintingHelper.Helpers
{
    class DictionaryHelper
    {
        /// <summary>
        /// Helper method for a dictionary containing lists,
        /// Returns either the list of the given <paramref name="key"/> or a new list, if the key does not exist.
        /// </summary>
        public static List<T> GetOrCreate<S, T>(Dictionary<S, List<T>> dict, S key)
        {
            List<T> list;
            if (dict.ContainsKey(key))
            {
                list = dict[key];
            }
            else
            {
                list = new List<T>();
                dict[key] = list;
            }

            return list;
        }
    }
}
