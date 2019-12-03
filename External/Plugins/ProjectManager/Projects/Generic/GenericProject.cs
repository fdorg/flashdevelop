using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ProjectManager.Projects.Generic
{
    public class GenericProject : Project
    {
        public GenericProject(string path) : base(path, new GenericOptions())
        {
            movieOptions = new GenericMovieOptions();
        }

        public override string Language => "*";
        public override string LanguageDisplayName => "*";
        public override bool IsCompilable => false;

        public override string DefaultSearchFilter
        {
            get
            {
                if (OutputType == OutputType.Website) return "*.html;*.css;*.js";
                return "*.*";
            }
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            string inPath = Path.GetDirectoryName(inFile);
            return ProjectPaths.GetRelativePath(inPath, path);
        }

        #region Load/Save

        public static GenericProject Load(string path)
        {
            GenericProjectReader reader = new GenericProjectReader(path);
            try
            {
                return reader.ReadProject();
            }
            catch (XmlException exception)
            {
                string format =
                    $"Error in XML Document line {exception.LineNumber}, position {exception.LinePosition}.";
                throw new Exception(format, exception);
            }
            finally { reader.Close(); }
        }

        public override void Save() => SaveAs(ProjectPath);

        public override void SaveAs(string fileName)
        {
            if (!AllowedSaving(fileName)) return;
            try
            {
                GenericProjectWriter writer = new GenericProjectWriter(this, fileName);
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