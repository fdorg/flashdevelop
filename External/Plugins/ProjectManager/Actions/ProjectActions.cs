using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace ProjectManager.Actions
{
    public delegate void ProjectModifiedHandler(string[] paths);

    /// <summary>
    /// Provides high-level actions for working with Project files.
    /// </summary>
    public class ProjectActions
    {
        IWin32Window owner; // for dialogs
        string currentLang;

        public event ProjectModifiedHandler ProjectModified;

        public ProjectActions(IWin32Window owner)
        {
            this.owner = owner;
        }

        #region New/Open Project

        public Project NewProject()
        {
            NewProjectDialog dialog = new NewProjectDialog();
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                try
                {
                    FlashDevelopActions.CheckAuthorName();
                    ProjectCreator creator = new ProjectCreator();
                    Project created = creator.CreateProject(dialog.TemplateDirectory, dialog.ProjectLocation, dialog.ProjectName, dialog.PackageName);
                    PatchProject(created);
                    return created;
                }
                catch (Exception exception)
                {
                    string msg = TextHelper.GetString("Info.CouldNotCreateProject");
                    ErrorManager.ShowInfo(msg + " " + exception.Message);
                }
            }

            return null;
        }

        public Project OpenProject()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = " " + TextHelper.GetString("Title.OpenProjectDialog");
            dialog.Filter = ProjectCreator.GetProjectFilters();

            if (dialog.ShowDialog(owner) == DialogResult.OK)
                return OpenProjectSilent(dialog.FileName);
            else
                return null;
        }

        public Project OpenProjectSilent(string path)
        {
            try
            {
                String physical = PathHelper.GetPhysicalPathName(path);
                Project loaded = ProjectLoader.Load(physical);
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

        public string ImportProject()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = TextHelper.GetString("Title.ImportProject");
            dialog.Filter = TextHelper.GetString("Info.ImportProjectFilter");
            if (dialog.ShowDialog() == DialogResult.OK && File.Exists(dialog.FileName))
            {
                string fbProject = dialog.FileName;
                string currentDirectory = Directory.GetCurrentDirectory();

                try
                {
                    if (FileInspector.IsFlexBuilderPackagedProject(fbProject))
                    {
                        fbProject = ExtractPackagedProject(fbProject);
                    }

                    if (FileInspector.IsFlexBuilderProject(fbProject))
                    {
                        AS3Project imported = AS3Project.Load(fbProject);
                        string path = Path.GetDirectoryName(imported.ProjectPath);
                        string name = Path.GetFileNameWithoutExtension(imported.OutputPath);
                        string newPath = Path.Combine(path, name + ".as3proj");
                        PatchProject(imported);
                        PatchFbProject(imported);
                        imported.SaveAs(newPath);

                        return newPath;
                    }
                    else
                        ErrorManager.ShowInfo(TextHelper.GetString("Info.NotValidFlashBuilderProject"));
                }
                catch (Exception exception)
                {
                    Directory.SetCurrentDirectory(currentDirectory);
                    string msg = TextHelper.GetString("Info.CouldNotOpenProject");
                    ErrorManager.ShowInfo(msg + " " + exception.Message);
                }
            }
            return null;
        }

        private void PatchFbProject(AS3Project project)
        {
            if (project == null || !project.MovieOptions.Platform.StartsWithOrdinal("AIR")) return;

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
            if (Win32.isRunningOnWine())
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

            if (projectPath == null || !Directory.Exists(projectPath = Path.Combine(PathHelper.ProjectsDir, projectPath)))
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

        private void PatchProject(Project project)
        {
            if (project == null) return;
            if (!project.HiddenPaths.IsHidden("obj"))
                project.HiddenPaths.Add("obj");
        }

        private string ExtractPackagedProject(string packagePath)
        {
            using (FileStream fs = new FileStream(packagePath, FileMode.Open, FileAccess.Read))
            using (ZipFile zFile = new ZipFile(fs))
            {
                if (zFile.GetEntry(".actionscriptProperties") != null)
                {
                    using (VistaFolderBrowserDialog saveDialog = new VistaFolderBrowserDialog())
                    {
                        saveDialog.ShowNewFolderButton = true;
                        saveDialog.UseDescriptionForTitle = true;
                        saveDialog.Description = TextHelper.GetString("Title.ImportPackagedProject");

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (ZipEntry entry in zFile)
                            {
                                Int32 size = 4095;
                                Byte[] data = new Byte[4095];
                                string newPath = Path.Combine(saveDialog.SelectedPath, entry.Name.Replace('/', '\\'));

                                if (entry.IsFile)
                                {
                                    Stream zip = zFile.GetInputStream(entry);
                                    String dirPath = Path.GetDirectoryName(newPath);
                                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                                    FileStream extracted = new FileStream(newPath, FileMode.Create);
                                    while (true)
                                    {
                                        size = zip.Read(data, 0, data.Length);
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
                        }

                        return Path.Combine(saveDialog.SelectedPath, ".actionScriptProperties");
                    }
                }
            }

            return string.Empty;
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
                        absPath = PathHelper.ResolvePath(relPath);
                        if (absPath == null) absPath = project.GetAbsolutePath(relPath);
                        if (absPath == null) continue;
                        if (Directory.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.LibraryPaths)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath == null) continue;
                        if (File.Exists(absPath)) classPaths.Add(absPath);
                        else if (Directory.Exists(absPath))
                        {
                            string[] libs = Directory.GetFiles(absPath, "*.swc");
                            foreach (string lib in libs) classPaths.Add(lib);
                        }
                    }
                    foreach (string relPath in options.IncludeLibraries)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath == null) continue;
                        if (Directory.Exists(absPath) || File.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.ExternalLibraryPaths)
                    {
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath == null) continue;
                        if (Directory.Exists(absPath) || File.Exists(absPath)) classPaths.Add(absPath);
                    }
                    foreach (string relPath in options.RSLPaths)
                    {
                        string[] parts = relPath.Split(',');
                        if (parts.Length < 2) continue;
                        absPath = project.GetAbsolutePath(relPath);
                        if (absPath == null) continue;
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
            if (currentLang != null && project == null)
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
            string nodeType = (node != null) ? node.GetType().ToString() : null;
            string export = (node != null && node is ExportNode) ? (node as ExportNode).Export : null;
            string textToInsert = project.GetInsertFileText(mainForm.CurrentDocument.FileName, path, export, nodeType);
            if (textToInsert == null) return;
            if (mainForm.CurrentDocument.IsEditable)
            {
                mainForm.CurrentDocument.SciControl.AddText(textToInsert.Length, textToInsert);
                mainForm.CurrentDocument.Activate();
            }
            else
            {
                string msg = TextHelper.GetString("Info.EmbedNeedsOpenDocument");
                ErrorManager.ShowInfo(msg);
            }
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
                    OnProjectModified(new string[] { path });
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

        private void OnProjectModified(string[] paths)
        {
            if (ProjectModified != null)
                ProjectModified(paths);
        }
    }
}

