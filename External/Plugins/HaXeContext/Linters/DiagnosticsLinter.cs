using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASCompletion.Completion;
using ASCompletion.Context;
using LintingHelper;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;

namespace HaXeContext.Linters
{
    internal class DiagnosticsLinter : ILintProvider
    {
        readonly ProcessingQueue fileQueue;

        public DiagnosticsLinter(HaXeSettings settings)
        {
            fileQueue = new ProcessingQueue(settings.MaximumDiagnosticsProcesses <= 0 ? 5 : settings.MaximumDiagnosticsProcesses);
        }

        public void LintAsync(IEnumerable<string> files, LintCallback callback)
        {
            var context = ASContext.GetLanguageContext("haxe") as Context;

            if (context == null || !(PluginBase.CurrentProject is ProjectManager.Projects.Haxe.HaxeProject) || !CanContinue(context)) return;
            
            var total = files.Count();
            var list = new List<LintingResult>();

            String untitledFileStart = TextHelper.GetString("FlashDevelop.Info.UntitledFileStart");
            foreach (var file in files)
            {
                if (!File.Exists(file) || file.StartsWithOrdinal(untitledFileStart))
                {
                    total--;
                    continue;
                }

                var sci = GetStubSci(file);

                var hc = context.GetHaxeComplete(sci, new ASExpr { Position = 0 }, true, HaxeCompilerService.DIAGNOSTICS);

                fileQueue.Run(finished =>
                {
                    hc.GetDiagnostics((complete, results, status) =>
                    {
                        total--;

                        sci.Dispose();

                        AddDiagnosticsResults(list, status, results, hc);

                        if (total == 0)
                            callback(list);

                        finished();
                    });
                });
            }
        }

        public void LintProjectAsync(IProject project, LintCallback callback)
        {
            var context = ASContext.GetLanguageContext("haxe") as Context;
            if (context == null || !CanContinue(context)) return;

            var list = new List<LintingResult>();
            var sci = GetStubSci();
            var hc = context.GetHaxeComplete(sci, new ASExpr { Position = 0 }, true, HaxeCompilerService.GLOBAL_DIAGNOSTICS);

            hc.GetDiagnostics((complete, results, status) =>
            {
                sci.Dispose();
                
                AddDiagnosticsResults(list, status, results, hc);

                callback(list);
            });
        }

        static bool CanContinue(Context context)
        {
            var settings = (HaXeSettings) context.Settings;
            var completionMode = settings.CompletionMode;
            if (completionMode == HaxeCompletionModeEnum.FlashDevelop) return false;
            if ((settings.EnabledFeatures & CompletionFeatures.Diagnostics) == 0) return false;
            var haxeVersion = context.GetCurrentSDKVersion();

            return haxeVersion >= "3.3.0";
        }

        static ScintillaControl GetStubSci(string filename = "") => new ScintillaControl
        {
            FileName = filename,
            ConfigurationLanguage = "haxe"
        };

        void AddDiagnosticsResults(List<LintingResult> list, HaxeCompleteStatus status, List<HaxeDiagnosticsResult> results, HaxeComplete hc)
        {
            if (status == HaxeCompleteStatus.DIAGNOSTICS && results != null)
            {
                foreach (var res in results)
                {
                    var range = res.Range ?? res.Args.Range;

                    var result = new LintingResult
                    {
                        File = range.Path,
                        FirstChar = range.CharacterStart,
                        Length = range.CharacterEnd - range.CharacterStart,
                        Line = range.LineStart + 1,
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
                    TraceManager.Add(hc.Errors, (int)TraceType.Error);
                });
            }
        }
    }

    class ProcessingQueue
    {
        readonly BlockingCollection<Action<Action>> queue = new BlockingCollection<Action<Action>>(new ConcurrentQueue<Action<Action>>());
        readonly int maxRunning;
        readonly HashSet<Task> running = new HashSet<Task>(); //TODO: running is modified on different threads

        public ProcessingQueue(int maxConcurrent)
        {
            maxRunning = maxConcurrent;
        }

        public void Run(Action<Action> action)
        {
            lock (running)
            {
                if (running.Count < maxRunning)
                {
                    running.Add(CreateTask(action));
                }
                else
                {
                    queue.Add(action);
                }
            }
        }

        void TaskFinished(Task task)
        {
            lock (running)
                running.Remove(task);

            Action<Action> act;
            if (queue.TryTake(out act))
            {
                Task t = CreateTask(act);

                lock (running)
                    running.Add(t);
            }
        }

        Task CreateTask(Action<Action> action)
        {
            Task task = null;
            task = Task.Factory.StartNew(() => action(() => TaskFinished(task)));

            return task;
        }

    }
}
