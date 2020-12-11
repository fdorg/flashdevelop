using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using PluginCore;

namespace ScintillaNet.Configuration
{
    [Serializable]
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

        [XmlAttribute("highlightword-back")]
        public string highlightwordback;

        [XmlAttribute("errorline-back")]
        public string errorlineback;

        [XmlAttribute("debugline-back")]
        public string debuglineback;

        [XmlAttribute("disabledline-back")]
        public string disabledlineback;

        [XmlAttribute("colorize-marker-back")]
        public string colorizemarkerback;

        public int ResolveColor(string colorString)
        {
            if (string.IsNullOrEmpty(colorString)) return 0x000000;
            var value = _parent.MasterScintilla.GetValue(colorString);
            while (value != null)
            {
                colorString = value.val;
                value = _parent.MasterScintilla.GetValue(colorString);
            }
            int color = Color.FromName(colorString).ToArgb();
            if (color == 0x00000000)
            {
                if (!colorString.StartsWithOrdinal("0x") || !int.TryParse(colorString.Substring(2), NumberStyles.HexNumber, null, out color))
                {
                    int.TryParse(colorString, out color);
                }
            }
            return TO_COLORREF(color);
        }

        /// <summary>
        /// Converts ARGB color value to Scintilla's Colour format (BGR)
        /// </summary>
        int TO_COLORREF(int color) => ((color & 0xFF0000) >> 16) + (color & 0x00FF00) + ((color & 0x0000FF) << 16);

        public int CaretForegroundColor => ResolveColor(string.IsNullOrEmpty(caretfore) ? "0x000000" : caretfore);

        public int CaretLineBackgroundColor => ResolveColor(string.IsNullOrEmpty(caretlineback) ? "0xececec" : caretlineback);

        public int SelectionForegroundColor => ResolveColor(string.IsNullOrEmpty(selectionfore) ? "0xffffff" : selectionfore);

        public int SelectionBackgroundColor => ResolveColor(string.IsNullOrEmpty(selectionback) ? "0x000000" : selectionback);

        public int MarkerBackgroundColor => ResolveColor(string.IsNullOrEmpty(markerback) ? "0x808080" : markerback);

        public int MarkerForegroundColor => ResolveColor(string.IsNullOrEmpty(markerfore) ? "0xffffff" : markerfore);

        public int MarginForegroundColor => ResolveColor(string.IsNullOrEmpty(marginfore) ? "0xf5f5f5" : marginfore);

        public int MarginBackgroundColor => ResolveColor(string.IsNullOrEmpty(marginback) ? "0xfaf0e6" : marginback);

        public int PrintMarginColor => ResolveColor(string.IsNullOrEmpty(printmargin) ? "0xCCCCCC" : printmargin);

        public int BookmarkLineColor => ResolveColor(string.IsNullOrEmpty(bookmarkline) ? "0xffff00" : bookmarkline);

        public int ModifiedLineColor => ResolveColor(string.IsNullOrEmpty(modifiedline) ? "0xffff00" : modifiedline);

        public int HighlightBackColor => ResolveColor(string.IsNullOrEmpty(highlightback) ? "0x808000" : highlightback);

        public int HighlightWordBackColor => ResolveColor(string.IsNullOrEmpty(highlightwordback) ? "0x0088ff" : highlightwordback);

        public int ErrorLineBack => ResolveColor(string.IsNullOrEmpty(errorlineback) ? "0xff0000" : errorlineback);

        public int DebugLineBack => ResolveColor(string.IsNullOrEmpty(debuglineback) ? "0xffa500" : debuglineback);

        public int DisabledLineBack => ResolveColor(string.IsNullOrEmpty(disabledlineback) ? "0xcccccc" : disabledlineback);

        public bool ColorizeMarkerBack => colorizemarkerback?.ToLower() == "true";
    }
}