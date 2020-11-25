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
        DockContent cmdPanelDockContent;
        Gui.TabbedConsole tabView;
        Image image;

        public int Api => 1;

        public string Author { get; } = "Christoph Otter";

        public string Description { get; } = "Plugin that adds an embedded console window to FlashDevelop.";

        public string Guid { get; } = "AEDE556E-54C3-4EFB-8EF3-54E85DC37D1E";

        public string Help { get; } = "www.flashdevelop.org/community/";

        public string Name { get; } = nameof(ConsolePanel);

        [Browsable(false)]
        public object Settings => settingObject;

        public IConsoleProvider ConsoleProvider { get; set; }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreatePluginPanel();
            CreateDefaultConsoleProvider();
            CreateConsolePanel();
            CreateMenuItem();

            EventManager.AddEventHandler(this, EventType.Command, HandlingPriority.Normal);
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
                            panel.WorkingDirectory = PluginBase.CurrentProject.GetAbsolutePath("");
                        }
                    }
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

        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        void CreatePluginPanel()
        {
            tabView = new Gui.TabbedConsole(this);
            cmdPanelDockContent = PluginBase.MainForm.CreateDockablePanel(tabView, Guid, image, DockState.DockBottom);
            cmdPanelDockContent.Text = "Console";
        }

        void CreateDefaultConsoleProvider() => ConsoleProvider = new Implementation.CmdProcess.CmdConsoleProvider();

        public IConsole CreateConsolePanel()
        {
            cmdPanelDockContent.Show();
            var cmdPanel = ConsoleProvider.GetConsole();
            cmdPanel.Exited += (sender, args) =>
            {
                if (tabView.InvokeRequired)
                {
                    tabView.Invoke((MethodInvoker) (() =>
                    {
                        if (!PluginBase.MainForm.ClosingEntirely)
                            tabView.RemoveConsole(cmdPanel);
                    }));
                }
                else if (!PluginBase.MainForm.ClosingEntirely)
                    tabView.RemoveConsole(cmdPanel);
            };
            tabView.AddConsole(cmdPanel);
            return cmdPanel;
        }

        void CreateMenuItem()
        {
            var label = "Console Panel";
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var cmdItem = new ToolStripMenuItem(label, image, OpenCmdPanel);
            viewMenu.DropDownItems.Add(cmdItem);
        }

        void OpenCmdPanel(object sender, EventArgs e) => CreateConsolePanel();
    }
}
