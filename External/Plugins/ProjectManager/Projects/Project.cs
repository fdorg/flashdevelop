using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

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

    public abstract class Project : PluginCore.IProject
	{
        string path; // full path to this project, including filename

        protected MovieOptions movieOptions;
        CompilerOptions compilerOptions;
		PathCollection classpaths;
		PathCollection compileTargets;
        HiddenPathCollection hiddenPaths;
        AssetCollection libraryAssets;
        internal Dictionary<string, string> storage;
        bool traceEnabled; // selected configuration 
        string targetBuild;
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
			this.path = path;
            this.compilerOptions = compilerOptions;

            TestMovieBehavior = TestMovieBehavior.Default;

			classpaths = new PathCollection();
			compileTargets = new PathCollection();
			hiddenPaths = new HiddenPathCollection();
            libraryAssets = new AssetCollection(this);
            storage = new Dictionary<string, string>();

            InputPath = "";
			OutputPath = "";
			PreBuildEvent = "";
			PostBuildEvent = "";
		}

        public abstract string Language { get; }
        public virtual bool IsCompilable { get { return false; } }
        public virtual bool ReadOnly { get { return false; } }
        public virtual bool UsesInjection { get { return false; } }
        public virtual bool HasLibraries { get { return false; } }
        public virtual bool RequireLibrary { get { return false; } }
        public virtual void ValidateBuild(out string error) { error = null; }
        public virtual int MaxTargetsCount { get { return 0; } }
        public abstract string DefaultSearchFilter { get; }

        public abstract void Save();
        public abstract void SaveAs(string fileName);

        protected bool AllowedSaving(string fileName)
        {
            if (ReadOnly && fileName == ProjectPath) return false;
            if (BeforeSave != null) return BeforeSave(this, fileName);
            else return true;
        }

        public virtual void PropertiesChanged() 
        {
            OnClasspathChanged();
        }

        public virtual ProjectManager.Controls.PropertiesDialog CreatePropertiesDialog()
        {
            return new ProjectManager.Controls.PropertiesDialog();
        }

        public void OnClasspathChanged()
        {
            absClasspaths = null;
            if (ClasspathChanged != null) ClasspathChanged(this);
        }

		#region Simple Properties

		public string ProjectPath { get { return path; } }
        public virtual string Name { get { return Path.GetFileNameWithoutExtension(path); } }
		public string Directory { get { return Path.GetDirectoryName(path); } }
        public bool TraceEnabled { set { traceEnabled = value; } get { return traceEnabled; } }
        public string TargetBuild { set { targetBuild = value; } get { return targetBuild; } }
        public virtual bool EnableInteractiveDebugger { get { return movieOptions.DebuggerSupported(TargetBuild); } }
        public string[] AdditionalPaths; // temporary storage of resolved classpaths
		
		// we only provide getters for these to preserve the original pointer
        public MovieOptions MovieOptions { get { return movieOptions; } }
        public PathCollection Classpaths { get { return classpaths; } }
        public PathCollection CompileTargets { get { return compileTargets; } }
		public HiddenPathCollection HiddenPaths { get { return hiddenPaths; } }
        public AssetCollection LibraryAssets { get { return libraryAssets; } }
        public virtual String LibrarySWFPath { get { return OutputPath; } }
        public Dictionary<string, string> Storage { get { return storage; } }

        public CompilerOptions CompilerOptions
        {
            get { return compilerOptions; }
            set { compilerOptions = value; }
        }

		public PathCollection AbsoluteClasspaths
		{
			get
			{
                // property is accessed quite intensively, adding some caching here
                if (absClasspaths != null) return absClasspaths;

				PathCollection absolute = new PathCollection();
                foreach (string cp in classpaths)
                {
                    absolute.Add(GetAbsolutePath(cp));
                }
                absClasspaths = absolute;
				return absolute;
			}
		}

        public string[] SourcePaths { get { return classpaths.ToArray(); } }

		public string OutputPathAbsolute 
        {
            get { return GetAbsolutePath(OutputPath); } 
        }

        public string PreferredSDK
        {
            get { return preferredSDK; }
            set
            {
                preferredSDK = value;
                currentSDK = null;
            }
        }

        public string CurrentSDK
        {
            get { return currentSDK; }
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
				hiddenPaths.Add(path);				
				compileTargets.RemoveAtOrBelow(path); // can't compile hidden files
                libraryAssets.RemoveAtOrBelow(path); // can't embed hidden resources
			}
			else hiddenPaths.Remove(path);
		}

		public bool IsPathHidden(string path)
		{
			return hiddenPaths.IsHidden(GetRelativePath(path));
		}
		
		public virtual void SetCompileTarget(string path, bool isCompileTarget)
		{
            string relPath = Path.IsPathRooted(path) ? GetRelativePath(path) : path;
			if (isCompileTarget) compileTargets.Add(relPath);
			else compileTargets.Remove(relPath);
		}

        public virtual void SetDocumentClass(string path, bool isMain)
		{
            // to be implemented
		}

        public bool IsCompileTarget(string path) { return compileTargets.Contains(GetRelativePath(path)); }

        public virtual bool IsDocumentClass(string path) { return false; }

        public bool IsClassPath(string path) { return AbsoluteClasspaths.Contains(path); }

        public virtual void SetLibraryAsset(string path, bool isLibraryAsset)
        {
            string relPath = Path.IsPathRooted(path) ? GetRelativePath(path) : path;
            if (isLibraryAsset) libraryAssets.Add(relPath);
            else libraryAssets.Remove(relPath);
        }

        public virtual bool IsLibraryAsset(string path) { return libraryAssets.Contains(GetRelativePath(path)); }
        public virtual LibraryAsset GetAsset(string path) { return libraryAssets[GetRelativePath(path)]; }

        public virtual void ChangeAssetPath(string fromPath, string toPath)
        {
            if (IsLibraryAsset(fromPath))
            {
                LibraryAsset asset = libraryAssets[GetRelativePath(fromPath)];
                libraryAssets.Remove(asset);
                asset.Path = GetRelativePath(toPath);
                libraryAssets.Add(asset);
            }
        }

        public bool IsInput(string path) { return GetRelativePath(path) == InputPath; }
		public bool IsOutput(string path) { return GetRelativePath(path) == OutputPath; }

		/// <summary>
		/// Call this when you delete a path so we can remove all our references to it
		/// </summary>
		public void NotifyPathsDeleted(string path)
		{
			path = GetRelativePath(path);
			hiddenPaths.Remove(path);
			compileTargets.RemoveAtOrBelow(path);
            libraryAssets.RemoveAtOrBelow(path);
		}

		/// <summary>
		/// Returns the path to the "obj\" subdirectory, creating it if necessary.
		/// </summary>
		public string GetObjDirectory()
		{
			string objPath = Path.Combine(this.Directory, "obj");
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

        public String[] GetHiddenPaths()
        {
            return this.hiddenPaths.ToArray();
        }

		public string GetRelativePath(string path)
		{
			return ProjectPaths.GetRelativePath(this.Directory, path);
		}

        public void UpdateVars(bool silent)
        {
            if (!silent && ProjectUpdating != null) ProjectUpdating(this);
            vars = new BuildEventVars(this).GetVars();
        }

		public string GetAbsolutePath(string path)
		{
            path = Environment.ExpandEnvironmentVariables(path);
            if (vars != null && path.IndexOf('$') >= 0)
                foreach (BuildEventInfo arg in vars) 
                    path = path.Replace(arg.FormattedName, arg.Value);
            return ProjectPaths.GetAbsolutePath(this.Directory, path);
		}

        /// <summary>
        /// When in Release configuration, remove 'debug' from the given path.
        /// Pattern: ([a-zA-Z0-9])[-_.]debug([\\/.])
        /// </summary>
        public string FixDebugReleasePath(string path)
        {
            if (!TraceEnabled)
                return Regex.Replace(path, @"([a-zA-Z0-9])[-_.]debug([\\/.])", "$1$2");
            else
                return path;
        }

        /// <summary>
        /// Replace accented characters and remove whitespace
        /// </summary>
        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

		#endregion


        public bool IsDirectory(string path)
        {
            return System.IO.Directory.Exists(path);
        }
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
