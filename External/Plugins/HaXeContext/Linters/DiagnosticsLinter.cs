using LintingHelper;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext.Linters
{
    class DiagnosticsLinter : ILintProvider
    {
        public void LintAsync(string[] files, LintCallback callback)
        {
            var context = ASContext.GetLanguageContext("haxe") as Context;
            if (context == null) return;
            var completionMode = ((HaXeSettings) context.Settings).CompletionMode;
            var haxeVersion = context.GetCurrentSDKVersion();
            if (completionMode == HaxeCompletionModeEnum.FlashDevelop || haxeVersion.IsOlderThan(new SemVer("3.3.0"))) return;

            var list = new List<LintingResult>();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                if (!File.Exists(file)) continue;
                var sci = DocumentManager.FindDocument(file)?.SciControl;
                if (sci == null)
                {
                    sci = new ScintillaControl
                    {
                        Text = File.ReadAllText(file),
                        FileName = file,
                        ConfigurationLanguage = "haxe"
                    };
                }

                var hc = context.GetHaxeComplete(sci, new ASExpr {Position = 0}, true, HaxeCompilerService.DIAGNOSTICS);
                var i1 = i;
                
                hc.GetDiagnostics((complete, results, status) =>
                {
                    if (status == HaxeCompleteStatus.DIAGNOSTICS && results != null && sci != null && !sci.IsDisposed)
                    {
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
                            };

                            switch (res.Severity)
                            {
                                case HaxeDiagnosticsSeverity.INFO:
                                    result.Severity = LintingSeverity.Info;
                                    break;
                                case HaxeDiagnosticsSeverity.ERROR:
                                    result.Severity = LintingSeverity.Error;
                                    break;
                                case HaxeDiagnosticsSeverity.WARNING:
                                    result.Severity = LintingSeverity.Warning;
                                    break;
                                default:
                                    continue;
                            }

                            switch (res.Kind)
                            {
                                case HaxeDiagnosticsKind.UnusedImport:
                                    result.Description = TextHelper.GetString("Info.UnusedImport");
                                    break;
                                case HaxeDiagnosticsKind.UnresolvedIdentifier:
                                    result.Description = TextHelper.GetString("Info.UnresolvedIdentifier");
                                    break;
                                case HaxeDiagnosticsKind.CompilerError:
                                case HaxeDiagnosticsKind.RemovableCode:
                                    result.Description = res.Args.Description;
                                    break;
                                default: //in case new kinds are added in new compiler versions
                                    continue;
                            }

                            list.Add(result);
                        }
                    }
                    else if (status == HaxeCompleteStatus.ERROR)
                    {
                        PluginBase.RunAsync(() =>
                        {
                            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
                            TraceManager.Add(hc.Errors, (int)TraceType.Error);
                        });
                        callback(list);
                        return;
                    }

                    if (i1 == files.Length - 1)
                        callback(list);
                });
            }
        }
    }
}
