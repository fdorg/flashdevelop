using System.Collections.Generic;
using ASCompletion.Completion;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore.Managers;
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
        }

        protected override string[] BuildHxmlArgs()
        {
            var args = base.BuildHxmlArgs();
            if (args == null) return null;
            var list = new List<string>(args) {"-D display-stdin"};
            var result = list.ToArray();
            return result;
        }

        protected override string GetStdin()
        {
            return Sci.Text;
        }
    }
}