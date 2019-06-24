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
using PluginCore;
using PluginCore.Helpers;

namespace ASCompletion.Model
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

        public override string ToString()
        {
            return Text;
        }
    }
    #endregion

    #region TypeDefinitionKind enum
    /// <summary>
    /// Contributor: i.o.
    /// </summary>
    public enum TypeDefinitionKind : uint
    {
        Null = 0,
        Simple = 1,
        TypedArray = 2,
        TypedCallback = 3,
        TypedObject = 4
    }
    
    #endregion

    #region TypeCommentUtils class
    /// <summary>
    /// Contributor: i.o.
    /// </summary>
    public class TypeCommentUtils
    {
        const string ObjectType = "Object";
        private static readonly Random random = new Random(123456);

        /// <summary>
        /// Type-comment parsing into model (source and destination)
        /// </summary>
        public static TypeDefinitionKind Parse(string comment, MemberModel model) => Parse(comment, model, false);

        public static TypeDefinitionKind Parse(string comment, MemberModel model, bool detectKindOnly)
        {
            if (model != null && !string.IsNullOrEmpty(comment))
            {
                switch (model.Type)
                {
                    case "Array":
                        return ParseTypedArray(comment, model, detectKindOnly);

                    case "Function":
                        return ParseTypedCallback(comment, model, detectKindOnly);
                }

                if (model.Type == ObjectType) 
                    return ParseTypedObject(comment, model, detectKindOnly);
            }
            return TypeDefinitionKind.Null;
        }

        /// <summary>
        /// Typed object parsing
        /// </summary>
        public static TypeDefinitionKind ParseTypedObject(string comment, MemberModel model) => ParseTypedObject(comment, model, false);

        public static TypeDefinitionKind ParseTypedObject(string comment, MemberModel model, bool detectKindOnly)
        {
            if (model != null && !string.IsNullOrEmpty(comment))
            {
                Match m = ASFileParserRegexes.ValidObjectType.Match(comment);
                if (m.Success)
                {
                    if (!detectKindOnly)
                        model.Type = ObjectType + "@" + m.Groups["type"].Value;
                    return TypeDefinitionKind.TypedObject;
                }
            }
            return TypeDefinitionKind.Null;
        }

        /// <summary>
        /// Typed array parsing
        /// </summary>
        public static TypeDefinitionKind ParseTypedArray(string comment, MemberModel model) => ParseTypedArray(comment, model, false);

        public static TypeDefinitionKind ParseTypedArray(string comment, MemberModel model, bool detectKindOnly)
        {
            if (model != null && !string.IsNullOrEmpty(comment))
            {
                Match m = ASFileParserRegexes.ValidTypeName.Match(comment);
                if (m.Success)
                {
                    if (!detectKindOnly)
                        model.Type = "Array@" + m.Groups["type"].Value;

                    return TypeDefinitionKind.TypedArray;
                }
            }
            return TypeDefinitionKind.Null;
        }

        /// <summary>
        /// Typed callbck parsing
        /// </summary>
        public static TypeDefinitionKind ParseTypedCallback(string comment, MemberModel model) => ParseTypedCallback(comment, model, false);

        public static TypeDefinitionKind ParseTypedCallback(string comment, MemberModel model, bool detectKindOnly)
        {
            if (model != null && !string.IsNullOrEmpty(comment)
                && (model.Flags & FlagType.Function) == 0)
            {
                MemberModel fnModel = extractTypedCallbackModel(comment);
                if (fnModel != null)
                {
                    if (!detectKindOnly)
                    {
                        model.Type = fnModel.Type;
                        model.Flags |= FlagType.Function;
                        model.Parameters = fnModel.Parameters;
                        if (model.Access == 0)
                        {
                            if (model.Namespace == "internal")
                            {
                                if ((model.Access & Visibility.Public) == 0)
                                    model.Access = Visibility.Internal;

                                model.Namespace = "";
                            }
                            else model.Access = Visibility.Public;
                        }
                    }
                    return TypeDefinitionKind.TypedCallback;
                }
            }
            return TypeDefinitionKind.Null;
        }


        //---------------------
        // PRIVATE METHODS
        //---------------------

        /// <summary>
        /// String randomer
        /// </summary>
        private static string getRandomStringRepl()
        {
            random.NextDouble();
            return "StringRepl" + random.Next(0xFFFFFFF);
        }

        /// <summary>
        /// TypedCallback model extracting
        /// </summary>
        private static MemberModel extractTypedCallbackModel(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return null;
            if (comment.IndexOf('(') != 0 || comment.IndexOf(')') < 1) return null;

            // replace strings by temp replacements
            MatchCollection qStrMatches = ASFileParserRegexes.QuotedString.Matches(comment);
            Dictionary<string, string> qStrRepls = new Dictionary<string, string>();
            int i = qStrMatches.Count;
            while (i-- > 0)
            {
                string strRepl = getRandomStringRepl();
                qStrRepls.Add(strRepl, qStrMatches[i].Value);
                comment = comment.Substring(0, qStrMatches[i].Index) + strRepl + comment.Substring(qStrMatches[i].Index + qStrMatches[i].Length);
            }

            // refreshing
            var idxBraceOp = comment.IndexOf('(');
            var idxBraceCl = comment.IndexOf(')');

            if (idxBraceOp != 0 || comment.LastIndexOf('(') != idxBraceOp
                || idxBraceCl < 0 || comment.LastIndexOf(')') != idxBraceCl)
                return null;

            MemberModel fm = new MemberModel("unknown", "*", FlagType.Function, Visibility.Default);
            fm.Parameters = new List<MemberModel>();

            // return type
            Match m = ASFileParserRegexes.FunctionType.Match(comment.Substring(idxBraceCl));
            if (m.Success)
                fm.Type = m.Groups["fType"].Value;

            // parameters
            string pBody = comment.Substring(idxBraceOp, 1 + idxBraceCl - idxBraceOp);
            MatchCollection pMatches = ASFileParserRegexes.Parameter.Matches(pBody);
            int l = pMatches.Count;
            for (i = 0; i < l; i++)
            {
                string pName = pMatches[i].Groups["pName"].Value;
                if (!string.IsNullOrEmpty(pName))
                {
                    foreach (KeyValuePair<string,string> replEntry in qStrRepls)
                    {
                        if (pName.Contains(replEntry.Key))
                        {
                            pName = "[COLOR=#F00][I]InvalidName[/I][/COLOR]";
                            break;
                        }
                    }
                }

                string pType = pMatches[i].Groups["pType"].Value;
                if (!string.IsNullOrEmpty(pType))
                {
                    foreach (KeyValuePair<string,string> replEntry in qStrRepls)
                    {
                        if (pType.Contains(replEntry.Key))
                        {
                            pType = "[COLOR=#F00][I]InvalidType[/I][/COLOR]";
                            break;
                        }
                    }
                }

                string pVal = pMatches[i].Groups["pVal"].Value;
                if (!string.IsNullOrEmpty(pVal))
                {
                    if (qStrRepls.ContainsKey(pVal))
                    {
                        pVal = qStrRepls[pVal];
                    }
                    else
                    {
                        foreach (KeyValuePair<string,string> replEntry in qStrRepls)
                        {
                            if (pVal.Contains(replEntry.Key))
                            {
                                pVal = "[COLOR=#F00][I]InvalidValue[/I][/COLOR]";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    pVal = null;
                }

                MemberModel pModel = new MemberModel();
                pModel.Name = pName;
                pModel.Type = pType;
                pModel.Value = pVal;
                pModel.Flags = FlagType.ParameterVar;
                pModel.Access = Visibility.Default;

                fm.Parameters.Add(pModel);
            }
            return fm;
        }
    }
    
    #endregion

    #region ASFileParserRegexes class
    
    public class ASFileParserRegexes
    {
        public static readonly Regex Spaces = new Regex("\\s+", RegexOptions.Compiled);
        public static readonly Regex RegionStart = new Regex(@"^{[ ]?region[:\\s]*(?<name>[^\r\n]*)", RegexOptions.Compiled);
        public static readonly Regex RegionEnd = new Regex(@"^}[ ]?endregion", RegexOptions.Compiled);
        public static readonly Regex QuotedString = new Regex("(\"(\\\\.|[^\"\\\\])*\")|('(\\\\.|[^'\\\\])*')", RegexOptions.Compiled);
        public static readonly Regex FunctionType = new Regex(@"\)\s*\:\s*(?<fType>[\w\$\.\<\>\@]+)", RegexOptions.Compiled);
        public static readonly Regex ValidTypeName = new Regex("^(\\s*of\\s*)?(?<type>[\\w.\\$]*)$", RegexOptions.Compiled);
        public static readonly Regex ValidObjectType = new Regex("^(?<type>[\\w.,\\$]*)$", RegexOptions.Compiled);
        public static readonly Regex Import = new Regex("^[\\s]*import[\\s]+(?<package>[\\w.]+)", ASFileParserRegexOptions.MultilineComment);
        public static readonly Regex Parameter = new Regex(@"[\(,]\s*((?<pName>(\.\.\.)?[\w\$]+)\s*(\:\s*(?<pType>[\w\$\*\.\<\>\@]+))?(\s*\=\s*(?<pVal>[^\,\)]+))?)", RegexOptions.Compiled);
        public static readonly Regex BalancedBraces = new Regex("{[^{}]*(((?<Open>{)[^{}]*)+((?<Close-Open>})[^{}]*)+)*(?(Open)(?!))}", ASFileParserRegexOptions.SinglelineComment);

        private const string typeChars = @"[\w\$][\w\d\$]*";
        private const string typeClsf = @"(\s*(?<Classifier>" + typeChars + @"(\." + typeChars + ")*" + @"(\:\:?" + typeChars + ")?" + @")\s*)";
        private const string typeComment = @"(\s*\/\*(?<Comment>.*)\*\/\s*)";
        public static readonly Regex TypeDefinition = new Regex(@"^((" + typeClsf + typeComment + ")|(" + typeComment + typeClsf + ")|(" + typeClsf + "))$", RegexOptions.Compiled);
    }
    
    #endregion

    #region ASFileParserRegexOptions class
    //
    public class ASFileParserRegexOptions
    {
        public const RegexOptions MultilineComment = RegexOptions.Compiled | RegexOptions.Multiline;
        public const RegexOptions SinglelineComment = RegexOptions.Compiled | RegexOptions.Singleline;
    }
    //
    #endregion

    #region ASFileParserUtils class
    
    public class ASFileParserUtils
    {
        /// <summary>
        /// Contributor: i.o.
        /// Description: Extracts from plain string a type classifier and type comment
        /// Example:
        ///     typeDefinition: "Array/*String*/" or "Array[spaces]/*String*/" or "/*String*/Array"
        ///     typeClassifier: "Array"
        ///     typeComment: "String"
        /// </summary>
        public static bool ParseTypeDefinition(string typeDefinition, out string typeClassifier, out string typeComment)
        {
            typeClassifier = null;
            typeComment = null;

            if (string.IsNullOrEmpty(typeDefinition))
                return false;

            Match m = ASFileParserRegexes.TypeDefinition.Match(typeDefinition);
            if (!m.Success)
                return false;

            typeClassifier = m.Groups["Classifier"].Value;

            if (m.Groups["Comment"].Success)
                typeComment = m.Groups["Comment"].Value;

            return true;
        }

        public static TypeDefinitionKind ParseTypeDefinitionInto(string typeDefinition, MemberModel model)
        {
            return ParseTypeDefinitionInto(typeDefinition, model, true, true);
        }
        public static TypeDefinitionKind ParseTypeDefinitionInto(string typeDefinition, MemberModel model, bool parseCommon, bool parseGeneric)
        {
            if (string.IsNullOrEmpty(typeDefinition))
                return TypeDefinitionKind.Null;

            if (!typeDefinition.Contains("/*") || !typeDefinition.Contains("*/"))
            {
                if (!parseCommon)
                    return TypeDefinitionKind.Null;

                model.Type = typeDefinition.Replace(":", ".");
                if (model.Type.IndexOf('$') > 0) model.Type = model.Type.Replace("$", ".<") + ">";
                return TypeDefinitionKind.Simple;
            }

            if (!parseGeneric)
                return TypeDefinitionKind.Null;

            string typeClassifier;
            string typeComment;
            if (!ParseTypeDefinition(typeDefinition, out typeClassifier, out typeComment))
                return TypeDefinitionKind.Null;

            model.Type = typeClassifier;

            return TypeCommentUtils.Parse(typeComment, model);
        }
    }
    
    #endregion

    public interface IFileParser
    {
        bool ScriptMode { set; }

        /// <summary>
        /// Rebuild a file model with the source provided
        /// </summary>
        /// <param name="fileModel">Model</param>
        /// <param name="src">Source</param>
        void ParseSrc(FileModel fileModel, string src);
        void ParseSrc(FileModel fileModel, string src, bool allowBaReExtract);
    }

    /// <summary>
    /// Old & clumsy AS2/AS3 file parser - beware!
    /// </summary>
    public class ASFileParser : IFileParser
    {

        #region public methods

        public static FileModel ParseFile(FileModel fileModel)
        {
            // parse file
            if (fileModel.FileName.Length > 0)
            {
                if (File.Exists(fileModel.FileName))
                {
                    var parser = new ASFileParser();
                    parser.Parse(fileModel);
                }
                // the file is not available (for the moment?)
                else if (Path.GetExtension(fileModel.FileName).Length > 0)
                {
                    fileModel.OutOfDate = true;
                }
            }
            // this is a package

            return fileModel;
        }
        #endregion

        #region parser context
        const int COMMENTS_BUFFER = 4096;
        const int TOKEN_BUFFER = 1024;
        const int VALUE_BUFFER = 1024;

        // parser context
        private FileModel model;
        private int version;
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
        private bool inGeneric;
        private bool inValue;
        private bool hadValue;
        private bool inConst;
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

        #region tokenizer

        public bool ScriptMode { private get; set; }

        public ASFileParser() : this(new ContextFeatures())
        {
        }

        public ASFileParser(ContextFeatures features)
        {
            this.features = features;
        }

        /// <summary>
        /// Rebuild a file model using the content of that file.
        /// </summary>
        /// <param name="fileModel">Model</param>
        public FileModel Parse(FileModel fileModel)
        {
            fileModel.LastWriteTime = File.GetLastWriteTime(fileModel.FileName);
            var src = FileHelper.ReadFile(fileModel.FileName);
            ParseSrc(fileModel, src);
            return fileModel;
        }

        /// <inheritdoc />
        public void ParseSrc(FileModel fileModel, string src) => ParseSrc(fileModel, src, true);

        public void ParseSrc(FileModel fileModel, string src, bool allowBaReExtract)
        {
            //TraceManager.Add("Parsing " + Path.GetFileName(fileModel.FileName));
            model = fileModel;
            model.OutOfDate = false;
            if (model.Context != null) features = model.Context.Features;
            if (features.hasModules) model.Module = Path.GetFileNameWithoutExtension(model.FileName);

            // pre-filtering
            if (allowBaReExtract && model.HasFiltering && model.Context != null)
                src = model.Context.FilterSource(fileModel.FileName, src);

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
            int len = src.Length;
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
            version = 1;
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
            inValue = false;
            hadValue = false;
            inConst = false;
            inType = false;
            inGeneric = false;
            inAnonType = false;

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
                var c1 = src[i++];
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
                                c2 = src[i];
                                if (c2 == '/')
                                {
                                    // Check if this this is a /// comment
                                    if (i + 1 < len && src[i + 1] == '/')
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
                                    isBlockComment = (i + 1 < len && src[i + 1] == '*');
                                    matching = 2;
                                    inCode = false;
                                    i++;
                                    while (i < len - 1)
                                    {
                                        c2 = src[i];
                                        if (c2 == '*' && src[i + 1] != '/') i++;
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
                                    c2 = src[ls--];
                                    if (c2 == 10 || c2 == 13) break;
                                    if (c2 > 32) { inlineDirective = true; break; }
                                }
                                c2 = src[i];
                                if (i < 2 || src[i - 2] < 33 && c2 >= 'a' && c2 <= 'z')
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
                            if (c1 == 10 || c1 == 13) { inString = 0; }
                            else if ((c1 == '"' && inString == 1) || (c1 == '\'' && inString == 2))
                            {
                                // Are we on an escaped ' or ""?
                                int escNo = 0;
                                int l = i - 2;
                                while (l > -1 && src[l--] == '\\')
                                    escNo++;

                                // Even number of escaped \ means we are not on an escaped ' or ""
                                if (escNo % 2 == 0) inString = 0;
                            }

                            // extract "include" declarations
                            if (inString == 0 && length == 7 && context == 0)
                            {
                                string token = new string(buffer, 0, length);
                                if (token == "include")
                                {
                                    string inc = src.Substring(tokPos, i - tokPos);
                                    ASMetaData meta = new ASMetaData("Include");
                                    meta.ParseParams(inc);
                                    if (curClass == null)
                                    {
                                        if (carriedMetaData == null) carriedMetaData = new List<ASMetaData>();
                                        carriedMetaData.Add(meta);
                                    }
                                    else
                                    {
                                        if (curClass.MetaDatas == null) curClass.MetaDatas = new List<ASMetaData>();
                                        curClass.MetaDatas.Add(meta);
                                    }
                                }
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
                                c2 = src[i];
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
                            if (i + 3 < len && src[i] == 'e' && src[i + 1] == 'n' && src[i + 2] == 'd' && src[i + 3] <= 32)
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
                            if (2 <= i && (src[i - 2] == 10 || src[i - 2] == 13))
                            {
                                // Check ahead to the next line, see if it has a /// comment on it too.
                                // If it does, we want to continue the comment with that line.  If it
                                // doesn't, then this comment is finished and we will set end to true.
                                for (int j = i + 1; j < len; ++j)
                                {
                                    // Skip whitespace
                                    char twoBack = src[j - 2];
                                    if (' ' != twoBack && '\t' != twoBack)
                                    {
                                        if ('/' == twoBack && '/' == src[j - 1] && '/' == src[j])
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
                    if (c1 == 13 && i < len && src[i] == 10) i++;
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
                            c2 = src[i];
                            if (commentLength < COMMENTS_BUFFER) commentBuffer[commentLength++] = c2;
                            if (c2 == 10 || c2 == 13)
                                break;
                            i++;
                        }

                        string comment = new string(commentBuffer, 0, commentLength);
                        
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
                if (braceCount > 0 && !inValue)
                {
                    if (c1 == '/')
                    {
                        LookupRegex(src, ref i);
                    }
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
                        if (i > 1 && src[i - 2] == '<') paramTempCount = 0; // a << b
                        else
                        {
                            if (inType) inGeneric = true;
                            paramTempCount++;
                        }
                    }
                    else if (c1 == '>')
                    {
                        if (src[i - 2] == '-') { /*haxe method signatures*/ }
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
                        if (LookupRegex(src, ref i) && valueLength < VALUE_BUFFER - 3)
                        {
                            valueBuffer[valueLength++] = '/';
                            for (; i0 < i; i0++)
                                if (valueLength < VALUE_BUFFER - 2) valueBuffer[valueLength++] = src[i0];
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
                    else if ((inParams || inType || inConst) && valueLength < VALUE_BUFFER)
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
                            c1 = src[i++];
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
                        curMember.Type = param;
                        length = 0;
                        inType = false;
                    }
                    // AS3 const or method parameter's default value 
                    else if (version > 2 && (curMember.Flags & FlagType.Variable) > 0)
                    {
                        if (inParams || inConst)
                        {
                            curMember.Value = param;
                            curMember.ValueEndPosition = i;
                        }
                        curMember.LineTo = line;
                        if (c1 == '\r' || c1 == '\n') curMember.LineTo--;
                        if (inConst && c1 != ',')
                        {
                            context = 0;
                            inConst = false;
                        }
                    }
                    //
                    hadValue = false;
                    valueLength = 0;
                    valueMember = null;
                    if (!inParams && !(inConst && context != 0) && c1 != '{' && c1 != ',') continue;
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
                    if (length > 0 || (inParams && version == 3))
                    {
                        hadWS = false;
                        hadDot = true;
                        addChar = true;
                        if (!inValue && context == FlagType.Variable && !foundColon)
                        {
                            bool keepContext = inParams && (length == 0 || buffer[0] == '.');
                            if (!keepContext) context = 0;
                        }
                    }
                    else continue;
                }
                else
                {
                    // function types
                    if (c1 == '-' && context != 0 && length > 0 && features.hasGenerics && i < len && src[i] == '>')
                    {
                        buffer[length++] = '-';
                        buffer[length++] = '>';
                        i++;
                        inType = true;
                        continue;
                    }

                    // should we evaluate the token?
                    if (hadWS && !hadDot && !inGeneric && length > 0 && paramBraceCount == 0)
                    {
                        evalToken = 1;
                    }
                    hadWS = false;
                    hadDot = false;
                    bool shortcut = true;

                    // valid char for keyword
                    if (c1 >= 'a' && c1 <= 'z') addChar = true;
                    else
                    {
                        // valid chars for identifiers
                        if (char.IsLetter(c1) || (c1 >= 'A' && c1 <= 'Z')) addChar = true;
                        else if (c1 == '$' || c1 == '_') addChar = true;
                        else if (length > 0)
                        {
                            if (c1 >= '0' && c1 <= '9') addChar = true;
                            else if (c1 == '*' && context == FlagType.Import) addChar = true;
                            // AS3 generics
                            else if (c1 == '<' && features.hasGenerics)
                            {
                                if (!inValue && i > 2 && length > 1 && i <= len - 3)
                                {
                                    if (src[i] == '*')
                                    {
                                        inGeneric = true;
                                        inValue = false;
                                        hadValue = false;
                                        inType = false;
                                        inAnonType = false;
                                        valueLength = 0;
                                        buffer[length++] = '<';
                                        buffer[length++] = '*';
                                        i++;
                                        continue;
                                    }
                                    if ((char.IsLetterOrDigit(src[i - 3]) || src[i - 3] == '_')
                                        && (char.IsLetter(src[i]))
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
                            else if (paramBraceCount == 0)
                            {
                                evalToken = 2;
                                shortcut = false;
                            }
                        }
                        // star is valid in import statements
                        else if (c1 == '*' && version >= 3) addChar = true;
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
                            // parse package/class block
                            if (context == FlagType.Package || context == FlagType.Class) context = 0;
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
                            }
                            else
                            {
                                if (hasPackageSection && model.PrivateSectionIndex == 0) model.PrivateSectionIndex = line + 1;
                            }
                        }
                        // member type declaration
                        else if (c1 == ':' && !inValue && !inGeneric)
                        {
                            foundColon = curMember != null && curMember.Type == null;
                            // recognize compiler config block
                            if (!foundColon && braceCount == 0 
                                && i < len - 2 && src[i] == ':' && char.IsLetter(src[i + 1]))
                                foundConstant = true;
                        }
                        // next variable declaration
                        else if (c1 == ',')
                        {
                            if (context == FlagType.Variable && curMember != null)
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
                            if (!inValue && context == FlagType.Variable && curToken.Text != "catch")
                            {
                                context = 0;
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
                            else if (curMember == null && curToken.Text != "catch")
                            {
                                context = 0;
                                inGeneric = false;
                            }
                        }
                        // end of statement
                        else if (c1 == ';')
                        {
                            context = 0;
                            inGeneric = false;
                            inType = false;
                            modifiers = 0;
                            inParams = false;
                            curMember = null;
                        }
                        // end of method parameters
                        else if (c1 == ')' && inParams)
                        {
                            context = FlagType.Variable;
                            modifiers = 0;
                            inParams = false;
                            curMember = curMethod;
                        }
                        // skip value of a declared variable
                        else if (c1 == '=')
                        {
                            if (context == FlagType.Variable)
                            {
                                if (!inValue && curMember != null)
                                {
                                    inValue = true;
                                    hadValue = false;
                                    inConst = (curMember.Flags & FlagType.Constant) > 0;
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
                            if (version == 3)
                            {
                                var meta = LookupMeta(ref src, ref i);
                                if (meta != null)
                                {
                                    carriedMetaData ??= new List<ASMetaData>();
                                    carriedMetaData.Add(meta);
                                }
                            }
                            else if (features.hasCArrays && curMember?.Type != null)
                            {
                                if (src[i] == ']') curMember.Type = features.CArrayTemplate + "@" + curMember.Type;
                            }
                        }
                        // escape next char
                        else if (c1 == '\\')
                        {
                            i++;
                            continue;
                        }
                        // literal regex
                        else if (c1 == '/' && version == 3)
                        {
                            if (LookupRegex(src, ref i))
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
            int len = ba.Length;
            char c;
            
            var i0 = i - 2;
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
                    break;// ok
                }
                if (c == ' ' || c == '\t') continue;
                return false; // anything else isn't expected before a regex
            }
            i0 = i;
            while (i0 < len)
            {
                c = ba[i0++];
                if (c == '\\') { i0++; continue; } // escape next
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
            int len = ba.Length;
            int i0 = i;
            int line0 = line;
            int inString = 0;
            int parCount = 0;
            bool isComplex = false;
            while (i < len)
            {
                char c = ba[i];
                if (c == 10 || c == 13)
                {
                    line++;
                    if (c == 13 && i < len && ba[i + 1] == 10) i++;
                }
                if (inString == 0)
                {
                    if (c == '"') inString = 1;
                    else if (c == '\'') inString = 2;
                    else if (c == '{' || c == ';' || c == '[')
                    {
                        i = i0;
                        line = line0;
                        return null;
                    }
                    else if (c == '(') parCount++;
                    else if (c == ')')
                    {
                        parCount--;
                        if (parCount < 0) return null;
                        isComplex = true;
                    }
                    else if (c == ']') break;
                }
                else if (inString == 1 && c == '"') inString = 0;
                else if (inString == 2 && c == '\'') inString = 0;
                else if (c == 10 || c == 13) inString = 0;
                i++;
            }

            string meta = ba.Substring(i0, i - i0);
            ASMetaData md = new ASMetaData(isComplex ? meta.Substring(0, meta.IndexOf('(')) : meta);
            md.LineFrom = line0;
            md.LineTo = line;
            if (isComplex)
            {
                meta = meta.Substring(meta.IndexOf('(') + 1);
                md.ParseParams(meta.Substring(0, meta.Length - 1));
                if (lastComment != null && isBlockComment && (md.Name == "Event" || md.Name == "Style"))
                {
                    md.Comments = lastComment;
                    lastComment = null;
                }
                else lastComment = null;
            }
            return md;
        }

        private void FinalizeModel()
        {
            model.Version = version;
            model.HasPackage = hasPackageSection;
            model.FullPackage = model.Module == "" ? model.Package
                : (model.Package == "" ? model.Module : model.Package + '.' + model.Module);
            if (model.FileName.Length == 0 || model.FileName.EndsWithOrdinal("_cache")) return;
            if (model.PrivateSectionIndex == 0) model.PrivateSectionIndex = line + 1;
            if (version == 2)
            {
                string className = model.GetPublicClass().Name;
                if (className.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                    return;

                string testPackage = Path.Combine(Path.GetDirectoryName(model.FileName), className);
                if (Directory.Exists(testPackage)) model.TryAsPackage = true;
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
            bool hadContext = (context != 0);
            bool hadKeyword = (foundKeyword != 0);
            foundKeyword = 0;

            /* KEYWORD EVALUATION */

            string token = curToken.Text;
            int dotIndex = token.LastIndexOf('.');
            if (evalKeyword && token.Length >= 3)
            {
                if (dotIndex > 0) token = token.Substring(dotIndex + 1);

                // members
                if (token == "var" || token == "catch")
                {
                    foundKeyword = FlagType.Variable;
                }
                else if (token == "function")
                {
                    foundKeyword = FlagType.Function;
                }
                else if (features.hasConsts && token == "const")
                {
                    foundKeyword = FlagType.Variable;
                    modifiers |= FlagType.Constant;
                }
                else if (features.hasNamespaces && token == "namespace")
                {
                    if (context == 0 && prevToken.Text != "use")
                        foundKeyword = FlagType.Namespace;
                }
                else if (features.hasDelegates && token == "delegate")
                {
                    foundKeyword = FlagType.Function;
                    modifiers |= FlagType.Delegate;
                }
                // class declaration
                else if (tryPackage && token == "package")
                {
                    foundKeyword = FlagType.Package;
                    if (version < 3)
                    {
                        version = 3;
                        hasPackageSection = true;
                        //model.Namespaces.Add("AS3", Visibility.Public);
                        //model.Namespaces.Add("ES", Visibility.Public);
                    }
                }
                else if (token == "class")
                {
                    foundKeyword = FlagType.Class;
                    modifiers |= FlagType.Class;
                    if (version == 1)
                    {
                        version = 2;
                        hasPackageSection = true;
                    }
                }
                else if (token == "interface")
                {
                    foundKeyword = FlagType.Class;
                    modifiers |= FlagType.Class | FlagType.Interface;
                    if (version == 1)
                    {
                        version = 2;
                        hasPackageSection = true;
                    }
                }
                else if (features.hasStructs && token == "struct")
                {
                    foundKeyword = FlagType.Struct;
                    modifiers |= FlagType.Class | FlagType.Struct;
                }
                // head declarations
                else if (token == features.importKey)
                {
                    foundKeyword = FlagType.Import;
                }
                // modifiers
                else
                {
                    if (context == FlagType.Class)
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
                    else if (token == features.protectedKey)
                    {
                        foundModifier = FlagType.Access;
                        curAccess = Visibility.Protected;
                    }
                    else if (token == features.internalKey)
                    {
                        foundModifier = FlagType.Access;
                        curAccess = Visibility.Internal;
                    }
                    else if (version == 3 && !hadContext) // TODO Handle namespaces properly
                    {
                        if (token == "AS3")
                        {
                            foundModifier = FlagType.Access;
                            curAccess = Visibility.Public;
                            curNamespace = token;
                        }
                        else if (token == "flash_proxy")
                        {
                            foundModifier = FlagType.Access;
                            curAccess = Visibility.Public;
                            curNamespace = token;
                        }
                    }

                    // other modifiers
                    if (foundModifier == 0)
                    {
                        if (token == "static")
                        {
                            foundModifier = FlagType.Static;
                        }
                        else if (version <= 3 && token == "intrinsic")
                        {
                            foundModifier = FlagType.Intrinsic;
                        }
                        else if (token == "override")
                        {
                            foundModifier = FlagType.Override;
                        }
                        else if (version == 3 && token == "native")
                        {
                            foundModifier = FlagType.Intrinsic | FlagType.Native;
                        }
                        else if (token == "final")
                        {
                            foundModifier = FlagType.Final;
                        }
                        else if (token == "dynamic")
                        {
                            foundModifier = FlagType.Dynamic;
                        }
                        // namespace modifier
                        else if (features.hasNamespaces && model.Namespaces.Count > 0)
                            foreach (var ns in model.Namespaces)
                                if (token == ns.Key)
                                {
                                    curAccess = ns.Value;
                                    curNamespace = token;
                                    foundModifier = FlagType.Namespace;
                                }
                    }
                    // a declaration modifier was recognized
                    if (foundModifier != 0)
                    {
                        if (inParams && inValue) valueKeyword = new Token(curToken);
                        inParams = false;
                        inValue = false;
                        hadValue = false;
                        inConst = false;
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
                inGeneric = false;
                inValue = false;
                hadValue = false;
                inConst = false;
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
                if (token == "catch")
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

            /* EVAL DECLARATION */

            if (foundColon && curMember != null)
            {
                foundColon = false;
                curMember.Type = curToken.Text;
                curMember.Type = ASFileParserRegexes.Spaces.Replace(curMember.Type, string.Empty).Replace(",", ", ");
                curMember.LineTo = curToken.Line;
                // Typed Arrays

                if (TypeCommentUtils.Parse(lastComment, curMember) != TypeDefinitionKind.Null)
                    lastComment = null;
            }
            else if (hadContext && (hadKeyword || inParams))
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

                    case FlagType.Namespace:
                        if (prevToken.Text == "namespace")
                        {
                            if (!model.Namespaces.ContainsKey(token))
                            {
                                model.Namespaces.Add(token, curAccess);
                                // namespace is treated as a variable
                                member = new MemberModel();
                                member.Comments = curComment;
                                member.Name = token;
                                member.Type = "Namespace";
                                member.Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Namespace;
                                member.Access = (curAccess == 0) ? features.varModifierDefault : curAccess;
                                member.Namespace = curNamespace;
                                member.LineFrom = (modifiersLine != 0) ? modifiersLine : curToken.Line;
                                member.LineTo = curToken.Line;
                                if (curClass != null) curClass.Members.Add(member);
                                else
                                {
                                    member.InFile = model;
                                    member.IsPackageLevel = true;
                                    model.Members.Add(member);
                                }
                            }
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
                            member.Flags = token.EndsWith('*') ? FlagType.Package : FlagType.Class;
                            if (flattenNextBlock > 0) // this declaration is inside a config block
                                member.Flags |= FlagType.Constant; 
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
                            int p = token.LastIndexOf('.');
                            if (p > 0)
                            {
                                if (version < 3)
                                {
                                    model.Package = token.Substring(0, p);
                                    token = token.Substring(p + 1);
                                }
                                //TODO  Error: AS3 classes are qualified by their package declaration
                            }

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

                    case FlagType.Variable:
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
                            if (curMethod.Parameters == null) curMethod.Parameters = new List<MemberModel>();
                            member.Access = 0;
                            if (member.Name.Length > 0)
                                curMethod.Parameters.Add(member);
                        }
                        // class member
                        else if (curClass != null)
                        {
                            FlagType forcePublic = FlagType.Interface;
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
            foreach(var aClass in model.Classes)
                if (aClass.Name == curClass.Name)
                {
                    if (aClass.Members.Count == 0)
                    {
                        model.Classes.Remove(aClass);
                        break;
                    }
                    return;
                }
            model.Classes.Add(curClass);
        }
        #endregion

        #region tool methods

        public QType QualifiedName(FileModel InFile, string Name) 
        {
            var qt = new QType();
            var type = Name;
            if (InFile.Package == "") type = Name;
            else if (InFile.Module == "" || InFile.Module == Name) type = InFile.Package + "." + Name;
            else type = InFile.Package + "." + InFile.Module + "." + Name;
            qt.Type = type;
            int p = Name.IndexOf('<');
            if (p > 0)
            {
                qt.Template = Name.Substring(p);
                if (Name[p - 1] == '.') p--;
                qt.Name = Name.Substring(0, p);
            }
            else qt.Name = Name;
            return qt;
        }

        private string LastStringToken(string token, string separator)
        {
            int p = token.LastIndexOfOrdinal(separator);
            return (p >= 0) ? token.Substring(p + 1) : token;
        }

        #endregion
    }

    public class QType
    {
        public string Name;
        public string Type;
        public string Template;
    }
}
