// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using FlashDevelop.Utilities;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    internal class SnippetManager
    {
        /// <summary>
        /// Gets a snippet from a file in the snippets directory
        /// </summary>
        public static string GetSnippet(string word, string syntax, Encoding current)
        {
            var specific = Path.Combine(PathHelper.SnippetDir, syntax, word + ".fds");
            if (File.Exists(specific))
            {
                var info = FileHelper.GetEncodingFileInfo(specific);
                return DataConverter.ChangeEncoding(info.Contents, info.CodePage, current.CodePage);
            }
            var global = Path.Combine(PathHelper.SnippetDir, word + ".fds");
            if (File.Exists(global))
            {
                var info = FileHelper.GetEncodingFileInfo(global);
                return DataConverter.ChangeEncoding(info.Contents, info.CodePage, current.CodePage);
            }
            return null;
        }

        /// <summary>
        /// Inserts text from the snippets class
        /// </summary>
        public static bool InsertTextByWord(string word)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return false;
            var canShowList = false; 
            string snippet = null;
            if (word is null)
            {
                canShowList = true;
                word = sci.GetWordFromPosition(sci.CurrentPos);
            }
            if (!string.IsNullOrEmpty(word)) snippet = GetSnippet(word, sci.ConfigurationLanguage, sci.Encoding);
            // let plugins handle the snippet
            var data = new Hashtable {["word"] = word, ["snippet"] = snippet};
            var de = new DataEvent(EventType.Command, "SnippetManager.Expand", data);
            EventManager.DispatchEvent(PluginBase.MainForm, de);
            if (de.Handled) return true;
            snippet = (string)data["snippet"];
            if (!string.IsNullOrEmpty(sci.SelText))
            {
                // Remember the previous selection
                ArgsProcessor.PrevSelText = sci.SelText;
            }
            if (snippet != null)
            {
                int endPos = sci.SelectionEnd;
                int startPos = sci.SelectionStart;
                string curWord = sci.GetWordFromPosition(endPos);
                if (startPos == endPos)
                {
                    endPos = sci.WordEndPosition(sci.CurrentPos, true);
                    startPos = sci.WordStartPosition(sci.CurrentPos, true);
                    sci.SetSel(startPos, endPos);
                }
                if (!string.IsNullOrEmpty(curWord))
                {
                    // Remember the current word
                    ArgsProcessor.PrevSelWord = curWord;
                }
                SnippetHelper.InsertSnippetText(sci, endPos, snippet);
                return true;
            }

            if (canShowList)
            {
                ICompletionListItem item;
                List<ICompletionListItem> items = new List<ICompletionListItem>();
                PathWalker walker = new PathWalker(PathHelper.SnippetDir, "*.fds", false);
                List<string> files = walker.GetFiles();
                foreach (string file in files)
                {
                    item = new SnippetItem(Path.GetFileNameWithoutExtension(file), file);
                    items.Add(item);
                }
                string path = Path.Combine(PathHelper.SnippetDir, sci.ConfigurationLanguage);
                if (Directory.Exists(path))
                {
                    walker = new PathWalker(path, "*.fds", false);
                    files = walker.GetFiles();
                    foreach (string file in files)
                    {
                        item = new SnippetItem(Path.GetFileNameWithoutExtension(file), file);
                        items.Add(item);
                    }
                }
                if (items.Count > 0)
                {
                    items.Sort();
                    if (!string.IsNullOrEmpty(sci.SelText)) word = sci.SelText;
                    else
                    {
                        word = sci.GetWordFromPosition(sci.CurrentPos) ?? string.Empty;
                    }
                    CompletionList.OnInsert += HandleListInsert;
                    CompletionList.OnCancel += HandleListInsert;
                    CompletionList.Show(items, false, word);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// On completion list insert or cancel, reset the previous selection
        /// </summary>
        static void HandleListInsert(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
        {
            CompletionList.OnInsert -= HandleListInsert;
            CompletionList.OnCancel -= HandleListInsert;
            ArgsProcessor.PrevSelText = string.Empty;
        }

    }

    public class SnippetItem : ICompletionListItem, IComparable, IComparable<ICompletionListItem>
    {
        string snippet;
        readonly string fileName;
        Bitmap icon;

        public SnippetItem(string word, string fileName)
        {
            Label = word;
            this.fileName = fileName;
        }

        /// <summary>
        /// Label of the snippet item
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Description of the snippet item
        /// </summary>
        public string Description
        {
            get
            {
                string desc = TextHelper.GetString("Info.SnippetItemDesc");
                if (snippet is null)
                {
                    snippet = FileHelper.ReadFile(fileName);
                    snippet = ArgsProcessor.ProcessCodeStyleLineBreaks(snippet);
                    snippet = snippet.Replace(SnippetHelper.ENTRYPOINT, "|");
                    snippet = snippet.Replace(SnippetHelper.EXITPOINT, "|");
                }
                if (snippet.Length > 40) return desc + ": " + snippet.Substring(0, 40) + "...";
                return desc + ": " + snippet;
            }
        }

        /// <summary>
        /// String value if the snippet item
        /// </summary>
        public string Value
        {
            get
            {
                SnippetManager.InsertTextByWord(Label);
                return null;
            }
        }

        /// <summary>
        /// Icon if the snippet item
        /// </summary>
        public Bitmap Icon
        {
            get => icon ??= (Bitmap) PluginBase.MainForm.FindImage("341");
            set => icon = value;
        }

        /// <summary>
        /// Checks the validity of the completion list item 
        /// </summary> 
        int IComparable.CompareTo(object obj)
        {
            if (obj is ICompletionListItem item) return string.Compare(Label, item.Label, true);
            string message = TextHelper.GetString("Info.CompareError");
            throw new Exception(message);
        }

        /// <summary>
        /// Compares the completion list items
        /// </summary> 
        int IComparable<ICompletionListItem>.CompareTo(ICompletionListItem other)
        {
            return string.Compare(Label, other.Label, true);
        }
    }
}