using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
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
            Register("as2", DefaultFactory);
            Register("as3", DefaultFactory);
            Register("haxe", DefaultFactory);
            Register("loom", DefaultFactory);
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

        public static ICommandFactory GetFactory(ASResult target)
        {
            return GetFactory(target.InFile ?? target.Type.InFile);
        }

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
