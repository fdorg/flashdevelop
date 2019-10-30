using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ASClassWizard.Resources;
using ASClassWizard.Wizards;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;

namespace ASClassWizard.Helpers
{
    public static class WizardUtils
    {
        public static bool IsWizardTemplate(string templateFile) => templateFile != null && File.Exists(templateFile + ".wizard");

        public static bool ProcessWizard(string inDirectory, string name, Project project, IWizard dialog, out string path, out string newFilePath)
        {
            var classpath = project.AbsoluteClasspaths.GetClosestParent(inDirectory) ?? inDirectory;
            var package = GetPackage(project, ref classpath, inDirectory);
            dialog.Project = project;
            dialog.Directory = inDirectory;
            dialog.StartupClassName = name;
            package = package.Replace(Path.DirectorySeparatorChar, '.');
            dialog.StartupPackage = package;

            var conflictResult = DialogResult.OK;
            var ext = project.DefaultSearchFilter.Split(';').FirstOrDefault() ?? String.Empty;
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
                    String.Format(message, newFilePath, "\n"), title,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (conflictResult == DialogResult.No) return true;
            } while (conflictResult == DialogResult.Cancel);
            return false;
        }

        static string GetPackage(Project project, ref string classpath, string inDirectory)
        {
            try
            {
                var package = GetPackage(classpath, inDirectory);
                if (String.IsNullOrEmpty(package) && !project.AdditionalPaths.IsNullOrEmpty())
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
                return String.Empty;
            }
        }

        static string GetPackage(string classpath, string path)
        {
            if (!path.StartsWith(classpath, StringComparison.OrdinalIgnoreCase)) return String.Empty;
            return path.Substring(classpath.Length)
                .Trim('/', '\\', ' ', '.')
                .Replace(Path.DirectorySeparatorChar, '.');
        }

        public static AS3ClassOptions GetWizardOptions(IProject project, IWizard dialog, string typeTemplate)
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

        public static void AddImports(IASContext ctx, MemberModel member, FileModel inFile, ICollection<string> result)
        {
            AddImport(ctx, member.Type, inFile, result);
            if (member.Parameters is null) return;
            foreach (var item in member.Parameters)
            {
                var types = ASContext.Context.DecomposeTypes(new[] {item.Type});
                foreach (var type in types)
                {
                    AddImport(ctx, type, inFile, result);
                }
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