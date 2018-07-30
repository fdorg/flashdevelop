using System;
using System.IO;
using System.Windows.Forms;
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
        private AS3ClassOptions lastFileOptions;
        private string lastFileFromTemplate;
        private IASContext processContext;
        private string processOnSwitch;
        private string constructorArgs;
        private List<string> constructorArgTypes;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name => "ASClassWizard";

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
            this.AddEventHandlers();
            this.InitLocalization();
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
                    DataEvent evt = (DataEvent)e;
                    if (evt.Action == "ProjectManager.CreateNewFile")
                    {
                        Hashtable table = evt.Data as Hashtable;
                        project = PluginBase.CurrentProject as Project;
                        if ((project.Language.StartsWithOrdinal("as") || project.Language == "haxe") && IsWizardTemplate(table["templatePath"] as String))
                        {
                            evt.Handled = true;
                            String className = table.ContainsKey("className") ? table["className"] as String : TextHelper.GetString("Wizard.Label.NewClass");
                            DisplayClassWizard(table["inDirectory"] as String, table["templatePath"] as String, className, table["constructorArgs"] as String, table["constructorArgTypes"] as List<String>);
                        }
                    }
                    break;

                case EventType.FileSwitch:
                    if (PluginBase.MainForm.CurrentDocument.FileName == processOnSwitch)
                    {
                        processOnSwitch = null;
                        if (lastFileOptions?.interfaces == null) return;
                        foreach (String cname in lastFileOptions.interfaces)
                        {
                            ASContext.Context.CurrentModel.Check();
                            ClassModel inClass = ASContext.Context.CurrentModel.GetPublicClass();
                            ASGenerator.SetJobContext(null, cname, null, null);
                            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, inClass, null, null);
                        }
                        lastFileOptions = null;
                    }
                    break;

                case EventType.ProcessArgs:
                    TextEvent te = e as TextEvent;
                    project = PluginBase.CurrentProject as Project;
                    if (lastFileFromTemplate != null && project != null && (project.Language.StartsWithOrdinal("as") || project.Language == "haxe"))
                    {
                        te.Value = ProcessArgs(project, te.Value);
                    }
                    break;
            }
        }

        private bool IsWizardTemplate(string templateFile)
        {
            return templateFile != null && File.Exists(templateFile + ".wizard");
        }
        
        #endregion

        #region Custom Methods

        private void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.AddEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
        }

        private void InitLocalization() => Description = TextHelper.GetString("Info.Description");

        private void DisplayClassWizard(string inDirectory, string templateFile, string className, string constructorArgs, List<string> constructorArgTypes)
        {
            var project = (Project) PluginBase.CurrentProject;
            var classpath = project.AbsoluteClasspaths.GetClosestParent(inDirectory) ?? inDirectory;
            string package;
            try
            {
                package = GetPackage(classpath, inDirectory);
                if (string.IsNullOrEmpty(package) && project.AdditionalPaths != null && project.AdditionalPaths.Length > 0)
                {
                    var closest = "";
                    foreach (var it in project.AdditionalPaths)
                        if ((classpath.StartsWithOrdinal(it) || it == ".") && it.Length > closest.Length)
                            closest = it;
                    if (closest.Length > 0) package = GetPackage(closest, inDirectory);
                }
                if (package == "")
                {
                    // search in Global classpath
                    var info = new Hashtable();
                    info["language"] = project.Language;
                    var de = new DataEvent(EventType.Command, "ASCompletion.GetUserClasspath", info);
                    EventManager.DispatchEvent(this, de);
                    if (de.Handled && info.ContainsKey("cp"))
                    {
                        var cps = info["cp"] as List<string>;
                        if (cps != null)
                        {
                            foreach (var cp in cps)
                            {
                                package = GetPackage(cp, inDirectory);
                                if (package == "") continue;
                                classpath = cp;
                                break;
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                package = "";
            }
            using (var dialog = new AS3ClassWizard())
            {
                dialog.Project = project;
                dialog.Directory = inDirectory;
                dialog.StartupClassName = className;
                if (package != null)
                {
                    package = package.Replace(Path.DirectorySeparatorChar, '.');
                    dialog.StartupPackage = package;
                }
                var conflictResult = DialogResult.OK;
                string path, newFilePath;
                var ext = project.DefaultSearchFilter.Split(';').FirstOrDefault() ?? string.Empty;
                if (ext.Length > 0) ext = ext.TrimStart('*');
                do
                {
                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    var cPackage = dialog.getPackage();
                    path = Path.Combine(classpath, cPackage.Replace('.', Path.DirectorySeparatorChar));
                    newFilePath = Path.ChangeExtension(Path.Combine(path, dialog.getClassName()), ext);
                    if (File.Exists(newFilePath))
                    {
                        string title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                        string message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFile");
                        conflictResult = MessageBox.Show(PluginBase.MainForm,
                            string.Format(message, newFilePath, "\n"), title,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        if (conflictResult == DialogResult.No) return;
                    }
                } while (conflictResult == DialogResult.Cancel);

                string templatePath = templateFile + ".wizard";
                this.lastFileFromTemplate = newFilePath;
                this.constructorArgs = constructorArgs;
                this.constructorArgTypes = constructorArgTypes;
                lastFileOptions = new AS3ClassOptions(
                    project.Language,
                    dialog.getPackage(),
                    dialog.getSuperClass(),
                    dialog.hasInterfaces() ? dialog.getInterfaces() : null,
                    dialog.isPublic(),
                    dialog.isDynamic(),
                    dialog.isFinal(),
                    dialog.getGenerateInheritedMethods(),
                    dialog.getGenerateConstructor()
                );

                try
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    PluginBase.MainForm.FileFromTemplate(templatePath, newFilePath);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowError(ex);
                }
            }
        }

        private string GetPackage(string classpath, string path)
        {
            if (!path.StartsWith(classpath, StringComparison.OrdinalIgnoreCase))
                return "";
            string subPath = path.Substring(classpath.Length).Trim('/', '\\', ' ', '.');
            return subPath.Replace(Path.DirectorySeparatorChar, '.');
        }

        private string ProcessArgs(Project project, string args)
        {
            if (lastFileFromTemplate != null)
            {
                string package = lastFileOptions != null ? lastFileOptions.Package : "";
                string fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);
                args = args.Replace("$(FileName)", fileName);
                if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
                {
                    if (package == "") args = args.Replace(" $(Package)", "");
                    args = args.Replace("$(Package)", package);
                    if (package != "") args = args.Replace("$(FileNameWithPackage)", package + "." + fileName);
                    else args = args.Replace("$(FileNameWithPackage)", fileName);
                    if (lastFileOptions != null)
                    {
                        args = ProcessFileTemplate(args);
                        if (processOnSwitch == null) lastFileOptions = null;
                    }
                }
                lastFileFromTemplate = null;
            }
            return args;
        }

        private string ProcessFileTemplate(string args)
        {
            Int32 eolMode = (Int32)PluginBase.MainForm.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            List<String> imports = new List<string>();
            string extends = "";
            string implements = "";
            string access = "";
            string inheritedMethods = "";
            string paramString = "";
            string superConstructor = "";
            string classMetadata = "";
            int index;
            // resolve imports
            if (lastFileOptions.interfaces != null && lastFileOptions.interfaces.Count > 0)
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
            if (lastFileOptions.superClass != "")
            {
                var superClassFullName = lastFileOptions.superClass;
                if (superClassFullName.Contains(".")) imports.Add(superClassFullName);
                var superClassShortName = superClassFullName.Split('.').Last();
                var fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);
                extends = fileName == superClassShortName ? $" extends {superClassFullName}" : $" extends {superClassShortName}";
                processContext = ASContext.GetLanguageContext(lastFileOptions.Language);
                if (lastFileOptions.createConstructor && processContext != null && constructorArgs == null)
                {
                    var lastDotIndex = superClassFullName.LastIndexOf('.');
                    var cmodel = processContext.GetModel(lastDotIndex < 0 ? "" : superClassFullName.Substring(0, lastDotIndex), superClassShortName, "");
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
                        foreach (MemberModel member in cmodel.Members)
                        {
                            if (member.Name == cmodel.Constructor)
                            {
                                paramString = member.ParametersString();
                                AddImports(imports, member, cmodel);
                                superConstructor = "super(";
                                index = 0;
                                if (member.Parameters != null)
                                foreach (MemberModel param in member.Parameters)
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
                processContext = null;
            }
            if (constructorArgs != null)
            {
                paramString = constructorArgs;
                foreach (String type in constructorArgTypes)
                {
                    if (!imports.Contains(type))
                    {
                        imports.Add(type);
                    }
                }
            }
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
            else
            {
                access = lastFileOptions.isDynamic ? "dynamic " : "";
            }
            string importsSrc = "";
            string prevImport = null;
            imports.Sort();
            foreach (string import in imports)
            {
                if (prevImport != import)
                {
                    prevImport = import;
                    if (import.LastIndexOf('.') == -1) continue;
                    if (import.Substring(0, import.LastIndexOf('.')) == lastFileOptions.Package) continue;
                    importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "") + "import " + import + ";" + lineBreak;
                }
            }
            if (importsSrc.Length > 0)
            {
                importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "") + lineBreak;
            }
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

        private void AddImports(List<String> imports, MemberModel member, ClassModel inClass)
        {
            AddImport(imports, member.Type, inClass);
            if (member.Parameters != null)
            {
                foreach (MemberModel item in member.Parameters)
                {
                    AddImport(imports, item.Type, inClass);
                }
            }
        }

        private void AddImport(List<string> imports, string cname, ClassModel inClass)
        {
            var aClass = processContext.ResolveType(cname, inClass.InFile);
            if (!aClass.IsVoid() && aClass.InFile.Package != "")
            {
                imports.Add(aClass.QualifiedName);
            }
        }

        #endregion
    }
}
