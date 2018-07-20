using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Controls;
using ASCompletion.Context;
using ASCompletion.Completion;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore.Localization;
using PluginCore.Helpers;

namespace UnityContext
{
    /// <summary>
    /// Unity context
    /// </summary>
    public class Context: AS2Context.Context
    {
        #region regular_expressions_definitions
        static readonly protected Regex re_genericType =
            new Regex("(?<gen>[^<]+)\\.<(?<type>.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region initialization

        private UnitySettings unitysettings;

        public override IContextSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// Do not call directly
        /// </summary>
        protected Context()
        {
        }

        public Context(UnitySettings initSettings)
        {
            unitysettings = initSettings;

            /* AS-LIKE OPTIONS */

            hasLevels = true;
            docType = "MonoBehaviour";

            /* DESCRIBE LANGUAGE FEATURES */

            // language constructs
            features.hasPackages = false;
            features.hasImports = true;
            features.hasImportsWildcard = false;
            features.hasClasses = true;
            features.hasMultipleDefs = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = true;
            features.hasGenerics = true;
            features.hasEcmaTyping = true;
            features.hasCArrays = true;
            features.hasDirectives = true;
            features.hasVars = true;
            features.hasConsts = false;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasTryCatch = true;
            features.hasStaticInheritance = true;
            features.checkFileName = false;

            // allowed declarations access modifiers
            features.classModifiers = Visibility.Public | Visibility.Private;
            features.enumModifiers = Visibility.Public | Visibility.Private;
            features.varModifiers = Visibility.Public | Visibility.Private;
            features.methodModifiers = Visibility.Public | Visibility.Private;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Public;
            features.enumModifierDefault = Visibility.Public;
            features.varModifierDefault = Visibility.Public;
            features.methodModifierDefault = Visibility.Public;

            // keywords
            features.dot = ".";
            features.voidKey = "void";
            features.objectKey = "Object";
            features.booleanKey = "boolean";
            features.numberKey = "float";
            features.arrayKey = "Array";
            features.importKey = "import";
            features.CArrayTemplate = "BuiltInArray";
            features.typesPreKeys = new string[] { "import", "new", "instanceof", "extends", "implements" };
            features.codeKeywords = new string[] { 
                "class", "interface", "var", "function", "new", "delete", "instanceof", "return", "break", "continue",
                "if", "else", "for", "in", "while", "do", "switch", "case", "default", "with",
                "null", "undefined", "true", "false", "try", "catch", "finally", "throw"
            };
            features.varKey = "var";
            features.functionKey = "function";
            features.getKey = "get";
            features.setKey = "set";
            features.staticKey = "static";
            features.overrideKey = "override";
            features.publicKey = "public";
            features.privateKey = "private";
            features.intrinsicKey = "intrinsic";

            features.functionArguments = new MemberModel("arguments", "FunctionArguments", FlagType.Variable | FlagType.LocalVar, 0);

            /* INITIALIZATION */

            settings = initSettings;
            //BuildClassPath(); // defered to first use
        }
        #endregion
        
        #region classpath management
        /// <summary>
        /// Classpathes & classes cache initialisation
        /// </summary>
        public override void BuildClassPath()
        {
            ReleaseClasspath();
            started = true;
            if (unitysettings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "Unity3D";
                contextSetup.Version = unitysettings.DefaultVersion;
            }

            // external version definition
            platform = contextSetup.Platform;
            majorVersion = 0;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);
            
            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            
            // SDK
            string sdkPath = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(unitysettings.GetDefaultSDK().Path);
            //TODO?

            // add external pathes
            List<PathModel> initCP = classPath;
            classPath = new List<PathModel>();
            if (contextSetup.Classpath != null)
            {
                foreach (string cpath in contextSetup.Classpath) 
                    AddPath(cpath.Trim());
            }

            // add library
            AddPath(Path.Combine(PathHelper.LibraryDir, "UNITYSCRIPT/Classes"));

            // add user pathes from settings
            if (settings.UserClasspath != null && settings.UserClasspath.Length > 0)
            {
                foreach(string cpath in settings.UserClasspath) AddPath(cpath.Trim());
            }
            // add initial pathes
            foreach(PathModel mpath in initCP) AddPath(mpath);

            // parse top-level elements
            InitTopLevelElements();
            if (cFile != null) UpdateTopLevelElements();
            
            // add current temporaty path
            if (temporaryPath != null)
            {
                string tempPath = temporaryPath;
                temporaryPath = null;
                SetTemporaryPath(tempPath);
            }
            FinalizeClasspath();
        }
        #endregion

        #region class resolution

        /// <summary>
        /// Create a new file model using the default file parser
        /// </summary>
        /// <param name="filename">Full path</param>
        /// <returns>File model</returns>
        public override FileModel GetFileModel(string fileName)
        {
            FileModel nFile = base.GetFileModel(fileName);
            ScriptToClass(nFile);
            return nFile;
        }

        /// <summary>
        /// Retrieve a file model from the classpath cache
        /// </summary>
        /// <param name="fileName">Full path</param>
        /// <returns>File model</returns>
        public override FileModel GetCachedFileModel(string fileName)
        {
            FileModel nFile = base.GetCachedFileModel(fileName);
            ScriptToClass(nFile);
            return nFile;
        }

        /// <summary>
        /// Refresh the file model
        /// </summary>
        /// <param name="updateUI">Update outline view</param>
        public override void UpdateCurrentFile(bool updateUI)
        {
            if (cFile == null || CurSciControl == null)
                return;
            ASFileParser parser = new ASFileParser();
            parser.ParseSrc(cFile, CurSciControl.Text);
            ScriptToClass(cFile);
            cLine = CurSciControl.LineFromPosition(CurSciControl.CurrentPos);
            UpdateContext(cLine);

            // update outline
            if (updateUI) ASContext.Context = this;
        }

        private void ScriptToClass(FileModel nFile)
        {
            if (nFile.Classes.Count == 0 && nFile.Members.Count > 0)
            {
                ClassModel auto = new ClassModel();
                auto.LineFrom = nFile.Members[0].LineFrom;
                auto.LineTo = nFile.Members[nFile.Members.Count - 1].LineTo;
                auto.Flags = FlagType.Class;
                auto.InFile = nFile;
                auto.Access = Visibility.Public;
                auto.Name = Path.GetFileNameWithoutExtension(nFile.FileName);
                auto.ExtendsType = docType;
                auto.Members = nFile.Members;
                foreach (MemberModel member in auto.Members)
                {
                    member.IsPackageLevel = false;
                    member.InFile = null;
                }
                nFile.Members = new MemberList();
                nFile.Classes.Add(auto);
                nFile.Version = 2;
            }
        }

        /// <summary>
        /// Return imported classes list (not null)
        /// </summary>
        /// <param name="package">Package to explore</param>
        /// <param name="inFile">Current file</param>
        public override MemberList ResolveImports(FileModel inFile)
        {
            bool filterImports = (inFile == cFile) && inFile.Classes.Count > 1;
            int lineMin = (filterImports && inPrivateSection) ? inFile.PrivateSectionIndex : 0;
            int lineMax = (filterImports && inPrivateSection) ? int.MaxValue : inFile.PrivateSectionIndex;
            MemberList imports = new MemberList();
            foreach (MemberModel item in inFile.Imports)
            {
                if (filterImports && (item.LineFrom < lineMin || item.LineFrom > lineMax)) continue;
                ClassModel type = ResolveType(item.Type, null);
                if (!type.IsVoid()) imports.Add(type);
                else
                {
                    // package-level declarations
                    FileModel matches = ResolvePackage(item.Type, false);
                    if (matches != null)
                    {
                        foreach (MemberModel import in matches.Imports)
                            imports.Add(import);
                        foreach (MemberModel member in matches.Members)
                            imports.Add(member);
                    }
                }
            }
            return imports;
        }

        /// <summary>
        /// Check if a type is already in the file's imports
        /// Throws an Exception if the type name is ambiguous 
        /// (ie. same name as an existing import located in another package)
        /// </summary>
        /// <param name="member">Element to search in imports</param>
        /// <param name="atLine">Position in the file</param>
        public override bool IsImported(MemberModel member, int atLine)
        {
            FileModel cFile = ASContext.Context.CurrentModel;
            string fullName = member.Type;
            foreach (MemberModel import in cFile.Imports)
            {
                if (fullName.StartsWith(import.Type + "."))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Top-level elements lookup
        /// </summary>
        /// <param name="token">Element to search</param>
        /// <param name="result">Response structure</param>
        public override void ResolveTopLevelElement(string token, ASResult result)
        {
            if (topLevel != null && topLevel.Members.Count > 0)
            {
                // current class
                ClassModel inClass = ASContext.Context.CurrentClass;
                if (token == "this")
                {
                    result.Member = topLevel.Members.Search("this", 0, 0);
                    if (inClass.IsVoid()) 
                        inClass = ASContext.Context.ResolveType(result.Member.Type, null);
                    result.Type = inClass;
                    result.InFile = ASContext.Context.CurrentModel;
                    return;
                }
                else if (token == "super")
                {
                    if (inClass.IsVoid())
                    {
                        MemberModel thisMember = topLevel.Members.Search("this", 0, 0);
                        inClass = ASContext.Context.ResolveType(thisMember.Type, null);
                    }
                    inClass.ResolveExtends();
                    ClassModel extends = inClass.Extends;
                    if (!extends.IsVoid())
                    {
                        result.Member = topLevel.Members.Search("super", 0, 0);
                        result.Type = extends;
                        result.InFile = extends.InFile;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a class model from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inClass">Current file</param>
        /// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public override ClassModel ResolveType(string cname, FileModel inFile)
        {
            // handle generic types
            if (cname != null && cname.IndexOf('<') > 0)
            {
                Match genType = re_genericType.Match(cname);
                if (genType.Success)
                    return ResolveGenericType(genType.Groups["gen"].Value + ".<T>", genType.Groups["type"].Value, inFile);
                else return ClassModel.VoidClass;
            }
            return base.ResolveType(cname, inFile);
        }

        /// <summary>
        /// Retrieve/build typed copies of generic types
        /// </summary>
        private ClassModel ResolveGenericType(string baseType, string indexType, FileModel inFile)
        {
            ClassModel originalClass = base.ResolveType(baseType, inFile);
            if (originalClass.IsVoid()) return originalClass;

            ClassModel indexClass = ResolveType(indexType, inFile);
            if (indexClass.IsVoid()) return originalClass;
            indexType = indexClass.QualifiedName;

            FileModel aFile = originalClass.InFile;
            // is the type already cloned?
            foreach (ClassModel otherClass in aFile.Classes)
                if (otherClass.IndexType == indexType) return otherClass;

            // clone the type
            ClassModel aClass = originalClass.Clone() as ClassModel;

            aClass.Name = baseType + "@" + indexType;
            aClass.IndexType = indexType;

            string typed = "<" + indexType + ">";
            foreach (MemberModel member in aClass.Members)
            {
                if (member.Name == baseType) member.Name = baseType.Replace("<T>", typed);
                if (member.Type != null && member.Type.IndexOf('T') >= 0)
                {
                    if (member.Type == "T") member.Type = indexType;
                    else member.Type = member.Type.Replace("<T>", typed);
                }
                if (member.Parameters != null)
                {
                    foreach (MemberModel param in member.Parameters)
                    {
                        if (param.Type != null && param.Type.IndexOf('T') >= 0)
                        {
                            if (param.Type == "T") param.Type = indexType;
                            else param.Type = param.Type.Replace("<T>", typed);
                        }
                    }
                }
            }

            aFile.Classes.Add(aClass);
            return aClass;
        }

        /// <summary>
        /// Update Flash intrinsic known vars
        /// </summary>
        protected override void UpdateTopLevelElements()
        {
            MemberModel special;
            special = topLevel.Members.Search("this", 0, 0);
            if (special != null)
            {
                if (!cClass.IsVoid()) special.Type = cClass.QualifiedName;
                else special.Type = (cFile.Version > 1) ? features.voidKey : docType;
            }
            special = topLevel.Members.Search("super", 0, 0);
            if (special != null) 
            {
                cClass.ResolveExtends();
                ClassModel extends = cClass.Extends;
                if (!extends.IsVoid()) special.Type = extends.QualifiedName;
                else special.Type = (cFile.Version > 1) ? features.voidKey : features.objectKey;
            }
        }
        
        /// <summary>
        /// Prepare JS intrinsic known vars/methods/classes
        /// </summary>
        protected override void InitTopLevelElements()
        {
            string filename = "toplevel.js";
            topLevel = new FileModel(filename);

            // search top-level declaration
            foreach(PathModel aPath in classPath)
            if (File.Exists(Path.Combine(aPath.Path, filename)))
            {
                filename = Path.Combine(aPath.Path, filename);
                topLevel = GetCachedFileModel(filename);
                break;
            }

            if (File.Exists(filename))
            {
            }
            // not found
            else
            {
                //ErrorHandler.ShowInfo("Top-level elements class not found. Please check your Program Settings.");
            }
            if (topLevel.Members.Search("this", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("this", "", FlagType.Variable, Visibility.Public));
            if (topLevel.Members.Search("super", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("super", "", FlagType.Variable, Visibility.Public));
            if (topLevel.Members.Search(features.voidKey, 0, 0) == null)
                topLevel.Members.Add(new MemberModel(features.voidKey, "", FlagType.Class | FlagType.Intrinsic, Visibility.Public));
            topLevel.Members.Sort();
            foreach (MemberModel member in topLevel.Members)
                member.Flags |= FlagType.Intrinsic;
        }

        /// <summary>
        /// Return the full project classes list
        /// </summary>
        /// <returns></returns>
        public override MemberList GetAllProjectClasses()
        {
            // from cache
            if (!completionCache.IsDirty && completionCache.AllTypes != null)
                return completionCache.AllTypes;

            MemberList fullList = new MemberList();
            MemberModel item;
            // public classes
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
            {
                aPath.ForeachFile((aFile) =>
                {
                    foreach(ClassModel aClass in aFile.Classes)
                        if (!aClass.IsVoid() && aClass.IndexType == null && aClass.Access == Visibility.Public)
                        {
                            item = aClass.ToMemberModel();
                            item.Name = item.Type;
                            fullList.Add(item);
                        }
                    return true;
                });
            }
            // void
            fullList.Add(new MemberModel(features.voidKey, features.voidKey, FlagType.Class | FlagType.Intrinsic, 0));

            // in cache
            fullList.Sort();
            completionCache.AllTypes = fullList;
            return fullList;
        }
        #endregion
        
        #region command line compiler

        override public bool CanBuild
        {
            get { return cFile != null && cFile != FileModel.Ignore; }
        }

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath()
        {
            if (unitysettings != null) return unitysettings.GetDefaultSDK().Path;
            else return null;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (unitysettings == null)
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.FeatureMissing"));
                return;
            }
        }

        /// <summary>
        /// Run MTASC compiler in the current class's base folder with current classpath
        /// </summary>
        /// <param name="append">Additional comiler switches</param>
        public override void RunCMD(string append)
        {
            /*if (unitysettings == null)
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.FeatureMissing"));
                return;
            }

            if (!IsFileValid || !File.Exists(CurrentFile))
                return;
            if (CurrentModel.Version != 2)
            {
                MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidClass"));
                return;
            }

            string mtascPath = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(unitysettings.GetDefaultSDK().Path);

            if (!Directory.Exists(mtascPath) && !File.Exists(mtascPath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidMtascPath"));
                return;
            }
            
            SetStatusText(settings.CheckSyntaxRunning);
            
            try 
            {
                // save modified files if needed
                if (outputFile != null) MainForm.CallCommand("SaveAllModified", null);
                else MainForm.CallCommand("SaveAllModified", ".as");
                
                // prepare command
                string command = mtascPath;
                if (Path.GetExtension(command) == "") command = Path.Combine(command, "mtasc.exe");
                else mtascPath = Path.GetDirectoryName(mtascPath);

                command += ";\"" + CurrentFile + "\"";
                if (append == null || append.IndexOf("-swf-version") < 0)
                    command += " -version "+majorVersion;
                // classpathes
                foreach(PathModel aPath in classPath)
                if (aPath.Path != temporaryPath
                    && !aPath.Path.StartsWith(mtascPath, StringComparison.OrdinalIgnoreCase))
                    command += " -cp \"" + aPath.Path.TrimEnd('\\') + "\"";
                
                // run
                string filePath = NormalizePath(cFile.BasePath);
                if (PluginBase.CurrentProject != null)
                    filePath = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath); 
                string workDir = MainForm.WorkingDirectory;
                MainForm.WorkingDirectory = filePath;
                MainForm.CallCommand("RunProcessCaptured", command+" "+append);
                MainForm.WorkingDirectory = workDir;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }*/
        }
        
        /// <summary>
        /// Calls RunCMD with additional parameters taken from the classes @mtasc doc tag
        /// </summary>
        public override bool BuildCMD(bool failSilently)
        {
            return false;
            /*
            if (unitysettings == null)
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.FeatureMissing"));
                return false;
            }

            if (!File.Exists(CurrentFile)) 
                return false;
            // check if @mtasc is defined
            Match mCmd = null;
            ClassModel cClass = cFile.GetPublicClass();
            if (IsFileValid && cClass.Comments != null)
                mCmd = re_CMD_BuildCommand.Match(cClass.Comments);
            
            if (CurrentModel.Version != 2 || mCmd == null || !mCmd.Success) 
            {
                if (!failSilently)
                {
                    MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidForQuickBuild"));
                }
                return false;
            }
            
            // build command
            string command = mCmd.Groups["params"].Value.Trim();
            try
            {
                command = Regex.Replace(command, "[\\r\\n]\\s*\\*", "", RegexOptions.Singleline);
                command = " " + MainForm.ProcessArgString(command) + " ";
                if (command == null || command.Length == 0)
                {
                    if (!failSilently)
                        throw new Exception(TextHelper.GetString("Info.InvalidQuickBuildCommand"));
                    return false;
                }
                outputFile = null;
                outputFileDetails = "";
                trustFileWanted = false;

                // get SWF url
                MatchCollection mPar = re_SplitParams.Matches(command + "-eof");
                int mPlayIndex = -1;
                bool noPlay = false;
                if (mPar.Count > 0)
                {
                    string op;
                    for (int i = 0; i < mPar.Count; i++)
                    {
                        op = mPar[i].Groups["switch"].Value;
                        int start = mPar[i].Index + mPar[i].Length;
                        int end = (mPar.Count > i + 1) ? mPar[i + 1].Index : start;
                        if ((op == "-swf") && (outputFile == null) && (mPlayIndex < 0))
                        {
                            if (end > start)
                                outputFile = command.Substring(start, end - start).Trim();
                        }
                        else if ((op == "-out") && (mPlayIndex < 0))
                        {
                            if (end > start)
                                outputFile = command.Substring(start, end - start).Trim();
                        }
                        else if (op == "-header")
                        {
                            if (end > start)
                            {
                                string[] dims = command.Substring(start, end - start).Trim().Split(':');
                                if (dims.Length > 2) outputFileDetails = ";" + dims[0] + ";" + dims[1];
                            }
                        }
                        else if (op == "-play")
                        {
                            if (end > start)
                            {
                                mPlayIndex = i;
                                outputFile = command.Substring(start, end - start).Trim();
                            }
                        }
                        else if (op == "-trust")
                        {
                            trustFileWanted = true;
                        }
                        else if (op == "-noplay")
                        {
                            noPlay = true;
                        }
                    }
                }
                if (outputFile.Length == 0) outputFile = null;

                // cleaning custom switches
                if (mPlayIndex >= 0)
                {
                    command = command.Substring(0, mPar[mPlayIndex].Index) + command.Substring(mPar[mPlayIndex + 1].Index);
                }
                if (trustFileWanted)
                {
                    command = command.Replace("-trust", "");
                }
                if (noPlay || !settings.PlayAfterBuild)
                {
                    command = command.Replace("-noplay", "");
                    outputFile = null;
                    runAfterBuild = false;
                }
                else runAfterBuild = (outputFile != null);

                // fixing output path
                if (runAfterBuild)
                {
                    if (!Path.IsPathRooted(outputFile))
                    {
                        string filePath = NormalizePath(cFile.BasePath);
                        if (PluginBase.CurrentProject != null)
                            filePath = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
                        outputFile = Path.Combine(filePath, outputFile);
                    }
                    string outputPath = Path.GetDirectoryName(outputFile);

                    if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return false;
            }
            
            // run
            RunCMD(command);
            return true;*/
        }
        #endregion
    }
}
