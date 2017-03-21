using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;

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
            Register("loom", DefaultFactory);
            Register("haxe", DefaultFactory);
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
            return GetFactoryFromFile(target.InFile ?? target.Type.InFile);
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
