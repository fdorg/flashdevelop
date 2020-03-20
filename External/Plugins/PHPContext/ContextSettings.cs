using System;
using System.ComponentModel;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Localization;

namespace PHPContext
{
    public delegate void ClasspathChangedEvent();

    [Serializable]
    public class ContextSettings : IContextSettings, InstalledSDKOwner
    {
        public event ClasspathChangedEvent OnClasspathChanged;

        #region IContextSettings Documentation

        const string LANGUAGE_WEBSITE = "http://php.net/manual";
        const string DEFAULT_DOC_COMMAND = "http://www.google.com/search?q=$(ItmTypPkg)+$(ItmTypName)+$(ItmName)+site:" + LANGUAGE_WEBSITE;
        protected string documentationCommandLine = DEFAULT_DOC_COMMAND;

        [DisplayName("Documentation Command Line")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.DocumentationCommandLine"), DefaultValue(DEFAULT_DOC_COMMAND)]
        public string DocumentationCommandLine
        {
            get => documentationCommandLine;
            set => documentationCommandLine = value;
        }

        #endregion

        #region IContextSettings Members

        const bool DEFAULT_CHECKSYNTAX = false;
        const bool DEFAULT_COMPLETIONENABLED = true;
        const bool DEFAULT_GENERATEIMPORTS = false;
        const bool DEFAULT_PLAY = false;
        const bool DEFAULT_LAZYMODE = false;
        const bool DEFAULT_LISTALL = true;
        const bool DEFAULT_QUALIFY = true;
        const bool DEFAULT_FIXPACKAGEAUTOMATICALLY = true;

        protected bool checkSyntaxOnSave = DEFAULT_CHECKSYNTAX;
        bool lazyClasspathExploration = DEFAULT_LAZYMODE;
        protected bool completionListAllTypes = DEFAULT_LISTALL;
        protected bool completionShowQualifiedTypes = DEFAULT_QUALIFY;
        protected bool completionEnabled = DEFAULT_COMPLETIONENABLED;
        protected bool generateImports = DEFAULT_GENERATEIMPORTS;
        protected bool playAfterBuild = DEFAULT_PLAY;
        protected bool fixPackageAutomatically = DEFAULT_FIXPACKAGEAUTOMATICALLY;
        protected string[] userClasspath = null;

        [Browsable(false)]
        public string LanguageId => "PHP";

        [Browsable(false)]
        public string DefaultExtension => ".php";

        [Browsable(false)]
        public string CheckSyntaxRunning => TextHelper.GetString("Info.PHPRunning");

        [Browsable(false)]
        public string CheckSyntaxDone => TextHelper.GetString("Info.PHPDone");

        [DisplayName("Check Syntax On Save")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CheckSyntaxOnSave"), DefaultValue(DEFAULT_CHECKSYNTAX)]
        public bool CheckSyntaxOnSave
        {
            get => checkSyntaxOnSave;
            set => checkSyntaxOnSave = value;
        }

        [DisplayName("User Classpath")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.UserClasspath")]
        public string[] UserClasspath
        {
            get => userClasspath;
            set
            {
                userClasspath = value;
                FireChanged();
            }
        }

        [DisplayName("Enable Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionEnabled"), DefaultValue(DEFAULT_COMPLETIONENABLED)]
        public bool CompletionEnabled
        {
            get => completionEnabled;
            set => completionEnabled = value;
        }

        [DisplayName("Generate Imports")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.GenerateImports"), DefaultValue(DEFAULT_GENERATEIMPORTS)]
        public bool GenerateImports
        {
            get => generateImports;
            set => generateImports = value;
        }

        /// <summary>
        /// In completion, show all known types in project
        /// </summary>
        [DisplayName("List All Types In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionListAllTypes"), DefaultValue(DEFAULT_LISTALL)]
        public bool CompletionListAllTypes
        {
            get => completionListAllTypes;
            set => completionListAllTypes = value;
        }

        /// <summary>
        /// In completion, show qualified type names (package + type)
        /// </summary>
        [DisplayName("Show QualifiedTypes In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionShowQualifiedTypes"), DefaultValue(DEFAULT_QUALIFY)]
        public bool CompletionShowQualifiedTypes
        {
            get => completionShowQualifiedTypes;
            set => completionShowQualifiedTypes = value;
        }

        /// <summary>
        /// Defines if each classpath is explored immediately (PathExplorer) 
        /// </summary>
        [DisplayName("Lazy Classpath Exploration")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.LazyClasspathExploration"), DefaultValue(DEFAULT_LAZYMODE)]
        public bool LazyClasspathExploration
        {
            get => lazyClasspathExploration;
            set => lazyClasspathExploration = value;
        }

        [DisplayName("Play After Build")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.PlayAfterBuild"), DefaultValue(DEFAULT_PLAY)]
        public bool PlayAfterBuild
        {
            get => playAfterBuild;
            set => playAfterBuild = value;
        }

        [DisplayName("Fix Package Automatically")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.FixPackageAutomatically"), DefaultValue(DEFAULT_FIXPACKAGEAUTOMATICALLY)]
        public bool FixPackageAutomatically
        {
            get => fixPackageAutomatically;
            set => fixPackageAutomatically = value;
        }

        #endregion

        #region Language specific members

        string intrinsicPath;

        [DisplayName("Intrinsic Definitions")]
        [DefaultValue("Library\\PHP\\intrinsic")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("PHPContext.Description.IntrinsicDefinitions")]
        public string LanguageDefinitions
        {
            get => intrinsicPath;
            set
            {
                if (value == intrinsicPath) return;
                intrinsicPath = value;
                FireChanged();
            }
        }

        #endregion

        #region Interface Implementations

        public InstalledSDK GetDefaultSDK() => null;

        public bool ValidateSDK(InstalledSDK sdk) => true;

        [Browsable(false)]
        public InstalledSDK[] InstalledSDKs
        {
            get => Array.Empty<InstalledSDK>();
            set { /* Do nothing..*/ }
        }

        #endregion
        
        [Browsable(false)]
        void FireChanged() => OnClasspathChanged?.Invoke();

        [
            DisplayName("Always Add Space After"),
            LocalizedCategory("ASCompletion.Category.Helpers"),
            LocalizedDescription("ASCompletion.Description.AddSpaceAfter"),
            DefaultValue(""),
        ]
        public string AddSpaceAfter { get; set; } = string.Empty;
    }
}