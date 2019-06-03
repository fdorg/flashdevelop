using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace CodeAnalyzer
{
    public class PluginMain : IPlugin
    {
        private ToolStripMenuItem analyzeMenuItem;
        private ToolStripMenuItem creatorMenuItem;
        private string settingFilename;
        private Settings settingObject;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = "CodeAnalyzer";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "a6bab962-9ee8-4ed7-b5f7-08c3367eaf5e";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "Integrates Flex PMD code analyzer into FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => this.settingObject;

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
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    if (((DataEvent)e).Action == "ProjectManager.Project")
                    {
                        IProject project = PluginBase.CurrentProject;
                        this.analyzeMenuItem.Enabled = (project != null && project.Language == "as3");
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods
        
        /// <summary>
        /// Initializes important variables
        /// </summary>
        private void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, "CodeAnalyzer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Listen for the necessary events
        /// </summary>
        private void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        private void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("FlashToolsMenu");
            this.creatorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.RulesetCreator"), null, OpenCreator);
            this.analyzeMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.AnalyzeProject"), null, AnalyzeProject, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.AnalyzeProject", this.analyzeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.RulesetCreator", this.creatorMenuItem);
            viewMenu.DropDownItems.Insert(2, this.analyzeMenuItem);
            viewMenu.DropDownItems.Insert(3, this.creatorMenuItem);
            this.analyzeMenuItem.Enabled = false;
        }

        /// <summary>
        /// Opens the ruleset creator page
        /// </summary>
        private void OpenCreator(object sender, EventArgs e)
        {
            string url = "http://www.flashdevelop.org/flexpmd/index.html";
            PluginBase.MainForm.CallCommand("Browse", url);
        }

        /// <summary>
        /// Analyzes the current project
        /// </summary>
        private void AnalyzeProject(object sender, EventArgs e)
        {
            if (PluginBase.CurrentProject != null)
            {
                string pmdJar = Path.Combine(PathHelper.ToolDir, "flexpmd", "flex-pmd-command-line-1.2.jar");
                string ruleFile = Path.Combine(this.GetProjectPath(), "Ruleset.xml");
                if (!File.Exists(ruleFile)) ruleFile = settingObject.PMDRuleset; // Use default...
                PMDRunner.Analyze(pmdJar, this.GetProjectPath(), this.GetSourcePath(), ruleFile);
            }
        }

        /// <summary>
        /// Gets the first available source path
        /// </summary>
        private string GetSourcePath()
        {
            var project = PluginBase.CurrentProject;
            if (project.SourcePaths.Length > 0)
            {
                string first = project.GetAbsolutePath(project.SourcePaths[0]);
                return Path.Combine(this.GetProjectPath(), first);
            }
            return Path.Combine(this.GetProjectPath(), "src");
        }

        /// <summary>
        /// Gets the root directory of a project
        /// </summary>
        private string GetProjectPath()
        {
            return Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        private void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            if (string.IsNullOrEmpty(this.settingObject.PMDRuleset))
            {
                this.settingObject.PMDRuleset = Path.Combine(PathHelper.ToolDir, "flexpmd", "default-ruleset.xml");
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        #endregion

    }
    
}