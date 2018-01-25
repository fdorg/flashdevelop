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
        public override Command CreateOrganizeImportsCommand() => new Commands.HaxeOrganizeImports();

        public override Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            var context = (Context)ASContext.GetLanguageContext("haxe");
            var settings = (HaXeSettings)context.Settings;
            if ((settings.EnabledFeatures & CompletionFeatures.EnableForFindAllReferences) == CompletionFeatures.EnableForFindAllReferences
                && settings.CompletionMode != HaxeCompletionModeEnum.FlashDevelop
                && target.Member != null && ((target.Member.Flags & FlagType.LocalVar) > 0 || (target.Member.Flags & FlagType.ParameterVar) > 0)
                && context.GetCurrentSDKVersion() >= "3.2.0")
            {
                return new Commands.HaxeFindAllReferences(target, output, ignoreDeclarations)
                {
                    OnlySourceFiles = onlySourceFiles
                };
            }
            return base.CreateFindAllReferencesCommand(target, output, ignoreDeclarations, onlySourceFiles);
        }
    }
}
