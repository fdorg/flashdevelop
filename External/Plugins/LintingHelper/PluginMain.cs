using CodeRefactor.Managers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace LintingHelper
{
    public class PluginMain : IPlugin
    {
        private string pluginName = "LintingHelper";
        private string pluginGuid = "279C4926-5AC6-49E1-A0AC-66B7275C13DB";
        private string pluginHelp = "www.flashdevelop.org/community/";
        private string pluginDesc = "Plugin that adds a generic interface for linting / code analysis.";
        private string pluginAuth = "Christoph Otter";
        private Settings settingObject;
        private string settingFilename;

        internal static RichToolTip Tip;
        private List<string> fileCache = new List<string>();
        private IProject lastProject;

        public int Api
        {
            get
            {
                return 1;
            }
        }

        public string Author
        {
            get
            {
                return pluginAuth;
            }
        }

        public string Description
        {
            get
            {
                return pluginDesc;
            }
        }

        public string Guid
        {
            get
            {
                return pluginGuid;
            }
        }

        public string Help
        {
            get
            {
                return pluginHelp;
            }
        }

        public string Name
        {
            get
            {
                return pluginName;
            }
        }

        [Browsable(false)]
        public object Settings
        {
            get
            {
                return settingObject;
            }
        }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
        }

        public void Dispose()
        {
            SaveSettings();
        }

        private void AddEventHandlers()
        {
            BatchProcessManager.AddBatchProcessor(new BatchProcess.LintProcessor());
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileSave | EventType.Command);

            UITools.Manager.OnMouseHover += Scintilla_OnMouseHover;
            UITools.Manager.OnMouseHoverEnd += Scintilla_OnMouseHoverEnd;
        }

        private void Scintilla_OnMouseHover(ScintillaNet.ScintillaControl sender, int position)
        {
            if (!UITools.Tip.Visible) //do not show when documentation tip is shown already
            {
                var results = Managers.LintingManager.Cache.GetResultsFromPosition(DocumentManager.FindDocument(sender), position);
                if (results == null)
                {
                    return;
                }

                var desc = "";

                foreach (var result in results)
                {
                    desc += "\r\n" + result.Description;
                }

                if (desc != string.Empty)
                {
                    desc = desc.Remove(0, 2); //remove \r\n
                    Tip.ShowAtMouseLocation(desc);
                }
            }
        }

        private void Scintilla_OnMouseHoverEnd(ScintillaNet.ScintillaControl sender, int position)
        {
            Tip.Hide();
        }

        private void InitBasics()
        {
            Tip = new RichToolTip(PluginBase.MainForm);

            string dataPath = Path.Combine(PathHelper.DataDir, "LintingHelper");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        private void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch(e.Type)
            {
                case EventType.FileOpen:
                    TextEvent fileOpen = (TextEvent)e;
                    if (this.settingObject.LintOnOpen)
                    {
                        Managers.LintingManager.LintFiles(new string[] { fileOpen.Value });
                    }
                    break;
                case EventType.FileSave:
                    TextEvent fileSave = (TextEvent)e;
                    if (this.settingObject.LintOnSave)
                    {
                        Managers.LintingManager.LintFiles(new string[] { fileSave.Value });
                    }
                    break;
                case EventType.Command:
                    DataEvent de = e as DataEvent;
                    if (de == null) return;

                    var project = de.Data as IProject;

                    if (this.settingObject.LintOnProjectLoad && project != lastProject && de.Action == "ProjectManager.Project")
                    {
                        Managers.LintingManager.LintFiles(GetProjectFiles(project).ToArray());
                        lastProject = project;
                    }
                    break;
            }
        }

        private List<string> GetProjectFiles(IProject project)
        {
            List<String> files = new List<String>();
            String[] filters = project.DefaultSearchFilter.Split(';');
            foreach (String path in project.SourcePaths)
            {
                foreach (String filter in filters)
                {
                    files.AddRange(Directory.GetFiles(project.GetAbsolutePath(path), filter, SearchOption.AllDirectories));
                }
            }

            return files;
        }
    }
}
