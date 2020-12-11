using System;
using System.Collections.Generic;
using System.Drawing;
using PluginCore;
using PluginCore.Localization;

namespace XMLCompletion
{
    public enum XMLType
    {
        Unknown = 0,
        Invalid,
        XML,
        Known
    }

    public struct XMLContextTag
    {
        public string Tag;
        public string Name;
        public string NameSpace;
        public int Position;
        public bool Closing;
        public bool Closed;
    }

    public class HTMLTag : IComparable
    {
        public string Tag;
        public string Name;
        public string NS;
        public List<string> Attributes;
        public bool IsLeaf;

        public HTMLTag(string tag, string ns, bool isLeaf)
        {
            Name = tag;
            NS = ns ?? "";
            Tag = (ns != null) ? ns + ":" + tag : tag;
            IsLeaf = isLeaf;
        }

        /// <summary>
        /// Compares tags
        /// </summary>
        public int CompareTo(object obj)
        {
            if (!(obj is HTMLTag)) throw new InvalidCastException("This object is not of type HTMLTag");
            return string.Compare(Tag, ((HTMLTag)obj).Tag);
        }
    }

    public interface IHtmlCompletionListItem : ICompletionListItem
    {
        string Name { get; }
    }

    public class HtmlTagItem : IHtmlCompletionListItem
    {
        readonly string uri;

        public HtmlTagItem(string name, string tag, string uri)
        {
            Name = name;
            Label = tag;
            Value = tag;
            this.uri = uri;
        }

        public HtmlTagItem(string name, string tag)
        {
            Name = name;
            Label = tag;
            Value = tag;
        }

        /// <summary>
        /// Gets the name of the tag (without namespace)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => "<" + Value + ">" + (uri != null ? " - " + uri : "");

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => XMLComplete.HtmlTagIcon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value { get; }
    }

    public class NamespaceItem : ICompletionListItem
    {
        readonly string uri;

        public NamespaceItem(string name, string uri)
        {
            Value = name;
            this.uri = uri;
        }

        public NamespaceItem(string name) => Value = name;

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label => Value;

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => "xmlns:" + Value + (uri != null ? " - " + uri : "");

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => XMLComplete.NamespaceTagIcon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value { get; }
    }

    public class HtmlAttributeItem : IHtmlCompletionListItem
    {
        public HtmlAttributeItem(string name, string type, string className, string ns)
        {
            SetName(name);
            SetExt(type, className);
            if (!string.IsNullOrEmpty(ns)) Label = ns + ":" + Label;
        }

        public HtmlAttributeItem(string name, string type, string className)
        {
            SetName(name);
            SetExt(type, className);
        }

        void SetExt(string type, string className)
        {
            ClassName = className;
            if (Icon == XMLComplete.HtmlAttributeIcon || Icon == XMLComplete.StyleAttributeIcon)
            {
                Description = Label;
                if (!string.IsNullOrEmpty(type)) Description += " : " + type;
            }
            if (Icon == XMLComplete.EffectAttributeIcon)
            {
                if (!string.IsNullOrEmpty(type)) Description += " > " + type;
            }
            if (!string.IsNullOrEmpty(className)) Description += " - " + className;
        }

        public HtmlAttributeItem(string name) => SetName(name);

        void SetName(string name)
        {
            int p = name.LastIndexOf(':');
            if (p > 0)
            {
                string ic = name.Substring(p + 1);
                if (ic == "s" || ic == "style")
                {
                    Icon = XMLComplete.StyleAttributeIcon;
                    Description = TextHelper.GetString("Info.StylingAttribute");
                }
                else if (ic == "e" || ic == "event")
                {
                    Icon = XMLComplete.EventAttributeIcon;
                    Description = TextHelper.GetString("Info.EventAttribute");
                }
                else if (ic == "x" || ic == "effect")
                {
                    Icon = XMLComplete.EffectAttributeIcon;
                    Description = TextHelper.GetString("Info.EffectAttribute");
                }
                else
                {
                    Icon = XMLComplete.HtmlAttributeIcon;
                    Description = TextHelper.GetString("Info.Attribute");
                }
                name = name.Substring(0, p);
            }
            else
            {
                Icon = XMLComplete.HtmlAttributeIcon;
                Description = TextHelper.GetString("Info.Attribute");
            }
            Name = name;
            Label = name;
        }

        /// <summary>
        /// Gets the name of the item (without namespace)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon { get; set; }

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value => Label;

        /// <summary>
        /// Gets the class name of the attribute item
        /// </summary>
        public string ClassName { get; set; }
    }

    public class XMLBlockItem : ICompletionListItem
    {
        readonly string replace;
 
        public XMLBlockItem(string label, string desc, string replace)
        {
            Description = desc;
            Label = label;
            this.replace = replace;
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => XMLComplete.HtmlTagIcon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value
        {
            get
            {
                var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
                if (sci is null) return null;
                var position = sci.SelectionStart;
                var rep = replace.Split('|');
                sci.ReplaceSel(rep[0]);
                sci.ReplaceSel(rep[1]);
                sci.SetSel(position + rep[0].Length, position + rep[0].Length);
                return null;
            }
        }

    }

    public class ListItemComparer : IComparer<ICompletionListItem>
    {

        public int Compare(ICompletionListItem a, ICompletionListItem b)
        {
            if (a is HtmlTagItem aItem && b is HtmlTagItem bItem)
                return string.Compare(aItem.Name, bItem.Name);
            return string.Compare(a.Label, b.Label);
        }

    }
}
