using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace AirProperties
{
    [Serializable]
    public class Settings
    {
        private Boolean renameIcons = true;
        private Boolean useUniformFilenames = true;
        private Boolean selectPropertiesFileOnLoad = false;
        private String projectIconsFolder = @"bin\icons";
        private String packageIconsFolder = "icons";

        [DisplayName("Project Icons Folder")]
        [LocalizedDescription("AirProperties.Description.ProjectIconsFolder"), DefaultValue(@"bin\icons")]
        public String ProjectIconsFolder 
        {
            get { return this.projectIconsFolder; }
            set { this.projectIconsFolder = value; }
        }

        [DisplayName("Package Icons Folder")]
        [LocalizedDescription("AirProperties.Description.PackageIconsFolder"), DefaultValue(@"icons")]
        public String PackageIconsFolder
        {
            get { return this.packageIconsFolder; }
            set { this.packageIconsFolder = value; }
        }

        [DisplayName("Use Uniform File Names")]
        [LocalizedDescription("AirProperties.Description.UseUniformFileNames"), DefaultValue(true)]
        public Boolean UseUniformFilenames
        {
            get { return this.useUniformFilenames; }
            set { this.useUniformFilenames = value; }
        }

        [DisplayName("Rename Icons With Size")]
        [LocalizedDescription("AirProperties.Description.RenameIconsWithSize"), DefaultValue(true)]
        public Boolean RenameIconsWithSize
        {
            get { return this.renameIcons; }
            set { this.renameIcons = value; }
        }

        [DisplayName("Select Properties File On Open")]
        [LocalizedDescription("AirProperties.Description.SelectPropertiesFileOnOpen"), DefaultValue(false)]
        public Boolean SelectPropertiesFileOnOpen
        {
            get { return this.selectPropertiesFileOnLoad; }
            set { this.selectPropertiesFileOnLoad = value; }
        }

    }

}
