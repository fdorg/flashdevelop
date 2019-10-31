using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore;
using ProjectManager.Projects;
using ASCompletion.Model;
using ASCompletion.Context;
using ASClassWizard.Resources;
using ASClassWizard.Wizards;
using ASCompletion.Completion;
using System.Collections.Generic;
using System.Linq;

namespace ASClassWizard
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
                            if (WizardContext.IsWizardTemplate(templateFile))
                            {
                                var fileName = Path.GetFileName(templateFile);
                                var templateType = !string.IsNullOrEmpty(fileName) && fileName.IndexOf('.') is int p && p != -1
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
                                    DisplayClassWizard(inDirectory, templateFile, typeTemplate, name, constructorArgs, constructorArgsTypes);
                                }
                                else if (templateType.Equals("interface", StringComparison.OrdinalIgnoreCase))
                                {
                                    de.Handled = true;
                                    var inDirectory = (string) table["inDirectory"];
                                    var typeTemplate = table["GenericTemplate"] as string;
                                    var name = table["interfaceName"] as string ?? TextHelper.GetString("Wizard.Label.NewInterface");
                                    DisplayInterfaceWizard(inDirectory, templateFile, typeTemplate, name);
                                }
                            }
                        }
                    }
                    break;

                case EventType.FileSwitch:
                    if (PluginBase.MainForm.CurrentDocument.FileName == processOnSwitch)
                    {
                        processOnSwitch = null;
                        if (lastFileOptions?.interfaces is null) return;
                        foreach (var cname in lastFileOptions.interfaces)
                        {
                            ASContext.Context.CurrentModel.Check();
                            var inClass = ASContext.Context.CurrentModel.GetPublicClass();
                            ASGenerator.SetJobContext(null, cname, null, null);
                            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, inClass, null, null);
                        }
                        lastFileOptions = null;
                    }
                    break;

                case EventType.ProcessArgs:
                    project = PluginBase.CurrentProject as Project;
                    if (lastFileFromTemplate != null && project != null && (project.Language.StartsWithOrdinal("as") || project.Language == "haxe"))
                    {
                        var te = (TextEvent) e;
                        te.Value = ProcessArgs(te.Value);
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

        void DisplayClassWizard(string inDirectory, string templateFile, string typeTemplate, string className, string constructorArgs, List<string> constructorArgTypes)
        {
            var project = (Project) PluginBase.CurrentProject;
            using var dialog = new AS3ClassWizard();
            if (WizardContext.ProcessWizard(inDirectory, className, project, dialog, out var path, out var newFilePath)) return;
            lastFileFromTemplate = newFilePath;
            this.constructorArgs = constructorArgs;
            this.constructorArgTypes = constructorArgTypes;
            lastFileOptions = WizardContext.GetWizardOptions(project, dialog, typeTemplate);
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

        void DisplayInterfaceWizard(string inDirectory, string templateFile, string typeTemplate, string name)
        {
            var project = (Project) PluginBase.CurrentProject;
            using var dialog = new AS3InterfaceWizard();
            if (WizardContext.ProcessWizard(inDirectory, name, project, dialog, out var path, out var newFilePath)) return;
            lastFileFromTemplate = newFilePath;
            constructorArgs = null;
            constructorArgTypes = null;
            lastFileOptions = WizardContext.GetWizardOptions(project, dialog, typeTemplate);
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

        string ProcessArgs(string args)
        {
            if (lastFileFromTemplate == null) return args;
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
            var paramString = "";
            var superConstructor = "";
            int index;
            // resolve imports
            if (!lastFileOptions.interfaces.IsNullOrEmpty())
            {
                string implementContinuation;
                implements = " implements ";
                index = 0;

                if (lastFileOptions.Language == "haxe")
                {
                    bool isHaxe2 = PluginBase.CurrentSDK != null && PluginBase.CurrentSDK.Name.ToLower().Contains("haxe 2");
                    implementContinuation = isHaxe2 ? ", implements " : " implements ";
                }
                else
                {
                    implementContinuation = ", ";
                }

                foreach (string item in lastFileOptions.interfaces)
                {
                    if (item.Contains('.')) imports.Add(item);
                    implements += (index > 0 ? implementContinuation : "") + item.Split('.').Last();
                    if (lastFileOptions.createInheritedMethods)
                    {
                        processOnSwitch = lastFileFromTemplate; 
                        // let ASCompletion generate the implementations when file is opened
                    }
                    index++;
                }
            }
            if (!string.IsNullOrEmpty(lastFileOptions.superClass))
            {
                var superClassFullName = lastFileOptions.superClass;
                if (superClassFullName.Contains(".")) imports.Add(superClassFullName);
                var superClassShortName = superClassFullName.Split('.').Last();
                var fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);
                extends = fileName == superClassShortName ? $" extends {superClassFullName}" : $" extends {superClassShortName}";
                if (lastFileOptions.createConstructor
                    && constructorArgs is null
                    && ASContext.GetLanguageContext(lastFileOptions.Language) is { } ctx)
                {
                    var lastDotIndex = superClassFullName.LastIndexOf('.');
                    var cmodel = ctx.GetModel(lastDotIndex == -1 ? "" : superClassFullName.Substring(0, lastDotIndex), superClassShortName, "");
                    if (!cmodel.IsVoid())
                    {
                        if ((cmodel.Flags & FlagType.TypeDef) != 0)
                        {
                            var tmp = cmodel;
                            tmp.ResolveExtends();
                            while (!tmp.IsVoid())
                            {
                                if (!string.IsNullOrEmpty(tmp.Constructor))
                                {
                                    cmodel = tmp;
                                    break;
                                }
                                tmp = tmp.Extends;
                            }
                        }
                        foreach (var member in cmodel.Members)
                        {
                            if (member.Name != cmodel.Constructor) continue;
                            paramString = member.ParametersString();
                            WizardContext.AddImports(ctx, member, cmodel.InFile, imports);
                            superConstructor = "super(";
                            index = 0;
                            if (member.Parameters != null)
                                foreach (var param in member.Parameters)
                                {
                                    if (param.Name.StartsWith('.')) break;
                                    var pname = TemplateUtils.GetParamName(param);
                                    superConstructor += (index > 0 ? ", " : "") + pname;
                                    index++;
                                }
                            superConstructor += ");\n" + (lastFileOptions.Language == "as3" ? "\t\t\t" : "\t\t");
                            break;
                        }
                    }
                }
            }
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
            string access;
            var classMetadata = "";
            if (lastFileOptions.Language == "as3")
            {
                access = lastFileOptions.isPublic ? "public " : "internal ";
                access += lastFileOptions.isDynamic ? "dynamic " : "";
                access += lastFileOptions.isFinal ? "final " : "";
            }
            else if (lastFileOptions.Language == "haxe")
            {
                access = lastFileOptions.isPublic ? "public " : "private ";
                access += lastFileOptions.isDynamic ? "dynamic " : "";
                if (lastFileOptions.isFinal) classMetadata += "@:final\n";
            }
            else access = lastFileOptions.isDynamic ? "dynamic " : "";
            var importsSrc = "";
            string prevImport = null;
            imports.Sort();
            foreach (var import in imports)
            {
                if (prevImport == import) continue;
                prevImport = import;
                if (import.LastIndexOf('.') is int p && (p == -1 || import.Substring(0, p) == lastFileOptions.Package)) continue;
                importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "") + "import " + import + ";" + lineBreak;
            }
            if (importsSrc.Length > 0) importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "") + lineBreak;
            args = args.Replace("$(Template)", lastFileOptions.Template ?? string.Empty);
            args = args.Replace("$(Import)", importsSrc);
            args = args.Replace("$(Extends)", extends);
            args = args.Replace("$(Implements)", implements);
            args = args.Replace("$(Access)", access);
            args = args.Replace("$(InheritedMethods)", string.Empty);
            args = args.Replace("$(ConstructorArguments)", paramString);
            args = args.Replace("$(Super)", superConstructor);
            args = args.Replace("$(ClassMetadata)", classMetadata);
            return args;
        }

        #endregion
    }
}