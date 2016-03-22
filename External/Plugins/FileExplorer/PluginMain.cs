using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private String pluginName = "FileExplorer";
        private String pluginGuid = "f534a520-bcc7-4fe4-a4b9-6931948b2686";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds a file explorer panel to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private String settingFilename;
        private String configFilename;
        private Settings settingObject;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
        private Image pluginImage;
        private const String explorerAction = "explorer.exe /e,{0}";
        private const String cmdAction = "cmd.exe";

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
        Object IPlugin.Settings
        {
            get { return this.settingObject; }
        }

        /// <summary>
        /// Internal access to settings
        /// </summary>
        [Browsable(false)]
        public Settings Settings
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
            this.CreatePluginPanel();
            this.CreateMenuItem();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.UIStarted:
                    this.pluginUI.Initialize(null, null);
                    break;

                case EventType.Command:
                    DataEvent evnt = (DataEvent)e;
                    switch (evnt.Action)
                    {
                        case "FileExplorer.BrowseTo":
                            this.pluginUI.BrowseTo(evnt.Data.ToString());
                            this.OpenPanel(null, null);
                            evnt.Handled = true;
                            break;

                        case "FileExplorer.Explore":
                            ExploreDirectory(evnt.Data.ToString());
                            evnt.Handled = true;
                            break;

                        case "FileExplorer.FindHere":
                            FindHere((String[])evnt.Data);
                            evnt.Handled = true;
                            break;

                        case "FileExplorer.PromptHere":
                            PromptHere(evnt.Data.ToString());
                            evnt.Handled = true;
                            break;

                        case "FileExplorer.GetContextMenu":
                            evnt.Data = this.pluginUI.GetContextMenu();
                            evnt.Handled = true;
                            break;
                    }
                    break;

                case EventType.FileOpen:
                    TextEvent evnt2 = (TextEvent)e;
                    if (File.Exists(evnt2.Value))
                    {
                        this.pluginUI.AddToMRU(evnt2.Value);
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods

        /// <summary>
        /// Opens the selected path in windows explorer
        /// </summary>
        private void ExploreDirectory(string path)
        {
            try
            {
                path = PluginBase.MainForm.ProcessArgString(path);
                if (BridgeManager.Active && BridgeManager.IsRemote(path) && BridgeManager.Settings.UseRemoteExplorer)
                {
                    BridgeManager.RemoteOpen(path);
                    return;
                }
                Dictionary<string, string> config = ConfigHelper.Parse(configFilename, true).Flatten();
                if (!config.ContainsKey("explorer")) config["explorer"] = explorerAction;
                String explorer = PluginBase.MainForm.ProcessArgString(config["explorer"]);
                int start = explorer.StartsWith('\"') ? explorer.IndexOfOrdinal("\"", 2) : 0;
                int p = explorer.IndexOfOrdinal(" ", start);
                if (!path.StartsWith('\"')) path = "\"" + path + "\"";
                // Start the process...
                ProcessStartInfo psi = new ProcessStartInfo(explorer.Substring(0, p));
                psi.Arguments = String.Format(explorer.Substring(p + 1), path);
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
        private void FindHere(string[] paths)
        {
            if (paths == null) return;
            List<String> pathsList = new List<String>(paths);
            pathsList.RemoveAll(p => !Directory.Exists(p));
            if (pathsList.Count > 0)
            {
                String path = String.Join(";", pathsList.ToArray());
                PluginBase.MainForm.CallCommand("FindAndReplaceInFilesFrom", path);
            }
        }

        /// <summary>
        /// Opens the selected path in command prompt
        /// </summary>
        private void PromptHere(string path)
        {
            try
            {
                path = PluginBase.MainForm.ProcessArgString(path);
                /*if (BridgeManager.Active && BridgeManager.IsRemote(path) && BridgeManager.Settings.UseRemoteConsole)
                {
                    BridgeManager.RemoteConsole(path);
                    return;
                }*/
                Dictionary<string, string> config = ConfigHelper.Parse(configFilename, true).Flatten();
                if (!config.ContainsKey("cmd")) config["cmd"] = cmdAction;
                String cmd = PluginBase.MainForm.ProcessArgString(config["cmd"]).Replace("{0}", path);
                int start = cmd.StartsWith('\"') ? cmd.IndexOfOrdinal("\"", 2) : 0;
                int p = cmd.IndexOfOrdinal(" ", start);
                if (path.StartsWith('\"') && path.Length > 2) path = path.Substring(1, path.Length - 2);
                // Start the process...
                ProcessStartInfo psi = new ProcessStartInfo(p > 0 ? cmd.Substring(0, p) : cmd);
                if (p > 0) psi.Arguments = String.Format(cmd.Substring(p + 1), path);
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
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "FileExplorer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.configFilename = Path.Combine(dataPath, "Config.ini");
            this.pluginDesc = TextHelper.GetString("Info.Description");
            this.pluginImage = PluginBase.MainForm.FindImage("209");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventType eventMask = EventType.Command | EventType.FileOpen | EventType.UIStarted;
            EventManager.AddEventHandler(this, eventMask, HandlingPriority.Low);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            String label = TextHelper.GetString("Label.ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(label, this.pluginImage, new EventHandler(this.OpenPanel));
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowFiles", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            if (!File.Exists(configFilename))
            {
                File.WriteAllText(configFilename, "[actions]\r\n#explorer=" + explorerAction + "\r\n#cmd=" + cmdAction + "\r\n");
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, EventArgs e)
        {
            this.pluginPanel.Show();
        }

        #endregion

    }
    
}
