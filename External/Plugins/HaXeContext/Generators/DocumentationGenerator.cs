using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Utilities;

namespace HaXeContext.Generators
{
    public class DocumentationGenerator : ASCompletion.Generators.DocumentationGenerator
    {
        protected override void GenerateDocumentation(string context)
        {
            var sci = ASContext.CurSciControl;
            if (sci == null) return;
            var position = sci.CurrentPos;
            var line = sci.LineFromPosition(position);
            var indent = sci.LineIndentPosition(line) - sci.PositionFromLine(line);
            var tab = sci.GetLine(line).Substring(0, indent);
            var newline = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            var cbs = PluginBase.Settings.CommentBlockStyle;
            var headerStar = cbs == CommentBlockStyle.Indented ? " *" : "*";
            string bodyStar;
            var enableLeadingAsterisks = ((HaXeSettings) ASContext.Context.Settings).EnableLeadingAsterisks;
            if (enableLeadingAsterisks) bodyStar = headerStar;
            else bodyStar = cbs == CommentBlockStyle.Indented ? "  " : " ";
            var parInd = cbs == CommentBlockStyle.Indented ? "\t" : " ";
            if (!PluginBase.MainForm.Settings.UseTabs) parInd = " ";

            // empty box
            if (context == null)
            {
                sci.ReplaceSel(newline + tab + headerStar + " " + newline + tab + headerStar + "/");
                position += newline.Length + tab.Length + 1 + headerStar.Length;
                sci.SetSel(position, position);
            }
            // method details
            else
            {
                var box = newline + tab + bodyStar + " ";
                var mFun = re_splitFunction.Match(context);
                if (mFun.Success && !re_property.IsMatch(mFun.Groups["fname"].Value))
                {
                    // parameters
                    var list = ParseMethodParameters(mFun.Groups["params"].Value);
                    box = list.Aggregate(box, (current, param) => current + (newline + tab + bodyStar + " @param" + parInd + param.Name));
                    // return type
                    var mType = re_variableType.Match(mFun.Groups["type"].Value);
                    if (mType.Success && !mType.Groups["type"].Value.Equals("void", StringComparison.OrdinalIgnoreCase))
                        box += newline + tab + bodyStar + " @return";
                }
                if (enableLeadingAsterisks) box += newline + tab + headerStar + "/";
                else box += newline + tab + "**" + "/";
                sci.ReplaceSel(box);
                position += newline.Length + tab.Length + 1 + headerStar.Length;
                sci.SetSel(position, position);
            }
        }

        /// <summary>
        /// Returns parameters string as member list
        /// </summary>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Member list</returns>
        private static IEnumerable<MemberModel> ParseMethodParameters(string parameters)
        {
            var list = new List<MemberModel>();
            if (parameters == null) return list;
            var p = parameters.IndexOf('(');
            if (p >= 0) parameters = parameters.Substring(p + 1, parameters.IndexOf(')') - p - 1);
            parameters = parameters.Trim();
            if (parameters.Length == 0) return list;
            var toClean = new[] {' ', '\t', '\n', '\r', '?'};
            var braCount = 0;
            var genCount = 0;
            var sb = new StringBuilder();
            var length = parameters.Length - 1;
            for (var i = 0; i <= length; i++)
            {
                var c = parameters[i];
                if (c == '{') braCount++;
                else if (c == '}') braCount--;
                else if (c == '<') genCount++;
                else if (c == '>' && i > 0 && parameters[i - 1] != '-') genCount--;
                if (braCount == 0 && genCount == 0 && (c == ',' || i == length))
                {
                    var s = sb.ToString();
                    sb.Clear();
                    string name;
                    string type;
                    var pos = s.IndexOf(':');
                    if (pos == -1)
                    {
                        name = s;
                        type = ASContext.Context.Features.objectKey;
                    }
                    else
                    {
                        name = s.Substring(0, pos);
                        type = s.Substring(pos);
                    }
                    var param = new MemberModel {Name = name.Trim(toClean), Type = type.Trim(toClean)};
                    if (param.Name.Length == 0) continue;
                    param.Flags = FlagType.Variable | FlagType.Dynamic;
                    list.Add(param);
                }
                else sb.Append(c);
            }
            return list;
        }
    }
}