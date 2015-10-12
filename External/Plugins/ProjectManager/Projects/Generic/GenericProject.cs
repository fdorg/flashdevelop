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

        public override string Language { get { return "*"; } }
        public override string LanguageDisplayName { get { return "*"; } }
        public override bool IsCompilable { get { return false; } }

        public override string DefaultSearchFilter
        {
            get
            {
                if (OutputType == OutputType.Website) return "*.html;*.css;*.js";
                else return "*.*";
            }
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            String inPath = Path.GetDirectoryName(inFile);
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
                string format = string.Format("Error in XML Document line {0}, position {1}.", exception.LineNumber, exception.LinePosition);
                throw new Exception(format, exception);
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

