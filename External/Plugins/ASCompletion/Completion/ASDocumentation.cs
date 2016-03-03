/*
 * Documentation completion/generation
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public class CommentBlock
    {
        public string Description;
        public string InfoTip;
        public string Return;
        public bool IsFunctionWithArguments;
        public ArrayList ParamName; // TODO: change ArrayList for List<string>
        public ArrayList ParamDesc;
        public ArrayList TagName;
        public ArrayList TagDesc;
    }
    
    public class ASDocumentation
    {
        static private List<ICompletionListItem> docVariables;
        static private BoxItem boxSimpleClose;
        static private BoxItem boxMethodParams;
        
        #region regular_expressions
        static private Regex re_splitFunction = new Regex("(?<keys>[\\w\\s]*)[\\s]function[\\s]*(?<fname>[^(]*)\\((?<params>[^()]*)\\)(?<type>.*)",
                                                          ASFileParserRegexOptions.SinglelineComment);
        static private Regex re_property = new Regex("^(get|set)\\s", RegexOptions.Compiled);
        static private Regex re_variableType = new Regex("[\\s]*:[\\s]*(?<type>[\\w.?*]+)", ASFileParserRegexOptions.SinglelineComment);
        static private Regex re_functionDeclaration = new Regex("[\\s\\w]*[\\s]function[\\s][\\s\\w$]+\\($", ASFileParserRegexOptions.SinglelineComment);
        static private Regex re_tags = new Regex("<[/]?(p|br)[/]?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion
        
        #region Comment generation
        static ASDocumentation()
        {
            boxSimpleClose = new BoxItem(TextHelper.GetString("Label.CompleteDocEmpty"));
            boxMethodParams = new BoxItem(TextHelper.GetString("Label.CompleteDocDetails"));
        }
        
        static public bool OnChar(ScintillaControl Sci, int Value, int position, int style)
        {
            if (style == 3 || style == 124)
            {
                switch (Value)
                {
                    // documentation tag
                    case '@':
                        return HandleDocTagCompletion(Sci);
                    
                    // documentation bloc
                    case '*':
                        if ((position > 2) && (Sci.CharAt(position-3) == '/') && (Sci.CharAt(position-2) == '*')
                            && ((position == 3) || (Sci.BaseStyleAt(position-4) != 3)))
                        HandleBoxCompletion(Sci, position);
                        break;
                }
            }
            return false;
        }
        
        static private void CompleteTemplate(string Context)
        {
            // get indentation
            ScintillaControl Sci = ASContext.CurSciControl;
            if (Sci == null) return;
            int position = Sci.CurrentPos;
            int line = Sci.LineFromPosition(position);
            int indent = Sci.LineIndentPosition(line) - Sci.PositionFromLine(line);
            string tab = Sci.GetLine(line).Substring(0, indent);
            // get EOL
            int eolMode = Sci.EOLMode;
            string newline = LineEndDetector.GetNewLineMarker(eolMode);

            CommentBlockStyle cbs = PluginBase.Settings.CommentBlockStyle;
            string star = cbs == CommentBlockStyle.Indented ? " *" : "*";
            string parInd = cbs == CommentBlockStyle.Indented ? "\t" : " ";
            if (!PluginBase.MainForm.Settings.UseTabs) parInd = " ";
            
            // empty box
            if (Context == null)
            {
                Sci.ReplaceSel(newline + tab + star + " " + newline + tab + star + "/");
                position += newline.Length + tab.Length + 1 + star.Length;
                Sci.SetSel(position, position);
            }

            // method details
            else
            {
                string box = newline + tab + star + " ";
                Match mFun = re_splitFunction.Match(Context);
                if (mFun.Success && !re_property.IsMatch(mFun.Groups["fname"].Value))
                {
                    // parameters
                    MemberList list = ParseMethodParameters(mFun.Groups["params"].Value);
                    foreach (MemberModel param in list)
                        box += newline + tab + star + " @param" + parInd + param.Name;
                    // return type
                    Match mType = re_variableType.Match(mFun.Groups["type"].Value);
                    if (mType.Success && !mType.Groups["type"].Value.Equals("void", StringComparison.OrdinalIgnoreCase))
                        box += newline + tab + star + " @return"; //+mType.Groups["type"].Value;
                }
                box += newline + tab + star + "/";
                Sci.ReplaceSel(box);
                position += newline.Length + tab.Length + 1 + star.Length;
                Sci.SetSel(position, position);
            }
        }

        /// <summary>
        /// Returns parameters string as member list
        /// </summary>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Member list</returns>
        static private MemberList ParseMethodParameters(string parameters)
        {
            MemberList list = new MemberList();
            if (parameters == null)
                return list;
            int p = parameters.IndexOf('(');
            if (p >= 0)
                parameters = parameters.Substring(p + 1, parameters.IndexOf(')') - p - 1);
            parameters = parameters.Trim();
            if (parameters.Length == 0)
                return list;
            string[] sparam = parameters.Split(',');
            string[] parType;
            MemberModel param;
            char[] toClean = new char[] { ' ', '\t', '\n', '\r', '*', '?' };
            foreach (string pt in sparam)
            {
                parType = pt.Split(':');
                param = new MemberModel();
                param.Name = parType[0].Trim(toClean);
                if (param.Name.Length == 0)
                    continue;
                if (parType.Length == 2) param.Type = parType[1].Trim();
                else param.Type = ASContext.Context.Features.objectKey;
                param.Flags = FlagType.Variable | FlagType.Dynamic;
                list.Add(param);
            }
            return list;
        }
        
        static private bool HandleDocTagCompletion(ScintillaControl Sci)
        {
            if (ASContext.CommonSettings.JavadocTags == null || ASContext.CommonSettings.JavadocTags.Length == 0)
                return false;

            string txt = Sci.GetLine(Sci.CurrentLine).TrimStart();
            if (!Regex.IsMatch(txt, "^\\*[\\s]*\\@"))
                return false;
            
            // build tag list
            if (docVariables == null)
            {
                docVariables = new List<ICompletionListItem>();
                TagItem item;
                foreach (string tag in ASContext.CommonSettings.JavadocTags)
                {
                    item = new TagItem(tag);
                    docVariables.Add(item);
                }               
            }
            
            // show
            CompletionList.Show(docVariables, true, "");
            return true;
        }
        
        static private bool HandleBoxCompletion(ScintillaControl Sci, int position)
        {
            // is the block before a function declaration?
            int len = Sci.TextLength-1;
            char c;
            StringBuilder sb = new StringBuilder();
            while (position < len)
            {
                c = (char)Sci.CharAt(position);
                sb.Append(c);
                if (c == '(' || c == ';' || c == '{' || c == '}') break;
                position++;
            }
            string signature = sb.ToString();
            if (re_functionDeclaration.IsMatch(signature))
            {
                // get method signature
                position++;
                while (position < len)
                {
                    c = (char)Sci.CharAt(position);
                    sb.Append(c);
                    if (c == ';' || c == '{') break;
                    position++;
                }
                signature = sb.ToString();
            }
            else signature = null;
            
            // build templates list
            List<ICompletionListItem> templates = new List<ICompletionListItem>();
            if (signature != null)
            {
                boxMethodParams.Context = signature;
                templates.Add(boxMethodParams);
            }
            templates.Add(boxSimpleClose);
            
            // show
            CompletionList.Show(templates, true, "");
            return true;
        }
        
        
        /// <summary>
        /// Box template completion list item
        /// </summary>
        private class BoxItem : ICompletionListItem
        {
            private string label;
            public string Context;
            
            public BoxItem(string label) 
            {
                this.label = label;
            }
            
            public string Label { 
                get { return label; }
            }
            public string Description { 
                get { return TextHelper.GetString("Label.DocBoxTemplate"); }
            }
            
            public Bitmap Icon {
                get { return (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_TEMPLATE); }
            }
            
            public string Value { 
                get {
                    CompleteTemplate(Context);
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Documentation tag template completion list item
        /// </summary>
        private class TagItem : ICompletionListItem
        {
            private string label;
            
            public TagItem(string label) 
            {
                this.label = label;
            }
            
            public string Label { 
                get { return label; }
            }
            public string Description {
                get { return TextHelper.GetString("Label.DocTagTemplate"); }
            }
            
            public Bitmap Icon {
                get { return (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_DECLARATION); }
            }
            
            public string Value { 
                get { return label; }
            }
        }
        #endregion
        
        #region Tooltips

        static private Regex reNewLine = new Regex("[\r\n]+", RegexOptions.Compiled);
        static private Regex reKeepTags = new Regex("<([/]?(b|i|s|u))>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static private Regex reSpecialTags = new Regex("<([/]?)(code|small|strong|em)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static private Regex reStripTags = new Regex("<[/]?[a-z]+[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static private Regex reDocTags = new Regex("\n@(?<tag>[a-z]+)\\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static private Regex reSplitParams = new Regex("(?<var>[\\w$]+)\\s", RegexOptions.Compiled);

        static public CommentBlock ParseComment(string comment)
        {
            // cleanup
            comment = comment.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ");
            comment = reKeepTags.Replace(comment, "[$1]");
            comment = reSpecialTags.Replace(comment, match =>
            {
                string tag = match.Groups[2].Value;
                bool open = match.Groups[1].Length == 0;
                switch (tag)
                {
                    case "small": return open ? "[size=-2]" : "[/size]";
                    case "code": return open ? "[font=Courier New]" : "[/font]";
                    case "strong": return open ? "[b]" : "[/b]";
                    case "em": return open ? "[i]" : "[/i]";
                }
                return "";
            });
            comment = reStripTags.Replace(comment, "");
            string[] lines = reNewLine.Split(comment);
            char[] trim = new char[] { ' ', '\t', '*' };
            bool addNL = false;
            comment = "";
            foreach (string line in lines)
            {
                string temp = line.Trim(trim);
                if (addNL) comment += '\n' + temp;
                else { comment += temp; addNL = true; }
            }
            // extraction
            CommentBlock cb = new CommentBlock();
            MatchCollection tags = reDocTags.Matches(comment);
            
            if (tags.Count == 0)
            {
                cb.Description = comment.Trim();
                return cb;
            }
            
            if (tags[0].Index > 0) cb.Description = comment.Substring(0, tags[0].Index).Trim();
            else cb.Description = "";
            cb.TagName = new ArrayList();
            cb.TagDesc = new ArrayList();
            
            Group gTag;
            for(int i=0; i<tags.Count; i++)
            {
                gTag = tags[i].Groups["tag"];
                string tag = gTag.Value;
                int start = gTag.Index+gTag.Length;
                int end = (i<tags.Count-1) ? tags[i+1].Index : comment.Length;
                string desc = comment.Substring(start, end-start).Trim();
                if (tag == "param")
                {
                    Match mParam = reSplitParams.Match(desc);
                    if (mParam.Success)
                    {
                        Group mVar = mParam.Groups["var"];
                        if (cb.ParamName == null) {
                            cb.ParamName = new ArrayList();
                            cb.ParamDesc = new ArrayList();
                        }
                        cb.ParamName.Add(mVar.Value);
                        cb.ParamDesc.Add(desc.Substring(mVar.Index + mVar.Length).TrimStart());
                    }
                }
                else if (tag == "return")
                {
                    cb.Return = desc;
                }
                else if (tag == "infotip")
                {
                    cb.InfoTip = desc;
                    if (cb.Description.Length == 0) cb.Description = cb.InfoTip;
                }
                cb.TagName.Add(tag);
                cb.TagDesc.Add(desc);
            }
            return cb;
            
        }
        
        static public string GetTipDetails(MemberModel member, string highlightParam)
        {
            try
            {
                string tip = (UITools.Manager.ShowDetails) ? GetTipFullDetails(member, highlightParam) : GetTipShortDetails(member, highlightParam);
                // remove paragraphs from comments
                return RemoveHTMLTags(tip);
            }
            catch(Exception ex)
            {
                ErrorManager.ShowError(/*"Error while parsing comments.\n"+ex.Message,*/ ex);
                return "";
            }
        }

        static public string RemoveHTMLTags(string tip)
        {
            return re_tags.Replace(tip, "");
        }
        
        /// <summary>
        /// Short contextual details to display in tips
        /// </summary>
        /// <param name="member">Member data</param>
        /// <param name="highlightParam">Parameter to detail</param>
        /// <returns></returns>
        static public string GetTipShortDetails(MemberModel member, string highlightParam)
        {
            if (member == null || member.Comments == null || !ASContext.CommonSettings.SmartTipsEnabled) return "";
            CommentBlock cb = ParseComment(member.Comments);
            cb.IsFunctionWithArguments = IsFunctionWithArguments(member);
            return " \u2026" + GetTipShortDetails(cb, highlightParam);
        }

        static bool IsFunctionWithArguments(MemberModel member)
        {
            return member != null && (member.Flags & FlagType.Function) > 0
                && member.Parameters != null && member.Parameters.Count > 0;
        }

        /// <summary>
        /// Short contextual details to display in tips
        /// </summary>
        /// <param name="cb">Parsed comments</param>
        /// <returns>Formated comments</returns>
        static public string GetTipShortDetails(CommentBlock cb, string highlightParam)
        {
            string details = "";
            
            // get parameter detail
            if (!string.IsNullOrEmpty(highlightParam) && cb.ParamName != null)
            {
                for(int i=0; i<cb.ParamName.Count; i++)
                {
                    if (highlightParam == (string)cb.ParamName[i])
                    {
                        details += "\n" + MethodCallTip.HLTextStyleBeg + highlightParam + ":" + MethodCallTip.HLTextStyleEnd 
                                + " " + Get2LinesOf((string)cb.ParamDesc[i], true).TrimStart();
                        return details;
                    }
                }
            }
            // get description extract
            if (ASContext.CommonSettings.SmartTipsEnabled)
            {
                if (!string.IsNullOrEmpty(cb.InfoTip))
                    details += "\n"+cb.InfoTip;
                else if (!string.IsNullOrEmpty(cb.Description)) 
                    details += Get2LinesOf(cb.Description, cb.IsFunctionWithArguments);
            }

            return details;
        }

        static private string GetShortcutDocs()
        {
            Color themeForeColor = PluginBase.MainForm.GetThemeColor("MethodCallTip.InfoColor");
            string foreColorString = themeForeColor != Color.Empty ? DataConverter.ColorToHex(themeForeColor).Replace("0x", "#") : "#666666:MULTIPLY";
            return "\n[COLOR=" + foreColorString + "][i](" + TextHelper.GetString("Info.ShowDetails") + ")[/i][/COLOR]";
        }

        /// <summary>
        /// Split multiline text and return 2 lines or less of text
        /// </summary>
        static public string Get2LinesOf(string text)
        {
            return Get2LinesOf(text, false);
        }

        static public string Get2LinesOf(string text, bool alwaysAddShortcutDocs)
        {
            string[] lines = text.Split('\n');
            text = "";
            int n = Math.Min(lines.Length, 2);
            for (int i = 0; i < n; i++) text += "\n" + lines[i];
            if (lines.Length > 2 || alwaysAddShortcutDocs) text += " \x86" + GetShortcutDocs();
            return text;
        }
        
        /// <summary>
        /// Extract member comments for display in the completion list
        /// </summary>
        /// <param name="member">Member data</param>
        /// <param name="highlightParam">Parameter to highlight</param>
        /// <returns>Formated comments</returns>
        static public string GetTipFullDetails(MemberModel member, string highlightParam)
        {
            if (member == null || member.Comments == null || !ASContext.CommonSettings.SmartTipsEnabled) return "";
            CommentBlock cb = ParseComment(member.Comments);
            cb.IsFunctionWithArguments = IsFunctionWithArguments(member);
            return GetTipFullDetails(cb, highlightParam);
        }

        /// <summary>
        /// Extract comments for display in the completion list
        /// </summary>
        /// <param name="cb">Parsed comments</param>
        /// <returns>Formated comments</returns>
        static public string GetTipFullDetails(CommentBlock cb, string highlightParam)
        {
            string details = "";
            if (cb.Description.Length > 0) 
            {
                string[] lines = cb.Description.Split('\n');
                int n = Math.Min(lines.Length, ASContext.CommonSettings.DescriptionLinesLimit);
                for(int i=0; i<n; i++) details += lines[i]+"\n";
                if (lines.Length > ASContext.CommonSettings.DescriptionLinesLimit) details = details.TrimEnd() + " \u2026\n";
            }
            
            // @usage
            if (cb.TagName != null)
            {
                bool hasUsage = false;
                for(int i=0; i<cb.TagName.Count; i++)
                if ((string)cb.TagName[i] == "usage") 
                {
                    hasUsage = true;
                    details += "\n    "+(string)cb.TagDesc[i];
                }
                if (hasUsage) details += "\n";
            }
            
            // @param
            if (cb.ParamName != null && cb.ParamName.Count > 0)
            {
                details += "\nParam:";
                for(int i=0; i<cb.ParamName.Count; i++)
                {
                    details += "\n    ";
                    if (highlightParam == (string)cb.ParamName[i])
                    {
                        details += MethodCallTip.HLBgStyleBeg 
                                + MethodCallTip.HLTextStyleBeg + highlightParam + ":" + MethodCallTip.HLTextStyleEnd + " "
                                + (string)cb.ParamDesc[i] 
                                + MethodCallTip.HLBgStyleEnd;
                    }
                    else details += cb.ParamName[i] + ": " + (string)cb.ParamDesc[i];
                }
            }
            
            // @return
            if (cb.Return != null)
            {
                details += "\n\nReturn:\n    "+cb.Return;
            }
            return "\n\n"+details.Trim();
        }
        #endregion
    }

}
