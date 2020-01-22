// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using ASCompletion.Context;
using PluginCore.Localization;
using PluginCore;

namespace UnityContext
{
    public delegate void ClasspathChangedEvent();

    [Serializable]
    public class UnitySettings : ASCompletion.Settings.IContextSettings
    {
        [field: NonSerialized]
        public event ClasspathChangedEvent OnClasspathChanged;

        #region IContextSettings Documentation

        const string DEFAULT_DOC_COMMAND = 
            "http://google.com/search?q=unityscript+$(ItmTypPkg)+$(ItmTypName)+$(ItmName)"
            + "+unity3d.com";
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
        const bool DEFAULT_FIXPACKAGEAUTOMATICALLY = false;

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
            get { return "UnityScript"; }
        }

        [Browsable(false)]
        public string DefaultExtension 
        {
            get { return ".js"; }
        }

        [Browsable(false)]
        public string CheckSyntaxRunning
        {
            get { return /*TextHelper.GetString("Info.MTASCRunning")*/"Checking syntax"; }
        }

        [Browsable(false)]
        public string CheckSyntaxDone
        {
            get { return /*TextHelper.GetString("Info.MTASCDone")*/"Done"; }
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

        [DisplayName("Installed Unity SDKs")]
        [LocalizedCategory("ASCompletion.Category.Language"), Description("Path to Unity3D installation")
        /*LocalizedDescription("AS2Context.Description.MtascPath")*/]
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

        #region Unity specific settings

        private string unityVersion;
        private const string DEFAULT_UNITYVERSION = "3.3";

        [DisplayName("Default Unity3D Version")]
        [LocalizedCategory("ASCompletion.Category.Language"), Description("Unity3D API Version")
        /*LocalizedDescription("AS2Context.Description.DefaultFlashVersion")*/, DefaultValue(DEFAULT_UNITYVERSION)]
        public string DefaultVersion
        {
            get { return unityVersion; }
            set {
                if (value == unityVersion) return;
                unityVersion = value;
                FireChanged();
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
