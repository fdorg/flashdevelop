using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

            if (References != null && References.Parent is null)
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

        void NotifyProjectRefresh()
        {
            base.NotifyRefresh();
        }

        protected override void NotifyRefresh()
        {
            // do nothing yet, we are not finished
        }

        void RefreshReferences(bool recursive)
        {
            if (References != null && References.Parent is null)
            {
                Nodes.Insert(0, References);
                References.Refresh(recursive);
            }
        }

        void RemoveReferences()
        {
            if (References != null && References.Parent == this)
                Nodes.Remove(References);
        }

        public Project ProjectRef => project;

        public ReferencesNode References
        {
            get => references;
            set
            {
                references = value;
                if (references != null)
                    RefreshReferences(true);
            }
        }

        public bool IsActive 
        {
            get => isActive;
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

        public ClasspathNode(string classpath, string text) : base(classpath)
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
                    string part = parts[i];
                    if (part != "" && part != "." && part != ".." && !excludes.Contains(part.ToLower()))
                    {
                        if (char.IsDigit(part[0]) && reVersion.IsMatch(part)) label.Add(part);
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
            Text = string.Join("/", label.ToArray());
            ToolTipText = classpath;
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);

            isInvalid = !Directory.Exists(BackingPath);
            ImageIndex = !isInvalid
                ? Icons.Classpath.Index
                : Icons.ClasspathError.Index;

            SelectedImageIndex = ImageIndex;
            NotifyRefresh();
        }
    }

    public class ProjectClasspathNode : ClasspathNode
    {
        public ProjectClasspathNode(string classpath, string text) : base(classpath, text)
        {
            ToolTipText = text != Text
                ? text
                : "";
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);
            ImageIndex = !IsInvalid
                ? Icons.ProjectClasspath.Index
                : Icons.ProjectClasspathError.Index;

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
            var nodesToDie = new GenericNodeList();
            nodesToDie.AddRange(Nodes);
            if (PluginMain.Settings.ShowExternalLibraries)
            {
                foreach (var it in project.ExternalLibraries)
                {
                    var node = ReuseNode(it, nodesToDie) as ProjectClasspathNode ?? new ProjectClasspathNode(it, it);
                    Nodes.Add(node);
                    node.Refresh(recursive);
                }
            }

            // explore classpaths
            if (PluginMain.Settings.ShowProjectClasspaths)
            {
                var projectClasspaths = new List<string>(project.Classpaths);
                if (project.AdditionalPaths != null) projectClasspaths.AddRange(project.AdditionalPaths);
                projectClasspaths.Sort();
                // create references nodes
                foreach (string projectClasspath in projectClasspaths)
                {
                    string absolute = projectClasspath;
                    if (!Path.IsPathRooted(absolute))
                        absolute = project.GetAbsolutePath(projectClasspath);
                    if ((absolute + "\\").StartsWithOrdinal(project.Directory + "\\"))
                        continue;
                    if (!project.ShowHiddenPaths && project.IsPathHidden(absolute))
                        continue;

                    var cpNode = ReuseNode(absolute, nodesToDie) as ProjectClasspathNode ?? new ProjectClasspathNode(absolute, projectClasspath);
                    Nodes.Add(cpNode);
                    cpNode.Refresh(recursive);
                }
            }

            if (PluginMain.Settings.ShowGlobalClasspaths)
            {
                var globalClasspaths = new List<string>(PluginMain.Settings.GlobalClasspaths);
                globalClasspaths.Sort();
                foreach (string globalClasspath in globalClasspaths)
                {
                    string absolute = globalClasspath;
                    if (!Path.IsPathRooted(absolute))
                        absolute = project.GetAbsolutePath(globalClasspath);
                    if (absolute.StartsWithOrdinal(project.Directory + Path.DirectorySeparatorChar))
                        continue;

                    var cpNode = ReuseNode(absolute, nodesToDie) as ProjectClasspathNode ?? new ClasspathNode(absolute, globalClasspath);
                    Nodes.Add(cpNode);
                    cpNode.Refresh(recursive);
                }
            }

            // add external libraries at the top level also
            if (project is AS3Project as3Project)
                foreach (LibraryAsset asset in as3Project.SwcLibraries)
                {
                    if (!asset.IsSwc) continue;
                    // check if SWC is inside the project or inside a classpath
                    string absolute = asset.Path;
                    if (!Path.IsPathRooted(absolute))
                        absolute = as3Project.GetAbsolutePath(asset.Path);

                    var showNode = !absolute.StartsWithOrdinal(as3Project.Directory);
                    foreach (string path in as3Project.AbsoluteClasspaths)
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

                    if (showNode && !as3Project.ShowHiddenPaths && as3Project.IsPathHidden(absolute))
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

        GenericNode ReuseNode(string absolute, GenericNodeList nodesToDie)
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
