using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore;
using ProjectManager.Projects;
using ASClassWizard;
using HaxeTypeWizard.Wizards;

namespace HaxeTypeWizard
{
    public class PluginMain : IPlugin
    {
        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name => nameof(HaxeTypeWizard);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid => "E0F754D5-A95B-4478-8A9E-4D35D41EAA15";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author => "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; private set; } = "Provides an Haxe type wizard for FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help => "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => null;

        #endregion
        
        #region Required Methods

        public void Initialize()
        {
            AddEventHandlers();
            InitLocalization();
        }
        
        public void Dispose()
        {
            // Nothing here...
        }
        
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (!(PluginBase.CurrentProject is Project project) || project.Language != "haxe") return;
            switch (e.Type)
            {
                case EventType.Command:
                    var de = (DataEvent)e;
                    if (de.Action == "ProjectManager.CreateNewFile")
                    {
                        var table = (Hashtable) de.Data;
                        var templateFile = table["templatePath"] as string;
                        if (WizardContext.IsWizardTemplate(templateFile))
                        {
                            var fileName = Path.GetFileName(templateFile);
                            if (string.IsNullOrEmpty(fileName) || !fileName.Contains('.', out var p)) return;
                            var templateType = fileName.Substring(0, p);
                            if (templateType.Equals("enum", StringComparison.OrdinalIgnoreCase))
                            {
                                de.Handled = true;
                                var inDirectory = (string)table["inDirectory"];
                                var typeTemplate = table["GenericTemplate"] as string;
                                var name = table["className"] as string ?? TextHelper.GetString("Wizard.Label.NewEnum");
                                using var dialog = new EnumWizard();
                                WizardContext.DisplayWizard(dialog, inDirectory, templateFile, typeTemplate, name, null, null);
                            }
                            else if (templateType.Equals("typedef", StringComparison.OrdinalIgnoreCase))
                            {
                                // TODO slavara: implement me
                            }
                            else if (templateType.Equals("abstract", StringComparison.OrdinalIgnoreCase))
                            {
                                // TODO slavara: implement me
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Custom Methods

        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);

        // TODO slavara: localize me
        void InitLocalization() => Description = TextHelper.GetString("Info.Description");

        #endregion
    }
}