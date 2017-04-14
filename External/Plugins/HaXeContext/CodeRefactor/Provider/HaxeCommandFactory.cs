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
            if (target.Member != null && (target.Member.Flags & (FlagType.LocalVar | FlagType.ParameterVar)) != 0)
            {
                var context = (Context) ASContext.GetLanguageContext("haxe");
                var settings = (HaXeSettings) context.Settings;
                if (settings.CompletionMode != HaxeCompletionModeEnum.FlashDevelop
                    && context.GetCurrentSDKVersion().IsGreaterThanOrEquals(new SemVer("3.2.0")) && settings.EnableCompilerServices)
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
