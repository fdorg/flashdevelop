// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        HaXeSettings settingObject;
        Context contextInstance;
        string settingFilename;
        KeyValuePair<string, InstalledSDK> customSDK;
        int logCount;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = "HaxeContext";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "ccf2c534-db6b-4c58-b90e-cd0b837e61c5";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "Haxe context for the ASCompletion engine.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
            LintingManager.RegisterLinter("haxe", new DiagnosticsLinter(settingObject));
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            SaveSettings();
            if (Context.TemporaryOutputFile != null && File.Exists(Context.TemporaryOutputFile))
            {
                File.Delete(Context.TemporaryOutputFile);
            }
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    if (!(e is DataEvent de)) return;
                    var action = de.Action;
                    if (action == ProjectManagerEvents.RunCustomCommand)
                    {
                        if (ExternalToolchain.HandleProject(PluginBase.CurrentProject))
                            e.Handled = ExternalToolchain.Run(de.Data as string);
                    }
                    else if (action == ProjectManagerEvents.BuildProject || action == ProjectManagerEvents.TestProject)
                    {
                        if (contextInstance.completionModeHandler is CompletionServerCompletionHandler completionHandler && !completionHandler.IsRunning())
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
            var path = Path.Combine(PathHelper.DataDir, nameof(HaXeContext));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
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
            settingObject = new HaXeSettings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (HaXeSettings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        void ValidateSettings()
        {
            if (settingObject.InstalledSDKs.IsNullOrEmpty() || PluginBase.MainForm.RefreshConfig)
            {
                var sdks = new List<InstalledSDK>();
                var externalSDK = Environment.ExpandEnvironmentVariables("%HAXEPATH%");
                if (!string.IsNullOrEmpty(externalSDK) && Directory.Exists(PathHelper.ResolvePath(externalSDK)))
                {
                    InstalledSDKContext.Current = this;
                    sdks.Add(new InstalledSDK(this) {Path = externalSDK});
                }
                if (settingObject.InstalledSDKs != null)
                {
                    char[] slashes = { '/', '\\' };
                    foreach (var oldSdk in settingObject.InstalledSDKs)
                    {
                        var oldPath = oldSdk.Path.TrimEnd(slashes);
                        foreach (var newSdk in sdks)
                        {
                            var newPath = newSdk.Path.TrimEnd(slashes);
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
                foreach (var sdk in settingObject.InstalledSDKs)
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
        void SettingObjectOnClasspathChanged() => contextInstance?.BuildClassPath();

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        bool OpenVirtualFileModel(string virtualPath)
        {
            var p = virtualPath.IndexOfOrdinal("::");
            if (p < 0) return false;
            var container = virtualPath.Substring(0, p);
            if (!File.Exists(container)) return false;
            var ext = Path.GetExtension(container).ToLower();
            if (ext != ".swf" && ext != ".swc") return false;
            var ctx = ASCompletion.Context.ASContext.GetLanguageContext("as3") ?? contextInstance;
            var path = new PathModel(container, ctx);
            var parser = new ContentParser(path.Path);
            parser.Run();
            AbcConverter.Convert(parser, path, ctx);
            var fileName = Path.Combine(container, virtualPath.Substring(p + 2).Replace('.', Path.DirectorySeparatorChar));
            if (!path.TryGetFile(fileName, out var model)) return false;
            ASComplete.OpenVirtualFile(model);
            return true;
        }

        InstalledSDK GetCustomSDK(string path)
        {
            InstalledSDK sdk;
            if (customSDK.Key == path) sdk = customSDK.Value;
            else
            {
                sdk = new InstalledSDK(this) {Path = path};
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

            var path = GetSDKPath(sdk);
            if (path == "") return false;

            var result = ValidateHaxeShimSDK(sdk, path)
                         || ValidateHaxeSDK(sdk, path)
                         || ValidateUnknownHaxeSDK(sdk, path);

            if (!result) ErrorManager.ShowInfo("Unable to identify a Haxe SDK at path:\n" + sdk.Path);
            return result;
        }

        static string GetSDKPath(InstalledSDK sdk)
        {
            var project = PluginBase.CurrentProject;
            var path = sdk.Path;
            path = project != null
                ? PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath))
                : PathHelper.ResolvePath(path);

            try
            {
                if (path is null || !Directory.Exists(path))
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

        bool ValidateHaxeShimSDK(InstalledSDK sdk, string path, string projectPath = "")
        {
            var result = false;
            var haxePath = Path.Combine(path, "haxe.exe");
            if (!File.Exists(haxePath)) haxePath = Path.Combine(path, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            if (!File.Exists(haxePath)) return result;
            var p = StartHiddenProcess(haxePath, "--run show-version", projectPath);
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            if (p.ExitCode == 0)
            {
                var mVer = Regex.Match(output, "^-D haxe-ver=([0-9.]+)(?:\\s*\\(git\\s*[^)]*@\\s*([0-9a-f]+)\\))?\\s*-cp (.*)\\s*");
                if (mVer.Success)
                {
                    sdk.Version = mVer.Groups[1].Value;
                    sdk.ClassPath = ASCompletion.Context.ASContext.NormalizePath(mVer.Groups[3].Value).TrimEnd(Path.DirectorySeparatorChar);
                    // Get pre-release version from class path, if present
                    var mSuffix = Regex.Match(mVer.Groups[3].Value, ".*/" + sdk.Version + "(-[0-9A-Za-z.-]+)/");
                    if (mSuffix.Success) sdk.Version += mSuffix.Groups[1].Value;
                    if (mVer.Groups[2].Success) sdk.Version += "+git." + mVer.Groups[2].Value;
                    sdk.Name = "Haxe Shim " + sdk.Version;
                    result = true;
                }
            }
            p.Close();
            return result;
        }

        bool ValidateHaxeSDK(InstalledSDK sdk, string path)
        {
            var result = false;
            var gitSha = string.Empty;
            var haxePath = Path.Combine(path, "haxe.exe");
            if (File.Exists(haxePath))
            {
                var p = StartHiddenProcess(haxePath, "-version");
                var output = p.StandardError.ReadToEnd();
                if (output.Length == 0) output = p.StandardOutput.ReadToEnd(); // haxe >= 4.0.0
                p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    var mVer = Regex.Match(output, "^([0-9.]+)(?:\\s*\\(git\\s*[^)]*@\\s*([0-9a-f]+)\\))?\\s*");
                    if (mVer.Success)
                    {
                        sdk.Version = mVer.Groups[1].Value;
                        if (mVer.Groups[2].Success) gitSha = mVer.Groups[2].Value;
                        result = true;
                    }
                }

                p.Close();
            }

            string[] lookup = {
                Path.Combine(path, "CHANGES.txt"),
                Path.Combine(path, Path.Combine("extra", "CHANGES.txt")),
                Path.Combine(path, Path.Combine("doc", "CHANGES.txt"))
            };
            var descriptor = lookup.FirstOrDefault(File.Exists);
            if (descriptor != null)
            {
                var raw = File.ReadAllText(descriptor);
                var mVer = Regex.Match(raw, "[0-9\\-?]+\\s*:\\s*([0-9.]+(-[0-9A-Za-z.-]+)?)");
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

        static bool ValidateUnknownHaxeSDK(InstalledSDK sdk, string path)
        {
            if (!File.Exists(Path.Combine(path, "haxe.exe"))) return false;
            sdk.Version = "0.0.0";
            sdk.Name = "Haxe ?";
            return true;
        }

        public Process StartHiddenProcess(string fileName, string arguments, string workingDirectory = "")
        {
            var pi = new ProcessStartInfo();
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