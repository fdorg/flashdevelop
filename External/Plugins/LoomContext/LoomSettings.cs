using System;
using System.ComponentModel;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Localization;

namespace LoomContext
{
    public delegate void ClasspathChangedEvent();
    public delegate void InstalledSDKsChangedEvent();

    [Serializable]
    public class LoomSettings : IContextSettings
    {
        [field: NonSerialized]
        public event ClasspathChangedEvent OnClasspathChanged;

        [field: NonSerialized]
        public event InstalledSDKsChangedEvent OnInstalledSDKsChanged;

        #region IContextSettings Documentation

        const string DEFAULT_DOC_COMMAND =
            "http://google.com/search?q=%22loomscript%22+$(ItmTypPkg)+$(ItmTypName)+$(ItmName)"
            + "+theengine.co";
        protected string documentationCommandLine = DEFAULT_DOC_COMMAND;

        [DisplayName("Documentation Command Line")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.DocumentationCommandLine"), DefaultValue(DEFAULT_DOC_COMMAND)]
        public string DocumentationCommandLine
        {
            get { return documentationCommandLine; }
            set { documentationCommandLine = value; }
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
        const string DEFAULT_LOOMLIBRARY = @"Library\Loom\intrinsic";

        protected bool checkSyntaxOnSave = DEFAULT_CHECKSYNTAX;
        protected bool lazyClasspathExploration = DEFAULT_LAZYMODE;
        protected bool completionListAllTypes = DEFAULT_LISTALL;
        protected bool completionShowQualifiedTypes = DEFAULT_QUALIFY;
        protected bool completionEnabled = DEFAULT_COMPLETIONENABLED;
        protected bool generateImports = DEFAULT_GENERATEIMPORTS;
        protected bool playAfterBuild = DEFAULT_PLAY;
        protected bool fixPackageAutomatically = DEFAULT_FIXPACKAGEAUTOMATICALLY;
        protected string[] userClasspath = null;
        protected InstalledSDK[] installedSDKs = null;

        [Browsable(false)]
        public string LanguageId
        {
            get { return "loom"; }
        }

        [Browsable(false)]
        public string DefaultExtension
        {
            get { return ".ls"; }
        }

        [Browsable(false)]
        public string CheckSyntaxRunning
        {
            get { return TextHelper.GetString("Info.LoomRunning"); }
        }

        [Browsable(false)]
        public string CheckSyntaxDone
        {
            get { return TextHelper.GetString("Info.LoomDone"); }
        }

        [DisplayName("Check Syntax On Save")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CheckSyntaxOnSave"), DefaultValue(DEFAULT_CHECKSYNTAX)]
        public bool CheckSyntaxOnSave
        {
            get { return checkSyntaxOnSave; }
            set { checkSyntaxOnSave = value; }
        }

        [DisplayName("User Classpath")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.UserClasspath"), DefaultValue(DEFAULT_LOOMLIBRARY)]
        public string[] UserClasspath
        {
            get { return userClasspath; }
            set
            {
                userClasspath = value;
                FireChanged();
            }
        }

        [DisplayName("Installed Loom SDKs")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("LoomContext.Description.SDKs")]
        public InstalledSDK[] InstalledSDKs
        {
            get { return installedSDKs; }
            set
            {
                installedSDKs = value;
                FireChanged();
                if (OnInstalledSDKsChanged != null) OnInstalledSDKsChanged();
            }
        }

        public InstalledSDK GetDefaultSDK()
        {
            if (installedSDKs == null || installedSDKs.Length == 0) 
                return InstalledSDK.INVALID_SDK;

            foreach (InstalledSDK sdk in installedSDKs)
                if (sdk.IsValid) return sdk;
            return InstalledSDK.INVALID_SDK;
        }

        [DisplayName("Enable Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionEnabled"), DefaultValue(DEFAULT_COMPLETIONENABLED)]
        public bool CompletionEnabled
        {
            get { return completionEnabled; }
            set { completionEnabled = value; }
        }

        [DisplayName("Generate Imports")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.GenerateImports"), DefaultValue(DEFAULT_GENERATEIMPORTS)]
        public bool GenerateImports
        {
            get { return generateImports; }
            set { generateImports = value; }
        }

        [DisplayName("List All Types In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionListAllTypes"), DefaultValue(DEFAULT_LISTALL)]
        public bool CompletionListAllTypes
        {
            get { return completionListAllTypes; }
            set { completionListAllTypes = value; }
        }

        [DisplayName("Show Qualified Types In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionShowQualifiedTypes"), DefaultValue(DEFAULT_QUALIFY)]
        public bool CompletionShowQualifiedTypes
        {
            get { return completionShowQualifiedTypes; }
            set { completionShowQualifiedTypes = value; }
        }

        [DisplayName("Lazy Classpath Exploration")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.LazyClasspathExploration"), DefaultValue(DEFAULT_LAZYMODE)]
        public bool LazyClasspathExploration
        {
            get { return lazyClasspathExploration; }
            set { lazyClasspathExploration = value; }
        }

        [DisplayName("Play After Build")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.PlayAfterBuild"), DefaultValue(DEFAULT_PLAY)]
        public bool PlayAfterBuild
        {
            get { return playAfterBuild; }
            set { playAfterBuild = value; }
        }

        [DisplayName("Fix Package Automatically")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.FixPackageAutomatically"), DefaultValue(DEFAULT_FIXPACKAGEAUTOMATICALLY)]
        public bool FixPackageAutomatically
        {
            get { return fixPackageAutomatically; }
            set { fixPackageAutomatically = value; }
        }

        #endregion

        #region Loom specific members

        /*const string DEFAULT_FLASHVERSION = "10.1";

        private string flashVersion = DEFAULT_FLASHVERSION;
        private string as3ClassPath;
        private string[] as3FileTypes;

        [DisplayName("Default Flash Version")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS3Context.Description.DefaultFlashVersion"), DefaultValue(DEFAULT_FLASHVERSION)]
        public string DefaultFlashVersion
        {
            get { return flashVersion ?? DEFAULT_FLASHVERSION; }
            set
            {
                if (value == flashVersion) return;
                flashVersion = value;
                FireChanged();
            }
        }

        [DisplayName("AS3 Classpath")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS3Context.Description.AS3Classpath"), DefaultValue(DEFAULT_AS3LIBRARY)]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string AS3ClassPath
        {
            get { return as3ClassPath; }
            set
            {
                if (value == as3ClassPath) return;
                as3ClassPath = value;
                FireChanged();
            }
        }

        [DisplayName("AS3 File Types")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS3Context.Description.AS3FileTypes")]
        public string[] AS3FileTypes
        {
            get { return as3FileTypes; }
            set
            {
                if (value == as3FileTypes) return;
                as3FileTypes = value;
                FireChanged();
            }
        }*/
        #endregion

        private void FireChanged()
        {
            if (OnClasspathChanged != null) OnClasspathChanged();
        }
    }
}
