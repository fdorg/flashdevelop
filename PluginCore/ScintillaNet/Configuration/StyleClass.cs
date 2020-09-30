using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class StyleClass : ConfigItem
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string fore;

        [XmlAttribute]
        public string back;

        [XmlAttribute]
        public string size;

        [XmlAttribute]
        public string font;

        [XmlAttribute]
        public string bold;

        [XmlAttribute]
        public string eolfilled;

        [XmlAttribute]
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
                if (name.Equals("default")) return null;
                StyleClass defaultCls = language?.usestyles[0]; // First should be default...
                return defaultCls ?? _parent.MasterScintilla.GetStyleClass("default");
            }
        }

        public bool HasItalics => italics != null || (ParentClass?.HasItalics ?? false);

        public bool HasBold => bold != null || (ParentClass?.HasBold ?? false);

        public bool HasEolFilled => eolfilled != null || (ParentClass?.HasEolFilled ?? false);

        public bool HasFontName => font != null || (ParentClass?.HasFontName ?? false);

        public bool HasFontSize => size != null || (ParentClass?.HasFontSize ?? false);

        public bool HasBackgroundColor => back != null || (ParentClass?.HasBackgroundColor ?? false);

        public bool HasForegroundColor => fore != null || (ParentClass?.HasForegroundColor ?? false);

        public string FontName => font != null ? ResolveFont(font) : (ParentClass?.FontName ?? "Courier New");

        public int FontSize
        {
            get
            {
                if (size != null) return ResolveNumber(size);
                return ParentClass?.FontSize ?? 9;
            }
        }

        public int BackgroundColor
        {
            get
            {
                if (back != null) return ResolveColor(back);
                return ParentClass?.BackgroundColor ?? 0x000000;
            }
        }

        public int ForegroundColor
        {
            get
            {
                if (fore != null) return ResolveColor(fore);
                return ParentClass?.ForegroundColor ?? 0x000000;
            }
        }

        public bool IsItalics
        {
            get
            {
                if (italics!= null) return italics.Equals("true");
                return ParentClass?.IsItalics ?? false;
            }
        }

        public bool IsBold
        {
            get
            {
                if (bold != null) return bold.Equals("true");
                return ParentClass?.IsBold ?? false;
            }
        }

        public bool IsEolFilled
        {
            get
            {
                if (eolfilled != null) return eolfilled.Equals("true");
                return ParentClass?.IsEolFilled ?? false;
            }
        }

        public int ResolveColor(string aColor)
        {
            if (aColor != null)
            {
                var v = _parent.MasterScintilla.GetValue(aColor);
                while (v!= null)
                {
                    aColor = v.val;
                    v = _parent.MasterScintilla.GetValue(aColor);
                }
                var c = Color.FromName(aColor);
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

        int TO_COLORREF(int c) => ((c & 0xff0000) >> 16) + ((c & 0x0000ff) << 16) + (c & 0x00ff00);

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
                var fonts = name.Split(',');
                foreach (string font in fonts)
                {
                    if (IsFontInstalled(font)) return font;
                }
            }
            catch { /* No errors... */ }
            return name;
        }

        static bool IsFontInstalled(string fontName)
        {
            using var font = new Font(fontName, 9);
            return fontName.Equals(font.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}