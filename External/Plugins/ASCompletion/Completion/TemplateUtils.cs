using System;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public class TemplateUtils
    {
        public static string boundaries_folder = "boundaries";
        public static string generators_folder = "generators";
        public static string template_variable = @"<<[^\$]*?\$\({0}\).*?>>";

        public static string GetStaticExternOverride(MemberModel member)
        {
            FlagType ft = member.Flags;
            string modifiers = "";
            if ((ft & FlagType.Extern) > 0)
                modifiers += "extern ";
            if ((ft & FlagType.Static) > 0)
                modifiers += "static ";
            if ((ft & FlagType.Override) > 0)
                modifiers += "override ";
            return modifiers;
        }

        public static string GetModifiers(MemberModel member) => GetModifiers(member.Access);

        public static string GetModifiers(Visibility acceess)
        {
            if ((acceess & Visibility.Private) > 0) return "private ";
            if ((acceess & Visibility.Public) > 0) return "public ";
            if ((acceess & Visibility.Protected) > 0) return "protected ";
            if ((acceess & Visibility.Internal) > 0) return "internal ";
            return "";
        }

        public static string ToDeclarationWithModifiersString(MemberModel m, string template)
        {
            var features = ASContext.Context.Features;
            var accessModifier = m.Access == 0 && features.hasNamespaces && !string.IsNullOrEmpty(m.Namespace)
                               ? m.Namespace
                               : GetModifiers(m).Trim();
            if (accessModifier == "private" && features.methodModifierDefault == Visibility.Private
                && !ASContext.CommonSettings.GenerateDefaultModifierDeclaration)
                accessModifier = null;

            string modifiers = null;
            if ((m.Flags & FlagType.Constructor) > 0) modifiers = accessModifier;
            else
            {
                modifiers = GetStaticExternOverride(m);
                if (accessModifier != null) modifiers += accessModifier;
                modifiers = modifiers.Trim();
                if (modifiers.Length == 0) modifiers = null;
            }

            string res = ReplaceTemplateVariable(template, "Modifiers", modifiers);

            // Insert Declaration
            res = ToDeclarationString(m, res);

            return res;
        }

        public static string ToDeclarationString(MemberModel m, string template)
        {
            // Insert Name
            if (m.Name != null)
                template = ReplaceTemplateVariable(template, "Name", m.FullName);
            else
                template = ReplaceTemplateVariable(template, "Name", null);

            // If method, insert arguments
            template = ReplaceTemplateVariable(template, "Arguments", ParametersString(m, true));

            if (!string.IsNullOrEmpty(m.Type))
            {
                if ((m.Flags & FlagType.Setter) > 0 && m.Parameters != null && m.Parameters.Count == 1)
                    template = ReplaceTemplateVariable(template, "Type", FormatType(m.Parameters[0].Type));
                else
                    template = ReplaceTemplateVariable(template, "Type", FormatType(m.Type));
            }
            else
                template = ReplaceTemplateVariable(template, "Type", null);

            template = ReplaceTemplateVariable(template, "Value", m.Value);

            return template;
        }

        public static string ParametersString(MemberModel member, bool formated)
        {
            string template = GetTemplate("FunctionParameter");
            string res = "";
            if (member.Parameters != null && member.Parameters.Count > 0)
            {
                for (int i = 0; i < member.Parameters.Count; i++)
                {
                    MemberModel param = member.Parameters[i];
                    string one = template;

                    if (i + 1 < member.Parameters.Count)
                        one = ReplaceTemplateVariable(one, "PComma", ",");
                    else
                        one = ReplaceTemplateVariable(one, "PComma", null);

                    one = ReplaceTemplateVariable(one, "PName", param.Name);

                    if (!string.IsNullOrEmpty(param.Type))
                        one = ReplaceTemplateVariable(one, "PType", formated ? FormatType(param.Type) : param.Type);
                    else
                        one = ReplaceTemplateVariable(one, "PType", null);

                    if (param.Value != null)
                        one = ReplaceTemplateVariable(one, "PDefaultValue", param.Value.Trim());
                    else
                        one = ReplaceTemplateVariable(one, "PDefaultValue", null);

                    res += one;
                }
            }
            return res;
        }

        public static string CallParametersString(MemberModel member)
        {
            string template = GetTemplate("FunctionParameter");
            string res = "";
            if (member.Parameters != null && member.Parameters.Count > 0)
            {
                for (int i = 0; i < member.Parameters.Count; i++)
                {
                    MemberModel param = member.Parameters[i];
                    string one = template;

                    if (i + 1 < member.Parameters.Count)
                        one = ReplaceTemplateVariable(one, "PComma", ",");
                    else
                        one = ReplaceTemplateVariable(one, "PComma", null);

                    var pname = GetParamName(param);
                    one = ReplaceTemplateVariable(one, "PName", pname);

                    one = ReplaceTemplateVariable(one, "PType", null);
                    one = ReplaceTemplateVariable(one, "PDefaultValue", null);

                    res += one;
                }
            }
            return res;
        }

        public static string ReplaceTemplateVariable(string template, string var, string replace)
        {
            MatchCollection mc = Regex.Matches(template, String.Format(template_variable, var));
            int mcCount = mc.Count;
            if (mcCount > 0)
            {
                var sb = new System.Text.StringBuilder();
                int pos = 0;
                for (int i = 0; i < mcCount; i++)
                {
                    Match m = mc[i];
                    int endIndex = m.Index + m.Length;
                    sb.Append(template.Substring(pos, m.Index - pos));
                    if (replace != null)
                    {
                        string val = m.Value;
                        val = val.Substring(2, val.Length - 4);
                        sb.Append(val);
                    }
                    if (i == mcCount - 1)
                        sb.Append(template.Substring(endIndex));
                    else
                    {
                        int next = mc[i + 1].Index;
                        sb.Append(template.Substring(endIndex, next - endIndex));
                        pos += next;
                    }
                }

                template = sb.ToString();
            }
            template = template.Replace("$(" + var + ")", replace);
            return template;
        }

        private static string FormatType(string type)
        {
            return MemberModel.FormatType(type);
        }

        public static MemberModel GetTemplateBlockMember(ScintillaControl Sci, string blockTmpl)
        {
            if (string.IsNullOrEmpty(blockTmpl))
                return null;

            string firstLine = blockTmpl;
            int lineCount = 0;

            int index = blockTmpl.IndexOf('\n');
            if (index != -1)
            {
                firstLine = blockTmpl.Substring(0, index);
                lineCount = Regex.Matches(blockTmpl, "\n").Count;
            }

            int lineNum = 0;
            while (lineNum < Sci.LineCount)
            {
                string line = Sci.GetLine(lineNum);
                int funcBlockIndex = line.IndexOfOrdinal(firstLine);
                if (funcBlockIndex != -1)
                {
                    MemberModel latest = new MemberModel();
                    latest.LineFrom = lineNum;
                    latest.LineTo = lineNum + lineCount;
                    return latest;
                }
                lineNum++;
            }
            return null;
        }

        /// <summary>
        /// Templates are stored in the plugin's Data folder
        /// </summary>
        public static string GetTemplate(string name, string altName)
        {
            var tmp = GetTemplate(name);
            return tmp == "" ? GetTemplate(altName) : tmp;
        }

        /// <summary>
        /// Templates are stored in the plugin's Data folder
        /// </summary>
        public static string GetTemplate(string name)
        {
            var lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            var path = Path.Combine(PathHelper.SnippetDir, lang, generators_folder, name + ".fds");
            if (!File.Exists(path)) return "";
            string content;
            using (Stream src = File.OpenRead(path))
            {
                using (var sr = new StreamReader(src))
                {
                    content = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return "$(Boundary)" + content.Replace("\r\n", "\n") + "$(Boundary)";
        }

        public static string GetBoundary(string name)
        {
            var lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            var path = Path.Combine(PathHelper.SnippetDir, lang, boundaries_folder, name + ".fds");
            if (!File.Exists(path)) return "";
            string content;
            using (Stream src = File.OpenRead(path))
            {
                using (var sr = new StreamReader(src))
                {
                    content = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return content;
        }

        public static string GetParamName(MemberModel param)
        {
            return (param.Name ?? "").Replace("?", ""); // '?' is a marker for optional arguments
        }
    }
}
