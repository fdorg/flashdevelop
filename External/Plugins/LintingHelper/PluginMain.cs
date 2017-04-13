using CodeRefactor.Managers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.IO;

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
        internal static RichToolTip Tip;

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
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileSave | EventType.FileModify);

            UITools.Manager.OnMouseHover += Scintilla_OnMouseHover;
            UITools.Manager.OnMouseHoverEnd += Scintilla_OnMouseHoverEnd;
        }

        private void Scintilla_OnMouseHover(ScintillaNet.ScintillaControl sender, int position)
        {
            var results = Managers.LintingManager.Cache.GetResultsFromPosition(DocumentManager.FindDocument(sender), position);
            if (results == null)
                return;

            var desc = "";

            foreach (var result in results)
            {
                if (!string.IsNullOrEmpty(result.Description))
                    desc += "\r\n" + result.Description;
            }

            if (desc != string.Empty)
            {
                desc = desc.Remove(0, 2); //remove \r\n
                Tip.ShowAtMouseLocation(desc);

                //move simpleTip up to not overlap linting tip
                UITools.Tip.Location = new Point(UITools.Tip.Location.X, UITools.Tip.Location.Y - Tip.Size.Height);
            }
        }

        private void Scintilla_OnMouseHoverEnd(ScintillaNet.ScintillaControl sender, int position)
        {
            Tip.Hide();
        }

        private void InitBasics()
        {
            Tip = new RichToolTip(PluginBase.MainForm);

            string dataPath = Path.Combine(PathHelper.DataDir, nameof(LintingHelper));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, $"{nameof(Settings)}.fdb");
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
                        Managers.LintingManager.LintFiles(new string[] { fileOpen.Value });
                    }
                    break;
                case EventType.FileSave:
                    if (MessageBar.Locked) return;
                    var reason = (e as TextDataEvent)?.Data as string;
                    if (reason != "HaxeComplete" && this.settingObject.LintOnSave)
                    {
                        var fileSave = (TextEvent) e;
                        Managers.LintingManager.LintFiles(new string[] { fileSave.Value });
                    }
                    break;
                case EventType.FileModify:
                    var file = ((TextEvent)e).Value;
                    Managers.LintingManager.UnLintDocument(DocumentManager.FindDocument(file));
                    break;
            }
        }
    }
}
