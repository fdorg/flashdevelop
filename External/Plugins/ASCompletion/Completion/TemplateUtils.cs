// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public static class TemplateUtils
    {
        public const string boundaries_folder = "boundaries";
        public const string generators_folder = "generators";
        public const string template_variable = @"<<[^\$]*?\$\({0}\).*?>>";

        public static string GetStaticExternOverride(MemberModel member)
        {
            var modifiers = string.Empty;
            var flags = member.Flags;
            if ((flags & FlagType.Extern) > 0) modifiers += "extern ";
            if ((flags & FlagType.Static) > 0) modifiers += "static ";
            if ((flags & FlagType.Override) > 0) modifiers += "override ";
            return modifiers;
        }

        public static string GetModifiers(MemberModel member) => GetModifiers(member.Access);

        public static string GetModifiers(Visibility access)
        {
            if ((access & Visibility.Private) > 0) return "private ";
            if ((access & Visibility.Public) > 0) return "public ";
            if ((access & Visibility.Protected) > 0) return "protected ";
            if ((access & Visibility.Internal) > 0) return "internal ";
            return string.Empty;
        }

        public static string ToDeclarationWithModifiersString(MemberModel member, string template)
        {
            var features = ASContext.Context.Features;
            var accessModifier = member.Access == 0 && features.hasNamespaces && !string.IsNullOrEmpty(member.Namespace)
                               ? member.Namespace
                               : GetModifiers(member).Trim();
            if (accessModifier == "private" && features.methodModifierDefault == Visibility.Private
                && !ASContext.CommonSettings.GenerateDefaultModifierDeclaration)
                accessModifier = null;

            string modifiers;
            if ((member.Flags & FlagType.Constructor) > 0) modifiers = accessModifier;
            else
            {
                modifiers = GetStaticExternOverride(member);
                if (accessModifier != null) modifiers += accessModifier;
                modifiers = modifiers.Trim();
                if (modifiers.Length == 0) modifiers = null;
            }

            var result = ReplaceTemplateVariable(template, "Modifiers", modifiers);
            // Insert Declaration
            result = ToDeclarationString(member, result);
            return result;
        }

        public static string ToDeclarationString(MemberModel member, string template)
        {
            // Insert Name
            template = ReplaceTemplateVariable(template, "Name", member.Name is null ? null : member.FullName);

            // If method, insert arguments
            template = ReplaceTemplateVariable(template, "Arguments", ParametersString(member, true));

            if (!string.IsNullOrEmpty(member.Type))
            {
                if ((member.Flags & FlagType.Setter) > 0 && member.Parameters != null && member.Parameters.Count == 1)
                    template = ReplaceTemplateVariable(template, "Type", MemberModel.FormatType(member.Parameters[0].Type));
                else template = ReplaceTemplateVariable(template, "Type", MemberModel.FormatType(member.Type));
            }
            else template = ReplaceTemplateVariable(template, "Type", null);
            template = ReplaceTemplateVariable(template, "Value", member.Value);
            return template;
        }

        public static string ParametersString(MemberModel member, bool formatted)
        {
            if (member.Parameters.IsNullOrEmpty()) return string.Empty;
            var result = string.Empty;
            var template = GetTemplate("FunctionParameter");
            for (int i = 0, count = member.Parameters.Count; i < count; i++)
            {
                var param = member.Parameters[i];
                var one = template;
                one = ReplaceTemplateVariable(one, "PComma", i + 1 < count ? "," : null);
                one = ReplaceTemplateVariable(one, "PName", param.Name);
                one = string.IsNullOrEmpty(param.Type)
                    ? ReplaceTemplateVariable(one, "PType", null)
                    : ReplaceTemplateVariable(one, "PType", formatted ? MemberModel.FormatType(param.Type) : param.Type);
                one = ReplaceTemplateVariable(one, "PDefaultValue", param.Value?.Trim());
                result += one;
            }
            return result;
        }

        public static string CallParametersString(MemberModel member)
        {
            if (member.Parameters.IsNullOrEmpty()) return string.Empty;
            var result = string.Empty;
            var template = GetTemplate("FunctionParameter");
            for (int i = 0, count = member.Parameters.Count; i < count; i++)
            {
                var param = member.Parameters[i];
                var one = template;
                one = ReplaceTemplateVariable(one, "PComma", i + 1 < count ? "," : null);
                one = ReplaceTemplateVariable(one, "PName", GetParamName(param));
                one = ReplaceTemplateVariable(one, "PType", null);
                one = ReplaceTemplateVariable(one, "PDefaultValue", null);
                result += one;
            }
            return result;
        }

        public static string ReplaceTemplateVariable(string template, string var, string replace)
        {
            var mc = Regex.Matches(template, string.Format(template_variable, var));
            var mcCount = mc.Count;
            if (mcCount > 0)
            {
                var sb = new System.Text.StringBuilder();
                var pos = 0;
                for (var i = 0; i < mcCount; i++)
                {
                    var m = mc[i];
                    var endIndex = m.Index + m.Length;
                    sb.Append(template.Substring(pos, m.Index - pos));
                    if (replace != null)
                    {
                        var val = m.Value;
                        val = val.Substring(2, val.Length - 4);
                        sb.Append(val);
                    }
                    if (i == mcCount - 1) sb.Append(template.Substring(endIndex));
                    else
                    {
                        var next = mc[i + 1].Index;
                        sb.Append(template.Substring(endIndex, next - endIndex));
                        pos += next;
                    }
                }

                template = sb.ToString();
            }
            template = template.Replace("$(" + var + ")", replace);
            return template;
        }

        public static MemberModel GetTemplateBlockMember(ScintillaControl sci, string blockTmpl)
        {
            if (string.IsNullOrEmpty(blockTmpl)) return null;
            var firstLine = blockTmpl;
            var lineCount = 0;
            var index = blockTmpl.IndexOf('\n');
            if (index != -1)
            {
                firstLine = blockTmpl.Substring(0, index);
                lineCount = Regex.Matches(blockTmpl, "\n").Count;
            }
            var lineNum = 0;
            while (lineNum < sci.LineCount)
            {
                var line = sci.GetLine(lineNum);
                var funcBlockIndex = line.IndexOfOrdinal(firstLine);
                if (funcBlockIndex != -1)
                {
                    return new MemberModel {LineFrom = lineNum, LineTo = lineNum + lineCount};
                }
                lineNum++;
            }
            return null;
        }

        /// <summary>
        /// Templates are stored in the plugin's Data folder
        /// </summary>
        public static string GetTemplate(string name, string altName) => GetTemplate(name) is { } tmp && tmp.Length != 0
            ? tmp
            : GetTemplate(altName);

        /// <summary>
        /// Templates are stored in the plugin's Data folder
        /// </summary>
        public static string GetTemplate(string name)
        {
            var lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            var path = Path.Combine(PathHelper.SnippetDir, lang, generators_folder, name + ".fds");
            if (!File.Exists(path)) return string.Empty;
            using Stream src = File.OpenRead(path);
            using var sr = new StreamReader(src);
            var content = sr.ReadToEnd();
            sr.Close();
            return "$(Boundary)" + content.Replace("\r\n", "\n") + "$(Boundary)";
        }

        public static string GetBoundary(string name)
        {
            var lang = PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage.ToLower();
            var path = Path.Combine(PathHelper.SnippetDir, lang, boundaries_folder, name + ".fds");
            if (!File.Exists(path)) return string.Empty;
            using Stream src = File.OpenRead(path);
            using var sr = new StreamReader(src);
            var content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public static string GetParamName(MemberModel parameter)
        {
            return string.IsNullOrEmpty(parameter.Name)
                ? string.Empty
                : parameter.Name.Replace("?", "");
        }
    }
}
