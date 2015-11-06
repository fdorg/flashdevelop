using System;
using System.Collections.Generic;

namespace PluginCore.BBCode
{
    public class BBCodeTagHandler : IPairTagMatchHandler
    {

        protected delegate Boolean DHandler(BBCodeTagMatch tm);



        public BBCodeTagHandler()
        {
            _handlers = new Dictionary<String, DHandler>();

            _handlers["B"] = new DHandler(_hadleTag_B);
            _handlers["I"] = new DHandler(_hadleTag_I);
            _handlers["S"] = new DHandler(_hadleTag_S);
            _handlers["U"] = new DHandler(_hadleTag_U);
            _handlers["~B"] = new DHandler(_hadleTag_NotB);
            _handlers["~I"] = new DHandler(_hadleTag_NotI);
            _handlers["~S"] = new DHandler(_hadleTag_NotS);
            _handlers["~U"] = new DHandler(_hadleTag_NotU);
            _handlers["FONT"] = new DHandler(_hadleTag_Font);
            _handlers["SIZE"] = new DHandler(_hadleTag_Size);
            _handlers["COLOR"] = new DHandler(_hadleTag_Color);
            _handlers["BGCOLOR"] = new DHandler(_hadleTag_BgColor);
            _handlers["BACKCOLOR"] = new DHandler(_hadleTag_BgColor);
        }


        protected Dictionary<String, DHandler> _handlers;

        private BBCodeTagMatch _tm;
        private String _tmName;


        public Boolean handleTag(IPairTagMatch tagMatch)
        {
            if (!isHandleable(tagMatch))
                return false;

            if (_tm == null || _tm.bbCodeStyle == null)
                return false;

            return _handlers[_tmName](_tm);
        }

        public Boolean isHandleable(IPairTagMatch tagMatch)
        {
            _tm = tagMatch as BBCodeTagMatch;
            if (_tm == null || !_tm.isTagOpener)
                return false;

            _tmName = _tm.tagName;
            if (string.IsNullOrEmpty(_tmName))
                return false;

            _tmName = _tmName.ToUpper();
            if (!_handlers.ContainsKey(_tmName) || _handlers[_tmName] == null)
                return false;

            return true;
        }



        protected Boolean _hadleTag_B(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isBold = StateMode.ON;
            return true;
        }

        protected Boolean _hadleTag_I(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isItalic = StateMode.ON;
            return true;
        }

        protected Boolean _hadleTag_S(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isStriked = StateMode.ON;
            return true;
        }

        protected Boolean _hadleTag_U(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isUnderlined = StateMode.ON;
            return true;
        }


        protected Boolean _hadleTag_NotB(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isBold = StateMode.OFF;
            return true;
        }

        protected Boolean _hadleTag_NotI(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isItalic = StateMode.OFF;
            return true;
        }

        protected Boolean _hadleTag_NotS(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isStriked = StateMode.OFF;
            return true;
        }

        protected Boolean _hadleTag_NotU(BBCodeTagMatch tm)
        {
            tm.bbCodeStyle.isUnderlined = StateMode.OFF;
            return true;
        }


        protected Boolean _hadleTag_Font(BBCodeTagMatch tm)
        {
            if (tm.tagParam == null || tm.tagParam.Length < 2)
                return false;

            String fontName = tm.tagParam;
            String tmpSymbol = fontName.Substring(0, 1);
            if (tmpSymbol == "\"" || tmpSymbol == "'")
                fontName = fontName.Substring(1);

            tmpSymbol = fontName.Substring(fontName.Length - 1, 1);
            if (tmpSymbol == "\"" || tmpSymbol == "'")
                fontName = fontName.Substring(0, fontName.Length - 1);

            tm.bbCodeStyle.fontName = fontName.Length == 0 ? null : fontName;
            return true;
        }

        protected Boolean _hadleTag_Size(BBCodeTagMatch tm)
        {
            if (string.IsNullOrEmpty(tm.tagParam))
                return false;

            tm.bbCodeStyle.isAbsFontSize = true;

            String p = tm.tagParam;
            String firstSymbol = p.Substring(0, 1);
            if (firstSymbol == "+" || firstSymbol == "-")
            {
                //  p = p.Substring(1);
                tm.bbCodeStyle.isAbsFontSize = false;
            }

            tm.bbCodeStyle.fontSize = Convert.ToSingle(p);
            return true;
        }

        protected Boolean _hadleTag_Color(BBCodeTagMatch tm)
        {
            BBCodeStyle.Color c = _extractTagColor(tm.tagParam);
            if (c == null)
                return false;

            tm.bbCodeStyle.foreColor = c;
            return true;
        }

        protected Boolean _hadleTag_BgColor(BBCodeTagMatch tm)
        {
            BBCodeStyle.Color c = _extractTagColor(tm.tagParam);
            if (c == null)
                return false;

            tm.bbCodeStyle.backColor = c;
            return true;
        }


        protected BBCodeStyle.Color _extractTagColor(String tagColorParam)
        {
            if (string.IsNullOrEmpty(tagColorParam) || tagColorParam.Substring(0, 1) != "#")
                return null;

            String p = tagColorParam.Substring(1);
            int colonIndex = p.IndexOf(":");

            String colorStr = null;
            String modeStr = null;

            if (p.Length >= 8 && (colonIndex < 0 || colonIndex == 8))
            {
                colorStr = p.Substring(0, 8);
            }
            else if (p.Length >= 6 && (colonIndex < 0 || colonIndex == 6))
            {
                colorStr = "FF" + p.Substring(0, 6);
            }
            else if (p.Length >= 4 && (colonIndex < 0 || colonIndex == 4))
            {
                colorStr = p.Substring(0, 1) + p.Substring(0, 1)
                         + p.Substring(1, 1) + p.Substring(1, 1)
                         + p.Substring(2, 1) + p.Substring(2, 1)
                         + p.Substring(3, 1) + p.Substring(3, 1);
            }
            else if (p.Length >= 3 && (colonIndex < 0 || colonIndex == 3))
            {
                colorStr = "FF"
                         + p.Substring(0, 1) + p.Substring(0, 1)
                         + p.Substring(1, 1) + p.Substring(1, 1)
                         + p.Substring(2, 1) + p.Substring(2, 1);
            }
            else
            {
                if (colonIndex > -1)
                    colorStr = p.Substring(0, colonIndex);

                colorStr = "FF" + colorStr + "000000";
                colorStr = colorStr.Substring(0, 6);
            }

            if (colonIndex > -1)
                modeStr = p.Substring(colonIndex + 1).ToUpper();

            return new BBCodeStyle.Color(Convert.ToUInt32(colorStr, 16), BBCodeStyle.Color.ResolveColorMode(modeStr));
        }
    }
}
