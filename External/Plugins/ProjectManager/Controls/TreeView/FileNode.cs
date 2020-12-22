using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects;

namespace ProjectManager.Controls.TreeView
{
    public delegate FileNode FileNodeFactory(string filePath);
    public delegate void FileNodeRefresh(FileNode node);

    /// <summary>
    /// Represents a file on disk.
    /// </summary>
    public class FileNode : GenericNode
    {
        public static readonly Dictionary<string, FileNodeFactory> FileAssociations 
            = new Dictionary<string, FileNodeFactory>();

        public static event FileNodeRefresh OnFileNodeRefresh;

        protected FileNode(string filePath) : base(filePath)
        {
            isDraggable = true;
            isRenamable = true;
        }

        /// <summary>
        /// Creates the correct type of FileNode based on the file name.
        /// </summary>
        public static FileNode Create(string filePath, Project project)
        {
            if (project != null) 
            {
                if (project.IsOutput(filePath))
                    return new ProjectOutputNode(filePath);
                if (project.IsInput(filePath))
                    return new InputSwfNode(filePath);
            }

            string ext = Path.GetExtension(filePath).ToLower();

            if (FileInspector.IsSwf(ext) || FileInspector.IsSwc(filePath, ext))
                return new SwfFileNode(filePath);
            if (FileAssociations.ContainsKey(ext)) // custom nodes building
                return FileAssociations[ext](filePath);
            return new FileNode(filePath);
        }

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);

            string path = BackingPath;
            string ext = Path.GetExtension(path).ToLower();

            if (project != null && project.IsPathHidden(path))
                ImageIndex = Icons.HiddenFile.Index;
            else if (project.IsCompileTarget(path) && (FileInspector.IsActionScript(ext) || FileInspector.IsHaxeFile(ext)))
                ImageIndex = Icons.ActionScriptCompile.Index;
            else if (FileInspector.IsMxml(ext) && project.IsCompileTarget(path))
                ImageIndex = Icons.MxmlFileCompile.Index;
            else if (FileInspector.IsCss(ext) && project.IsCompileTarget(path))
                ImageIndex = Icons.ActionScriptCompile.Index;
            else if (FileInspector.IsSwc(path) && Parent is null) // external SWC library
                ImageIndex = Icons.Classpath.Index;
            else
                ImageIndex = Icons.GetImageForFile(path).Index;
            SelectedImageIndex = ImageIndex;

            Text = Path.GetFileName(path);

            string colorId = "ProjectTreeView.ForeColor";
            if (project != null && project.IsLibraryAsset(path))
            {
                LibraryAsset asset = project.GetAsset(path);
                if (asset != null && asset.IsSwc)
                {
                    colorId = asset.SwfMode switch
                    {
                        SwfAssetMode.ExternalLibrary => "ProjectTreeView.ExternalLibraryTextColor",
                        SwfAssetMode.Library => "ProjectTreeView.LibraryTextColor",
                        SwfAssetMode.IncludedLibrary => "ProjectTreeView.IncludedLibraryTextColor",
                        _ => colorId
                    };
                }

                if (asset != null && asset.HasManualID)
                    Text += " (" + asset.ManualID + ")";
            }

            var textColor = PluginBase.MainForm.GetThemeColor(colorId);
            if (colorId != "ProjectTreeView.ForeColor" && textColor == Color.Empty) textColor = SystemColors.Highlight;
            ForeColorRequest = textColor != Color.Empty ? textColor : SystemColors.ControlText;
            // hook for plugins
            OnFileNodeRefresh?.Invoke(this);
        }
    }

    /// <summary>
    /// A special FileNode that represents the project output file.  It won't disappear
    /// from the treeview while you're building.
    /// </summary>
    public class ProjectOutputNode : SwfFileNode
    {
        public ProjectOutputNode(string filePath) : base(filePath) {}

        public override void Refresh(bool recursive)
        {
            base.Refresh(recursive);
            if (!FileExists) ImageIndex = SelectedImageIndex = Icons.SwfFileHidden.Index;
        }
    }
}
