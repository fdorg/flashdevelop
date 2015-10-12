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
                String fileStateDir = FileNameHelper.FileStateDir;
                if (!Directory.Exists(fileStateDir)) Directory.CreateDirectory(fileStateDir);
                StateObject so = GetStateObject(document.SciControl);
                String fileName = ConvertToFileName(document.FileName);
                String stateFile = Path.Combine(fileStateDir, fileName + ".fdb");
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
        public static void ApplyFileState(ITabbedDocument document, Boolean restorePosition)
        {
            try
            {
                if (!document.IsEditable) return;
                String fileStateDir = FileNameHelper.FileStateDir;
                String fileName = ConvertToFileName(document.FileName);
                String stateFile = Path.Combine(fileStateDir, fileName + ".fdb");
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
        public static void RemoveStateFile(String file)
        {
            try
            {
                String fileName = ConvertToFileName(file);
                String fileStateDir = FileNameHelper.FileStateDir;
                String stateFile = Path.Combine(fileStateDir, fileName + ".fdb");
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
                String[] foundFiles = Directory.GetFiles(FileNameHelper.FileStateDir);
                foreach (String foundFile in foundFiles)
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
        private static void ApplyStateObject(ScintillaControl sci, StateObject so, Boolean restorePosition)
        {
            if (so.LineCount != sci.LineCount) return;
            sci.Refresh(); // Update the scintilla control state
            for (Int32 i = 0; i < so.FoldedLines.Count; i++)
            {
                Int32 foldedLine = so.FoldedLines[i];
                sci.ToggleFold(foldedLine);
            }
            if (so.BookmarkedLines != null)
            {
                for (Int32 i = 0; i < so.BookmarkedLines.Count; i++)
                {
                    Int32 bookmarkedLine = so.BookmarkedLines[i];
                    sci.MarkerAdd(bookmarkedLine, 0);
                }
                sci.Refresh(); // Update again
            }
            if (restorePosition)
            {
                Int32 line = sci.LineFromPosition(so.Position);
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
            for (Int32 line = 0;; line++)
            {
                Int32 lineNext = sci.ContractedFoldNext(line);
                if ((line < 0) || (lineNext < line)) break;
                line = lineNext;
                so.FoldedLines.Add(line);
            }
            Int32 lineBookmark = -1;
            while ((lineBookmark = sci.MarkerNext(lineBookmark + 1, 1 << 0)) >= 0)
            {
                so.BookmarkedLines.Add(lineBookmark);
            }
            return so;
        }

        /// <summary>
        /// Converts a path to a valid file name
        /// </summary>
        private static String ConvertToFileName(String path)
        {
            return HashCalculator.CalculateSHA1(path);
        }

    }

    [Serializable]
    public class StateObject
    {
        public Int32 Position = 0;
        public Int32 LineCount = 0;
        public String FileName = String.Empty;
        public List<Int32> BookmarkedLines = new List<Int32>();
        public List<Int32> FoldedLines = new List<Int32>();
    }

}

