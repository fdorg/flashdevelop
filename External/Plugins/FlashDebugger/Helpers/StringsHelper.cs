using System;

namespace FlashDebugger
{
    public static class Strings
    {

        /// <summary>
        /// Get the substring before the specified search term (find). 
        /// </summary>
        /// <param name="text">Main string</param>
        /// <param name="find">Search term</param>
        /// <param name="startAt">Starts searching for term at specified char indexed.</param>
        /// <param name="returnAll">If search term not found, return entire string? (true) or return "" (false)</param>
        /// <param name="forward">Search forward from startAt, or backward from it</param>
        /// <returns>Substring before the specified search term</returns>
        public static string Before(this string text, string find, int startAt = 0, bool returnAll = false, bool forward = true)
        {
            if (text == null) return returnAll ? null : "";
            int idx;
            if (forward)
            {
                idx = text.IndexOf(find, startAt, StringComparison.Ordinal);
            }
            else
            {
                idx = text.LastIndexOf(find, text.Length - startAt, StringComparison.Ordinal);
            }
            if (idx == -1) return returnAll ? text : "";
            return text.Substring(0, idx);
        }

        /// <summary>
        /// Get the substring after the specified search term (find). 
        /// </summary>
        /// <param name="text">Main string</param>
        /// <param name="find">Search term</param>
        /// <param name="startAt">Starts searching for term at specified char indexed.</param>
        /// <param name="returnAll">If search term not found, return entire string? (true) or return "" (false)</param>
        /// <param name="forward">Search forward from startAt, or backward from it</param>
        /// <returns>Substring after the specified search term</returns>
        public static string After(this string text, string find, int startAt = 0, bool returnAll = false, bool forward = true)
        {
            if (text == null) return returnAll ? null : "";
            int idx;
            if (!forward)
            {
                idx = text.LastIndexOf(find, text.Length - startAt, StringComparison.Ordinal);
            }
            else
            {
                idx = text.IndexOf(find, startAt, StringComparison.Ordinal);
            }
            if (idx == -1) return returnAll ? text : "";
            idx += find.Length;
            return text.Substring(idx);
        }

        /// <summary>
        /// Get the substring after the last instance of the search term (find). 
        /// </summary>
        /// <param name="text">Main string</param>
        /// <param name="find">Search term</param>
        /// <param name="returnAll">If search term not found, return entire string? (true) or return "" (false)</param>
        /// <returns>Substring after the last instance of search term</returns>
        public static string AfterLast(this string text, string find, bool returnAll = false)
        {
            int idx = text.LastIndexOf(find, StringComparison.Ordinal);
            if (idx == -1)
            {
                return returnAll ? text : "";
            }
            idx += find.Length;
            return text.Substring(idx);
        }

    }
}