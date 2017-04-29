using LintingHelper.Helpers;
using PluginCore;
using PluginCore.Managers;
using System.Collections.Generic;
using System.Linq;
using PluginCore.Localization;

namespace LintingHelper.Managers
{
    public static class LintingManager
    {
        const string TraceGroup = "LintingManager";

        static readonly Dictionary<string, List<ILintProvider>> linters = new Dictionary<string, List<ILintProvider>>();

        static readonly Dictionary<LintingSeverity, int> severityMap = new Dictionary<LintingSeverity, int>
        {
            {LintingSeverity.Info, (int) TraceType.Info},
            {LintingSeverity.Error, (int) TraceType.Error},
            {LintingSeverity.Warning, (int) TraceType.Warning}
        };

        static LintingManager()
        {
            TraceManager.RegisterTraceGroup(TraceGroup, TextHelper.GetStringWithoutMnemonics("LintingManager.Label.LintingResults"), null);
        }

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
                //remove cache
                foreach (var file in files)
                {
                    UnLintDocument(DocumentManager.FindDocument(file));
                }
                linter.LintAsync(files, (results) =>
                {
                    ApplyLint(files, language, results);
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

        public static void UnLintDocument(ITabbedDocument doc)
        {
            Cache.RemoveDocument(doc.FileName);
            doc.SciControl.RemoveHighlights();
        }

        /// <summary>
        /// Applies the results to all open files
        /// </summary>
        static void ApplyLint(string[] files, string language, List<LintingResult> results)
        {
            if (results == null)
                return;

            var fileList = new List<string>(files);
            fileList.AddRange(PluginBase.MainForm.Documents.Select(d => d.FileName));
            Cache.RemoveAllExcept(fileList);

            PluginBase.RunAsync(() =>
            {
                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + TraceGroup);
            });

            Cache.AddResults(results);

            
            foreach (var result in Cache.GetAllResults())
            {
                TraceResult(result);

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

                    var id = 0;
                    int color = 0;
                    var lang = PluginBase.MainForm.SciConfig.GetLanguage(language);
                    switch (result.Severity)
                    {
                        case LintingSeverity.Error:
                            color = lang.editorstyle.ErrorLineBack;
                            id = 3;
                            break;
                        case LintingSeverity.Warning:
                            color = lang.editorstyle.DebugLineBack;
                            id = 4;
                            break;
                        case LintingSeverity.Info:
                            color = lang.editorstyle.HighlightWordBackColor;
                            id = 5;
                            break;
                    }

                    PluginBase.RunAsync(() =>
                    {
                        doc.SciControl.AddHighlight(id, (int)ScintillaNet.Enums.IndicatorStyle.Squiggle, color, start, len);
                    });
                }
            }

            PluginBase.RunAsync(() =>
            {
                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + TraceGroup);
            });
        }

        static void TraceResult(LintingResult result)
        {
            var line = result.File + ":" + result.Line + ": " + result.Severity.ToString() + ": " + result.Description;

            TraceManager.Add(line, severityMap[result.Severity], TraceGroup);
        }
    }
}
