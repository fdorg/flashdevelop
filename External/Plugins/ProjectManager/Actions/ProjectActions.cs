using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Localization;
using ProjectManager.Controls;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using ProjectManager.Projects.AS2;
using ProjectManager.Projects.AS3;
using PluginCore.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using ProjectManager.Controls.TreeView;
using System.Text.RegularExpressions;

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

                try
                {
                    if (FileInspector.IsFlexBuilderPackagedProject(fbProject))
                    {
                        fbProject = ExtractPackagedProject(fbProject);
                    }

                    if (FileInspector.IsFlexBuilderProject(fbProject))
                    {
                        Project imported = AS3Project.Load(fbProject);
                        string path = Path.GetDirectoryName(imported.ProjectPath);
                        string name = Path.GetFileName(path);
                        string newPath = Path.Combine(path, name + ".as3proj");
                        PatchProject(imported);
                        imported.SaveAs(newPath);
                        return newPath;
                    }
                    else
                        ErrorManager.ShowInfo(TextHelper.GetString("Info.NotValidFlashBuilderProject"));
                }
                catch (Exception exception)
                {
                    string msg = TextHelper.GetString("Info.CouldNotOpenProject");
                    ErrorManager.ShowInfo(msg + " " + exception.Message);
                }
            }
            return null;
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
                    using (FolderBrowserDialog saveDialog = new FolderBrowserDialog())
                    {
                        saveDialog.ShowNewFolderButton = true;
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
                                    String ext = Path.GetExtension(newPath);
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
                        if (absPath.StartsWith(cp))
                        {
                            hiddenPaths.Add(absPath);
                            break;
                        }
                }
            }
            else
            {
                var targets = PluginCore.PlatformData.SupportedLanguages["as3"].Platforms;
                var flashPlatform = targets[PlatformData.FLASHPLAYER_PLATFORM];
                version = flashPlatform.LastVersion.Value;
            }

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
				project.ChangeAssetPath(fromPath,toPath);
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
