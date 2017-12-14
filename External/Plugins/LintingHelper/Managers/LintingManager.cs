using System.Collections.Generic;
using LintingHelper.Helpers;
using PluginCore;
using PluginCore.Managers;

namespace LintingHelper.Managers
{
    public static class LintingManager
    {
        internal const string TraceGroup = "LintingManager";

        static readonly Dictionary<string, List<ILintProvider>> linters = new Dictionary<string, List<ILintProvider>>();
        internal static LintingCache Cache = new LintingCache();

        /// <summary>
        /// Registers a new linter for the specified language. There can be more than one linter
        /// </summary>
        /// <param name="language">the language to use the linter for (cAsE is ignored)</param>
        /// <param name="provider">the linter</param>
        public static void RegisterLinter(string language, ILintProvider provider)
        {
            language = language.ToLower();

            var list = linters.GetOrCreate(language);

            if (!list.Contains(provider))
            {
                list.Add(provider);
            }

            EventManager.DispatchEvent(provider, new DataEvent(EventType.Command, "LintingManager.LinterRegistered", language));
        }

        /// <summary>
        /// Unregisters a linter from the specified language.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="provider"></param>
        public static void UnregisterLinter(string language, ILintProvider provider)
        {
            language = language.ToLower();

            if (linters.ContainsKey(language))
            {
                linters[language].Remove(provider);
            }

            EventManager.DispatchEvent(provider, new DataEvent(EventType.Command, "LintingManager.LinterUnregistered", language));
        }

        /// <summary>
        /// Checks whether there is at least one linter for the given <paramref name="language"/>.
        /// </summary>
        public static bool HasLanguage(string language)
        {
            language = language.ToLower();

            return linters.ContainsKey(language) && linters[language].Count > 0;
        }

        /// <summary>
        /// Gets the linters for a given <paramref name="language"/>.
        /// </summary>
        public static List<ILintProvider> GetLinters(string language)
        {
            return linters.GetOrCreate(language);
        }

        /// <summary>
        /// Lint <paramref name="files"/> without trying to autodetect the language
        /// </summary>
        /// <param name="files">the files to lint</param>
        /// <param name="language">the language to use</param>
        public static void LintFiles(IEnumerable<string> files, string language)
        {
            language = language.ToLower();

            foreach (var linter in GetLinters(language))
            {
                linter.LintAsync(files, results => PluginBase.RunAsync(() =>
                {
                    ApplyLint(results);
                    EventManager.DispatchEvent(linter, new DataEvent(EventType.Command, "LintingManager.FilesLinted", files));
                }));
            }
        }

        /// <summary>
        /// Lint <paramref name="files"/> trying to autodetect the language(s).
        /// </summary>
        public static void LintFiles(IEnumerable<string> files)
        {
            //detect languages
            var filesByLang = new Dictionary<string, List<string>>();
            foreach (var file in files)
            {
                var lang = PluginBase.MainForm.SciConfig.GetLanguageFromFile(file).ToLower();
                var list = filesByLang.GetOrCreate(lang);

                list.Add(file);
            }
            
            foreach (var lang in filesByLang.Keys)
            {
                LintFiles(filesByLang[lang], lang);
            }
        }

        /// <summary>
        /// Lint the whole <paramref name="project"/>
        /// </summary>
        public static void LintProject(IProject project)
        {
            var language = project.Language.ToLower();

            //remove cache
            Cache.RemoveAll();

            foreach (var linter in GetLinters(language))
            {
                linter.LintProjectAsync(project, results => PluginBase.RunAsync(() =>
                {
                    ApplyLint(results);
                    EventManager.DispatchEvent(linter, new DataEvent(EventType.Command, "LintingManager.ProjectLinted", null));
                }));
            }
        }

        public static void LintDocument(ITabbedDocument doc)
        {
            var files = new [] { doc.FileName };
            var language = doc.SciControl.ConfigurationLanguage;

            LintFiles(files, language);
        }

        /// <summary>
        /// Runs all applicable linters on the current document
        /// </summary>
        public static void LintCurrentDocument()
        {
            LintDocument(PluginBase.MainForm.CurrentDocument);
        }

        public static void UnLintFile(string file)
        {
            Cache.RemoveDocument(file);
            UpdateLinterPanel();
        }

        public static void UnLintDocument(ITabbedDocument doc)
        {
            Cache.RemoveDocument(doc.FileName);
            UpdateLinterPanel();
        }

        /// <summary>
        /// Applies the results to all open files
        /// </summary>
        static void ApplyLint(List<LintingResult> results)
        {
            if (results == null)
                return;

            Cache.AddResults(results);

            UpdateLinterPanel();
        }

        internal static void UpdateLinterPanel()
        {
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + TraceGroup);

            var cachedResults = Cache.GetAllResults();
            foreach (var result in cachedResults)
            {
                string chars;
                if (result.Length > 0)
                {
                    chars = $"chars {result.FirstChar}-{result.FirstChar + result.Length}";
                }
                else
                {
                    var sci = DocumentManager.FindDocument(result.File)?.SciControl;
                    if (sci != null)
                    {
                        chars = $"chars {result.FirstChar}-{sci.LineLength(result.Line - 1)}";
                    }
                    else
                    {
                        chars = $"char {result.FirstChar}";
                    }
                }
                string message = $"{result.File}:{result.Line}: {chars} : {result.Severity}: {result.Description}";
                int state;
                switch (result.Severity)
                {
                    case LintingSeverity.Info:
                        state = (int)TraceType.Info;
                        break;
                    case LintingSeverity.Warning:
                        state = (int)TraceType.Warning;
                        break;
                    case LintingSeverity.Error:
                        state = (int)TraceType.Error;
                        break;
                    default:
                        continue;
                }
                TraceManager.Add(message, state, TraceGroup);
            }
        }
    }
}
