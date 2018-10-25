using System.Collections.Generic;
using CodeRefactor.Managers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System.ComponentModel;
using System.IO;
using LintingHelper.Helpers;
using LintingHelper.Managers;
using PluginCore.Localization;
using ProjectManager;

namespace LintingHelper
{
    public class PluginMain : IPlugin
    {
        private const string pluginName = "LintingHelper";
        private const string pluginGuid = "279C4926-5AC6-49E1-A0AC-66B7275C13DB";
        private const string pluginHelp = "www.flashdevelop.org/community/";
        private const string pluginDesc = "Plugin that adds a generic interface for linting / code analysis.";
        private const string pluginAuth = "FlashDevelop Team";
        private Settings settingObject;
        private string settingFilename;

        public int Api
        {
            get
            {
                return 1;
            }
        }

        public string Author => pluginAuth;

        public string Description => pluginDesc;

        public string Guid => pluginGuid;

        public string Help => pluginHelp;

        public string Name => pluginName;

        [Browsable(false)]
        public object Settings => settingObject;

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
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileSave | EventType.FileModify | EventType.Command);
        }

        private void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, nameof(LintingHelper));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, $"{nameof(Settings)}.fdb");
            TraceManager.RegisterTraceGroup(LintingManager.TraceGroup, TextHelper.GetStringWithoutMnemonics("Label.LintingResults"));
        }

        private void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
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
                    if (MessageBar.Locked) return;
                    var fileOpen = (TextEvent) e;
                    if (this.settingObject.LintOnOpen)
                    {
                        LintingManager.Cache.RemoveDocument(fileOpen.Value);
                        LintingManager.LintFiles(new[] { fileOpen.Value });
                    }
                    break;
                case EventType.FileSave:
                    if (MessageBar.Locked) return;
                    var reason = (e as TextDataEvent)?.Data as string;
                    if (reason != "HaxeComplete" && this.settingObject.LintOnSave)
                    {
                        var fileSave = (TextEvent) e;
                        LintingManager.Cache.RemoveDocument(fileSave.Value);
                        LintingManager.LintFiles(new[] { fileSave.Value });
                    }
                    break;
                case EventType.Command:
                    var ev = (DataEvent) e;
                    if (ev.Action == ProjectManagerEvents.BuildComplete || ev.Action == ProjectManagerEvents.ProjectSetUp)
                    {
                        LintingManager.Cache.RemoveAll();
                        LintingManager.UpdateLinterPanel();
                        if (ev.Action == ProjectManagerEvents.ProjectSetUp)
                        {
                            var groupedFiles = new Dictionary<string, List<string>>();
                            foreach (var doc in PluginBase.MainForm.Documents)
                               /* if (!doc.IsUntitled && doc.SciControl != null)
                                    LintingManager.LintDocument(doc);*/
                            {
                                ScintillaNet.ScintillaControl sci;
                                if (doc.IsUntitled || (sci = doc.SciControl) == null) continue;

                                var files = groupedFiles.GetOrCreate(sci.ConfigurationLanguage);
                                files.Add(sci.FileName);
                            }

                            foreach (var languageFileGroup in groupedFiles)
                            {
                                LintingManager.LintFiles(languageFileGroup.Value, languageFileGroup.Key);
                            }
                        }
                    }
                    
                    break;
                case EventType.FileModify:
                    LintingManager.UnLintFile(((TextEvent)e).Value);
                    break;
            }
        }
    }
}
