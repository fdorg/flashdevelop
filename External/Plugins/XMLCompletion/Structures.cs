using System;
using System.Collections.Generic;
using System.Drawing;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

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
        readonly string name;
        readonly string tag;
        readonly string label;
        readonly string uri;

        public HtmlTagItem(string name, string tag, string uri)
        {
            this.name = name;
            this.label = tag;
            this.tag = tag;
            this.uri = uri;
        }

        public HtmlTagItem(string name, string tag)
        {
            this.name = name;
            this.label = tag;
            this.tag = tag;
        }

        /// <summary>
        /// Gets the name of the tag (without namespace)
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label => label;

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => "<" + tag + ">" + (uri != null ? " - " + uri : "");

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => XMLComplete.HtmlTagIcon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value => tag;
    }

    public class NamespaceItem : ICompletionListItem
    {
        readonly string label;
        readonly string uri;

        public NamespaceItem(string name, string uri)
        {
            this.label = name;
            this.uri = uri;
        }

        public NamespaceItem(string name)
        {
            this.label = name;
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label => label;

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => "xmlns:" + label + (uri != null ? " - " + uri : "");

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => XMLComplete.NamespaceTagIcon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value => label;
    }

    public class HtmlAttributeItem : IHtmlCompletionListItem
    {
        string name;
        string label;
        string desc;
        Bitmap icon;
        string type;
        string className;

        public HtmlAttributeItem(string name, string type, string className, string ns)
        {
            setName(name);
            setExt(type, className);
            if (!string.IsNullOrEmpty(ns)) label = ns + ":" + label;
        }

        public HtmlAttributeItem(string name, string type, string className)
        {
            setName(name);
            setExt(type, className);
        }

        void setExt(string type, string className)
        {
            this.type = type;
            this.className = className;
            if (icon == XMLComplete.HtmlAttributeIcon || icon == XMLComplete.StyleAttributeIcon)
            {
                this.desc = label;
                if (!string.IsNullOrEmpty(type)) this.desc += " : " + type;
            }
            if (icon == XMLComplete.EffectAttributeIcon)
            {
                if (!string.IsNullOrEmpty(type)) this.desc += " > " + type;
            }
            if (!string.IsNullOrEmpty(className)) this.desc += " - " + className;
        }

        public HtmlAttributeItem(string name)
        {
            setName(name);
        }

        void setName(string name)
        {
            int p = name.LastIndexOf(':');
            if (p > 0)
            {
                string ic = name.Substring(p + 1);
                if (ic == "s" || ic == "style")
                {
                    this.icon = XMLComplete.StyleAttributeIcon;
                    this.desc = TextHelper.GetString("Info.StylingAttribute");
                }
                else if (ic == "e" || ic == "event")
                {
                    this.icon = XMLComplete.EventAttributeIcon;
                    this.desc = TextHelper.GetString("Info.EventAttribute");
                }
                else if (ic == "x" || ic == "effect")
                {
                    this.icon = XMLComplete.EffectAttributeIcon;
                    this.desc = TextHelper.GetString("Info.EffectAttribute");
                }
                else
                {
                    this.icon = XMLComplete.HtmlAttributeIcon;
                    this.desc = TextHelper.GetString("Info.Attribute");
                }
                name = name.Substring(0, p);
            }
            else
            {
                this.icon = XMLComplete.HtmlAttributeIcon;
                this.desc = TextHelper.GetString("Info.Attribute");
            }
            this.name = name;
            this.label = name;
        }

        /// <summary>
        /// Gets the name of the item (without namespace)
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label => this.label;

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => this.desc;

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon => this.icon;

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public string Value => this.label;

        /// <summary>
        /// Gets the class name of the attribute item
        /// </summary>
        public string ClassName => this.className;
    }

    public class XMLBlockItem : ICompletionListItem
    {
        readonly string desc;
        readonly string label;
        readonly string replace;
 
        public XMLBlockItem(string label, string desc, string replace)
        {
            this.desc = desc;
            this.label = label;
            this.replace = replace;
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public string Label => this.label;

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public string Description => this.desc;

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
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                int position = sci.SelectionStart;
                string[] rep = replace.Split('|');
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
