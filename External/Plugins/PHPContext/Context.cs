using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Helpers;

namespace PHPContext
{
    public class Context : AS2Context.Context
    {
        #region initialization
        new static readonly protected Regex re_CMD_BuildCommand =
            new Regex("@php[\\s]+(?<params>.*)", RegexOptions.Compiled | RegexOptions.Multiline);

        static private readonly Regex re_PHPext =
            new Regex(".php[3-9]?", RegexOptions.Compiled);

        private ContextSettings langSettings;
        private List<InlineRange> phpRanges; // inlined PHP ranges in HTML

        public Context(ContextSettings initSettings)
        {
            langSettings = initSettings;

            /* AS-LIKE OPTIONS */

            hasLevels = false;
            docType = "void";

            /* DESCRIBE LANGUAGE FEATURES */

            // language constructs
            features.hasPackages = false;
            features.hasImports = false;
            features.hasImportsWildcard = false;
            features.hasClasses = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = false;
            features.hasGenerics = false;
            features.hasEcmaTyping = false;
            features.hasVars = true;
            features.hasConsts = true;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasTryCatch = true;
            features.checkFileName = false;

            // allowed declarations access modifiers
            Visibility all = Visibility.Public | Visibility.Protected | Visibility.Private;
            features.classModifiers = all;
            features.varModifiers = all;
            features.methodModifiers = all;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Public;
            features.varModifierDefault = Visibility.Public;
            features.methodModifierDefault = Visibility.Public;

            // keywords
            features.dot = "->";
            features.voidKey = "void";
            features.objectKey = "Object";
            features.typesPreKeys = new string[] { "namespace", "new", "extends", "implements", "as" };
            features.codeKeywords = new string[] {
                "and", "or", "xor", "exception", "as", "break", "case", "continue", "declare", "default", 
                "do", "else", "elseif", "enddeclare", "endfor", "endforeach", "endif", "endswitch", 
                "endwhile", "for", "foreach", "global", "if", "new", "switch", "use", "while", 
                "try", "catch", "throw"
            };
            features.varKey = "var";
            features.functionKey = "function";
            features.staticKey = "static";
            features.publicKey = "public";
            features.privateKey = "private";
            features.intrinsicKey = "extern";

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
            if (langSettings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "PHP";
                contextSetup.Version = "5.0";
            }

            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            // intrinsic language definitions
            if (langSettings.LanguageDefinitions != null)
            {
                string langPath = PathHelper.ResolvePath(langSettings.LanguageDefinitions);
                if (Directory.Exists(langPath)) AddPath(langPath);
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
            AddPath(Path.Combine(PathHelper.LibraryDir, settings.LanguageId + "/classes"));
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
        /// Build the file DOM
        /// </summary>
        /// <param name="fileName">File path</param>
        protected override void GetCurrentFileModel(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (!re_PHPext.IsMatch(ext))
            {
                cFile = FileModel.Ignore;
                UpdateContext(cLine);
            }
            else
            {
                cFile = new FileModel(fileName);
                cFile.Context = this;
                cFile.HasFiltering = true;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(cFile, CurSciControl.Text);
                cLine = CurSciControl.CurrentLine;
                UpdateContext(cLine);
            }
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - define inline AS3 ranges
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public override string FilterSource(string fileName, string src)
        {
            phpRanges = new List<InlineRange>();
            return PhpFilter.FilterSource(src, phpRanges);
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - modify parsed model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override void FilterSource(FileModel model)
        {
            PhpFilter.FilterSource(model, phpRanges);
        }
        #endregion

        #region class resolution
        /// <summary>
        /// Evaluates the visibility of one given type from another.
        /// Caller is responsible of calling ResolveExtends() on 'inClass'
        /// </summary>
        /// <param name="inClass">Completion context</param>
        /// <param name="withClass">Completion target</param>
        /// <returns>Completion visibility</returns>
        public override Visibility TypesAffinity(ClassModel inClass, ClassModel withClass)
        {
            // same file
            if (withClass != null && inClass.InFile == withClass.InFile)
                return Visibility.Public | Visibility.Protected | Visibility.Private;
            // inheritance affinity
            ClassModel tmp = inClass;
            while (!tmp.IsVoid())
            {
                if (tmp == withClass)
                    return Visibility.Public | Visibility.Protected;
                tmp = tmp.Extends;
            }
            // same package
            if (withClass != null && inClass.InFile.Package == withClass.InFile.Package)
                return Visibility.Public;
            // public only
            else
                return Visibility.Public;
        }

        /// <summary>
        /// Prepare intrinsic known vars/methods/classes
        /// </summary>
        protected override void InitTopLevelElements()
        {
            string filename = "toplevel" + settings.DefaultExtension;
            topLevel = new FileModel(filename);

            // search top-level declaration
            foreach (PathModel aPath in classPath)
            {
                var path = Path.Combine(aPath.Path, filename);
                if (File.Exists(path))
                {
                    filename = path;
                    topLevel = GetCachedFileModel(filename);
                    break;
                }
            }

            if (File.Exists(filename))
            {
                // copy declarations as file-level (ie. flatten class)
                /*ClassModel tlClass = topLevel.GetPublicClass();
                if (!tlClass.IsVoid() && tlClass.Members.Count > 0)
                {
                    topLevel.Members = tlClass.Members;
                    tlClass.Members = null;
                    topLevel.Classes = new List<ClassModel>();
                }*/
            }
            // not found
            else
            {
                //ErrorHandler.ShowInfo("Top-level elements class not found. Please check your Program Settings.");
            }

            // special variables
            topLevel.Members.Add(new MemberModel("$this", "", FlagType.Variable, Visibility.Public));
            topLevel.Members.Add(new MemberModel("self", "", FlagType.Variable, Visibility.Public));
            topLevel.Members.Add(new MemberModel("parent", "", FlagType.Variable, Visibility.Public));
            topLevel.Members.Sort();
            foreach (MemberModel member in topLevel.Members)
                member.Flags |= FlagType.Intrinsic;
        }

        public override void CheckModel(bool onFileOpen)
        {
            if (!File.Exists(cFile.FileName))
            {
                // refresh model
                base.CheckModel(onFileOpen);
            }
        }

        /// <summary>
        /// Update intrinsic known vars
        /// </summary>
        protected override void UpdateTopLevelElements()
        {
            var special = topLevel.Members.Search("$this", 0, 0);
            if (special != null)
            {
                if (!cClass.IsVoid()) special.Type = cClass.Name;
                else special.Type = (cFile.Version > 1) ? features.voidKey : docType;
            }
            special = topLevel.Members.Search("self", 0, 0);
            if (special != null)
            {
                if (!cClass.IsVoid()) special.Type = cClass.Name;
                else special.Type = (cFile.Version > 1) ? features.voidKey : docType;
            }
            special = topLevel.Members.Search("parent", 0, 0);
            if (special != null)
            {
                cClass.ResolveExtends();
                var extends = cClass.Extends;
                if (!extends.IsVoid()) special.Type = extends.Name;
                else special.Type = (cFile.Version > 1) ? features.voidKey : features.objectKey;
            }
        }

        /// <summary>
        /// Retrieves a package content
        /// </summary>
        /// <param name="name">Package path</param>
        /// <param name="lazyMode">Force file system exploration</param>
        /// <returns>Package folders and types</returns>
        public override FileModel ResolvePackage(string name, bool lazyMode)
        {
            return base.ResolvePackage(name, lazyMode);
        }
        #endregion

        #region command line compiler

        //static public string TemporaryOutputFile;

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath()
        {
            // to be implemented
            return null;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            // to be implemented
        }

        override public bool CanBuild
        {
            get { return false; }
        }

        /// <summary>
        /// Run compiler in the current files's base folder with current classpath
        /// </summary>
        /// <param name="append">Additional comiler switches</param>
        public override void RunCMD(string append)
        {
            // to be implemented
        }

        /// <summary>
        /// Calls RunCMD with additional parameters taken from the file's doc tag
        /// </summary>
        public override bool BuildCMD(bool failSilently)
        {
            // to be implemented
            return true;
        }
        #endregion
    }
}
