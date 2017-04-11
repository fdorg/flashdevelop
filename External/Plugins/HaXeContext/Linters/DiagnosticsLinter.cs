using LintingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCompletion.Completion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace HaXeContext.Linters
{
    class DiagnosticsLinter : ILintProvider
    {
        private static Dictionary<HaxeDiagnosticsSeverity, LintingSeverity> conversionDict =
            new Dictionary<HaxeDiagnosticsSeverity, LintingSeverity>()
            {
                {HaxeDiagnosticsSeverity.ERROR, LintingSeverity.Error},
                {HaxeDiagnosticsSeverity.INFO, LintingSeverity.Info},
                {HaxeDiagnosticsSeverity.WARNING, LintingSeverity.Warning}
            };

        public void LintAsync(string[] files, LintCallback callback)
        {
            foreach (var file in files)
            {
                var document = DocumentManager.FindDocument(file) ?? PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                var context = ASContext.GetLanguageContext("haxe") as Context;

                if (document == null || context == null) continue;

                //stub expression is needed for position
                var exprStub = new ASExpr();
                exprStub.Position = 0;

                var completionMode = ((HaXeSettings) context.Settings).CompletionMode;
                var haxeVersion = context.GetCurrentSDKVersion();
                if (completionMode != HaxeCompletionModeEnum.FlashDevelop && haxeVersion.IsGreaterThanOrEquals(new SemVer("3.3.0")))
                {
                    var hc = context.GetHaxeComplete(document.SciControl, exprStub, true, HaxeCompilerService.DIAGNOSTICS);
                    hc.GetDiagnostics((complete, results, status) =>
                    {
                        var list = new List<LintingResult>();
                        if (results == null)
                        {
                            callback(null);
                            return;
                        }

                        foreach (var res in results)
                        {
                            var line = res.Range.LineStart + 1;
                            var firstChar = document.SciControl.PositionFromLine(line) + res.Range.CharacterStart;
                            var lastChar = document.SciControl.PositionFromLine(res.Range.LineEnd + 1) + res.Range.CharacterEnd;
                            var result = new LintingResult
                            {
                                File = res.Range.Path,
                                FirstChar = res.Range.CharacterStart,
                                Length = lastChar - firstChar,
                                Line = line,
                                Severity = conversionDict[res.Severity]
                            };

                            switch (res.Kind)
                            {
                                case HaxeDiagnosticsKind.UNUSEDIMPORT:
                                    result.Description = "Unused import";
                                    break;
                                case HaxeDiagnosticsKind.UNUSEDVAR:
                                    result.Description = "Unused variable";
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
}
