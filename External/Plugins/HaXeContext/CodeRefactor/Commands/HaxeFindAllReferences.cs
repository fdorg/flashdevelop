using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.FRService;
using PluginCore.Localization;
using PluginCore.Managers;

namespace HaXeContext.CodeRefactor.Commands
{
    class HaxeFindAllReferences : FindAllReferences
    {
        public HaxeFindAllReferences(ASResult target, bool output, bool ignoreDeclarations) : base(target, output, ignoreDeclarations)
        {
        }

        protected override void ExecutionImplementation()
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("CodeRefactor.Info.FindingReferences"));
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("CodeRefactor.Info.SearchingFiles"));
            var context = (Context) ASContext.GetLanguageContext("haxe");
            var hc = context.GetHaxeComplete(ASContext.CurSciControl, CurrentTarget.Context, true, HaxeCompilerService.USAGE);
            hc.GetUsages(OnHaxeCompleteResultHandler);
        }

        void OnHaxeCompleteResultHandler(HaxeComplete hc, List<HaxePositionResult> result, HaxeCompleteStatus status)
        {
            var results = new FRResults();
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.AddAsync(hc.Errors, -3);
                    break;
                case HaxeCompleteStatus.USAGE:
                    if (!IgnoreDeclarationSource)
                    {
                        var sci = ASContext.CurSciControl;
                        var path = sci.FileName;
                        if (!results.ContainsKey(path)) results.Add(path, new List<SearchMatch>());
                        var index = hc.Expr.PositionExpression;
                        var line = sci.LineFromPosition(index);
                        var lineStart = sci.PositionFromLine(line);
                        var lineEnd = sci.LineEndPosition(line);
                        var lineText = sci.GetLine(line);
                        var value = hc.Expr.Value;
                        var match = new SearchMatch
                        {
                            Column = index - lineStart,
                            Index = index,
                            Length = value.Length,
                            Line = line + 1,
                            LineStart = lineStart,
                            LineEnd = lineEnd,
                            LineText = lineText,
                            Value = value
                        };
                        match.Index = sci.MBSafeCharPosition(match.Index);
                        results[path].Add(match);
                    }
                    foreach (var it in result)
                    {
                        var path = it.Path;
                        if (!results.ContainsKey(path)) results.Add(path, new List<SearchMatch>());
                        var matches = results[path];
                        var sci = AssociatedDocumentHelper.LoadDocument(path).SciControl;
                        var line = it.LineStart - 1;
                        var lineStart = sci.PositionFromLine(line);
                        var lineEnd = sci.LineEndPosition(line);
                        var lineText = sci.GetLine(line);
                        var value = lineText.Substring(it.CharacterStart, it.CharacterEnd - it.CharacterStart);
                        var match = new SearchMatch
                        {
                            Column = it.CharacterStart,
                            Index = lineStart + it.CharacterStart,
                            Length = value.Length,
                            Line = line + 1,
                            LineStart = lineStart,
                            LineEnd = lineEnd,
                            LineText = lineText,
                            Value = value
                        };
                        match.Index = sci.MBSafeCharPosition(match.Index);
                        matches.Add(match);
                    }
                    break;
            }
            FindFinished(results);
        }
    }
}
