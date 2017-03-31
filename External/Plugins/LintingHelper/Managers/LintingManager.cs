using LintingHelper.Helpers;
using PluginCore;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LintingHelper.Managers
{
    public class LintingManager
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

            var list = DictionaryHelper.GetOrCreate(linters, language);

            //TODO: maybe check whether it already is in there
            list.Add(provider);
        }

        /// <summary>
        /// Unregisters a linter from the specified language.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="provider"></param>
        public static void UnregisterLinter(string language, ILintProvider provider)
        {
            if (linters.ContainsKey(language))
            {
                linters[language].Remove(provider);
            }
        }

        /// <summary>
        /// Gets the linters for a given <paramref name="language"/>.
        /// </summary>
        public static List<ILintProvider> GetLinters(string language)
        {
            if (linters.ContainsKey(language))
            {
                return linters[language];
            }
            else
            {
                return linters[language] = new List<ILintProvider>();
            }
        }

        /// <summary>
        /// Lint <paramref name="files"/> without trying to autodetect the language
        /// </summary>
        /// <param name="files">the files to lint</param>
        /// <param name="language">the language to use</param>
        public static void LintFiles(string[] files, string language)
        {
            language = language.ToLower();
            //PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults"); //TODO: fix this
            foreach (var linter in GetLinters(language))
            {
                var results = linter.DoLint(files);
                
                ApplyLint(results);
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
                var list = DictionaryHelper.GetOrCreate(filesByLang, lang);

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
        static void ApplyLint(List<LintingResult> results)
        {
            Cache.Clear(PluginBase.MainForm.Documents.Select(d => d.FileName));

            if (results == null)
            {
                return;
            }

            Cache.AddResults(results);

            foreach (var result in results)
            {
                TraceManager.Add(result.Line + ":" + result.Description);
                var doc = DocumentManager.FindDocument(result.File);
                var start = doc.SciControl.PositionFromLine(result.Line-1);
                var len = doc.SciControl.LineLength(result.Line-1);
                start += result.FirstChar;
                if (result.Length > 0)
                {
                    len = result.Length;
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
                //doc.SciControl.CallTipShow(start, "Test");
                doc.SciControl.AddHighlight((int)ScintillaNet.Enums.IndicatorStyle.Squiggle, color, start, len);
            }
        }
    }
}
