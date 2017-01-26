using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.FRService;

namespace HaXeContext.CodeRefactor.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    internal class HaxeCommandFactory : CommandFactory
    {
        public override Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            if (target.Member != null && (target.Member.Flags & FlagType.LocalVar) != 0)
            {
                var settings = (HaXeSettings) ASContext.GetLanguageContext("haxe").Settings;
                if (settings.CompletionMode != HaxeCompletionModeEnum.FlashDevelop)
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
