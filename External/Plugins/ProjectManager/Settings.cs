// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Controls;
using ProjectManager.Projects;

namespace ProjectManager
{
    public delegate void SettingChangeHandler(string setting);

    [Serializable]
    public class ProjectManagerSettings
    {
        public event SettingChangeHandler Changed;

        private bool searchExternalClassPath = true;
        List<ProjectPreferences> projectPrefList = new List<ProjectPreferences>();
        readonly List<string> recentProjects = new List<string>();
        bool showProjectClasspaths = true;
        bool showGlobalClasspaths = false;
        int maxRecentProjects = 10;
        string lastProject = string.Empty;
        bool createProjectDirectory = true;
        bool useProjectSessions = false;
        bool disableExtFlashIntegration = false;
        string newProjectDefaultDirectory = string.Empty;
        bool enableMxmlMapping = false;
        int webserverPort = 2000;

        // These are string arrays because they are only edited by the propertygrid (which deals with them nicely)
        string[] excludedFileTypes = {".p", ".abc", ".bak", ".tmp"};
        string[] excludedDirectories = {".svn", "_svn", ".cvs", "_cvs", "cvs", "_sgbak", ".git", ".hg", "node_modules"};

        string[] executableFileTypes =
        {
            ".exe", ".lnk", ".fla", ".flump", ".doc", ".pps", ".psd", ".png", ".jpg", ".gif", ".xls", ".docproj",
            ".ttf", ".otf", ".wav", ".mp3", ".ppt", ".pptx", ".docx", ".xlsx", ".ai", ".pdf", ".zip", ".rar"
        };

        string[] filteredDirectoryNames =
        {
            "src", "source", "sources", "as", "as2", "as3", "actionscript", "flash", "classes", "trunk", "svn", "git",
            "hg", "github", "gitlab", "haxelib", "library", "..", "."
        };

        HighlightType tabHighlightType = HighlightType.ExternalFiles;

        #region Properties
        [Browsable(false)]
        public List<ProjectPreferences> ProjectPrefs
        {
            get => projectPrefList;
            set => projectPrefList = value;
        }

        [Browsable(false)]
        public string LastProject
        {
            get => lastProject;
            set { lastProject = value; FireChanged("LastProject"); }
        }

        [Browsable(false)]
        public List<string> RecentProjects => recentProjects;

        [Browsable(false)]
        public bool CreateProjectDirectory
        {
            get => createProjectDirectory;
            set { createProjectDirectory = value; FireChanged("CreateProjectDirectory"); }
        }

        [Browsable(false)]
        public string NewProjectDefaultDirectory
        {
            get => newProjectDefaultDirectory;
            set { newProjectDefaultDirectory = value; FireChanged("NewProjectDefaultDirectory"); }
        }

        [DisplayName("Search In External Classpath")]
        [LocalizedDescription("ProjectManager.Description.SearchExternalClassPath")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(true)]
        public bool SearchExternalClassPath
        {
            get => searchExternalClassPath;
            set => searchExternalClassPath = value;
        }

        [DisplayName("Use Project Sessions")]
        [LocalizedDescription("ProjectManager.Description.UseProjectSessions")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(false)]
        public bool UseProjectSessions
        {
            get => useProjectSessions;
            set => useProjectSessions = value;
        }

        [DisplayName("Maximum Recent Projects")]
        [LocalizedDescription("ProjectManager.Description.MaxRecentProjects")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(10)]
        public int MaxRecentProjects
        {
            get => maxRecentProjects;
            set { maxRecentProjects = value; FireChanged("MaxRecentProjects"); }
        }

        [DisplayName("Disable Extended Flash IDE Integration")]
        [LocalizedDescription("ProjectManager.Description.DisableExtFlashIntegration")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(false)]
        public bool DisableExtFlashIntegration
        {
            get => disableExtFlashIntegration;
            set => disableExtFlashIntegration = value;
        }

        [DisplayName("Webserver Port")]
        [LocalizedDescription("ProjectManager.Description.WebserverPort")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(2000)]
        public int WebserverPort
        {
            get => webserverPort;
            set => webserverPort = value;
        }
        
        [DisplayName("Excluded File Types")]
        [LocalizedDescription("ProjectManager.Description.ExcludedFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedFileTypes
        {
            get => excludedFileTypes;
            set { excludedFileTypes = value; FireChanged("ExcludedFileTypes"); }
        }

        [DisplayName("Executable File Types")]
        [LocalizedDescription("ProjectManager.Description.ExecutableFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExecutableFileTypes
        {
            get => executableFileTypes;
            set { executableFileTypes = value; FireChanged("ExecutableFileTypes"); }
        }

        [DisplayName("Excluded Directories")]
        [LocalizedDescription("ProjectManager.Description.ExcludedDirectories")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedDirectories
        {
            get => excludedDirectories;
            set { excludedDirectories = value; FireChanged("ExcludedDirectories"); }
        }

        [DisplayName("Filtered Directory Names")]
        [LocalizedDescription("ProjectManager.Description.FilteredDirectoryNames")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] FilteredDirectoryNames
        {
            get => filteredDirectoryNames;
            set { filteredDirectoryNames = value; FireChanged("FilteredDirectoryNames"); }
        }

        bool showExternalLibraries = false;

        [DisplayName("Show External Libraries")]
        [LocalizedDescription("ProjectManager.Description.ShowExternalLibraries")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool ShowExternalLibraries
        {
            get => showExternalLibraries;
            set { showExternalLibraries = value; FireChanged("ShowExternalLibraries"); }
        }

        [DisplayName("Show Project Classpaths")]
        [LocalizedDescription("ProjectManager.Description.ShowProjectClasspaths")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(true)]
        public bool ShowProjectClasspaths
        {
            get => showProjectClasspaths;
            set { showProjectClasspaths = value; FireChanged("ShowProjectClasspaths"); }
        }

        [DisplayName("Show Global Classpaths")]
        [LocalizedDescription("ProjectManager.Description.ShowGlobalClasspaths")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool ShowGlobalClasspaths
        {
            get => showGlobalClasspaths;
            set { showGlobalClasspaths = value; FireChanged("DisableMxmlMapping"); }
        }

        [DisplayName("Enable Mxml Sources Mapping")]
        [LocalizedDescription("ProjectManager.Description.EnableMxmlMapping")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool EnableMxmlMapping
        {
            get => enableMxmlMapping;
            set { enableMxmlMapping = value; FireChanged("ShowGlobalClasspaths"); }
        }

        [DisplayName("Tab Highlight Type")]
        [LocalizedDescription("ProjectManager.Description.TabHighlightType")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(HighlightType.ExternalFiles)]
        public HighlightType TabHighlightType
        {
            get => tabHighlightType;
            set => tabHighlightType = value;
        }

        [DisplayName("Track Active Document")]
        [LocalizedDescription("ProjectManager.Description.TrackActiveDocument")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool TrackActiveDocument { get; set; }

        #endregion

        [Browsable(false)]
        public string Language;

        [Browsable(false)]
        public List<string> GlobalClasspaths
        {
            get => GetGlobalClasspaths(Language);
            set => SetGlobalClasspaths(Language, value);
        }

        public void SetGlobalClasspaths(string lang, List<string> classpaths)
        {
            Hashtable info = new Hashtable();
            info["language"] = lang;
            info["cp"] = classpaths;
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.SetUserClasspath", info);
            EventManager.DispatchEvent(this, de);
            FireChanged("GlobalClasspath");
        }

        public List<string> GetGlobalClasspaths(string lang)
        {
            List<string> cp = null;
            Hashtable info = new Hashtable();
            info["language"] = lang;
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.GetUserClasspath", info);
            EventManager.DispatchEvent(this, de);
            if (de.Handled && info.ContainsKey("cp")) cp = info["cp"] as List<string>;
            return cp ?? new List<string>();
        }

        /// <summary>
        /// Returns the preferences object for the given project
        /// and creates a new instance if necessary.
        /// </summary>
        public ProjectPreferences GetPrefs(Project project)
        {
            foreach (ProjectPreferences prefs in projectPrefList)
                if (prefs.ProjectPath == project.ProjectPath)
                    return prefs;

            // ok, we haven't seen this project before.  let's take this opportunity
            // to clean out any prefs for projects that don't exist anymore
            CleanOldPrefs();

            ProjectPreferences newPrefs = new ProjectPreferences(project.ProjectPath);
            newPrefs.DebugMode = project.EnableInteractiveDebugger
                && project.OutputType != OutputType.OtherIDE && project.OutputPath != "";
            projectPrefList.Add(newPrefs);
            return newPrefs;
        }

        private void CleanOldPrefs()
        {
            for (int i = 0; i < projectPrefList.Count; i++)
                if (!File.Exists(projectPrefList[i].ProjectPath))
                    projectPrefList.RemoveAt(i--); // search this index again
        }

        private void FireChanged(string setting) => Changed?.Invoke(setting);

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string[] extraFilteredDirectoryNames = { "github", "gitlab", "haxelib", "library" };
            var filteredDirectoryNames = new List<string>(this.filteredDirectoryNames);

            foreach (var item in extraFilteredDirectoryNames)
                if (!filteredDirectoryNames.Contains(item)) filteredDirectoryNames.Add(item);

            this.filteredDirectoryNames = filteredDirectoryNames.ToArray();
        }

    }

    [Serializable]
    public class ProjectPreferences
    {
        public bool DebugMode;
        public List<string> ExpandedPaths;
        public string ProjectPath;
        public string TargetBuild;

        public ProjectPreferences()
        {
            ExpandedPaths = new List<string>();
        }
        public ProjectPreferences(string projectPath) : this()
        {
            ProjectPath = projectPath;
        }
    }
}
