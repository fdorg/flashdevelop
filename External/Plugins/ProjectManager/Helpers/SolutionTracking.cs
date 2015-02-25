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
        public event SetProjectHandler SetProject;

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

            var fileName = doc.FileName;
            if (IsProjectOf(PluginBase.CurrentProject as Project, fileName)) return;

            foreach (Project project in PluginBase.CurrentSolution.Projects)
            {
                if (IsProjectOf(project, fileName))
                {
                    SetCurrentProject(project);
                    return;
                }
            }
        }

        bool IsProjectOf(Project project, String fileName)
        {
            if (project == null) return false;
            try
            {
                // TODO this may require some proper path normalization (see ASContext.cs)
                var dir = Path.GetDirectoryName(project.ProjectPath) + Path.DirectorySeparatorChar;
                return fileName.StartsWith(dir, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        void SetCurrentProject(Project project)
        {
            if (SetProject != null) SetProject(project);
        }
    }

    delegate void SetProjectHandler(Project project);
}
