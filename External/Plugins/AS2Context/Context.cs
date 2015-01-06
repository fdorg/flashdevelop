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

namespace AS2Context
{
	/// <summary>
	/// Actionscript2 context
	/// </summary>
	public class Context: ASContext
	{
        #region regular_expressions_definitions
        static readonly protected Regex re_CMD_BuildCommand =
            new Regex("@mtasc[\\s]+(?<params>.*)", RegexOptions.Compiled | RegexOptions.Multiline);
        static readonly protected Regex re_SplitParams =
            new Regex("[\\s](?<switch>\\-[A-z]+)", RegexOptions.Compiled | RegexOptions.Singleline);
        static protected readonly Regex re_level =
            new Regex("^_level[0-9]+$", RegexOptions.Compiled | RegexOptions.Singleline);
        static protected readonly Regex re_token =
            new Regex("^[a-z$_][a-z0-9$_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static protected readonly Regex re_package =
            new Regex("^[a-z$_][a-z0-9$_.]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static protected readonly Regex re_lastDot =
            new Regex("\\.[^<]", RegexOptions.RightToLeft | RegexOptions.Compiled);
        #endregion

        #region initialization

        protected bool hasLevels = true;
        protected string docType;

        private AS2Settings as2settings;

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

		public Context(AS2Settings initSettings)
        {
            as2settings = initSettings;

            /* AS-LIKE OPTIONS */

            hasLevels = true;
            docType = "MovieClip";

            /* DESCRIBE LANGUAGE FEATURES */

            // language constructs
            features.hasImports = true;
            features.hasImportsWildcard = true;
            features.hasClasses = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = false;
            features.hasGenerics = false;
            features.hasEcmaTyping = true;
            features.hasVars = true;
            features.hasConsts = false;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasTryCatch = true;
            features.hasStaticInheritance = true;
            features.checkFileName = true;

            // allowed declarations access modifiers
            features.classModifiers = Visibility.Public | Visibility.Private;
            features.varModifiers = Visibility.Public | Visibility.Private;
            features.methodModifiers = Visibility.Public | Visibility.Private;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Public;
            features.varModifierDefault = Visibility.Public;
            features.methodModifierDefault = Visibility.Public;

            // keywords
            features.dot = ".";
            features.voidKey = "Void";
            features.objectKey = "Object";
            features.booleanKey = "Boolean";
            features.numberKey = "Number";
            features.arrayKey = "Array";
            features.importKey = "import";
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
            if (as2settings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "Flash Player";
                contextSetup.Version = as2settings.DefaultFlashVersion + ".0";
            }

			// external version definition
            platform = contextSetup.Platform;
            majorVersion = as2settings.DefaultFlashVersion;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);
			
            //
			// Class pathes
            //
			classPath = new List<PathModel>();
            
            // MTASC
            string mtascPath = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(as2settings.GetDefaultSDK().Path);
            if (Path.GetExtension(mtascPath) != "") mtascPath = Path.GetDirectoryName(mtascPath);

            string path;
            if ((as2settings.UseMtascIntrinsic || String.IsNullOrEmpty(as2settings.MMClassPath))
                && !String.IsNullOrEmpty(mtascPath) && System.IO.Directory.Exists(mtascPath))
			{
				try 
				{
                    if (majorVersion == 9)
                    {
                        path = Path.Combine(mtascPath, "std9");
                        if (System.IO.Directory.Exists(path)) AddPath(path);
                        else majorVersion = 8;
                    }
                    if (majorVersion == 8)
                    {
                        path = Path.Combine(mtascPath, "std8");
                        if (System.IO.Directory.Exists(path)) AddPath(path);
                    }
                    path = Path.Combine(mtascPath, "std");
                    if (System.IO.Directory.Exists(path)) AddPath(path);
				}
				catch {}
			}
			// Macromedia/Adobe
            if (!String.IsNullOrEmpty(as2settings.MMClassPath) && System.IO.Directory.Exists(as2settings.MMClassPath))
            {
                if (classPath.Count == 0)
                {
                    // Flash CS3: the FP9 classpath overrides some classes of FP8
                    int tempVersion = majorVersion;
                    if (tempVersion > 8)
                    {
                        path = Path.Combine(as2settings.MMClassPath, "FP" + tempVersion);
                        if (System.IO.Directory.Exists(path))
                            AddPath(path);
                        // now add FP8
                        tempVersion = 8;
                    }
                    path = Path.Combine(as2settings.MMClassPath, "FP" + Math.Max(7, tempVersion));
                    if (System.IO.Directory.Exists(path))
                    {
                        PathModel aPath = new PathModel(path, this);
                        ManualExploration(aPath, new string[] { "aso", "FP7", "FP8", "FP9" });
                        AddPath(aPath);
                    }
                }
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
            AddPath(Path.Combine(PathHelper.LibraryDir, "AS2/classes"));
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
		
		/// <summary>
		/// Delete current class's ASO file
		/// </summary>
		public override void RemoveClassCompilerCache()
		{
            if (as2settings == null) return;

            ClassModel pClass = cFile.GetPublicClass();
            if (as2settings.MMClassPath == null || pClass.IsVoid())
				return;
            string package = (cFile.Package.Length > 0) ? cFile.Package + "." : "";
            string packagePath = dirSeparator + package.Replace('.', dirSeparatorChar);
            string file = Path.Combine(as2settings.MMClassPath, "aso") + packagePath + package + pClass.Name + ".aso";
			try
			{
				if (File.Exists(file)) File.Delete(file);
			}
			catch {}
		}
        #endregion

        #region class resolution
		
		public override ClassModel CurrentClass 
		{
			get 
            {
                if (cFile == FileModel.Ignore)
                {
                    return ClassModel.VoidClass;
                }
				if (cClass == null)
				{
					cClass = ClassModel.VoidClass;
					cFile.OutOfDate = true;
				}
				// update class
				if (cFile.OutOfDate)
				{
					if (cFile.FileName.Length > 0)
					{
						string prevClassName = cClass.Name;
						UpdateCurrentFile(true);
					}
					// update "this" and "super" special vars
					UpdateTopLevelElements();
				}
				return cClass;
			}
		}

        /// <summary>
        /// Evaluates the visibility of one given type from another.
        /// Caller is responsible of calling ResolveExtends() on 'inClass'
        /// </summary>
        /// <param name="inClass">Completion context</param>
        /// <param name="withClass">Completion target</param>
        /// <returns>Completion visibility</returns>
        public override Visibility TypesAffinity(ClassModel inClass, ClassModel withClass)
        {
            if (inClass == null || withClass == null) return Visibility.Public;
            // inheritance affinity
            ClassModel tmp = inClass;
            while (!tmp.IsVoid())
            {
                if (tmp.Type == withClass.Type)
                    return Visibility.Public | Visibility.Private;
                tmp = tmp.Extends;
            }
            // public only
            return Visibility.Public;
        }

        /// <summary>
        /// Default types inheritance
        /// </summary>
        /// <param name="package">File package</param>
        /// <param name="classname">Class name</param>
        /// <returns>Inherited type</returns>
        public override string DefaultInheritance(string package, string classname)
        {
            if (package.Length == 0 && classname == features.objectKey) return features.voidKey;
            else return features.objectKey;
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

                // other top-level elements
                ASComplete.FindMember(token, topLevel, result, 0, 0);
                if (!result.IsNull()) return;

                // special _levelN
                if (hasLevels && token.StartsWith("_") && re_level.IsMatch(token))
                {
                    result.Member = new MemberModel();
                    result.Member.Name = token;
                    result.Member.Flags = FlagType.Variable;
                    result.Member.Type = "MovieClip";
                    result.Type = ResolveType("MovieClip", null);
                    result.InFile = topLevel;
                }
            }
        }

        /// <summary>
        /// Return imported classes list (not null)
        /// </summary>
        /// <param name="package">Package to explore</param>
        /// <param name="inFile">Current file</param>
        public override MemberList ResolveImports(FileModel inFile)
        {
            if (inFile == cFile && completionCache.Imports != null)
                return completionCache.Imports;

            MemberList imports = new MemberList();
            if (inFile == null) return imports;
            bool filterImports = (inFile == cFile) && inFile.Classes.Count > 1;
            int lineMin = (filterImports && inPrivateSection) ? inFile.PrivateSectionIndex : 0;
            int lineMax = (filterImports && inPrivateSection) ? int.MaxValue : inFile.PrivateSectionIndex;
            foreach (MemberModel item in inFile.Imports)
            {
                if (filterImports && (item.LineFrom < lineMin || item.LineFrom > lineMax)) continue;
                if (item.Name != "*")
                {
                    if (settings.LazyClasspathExploration) imports.Add(item);
                    else
                    {
                        ClassModel type = ResolveType(item.Type, null);
                        if (!type.IsVoid()) imports.Add(type);
                        else 
                        {
                            // package-level declarations
                            int p = item.Type.LastIndexOf('.');
                            if (p < 0) continue;
                            string package = item.Type.Substring(0, p);
                            string token = item.Type.Substring(p+1);
                            FileModel pack = ResolvePackage(package, false);
                            if (pack == null) continue;
                            MemberModel member = pack.Members.Search(token, 0, 0);
                            if (member != null) imports.Add(member);
                        }
                    }
                }
                else
                {
                    // classes matching wildcard
                    FileModel matches = ResolvePackage(item.Type.Substring(0, item.Type.Length - 2), false);

                    if (matches != null)
                    {
                        foreach (MemberModel import in matches.Imports)
                            imports.Add(import);
                        foreach (MemberModel member in matches.Members)
                            imports.Add(member);
                    }
                }
            }
            if (inFile == cFile) completionCache.Imports = imports;
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
            string name = member.Name;
            int lineMin = (ASContext.Context.InPrivateSection) ? cFile.PrivateSectionIndex : 0;
            int lineMax = atLine;
            foreach (MemberModel import in cFile.Imports)
            {
                if (import.LineFrom >= lineMin && import.LineFrom <= lineMax && import.Name == name)
                {
                    if (import.Type != fullName) throw new Exception("Ambiguous Type");
                    return true;
                }
                else if (import.Name == "*" && import.Type.Replace("*", name) == fullName)
                    return true;
            }
            return false;
        }

		/// <summary>
		/// Retrieves a class model from its name
		/// </summary>
		/// <param name="cname">Class (short or full) name</param>
		/// <param name="inClass">Current file</param>
		/// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public override ClassModel ResolveType(string cname, FileModel inFile)
		{
            // unknown type
            if (string.IsNullOrEmpty(cname) || cname == features.voidKey || classPath == null) 
                return ClassModel.VoidClass;

            // typed array
            if (cname.IndexOf('@') > 0)
                return ResolveTypeIndex(cname, inFile);

            string package = "";
            Match m = re_lastDot.Match(cname);
            if (m.Success)
            {
                package = cname.Substring(0, m.Index);
                cname = cname.Substring(m.Index + 1);
            }

            // quick check in current file
            if (inFile != null && inFile.Classes.Count > 0)
            {
                foreach (ClassModel aClass in inFile.Classes)
                    if (aClass.Name == cname && (package == "" || package == inFile.Package))
                        return aClass;
            }

            // package reference for resolution
            string inPackage = (features.hasPackages && inFile != null) ? inFile.Package : "";

            // search in imported classes
            if (package == "" && inFile != null)
            {
                foreach (MemberModel import in inFile.Imports)
                {
                    if (import.Name == cname)
                    {
                        if (import.Type.Length > import.Name.Length)
                            package = import.Type.Substring(0, import.Type.Length - cname.Length - 1);
                        break;
                    }
                    else if (features.hasImportsWildcard)
                    {
                        if (import.Name == "*" && import.Type.Length > 2)
                        {
                            // try wildcards
                            string testPackage = import.Type.Substring(0, import.Type.Length - 2);
                            if (settings.LazyClasspathExploration)
                            {
                                ClassModel testClass = GetModel(testPackage, cname, inPackage);
                                if (!testClass.IsVoid()) return testClass;
                            }
                            else
                            {
                                FileModel pack = ResolvePackage(testPackage, false);
                                if (pack == null) continue;
                                MemberModel found = pack.Imports.Search(cname, 0, 0);
                                if (found != null) return ResolveType(found.Type, null);
                            }
                        }
                    }
                    else
                    {
                        if (settings.LazyClasspathExploration)
                        {
                            ClassModel testClass = GetModel(import.Type, cname, inPackage);
                            if (!testClass.IsVoid()) return testClass;
                        }
                        else
                        {
                            FileModel pack = ResolvePackage(import.Type, false);
                            if (pack == null) continue;
                            MemberModel found = pack.Imports.Search(cname, 0, 0);
                            if (found != null) return ResolveType(found.Type, null);
                        }
                    }
                }
            }

            // search in classpath
            return GetModel(package, cname, inPackage);
        }

        protected ClassModel ResolveTypeIndex(string cname, FileModel inFile)
        {
            int p = cname.IndexOf('@');
            if (p < 0) return ClassModel.VoidClass;
            string indexType = cname.Substring(p + 1);
            string baseType = cname.Substring(0, p);

            ClassModel originalClass = ResolveType(baseType, inFile);
            if (originalClass.IsVoid()) return originalClass;
            baseType = originalClass.QualifiedName;

            ClassModel indexClass = ResolveType(indexType, inFile);

            if (baseType == "Object" || baseType == "Dynamic")
            {
                if (!indexClass.IsVoid()) return indexClass;
                return MakeCustomObjectClass(originalClass, indexType);
            }
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

            // special AS3 Proxy support
            if (originalClass.QualifiedName == "flash.utils.Proxy")
            {
                // have the proxy extend the index type
                aClass.ExtendsType = indexType;
            }
            // replace 'Object' and '*' by the index type
            else
            foreach (MemberModel member in aClass.Members)
            {
                if (member.Type == features.objectKey || member.Type == "*") member.Type = indexType;
                if (member.Parameters != null)
                {
                    foreach (MemberModel param in member.Parameters)
                    {
                        if (param.Name == "value" 
                            && (param.Type == features.objectKey || param.Type == "*"))
                            param.Type = indexType;
                    }
                }
            }

            aFile.Classes.Add(aClass);
            return aClass;
        }

        protected ClassModel MakeCustomObjectClass(ClassModel objectClass, string indexType)
        {
            foreach (ClassModel c in objectClass.InFile.Classes)
                if (c.IndexType == indexType) return c;

            ClassModel aClass = new ClassModel();
            aClass.Flags = objectClass.Flags;
            aClass.Access = objectClass.Access;
            aClass.ExtendsType = "";
            aClass.Name = objectClass.QualifiedName + "@" + indexType;
            aClass.IndexType = indexType;
            aClass.InFile = objectClass.InFile;

            FlagType flags = FlagType.Dynamic | FlagType.Variable | FlagType.AutomaticVar;
            foreach (string prop in indexType.Split(','))
            {
                MemberModel member = new MemberModel(prop, "", flags, Visibility.Public);
                aClass.Members.Add(member);
            }
            objectClass.InFile.Classes.Add(aClass);
            return aClass;
        }

        /// <summary>
        /// Search a fully qualified class in classpath
        /// </summary>
        /// <param name="package">Class package</param>
        /// <param name="cname">Class name</param>
        /// <param name="inPackage">Package reference for resolution</param>
        /// <returns></returns>
        public override ClassModel GetModel(string package, string cname, string inPackage)
        {
			if (!settings.LazyClasspathExploration)
            {
                bool testSamePackage = package.Length == 0 && features.hasPackages;
                bool testModule = package.Length > 0 && features.hasModules;
                foreach (PathModel aPath in classPath) 
                    if (aPath.IsValid && !aPath.Updating)
                    {
                        ClassModel found = LookupClass(package, cname, inPackage, testSamePackage, testModule, aPath);
                        if (found != null) return found;
                    }
                if (classPath.Count > 0 && classPath[0].IsTemporaryPath)
                {
                    // guess file name
                    string fullClass = ((package.Length > 0) ? package + "." : "") + cname;
                    string fileName = fullClass.Replace(".", dirSeparator) + ".as";

                    ClassModel model = null;
                    try
                    {
                        Path.Combine(classPath[0].Path, fileName); // test path for invalid characters
                        model = LocateClassFile(classPath[0], fileName);
                    }
                    catch { }
                    if (model != null) return model;
                    else return ClassModel.VoidClass;
                }
            }
            else
            {
                // guess file name
                string fullClass = ((package.Length > 0) ? package + "." : "") + cname;
                string fileName = fullClass.Replace(".", dirSeparator) + ".as";

                foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
                {
                    ClassModel model = LocateClassFile(aPath, fileName);
                    if (model != null) return model;
                }
            }
            return ClassModel.VoidClass;
		}

        private ClassModel LookupClass(string package, string cname, string inPackage, bool testSamePackage, bool testModule, PathModel aPath)
        {
            bool matchParentPackage = testSamePackage && features.hasFriendlyParentPackages;

            ClassModel found = null;
            int pLen = inPackage.Length;

            aPath.ForeachFile((aFile) =>
            {
                string pkg = aFile.Package;
                // qualified path
                if (pkg == package && aFile.Classes.Count > 0)
                {
                    foreach (ClassModel aClass in aFile.Classes)
                        if (aClass.Name == cname && (pkg == "" || aFile.Module == "" || aFile.Module == aClass.Name))
                        {
                            found = aClass;
                            return false;
                        }
                }
                else if (testModule && aFile.FullPackage == package && aFile.Classes.Count > 0)
                {
                    foreach (ClassModel aClass in aFile.Classes)
                        if (aClass.Name == cname)
                        {
                            found = aClass;
                            return false;
                        }
                }
                // in the same (or parent) package
                else if (testSamePackage)
                {
                    if (inPackage == pkg || (matchParentPackage && pkg.Length < pLen && inPackage.StartsWith(pkg + ".")))
                        foreach (ClassModel aClass in aFile.Classes)
                            if (aClass.Name == cname /*&& (aFile.Module == "" || aFile.Module == aClass.Name)*/)
                            {
                                found = aClass;
                                return false;
                            }
                }
                return true;
            });
            return found;
        }

        private ClassModel LocateClassFile(PathModel aPath, string fileName)
        {
            if (!aPath.IsValid) return null;
            try
            {
                string path = Path.Combine(aPath.Path, fileName);
                // cached file
                if (aPath.HasFile(path))
                {
                    FileModel nFile = aPath.GetFile(path);
                    if (nFile.Context != this)
                    {
                        // not associated with this context -> refresh
                        nFile.OutOfDate = true;
                        nFile.Context = this;
                    }
                    return nFile.GetPublicClass();
                }
                // non-cached existing file
                else if (File.Exists(path))
                {
                    FileModel nFile = GetFileModel(path);
                    if (nFile != null)
                    {
                        aPath.AddFile(nFile);
                        return nFile.GetPublicClass();
                    }
                }
            }
            catch (Exception ex)
            {
                aPath.IsValid = false;
                ErrorManager.ShowError(ex);
            }
            return null;
        }

        /// <summary>
        /// Update model if needed and warn user if it has problems
        /// <param name="onFileOpen">Flag indicating it is the first model check</param>
        /// </summary>
        public override void CheckModel(bool onFileOpen)
        {
            if (!File.Exists(cFile.FileName))
            {
                // refresh model
                base.CheckModel(onFileOpen);
                return;
            }
            string prevPackage = (onFileOpen) ? null : cFile.Package;
            string prevCname = (onFileOpen) ? null : cFile.GetPublicClass().Name;
            // refresh model
            base.CheckModel(onFileOpen);

            if (!MessageBar.Locked && features.checkFileName && cFile.Version > 1)
            {
                string package = cFile.Package;
                ClassModel pClass = cFile.GetPublicClass();
                if (package.Length > 0)
                {
                    string pathname = package.Replace('.', Path.DirectorySeparatorChar);
                    string fullpath = Path.GetDirectoryName(cFile.FileName);
                    if (!fullpath.EndsWith(pathname))
                    {
                        if (settings.FixPackageAutomatically && CurSciControl != null)
                        {
                            bool isAs2 = cFile.Context.Settings.LanguageId == "AS2";

                            int pos = -1;

                            string txt = "";
                            string regexPackageLine = "";

                            int counter = CurSciControl.Length;
                            int p = 0;
                            Regex packagePattern = null;
                            if (isAs2)
                            {
                                packagePattern = new Regex("class\\s+(" + cFile.Package.Replace(".", "\\.") + "\\." + pClass.Name + ')');
                            }
                            else
                            {
                                packagePattern = new Regex("package\\s+(" + cFile.Package.Replace(".", "\\.") + ')');
                            }
                            while (p < counter)
                            {
                                char c = (char)CurSciControl.CharAt(p++);
                                txt += c;
                                if (txt.Length > 5 && c <= 32)
                                {
                                    Match m = packagePattern.Match(txt);
                                    if (m.Success)
                                    {
                                        pos = m.Groups[1].Index;
                                        regexPackageLine = m.Value;
                                        break;
                                    }
                                }
                            }

                            if (regexPackageLine.Length > 0 && pos > -1)
                            {
                                string orgid = "Info.PackageDontMatchFilePath";
                                List<PathModel> classpaths = Context.Classpath;
                                if (classpaths != null)
                                {
                                    string correctPath = null;
                                    foreach (PathModel pm in classpaths)
                                    {
                                        if (fullpath.IndexOf(pm.Path) > -1 && fullpath.Length > pm.Path.Length)
                                        {
                                            correctPath = fullpath.Substring(pm.Path.Length + 1);
                                        }
                                        else if (fullpath.ToLower() == pm.Path.ToLower())
                                        {
                                            correctPath = ""; // We are in root, no package..
                                        }
                                    }
                                    if (correctPath != null)
                                    {
                                        correctPath = correctPath.Replace(Path.DirectorySeparatorChar, '.');
                                        CurSciControl.SetSel(pos, pos + cFile.Package.Length);
                                        CurSciControl.ReplaceSel(correctPath);
                                        orgid = "Info.PackageDidntMatchFilePath";
                                    }
                                }
                                string org = TextHelper.GetString(orgid);
                                string msg = String.Format(org, package) + "\n" + cFile.FileName;
                                MessageBar.ShowWarning(msg);
                            }

                        }
                        else
                        {
                            string org = TextHelper.GetString("Info.PackageDontMatchFilePath");
                            string msg = String.Format(org, package) + "\n" + cFile.FileName;
                            MessageBar.ShowWarning(msg);
                        }
                        return;
                    }
                    else MessageBar.HideWarning();
                }
                if (!pClass.IsVoid())
                {
                    string cname = pClass.Name;
                    if (prevPackage != package || prevCname != cname)
                    {
                        if (package.Length > 0) cname = package + "." + cname;
                        string filename = cname.Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(cFile.FileName);
                        if (!cFile.FileName.ToUpper().EndsWith(filename.ToUpper()))
                        {
                            string org = TextHelper.GetString("Info.TypeDontMatchFileName");
                            string msg = String.Format(org, cname) + "\n" + cFile.FileName;
                            MessageBar.ShowWarning(msg);
                        }
                        else MessageBar.HideWarning();
                    }
                }
            }
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
		/// Prepare AS2 intrinsic known vars/methods/classes
		/// </summary>
		protected override void InitTopLevelElements()
		{
            string filename = "toplevel.as";
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
                // MTASC toplevel-style declaration:
                ClassModel tlClass = topLevel.GetPublicClass();
                if (!tlClass.IsVoid())
                {
                    topLevel.Members = tlClass.Members;
                    tlClass.Members = null;
                    topLevel.Classes = new List<ClassModel>();
                }
            }
			// not found
			else
			{
                //ErrorHandler.ShowInfo("Top-level elements class not found. Please check your Program Settings.");
			}

            if (topLevel.Members.Search("_root", 0, 0) == null)
			    topLevel.Members.Add(new MemberModel("_root", docType, FlagType.Variable, Visibility.Public));
            if (topLevel.Members.Search("_global", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("_global", features.objectKey, FlagType.Variable, Visibility.Public));
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
        /// Retrieves a package content
        /// </summary>
        /// <param name="name">Package path</param>
        /// <param name="lazyMode">Force file system exploration</param>
        /// <returns>Package folders and types</returns>
        public override FileModel ResolvePackage(string name, bool lazyMode)
		{
            if (name == null) name = "";
            else if (!re_package.IsMatch(name)) return null;

            FileModel pModel = new FileModel();
            pModel.Package = name;
            pModel.OutOfDate = false;

            string packagePath = name.Replace('.', dirSeparatorChar);
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
            {
                // explore file system
                if (lazyMode || settings.LazyClasspathExploration || aPath.IsTemporaryPath)
                {
                    string path = Path.Combine(aPath.Path, packagePath);
                    if (aPath.IsValid && System.IO.Directory.Exists(path))
                    {
                        try
                        {
                            PopulatePackageEntries(name, path, pModel.Imports);
                            PopulateClassesEntries(name, path, pModel.Imports);
                        }
                        catch (Exception ex)
                        {
                            ErrorManager.ShowError(ex);
                        }
                    }
                }
                // explore parsed models
                else
                {
                    string prevPackage = null;
                    string packagePrefix = name.Length > 0 ? name + "." : "";
                    int nameLen = name.Length + 1;
                    aPath.ForeachFile((model) =>
                    {
                        if (!model.HasPackage)
                            return true; // skip
                        string package = model.Package;
                        if (package == name)
                        {
                            foreach (ClassModel type in model.Classes)
                            {
                                if (type.IndexType != null) continue;
                                MemberModel item = type.ToMemberModel();
                                if (type.Access == Visibility.Private)
                                {
                                    item.Type = item.Name;
                                    item.Access = Visibility.Private;
                                }
                                pModel.Imports.Add(item);
                            }
                            foreach (MemberModel member in model.Members)
                                pModel.Members.Add(member.Clone() as MemberModel);
                        }
                        else if (package != prevPackage
                                && (package.Length > name.Length && package.StartsWith(packagePrefix))) // imports
                        {
                            prevPackage = package;
                            if (nameLen > 1) package = package.Substring(nameLen);
                            int p = package.IndexOf('.');
                            if (p > 0) package = package.Substring(0, p);
                            if (pModel.Imports.Search(package, 0, 0) == null) // sub packages
                            {
                                pModel.Imports.Add(new MemberModel(package, package, FlagType.Package, Visibility.Public));
                            }
                        }
                        return true;
                    });
                }
            }

			// result
            if (pModel.Imports.Count > 0 || pModel.Members.Count > 0)
			{
                pModel.Imports.Sort();
				return pModel;
			}
			else return null;
		}

        private void PopulateClassesEntries(string package, string path, MemberList memberList)
        {
            string[] fileEntries = null;
            try
            {
                fileEntries = System.IO.Directory.GetFiles(path, "*" + settings.DefaultExtension);
            }
            catch { }
            if (fileEntries == null) return;
            string mname;
            string type;
            FlagType flag = FlagType.Class | ((package == null) ? FlagType.Intrinsic : 0);
            foreach (string entry in fileEntries)
            {
                mname = GetLastStringToken(entry, dirSeparator);
                mname = mname.Substring(0, mname.LastIndexOf("."));
                if (mname.Length > 0 && memberList.Search(mname, 0, 0) == null && re_token.IsMatch(mname))
                {
                    type = mname;
                    if (package.Length > 0) type = package + "." + mname;
                    memberList.Add(new MemberModel(mname, type, flag, Visibility.Public));
                }
            }
        }

        private void PopulatePackageEntries(string package, string path, MemberList memberList)
        {
            string[] dirEntries = null;
            try
            {
                dirEntries = System.IO.Directory.GetDirectories(path);
            }
            catch { }
            if (dirEntries == null) return;

            string mname;
            string type;
            foreach (string entry in dirEntries)
            {
                mname = GetLastStringToken(entry, dirSeparator);
                if (mname.Length > 0 && memberList.Search(mname, 0, 0) == null && re_token.IsMatch(mname))
                {
                    type = mname;
                    if (package.Length > 0) type = package + "." + mname;
                    memberList.Add(new MemberModel(mname, type, FlagType.Package, Visibility.Public));
                }
            }
        }

        /// <summary>
        /// Return the top-level elements (this, super) for the current file
        /// </summary>
        /// <returns></returns>
        public override MemberList GetTopLevelElements()
        {
            if (topLevel != null)
            {
                if (topLevel.OutOfDate) InitTopLevelElements();
                return topLevel.Members;
            }
            else return new MemberList();
        }

        /// <summary>
        /// Return the visible elements (types, package-level declarations) visible from the current file
        /// </summary>
        /// <returns></returns>
        public override MemberList GetVisibleExternalElements()
		{
            if (!IsFileValid) return new MemberList();

            if (completionCache.IsDirty)
            {
                MemberList elements = new MemberList();
                // root types & packages
                FileModel baseElements = ResolvePackage(null, false);
                if (baseElements != null)
                {
                    elements.Add(baseElements.Imports);
                    elements.Add(baseElements.Members);
                }
                elements.Add(new MemberModel(features.voidKey, features.voidKey, FlagType.Class | FlagType.Intrinsic, 0));

                //bool qualify = Settings.CompletionShowQualifiedTypes && settings.GenerateImports;
                
                // other classes in same package
                if (features.hasPackages && cFile.Package != "")
                {
                    FileModel packageElements = ResolvePackage(cFile.Package, false);
                    if (packageElements != null)
                    {
                        foreach (MemberModel member in packageElements.Imports)
                        {
                            if (member.Flags != FlagType.Package)
                            {
                                //if (qualify) member.Name = member.Type;
                                elements.Add(member);
                            }
                        }
                        foreach (MemberModel member in packageElements.Members)
                        {
                            string pkg = member.InFile.Package;
                            //if (qualify && pkg != "") member.Name = pkg + "." + member.Name;
                            member.Type = pkg != "" ? pkg + "." + member.Name : member.Name;
                            elements.Add(member);
                        }
                    }
                }
                // other classes in same file
                if (cFile.PrivateSectionIndex > 0)
                {
                    if (inPrivateSection && cFile.Classes.Count > 1)
                    {
                        ClassModel mainClass = cFile.GetPublicClass();
                        if (!mainClass.IsVoid())
                        {
                            MemberModel toRemove = elements.Search(mainClass.Name, 0, 0);
                            if (toRemove != null && toRemove.Type == mainClass.QualifiedName)
                                elements.Remove(toRemove);
                        }
                    }

                    MemberModel member;
                    foreach (ClassModel aClass in cFile.Classes)
                    {
                        if (features.hasMultipleDefs || aClass.Access == Visibility.Private)
                        {
                            member = aClass.ToMemberModel();
                            elements.Add(member);
                        }
                    }
                }

                // imports
                elements.Add(ResolveImports(CurrentModel));

                // in cache
                elements.Sort();
                completionCache = new CompletionCache(this, elements);

                // known classes colorization
                if (!CommonSettings.DisableKnownTypesColoring && !settings.LazyClasspathExploration && CurSciControl != null)
                {
                    try
                    {
                        CurSciControl.KeyWords(1, completionCache.Keywords); // additional-keywords index = 1
                        CurSciControl.Colourise(0, -1); // re-colorize the editor
                    } 
                    catch (AccessViolationException){} // catch memory errors
                }
            }
            return completionCache.Elements;
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
            ClassModel aClass;
            MemberModel item;
            // public classes
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
            {
                aPath.ForeachFile((aFile) =>
                {
                    aClass = aFile.GetPublicClass();
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
            if (as2settings != null) return as2settings.GetDefaultSDK().Path;
            else return null;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (as2settings == null)
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.FeatureMissing"));
                return;
            }
            // just run the compiler against the current file
            RunCMD(as2settings.MtascCheckParameters);
        }

		/// <summary>
		/// Run MTASC compiler in the current class's base folder with current classpath
		/// </summary>
		/// <param name="append">Additional comiler switches</param>
		public override void RunCMD(string append)
		{
            if (as2settings == null)
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
                    : PathHelper.ResolvePath(as2settings.GetDefaultSDK().Path);

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
			}
		}
		
		/// <summary>
		/// Calls RunCMD with additional parameters taken from the classes @mtasc doc tag
		/// </summary>
		public override bool BuildCMD(bool failSilently)
		{
            if (as2settings == null)
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
			return true;
		}
        #endregion
	}
}
