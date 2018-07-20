using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace TaskListPanel
{
    [Serializable]
    public enum ExploringMode
    {
        Light,
        Complete
    }

    [Serializable]
    public class Settings
    {
        private ExploringMode exploringMode = ExploringMode.Light;
        private Int32[] images = new Int32[] { 229, 197, 197 };
        private String[] extensions = new String[] { ".txt" };
        private String[] groups = new String[] { "TODO", "FIXME", "BUG" };
        private String[] excluded = new String[0] {};

        /// <summary> 
        /// Exploring mode, the way we should operate
        /// </summary>
        [DisplayName("Exploring Mode")]
        [LocalizedDescription("TaskListPanel.Description.ExploringMode")]
        [DefaultValue(ExploringMode.Light)]
        public ExploringMode ExploringMode
        {
            get { return this.exploringMode; }
            set { this.exploringMode = value; }
        }

        /// <summary> 
        /// Excluded directories, ie. external libraries
        /// </summary>
        [DisplayName("Excluded Paths")]
        [LocalizedDescription("TaskListPanel.Description.ExcludedPaths")]
        [DefaultValue(new String[0] {})]
        public String[] ExcludedPaths
        {
            get { return this.excluded; }
            set { this.excluded = value; }
        }

        /// <summary> 
        /// File extensions to listen for changes
        /// </summary>
        [DisplayName("File Extensions")]
        [LocalizedDescription("TaskListPanel.Description.FileExtensions")]
        [DefaultValue(new String[] { ".txt" })]
        public String[] FileExtensions
        {
            get { return this.extensions; }
            set { this.extensions = value; }
        }

        /// <summary> 
        /// Group values to look for.
        /// </summary>
        [DisplayName("Group Values")]
        [LocalizedDescription("TaskListPanel.Description.GroupValues")]
        [DefaultValue(new String[] { "TODO", "FIXME", "BUG" })]
        public String[] GroupValues
        {
            get { return this.groups; }
            set { this.groups = value; }
        }

        /// <summary> 
        /// Image indexes of the results.
        /// </summary>
        [DisplayName("Image Indexes")]
        [LocalizedDescription("TaskListPanel.Description.ImageIndexes")]
        [DefaultValue(new Int32[] { 229, 197, 197 })]
        public Int32[] ImageIndexes
        {
            get { return this.images; }
            set { this.images = value; }
        }

    }

}
