using System;
using System.Collections;
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
    internal delegate void HaXeCompletionResultHandler(HaxeComplete sender, HaxeCompleteStatus status);

    internal class HaxeComplete
    {
        static readonly Regex reArg = new Regex("^(-cp)\\s*([^\"'].*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // completion context
        public readonly ScintillaControl Sci;
        public readonly ASExpr Expr;
        public readonly bool AutoHide;

        // result
        public HaxeCompleteStatus Status;
        public string Errors;
        public MemberModel Type;
        public MemberList Members;

        readonly IHaxeCompletionHandler handler;
        readonly string FileName;

        public HaxeComplete(ScintillaControl sci, ASExpr expr, bool autoHide, IHaxeCompletionHandler completionHandler)
        {
            Sci = sci;
            Expr = expr;
            AutoHide = autoHide;
            handler = completionHandler;
            Status = HaxeCompleteStatus.NONE;
            FileName = PluginBase.MainForm.CurrentDocument.FileName;
        }

        /* EXECUTION */

        public void GetList(HaXeCompletionResultHandler callback)
        {
            PluginBase.MainForm.CallCommand("Save", null);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Status = ParseLines(handler.GetCompletion(BuildHxmlArgs()));
                Notify(callback);
            });
        }

        void Notify(HaXeCompletionResultHandler callback)
        {
            if (Sci.InvokeRequired)
            {
                Sci.BeginInvoke((MethodInvoker)delegate {
                    Notify(callback);
                });
                return;
            }
            callback(this, Status);
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

            // Build haXe command
            var paths = ProjectManager.PluginMain.Settings.GlobalClasspaths.ToArray();
            var hxmlArgs = new List<String>(hxproj.BuildHXML(paths, "Nothing__", true));
            QuotePath(hxmlArgs);

            // Get the current class edited (ensure completion even if class not reference in the project)
            var package = ASContext.Context.CurrentModel.Package;
            if (!string.IsNullOrEmpty(package))
            {
                var cl = ASContext.Context.CurrentModel.Package + "." + GetMainClassName();
                var libToAdd =
                    FileName.Split(
                        new[] {"\\" + String.Join("\\", cl.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries))},
                        StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString();
                hxmlArgs.Add("-cp \"" + libToAdd + "\" " + cl);
            }
            else
                hxmlArgs.Add(GetMainClassName());

            hxmlArgs.Insert(0, String.Format("--display \"{0}\"@{1}", FileName, pos));
            hxmlArgs.Insert(1, "-D use_rtti_doc");
            hxmlArgs.Insert(2, "-D display-details");
            if (hxproj.TraceEnabled) hxmlArgs.Insert(2, "-debug");

            return hxmlArgs.ToArray();
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
                    else if (arg.StartsWith("#")) // commented line
                        hxmlArgs[i] = "";
                }
            }
        }

        string GetMainClassName()
        {
            var start = FileName.LastIndexOf("\\") + 1;
            var end = FileName.LastIndexOf(".");
            return FileName.Substring(start, end - start);
        }

        int GetDisplayPosition()
        {
            var pos = Expr.Position;
            // locate a . or (
            while (pos > 1 && Sci.CharAt(pos - 1) != '.' && Sci.CharAt(pos - 1) != '(')
                pos--;

            // account for BOM characters
            pos += FileHelper.GetEncodingFileInfo(FileName).BomLength;
            return pos;
        }

        /* PROCESS RESPONSE */

        HaxeCompleteStatus ParseLines(string lines)
        {
            if (!lines.StartsWith("<"))
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
                    ProcessMembers(reader);
                    return HaxeCompleteStatus.MEMBERS;
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
            this.Type = type;
        }

        void ProcessMembers(XmlTextReader reader)
        {
            Members = new MemberList();
            MemberModel member = null; 

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    switch (reader.Name)
                    {
                        case "list": return;
                        case "i": member = null; break;
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
                        if (!IsOverload(member))
                            Members.Add(member);
                        break;
                }
            }
        }

        bool IsOverload(MemberModel member)
        {
            return Members.Count > 0 && Members[Members.Count - 1].FullName == member.FullName;
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
        MEMBERS = 4
    }
}
