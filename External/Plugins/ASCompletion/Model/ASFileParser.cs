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

        override public string ToString()
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
    //
    #endregion

    #region TypeCommentUtils class
    /// <summary>
    /// Contributor: i.o.
    /// </summary>
    public class TypeCommentUtils
    {
        //---------------------
        // FIELDS
        //---------------------

        public static string ObjectType = "Object"; // will differ in haxe
        private static Random random = new Random(123456);


        //---------------------
        // PUBLIC METHODS
        //---------------------

        /// <summary>
        /// Type-comment parsing into model (source and destination)
        /// </summary>
        public static TypeDefinitionKind Parse(string comment, MemberModel model)
        {
            return Parse(comment, model, false);
        }
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
        public static TypeDefinitionKind ParseTypedObject(string comment, MemberModel model)
        {
            return ParseTypedObject(comment, model, false);
        }
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
        public static TypeDefinitionKind ParseTypedArray(string comment, MemberModel model)
        {
            return ParseTypedArray(comment, model, false);
        }
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
        public static TypeDefinitionKind ParseTypedCallback(string comment, MemberModel model)
        {
            return ParseTypedCallback(comment, model, false);
        }
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
            if (string.IsNullOrEmpty(comment))
                return null;

            int idxBraceOp = comment.IndexOf('(');
            int idxBraceCl = comment.IndexOf(')');

            if (idxBraceOp != 0 || idxBraceCl < 1)
                return null;

            // replace strings by temp replacements
            MatchCollection qStrMatches = ASFileParserRegexes.QuotedString.Matches(comment);
            Dictionary<String, String> qStrRepls = new Dictionary<string, string>();
            int i = qStrMatches.Count;
            while (i-- > 0)
            {
                String strRepl = getRandomStringRepl();
                qStrRepls.Add(strRepl, qStrMatches[i].Value);
                comment = comment.Substring(0, qStrMatches[i].Index) + strRepl + comment.Substring(qStrMatches[i].Index + qStrMatches[i].Length);
            }

            // refreshing
            idxBraceOp = comment.IndexOf('(');
            idxBraceCl = comment.IndexOf(')');

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
            String pBody = comment.Substring(idxBraceOp, 1 + idxBraceCl - idxBraceOp);
            MatchCollection pMatches = ASFileParserRegexes.Parameter.Matches(pBody);
            int l = pMatches.Count;
            for (i = 0; i < l; i++)
            {
                string pName = pMatches[i].Groups["pName"].Value;
                if (!string.IsNullOrEmpty(pName))
                {
                    foreach (KeyValuePair<String,String> replEntry in qStrRepls)
                    {
                        if (pName.IndexOfOrdinal(replEntry.Key) > -1)
                        {
                            pName = "[COLOR=#F00][I]InvalidName[/I][/COLOR]";
                            break;
                        }
                    }
                }

                string pType = pMatches[i].Groups["pType"].Value;
                if (!string.IsNullOrEmpty(pType))
                {
                    foreach (KeyValuePair<String,String> replEntry in qStrRepls)
                    {
                        if (pType.IndexOfOrdinal(replEntry.Key) > -1)
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
                        foreach (KeyValuePair<String,String> replEntry in qStrRepls)
                        {
                            if (pVal.IndexOfOrdinal(replEntry.Key) > -1)
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
    //
    #endregion

    #region ASFileParserRegexes class
    //
    public class ASFileParserRegexes
    {
        public static readonly Regex Spaces = new Regex("\\s+", RegexOptions.Compiled);
        public static readonly Regex RegionStart = new Regex(@"^{[ ]?region[:\\s]*(?<name>[^\r\n]*)", RegexOptions.Compiled);
        public static readonly Regex RegionEnd = new Regex(@"^}[ ]?endregion", RegexOptions.Compiled);
        public static readonly Regex QuotedString = new Regex("(\"(\\\\.|[^\"\\\\])*\")|('(\\\\.|[^'\\\\])*')", RegexOptions.Compiled);
        public static readonly Regex FunctionType = new Regex(@"\)\s*\:\s*(?<fType>[\w\$\.\<\>\@]+)", RegexOptions.Compiled);
        public static readonly Regex ValidTypeName = new Regex("^(\\s*of\\s*)?(?<type>[\\w.\\$]*)$", RegexOptions.Compiled);
        public static readonly Regex ValidObjectType = new Regex("^(?<type>[\\w.,\\$]*)$", RegexOptions.Compiled);
        public static readonly Regex Import = new Regex("^[\\s]*import[\\s]+(?<package>[\\w.]+)",
                                                        ASFileParserRegexOptions.MultilineComment);
        public static readonly Regex Parameter = new Regex(@"[\(,]\s*((?<pName>(\.\.\.)?[\w\$]+)\s*(\:\s*(?<pType>[\w\$\*\.\<\>\@]+))?(\s*\=\s*(?<pVal>[^\,\)]+))?)",
                                                           RegexOptions.Compiled);
        public static readonly Regex BalancedBraces = new Regex("{[^{}]*(((?<Open>{)[^{}]*)+((?<Close-Open>})[^{}]*)+)*(?(Open)(?!))}",
                                                                ASFileParserRegexOptions.SinglelineComment);

        private const string typeChars = @"[\w\$][\w\d\$]*";
        private const string typeClsf = @"(\s*(?<Classifier>" + typeChars + @"(\." + typeChars + ")*" + @"(\:\:?" + typeChars + ")?" + @")\s*)";
        private const string typeComment = @"(\s*\/\*(?<Comment>.*)\*\/\s*)";
        public static readonly Regex TypeDefinition = new Regex(@"^((" + typeClsf + typeComment + ")|(" + typeComment + typeClsf + ")|(" + typeClsf + "))$",
                                                                RegexOptions.Compiled);
    }
    //
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
    //
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

            if (String.IsNullOrEmpty(typeDefinition))
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
            if (String.IsNullOrEmpty(typeDefinition))
                return TypeDefinitionKind.Null;

            if (typeDefinition.IndexOfOrdinal("/*") < 0 || typeDefinition.IndexOfOrdinal("*/") < 0)
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
    //
    #endregion

    /// <summary>
    /// Old & clumsy AS2/AS3/haxe file parser - beware!
    /// </summary>
    public class ASFileParser
    {

        #region public methods

        static public FileModel ParseFile(FileModel fileModel)
        {
            // parse file
            if (fileModel.FileName.Length > 0)
            {
                if (File.Exists(fileModel.FileName))
                {
                    var src = FileHelper.ReadFile(fileModel.FileName);
                    ASFileParser parser = new ASFileParser();
                    fileModel.LastWriteTime = File.GetLastWriteTime(fileModel.FileName);
                    parser.ParseSrc(fileModel, src);
                }
                // the file is not available (for the moment?)
                else if (Path.GetExtension(fileModel.FileName).Length > 0)
                {
                    fileModel.OutOfDate = true;
                }
            }
            // this is a package
            else
            {
                // ignore
            }
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
        private bool haXe;
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

        public bool ScriptMode;

        public ContextFeatures Features
        {
            get { return features; }
        }

        public ASFileParser()
        {
            features = new ContextFeatures();
        }

        /// <summary>
        /// Rebuild a file model with the source provided
        /// </summary>
        /// <param name="fileModel">Model</param>
        /// <param name="ba">Source</param>
        ///
        public void ParseSrc(FileModel fileModel, string ba)
        {
            ParseSrc(fileModel, ba, true);
        }
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
            haXe = model.haXe;
            TypeCommentUtils.ObjectType = features.dynamicKey;
            version = (haXe) ? 4 : 1;
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
                                else if (c2 == '*')
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
                                    else if (c2 > 32) { inlineDirective = true; break; }
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
                            if (c1 == 10 || c1 == 13) { if (!haXe) inString = 0; }
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

                            // extract "include" declarations
                            if (inString == 0 && length == 7 && context == 0)
                            {
                                string token = new string(buffer, 0, length);
                                if (token == "include")
                                {
                                    string inc = ba.Substring(tokPos, i - tokPos);
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
                                c2 = ba[i];
                                if (c2 == '\\') { i++; continue; }
                                if (c2 == '/')
                                {
                                    end = true;
                                    break;
                                }
                                else if (c2 == '*') i++;
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
                                        else
                                        {
                                            // Not a comment!  We're done!
                                            end = true;
                                            break;
                                        }
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
                            MemberModel region = new MemberModel(regionName, String.Empty, FlagType.Declaration, Visibility.Default);
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
                else if (isInString)
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
                        LookupRegex(ba, ref i);
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
                    if (inType && !inAnonType && !inGeneric && !char.IsLetterOrDigit(c1) && ".{}-><".IndexOf(c1) < 0)
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
                            EvalToken(true, true/*false*/, i - 1 - valueLength);
                            evalToken = 0;
                        }
                    }
                    else if (inType)
                    {
                        foundColon = false;
                        if (haXe)
                        {
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
                            if (param.EndsWith('}') || param.Contains(">"))
                            {
                                param = ASFileParserRegexes.Spaces.Replace(param, "");
                                param = param.Replace(",", ", ");
                                //param = param.Replace("->", " -> ");
                            }
                        }
                        curMember.Type = param;
                        length = 0;
                        inType = false;
                    }
                    // AS3 const or method parameter's default value 
                    else if (version > 2 && (curMember.Flags & FlagType.Variable) > 0)
                    {
                        if (inParams || inConst) curMember.Value = param;
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
                    else length = 0;
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
                    if (c1 == '-' && context != 0 && length > 0 && features.hasGenerics && i < len && ba[i] == '>')
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
                    if (c1 >= 'a' && c1 <= 'z')
                    {
                        addChar = true;
                    }
                    else
                    {
                        // valid chars for identifiers
                        if ((!haXe && char.IsLetter(c1)) || (c1 >= 'A' && c1 <= 'Z'))
                        {
                            addChar = true;
                        }
                        else if (c1 == '$' || c1 == '_')
                        {
                            addChar = true;
                        }
                        else if (length > 0)
                        {
                            if (c1 >= '0' && c1 <= '9')
                            {
                                addChar = true;
                            }
                            else if (c1 == '*' && context == FlagType.Import)
                            {
                                addChar = true;
                            }
                            // AS3/Haxe generics
                            else if (c1 == '<' && features.hasGenerics)
                            {
                                if (!inValue && i > 2 && length > 1 && i < len - 3
                                    && (char.IsLetterOrDigit(ba[i - 3]) || ba[i - 3] == '_') && (char.IsLetter(ba[i]) || (haXe && (ba[i] == '{' || ba[i] == '(' || ba[i] <= ' ' || ba[i] == '?')))
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
                                        /*
                                        paramBraceCount = 0;
                                        paramParCount = 0;
                                        paramSqCount = 0;*/
                                        paramTempCount++;
                                        continue;
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
                            else if (haXe && inType && c1 == ')')
                            {
                                if (paramParCount > 0)
                                {
                                    paramParCount--;
                                    addChar = true;
                                }
                                else if (paramParCount == 0 && paramTempCount == 0 && paramBraceCount == 0
                                    && paramSqCount == 0)
                                {
                                    inType = false;
                                    shortcut = false;
                                    evalToken = 1;
                                }
                            }
                            else if (haXe && inType && c1 == '(')
                            {
                                paramParCount++;
                                addChar = true;
                            }
                            else if (haXe && c1 == '{' && length > 1
                                    && (buffer[length - 2] == '-' && buffer[length - 1] == '>'
                                        || buffer[length - 1] == ':'
                                        || buffer[length - 1] == '('
                                        || buffer[length - 1] == '?'))
                            {
                                paramBraceCount++;
                                inAnonType = true;
                                addChar = true;
                            }
                            else if (haXe && inAnonType && c1 == '}')
                            {
                                paramBraceCount--;
                                if (paramBraceCount == 0) inAnonType = false;
                                addChar = true;
                            }
                            else if (haXe && inAnonType && paramBraceCount > 0) addChar = true;
                            else if (haXe && c1 == '?')
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
                        else if (c1 == '*' && version >= 3)
                        {
                            addChar = true;
                        }
                        // conditional Haxe parameter
                        else if (c1 == '?' && haXe && inParams && length == 0)
                        {
                            addChar = true;
                        }
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
                        EvalToken(!inValue, (c1 != '=' && c1 != ','), i - 1 - length);
                        length = 0;
                        evalToken = 0;
                    }

                    if (!shortcut)
                        // start of block
                        if (c1 == '{')
                        {
                            if (context == FlagType.Package || context == FlagType.Class) // parse package/class block
                            {
                                context = 0;
                            }
                            else if (context == FlagType.Enum) // parse enum block
                            {
                                if (curClass != null && (curClass.Flags & FlagType.Enum) > 0)
                                    inEnum = true;
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
                                    if (i < len && ba[i] == '>')
                                    {
                                        buffer[0] = 'e'; buffer[1] = 'x'; buffer[2] = 't'; buffer[3] = 'e'; buffer[4] = 'n'; buffer[5] = 'd'; buffer[6] = 's';
                                        length = 7;
                                        context = FlagType.Class;
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
                                if (curClass != null && (curClass.Flags & FlagType.Abstract) > 0)
                                    inAbstract = true;
                                else
                                {
                                    context = 0;
                                    curModifiers = 0;
                                    braceCount++; // ignore block
                                }
                            }
                            else if (foundColon && haXe && length == 0) // copy Haxe anonymous type
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
                                inEnum = false;
                                inTypedef = false;
                                inAbstract = false;
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
                                && i < len - 2 && ba[i] == ':' && Char.IsLetter(ba[i + 1]))
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
                            if (!inValue && context == FlagType.Variable && curToken.Text != "catch" && (!haXe || curToken.Text != "for"))
                            {
                                if (haXe && curMember != null && valueLength == 0)
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
                                        EvalToken(true, false, i);
                                        curMethod = curMember;
                                        context = FlagType.Variable;
                                    }
                                    else if ((curModifiers & FlagType.Setter) > 0)
                                    {
                                        curModifiers -= FlagType.Setter;
                                        EvalToken(true, false, i);
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
                                //
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
                                //
                                if (curClass != null && curMember == null) curClass.Members.Add(curMethod);
                            }

                            // an Abstract "opaque type"
                            else if (context == FlagType.Abstract && prevToken.Text == "abstract") 
                            {
                                foundKeyword = FlagType.Class;
                                curModifiers = FlagType.Extends;
                            }

                            else if (curMember == null && curToken.Text != "catch" && (!haXe || curToken.Text != "for"))
                            {
                                context = 0;
                                inGeneric = false;
                            }
                        }
                        // end of statement
                        else if (c1 == ';')
                        {
                            context = (inEnum) ? FlagType.Enum : 0;
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
                                var meta = LookupMeta(ref ba, ref i);
                                if (meta != null)
                                {
                                    carriedMetaData = carriedMetaData ?? new List<ASMetaData>();
                                    carriedMetaData.Add(meta);
                                }
                            }
                            else if (features.hasCArrays && curMember != null && curMember.Type != null)
                            {
                                if (ba[i] == ']') curMember.Type = features.CArrayTemplate + "@" + curMember.Type;
                            }
                        }
                        else if (!inValue && c1 == '@' && haXe)
                        {
                            var meta = LookupHaxeMeta(ref ba, ref i);
                            if (meta != null)
                            {
                                carriedMetaData = carriedMetaData ?? new List<ASMetaData>();
                                carriedMetaData.Add(meta);
                            }
                        }
                        // Unreachable code???? plus it seems a bit crazy we have so many places for function types
                        // Haxe signatures: T -> T -> T 
                        else if (haXe && c1 == '-' && curMember != null)
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
                        else if (c1 == '\\') { i++; continue; }
                        // literal regex
                        else if (c1 == '/' && version == 3)
                        {
                            if (LookupRegex(ba, ref i))
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
            if (model.HasFiltering && model.Context != null)
                model.Context.FilterSource(model);

            //  Debug.WriteLine("out model: " + model.GenerateIntrinsic(false));
        }

        private bool LookupRegex(string ba, ref int i)
        {
            int len = ba.Length;
            int i0;
            char c;
            // regex in valid context

            if (!haXe) i0 = i - 2;
            else
            {
                if (ba[i - 2] != '~')
                    return false;
                i0 = i - 3;
            }

            while (i0 > 0)
            {
                c = ba[i0--];
                if ("=(,[{;?:+-*/|&~^><n!".IndexOf(c) >= 0) break; // ok
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
                    else if ("{;[".IndexOf(c) >= 0)
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

        private ASMetaData LookupHaxeMeta(ref string ba, ref int i)
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
                if (inString == 0)
                {
                    if (c == '"') inString = 1;
                    else if (c == '\'') inString = 2;
                    else if ("{;[".IndexOf(c) >= 0) // Is this valid in Haxe meta?
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
                    else if (c <= 32 && parCount <= 0)
                    {
                        break;
                    }
                }
                else if (c == 10 || c == 13)
                {
                    line++;
                    if (c == 13 && i < len && ba[i + 1] == 10) i++;
                }
                else if (inString == 1 && c == '"') inString = 0;
                else if (inString == 2 && c == '\'') inString = 0;
                i++;
            }

            string meta = ba.Substring(i0, i - i0);
            ASMetaData md = new ASMetaData(isComplex ? meta.Substring(0, meta.IndexOf('(')) : meta);
            md.LineFrom = line0;
            md.LineTo = line;
            if (isComplex)
            {
                meta = meta.Substring(meta.IndexOf('(') + 1).Trim();
                md.Params = new Dictionary<string, string>();
                md.Params["Default"] = meta;
            }
            return md;
        }

        private void FinalizeModel()
        {
            model.Version = version;
            model.HasPackage = hasPackageSection || haXe;
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
        /// <param name="position">Parser position</param>
        /// <returns>A keyword was found</returns>
        private bool EvalToken(bool evalContext, bool evalKeyword, int position)
        {
            bool hadContext = (context != 0);
            bool hadKeyword = (foundKeyword != 0);
            foundKeyword = 0;

            /* KEYWORD EVALUATION */

            string token = curToken.Text;
            int dotIndex = token.LastIndexOf('.');
            if (evalKeyword && (token.Length > 2))
            {
                if (dotIndex > 0) token = token.Substring(dotIndex + 1);

                // members
                if (token == "var" || (token == "catch" && !haXe))
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
                else if (features.hasStructs && token == "struct")
                {
                    foundKeyword = FlagType.Struct;
                    modifiers |= FlagType.Class | FlagType.Struct;
                }
                else if (features.hasEnums && token == "enum")
                {
                    foundKeyword = FlagType.Enum;
                    modifiers |= FlagType.Enum;
                }

                // head declarations
                else if (token == features.importKey || (haXe && token == features.importKeyAlt))
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
                        else if (token == "implements")
                        {
                            foundKeyword = FlagType.Class;
                            curModifiers = FlagType.Implements;
                            return true;
                        }
                    }

                    else if (context == FlagType.Abstract) 
                    {
                        if (features.hasTypeDefs && token == "from")
                        {
                            foundKeyword = FlagType.Class;
                            curModifiers = FlagType.Extends;
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
                        else if (token == "set")
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
                        else if (version == 4 && token == "extern")
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
                        // namespace modifier
                        else if (features.hasNamespaces && model.Namespaces.Count > 0)
                            foreach (KeyValuePair<string, Visibility> ns in model.Namespaces)
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
                        inEnum = false;
                        inTypedef = false;
                        inAbstract = false;
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
                inEnum = false;
                inTypedef = false;
                inAbstract = false;
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
            else
            {
                // when not in a class, parse if/for/while blocks
                if (ScriptMode)
                {
                    if (token == "catch" || (haXe && token == "for"))
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
                else if (!inTypedef && curModifiers == FlagType.TypeDef && curClass != null && token != "extends")
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
            }


            /* EVAL DECLARATION */

            if (foundColon && curMember != null)
            {
                foundColon = false;
                if (haXe && curMember.Type != null) curMember.Type += curToken.Text;
                else curMember.Type = curToken.Text;
                curMember.Type = ASFileParserRegexes.Spaces.Replace(curMember.Type, string.Empty).Replace(",", ", ");
                curMember.LineTo = curToken.Line;
                // Typed Arrays

                if (TypeCommentUtils.Parse(lastComment, curMember) != TypeDefinitionKind.Null)
                    lastComment = null;
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
                            int p = token.LastIndexOf('.');
                            if (p > 0)
                            {
                                if (version < 3)
                                {
                                    model.Package = token.Substring(0, p);
                                    token = token.Substring(p + 1);
                                }
                                else
                                {
                                    //TODO  Error: AS3 & Haxe classes are qualified by their package declaration
                                }
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
                            else
                                foreach (var meta in carriedMetaData) curClass.MetaDatas.Add(meta);

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
                        break;

                    case FlagType.Variable:
                        // Haxe signatures: T -> T
                        if (haXe && curMember != null && curMember.Type != null
                            && curMember.Type.EndsWithOrdinal("->"))
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
                                if (haXe) forcePublic |= FlagType.Intrinsic | FlagType.TypeDef;
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
                                else
                                    foreach (var meta in carriedMetaData) member.MetaDatas.Add(meta);

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
                            if (token == curClass.Constructor)
                            {
                                if (haXe) // constructor is: new()
                                {
                                    member.Name = curClass.Name;
                                    curClass.Constructor = curClass.Name;
                                }
                                member.Flags |= FlagType.Constructor;
                                if ((member.Flags & FlagType.Dynamic) > 0) member.Flags -= FlagType.Dynamic;
                                if (curAccess == 0) curAccess = Visibility.Public;
                            }

                            FlagType forcePublic = FlagType.Interface;
                            if (haXe) forcePublic |= FlagType.Intrinsic | FlagType.TypeDef;
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
                            else
                                foreach (var meta in carriedMetaData) member.MetaDatas.Add(meta);

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
            foreach(ClassModel aClass in model.Classes) 
                if (aClass.Name == curClass.Name)
                {
                    if (aClass.Members.Count == 0)
                    {
                        model.Classes.Remove(aClass);
                        break;
                    }
                    else return;
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

        private String LastStringToken(string token, string separator)
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
