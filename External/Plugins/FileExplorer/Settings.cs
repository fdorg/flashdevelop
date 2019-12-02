// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        private int sortOrder = 0;
        private int sortColumn = 0;
        private string filePath = "C:\\";
        private bool synchronizeToProject = true;

        /// <summary> 
        /// Get and sets the filePath.
        /// </summary>
        [DisplayName("Active Path")]
        [LocalizedDescription("FileExplorer.Description.FilePath"), DefaultValue("C:\\")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string FilePath 
        {
            get => this.filePath;
            set => this.filePath = value;
        }

        /// <summary> 
        /// Get and sets the synchronizeToProject.
        /// </summary>
        [DisplayName("Synchronize To Project")]
        [LocalizedDescription("FileExplorer.Description.SynchronizeToProject"), DefaultValue(true)]
        public bool SynchronizeToProject
        {
            get => this.synchronizeToProject;
            set => this.synchronizeToProject = value;
        }

        /// <summary> 
        /// Get and sets the sortColumn.
        /// </summary>
        [DisplayName("Active Column")]
        [LocalizedDescription("FileExplorer.Description.SortColumn"), DefaultValue(0)]
        public int SortColumn
        {
            get => this.sortColumn;
            set => this.sortColumn = value;
        }

        /// <summary> 
        /// Get and sets the sortOrder.
        /// </summary>
        [DisplayName("Sort Order")]
        [LocalizedDescription("FileExplorer.Description.SortOrder"), DefaultValue(0)]
        public int SortOrder
        {
            get => this.sortOrder;
            set => this.sortOrder = value;
        }

    }

}
