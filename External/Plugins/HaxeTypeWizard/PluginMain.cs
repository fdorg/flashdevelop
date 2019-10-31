using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore;
using ProjectManager.Projects;
using ASClassWizard.Resources;
using System.Collections.Generic;
using ASClassWizard;
using ASClassWizard.Wizards;
using HaxeTypeWizard.Wizards;

namespace HaxeTypeWizard
{
    public class PluginMain : IPlugin
    {
        AS3ClassOptions lastFileOptions;
        string lastFileFromTemplate;
        string processOnSwitch;
        string constructorArgs;
        List<string> constructorArgTypes;

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
                        if (IsWizardTemplate(templateFile))
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
                                DisplayEnumWizard(inDirectory, templateFile, typeTemplate, name);
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
                case EventType.ProcessArgs:
                    if (lastFileFromTemplate != null)
                    {
                        var te = (TextEvent) e;
                        te.Value = ProcessArgs(te.Value);
                    }
                    break;
            }
        }

        static bool IsWizardTemplate(string templateFile) => templateFile != null && File.Exists(templateFile + ".wizard");

        void DisplayEnumWizard(string inDirectory, string templateFile, string typeTemplate, string name)
        {
            var project = (Project)PluginBase.CurrentProject;
            using var dialog = new EnumWizard();
            if (WizardContext.ProcessWizard(inDirectory, name, project, dialog, out var path, out var newFilePath)) return;
            lastFileFromTemplate = newFilePath;
            constructorArgs = null;
            constructorArgTypes = null;
            lastFileOptions = new AS3ClassOptions(
                    language: project.Language,
                    package: dialog.GetPackage(),
                    super_class: null,
                    Interfaces: null,
                    is_public: true,
                    is_dynamic: false,
                    is_final: false,
                    create_inherited: false,
                    create_constructor: false
                )
                {Template = typeTemplate};
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                PluginBase.MainForm.FileFromTemplate(templateFile + ".wizard", newFilePath);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        #endregion

        #region Custom Methods

        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);

        // TODO slavara: localize me
        void InitLocalization() => Description = TextHelper.GetString($"{nameof(HaxeTypeWizard)}.Info.Description");

        string ProcessArgs(string args)
        {
            if (lastFileFromTemplate is null) return args;
            var package = lastFileOptions != null ? lastFileOptions.Package : "";
            var fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);
            args = args.Replace("$(FileName)", fileName);
            if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
            {
                args = args.Replace("$(Package)", package);
                args = package.Length != 0
                    ? args.Replace("$(FileNameWithPackage)", package + "." + fileName)
                    : args.Replace("$(FileNameWithPackage)", fileName);
                if (lastFileOptions != null)
                {
                    args = ProcessFileTemplate(args);
                    if (processOnSwitch is null) lastFileOptions = null;
                }
            }
            lastFileFromTemplate = null;
            return args;
        }

        string ProcessFileTemplate(string args)
        {
            var eolMode = (int)PluginBase.MainForm.Settings.EOLMode;
            var lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            var imports = new List<string>();
            var extends = "";
            var implements = "";
            var inheritedMethods = "";
            var paramString = "";
            var superConstructor = "";
            if (constructorArgs != null)
            {
                paramString = constructorArgs;
                foreach (string type in constructorArgTypes)
                {
                    if (!imports.Contains(type))
                    {
                        imports.Add(type);
                    }
                }
            }
            string classMetadata = "";
            var access = lastFileOptions.isPublic ? "public " : "private ";
            access += lastFileOptions.isDynamic ? "dynamic " : "";
            if (lastFileOptions.isFinal) classMetadata += "@:final\n";
            string importsSrc = "";
            string prevImport = null;
            imports.Sort();
            foreach (string import in imports)
            {
                if (prevImport == import) continue;
                prevImport = import;
                if (import.LastIndexOf('.') is int p && (p == -1 || import.Substring(0, p) == lastFileOptions.Package)) continue;
                importsSrc += "import " + import + ";" + lineBreak;
            }
            if (importsSrc.Length > 0) importsSrc += lineBreak;
            args = args.Replace("$(Template)", lastFileOptions.Template ?? string.Empty);
            args = args.Replace("$(Import)", importsSrc);
            args = args.Replace("$(Extends)", extends);
            args = args.Replace("$(Implements)", implements);
            args = args.Replace("$(Access)", access);
            args = args.Replace("$(InheritedMethods)", inheritedMethods);
            args = args.Replace("$(ConstructorArguments)", paramString);
            args = args.Replace("$(Super)", superConstructor);
            args = args.Replace("$(ClassMetadata)", classMetadata);
            return args;
        }

        #endregion
    }
}