﻿using LintingHelper;
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace HaXeContext.Linters
{
    class DiagnosticsLinter : ILintProvider
    {
        static readonly Dictionary<HaxeDiagnosticsSeverity, LintingSeverity> DiagnosticsSeverityToLintingSeverity =
            new Dictionary<HaxeDiagnosticsSeverity, LintingSeverity>
            {
                {HaxeDiagnosticsSeverity.ERROR, LintingSeverity.Error},
                {HaxeDiagnosticsSeverity.INFO, LintingSeverity.Info},
                {HaxeDiagnosticsSeverity.WARNING, LintingSeverity.Warning}
            };

        public void LintAsync(string[] files, LintCallback callback)
        {
            var context = ASContext.GetLanguageContext("haxe") as Context;
            if (context == null) return;
            var completionMode = ((HaXeSettings) context.Settings).CompletionMode;
            var haxeVersion = context.GetCurrentSDKVersion();
            if (completionMode == HaxeCompletionModeEnum.FlashDevelop || haxeVersion.IsOlderThan(new SemVer("3.3.0"))) return;
            foreach (var file in files)
            {
                var document = DocumentManager.FindDocument(file) ?? PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                if (document == null) continue;
                var hc = context.GetHaxeComplete(document.SciControl, new ASExpr {Position = 0}, true, HaxeCompilerService.DIAGNOSTICS);
                hc.GetDiagnostics((complete, results, status) =>
                {
                    var sci = document.SciControl;
                    if (results == null || sci == null)
                    {
                        callback(null);
                        return;
                    }
                    var list = new List<LintingResult>();
                    foreach (var res in results)
                    {
                        var line = res.Range.LineStart + 1;
                        var firstChar = sci.PositionFromLine(line) + res.Range.CharacterStart;
                        var lastChar = sci.PositionFromLine(res.Range.LineEnd + 1) + res.Range.CharacterEnd;
                        var result = new LintingResult
                        {
                            File = res.Range.Path,
                            FirstChar = res.Range.CharacterStart,
                            Length = lastChar - firstChar,
                            Line = line,
                            Severity = DiagnosticsSeverityToLintingSeverity[res.Severity]
                        };

                        switch (res.Kind)
                        {
                            case HaxeDiagnosticsKind.UNUSEDIMPORT:
                                result.Description = TextHelper.GetString("HaXeContext.Info.UnusedImport");
                                break;
                            case HaxeDiagnosticsKind.UNUSEDVAR:
                                result.Description = TextHelper.GetString("HaXeContext.Info.UnusedVariable");
                                break;
                        }

                        list.Add(result);
                    }
                    callback(list);
                });
            }
        }
    }
}
