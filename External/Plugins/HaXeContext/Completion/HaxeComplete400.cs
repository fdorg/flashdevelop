using System.Xml;
using ASCompletion.Completion;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext
{
    class HaxeComplete400 : HaxeComplete330
    {
        public HaxeComplete400(ScintillaControl sci, ASExpr expr, bool autoHide, IHaxeCompletionHandler completionHandler, HaxeCompilerService compilerService, SemVer haxeVersion) : base(sci, expr, autoHide, completionHandler, compilerService, haxeVersion)
        {
        }

        protected override int GetDisplayPosition()
        {
            var result = base.GetDisplayPosition();
            result = Sci.MBSafeCharPosition(result);
            return result;
        }

        protected override HaxePositionResult ExtractPos(XmlReader reader)
        {
            var result = base.ExtractPos(reader);
            if (result.RangeType == HaxePositionCompleteRangeType.CHARACTERS)
            {
                result.CharacterStart = result.CharacterStart - 1;
                result.CharacterEnd = result.CharacterEnd - 1;
            }
            return result;
        }
    }
}