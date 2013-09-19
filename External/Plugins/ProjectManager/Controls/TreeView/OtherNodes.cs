using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using ProjectManager.Projects;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ProjectManager.Controls.TreeView
{
	public class ProjectNode : WatcherNode
	{
        Project projectRef;
        ReferencesNode references;
        bool isActive;

		public ProjectNode(Project project) : base(project.Directory)
		{
            projectRef = project;
			isDraggable = false;
			isRenamable = false;
		}

		public override void Refresh(bool recursive)
		{
            if (References != null && References.Parent == this) Nodes.Remove(References);

			base.Refresh(recursive);
            Text = ProjectRef.Name + " (" + ProjectRef.Language.ToUpper() + ")";
			ImageIndex = Icons.Project.Index;
			SelectedImageIndex = ImageIndex;
            Expand();

            if (References != null) Nodes.Insert(0, References);
            NotifyRefresh();
		}

        public Project ProjectRef
        {
            get { return projectRef; }
        }

        public ReferencesNode References
        {
            get { return references; }
            set
            {
                references = value;
                if (references != null) Nodes.Insert(0, references);
            }
        }

        public bool IsActive 
        {
            get { return isActive; }
            set 
            {
                if (isActive == value) return;
                isActive = value;
                FontStyle style = isActive ? FontStyle.Bold : FontStyle.Regular;
                NodeFont = new System.Drawing.Font(PluginCore.PluginBase.Settings.DefaultFont, style);
                Text = Text; // Reset text to update the font
            }
        }
	}

	public class ClasspathNode : WatcherNode
	{
        public string classpath;

		public ClasspathNode(Project project, string classpath, string text) : base(classpath)
		{
			isDraggable = false;
			isRenamable = false;

            this.classpath = classpath;

            // shorten text
            string[] excludes = PluginMain.Settings.FilteredDirectoryNames;
            char sep = Path.DirectorySeparatorChar;
            string[] parts = text.Split(sep);
            List<string> label = new List<string>();
            Regex reVersion = new Regex("^[0-9]+[.,-][0-9]+");

            if (parts.Length > 0)
            {
                for (int i = parts.Length - 1; i > 0; --i)
                {
                    String part = parts[i] as String;
                    if (part != "" && part != "." && part != ".." && Array.IndexOf(excludes, part.ToLower()) == -1)
                    {
                        if (Char.IsDigit(part[0]) && reVersion.IsMatch(part)) label.Add(part);
                        else
                        {
                            label.Add(part);
                            break;
                        }
                    }
                    else label.Add(part);
                }
            }
            label.Reverse();
            Text = String.Join("/", label.ToArray());
            ToolTipText = classpath;
		}

		public override void Refresh(bool recursive)
		{

			base.Refresh(recursive);

            base.isInvalid = !Directory.Exists(BackingPath);

            if (!isInvalid)
            {
                ImageIndex = Icons.Classpath.Index;
            }
            else
            {
                ImageIndex = Icons.ClasspathError.Index;
            }

            SelectedImageIndex = ImageIndex;

            NotifyRefresh();
		}
	}

    public class ProjectClasspathNode : ClasspathNode
    {
        public ProjectClasspathNode(Project project, string classpath, string text) : base(project, classpath, text)
        {
            if (text != Text)
            {
                ToolTipText = text;
            }
            else
            {
                ToolTipText = "";
            }
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);

            if (!IsInvalid)
            {
                ImageIndex = Icons.ProjectClasspath.Index;
            }
            else
            {
                ImageIndex = Icons.ProjectClasspathError.Index;
            }

            SelectedImageIndex = ImageIndex;

            NotifyRefresh();
        }
    }

    public class ReferencesNode : GenericNode
    {
        public ReferencesNode(string projectPath, string text)
            : base(Path.Combine(projectPath, "__References__"))
        {
            Text = text;
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
            isDraggable = false;
            isRenamable = false;
        }
    }
}
