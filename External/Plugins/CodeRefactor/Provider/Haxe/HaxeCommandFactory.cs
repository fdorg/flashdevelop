using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Commands;
using CodeRefactor.Commands.Haxe;
using HaXeContext;
using PluginCore.FRService;
using PluginCore.Utilities;

namespace CodeRefactor.Provider.Haxe
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    class HaxeCommandFactory : CommandFactory
    {
        public override Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            var context = (Context)ASContext.GetLanguageContext("haxe");
            if (context.GetCurrentSDKVersion().IsOlderThan(new SemVer("3.2.0")))
                return base.CreateFindAllReferencesCommand(target, output, ignoreDeclarations, onlySourceFiles);
            return new HaxeFindAllReferencesCommand(target, output, ignoreDeclarations) {OnlySourceFiles = onlySourceFiles};
        }
    }
}
