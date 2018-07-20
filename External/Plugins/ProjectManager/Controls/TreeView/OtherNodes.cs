using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore;
using ProjectManager.Projects;
using ProjectManager.Projects.AS3;

namespace ProjectManager.Controls.TreeView
{
    public class ProjectNode : WatcherNode
    {
        ReferencesNode references;
        bool isActive;

        public ProjectNode(Project project) : base(project.Directory)
        {
            this.project = project;
            isDraggable = false;
        }

        public override void Refresh(bool recursive)
        {
            RemoveReferences();

            base.Refresh(recursive);
            NodeFont = new Font(PluginBase.Settings.DefaultFont, FontStyle.Bold);
            Text = ProjectRef.Name + " (" + ProjectRef.LanguageDisplayName + ")";
            ImageIndex = Icons.Project.Index;
            SelectedImageIndex = ImageIndex;

            if (References != null && References.Parent == null)
            {
                if (recursive)
                {
                    RefreshReferences(recursive);
                    return;
                }
                Nodes.Insert(0, References);
            }

            NotifyProjectRefresh();
            Expand();
        }

        private void NotifyProjectRefresh()
        {
            base.NotifyRefresh();
        }

        protected override void NotifyRefresh()
        {
            // do nothing yet, we are not finished
        }

        private void RefreshReferences(bool recursive)
        {
            if (References != null && References.Parent == null)
            {
                Nodes.Insert(0, References);
                References.Refresh(recursive);
            }
        }

        private void RemoveReferences()
        {
            if (References != null && References.Parent == this)
                Nodes.Remove(References);
        }

        public Project ProjectRef
        {
            get { return project; }
        }

        public ReferencesNode References
        {
            get { return references; }
            set
            {
                references = value;
                if (references != null)
                    RefreshReferences(true);
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
                NodeFont = new Font(PluginBase.Settings.DefaultFont, style);
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
            Regex reSHAHash = new Regex("^[0-9a-f]+$");

            if (parts.Length > 0)
            {
                for (int i = parts.Length - 1; i > 0; --i)
                {
                    String part = parts[i] as String;
                    if (part != "" && part != "." && part != ".." && Array.IndexOf(excludes, part.ToLower()) == -1)
                    {
                        if (Char.IsDigit(part[0]) && reVersion.IsMatch(part)) label.Add(part);
                        else if (part.Length == 40 && reSHAHash.IsMatch(part)) label.Add(part);
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
        public ReferencesNode(Project project, string text)
            : base(Path.Combine(project.Directory, "__References__"))
        {
            Text = text;
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.ForeColor", SystemColors.WindowText);
            isDraggable = false;
            isRenamable = false;
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);

            ArrayList projectClasspaths = new ArrayList();
            ArrayList globalClasspaths = new ArrayList();

            GenericNodeList nodesToDie = new GenericNodeList();
            foreach (GenericNode oldRef in Nodes) nodesToDie.Add(oldRef);
            //if (Nodes.Count == 0) recursive = true;

            // explore classpaths
            if (PluginMain.Settings.ShowProjectClasspaths)
            {
                projectClasspaths.AddRange(project.Classpaths);
                if (project.AdditionalPaths != null) projectClasspaths.AddRange(project.AdditionalPaths);
            }
            projectClasspaths.Sort();

            if (PluginMain.Settings.ShowGlobalClasspaths)
                globalClasspaths.AddRange(PluginMain.Settings.GlobalClasspaths);
            globalClasspaths.Sort();

            // create references nodes
            ClasspathNode cpNode;
            foreach (string projectClasspath in projectClasspaths)
            {
                string absolute = projectClasspath;
                if (!Path.IsPathRooted(absolute))
                    absolute = project.GetAbsolutePath(projectClasspath);
                if ((absolute + "\\").StartsWithOrdinal(project.Directory + "\\"))
                    continue;
                if (!project.ShowHiddenPaths && project.IsPathHidden(absolute))
                    continue;

                cpNode = ReuseNode(absolute, nodesToDie) as ProjectClasspathNode ?? new ProjectClasspathNode(project, absolute, projectClasspath);
                Nodes.Add(cpNode);
                cpNode.Refresh(recursive);
            }

            foreach (string globalClasspath in globalClasspaths)
            {
                string absolute = globalClasspath;
                if (!Path.IsPathRooted(absolute))
                    absolute = project.GetAbsolutePath(globalClasspath);
                if (absolute.StartsWithOrdinal(project.Directory + Path.DirectorySeparatorChar))
                    continue;

                cpNode = ReuseNode(absolute, nodesToDie) as ProjectClasspathNode ?? new ClasspathNode(project, absolute, globalClasspath);
                Nodes.Add(cpNode);
                cpNode.Refresh(recursive);
            }

            // add external libraries at the top level also
            if (project is AS3Project)
                foreach (LibraryAsset asset in (project as AS3Project).SwcLibraries)
                {
                    if (!asset.IsSwc) continue;
                    // check if SWC is inside the project or inside a classpath
                    string absolute = asset.Path;
                    if (!Path.IsPathRooted(absolute))
                        absolute = project.GetAbsolutePath(asset.Path);

                    bool showNode = true;
                    if (absolute.StartsWithOrdinal(project.Directory))
                        showNode = false;
                    foreach (string path in project.AbsoluteClasspaths)
                        if (absolute.StartsWithOrdinal(path))
                        {
                            showNode = false;
                            break;
                        }
                    foreach (string path in PluginMain.Settings.GlobalClasspaths)
                        if (absolute.StartsWithOrdinal(path))
                        {
                            showNode = false;
                            break;
                        }

                    if (showNode && !project.ShowHiddenPaths && project.IsPathHidden(absolute))
                        continue;

                    if (showNode && File.Exists(absolute))
                    {
                        SwfFileNode swcNode = ReuseNode(absolute, nodesToDie) as SwfFileNode ?? new SwfFileNode(absolute);
                        Nodes.Add(swcNode);
                        swcNode.Refresh(recursive);
                    }
                }

            foreach (GenericNode node in nodesToDie)
            {
                node.Dispose();
                Nodes.Remove(node);
            }
        }

        private GenericNode ReuseNode(string absolute, GenericNodeList nodesToDie)
        {
            foreach (GenericNode node in nodesToDie)
                if (node.BackingPath == absolute)
                {
                    nodesToDie.Remove(node);
                    Nodes.Remove(node);
                    return node;
                }
            return null;
        }
    }
}
