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

namespace ASClassWizard
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "ASClassWizard";
        private String pluginGuid = "a2c159c1-7d21-4483-aeb1-38d9fdc4c7f3";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Provides an ActionScript class wizard for FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";

        private AS3ClassOptions lastFileOptions;
        private String lastFileFromTemplate;
        private IASContext processContext;
        private String processOnSwitch;
        private String constructorArgs;
        private List<String> constructorArgTypes;

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
            get { return null; }
        }
        
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
        
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
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
                        if (lastFileOptions == null || lastFileOptions.interfaces == null) return;
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

        public static IMainForm MainForm { get { return PluginBase.MainForm; } }

        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.AddEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
        }

        public void InitLocalization()
        {
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        private void DisplayClassWizard(String inDirectory, String templateFile, String className, String constructorArgs, List<String> constructorArgTypes)
        {
            Project project = PluginBase.CurrentProject as Project;
            String classpath = project.AbsoluteClasspaths.GetClosestParent(inDirectory) ?? inDirectory;
            String package;
            try
            {
                package = GetPackage(classpath, inDirectory);
                if (package == "")
                {
                    // search in Global classpath
                    Hashtable info = new Hashtable();
                    info["language"] = project.Language;
                    DataEvent de = new DataEvent(EventType.Command, "ASCompletion.GetUserClasspath", info);
                    EventManager.DispatchEvent(this, de);
                    if (de.Handled && info.ContainsKey("cp"))
                    {
                        List<string> cps = info["cp"] as List<string>;
                        if (cps != null)
                        {
                            foreach (string cp in cps)
                            {
                                package = GetPackage(cp, inDirectory);
                                if (package != "")
                                {
                                    classpath = cp;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.NullReferenceException)
            {
                package = "";
            }
            using (AS3ClassWizard dialog = new AS3ClassWizard())
            {
                bool isHaxe = project.Language == "haxe";
                dialog.Project = project;
                dialog.Directory = inDirectory;
                dialog.StartupClassName = className;
                if (package != null)
                {
                    package = package.Replace(Path.DirectorySeparatorChar, '.');
                    dialog.StartupPackage = package;
                }
                DialogResult conflictResult = DialogResult.OK;
                string cPackage, path, newFilePath;
                do
                {
                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    cPackage = dialog.getPackage();
                    path = Path.Combine(classpath, cPackage.Replace('.', Path.DirectorySeparatorChar));
                    newFilePath = Path.ChangeExtension(Path.Combine(path, dialog.getClassName()),
                                                              isHaxe ? ".hx" : ".as");
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
                    MainForm.FileFromTemplate(templatePath, newFilePath);
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
            string subPath = path.Substring(classpath.Length).Trim(new char[] { '/', '\\', ' ', '.' });
            return subPath.Replace(Path.DirectorySeparatorChar, '.');
        }

        public string ProcessArgs(Project project, string args)
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
            Int32 eolMode = (Int32)MainForm.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            ClassModel cmodel;
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
                bool isHaxe2 = PluginBase.CurrentSDK != null && PluginBase.CurrentSDK.Name.ToLower().Contains("haxe 2");
                implements = " implements ";
                string[] _implements;
                index = 0;
                foreach (string item in lastFileOptions.interfaces)
                {
                    if (item.Split('.').Length > 1) imports.Add(item);
                    _implements = item.Split('.');
                    implements += (index > 0 ? (isHaxe2 ? ", implements " : ", ") : "") + _implements[_implements.Length - 1];
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
                String super = lastFileOptions.superClass;
                if (lastFileOptions.superClass.Split('.').Length > 1) imports.Add(super);
                string[] _extends = super.Split('.');
                extends = " extends " + _extends[_extends.Length - 1];
                processContext = ASContext.GetLanguageContext(lastFileOptions.Language);
                if (lastFileOptions.createConstructor && processContext != null && constructorArgs == null)
                {
                    cmodel = processContext.GetModel(super.LastIndexOf('.') < 0 ? "" : super.Substring(0, super.LastIndexOf('.')), _extends[_extends.Length - 1], "");
                    if (!cmodel.IsVoid())
                    {
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

        private void AddImport(List<string> imports, String cname, ClassModel inClass)
        {
            ClassModel aClass = processContext.ResolveType(cname, inClass.InFile);
            if (aClass != null && !aClass.IsVoid() && aClass.InFile.Package != "")
            {
                imports.Add(aClass.QualifiedName);
            }
        }

        #endregion
    }
}
