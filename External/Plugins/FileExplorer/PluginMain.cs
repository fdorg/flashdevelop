using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace FileExplorer
{
    public class PluginMain : IPlugin
    {
        string settingFilename;
        string configFilename;
        DockContent pluginPanel;
        PluginUI pluginUI;
        Image pluginImage;
        const string explorerAction = "explorer.exe /e,{0}";

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(FileExplorer);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "f534a520-bcc7-4fe4-a4b9-6931948b2686";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a file explorer panel to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        object IPlugin.Settings => Settings;

        /// <summary>
        /// Internal access to settings
        /// </summary>
        [Browsable(false)]
        public Settings Settings { get; set; }

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
            CreatePluginPanel();
            CreateMenuItem();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.UIStarted:
                    pluginUI.Initialize(null, null);
                    break;

                case EventType.Command:
                    var de = (DataEvent)e;
                    switch (de.Action)
                    {
                        case "FileExplorer.BrowseTo":
                            pluginUI.BrowseTo(de.Data.ToString());
                            OpenPanel(null, null);
                            de.Handled = true;
                            break;

                        case "FileExplorer.Explore":
                            ExploreDirectory(de.Data.ToString());
                            de.Handled = true;
                            break;

                        case "FileExplorer.FindHere":
                            FindHere((string[])de.Data);
                            de.Handled = true;
                            break;

                        case "FileExplorer.PromptHere":
                            PromptHere(de.Data.ToString());
                            de.Handled = true;
                            break;

                        case "FileExplorer.GetContextMenu":
                            de.Data = pluginUI.GetContextMenu();
                            de.Handled = true;
                            break;
                    }
                    break;

                case EventType.FileOpen:
                    TextEvent te = (TextEvent)e;
                    if (File.Exists(te.Value))
                    {
                        pluginUI.AddToMRU(te.Value);
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods

        /// <summary>
        /// Opens the selected path in windows explorer
        /// </summary>
        void ExploreDirectory(string path)
        {
            try
            {
                path = PluginBase.MainForm.ProcessArgString(path);
                if (BridgeManager.Active && BridgeManager.IsRemote(path) && BridgeManager.Settings.UseRemoteExplorer)
                {
                    BridgeManager.RemoteOpen(path);
                    return;
                }
                var config = ConfigHelper.Parse(configFilename, true).Flatten();
                if (!config.ContainsKey("explorer")) config["explorer"] = explorerAction;
                string explorer = PluginBase.MainForm.ProcessArgString(config["explorer"]);
                int start = explorer.StartsWith('\"') ? explorer.IndexOf('\"', 2) : 0;
                int p = explorer.IndexOf(' ', start);
                if (!path.StartsWith('\"')) path = "\"" + path + "\"";
                // Start the process...
                ProcessStartInfo psi = new ProcessStartInfo(explorer.Substring(0, p));
                psi.Arguments = string.Format(explorer.Substring(p + 1), path);
                psi.WorkingDirectory = path;
                ProcessHelper.StartAsync(psi);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Opens the selected path in command prompt
        /// </summary>
        static void FindHere(IEnumerable<string> paths)
        {
            if (paths is null) return;
            var list = paths.Where(Directory.Exists).ToArray();
            if (list.Length == 0) return;
            var path = string.Join(";", list);
            PluginBase.MainForm.CallCommand("FindAndReplaceInFilesFrom", path);
        }

        /// <summary>
        /// Opens the selected path in command prompt
        /// </summary>
        void PromptHere(string path)
        {
            try
            {
                path = PluginBase.MainForm.ProcessArgString(path);
                var config = ConfigHelper.Parse(configFilename, true).Flatten();
                if (!config.ContainsKey("cmd")) config["cmd"] = PluginBase.MainForm.CommandPromptExecutable;
                var cmd = PluginBase.MainForm.ProcessArgString(config["cmd"]).Replace("{0}", path);
                var start = cmd.StartsWith('\"') ? cmd.IndexOf('\"', 2) : 0;
                var p = cmd.IndexOf(' ', start);
                if (path.StartsWith('\"') && path.Length > 2) path = path.Substring(1, path.Length - 2);
                // Start the process...
                var psi = new ProcessStartInfo(p > 0 ? cmd.Substring(0, p) : cmd);
                if (p > 0) psi.Arguments = string.Format(cmd.Substring(p + 1), path);
                psi.WorkingDirectory = path;
                ProcessHelper.StartAsync(psi);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(FileExplorer));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            configFilename = Path.Combine(path, "Config.ini");
            Description = TextHelper.GetString("Info.Description");
            pluginImage = PluginBase.MainForm.FindImage("209");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.Command | EventType.FileOpen | EventType.UIStarted, HandlingPriority.Low);

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        void CreateMenuItem()
        {
            var label = TextHelper.GetString("Label.ViewMenuItem");
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var viewItem = new ToolStripMenuItem(label, pluginImage, OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowFiles", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        void CreatePluginPanel()
        {
            pluginUI = new PluginUI(this) {Text = TextHelper.GetString("Title.PluginPanel")};
            pluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, Guid, pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            Settings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else Settings = ObjectSerializer.Deserialize(settingFilename, Settings);
            if (!File.Exists(configFilename))
            {
                File.WriteAllText(configFilename, "[actions]\r\n#explorer=" + explorerAction + "\r\n#cmd=" + PluginBase.MainForm.CommandPromptExecutable + "\r\n");
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, Settings);

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        void OpenPanel(object sender, EventArgs e) => pluginPanel.Show();

        #endregion
    }
}