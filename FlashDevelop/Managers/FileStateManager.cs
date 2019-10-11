using System;
using System.Collections.Generic;
using System.IO;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    class FileStateManager
    {
        /// <summary>
        /// Saves the file state to a binary file
        /// </summary>
        public static void SaveFileState(ITabbedDocument document)
        {
            try
            {
                if (!document.IsEditable) return;
                string fileStateDir = FileNameHelper.FileStateDir;
                if (!Directory.Exists(fileStateDir)) Directory.CreateDirectory(fileStateDir);
                StateObject so = GetStateObject(document.SciControl);
                string fileName = ConvertToFileName(document.FileName);
                string stateFile = Path.Combine(fileStateDir, fileName + ".fdb");
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
        public static void ApplyFileState(ITabbedDocument document, bool restorePosition)
        {
            try
            {
                if (!document.IsEditable) return;
                string fileName = ConvertToFileName(document.FileName);
                string stateFile = Path.Combine(FileNameHelper.FileStateDir, fileName + ".fdb");
                if (File.Exists(stateFile))
                {
                    StateObject so = new StateObject();
                    so = (StateObject)ObjectSerializer.Deserialize(stateFile, so);
                    ApplyStateObject(document.SciControl, so, restorePosition);
                }
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
                string fileName = ConvertToFileName(file);
                string stateFile = Path.Combine(FileNameHelper.FileStateDir, fileName + ".fdb");
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
                DateTime timeNow = DateTime.Now;
                if (!Directory.Exists(FileNameHelper.FileStateDir)) return;
                string[] foundFiles = Directory.GetFiles(FileNameHelper.FileStateDir);
                foreach (string foundFile in foundFiles)
                {
                    TimeSpan twoWeeks = new TimeSpan(14, 0, 0, 0);
                    DateTime writeTime = File.GetLastWriteTime(foundFile);
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
        private static void ApplyStateObject(ScintillaControl sci, StateObject so, bool restorePosition)
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
                int line = sci.LineFromPosition(so.Position);
                sci.SetSel(so.Position, so.Position);
                sci.EnsureVisible(line);
            }
        }

        /// <summary>
        /// Gets the state object from a scintilla control
        /// </summary>
        private static StateObject GetStateObject(ScintillaControl sci)
        {
            StateObject so = new StateObject();
            so.LineCount = sci.LineCount;
            so.Position = sci.CurrentPos;
            so.FileName = sci.FileName;
            so.LineScroll = sci.FirstVisibleLine;
            for (int line = 0;; line++)
            {
                int lineNext = sci.ContractedFoldNext(line);
                if ((line < 0) || (lineNext < line)) break;
                line = lineNext;
                so.FoldedLines.Add(line);
            }
            int lineBookmark = -1;
            while ((lineBookmark = sci.MarkerNext(lineBookmark + 1, 1 << 0)) >= 0)
            {
                so.BookmarkedLines.Add(lineBookmark);
            }
            return so;
        }

        /// <summary>
        /// Converts a path to a valid file name
        /// </summary>
        private static string ConvertToFileName(string path)
        {
            return HashCalculator.CalculateSHA1(path);
        }

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

