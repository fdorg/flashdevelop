// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PluginCore.Helpers;

namespace PluginCore.FRService
{
    #region Search Classes And Enums

    [Flags]
    public enum SearchFilter
    {
        None=0,
        InCodeComments=1,
        OutsideCodeComments=2,
        InStringLiterals=4,
        OutsideStringLiterals=8
    }

    public class SearchMatch
    {
        public int Index;
        public int Length;
        public int Line;
        public int Column;
        public int LineStart;
        public int LineEnd;
        public string Value;
        public string LineText;
        public SearchGroup[] Groups;
    }

    public class SearchGroup
    {
        public int Index;
        public int Length;
        public int Line;
        public string Value;

        public SearchGroup(int index, int length, string value)
        {
            Index = index;
            Length = length;
            Value = value;
        }
    }

    #endregion

    public class FRSearch
    {
        #region Static Methods

        /// <summary>
        /// Make the pattern regex-safe
        /// </summary>
        /// <param name="pattern">Text to escape</param>
        /// <returns>Regex-safe text</returns>
        public static string Escape(string pattern) => Regex.Escape(pattern);

        /// <summary>
        /// Replace escaped characters in replacement text
        /// </summary>
        /// <param name="text">Text to unescape</param>
        public static string Unescape(string text)
        {
            var p = text.IndexOf('\\');
            if (p == -1) return text;
            var sb = new StringBuilder(text.Substring(0, p));
            var n = text.Length;
            for (var i = p; i < n; i++)
            {
                if (i < n - 1 && text[i] == '\\')
                {
                    i++;
                    var c = text[i];
                    if (c == 'r') sb.Append('\r');
                    else if (c == 'n') sb.Append('\n');
                    else if (c == 't') sb.Append('\t');
                    else if (c == 'v') sb.Append('\v');
                    else sb.Append(c);
                }
                else sb.Append(text[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Replace regular expressions groups in replacement text
        /// </summary>
        /// <param name="text">Text to expand</param>
        /// <param name="match">Search result (for reinjecting groups)</param>
        public static string ExpandGroups(string text, SearchMatch match)
        {
            if (!text.Contains('$')) return text;
            for (int i = 0; i < match.Groups.Length; i++)
                text = text.Replace("$" + i, match.Groups[i].Value);
            return text;
        }

        /// <summary>
        /// Update the matches indexes to take in account the pattern & replacement lengthes
        /// </summary>
        /// <param name="matches">Search results to update</param>
        /// <param name="fromMatchIndex">First result to update</param>
        /// <param name="found">Text matched</param>
        /// <param name="replacement">Text replacement</param>
        public static void PadIndexes(List<SearchMatch> matches, int fromMatchIndex, string found, string replacement)
        {
            int linesDiff = CountNewLines(replacement) - CountNewLines(found);
            int charsDiff = replacement.Length - found.Length;
            if (charsDiff != 0 || linesDiff != 0)
                for (int i = fromMatchIndex; i < matches.Count; i++)
                {
                    var match = matches[i];
                    match.Index += charsDiff;
                    match.LineStart += charsDiff;
                    match.LineEnd += charsDiff;
                    match.Line += linesDiff;
                }
        }

        static int CountNewLines(string src)
        {
            var result = 0;
            var c2 = ' ';
            foreach (var c1 in src)
            {
                if (c1 == '\r') result++;
                else if (c1 == '\n' && c2 != '\r') result++;
                c2 = c1;
            }
            return result;
        }

        public static void ExtractResultsLineText(List<SearchMatch> results, string src)
        {
            if (results is null) return;
            int len = src.Length;
            foreach (var sm in results)
            {
                if (sm.LineStart > len || sm.LineEnd > len) break;
                sm.LineText = src.Substring(sm.LineStart, sm.LineEnd - sm.LineStart);
            }
        }

        /** TODO : needs something like SearchOptions flags to be functional
        
        /// <summary>
        /// Quick search
        /// </summary>
        /// <param name="pattern">Search pattern</param>
        /// <param name="input">Source text</param>
        /// <returns>First result</returns>
        static public SearchMatch Match(string pattern, string input, SearchOptions options)
        {
            Search search = new Search(pattern);
            // eval options
            return search.Match(input);
        }

        /// <summary>
        /// Quick search
        /// </summary>
        /// <param name="pattern">Search pattern</param>
        /// <param name="input">Source text</param>
        /// <returns>All results</returns>
        static public List<SearchMatch> Matches(string pattern, string input, SearchOptions options)
        {
            Search search = new Search(pattern);
            // eval options
            return search.Matches(input);
        }

        /// <summary>
        /// Quick replace
        /// </summary>
        /// <param name="pattern">Search pattern</param>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <returns></returns>
        static public string Replace(string pattern, string input, string replacement, SearchOptions options)
        {
            Search search = new Search(pattern);
            // eval options
            List<SearchMatch> matches = search.Matches(input);
            return search.ReplaceAll(input, replacement, matches);
        }
        */

        #endregion

        #region Public Replace Methods

        /// <summary>
        /// Replace one search result
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <param name="match">Search result to replace</param>
        /// <returns>Updated text</returns>
        public string Replace(string input, string replacement, SearchMatch match) => ReplaceOneMatch(input, replacement, new List<SearchMatch> {match}, 0);

        /// <summary>
        /// Replace one search result - updates other matches indexes accordingly
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <param name="matches">Search results</param>
        /// <param name="matchIndex">Index of the search result to replace</param>
        /// <returns>Updated text</returns>
        public string Replace(string input, string replacement, List<SearchMatch> matches, int matchIndex) => ReplaceOneMatch(input, replacement, matches, matchIndex);

        /// <summary>
        /// Replace one search result
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <param name="matches">Search results to replace</param>
        /// <returns>Updated text</returns>
        public string ReplaceAll(string input, string replacement, List<SearchMatch> matches) => ReplaceAllMatches(input, replacement, matches);

        #endregion

        #region Internal Replace Methods

        string ReplaceOneMatch(string src, string replacement, List<SearchMatch> matches, int matchIndex)
        {
            if (matches.IsNullOrEmpty()) return src;
            var match = matches[matchIndex];

            // replace text
            if (isEscaped) replacement = Unescape(replacement);
            if (isRegex) replacement = ExpandGroups(replacement, match);
            src = src.Substring(0, match.Index) + replacement + src.Substring(match.Index + match.Length);

            // update next matches
            if (matches.Count > matchIndex + 1) PadIndexes(matches, matchIndex + 1, match.Value, replacement);
            return src;
        }

        string ReplaceAllMatches(string src, string replacement, ICollection<SearchMatch> matches)
        {
            if (matches.IsNullOrEmpty()) return src;
            var sb = new StringBuilder();
            var original = replacement;
            var lastIndex = 0;
            foreach (var match in matches)
            {
                sb.Append(src.Substring(lastIndex, match.Index - lastIndex));
                // replace text
                replacement = original;
                if (isEscaped) replacement = Unescape(replacement);
                if (isRegex) replacement = ExpandGroups(replacement, match);
                sb.Append(replacement);
                lastIndex = match.Index + match.Length;
            }

            sb.Append(src.Substring(lastIndex));
            return sb.ToString();
        }
        #endregion

        #region Public Properties

        public bool IsEscaped
        {
            get => isEscaped;
            set
            {
                isEscaped = value;
                needParsePattern = true;
            }
        }
        public bool IsRegex
        {
            get => isRegex;
            set
            {
                isRegex = value;
                needParsePattern = true;
            }
        }
        public bool NoCase
        {
            get => noCase;
            set
            {
                noCase = value;
                needParsePattern = true;
            }
        }
        public bool WholeWord
        {
            get => wholeWord;
            set
            {
                wholeWord = value;
                needParsePattern = true;
            }
        }
        public bool SingleLine { get; set; }
        public SearchFilter Filter { get; set; }
        public string Pattern
        {
            get => pattern;
            set
            {
                pattern = value;
                needParsePattern = true;
            }
        }

        bool hasRegexLiterals;
        bool isHaxeFile;
        string sourceFile;
        public string SourceFile
        {
            get => sourceFile;
            set
            {
                if (value == sourceFile) return;
                sourceFile = value;
                if (sourceFile is null)
                {
                    hasRegexLiterals = false;
                    isHaxeFile = false;
                    return;
                }

                var ext = Path.GetExtension(SourceFile).ToLowerInvariant();

                isHaxeFile = FileInspector.IsHaxeFile(ext);

                // Haxe, ActionScript, JavaScript and LoomScript support Regex literals
                hasRegexLiterals = isHaxeFile 
                                   || FileInspector.IsActionScript(ext)
                                   || FileInspector.IsMxml(ext)
                                   || FileInspector.IsHtml(ext)
                                   || ext == ".js" || ext == ".ls" || ext == ".ts";
            }
        }

        #endregion

        #region Public Search Methods

        /// <summary>
        /// Create a search engine
        /// </summary>
        /// <param name="pattern">Search pattern</param>
        public FRSearch(string pattern) => this.pattern = pattern;

        /// <summary>
        /// Find a match
        /// </summary>
        /// <param name="input">Source text</param>
        /// <returns>Search result</returns>
        public SearchMatch Match(string input) => Match(input, 0, 1);

        /// <summary>
        /// Find a match - both startIndex & startLine must be defined
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="startIndex">Character offset</param>
        /// <param name="startLine">Line offset</param>
        /// <returns>Search result</returns>
        public SearchMatch Match(string input, int startIndex, int startLine)
        {
            returnAllMatches = false;
            var res = SearchSource(input, startIndex, startLine);
            return res.Count > 0 ? res[0] : null;
        }

        /// <summary>
        /// Find all matches
        /// </summary>
        /// <param name="input">Source text</param>
        /// <returns>Search results</returns>
        public List<SearchMatch> Matches(string input) => Matches(input, 0, 1);

        /// <summary>
        /// Find all matches - both startIndex & startLine must be defined
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="startIndex">Character offset</param>
        /// <param name="startLine">Line offset</param>
        /// <returns>Search results</returns>
        public List<SearchMatch> Matches(string input, int startIndex, int startLine)
        {
            returnAllMatches = true;
            return SearchSource(input, startIndex, startLine);
        }
        #endregion

        #region Internal Parser Configuration

        bool needParsePattern = true;
        Regex operation;
        string pattern;
        bool noCase;
        bool wholeWord;
        bool isRegex;
        bool isEscaped;
        bool returnAllMatches;

        #endregion

        #region Internal Search Methods

        void BuildRegex(string pattern)
        {
            if (isEscaped) pattern = Unescape(pattern);
            if (!isRegex) pattern = Regex.Escape(pattern);
            if (wholeWord)
            {
                if (pattern.StartsWithOrdinal("\\$")) pattern += "\\b";
                else pattern = "\\b" + pattern + "\\b";
            }
            
            var options = RegexOptions.None;
            if (!SingleLine) options |= RegexOptions.Multiline;
            if (noCase) options |= RegexOptions.IgnoreCase;

            try
            {
                operation = new Regex(pattern, options);
            }
            catch
            {
                operation = null;
            }
        }

        List<SearchMatch> SearchSource(string src, int startIndex, int startLine)
        {
            var results = new List<SearchMatch>();

            // raw search results
            if (needParsePattern) BuildRegex(pattern);
            if (operation is null) return results;
            var matches = operation.Matches(src, startIndex);
            if (matches.Count == 0) return results;

            // index sources
            int len = src.Length;
            int pos = startIndex - 1;
            int line = startLine;
            List<int> lineStart = new List<int>();
            for (int i = 0; i < startLine; i++) lineStart.Add(startIndex);
            char c = ' ';

            int matchIndex = 0;
            int matchCount = matches.Count;
            Match match = matches[0];
            int nextPos = match.Index;

            // filters
            bool inComments = (Filter & SearchFilter.InCodeComments) > 0;
            bool outComments = (Filter & SearchFilter.OutsideCodeComments) > 0;
            bool filterComments = inComments || outComments;
            int commentMatch = 0;
            bool inLiterals = (Filter & SearchFilter.InStringLiterals) > 0;
            bool outLiterals = (Filter & SearchFilter.OutsideStringLiterals) > 0;
            bool filterLiterals = inLiterals || outLiterals;
            int literalMatch = 0;

            while (pos < len - 1)
            {
                c = src[++pos];

                // counting lines
                var hadNL = false;
                if (c == '\n')
                {
                    line++;
                    hadNL = true;
                }
                else if (c == '\r')
                {
                    if (pos < len - 1 && src[pos + 1] != '\n')
                    {
                        line++;
                        hadNL = true;
                    }
                }
                if (hadNL)
                {
                    lineStart.Add(pos + 1);
                }

                // filters
                if (filterComments || filterLiterals)
                {
                    if (literalMatch == 0) // discover comments if not in a literal
                    {
                        if (commentMatch == 0)
                        {
                            if (c == '/' && pos < len - 1)
                                if (src[pos + 1] == '*') commentMatch = 1;
                                else if (src[pos + 1] == '/') commentMatch = 2;
                                else LookupRegex(src, ref pos);
                        }
                        else if (commentMatch == 1)
                        {
                            if (c == '*' && pos < len - 1 && src[pos + 1] == '/') commentMatch = 0;
                        }
                        else if (commentMatch == 2)
                        {
                            if (hadNL) commentMatch = 0;
                        }
                        if ((inComments && commentMatch == 0) || (outComments && commentMatch > 0))
                            continue;
                    }

                    if (commentMatch == 0) // discover literals if not in a comment
                    {
                        if (literalMatch == 0)
                        {
                            if (c == '"') literalMatch = 1;
                            else if (c == '\'') literalMatch = 2;
                        }
                        else if (pos > 1)
                            if (literalMatch == 1)
                            {
                                if (c == '"' && !IsEscapedCharacter(src, pos)) literalMatch = 0;
                            }
                            else
                            {
                                if (c == '\'' && !IsEscapedCharacter(src, pos)) literalMatch = 0;
                            }
                        if ((inLiterals && literalMatch == 0) || (outLiterals && literalMatch > 0))
                            continue;
                    }
                }

                // wait for next match
                while (pos > nextPos)
                {
                    if (++matchIndex == matchCount)
                    {
                        match = null;
                        break;
                    }
                    match = matches[matchIndex];
                    nextPos = match.Index;
                }
                if (match is null) break;
                if (pos < nextPos) continue;

                // store match data
                var sm = new SearchMatch {Index = match.Index, Value = match.Value, Length = match.Length, Line = line};
                sm.LineStart = sm.LineEnd = lineStart[line - 1];
                sm.Column = match.Index - sm.LineStart;

                int gcount = match.Groups.Count;
                sm.Groups = new SearchGroup[gcount];
                for (int i = 0; i < gcount; i++)
                {
                    Group group = match.Groups[i];
                    sm.Groups[i] = new SearchGroup(group.Index, group.Length, group.Value);
                }
                results.Add(sm);
                if (!returnAllMatches) break;
            }
            // last line end
            while (pos < len && c != '\r' && c != '\n') 
                c = src[pos++];
            if (c == '\r' || c == '\n') pos--;
            lineStart.Add(pos);

            // extract line texts
            int maxLine = lineStart.Count;
            foreach(SearchMatch sm in results)
            {
                int endIndex = sm.Index + sm.Length;
                int endLine = sm.Line + 1;
                while (endLine < maxLine && lineStart[endLine - 1] < endIndex)
                    endLine++;
                sm.LineEnd = lineStart[endLine - 1];
            }
            return results;
        }

        void LookupRegex(string ba, ref int i)
        {
            if (!hasRegexLiterals) return;

            int len = ba.Length;
            int i0;
            char c;
            // regex in valid context

            if (!isHaxeFile) i0 = i - 2;
            else
            {
                if (ba[i - 2] != '~') return;
                i0 = i - 3;
            }

            while (i0 > 0)
            {
                c = ba[i0--];
                if ("=(,[{;:".Contains(c)) break; // ok
                if (c == ' ' || c == '\t') continue;
                return;
            }
            i0 = i;
            while (i0 < len)
            {
                c = ba[i0++];
                if (c == '\\') { i0++; continue; } // escape next
                if (c == '/') break; // end of regex
                if (c == '\r' || c== '\n') return;
            }
            while (i0 < len)
            {
                c = ba[i0++];
                if (!char.IsLetter(c))
                {
                    i0--;
                    break;
                }
            }
            i = i0; // ok, skip this regex
        }

        #endregion

        static bool IsEscapedCharacter(string src, int position, char escapeChar = '\\')
        {
            var result = false;
            for (var i = position - 1; i >= 0; i--)
            {
                if (src[i] != escapeChar) break;
                result = !result;
            }
            return result;
        }
    }
}