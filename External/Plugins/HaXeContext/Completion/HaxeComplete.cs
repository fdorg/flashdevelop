using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore;
using ProjectManager.Projects.Haxe;
using ScintillaNet;
using ASCompletion.Completion;
using System.Threading;
using System.Xml;
using ASCompletion.Model;
using PluginCore.Helpers;
using System.Windows.Forms;
using LitJson;
using PluginCore.Utilities;

namespace HaXeContext
{
    public delegate void HaxeCompleteResultHandler<T>(HaxeComplete hc, T result, HaxeCompleteStatus status);

    public class HaxeComplete
    {
        static readonly Regex reArg =
            new Regex("^(-cp|-resource|-cmd)\\s*([^\"'].*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex reMacro =
            new Regex("^(--macro)\\s*([^\"'].*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static readonly Regex rePosition =
            new Regex("(?<path>.*?):(?<line>[0-9]*): (?<range>characters|lines) (?<start>[0-9]*)-(?<end>[0-9]*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // completion context
        public readonly ScintillaControl Sci;
        public readonly ASExpr Expr;
        public readonly string CurrentWord;
        public readonly bool AutoHide;
        public readonly HaxeCompilerService CompilerService;

        // result
        public HaxeCompleteStatus Status;
        public string Errors;
        HaxeCompleteResult result;
        List<HaxePositionResult> positionResults;
        List<HaxeDiagnosticsResult> diagnosticsResults;

        readonly IHaxeCompletionHandler handler;
        readonly string FileName;
        readonly SemVer haxeVersion;

        public HaxeComplete(ScintillaControl sci, ASExpr expr, bool autoHide, IHaxeCompletionHandler completionHandler, HaxeCompilerService compilerService, SemVer haxeVersion)
        {
            Sci = sci;
            Expr = expr;
            CurrentWord = Sci.GetWordFromPosition(Sci.CurrentPos);
            AutoHide = autoHide;
            handler = completionHandler;
            CompilerService = compilerService;
            Status = HaxeCompleteStatus.NONE;
            FileName = sci.FileName;
            this.haxeVersion = haxeVersion;
        }

        /* EXECUTION */

        public void GetList(HaxeCompleteResultHandler<HaxeCompleteResult> callback) => StartThread(callback, () => result);

        public void GetPosition(HaxeCompleteResultHandler<HaxePositionResult> callback)
        {
            StartThread(callback, () => !positionResults.IsNullOrEmpty() ? positionResults[0] : null);
        }

        public void GetUsages(HaxeCompleteResultHandler<List<HaxePositionResult>> callback) => StartThread(callback, () => positionResults);

        public void GetDiagnostics(HaxeCompleteResultHandler<List<HaxeDiagnosticsResult>> callback) => StartThread(callback, () => diagnosticsResults);

        void StartThread<T>(HaxeCompleteResultHandler<T> callback, Func<T> resultFunc)
        {
            SaveFile();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Status = ParseLines(handler.GetCompletion(BuildHxmlArgs()?.ToArray(), GetFileContent()));
                Notify(callback, resultFunc());
            });
        }

        protected virtual void SaveFile() => PluginBase.MainForm.CallCommand("Save", nameof(HaxeComplete));

        void Notify<T>(HaxeCompleteResultHandler<T> callback, T result)
        {
            if (Sci.InvokeRequired)
            {
                Sci.BeginInvoke((MethodInvoker)(() => Notify(callback, result)));
                return;
            }
            callback(this, result, Status);
        }

        /* HAXE COMPILER ARGS */

        protected virtual List<string> BuildHxmlArgs()
        {
            // check haxe project
            if (!(PluginBase.CurrentProject is HaxeProject)) return null;
            var project = (HaxeProject) PluginBase.CurrentProject;
            var pos = GetDisplayPosition();

            // Build Haxe command
            var paths = ProjectManager.PluginMain.Settings.GlobalClasspaths.ToArray();
            var hxmlArgs = new List<string>(project.BuildHXML(paths, "Nothing__", true));
            RemoveComments(hxmlArgs);
            QuotePath(hxmlArgs);
            EscapeMacros(hxmlArgs);

            if (CompilerService == HaxeCompilerService.GLOBAL_DIAGNOSTICS)
                hxmlArgs.Add("--display diagnostics");
            else
                hxmlArgs.Add($"--display \"{FileName}\"@{pos}{GetMode()}");

            hxmlArgs.Add("-D use_rtti_doc");
            hxmlArgs.Add("-D display-details");
            if (project.TraceEnabled) hxmlArgs.Add("-debug");
            return hxmlArgs;
        }

        protected virtual string GetFileContent() => null;

        string GetMode()
        {
            return CompilerService switch
            {
                HaxeCompilerService.POSITION => "@position",
                HaxeCompilerService.USAGE => "@usage",
                HaxeCompilerService.TOP_LEVEL => "@toplevel",
                HaxeCompilerService.DIAGNOSTICS => "@diagnostics",
                _ => string.Empty,
            };
        }

        static void RemoveComments(IList<string> hxmlArgs)
        {
            for (var i = 0; i < hxmlArgs.Count; i++)
            {
                var arg = hxmlArgs[i];
                if (string.IsNullOrEmpty(arg)) continue;
                if (arg.StartsWith('#')) // commented line
                    hxmlArgs[i] = "";
            }
        }

        static void EscapeMacros(IList<string> hxmlArgs)
        {
            for (var i = 0; i < hxmlArgs.Count; i++)
            {
                var arg = hxmlArgs[i];
                if (string.IsNullOrEmpty(arg)) continue;
                var m = reMacro.Match(arg);
                if (m.Success)
                    hxmlArgs[i] = m.Groups[1].Value + " \"" + m.Groups[2].Value.Trim(' ', '"', '\'').Replace("\"", "\\\"") + "\"";
            }
        }

        static void QuotePath(IList<string> hxmlArgs)
        {
            for (var i = 0; i < hxmlArgs.Count; i++)
            {
                var arg = hxmlArgs[i];
                if (string.IsNullOrEmpty(arg)) continue;
                var m = reArg.Match(arg);
                if (m.Success)
                    hxmlArgs[i] = m.Groups[1].Value + " \"" + m.Groups[2].Value.Trim(' ', '"', '\'') + "\"";
            }
        }

        protected virtual int GetDisplayPosition()
        {
            var result = Expr.Position;

            switch (CompilerService)
            {
                case HaxeCompilerService.COMPLETION:
                    // locate a . or (
                    while (result > 1 && Sci.CharAt(result - 1) != '.' && Sci.CharAt(result - 1) != '(')
                        result--;
                    break;

                case HaxeCompilerService.POSITION:
                case HaxeCompilerService.USAGE:
                    result = Sci.WordEndPosition(Sci.CurrentPos, true);
                    // necessary to get results with older versions due to a compiler bug
                    if (haxeVersion < "3.3.0") result++;
                    break;
                case HaxeCompilerService.GLOBAL_DIAGNOSTICS:
                case HaxeCompilerService.DIAGNOSTICS:
                    result = 0;
                    break;
            }

            // account for BOM characters
            result += FileHelper.GetEncodingFileInfo(FileName).BomLength;
            return result;
        }

        /* PROCESS RESPONSE */

        HaxeCompleteStatus ParseLines(string lines)
        {
            switch (CompilerService)
            {
                case HaxeCompilerService.DIAGNOSTICS:
                case HaxeCompilerService.GLOBAL_DIAGNOSTICS:
                    try
                    {
                        return ProcessResponse(JsonMapper.ToObject(lines));
                    }
                    catch (Exception)
                    {
                        Errors = lines;
                        return HaxeCompleteStatus.ERROR;
                    }
                default:
                    try
                    {
                        if (!lines.StartsWith('<'))
                        {
                            Errors = lines.Trim();
                            return HaxeCompleteStatus.ERROR;
                        }

                        using TextReader stream = new StringReader(lines);
                        using var reader = new XmlTextReader(stream);
                        return ProcessResponse(reader);
                    }
                    catch (Exception ex)
                    {
                        Errors = "Error parsing Haxe compiler output: " + ex.Message;
                        return HaxeCompleteStatus.ERROR;
                    }
            }
        }

        HaxeCompleteStatus ProcessResponse(JsonData json)
        {
            diagnosticsResults = new List<HaxeDiagnosticsResult>();

            if (!json.IsArray) return HaxeCompleteStatus.ERROR;
            
            foreach (JsonData file in json)
            {
                var path = (string)file["file"];
                var diagnostics = file["diagnostics"];

                foreach (JsonData diag in diagnostics)
                {
                    var range = diag["range"];
                    var args = diag["args"];

                    var result = new HaxeDiagnosticsResult
                    {
                        Kind = (HaxeDiagnosticsKind) (int) diag["kind"],
                        Range = ParseRange(range, path),
                        Severity = (HaxeDiagnosticsSeverity) (int) diag["severity"]
                    };
                    if (args != null)
                    {
                        if (args.IsString)
                        {
                            result.Args = new HaxeDiagnosticsArgs
                            {
                                Description = (string) args
                            };
                        }
                        else if (args.IsObject)
                        {
                            result.Args = new HaxeDiagnosticsArgs
                            {
                                Description = (string) args["description"],
                                Range = ParseRange(args["range"], path)
                            };
                        }
                    }
                    diagnosticsResults.Add(result);
                }
            }
            return HaxeCompleteStatus.DIAGNOSTICS;
        }

        static HaxePositionResult ParseRange(JsonData range, string path)
        {
            if (range is null) return null;

            var start = range["start"];
            var end = range["end"];

            return new HaxePositionResult
            {
                Path = path,
                //RangeType = HaxePositionCompleteRangeType.LINES, //RangeType is a mix of lines and characters
                CharacterStart = (int) start["character"],
                CharacterEnd = (int) end["character"],
                LineStart = (int) start["line"],
                LineEnd = (int) end["line"]
            };
        }

        HaxeCompleteStatus ProcessResponse(XmlReader reader)
        {
            reader.MoveToContent();

            switch (reader.Name)
            {
                case "type":
                    ProcessType(reader);
                    return HaxeCompleteStatus.TYPE;

                case "list":
                    return ProcessList(reader);

                case "il":
                    return ProcessTopLevel(reader);
            }
            return HaxeCompleteStatus.FAILED;
        }

        void ProcessType(XmlReader reader)
        {
            var parts = Expr.Value.Split('.');
            var name = parts[parts.Length - 1];

            var type = new MemberModel();
            type.Name = name;
            type.Comments = reader.GetAttribute("d");
            ExtractType(reader, type);
            result = new HaxeCompleteResult();
            result.Type = type;
        }

        HaxeCompleteStatus ProcessList(XmlReader reader)
        {
            result = new HaxeCompleteResult();
            result.Members = new MemberList();
            positionResults = new List<HaxePositionResult>();
            MemberModel member = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    switch (reader.Name)
                    {
                        case "list":
                            switch (CompilerService)
                            {
                                case HaxeCompilerService.COMPLETION:
                                    result.Members.Sort();
                                    return HaxeCompleteStatus.MEMBERS;

                                case HaxeCompilerService.POSITION:
                                    return HaxeCompleteStatus.POSITION;

                                case HaxeCompilerService.USAGE:
                                    return HaxeCompleteStatus.USAGE;

                                case HaxeCompilerService.TOP_LEVEL:
                                    return HaxeCompleteStatus.TOP_LEVEL;
                            }
                            break;

                        case "i":
                            member = null;
                            break;
                    }
                    continue;
                }
                if (reader.NodeType != XmlNodeType.Element) continue;

                switch (reader.Name)
                {
                    case "i":
                        member = ExtractMember(reader);
                        break;

                    case "d":
                        if (member is null) continue;
                        member.Comments = ReadValue(reader);
                        break;

                    case "t":
                        if (member is null) continue;
                        ExtractType(reader, member);
                        if (!IsOverload(result.Members, member))
                            result.Members.Add(member);
                        break;

                    case "pos":
                        positionResults.Add(ExtractPos(reader));
                        break;
                }
            }

            result.Members.Sort();
            return HaxeCompleteStatus.MEMBERS;
        }

        HaxeCompleteStatus ProcessTopLevel(XmlReader reader)
        {
            result = new HaxeCompleteResult {Members = new MemberList()};
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;
                switch (reader.Name)
                {
                    case "i":
                        var k = reader.GetAttribute("k");
                        switch (k)
                        {
                            case "local":
                            case "member":
                            case "static":
                            case "enum":
                            case "global":
                            case "package":
                            case "type":
                                var t = k == "global" ? reader.GetAttribute("t") : null;
                                reader.Read();
                                var member = new MemberModel {Name = reader.Value};
                                ExtractType(t, member);
                                result.Members.Add(member);
                                break;
                        }
                        break;
                }
            }
            result.Members.Sort();
            return HaxeCompleteStatus.TOP_LEVEL;
        }

        protected virtual HaxePositionResult ExtractPos(XmlReader reader)
        {
            var result = new HaxePositionResult();

            var value = ReadValue(reader);
            var match = rePosition.Match(value);
            result.Path = match.Groups["path"].Value;
            int.TryParse(match.Groups["line"].Value, out result.LineStart);
            var rangeType = match.Groups["range"].Value;
            result.RangeType = rangeType == "lines"
                ? HaxePositionCompleteRangeType.LINES
                : HaxePositionCompleteRangeType.CHARACTERS;

            int.TryParse(match.Groups["start"].Value, out var start);
            int.TryParse(match.Groups["end"].Value, out var end);

            if (result.RangeType == HaxePositionCompleteRangeType.LINES)
            {
                result.LineStart = start;
                result.LineEnd = end;
            }
            else
            {
                result.CharacterStart = start;
                result.CharacterEnd = end;
            }

            return result;
        }

        static bool IsOverload(MemberList members, MemberModel member)
        {
            return members.Count > 0 && members[members.Count - 1].FullName == member.FullName;
        }

        static MemberModel ExtractMember(XmlReader reader)
        {
            var name = reader.GetAttribute("n");
            if (name is null) return null;

            var result = new MemberModel();
            result.Name = name;
            result.Access = Visibility.Public;
            result.Flags = reader.GetAttribute("k") switch
            {
                "var" => FlagType.Variable,
                "method" => FlagType.Function,
                _ => (FlagType) 0,
            };
            return result;
        }

        static void ExtractType(XmlReader reader, MemberModel member)
        {
            var type = ReadValue(reader);
            ExtractType(type, member);
        }

        static void ExtractType(string type, MemberModel member)
        {
            // Package or Class
            if (string.IsNullOrEmpty(type))
            {
                if (member.Flags != 0) return;
                member.Flags = char.IsLower(member.Name[0])
                    ? FlagType.Package
                    : FlagType.Class;
            }
            // Function or Variable
            else
            {
                var types = type.Split(new[] {"->"}, StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 1)
                {
                    member.Flags = FlagType.Function;
                    member.Parameters = new List<MemberModel>();
                    for (var i = 0; i < types.Length - 1; i++)
                    {
                        var param = new MemberModel(types[i].Trim(), "", FlagType.ParameterVar, Visibility.Public);
                        member.Parameters.Add(param);
                    }
                    member.Type = types[types.Length - 1].Trim();
                }
                else
                {
                    if (member.Flags == 0) member.Flags = FlagType.Variable;
                    member.Type = type;
                }
            }
        }

        static string ReadValue(XmlReader reader)
        {
            if (reader.IsEmptyElement) return string.Empty;
            reader.Read();
            return reader.Value.Trim();
        }
    }

    public enum HaxeCompleteStatus
    {
        NONE = 0,
        FAILED = 1,
        ERROR = 2,
        TYPE = 3,
        MEMBERS = 4,
        POSITION = 5,
        USAGE = 6,
        DIAGNOSTICS = 7,
        TOP_LEVEL
    }

    public enum HaxeCompilerService
    {
        COMPLETION,

        /// <summary>
        /// Since Haxe 3.2.0
        /// https://haxe.org/manual/cr-completion-position.html
        /// </summary>
        POSITION,

        /// <summary>
        /// Since Haxe 3.2.0
        /// https://haxe.org/manual/cr-completion-usage.html
        /// </summary>
        USAGE,

        /// <summary>
        /// Since Haxe 3.2.0
        /// https://haxe.org/manual/cr-completion-top-level.html
        /// </summary>
        TOP_LEVEL,

        /// <summary>
        /// Since Haxe 3.3.0-rc1
        /// </summary>
        DIAGNOSTICS,

        /// <summary>
        /// Since Haxe 3.3.0-rc1
        /// </summary>
        GLOBAL_DIAGNOSTICS
    }

    public class HaxeDiagnosticsResult
    {
        public HaxeDiagnosticsKind Kind;
        public HaxeDiagnosticsSeverity Severity;
        public HaxePositionResult Range;
        public HaxeDiagnosticsArgs Args;
    }

    public class HaxeDiagnosticsArgs
    {
        public string Description;
        public HaxePositionResult Range;
    }

    public class HaxeCompleteResult
    {
        public MemberModel Type;
        public MemberList Members;
    }

    public class HaxePositionResult
    {
        public string Path;
        public HaxePositionCompleteRangeType RangeType;
        public int LineStart;
        public int LineEnd;
        public int CharacterStart;
        public int CharacterEnd;
    }

    public enum HaxePositionCompleteRangeType
    {
        CHARACTERS,
        LINES
    }

    public enum HaxeDiagnosticsKind
    {
        UnusedImport = 0,
        UnresolvedIdentifier = 1,
        CompilerError = 2,
        RemovableCode = 3
        //there seem to be more kinds, but they are not documented.
    }

    public enum HaxeDiagnosticsSeverity
    {
        INFO = 0,
        ERROR = 1,
        WARNING = 2
    }
}