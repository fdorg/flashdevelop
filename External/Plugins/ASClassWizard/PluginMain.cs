using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore;
using ProjectManager.Projects;
using ASCompletion.Context;
using ASCompletion.Completion;
using System.Collections.Generic;
using ASClassWizard.Helpers;
using ASClassWizard.Wizards;

namespace ASClassWizard
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
        public string Name => nameof(ASClassWizard);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid => "a2c159c1-7d21-4483-aeb1-38d9fdc4c7f3";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author => "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; private set; } = "Provides an ActionScript class wizard for FlashDevelop.";

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
            Project project;
            switch (e.Type)
            {
                case EventType.Command:
                    var de = (DataEvent)e;
                    if (de.Action == "ProjectManager.CreateNewFile")
                    {
                        project = (Project) PluginBase.CurrentProject;
                        if (project.Language.StartsWithOrdinal("as") || project.Language == "haxe")
                        {
                            var table = (Hashtable) de.Data;
                            var templateFile = table["templatePath"] as string;
                            if (WizardUtils.IsWizardTemplate(templateFile))
                            {
                                var fileName = Path.GetFileName(templateFile);
                                var templateType = !string.IsNullOrEmpty(fileName) && fileName.Contains('.', out var p)
                                                 ? fileName.Substring(0, p)
                                                 : "class";
                                if (templateType.Equals("class", StringComparison.OrdinalIgnoreCase))
                                {
                                    de.Handled = true;
                                    var inDirectory = (string)table["inDirectory"];
                                    var typeTemplate = table["GenericTemplate"] as string;
                                    var name = table["className"] as string ?? TextHelper.GetString("Wizard.Label.NewClass");
                                    var constructorArgs = table["constructorArgs"] as string;
                                    var constructorArgsTypes = table["constructorArgTypes"] as List<string>;
                                    using var dialog = new AS3ClassWizard();
                                    WizardUtils.DisplayWizard(dialog, inDirectory, templateFile, typeTemplate, name, constructorArgs, constructorArgsTypes);
                                }
                                else if (templateType.Equals("interface", StringComparison.OrdinalIgnoreCase))
                                {
                                    de.Handled = true;
                                    var inDirectory = (string) table["inDirectory"];
                                    var typeTemplate = table["GenericTemplate"] as string;
                                    var name = table["interfaceName"] as string ?? TextHelper.GetString("Wizard.Label.NewInterface");
                                    using var dialog = new AS3InterfaceWizard();
                                    WizardUtils.DisplayWizard(dialog, inDirectory, templateFile, typeTemplate, name, null, null);
                                }
                            }
                        }
                    }
                    break;

                case EventType.FileSwitch:
                    if (PluginBase.MainForm.CurrentDocument.FileName == WizardUtils.processOnSwitch)
                    {
                        WizardUtils.processOnSwitch = null;
                        if (WizardUtils.lastFileOptions?.interfaces is null) return;
                        foreach (var cname in WizardUtils.lastFileOptions.interfaces)
                        {
                            ASContext.Context.CurrentModel.Check();
                            var inClass = ASContext.Context.CurrentModel.GetPublicClass();
                            ASGenerator.SetJobContext(null, cname, null, null);
                            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, inClass, null, null);
                        }
                        WizardUtils.lastFileOptions = null;
                    }
                    break;

                case EventType.ProcessArgs:
                    project = PluginBase.CurrentProject as Project;
                    if (WizardUtils.lastFileFromTemplate != null && project != null && (project.Language.StartsWithOrdinal("as") || project.Language == "haxe"))
                    {
                        var te = (TextEvent) e;
                        te.Value = WizardUtils.ProcessArgs(te.Value);
                    }
                    break;
            }
        }

        #endregion

        #region Custom Methods

        void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.AddEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
        }

        void InitLocalization() => Description = TextHelper.GetString("Info.Description");

        #endregion
    }
}