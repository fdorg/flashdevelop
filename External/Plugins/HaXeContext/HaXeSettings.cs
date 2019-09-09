// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Drawing.Design;
using PluginCore.Localization;
using ProjectManager.Projects.Haxe;
using PluginCore;

namespace HaXeContext
{
    public delegate void ClasspathChangedEvent();
    public delegate void CompletionModeChangedEventHandler();
    public delegate void UseGenericsShortNotationChangedEventHandler();

    [Serializable]
    public class HaXeSettings : ASCompletion.Settings.IContextSettings
    {
        [field: NonSerialized]
        public event ClasspathChangedEvent OnClasspathChanged;

        #region IContextSettings Documentation

        const string DEFAULT_DOC_COMMAND = "http://www.google.com/search?q=$(ItmTypPkg)+$(ItmTypName)+$(ItmName)+Haxe";
        protected string documentationCommandLine = DEFAULT_DOC_COMMAND;

        [DisplayName("Documentation Command Line")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.DocumentationCommandLine"), DefaultValue(DEFAULT_DOC_COMMAND)]
        public string DocumentationCommandLine
        {
            get => documentationCommandLine;
            set => documentationCommandLine = value;
        }
        
        bool disableTypeDeclaration = false;

        [DisplayName("Disable type declaration for variables")]
        [LocalizedCategory("ASCompletion.Category.Generation")]
        [DefaultValue(false)]
        public bool DisableTypeDeclaration
        {
            get => disableTypeDeclaration;
            set => disableTypeDeclaration = value;
        }
        
        bool disableVoidTypeDeclaration = false;

        [DisplayName("Disable void type declaration for functions")]
        [LocalizedCategory("ASCompletion.Category.Generation")]
        [DefaultValue(false)]
        public bool DisableVoidTypeDeclaration
        {
            get => disableVoidTypeDeclaration;
            set => disableVoidTypeDeclaration = value;
        }

        bool enableLeadingAsterisks = true;

        [DisplayName("Enable Leading Asterisks")]
        [Category("Documentation Generator")]
        [DefaultValue(true)]
        public bool EnableLeadingAsterisks
        {
            get => enableLeadingAsterisks;
            set => enableLeadingAsterisks = value;
        }

        #endregion

        #region IContextSettings Members

        const bool DEFAULT_CHECKSYNTAX = false;
        const bool DEFAULT_COMPLETIONENABLED = true;
        const bool DEFAULT_GENERATEIMPORTS = true;
        const bool DEFAULT_PLAY = true;
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
        protected InstalledSDK[] installedSDKs = null;

        [Browsable(false)]
        public string LanguageId => "HAXE";

        [Browsable(false)]
        public string DefaultExtension => ".hx";

        [Browsable(false)]
        public string CheckSyntaxRunning => TextHelper.GetString("Info.HaXeRunning");

        [Browsable(false)]
        public string CheckSyntaxDone => TextHelper.GetString("Info.HaXeDone");

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

        [DisplayName("Installed Haxe SDKs")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.HaXePath")]
        public InstalledSDK[] InstalledSDKs
        {
            get => installedSDKs;
            set
            {
                installedSDKs = value;
                FireChanged();
            }
        }

        public InstalledSDK GetDefaultSDK()
        {
            if (installedSDKs.IsNullOrEmpty()) return InstalledSDK.INVALID_SDK;
            foreach (InstalledSDK sdk in installedSDKs)
                if (sdk.IsValid) return sdk;
            return InstalledSDK.INVALID_SDK;
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
        [DisplayName("Show Qualified Types In Completion")]
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

        #region Haxe specific members

        [field: NonSerialized]
        public event CompletionModeChangedEventHandler CompletionModeChanged;

        [field: NonSerialized]
        public event UseGenericsShortNotationChangedEventHandler UseGenericsShortNotationChanged;

        const int DEFAULT_COMPLETION_SERVER_PORT = 6000;
        const int DEFAULT_FLASHVERSION = 10;
        const string DEFAULT_HAXECHECKPARAMS = "";
        const HaxeCompletionModeEnum DEFAULT_HAXECOMPLETIONMODE = HaxeCompletionModeEnum.Compiler;

        const bool DEFAULT_DISABLEMIXEDCOMPLETION = false;
        const bool DEFAULT_DISABLECOMPLETIONONDEMAND = true;
        const bool DEFAULT_EXPORTHXML = false;
        const bool DEFAULT_DISABLE_LIB_INSTALLATION = true;
        const bool DEFAULT_USEGENERICSSHORTNOTATION = true;
        const CompletionFeatures DEFAULT_ENABLEDCOMPILERSERVICES = CompletionFeatures.Diagnostics | CompletionFeatures.DisplayStdIn | CompletionFeatures.Usage | CompletionFeatures.EnableForFindAllReferences;

        int completionServerPort = DEFAULT_COMPLETION_SERVER_PORT;
        int flashVersion = 10;
        string haXeCheckParameters = DEFAULT_HAXECHECKPARAMS;
        bool disableMixedCompletion = DEFAULT_DISABLEMIXEDCOMPLETION;
        bool disableCompletionOnDemand = DEFAULT_DISABLECOMPLETIONONDEMAND;
        bool exportHXML = DEFAULT_EXPORTHXML;
        HaxeCompletionModeEnum _completionMode = DEFAULT_HAXECOMPLETIONMODE;
        bool disableLibInstallation = DEFAULT_DISABLE_LIB_INSTALLATION;
        bool useGenericsShortNotation = DEFAULT_USEGENERICSSHORTNOTATION;

        [DisplayName("Default Flash Version")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.DefaultFlashVersion"), DefaultValue(DEFAULT_FLASHVERSION)]
        public int DefaultFlashVersion
        {
            get => flashVersion;
            set
            {
                if (value == flashVersion) return;
                if (value >= 6 && value <= 12)
                {
                    flashVersion = value;
                    FireChanged();
                }
            }
        }

        [DisplayName("Check Parameters")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.HaXeCheckParameters"), DefaultValue(DEFAULT_HAXECHECKPARAMS)]
        public string HaXeCheckParameters
        {
            get => haXeCheckParameters;
            set => haXeCheckParameters = value;
        }

        [DisplayName("Completion Mode")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.CompletionMode"), DefaultValue(DEFAULT_HAXECOMPLETIONMODE)]
        public HaxeCompletionModeEnum CompletionMode
        {
            get => _completionMode;
            set
            {
                _completionMode = value;
                FireCompletionMode();
            }
        }

        [DisplayName("Completion Server Port")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.CompletionServerPort"), DefaultValue(DEFAULT_COMPLETION_SERVER_PORT)]
        public int CompletionServerPort
        {
            get => completionServerPort;
            set
            {
                completionServerPort = value;
                FireCompletionMode();
            }
        }

        [DisplayName("Disable Mixed Completion")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.DisableMixedCompletion"), DefaultValue(DEFAULT_DISABLEMIXEDCOMPLETION)]
        public bool DisableMixedCompletion
        {
            get => disableMixedCompletion;
            set => disableMixedCompletion = value;
        }

        [DisplayName("Disable Completion On Demand")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.DisableCompletionOnDemand"), DefaultValue(DEFAULT_DISABLECOMPLETIONONDEMAND)]
        public bool DisableCompletionOnDemand
        {
            get => disableCompletionOnDemand;
            set => disableCompletionOnDemand = value;
        }

        [DisplayName("Export HXML")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.ExportHXML"), DefaultValue(DEFAULT_EXPORTHXML)]
        public bool ExportHXML
        {
            get => exportHXML;
            set => HaxeProject.saveHXML = exportHXML = value;
        }

        /// <summary>
        /// A flag enum of enabled compiler features.
        /// It should never actually be set to null (only 0).
        /// It is only nullable because otherwise the deserializer will set it to 0 by default (instead of everything enabled)
        /// </summary>
        [DisplayName("Enabled Compiler Services")]
        [LocalizedCategory("ASCompletion.Category.Language"),
         LocalizedDescription("HaXeContext.Description.EnabledCompilerServices"),
         DefaultValue(DEFAULT_ENABLEDCOMPILERSERVICES)]
        [Editor(typeof(Helpers.FlagEnumEditor),
            typeof(UITypeEditor))]
        public CompletionFeatures? EnabledFeatures { get; set; } = DEFAULT_ENABLEDCOMPILERSERVICES;

        [DisplayName("Disable Automatic Libraries Installation")]
        [DefaultValue(DEFAULT_DISABLE_LIB_INSTALLATION)]
        public bool DisableLibInstallation
        {
            get => disableLibInstallation;
            set => disableLibInstallation = value;
        }

        [DisplayName("Use Short Notation For Generics")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.UseGenericsShortNotation"), DefaultValue(DEFAULT_USEGENERICSSHORTNOTATION)]
        public bool UseGenericsShortNotation
        {
            get => useGenericsShortNotation;
            set
            {
                if (value != useGenericsShortNotation)
                {
                    useGenericsShortNotation = value;
                    UseGenericsShortNotationChanged?.Invoke();
                }
            }
        }

        [DisplayName("Maximum Number Of Diagnostics Processes")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("HaXeContext.Description.MaximumDiagnosticsProcesses"), DefaultValue(1)]
        public int MaximumDiagnosticsProcesses { get; set; } = 1;

        #endregion

        [Browsable(false)]
        void FireChanged() => OnClasspathChanged?.Invoke();

        [Browsable(false)]
        void FireCompletionMode() => CompletionModeChanged?.Invoke();

        [Browsable(false)]
        public void Init() => HaxeProject.saveHXML = exportHXML;
    }

    public enum HaxeCompletionModeEnum
    {
        FlashDevelop,
        Compiler,
        CompletionServer
    }

    [Flags]
    public enum CompletionFeatures
    {
        Diagnostics = 1,
        Usage = 2,
        DisplayStdIn = 4,
        EnableForFindAllReferences = 8
    }
}
