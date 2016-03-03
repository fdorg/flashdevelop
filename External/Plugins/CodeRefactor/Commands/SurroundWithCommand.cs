using System;
using System.IO;
using System.Text;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Utilities;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    public class SurroundWithCommand
    {
        protected string SnippetCode;

        public SurroundWithCommand(string snippet)
        {
            this.SnippetCode = snippet;
        }

        public void Execute()
        {
            ExecutionImplementation();
        }

        /// <summary>
        /// The actual process implementation
        /// </summary>
        protected void ExecutionImplementation()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.BeginUndoAction();
            try
            {
                string selection = sci.SelText;
                if (string.IsNullOrEmpty(selection))
                {
                    return;
                }

                if (selection.TrimStart().Length == 0)
                {
                    return;
                }

                sci.SetSel(sci.SelectionStart + selection.Length - selection.TrimStart().Length,
                    sci.SelectionEnd);
                sci.CurrentPos = sci.SelectionEnd;
                selection = sci.SelText;

                int lineStart = sci.LineFromPosition(sci.SelectionStart);
                int lineEnd = sci.LineFromPosition(sci.SelectionEnd);
                int firstLineIndent = sci.GetLineIndentation(lineStart);
                int entryPointIndent = 0;

                string snippet = GetSnippet(SnippetCode, sci.ConfigurationLanguage, sci.Encoding);
                int pos = snippet.IndexOfOrdinal("{0}");
                if (pos > -1)
                {
                    while (pos >= 0)
                    {
                        string c = snippet.Substring(--pos, 1);
                        if (c.Equals("\t"))
                        {
                            entryPointIndent += sci.Indent;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                for (int i = lineStart; i <= lineEnd; i++)
                {
                    int indent = sci.GetLineIndentation(i);
                    if (i > lineStart)
                    {
                        sci.SetLineIndentation(i, indent - firstLineIndent + entryPointIndent);
                    }
                }

                snippet = snippet.Replace("{0}", sci.SelText);

                int insertPos = sci.SelectionStart;
                int selEnd = sci.SelectionEnd;

                sci.SetSel(insertPos, selEnd);
                SnippetHelper.InsertSnippetText(sci, insertPos, snippet);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        public static String GetSnippet(String word, String syntax, Encoding current)
        {
            string surroundFolder = "surround";

            String globalDir = Path.Combine(PathHelper.SnippetDir, surroundFolder);
            String global = Path.Combine(globalDir, word + ".fds");
            String specificDir = Path.Combine(PathHelper.SnippetDir, syntax);
            specificDir = Path.Combine(specificDir, surroundFolder);
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

    }
}