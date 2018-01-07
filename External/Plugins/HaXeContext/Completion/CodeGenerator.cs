using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext.Completion
{
    internal class CodeGenerator : ASGenerator
    {
        public override bool ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options, ASResult expr)
        {
            var context = ASContext.Context;
            var member = expr.Member;
            if (context.CurrentClass.Flags.HasFlag(FlagType.Interface) && (member == null || member.Flags.HasFlag(FlagType.Variable)))
            {
                return true;
            }
            if (context.CurrentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef))
            {
                if (contextToken != null && member == null)
                {
                    var type = expr.Type ?? ClassModel.VoidClass;
                    if (!context.IsImported(type, sci.CurrentLine)) CheckAutoImport(expr, options);
                }
                return true;
            }
            if (member != null
                && member.Parameters?.Count > 0
                && (member.LineFrom != sci.CurrentLine || !expr.Context.BeforeBody)
                && member.Flags.HasFlag(FlagType.Static | FlagType.Function))
            {
                ShowConvertToUsing(sci, options, expr);
            }
            return false;
        }

        static void ShowConvertToUsing(ScintillaControl sci, ICollection<ICompletionListItem> options, ASResult expr)
        {
            var label = TextHelper.GetString("HaXeContext.Label.ConvertStaticCallToUsingCall");
            options.Add(new GeneratorItem(label, () =>
            {
                sci.BeginUndoAction();
                try
                {
                    ConvertStaticMethodCallIntoStaticExtensionCall(sci, expr);
                }
                finally
                {
                    sci.EndUndoAction();
                }
            }));
        }

        internal static void ConvertStaticMethodCallIntoStaticExtensionCall(ScintillaControl sci, ASResult expr)
        {
            var member = expr.Member;
            var inClass = expr.InClass;
            var isImported = ASContext.Context.IsImported(inClass, sci.CurrentLine);
            var caretPos = sci.CurrentPos;
            var startPos = expr.Context.PositionExpression;
            int endPos;
            if (expr.Context.ContextFunction != null) endPos = sci.LineEndPosition(expr.Context.ContextFunction.LineTo);
            else endPos = sci.LineEndPosition(expr.Context.ContextMember.LineFrom);
            endPos = ASGenerator.GetEndOfStatement(startPos, endPos, sci);
            var parameters = ASGenerator.ParseFunctionParameters(sci, sci.WordEndPosition(caretPos, true));
            var ctx = parameters[0].result.Context;
            var value = ctx.Value;
            if (ctx.SubExpressions != null)
            {
                var startIndex = 0;
                for (var i = ctx.SubExpressions.Count - 1; i >= 0; i--)
                {
                    var pattern = ".#" + i + "~";
                    startIndex = value.IndexOf(pattern, startIndex);
                    if (startIndex != -1)
                    {
                        var newValue = ctx.SubExpressions[i];
                        value = value.Replace(pattern, newValue);
                        startIndex += newValue.Length;
                    }
                }
            }
            value = value.Replace(".[", "[");
            if (ctx.WordBefore == "new") value = "new " + value;
            var statement = value + "." + member.Name + "(" + string.Join(", ", parameters.Skip(1).Select(it => it.result.Context.Value)) + ");";
            sci.SetSel(startPos, endPos);
            sci.ReplaceSel(statement);
            if (isImported) return;
            caretPos += value.Length - (expr.Path.Length - member.Name.Length);
            caretPos += InsertUsing(inClass);
            sci.SetSel(caretPos, caretPos);
        }

        /// <summary>
        /// Add an 'using' statement in the current file
        /// </summary>
        /// <param name="member">Generates 'using {member.Type};'</param>
        /// <returns>Inserted characters count</returns>
        static int InsertUsing(MemberModel member)
        {
            var statement = string.Empty;
            var inFile = member.InFile;
            if (inFile != null && member.Name != inFile.Module)
            {
                if (!string.IsNullOrEmpty(inFile.Package)) statement = inFile.Package + ".";
                statement += inFile.Module + "." + member.Name;
            }
            if (string.IsNullOrEmpty(statement)) statement = member.Type;
            var sci = ASContext.CurSciControl;
            var newLineMarker = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            statement = "using " + statement + ";" + newLineMarker;
            var position = sci.CurrentPos;
            var cFile = ASContext.Context.CurrentModel;
            var line = ASContext.Context.InPrivateSection ? cFile.PrivateSectionIndex : 0;
            if (cFile.InlinedRanges != null)
            {
                foreach (var range in cFile.InlinedRanges)
                {
                    if (position > range.Start && position < range.End)
                    {
                        line = sci.LineFromPosition(range.Start) + 1;
                        break;
                    }
                }
            }
            var firstLine = line;
            var found = false;
            var indent = 0;
            var skipIfDef = 0;
            var importKey = ASContext.Context.Features.importKey;
            var importKeyAlt = ASContext.Context.Features.importKeyAlt;
            var importKeyAltLenght = importKeyAlt.Length;
            var curLine = sci.CurrentLine;
            while (line < curLine)
            {
                var txt = sci.GetLine(line++).TrimStart();
                if (txt.StartsWith("package") || txt.StartsWithOrdinal(importKey)) firstLine = line;
                else if (txt.StartsWithOrdinal("#if") && txt.IndexOfOrdinal("#end") == -1) skipIfDef++;
                else if (skipIfDef > 0)
                {
                    if (txt.StartsWithOrdinal("#end")) skipIfDef--;
                }
                else if (txt.Length > importKeyAltLenght && txt.StartsWithOrdinal(importKeyAlt) && txt[importKeyAltLenght] <= 32)
                {
                    found = true;
                    indent = sci.GetLineIndentation(line - 1);
                    var m = ASFileParserRegexes.Import.Match(txt);
                    if (m.Success && CaseSensitiveImportComparer.CompareImports(m.Groups["package"].Value, member.Type) > 0)
                    {
                        line--;
                        break;
                    }
                }
                else if (found)
                {
                    line--;
                    break;
                }
            }
            if (line == curLine) line = firstLine;
            position = sci.PositionFromLine(line);
            firstLine = sci.FirstVisibleLine;
            sci.SetSel(position, position);
            sci.ReplaceSel(statement);
            sci.SetLineIndentation(line, indent);
            sci.LineScroll(0, firstLine - sci.FirstVisibleLine + 1);
            ASContext.Context.RefreshContextCache(member.Type);
            return sci.GetLine(line).Length;
        }
    }

    class GeneratorItem : ICompletionListItem
    {
        readonly Action action;

        public GeneratorItem(string label, Action action)
        {
            Label = label;
            this.action = action;
        }

        public string Label { get; }

        public string Value
        {
            get
            {
                action.Invoke();
                return null;
            }
        }

        public string Description => TextHelper.GetString("ASCompletion.Info.GeneratorTemplate");

        public Bitmap Icon => (Bitmap) ASContext.Panel.GetIcon(34);
    }
}
