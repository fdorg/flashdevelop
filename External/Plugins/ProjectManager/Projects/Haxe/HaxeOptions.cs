using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ProjectManager.Projects.Haxe
{
    [Serializable]
    public class HaxeOptions : CompilerOptions
    {
        string[] directives = new string[] { };
        string mainClass = "";
        bool flashStrict = false;
        bool enableDebug = false;
        bool noInlineOnDebug = false;
        string[] additional = new string[] { };
        string[] libraries = new string[] { };

        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Additional Compiler Options")]
        [LocalizedDescription("ProjectManager.Description.Additional")]
        [DefaultValue(new string[] { })]
        public string[] Additional { get { return additional; } set { additional = value; } }

        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Enable Debugger")]
        [LocalizedDescription("ProjectManager.Description.EnableDebug")]
        [DefaultValue(false)]
        public bool EnableDebug { get { return enableDebug; } set { enableDebug = value; } }

        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Flash Strict")]
        [LocalizedDescription("ProjectManager.Description.FlashStrict")]
        [DefaultValue(false)]
        public bool FlashStrict { get { return flashStrict; } set { flashStrict = value; } }

        [LocalizedCategory("ProjectManager.Category.CompilerOptions")]
        [DisplayName("Set No-Inline On Debug")]
        [LocalizedDescription("ProjectManager.Description.NoInlineOnDebug")]
        [DefaultValue(false)]
        public bool NoInlineOnDebug { get { return noInlineOnDebug; } set { noInlineOnDebug = value; } }

        [LocalizedCategory("ProjectManager.Category.General")]
        [DisplayName("Directives")]
        [LocalizedDescription("ProjectManager.Description.Directives")]
        [DefaultValue("")]
        public string[] Directives { get { return directives; } set { directives = value; } }

        [DisplayName("Libraries")]
        [LocalizedCategory("ProjectManager.Category.General")]
        [LocalizedDescription("ProjectManager.Description.HaXeLibraries")]
        [DefaultValue(new string[] { })]
        public string[] Libraries
        {
            get { return libraries; }
            set { libraries = value; }
        }

        [LocalizedCategory("ProjectManager.Category.General")]
        [DisplayName("Main Class")]
        [LocalizedDescription("ProjectManager.Description.MainClass")]
        [DefaultValue("")]
        public string MainClass { get { return mainClass; } set { mainClass = value; } }

    }
}
