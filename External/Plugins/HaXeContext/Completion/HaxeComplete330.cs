using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext
{
    class HaxeComplete330 : HaxeComplete
    {
        public HaxeComplete330(ScintillaControl sci, ASExpr expr, bool autoHide, IHaxeCompletionHandler completionHandler, HaxeCompilerService compilerService, SemVer haxeVersion) : base(sci, expr, autoHide, completionHandler, compilerService, haxeVersion)
        {
        }

        protected override void SaveFile()
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if(document.FileName != Sci.FileName && document.IsModified) document.Save();
            }
        }

        protected override string[] BuildHxmlArgs()
        {
            var args = base.BuildHxmlArgs();
            if (args == null) return null;
            var settings = (ASContext.GetLanguageContext("haxe") as Context)?.Settings as HaXeSettings;
            if (settings == null || (settings.EnabledFeatures & CompletionFeatures.DisplayStdIn) == 0) return args;
            var list = new List<string>(args) {"-D display-stdin"};
            var result = list.ToArray();
            return result;
        }

        protected override string GetFileContent() => Sci.Text;
    }
}