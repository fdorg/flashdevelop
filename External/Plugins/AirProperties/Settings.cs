using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace AirProperties
{
    [Serializable]
    public class Settings
    {
        private bool renameIcons = true;
        private bool useUniformFilenames = true;
        private bool selectPropertiesFileOnLoad = false;
        private string projectIconsFolder = @"bin\icons";
        private string packageIconsFolder = "icons";

        [DisplayName("Project Icons Folder")]
        [LocalizedDescription("AirProperties.Description.ProjectIconsFolder"), DefaultValue(@"bin\icons")]
        public string ProjectIconsFolder 
        {
            get => projectIconsFolder;
            set => projectIconsFolder = value;
        }

        [DisplayName("Package Icons Folder")]
        [LocalizedDescription("AirProperties.Description.PackageIconsFolder"), DefaultValue(@"icons")]
        public string PackageIconsFolder
        {
            get => packageIconsFolder;
            set => packageIconsFolder = value;
        }

        [DisplayName("Use Uniform File Names")]
        [LocalizedDescription("AirProperties.Description.UseUniformFileNames"), DefaultValue(true)]
        public bool UseUniformFilenames
        {
            get => useUniformFilenames;
            set => useUniformFilenames = value;
        }

        [DisplayName("Rename Icons With Size")]
        [LocalizedDescription("AirProperties.Description.RenameIconsWithSize"), DefaultValue(true)]
        public bool RenameIconsWithSize
        {
            get => renameIcons;
            set => renameIcons = value;
        }

        [DisplayName("Select Properties File On Open")]
        [LocalizedDescription("AirProperties.Description.SelectPropertiesFileOnOpen"), DefaultValue(false)]
        public bool SelectPropertiesFileOnOpen
        {
            get => selectPropertiesFileOnLoad;
            set => selectPropertiesFileOnLoad = value;
        }

    }

}
