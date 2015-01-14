using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using ProjectManager.Controls;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Utilities;
using System.Collections.Generic;
using ProjectManager.Projects;
using PluginCore.Helpers;

namespace ProjectManager.Controls
{
	public class FDMenus
	{
        public ToolStripMenuItem View;
        public ToolStripMenuItem GlobalClasspaths;
        public ToolStripButton TestMovie;
        public ToolStripButton BuildProject;
        public ToolStripComboBox ConfigurationSelector;
        public ToolStripComboBox TargetBuildSelector;
        public RecentProjectsMenu RecentProjects;
		public ProjectMenu ProjectMenu;

		public FDMenus(IMainForm mainForm)
		{
            // modify the file menu
            ToolStripMenuItem fileMenu = (ToolStripMenuItem)mainForm.FindMenuItem("FileMenu");
            RecentProjects = new RecentProjectsMenu();
            fileMenu.DropDownItems.Insert(5, RecentProjects);

            // modify the view menu
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)mainForm.FindMenuItem("ViewMenu");
            View = new ToolStripMenuItem(TextHelper.GetString("Label.MainMenuItem"));
			View.Image = Icons.Project.Img;
			viewMenu.DropDownItems.Add(View);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowProject", View);

			// modify the tools menu - add a nice GUI classpath editor
            ToolStripMenuItem toolsMenu = (ToolStripMenuItem)mainForm.FindMenuItem("ToolsMenu");
            GlobalClasspaths = new ToolStripMenuItem(TextHelper.GetString("Label.GlobalClasspaths"));
			GlobalClasspaths.ShortcutKeys = Keys.F9 | Keys.Control;
            GlobalClasspaths.Image = Icons.Classpath.Img;
            toolsMenu.DropDownItems.Insert(toolsMenu.DropDownItems.Count - 4, GlobalClasspaths);
            PluginBase.MainForm.RegisterShortcutItem("ToolsMenu.GlobalClasspaths", GlobalClasspaths);

			ProjectMenu = new ProjectMenu();

            MenuStrip mainMenu = mainForm.MenuStrip;
            mainMenu.Items.Insert(5, ProjectMenu);

            ToolStrip toolBar = mainForm.ToolStrip;
			toolBar.Items.Add(new ToolStripSeparator());

            toolBar.Items.Add(RecentProjects.ToolbarSelector);

            BuildProject = new ToolStripButton(Icons.Gear.Img);
            BuildProject.Name = "BuildProject";
            BuildProject.ToolTipText = TextHelper.GetString("Label.BuildProject").Replace("&", "");
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.BuildProject", BuildProject);
            toolBar.Items.Add(BuildProject);

            TestMovie = new ToolStripButton(Icons.GreenCheck.Img);
            TestMovie.Name = "TestMovie";
            TestMovie.ToolTipText = TextHelper.GetString("Label.TestMovie").Replace("&", "");
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.TestMovie", TestMovie);
            toolBar.Items.Add(TestMovie);

            ConfigurationSelector = new ToolStripComboBox();
            ConfigurationSelector.Name = "ConfigurationSelector";
            ConfigurationSelector.ToolTipText = TextHelper.GetString("ToolTip.SelectConfiguration");
            ConfigurationSelector.Items.AddRange(new string[] { TextHelper.GetString("Info.Debug"), TextHelper.GetString("Info.Release") });
            ConfigurationSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigurationSelector.AutoSize = false;
            ConfigurationSelector.Enabled = false;
            ConfigurationSelector.Width = ScaleHelper.Scale(85);
            ConfigurationSelector.Margin = new Padding(1, 0, 0, 0);
            ConfigurationSelector.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            ConfigurationSelector.Font = PluginBase.Settings.DefaultFont;
            toolBar.Items.Add(ConfigurationSelector);
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.ConfigurationSelectorToggle", Keys.Control | Keys.F5);
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.ConfigurationSelectorToggle", ConfigurationSelector);
            
            TargetBuildSelector = new ToolStripComboBox();
            TargetBuildSelector.Name = "TargetBuildSelector";
            TargetBuildSelector.ToolTipText = TextHelper.GetString("ToolTip.TargetBuild");
            TargetBuildSelector.AutoSize = false;
            TargetBuildSelector.Width = ScaleHelper.Scale(85);
            TargetBuildSelector.Margin = new Padding(1, 0, 0, 0);
            TargetBuildSelector.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            TargetBuildSelector.Font = PluginBase.Settings.DefaultFont;
            toolBar.Items.Add(TargetBuildSelector);
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.TargetBuildSelector", Keys.Control | Keys.F6);
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.TargetBuildSelector", TargetBuildSelector);
            EnableTargetBuildSelector(false);
        }

        public void EnableTargetBuildSelector(bool enabled)
        {
            TargetBuildSelector.Enabled = enabled;
        }

        public bool DisabledForBuild
        {
            get { return !TestMovie.Enabled; }
            set
            {
                BuildProject.Enabled = TestMovie.Enabled = ProjectMenu.AllItemsEnabled = ConfigurationSelector.Enabled = !value;
                EnableTargetBuildSelector(!value);
            }
        }

        public void SetProject(Project project)
        {
            RecentProjects.AddOpenedProject(project.ProjectPath);
            ConfigurationSelector.Enabled = true;
            ProjectMenu.ProjectItemsEnabled = true;
            TestMovie.Enabled = true;
            BuildProject.Enabled = true;
            ProjectChanged(project);
        }

        public void ProjectChanged(Project project)
        {
            TargetBuildSelector.Items.Clear();
            if (project.MovieOptions.TargetBuildTypes != null)
            {
                TargetBuildSelector.Items.AddRange(project.MovieOptions.TargetBuildTypes);
                string target = project.TargetBuild ?? project.MovieOptions.TargetBuildTypes[0];
                if (target != "" && !TargetBuildSelector.Items.Contains(target)) TargetBuildSelector.Items.Insert(0, target);
                TargetBuildSelector.Text = target;
                EnableTargetBuildSelector(true);
            }
            else if (project.OutputType == OutputType.CustomBuild)
            {
                string target = project.TargetBuild ?? "";
                if (target != "") TargetBuildSelector.Items.Insert(0, target);
                TargetBuildSelector.Text = target;
                EnableTargetBuildSelector(true);
            }
            else
            {
                TargetBuildSelector.Text = "";
                EnableTargetBuildSelector(false);
            }
        }

        
        public void ToggleDebugRelease()
        {
            ConfigurationSelector.SelectedIndex = (ConfigurationSelector.SelectedIndex + 1) % 2;
        }
    }

	/// <summary>
	/// The "Project" menu for FD's main menu
	/// </summary>
	public class ProjectMenu : ToolStripMenuItem
	{
		public ToolStripMenuItem NewProject;
        public ToolStripMenuItem OpenProject;
        public ToolStripMenuItem ImportProject;
        public ToolStripMenuItem CloseProject;
        public ToolStripMenuItem OpenResource;
        public ToolStripMenuItem TestMovie;
        public ToolStripMenuItem RunProject;
        public ToolStripMenuItem BuildProject;
        public ToolStripMenuItem CleanProject;
        public ToolStripMenuItem Properties;

        private List<ToolStripItem> AllItems;

		public ProjectMenu()
		{
            AllItems = new List<ToolStripItem>();

            NewProject = new ToolStripMenuItem(TextHelper.GetString("Label.NewProject"));
			NewProject.Image = Icons.NewProject.Img;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.NewProject", NewProject);
            //AllItems.Add(NewProject);

            OpenProject = new ToolStripMenuItem(TextHelper.GetString("Label.OpenProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.OpenProject", OpenProject);
            //AllItems.Add(OpenProject);

            ImportProject = new ToolStripMenuItem(TextHelper.GetString("Label.ImportProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.ImportProject", ImportProject);
            //AllItems.Add(ImportProject);

            CloseProject = new ToolStripMenuItem(TextHelper.GetString("Label.CloseProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.CloseProject", CloseProject);
            AllItems.Add(CloseProject);

            OpenResource = new ToolStripMenuItem(TextHelper.GetString("Label.OpenResource"));
            OpenResource.Image = PluginBase.MainForm.FindImage("209");
            OpenResource.ShortcutKeys = Keys.Control | Keys.R;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.OpenResource", OpenResource);
            AllItems.Add(OpenResource);

            TestMovie = new ToolStripMenuItem(TextHelper.GetString("Label.TestMovie"));
			TestMovie.Image = Icons.GreenCheck.Img;
            TestMovie.ShortcutKeys = Keys.F5;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.TestMovie", TestMovie);
            AllItems.Add(TestMovie);

            RunProject = new ToolStripMenuItem(TextHelper.GetString("Label.RunProject"));
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.RunProject", RunProject);
            AllItems.Add(RunProject);

            BuildProject = new ToolStripMenuItem(TextHelper.GetString("Label.BuildProject"));
			BuildProject.Image = Icons.Gear.Img;
            BuildProject.ShortcutKeys = Keys.F8;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.BuildProject", BuildProject);
            AllItems.Add(BuildProject);

            CleanProject = new ToolStripMenuItem(TextHelper.GetString("Label.CleanProject"));
            CleanProject.ShortcutKeys = Keys.Shift | Keys.F8;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.CleanProject", CleanProject);
            AllItems.Add(CleanProject);

            Properties = new ToolStripMenuItem(TextHelper.GetString("Label.Properties"));
			Properties.Image = Icons.Options.Img;
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.Properties", Properties);
            AllItems.Add(Properties);

            base.Text = TextHelper.GetString("Label.Project");
            base.DropDownItems.Add(NewProject);
            base.DropDownItems.Add(OpenProject);
            base.DropDownItems.Add(ImportProject);
            base.DropDownItems.Add(CloseProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(OpenResource);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(TestMovie);
            base.DropDownItems.Add(RunProject);
            base.DropDownItems.Add(BuildProject);
            base.DropDownItems.Add(CleanProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(Properties);
		}

		public bool ProjectItemsEnabled
		{
			set
			{
				CloseProject.Enabled = value;
				TestMovie.Enabled = value;
                BuildProject.Enabled = value;
                CleanProject.Enabled = value;
				Properties.Enabled = value;
                OpenResource.Enabled = value;
			}
		}

        public bool AllItemsEnabled
        {
            set
            {
                foreach (ToolStripItem item in DropDownItems)
                {
                    // Toggle items only if it's our creation
                    if (AllItems.Contains(item)) item.Enabled = value;
                }
            }
        }
	}

}
