using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.FRService;
using PluginCore.Utilities;

namespace HaXeContext.CodeRefactor.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    internal class HaxeCommandFactory : CommandFactory
    {
        public override Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            if (target.Member != null && (target.Member.Flags & FlagType.LocalVar) != 0)
            {
                var context = (HaXeContext.Context) ASContext.GetLanguageContext("haxe");
                if (((HaXeSettings) context.Settings).CompletionMode != HaxeCompletionModeEnum.FlashDevelop
                    && context.GetCurrentSDKVersion().IsGreaterThanOrEquals(new SemVer("3.2.0")))
                {
                    return new Commands.HaxeFindAllReferences(target, output, ignoreDeclarations)
                    {
                        OnlySourceFiles = onlySourceFiles
                    };
                }
            }
            return base.CreateFindAllReferencesCommand(target, output, ignoreDeclarations, onlySourceFiles);
        }
    }
}
