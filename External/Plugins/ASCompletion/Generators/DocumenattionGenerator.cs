using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;
using PluginCore.Utilities;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public class DocumentationGenerator : IContextualGenerator
    {
        private static Regex re_functionDeclaration = new Regex("[\\s\\w]*[\\s]function[\\s][\\s\\w$]+\\($", ASFileParserRegexOptions.SinglelineComment);
        private static Regex re_splitFunction = new Regex("(?<keys>[\\w\\s]*)[\\s]function[\\s]*(?<fname>[^(]*)\\((?<params>[^()]*)\\)(?<type>.*)", ASFileParserRegexOptions.SinglelineComment);
        private static Regex re_property = new Regex("^(get|set)\\s", RegexOptions.Compiled);
        private static Regex re_variableType = new Regex("[\\s]*:[\\s]*(?<type>[\\w.?*]+)", ASFileParserRegexOptions.SinglelineComment);

        public bool ContextualGenerator(ScintillaControl sci, int position, List<ICompletionListItem> options)
        {
            // is the block before a function declaration?
            var len = sci.TextLength - 1;
            var c = ' ';
            var sb = new StringBuilder();
            while (position < len)
            {
                c = (char)sci.CharAt(position);
                sb.Append(c);
                if (c == '(' || c == ';' || c == '{' || c == '}') break;
                position++;
            }
            var signature = sb.ToString();
            if (re_functionDeclaration.IsMatch(signature))
            {
                // get method signature
                var parCount = c == '(' ? 1 : 0;
                position++;
                while (position < len)
                {
                    c = (char)sci.CharAt(position);
                    sb.Append(c);
                    if (c == '(') parCount++;
                    else if (c == ')') parCount--;
                    else if (c == ';' || (parCount == 0 && c == '{')) break;
                    position++;
                }
                signature = sb.ToString();
            }
            else signature = null;
            if (signature != null) options.Add(new GeneratorItem(TextHelper.GetString("Label.CompleteDocDetails")) {Context = signature});
            options.Add(new GeneratorItem(TextHelper.GetString("Label.CompleteDocEmpty")));
            CompletionList.Show(options, true, "");
            return true;
        }

        private static void GenerateDocumentation(string context)
        {
            // get indentation
            var sci = ASContext.CurSciControl;
            if (sci == null) return;
            var position = sci.CurrentPos;
            var line = sci.LineFromPosition(position);
            var indent = sci.LineIndentPosition(line) - sci.PositionFromLine(line);
            var tab = sci.GetLine(line).Substring(0, indent);
            var newline = LineEndDetector.GetNewLineMarker(sci.EOLMode);

            var cbs = PluginBase.Settings.CommentBlockStyle;
            var star = cbs == CommentBlockStyle.Indented ? " *" : "*";
            var parInd = cbs == CommentBlockStyle.Indented ? "\t" : " ";
            if (!PluginBase.MainForm.Settings.UseTabs) parInd = " ";

            // empty box
            if (context == null)
            {
                sci.ReplaceSel(newline + tab + star + " " + newline + tab + star + "/");
                position += newline.Length + tab.Length + 1 + star.Length;
                sci.SetSel(position, position);
            }
            // method details
            else
            {
                var box = newline + tab + star + " ";
                var mFun = re_splitFunction.Match(context);
                if (mFun.Success && !re_property.IsMatch(mFun.Groups["fname"].Value))
                {
                    // parameters
                    var list = ParseMethodParameters(mFun.Groups["params"].Value);
                    foreach (var param in list)
                        box += newline + tab + star + " @param" + parInd + param.Name;
                    // return type
                    var mType = re_variableType.Match(mFun.Groups["type"].Value);
                    if (mType.Success && !mType.Groups["type"].Value.Equals("void", StringComparison.OrdinalIgnoreCase))
                        box += newline + tab + star + " @return"; //+mType.Groups["type"].Value;
                }
                box += newline + tab + star + "/";
                sci.ReplaceSel(box);
                position += newline.Length + tab.Length + 1 + star.Length;
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
            var toClean = new[] {' ', '\t', '\n', '\r', '*', '?'};
            foreach (var parameter in parameters.Split(','))
            {
                var parType = parameter.Split(':');
                var param = new MemberModel {Name = parType[0].Trim(toClean)};
                if (param.Name.Length == 0) continue;
                if (parType.Length == 2) param.Type = parType[1].Trim();
                else param.Type = ASContext.Context.Features.objectKey;
                param.Flags = FlagType.Variable | FlagType.Dynamic;
                list.Add(param);
            }
            return list;
        }

        /// <summary>
        /// Box template completion list item
        /// </summary>
        private class GeneratorItem : ICompletionListItem
        {
            public string Context;

            public GeneratorItem(string label)
            {
                Label = label;
            }

            public string Label { get; }

            public string Description => TextHelper.GetString("Label.DocBoxTemplate");

            public Bitmap Icon => (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_TEMPLATE);

            public string Value
            {
                get
                {
                    GenerateDocumentation(Context);
                    return null;
                }
            }
        }
    }
}