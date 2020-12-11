using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using ScintillaNet.Lexers;

namespace CssCompletion
{
    public class Completion
    {
        readonly Regex reNavPrefix = new Regex("\\-[a-z]+\\-(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        readonly Settings settings;
        readonly Language lang;
        string wordChars;
        List<ICompletionListItem> htmlTags;
        List<ICompletionListItem> properties;
        List<ICompletionListItem> pseudos;
        List<ICompletionListItem> prefixes;
        List<ICompletionListItem> blockLevel;
        Dictionary<string, string[]> values;
        string[] tags;
        bool enabled;
        CssFeatures features;
        int lastColonInsert;

        public Completion(SimpleIni config, Settings settings)
        {
            this.settings = settings;
            lang = ScintillaControl.Configuration.GetLanguage("css");
            InitProperties(GetSection(config, "Properties"));
            InitLists(GetSection(config, "Lists"));
        }

        internal void OnFileChanged(CssFeatures features)
        {
            if (features == this.features) return;
            this.features = features;
            enabled = features != null;
            if (enabled)
            {
                wordChars = lang.characterclass.Characters;
                if (features.Mode != "CSS") wordChars += features.Trigger;
                InitBlockLevel();
            }
        }

        internal void OnCharAdded(ScintillaControl sci, int position, int value)
        {
            if (!enabled) return;

            bool autoInsert = false;

            char c = (char)value;
            if (!wordChars.Contains(c))
            {
                if (c == ':')
                {
                    if (lastColonInsert == position - 1)
                    {
                        sci.DeleteBack();
                        lastColonInsert = -1;
                        return;
                    }
                }
                else if (c == ';')
                {
                    char c2 = (char)sci.CharAt(position);
                    if (c2 == ';')
                    {
                        sci.DeleteBack();
                        sci.SetSel(position, position);
                        return;
                    }
                }
                else if (c == '\n' && !settings.DisableAutoCloseBraces)
                {
                    int line = sci.LineFromPosition(position);
                    string text = sci.GetLine(line - 1).TrimEnd();
                    if (text.EndsWith('{')) AutoCloseBrace(sci, line);
                }
                else if (c == '\t') // TODO get tab notification!
                {
                    position--;
                    autoInsert = true;
                }
                else return;
            }

            var context = GetContext(sci, position);
            var mode = CompleteMode.None;

            if (context.InComments) return;
            if (context.InBlock)
            {
                if (context.Word == "-") mode = CompleteMode.Prefix;
                else if (context.Word.Length >= 2 || (char)value == '-')
                    mode = CompleteMode.Attribute;
            }
            else if (context.InValue)
            {
                if (features.Mode != "CSS" && c == features.Trigger)
                {
                    context.Word = context.Word.Substring(1);
                    context.Position++;
                    mode = CompleteMode.Variable;
                }
                else if (context.Word.Length == 1 && "abcdefghijklmnopqrstuvwxyz".Contains(context.Word[0]))
                    mode = CompleteMode.Value;
            }
            else if (c == ':' && !context.IsVar) mode = CompleteMode.Pseudo;

            HandleCompletion(mode, context, autoInsert, true);
        }

        internal void OnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
        }

        internal void OnComplete(ScintillaControl sci, int position)
        {
            if (!enabled) return;
            var context = GetContext(sci, position);
            var mode = CompleteMode.Selector;

            if (context.Word.Length > 0 && context.Word[0] == features.Trigger)
            {
                context.Word = context.Word.Substring(1);
                context.Position++;
                mode = CompleteMode.Variable;
            } 
            else if (context.InBlock) mode = CompleteMode.Attribute;
            else if (context.InValue)
            {
                if (context.Word.Length > 0 && context.Word[0] == features.Trigger)
                {
                    context.Word = context.Word.Substring(1);
                    context.Position++;
                    mode = CompleteMode.Variable;
                }
                else
                    mode = CompleteMode.Value;
            }
            else if (context.Separator == ':') mode = CompleteMode.Pseudo;

            HandleCompletion(mode, context, false, false);
        }

        void HandleCompletion(CompleteMode mode, LocalContext context, bool autoInsert, bool autoHide)
        {
            var items = mode switch
            {
                CompleteMode.Selector => HandleSelectorCompletion(context),
                CompleteMode.Pseudo => HandlePseudoCompletion(context),
                CompleteMode.Prefix => HandlePrefixCompletion(context),
                CompleteMode.Attribute => HandlePropertyCompletion(context),
                CompleteMode.Variable => HandleVariableCompletion(context),
                CompleteMode.Value => HandleValueCompletion(context),
                _ => null,
            };
            if (items is null) return;

            if (autoInsert && !string.IsNullOrEmpty(context.Word))
            {
                var matches = items.Where(item => item.Label.StartsWithOrdinal(context.Word)).ToList();
                if (matches.Count == 1)
                {
                    var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    sci.SetSel(context.Position, sci.CurrentPos);
                    sci.ReplaceSel(matches[0].Label);
                }
            }
            else CompletionList.Show(items, autoHide, context.Word);
        }

        internal void OnInsert(ScintillaControl sci, int position, string text, char trigger, ICompletionListItem item)
        {
            if (!(item is CompletionItem)) return;
            var it = (CompletionItem) item;
            if (trigger == ':')
            {
                lastColonInsert = position + text.Length + 1;
            }
            else if (it.Kind == ItemKind.Property && !settings.DisableInsertColon)
            {
                int pos = position + text.Length;
                char c = (char)sci.CharAt(pos);
                if (c != ':') sci.InsertText(pos, ":");
                sci.SetSel(pos + 1, pos + 1);
                lastColonInsert = pos + 1;
            }
            else lastColonInsert = -1;
        }

        #region parsing

        LocalContext GetContext(ScintillaControl sci, int position)
        {
            var ctx = new LocalContext(sci);
            int i = position - 1;
            int style = sci.StyleAt(i-1);

            if (style == (int)CSS.COMMENT) // inside comments
            {
                ctx.InComments = true;
                return ctx;
            }

            int inString = 0;
            if (style == 14) inString = 1;
            if (style == 13) inString = 2;

            bool inWord = true;
            bool inComment = false;
            bool inPar = false;
            string word = "";
            int lastCharPos = i;

            while (i > 1)
            {
                char c = (char)sci.CharAt(i--);

                if (wordChars.Contains(c))
                {
                    lastCharPos = i + 1;
                    if (inWord) word = c + word;
                }
                else inWord = false;

                if (inString > 0)
                {
                    if (inString == 1 && c == '\'') inString = 0;
                    else if (inString == 2 && c == '"') inString = 0;
                    continue;
                }
                if (inComment)
                {
                    if (c == '*' && i > 0 && (char)sci.CharAt(i) == '/') inComment = false;
                    continue;
                }
                if (c == '/' && i > 0 && (char)sci.CharAt(i) == '*') // entering comment
                    inComment = true;
                if (c == '\'') inString = 1; // entering line
                else if (c == '"') inString = 2;

                else if (c == ')') inPar = true;
                else if (inPar)
                {
                    if (c == '(') inPar = false;
                    continue;
                }

                else if (c == ':')
                {
                    ctx.Separator = c;
                    ctx.Position = lastCharPos;
                    string attr = ReadAttribute(sci, i);
                    if (attr.Length > 1)
                    {
                        if (attr[0] == features.Trigger || IsVarDecl(sci, i))
                            ctx.IsVar = true;
                        else if (!IsTag(attr))
                        {
                            ctx.InValue = true;
                            ctx.Property = attr;
                        }
                    }
                    break;
                }
                else if (c == ';' || c == '{')
                {
                    ctx.Separator = c;
                    ctx.Position = lastCharPos;
                    ctx.InBlock = !IsVarDecl(sci, i);
                    break;
                }
                else if (c == '}' || c == ',' || c == '.' || c == '#')
                {
                    ctx.Separator = c;
                    ctx.Position = lastCharPos;
                    break;
                }
                else if (c == '(')
                {
                    string tok = ReadWordLeft(sci, i);
                    if (tok == "url")
                    {
                        ctx.Separator = '(';
                        ctx.InUrl = true;
                        ctx.Position = i + 1;
                        word = "";
                        for (int j = i + 2; j < position; j++)
                            word += (char)sci.CharAt(j);
                        break;
                    }
                }
            }
            if (word.Length > 0)
            {
                if (word[0] == '-')
                {
                    Match m = reNavPrefix.Match(word);
                    if (m.Success) word = m.Groups[1].Value;
                }
            }
            ctx.Word = word;
            return ctx;
        }

        bool IsVarDecl(ScintillaControl sci, int i)
        {
            if (features.Pattern is null) return false;
            int line = sci.LineFromPosition(i);
            string text = sci.GetLine(line);
            return features.Pattern.IsMatch(text);
        }

        bool IsTag(string word) => tags.Contains(word);

        string ReadWordLeft(ScintillaControl sci, int i)
        {
            bool inWord = false;
            string word = "";

            while (i > 1)
            {
                char c = (char)sci.CharAt(i--);

                if (wordChars.Contains(c))
                {
                    inWord = true;
                    word = c + word;
                }
                else if (inWord) break;
            }
            return word;
        }

        string ReadAttribute(ScintillaControl sci, int i)
        {
            bool inWord = false;
            string word = "";

            while (i > 1)
            {
                char c = (char)sci.CharAt(i--);

                if (wordChars.Contains(c))
                {
                    inWord = true;
                    word = c + word;
                }
                else if (c > 32) return "";
                else if (inWord) break;
            }
            if (word.Length > 0 && word[0] == '-')
            {
                Match m = reNavPrefix.Match(word);
                if (m.Success) word = m.Groups[1].Value;
            }
            return word;
        }

        CssBlock FindBlock(ScintillaControl sci, bool parseIfDirty, int line, int col)
        {
            List<CssBlock> blocks = ParseBlocks(sci);
            return LookupBlock(blocks, null, line, col);
        }

        CssBlock LookupBlock(List<CssBlock> blocks, CssBlock parent, int line, int col)
        {
            foreach (CssBlock block in blocks)
            {
                if (CursorInBlock(block, line, col))
                    return LookupBlock(block.Children, block, line, col);
            }
            return parent;
        }

        bool CursorInBlock(CssBlock block, int line, int col)
        {
            if (line < block.LineFrom || line > block.LineTo) return false;
            if (line == block.LineFrom && col <= block.ColFrom) return false;
            if (line == block.LineTo && col > block.ColTo) return false;
            return true;
        }

        List<CssBlock> ParseBlocks(ScintillaControl sci)
        {
            List<CssBlock> blocks = new List<CssBlock>();
            blocks.Clear();
            int lines = sci.LineCount;
            int inString = 0;
            bool inComment = false;
            CssBlock block = null;
            for (int i = 0; i < lines; i++)
            {
                string line = sci.GetLine(i);
                int len = line.Length;
                int safeLen = len - 1;
                for (int j = 0; j < len; j++)
                {
                    char c = line[j];
                    if (inComment)
                    {
                        if (c == '*' && j < safeLen && line[j + 1] == '/') inComment = false;
                        else continue;
                    }
                    else if (inString > 0)
                    {
                        if (inString == 1 && c == '\'') inString = 0;
                        else if (inString == 2 && c == '"') inString = 0;
                        else continue;
                    }
                    else if (c == '\'') inString = 1;
                    else if (c == '"') inString = 2;
                    else if (c == '/' && j < safeLen && line[j + 1] == '/')
                        break;
                    else if (c == '/' && j < safeLen && line[j + 1] == '*')
                        inComment = true;
                    else if (c == '{')
                    {
                        CssBlock parent = block;
                        block = new CssBlock();
                        block.LineFrom = i;
                        block.ColFrom = j;
                        if (parent != null)
                        {
                            block.Parent = parent;
                            parent.Children.Add(block);
                        }
                        else blocks.Add(block);
                    }
                    else if (c == '}')
                    {
                        if (block != null)
                        {
                            block.LineTo = i;
                            block.ColTo = j;
                            block = block.Parent;
                            if (block != null)
                            {
                                block.LineTo = i;
                                block.ColTo = j;
                            }
                        }
                    }
                }
            }
            return blocks;
        }

        #endregion

        #region completion

        List<ICompletionListItem> HandlePrefixCompletion(LocalContext context) => prefixes;

        List<ICompletionListItem> HandlePseudoCompletion(LocalContext context) => pseudos;

        List<ICompletionListItem> HandleValueCompletion(LocalContext context)
        {
            var items = new List<ICompletionListItem>();
            AddProperties(items, context.Property);
            if (items.Count > 0)
                items.Add(new CompletionItem("inherit", ItemKind.Value));
            return items;
        }

        List<ICompletionListItem> HandleVariableCompletion(LocalContext context)
        {
            string src = context.Sci.Text;
            if (context.Sci.CurrentPos < src.Length)
                src = src.Substring(0, context.Sci.CurrentPos);
            var matches = features.Pattern.Matches(src);
            if (matches.Count == 0) 
                return null;

            var tokens = new Dictionary<string, Match>();
            foreach (Match m in matches)
                tokens[m.Groups[1].Value] = m;

            var items = new List<ICompletionListItem>();
            foreach (string token in tokens.Keys)
            {
                string desc = GetVariableValue(src, tokens[token]);
                items.Add(new CompletionItem(token, ItemKind.Variable, desc));
            }
            items.Sort();

            return items;
        }

        static string GetVariableValue(string src, Capture m)
        {
            // extract value
            int i = m.Index + m.Length;
            int len = src.Length;
            string value = "";
            while (i < len)
            {
                char c = src[i++];
                if (c == 13 || c == 10 || c == ';' || c == ')' || c == '(') break;
                value += c;
            }

            // extract comment before declaration
            i = m.Index - 1;
            string prevLine = "";
            bool isPrev = false;
            while (i > 0)
            {
                char c = src[i--];
                if (!isPrev)
                {
                    if (c == 13 || c == 10) { isPrev = true; }
                    else if (c > 32) break;
                }
                else
                {
                    if (c == 13 || c == 10)
                    {
                        prevLine = prevLine.Trim();
                        if (prevLine.StartsWithOrdinal("//")) break;
                        if (!prevLine.EndsWithOrdinal("*/") || prevLine.Contains("/*")) break;
                    }
                    prevLine = c + prevLine;
                }
            }
            if (prevLine.Length > 0 && prevLine[0] == '/')
            {
                if (prevLine.StartsWithOrdinal("//")) value += "//" + prevLine.Substring(2);
                else if (prevLine.StartsWithOrdinal("/*")) value += "//" + prevLine.Substring(2, prevLine.Length - 4);
            }

            return value.Trim();
        }

        List<ICompletionListItem> HandlePropertyCompletion(LocalContext context) => blockLevel;

        List<ICompletionListItem> HandleSelectorCompletion(LocalContext context) => htmlTags;

        void AddProperties(ICollection<ICompletionListItem> items, string name)
        {
            if (!values.ContainsKey(name)) return;
            var vals = values[name];
            foreach (string val in vals)
            {
                if (val[0] == '<')
                {
                    string inherit = val.Substring(1, val.Length - 2);
                    if (inherit != name)
                        AddProperties(items, inherit);
                }
                else items.Add(new CompletionItem(val, ItemKind.Value));
            }
        }

        void InitBlockLevel()
        {
            if (features.Mode == "CSS") blockLevel = properties;
            else
            {
                blockLevel = new List<ICompletionListItem>(properties);
                blockLevel.AddRange(htmlTags);
                blockLevel.Sort();
            }
        }

        void InitProperties(Dictionary<string, string> section)
        {
            properties = new List<ICompletionListItem>();
            values = new Dictionary<string, string[]>();
            foreach (var prop in section)
            {
                string[] names = prop.Key.Split(',');
                string[] vals = prop.Value.Split(' ');
                foreach (string name in names)
                {
                    properties.Add(new CompletionItem(name, ItemKind.Property));
                    values.Add(name, vals);
                }
            }
        }

        void InitLists(IDictionary<string, string> section)
        {
            tags = Regex.Split(section["tags"], "\\s+");
            htmlTags = new List<ICompletionListItem>();
            foreach (string tag in tags)
                htmlTags.Add(new CompletionItem(tag, ItemKind.Tag));
            htmlTags.Sort();

            pseudos = MakeList(section["pseudo"], ItemKind.Pseudo);
            prefixes = MakeList(section["prefixes"], ItemKind.Prefixes);
        }

        static List<ICompletionListItem> MakeList(string raw, ItemKind kind)
        {
            return Regex.Split(raw, "\\s+")
                .Select(def => new CompletionItem(def, kind))
                .ToList<ICompletionListItem>();
        }

        static Dictionary<string, string> GetSection(SimpleIni config, string name)
        {
            foreach (var def in config)
                if (def.Key == name) return def.Value;
            return null;
        }

        /// <summary>
        /// Add closing brace to a code block.
        /// If enabled, move the starting brace to a new line.
        /// </summary>
        /// <param name="sci"></param>
        /// <param name="line"></param>
        public static void AutoCloseBrace(ScintillaControl sci, int line)
        {
            // find matching brace
            int bracePos = sci.LineEndPosition(line - 1) - 1;
            while ((bracePos > 0) && (sci.CharAt(bracePos) != '{')) bracePos--;
            if (bracePos == 0 || sci.BaseStyleAt(bracePos) != 5) return;
            int match = sci.SafeBraceMatch(bracePos);
            int start = line;
            int indent = sci.GetLineIndentation(start - 1);
            if (match > 0)
            {
                int endIndent = sci.GetLineIndentation(sci.LineFromPosition(match));
                if (endIndent + sci.TabWidth > indent)
                    return;
            }

            // find where to include the closing brace
            int startIndent = indent;
            int count = sci.LineCount;
            int lastLine = line;
            int position;
            string txt = sci.GetLine(line).Trim();
            line++;
            int eolMode = sci.EOLMode;
            string NL = LineEndDetector.GetNewLineMarker(eolMode);

            if (txt.Length > 0 && ")]};,".Contains(txt[0]))
            {
                sci.BeginUndoAction();
                try
                {
                    position = sci.CurrentPos;
                    sci.InsertText(position, NL + "}");
                    sci.SetLineIndentation(line, startIndent);
                }
                finally
                {
                    sci.EndUndoAction();
                }
                return;
            }

            while (line < count - 1)
            {
                txt = sci.GetLine(line).TrimEnd();
                if (txt.Length != 0)
                {
                    indent = sci.GetLineIndentation(line);
                    if (indent <= startIndent) break;
                    lastLine = line;
                }
                else break;
                line++;
            }
            if (line >= count - 1) lastLine = start;

            // insert closing brace
            sci.BeginUndoAction();
            try
            {
                position = sci.LineEndPosition(lastLine);
                sci.InsertText(position, NL + "}");
                sci.SetLineIndentation(lastLine + 1, startIndent);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        #endregion
    }
}