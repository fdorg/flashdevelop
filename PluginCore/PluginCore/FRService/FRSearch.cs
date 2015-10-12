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
        static public string Escape(string pattern)
        {
            return Regex.Escape(pattern);
        }

        /// <summary>
        /// Replace escaped characters in replacement text
        /// </summary>
        /// <param name="text">Text to unescape</param>
        static public string Unescape(string text)
        {
            int p = text.IndexOf('\\');
            if (p < 0) return text;

            string result = text.Substring(0, p);
            int n = text.Length;
            for (int i = p; i < n; i++)
            {
                if (i < n - 1 && text[i] == '\\')
                {
                    i++;
                    char c = text[i];
                    if (c == 'r') result += '\r';
                    else if (c == 'n') result += '\n';
                    else if (c == 't') result += '\t';
                    else if (c == 'v') result += '\v';
                    else result += c;
                }
                else result += text[i];
            }
            return result;
        }

        /// <summary>
        /// Replace regular expressions groups in replacement text
        /// </summary>
        /// <param name="escapedText">Text to expand</param>
        /// <param name="match">Search result (for reinjecting groups)</param>
        static public string ExpandGroups(string text, SearchMatch match)
        {
            if (text.IndexOf('$') < 0) return text;
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
        static public void PadIndexes(List<SearchMatch> matches, int fromMatchIndex, string found, string replacement)
        {
            int linesDiff = CountNewLines(replacement) - CountNewLines(found);
            int charsDiff = replacement.Length - found.Length;
            SearchMatch match;
            if (charsDiff != 0 || linesDiff != 0)
                for (int i = fromMatchIndex; i < matches.Count; i++)
                {
                    match = matches[i];
                    match.Index += charsDiff;
                    match.LineStart += charsDiff;
                    match.LineEnd += charsDiff;
                    match.Line += linesDiff;
                }
        }

        static private int CountNewLines(string src)
        {
            int lines = 0;
            char c1;
            char c2 = ' ';
            for (int i = 0; i < src.Length; i++)
            {
                c1 = src[i];
                if (c1 == '\r') lines++;
                else if (c1 == '\n' && c2 != '\r') lines++;
                c2 = c1;
            }
            return lines;
        }

        static public void ExtractResultsLineText(List<SearchMatch> results, string src)
        {
            if (results == null) 
                return;
            int len = src.Length;
            foreach (SearchMatch sm in results)
            {
                if (sm.LineStart > len || sm.LineEnd > len) 
                    break;
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
        public string Replace(string input, string replacement, SearchMatch match)
        {
            List<SearchMatch> matches = new List<SearchMatch>();
            matches.Add(match);
            return ReplaceOneMatch(input, replacement, matches, 0);
        }

        /// <summary>
        /// Replace one search result - updates other matches indexes accordingly
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <param name="matches">Search results</param>
        /// <param name="matchIndex">Index of the search result to replace</param>
        /// <returns>Updated text</returns>
        public string Replace(string input, string replacement, List<SearchMatch> matches, int matchIndex)
        {
            return ReplaceOneMatch(input, replacement, matches, matchIndex);
        }

        /// <summary>
        /// Replace one search result
        /// </summary>
        /// <param name="input">Source text</param>
        /// <param name="replacement">Replacement pattern</param>
        /// <param name="matches">Search results to replace</param>
        /// <returns>Updated text</returns>
        public string ReplaceAll(string input, string replacement, List<SearchMatch> matches)
        {
            return ReplaceAllMatches(input, replacement, matches);
        }

        #endregion

        #region Internal Replace Methods

        string ReplaceOneMatch(string src, string replacement, List<SearchMatch> matches, int matchIndex)
        {
            if (matches == null || matches.Count == 0) return src;
            SearchMatch match = matches[matchIndex];

            // replace text
            if (isEscaped) replacement = Unescape(replacement);
            if (isRegex) replacement = ExpandGroups(replacement, match);
            src = src.Substring(0, match.Index) + replacement + src.Substring(match.Index + match.Length);

            // update next matches
            if (matches.Count > matchIndex + 1) PadIndexes(matches, matchIndex + 1, match.Value, replacement);
            return src;
        }

        string ReplaceAllMatches(string src, string replacement, List<SearchMatch> matches)
        {
            if (matches == null || matches.Count == 0) return src;
            StringBuilder sb = new StringBuilder();
            string original = replacement;
            int lastIndex = 0;

            foreach (SearchMatch match in matches)
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
            get { return isEscaped; }
            set
            {
                isEscaped = value;
                needParsePattern = true;
            }
        }
        public bool IsRegex
        {
            get { return isRegex; }
            set
            {
                isRegex = value;
                needParsePattern = true;
            }
        }
        public bool NoCase
        {
            get { return noCase; }
            set
            {
                noCase = value;
                needParsePattern = true;
            }
        }
        public bool WholeWord
        {
            get { return wholeWord; }
            set
            {
                wholeWord = value;
                needParsePattern = true;
            }
        }
        public bool SingleLine
        {
            get { return singleLine; }
            set
            {
                singleLine = value;
            }
        }
        public SearchFilter Filter
        {
            get { return filter; }
            set
            {
                filter = value;
            }
        }
        public string Pattern
        {
            get { return pattern; }
            set
            {
                pattern = value;
                needParsePattern = true;
            }
        }
        public bool CopyFullLineContext
        {
            get { return copyFullLineContext; }
            set
            {
                copyFullLineContext = value;
            }
        }

        private bool hasRegexLiterals;
        private bool isHaxeFile;
        private string sourceFile;
        public string SourceFile
        {
            get { return sourceFile; } 
            set
            {
                if (sourceFile == value) return;
                
                sourceFile = value;

                if (sourceFile == null)
                {
                    hasRegexLiterals = false;
                    isHaxeFile = false;
                    return;
                }

                string ext = Path.GetExtension(SourceFile).ToLowerInvariant();

                isHaxeFile = FileInspector.IsHaxeFile(SourceFile, ext);

                // Haxe, ActionScript, JavaScript and LoomScript support Regex literals
                hasRegexLiterals = isHaxeFile || FileInspector.IsActionScript(SourceFile, ext) ||
                                   FileInspector.IsMxml(SourceFile, ext) ||
                                   FileInspector.IsHtml(SourceFile, ext) || ext == ".js" || ext == ".ls" ||
                                   ext == ".ts";
            }
        }

        #endregion

        #region Public Search Methods

        /// <summary>
        /// Create a seach engine
        /// </summary>
        /// <param name="pattern">Search pattern</param>
        public FRSearch(string pattern)
        {
            this.pattern = pattern;
        }

        /// <summary>
        /// Find a match
        /// </summary>
        /// <param name="input">Source text</param>
        /// <returns>Search result</returns>
        public SearchMatch Match(string input)
        {
            return Match(input, 0, 1);
        }

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
            List<SearchMatch> res = SearchSource(input, startIndex, startLine);
            if (res.Count > 0) return res[0];
            else return null;
        }

        /// <summary>
        /// Find all matches
        /// </summary>
        /// <param name="input">Source text</param>
        /// <returns>Search results</returns>
        public List<SearchMatch> Matches(string input)
        {
            return Matches(input, 0, 1);
        }

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

        private bool needParsePattern = true;
        private Regex operation;
        private string pattern;
        private bool noCase;
        private bool wholeWord;
        private bool isRegex;
        private bool isEscaped;
        private bool singleLine;
        private bool returnAllMatches;
        private bool copyFullLineContext = true;
        private SearchFilter filter;

        #endregion

        #region Internal Search Methods

        private void BuildRegex(string pattern)
        {
            if (isEscaped) pattern = Unescape(pattern);
            if (!isRegex) pattern = Regex.Escape(pattern);
            if (wholeWord)
            {
                if (pattern.StartsWith("\\$")) pattern += "\\b";
                else pattern = "\\b" + pattern + "\\b";
            }
            
            RegexOptions options = RegexOptions.None;
            if (!singleLine) options |= RegexOptions.Multiline;
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

        private List<SearchMatch> SearchSource(string src, int startIndex, int startLine)
        {
            List<SearchMatch> results = new List<SearchMatch>();

            // raw search results
            if (needParsePattern) BuildRegex(pattern);
            if (operation == null)
                return results;
            MatchCollection matches = operation.Matches(src, startIndex);
            if (matches.Count == 0) 
                return results;

            // index sources
            int len = src.Length;
            int pos = startIndex - 1;
            int line = startLine;
            List<int> lineStart = new List<int>();
            for (int i = 0; i < startLine; i++) lineStart.Add(startIndex);
            char c = ' ';
            bool hadNL = false;

            int matchIndex = 0;
            int matchCount = matches.Count;
            Match match = matches[0];
            int nextPos = match.Index;

            // filters
            bool inComments = (filter & SearchFilter.InCodeComments) > 0;
            bool outComments = (filter & SearchFilter.OutsideCodeComments) > 0;
            bool filterComments = inComments || outComments;
            int commentMatch = 0;
            bool inLiterals = (filter & SearchFilter.InStringLiterals) > 0;
            bool outLiterals = (filter & SearchFilter.OutsideStringLiterals) > 0;
            bool filterLiterals = inLiterals || outLiterals;
            int literalMatch = 0;

            while (pos < len - 1)
            {
                c = src[++pos];

                // counting lines
                hadNL = false;
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
                                if (src[pos - 1] != '\\' && c == '"') literalMatch = 0;
                            }
                            else if (literalMatch == 2)
                            {
                                if (src[pos - 1] != '\\' && c == '\'') literalMatch = 0;
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
                if (match == null) 
                    break;
                if (pos < nextPos) 
                    continue;

                // store match data
                SearchMatch sm = new SearchMatch();
                sm.Index = match.Index;
                sm.Value = match.Value;
                sm.Length = match.Length;
                sm.Line = line;
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
                if (!returnAllMatches)
                    break;
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

        private bool LookupRegex(string ba, ref int i)
        {
            if (!hasRegexLiterals)
                return false;

            int len = ba.Length;
            int i0;
            char c;
            // regex in valid context

            if (!isHaxeFile)
                i0 = i - 2;
            else
            {
                if (ba[i - 2] != '~')
                    return false;
                i0 = i - 3;
            }

            while (i0 > 0)
            {
                c = ba[i0--];
                if ("=(,[{;:".IndexOf(c) >= 0) break; // ok
                if (" \t".IndexOf(c) >= 0) continue;
                return false; // anything else isn't expected before a regex
            }
            i0 = i;
            while (i0 < len)
            {
                c = ba[i0++];
                if (c == '\\') { i0++; continue; } // escape next
                if (c == '/') break; // end of regex
                if ("\r\n".IndexOf(c) >= 0) return false;
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
            return true;
        }

        #endregion

    }
}
