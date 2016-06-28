using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Provider;
using HaXeContext;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace CodeRefactor.Commands.Haxe
{
    class HaxeFindAllReferencesCommand : FindAllReferences
    {
        readonly SemVer sdkVersion;
        IHaxeCompletionHandler completionModeHandler;
        FRResults usageResults;

        public HaxeFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations) : base(target, output, ignoreDeclarations)
        {
            sdkVersion = ((Context)ASContext.Context).GetCurrentSDKVersion();
        }

        protected override void ExecutionImplementation()
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.FindingReferences"));
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.SearchingFiles"));
            CreateCompletionHandler();
            var complete = new HaxeComplete(ASContext.CurSciControl, CurrentTarget.Context, false, completionModeHandler, HaxeCompilerService.USAGE, sdkVersion);
            complete.GetUsages(OnUsagesResult);
        }

        void CreateCompletionHandler()
        {
            if (completionModeHandler != null)
            {
                completionModeHandler.Stop();
                completionModeHandler = null;
            }
            var context = ASContext.Context;
            var settings = (HaXeSettings) context.Settings;
            context.Features.externalCompletion = settings.CompletionMode != HaxeCompletionModeEnum.FlashDevelop;
            switch (settings.CompletionMode)
            {
                case HaxeCompletionModeEnum.Compiler:
                    completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcess(""));
                    break;
                case HaxeCompletionModeEnum.CompletionServer:
                    if (settings.CompletionServerPort < 1024) completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcess(""));
                    else
                    {
                        var args = "--wait " + settings.CompletionServerPort;
                        completionModeHandler = new CompletionServerCompletionHandler(CreateHaxeProcess(args), settings.CompletionServerPort);
                        ((CompletionServerCompletionHandler) completionModeHandler).FallbackNeeded += Context_FallbackNeeded;
                    }
                    break;
            }
        }

        static Process CreateHaxeProcess(string args)
        {
            var executablePath = Path.Combine(PluginBase.CurrentSDK.Path ?? "", "haxe.exe");
            if (!File.Exists(executablePath)) return null;
            var result = new Process
            {
                StartInfo =
                {
                    FileName = executablePath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                },
                EnableRaisingEvents = true
            };
            return result;
        }

        void Context_FallbackNeeded(bool notSupported)
        {
            TraceManager.AddAsync("This SDK does not support server mode");
            if (completionModeHandler != null)
            {
                completionModeHandler.Stop();
                completionModeHandler = null;
            }
            completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcess(""));
        }

        void OnUsagesResult(HaxeComplete hc, List<HaxePositionResult> result, HaxeCompleteStatus status)
        {
            if (hc.Sci.InvokeRequired) hc.Sci.BeginInvoke((MethodInvoker)(() => HandleUsageResult(hc, result, status)));
            else HandleUsageResult(hc, result, status);
        }

        void HandleUsageResult(HaxeComplete hc, IEnumerable<HaxePositionResult> result, HaxeCompleteStatus status)
        {
            UserInterfaceManager.ProgressDialog.Reset();
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.ResolvingReferences"));
            usageResults = new FRResults();
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors, -3);
                    break;
                case HaxeCompleteStatus.USAGE:
                    ScintillaControl sci;
                    foreach (var it in result)
                    {
                        var path = it.Path;
                        if (!usageResults.ContainsKey(path)) usageResults[path] = new List<SearchMatch>();
                        var matches = usageResults[path];
                        sci = AssociatedDocumentHelper.LoadDocument(path).SciControl;
                        var line = it.LineStart - 1;
                        var lineStartPosition = sci.PositionFromLine(line);
                        var wordStartPosition = sci.WordStartPosition(lineStartPosition + it.CharacterStart, true);
                        var lineText = sci.GetLine(line);
                        var name = sci.GetWordFromPosition(wordStartPosition);
                        matches.Add(new SearchMatch
                        {
                            Column = it.CharacterStart,
                            Index = wordStartPosition,
                            Line = it.LineStart,
                            LineStart = lineStartPosition,
                            LineEnd = lineStartPosition + sci.MBSafeTextLength(lineText),
                            LineText = lineText,
                            Length = name.Length,
                            Value = name
                        });
                    }
                    if (!IgnoreDeclarationSource)
                    {
                        sci = AssociatedDocumentHelper.LoadDocument(CurrentTarget.InFile.FileName).SciControl;
                        hc = new HaxeComplete(sci, hc.Expr, false, completionModeHandler, HaxeCompilerService.POSITION, sdkVersion);
                        hc.GetPosition(OnPositionResult);
                    }
                    else FindFinished(usageResults);
                    break;
            }
        }

        void OnPositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            if (hc.Sci.InvokeRequired)
            {
                hc.Sci.BeginInvoke((MethodInvoker)(() => HandlePositionResult(hc, result, status)));
            }
            else HandlePositionResult(hc, result, status);
        }

        void HandlePositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors, -3);
                    break;
                case HaxeCompleteStatus.POSITION:
                    var path = result.Path;
                    var sci = AssociatedDocumentHelper.LoadDocument(path).SciControl;
                    var name = RefactoringHelper.GetRefactorTargetName(CurrentTarget);
                    var pattern = string.Format("\\s*(?<name>{0})[^A-z0-9]", name.Replace(".", "\\s*.\\s*"));
                    var line = result.LineStart - 1;
                    var re = new Regex(pattern);
                    for (int i = line; i < line + 2; i++)
                        if (i < sci.LineCount)
                        {
                            var lineText = sci.GetLine(i);
                            var m = re.Match(lineText);
                            if (!m.Success) continue;
                            var column = m.Index + 1;
                            var lineStartPosition = sci.PositionFromLine(i);
                            var lineEndPosition = lineStartPosition + sci.MBSafeTextLength(lineText);
                            if (!usageResults.ContainsKey(path)) usageResults[path] = new List<SearchMatch>();
                            usageResults[path].Add(new SearchMatch
                            {
                                Column = column,
                                Index = lineStartPosition + column,
                                Line = result.LineStart,
                                LineStart = lineStartPosition,
                                LineEnd = lineEndPosition,
                                LineText = lineText,
                                Length = name.Length,
                                Value = name
                            });
                            break;
                        }
                    break;
            }
            FindFinished(usageResults);
        }
    }
}
