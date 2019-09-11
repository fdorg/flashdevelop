using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using PluginCore;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class StyleClass : ConfigItem
    {
        [XmlAttribute()]
        public string name;

        [XmlAttribute()]
        public string fore;

        [XmlAttribute()]
        public string back;

        [XmlAttribute()]
        public string size;

        [XmlAttribute()]
        public string font;

        [XmlAttribute()]
        public string bold;

        [XmlAttribute()]
        public string eolfilled;

        [XmlAttribute()]
        public string italics;

        [XmlAttribute("inherit-style")]
        public string inheritstyle;

        [NonSerialized]
        public Language language;

        public StyleClass(){}
        
        public StyleClass ParentClass
        {
            get
            {
                if (!string.IsNullOrEmpty(inheritstyle))
                {
                    return _parent.MasterScintilla.GetStyleClass(inheritstyle);
                }
                // Get the parent class. It should be either language's own default or global default
                // Caution: It is not programmatically guaranteed that there is a default.
                if (!name.Equals("default"))
                {
                    StyleClass defaultCls = language?.usestyles[0]; // First should be default...
                    if (defaultCls != null) return defaultCls;
                    return _parent.MasterScintilla.GetStyleClass("default");
                }
                return null;
            }
        }

        public bool HasItalics
        {
            get
            {
                if (italics != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasItalics;
                return false;
            }
        }

        public bool HasBold
        {
            get
            {
                if (bold != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasBold;
                return false;
            }
        }

        public bool HasEolFilled
        {
            get
            {
                if (eolfilled != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasEolFilled;
                return false;
            }
        }

        public bool HasFontName
        {
            get
            {
                if (font != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasFontName;
                return false;
            }
        }

        public bool HasFontSize
        {
            get
            {
                if (size != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasFontSize;
                return false;
            }
        }

        public bool HasBackgroundColor
        {
            get
            {
                if (back != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasBackgroundColor;
                return false;
            }
        }

        public bool HasForegroundColor
        {
            get
            {
                if (fore  != null) return true;
                StyleClass p = ParentClass;
                if (p != null) return p.HasForegroundColor;
                return false;
            }
        }

        public string FontName
        {
            get
            {
                if (font != null) return ResolveFont(font);
                StyleClass p = ParentClass;
                if (p != null) return p.FontName;
                return "Courier New";
            }
        }

        public int FontSize
        {
            get
            {
                if (size != null) return ResolveNumber(size);
                StyleClass p = ParentClass;
                if (p != null) return p.FontSize;
                return 9;
            }
        }

        public int BackgroundColor
        {
            get
            {
                if (back != null) return ResolveColor(back);
                StyleClass p = ParentClass;
                if (p != null) return p.BackgroundColor;
                return 0x000000;
            }
        }

        public int ForegroundColor
        {
            get
            {
                if (fore != null) return ResolveColor(fore);
                StyleClass p = ParentClass;
                if (p  != null) return p.ForegroundColor;
                return 0x000000;
            }
        }

        public bool IsItalics
        {
            get
            {
                if (italics!= null) return italics.Equals("true");
                StyleClass p = ParentClass;
                if (p != null) return p.IsItalics;
                return false;
            }
        }

        public bool IsBold
        {
            get
            {
                if (bold != null) return bold.Equals("true");
                StyleClass p = ParentClass;
                if (p != null) return p.IsBold;
                return false;
            }
        }

        public bool IsEolFilled
        {
            get
            {
                if (eolfilled != null) return eolfilled.Equals("true");
                StyleClass p = ParentClass;
                if (p  != null) return p.IsEolFilled;
                return false;
            }
        }

        public int ResolveColor(string aColor)
        {
            if (aColor != null)
            {
                Value v = _parent.MasterScintilla.GetValue(aColor);
                while (v!= null)
                {
                    aColor = v.val;
                    v = _parent.MasterScintilla.GetValue(aColor);
                }
                Color c = Color.FromName(aColor);
                if (c.ToArgb() == 0)
                {
                    if (aColor.IndexOfOrdinal("0x") == 0) return TO_COLORREF(int.Parse(aColor.Substring(2), NumberStyles.HexNumber));
                    try
                    {
                        return TO_COLORREF(int.Parse(aColor));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                return TO_COLORREF(c.ToArgb() & 0x00ffffff);
            }
            return 0;
        }
        private int TO_COLORREF(int c)
        {
            return (((c & 0xff0000) >> 16) + ((c & 0x0000ff) << 16) + (c & 0x00ff00) );
        }
        
        public int ResolveNumber(string number)
        {
            if (number != null)
            {
                var v = _parent.MasterScintilla.GetValue(number);
                while (v != null)
                {
                    number = v.val;
                    v = _parent.MasterScintilla.GetValue(number);
                }
                if (number.IndexOfOrdinal("0x") == 0) return int.Parse(number.Substring(2), NumberStyles.HexNumber);
                try
                {
                    return int.Parse(number);
                }
                catch
                {
                    // ignored
                }
            }
            return 0;

        }

        public string ResolveString(string number)
        {
            if (number != null)
            {
                Value value;
                for (value = _parent.MasterScintilla.GetValue(number); value != null; value = _parent.MasterScintilla.GetValue(number))
                {
                    number = value.val;
                }
            }
            return number;
        }

        public string ResolveFont(string name)
        {
            if (name != null)
            {
                Value value;
                for (value = _parent.MasterScintilla.GetValue(name); value != null; value = _parent.MasterScintilla.GetValue(name))
                {
                    name = value.val;
                }
            }
            try  // Choose first font that is found...
            {
                string[] fonts = name.Split(',');
                foreach (string font in fonts)
                {
                    if (IsFontInstalled(font)) return font;
                }
            }
            catch { /* No errors... */ }
            return name;
        }

        private static bool IsFontInstalled(string fontName)
        {
            using var testFont = new Font(fontName, 9);
            return fontName.Equals(testFont.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        
    }

}
