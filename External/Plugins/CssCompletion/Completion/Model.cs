using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace CssCompletion
{
    public class CssFeatures
    {
        public string Mode;
        public string Compile;
        public string Syntax;
        public Regex Pattern;
        public Regex ErrorPattern;
        public char Trigger;
        public bool AutoCompile;
        public bool AutoMinify;

        public CssFeatures(string mode, IDictionary<string, string> config)
        {
            Mode = mode;
            string compile = GetParam("compile", config);
            if (compile.Length > 0)
                Compile = compile;
            string syntax = GetParam("syntax", config);
            if (syntax.Length > 0)
                Syntax = syntax;
            string pattern = GetParam("variable", config);
            if (pattern.Length > 0)
                Pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string error = GetParam("error", config);
            if (error.Length > 0)
            {
                RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
                if (error.EndsWithOrdinal("/s")) options |= RegexOptions.Singleline;
                else options |= RegexOptions.Multiline;
                ErrorPattern = new Regex(error.Substring(1, error.Length - 3), options);
            }
            string trigger = GetParam("trigger", config);
            if (trigger.Length > 0)
                Trigger = trigger[0];
        }

        static string GetParam(string name, IDictionary<string, string> config)
        {
            if (!config.ContainsKey(name)) return "";
            return config[name].Trim();
        }
    }

    internal class LocalContext
    {
        public ScintillaControl Sci;
        public char Separator;
        public int Position;
        public bool InValue;
        public bool InBlock;
        public bool InUrl;
        public bool InComments;
        public bool IsVar;
        public string Word;
        public string Property;

        public LocalContext(ScintillaControl sci)
        {
            Sci = sci;
        }
    }

    internal class CssBlock
    {
        public CssBlock Parent;
        public List<CssBlock> Children = new List<CssBlock>();
        public int LineFrom;
        public int LineTo;
        public int ColFrom;
        public int ColTo;
    }

    internal enum CompleteMode
    {
        None,
        Selector,
        Pseudo,
        Attribute,
        Prefix,
        Value,
        Variable
    }

    public enum ItemKind 
    {
        Tag,
        Property,
        Value,
        Variable,
        Pseudo,
        Prefixes
    }

    /// <summary>
    /// Simple completion list item
    /// </summary>
    public class CompletionItem : ICompletionListItem, IComparable, IComparable<ICompletionListItem>
    {
        public static Bitmap TagIcon;
        public static Bitmap PropertyIcon;
        public static Bitmap VariableIcon;
        public static Bitmap ValueIcon;
        public static Bitmap PseudoIcon;
        public static Bitmap PrefixesIcon;
        readonly string description;

        public CompletionItem(string label, ItemKind kind)
        {
            Label = label;
            Kind = kind;
            description = "";
        }
        public CompletionItem(string label, ItemKind kind, string description)
        {
            Label = label;
            Kind = kind;
            this.description = description;
        }
        public string Label { get; }

        public string Description
        {
            get
            {
                var desc = Kind switch
                {
                    ItemKind.Tag => TextHelper.GetString("Info.CompletionTagDesc"),
                    ItemKind.Property => TextHelper.GetString("Info.CompletionPropertyDesc"),
                    ItemKind.Variable => TextHelper.GetString("Info.CompletionVariableDesc"),
                    ItemKind.Value => TextHelper.GetString("Info.CompletionValueDesc"),
                    ItemKind.Pseudo => TextHelper.GetString("Info.CompletionPseudoDesc"),
                    ItemKind.Prefixes => TextHelper.GetString("Info.CompletionPrefixesDesc"),
                    _ => string.Empty,
                };
                if (description.Length > 0)
                {
                    int comment = description.IndexOfOrdinal("//");
                    if (comment >= 0)
                    {
                        desc = "[I]" + description.Substring(comment + 2).Trim() + "[/I]\n" 
                            + description.Substring(0, comment).Trim();
                    }
                    else desc = description;
                }
                return desc;
            }
        }
        public ItemKind Kind { get; }

        public Bitmap Icon => Kind switch
        {
            ItemKind.Tag => TagIcon,
            ItemKind.Property => PropertyIcon,
            ItemKind.Variable => VariableIcon,
            ItemKind.Value => ValueIcon,
            ItemKind.Pseudo => PseudoIcon,
            ItemKind.Prefixes => PrefixesIcon,
            _ => PropertyIcon,
        };

        public string Value => Label;

        int IComparable.CompareTo(object obj) => string.Compare(Label, ((ICompletionListItem) obj).Label, true);

        int IComparable<ICompletionListItem>.CompareTo(ICompletionListItem other) => string.Compare(Label, other.Label, true);
    }
}
