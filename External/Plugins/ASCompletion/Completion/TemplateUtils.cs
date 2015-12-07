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

        public static string GetModifiers(MemberModel member)
        {
            string modifiers = "";
            Visibility acc = member.Access;
            if ((acc & Visibility.Private) > 0)
                modifiers += "private ";
            else if ((acc & Visibility.Public) > 0)
                modifiers += "public ";
            else if ((acc & Visibility.Protected) > 0)
                modifiers += "protected ";
            else if ((acc & Visibility.Internal) > 0)
                modifiers += "internal ";
            return modifiers;
        }

        public static string ToDeclarationWithModifiersString(MemberModel m, string template)
        {
            bool isConstructor = (m.Flags & FlagType.Constructor) > 0;

            string methodModifiers;
            if (isConstructor)
                methodModifiers = GetModifiers(m).Trim();
            else
                methodModifiers = (GetStaticExternOverride(m) + GetModifiers(m)).Trim();

            // Insert Modifiers (private, static, etc)
            if (methodModifiers == "private" && ASContext.Context.Features.methodModifierDefault == Visibility.Private)
                methodModifiers = null;
            string res = ReplaceTemplateVariable(template, "Modifiers", methodModifiers);

            // Insert Declaration
            res = ToDeclarationString(m, res);

            return res;
        }

        public static string ToDeclarationString(MemberModel m, string template)
        {
            // Insert Name
            if (m.Name != null)
                template = ReplaceTemplateVariable(template, "Name", m.Name);
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

            if (m.Value != null)
                template = ReplaceTemplateVariable(template, "Value", m.Value);
            else
                template = ReplaceTemplateVariable(template, "Value", null);

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
            Match m = Regex.Match(template, String.Format(template_variable, var));
            if (m.Success)
            {
                if (replace == null)
                {
                    template = template.Substring(0, m.Index) + template.Substring(m.Index + m.Length);
                    return template;
                }
                else
                {
                    string val = m.Value;
                    val = val.Substring(2, val.Length - 4);
                    template = template.Substring(0, m.Index) + val + template.Substring(m.Index + m.Length);
                }
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

            int index = blockTmpl.IndexOf("\n");
            if (index != -1)
            {
                firstLine = blockTmpl.Substring(0, index);
                lineCount = Regex.Matches(blockTmpl, "\n").Count;
            }

            int lineNum = 0;
            while (lineNum < Sci.LineCount)
            {
                string line = Sci.GetLine(lineNum);
                int funcBlockIndex = line.IndexOf(firstLine);
                if (funcBlockIndex != -1)
                {
                    MemberModel latest = new MemberModel();
                    latest.LineFrom = lineNum;
                    latest.LineTo = lineNum;
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
            string tmp = GetTemplate(name);
            if (tmp == "") return GetTemplate(altName);
            else return tmp;
        }

        /// <summary>
        /// Templates are stored in the plugin's Data folder
        /// </summary>
        public static string GetTemplate(string name)
        {
            string lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            string path = Path.Combine(PathHelper.SnippetDir, lang);
            path = Path.Combine(path, generators_folder);
            path = Path.Combine(path, name + ".fds");
            if (!File.Exists(path)) return "";

            Stream src = File.OpenRead(path);
            if (src == null) return "";

            String content;
            using (StreamReader sr = new StreamReader(src))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }
            return "$(Boundary)" + content.Replace("\r\n", "\n") + "$(Boundary)";
        }

        public static string GetBoundary(string name)
        {
            string lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            string path = Path.Combine(PathHelper.SnippetDir, lang);
            path = Path.Combine(path, boundaries_folder);
            path = Path.Combine(path, name + ".fds");
            if (!File.Exists(path)) return "";

            Stream src = File.OpenRead(path);
            if (src == null) return "";

            String content;
            using (StreamReader sr = new StreamReader(src))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }
            return content;
        }

        public static string GetParamName(MemberModel param)
        {
            return (param.Name ?? "").Replace("?", ""); // '?' is a marker for optional arguments
        }
    }
}
