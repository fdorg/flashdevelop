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

        private Boolean searchExternalClassPath = true;
        List<ProjectPreferences> projectPrefList = new List<ProjectPreferences>();
        List<string> recentProjects = new List<string>();
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
        string[] excludedFileTypes = new string[] { ".p", ".abc", ".bak", ".tmp" };
        string[] excludedDirectories = new string[] { ".svn", "_svn", ".cvs", "_cvs", "cvs", "_sgbak", ".git", ".hg", "node_modules" };
        string[] executableFileTypes = new string[] { ".exe", ".lnk", ".fla", ".flump", ".doc", ".pps", ".psd", ".png", ".jpg", ".gif", ".xls", ".docproj", ".ttf", ".otf", ".wav", ".mp3", ".ppt", ".pptx", ".docx", ".xlsx", ".ai", ".pdf", ".zip", ".rar" };
        string[] filteredDirectoryNames = new string[] { "src", "source", "sources", "as", "as2", "as3", "actionscript", "flash", "classes", "trunk", "svn", "git", "hg", "github", "gitlab", "haxelib", "library", "..", "." };

        HighlightType tabHighlightType = HighlightType.ExternalFiles;

        #region Properties
        [Browsable(false)]
        public List<ProjectPreferences> ProjectPrefs
        {
            get { return projectPrefList; }
            set { projectPrefList = value; }
        }

        [Browsable(false)]
        public string LastProject
        {
            get { return lastProject; }
            set { lastProject = value; FireChanged("LastProject"); }
        }

        [Browsable(false)]
        public List<string> RecentProjects
        {
            get { return recentProjects; }
        }

        [Browsable(false)]
        public bool CreateProjectDirectory
        {
            get { return createProjectDirectory; }
            set { createProjectDirectory = value; FireChanged("CreateProjectDirectory"); }
        }

        [Browsable(false)]
        public String NewProjectDefaultDirectory
        {
            get { return newProjectDefaultDirectory; }
            set { newProjectDefaultDirectory = value; FireChanged("NewProjectDefaultDirectory"); }
        }

        [DisplayName("Search In External Classpath")]
        [LocalizedDescription("ProjectManager.Description.SearchExternalClassPath")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(true)]
        public Boolean SearchExternalClassPath
        {
            get { return searchExternalClassPath; }
            set { searchExternalClassPath = value; }
        }

        [DisplayName("Use Project Sessions")]
        [LocalizedDescription("ProjectManager.Description.UseProjectSessions")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(false)]
        public Boolean UseProjectSessions
        {
            get { return useProjectSessions; }
            set { useProjectSessions = value; }
        }

        [DisplayName("Maximum Recent Projects")]
        [LocalizedDescription("ProjectManager.Description.MaxRecentProjects")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(10)]
        public Int32 MaxRecentProjects
        {
            get { return maxRecentProjects; }
            set { maxRecentProjects = value; FireChanged("MaxRecentProjects"); }
        }

        [DisplayName("Disable Extended Flash IDE Integration")]
        [LocalizedDescription("ProjectManager.Description.DisableExtFlashIntegration")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(false)]
        public bool DisableExtFlashIntegration
        {
            get { return disableExtFlashIntegration; }
            set { disableExtFlashIntegration = value; }
        }

        [DisplayName("Webserver Port")]
        [LocalizedDescription("ProjectManager.Description.WebserverPort")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(2000)]
        public Int32 WebserverPort
        {
            get { return webserverPort; }
            set { webserverPort = value; }
        }
        
        [DisplayName("Excluded File Types")]
        [LocalizedDescription("ProjectManager.Description.ExcludedFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedFileTypes
        {
            get { return excludedFileTypes; }
            set { excludedFileTypes = value; FireChanged("ExcludedFileTypes"); }
        }

        [DisplayName("Executable File Types")]
        [LocalizedDescription("ProjectManager.Description.ExecutableFileTypes")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExecutableFileTypes
        {
            get { return executableFileTypes; }
            set { executableFileTypes = value; FireChanged("ExecutableFileTypes"); }
        }

        [DisplayName("Excluded Directories")]
        [LocalizedDescription("ProjectManager.Description.ExcludedDirectories")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] ExcludedDirectories
        {
            get { return excludedDirectories; }
            set { excludedDirectories = value; FireChanged("ExcludedDirectories"); }
        }

        [DisplayName("Filtered Directory Names")]
        [LocalizedDescription("ProjectManager.Description.FilteredDirectoryNames")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] FilteredDirectoryNames
        {
            get { return filteredDirectoryNames; }
            set { filteredDirectoryNames = value; FireChanged("FilteredDirectoryNames"); }
        }

        bool showExternalLibraries = false;

        [DisplayName("Show External Libraries")]
        [LocalizedDescription("ProjectManager.Description.ShowExternalLibraries")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool ShowExternalLibraries
        {
            get { return showExternalLibraries; }
            set { showExternalLibraries = value; FireChanged("ShowExternalLibraries"); }
        }

        [DisplayName("Show Project Classpaths")]
        [LocalizedDescription("ProjectManager.Description.ShowProjectClasspaths")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(true)]
        public bool ShowProjectClasspaths
        {
            get { return showProjectClasspaths; }
            set { showProjectClasspaths = value; FireChanged("ShowProjectClasspaths"); }
        }

        [DisplayName("Show Global Classpaths")]
        [LocalizedDescription("ProjectManager.Description.ShowGlobalClasspaths")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool ShowGlobalClasspaths
        {
            get { return showGlobalClasspaths; }
            set { showGlobalClasspaths = value; FireChanged("DisableMxmlMapping"); }
        }

        [DisplayName("Enable Mxml Sources Mapping")]
        [LocalizedDescription("ProjectManager.Description.EnableMxmlMapping")]
        [LocalizedCategory("ProjectManager.Category.ProjectTree")]
        [DefaultValue(false)]
        public bool EnableMxmlMapping
        {
            get { return enableMxmlMapping; }
            set { enableMxmlMapping = value; FireChanged("ShowGlobalClasspaths"); }
        }

        [DisplayName("Tab Highlight Type")]
        [LocalizedDescription("ProjectManager.Description.TabHighlightType")]
        [LocalizedCategory("ProjectManager.Category.OtherOptions")]
        [DefaultValue(HighlightType.ExternalFiles)]
        public HighlightType TabHighlightType
        {
            get { return tabHighlightType; }
            set { tabHighlightType = value; }
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
            get { return this.GetGlobalClasspaths(Language); }
            set { this.SetGlobalClasspaths(Language, value); }
        }

        public void SetGlobalClasspaths(String lang, List<String> classpaths)
        {
            Hashtable info = new Hashtable();
            info["language"] = lang;
            info["cp"] = classpaths;
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.SetUserClasspath", info);
            EventManager.DispatchEvent(this, de);
            FireChanged("GlobalClasspath");
        }

        public List<string> GetGlobalClasspaths(String lang)
        {
            List<String> cp = null;
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

        private void FireChanged(string setting)
        {
            if (Changed != null)
                Changed(setting);
        }

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
        public Boolean DebugMode;
        public List<String> ExpandedPaths;
        public String ProjectPath;
        public String TargetBuild;

        public ProjectPreferences()
        {
            this.ExpandedPaths = new List<String>();
        }
        public ProjectPreferences(String projectPath) : this()
        {
            this.ProjectPath = projectPath;
        }
    }
}
