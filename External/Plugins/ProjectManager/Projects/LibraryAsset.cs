using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PluginCore.Helpers;
using ProjectManager.Projects.AS2;

namespace ProjectManager.Projects
{
    /// <summary>
    /// Represents an "asset" or project resource to be embedded/referenced in a Project.
    /// </summary>
    public class LibraryAsset
    {
        public Project Project;
        public string Path;
        public string ManualID;
        public string UpdatePath;
        public string FontGlyphs;
        public string Sharepoint;
        public SwfAssetMode SwfMode;
        public bool BitmapLinkage;

        public LibraryAsset(Project project, string path)
        {
            Project = project;
            Path = path;
            ManualID = null;
            UpdatePath = null;
            FontGlyphs = null;
            Sharepoint = null;
            BitmapLinkage = false;
            SwfMode = SwfAssetMode.Library;
        }

        public bool IsImage => FileInspector.IsImage(Extension);
        public bool IsSound => FileInspector.IsSound(Extension);
        public bool IsFont => FileInspector.IsFont(Extension);
        public bool IsSwf => FileInspector.IsSwf(Extension);
        public bool IsSwc => FileInspector.IsSwc(Path, Extension);

        public string Extension => System.IO.Path.GetExtension(Path).ToLower();

        public string ID => ManualID ?? GetAutoID();

        public string GetAutoID()
        {
            // build an ID based on the relative path and library prefix project setting
            string autoID = Path.Replace(System.IO.Path.DirectorySeparatorChar,'.');

            // prefix with libraryprefix if this is an as2 project
            if (Project is AS2Project project && project.CompilerOptions.LibraryPrefix.Length > 0)
                autoID = project.CompilerOptions.LibraryPrefix + "." + autoID;
            
            return autoID;
        }

        public bool HasManualID => ManualID != null;
    }

    public enum SwfAssetMode
    {
        Ignore,
        Library,
        Preloader,
        Shared,
        IncludedLibrary,
        ExternalLibrary
    }

    #region AssetCollection

    public class AssetCollection : Collection<LibraryAsset>
    {
        readonly Project project;

        public AssetCollection(Project project)
        {
            this.project = project;
        }

        public void Add(string path) => Add(new LibraryAsset(project, path));

        public bool Contains(string path) => this[path] != null;

        public LibraryAsset this[string path] => this.FirstOrDefault(asset => asset.Path == path);

        /// <summary>
        /// Removes any paths equal to or below the gives path.
        /// </summary>
        public void RemoveAtOrBelow(string path)
        {
            Remove(path);
            RemoveBelow(path);
        }

        /// <summary>
        /// Removes any paths below the given path.
        /// </summary>
        public void RemoveBelow(string path)
        {
            for (var i = 0; i < Count; i++)
            {
                var asset = this[i];
                if (asset.Path.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.Ordinal) || asset.Path == path)
                {
                    RemoveAt(i--); // search this index again
                }
            }
        }

        public void Remove(string path)
        {
            foreach (var asset in this)
                if (asset.Path == path)
                {
                    Remove(asset);
                    return;
                }
        }
    }

    #endregion
}
