using System.Collections.Generic;
using System.IO;
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
        public HaxeCommandFactory()
        {
            RegisterValidator(typeof(OrganizeImports), expr =>
            {
                var inFile = expr.InFile;
                return Path.GetFileName(inFile.FileName) != "import.hx" && inFile.Imports.Count > 0;
            });
            RegisterValidator(typeof(DelegateMethods), expr =>
            {
                var validator = CommandFactoryProvider.DefaultFactory.GetValidator(typeof(DelegateMethods));
                return validator != null && validator(expr)
                    && !expr.InClass.Flags.HasFlag(FlagType.Interface)
                    && !expr.InClass.Flags.HasFlag(FlagType.TypeDef);
            });
        }

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
