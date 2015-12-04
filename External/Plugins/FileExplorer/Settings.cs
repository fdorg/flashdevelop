using System;
using System.ComponentModel;
using System.Drawing.Design;
using Ookii.Dialogs;
using PluginCore.Localization;

namespace FileExplorer
{
    [Serializable]
    public class Settings
    {
        private Int32 sortOrder = 0;
        private Int32 sortColumn = 0;
        private String filePath = "C:\\";
        private Boolean synchronizeToProject = true;

        /// <summary> 
        /// Get and sets the filePath.
        /// </summary>
        [DisplayName("Active Path")]
        [LocalizedDescription("FileExplorer.Description.FilePath"), DefaultValue("C:\\")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public String FilePath 
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }

        /// <summary> 
        /// Get and sets the synchronizeToProject.
        /// </summary>
        [DisplayName("Synchronize To Project")]
        [LocalizedDescription("FileExplorer.Description.SynchronizeToProject"), DefaultValue(true)]
        public Boolean SynchronizeToProject
        {
            get { return this.synchronizeToProject; }
            set { this.synchronizeToProject = value; }
        }

        /// <summary> 
        /// Get and sets the sortColumn.
        /// </summary>
        [DisplayName("Active Column")]
        [LocalizedDescription("FileExplorer.Description.SortColumn"), DefaultValue(0)]
        public Int32 SortColumn
        {
            get { return this.sortColumn; }
            set { this.sortColumn = value; }
        }

        /// <summary> 
        /// Get and sets the sortOrder.
        /// </summary>
        [DisplayName("Sort Order")]
        [LocalizedDescription("FileExplorer.Description.SortOrder"), DefaultValue(0)]
        public Int32 SortOrder
        {
            get { return this.sortOrder; }
            set { this.sortOrder = value; }
        }

    }

}
