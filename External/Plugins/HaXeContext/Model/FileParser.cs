/*
 * 
 * User: Philippe Elsass
 * Date: 18/03/2006
 * Time: 19:03
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;

namespace HaXeContext.Model
{
    #region Token class
    class Token
    {
        public int Position;
        public int Line;
        public string Text;

        public Token()
        {
        }

        public Token(Token copy)
        {
            Text = copy.Text;
            Line = copy.Line;
            Position = copy.Position;
        }

        public override string ToString() => Text;
    }
    #endregion

    /// <summary>
    /// Old & clumsy Haxe file parser - beware!
    /// </summary>
    public class FileParser : IFileParser
    {
        #region parser context
        const int COMMENTS_BUFFER = 4096;
        const int TOKEN_BUFFER = 1024;
        const int VALUE_BUFFER = 1024;

        // parser context
        private FileModel model;
        private bool tryPackage;
        private bool hasPackageSection;
        private FlagType context;
        private FlagType modifiers;
        private FlagType curModifiers;
        //private int modifiersPos;
        private int line;
        private int modifiersLine;
        private bool foundColon;
        private bool foundConstant;
        private bool inParams;
        private bool inEnum;
        private bool inTypedef;
        private bool inAbstract;
        private bool inGeneric;
        private bool inValue;
        private bool hadValue;
        private bool inType;
        private bool inAnonType;
        private int flattenNextBlock;
        private FlagType foundKeyword;
        private Token valueKeyword;
        private MemberModel valueMember;
        private Token curToken;
        private Token prevToken;
        private MemberModel curMember;
        private MemberModel curMethod;
        private Visibility curAccess;
        private string curNamespace;
        private ClassModel curClass;
        private string lastComment;
        private string curComment;
        private bool isBlockComment;
        private ContextFeatures features;
        private List<ASMetaData> carriedMetaData;
        #endregion

        public bool ScriptMode { private get; set; }

        #region tokenizer

        public FileParser() : this(new ContextFeatures())
        {
        }

        public FileParser(ContextFeatures features)
        {
            this.features = features;
        }

        /// <summary>
        /// Rebuild a file model with the source provided
        /// </summary>
        /// <param name="fileModel">Model</param>
        /// <param name="src">Source</param>
        public void ParseSrc(FileModel fileModel, string src) => ParseSrc(fileModel, src, true);

        public void ParseSrc(FileModel fileModel, string ba, bool allowBaReExtract)
        {
            //TraceManager.Add("Parsing " + Path.GetFileName(fileModel.FileName));
            model = fileModel;
            model.OutOfDate = false;
            if (model.Context != null) features = model.Context.Features;
            if (features.hasModules) model.Module = Path.GetFileNameWithoutExtension(model.FileName);

            // pre-filtering
            if (allowBaReExtract && model.HasFiltering && model.Context != null)
                ba = model.Context.FilterSource(fileModel.FileName, ba);

            model.InlinedIn = null;
            model.InlinedRanges = null;

            // language features
            model.Imports.Clear();
            model.Classes.Clear();
            model.Members.Clear();
            model.Namespaces.Clear();
            model.Regions.Clear();
            model.PrivateSectionIndex = 0;
            model.Package = "";
            model.MetaDatas = null;

            // state
            int len = ba.Length;
            int i = 0;
            line = 0;
            int matching = 0;
            int inString = 0;
            int braceCount = 0;
            bool inCode = true;

            // comments
            char[] commentBuffer = new char[COMMENTS_BUFFER];
            int commentLength = 0;
            lastComment = null;
            curComment = null;

            // tokenisation
            tryPackage = true;
            hasPackageSection = false;
            curToken = new Token();
            prevToken = new Token();
            int tokPos = 0;
            int tokLine = 0;
            curMethod = null;
            curMember = null;
            valueKeyword = null;
            valueMember = null;
            curModifiers = 0;
            curNamespace = "internal";
            curAccess = 0;

            char[] buffer = new char[TOKEN_BUFFER];
            int length = 0;
            char[] valueBuffer = new char[VALUE_BUFFER];
            int valueLength = 0;
            int paramBraceCount = 0;
            int paramTempCount = 0;
            int paramParCount = 0;
            int paramSqCount = 0;

            bool hadWS = true;
            bool hadDot = false;
            inParams = false;
            inEnum = false;
            inTypedef = false;
            inAbstract = false;
            inValue = false;
            hadValue = false;
            inType = false;
            inGeneric = false;
            inAnonType = false;
            var inFunction = false;

            bool addChar = false;
            int evalToken = 0;
            //bool evalKeyword = true;
            context = 0;
            modifiers = 0;
            foundColon = false;

            bool handleDirectives = features.hasDirectives;
            bool inlineDirective = false;

            while (i < len)
            {
                var c1 = ba[i++];
                var isInString = (inString > 0);

                /* MATCH COMMENTS / STRING LITERALS */

                char c2;
                switch (matching)
                {
                    // look for comment block/line and preprocessor commands
                    case 0:
                        if (!isInString)
                        {
                            // new comment
                            if (c1 == '/' && i < len)
                            {
                                c2 = ba[i];
                                if (c2 == '/')
                                {
                                    // Check if this this is a /// comment
                                    if (i + 1 < len && ba[i + 1] == '/')
                                    {
                                        // This is a /// comment
                                        matching = 4;
                                        isBlockComment = true;
                                        i++;
                                    }
                                    else
                                    {
                                        // This is a regular comment
                                        matching = 1;
                                        isBlockComment = false;
                                    }
                                    inCode = false;
                                    i++;
                                    continue;
                                }
                                if (c2 == '*')
                                {
                                    isBlockComment = (i + 1 < len && ba[i + 1] == '*');
                                    matching = 2;
                                    inCode = false;
                                    i++;
                                    while (i < len - 1)
                                    {
                                        c2 = ba[i];
                                        if (c2 == '*' && ba[i + 1] != '/') i++;
                                        else break;
                                    }
                                    continue;
                                }
                            }
                            // don't look for comments in strings
                            else if (c1 == '"')
                            {
                                isInString = true;
                                inString = 1;
                            }
                            else if (c1 == '\'')
                            {
                                isInString = true;
                                inString = 2;
                            }
                            // preprocessor statements
                            else if (c1 == '#' && handleDirectives && i < len)
                            {
                                int ls = i - 2;
                                inlineDirective = false;
                                while (ls > 0)
                                {
                                    c2 = ba[ls--];
                                    if (c2 == 10 || c2 == 13) break;
                                    if (c2 > 32) { inlineDirective = true; break; }
                                }
                                c2 = ba[i];
                                if (i < 2 || ba[i - 2] < 33 && c2 >= 'a' && c2 <= 'z')
                                {
                                    matching = 3;
                                    inCode = false;
                                    continue;
                                }
                            }
                        }
                        // end of string?
                        else
                        {
                            if (c1 == 10 || c1 == 13) { }
                            else if ((c1 == '"' && inString == 1) || (c1 == '\'' && inString == 2))
                            {
                                // Are we on an escaped ' or ""?
                                int escNo = 0;
                                int l = i - 2;
                                while (l > -1 && ba[l--] == '\\')
                                    escNo++;

                                // Even number of escaped \ means we are not on an escaped ' or ""
                                if (escNo % 2 == 0) inString = 0;
                            }
                        }
                        break;

                    // skip commented line
                    case 1:
                        if (c1 == 10 || c1 == 13)
                        {
                            // ignore single comments
                            commentLength = 0;
                            inCode = true;
                            matching = 0;
                        }
                        break;

                    // skip commented block
                    case 2:
                        if (c1 == '*')
                        {
                            bool end = false;
                            while (i < len)
                            {
                                c2 = ba[i];
                                if (c2 == '\\') { i++; continue; }
                                if (c2 == '/')
                                {
                                    end = true;
                                    break;
                                }
                                if (c2 == '*') i++;
                                else break;
                            }
                            if (end)
                            {
                                lastComment = (commentLength > 0) ? new string(commentBuffer, 0, commentLength) : null;
                                // TODO  parse for TODO statements?
                                commentLength = 0;
                                inCode = true;
                                matching = 0;
                                i++;
                                continue;
                            }
                        }
                        break;

                    // directive/preprocessor statement
                    case 3:
                        if (c1 == 10 || c1 == 13 || (inlineDirective && c1 <= 32))
                        {
                            inCode = true;
                            commentLength = 0;
                            matching = 0;
                        }
                        else if (c1 == '#') // peek for #end
                        {
                            if (i + 3 < len && ba[i] == 'e' && ba[i + 1] == 'n' && ba[i + 2] == 'd' && ba[i + 3] <= 32)
                            {
                                matching = 0;
                                inCode = true;
                                commentLength = 0;
                                i += 3;
                                continue;
                            }
                        }

                        break;

                    // We are inside a /// comment
                    case 4:
                        {
                            bool end = false;
                            bool skipAhead = false;

                            // See if we just ended a line
                            if (2 <= i && (ba[i - 2] == 10 || ba[i - 2] == 13))
                            {
                                // Check ahead to the next line, see if it has a /// comment on it too.
                                // If it does, we want to continue the comment with that line.  If it
                                // doesn't, then this comment is finished and we will set end to true.
                                for (int j = i + 1; j < len; ++j)
                                {
                                    // Skip whitespace
                                    char twoBack = ba[j - 2];
                                    if (' ' != twoBack && '\t' != twoBack)
                                    {
                                        if ('/' == twoBack && '/' == ba[j - 1] && '/' == ba[j])
                                        {
                                            // There is a comment ahead.  Move up to it so we can gather the
                                            // rest of the comment
                                            i = j + 1;
                                            skipAhead = true;
                                            break;
                                        }
                                        // Not a comment!  We're done!
                                        end = true;
                                        break;
                                    }
                                }
                            }
                            if (end)
                            {
                                // The comment is over and we want to write it out
                                lastComment = (commentLength > 0) ? new string(commentBuffer, 0, commentLength).Trim() : null;
                                commentLength = 0;
                                inCode = true;
                                matching = 0;

                                // Back up i so we can start gathering comments from right after the line break
                                --i;
                                continue;
                            }
                            if (skipAhead)
                            {
                                // We just hit another /// and are skipping up to right after it.
                                continue;
                            }
                            break;
                        }
                }

                /* LINE/COLUMN NUMBER */

                if (c1 == 10 || c1 == 13)
                {
                    line++;
                    if (c1 == 13 && i < len && ba[i] == 10) i++;
                }


                /* SKIP CONTENT */

                if (!inCode)
                {
                    // store comments
                    if (matching == 2 || (matching == 3 && handleDirectives) || matching == 4)
                    {
                        if (commentLength < COMMENTS_BUFFER) commentBuffer[commentLength++] = c1;
                    }
                    else if (matching == 1 && (c1 == '{' || c1 == '}'))
                    {
                        commentBuffer[commentLength++] = c1;
                        while (i < len)
                        {
                            c2 = ba[i];
                            if (commentLength < COMMENTS_BUFFER) commentBuffer[commentLength++] = c2;
                            if (c2 == 10 || c2 == 13)
                                break;
                            i++;
                        }

                        string comment = new String(commentBuffer, 0, commentLength);
                        
                        // region start
                        Match matchStart = ASFileParserRegexes.RegionStart.Match(comment);
                        if (matchStart.Success)
                        {
                            string regionName = matchStart.Groups["name"].Value.Trim();
                            MemberModel region = new MemberModel(regionName, string.Empty, FlagType.Declaration, Visibility.Default);
                            region.LineFrom = line;
                            model.Regions.Add(region);
                        }
                        else
                        {
                            // region end
                            Match matchEnd = ASFileParserRegexes.RegionEnd.Match(comment);
                            if (matchEnd.Success && model.Regions.Count > 0)
                            {
                                MemberModel region = model.Regions[model.Regions.Count - 1];
                                if (region.LineTo == 0)
                                {
                                    region.LineTo = line;
                                }
                            }
                        }
                    }
                    continue;
                }
                if (isInString)
                {
                    // store parameter default value
                    if (inValue && valueLength < VALUE_BUFFER)
                        valueBuffer[valueLength++] = c1;
                    continue;
                }
                if (!inValue)
                {
                    if (inFunction)
                    {
                        var abort = false;
                        if (c1 <= 32)
                        {
                            var start = 0;
                            var end = -1;
                            for (var j = 0; j < valueLength; j++)
                            {
                                if (valueBuffer[j] >= ' ') end = j + 1;
                                else if (end != -1) break;
                                else start = j + 1;
                            }
                            var count = end - start;
                            if (count > 4)
                            {
                                var word = new string(valueBuffer, start, count);
                                if (word == "macro"
                                    || word == "extern" || word == "public" || word == "static" || word == "inline"
                                    || word == "private"
                                    || word == "override"
                                    || word == "function")
                                {
                                    abort = true;
                                    i -= valueLength;
                                }
                            }
                            valueLength = 0;
                        }
                        if (c1 == '{') braceCount++;
                        else if (c1 == '}') braceCount--;
                        else if (abort || (braceCount == 0 && c1 == ';'))
                        {
                            lastComment = null;
                            if (curMethod != null)
                            {
                                if (curMethod.Equals(curMember)) curMember = null;
                                curMethod.LineTo = line;
                                curMethod = null;
                            }
                            inFunction = false;
                            inType = false;
                            length = 0;
                        }
                        else if (valueLength < VALUE_BUFFER) valueBuffer[valueLength++] = c1;
                        continue;
                    }
                    if (braceCount > 0)
                    {
                        if (c1 == '/') LookupRegex(ba, ref i);
                        else if (c1 == '}')
                        {
                            lastComment = null;
                            braceCount--;
                            if (braceCount == 0 && curMethod != null)
                            {
                                if (curMethod.Equals(curMember)) curMember = null;
                                curMethod.LineTo = line;
                                curMethod = null;
                            }
                        }
                        else if (c1 == '{') braceCount++;
                        // escape next char
                        else if (c1 == '\\') i++;
                        continue;
                    }
                }


                /* PARSE DECLARATION VALUES/TYPES */
                
                if (inValue)
                {
                    bool stopParser = false;
                    bool valueError = false;
                    if (inType && !inAnonType && !inGeneric && !char.IsLetterOrDigit(c1)
                        && (c1 != '.' && c1 != '{' && c1 != '}' && c1 != '-' && c1 != '>' && c1 != '<'))
                    {
                        inType = false;
                        inValue = false;
                        hadValue = false;
                        inGeneric = false;
                        valueLength = 0;
                        length = 0;
                        context = 0;
                    }
                    else if (c1 == '{')
                    {
                        if (!inType || valueLength == 0 || valueBuffer[valueLength - 1] == '<' || paramBraceCount > 0 || paramTempCount > 0)
                        {
                            paramBraceCount++;
                            stopParser = true;
                        }
                    }
                    else if (c1 == '}')
                    {
                        if (paramBraceCount > 0)
                        {
                            paramBraceCount--;
                            stopParser = true;
                        }
                        else valueError = true;
                    }
                    else if (c1 == '(')
                    {
                        paramParCount++;
                        stopParser = true;
                    }
                    else if (c1 == ')')
                    {
                        if (paramParCount > 0) { paramParCount--; stopParser = true; }
                        else valueError = true;
                    }
                    else if (c1 == '[') paramSqCount++;
                    else if (c1 == ']')
                    {
                        if (paramSqCount > 0) { paramSqCount--; stopParser = true; }
                        else valueError = true;
                    }
                    else if (c1 == '<')
                    {
                        if (i > 1 && ba[i - 2] == '<') paramTempCount = 0; // a << b
                        else
                        {
                            if (inType) inGeneric = true;
                            paramTempCount++;
                        }
                    }
                    else if (c1 == '>')
                    {
                        if (ba[i - 2] == '-') { /*haxe method signatures*/ }
                        else if (paramBraceCount > 0 && inAnonType)
                        {
                            valueBuffer[valueLength++] = c1;
                            if (paramTempCount > 0) paramTempCount--;
                            continue;
                        }
                        else if (paramTempCount > 0)
                        {
                            paramTempCount--;
                            stopParser = true; //paramTempCount == 0 && paramBraceCount == 0; this would make more sense but just restore the original for now
                        }
                        else valueError = true;
                    }
                    else if (c1 == '/')
                    {
                        int i0 = i;
                        if (LookupRegex(ba, ref i) && valueLength < VALUE_BUFFER - 3)
                        {
                            valueBuffer[valueLength++] = '/';
                            for (; i0 < i; i0++)
                                if (valueLength < VALUE_BUFFER - 2) valueBuffer[valueLength++] = ba[i0];
                            valueBuffer[valueLength++] = '/';
                            continue;
                        }
                    }
                    else if ((c1 == ':' || c1 == ',') && paramBraceCount > 0) stopParser = true;

                    // end of value
                    if ((valueError || (!stopParser && paramBraceCount == 0 && paramParCount == 0 && paramSqCount == 0 && paramTempCount == 0))
                        && (c1 == ',' || c1 == ';' || c1 == '}' || c1 == '\r' || c1 == '\n' || (inParams && c1 == ')') || inType))
                    {
                        if (!inType && (!inValue || c1 != ','))
                        {
                            length = 0;
                            context = 0;
                        }
                        inValue = false;
                        inGeneric = false;
                        hadValue = true;
                    }
                    // in params, store the default value
                    else if ((inParams || inType) && valueLength < VALUE_BUFFER)
                    {
                        if (c1 <= 32)
                        {
                            if (valueLength > 0 && valueBuffer[valueLength - 1] != ' ')
                                valueBuffer[valueLength++] = ' ';
                        }
                        else valueBuffer[valueLength++] = c1;
                    }

                    // detect keywords
                    if (!char.IsLetterOrDigit(c1))
                    {
                        // escape next char
                        if (c1 == '\\' && i < len)
                        {
                            c1 = ba[i++];
                            if (valueLength < VALUE_BUFFER) valueBuffer[valueLength++] = c1;
                            continue;
                        }
                        if (stopParser || paramBraceCount > 0 || paramSqCount > 0 || paramParCount > 0 || paramTempCount > 0) continue;
                        if (valueError && c1 == ')') inValue = false;
                        else if (inType && inGeneric && (c1 == '<' || c1 == '.')) continue;
                        else if (inAnonType) continue;
                        else if (c1 != '_') hadWS = true;
                    }
                }

                // store type / parameter value
                if (hadValue) //!inValue && valueLength > 0)
                {
                    string param = valueLength > 0 ? new string(valueBuffer, 0, valueLength) : "";
                    // get text before the last keyword found
                    if (valueKeyword != null)
                    {
                        int p = param.LastIndexOfOrdinal(valueKeyword.Text);
                        if (p > 0) param = param.Substring(0, p).TrimEnd();
                    }
                    if (curMember == null)
                    {
                        if (inType)
                        {
                            prevToken.Text = curToken.Text;
                            prevToken.Line = curToken.Line;
                            prevToken.Position = curToken.Position;
                            curToken.Text = param;
                            curToken.Line = tokLine;
                            curToken.Position = tokPos;
                            EvalToken(true, true);
                            evalToken = 0;
                        }
                    }
                    else if (inType)
                    {
                        foundColon = false;
                        if (c1 == '>' && ba[i - 2] == '-' && inAnonType)
                        {
                            length = 0;
                            valueLength = 0;
                            hadValue = false;
                            inValue = false;
                            param = ASFileParserRegexes.Spaces.Replace(param, "").Replace(",", ", ");
                            if (string.IsNullOrEmpty(curMember.Type)) curMember.Type = param;
                            else curMember.Type += param;
                            i -= 2;
                            continue;
                        }
                        if (param.EndsWith('}') || param.Contains('>'))
                        {
                            param = ASFileParserRegexes.Spaces.Replace(param, "");
                            param = param.Replace(",", ", ");
                            //param = param.Replace("->", " -> ");
                        }
                        curMember.Type = param;
                        length = 0;
                        inType = false;
                    }
                    // method parameter's default value 
                    else if ((curMember.Flags & FlagType.Variable) > 0)
                    {
                        if (inParams)
                        {
                            curMember.Value = param;
                            curMember.ValueEndPosition = i;
                        }
                        curMember.LineTo = line;
                        if (c1 == '\r' || c1 == '\n') curMember.LineTo--;
                    }
                    //
                    hadValue = false;
                    valueLength = 0;
                    valueMember = null;
                    if (!inParams && c1 != '{' && c1 != ',') continue;
                    length = 0;
                }

                /* TOKENIZATION */

                // whitespace
                if (c1 <= 32)
                {
                    hadWS = true;
                    continue;
                }
                // a dot can be in an identifier
                if (c1 == '.')
                {
                    if (length > 0)
                    {
                        hadWS = false;
                        hadDot = true;
                        addChar = true;
                        if (!inValue && context == FlagType.Variable && !foundColon)
                        {
                            var keepContext = inParams && (length == 0 || buffer[0] == '.');
                            if (!keepContext) context = 0;
                        }
                    }
                    else continue;
                }
                else
                {
                    // function types
                    if (c1 == '-' && context != 0 && length > 0 && features.hasGenerics && i < len && ba[i] == '>')
                    {
                        buffer[length++] = '-';
                        buffer[length++] = '>';
                        i++;
                        inType = true;
                        hadWS = false;
                        hadDot = false;
                        continue;
                    }
                    // should we evaluate the token?
                    if (hadWS && !hadDot && !inGeneric && length > 0 && paramBraceCount == 0
                        // for example: foo(? v)
                        && (!inParams || (buffer[length - 1] != '?'))
                        // for example: String -> Int
                        && (!inType || (length < 2 || (buffer[length - 2] != '-' && buffer[length - 1] != '>'))))
                    {
                        evalToken = 1;
                    }
                    hadWS = false;
                    hadDot = false;
                    var shortcut = true;
                    // for example: function foo() return null;
                    if (!inFunction && context != 0 && curClass != null && curMethod != null && !inParams && !foundColon && c1 != ':' && c1 != ';' && c1 != '{' && c1 != '}' && braceCount == 0
                        && (curModifiers & FlagType.Function) != 0 && (curModifiers & FlagType.Extern) == 0
                        && curClass.Flags is var f && (f & FlagType.Extern) == 0 && (f & FlagType.TypeDef) == 0 && (f & FlagType.Interface) == 0)
                    {
                        inFunction = true;
                        inType = false;
                        i -= 2;
                        continue;
                    }
                    if ((c1 >= 'a' && c1 <= 'z') // valid char for keyword
                        || (c1 >= 'A' && c1 <= 'Z') // valid chars for identifiers
                        || (c1 == '$' || c1 == '_'))
                    {
                        addChar = true;
                    }
                    else
                    {
                        if (length > 0)
                        {
                            if (c1 >= '0' && c1 <= '9') addChar = true;
                            else if (c1 == '*' && context == FlagType.Import) addChar = true;
                            // generics
                            else if (c1 == '<')
                            {
                                if (!inValue && i > 2 && length > 1 && i <= len - 3)
                                {
                                    if ((char.IsLetterOrDigit(ba[i - 3]) || ba[i - 3] == '_')
                                        && (char.IsLetter(ba[i]) || (ba[i] == '{' || ba[i] == '(' || ba[i] <= ' ' || ba[i] == '?'))
                                        && (char.IsLetter(buffer[0]) || buffer[0] == '_' || inType && buffer[0] == '('))
                                    {
                                        if (curMember == null)
                                        {
                                            evalToken = 0;
                                            if (inGeneric) paramTempCount++;
                                            else
                                            {
                                                paramTempCount = 1;
                                                inGeneric = true;
                                            }
                                            addChar = true;
                                        }
                                        else if (foundColon)
                                        {
                                            evalToken = 0;
                                            inGeneric = true;
                                            inValue = true;
                                            hadValue = false;
                                            inType = true;
                                            inAnonType = false;
                                            valueLength = 0;
                                            for (int j = 0; j < length; j++)
                                                valueBuffer[valueLength++] = buffer[j];
                                            valueBuffer[valueLength++] = c1;
                                            length = 0;
                                            paramTempCount++;
                                            continue;
                                        }
                                    }
                                }
                            }
                            else if (inGeneric && (c1 == ',' || c1 == '-' || c1 == '>' || c1 == ':' || c1 == '(' || c1 == ')' || c1 == '{' || c1 == '}' || c1 == ';'))
                            {
                                hadWS = false;
                                hadDot = false;
                                evalToken = 0;
                                if (!inValue)
                                {
                                    addChar = true;
                                    if (c1 == '>')
                                    {
                                        if (paramTempCount > 0) paramTempCount--;
                                        if (paramTempCount == 0 && paramBraceCount == 0
                                            && paramSqCount == 0 && paramParCount == 0) inGeneric = false;
                                    }
                                }
                            }
                            else if (inType && c1 == ')')
                            {
                                if (paramParCount > 0)
                                {
                                    paramParCount--;
                                    addChar = true;
                                }
                                else if (paramParCount == 0 && paramTempCount == 0 && paramBraceCount == 0 && paramSqCount == 0)
                                {
                                    inType = false;
                                    shortcut = false;
                                    evalToken = 1;
                                }
                            }
                            else if (inType && c1 == '(')
                            {
                                paramParCount++;
                                addChar = true;
                            }
                            else if (c1 == '{' && length > 1
                                     && (buffer[length - 2] == '-' && buffer[length - 1] == '>'
                                        || buffer[length - 1] == ':'
                                        || buffer[length - 1] == '('
                                        || buffer[length - 1] == '?'))
                            {
                                paramBraceCount++;
                                inAnonType = true;
                                addChar = true;
                            }
                            else if (inAnonType && paramBraceCount > 0)
                            {
                                if (c1 == '}')
                                {
                                    paramBraceCount--;
                                    if (paramBraceCount == 0) inAnonType = false;
                                }
                                addChar = true;
                            }
                            else if (c1 == '?')
                            {
                                hadWS = false;
                                evalToken = 0;
                                addChar = true;
                            }
                            else if (paramBraceCount == 0)
                            {
                                evalToken = 2;
                                shortcut = false;
                            }
                        }
                        // star is valid in import statements
                        else if (c1 == '*') addChar = true;
                        else if (c1 == '?' && inParams && length == 0) addChar = true;
                        else shortcut = false;
                    }
                    // eval this word
                    if (evalToken > 0)
                    {
                        prevToken.Text = curToken.Text;
                        prevToken.Line = curToken.Line;
                        prevToken.Position = curToken.Position;
                        curToken.Text = new string(buffer, 0, length);
                        curToken.Line = tokLine;
                        curToken.Position = tokPos;
                        EvalToken(!inValue, (c1 != '=' && c1 != ','));
                        length = 0;
                        evalToken = 0;
                    }
                    if (!shortcut)
                        // start of block
                        if (c1 == '{')
                        {
                            if (foundColon && length == 0) // copy Haxe anonymous type
                            {
                                inValue = true;
                                hadValue = false;
                                inType = true;
                                inAnonType = true;
                                valueLength = 0;
                                valueBuffer[valueLength++] = c1;
                                paramBraceCount = 1;
                                paramParCount = 0;
                                paramSqCount = 0;
                                paramTempCount = 0;
                                continue;
                            }
                            // parse package/class block
                            if (context == FlagType.Package || context == FlagType.Class) context = 0;
                            else if (context == FlagType.Enum) // parse enum block
                            {
                                if (curClass != null && (curClass.Flags & FlagType.Enum) > 0) inEnum = true;
                                else
                                {
                                    context = 0;
                                    curModifiers = 0;
                                    braceCount++; // ignore block
                                }
                            }
                            else if (context == FlagType.TypeDef) // parse typedef block
                            {
                                if (curClass != null && (curClass.Flags & FlagType.TypeDef) > 0)
                                {
                                    inTypedef = true;
                                    var pos = i;
                                    while (pos < len)
                                    {
                                        var c = ba[pos++];
                                        if (c <= ' ') continue;
                                        if (c == '>')
                                        {
                                            buffer[0] = 'e';
                                            buffer[1] = 'x';
                                            buffer[2] = 't';
                                            buffer[3] = 'e';
                                            buffer[4] = 'n';
                                            buffer[5] = 'd';
                                            buffer[6] = 's';
                                            length = 7;
                                            context = FlagType.Class;
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    context = 0;
                                    curModifiers = 0;
                                    braceCount++; // ignore block
                                }
                            }
                            else if (context == FlagType.Abstract) // parse abstract block
                            {
                                if (curClass != null && (curClass.Flags & FlagType.Abstract) > 0) inAbstract = true;
                                else
                                {
                                    context = 0;
                                    curModifiers = 0;
                                    braceCount++; // ignore block
                                }
                            }
                            else if (foundConstant) // start config block
                            {
                                flattenNextBlock++;
                                foundConstant = false;
                                context = 0;
                            }
                            else if (ScriptMode) // not in a class, parse if/for/while/do blocks
                            {
                                context = 0;
                            }
                            else if (curMember != null && (curMember.Flags & FlagType.Function) != 0 && length == 0 && !foundColon)
                            {
                                inType = false;
                                braceCount++;
                            }
                            else braceCount++; // ignore block
                        }
                        // end of block
                        else if (c1 == '}')
                        {
                            curComment = null;
                            foundColon = false;
                            foundConstant = false;

                            if (flattenNextBlock > 0) // content of this block was parsed
                            {
                                flattenNextBlock--;
                            }
                            // outside of a method, the '}' ends the current class
                            else if (curClass != null)
                            {
                                curClass.LineTo = line;
                                curClass = null;
                                inEnum = false;
                                inTypedef = false;
                                inAbstract = false;
                            }
                            else if (hasPackageSection && model.PrivateSectionIndex == 0)
                                model.PrivateSectionIndex = line + 1;
                        }
                        // member type declaration
                        else if (c1 == ':' && !inValue && !inGeneric)
                        {
                            foundColon = curMember != null && curMember.Type == null;
                            // recognize compiler config block
                            if (!foundColon && braceCount == 0 
                                && i < len - 2 && ba[i] == ':' && char.IsLetter(ba[i + 1]))
                                foundConstant = true;
                        }
                        // next variable declaration
                        else if (c1 == ',')
                        {
                            if ((context == FlagType.Variable || context == FlagType.TypeDef) && curMember != null)
                            {
                                curAccess = curMember.Access;
                                foundKeyword = FlagType.Variable;
                                foundColon = false;
                                lastComment = null;
                            }
                            else if (context == FlagType.Class && prevToken.Text == "implements")
                            {
                                curToken.Text = "implements";
                                foundKeyword = FlagType.Implements;
                            }
                        }
                        else if (c1 == '(')
                        {
                            if (!inValue && context == FlagType.Variable && curToken.Text != "catch" && curToken.Text != "for")
                            {
                                if (curMember != null && valueLength == 0)
                                {
                                    if (!foundColon && !inType) // Haxe properties
                                    {
                                        curMember.Flags -= FlagType.Variable;
                                        curMember.Flags |= FlagType.Getter | FlagType.Setter;
                                        context = FlagType.Function;
                                    }
                                    else // Haxe function types with subtypes
                                    {
                                        inType = true;
                                        addChar = true;
                                        paramParCount++;
                                    }
                                }
                                else context = 0;
                            }

                            // beginning of method parameters
                            if (context == FlagType.Function)
                            {
                                context = FlagType.Variable;
                                inParams = true;
                                inGeneric = false;
                                if (valueMember != null && curMember == null)
                                {
                                    valueLength = 0;
                                    //valueMember.Flags -= FlagType.Variable; ???
                                    valueMember.Flags = FlagType.Function;
                                    curMethod = curMember = valueMember;
                                    valueMember = null;
                                }
                                else if (curMember == null)
                                {
                                    context = FlagType.Function;
                                    if ((curModifiers & FlagType.Getter) > 0)
                                    {
                                        curModifiers -= FlagType.Getter;
                                        EvalToken(true, false);
                                        curMethod = curMember;
                                        context = FlagType.Variable;
                                    }
                                    else if ((curModifiers & FlagType.Setter) > 0)
                                    {
                                        curModifiers -= FlagType.Setter;
                                        EvalToken(true, false);
                                        curMethod = curMember;
                                        context = FlagType.Variable;
                                    }
                                    else
                                    {
                                        inParams = false;
                                        context = 0;
                                    }
                                }
                                else
                                {
                                    curMethod = curMember;
                                }
                            }
                            // an Enum value with parameters
                            else if (inEnum)
                            {
                                context = FlagType.Variable;
                                inParams = true;
                                curMethod = curMember ?? new MemberModel();
                                curMethod.Name = curToken.Text;
                                curMethod.Flags = curModifiers | FlagType.Function | FlagType.Static;
                                curMethod.Parameters = new List<MemberModel>();
                                if (curClass != null && curMember == null) curClass.Members.Add(curMethod);
                            }
                            // a TypeDef method with parameters
                            else if (inTypedef)
                            {
                                context = FlagType.Variable;
                                inParams = true;
                                curMethod = curMember ?? new MemberModel();
                                curMethod.Name = curToken.Text;
                                curMethod.Flags = curModifiers | FlagType.Function;
                                curMethod.Parameters = new List<MemberModel>();
                                if (curClass != null && curMember == null) curClass.Members.Add(curMethod);
                            }
                            // an Abstract "opaque type"
                            else if (context == FlagType.Abstract && prevToken.Text == "abstract") 
                            {
                                foundKeyword = FlagType.Class;
                                curModifiers = FlagType.Extends;
                            }
                            else if (curMember == null && curToken.Text != "catch" && curToken.Text != "for")
                            {
                                context = 0;
                                inGeneric = false;
                            }
                        }
                        // end of statement
                        else if (c1 == ';')
                        {
                            context = inEnum ? FlagType.Enum : 0;
                            inGeneric = false;
                            inType = false;
                            modifiers = 0;
                            inParams = false;
                            curMember = null;
                        }
                        // end of method parameters
                        else if (c1 == ')' && inParams)
                        {
                            context = 0;
                            if (inEnum) context = FlagType.Enum;
                            else if (inTypedef) context = FlagType.TypeDef;
                            else context = FlagType.Variable;
                            modifiers = 0;
                            inParams = false;
                            curMember = curMethod;
                        }
                        // skip value of a declared variable
                        else if (c1 == '=')
                        {
                            if (context == FlagType.Variable || (context == FlagType.Enum && inEnum))
                            {
                                if (!inValue && curMember != null)
                                {
                                    inValue = true;
                                    hadValue = false;
                                    inType = false;
                                    inGeneric = false;
                                    paramBraceCount = 0;
                                    paramParCount = 0;
                                    paramSqCount = 0;
                                    paramTempCount = 0;
                                    valueLength = 0;
                                    valueMember = curMember;
                                }
                            }
                        }
                        // metadata, contexts should define a meta keyword and a way to parse metadata
                        else if (!inValue && c1 == '[')
                        {
                            if (features.hasCArrays && curMember?.Type != null)
                            {
                                if (ba[i] == ']') curMember.Type = features.CArrayTemplate + "@" + curMember.Type;
                            }
                        }
                        else if (!inValue && c1 == '@')
                        {
                            var meta = LookupMeta(ref ba, ref i);
                            if (meta != null)
                            {
                                carriedMetaData = carriedMetaData ?? new List<ASMetaData>();
                                carriedMetaData.Add(meta);
                            }
                        }
                        // Unreachable code???? plus it seems a bit crazy we have so many places for function types
                        // Haxe signatures: T -> T -> T 
                        else if (c1 == '-' && curMember != null)
                        {
                            if (ba[i] == '>' && curMember.Type != null)
                            {
                                curMember.Type += "->";
                                foundColon = true;
                                i++;
                                continue;
                            }
                        }
                        // escape next char
                        else if (c1 == '\\')
                        {
                            i++;
                            continue;
                        }
                }

                // put in buffer
                if (addChar)
                {
                    if (length < TOKEN_BUFFER) buffer[length++] = c1;
                    if (length == 1)
                    {
                        tokPos = i - 1;
                        tokLine = line;
                    }
                    addChar = false;
                }
            }

            // parsing done!
            FinalizeModel();

            // post-filtering
            if (model.HasFiltering) model.Context?.FilterSource(model);

            //  Debug.WriteLine("out model: " + model.GenerateIntrinsic(false));
        }

        private bool LookupRegex(string ba, ref int i)
        {
            if (ba[i - 2] != '~') return false;
            var len = ba.Length;
            var i0 = i - 3;
            char c;

            while (i0 > 0)
            {
                c = ba[i0--];
                if (c == '=' || c == '(' || c == '[' || c == '{' || c == '}' || c == ',' || c == ';'
                    || c == '?' || c == ':'
                    || c == '+' || c == '-' || c == '*' || c == '/'
                    || c == '|' || c == '&' || c == '~' || c == '^'
                    || c == '>' || c == '<' || c == '!'
                    || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')
                    || (c >= '0' && c <= '9'))
                {
                    break; // ok
                }

                if (c == ' ' || c == '\t') continue;
                return false; // anything else isn't expected before a regex
            }

            i0 = i;
            while (i0 < len)
            {
                c = ba[i0++];
                if (c == '\\')
                {
                    i0++;
                    continue;
                } // escape next

                if (c == '/') break; // end of regex
                if (c == '\r' || c == '\n') return false;
            }

            while (i0 < len)
            {
                c = ba[i0++];
                if (!char.IsLetter(c))
                {
                    i--;
                    break;
                }
            }

            i = i0; // ok, skip this regex
            return true;
        }

        private ASMetaData LookupMeta(ref string ba, ref int i)
        {
            var i0 = i;
            var line0 = line;
            var inString = 0;
            var parCount = 0;
            var isComplex = false;
            var isOverload = false;
            var count = 0;
            var len = ba.Length;
            while (i < len)
            {
                var c = ba[i];
                if (inString == 0)
                {
                    /**
                     * for example:
                     * @:overload(function(port:Int, host:String, ?listener:Void->Void):Socket {})
                     * static function connect(options:EitherType<NetConnectOptionsTcp, NetConnectOptionsUnix>, ?listener:Void->Void):Socket;
                     */
                    if (count == 8
                        && ba[i - 8] == ':'
                        && ba[i - 7] == 'o'
                        && ba[i - 6] == 'v'
                        && ba[i - 5] == 'e'
                        && ba[i - 4] == 'r'
                        && ba[i - 3] == 'l'
                        && ba[i - 2] == 'o'
                        && ba[i - 1] == 'a'
                        && c == 'd')
                    {
                        isOverload = true;
                    }
                    else if (c == '"') inString = 1;
                    else if (c == '\'') inString = 2;
                    else if ((!isOverload && (c == '{' || c == ';' || c == '[')) // Is this valid in Haxe meta? - slavara: yes, https://haxe.org/manual/lf-metadata.html
                             || (isOverload && (c == '\n' || c == '\r')))
                    {
                        i = i0;
                        line = line0;
                        return null;
                    }
                    else if (c == '(')
                    {
                        parCount++;
                        isComplex = true;
                    }
                    else if (c == ')')
                    {
                        parCount--;
                        if (parCount <= 0) break;
                    }
                    else if (c <= 32 && parCount <= 0) break;
                }
                else if (inString == 1 && c == '"') inString = 0;
                else if (inString == 2 && c == '\'') inString = 0;
                if (c == 10 || c == 13)
                {
                    line++;
                    if (c == 13 && i < len && ba[i + 1] == 10) i++;
                }
                i++;
                count++;
            }
            var meta = ba.Substring(i0, i - i0);
            var parIndex = meta.IndexOf('(');
            var md = new ASMetaData(isComplex ? meta.Substring(0, parIndex) : meta);
            md.LineFrom = line0;
            md.LineTo = line;
            if (isComplex) md.Params = new Dictionary<string, string> {["Default"] = meta.Substring(parIndex + 1).Trim()};
            return md;
        }

        private void FinalizeModel()
        {
            model.Version = 4;
            model.HasPackage = true;
            model.FullPackage = model.Module == ""
                ? model.Package
                : (model.Package == ""
                    ? model.Module
                    : model.Package + '.' + model.Module);
            for (var i = model.Classes.Count - 1; i >= 0; i--)
            {
                var @class = model.Classes[i];
                FinalizeMembers(@class.Members.Items);
                if (@class.MetaDatas == null || @class.Members.Count == 0) continue;
                for (var j = @class.MetaDatas.Count - 1; j >= 0; j--)
                {
                    if (@class.MetaDatas[j].Name != ":publicFields") continue;
                    for (var k = @class.Members.Count - 1; k >= 0; k--)
                    {
                        var member = @class.Members[k];
                        if ((member.Flags & FlagType.Override) != 0) continue;
                        member.Access = Visibility.Public;
                        member.Flags |= FlagType.Access;
                    }
                }
            }
            FinalizeMembers(model.Members.Items);
            if (model.FileName.Length == 0 || model.FileName.EndsWithOrdinal("_cache")) return;
            if (model.PrivateSectionIndex == 0) model.PrivateSectionIndex = line + 1;
            // utils
            void FinalizeMembers(IList<MemberModel> members)
            {
                for (var i = members.Count - 1; i >= 0; i--)
                {
                    var member = members[i];
                    if (((member.Flags & FlagType.Variable) != 0)
                        && member.Type != null && member.Type.Contains("->"))
                    {
                        member.Flags |= FlagType.Function;
                        FunctionTypeToMemberModel(member.Type, member, features);
                    }
                }
            }
        }
        #endregion

        #region lexer

        /// <summary>
        /// Eval a token depending on the parser context
        /// </summary>
        /// <param name="evalContext">The token could be an identifier</param>
        /// <param name="evalKeyword">The token could be a keyword</param>
        /// <returns>A keyword was found</returns>
        private bool EvalToken(bool evalContext, bool evalKeyword)
        {
            bool hadContext = context != 0;
            bool hadKeyword = foundKeyword != 0;
            foundKeyword = 0;

            /* KEYWORD EVALUATION */

            string token = curToken.Text;
            int dotIndex = token.LastIndexOf('.');
            if (evalKeyword && token.Length >= 2)
            {
                if (dotIndex > 0) token = token.Substring(dotIndex + 1);

                // members
                if (token == "var")
                {
                    foundKeyword = FlagType.Variable;
                }
                else if (token == "function")
                {
                    foundKeyword = FlagType.Function;
                }
                // class declaration
                else if (tryPackage && token == "package")
                {
                    foundKeyword = FlagType.Package;
                }
                else if (token == "class")
                {
                    foundKeyword = FlagType.Class;
                    modifiers |= FlagType.Class;
                }
                else if (token == "interface")
                {
                    foundKeyword = FlagType.Class;
                    modifiers |= FlagType.Class | FlagType.Interface;
                }
                else if (features.hasTypeDefs && token == "typedef")
                {
                    foundKeyword = FlagType.TypeDef;
                    modifiers |= FlagType.TypeDef;
                }
                else if (features.hasTypeDefs && token == "abstract")
                {
                    foundKeyword = FlagType.Abstract;
                    modifiers |= FlagType.Abstract;
                }
                else if (features.hasEnums && token == "enum")
                {
                    foundKeyword = FlagType.Enum;
                    modifiers |= FlagType.Enum;
                }
                // head declarations
                else if (token == features.importKey || token == features.importKeyAlt)
                {
                    foundKeyword = FlagType.Import;
                }
                // modifiers
                else
                {
                    if (context == FlagType.Class || context == FlagType.TypeDef)
                    {
                        if (token == "extends")
                        {
                            foundKeyword = FlagType.Class;
                            curModifiers = FlagType.Extends;
                            return true;
                        }
                        if (token == "implements")
                        {
                            foundKeyword = FlagType.Class;
                            curModifiers = FlagType.Implements;
                            return true;
                        }
                    }
                    else if (context == FlagType.Abstract)
                    {
                        if (features.hasTypeDefs)
                        {
                            if (token == "from")
                            {
                                foundKeyword = FlagType.Class;
                                curModifiers = FlagType.Extends;
                                if (curClass != null)
                                {
                                    if (curClass.MetaDatas == null) curClass.MetaDatas = new List<ASMetaData>();
                                    curClass.MetaDatas.Add(new ASMetaData(token) {RawParams = prevToken.Text});
                                }
                                return true;
                            }
                            if (token == "to")
                            {
                                if (curClass != null)
                                {
                                    if (curClass.MetaDatas == null) curClass.MetaDatas = new List<ASMetaData>();
                                    curClass.MetaDatas.Add(new ASMetaData(token) {RawParams = prevToken.Text});
                                }
                                return true;
                            }
                        }
                    }
                    // properties
                    else if (context == FlagType.Function)
                    {
                        if (token == "get")
                        {
                            foundKeyword = FlagType.Function;
                            curModifiers |= FlagType.Getter;
                            return true;
                        }
                        if (token == "set")
                        {
                            foundKeyword = FlagType.Function;
                            curModifiers |= FlagType.Setter;
                            return true;
                        }
                    }

                    FlagType foundModifier = 0;

                    // access modifiers
                    if (token == "public")
                    {
                        foundModifier = FlagType.Access;
                        curAccess = Visibility.Public;
                    }
                    else if (token == "private")
                    {
                        foundModifier = FlagType.Access;
                        curAccess = Visibility.Private;
                    }

                    // other modifiers
                    if (foundModifier == 0)
                    {
                        if (token == "static")
                        {
                            foundModifier = FlagType.Static;
                        }
                        else if (token == "override")
                        {
                            foundModifier = FlagType.Override;
                        }
                        else if (token == "extern" && context != FlagType.Package)
                        {
                            foundModifier = FlagType.Intrinsic | FlagType.Extern;
                        }
                        else if (token == "final")
                        {
                            foundModifier = FlagType.Final;
                        }
                        else if (token == "dynamic")
                        {
                            foundModifier = FlagType.Dynamic;
                        }
                    }
                    // a declaration modifier was recognized
                    if (foundModifier != 0)
                    {
                        if (inParams && inValue) valueKeyword = new Token(curToken);
                        inParams = false;
                        inEnum = false;
                        inTypedef = false;
                        inAbstract = false;
                        inValue = false;
                        hadValue = false;
                        inType = false;
                        inGeneric = false;
                        valueMember = null;
                        foundColon = false;
                        if (curNamespace == "internal") curNamespace = "";
                        if (context != 0)
                        {
                            modifiers = 0;
                            context = 0;
                        }
                        if (modifiers == 0)
                        {
                            modifiersLine = curToken.Line;
                            //modifiersPos = curToken.Position;
                        }
                        modifiers |= foundModifier;
                        return true;
                    }
                }
            }
            // a declaration keyword was recognized
            if (foundKeyword != 0)
            {
                if (dotIndex > 0)
                {
                    // an unexpected keyword was found, ignore previous context 
                    curToken.Text = token;
                    curToken.Line = line;
                    // TODO  Should the parser note this error?
                }

                if (inParams && inValue) valueKeyword = new Token(curToken);
                inParams = false;
                inEnum = false;
                inTypedef = false;
                inAbstract = false;
                inGeneric = false;
                inValue = false;
                hadValue = false;
                if (token != "function") valueMember = null;
                foundColon = false;
                foundConstant = false;
                context = foundKeyword;
                curModifiers = modifiers;
                if (!isBlockComment) lastComment = null;
                curComment = lastComment;
                if (foundKeyword != FlagType.Import)
                    lastComment = null;
                modifiers = 0;
                curMember = null;
                return true;
            }
            // when not in a class, parse if/for/while blocks
            if (ScriptMode)
            {
                if (token == "catch" || token == "for")
                {
                    curModifiers = 0;
                    foundKeyword = FlagType.Variable;
                    context = FlagType.Variable;
                    return false;
                }
            }
            if (inValue && valueMember != null) valueMember = null;
            if (!evalContext) return false;
            if (dotIndex > 0) token = curToken.Text;
            // some heuristic around Enums & Typedefs
            if (inEnum && !inValue)
            {
                curModifiers = 0;
                curAccess = Visibility.Public;
            }
            if (inTypedef && !inValue && curModifiers != FlagType.Extends)
            {
                curModifiers = 0;
                curAccess = Visibility.Public;
            }
            else if (!inTypedef && (curModifiers & FlagType.TypeDef) != 0
                     && curClass != null && (curClass.Flags & FlagType.TypeDef) != 0
                     && token != "extends")
            {
                curClass.ExtendsType = token;
                curModifiers = 0;
                context = 0;
                curComment = null;
                curClass = null;
                curNamespace = "internal";
                curAccess = 0;
                modifiers = 0;
                modifiersLine = 0;
                return true;
            }


            /* EVAL DECLARATION */

            if (foundColon && curMember != null)
            {
                foundColon = false;
                if (curMember.Type != null) curMember.Type += curToken.Text;
                else curMember.Type = curToken.Text;
                curMember.Type = ASFileParserRegexes.Spaces.Replace(curMember.Type, string.Empty).Replace(",", ", ");
                curMember.LineTo = curToken.Line;
            }
            else if (hadContext && (hadKeyword || inParams || inEnum || inTypedef))
            {
                MemberModel member;
                switch (context)
                {
                    case FlagType.Package:
                        if (prevToken.Text == "package")
                        {
                            model.Package = token;
                            model.Comments = curComment;
                        }
                        break;

                    case FlagType.Import:
                        if (prevToken.Text == features.importKey)
                        {
                            member = new MemberModel();
                            member.Name = LastStringToken(token, ".");
                            member.Type = token;
                            member.LineFrom = prevToken.Line;
                            member.LineTo = curToken.Line;
                            member.Flags = (token.EndsWith('*')) ? FlagType.Package : FlagType.Class;
                            if (flattenNextBlock > 0) // this declaration is inside a config block
                                member.Flags |= FlagType.Constant; 
                            model.Imports.Add(member);
                        }
                        else if (prevToken.Text == features.importKeyAlt)
                        {
                            member = new MemberModel();
                            member.Name = LastStringToken(token, ".");
                            member.Type = token;
                            member.LineFrom = prevToken.Line;
                            member.LineTo = curToken.Line;
                            member.Flags = FlagType.Class | FlagType.Using;
                            model.Imports.Add(member);
                        }
                        break;

                    case FlagType.Class: 
                    case FlagType.Struct:
                        if (curModifiers == FlagType.Extends)
                        {
                            if (curClass != null)
                            {
                                // typed Array & Proxy
                                if ((token == "Array" || token == "Proxy" || token == "flash.utils.Proxy")
                                    && lastComment != null && ASFileParserRegexes.ValidTypeName.IsMatch(lastComment))
                                {
                                    Match m = ASFileParserRegexes.ValidTypeName.Match(lastComment);
                                    if (m.Success)
                                    {
                                        token += "<" + m.Groups["type"].Value + ">";
                                        lastComment = null;
                                    }
                                }
                                curClass.ExtendsType = token;
                                if (inTypedef) context = FlagType.TypeDef;
                            }
                        }
                        else if (curModifiers == FlagType.Implements)
                        {
                            if (curClass != null)
                            {
                                if (curClass.Implements == null) curClass.Implements = new List<string>();
                                curClass.Implements.Add(token);
                            }
                        }
                        else if ((context == FlagType.Class && (prevToken.Text == "class" || prevToken.Text == "interface"))
                                 || (context == FlagType.Struct && prevToken.Text == "struct"))
                        {
                            if (curClass != null)
                            {
                                curClass.LineTo = (modifiersLine != 0) ? modifiersLine - 1 : curToken.Line - 1;
                            }
                            // check classname
                            //int p = token.LastIndexOf('.');
                            //if (p > 0)
                            //{
                            //    //TODO  Error: AS3 & Haxe classes are qualified by their package declaration
                            //}

                            if (model.PrivateSectionIndex != 0 && curToken.Line > model.PrivateSectionIndex)
                                curAccess = Visibility.Private;

                            curClass = new ClassModel();
                            curClass.InFile = model;
                            curClass.Comments = curComment;
                            var qtype = QualifiedName(model, token);
                            curClass.Type = qtype.Type;
                            curClass.Template = qtype.Template;
                            curClass.Name = qtype.Name;
                            curClass.Constructor = string.IsNullOrEmpty(features.ConstructorKey) ? token : features.ConstructorKey;
                            curClass.Flags = curModifiers;
                            curClass.Access = (curAccess == 0) ? features.classModifierDefault : curAccess;
                            curClass.Namespace = curNamespace;
                            curClass.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                            curClass.LineTo = curToken.Line;
                            AddClass(model, curClass);
                        }
                        else
                        {
                            context = 0;
                            modifiers = 0;
                        }
                        if (carriedMetaData != null)
                        {
                            if (curClass.MetaDatas == null)
                                curClass.MetaDatas = carriedMetaData;
                            else curClass.MetaDatas.AddRange(carriedMetaData);

                            carriedMetaData = null;
                        }
                        break;

                    case FlagType.Enum:
                        if (inEnum && curClass != null && prevToken.Text != "enum")
                        {
                            member = new MemberModel();
                            member.Comments = curComment;
                            member.Name = token;
                            member.Flags = curModifiers | FlagType.Variable | FlagType.Enum | FlagType.Static;
                            member.Access = Visibility.Public;
                            member.Namespace = curNamespace;
                            member.LineFrom = member.LineTo = curToken.Line;
                            curClass.Members.Add(member);
                            //
                            curMember = member;
                        }
                        else
                        {
                            if (curClass != null)
                            {
                                curClass.LineTo = (modifiersLine != 0) ? modifiersLine - 1 : curToken.Line - 1;
                            }
                            curClass = new ClassModel();
                            curClass.InFile = model;
                            curClass.Comments = curComment;
                            var qtype = QualifiedName(model, token);
                            curClass.Type = qtype.Type;
                            curClass.Template = qtype.Template;
                            curClass.Name = qtype.Name;
                            curClass.Flags = curModifiers;
                            curClass.Access = (curAccess == 0) ? features.enumModifierDefault : curAccess;
                            curClass.Namespace = curNamespace;
                            curClass.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                            curClass.LineTo = curToken.Line;
                            AddClass(model, curClass);
                        }
                        break;

                    case FlagType.TypeDef:
                        if (inTypedef && curClass != null && prevToken.Text != "typedef")
                        {
                            member = new MemberModel();
                            member.Comments = curComment;
                            member.Name = token;
                            member.Flags = curModifiers | FlagType.Variable | FlagType.Dynamic;
                            member.Access = Visibility.Public;
                            member.Namespace = curNamespace;
                            member.LineFrom = member.LineTo = curToken.Line;
                            curClass.Members.Add(member);
                            //
                            curMember = member;
                        }
                        else 
                        {
                            if (curClass != null)
                            {
                                curClass.LineTo = (modifiersLine != 0) ? modifiersLine - 1 : curToken.Line - 1;
                            }
                            curClass = new ClassModel();
                            curClass.InFile = model;
                            curClass.Comments = curComment;
                            var qtype = QualifiedName(model, token);
                            curClass.Type = qtype.Type;
                            curClass.Template = qtype.Template;
                            curClass.Name = qtype.Name;
                            curClass.Flags = FlagType.Class | FlagType.TypeDef;
                            curClass.Access = (curAccess == 0) ? features.typedefModifierDefault : curAccess;
                            curClass.Namespace = curNamespace;
                            curClass.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                            curClass.LineTo = curToken.Line;
                            AddClass(model, curClass);
                        }
                        break;

                    case FlagType.Abstract:
                        if (inAbstract && curClass != null && prevToken.Text != "abstract")
                        {
                            member = new MemberModel();
                            member.Comments = curComment;
                            member.Name = token;
                            member.Flags = curModifiers | FlagType.Variable | FlagType.Dynamic;
                            member.Access = Visibility.Public;
                            member.Namespace = curNamespace;
                            member.LineFrom = member.LineTo = curToken.Line;
                            curClass.Members.Add(member);
                            //
                            curMember = member;
                        }
                        else if (!inAbstract && curClass != null && (curClass.Flags & FlagType.Abstract) > 0)
                        {
                            if (prevToken.Text == "to") { /* can be casted to X */ }
                            else curClass.ExtendsType = curToken.Text;
                        }
                        else
                        {
                            if (curClass != null)
                            {
                                curClass.LineTo = (modifiersLine != 0) ? modifiersLine - 1 : curToken.Line - 1;
                            }
                            curClass = new ClassModel();
                            curClass.InFile = model;
                            curClass.Comments = curComment;
                            var qtype = QualifiedName(model, token);
                            curClass.Type = qtype.Type;
                            curClass.Template = qtype.Template;
                            curClass.Name = qtype.Name;
                            curClass.Flags = FlagType.Class | FlagType.Abstract;
                            curClass.Access = (curAccess == 0) ? features.typedefModifierDefault : curAccess;
                            curClass.Namespace = curNamespace;
                            curClass.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                            curClass.LineTo = curToken.Line;
                            AddClass(model, curClass);
                        }
                        if (carriedMetaData != null)
                        {
                            if (curClass.MetaDatas == null) curClass.MetaDatas = carriedMetaData;
                            else curClass.MetaDatas.AddRange(carriedMetaData);
                            carriedMetaData = null;
                        }
                        break;

                    case FlagType.Variable:
                        // Haxe signatures: T -> T
                        if (curMember?.Type != null && curMember.Type.EndsWithOrdinal("->"))
                        {
                            curMember.Type += token;
                            curMember.Type = ASFileParserRegexes.Spaces.Replace(curMember.Type, string.Empty).Replace(",", ", ");
                            return false;
                        }
                        else
                        {
                            member = new MemberModel();
                            member.Comments = curComment;
                            member.Name = token;
                            if ((curModifiers & FlagType.Static) == 0) curModifiers |= FlagType.Dynamic;
                            member.Flags = curModifiers | FlagType.Variable;
                            member.Access = (curAccess == 0) ? features.varModifierDefault : curAccess;
                            member.Namespace = curNamespace;
                            member.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                            member.LineTo = curToken.Line;
                            //
                            // method parameter
                            if (inParams && curMethod != null)
                            {
                                member.Flags = FlagType.Variable | FlagType.ParameterVar;
                                if (inEnum) member.Flags |= FlagType.Enum;
                                if (curMethod.Parameters == null) curMethod.Parameters = new List<MemberModel>();
                                member.Access = 0;
                                if (member.Name.Length > 0)
                                    curMethod.Parameters.Add(member);
                            }
                            // class member
                            else if (curClass != null)
                            {
                                FlagType forcePublic = FlagType.Interface;
                                forcePublic |= FlagType.Intrinsic | FlagType.TypeDef;
                                if ((curClass.Flags & forcePublic) > 0)
                                    member.Access = Visibility.Public;

                                curClass.Members.Add(member);
                                curClass.LineTo = member.LineTo;
                            }
                            // package member
                            else
                            {
                                member.InFile = model;
                                member.IsPackageLevel = true;
                                model.Members.Add(member);
                            }
                            //
                            curMember = member;

                            if (carriedMetaData != null)
                            {
                                if (member.MetaDatas == null)
                                    member.MetaDatas = carriedMetaData;
                                else member.MetaDatas.AddRange(carriedMetaData);

                                carriedMetaData = null;
                            }
                        }
                        break;

                    case FlagType.Function:
                        member = new MemberModel();
                        member.Comments = curComment;

                        int t = token.IndexOf('<');
                        if (t > 0)
                        {
                            member.Template = token.Substring(t);
                            token = token.Substring(0, t); 
                        }
                        member.Name = token;

                        if ((curModifiers & FlagType.Static) == 0) curModifiers |= FlagType.Dynamic;
                        if ((curModifiers & (FlagType.Getter | FlagType.Setter)) == 0)
                            curModifiers |= FlagType.Function;
                        member.Flags = curModifiers;
                        member.Access = (curAccess == 0) ? features.methodModifierDefault : curAccess;
                        member.Namespace = curNamespace;
                        member.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                        member.LineTo = curToken.Line;
                        //
                        if (curClass != null)
                        {
                            var constructorKey = features.ConstructorKey;
                            if (token == curClass.Constructor || (!string.IsNullOrEmpty(constructorKey) && token == constructorKey))
                            {
                                if (token == constructorKey)
                                {
                                    member.Name = curClass.Name;
                                    curClass.Constructor = curClass.Name;
                                }
                                member.Flags |= FlagType.Constructor;
                                if ((member.Flags & FlagType.Dynamic) > 0) member.Flags -= FlagType.Dynamic;
                                if (curAccess == 0) curAccess = Visibility.Public;
                            }

                            FlagType forcePublic = FlagType.Interface;
                            forcePublic |= FlagType.Intrinsic | FlagType.TypeDef;
                            if (curAccess == 0 && (curClass.Flags & forcePublic) > 0)
                                member.Access = Visibility.Public;

                            curClass.Members.Add(member);
                            curClass.LineTo = member.LineTo;
                        }
                        // package-level function
                        else
                        {
                            member.InFile = model;
                            member.IsPackageLevel = true;
                            model.Members.Add(member);
                        }
                        //
                        curMember = member;
                        if (carriedMetaData != null)
                        {
                            if (member.MetaDatas == null)
                                member.MetaDatas = carriedMetaData;
                            else member.MetaDatas.AddRange(carriedMetaData);

                            carriedMetaData = null;
                        }
                        break;
                }
                if (context != FlagType.Function && !inParams) curMethod = null;
                curComment = null;
                curNamespace = "internal";
                curAccess = 0;
                modifiers = 0;
                modifiersLine = 0;
                inGeneric = false;
                tryPackage = false;
            }
            return false;
        }

        private void AddClass(FileModel model, ClassModel curClass)
        {
            // avoid empty duplicates due to Haxe directives
            for (int i = 0, count = model.Classes.Count; i < count; i++)
            {
                var aClass = model.Classes[i];
                if (aClass.Name != curClass.Name) continue;
                if (aClass.Members.Count != 0) return;
                model.Classes.Remove(aClass);
                break;
            }
            model.Classes.Add(curClass);
        }
        #endregion

        #region tool methods

        QType QualifiedName(FileModel inFile, string name) 
        {
            var qt = new QType();
            var type = name;
            if (inFile.Package == "") type = name;
            else if (inFile.Module == "" || inFile.Module == name) type = inFile.Package + "." + name;
            else type = inFile.Package + "." + inFile.Module + "." + name;
            qt.Type = type;
            int p = name.IndexOf('<');
            if (p > 0)
            {
                qt.Template = name.Substring(p);
                if (name[p - 1] == '.') p--;
                qt.Name = name.Substring(0, p);
            }
            else qt.Name = name;
            return qt;
        }

        string LastStringToken(string token, string separator)
        {
            var p = token.LastIndexOfOrdinal(separator);
            return (p >= 0) ? token.Substring(p + 1) : token;
        }

        #endregion

        public static MemberModel FunctionTypeToMemberModel(string type, ContextFeatures features) => FunctionTypeToMemberModel(type, new MemberModel(), features);

        static MemberModel FunctionTypeToMemberModel(string type, MemberModel result, ContextFeatures features)
        {
            var voidKey = features.voidKey;
            if (result.Parameters == null) result.Parameters = new List<MemberModel>();
            var parCount = 0;
            var braCount = 0;
            var genCount = 0;
            var startPosition = 0;
            var typeLength = type.Length;
            for (var i = 0; i < typeLength; i++)
            {
                string parameterType = null;
                var c = type[i];
                if (c == '(') parCount++;
                else if (c == ')')
                {
                    parCount--;
                    if (parCount == 0 && braCount == 0 && genCount == 0)
                    {
                        parameterType = type.Substring(startPosition, (i + 1) - startPosition);
                        startPosition = i + 1;
                    }
                }
                else if (c == '{') braCount++;
                else if (c == '}')
                {
                    braCount--;
                    if (parCount == 0 && braCount == 0 && genCount == 0)
                    {
                        parameterType = type.Substring(startPosition, (i + 1) - startPosition);
                        startPosition = i + 1;
                    }
                }
                else if (c == '<') genCount++;
                else if (c == '>' && type[i - 1] != '-')
                {
                    genCount--;
                    if (parCount == 0 && braCount == 0 && genCount == 0)
                    {
                        parameterType = type.Substring(startPosition, (i + 1) - startPosition);
                        startPosition = i + 1;
                    }
                }
                else if (parCount == 0 && braCount == 0 && genCount == 0 && c == '-' && type[i + 1] == '>')
                {
                    if (i > startPosition) parameterType = type.Substring(startPosition, i - startPosition);
                    startPosition = i + 2;
                    i++;
                }
                if (parameterType == null)
                {
                    if (i == typeLength - 1 && i > startPosition) result.Type = type.Substring(startPosition);
                    continue;
                }
                var parameterName = $"parameter{result.Parameters.Count}";
                if (parameterType.StartsWith('?'))
                {
                    parameterName = $"?{parameterName}";
                    parameterType = parameterType.TrimStart('?');
                }
                if (i == typeLength - 1) result.Type = parameterType;
                else result.Parameters.Add(new MemberModel(parameterName, parameterType, FlagType.ParameterVar, 0));
            }
            if (result.Parameters.Count == 1 && result.Parameters[0].Type == voidKey)
                result.Parameters.Clear();
            return result;
        }
    }

    public class QType
    {
        public string Name;
        public string Type;
        public string Template;
    }
}
