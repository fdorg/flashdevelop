using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ProjectManager.Projects.AS2
{
    public enum TraceMode
    {
        Disable,
        FlashMX,
        FlashViewer,
        FlashViewerExtended,
        FlashConnect,
        FlashConnectExtended,
        CustomFunction
    }

    [Serializable]
    public class MtascOptions : CompilerOptions
    {
        int frame = 1;
        bool useMX = true;
        bool infer = false;
        bool strict = false;
        bool useMain = true;
        bool verbose = false;
        bool keep = true;
        bool groupClasses = false;
        bool warnUnusedImports = false;
        TraceMode traceMode = TraceMode.FlashConnectExtended;
        string traceFunction = "";
        string libraryPrefix = "";
        string[] includePackages = new string[] { };
        string excludeFile = "";

        [DisplayName("Strict Mode")]
        [LocalizedCategory("ProjectManager.Category.General")]
        [LocalizedDescription("ProjectManager.Description.Strict")]
        [DefaultValue(false)]
        public bool Strict { get { return strict; } set { strict = value; } }

        [DisplayName("Infer Types")]
        [LocalizedCategory("ProjectManager.Category.General")]
        [LocalizedDescription("ProjectManager.Description.Infer")]
        [DefaultValue(false)]
        public bool Infer { get { return infer; } set { infer = value; } }

        [DisplayName("Verbose Output")]
        [LocalizedCategory("ProjectManager.Category.General")]
        [LocalizedDescription("ProjectManager.Description.Verbose")]
        [DefaultValue(false)]
        public bool Verbose { get { return verbose; } set { verbose = value; } }

        [DisplayName("Warn on Unused Imports")]
        [LocalizedCategory("ProjectManager.Category.General")]
        [LocalizedDescription("ProjectManager.Description.WarnUnusedImports")]
        [DefaultValue(false)]
        public bool WarnUnusedImports
        {
            get { return warnUnusedImports; }
            set { warnUnusedImports = value; }
        }

        [DisplayName("Trace Mode")]
        [LocalizedCategory("ProjectManager.Category.Trace")]
        [LocalizedDescription("ProjectManager.Description.TraceMode")]
        [DefaultValue(TraceMode.FlashConnectExtended)]
        public TraceMode TraceMode
        {
            get { return traceMode; }
            set { traceMode = value; }
        }

        [DisplayName("Custom Trace Function")]
        [LocalizedCategory("ProjectManager.Category.Trace")]
        [LocalizedDescription("ProjectManager.Description.TraceFunction")]
        [DefaultValue("")]
        public string TraceFunction
        {
            get { return (traceMode == TraceMode.CustomFunction) ? traceFunction : ""; }
            set { traceFunction = value; }
        }

        [DisplayName("Library Prefix")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.LibraryPrefix")]
        [DefaultValue("")]
        public string LibraryPrefix
        {
            get { return libraryPrefix; }
            set { libraryPrefix = value; }
        }

        [DisplayName("Use Main Entry Point")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.UseMain")]
        [DefaultValue(true)]
        public bool UseMain { get { return useMain; } set { useMain = value; } }

        [DisplayName("Include Packages")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.IncludePackages")]
        [DefaultValue(new string[]{})]
        public string[] IncludePackages
        {
            get { return includePackages; }
            set { includePackages = value; }
        }

        [DisplayName("Excludes File")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.ExcludeFile")]
        [DefaultValue("")]
        public string ExcludeFile
        {
            get { return excludeFile; }
            set { excludeFile = value; }
        }

        [DisplayName("Group Classes")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.GroupClasses")]
        [DefaultValue(false)]
        public bool GroupClasses
        {
            get { return groupClasses; }
            set { groupClasses = value; }
        }

        [DisplayName("Use MX Classes")]
        [LocalizedCategory("ProjectManager.Category.Advanced")]
        [LocalizedDescription("ProjectManager.Description.UseMX")]
        [DefaultValue(true)]
        public bool UseMX { get { return useMX; } set { useMX = value; } }

        [DisplayName("Injection Frame")]
        [LocalizedCategory("ProjectManager.Category.CodeInjection")]
        [LocalizedDescription("ProjectManager.Description.Frame")]
        [DefaultValue(1)]
        public int Frame
        {
            get { return frame; }
            set
            {
                if (frame < 1) throw new ArgumentException("The value for Frame must be greater than zero.");
                frame = value;
            }
        }

        [DisplayName("Keep Classes in SWF")]
        [LocalizedCategory("ProjectManager.Category.CodeInjection")]
        [LocalizedDescription("ProjectManager.Description.Keep")]
        [DefaultValue(true)]
        public bool Keep
        {
            get { return keep; }
            set { keep = value; }
        }
    }
}
