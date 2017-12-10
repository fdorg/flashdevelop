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
        static readonly Regex reQuote =
            new Regex("([^\"])\"", RegexOptions.Compiled);

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
        private HaxeCompleteResult result;
        private List<HaxePositionResult> positionResults;
        private List<HaxeDiagnosticsResult> diagnosticsResults;

        readonly IHaxeCompletionHandler handler;
        readonly string FileName;
        private readonly SemVer haxeVersion;

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

        public void GetList(HaxeCompleteResultHandler<HaxeCompleteResult> callback)
        {
            StartThread(callback, () => result);
        }

        public void GetPosition(HaxeCompleteResultHandler<HaxePositionResult> callback)
        {
            StartThread(callback, () => positionResults != null && positionResults.Count > 0 ? positionResults[0] : null);
        }

        public void GetUsages(HaxeCompleteResultHandler<List<HaxePositionResult>> callback)
        {
            StartThread(callback, () => positionResults);
        }

        public void GetDiagnostics(HaxeCompleteResultHandler<List<HaxeDiagnosticsResult>> callback)
        {
            StartThread(callback, () => diagnosticsResults);
        }

        private void StartThread<T>(HaxeCompleteResultHandler<T> callback, Func<T> resultFunc)
        {
            SaveFile();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Status = ParseLines(handler.GetCompletion(BuildHxmlArgs(), GetFileContent()));
                Notify(callback, resultFunc());
            });
        }

        protected virtual void SaveFile()
        {
            PluginBase.MainForm.CallCommand("Save", "HaxeComplete");
        }

        void Notify<T>(HaxeCompleteResultHandler<T> callback, T result)
        {
            if (Sci.InvokeRequired)
            {
                Sci.BeginInvoke((MethodInvoker)delegate {
                    Notify(callback, result);
                });
                return;
            }
            callback(this, result, Status);
        }

        /* HAXE COMPILER ARGS */

        protected virtual string[] BuildHxmlArgs()
        {
            // check haxe project
            if (!(PluginBase.CurrentProject is HaxeProject))
                return null;

            var hxproj = (PluginBase.CurrentProject as HaxeProject);
            var pos = GetDisplayPosition();

            // Build Haxe command
            var paths = ProjectManager.PluginMain.Settings.GlobalClasspaths.ToArray();
            var hxmlArgs = new List<String>(hxproj.BuildHXML(paths, "Nothing__", true));
            RemoveComments(hxmlArgs);
            QuotePath(hxmlArgs);
            EscapeMacros(hxmlArgs);

            if (CompilerService == HaxeCompilerService.GLOBAL_DIAGNOSTICS)
                hxmlArgs.Add("--display diagnostics");
            else
                hxmlArgs.Add($"--display \"{FileName}\"@{pos}{GetMode()}");

            hxmlArgs.Add("-D use_rtti_doc");
            hxmlArgs.Add("-D display-details");
            if (hxproj.TraceEnabled) hxmlArgs.Add("-debug");
            
            return hxmlArgs.ToArray();
        }

        protected virtual string GetFileContent()
        {
            return null;
        }

        private string GetMode()
        {
            switch (CompilerService)
            {
                case HaxeCompilerService.POSITION:
                    return "@position";

                case HaxeCompilerService.USAGE:
                    return "@usage";

                case HaxeCompilerService.TOP_LEVEL:
                    return "@toplevel";

                case HaxeCompilerService.DIAGNOSTICS:
                    return "@diagnostics";

                //case HaxeCompilerService.GLOBAL_DIAGNOSTICS:
                //    return "diagnostics";
            }

            return "";
        }

        private void RemoveComments(List<string> hxmlArgs)
        {
            for (int i = 0; i < hxmlArgs.Count; i++)
            {
                string arg = hxmlArgs[i];
                if (!string.IsNullOrEmpty(arg))
                {
                    if (arg.StartsWith('#')) // commented line
                        hxmlArgs[i] = "";
                }
            }
        }

        private void EscapeMacros(List<string> hxmlArgs)
        {
            for (int i = 0; i < hxmlArgs.Count; i++)
            {
                string arg = hxmlArgs[i];
                if (!string.IsNullOrEmpty(arg))
                {
                    Match m = reMacro.Match(arg);
                    if (m.Success)
                        hxmlArgs[i] = m.Groups[1].Value + " \"" + m.Groups[2].Value.Trim() + "\"";
                }
            }
        }

        private string EscapeQuotes(string expr)
        {
            return reQuote.Replace(expr, "$1\\\"");
        }

        void QuotePath(List<string> hxmlArgs)
        {
            for (int i = 0; i < hxmlArgs.Count; i++)
            {
                string arg = hxmlArgs[i];
                if (!string.IsNullOrEmpty(arg))
                {
                    Match m = reArg.Match(arg);
                    if (m.Success)
                        hxmlArgs[i] = m.Groups[1].Value + " \"" + m.Groups[2].Value.Trim() + "\"";
                }
            }
        }

        string GetMainClassName()
        {
            var start = FileName.LastIndexOf('\\') + 1;
            var end = FileName.LastIndexOf('.');
            return FileName.Substring(start, end - start);
        }

        int GetDisplayPosition()
        {
            var pos = Expr.Position;

            switch (CompilerService)
            {
                case HaxeCompilerService.COMPLETION:
                    // locate a . or (
                    while (pos > 1 && Sci.CharAt(pos - 1) != '.' && Sci.CharAt(pos - 1) != '(')
                        pos--;
                    break;

                case HaxeCompilerService.POSITION:
                case HaxeCompilerService.USAGE:
                    pos = Sci.WordEndPosition(Sci.CurrentPos, true);
                    // necessary to get results with older versions due to a compiler bug
                    if (haxeVersion < "3.3.0") pos++;
                    break;
                case HaxeCompilerService.GLOBAL_DIAGNOSTICS:
                case HaxeCompilerService.DIAGNOSTICS:
                    pos = 0;
                    break;
            }
            
            // account for BOM characters
            pos += FileHelper.GetEncodingFileInfo(FileName).BomLength;
            return pos;
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

                        using (TextReader stream = new StringReader(lines))
                        {
                            using (XmlTextReader reader = new XmlTextReader(stream))
                            {
                                return ProcessResponse(reader);
                            }
                        }
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



        HaxePositionResult ParseRange(JsonData range, string path)
        {
            if (range == null) return null;

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

        HaxeCompleteStatus ProcessResponse(XmlTextReader reader)
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

        void ProcessType(XmlTextReader reader)
        {
            string[] parts = Expr.Value.Split('.');
            string name = parts[parts.Length - 1];

            var type = new MemberModel();
            type.Name = name;
            type.Comments = reader.GetAttribute("d");
            ExtractType(reader, type);
            result = new HaxeCompleteResult();
            result.Type = type;
        }

        HaxeCompleteStatus ProcessList(XmlTextReader reader)
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
                else if (reader.NodeType != XmlNodeType.Element)
                    continue;

                switch (reader.Name)
                {
                    case "i":
                        member = ExtractMember(reader);
                        break;

                    case "d":
                        if (member == null) continue;
                        member.Comments = ReadValue(reader);
                        break;

                    case "t":
                        if (member == null) continue;
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
                                string t = k == "global" ? reader.GetAttribute("t") : null;
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

        HaxePositionResult ExtractPos(XmlTextReader reader)
        {
            var result = new HaxePositionResult();

            string value = ReadValue(reader);
            Match match = rePosition.Match(value);
            result.Path = match.Groups["path"].Value;
            int.TryParse(match.Groups["line"].Value, out result.LineStart);
            string rangeType = match.Groups["range"].Value;
            if (rangeType == "lines")
                result.RangeType = HaxePositionCompleteRangeType.LINES;
            else
                result.RangeType = HaxePositionCompleteRangeType.CHARACTERS;

            int start = 0;
            int end = 0;
            int.TryParse(match.Groups["start"].Value, out start);
            int.TryParse(match.Groups["end"].Value, out end);

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

        bool IsOverload(MemberList members, MemberModel member)
        {
            return members.Count > 0 && members[members.Count - 1].FullName == member.FullName;
        } 

        MemberModel ExtractMember(XmlReader reader)
        {
            var name = reader.GetAttribute("n");
            if (name == null) return null;

            var member = new MemberModel();
            member.Name = name;
            member.Access = Visibility.Public;

            var k = reader.GetAttribute("k");
            switch (k)
            {
                case "var": member.Flags = FlagType.Variable; break;
                case "method": member.Flags = FlagType.Function; break;
            }
            return member;
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

                if (char.IsLower(member.Name[0]))
                    member.Flags = FlagType.Package;
                else
                    member.Flags = FlagType.Class;
            }
            // Function or Variable
            else
            {
                var types = type.Split(new[] {"->"}, StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 1)
                {
                    member.Flags = FlagType.Function;
                    member.Parameters = new List<MemberModel>();
                    for (int i = 0; i < types.Length - 1; i++)
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
