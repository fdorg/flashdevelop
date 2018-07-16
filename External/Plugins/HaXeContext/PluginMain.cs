using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AS3Context;
using ASCompletion.Completion;
using ASCompletion.Model;
using CodeRefactor.Provider;
using HaXeContext.CodeRefactor.Provider;
using HaXeContext.Linters;
using LintingHelper.Managers;
using ProjectManager;
using ProjectManager.Projects.Haxe;
using SwfOp;
using System.Diagnostics;

namespace HaXeContext
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        private String pluginName = "HaxeContext";
        private String pluginGuid = "ccf2c534-db6b-4c58-b90e-cd0b837e61c5";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Haxe context for the ASCompletion engine.";
        private String pluginAuth = "FlashDevelop Team";
        private HaXeSettings settingObject;
        private Context contextInstance;
        private String settingFilename;
        private KeyValuePair<string, InstalledSDK> customSDK;
        private int logCount;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary>
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();

            LintingManager.RegisterLinter("haxe", new DiagnosticsLinter(settingObject));
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
            if (Context.TemporaryOutputFile != null && File.Exists(Context.TemporaryOutputFile))
            {
                File.Delete(Context.TemporaryOutputFile);
            }
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    DataEvent de = e as DataEvent;
                    if (de == null) return;
                    var action = de.Action;
                    if (action == ProjectManagerEvents.RunCustomCommand)
                    {
                        if (ExternalToolchain.HandleProject(PluginBase.CurrentProject))
                            e.Handled = ExternalToolchain.Run(de.Data as string);
                    }
                    else if (action == ProjectManagerEvents.BuildProject || action == ProjectManagerEvents.TestProject)
                    {
                        var completionHandler = contextInstance.completionModeHandler as CompletionServerCompletionHandler;
                        if (completionHandler != null && !completionHandler.IsRunning())
                            completionHandler.StartServer();
                    }
                    else if (action == ProjectManagerEvents.CleanProject)
                    {
                        var project = de.Data as IProject;
                        if (ExternalToolchain.HandleProject(project))
                            e.Handled = ExternalToolchain.Clean(project);
                    }
                    else if (action == ProjectManagerEvents.Project || action == ProjectManagerEvents.OpenProjectProperties)
                    {
                        var project = de.Data as IProject;

                        if (action == ProjectManagerEvents.Project) ExternalToolchain.Monitor(project);

                        var projectPath = project != null ? Path.GetDirectoryName(project.ProjectPath) : "";
                        foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                            if (sdk.IsHaxeShim) ValidateHaxeShimSDK(sdk, GetSDKPath(sdk), projectPath);
                        if (project?.CurrentSDK == customSDK.Key && (customSDK.Value?.IsHaxeShim ?? false))
                            ValidateHaxeShimSDK(customSDK.Value, GetSDKPath(customSDK.Value), projectPath);
                    }
                    else if (action == "Context.SetHaxeEnvironment")
                    {
                        contextInstance.SetHaxeEnvironment(de.Data as string);
                    }
                    else if (action == ProjectManagerEvents.OpenVirtualFile)
                    {
                        if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language == "haxe")
                            e.Handled = OpenVirtualFileModel((string) de.Data);
                    }
                    break;

                case EventType.UIStarted:
                    ValidateSettings();
                    customSDK = new KeyValuePair<string, InstalledSDK>();
                    contextInstance = new Context(settingObject, GetCustomSDK);
                    // Associate this context with haxe language
                    ASCompletion.Context.ASContext.RegisterLanguage(contextInstance, "haxe");
                    CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
                    break;
                case EventType.Trace:
                    if (settingObject.DisableLibInstallation) return;
                    var count = TraceManager.TraceLog.Count;
                    if (count <= logCount)
                    {
                        logCount = count;
                        return;
                    }
                    var patterns = new[]
                    {
                        "Library \\s*(?<name>[^ ]+)\\s*?(\\s*version (?<version>[^ ]+))?",
                        "Could not find haxelib\\s*(?<name>\"[^ ]+\")?(\\s*version \"(?<version>[^ ]+)\")?", // openfl project
                        "Cannot resolve `-lib\\s*(?<name>[^ ]+)`" // lix library
                    };
                    var nameToVersion = new Dictionary<string, string>();
                    for (; logCount < count; logCount++)
                    {
                        var message = TraceManager.TraceLog[logCount].Message?.Trim();
                        if (string.IsNullOrEmpty(message)) continue;
                        foreach (var pattern in patterns)
                        {
                            var m = Regex.Match(message, pattern);
                            if (m.Success) nameToVersion[m.Groups["name"].Value] = m.Groups["version"].Value;
                        }
                    }
                    if (nameToVersion.Count == 0) return;
                    var compilerOptions = ((HaxeProject)PluginBase.CurrentProject).CompilerOptions.Additional;
                    foreach (var lib in nameToVersion.Keys.ToArray())
                    {
                        var pattern = "-lib " + lib;
                        foreach (var line in compilerOptions)
                        {
                            if (!line.Contains(pattern) || line.StartsWithOrdinal("-lib")) continue;
                            nameToVersion.Remove(lib);
                            if (nameToVersion.Count == 0) return;
                        }
                    }
                    var text = TextHelper.GetString("Info.MissingLib");
                    // TODO: Show information about which libraries are missing in a single dialog?
                    // TODO: Prevent showing multiple dialogs about the same library.
                    var result = MessageBox.Show(PluginBase.MainForm, text, string.Empty, MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK) contextInstance.InstallLibrary(nameToVersion);
                    break;
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, nameof(HaXeContext));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Trace, HandlingPriority.High);
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new HaXeSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (HaXeSettings)obj;
            }
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            if (settingObject.InstalledSDKs == null || settingObject.InstalledSDKs.Length == 0 || PluginBase.MainForm.RefreshConfig)
            {
                List<InstalledSDK> sdks = new List<InstalledSDK>();
                var externalSDK = Environment.ExpandEnvironmentVariables("%HAXEPATH%");
                if (!String.IsNullOrEmpty(externalSDK) && Directory.Exists(PathHelper.ResolvePath(externalSDK)))
                {
                    InstalledSDKContext.Current = this;
                    var sdk = new InstalledSDK(this);
                    sdk.Path = externalSDK;
                    sdks.Add(sdk);
                }
                if (settingObject.InstalledSDKs != null)
                {
                    char[] slashes = new char[] { '/', '\\' };
                    foreach (InstalledSDK oldSdk in settingObject.InstalledSDKs)
                    {
                        string oldPath = oldSdk.Path.TrimEnd(slashes);
                        foreach (InstalledSDK newSdk in sdks)
                        {
                            string newPath = newSdk.Path.TrimEnd(slashes);
                            if (newPath.Equals(oldPath, StringComparison.OrdinalIgnoreCase))
                            {
                                sdks.Remove(newSdk);
                                break;
                            }
                        }
                    }
                    sdks.InsertRange(0, settingObject.InstalledSDKs);
                }
                settingObject.InstalledSDKs = sdks.ToArray();
            }
            else
            {
                foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                {
                    ValidateSDK(sdk);
                }
            }
            if (settingObject.CompletionServerPort == 0)
            {
                settingObject.CompletionServerPort = 6000;
                settingObject.CompletionMode = HaxeCompletionModeEnum.CompletionServer;
            }
            settingObject.OnClasspathChanged += SettingObjectOnClasspathChanged;
        }

        /// <summary>
        /// Update the classpath if an important setting has changed
        /// </summary>
        private void SettingObjectOnClasspathChanged()
        {
            if (contextInstance != null) contextInstance.BuildClassPath();
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        bool OpenVirtualFileModel(string virtualPath)
        {
            var p = virtualPath.IndexOfOrdinal("::");
            if (p < 0) return false;
            var container = virtualPath.Substring(0, p);
            if (!File.Exists(container)) return false;
            var ext = Path.GetExtension(container).ToLower();
            if (ext == ".swf" || ext == ".swc")
            {
                var ctx = ASCompletion.Context.ASContext.GetLanguageContext("as3") ?? contextInstance;
                var path = new PathModel(container, ctx);
                var parser = new ContentParser(path.Path);
                parser.Run();
                AbcConverter.Convert(parser, path, ctx);
                var fileName = Path.Combine(container, virtualPath.Substring(p + 2).Replace('.', Path.DirectorySeparatorChar));
                FileModel model;
                if (!path.TryGetFile(fileName, out model)) return false;
                ASComplete.OpenVirtualFile(model);
                return true;
            }
            return false;
        }

        private InstalledSDK GetCustomSDK(string path)
        {
            InstalledSDK sdk;
            if (customSDK.Key == path) sdk = customSDK.Value;
            else
            {
                sdk = new InstalledSDK(this);
                sdk.Path = path;
                if (sdk.IsValid) customSDK = new KeyValuePair<string, InstalledSDK>(path, sdk);
                else sdk = null;
            }
            return sdk;
        }

        #endregion

        #region InstalledSDKOwner Members

        public bool ValidateSDK(InstalledSDK sdk)
        {
            sdk.Owner = this;
            sdk.ClassPath = null;

            string path = GetSDKPath(sdk);
            if (path == "") return false;

            bool result = 
                ValidateHaxeShimSDK(sdk, path) ||
                ValidateHaxeSDK(sdk, path) ||
                ValidateUnknownHaxeSDK(sdk, path);

            if (!result) ErrorManager.ShowInfo("Unable to identify a Haxe SDK at path:\n" + sdk.Path);

            return result;
        }

        private string GetSDKPath(InstalledSDK sdk)
        {
            IProject project = PluginBase.CurrentProject;
            string path = sdk.Path;
            if (project != null)
                path = PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath));
            else
                path = PathHelper.ResolvePath(path);

            try
            {
                if (path == null || !Directory.Exists(path))
                {
                    //ErrorManager.ShowInfo("Path not found:\n" + sdk.Path);
                    return "";
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowInfo("Invalid path (" + ex.Message + "):\n" + sdk.Path);
                return "";
            }

            return path;
        }

        private bool ValidateHaxeShimSDK(InstalledSDK sdk, string path, string projectPath = "")
        {
            bool result = false;

            string haxePath = Path.Combine(path, "haxe.exe");
            if (!File.Exists(haxePath)) haxePath = Path.Combine(path, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            if (File.Exists(haxePath))
            {
                Process p = StartHiddenProcess(haxePath, "--run show-version", projectPath);
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    Match mVer = Regex.Match(output, "^-D haxe-ver=([0-9.]+)(?:\\s*\\(git\\s*[^)]*@\\s*([0-9a-f]+)\\))?\\s*-cp (.*)\\s*");
                    if (mVer.Success)
                    {
                        sdk.Version = mVer.Groups[1].Value;
                        sdk.ClassPath = ASCompletion.Context.ASContext.NormalizePath(mVer.Groups[3].Value).TrimEnd(Path.DirectorySeparatorChar);
                        // Get pre-release version from class path, if present
                        Match mSuffix = Regex.Match(mVer.Groups[3].Value, ".*/" + sdk.Version + "(-[0-9A-Za-z.-]+)/");
                        if (mSuffix.Success) sdk.Version += mSuffix.Groups[1].Value;
                        if (mVer.Groups[2].Success) sdk.Version += "+git." + mVer.Groups[2].Value;
                        sdk.Name = "Haxe Shim " + sdk.Version;
                        result = true;
                    }
                }

                p.Close();
            }

            return result;
        }

        private bool ValidateHaxeSDK(InstalledSDK sdk, string path)
        {
            bool result = false;
            string gitSha = "";

            string haxePath = Path.Combine(path, "haxe.exe");
            if (File.Exists(haxePath))
            {
                Process p = StartHiddenProcess(haxePath, "-version");

                string output = p.StandardError.ReadToEnd();
                if (output == "") output = p.StandardOutput.ReadToEnd(); // haxe >= 4.0.0
                p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    Match mVer = Regex.Match(output, "^([0-9.]+)(?:\\s*\\(git\\s*[^)]*@\\s*([0-9a-f]+)\\))?\\s*");
                    if (mVer.Success)
                    {
                        sdk.Version = mVer.Groups[1].Value;
                        if (mVer.Groups[2].Success) gitSha = mVer.Groups[2].Value;
                        result = true;
                    }
                }

                p.Close();
            }

            string[] lookup = new string[] {
                Path.Combine(path, "CHANGES.txt"),
                Path.Combine(path, Path.Combine("extra", "CHANGES.txt")),
                Path.Combine(path, Path.Combine("doc", "CHANGES.txt"))
            };
            string descriptor = null;
            foreach (string p in lookup)
                if (File.Exists(p))
                {
                    descriptor = p;
                    break;
                }
            if (descriptor != null)
            {
                string raw = File.ReadAllText(descriptor);
                Match mVer = Regex.Match(raw, "[0-9\\-?]+\\s*:\\s*([0-9.]+(-[0-9A-Za-z.-]+)?)");
                if (mVer.Success)
                {
                    if (!result)
                    {
                        sdk.Version = mVer.Groups[1].Value;
                        result = true;
                    }
                    else if (mVer.Groups[2].Success)
                    {
                        // Get pre-release version from CHANGES.txt, if present
                        sdk.Version += mVer.Groups[2].Value;
                    }
                }
                else if (!result)
                {
                    ErrorManager.ShowInfo("Invalid CHANGES.txt file:\n" + descriptor);
                }
            }

            if (result)
            {
                if (gitSha != "") sdk.Version += "+git." + gitSha;
                sdk.Name = "Haxe " + sdk.Version;
            }
            return result;
        }

        private bool ValidateUnknownHaxeSDK(InstalledSDK sdk, string path)
        {
            string haxePath = Path.Combine(path, "haxe.exe");
            if (File.Exists(haxePath))
            {
                sdk.Version = "0.0.0";
                sdk.Name = "Haxe ?";
                return true;
            }
            return false;
        }

        private Process StartHiddenProcess(string fileName, string arguments, string workingDirectory = "")
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = fileName;
            pi.Arguments = arguments;
            pi.WorkingDirectory = workingDirectory;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;

            return Process.Start(pi);
        }

        #endregion

    }

}
