using System;
using System.ComponentModel;
using PluginCore.Localization;
using ProjectManager.Projects;

namespace LoomContext.Projects
{
    [Serializable]
    public class LoomOptions : CompilerOptions
    {
        #region Compiler Options

        /*bool warnings = true;
        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Enable All Warnings")]
        [LocalizedDescription("ProjectManager.Description.Warnings")]
        [DefaultValue(true)]
        public bool Warnings { get { return warnings; } set { warnings = value; } }*/

        #endregion

        #region Advanced options

        string[] additional = new string[] { };
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [DisplayName("Additional Compiler Options")]
        [LocalizedDescription("ProjectManager.Description.Additional")]
        [DefaultValue(new string[] { })]
        public string[] Additional { get { return additional; } set { additional = value; } }

        /*string[] intrinsicPaths = new string[] { };
        [DisplayName("Intrinsic Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.IntrinsicPaths")]
        [DefaultValue(new string[] { })]
        public string[] IntrinsicPaths
        {
            get { return intrinsicPaths; }
            set { intrinsicPaths = value; }
        }*/
        
        string[] libraryPaths = new string[] { };
        /*[DisplayName("Loom Libraries")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.LibraryPaths")]
        [DefaultValue(new string[] { })]*/
        [Browsable(false)]
        public string[] LibraryPaths
        {
            get { return libraryPaths; }
            set { libraryPaths = value; }
        }

        #endregion
    }

}
