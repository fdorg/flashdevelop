using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Helpers;
using ProjectManager.Projects;

namespace ProjectManager.Actions
{
    public delegate void BuildCompleteHandler(IProject project, bool runOutput);

    /// <summary>
    /// Provides methods for building a project inside FlashDevelop
    /// </summary>
    public class BuildActions
    {
        public static int LatestSDKMatchQuality;
        static bool setPlayerglobalHomeEnv;

        readonly IMainForm mainForm;
        readonly PluginMain pluginMain;
        readonly FDProcessRunner fdProcess;

        public event BuildCompleteHandler BuildComplete;
        public event BuildCompleteHandler BuildFailed;

        public string IPCName { get; }

        public BuildActions(IMainForm mainForm, PluginMain pluginMain)
        {
            this.mainForm = mainForm;
            this.pluginMain = pluginMain;

            // setup FDProcess helper class
            this.fdProcess = new FDProcessRunner(mainForm);

            // setup remoting service so FDBuild can use our in-memory services like FlexCompilerShell
            this.IPCName = Guid.NewGuid().ToString();
            SetupRemotingServer();
        }

        private void SetupRemotingServer()
        {
            IpcChannel channel = new IpcChannel(IPCName);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(FlexCompilerShell), "FlexCompilerShell", WellKnownObjectMode.Singleton);
        }

        public bool Build(Project project, bool runOutput, bool releaseMode)
        {
            // save modified files
            mainForm.CallCommand("SaveAllModified", null);

            string compiler = null;
            InstalledSDK sdk = null;
            if (project.IsCompilable)
            {
                sdk = GetProjectSDK(project);
                compiler = GetCompilerPath(project, sdk);
            }
            project.TraceEnabled = !releaseMode;

            if (project.OutputType == OutputType.OtherIDE)
            {
                // compile using associated IDE
                var command = project.GetOtherIDE(runOutput, releaseMode, out var error);
                if (error != null) ErrorManager.ShowInfo(TextHelper.GetString(error));
                else
                {
                    if (command == "FlashIDE") RunFlashIDE(project, runOutput, releaseMode);
                    else
                    {
                        Hashtable data = new Hashtable();
                        data["command"] = command;
                        data["project"] = project;
                        data["runOutput"] = runOutput;
                        data["releaseMode"] = releaseMode;
                        DataEvent de = new DataEvent(EventType.Command, "ProjectManager.RunWithAssociatedIDE", data);
                        EventManager.DispatchEvent(project, de);
                        if (de.Handled) return true;
                    }
                }
                return false;
            }

            if (project.OutputType == OutputType.CustomBuild)
            {
                // validate commands not empty
                if (project.PreBuildEvent.Trim().Length == 0 && project.PostBuildEvent.Trim().Length == 0)
                {
                    string info = TextHelper.GetString("Info.NoOutputAndNoBuild");
                    TraceManager.Add(info);
                }
            }
            else if (project.IsCompilable)
            {
                // ask the project to validate itself
                project.ValidateBuild(out var error);
                if (error != null)
                {
                    ErrorManager.ShowInfo(TextHelper.GetString(error));
                    return false;
                }

                if (project.OutputPath.Length == 0)
                {
                    string info = TextHelper.GetString("Info.SpecifyValidOutputSWF");
                    ErrorManager.ShowInfo(info);
                    return false;
                }

                if (!Directory.Exists(compiler) && !File.Exists(compiler))
                {
                    string info = TextHelper.GetString("Info.CheckSDKSettings");
                    MessageBox.Show(info, TextHelper.GetString("Title.ConfigurationRequired"), MessageBoxButtons.OK);
                    return false;
                }
            }

            // close running AIR projector
            if (project.MovieOptions.Platform.StartsWithOrdinal("AIR"))
            {
                foreach (Process proc in Process.GetProcessesByName("adl"))
                {
                    try { proc.Kill(); proc.WaitForExit(10 * 1000); }
                    catch { }
                }
            }

            return FDBuild(project, runOutput, releaseMode, sdk);
        }

        public static bool RunFlashIDE(Project project, bool runOutput, bool releaseMode)
        {
            string cmd = (runOutput) ? "testmovie" : "buildmovie";
            if (!PluginMain.Settings.DisableExtFlashIntegration) cmd += "-fd";

            cmd += ".jsfl";
            if (!releaseMode) cmd = "debug-" + cmd;

            cmd = Path.Combine("Tools", "flashide", cmd);
            cmd = PathHelper.ResolvePath(cmd, null);
            if (!File.Exists(cmd))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.JsflNotFound"));
                return false;
            }

            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.CallFlashIDE", cmd);
            EventManager.DispatchEvent(project, de);
            return de.Handled;
        }

        public bool FDBuild(Project project, bool runOutput, bool releaseMode, InstalledSDK sdk)
        {
            string directory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = project.Directory;

            string fdBuildPath = Path.Combine(PathHelper.ToolDir, "fdbuild", "fdbuild.exe");

            string arguments = " -ipc " + IPCName;
            if (sdk != null)
            {
                if (!string.IsNullOrEmpty(sdk.Version))
                    arguments += " -version \"" + sdk.Version.Replace(',', ';') + "\"";
                if (!string.IsNullOrEmpty(project.CurrentSDK))
                    arguments += " -compiler \"" + project.CurrentSDK + "\"";
            }

            if (releaseMode) arguments += " -notrace";
            arguments += " -library \"" + PathHelper.LibraryDir + "\"";

            foreach (string cp in PluginMain.Settings.GlobalClasspaths)
            {
                arguments += " -cp \"" + Environment.ExpandEnvironmentVariables(cp) + "\"";
            }

            if (project.TargetBuild != null)
                arguments += " -target \"" + project.TargetBuild + "\"";
            
            arguments = arguments.Replace("\\\"", "\""); // c# console args[] bugfix

            SetStatusBar(TextHelper.GetString("Info.BuildStarted"));
            pluginMain.UpdateUIStatus(ProjectManagerUIStatus.Building);

            // Apache Flex compat
            if (project.Language == "as3") 
            {
                string playerglobalHome = Environment.ExpandEnvironmentVariables("%PLAYERGLOBAL_HOME%");
                if (playerglobalHome.StartsWith('%')) setPlayerglobalHomeEnv = true;
                if (setPlayerglobalHomeEnv)
                {
                    Environment.SetEnvironmentVariable("PLAYERGLOBAL_HOME", Path.Combine(project.CurrentSDK, "frameworks/libs/player"));
                }
            }

            // Lets expose current sdk
            Environment.SetEnvironmentVariable("FD_CUR_SDK", project.CurrentSDK ?? "");

            // run FDBuild
            fdProcess.StartProcess(fdBuildPath, "\"" + project.ProjectPath + "\"" + arguments,
                project.Directory, delegate(bool success)
                {
                    pluginMain.UpdateUIStatus(ProjectManagerUIStatus.NotBuilding);
                    if (success)
                    {
                        SetStatusBar(TextHelper.GetString("Info.BuildSucceeded"));
                        AddTrustFile(project);
                        OnBuildComplete(project, runOutput);
                    }
                    else
                    {
                        SetStatusBar(TextHelper.GetString("Info.BuildFailed"));
                        OnBuildFailed(project, runOutput);
                    }
                    Environment.CurrentDirectory = directory;
                });
            return true;
        }

        void OnBuildComplete(IProject project, bool runOutput) => BuildComplete?.Invoke(project, runOutput);

        void OnBuildFailed(IProject project, bool runOutput) => BuildFailed?.Invoke(project, runOutput);

        void AddTrustFile(IProject project)
        {
            var directory = Path.GetDirectoryName(project.OutputPathAbsolute);
            if (!Directory.Exists(directory)) return;
            var trustParams = "FlashDevelop.cfg;" + directory;
            var de = new DataEvent(EventType.Command, "ASCompletion.CreateTrustFile", trustParams);
            EventManager.DispatchEvent(this, de);
        }

        public void NotifyBuildStarted() => fdProcess.ProcessStartedEventCaught();

        public void NotifyBuildEnded(string result) => fdProcess.ProcessEndedEventCaught(result);

        public void SetStatusBar(string text) => mainForm.StatusLabel.Text = " " + text;

        /* SDK MANAGEMENT */

        public static InstalledSDK GetProjectSDK(Project project)
        {
            if (project is null) return null;
            InstalledSDK[] sdks = GetInstalledSDKs(project);
            return MatchSDK(sdks, project);
        }

        public static string GetCompilerPath(Project project) => GetCompilerPath(project, GetProjectSDK(project));

        public static string GetCompilerPath(Project project, InstalledSDK sdk)
        {
            if (project is null) return null;
            project.CurrentSDK = PathHelper.ResolvePath(sdk.Path, project.Directory);
            if (project == PluginBase.CurrentProject) PluginBase.CurrentSDK = sdk;
            return project.CurrentSDK;
        }

        public static InstalledSDK MatchSDK(InstalledSDK[] sdks, IProject project) => MatchSDK(sdks, project.PreferredSDK);

        public static InstalledSDK MatchSDK(InstalledSDK[] sdks, string preferredSDK)
        {
            if (sdks is null) sdks = Array.Empty<InstalledSDK>();

            // default sdk
            if (string.IsNullOrEmpty(preferredSDK))
            {
                LatestSDKMatchQuality = -1;
                foreach (InstalledSDK sdk in sdks)
                    if (sdk.IsValid) return sdk;
                return InstalledSDK.INVALID_SDK;
            }

            var parts = (";;" + preferredSDK).Split(';'); // name;version
            
            // match name
            var name = parts[parts.Length - 3];
            if (name.Length != 0)
                foreach (InstalledSDK sdk in sdks)
                    if (sdk.IsValid && ((name.StartsWithOrdinal("Haxe Shim ") && sdk.IsHaxeShim) || sdk.Name == name))
                    {
                        LatestSDKMatchQuality = 0;
                        return sdk;
                    }

            // match version
            var version = parts[parts.Length - 2];
            if (version.Length != 0)
            {
                InstalledSDK bestMatch = null;
                int bestScore = int.MaxValue;
                foreach (InstalledSDK sdk in sdks)
                {
                    if (!sdk.IsValid) continue;
                    int score = CompareVersions(sdk.Version ?? "", version);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMatch = sdk;
                    }
                }
                if (bestMatch != null)
                {
                    LatestSDKMatchQuality = bestScore;
                    return bestMatch;
                }
            }

            // new SDK from path
            string sdkPath = parts[parts.Length - 1];
            if (sdks.Length > 0) InstalledSDKContext.Current = sdks[0].Owner;
            else TraceManager.AddAsync("Warning: compilation may fail if you do not have any (non-custom) SDK configured for the language", -3);
            InstalledSDK newSdk = new InstalledSDK();
            InstalledSDKContext.Current = null;
            newSdk.Path = sdkPath;
            LatestSDKMatchQuality = -1;
            return newSdk;
        }

        private static int CompareVersions(string sdkVersion, string version)
        {
            int score = 0;
            string[] sa = sdkVersion.Split(',');
            string[] sb = version.Split(',');

            for (int j = 0; j < sb.Length; j++)
            {
                try
                {
                    // TODO: Adjust scoring based on pre-release metadata (e.g. 4.0.0 is better than 4.0.0-preview.3). Handling various possible cases might get complicated, though.
                    string[] pa = new SemVer(sa[j].Trim()).ToString().Split('.');
                    string[] pb = new SemVer(sb[j].Trim()).ToString().Split('.');
                    int major = int.Parse(pa[0]) - int.Parse(pb[0]);
                    if (major < 0) return int.MaxValue;
                    if (major > 0) score += 10;
                    else
                    {
                        var minor = int.Parse(pa[1]) - int.Parse(pb[1]);
                        if (minor < 0) score += 5;
                        else if (minor > 0) score += 2;
                        else
                        {
                            var detail = int.Parse(pa[2]) - int.Parse(pb[2]);
                            if (detail < 0) score += 2;
                        }
                    }
                }
                catch { score += 40; }
            }
            if (sb.Length > sa.Length) score += 20;
            return score;
        }

        public static InstalledSDK[] GetInstalledSDKs(IProject project) => GetInstalledSDKs(project.Language);

        public static InstalledSDK[] GetInstalledSDKs(string language)
        {
            var infos = new Hashtable {["language"] = language};
            var de = new DataEvent(EventType.Command, "ASCompletion.InstalledSDKs", infos);
            EventManager.DispatchEvent(null, de);
            if (infos.ContainsKey("sdks") && infos["sdks"] != null) return (InstalledSDK[])infos["sdks"];
            return null;
        }
    }
}