using System;
using System.Collections.Generic;
using System.IO;
using FlashDevelop.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    internal class FileStateManager
    {
        /// <summary>
        /// Saves the file state to a binary file
        /// </summary>
        public static void SaveFileState(ScintillaControl sci)
        {
            if (sci is null) return;
            try
            {
                var fileStateDir = FileNameHelper.FileStateDir;
                if (!Directory.Exists(fileStateDir)) Directory.CreateDirectory(fileStateDir);
                var so = GetStateObject(sci);
                var fileName = ConvertToFileName(sci.FileName);
                var stateFile = Path.Combine(fileStateDir, fileName + ".fdb");
                ObjectSerializer.Serialize(stateFile, so);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Applies the file state to a scintilla control
        /// </summary>
        public static void ApplyFileState(ScintillaControl sci, bool restorePosition)
        {
            if (sci is null) return;
            try
            {
                var fileName = ConvertToFileName(sci.FileName);
                var stateFile = Path.Combine(FileNameHelper.FileStateDir, fileName + ".fdb");
                if (!File.Exists(stateFile)) return;
                var so = new StateObject();
                so = ObjectSerializer.Deserialize(stateFile, so);
                ApplyStateObject(sci, so, restorePosition);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Removes old state file from disk
        /// </summary>
        public static void RemoveStateFile(string file)
        {
            try
            {
                var fileName = ConvertToFileName(file);
                var stateFile = Path.Combine(FileNameHelper.FileStateDir, fileName + ".fdb");
                if (File.Exists(stateFile)) File.Delete(stateFile);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Removes old state files
        /// </summary>
        public static void RemoveOldStateFiles()
        {
            try
            {
                if (!Directory.Exists(FileNameHelper.FileStateDir)) return;
                var timeNow = DateTime.Now;
                var foundFiles = Directory.GetFiles(FileNameHelper.FileStateDir);
                foreach (var foundFile in foundFiles)
                {
                    var twoWeeks = new TimeSpan(14, 0, 0, 0);
                    var writeTime = File.GetLastWriteTime(foundFile);
                    if (timeNow.CompareTo(writeTime.Add(twoWeeks)) > 0)
                    {
                        File.Delete(foundFile);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Applies the state object to a scintilla control
        /// </summary>
        static void ApplyStateObject(ScintillaControl sci, StateObject so, bool restorePosition)
        {
            if (so.LineCount != sci.LineCount) return;
            sci.Refresh(); // Update the scintilla control state
            foreach (var foldedLine in so.FoldedLines)
            {
                sci.ToggleFold(foldedLine);
            }
            if (so.BookmarkedLines != null)
            {
                foreach (var bookmarkedLine in so.BookmarkedLines)
                {
                    sci.MarkerAdd(bookmarkedLine, 0);
                }
                sci.Refresh(); // Update again
            }
            if (restorePosition)
            {
                sci.FirstVisibleLine = so.LineScroll;
                var line = sci.LineFromPosition(so.Position);
                sci.SetSel(so.Position, so.Position);
                sci.EnsureVisible(line);
            }
        }

        /// <summary>
        /// Gets the state object from a scintilla control
        /// </summary>
        static StateObject GetStateObject(ScintillaControl sci)
        {
            var so = new StateObject
            {
                LineCount = sci.LineCount,
                Position = sci.CurrentPos,
                FileName = sci.FileName,
                LineScroll = sci.FirstVisibleLine
            };
            for (var line = 0;; line++)
            {
                var lineNext = sci.ContractedFoldNext(line);
                if (line < 0 || lineNext < line) break;
                line = lineNext;
                so.FoldedLines.Add(line);
            }
            var lineBookmark = -1;
            while ((lineBookmark = sci.MarkerNext(lineBookmark + 1, 1)) >= 0)
            {
                so.BookmarkedLines.Add(lineBookmark);
            }
            return so;
        }

        /// <summary>
        /// Converts a path to a valid file name
        /// </summary>
        static string ConvertToFileName(string path) => HashCalculator.CalculateSHA1(path);
    }

    [Serializable]
    public class StateObject
    {
        public int Position = 0;
        public int LineCount = 0;
        public int LineScroll = 0;
        public string FileName = string.Empty;
        public List<int> BookmarkedLines = new List<int>();
        public List<int> FoldedLines = new List<int>();
    }
}