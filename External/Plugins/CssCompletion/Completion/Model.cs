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

        public CssFeatures(string mode, Dictionary<string, string> config)
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

        private string GetParam(string name, Dictionary<string, string> config)
        {
            if (!config.ContainsKey(name)) return "";
            return config[name].Trim();
        }
    }

    class LocalContext
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

    class CssBlock
    {
        public CssBlock Parent;
        public List<CssBlock> Children = new List<CssBlock>();
        public int LineFrom;
        public int LineTo;
        public int ColFrom;
        public int ColTo;
    }

    enum CompleteMode
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
        static public Bitmap TagIcon;
        static public Bitmap PropertyIcon;
        static public Bitmap VariableIcon;
        static public Bitmap ValueIcon;
        static public Bitmap PseudoIcon;
        static public Bitmap PrefixesIcon;

        private string label;
        private string description;
        private ItemKind kind;

        public CompletionItem(string label, ItemKind kind)
        {
            this.label = label;
            this.kind = kind;
            description = "";
        }
        public CompletionItem(string label, ItemKind kind, string description)
        {
            this.label = label;
            this.kind = kind;
            this.description = description;
        }
        public string Label
        {
            get { return label; }
        }
        public string Description
        {
            get 
            {
                string desc = "";
                switch (kind)
                {
                    case ItemKind.Tag: desc = TextHelper.GetString("Info.CompletionTagDesc"); break;
                    case ItemKind.Property: desc = TextHelper.GetString("Info.CompletionPropertyDesc"); break;
                    case ItemKind.Variable: desc = TextHelper.GetString("Info.CompletionVariableDesc"); break;
                    case ItemKind.Value: desc = TextHelper.GetString("Info.CompletionValueDesc"); break;
                    case ItemKind.Pseudo: desc = TextHelper.GetString("Info.CompletionPseudoDesc"); break;
                    case ItemKind.Prefixes: desc = TextHelper.GetString("Info.CompletionPrefixesDesc"); break;
                }
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
        public ItemKind Kind { get { return kind; } }
        public Bitmap Icon
        {
            get 
            {
                switch (kind)
                {
                    case ItemKind.Tag: return TagIcon;
                    case ItemKind.Property: return PropertyIcon;
                    case ItemKind.Variable: return VariableIcon;
                    case ItemKind.Value: return ValueIcon;
                    case ItemKind.Pseudo: return PseudoIcon;
                    case ItemKind.Prefixes: return PrefixesIcon;
                }
                return PropertyIcon;
            }
        }
        public string Value
        {
            get { return label; }
        }
        Int32 IComparable.CompareTo(Object obj)
        {
            return String.Compare(Label, (obj as ICompletionListItem).Label, true);
        }
        Int32 IComparable<ICompletionListItem>.CompareTo(ICompletionListItem other)
        {
            return String.Compare(Label, other.Label, true);
        }
    }
}
