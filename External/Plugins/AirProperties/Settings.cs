// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace AirProperties
{
    [Serializable]
    public class Settings
    {
        bool renameIcons = true;
        bool useUniformFilenames = true;
        bool selectPropertiesFileOnLoad = false;
        string projectIconsFolder = @"bin\icons";
        string packageIconsFolder = "icons";

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