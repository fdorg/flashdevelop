using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using PluginCore.Helpers;
using ProjectManager.Controls;
using ProjectManager.Controls.AS3;

namespace ProjectManager.Projects.AS3
{
    public class AS3Project : Project
    {
        public AS3Project(string path) : base(path, new MxmlcOptions())
        {
            movieOptions = new AS3MovieOptions();
            SwcLibraries = new AssetCollection(this);
        }
        
        public override string Name 
        { 
            get 
            {
                if (FileInspector.IsFlexBuilderProject(ProjectPath)) return Path.GetFileName(Path.GetDirectoryName(ProjectPath));
                else return Path.GetFileNameWithoutExtension(ProjectPath); 
            } 
        }

        public override string Language { get { return "as3"; } }
        public override string LanguageDisplayName { get { return "AS3"; } }
        public override bool IsCompilable { get { return true; } }
        public override bool ReadOnly { get { return FileInspector.IsFlexBuilderProject(ProjectPath); } }
        public override bool HasLibraries { get { return OutputType == OutputType.Application || OutputType == OutputType.Library; } }
        public override int MaxTargetsCount { get { return 1; } }
        public override string DefaultSearchFilter { get { return "*.as;*.mxml"; } }

        public new MxmlcOptions CompilerOptions { get { return (MxmlcOptions)base.CompilerOptions; } }

        public override PropertiesDialog CreatePropertiesDialog()
        {
            return new AS3PropertiesDialog();
        }

        public override void ValidateBuild(out string error)
        {
            if (CompileTargets.Count == 0) error = "Description.MissingEntryPoint";
            else error = null;
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            if (nodeType == "ProjectManager.Controls.TreeView.ClassExportNode") return export;
            
            string pre = "";
            string post = "";
            string ext = Path.GetExtension(inFile).ToLower();
            if (ext == ".as") { pre = "["; post = "]"; }

            string relPath = ProjectPaths.GetRelativePath(Path.GetDirectoryName(inFile), path).Replace('\\', '/');
            string fileExt = Path.GetExtension(path).ToLower();
            if (export != null)
            {
                if (export.IndexOf('(') > 0)
                {
                    string fontName = export.Substring(0, export.IndexOf('(')).Trim();
                    return String.Format("{0}Embed(source=\"{1}\", fontFamily=\"{2}\"){3}", pre, relPath, fontName, post);
                }
                else return String.Format("{0}Embed(source=\"{1}\", symbol=\"{2}\"){3}", pre, relPath, export, post);
            }
            else if (FileInspector.IsImage(relPath, fileExt) || IsText(fileExt) 
                || FileInspector.IsFont(relPath, fileExt) || FileInspector.IsSound(relPath, fileExt))
            {
                return String.Format("{0}Embed(source=\"{1}\"){2}", pre, relPath, post);
            }
            else return String.Format("{0}Embed(source=\"{1}\", mimeType=\"application/octet-stream\"){2}", pre, relPath, post);
        }

        private bool IsText(string ext)
        {
            return ext == ".txt" || ext == ".xml";
        }

        public override CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            if (isDirectory || (Path.GetExtension(path) != ".as" && Path.GetExtension(path) != ".mxml")) 
                return CompileTargetType.None;

            /*foreach (string cp in AbsoluteClasspaths)
                if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                    return CompileTargetType.DocumentClass;
            return CompileTargetType.None;*/
            return CompileTargetType.DocumentClass; // can actually be outside of the classpath...
        }

        public override bool IsDocumentClass(string path)
        {
            return IsCompileTarget(path);
        }

        public override void SetDocumentClass(string path, bool isMain)
        {
            CompileTargets.Clear();
            if (isMain) SetCompileTarget(path, true);
        }

        public override bool Clean()
        {
            try
            {
                if (!string.IsNullOrEmpty(OutputPath) && File.Exists(GetAbsolutePath(OutputPath)))
                    File.Delete(GetAbsolutePath(OutputPath));
                return true;
            }
            catch {
                return false;
            }
        }

        public override string GetOtherIDE(bool runOutput, bool releaseMode, out string error)
        {
            error = null;
            return "FlashIDE";
        }

        #region SWC assets management

        public AssetCollection SwcLibraries;

        public override bool IsLibraryAsset(string path)
        {
            if (!FileInspector.IsSwc(path) && !base.IsDirectory(path)) return base.IsLibraryAsset(path);
            else return SwcLibraries.Contains(path) || SwcLibraries.Contains(GetRelativePath(path));
        }

        public override LibraryAsset GetAsset(string path)
        {
            if (!FileInspector.IsSwc(path) && !base.IsDirectory(path)) return base.GetAsset(path);
            else if (SwcLibraries.Contains(path)) return SwcLibraries[path];
            else return SwcLibraries[GetRelativePath(path)];
        }

        public override void ChangeAssetPath(string fromPath, string toPath)
        {
            if (!FileInspector.IsSwc(fromPath)) base.ChangeAssetPath(fromPath, toPath);
            else
            {
                LibraryAsset asset = SwcLibraries[GetRelativePath(fromPath)];
                SwcLibraries.Remove(asset);
                asset.Path = GetRelativePath(toPath);
                SwcLibraries.Add(asset);
            }
        }

        public override void SetLibraryAsset(string path, bool isLibraryAsset)
        {
            if (!FileInspector.IsSwc(path) && !base.IsDirectory(path)) base.SetLibraryAsset(path, isLibraryAsset);
            else
            {
                string relPath = GetRelativePath(path);
                if (isLibraryAsset)
                {
                    LibraryAsset asset = new LibraryAsset(this, relPath);
                    asset.SwfMode = SwfAssetMode.Library;
                    SwcLibraries.Add(asset);
                }
                else
                {
                    SwcLibraries.Remove(path);
                    SwcLibraries.Remove(relPath);
                }
                RebuildCompilerOptions();
                OnClasspathChanged();
            }
        }
        #endregion

        #region Load/Save

        public override void PropertiesChanged()
        {
            // rebuild Swc assets list
            SwcLibraries.Clear();
            LibraryAsset asset;
            foreach (string path in CompilerOptions.LibraryPaths)
            {
                asset = new LibraryAsset(this, path);
                asset.SwfMode = SwfAssetMode.Library;
                SwcLibraries.Add(asset);
            }
            foreach (string path in CompilerOptions.IncludeLibraries)
            {
                asset = new LibraryAsset(this, path);
                asset.SwfMode = SwfAssetMode.IncludedLibrary;
                SwcLibraries.Add(asset);
            }
            foreach (string path in CompilerOptions.ExternalLibraryPaths)
            {
                asset = new LibraryAsset(this, path);
                asset.SwfMode = SwfAssetMode.ExternalLibrary;
                SwcLibraries.Add(asset);
            }
            base.PropertiesChanged();
        }

        public static AS3Project Load(string path)
        {
            ProjectReader reader;
            if (FileInspector.IsFlexBuilderProject(path)) reader = new FlexProjectReader(path);
            else reader = new AS3ProjectReader(path);

            try
            {
                return reader.ReadProject() as AS3Project;
            }
            catch (XmlException exception)
            {
                string format = string.Format("Error in XML Document line {0}, position {1}.",
                    exception.LineNumber, exception.LinePosition);
                throw new Exception(format, exception);
            }
            finally { reader.Close(); }
        }

        public override void Save()
        {
            if (ReadOnly) return;
            RebuildCompilerOptions();
            SaveAs(ProjectPath);
        }

        public override void SaveAs(string fileName)
        {
            if (!AllowedSaving(fileName)) return;
            try
            {
                AS3ProjectWriter writer = new AS3ProjectWriter(this, fileName);

                writer.WriteProject();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void RebuildCompilerOptions()
        {
            // rebuild Swc libraries lists
            CompilerOptions.LibraryPaths = GetLibraryPaths(SwfAssetMode.Library);
            CompilerOptions.IncludeLibraries = GetLibraryPaths(SwfAssetMode.IncludedLibrary);
            CompilerOptions.ExternalLibraryPaths = GetLibraryPaths(SwfAssetMode.ExternalLibrary);
        }

        private string[] GetLibraryPaths(SwfAssetMode mode)
        {
            List<string> paths = new List<string>();
            foreach (LibraryAsset asset in SwcLibraries)
                if (asset.SwfMode == mode)
                {
                    asset.Path = asset.Path.Replace("/", "\\");
                    paths.Add(asset.Path);
                }
            string[] newList = new string[paths.Count];
            paths.CopyTo(newList);
            return newList;
        }

        #endregion

    }

}

