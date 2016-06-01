using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;

namespace CodeRefactor.Provider
{
    public static class CommandFactoryProvider
    {
        static readonly Dictionary<string, ICommandFactory> LanguageToFactory = new Dictionary<string, ICommandFactory>();

        static CommandFactoryProvider()
        {
            Register("as2", new CommandFactory());
            Register("as3", new CommandFactory());
            Register("haxe", new CommandFactory());
            Register("loom", new CommandFactory());
        }

        public static void Register(string language, ICommandFactory factory)
        {
            LanguageToFactory.Add(language, factory);
        }

        public static bool ContainsLanguage(string language)
        {
            return LanguageToFactory.ContainsKey(language);
        }

        public static ICommandFactory GetFactoryFromCurrentDocument()
        {
            var document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return null;
            return GetFactoryFromDocument(document);
        }

        public static ICommandFactory GetFactoryFromDocument(ITabbedDocument document)
        {
            var language = document.SciControl.ConfigurationLanguage;
            return GetFactoryFromLanguage(language);
        }

        public static ICommandFactory GetFactoryFromTarget(ASResult target)
        {
            return GetFactoryFromFile(target.InFile);
        }

        public static ICommandFactory GetFactoryFromFile(FileModel file)
        {
            var language = PluginBase.MainForm.SciConfig.GetLanguageFromFile(file.FileName);
            return GetFactoryFromLanguage(language);
        }

        public static ICommandFactory GetFactoryFromLanguage(string language)
        {
            return LanguageToFactory[language];
        }
    }
}
