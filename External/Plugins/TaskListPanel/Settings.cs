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
        ExploringMode exploringMode = ExploringMode.Light;
        int[] images = new[] { 229, 197, 197 };
        string[] extensions = new[] { ".txt" };
        string[] groups = new[] { "TODO", "FIXME", "BUG" };
        string[] excluded = Array.Empty<string>();

        /// <summary> 
        /// Exploring mode, the way we should operate
        /// </summary>
        [DisplayName("Exploring Mode")]
        [LocalizedDescription("TaskListPanel.Description.ExploringMode")]
        [DefaultValue(ExploringMode.Light)]
        public ExploringMode ExploringMode
        {
            get => exploringMode;
            set => exploringMode = value;
        }

        /// <summary> 
        /// Excluded directories, ie. external libraries
        /// </summary>
        [DisplayName("Excluded Paths")]
        [LocalizedDescription("TaskListPanel.Description.ExcludedPaths")]
        [DefaultValue(new string[] {})]
        public string[] ExcludedPaths
        {
            get => excluded;
            set => excluded = value;
        }

        /// <summary> 
        /// File extensions to listen for changes
        /// </summary>
        [DisplayName("File Extensions")]
        [LocalizedDescription("TaskListPanel.Description.FileExtensions")]
        [DefaultValue(new[] { ".txt" })]
        public string[] FileExtensions
        {
            get => extensions;
            set => extensions = value;
        }

        /// <summary> 
        /// Group values to look for.
        /// </summary>
        [DisplayName("Group Values")]
        [LocalizedDescription("TaskListPanel.Description.GroupValues")]
        [DefaultValue(new[] { "TODO", "FIXME", "BUG" })]
        public string[] GroupValues
        {
            get => groups;
            set => groups = value;
        }

        /// <summary> 
        /// Image indexes of the results.
        /// </summary>
        [DisplayName("Image Indexes")]
        [LocalizedDescription("TaskListPanel.Description.ImageIndexes")]
        [DefaultValue(new[] { 229, 197, 197 })]
        public int[] ImageIndexes
        {
            get => images;
            set => images = value;
        }

    }

}
