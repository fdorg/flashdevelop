using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PluginCore;
using ProjectManager.Controls;

namespace ProjectManager.Projects
{
    public enum TestMovieBehavior
    {
        Default,
        NewTab,
        NewWindow,
        ExternalPlayer,
        OpenDocument,
        Webserver,
        Custom,
        Unknown
    }

    public delegate void ChangedHandler(Project project);
    public delegate void ProjectUpdatingHandler(Project project);
    public delegate bool BeforeSaveHandler(Project project, string fileName);

    public abstract class Project : IProject
    {
        protected MovieOptions movieOptions;
        internal Dictionary<string, string> storage;
        string preferredSDK;
        string currentSDK;
        PathCollection absClasspaths;
        BuildEventInfo[] vars; // arguments to replace in paths

        public OutputType OutputType = OutputType.Unknown;
        public string InputPath; // For code injection
        public string OutputPath;
        public string PreBuildEvent;
        public string PostBuildEvent;
        public bool AlwaysRunPostBuild;
        public bool ShowHiddenPaths;
        public TestMovieBehavior TestMovieBehavior;
        public string TestMovieCommand;

        public event ChangedHandler ClasspathChanged; // inner operation changed the classpath
        public event BeforeSaveHandler BeforeSave;
        public event ProjectUpdatingHandler ProjectUpdating;

        public Project(string path, CompilerOptions compilerOptions)
        {
            this.ProjectPath = path;
            this.CompilerOptions = compilerOptions;

            TestMovieBehavior = TestMovieBehavior.Default;

            Classpaths = new PathCollection();
            CompileTargets = new PathCollection();
            HiddenPaths = new HiddenPathCollection();
            LibraryAssets = new AssetCollection(this);
            storage = new Dictionary<string, string>();

            InputPath = "";
            OutputPath = "";
            PreBuildEvent = "";
            PostBuildEvent = "";
        }

        public abstract string Language { get; }
        public abstract string LanguageDisplayName { get; }
        public virtual bool IsCompilable => false;
        public virtual bool ReadOnly => false;
        public virtual bool UsesInjection => false;
        public virtual bool HasLibraries => false;
        public virtual bool RequireLibrary => false;
        public virtual void ValidateBuild(out string error) { error = null; }
        public virtual int MaxTargetsCount => 0;
        public abstract string DefaultSearchFilter { get; }

        public abstract void Save();
        public abstract void SaveAs(string fileName);

        protected bool AllowedSaving(string fileName)
        {
            if (ReadOnly && fileName == ProjectPath) return false;
            var onBeforeSave = BeforeSave;
            return onBeforeSave is null
                || onBeforeSave(this, fileName);
        }

        public virtual void PropertiesChanged() => OnClasspathChanged();

        public virtual PropertiesDialog CreatePropertiesDialog() => new PropertiesDialog();

        public void OnClasspathChanged()
        {
            absClasspaths = null;
            ClasspathChanged?.Invoke(this);
        }

        #region Simple Properties

        public string ProjectPath { get; }
        public virtual string Name => Path.GetFileNameWithoutExtension(ProjectPath);
        public string Directory => Path.GetDirectoryName(ProjectPath);
        public bool TraceEnabled { set; get; }
        public string TargetBuild { set; get; }
        public virtual bool EnableInteractiveDebugger => movieOptions.DebuggerSupported(TargetBuild);
        public string[] AdditionalPaths; // temporary storage of resolved classpaths
        
        // we only provide getters for these to preserve the original pointer
        public MovieOptions MovieOptions => movieOptions;
        public PathCollection Classpaths { get; }
        public PathCollection CompileTargets { get; }
        public HiddenPathCollection HiddenPaths { get; }
        public AssetCollection LibraryAssets { get; }
        public virtual string LibrarySWFPath => OutputPath;
        public Dictionary<string, string> Storage => storage;
        public List<string> ExternalLibraries { get; } = new List<string>();

        public CompilerOptions CompilerOptions { get; set; }

        public PathCollection AbsoluteClasspaths
        {
            get
            {
                // property is accessed quite intensively, adding some caching here
                if (absClasspaths != null) return absClasspaths;

                PathCollection absolute = new PathCollection();
                foreach (string cp in Classpaths)
                {
                    absolute.Add(GetAbsolutePath(cp));
                }
                absClasspaths = absolute;
                return absolute;
            }
        }

        public string[] SourcePaths => Classpaths.ToArray();

        public string OutputPathAbsolute => GetAbsolutePath(OutputPath);

        public string PreferredSDK
        {
            get => preferredSDK;
            set
            {
                preferredSDK = value;
                currentSDK = null;
            }
        }

        public string CurrentSDK
        {
            get => currentSDK;
            set
            {
                if (value != currentSDK)
                {
                    currentSDK = value; 
                    OnClasspathChanged();
                }
            }
        }

        #endregion

        #region Project Methods

        // all the Set/Is methods expect absolute paths (as opposed to the way they're
        // actually stored)

        public void SetPathHidden(string path, bool isHidden)
        {
            path = GetRelativePath(path);

            if (isHidden)
            {
                HiddenPaths.Add(path);              
                CompileTargets.RemoveAtOrBelow(path); // can't compile hidden files
                LibraryAssets.RemoveAtOrBelow(path); // can't embed hidden resources
            }
            else HiddenPaths.Remove(path);
        }

        public bool IsPathHidden(string path) => HiddenPaths.IsHidden(GetRelativePath(path));

        public virtual void SetCompileTarget(string path, bool isCompileTarget)
        {
            string relPath = Path.IsPathRooted(path) ? GetRelativePath(path) : path;
            if (isCompileTarget) CompileTargets.Add(relPath);
            else CompileTargets.Remove(relPath);
        }

        public virtual void SetDocumentClass(string path, bool isMain)
        {
            // to be implemented
        }

        public bool IsCompileTarget(string path) => CompileTargets.Contains(GetRelativePath(path));

        public virtual bool IsDocumentClass(string path) => false;

        public bool IsClassPath(string path) => AbsoluteClasspaths.Contains(path);

        public virtual void SetLibraryAsset(string path, bool isLibraryAsset)
        {
            string relPath = Path.IsPathRooted(path) ? GetRelativePath(path) : path;
            if (isLibraryAsset) LibraryAssets.Add(relPath);
            else LibraryAssets.Remove(relPath);
        }

        public virtual bool IsLibraryAsset(string path) => LibraryAssets.Contains(GetRelativePath(path));

        public virtual LibraryAsset GetAsset(string path) => LibraryAssets[GetRelativePath(path)];

        public virtual void ChangeAssetPath(string fromPath, string toPath)
        {
            if (IsLibraryAsset(fromPath))
            {
                LibraryAsset asset = LibraryAssets[GetRelativePath(fromPath)];
                LibraryAssets.Remove(asset);
                asset.Path = GetRelativePath(toPath);
                LibraryAssets.Add(asset);
            }
        }

        public bool IsInput(string path) => GetRelativePath(path) == InputPath;

        public bool IsOutput(string path) => GetRelativePath(path) == OutputPath;

        /// <summary>
        /// Call this when you delete a path so we can remove all our references to it
        /// </summary>
        public void NotifyPathsDeleted(string path)
        {
            path = GetRelativePath(path);
            HiddenPaths.Remove(path);
            CompileTargets.RemoveAtOrBelow(path);
            LibraryAssets.RemoveAtOrBelow(path);
        }

        /// <summary>
        /// Returns the path to the "obj\" subdirectory, creating it if necessary.
        /// </summary>
        public string GetObjDirectory()
        {
            string objPath = Path.Combine(Directory, "obj");
            if (!System.IO.Directory.Exists(objPath))
                System.IO.Directory.CreateDirectory(objPath);
            return objPath;
        }

        /// <summary>
        /// Return text to "Insert Into Document"
        /// </summary>
        public virtual string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            // to be implemented
            return null;
        }

        /// <summary>
        /// Indicate if the path can be flagged as "Always Compile"
        /// </summary>
        public virtual CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            // to be implemented
            return CompileTargetType.None;
        }

        /// <summary>
        /// Clear output
        /// </summary>
        public virtual bool Clean()
        {
            // to be implemented
            return true;
        }

        /// <summary>
        /// Return name of external IDE to use for compilation:
        /// - Adobe Flash Professional: "FlashIDE"
        /// - other value will be dispatched as a "ProjectManager.RunWithAssociatedIDE" command event
        /// </summary>
        public virtual string GetOtherIDE(bool runOutput, bool releaseMode, out string error)
        {
            error = "Info.NoAssociatedIDE";
            return null;
        }

        #endregion

        #region Path Helpers

        public string[] GetHiddenPaths() => HiddenPaths.ToArray();

        public string GetRelativePath(string path) => ProjectPaths.GetRelativePath(Directory, path);

        public void UpdateVars(bool silent)
        {
            if (!silent) ProjectUpdating?.Invoke(this);
            vars = new BuildEventVars(this).GetVars();
        }

        public string GetAbsolutePath(string path)
        {
            path = Environment.ExpandEnvironmentVariables(path);
            if (vars != null && path.IndexOf('$') >= 0)
                foreach (BuildEventInfo arg in vars) 
                    path = path.Replace(arg.FormattedName, arg.Value);
            return ProjectPaths.GetAbsolutePath(Directory, path);
        }

        /// <summary>
        /// When in Release configuration, remove 'debug' from the given path.
        /// Pattern: ([a-zA-Z0-9])[-_.]debug([\\/.])
        /// </summary>
        public string FixDebugReleasePath(string path)
        {
            if (!TraceEnabled)
                return Regex.Replace(path, @"([a-zA-Z0-9])[-_.]debug([\\/.])", "$1$2");
            return path;
        }

        /// <summary>
        /// Replace accented characters and remove whitespace
        /// </summary>
        public static string RemoveDiacritics(string s)
        {
            var normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        #endregion


        public bool IsDirectory(string path) => System.IO.Directory.Exists(path);
    }

    public enum OutputType
    {
        Unknown,
        OtherIDE,
        CustomBuild,
        Application,
        Library,
        Website
    }

    public enum CompileTargetType
    {
        None = 0,
        AlwaysCompile = 1,
        DocumentClass = 2
    }
}
