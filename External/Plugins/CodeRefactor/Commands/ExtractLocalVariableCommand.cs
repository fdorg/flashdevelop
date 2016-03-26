using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Localization;
using ProjectManager.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor.Commands
{
    class ExtractLocalVariableCommand
    {
        public void Execute()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var fileModel = ASContext.Context.CurrentModel;
            var parser = new ASFileParser();
            parser.ParseSrc(fileModel, sci.Text);
            var search = new FRSearch(sci.SelText) {SourceFile = sci.FileName};
            var mathes = search.Matches(sci.Text);
            var currentMember = fileModel.Context.CurrentMember;
            var lineFrom = currentMember.LineFrom;
            var lineTo = currentMember.LineTo;
            var currentLine = sci.CurrentLine + 1;
            mathes.RemoveAll(it => it.Line < lineFrom || it.Line > lineTo || it.Line == currentLine);
            var list = new List<ICompletionListItem> {new CompletionListItem(sci, OnItemClick)};
            if (mathes.Count > 1) list.Insert(0, new CompletionListItem(mathes, sci, OnItemClick));
            sci.DisableAllSciEvents = true;
            CompletionList.Show(list, false);
        }

        static void OnItemClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.DisableAllSciEvents = false;
            var newName = "newVar";
            var label = TextHelper.GetString("Label.NewName");
            var title = TextHelper.GetString("Title.ExtractLocalVariableDialog");
            var askName = new LineEntryDialog(title, label, newName);
            var choice = askName.ShowDialog();
            if (choice != DialogResult.OK) return;
            var name = askName.Line.Trim();
            if (name.Length > 0 && name != newName) newName = name;
            sci.BeginUndoAction();
            try
            {
                //var item = (CompletionListItem) sender;
                //RefactoringHelper.ReplaceMatches(item.Matches, sci, newName);
                ASGenerator.GenerateExtractVariable(sci, newName);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }

    internal class CompletionListItem : ToolStripMenuItem, ICompletionListItem
    {
        const int Indicator = 0;
        public readonly List<SearchMatch> Matches;
        public readonly ScintillaControl Sci;

        public CompletionListItem(ScintillaControl sci, EventHandler onClick) : this(null, sci, onClick)
        {
        }
        public CompletionListItem(List<SearchMatch> matches, ScintillaControl sci, EventHandler onClick)
        {
            if (matches == null)
            {
                Text = "Replace one occurrence";
                description = "Replace initial expression only.";
            }
            else
            {
                Text = string.Format("Replace {0} occurrences", matches.Count);
                description = "Replace all highlighted occurrences of initial expression with new variable.";
            }
            Image = PluginBase.MainForm.FindImage("-1");
            Click += onClick;
            Icon = new Bitmap(Image);
            Matches = matches;
            Sci = sci;
        }

        public string Label { get { return Text; } }

        public string Value
        {
            get
            {
                RemoveHighlights();
                PerformClick();
                return string.Empty;
            }
        }

        string description;
        public string Description
        {
            get
            {
                RemoveHighlights();
                if (Matches != null)
                {
                    foreach (var m in Matches)
                    {
                        Highlight(m.Index, m.Length);
                    }
                }
                return description;
            }
        }

        public Bitmap Icon { get; private set; }

        /// <summary>
        /// Modify the highlight indicator alpha and select current word.
        /// </summary>
        void RemoveHighlights()
        {
            Sci.RemoveHighlights(Indicator);
            Sci.SetIndicSetAlpha(Indicator, 100);
        }

        /// <summary>
        /// Highlight the specified region.
        /// </summary>
        /// <param name="startIndex">The start index of the highlight.</param>
        /// <param name="length">The length of the highlight.</param>
        void Highlight(int startIndex, int length)
        {
            Sci.SetIndicStyle(Indicator, (int) IndicatorStyle.Container);
            Sci.SetIndicFore(Indicator, 0x00FF00);
            Sci.CurrentIndicator = Indicator;
            Sci.IndicatorFillRange(startIndex, length);
            var es = Sci.EndStyled;
            var mask = (1 << Sci.StyleBits) - 1;
            Sci.StartStyling(es, mask);
        }
    }
}