/*
 * Actionscript classes parser
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ASCompletion
{
    /// <summary>
    /// Actionscript class parser
    /// </summary>
    public class ASClassParser
    {
        #region interface with application
        static private IASContext context;
        
        static public IASContext Context
        {
            get { return context; }
            set { context = value; }
        }
        #endregion
        
        #region regular_expressions_definitions
        static public readonly RegexOptions ro_cm =
            RegexOptions.Compiled | RegexOptions.Multiline;
        static public readonly RegexOptions ro_cs =
            RegexOptions.Compiled | RegexOptions.Singleline;

        // uncommenting source
        //static public readonly Regex re_lineAgainstBlocComments =
        //  new Regex("^(?<keep>[^*]*//[^\r\n]*)/\\*", ro_cm);
            //new Regex("^(?<keep>[^*]*//.*)/\\*", ro_cm);
        static public readonly Regex re_blocComments =
            new Regex("(?<keep>[^\\/])/\\*.*?\\*/", ro_cs);
        static public readonly Regex re_lineComments =
            new Regex("^(?<keep>.*?)//.*$", ro_cm);
        static public readonly Regex re_usefullComments =
            new Regex("(?<keep>[^\\/])(?<comment>/\\*\\*.*?\\*/)", ro_cs);
        static public readonly Regex re_commentIndex =
            new Regex("comment(?<index>[\\d]*)", ro_cs);
        static public readonly Regex re_commentContent =
            new Regex("^.*\\*(?<keep>.*)$", ro_cm);

        // class parsing
        static public readonly Regex re_AS3package =
            new Regex("[\\s]*package[\\s]+(?<name>[\\w.]*)[\\s]*{", ro_cs);
        static public readonly Regex re_import =
            new Regex("^[\\s]*import[\\s]+(?<package>[\\w.]+)", ro_cm);
        static public readonly Regex re_class =
            new Regex("(?<keys>[^;.]*)[\\s]?(?<ctype>(class|interface))[\\s]+(?<cname>[\\w.]+)(?<herit>.*?){", ro_cs);
        static public readonly Regex re_extends =
            new Regex("[\\s]extends[\\s]+(?<cname>[\\w.]+)", ro_cs);
        static public readonly Regex re_implements =
            new Regex("[\\s]implements[\\s]", ro_cs);

        static public readonly Regex re_functions =
            new Regex("[\\w\\s]*[\\s]function[\\s]*[^;]*", ro_cs);
            //new Regex(";[\\w\\s]*[\\s]function[\\s]*[^;]*", ro_cs);
        static public readonly Regex re_splitFunction =
            new Regex("(?<keys>[\\w\\s]*)[\\s]function[\\s]*(?<fname>[^(]*)\\((?<params>[^()]*)\\)(?<type>.*)", ro_cs);
        static public readonly Regex re_parametersSeparator =
            new Regex("[\\s]*,[\\s]*", ro_cs);

        static public readonly Regex re_variable =
            new Regex("[\\w\\s]*[\\s]var[\\s]+[^,;=\r\n]+", ro_cs);
            //new Regex("[;{}][\\w\\s]*[\\s]var[\\s]+[^;=]+", ro_cs);
        static public readonly Regex re_splitVariable =
            new Regex("(?<keys>[\\w\\s]*)[\\s]var[\\s]+(?<pname>[\\w$]+)(?<type>.*)", ro_cs);
        static public readonly Regex re_variableType =
            new Regex("[\\s]*:[\\s]*(?<type>[\\w.]+)", ro_cs);
        static public readonly Regex re_isGetterSetter =
            new Regex("^(?<type>[gs])et[\\s]+(?<pname>.+)$", ro_cs);
        
        // HACK  AS3 'const' support
        static public readonly Regex re_constant =
            new Regex("[\\w\\s]*[\\s]const[\\s]+[^,;=\r\n]+", ro_cs);
        static public readonly Regex re_splitConstant =
            new Regex("(?<keys>[\\w\\s]*)[\\s]const[\\s]+(?<pname>[\\w$]+)(?<type>.*)", ro_cs);

        // cleaning
        static private readonly Regex re_colonParams =
            new Regex("[\\s]*:[\\s]*", ro_cs);

        // balanced matching, see: http://blogs.msdn.com/bclteam/archive/2005/03/15/396452.aspx
        static public readonly Regex re_balancedBraces =
            new Regex("{[^{}]*(((?<Open>{)[^{}]*)+((?<Close-Open>})[^{}]*)+)*(?(Open)(?!))}", ro_cs);
        #endregion

        #region main_parseclass_function
        /// <summary>
        /// Parse a class from file
        /// </summary>
        /// <param name="aClass">Class object</param>
        /// <param name="filename">Class filename</param>
        static public void ParseClass(ASClass aClass)
        {
            string src;
            // read file content
            try
            {
                if (!File.Exists(aClass.FileName)) return;
                StreamReader sr = new StreamReader(aClass.FileName);
                src = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                context.DisplayError(ex.Message);
                return;
            }
            // parse
            ParseClass(aClass, src);
        }

        /// <summary>
        /// Parse a class from source
        /// </summary>
        /// <param name="aClass">Class object</param>
        /// <param name="src">Class source</param>
        static public void ParseClass(ASClass aClass, string src)
        {
            // remove comments
            StringCollection comments = new StringCollection();
            src = CleanClassSource(src, comments);

            // check class definition
            Match mClass = re_class.Match(src);
            if (!mClass.Success) {
                aClass.ClassName = null;
                return;
            }
            
            // classname
            string prevCName = aClass.ClassName;
            aClass.ClassName = mClass.Groups["cname"].Value;
            
            // HACK  AS3 package support
            string preClassSrc = src.Substring(0,mClass.Groups["ctype"].Index);
            string AS3package = null;
            Match mPackage = re_AS3package.Match(preClassSrc);
            if (mPackage.Success)
            {
                aClass.IsAS3 = true;
                AS3package = mPackage.Groups["name"].Value;
                if (AS3package != null && AS3package.Length > 0)
                    aClass.ClassName = AS3package+"."+aClass.ClassName;
            }
            else aClass.IsAS3 = false;
            
            // check classname
            int p = aClass.ClassName.LastIndexOf(".");
            string constructor = (p >= 0) ? aClass.ClassName.Substring(p+1) : aClass.ClassName;
            string classType = mClass.Groups["ctype"].Value;
            if (src.Substring(0, mClass.Groups["cname"].Index).IndexOf(" comment0 ") >= 0)
                aClass.Comments = comments[0];

            // class base path
            bool validClassFile = true;
            int basepos = aClass.FileName.LastIndexOf( aClass.ClassName.Replace('.', Path.DirectorySeparatorChar)+".as" );
            if (basepos < 0)
            {
                // this class name & file don't match, it can lead to dangerous errors
                validClassFile = false;
                // warm about the misspelled class name
                if (!aClass.FileName.EndsWith("/"+constructor+".as") 
                     || !aClass.FileName.ToUpper().EndsWith("\\"+aClass.ClassName.ToUpper().Replace('.','\\')+".AS"))
                {
                    if (prevCName != aClass.ClassName)
                    {
                        string msg = String.Format("The {0} '{1}' does not match the file name:\n{2}",
                                                   classType,
                                                   aClass.ClassName,
                                                   aClass.FileName);
                        context.DisplayError(msg);
                    }
                }
                aClass.BasePath = System.IO.Path.GetDirectoryName(aClass.FileName)+"\\";
            }
            else
            {
                aClass.BasePath = aClass.FileName.Substring(0, basepos);
            }
            
            // add to classpath
            context.SetTemporaryBasePath(aClass.FileName, aClass.BasePath);

            // class flag
            aClass.Flags = FlagType.Class;
            if (classType == "interface") aClass.Flags |= FlagType.Interface;
            if (mClass.Groups["keys"].Value.IndexOf("intrinsic") >= 0) aClass.Flags |= FlagType.Intrinsic;
            if (mClass.Groups["keys"].Value.IndexOf("dynamic") >= 0) aClass.Flags |= FlagType.Dynamic;

            // import statements
            ParseImports(preClassSrc, aClass);
            preClassSrc = null;

            // inheritance
            string herit = mClass.Groups["herit"].Value;
            Match mExtends = re_extends.Match(herit);
            string extends = (validClassFile && mExtends.Success) ? mExtends.Groups["cname"].ToString() : "Object";
            if ((extends != aClass.ClassName) && (aClass.ClassName != "TopLevel"))
            {
                aClass.Extends = null;
                // resolve extended class
                ASClass extendsClass = context.GetClassByName(extends, aClass);
                // detect infinite extension loop
                ASClass tmpClass = extendsClass;
                while (tmpClass != null)
                {
                    if (tmpClass == aClass)
                    {
                        string msg = String.Format("The {0} '{1}' extends itself.",
                                                   classType,
                                                   aClass.ClassName);
                        context.DisplayError(msg);
                        extendsClass = null;
                        break;
                    }
                    tmpClass = tmpClass.Extends;
                }
                if (extendsClass != null) aClass.Extends = extendsClass;
                else aClass.Extends = new ASClass();
            }
            
            Match mImplements = re_implements.Match(herit);
            if (mImplements.Success)
            {
                string implements;
                if (!mExtends.Success || mImplements.Index > mExtends.Index)
                    implements = herit.Substring(mImplements.Index+mImplements.Length).Trim();
                else 
                    implements = herit.Substring(mImplements.Index+mImplements.Length, mExtends.Index-mImplements.Index-mImplements.Length).Trim();
                aClass.Implements = re_parametersSeparator.Replace(implements, ", ");
            }
            else aClass.Implements = null;
            
            // clean class body
            src = "; "+src.Substring(mClass.Groups["herit"].Index + mClass.Groups["herit"].Value.Length+1);
            src = re_balancedBraces.Replace(src, ";");

            // if updating, clear
            aClass.Methods.Clear();
            aClass.Properties.Clear();
            aClass.Vars.Clear();

            // parse functions
            string keys;
            bool isStatic;
            MatchCollection mcFunc = re_functions.Matches(src);
            Match mFunc;
            Match mType;
            Match mComments;
            ASMember member;
            foreach(Match m in mcFunc)
            {
                mFunc = re_splitFunction.Match(m.Value);
                if (!mFunc.Success) continue;
                // keywords
                keys = mFunc.Groups["keys"].Value;
                member = new ASMember();
                member.Flags = FlagType.Function;
                if (keys.IndexOf("private") >= 0) member.Flags |= FlagType.Private;
                else member.Flags |= FlagType.Public;
                isStatic = (keys.IndexOf("static") >= 0);
                if (isStatic) member.Flags |= FlagType.Static;
                else member.Flags |= FlagType.Dynamic;
                // comments
                if (comments.Count > 0)
                {
                    mComments = re_commentIndex.Match(keys);
                    if (mComments.Success) {
                        member.Comments = comments[ Convert.ToInt16(mComments.Groups["index"].Value) ];
                    }
                }
                // method
                member.Name = mFunc.Groups["fname"].Value.Trim();
                if (member.Name.Length == 0)
                    continue;
                // parameters
                member.Parameters = re_colonParams.Replace( re_parametersSeparator.Replace(mFunc.Groups["params"].Value.Trim(), ", "), ":");
                // return type
                mType = re_variableType.Match(mFunc.Groups["type"].Value);
                if (mType.Success) member.Type = mType.Groups["type"].Value;
                else member.Type = "";
                // constructor type
                if (member.Name == constructor)
                {
                    member.Flags |= FlagType.Constructor;
                    member.Type = constructor;
                }

                // getter/setter
                if ((member.Name.Length > 4) && ((int)member.Name[3] < 33))
                {
                    Match mProp = re_isGetterSetter.Match(member.Name);
                    if (mProp.Success)
                    {
                        string pname = mProp.Groups["pname"].Value;
                        ASMember prop = aClass.Properties.Search(pname, 0);
                        if (prop == null)
                        {
                            prop = member;
                            prop.Name = pname;
                            prop.Flags -= FlagType.Function;
                            aClass.Properties.Add(prop);
                        }
                        if (mProp.Groups["type"].Value == "g")
                        {
                            prop.Flags |= FlagType.Getter;
                            prop.Type = member.Type;
                            if (!mType.Success) prop.Type = "Object";
                        }
                        else
                        {
                            prop.Flags |= FlagType.Setter;
                            prop.Parameters = member.Parameters;
                        }
                        if ((member.Comments != null) && 
                            ((prop.Comments == null) || (prop.Comments.Length < member.Comments.Length)))
                            prop.Comments = member.Comments;
                    }
                    // store method
                    else aClass.Methods.Add(member);
                }
                // store method
                else aClass.Methods.Add(member);
            }

            // parse variables
            MatchCollection mcVars = re_variable.Matches(src);
            Match mVar;
            foreach(Match m in mcVars)
            {
                mVar = re_splitVariable.Match(m.Value);
                if (!mVar.Success) continue;
                // parse method definition
                keys = mVar.Groups["keys"].Value;
                member = new ASMember();
                member.Flags = FlagType.Variable;
                // keywords
                if (keys.IndexOf("private") >= 0) member.Flags |= FlagType.Private;
                else member.Flags |= FlagType.Public;
                isStatic = (keys.IndexOf("static") >= 0);
                if (isStatic) member.Flags |= FlagType.Static;
                else member.Flags |= FlagType.Dynamic;
                // comments
                mComments = re_commentIndex.Match(keys);
                if (mComments.Success)
                    member.Comments = comments[ Convert.ToInt16(mComments.Groups["index"].Value) ];
                // name
                member.Name = mVar.Groups["pname"].Value;
                // type
                mType = re_variableType.Match(mVar.Groups["type"].Value);
                if (mType.Success)
                    member.Type = mType.Groups["type"].Value;
                else member.Type = "Object";
                // store
                aClass.Vars.Add(member);
            }
            
            // HACK AS3 'const' declarations
            if (AS3package != null)
            {
                mcVars = re_constant.Matches(src);
                foreach(Match m in mcVars)
                {
                    mVar = re_splitConstant.Match(m.Value);
                    if (!mVar.Success) continue;
                    // parse method definition
                    keys = mVar.Groups["keys"].Value;
                    member = new ASMember();
                    member.Flags = FlagType.Variable;
                    // keywords
                    if (keys.IndexOf("private") >= 0) member.Flags |= FlagType.Private;
                    else member.Flags |= FlagType.Public;
                    isStatic = (keys.IndexOf("static") >= 0);
                    if (isStatic) member.Flags |= FlagType.Static;
                    else member.Flags |= FlagType.Dynamic;
                    // comments
                    mComments = re_commentIndex.Match(keys);
                    if (mComments.Success)
                        member.Comments = comments[ Convert.ToInt16(mComments.Groups["index"].Value) ];
                    // name
                    member.Name = mVar.Groups["pname"].Value;
                    // type
                    mType = re_variableType.Match(mVar.Groups["type"].Value);
                    if (mType.Success)
                        member.Type = mType.Groups["type"].Value;
                    else member.Type = "Object";
                    // store
                    aClass.Vars.Add(member);
                }
            }

            // is also a package?
            //DebugConsole.Trace("check folder "+aClass.FileName.Substring(0, aClass.FileName.Length-3));
            if (System.IO.Directory.Exists(aClass.FileName.Substring(0, aClass.FileName.Length-3)))
            {
                string package = aClass.FileName.Substring(aClass.BasePath.Length);
                package = package.Substring(0, package.IndexOf('.'));
                ASMemberList pList = context.GetSubClasses(package);
                if ((pList != null) && (pList.Count > 0))
                {
                    //DebugConsole.Trace("Sub classes/packages "+package+" "+pList.Count);
                    aClass.Flags |= FlagType.Package;
                    aClass.Package = pList;
                    // if intrinsic class, inherit flag
                    if ((aClass.Flags & FlagType.Intrinsic) == FlagType.Intrinsic)
                    foreach(ASMember import in pList)
                        import.Flags |= FlagType.Intrinsic;
                }
            }

            // done
        }
        #endregion

        #region tools_functions
        /// <summary>
        /// Remove comments from the Class' source
        /// </summary>
        /// <param name="filename">Original Class source</param>
        /// <returns>Class source without comments</returns>
        static public string CleanClassSource(string src, StringCollection comments)
        {
            int len = src.Length-1;
            if (len <= 0) 
                return src;
            char[] ba = src.ToCharArray();
            int i = 0;
            char c1;
            char c2;
            int matching = 0;
            bool addText = true;
            bool addComment = false;
            int inString = 0;
            StringBuilder sb = new StringBuilder(len);
            StringBuilder comment = new StringBuilder();
            while (i < len-1) {
                // next chars
                c1 = ba[i++];
                // match
                switch (matching) {
                    // look for comment block/line
                    case 0:
                        if (inString == 0) 
                        {
                            // new comment
                            if (c1 == '/')
                            {
                                c2 = ba[i];
                                if (c2 == '/') {
                                    matching = 1;
                                    addText = false;
                                    i++;
                                    continue;
                                }
                                else if (c2 == '*') {
                                    matching = 2;
                                    addText = false;
                                    if (i < len-1 && ba[i+1] == '*' && ba[i+2] != '/') {
                                        addComment = (comments != null);
                                        i++;
                                    }
                                    i++;
                                    continue;
                                }
                            }
                            // don't look for comments in strings
                            else if (c1 == '"') inString = 1;
                            else if (c1 == '\'') inString = 2;
                        }
                        // end of string
                        else if ((inString == 1) && (c1 == '"')) inString = 0;
                        else if ((inString == 2) && (c1 == '\'')) inString = 0;
                        break;
                    case 1:
                        if (c1 == 10 || c1 == 13) {
                            addText = true;
                            comment = new StringBuilder();
                            matching = 0;
                        }
                        break;
                    case 2:
                        if (c1 == '*') 
                        {
                            c2 = ba[i];
                            if (c2 == '/') {
                                string temp = comment.ToString();
                                // extract doc comments
                                if (addComment && (comment.Length > 0)) 
                                {
                                    sb.Append(" comment").Append(comments.Count).Append(' ');
                                    comments.Add(temp);
                                }
                                // TODO  ASClassParser: parse for TODO statements!
                                comment = new StringBuilder();
                                addText = true;
                                addComment = false;
                                matching = 0;
                                i++;
                                continue;
                            }
                        }
                        break;
                }
                // add char
                if (addText) sb.Append(c1);
                else comment.Append(c1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parse import statements in source
        /// </summary>
        /// <param name="src">Class source</param>
        /// <param name="aClass">Class object to update</param>
        static private void ParseImports(string src, ASClass aClass)
        {
            aClass.Imports.Clear();
            src = src.Replace('\r', '\n'); // fix .NET Regex line-ends detection
            MatchCollection mcImports = re_import.Matches(src);
            if (mcImports.Count > 0)
            {
                ArrayList known = new ArrayList();
                string package;
                string cname;
                ASMember newImport;
                foreach(Match mImport in mcImports)
                {
                    package = mImport.Groups["package"].Value;
                    //DebugConsole.Trace("IMPORT '"+package+"'");
                    int p = package.LastIndexOf(".");
                    cname = (p >= 0) ? package.Substring(p+1) : package;
                    // resolve wildcard
                    if (cname.Length == 0)
                    {
                        context.ResolveWildcards(package, aClass, known);
                    }
                    else if (!known.Contains(package))
                    {
                        known.Add(package);
                        newImport = new ASMember();
                        newImport.Name = cname;
                        newImport.Type = package;
                        aClass.Imports.Add(newImport);
                    }
                }
            }
            //else DebugConsole.Trace("NO IMPORTS");
        }
        #endregion
        
    }

}
