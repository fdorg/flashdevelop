using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ProjectManager.Projects.AS3
{
    [Serializable]
    public class MxmlcOptions : CompilerOptions
    {
        #region Compiler Options

        bool accessible = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Enable Accessibility Features")]
        [LocalizedDescription("ProjectManager.Description.Accessible")]
        [DefaultValue(false)]
        public bool Accessible {
            get => accessible;
            set => accessible = value;
        }

        bool allowSourcePathOverlap = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Allow Source Path Overlap")]
        [LocalizedDescription("ProjectManager.Description.AllowSourcePathOverlap")]
        [DefaultValue(false)]
        public bool AllowSourcePathOverlap {
            get => allowSourcePathOverlap;
            set => allowSourcePathOverlap = value;
        }

        bool benchmark = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Benchmark Results")]
        [LocalizedDescription("ProjectManager.Description.Benchmark")]
        [DefaultValue(false)]
        public bool Benchmark {
            get => benchmark;
            set => benchmark = value;
        }

        bool es = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Use ECMAScript Object Model")]
        [LocalizedDescription("ProjectManager.Description.ES")]
        [DefaultValue(false)]
        public bool ES {
            get => es;
            set => es = value;
        }
        
        string loadConfig = "";
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Load Configuration File")]
        [LocalizedDescription("ProjectManager.Description.LoadConfig")]
        [DefaultValue("")]
        public string LoadConfig {
            get => loadConfig;
            set => loadConfig = value;
        }

        string locale = "";
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Language Locale")]
        [LocalizedDescription("ProjectManager.Description.Locale")]
        [DefaultValue("")]
        public string Locale {
            get => locale;
            set => locale = value;
        }

        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("MXML Namespaces")]
        [LocalizedDescription("ProjectManager.Description.Namespaces")]
        [DefaultValue(null)]
        public MxmlNamespace[] Namespaces { get; set; }

        bool optimize = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Optimize Bytecode")]
        [LocalizedDescription("ProjectManager.Description.Optimize")]
        [DefaultValue(false)]
        public bool Optimize {
            get => optimize;
            set => optimize = value;
        }

        bool omitTraces = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Omit Trace Statements")]
        [LocalizedDescription("ProjectManager.Description.OmitTraces")]
        [DefaultValue(true)]
        public bool OmitTraces {
            get => omitTraces;
            set => omitTraces = value;
        }

        bool showBindingWarnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Binding Warnings")]
        [LocalizedDescription("ProjectManager.Description.ShowBindingWarnings")]
        [DefaultValue(true)]
        public bool ShowBindingWarnings {
            get => showBindingWarnings;
            set => showBindingWarnings = value;
        }

        bool showInvalidCSS = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Invalid CSS Property Warnings")]
        [LocalizedDescription("ProjectManager.Description.ShowInvalidCSS")]
        [DefaultValue(true)]
        public bool ShowInvalidCSS {
            get => showInvalidCSS;
            set => showInvalidCSS = value;
        }

        bool showActionScriptWarnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Actionscript Warnings")]
        [LocalizedDescription("ProjectManager.Description.ShowActionScriptWarnings")]
        [DefaultValue(true)]
        public bool ShowActionScriptWarnings {
            get => showActionScriptWarnings;
            set => showActionScriptWarnings = value;
        }

        bool showDeprecationWarnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Deprecation Warnings")]
        [LocalizedDescription("ProjectManager.Description.ShowDeprecationWarnings")]
        [DefaultValue(true)]
        public bool ShowDeprecationWarnings {
            get => showDeprecationWarnings;
            set => showDeprecationWarnings = value;
        }

        bool showUnusedTypeSelectorWarnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Show Unused Type Selector Warnings")]
        [LocalizedDescription("ProjectManager.Description.ShowUnusedTypeSelectorWarnings")]
        [DefaultValue(true)]
        public bool ShowUnusedTypeSelectorWarnings {
            get => showUnusedTypeSelectorWarnings;
            set => showUnusedTypeSelectorWarnings = value;
        }

        bool strict = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Enable Strict Mode")]
        [LocalizedDescription("ProjectManager.Description.StrictAS3")]
        [DefaultValue(true)]
        public bool Strict {
            get => strict;
            set => strict = value;
        }

        bool useResourceBundleMetadata = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Use Resource Bundle Metadata")]
        [LocalizedDescription("ProjectManager.Description.UseResourceBundleMetadata")]
        [DefaultValue(true)]
        public bool UseResourceBundleMetadata {
            get => useResourceBundleMetadata;
            set => useResourceBundleMetadata = value;
        }

        bool verboseStackTraces = false;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Verbose Stack Traces")]
        [LocalizedDescription("ProjectManager.Description.VerboseStackTraces")]
        [DefaultValue(false)]
        public bool VerboseStackTraces {
            get => verboseStackTraces;
            set => verboseStackTraces = value;
        }

        bool warnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Enable All Warnings")]
        [LocalizedDescription("ProjectManager.Description.Warnings")]
        [DefaultValue(true)]
        public bool Warnings {
            get => warnings;
            set => warnings = value;
        }

        #endregion

        #region Advanced options

        string[] additional = Array.Empty<string>();
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Additional Compiler Options")]
        [LocalizedDescription("ProjectManager.Description.Additional")]
        [DefaultValue(new string[] { })]
        public string[] Additional
        {
            get => additional;
            set => additional = value;
        }

        bool advancedTelemetry = false;
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Advanced Telemetry")]
        [LocalizedDescription("ProjectManager.Description.AdvancedTelemetry")]
        [DefaultValue(false)]
        public bool AdvancedTelemetry
        {
            get => advancedTelemetry;
            set => advancedTelemetry = value;
        }

        string advancedTelemetryPassword = "";
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Advanced Telemetry Password")]
        [LocalizedDescription("ProjectManager.Description.AdvancedTelemetryPassword")]
        [DefaultValue("")]
        public string AdvancedTelemetryPassword
        {
            get => advancedTelemetryPassword;
            set => advancedTelemetryPassword = value;
        }

        bool inlineFunctions = false;
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Inline Functions")]
        [LocalizedDescription("ProjectManager.Description.InlineFunctions")]
        [DefaultValue(false)]
        public bool InlineFunctions {
            get => inlineFunctions;
            set => inlineFunctions = value;
        }

        string[] compilerConstants = Array.Empty<string>();
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Compiler Constants")]
        [LocalizedDescription("ProjectManager.Description.CompilerConstants")]
        [DefaultValue(new string[] { })]
        public string[] CompilerConstants {
            get => compilerConstants;
            set => compilerConstants = value;
        }

        string minorVersion = "";
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Player Minor Version")]
        [LocalizedDescription("ProjectManager.Description.MinorVersion")]
        [DefaultValue("")]
        public string MinorVersion
        {
            get => minorVersion;
            set => minorVersion = value;
        }

        string[] intrinsicPaths = Array.Empty<string>();
        [DisplayName("Intrinsic Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.IntrinsicPaths")]
        [DefaultValue(new string[] { })]
        public string[] IntrinsicPaths
        {
            get => intrinsicPaths;
            set => intrinsicPaths = value;
        }

        string[] externalLibraryPaths = Array.Empty<string>();
        [DisplayName("External Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.ExternalLibraryPaths")]
        [DefaultValue(new string[] { })]
        public string[] ExternalLibraryPaths
        {
            get => externalLibraryPaths;
            set => externalLibraryPaths = value;
        }

        string[] includeLibraries = Array.Empty<string>();
        [DisplayName("SWC Include Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.IncludeLibraries")]
        [DefaultValue(new string[] { })]
        public string[] IncludeLibraries
        {
            get => includeLibraries;
            set => includeLibraries = value;
        }
        
        string[] libraryPaths = Array.Empty<string>();
        [DisplayName("SWC Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.LibraryPaths")]
        [DefaultValue(new string[] { })]
        public string[] LibraryPaths
        {
            get => libraryPaths;
            set => libraryPaths = value;
        }

        string[] rslPaths = Array.Empty<string>();
        [DisplayName("Runtime Shared Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.RSLPaths")]
        [DefaultValue(new string[] { })]
        public string[] RSLPaths
        {
            get => rslPaths;
            set => rslPaths = value;
        }

        bool useNetwork = true;
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Use Network Services")]
        [LocalizedDescription("ProjectManager.Description.UseNetwork")]
        [DefaultValue(true)]
        public bool UseNetwork {
            get => useNetwork;
            set => useNetwork = value;
        }

        string linkReport = "";
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Link Report")]
        [LocalizedDescription("ProjectManager.Description.LinkReport")]
        [DefaultValue("")]
        public string LinkReport {
            get => linkReport;
            set => linkReport = value;
        }

        string loadExterns = "";
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Load Externs")]
        [LocalizedDescription("ProjectManager.Description.LoadExterns")]
        [DefaultValue("")]
        public string LoadExterns {
            get => loadExterns;
            set => loadExterns = value;
        }

        bool staticLinkRSL = true;
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Static Link RSL")]
        [LocalizedDescription("ProjectManager.Description.StaticLinkRSL")]
        [DefaultValue(true)]
        public bool StaticLinkRSL {
            get => staticLinkRSL;
            set => staticLinkRSL = value;
        }

        #endregion
    }
}