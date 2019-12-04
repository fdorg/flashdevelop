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
        int sortOrder = 0;
        int sortColumn = 0;
        string filePath = "C:\\";
        bool synchronizeToProject = true;

        /// <summary> 
        /// Get and sets the filePath.
        /// </summary>
        [DisplayName("Active Path")]
        [LocalizedDescription("FileExplorer.Description.FilePath"), DefaultValue("C:\\")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string FilePath 
        {
            get => filePath;
            set => filePath = value;
        }

        /// <summary> 
        /// Get and sets the synchronizeToProject.
        /// </summary>
        [DisplayName("Synchronize To Project")]
        [LocalizedDescription("FileExplorer.Description.SynchronizeToProject"), DefaultValue(true)]
        public bool SynchronizeToProject
        {
            get => synchronizeToProject;
            set => synchronizeToProject = value;
        }

        /// <summary> 
        /// Get and sets the sortColumn.
        /// </summary>
        [DisplayName("Active Column")]
        [LocalizedDescription("FileExplorer.Description.SortColumn"), DefaultValue(0)]
        public int SortColumn
        {
            get => sortColumn;
            set => sortColumn = value;
        }

        /// <summary> 
        /// Get and sets the sortOrder.
        /// </summary>
        [DisplayName("Sort Order")]
        [LocalizedDescription("FileExplorer.Description.SortOrder"), DefaultValue(0)]
        public int SortOrder
        {
            get => sortOrder;
            set => sortOrder = value;
        }
    }
}