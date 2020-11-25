using System;
using PluginCore;
using System.ComponentModel;
using PluginCore.Managers;
using PluginCore.Utilities;
using System.IO;
using PluginCore.Helpers;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;

namespace ConsolePanel
{
    public class PluginMain : IPlugin
    {
        string settingFilename;
        Settings settingObject;
        DockContent pluginPanel;
        Gui.TabbedConsole tabView;
        Image image;

        #region Required Properties

        public int Api => 1;

        public string Author { get; } = "Christoph Otter";

        public string Description { get; } = "Plugin that adds an embedded console window to FlashDevelop.";

        public string Guid { get; } = "AEDE556E-54C3-4EFB-8EF3-54E85DC37D1E";

        public string Help { get; } = "www.flashdevelop.org/community/";

        public string Name { get; } = nameof(ConsolePanel);

        [Browsable(false)]
        public object Settings => settingObject;

        #endregion

        public IConsoleProvider ConsoleProvider { get; set; }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreatePluginPanel();
            CreateMenuItem();
            CreateDefaultConsoleProvider();
            AddEventHandlers();
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    var data = (DataEvent)e;
                    if (data.Action == "ProjectManager.Project")
                    {
                        foreach (var panel in tabView.Consoles)
                        {
                            panel.WorkingDirectory = PluginBase.CurrentProject.GetAbsolutePath(string.Empty);
                        }
                    }
                    break;
                case EventType.UIStarted:
                    CreateConsolePanel();
                    break;
            }
        }

        public void Dispose() => SaveSettings();

        void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, Name);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            image = PluginBase.MainForm.FindImage("57");
            Managers.ConsoleManager.Init(this);
        }

        void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command);
            EventManager.AddEventHandler(this, EventType.UIStarted, HandlingPriority.Low);
        }

        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        void CreatePluginPanel()
        {
            tabView = new Gui.TabbedConsole(this);
            pluginPanel = PluginBase.MainForm.CreateDockablePanel(tabView, Guid, image, DockState.DockBottom);
            pluginPanel.Text = "Console";
        }

        void CreateMenuItem()
        {
            var label = "Console Panel";
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var cmdItem = new ToolStripMenuItem(label, image, OpenPanel);
            viewMenu.DropDownItems.Add(cmdItem);
        }

        void OpenPanel(object sender, EventArgs e) => pluginPanel.Show();

        void CreateDefaultConsoleProvider() => ConsoleProvider = new Implementation.CmdProcess.CmdConsoleProvider();

        public IConsole CreateConsolePanel()
        {
            var workingDirectory = PluginBase.CurrentProject?.GetAbsolutePath(string.Empty);
            var panel = ConsoleProvider.GetConsole(workingDirectory);
            panel.Exited += (sender, args) =>
            {
                if (tabView.InvokeRequired)
                {
                    tabView.Invoke((MethodInvoker)(() =>
                    {
                        if (!PluginBase.MainForm.ClosingEntirely)
                            tabView.RemoveConsole(panel);
                    }));
                }
                else if (!PluginBase.MainForm.ClosingEntirely)
                    tabView.RemoveConsole(panel);
            };
            tabView.AddConsole(panel);
            return panel;
        }
    }
}
