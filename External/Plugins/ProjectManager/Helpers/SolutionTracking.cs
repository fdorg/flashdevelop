using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ProjectManager.Controls;
using ProjectManager.Projects;
using PluginCore;
using System.IO;

namespace ProjectManager.Helpers
{
    class SolutionTracking
    {
        public event ActiveProjectHandler ActiveProject;

        public bool CurrentDocumentInProject { get; private set; }

        PluginUI pluginUI;
        ProjectManagerSettings settings;
        Timer timer;

        public SolutionTracking(PluginUI pluginUI, ProjectManagerSettings Settings)
        {
            this.pluginUI = pluginUI;
            this.settings = Settings;

            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += timer_Tick;
        }

        public void Update(bool async = true)
        {
            /*if (async)
            {
                timer.Stop();
                timer.Start();
            }
            else*/ TrackProject();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TrackProject();
            TabColors.UpdateTabs(settings);
        }

        void TrackProject()
        {
            if (PluginBase.CurrentSolution == null)
            {
                SetCurrentProject(null);
                return;
            }
            var doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable || !File.Exists(doc.FileName)) return;

            var docUri = new Uri(doc.FileName);
            if (IsProjectOf(PluginBase.CurrentProject as Project, docUri)) return;

            foreach (Project project in PluginBase.CurrentSolution.Projects)
            {
                if (IsProjectOf(project, docUri))
                    SetCurrentProject(project);
            }
        }

        bool IsProjectOf(Project project, Uri docUri)
        {
            if (project == null) return false;
            try
            {
                var dir = Path.GetDirectoryName(project.ProjectPath);
                return new Uri(dir).IsBaseOf(docUri);
            }
            catch (Exception)
            {
                return false;
            }
        }

        void SetCurrentProject(Project project)
        {
            if (ActiveProject != null) ActiveProject(project);
        }
    }

    delegate void ActiveProjectHandler(Project project);
}
