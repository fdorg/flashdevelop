using CodeRefactor.Managers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using LintingHelper.Managers;
using PluginCore.Localization;
using ProjectManager;

namespace LintingHelper
{
    public class PluginMain : IPlugin
    {
        private string pluginName = "LintingHelper";
        private string pluginGuid = "279C4926-5AC6-49E1-A0AC-66B7275C13DB";
        private string pluginHelp = "www.flashdevelop.org/community/";
        private string pluginDesc = "Plugin that adds a generic interface for linting / code analysis.";
        private string pluginAuth = "FlashDevelop Team";
        private Settings settingObject;
        private string settingFilename;

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
                        LintingManager.LintFiles(new string[] { fileOpen.Value });
                    }
                    break;
                case EventType.FileSave:
                    if (MessageBar.Locked) return;
                    var reason = (e as TextDataEvent)?.Data as string;
                    if (reason != "HaxeComplete" && this.settingObject.LintOnSave)
                    {
                        var fileSave = (TextEvent) e;
                        LintingManager.LintFiles(new string[] { fileSave.Value });
                    }
                    break;
                case EventType.Command:
                    var ev = (DataEvent) e;
                    if (ev.Action == ProjectManagerEvents.BuildComplete || ev.Action == ProjectManagerEvents.Project)
                    {
                        LintingManager.Cache.RemoveAllExcept(new string[] { });
                        LintingManager.UpdateLinterPanel();
                    }
                    break;
                case EventType.FileModify:
                    LintingManager.UnLintFile(((TextEvent)e).Value);
                    break;
            }
        }
    }
}
