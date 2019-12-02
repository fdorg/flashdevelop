// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.IO;

namespace PluginCore.Localization
{
    // These exist because we want to decorate some of our compiler option classes
    // with localized descriptions for the PropertyGrid, but we don't want fdbuild
    // to have to reference PluginCore.

    class LocalizedDescriptionAttribute : Attribute
    {
        public LocalizedDescriptionAttribute(string fake) { }
    }
    class LocalizedCategoryAttribute : Attribute
    {
        public LocalizedCategoryAttribute(string fake) { }
    }
}

namespace PluginCore
{
    interface IProject { }
}

namespace ProjectManager.Controls
{
    public class PropertiesDialog { }
}
namespace ProjectManager.Controls.AS2
{
    public class AS2PropertiesDialog : PropertiesDialog { }
}
namespace ProjectManager.Controls.AS3
{
    public class AS3PropertiesDialog : PropertiesDialog { }
}

namespace ProjectManager.Helpers
{
    /// <summary>
    /// Can be extended at runtime by a FlashDevelop plugin, but not in the command line
    /// </summary>
    class ProjectCreator
    {
        private static readonly Hashtable projectTypes = new Hashtable();
        private static bool projectTypesSet;

        private static void SetInitialProjectHash()
        {
            projectTypes["project.fdp"] = typeof(Projects.AS2.AS2Project);
            projectTypes["project.as2proj"] = typeof(Projects.AS2.AS2Project);
            projectTypes["project.as3proj"] = typeof(Projects.AS3.AS3Project);
            projectTypes["project.hxproj"] = typeof(Projects.Haxe.HaxeProject);
            projectTypes["project.fdproj"] = typeof(Projects.Generic.GenericProject);
            projectTypesSet = true;
        }

        public static bool IsKnownProject(string ext)
        {
            if (!projectTypesSet) SetInitialProjectHash();
            return projectTypes.ContainsKey("project" + ext);
        }

        public static Type GetProjectType(string key)
        {
            if (!projectTypesSet) SetInitialProjectHash();
            if (projectTypes.ContainsKey(key))
                return (Type)projectTypes[key];
            return null;
        }

        public static string KeyForProjectPath(string path)
        {
            return "project" + Path.GetExtension(path).ToLower();
        }
    }
}

namespace ProjectManager.Projects.AS3
{
    internal class FlexProjectReader : ProjectReader
    {
        public FlexProjectReader(string filename)
            : base(filename, new AS3Project(filename))
        {
        }
    }

}
