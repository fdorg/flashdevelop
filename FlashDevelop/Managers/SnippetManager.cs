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
    class SnippetManager
    {
        /// <summary>
        /// Gets a snippet from a file in the snippets directory
        /// </summary>
        public static String GetSnippet(String word, String syntax, Encoding current)
        {
            String global = Path.Combine(PathHelper.SnippetDir, word + ".fds");
            String specificDir = Path.Combine(PathHelper.SnippetDir, syntax);
            String specific = Path.Combine(specificDir, word + ".fds");
            if (File.Exists(specific))
            {
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(specific);
                return DataConverter.ChangeEncoding(info.Contents, info.CodePage, current.CodePage);
            }
            else if (File.Exists(global))
            {
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(global);
                return DataConverter.ChangeEncoding(info.Contents, info.CodePage, current.CodePage);
            }
            else return null;
        }

        /// <summary>
        /// Inserts text from the snippets class
        /// </summary>
        public static Boolean InsertTextByWord(String word, Boolean emptyUndoBuffer)
        {
            ScintillaControl sci = Globals.SciControl;
            if (sci == null) return false;
            Boolean canShowList = false; 
            String snippet = null;
            if (word == null)
            {
                canShowList = true;
                word = sci.GetWordFromPosition(sci.CurrentPos);
            }
            if (!string.IsNullOrEmpty(word))
            {
                snippet = GetSnippet(word, sci.ConfigurationLanguage, sci.Encoding);
            }
            // let plugins handle the snippet
            Hashtable data = new Hashtable();
            data["word"] = word;
            data["snippet"] = snippet;
            DataEvent de = new DataEvent(EventType.Command, "SnippetManager.Expand", data);
            EventManager.DispatchEvent(Globals.MainForm, de);
            if (de.Handled) return true;
            snippet = (string)data["snippet"];
            if (!String.IsNullOrEmpty(sci.SelText))
            {
                // Remember the previous selection
                ArgsProcessor.PrevSelText = sci.SelText;
            }
            if (snippet != null)
            {
                Int32 endPos = sci.SelectionEnd;
                Int32 startPos = sci.SelectionStart;
                String curWord = sci.GetWordFromPosition(endPos);
                if (startPos == endPos)
                {
                    endPos = sci.WordEndPosition(sci.CurrentPos, true);
                    startPos = sci.WordStartPosition(sci.CurrentPos, true);
                    sci.SetSel(startPos, endPos);
                }
                if (!String.IsNullOrEmpty(curWord))
                {
                    // Remember the current word
                    ArgsProcessor.PrevSelWord = curWord;
                }
                SnippetHelper.InsertSnippetText(sci, endPos, snippet);
                return true;
            }
            else if (canShowList)
            {
                ICompletionListItem item;
                List<ICompletionListItem> items = new List<ICompletionListItem>();
                PathWalker walker = new PathWalker(PathHelper.SnippetDir, "*.fds", false);
                List<String> files = walker.GetFiles();
                foreach (String file in files)
                {
                    item = new SnippetItem(Path.GetFileNameWithoutExtension(file), file);
                    items.Add(item);
                }
                String path = Path.Combine(PathHelper.SnippetDir, sci.ConfigurationLanguage);
                if (Directory.Exists(path))
                {
                    walker = new PathWalker(path, "*.fds", false);
                    files = walker.GetFiles();
                    foreach (String file in files)
                    {
                        item = new SnippetItem(Path.GetFileNameWithoutExtension(file), file);
                        items.Add(item);
                    }
                }
                if (items.Count > 0)
                {
                    items.Sort();
                    if (!String.IsNullOrEmpty(sci.SelText)) word = sci.SelText;
                    else
                    {
                        word = sci.GetWordFromPosition(sci.CurrentPos);
                        if (word == null) word = String.Empty;
                    }
                    CompletionList.OnInsert += new InsertedTextHandler(HandleListInsert);
                    CompletionList.OnCancel += new InsertedTextHandler(HandleListInsert);
                    CompletionList.Show(items, false, word);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// On completion list insert or cancel, reset the previous selection
        /// </summary>
        private static void HandleListInsert(ScintillaControl sender, Int32 position, String text, Char trigger, ICompletionListItem item)
        {
            CompletionList.OnInsert -= new InsertedTextHandler(HandleListInsert);
            CompletionList.OnCancel -= new InsertedTextHandler(HandleListInsert);
            ArgsProcessor.PrevSelText = String.Empty;
        }

    }

    public class SnippetItem : ICompletionListItem, IComparable, IComparable<ICompletionListItem>
    {
        private String word;
        private String snippet;
        private String fileName;
        private Bitmap icon;

        public SnippetItem(String word, String fileName)
        {
            this.word = word;
            this.fileName = fileName;
        }

        /// <summary>
        /// Label of the snippet item
        /// </summary>
        public String Label
        {
            get { return this.word; }
        }

        /// <summary>
        /// Description of the snippet item
        /// </summary>
        public String Description
        {
            get
            {
                String desc = TextHelper.GetString("Info.SnippetItemDesc");
                if (this.snippet == null)
                {
                    this.snippet = FileHelper.ReadFile(this.fileName);
                    this.snippet = ArgsProcessor.ProcessCodeStyleLineBreaks(this.snippet);
                    this.snippet = this.snippet.Replace(SnippetHelper.ENTRYPOINT, "|");
                    this.snippet = this.snippet.Replace(SnippetHelper.EXITPOINT, "|");
                }
                if (this.snippet.Length > 40) return desc + ": " + this.snippet.Substring(0, 40) + "...";
                else return desc + ": " + this.snippet;
            }
        }

        /// <summary>
        /// String value if the snippet item
        /// </summary>
        public String Value
        {
            get
            {
                SnippetManager.InsertTextByWord(this.word, false);
                return null;
            }
        }

        /// <summary>
        /// Icon if the snippet item
        /// </summary>
        public Bitmap Icon
        {
            get 
            {
                if (icon == null)
                {
                    this.icon = (Bitmap)Globals.MainForm.FindImage("341");
                }
                return icon;
            }
            set 
            {
                this.icon = value;
            }
        }

        /// <summary>
        /// Checks the validity of the completion list item 
        /// </summary> 
        Int32 IComparable.CompareTo(Object obj)
        {
            if (obj as ICompletionListItem != null) return String.Compare(Label, (obj as ICompletionListItem).Label, true);
            else
            {
                String message = TextHelper.GetString("Info.CompareError");
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Compares the completion list items
        /// </summary> 
        Int32 IComparable<ICompletionListItem>.CompareTo(ICompletionListItem other)
        {
            return String.Compare(Label, other.Label, true);
        }

    }

}
