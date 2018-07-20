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
        public String Tag;
        public String Name;
        public String NameSpace;
        public Int32 Position;
        public Boolean Closing;
        public Boolean Closed;
    }

    public class HTMLTag : IComparable
    {
        public String Tag;
        public String Name;
        public String NS;
        public List<string> Attributes;
        public Boolean IsLeaf;

        public HTMLTag(String tag, String ns, Boolean isLeaf)
        {
            Name = tag;
            NS = ns ?? "";
            Tag = (ns != null) ? ns + ":" + tag : tag;
            IsLeaf = isLeaf;
        }

        /// <summary>
        /// Compares tags
        /// </summary>
        public Int32 CompareTo(Object obj)
        {
            if (!(obj is HTMLTag)) throw new InvalidCastException("This object is not of type HTMLTag");
            return String.Compare(Tag, ((HTMLTag)obj).Tag);
        }
    }

    public interface IHtmlCompletionListItem : ICompletionListItem
    {
        String Name { get; }
    }

    public class HtmlTagItem : IHtmlCompletionListItem
    {
        private String name;
        private String tag;
        private String label;
        private String uri;

        public HtmlTagItem(String name, String tag, String uri)
        {
            this.name = name;
            this.label = tag;
            this.tag = tag;
            this.uri = uri;
        }

        public HtmlTagItem(String name, String tag)
        {
            this.name = name;
            this.label = tag;
            this.tag = tag;
        }

        /// <summary>
        /// Gets the name of the tag (without namespace)
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public String Label
        {
            get { return label; }
        }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public String Description
        {
            get { return "<" + tag + ">" + (uri != null ? " - " + uri : ""); }
        }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon
        {
            get { return XMLComplete.HtmlTagIcon; }
        }

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public String Value
        {
            get { return tag; }
        }

    }

    public class NamespaceItem : ICompletionListItem
    {
        private String label;
        private String uri;

        public NamespaceItem(String name, String uri)
        {
            this.label = name;
            this.uri = uri;
        }

        public NamespaceItem(String name)
        {
            this.label = name;
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public String Label
        {
            get { return label; }
        }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public String Description
        {
            get { return "xmlns:" + label + (uri != null ? " - " + uri : ""); }
        }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon
        {
            get { return XMLComplete.NamespaceTagIcon; }
        }

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public String Value
        {
            get { return label; }
        }

    }

    public class HtmlAttributeItem : IHtmlCompletionListItem
    {
        private String name;
        private String label;
        private String desc;
        private Bitmap icon;
        private String type;
        private String className;

        public HtmlAttributeItem(String name, String type, String className, String ns)
        {
            setName(name);
            setExt(type, className);
            if (!string.IsNullOrEmpty(ns)) label = ns + ":" + label;
        }

        public HtmlAttributeItem(String name, String type, String className)
        {
            setName(name);
            setExt(type, className);
        }

        private void setExt(String type, String className)
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

        public HtmlAttributeItem(String name)
        {
            setName(name);
        }

        private void setName(String name)
        {
            Int32 p = name.LastIndexOf(':');
            if (p > 0)
            {
                String ic = name.Substring(p + 1);
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
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public String Label
        {
            get { return this.label; }
        }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public String Description
        {
            get { return this.desc; }
        }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon
        {
            get { return this.icon; }
        }

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public String Value
        {
            get { return this.label; }
        }

        /// <summary>
        /// Gets the class name of the attribute item
        /// </summary>
        public String ClassName
        {
            get { return this.className; }
        }
    }

    public class XMLBlockItem : ICompletionListItem
    {
        private String desc;
        private String label;
        private String replace;
 
        public XMLBlockItem(String label, String desc, String replace)
        {
            this.desc = desc;
            this.label = label;
            this.replace = replace;
        }

        /// <summary>
        /// Gets the label of the list item
        /// </summary>
        public String Label
        {
            get { return this.label; }
        }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        public String Description
        {
            get { return this.desc; }
        }

        /// <summary>
        /// Gets the icon of the list item
        /// </summary>
        public Bitmap Icon
        {
            get { return XMLComplete.HtmlTagIcon; }
        }

        /// <summary>
        /// Gets the value of the list item
        /// </summary>
        public String Value
        {
            get
            {
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                Int32 position = sci.SelectionStart;
                String[] rep = replace.Split('|');
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
            if (a is HtmlTagItem && b is HtmlTagItem)
                return string.Compare(((HtmlTagItem)a).Name, ((HtmlTagItem)b).Name);
            return string.Compare(a.Label, b.Label);
        }

    }
}
