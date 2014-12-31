using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;

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
    public class AS2PropertiesDialog : ProjectManager.Controls.PropertiesDialog { }
}
namespace ProjectManager.Controls.AS3
{
    public class AS3PropertiesDialog : ProjectManager.Controls.PropertiesDialog { }
}

namespace ProjectManager.Helpers
{
    /// <summary>
    /// Can be extended at runtime by a FlashDevelop plugin, but not in the command line
    /// </summary>
    class ProjectCreator
    {
        private static Hashtable projectTypes = new Hashtable();
        private static bool projectTypesSet = false;

        private static void SetInitialProjectHash()
        {
            projectTypes["project.fdp"] = typeof(ProjectManager.Projects.AS2.AS2Project);
            projectTypes["project.as2proj"] = typeof(ProjectManager.Projects.AS2.AS2Project);
            projectTypes["project.as3proj"] = typeof(ProjectManager.Projects.AS3.AS3Project);
            projectTypes["project.hxproj"] = typeof(ProjectManager.Projects.Haxe.HaxeProject);
            projectTypes["project.fdproj"] = typeof(ProjectManager.Projects.Generic.GenericProject);
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
