using System;
using System.Collections.Generic;
using System.Text;
using ASCompletion.Model;
using ASCompletion.Context;
using PluginCore;
using System.IO;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;
using System.Text.RegularExpressions;
using ASCompletion.Completion;

namespace LoomContext
{
    public class Context: AS3Context.Context
    {
        #region initialization
        LoomSettings loomSettings;

        public Context(LoomSettings initSettings)
        {
            loomSettings = initSettings;

            /* DESCRIBE LANGUAGE FEATURES */

            mxmlEnabled = false;

            // language constructs
            features.hasPackages = true;
            features.hasNamespaces = false;
            features.hasImports = true;
            features.hasImportsWildcard = true;
            features.hasClasses = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = true;
            features.hasStructs = true;
            features.hasDelegates = true;
            features.hasGenerics = true;
            features.hasEcmaTyping = true;
            features.hasInference = true;
            features.hasVars = true;
            features.hasConsts = true;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasOverride = true;
            features.hasTryCatch = true;
            features.hasE4X = false;
            features.hasStaticInheritance = true;
            features.checkFileName = false;
            features.hasMultipleDefs = true;

            // allowed declarations access modifiers
            Visibility all = Visibility.Public | Visibility.Internal | Visibility.Protected | Visibility.Private;
            features.classModifiers = all;
            features.varModifiers = all;
            features.constModifiers = all;
            features.methodModifiers = all;
            features.enumModifiers = all;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Internal;
            features.varModifierDefault = Visibility.Internal;
            features.methodModifierDefault = Visibility.Internal;
            features.enumModifierDefault = Visibility.Internal;

            // keywords
            features.dot = ".";
            features.voidKey = "void";
            features.objectKey = "Object";
            features.booleanKey = "Boolean";
            features.numberKey = "Number";
            features.arrayKey = "Array";
            features.importKey = "import";
            features.typesPreKeys = new string[] { "import", "new", "typeof", "is", "as", "extends", "implements" };
            features.codeKeywords = new string[] { 
                "var", "function", "const", "new", "delete", "typeof", "is", "as", "return", 
                "break", "continue", "if", "else", "for", "each", "in", "while", "do", "switch", "case", "default", "with",
                "null", "true", "false", "try", "catch", "finally", "throw"
            };
            features.accessKeywords = new string[] { 
                "extern", "dynamic", "inline", "final", "public", "private", "protected", "internal", "static", "override"
            };
            features.declKeywords = new string[] { "var", "function", "const", "get", "set" };
            features.typesKeywords = new string[] { "import", "class", "interface", "delegate", "struct", "enum" };
            features.varKey = "var";
            features.constKey = "const";
            features.functionKey = "function";
            features.getKey = "get";
            features.setKey = "set";
            features.staticKey = "static";
            features.finalKey = "final";
            features.overrideKey = "override";
            features.publicKey = "public";
            features.internalKey = "internal";
            features.protectedKey = "protected";
            features.privateKey = "private";
            features.intrinsicKey = "native";

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
            if (loomSettings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "loom";
                contextSetup.Version = "1.0";
            }

            // external version definition
            platform = contextSetup.Platform;
            majorVersion = 1;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);

            string cpCheck = contextSetup.Classpath != null ?
                String.Join(";", contextSetup.Classpath).Replace('\\', '/') : "";

            //
            // Class pathes
            //
            classPath = new List<PathModel>();

            // SDK
            string compiler = PluginBase.CurrentProject != null && PluginBase.CurrentProject.CurrentSDK != null
                ? PluginBase.CurrentProject.CurrentSDK
                : loomSettings.GetDefaultSDK().Path;

            char S = Path.DirectorySeparatorChar;
            string sdkLibs = compiler + S + "libs";

            if (majorVersion > 0 && !String.IsNullOrEmpty(sdkLibs) && Directory.Exists(sdkLibs))
            {
                foreach (string loomlib in Directory.GetFiles(sdkLibs, "*.loomlib"))
                    AddPath(loomlib);
            }

            // add external pathes
            List<PathModel> initCP = classPath;
            classPath = new List<PathModel>();
            if (contextSetup.Classpath != null)
            {
                foreach (string cpath in contextSetup.Classpath)
                    AddPath(cpath.Trim());
            }

            // add library
            AddPath(PathHelper.LibraryDir + S + "LOOM" + S + "classes");
            // add user pathes from settings
            if (settings.UserClasspath != null && settings.UserClasspath.Length > 0)
            {
                foreach (string cpath in settings.UserClasspath) AddPath(cpath.Trim());
            }

            // add initial pathes
            foreach (PathModel mpath in initCP) AddPath(mpath);

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

        /// <summary>
        /// Build a list of file mask to explore the classpath
        /// </summary>
        public override string[] GetExplorerMask()
        {
            return new string[] { "*.ls" };
        }

        /// <summary>
        /// Parse a packaged library file
        /// </summary>
        /// <param name="path">Models owner</param>
        public override void ExploreVirtualPath(PathModel path)
        {
            if (path.WasExplored)
            {
                if (path.FilesCount != 0) // already parsed
                    return;
            }

            try
            {
                if (File.Exists(path.Path) && !path.WasExplored)
                {
                    bool isRefresh = path.FilesCount > 0;
                    //TraceManager.AddAsync("parse " + path.Path);
                    lock (path)
                    {
                        path.WasExplored = true;
                        // PARSE LOOMLIB
                        LibParser.Parse(path, this);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = TextHelper.GetString("AS3Context.Info.ExceptionWhileParsing");
                TraceManager.AddAsync(message + " " + path.Path);
                TraceManager.AddAsync(ex.Message);
                TraceManager.AddAsync(ex.StackTrace);
            }
        }

        /// <summary>
        /// Prepare Loom intrinsic known vars/methods/classes
        /// </summary>
        protected override void InitTopLevelElements()
        {
            string filename = "toplevel.ls";
            topLevel = new FileModel(filename);

            if (topLevel.Members.Search("this", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("this", "", FlagType.Variable | FlagType.Intrinsic, Visibility.Public));
            if (topLevel.Members.Search("super", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("super", "", FlagType.Variable | FlagType.Intrinsic, Visibility.Public));
            if (topLevel.Members.Search(features.voidKey, 0, 0) == null)
                topLevel.Members.Add(new MemberModel(features.voidKey, "", FlagType.Intrinsic, Visibility.Public));
            topLevel.Members.Sort();
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
            // public & internal classes
            string package = CurrentModel.Package;
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
                {
                    aPath.ForeachFile((aFile) =>
                    {
                        if (!aFile.HasPackage)
                            return true; // skip

                        foreach(ClassModel aClass in aFile.Classes)
                            if (!aClass.IsVoid() && aClass.IndexType == null)
                            {
                                if (aClass.Access == Visibility.Public
                                    || (aClass.Access == Visibility.Internal && aFile.Package == package))
                                {
                                    item = aClass.ToMemberModel();
                                    item.Name = item.Type;
                                    fullList.Add(item);
                                }
                            }
                        if (aFile.Package.Length > 0 && aFile.Members.Count > 0)
                        {
                            foreach (MemberModel member in aFile.Members)
                            {
                                item = member.Clone() as MemberModel;
                                item.Name = aFile.Package + "." + item.Name;
                                fullList.Add(item);
                            }
                        }
                        else if (aFile.Members.Count > 0)
                        {
                            foreach (MemberModel member in aFile.Members)
                            {
                                item = member.Clone() as MemberModel;
                                fullList.Add(item);
                            }
                        }
                        return true;
                    });
                }
            // void
            fullList.Add(new MemberModel(features.voidKey, features.voidKey, FlagType.Class | FlagType.Intrinsic, 0));
            // private classes
            fullList.Add(GetPrivateClasses());

            // in cache
            fullList.Sort();
            completionCache.AllTypes = fullList;
            return fullList;
        }

        public override bool OnCompletionInsert(ScintillaNet.ScintillaControl sci, int position, string text, char trigger)
        {
            if (text == "Dictionary")
            {
                string insert = null;
                string line = sci.GetLine(sci.LineFromPosition(position));
                Match m = Regex.Match(line, @"\svar\s+(?<varname>.+)\s*:\s*Dictionary\.<(?<indextype>.+)(?=(>\s*=))");
                if (m.Success)
                {
                    insert = String.Format(".<{0}>", m.Groups["indextype"].Value);
                }
                else
                {
                    m = Regex.Match(line, @"\s*=\s*new");
                    if (m.Success)
                    {
                        ASResult result = ASComplete.GetExpressionType(sci, sci.PositionFromLine(sci.LineFromPosition(position)) + m.Index);
                        if (result != null && !result.IsNull() && result.Member != null && result.Member.Type != null)
                        {
                            m = Regex.Match(result.Member.Type, @"(?<=<).+(?=>)");
                            if (m.Success)
                            {
                                insert = String.Format(".<{0}>", m.Value);
                            }
                        }
                    }
                    if (insert == null)
                    {
                        if (trigger == '.' || trigger == '(') return true;
                        insert = ".<>";
                        sci.InsertText(position + text.Length, insert);
                        sci.CurrentPos = position + text.Length + 2;
                        sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                        ASComplete.HandleAllClassesCompletion(sci, "", false, true);
                        return true;
                    }
                }
                if (insert == null) return false;
                if (trigger == '.')
                {
                    sci.InsertText(position + text.Length, insert.Substring(1));
                    sci.CurrentPos = position + text.Length;
                }
                else
                {
                    sci.InsertText(position + text.Length, insert);
                    sci.CurrentPos = position + text.Length + insert.Length;
                }
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                return true;
            }

            return base.OnCompletionInsert(sci, position, text, trigger);
        }
        #endregion

        #region Command line compiler

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath()
        {
            return loomSettings.GetDefaultSDK().Path;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (IsFileValid && cFile.InlinedIn == null)
            {
                PluginBase.MainForm.CallCommand("Save", null);

                string sdk = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(loomSettings.GetDefaultSDK().Path);
                // TODO CheckSyntax
            }
        }

        /// <summary>
        /// Run Loom compiler in the current class's base folder with current classpath
        /// </summary>
        /// <param name="append">Additional comiler switches</param>
        public override void RunCMD(string append)
        {
            if (!IsCompilationTarget())
            {
                MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidClass"));
                return;
            }

            string command = (append ?? "") + " -- " + CurrentFile;
            // TODO RunCMD does it make sense?
        }

        private bool IsCompilationTarget()
        {
            return (!MainForm.CurrentDocument.IsUntitled && CurrentModel.Version >= 3);
        }

        /// <summary>
        /// Calls RunCMD with additional parameters taken from the classes @loom doc tag
        /// </summary>
        public override bool BuildCMD(bool failSilently)
        {
            if (!IsCompilationTarget())
            {
                MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidClass"));
                return false;
            }

            MainForm.CallCommand("SaveAllModified", null);

            string sdk = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : loomSettings.GetDefaultSDK().Path;
            // TODO BuildCMD does it make sense?
            return true;
        }
        #endregion

    }
}
