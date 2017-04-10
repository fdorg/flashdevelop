using LintingHelper.Helpers;
using PluginCore;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LintingHelper
{
    class LintingCache
    {
        Dictionary<string, List<LintingResult>> results;

        public LintingCache()
        {
            results = new Dictionary<string, List<LintingResult>>();
        }

        public List<LintingResult> GetResultsFromPosition(ITabbedDocument document, int position)
        {
            List<LintingResult> list;
            if (results.TryGetValue(document.FileName, out list))
            {
                var line = document.SciControl.LineFromPosition(position);

                var localResults = new List<LintingResult>();
                foreach (var result in list)
                {
                    var start = document.SciControl.PositionFromLine(result.Line - 1);
                    var len = document.SciControl.LineLength(result.Line - 1);
                    start += result.FirstChar;
                    if (result.Length > 0)
                    {
                        len = result.Length;
                    }
                    else
                    {
                        len -= result.FirstChar;
                    }
                    var end = start + len;
                    
                    if (start <= position && (end >= position || result.Length < 0 && result.Line == line + 1))
                    {
                        //suitable result
                        localResults.Add(result);
                    }
                }

                return localResults;
            }

            return null;
        }

        /// <summary>
        /// Adds the given <paramref name="results"/>.
        /// Uses DocumentManager to look up the documents.
        /// Files that are not opened are 
        /// </summary>
        /// <param name="results"></param>
        public void AddResults(List<LintingResult> results)
        {
            foreach (var result in results)
            {
                AddResult(result.File, result);
            }
        }

        private void AddResult(string document, LintingResult result)
        {
            var list = results.GetOrCreate(document);
            if (list.Find(result.Equals) == null)
                list.Add(result);
        }

        public void RemoveDocument(string document)
        {
            if (results.ContainsKey(document))
            {
                results.Remove(document);
            }
        }

        /// <summary>
        /// Removes documents that are not opened anymore
        /// </summary>
        /// <param name="documents">The documents that are currently opened</param>
        public void RemoveAllExcept(IEnumerable<string> documents)
        {
            var copy = new List<string>(results.Keys);
            foreach (var doc in copy)
            {
                if (!documents.Contains(doc))
                {
                    results.Remove(doc);
                }
            }
        }

    }
}
