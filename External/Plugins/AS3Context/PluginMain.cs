// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AS3Context.Compiler;
using AS3Context.Controls;
using ASCompletion.Commands;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using SwfOp;
using WeifenLuo.WinFormsUI.Docking;

namespace AS3Context
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        Context contextInstance;
        string settingFilename;
        bool inMXML;
        Image pluginIcon;
        ProfilerUI profilerUI;
        DockContent profilerPanel;
        ToolStripButton viewButton;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = nameof(AS3Context);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "ccf2c534-db6b-4c58-b90e-cd0b837e61c4";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "ActionScript 3 context for the ASCompletion engine.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        object IPlugin.Settings => Settings;

        public static AS3Settings Settings { get; set; }

        #endregion
        
        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreatePanels();
            CreateMenuItems();
            AddEventHandlers();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            FlexDebugger.Stop();
            profilerUI?.Cleanup();
            SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (priority == HandlingPriority.Low)
            {
                switch (e.Type)
                {
                    case EventType.ProcessArgs:
                        var te = (TextEvent) e;
                        if (te.Value.Contains("$(FlexSDK)"))
                        {
                            te.Value = te.Value.Replace("$(FlexSDK)", contextInstance.GetCompilerPath());
                        }
                        break;

                    case EventType.Command:
                        var de = (DataEvent) e;
                        var action = de.Action;
                        if (action == "ProjectManager.Project")
                        {
                            FlexShells.Instance.Stop(); // clear
                        }
                        else if (action == "ProjectManager.OpenVirtualFile")
                        {
                            if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language == "as3")
                                e.Handled = OpenVirtualFileModel((string) de.Data);
                        }
                        else if (!Settings.DisableFDB && action == "AS3Context.StartDebugger")
                        {
                            string workDir = (PluginBase.CurrentProject != null)
                                ? Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath)
                                : Environment.CurrentDirectory;

                            string flexSdk = Settings.GetDefaultSDK().Path;

                            // if the default sdk is not defined ask for project sdk
                            if (string.IsNullOrEmpty(flexSdk))
                            {
                                flexSdk = PluginBase.MainForm.ProcessArgString("$(CompilerPath)");
                            }
                            e.Handled = FlexDebugger.Start(workDir, flexSdk, null);
                        }
                        else if (action == "AS3Context.StartProfiler")
                        {
                            if (profilerUI.AutoStart) profilerUI.StartProfiling();
                        }
                        break;

                    case EventType.Keys:
                        if (inMXML)
                        {
                            var ke = (KeyEvent) e;
                            if (ke.Value == PluginBase.MainForm.GetShortcutItemKeys("SearchMenu.GotoDeclaration"))
                            {
                                if (MxmlComplete.GotoDeclaration())
                                {
                                    ke.Handled = true;
                                    ke.ProcessKey = false;
                                }
                            }
                        }
                        break;
                }
                return;
            }
            if (priority == HandlingPriority.Normal)
            {
                switch (e.Type)
                {
                    case EventType.UIStarted:
                        contextInstance = new Context(Settings);
                        ValidateSettings();
                        AddToolbarItems();
                        // Associate this context with AS3 language
                        ASContext.RegisterLanguage(contextInstance, "as3");
                        ASContext.RegisterLanguage(contextInstance, "mxml");
                        break;

                    case EventType.FileSave:
                    case EventType.FileSwitch:
                        contextInstance?.OnFileOperation(e);

                        if (PluginBase.MainForm.CurrentDocument.IsEditable)
                        {
                            string ext = Path.GetExtension(PluginBase.MainForm.CurrentDocument.FileName);
                            inMXML = (ext.ToLower() == ".mxml");
                            MxmlComplete.IsDirty = true;
                        }
                        else inMXML = false;
                        break;
                }
                return;
            }
            if (priority == HandlingPriority.High)
            {
                if (e.Type == EventType.Command)
                {
                    var action = ((DataEvent) e).Action;
                    if (action == "ProjectManager.Project")
                    {
                        FlexDebugger.Stop();
                        var project = PluginBase.CurrentProject;
                        viewButton.Enabled = project is null || project.Language == "as3" || project.Language == "haxe";
                    }
                    else if (action.StartsWithOrdinal("FlashViewer."))
                    {
                        if (action == "FlashViewer.Closed")
                        {
                            FlexDebugger.Stop();
                        }
                        else if (action == "FlashViewer.External" || action == "FlashViewer.Default" || action == "FlashViewer.Popup" || action == "FlashViewer.Document")
                        {
                            if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.EnableInteractiveDebugger)
                            {
                                DataEvent de = new DataEvent(EventType.Command, "AS3Context.StartProfiler", null);
                                EventManager.DispatchEvent(this, de);
                                
                                if (PluginBase.CurrentProject.TraceEnabled)
                                {
                                    de = new DataEvent(EventType.Command, "AS3Context.StartDebugger", ((DataEvent) e).Data);
                                    EventManager.DispatchEvent(this, de);
                                }
                            }
                        }
                    }
                    else if (action == "FlashConnect")
                    {
                        ProfilerUI.HandleFlashConnect(sender, ((DataEvent) e).Data);
                    }
                    else if (inMXML)
                    {
                        var de = (DataEvent) e;
                        de.Handled = de.Action switch
                        {
                            "XMLCompletion.Element" => MxmlComplete.HandleElement(de.Data),
                            "XMLCompletion.Namespace" => MxmlComplete.HandleNamespace(de.Data),
                            "XMLCompletion.CloseElement" => MxmlComplete.HandleElementClose(de.Data),
                            "XMLCompletion.Attribute" => MxmlComplete.HandleAttribute(de.Data),
                            "XMLCompletion.AttributeValue" => MxmlComplete.HandleAttributeValue(de.Data),
                            _ => de.Handled
                        };
                    }
                }
            }
        }

        bool OpenVirtualFileModel(string virtualPath)
        {
            var p = virtualPath.IndexOfOrdinal("::");
            if (p < 0) return false;

            var container = virtualPath.Substring(0, p);
            var ext = Path.GetExtension(container).ToLower();
            if (ext != ".swf" && ext != ".swc" && ext != ".ane") return false;
            if (!File.Exists(container)) return false;

            var path = new PathModel(container, contextInstance);
            var parser = new ContentParser(path.Path);
            parser.Run();
            AbcConverter.Convert(parser, path, contextInstance);

            string fileName = Path.Combine(container, virtualPath.Substring(p + 2).Replace('.', Path.DirectorySeparatorChar));
            if (path.TryGetFile(fileName, out var model))
            {
                ASComplete.OpenVirtualFile(model);
                return true;
            }
            int split = fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            fileName = fileName.Substring(0, split) + "package.as";
            if (path.TryGetFile(fileName, out model))
            {
                ASComplete.OpenVirtualFile(model);
                return true;
            }
            fileName = fileName.Substring(0, split) + "toplevel.as";
            if (path.TryGetFile(fileName, out model))
            {
                ASComplete.OpenVirtualFile(model);
                return true;
            }
            return false;
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(AS3Context));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
            pluginIcon = PluginBase.MainForm.FindImage("123");
        }

        /// <summary>
        /// Create dock panels
        /// </summary>
        void CreatePanels()
        {
            profilerUI = new ProfilerUI {Text = TextHelper.GetString("Title.Profiler")};
            profilerPanel = PluginBase.MainForm.CreateDockablePanel(profilerUI, Guid, pluginIcon, DockState.Hidden);
            profilerPanel.VisibleState = DockState.Float;
            profilerUI.PanelRef = profilerPanel;
        }

        /// <summary>
        /// Create toolbar icons & menu items
        /// </summary>
        void CreateMenuItems()
        {
            if (!(PluginBase.MainForm.FindMenuItem("ViewMenu") is ToolStripMenuItem menu)) return;

            var viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginIcon, OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowProfiler", viewItem);
            menu.DropDownItems.Add(viewItem);

            viewButton = new ToolStripButton(pluginIcon);
            viewButton.Name = "ShowProfiler";
            viewButton.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.ViewMenuItem");
            PluginBase.MainForm.RegisterSecondaryItem("ViewMenu.ShowProfiler", viewButton);
            viewButton.Click += OpenPanel;
        }

        /// <summary>
        /// Insert toolbar item at right position
        /// </summary>
        void AddToolbarItems()
        {
            ToolStripItem checkSyntax = null;
            var toolbar = PluginBase.MainForm.ToolStrip;
            foreach (ToolStripItem item in toolbar.Items)
            {
                if (item.Name == "CheckSyntax") 
                { 
                    checkSyntax = item; 
                    break;
                }
            }
            if (checkSyntax != null) toolbar.Items.Insert(toolbar.Items.IndexOf(checkSyntax) + 1, viewButton);
            else
            {
                toolbar.Items.Add(new ToolStripSeparator());
                toolbar.Items.Add(viewButton);
            }
        }


        /// <summary>
        /// Opens the plugin panel again if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e)
        {
            if (sender is ToolStripButton && profilerPanel.Visible && !profilerPanel.DockState.ToString().Contains("AutoHide"))
            {
                profilerPanel.Hide();
            }
            else profilerPanel.Show();
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.ProcessArgs | EventType.FileSwitch | EventType.FileSave);
            EventManager.AddEventHandler(this, EventType.Command, HandlingPriority.High);
            EventManager.AddEventHandler(this, EventType.Command | EventType.Keys | EventType.ProcessArgs, HandlingPriority.Low);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            Settings = new AS3Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else Settings = (AS3Settings) ObjectSerializer.Deserialize(settingFilename, Settings);
            if (Settings.AS3ClassPath is null) Settings.AS3ClassPath = @"Library\AS3\intrinsic";
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        void ValidateSettings()
        {
            if (Settings.InstalledSDKs.IsNullOrEmpty() || PluginBase.MainForm.RefreshConfig)
            {
                var sdks = new List<InstalledSDK>();
                var includedSDK = "Tools\\flexsdk";
                if (Directory.Exists(PathHelper.ResolvePath(includedSDK)))
                {
                    InstalledSDKContext.Current = this;
                    sdks.Add(new InstalledSDK(this) {Path = includedSDK});
                }
                includedSDK = "Tools\\ascsdk";
                if (Directory.Exists(PathHelper.ResolvePath(includedSDK)))
                {
                    InstalledSDKContext.Current = this;
                    sdks.Add(new InstalledSDK(this) {Path = includedSDK});
                }
                /* Resolve AppMan Flex SDKs */
                var appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\flexsdk");
                if (Directory.Exists(appManDir))
                {
                    var versionDirs = Directory.GetDirectories(appManDir);
                    foreach (var versionDir in versionDirs)
                    {
                        if (Directory.Exists(versionDir))
                        {
                            sdks.Add(new InstalledSDK(this) {Path = versionDir});
                        }
                    }
                }
                /* Resolve AppMan Flex+AIR SDKs */
                appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\flexairsdk");
                if (Directory.Exists(appManDir))
                {
                    var versionDirs = Directory.GetDirectories(appManDir);
                    foreach (var versionDir in versionDirs)
                    {
                        if (Directory.Exists(versionDir))
                        {
                            sdks.Add(new InstalledSDK(this) {Path = versionDir});
                        }
                    }
                }
                /* Resolve AppMan AIR SDKs */
                appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\ascsdk");
                if (Directory.Exists(appManDir))
                {
                    var versionDirs = Directory.GetDirectories(appManDir);
                    foreach (var versionDir in versionDirs)
                    {
                        if (Directory.Exists(versionDir))
                        {
                            sdks.Add(new InstalledSDK(this) {Path = versionDir});
                        }
                    }
                }
                //
                // TODO: Resolve Apache Flex SDK
                //
                if (Settings.InstalledSDKs != null)
                {
                    char[] slashes = { '/', '\\' };
                    foreach (var oldSdk in Settings.InstalledSDKs)
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
                    sdks.InsertRange(0, Settings.InstalledSDKs);
                }
                Settings.InstalledSDKs = sdks.ToArray();
            }
            else
            {
                foreach (var sdk in Settings.InstalledSDKs)
                {
                    sdk.Validate();
                }
            }
            Settings.OnClasspathChanged += SettingObjectOnClasspathChanged;
            Settings.OnInstalledSDKsChanged += settingObjectOnInstalledSDKsChanged;
        }

        /// <summary>
        /// Notify of SDK collection changes
        /// </summary>
        void settingObjectOnInstalledSDKsChanged()
        {
            if (contextInstance is null) return;
            var de = new DataEvent(EventType.Command, "ProjectManager.InstalledSDKsChanged", "as3");
            EventManager.DispatchEvent(contextInstance, de);
            if (!de.Handled) contextInstance.BuildClassPath();
        }

        /// <summary>
        /// Update the classpath if an important setting has changed
        /// </summary>
        void SettingObjectOnClasspathChanged() => contextInstance?.BuildClassPath();

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, Settings);

        /// <summary>
        /// Explore the possible locations for the Macromedia Flash IDE classpath
        /// </summary>
        public static string FindAuthoringConfigurationPath(string flashPath)
        {
            if (flashPath is null)
            {
                flashPath = CallFlashIDE.FindFlashIDE(true);
                if (flashPath is null) return null;
            }
            var ext = Path.GetExtension(flashPath).ToLower();
            if (ext == ".exe" || ext == ".bat" || ext == ".cmd")
            {
                flashPath = Path.GetDirectoryName(flashPath);
            }
            string basePath = flashPath;
            string deflang = CultureInfo.CurrentUICulture.Name;
            deflang = deflang.Substring(0, 2);
            // CS4+ default configuration
            if (Directory.Exists(basePath + "\\Common\\Configuration\\ActionScript 3.0"))
            {
                return basePath + "\\Common\\Configuration\\";
            }
            // default language
            if (Directory.Exists(basePath + deflang + "\\Configuration\\ActionScript 3.0"))
            {
                return basePath + deflang + "\\Configuration\\";
            }
            // look for other languages
            if (Directory.Exists(basePath))
            {
                var dirs = Directory.GetDirectories(basePath);
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir + "\\Configuration\\ActionScript 3.0"))
                    {
                        return dir + "\\Configuration\\";
                    }
                }
            }
            return null;
        }

        #endregion

        #region InstalledSDKOwner Membres

        public bool ValidateSDK(InstalledSDK sdk)
        {
            sdk.Owner = this;
            var path = sdk.Path;
            if (path is null) return false;
            var mBin = Regex.Match(path, "[/\\\\]bin$", RegexOptions.IgnoreCase);
            if (mBin.Success) sdk.Path = path = path.Substring(0, mBin.Index);

            var project = PluginBase.CurrentProject;
            path = project != null
                ? PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath))
                : PathHelper.ResolvePath(path);

            try
            {
                if (path is null || !Directory.Exists(path))
                {
                    ErrorManager.ShowInfo("Path not found:\n" + sdk.Path);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowInfo("Invalid path (" + ex.Message + "):\n" + sdk.Path);
                return false;
            }

            var descriptor = Path.Combine(path, "flex-sdk-description.xml");
            if (!File.Exists(descriptor)) 
                descriptor = Path.Combine(path, "air-sdk-description.xml");

            if (File.Exists(descriptor))
            {
                var raw = File.ReadAllText(descriptor);
                var mName = Regex.Match(raw, "<name>([^<]+)</name>");
                var mVer = Regex.Match(raw, "<version>([^<]+)</version>");
                if (mName.Success && mVer.Success)
                {
                    sdk.Name = mName.Groups[1].Value;
                    sdk.Version = mVer.Groups[1].Value;

                    descriptor = Path.Combine(path, "AIR SDK Readme.txt");
                    if (sdk.Name.StartsWithOrdinal("Flex") && File.Exists(descriptor))
                    {
                        raw = File.ReadAllText(descriptor);
                        var mAIR = Regex.Match(raw, "Adobe AIR ([0-9.]+) SDK");
                        if (mAIR.Success)
                        {
                            sdk.Name += ", AIR " + mAIR.Groups[1].Value;
                            sdk.Version += ", " + mAIR.Groups[1].Value;
                        }
                    }
                    return true;
                }
                ErrorManager.ShowInfo("Invalid SDK descriptor:\n" + descriptor);
            }
            else ErrorManager.ShowInfo("No SDK descriptor found:\n" + descriptor);
            return false;
        }

        #endregion
    }
}