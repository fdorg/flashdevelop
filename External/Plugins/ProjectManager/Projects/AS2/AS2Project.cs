using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using PluginCore.Helpers;
using ProjectManager.Controls;
using ProjectManager.Controls.AS2;

namespace ProjectManager.Projects.AS2
{
    public class AS2Project : Project
    {
        public AS2Project(string path)
            : base(path, new MtascOptions()) 
        {
            movieOptions = new AS2MovieOptions();
        }

        public override string Language => "as2";
        public override string LanguageDisplayName => "AS2";
        public override bool IsCompilable => true;
        public override bool UsesInjection => InputPath != "";
        public override bool HasLibraries => OutputType == OutputType.Application && !UsesInjection;
        public override bool RequireLibrary => true;
        public override string DefaultSearchFilter => "*.as";

        public new MtascOptions CompilerOptions => (MtascOptions)base.CompilerOptions;

        public override PropertiesDialog CreatePropertiesDialog()
        {
            return new AS2PropertiesDialog();
        }

        public override void ValidateBuild(out string error)
        {
            if (CompilerOptions.UseMain && CompileTargets.Count == 0)
                error = "Description.MissingTarget";
            else
                error = null;
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            bool isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (export != null) return export;
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;

            if (FileInspector.IsActionScript(inFile, Path.GetExtension(inFile).ToLower()))
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(ProjectPath), path).Replace('\\', '/');
            return ProjectPaths.GetRelativePath(Path.GetDirectoryName(inFile), path).Replace('\\', '/');
        }

        public override CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            if (!isDirectory && Path.GetExtension(path) != ".as") return CompileTargetType.None;

            foreach(string cp in AbsoluteClasspaths)
                if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                    return CompileTargetType.AlwaysCompile;
            return CompileTargetType.None;
        }

        public override bool Clean()
        {
            try
            {
                if (!string.IsNullOrEmpty(OutputPath) && File.Exists(GetAbsolutePath(OutputPath)))
                    File.Delete(GetAbsolutePath(OutputPath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string GetOtherIDE(bool runOutput, bool releaseMode, out string error)
        {
            error = null;
            return "FlashIDE";
        }

        #region Load/Save

        public static AS2Project Load(string path)
        {
            AS2ProjectReader reader = new AS2ProjectReader(path);

            try
            {
                return reader.ReadProject();
            }
            catch (XmlException exception)
            {
                string format = string.Format("Error in XML Document line {0}, position {1}.",
                    exception.LineNumber,exception.LinePosition);
                throw new Exception(format,exception);
            }
            finally { reader.Close(); }
        }

        public override void Save()
        {
            SaveAs(ProjectPath);
        }

        public override void SaveAs(string fileName)
        {
            if (!AllowedSaving(fileName)) return;
            try
            {
                AS2ProjectWriter writer = new AS2ProjectWriter(this, fileName);
                writer.WriteProject();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
