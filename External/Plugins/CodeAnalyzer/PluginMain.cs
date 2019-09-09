// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public string Name { get; } = nameof(CodeAnalyzer);

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
                case EventType.Command:
                    if (((DataEvent)e).Action == "ProjectManager.Project")
                    {
                        IProject project = PluginBase.CurrentProject;
                        analyzeMenuItem.Enabled = (project != null && project.Language == "as3");
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
            var path = Path.Combine(PathHelper.DataDir, nameof(CodeAnalyzer));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Listen for the necessary events
        /// </summary>
        private void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.Command);

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        private void CreateMenuItem()
        {
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("FlashToolsMenu");
            creatorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.RulesetCreator"), null, OpenCreator);
            analyzeMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.AnalyzeProject"), null, AnalyzeProject, Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.AnalyzeProject", analyzeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.RulesetCreator", creatorMenuItem);
            viewMenu.DropDownItems.Insert(2, analyzeMenuItem);
            viewMenu.DropDownItems.Insert(3, creatorMenuItem);
            analyzeMenuItem.Enabled = false;
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
            if (PluginBase.CurrentProject is null) return;
            string pmdJar = Path.Combine(PathHelper.ToolDir, "flexpmd", "flex-pmd-command-line-1.2.jar");
            string ruleFile = Path.Combine(GetProjectPath(), "Ruleset.xml");
            if (!File.Exists(ruleFile)) ruleFile = settingObject.PMDRuleset; // Use default...
            PMDRunner.Analyze(pmdJar, GetProjectPath(), GetSourcePath(), ruleFile);
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
                return Path.Combine(GetProjectPath(), first);
            }
            return Path.Combine(GetProjectPath(), "src");
        }

        /// <summary>
        /// Gets the root directory of a project
        /// </summary>
        private string GetProjectPath() => Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        private void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
            if (string.IsNullOrEmpty(settingObject.PMDRuleset))
            {
                settingObject.PMDRuleset = Path.Combine(PathHelper.ToolDir, "flexpmd", "default-ruleset.xml");
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        private void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion
    }
}