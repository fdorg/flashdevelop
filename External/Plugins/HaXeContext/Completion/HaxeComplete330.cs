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

        protected override List<string> BuildHxmlArgs()
        {
            var result = base.BuildHxmlArgs();
            if (result == null) return null;
            if ((ASContext.GetLanguageContext("haxe") as Context)?.Settings is HaXeSettings settings
                && (settings.EnabledFeatures & CompletionFeatures.DisplayStdIn) != 0
                && settings.CompletionMode == HaxeCompletionModeEnum.CompletionServer)
                result.Add("-D display-stdin");
            return result;
        }

        protected override string GetFileContent() => Sci.Text;
    }
}