using System;
using System.ComponentModel;
using System.Drawing.Design;
using ASCompletion.Settings;
using Ookii.Dialogs;
using PluginCore;
using PluginCore.Localization;

namespace AS2Context
{
    public delegate void ClasspathChangedEvent();

    [Serializable]
    public class AS2Settings : IContextSettings
    {
        [field: NonSerialized]
        public event ClasspathChangedEvent OnClasspathChanged;

        #region IContextSettings Documentation

        const string DEFAULT_DOC_COMMAND = 
            "http://google.com/search?q=%22actionscript 2.0%22+$(ItmTypPkg)+$(ItmTypName)+$(ItmName)"
            + "+help.adobe.com+livedocs.adobe.com";
        protected string documentationCommandLine = DEFAULT_DOC_COMMAND;

        [DisplayName("Documentation Command Line")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.DocumentationCommandLine"), DefaultValue(DEFAULT_DOC_COMMAND)]
        public string DocumentationCommandLine
        {
            get { return documentationCommandLine; }
            set { documentationCommandLine = value; }
        }

        #endregion

        #region OLD
        // TODO  Finish integrating "old" AS2 settings
        /* 
            static readonly protected string[] SETTING_ALWAYS_SHOW_INTRINSIC_MEMBERS = {
                "ASCompletion.AS2.IntrinsicMembers.AlwaysShow"
            };
            static readonly protected string[] SETTING_HIDE_INTRINSIC_MEMBERS = {
                "ASCompletion.AS2.IntrinsicMembers.Hide"
            };
            static readonly protected string[] SETTING_CMD_CHECKPARAMS = {
                "ASCompletion.AS2.MTASC.CheckParameters"
            };
            static readonly protected string[] SETTING_CMD_PATH = {
                "ASCompletion.AS2.MTASC.Path"
            };
            static readonly protected string[] SETTING_CMD_RUNAFTERBUILD = {
                "ASCompletion.AS2.MTASC.RunAfterBuild"
            };
            static readonly protected string[] DEFAULT_CMD_PARAMS = {
                "-mx"
            };
         */
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
        private bool lazyClasspathExploration = DEFAULT_LAZYMODE;
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
            get { return "AS2"; }
        }

        [Browsable(false)]
        public string DefaultExtension 
        {
            get { return ".as"; }
        }

        [Browsable(false)]
        public string CheckSyntaxRunning
        {
            get { return TextHelper.GetString("Info.MTASCRunning"); }
        }

        [Browsable(false)]
        public string CheckSyntaxDone
        {
            get { return TextHelper.GetString("Info.MTASCDone"); }
        }

        [DisplayName("Check Syntax On Save")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CheckSyntaxOnSave"), DefaultValue(DEFAULT_CHECKSYNTAX)]
        public bool CheckSyntaxOnSave
        {
            get { return checkSyntaxOnSave; }
            set { checkSyntaxOnSave = value; }
        }

        [DisplayName("User Classpath")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.UserClasspath")]
        public string[] UserClasspath
        {
            get { return userClasspath; }
            set
            {
                userClasspath = value;
                FireChanged();
            }
        }

        [DisplayName("Installed MTASC SDKs")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS2Context.Description.MtascPath")]
        public InstalledSDK[] InstalledSDKs
        {
            get { return installedSDKs; }
            set
            {
                installedSDKs = value;
                FireChanged();
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

        /// <summary>
        /// In completion, show all known types in project
        /// </summary>
        [DisplayName("List All Types In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionListAllTypes"), DefaultValue(DEFAULT_LISTALL)]
        public bool CompletionListAllTypes
        {
            get { return completionListAllTypes; }
            set { completionListAllTypes = value; }
        }

        /// <summary>
        /// In completion, show qualified type names (package + type)
        /// </summary>
        [DisplayName("Show Qualified Types In Completion")]
        [LocalizedCategory("ASCompletion.Category.Common"), LocalizedDescription("ASCompletion.Description.CompletionShowQualifiedTypes"), DefaultValue(DEFAULT_QUALIFY)]
        public bool CompletionShowQualifiedTypes
        {
            get { return completionShowQualifiedTypes; }
            set { completionShowQualifiedTypes = value; }
        }

        /// <summary>
        /// Defines if each classpath is explored immediately (PathExplorer) 
        /// </summary>
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

        #region AS2 specific settings

        const bool DEFAULT_USEMTASC = true;
        const int DEFAULT_FLASHVERSION = 9; // Flash CS3 has a specific FP9 support for AS2
        const string DEFAULT_MTASCCHECKPARAMS = "-mx -wimp";

        private int flashVersion = 9;
        private string mmClassPath;
        private bool useMtascIntrinsic = DEFAULT_USEMTASC;
        private string mtascCheckParameters = DEFAULT_MTASCCHECKPARAMS;

        [DisplayName("Use MTASC Intrinsics")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS2Context.Description.UseMtascIntrinsic"), DefaultValue(DEFAULT_USEMTASC)]
        public bool UseMtascIntrinsic
        {
            get { return useMtascIntrinsic; }
            set
            {
                useMtascIntrinsic = value;
                FireChanged();
            }
        }

        [DisplayName("Check Parameters")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS2Context.Description.MtascCheckParameters"), DefaultValue(DEFAULT_MTASCCHECKPARAMS)]
        public string MtascCheckParameters
        {
            get { return mtascCheckParameters; }
            set { mtascCheckParameters = value; }
        }

        [DisplayName("Flash IDE Classpath")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS2Context.Description.MMClassPath")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string MMClassPath
        {
            get { return mmClassPath; }
            set {
                if (value == mmClassPath) return;
                mmClassPath = value;
                FireChanged();
            }
        }

        [DisplayName("Default Flash Version")]
        [LocalizedCategory("ASCompletion.Category.Language"), LocalizedDescription("AS2Context.Description.DefaultFlashVersion"), DefaultValue(DEFAULT_FLASHVERSION)]
        public int DefaultFlashVersion
        {
            get { return flashVersion; }
            set {
                if (value == flashVersion) return;
                if (value >= 6 && value <= 9)
                {
                    flashVersion = value;
                    FireChanged();
                }
            }
        }

        #endregion

        [Browsable(false)]
        private void FireChanged()
        {
            if (OnClasspathChanged != null) OnClasspathChanged();
        }
    }
}
