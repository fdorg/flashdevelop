using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor.Commands
{
    internal class ExtractLocalVariableCommand : RefactorCommand<IDictionary<string, List<SearchMatch>>>
    {
        readonly bool outputResults;
        internal List<ICompletionListItem> CompletionList;
        string newName;

        /// <summary>
        /// A new ExtractLocalVariableCommand refactoring command.
        /// Outputs found results.
        /// Uses the current selected text as the declaration target.
        /// </summary>
        public ExtractLocalVariableCommand() : this(true)
        {
        }

        /// <summary>
        /// A new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public ExtractLocalVariableCommand(bool outputResults) : this(outputResults, null)
        {
        }

        /// <summary>
        /// A new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        public ExtractLocalVariableCommand(bool outputResults, string newName)
        {
            this.outputResults = outputResults;
            this.newName = newName;
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Entry point to execute renaming.
        /// </summary>
        protected override void ExecutionImplementation()
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
            mathes.RemoveAll(it => it.Line < lineFrom || it.Line > lineTo);
            var target = mathes.FindAll(it => sci.MBSafePosition(it.Index) == sci.SelectionStart);
            if (mathes.Count > 1)
            {
                CompletionList = new List<ICompletionListItem> {new CompletionListItem(target, sci, OnItemClick)};
                CompletionList.Insert(0, new CompletionListItem(mathes, sci, OnItemClick));
                sci.DisableAllSciEvents = true;
                PluginCore.Controls.CompletionList.Show(CompletionList, true);
            }
            else GenerateExtractVariable(target);
        }

        /// <summary>
        /// This retrieves the new name from the user
        /// </summary>
        static string GetNewName()
        {
            var newName = "newVar";
            var label = TextHelper.GetString("Label.NewName");
            var title = TextHelper.GetString("Title.ExtractLocalVariableDialog");
            var askName = new LineEntryDialog(title, label, newName);
            var choice = askName.ShowDialog();
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (choice != DialogResult.OK)
            {
                sci.DisableAllSciEvents = false;
                return null;
            }
            sci.DisableAllSciEvents = false;
            var name = askName.Line.Trim();
            if (name.Length > 0 && name != newName) newName = name;
            return newName;
        }

        void GenerateExtractVariable(List<SearchMatch> matches)
        {
            if (string.IsNullOrEmpty(newName)) newName = GetNewName();
            if (string.IsNullOrEmpty(newName)) return;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.BeginUndoAction();
            try
            {
                var expression = sci.SelText.Trim(new char[] {'=', ' ', '\t', '\n', '\r', ';', '.'});
                expression = expression.TrimEnd(new char[] {'(', '[', '{', '<'});
                expression = expression.TrimStart(new char[] {')', ']', '}', '>'});
                var insertPosition = sci.PositionFromLine(ASContext.Context.CurrentMember.LineTo);
                foreach (var match in matches)
                {
                    var position = sci.MBSafePosition(match.Index);
                    insertPosition = Math.Min(insertPosition, position);
                    match.LineText = sci.GetLine(match.Line - 1);
                }
                insertPosition = sci.LineFromPosition(insertPosition);
                insertPosition = sci.LineIndentPosition(insertPosition);
                RefactoringHelper.ReplaceMatches(matches, sci, newName);
                sci.SetSel(insertPosition, insertPosition);
                var member = new MemberModel(newName, string.Empty, FlagType.LocalVar, 0) {Value = expression};
                var snippet = TemplateUtils.GetTemplate("Variable");
                snippet = TemplateUtils.ReplaceTemplateVariable(snippet, "Modifiers", null);
                snippet = TemplateUtils.ToDeclarationString(member, snippet);
                snippet += "$(Boundary)\n$(Boundary)";
                SnippetHelper.InsertSnippetText(sci, sci.CurrentPos, snippet);
                foreach (var match in matches)
                {
                    match.Line += 1;
                }
                Results = new Dictionary<string, List<SearchMatch>> {{sci.FileName, matches}};
                if (outputResults) ReportResults();
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        /// <summary>
        /// Outputs the results to the TraceManager
        /// </summary>
        void ReportResults()
        {
            var newNameLength = newName.Length;
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            foreach (var entry in Results)
            {
                var lineOffsets = new Dictionary<int, int>();
                var lineChanges = new Dictionary<int, string>();
                var reportableLines = new Dictionary<int, List<string>>();
                foreach (var match in entry.Value)
                {
                    var column = match.Column;
                    var lineNumber = match.Line;
                    var changedLine = lineChanges.ContainsKey(lineNumber) ? lineChanges[lineNumber] : match.LineText;
                    var offset = lineOffsets.ContainsKey(lineNumber) ? lineOffsets[lineNumber] : 0;
                    column = column + offset;
                    changedLine = changedLine.Substring(0, column) + newName + changedLine.Substring(column + match.Length);
                    lineChanges[lineNumber] = changedLine;
                    lineOffsets[lineNumber] = offset + (newNameLength - match.Length);
                    if (!reportableLines.ContainsKey(lineNumber)) reportableLines[lineNumber] = new List<string>();
                    reportableLines[lineNumber].Add(entry.Key + ":" + match.Line + ": chars " + column + "-" + (column + newNameLength) + " : {0}");
                }
                foreach (var lineSetsToReport in reportableLines)
                {
                    var renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (var lineToReport in lineSetsToReport.Value)
                    {
                        TraceManager.Add(string.Format(lineToReport, renamedLine), (int) TraceType.Info);
                    }
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }

        void OnItemClick(object sender, EventArgs e)
        {
            GenerateExtractVariable(((CompletionListItem) sender).Matches);
        }
    }

    internal sealed class CompletionListItem : ToolStripMenuItem, ICompletionListItem
    {
        const int Indicator = 0;
        public readonly List<SearchMatch> Matches;
        public readonly ScintillaControl Sci;

        public CompletionListItem(List<SearchMatch> matches, ScintillaControl sci, EventHandler onClick)
        {
            var count = matches.Count;
            Text = string.Format(TextHelper.GetString("Info.ReplacedOccurrences"), count);
            description = count == 1
                ? TextHelper.GetString("Description.ReplaceInitialOccurrence")
                : TextHelper.GetString("Description.ReplaceAllOccurrences");
            Click += onClick;
            Icon = new Bitmap(16, 16);
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
                return null;
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