using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class EditorStyle : ConfigItem
    {
        [XmlAttribute("caret-fore")]
        public string caretfore;
        
        [XmlAttribute("caretline-back")]
        public string caretlineback;

        [XmlAttribute("selection-fore")]
        public string selectionfore;

        [XmlAttribute("selection-back")]
        public string selectionback;

        [XmlAttribute("marker-fore")]
        public string markerfore;

        [XmlAttribute("marker-back")]
        public string markerback;

        [XmlAttribute("margin-fore")]
        public string marginfore;

        [XmlAttribute("margin-back")]
        public string marginback;

        [XmlAttribute("print-margin")]
        public string printmargin;

        [XmlAttribute("bookmarkline-back")]
        public string bookmarkline;

        [XmlAttribute("modifiedline-back")]
        public string modifiedline;
        
        [XmlAttribute("highlight-back")]
        public string highlightback;
        
        [XmlAttribute("errorline-back")]
        public string errorlineback;

        [XmlAttribute("debugline-back")]
        public string debuglineback;

        [XmlAttribute("disabledline-back")]
        public string disabledlineback;

        [XmlAttribute("colorize-marker-back")]
        public string colorizemarkerback;

        public int ResolveColor(string aColor)
        {
            if (aColor != null)
            {
                Value v = _parent.MasterScintilla.GetValue(aColor);
                while (v != null)
                {
                    aColor = v.val;
                    v = _parent.MasterScintilla.GetValue(aColor);
                }
                Color c = Color.FromName(aColor);
                if (c.ToArgb() == 0)
                {
                    if (aColor.IndexOf("0x") == 0)
                    {
                        return TO_COLORREF(Int32.Parse(aColor.Substring(2), NumberStyles.HexNumber));
                    } 
                    else 
                    {
                        try
                        {
                            return TO_COLORREF(Int32.Parse(aColor));
                        }
                        catch (Exception){}
                    }
                }
                return TO_COLORREF(c.ToArgb() & 0x00ffffff);
            }
            return 0;
        }
        private int TO_COLORREF(int c)
        {
            return (((c & 0xff0000) >> 16)+((c & 0x0000ff) << 16)+(c & 0x00ff00));
        }
        
        public int CaretForegroundColor
        {
            get
            {
                if (caretfore != null && caretfore.Length > 0)
                {
                    return ResolveColor(caretfore);
                }
                return ResolveColor("0x000000");
            }
        }
        
        public int CaretLineBackgroundColor
        {
            get
            {
                if (caretlineback != null && caretlineback.Length > 0)
                {
                    return ResolveColor(caretlineback);
                }
                return ResolveColor("0xececec");
            }
        }
        
        public int SelectionForegroundColor
        {
            get
            {
                if (selectionfore != null && selectionfore.Length > 0)
                {
                    return ResolveColor(selectionfore);
                }
                return ResolveColor("0xffffff");
            }
        }
        
        public int SelectionBackgroundColor
        {
            get
            {
                if (selectionback != null && selectionback.Length > 0)
                {
                    return ResolveColor(selectionback);
                }
                return ResolveColor("0x000000");
            }
        }

        public int MarkerBackgroundColor
        {
            get
            {
                if (markerback != null && markerback.Length > 0)
                {
                    return ResolveColor(markerback);
                }
                return ResolveColor("0x808080");
            }
        }

        public int MarkerForegroundColor
        {
            get
            {
                if (markerfore != null && markerfore.Length > 0)
                {
                    return ResolveColor(markerfore);
                }
                return ResolveColor("0xffffff");
            }
        }

        public int MarginForegroundColor
        {
            get
            {
                if (marginfore != null && marginfore.Length > 0)
                {
                    return ResolveColor(marginfore);
                }
                return ResolveColor("0xf5f5f5");
            }
        }

        public int MarginBackgroundColor
        {
            get
            {
                if (marginback != null && marginback.Length > 0)
                {
                    return ResolveColor(marginback);
                }
                return ResolveColor("0xfaf0e6");
            }
        }

        public int PrintMarginColor
        {
            get
            {
                if (printmargin != null && printmargin.Length > 0)
                {
                    return ResolveColor(printmargin);
                }
                return ResolveColor("0xCCCCCC");
            }
        }

        public int BookmarkLineColor
        {
            get
            {
                if (bookmarkline != null && bookmarkline.Length > 0)
                {
                    return ResolveColor(bookmarkline);
                }
                return ResolveColor("0xffff00");
            }
        }
        
        public int ModifiedLineColor
        {
            get
            {
                if (modifiedline != null && modifiedline.Length > 0)
                {
                    return ResolveColor(modifiedline);
                }
                return ResolveColor("0xffff00");
            }
        }

        public int HighlightBackColor
        {
            get
            {
                if (highlightback != null && highlightback.Length > 0)
                {
                    return ResolveColor(highlightback);
                }
                return ResolveColor("0x0000ff");
            }
        }

        public int ErrorLineBack
        {
            get
            {
                if (errorlineback != null && errorlineback.Length > 0)
                {
                    return ResolveColor(errorlineback);
                }
                return ResolveColor("0xff0000");
            }
        }

        public int DebugLineBack
        {
            get
            {
                if (debuglineback != null && debuglineback.Length > 0)
                {
                    return ResolveColor(debuglineback);
                }
                return ResolveColor("0xffff00");
            }
        }

        public int DisabledLineBack
        {
            get
            {
                if (disabledlineback != null && disabledlineback.Length > 0)
                {
                    return ResolveColor(disabledlineback);
                }
                return ResolveColor("0xcccccc");
            }
        }

        public bool ColorizeMarkerBack
        {
            get
            {
                if (colorizemarkerback != null && colorizemarkerback.Length > 0)
                {
                    return colorizemarkerback.ToLower() == "true";
                }
                return false;
            }
        }

    }
    
}
