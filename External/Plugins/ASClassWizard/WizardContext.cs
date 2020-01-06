using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ASClassWizard.Resources;
using ASClassWizard.Wizards;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Projects;

namespace ASClassWizard
{
    public static class WizardContext
    {

        internal static AS3ClassOptions lastFileOptions;
        internal static string lastFileFromTemplate;
        internal static string processOnSwitch;
        internal static string constructorArgs;
        internal static List<string> constructorArgTypes;

        public static bool IsWizardTemplate(string templateFile) => templateFile != null && File.Exists(templateFile + ".wizard");

        public static void DisplayWizard(IWizard dialog, string inDirectory, string templateFile, string typeTemplate, string name, string constructorArgs, List<string> constructorArgTypes)
        {
            var project = (Project) PluginBase.CurrentProject;
            if (ProcessWizard(dialog, inDirectory, name, project, out var path, out var newFilePath)) return;
            lastFileFromTemplate = newFilePath;
            WizardContext.constructorArgs = constructorArgs;
            WizardContext.constructorArgTypes = constructorArgTypes;
            lastFileOptions = GetWizardOptions(project, dialog, typeTemplate);
            FileFromTemplate(path, templateFile, newFilePath);
        }

        public static bool ProcessWizard(IWizard dialog, string inDirectory, string name, Project project, out string path, out string newFilePath)
        {
            var classpath = project.AbsoluteClasspaths.GetClosestParent(inDirectory) ?? inDirectory;
            var package = GetPackage(project, ref classpath, inDirectory);
            dialog.Project = project;
            dialog.Directory = inDirectory;
            dialog.StartupClassName = name;
            package = package.Replace(Path.DirectorySeparatorChar, '.');
            dialog.StartupPackage = package;

            var conflictResult = DialogResult.OK;
            var ext = project.DefaultSearchFilter.Split(';').FirstOrDefault() ?? string.Empty;
            if (ext.Length > 0) ext = ext.TrimStart('*');
            do
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    path = null;
                    newFilePath = null;
                    return true;
                }
                var cPackage = dialog.GetPackage();
                path = Path.Combine(classpath, cPackage.Replace('.', Path.DirectorySeparatorChar));
                newFilePath = Path.ChangeExtension(Path.Combine(path, dialog.GetName()), ext);
                if (!File.Exists(newFilePath)) continue;
                var title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                var message = TextHelper.GetString("PluginCore.Info.FolderAlreadyContainsFile");
                conflictResult = MessageBox.Show(PluginBase.MainForm, 
                    string.Format(message, newFilePath, "\n"), title,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (conflictResult == DialogResult.No) return true;
            } while (conflictResult == DialogResult.Cancel);
            return false;
        }

        static AS3ClassOptions GetWizardOptions(IProject project, IWizard dialog, string typeTemplate)
        {
            return new AS3ClassOptions(
                language: project.Language,
                package: dialog.GetPackage(),
                super_class: dialog.GetExtends(),
                Interfaces: dialog.GetInterfaces(),
                is_public: dialog.IsPublic(),
                is_dynamic: dialog.IsDynamic(),
                is_final: dialog.IsFinal(),
                create_inherited: dialog.GetGenerateInheritedMethods(),
                create_constructor: dialog.GetGenerateConstructor()
            ) {Template = typeTemplate};
        }

        static void FileFromTemplate(string directoryPath, string templateFile, string filePath)
        {
            try
            {
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                PluginBase.MainForm.FileFromTemplate(templateFile + ".wizard", filePath);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        static string GetPackage(Project project, ref string classpath, string inDirectory)
        {
            try
            {
                var package = GetPackage(classpath, inDirectory);
                if (string.IsNullOrEmpty(package) && !project.AdditionalPaths.IsNullOrEmpty())
                {
                    var closest = "";
                    foreach (var it in project.AdditionalPaths)
                        if ((classpath.StartsWithOrdinal(it) || it == ".") && it.Length > closest.Length)
                            closest = it;
                    if (closest.Length > 0) package = GetPackage(closest, inDirectory);
                }
                if (package != "") return package;
                // search in Global classpath
                var info = new Hashtable {["language"] = project.Language};
                var de = new DataEvent(EventType.Command, "ASCompletion.GetUserClasspath", info);
                EventManager.DispatchEvent(null, de);
                if (de.Handled && info.ContainsKey("cp") && info["cp"] is List<string> cps)
                {
                    foreach (var cp in cps)
                    {
                        package = GetPackage(cp, inDirectory);
                        if (package == "") continue;
                        classpath = cp;
                        break;
                    }
                }
                return package;
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
        }

        static string GetPackage(string classpath, string path)
        {
            if (!path.StartsWith(classpath, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            return path.Substring(classpath.Length)
                .Trim('/', '\\', ' ', '.')
                .Replace(Path.DirectorySeparatorChar, '.');
        }

        public static string ProcessArgs(string args)
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

        static string ProcessFileTemplate(string args)
        {
            var eolMode = (int)PluginBase.Settings.EOLMode;
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
                    var isHaxe2 = PluginBase.CurrentSDK != null && PluginBase.CurrentSDK.Name.ToLower().Contains("haxe 2");
                    implementContinuation = isHaxe2 ? ", implements " : " implements ";
                }
                else
                {
                    implementContinuation = ", ";
                }

                foreach (var item in lastFileOptions.interfaces)
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
                if (superClassFullName.Contains('.')) imports.Add(superClassFullName);
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
                            cmodel.SearchMember(FlagType.Constructor, true, out cmodel);
                        foreach (var member in cmodel.Members)
                        {
                            if (member.Name != cmodel.Constructor) continue;
                            paramString = member.ParametersString();
                            AddImports(ctx, member, cmodel.InFile, imports);
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
                foreach (var type in constructorArgTypes)
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
                if (import.LastIndexOf('.') is { } p && (p == -1 || import.Substring(0, p) == lastFileOptions.Package)) continue;
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

        static void AddImports(IASContext ctx, MemberModel member, FileModel inFile, ICollection<string> result)
        {
            var types = new List<string> {member.Type};
            if (member.Parameters != null) types.AddRange(member.Parameters.Select(it => it.Type));
            foreach (var type in ASContext.Context.DecomposeTypes(types))
            {
                AddImport(ctx, type, inFile, result);
            }
        }

        static void AddImport(IASContext ctx, string cname, FileModel inFile, ICollection<string> result)
        {
            var aClass = ctx.ResolveType(cname, inFile);
            if (!aClass.IsVoid() && aClass.InFile.Package != "")
            {
                result.Add(aClass.QualifiedName);
            }
        }
    }
}