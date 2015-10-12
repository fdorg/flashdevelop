using PluginCore;

namespace ASCompletion.Settings
{
    public interface IContextSettings
    {
        /// <summary>
        /// Language short name (ie. AS2, AS3, JS, etc)
        /// </summary>
        string LanguageId { get; }

        /// <summary>
        /// Default file extension of the language, including dot
        /// </summary>
        string DefaultExtension { get; }

        /// <summary>
        /// Command to execute for help search
        /// </summary>
        string DocumentationCommandLine { get; set; }

        /// <summary>
        /// Completion engine enabled
        /// </summary>
        bool CompletionEnabled { get; set; }

        /// <summary>
        /// User global classpath
        /// </summary>
        string[] UserClasspath { get; set; }

        /// <summary>
        /// Imports statements automatic generation
        /// </summary>
        bool GenerateImports { get; set; }

        /// <summary>
        /// Defines if each classpath is explored immediately (PathExplorer) 
        /// </summary>
        bool LazyClasspathExploration { get; set; }

        /// <summary>
        /// In completion, show all known types in project
        /// </summary>
        bool CompletionListAllTypes { get; set; }

        /// <summary>
        /// In completion, show qualified type names (package + type)
        /// </summary>
        bool CompletionShowQualifiedTypes { get; set; }

        /// <summary>
        /// Run syntax checking on file save
        /// </summary>
        bool CheckSyntaxOnSave { get; set; }

        /// <summary>
        /// Automatically play project after build
        /// </summary>
        bool PlayAfterBuild { get; set; }

        /// <summary>
        /// On opening a file, automatically change the package declaration 
        /// if it doesn't match the classpath
        /// </summary>
        bool FixPackageAutomatically { get; set; }

        /// <summary>
        /// Language SDKs list
        /// </summary>
        InstalledSDK[] InstalledSDKs { get; set; }

        /// <summary>
        /// Return default language SDK
        /// - if no (valid) SDK is defined, this method must return InstalledSDK.INVALID_SDK;
        /// </summary>
        InstalledSDK GetDefaultSDK();

        /// <summary>
        /// Status text when running syntax check
        /// </summary>
        string CheckSyntaxRunning { get; }

        /// <summary>
        /// Status text when syntax checking is finished
        /// </summary>
        string CheckSyntaxDone { get; }

    }
}
