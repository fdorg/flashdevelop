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
            get { return this.projectIconsFolder; }
            set { this.projectIconsFolder = value; }
        }

        [DisplayName("Package Icons Folder")]
        [LocalizedDescription("AirProperties.Description.PackageIconsFolder"), DefaultValue(@"icons")]
        public string PackageIconsFolder
        {
            get { return this.packageIconsFolder; }
            set { this.packageIconsFolder = value; }
        }

        [DisplayName("Use Uniform File Names")]
        [LocalizedDescription("AirProperties.Description.UseUniformFileNames"), DefaultValue(true)]
        public bool UseUniformFilenames
        {
            get { return this.useUniformFilenames; }
            set { this.useUniformFilenames = value; }
        }

        [DisplayName("Rename Icons With Size")]
        [LocalizedDescription("AirProperties.Description.RenameIconsWithSize"), DefaultValue(true)]
        public bool RenameIconsWithSize
        {
            get { return this.renameIcons; }
            set { this.renameIcons = value; }
        }

        [DisplayName("Select Properties File On Open")]
        [LocalizedDescription("AirProperties.Description.SelectPropertiesFileOnOpen"), DefaultValue(false)]
        public bool SelectPropertiesFileOnOpen
        {
            get { return this.selectPropertiesFileOnLoad; }
            set { this.selectPropertiesFileOnLoad = value; }
        }

    }

}
