using LintingHelper.Helpers;
using PluginCore;
using PluginCore.Managers;
using System.Collections.Generic;
using System.Linq;

namespace LintingHelper.Managers
{
    public static class LintingManager
    {
        static Dictionary<string, List<ILintProvider>> linters = new Dictionary<string, List<ILintProvider>>();

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
        public static void LintFiles(string[] files, string language)
        {
            language = language.ToLower();

            foreach (var linter in GetLinters(language))
            {
                ApplyLint(files, null); //removes cache
                linter.LintAsync(files, (results) =>
                {
                    ApplyLint(files, results);
                    EventManager.DispatchEvent(linter, new DataEvent(EventType.Command, "LintingManager.FilesLinted", files));
                });
            }
        }

        /// <summary>
        /// Lint <paramref name="files"/> trying to autodetect the language(s).
        /// </summary>
        public static void LintFiles(string[] files)
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
                LintFiles(filesByLang[lang].ToArray(), lang);
            }
        }

        public static void LintDocument(ITabbedDocument doc)
        {
            var files = new string[] { doc.FileName };
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

        /// <summary>
        /// Applies the results to all open files
        /// </summary>
        /// <param name="results"></param>
        static void ApplyLint(string[] files, List<LintingResult> results)
        {
            Cache.Clear(PluginBase.MainForm.Documents.Select( (d) => d.FileName ));

            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");

            foreach (var file in files)
            {
                Cache.RemoveDocument(file);
                DocumentManager.FindDocument(file)?.SciControl.RemoveHighlights();
            }

            if (results == null)
            {
                return;
            }

            Cache.AddResults(results);

            foreach (var result in results)
            {
                var line = result.File + ":" + result.Line + ": " + result.Severity.ToString() + ": " + result.Description;

                int state = 0;
                if (result.Severity == LintingSeverity.Warning)
                {
                    state = -3;
                }
                TraceManager.Add(new TraceItem(line, state));
                
                var doc = DocumentManager.FindDocument(result.File);
                if (doc != null)
                {
                    var start = doc.SciControl.PositionFromLine(result.Line - 1);
                    var len = doc.SciControl.LineLength(result.Line - 1);
                    start += result.FirstChar;
                    if (result.Length > 0)
                    {
                        len = result.Length;
                    }
                    else
                    {
                        len -= result.FirstChar;
                    }

                    var color = 0;
                    switch (result.Severity)
                    {
                        case LintingSeverity.Error:
                            color = 0x00000080;
                            break;
                        case LintingSeverity.Warning:
                            color = 0x00008080;
                            break;
                        case LintingSeverity.Info:
                            color = 0x00008000;
                            break;
                    }
                    doc.SciControl.AddHighlight((int)ScintillaNet.Enums.IndicatorStyle.Squiggle, color, start, len);
                }
            }
        }
    }
}
