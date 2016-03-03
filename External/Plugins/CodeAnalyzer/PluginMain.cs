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
        private String pluginName = "CodeAnalyzer";
        private String pluginGuid = "a6bab962-9ee8-4ed7-b5f7-08c3367eaf5e";
        private String pluginDesc = "Integrates Flex PMD code analyzer into FlashDevelop.";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginAuth = "FlashDevelop Team";
        private ToolStripMenuItem analyzeMenuItem;
        private ToolStripMenuItem creatorMenuItem;
        private String settingFilename;
        private Settings settingObject;

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
            String dataPath = Path.Combine(PathHelper.DataDir, "CodeAnalyzer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
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
            this.creatorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.RulesetCreator"), null, new EventHandler(this.OpenCreator));
            this.analyzeMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.AnalyzeProject"), null, new EventHandler(this.AnalyzeProject), Keys.None);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.AnalyzeProject", this.analyzeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.RulesetCreator", this.creatorMenuItem);
            viewMenu.DropDownItems.Insert(2, this.analyzeMenuItem);
            viewMenu.DropDownItems.Insert(3, this.creatorMenuItem);
            this.analyzeMenuItem.Enabled = false;
        }

        /// <summary>
        /// Opens the ruleset creator page
        /// </summary>
        private void OpenCreator(Object sender, EventArgs e)
        {
            String url = "http://opensource.adobe.com/svn/opensource/flexpmd/bin/flex-pmd-ruleset-creator.html";
            PluginBase.MainForm.CallCommand("Browse", url);
        }

        /// <summary>
        /// Analyzes the current project
        /// </summary>
        private void AnalyzeProject(Object sender, EventArgs e)
        {
            if (PluginBase.CurrentProject != null)
            {
                String pmdDir = Path.Combine(PathHelper.ToolDir, "flexpmd");
                String pmdJar = Path.Combine(pmdDir, "flex-pmd-command-line-1.2.jar");
                String ruleFile = Path.Combine(this.GetProjectPath(), "Ruleset.xml");
                if (!File.Exists(ruleFile)) ruleFile = settingObject.PMDRuleset; // Use default...
                PMDRunner.Analyze(pmdJar, this.GetProjectPath(), this.GetSourcePath(), ruleFile);
            }
        }

        /// <summary>
        /// Gets the first available source path
        /// </summary>
        private String GetSourcePath()
        {
            IProject project = PluginBase.CurrentProject;
            if (project.SourcePaths.Length > 0)
            {
                String first = project.GetAbsolutePath(project.SourcePaths[0]);
                return Path.Combine(this.GetProjectPath(), first);
            }
            else return Path.Combine(this.GetProjectPath(), "src");
        }

        /// <summary>
        /// Gets the root directory of a project
        /// </summary>
        private String GetProjectPath()
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
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            if (String.IsNullOrEmpty(this.settingObject.PMDRuleset))
            {
                String pmdDir = Path.Combine(PathHelper.ToolDir, "flexpmd");
                this.settingObject.PMDRuleset = Path.Combine(pmdDir, "default-ruleset.xml");
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