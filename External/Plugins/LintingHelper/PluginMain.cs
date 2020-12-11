// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        Settings settingObject;
        string settingFilename;

        public int Api => 1;

        public string Author => "FlashDevelop Team";

        public string Description => "Plugin that adds a generic interface for linting / code analysis.";

        public string Guid => "279C4926-5AC6-49E1-A0AC-66B7275C13DB";

        public string Help => "www.flashdevelop.org/community/";

        public string Name => nameof(LintingHelper);

        [Browsable(false)]
        public object Settings => settingObject;

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
        }

        public void Dispose() => SaveSettings();

        void AddEventHandlers()
        {
            BatchProcessManager.AddBatchProcessor(new BatchProcess.LintProcessor());
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileSave | EventType.FileModify | EventType.Command);
        }

        void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(LintingHelper));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, $"{nameof(Settings)}.fdb");
            TraceManager.RegisterTraceGroup(LintingManager.TraceGroup, TextHelper.GetStringWithoutMnemonics("Label.LintingResults"));
        }

        void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch(e.Type)
            {
                case EventType.FileOpen:
                    if (MessageBar.Locked || !settingObject.LintOnOpen) return;
                    var fileOpen = (TextEvent) e;
                    LintingManager.Cache.RemoveDocument(fileOpen.Value);
                    LintingManager.LintFiles(new[] {fileOpen.Value});
                    break;
                case EventType.FileSave:
                    if (MessageBar.Locked || !settingObject.LintOnSave) return;
                    var reason = (e as TextDataEvent)?.Data as string;
                    if (reason != "HaxeComplete")
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
                            var projRoot = Path.GetDirectoryName(PluginBase.CurrentProject?.ProjectPath);
                            if (projRoot.IsNullOrEmpty()) return;
                            var groupedFiles = new Dictionary<string, List<string>>();
                            foreach (var doc in PluginBase.MainForm.Documents)
                            {
                                var sci = doc.SciControl;
                                if (sci is null || doc.IsUntitled || !sci.FileName.StartsWithOrdinal(projRoot)) continue;

                                var files = groupedFiles.GetOrCreate(sci.ConfigurationLanguage);
                                files.Add(sci.FileName);
                            }
                            foreach (var group in groupedFiles)
                            {
                                LintingManager.LintFiles(group.Value, group.Key);
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