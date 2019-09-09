using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;

namespace UnityContext
{
    public class UnityProject : Project
    {
        public UnityProject(string path)
            : base(path, new UnityCompilerOptions()) 
        {
            movieOptions = new UnityMovieOptions();
        }

        public override string Language { get { return "unityscript"; } }
        public override string DefaultSearchFilter { get { return "*.js"; } }


        #region Load/Save

        public static UnityProject Load(string path)
        {
            UnityProjectReader reader = new UnityProjectReader(path);

            try
            {
                return reader.ReadProject();
            }
            catch (System.Xml.XmlException exception)
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
                UnityProjectWriter writer = new UnityProjectWriter(this, fileName);
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
