using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ProjectManager.Controls;
using ProjectManager.Projects;

namespace LoomContext.Projects
{
    public class LoomProject : Project
    {
        public LoomProject(string path) : base(path, new LoomOptions())
        {
            movieOptions = new LoomMovieOptions();
            LoomLibraries = new AssetCollection(this);
        }
        
        public override string Name 
        { 
            get 
            {
                return Path.GetFileNameWithoutExtension(ProjectPath); 
            } 
        }

        public override string Language { get { return "loom"; } }
        public override string LanguageDisplayName { get { return "Loom"; } }
        public override bool IsCompilable { get { return true; } }
        public override bool ReadOnly { get { return false; } }
        public override bool HasLibraries { get { return OutputType == OutputType.Application; } }
        public override int MaxTargetsCount { get { return 1; } }
        public override string DefaultSearchFilter { get { return "*.ls"; } }

        public new LoomOptions CompilerOptions { get { return (LoomOptions)base.CompilerOptions; } }

        public override PropertiesDialog CreatePropertiesDialog()
        {
            return new LoomPropertiesDialog();
        }

        public override void ValidateBuild(out string error)
        {
            if (CompileTargets.Count == 0) error = "Description.MissingEntryPoint";
            else error = null;
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            return ProjectPaths.GetRelativePath(this.Directory, path).Replace("\\", "/");
        }

        private bool IsText(string ext)
        {
            return ext == ".txt" || ext == ".xml";
        }

        public override CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            if (isDirectory || Path.GetExtension(path) != ".ls") 
                return CompileTargetType.None;

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
                //if (OutputPath != null && OutputPath.Length > 0 && File.Exists(GetAbsolutePath(OutputPath)))
                //    File.Delete(GetAbsolutePath(OutputPath));
                return true;
            }
            catch {
                return false;
            }
        }

        #region Assets management

        public AssetCollection LoomLibraries;

        public bool IsLoomLib(string path)
        {
            return Path.GetExtension(path).ToLower() == ".loomlib";
        }

        public override bool IsLibraryAsset(string path)
        {
            if (!IsLoomLib(path)) return false;
            else return LoomLibraries.Contains(path) || LoomLibraries.Contains(GetRelativePath(path));
        }

        public override LibraryAsset GetAsset(string path)
        {
            if (!IsLoomLib(path)) return null;
            else return LoomLibraries[GetRelativePath(path)];
        }

        public override void ChangeAssetPath(string fromPath, string toPath)
        {
            if (!IsLoomLib(fromPath)) return;
            else
            {
                LibraryAsset asset = LoomLibraries[GetRelativePath(fromPath)];
                LoomLibraries.Remove(asset);
                asset.Path = GetRelativePath(toPath);
                LoomLibraries.Add(asset);
            }
        }

        public override void SetLibraryAsset(string path, bool isLibraryAsset)
        {
            if (!IsLoomLib(path)) return;
            else
            {
                string relPath = GetRelativePath(path);
                if (isLibraryAsset)
                {
                    LibraryAsset asset = new LibraryAsset(this, relPath);
                    asset.SwfMode = SwfAssetMode.Library;
                    LoomLibraries.Add(asset);
                }
                else
                {
                    LoomLibraries.Remove(path);
                    LoomLibraries.Remove(relPath);
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
            LoomLibraries.Clear();
            LibraryAsset asset;
            foreach (string path in CompilerOptions.LibraryPaths)
            {
                asset = new LibraryAsset(this, path);
                asset.SwfMode = SwfAssetMode.Library;
                LoomLibraries.Add(asset);
            }
            base.PropertiesChanged();
        }

        public static LoomProject Load(string path)
        {
            ProjectReader reader = new LoomProjectReader(path);

            try
            {
                return reader.ReadProject() as LoomProject;
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
                LoomProjectWriter writer = new LoomProjectWriter(this, fileName);

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
        }

        private string[] GetLibraryPaths(SwfAssetMode mode)
        {
            List<string> paths = new List<string>();
            foreach (LibraryAsset asset in LoomLibraries)
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
