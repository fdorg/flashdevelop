using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Provider
{
    public static class CommandFactoryProvider
    {
        public static readonly ICommandFactory DefaultFactory = new CommandFactory();
        static readonly Dictionary<string, ICommandFactory> LanguageToFactory = new Dictionary<string, ICommandFactory>();

        static CommandFactoryProvider()
        {
            RegisterValidators();
            Register("as2", DefaultFactory);
            Register("as3", DefaultFactory);
            Register("haxe", DefaultFactory);
            Register("loom", DefaultFactory);
        }

        static void RegisterValidators()
        {
            DefaultFactory.RegisterValidator(typeof(Rename), expr =>
            {
                if (expr == null || expr.IsNull()) return false;
                var c = expr.Context.Value[0];
                if (char.IsDigit(c)) return false;
                var file = expr.InFile ?? expr.Type.InFile;
                var language = PluginBase.MainForm.SciConfig.GetLanguageFromFile(file.FileName);
                var characterClass = ScintillaControl.Configuration.GetLanguage(language).characterclass.Characters;
                if (!characterClass.Contains(c)) return false;
                return (expr.Member != null && RefactoringHelper.ModelFileExists(expr.Member.InFile) && !RefactoringHelper.IsUnderSDKPath(expr.Member.InFile))
                    || (expr.Type != null && RefactoringHelper.ModelFileExists(expr.Type.InFile) && !RefactoringHelper.IsUnderSDKPath(expr.Type.InFile))
                    || (RefactoringHelper.ModelFileExists(expr.InFile) && !RefactoringHelper.IsUnderSDKPath(expr.InFile))
                    || expr.IsPackage;
            });
            DefaultFactory.RegisterValidator(typeof(OrganizeImports), expr => expr.InFile.Imports.Count > 0);
            DefaultFactory.RegisterValidator(typeof(DelegateMethods), expr => expr != null && !expr.IsNull() && expr.InFile != null && expr.InClass != null
                                                                              && expr.Type is ClassModel type && !type.IsVoid()
                                                                              && expr.Member is MemberModel member && member.Flags is FlagType flags
                                                                              && flags.HasFlag(FlagType.Variable)
                                                                              && !flags.HasFlag(FlagType.LocalVar) && !flags.HasFlag(FlagType.ParameterVar)
                                                                              && expr.Type != ASContext.Context.CurrentClass);
        }

        public static void Register(string language, ICommandFactory factory)
        {
            if (ContainsLanguage(language)) LanguageToFactory.Remove(language);
            LanguageToFactory.Add(language, factory);
        }

        public static bool ContainsLanguage(string language)
        {
            return LanguageToFactory.ContainsKey(language);
        }

        public static ICommandFactory GetFactoryForCurrentDocument()
        {
            var document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return null;
            return GetFactory(document);
        }

        public static ICommandFactory GetFactory(ASResult target) => GetFactory(target.InFile ?? target.Type.InFile);

        public static ICommandFactory GetFactory(FileModel file)
        {
            var language = PluginBase.MainForm.SciConfig.GetLanguageFromFile(file.FileName);
            return GetFactory(language);
        }

        public static ICommandFactory GetFactory(ITabbedDocument document) => GetFactory(document.SciControl);

        public static ICommandFactory GetFactory(ScintillaControl sci) => GetFactory(sci.ConfigurationLanguage);

        public static ICommandFactory GetFactory(string language)
        {
            ICommandFactory factory;
            LanguageToFactory.TryGetValue(language, out factory);
            return factory;
        }
    }
}
