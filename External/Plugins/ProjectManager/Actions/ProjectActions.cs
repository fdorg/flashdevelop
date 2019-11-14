using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using Ookii.Dialogs;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Controls;
using ProjectManager.Controls.TreeView;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.Haxe;
using ProjectManager.Projects.Generic;

namespace ProjectManager.Actions
{
    public delegate void ProjectModifiedHandler(string[] paths);

    /// <summary>
    /// Provides high-level actions for working with Project files.
    /// </summary>
    public class ProjectActions
    {
        readonly IWin32Window owner; // for dialogs
        string currentLang;

        public event ProjectModifiedHandler ProjectModified;

        public ProjectActions(IWin32Window owner)
        {
            this.owner = owner;
        }

        #region New/Open Project

        public Project NewProject()
        {
            using var dialog = new NewProjectDialog();
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                try
                {
                    FlashDevelopActions.CheckAuthorName();
                    var creator = new ProjectCreator();
                    var created = creator.CreateProject(dialog.TemplateDirectory, dialog.ProjectLocation, dialog.ProjectName, dialog.PackageName);
                    PatchProject(created);
                    return created;
                }
                catch (Exception exception)
                {
                    var msg = TextHelper.GetString("Info.CouldNotCreateProject");
                    ErrorManager.ShowInfo(msg + " " + exception.Message);
                }
            }

            return null;
        }

        public Project OpenProject()
        {
            using var dialog = new OpenFileDialog();
            dialog.Title = " " + TextHelper.GetString("Title.OpenProjectDialog");
            dialog.Filter = ProjectCreator.GetProjectFilters();

            if (dialog.ShowDialog(owner) == DialogResult.OK)
                return OpenProjectSilent(dialog.FileName);

            return null;
        }

        public Project OpenProjectSilent(string path)
        {
            try
            {
                var physical = PathHelper.GetPhysicalPathName(path);
                var loaded = ProjectLoader.Load(physical);
                PatchProject(loaded);
                return loaded;
            }
            catch (Exception exception)
            {
                string msg = TextHelper.GetString("Info.CouldNotOpenProject");
                ErrorManager.ShowInfo(msg + " " + exception.Message);
                return null;
            }
        }

        public Project OpenFolder()
        {
            using (VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog())
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return OpenFolderSilent(dialog.SelectedPath);
                }
            }
            return null;
        }

        public Project OpenFolderSilent(string path)
        {
            String[] hxmlFiles = Directory.GetFiles(path, "*.hxml");
            if (hxmlFiles.Length > 0)
            {
                var project = new HaxeProject(path);
                project.RawHXML = File.ReadAllLines(hxmlFiles[0]);
                PatchProject(project);
                PatchHxmlProject(project);
                return project;
            }
            else return new GenericProject(path);
        }

        public string ImportProject() => ImportProject(null);

        internal string ImportProject(string importFrom)
        {
            using var dialog = new OpenFileDialog();
            dialog.Title = TextHelper.GetString("Title.ImportProject");
            dialog.Filter = TextHelper.GetString("Info.ImportProjectFilter");
            if (importFrom == "hxml") dialog.FilterIndex = 3;
            if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName)) return null;
            var fileName = dialog.FileName;
            var currentDirectory = Directory.GetCurrentDirectory();
            try
            {
                if (FileInspector.IsHxml(Path.GetExtension(fileName).ToLower()))
                {
                    var project = HaxeProject.Load(fileName);
                    var path = Path.GetDirectoryName(project.ProjectPath);
                    var name = Path.GetFileNameWithoutExtension(project.OutputPath);
                    var newPath = Path.Combine(path, $"{name}.hxproj");
                    PatchProject(project);
                    PatchHxmlProject(project);
                    project.SaveAs(newPath);
                    return newPath;
                }
                if (FileInspector.IsFlexBuilderPackagedProject(fileName))
                {
                    fileName = ExtractPackagedProject(fileName);
                }
                if (FileInspector.IsFlexBuilderProject(fileName))
                {
                    var imported = AS3Project.Load(fileName);
                    var path = Path.GetDirectoryName(imported.ProjectPath);
                    var name = Path.GetFileNameWithoutExtension(imported.OutputPath);
                    var newPath = Path.Combine(path, $"{name}.as3proj");
                    PatchProject(imported);
                    PatchFbProject(imported);
                    imported.SaveAs(newPath);
                    return newPath;
                }
                ErrorManager.ShowInfo(TextHelper.GetString("Info.NotValidFlashBuilderProject"));
            }
            catch (Exception exception)
            {
                Directory.SetCurrentDirectory(currentDirectory);
                var msg = TextHelper.GetString("Info.CouldNotOpenProject");
                ErrorManager.ShowInfo(msg + " " + exception.Message);
            }
            return null;
        }

        static void PatchHxmlProject(Project project)
        {
            project.OutputPath = Path.GetFileName(project.ProjectPath);
            project.MovieOptions.Background = string.Empty;
            project.MovieOptions.BackgroundColor = Color.Empty;
            project.MovieOptions.Platform = "hxml";
            project.MovieOptions.Fps = 0;
            project.MovieOptions.Width = 0;
            project.MovieOptions.Height = 0;
            project.MovieOptions.Version = string.Empty;
        }

        static void PatchFbProject(Project project)
        {
            if (project is null || !project.MovieOptions.Platform.StartsWithOrdinal("AIR")) return;

            // We do this because the batch files cannot automatically detect the path changes caused by debug/release differences
            bool trace = project.TraceEnabled;
            project.TraceEnabled = false;
            project.OutputPath = project.FixDebugReleasePath(project.OutputPath);
            project.TraceEnabled = trace;

            string path = Path.GetDirectoryName(project.ProjectPath);
            string descriptor = "src\\" + Path.GetFileNameWithoutExtension(project.OutputPath) + "-app.xml";

            project.TestMovieBehavior = TestMovieBehavior.Custom;
            project.TestMovieCommand = "bat\\RunApp.bat";

            // CrossOver template related mod
            if (PlatformHelper.isRunningOnWine())
            {
                project.TestMovieCommand += " $(TargetBuild)";
            }

            if (!File.Exists(Path.Combine(path, descriptor)))
            {
                // Either it's some library project (we'll deal with these later) 
                // or it's placed in some folder different to the default one (same as above)
                return;
            }

            // We copy the needed project template files
            bool isFlex = project.CompileTargets.Count > 0 && Path.GetExtension(project.CompileTargets[0]).ToLower() == ".mxml";
            string projectPath;
            var excludedFiles = new List<string>(); // This could be a setting, in any case, decided to do this in case someone added custom files to the project templates...
            if (project.MovieOptions.Platform == "AIR Mobile")
            {
                projectPath = isFlex ? project.MovieOptions.PlatformSupport.GetProjectTemplate("flex") : project.MovieOptions.PlatformSupport.GetProjectTemplate("as3");
                excludedFiles.AddRange(new[] { "application.xml.template", "Project.as3proj", "Project.png", "Project.txt", "bin", "cert", "icons", "src" });
            }
            else
            {
                // The files are the same for Flex 3 and 4, so no need to discern them
                projectPath = isFlex ? project.MovieOptions.PlatformSupport.GetProjectTemplate("flex4") : project.MovieOptions.PlatformSupport.GetProjectTemplate("as3");
                excludedFiles.AddRange(new[] { "application.xml.template", "Project.as3proj", "Project.png", "Project.txt", "bin", "src" });
            }

            if (projectPath is null || !Directory.Exists(projectPath = Path.Combine(PathHelper.ProjectsDir, projectPath)))
            {
                string info = TextHelper.GetString("Info.TemplateDirNotFound");
                ErrorManager.ShowWarning(info, null);
                return;
            }
            var creator = new ProjectCreator();
            creator.SetContext(Path.GetFileNameWithoutExtension(project.OutputPath), string.Empty);
            foreach (var file in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
            {
                bool excluded = false;
                foreach (var excludedFile in excludedFiles)
                    if (file.StartsWithOrdinal(Path.Combine(projectPath, excludedFile)))
                    {
                        excluded = true;
                        break;
                    }

                if (excluded) continue;
                var fileDirectory = Path.GetDirectoryName(file).Replace(projectPath, string.Empty);
                if (fileDirectory.StartsWith('\\')) fileDirectory = fileDirectory.Substring(1);
                var folder = Path.Combine(path, fileDirectory);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string newFile = Path.Combine(folder, Path.GetFileName(file));
                if (Path.GetExtension(file).ToLower() == ".template")
                {
                    creator.CopyFile(file, newFile);
                }
                else
                {
                    File.Copy(file, newFile, true);
                }
            }

            // We configure the batch files
            var configurator = new AirConfigurator { ApplicationSetupBatch = Path.Combine(path, "bat\\SetupApp.bat") };
            configurator.ApplicationSetupParams[AirConfigurator.DescriptorPath] = descriptor;
            configurator.ApplicationSetupParams[AirConfigurator.PackageDir] = Path.GetFileName(Path.GetDirectoryName(project.OutputPath));
            configurator.SetUp();

            // We change the descriptor file so it targets our output file. FB does this dynamically.
            descriptor = Path.Combine(path, descriptor);
            var fileInfo = FileHelper.GetEncodingFileInfo(descriptor);
            string contents = Regex.Replace(fileInfo.Contents, "<content>\\[This value will be overwritten by (Flex|Flash) Builder in the output app.xml]</content>", "<content>" + Path.GetFileName(project.OutputPath) + "</content>");
            FileHelper.WriteFile(descriptor, contents, Encoding.GetEncoding(fileInfo.CodePage), fileInfo.ContainsBOM);
        }

        static void PatchProject(Project project)
        {
            if (project is null) return;
            if (!project.HiddenPaths.IsHidden("obj"))
                project.HiddenPaths.Add("obj");
        }

        static string ExtractPackagedProject(string packagePath)
        {
            using var stream = new FileStream(packagePath, FileMode.Open, FileAccess.Read);
            using var zFile = new ZipFile(stream);
            if (zFile.GetEntry(".actionscriptProperties") is null) return string.Empty;
            using var saveDialog = new VistaFolderBrowserDialog();
            saveDialog.ShowNewFolderButton = true;
            saveDialog.UseDescriptionForTitle = true;
            saveDialog.Description = TextHelper.GetString("Title.ImportPackagedProject");
            if (saveDialog.ShowDialog() != DialogResult.OK) return Path.Combine(saveDialog.SelectedPath, ".actionScriptProperties");
            foreach (ZipEntry entry in zFile)
            {
                var newPath = Path.Combine(saveDialog.SelectedPath, entry.Name.Replace('/', '\\'));
                if (entry.IsFile)
                {
                    var data = new byte[4095];
                    var zip = zFile.GetInputStream(entry);
                    var dirPath = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                    var extracted = new FileStream(newPath, FileMode.Create);
                    while (true)
                    {
                        var size = zip.Read(data, 0, data.Length);
                        if (size > 0) extracted.Write(data, 0, size);
                        else break;
                    }
                    extracted.Close();
                    extracted.Dispose();
                }
                else
                {
                    Directory.CreateDirectory(newPath);
                }
            }
            return Path.Combine(saveDialog.SelectedPath, ".actionScriptProperties");
        }

        #endregion

        #region Update ASCompletion

        public void UpdateASCompletion(IMainForm mainForm, Project project)
        {
            List<string> classPaths = new List<string>();
            List<string> hiddenPaths = new List<string>();
            string version;
            string platform = "";

            if (project != null)
            {
                BuildActions.GetCompilerPath(project); // refresh project's SDK
                project.UpdateVars(true);

                // platform/version
                platform = project.MovieOptions.Platform;
                version = project.MovieOptions.Version;
                if (platform != PlatformData.FLASHPLAYER_PLATFORM
                    && project.MovieOptions.HasPlatformSupport && project.MovieOptions.PlatformSupport.IsFlashPlatform)
                    version = PlatformData.ResolveFlashPlayerVersion(project.Language, platform, version);

                // add project classpaths
                foreach (string cp in project.AbsoluteClasspaths)
                    if (Directory.Exists(cp)) classPaths.Add(cp);

                // add AS3 libraries
                string absPath;
                if (project is AS3Project)
                {
                    MxmlcOptions options = (project as AS3Project).CompilerOptions;
                    foreach (string relPath in options.IntrinsicPaths)
                    {
                        absPath = PathHelper.ResolvePath(relPath) ?? project.GetAbsolutePath(relPath);
                        if (absPath is null) continue;
                        if (Directory.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.LibraryPaths)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath is null) continue;
                        if (File.Exists(absPath)) classPaths.Add(absPath);
                        else if (Directory.Exists(absPath))
                        {
                            string[] libs = Directory.GetFiles(absPath, "*.swc");
                            classPaths.AddRange(libs);
                        }
                    }
                    foreach (string relPath in options.IncludeLibraries)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath is null) continue;
                        if (Directory.Exists(absPath) || File.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.ExternalLibraryPaths)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath is null) continue;
                        if (Directory.Exists(absPath) || File.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.RSLPaths)
                    {
                        string[] parts = relPath.Split(',');
                        if (parts.Length < 2) continue;
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath is null) continue;
                        if (File.Exists(absPath)) classPaths.Add(absPath);
                    }
                }

                string dir = project.Directory;
                foreach (string hidPath in project.HiddenPaths)
                {
                    absPath = Path.Combine(dir, hidPath);
                    foreach (string cp in classPaths)
                        if (absPath.StartsWithOrdinal(cp))
                        {
                            hiddenPaths.Add(absPath);
                            break;
                        }
                }
            }
            else if (PlatformData.SupportedLanguages.ContainsKey("as3"))
            {
                var targets = PlatformData.SupportedLanguages["as3"].Platforms;
                var flashPlatform = targets[PlatformData.FLASHPLAYER_PLATFORM];
                version = flashPlatform.LastVersion.Value;
            }
            else version = "11.0";

            DataEvent de;
            Hashtable info = new Hashtable();
            // release old classpath            
            if (currentLang != null && project is null)
            {
                info["lang"] = currentLang;
                info["platform"] = "";
                info["targetBuild"] = "";
                info["version"] = "0.0";
                info["classpath"] = null;
                info["hidden"] = null;

                de = new DataEvent(EventType.Command, "ASCompletion.ClassPath", info);
                EventManager.DispatchEvent(this, de);
            }

            // set new classpath
            if (project != null)
            {
                currentLang = project.Language;

                info["platform"] = platform;
                info["version"] = version;
                info["targetBuild"] = project.TargetBuild;
                info["lang"] = currentLang;
                info["classpath"] = classPaths.ToArray();
                info["hidden"] = hiddenPaths.ToArray();

                de = new DataEvent(EventType.Command, "ASCompletion.ClassPath", info);
                EventManager.DispatchEvent(this, de);

                project.AdditionalPaths = info.ContainsKey("additional") ? info["additional"] as string[] : null;
            }
            else currentLang = null;
        }

        #endregion

        #region Project File Reference Updating

        public void RemoveAllReferences(Project project, string path)
        {
            if (project.IsLibraryAsset(path))
                project.SetLibraryAsset(path, false);

            if (project.IsCompileTarget(path))
                project.SetCompileTarget(path, false);
        }

        public void MoveReferences(Project project, string fromPath, string toPath)
        {
            if (project.IsCompileTarget(fromPath))
            {
                project.SetCompileTarget(fromPath, false);
                project.SetCompileTarget(toPath, true);
            }

            if (project.IsLibraryAsset(fromPath))
            {
                project.ChangeAssetPath(fromPath, toPath);
            }
        }

        #endregion

        #region Working with Project Files

        public void InsertFile(IMainForm mainForm, Project project, string path, GenericNode node)
        {
            if (!mainForm.CurrentDocument.IsEditable) return;
            var nodeType = node?.GetType().ToString();
            var export = (node is ExportNode exportNode) ? exportNode.Export : null;
            var textToInsert = project.GetInsertFileText(mainForm.CurrentDocument.FileName, path, export, nodeType);
            if (textToInsert is null) return;
            mainForm.CurrentDocument.SciControl.AddText(textToInsert.Length, textToInsert);
            mainForm.CurrentDocument.Activate();
        }

        public void ToggleLibraryAsset(Project project, string[] paths)
        {
            foreach (string path in paths)
            {
                bool isResource = project.IsLibraryAsset(path);
                project.SetLibraryAsset(path, !isResource);
            }
            project.Save();
            OnProjectModified(paths);
        }

        public void ToggleShowHidden(Project project)
        {
            project.ShowHiddenPaths = !project.ShowHiddenPaths;
            project.Save();
            OnProjectModified(null);
        }

        public void ToggleDocumentClass(Project project, string[] paths)
        {
            foreach (string path in paths)
            {
                bool isMain = project.IsDocumentClass(path);
                project.SetDocumentClass(path, !isMain);
            }
            project.Save();
            OnProjectModified(null);
        }

        public void ToggleAlwaysCompile(Project project, string[] paths)
        {
            foreach (string path in paths)
            {
                bool isTarget = project.IsCompileTarget(path);
                project.SetCompileTarget(path, !isTarget);
            }
            if (project.MaxTargetsCount > 0)
            {
                while (project.CompileTargets.Count > project.MaxTargetsCount)
                {
                    int len = project.CompileTargets.Count;
                    string relPath = project.CompileTargets[0];
                    project.SetCompileTarget(relPath, false);
                    if (project.CompileTargets.Count == len) // safety if path is not removed
                        project.CompileTargets.RemoveAt(0);

                    string path = project.GetAbsolutePath(relPath);
                    OnProjectModified(new[] { path });
                }
            }
            project.Save();
            OnProjectModified(paths);
        }

        public void ToggleHidden(Project project, string[] paths)
        {
            foreach (string path in paths)
            {
                bool isHidden = project.IsPathHidden(path);
                project.SetPathHidden(path, !isHidden);
            }
            project.Save();

            OnProjectModified(null);
        }

        #endregion

        void OnProjectModified(string[] paths) => ProjectModified?.Invoke(paths);
    }
}

