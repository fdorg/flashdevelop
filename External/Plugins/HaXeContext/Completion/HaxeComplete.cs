using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using PluginCore;
using ProjectManager.Projects.Haxe;
using ScintillaNet;
using ASCompletion.Completion;
using System.Threading;
using System.Xml;
using ASCompletion.Model;
using PluginCore.Helpers;
using System.Windows.Forms;

namespace HaXeContext
{
    internal delegate void HaxeCompleteResultHandler<T>(HaxeComplete hc, T result, HaxeCompleteStatus status);

    internal class HaxeComplete
    {
        static readonly Regex reArg =
            new Regex("^(-cp|-resource)\\s*([^\"'].*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

        readonly IHaxeCompletionHandler handler;
        readonly string FileName;

        public HaxeComplete(ScintillaControl sci, ASExpr expr, bool autoHide, IHaxeCompletionHandler completionHandler, HaxeCompilerService compilerService)
        {
            Sci = sci;
            Expr = expr;
            CurrentWord = Sci.GetWordFromPosition(Sci.CurrentPos);
            AutoHide = autoHide;
            handler = completionHandler;
            CompilerService = compilerService;
            Status = HaxeCompleteStatus.NONE;
            FileName = PluginBase.MainForm.CurrentDocument.FileName;
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

        private void StartThread<T>(HaxeCompleteResultHandler<T> callback, Func<T> resultFunc)
        {
            PluginBase.MainForm.CallCommand("Save", null);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Status = ParseLines(handler.GetCompletion(BuildHxmlArgs()));
                Notify(callback, resultFunc());
            });
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

        string[] BuildHxmlArgs()
        {
            // check haxe project & context
            if (PluginBase.CurrentProject == null || !(PluginBase.CurrentProject is HaxeProject)
                || !(ASContext.Context is Context))
                return null;

            var hxproj = (PluginBase.CurrentProject as HaxeProject);
            var pos = GetDisplayPosition();

            // Build Haxe command
            var paths = ProjectManager.PluginMain.Settings.GlobalClasspaths.ToArray();
            var hxmlArgs = new List<String>(hxproj.BuildHXML(paths, "Nothing__", true));
            RemoveComments(hxmlArgs);
            QuotePath(hxmlArgs);
            EscapeMacros(hxmlArgs);

            hxmlArgs.Insert(0, String.Format("--display \"{0}\"@{1}{2}", FileName, pos, GetMode()));
            hxmlArgs.Insert(1, "-D use_rtti_doc");
            hxmlArgs.Insert(2, "-D display-details");
            
            if (hxproj.TraceEnabled) hxmlArgs.Insert(2, "-debug");

            return hxmlArgs.ToArray();
        }

        private string GetMode()
        {
            switch (CompilerService)
            {
                case HaxeCompilerService.POSITION:
                    return "@position";

                case HaxeCompilerService.USAGE:
                    return "@usage";
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
                    pos = Sci.WordEndPosition(Sci.CurrentPos, true) + 1;
                    break;
            }
            
            // account for BOM characters
            pos += FileHelper.GetEncodingFileInfo(FileName).BomLength;
            return pos;
        }

        /* PROCESS RESPONSE */

        HaxeCompleteStatus ParseLines(string lines)
        {
            if (!lines.StartsWith('<'))
            {
                Errors = lines.Trim();
                return HaxeCompleteStatus.ERROR;
            }

            try 
            {
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

            }
            return HaxeCompleteStatus.FAILED;
        }

        void ProcessType(XmlTextReader reader)
        {
            string[] parts = Expr.Value.Split('.');
            string name = parts[parts.Length - 1];

            var type = new MemberModel();
            type.Name = name;
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

        MemberModel ExtractMember(XmlTextReader reader)
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

        void ExtractType(XmlTextReader reader, MemberModel member)
        {
            var type = ReadValue(reader);

            // Package or Class
            if (string.IsNullOrEmpty(type))
            {
                if (member.Flags != 0) return;

                if (Char.IsLower(member.Name[0]))
                    member.Flags = FlagType.Package;
                else
                    member.Flags = FlagType.Class;
            }
            // Function or Variable
            else
            {
                string[] types = type.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 1)
                {
                    member.Flags = FlagType.Function;
                    member.Parameters = new List<MemberModel>();
                    for (int i = 0; i < types.Length - 1; i++)
                    {
                        MemberModel param = new MemberModel(types[i].Trim(), "", FlagType.ParameterVar, Visibility.Public);
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

        string ReadValue(XmlTextReader reader)
        {
            if (reader.IsEmptyElement) return string.Empty;
            reader.Read();
            return reader.Value.Trim();
        }
    }

    enum HaxeCompleteStatus: int
    {
        NONE = 0,
        FAILED = 1,
        ERROR = 2,
        TYPE = 3,
        MEMBERS = 4,
        POSITION = 5,
        USAGE = 6
    }

    enum HaxeCompilerService
    {
        COMPLETION,
        POSITION,
        USAGE
    }

    class HaxeCompleteResult
    {
        public MemberModel Type;
        public MemberList Members;
    }

    class HaxePositionResult
    {
        public string Path;
        public HaxePositionCompleteRangeType RangeType;
        public int LineStart;
        public int LineEnd;
        public int CharacterStart;
        public int CharacterEnd;
    }

    enum HaxePositionCompleteRangeType
    {
        CHARACTERS,
        LINES
    }
}
