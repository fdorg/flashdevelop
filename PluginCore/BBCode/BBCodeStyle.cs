using System;
using System.Collections.Generic;

namespace PluginCore.BBCode
{
    public class BBCodeStyle
    {
        #region Statics

        public static BBCodeStyle fuseStyles(BBCodeStyle child, BBCodeStyle parent)
        {
            if (parent == null)
                return child != null ? child.clone() : null;

            if (child == null)
                return parent.clone();

            BBCodeStyle outStyle = child.clone();

            if (outStyle.isBold == StateMode.DEFAULT)
                outStyle.isBold = parent.isBold;

            if (outStyle.isItalic == StateMode.DEFAULT)
                outStyle.isItalic = parent.isItalic;

            if (outStyle.isStriked == StateMode.DEFAULT)
                outStyle.isStriked = parent.isStriked;

            if (outStyle.isUnderlined == StateMode.DEFAULT)
                outStyle.isUnderlined = parent.isUnderlined;

            if (outStyle.fontSize == 0)
            {
                outStyle.fontSize = parent.fontSize;
                outStyle.isAbsFontSize = parent.isAbsFontSize;
            }
            else
            {
                if (!outStyle.isAbsFontSize)
                {
                    outStyle.isAbsFontSize = (bool)(parent.isAbsFontSize && parent.fontSize != 0);
                    outStyle.fontSize = parent.fontSize + outStyle.fontSize;
                }
                else
                {
                    if (outStyle.fontSize < 0)
                        outStyle.fontSize = 1;
                }
            }

            if (string.IsNullOrEmpty(outStyle.fontName))
                outStyle.fontName = parent.fontName;

            if (outStyle.backColor == null)
                outStyle.backColor = parent.backColor;
            else
                outStyle.backColor = Color.Mix(parent.backColor, outStyle.backColor);

            if (outStyle.foreColor == null)
                outStyle.foreColor = parent.foreColor;
            else
                outStyle.foreColor = Color.Mix(outStyle.backColor, outStyle.foreColor);

            return outStyle;
        }

        public static BBCodeStyle fuseStyleHierarchy(List<BBCodeStyle> parentToChildHierarchy)
        {
            if (parentToChildHierarchy == null || parentToChildHierarchy.Count < 1)
                return null;

            if (parentToChildHierarchy.Count == 1)
                return parentToChildHierarchy[0].clone();

            BBCodeStyle t = parentToChildHierarchy[0].clone();

            if (t.foreColor == null) t.foreColor = new Color(0x000000, Mode.NORMAL);
            if (t.backColor == null) t.backColor = new Color(0xCC99CC, Mode.NORMAL);

            if (t.isBold == StateMode.DEFAULT) t.isBold = StateMode.OFF;
            if (t.isItalic == StateMode.DEFAULT) t.isItalic = StateMode.OFF;
            if (t.isStriked == StateMode.DEFAULT) t.isStriked = StateMode.OFF;
            if (t.isUnderlined == StateMode.DEFAULT) t.isUnderlined = StateMode.OFF;

            if (!t.isAbsFontSize)
            {
                t.isAbsFontSize = true;

                if(t.fontSize <= 0.0f)
                    t.fontSize = 12.0f;
            }

            int i, l = parentToChildHierarchy.Count;
            for (i = 1; i < l; i++)
                t = fuseStyles(parentToChildHierarchy[i], t);

            return t;
        }

        #endregion

        #region Dynamics

        public BBCodeStyle()
        {
        }


        public StateMode isBold = StateMode.DEFAULT;
        public StateMode isItalic = StateMode.DEFAULT;
        public StateMode isStriked = StateMode.DEFAULT;
        public StateMode isUnderlined = StateMode.DEFAULT;

        public String fontName = null;
        public float fontSize = 0;
        public bool isAbsFontSize = true;

        public Color foreColor = null;
        public Color backColor = null;


        public BBCodeStyle clone()
        {
            BBCodeStyle c = new BBCodeStyle();

            c.isBold = this.isBold;
            c.isItalic = this.isItalic;
            c.isStriked = this.isStriked;
            c.isUnderlined = this.isUnderlined;
            c.fontName = this.fontName;
            c.fontSize = this.fontSize;
            c.isAbsFontSize = this.isAbsFontSize;
            c.foreColor = this.foreColor == null ? null : this.foreColor.clone();
            c.backColor = this.backColor == null ? null : this.backColor.clone();

            return c;
        }

        override public String ToString()
        {
            return "[bbCodeStyle"
                   + " isBold=" + isBold
                   + " isItalic='" + isItalic + "'"
                   + " isStriked='" + isStriked + "'"
                   + " isUnderlined='" + isUnderlined + "'"
                   + " fontName='" + (fontName == null ? "null" : fontName) + "'"
                   + " fontSize='" + fontSize + "'"
                   + " isAbsFontSize='" + isAbsFontSize.ToString() + "'"
                   + " foreColor='" + (foreColor == null ? "null" : foreColor.ToString()) + "'"
                   + " backColor='" + (backColor == null ? "null" : backColor.ToString()) + "'"
                   + "]";
        }

        #endregion

        #region SubClasses

        public class Color
        {
            private delegate float DColorChannelMixer(float back, float fore);

            #region Statics

            public static Color Mix(Color back, Color fore)
            {
                if (back == null || fore == null)
                    return null;

                float backR = ((float)((back.color >> 16) & 0xFF) / 255.0f);
                float backG = ((float)((back.color >> 8) & 0xFF) / 255.0f);
                float backB = ((float)((back.color) & 0xFF) / 255.0f);

                float foreA = ((float)((fore.color >> 24) & 0xFF) / 255.0f);
                float foreR = ((float)((fore.color >> 16) & 0xFF) / 255.0f);
                float foreG = ((float)((fore.color >> 8) & 0xFF) / 255.0f);
                float foreB = ((float)((fore.color) & 0xFF) / 255.0f);

                float outR = MixChannel(backR, foreR, fore.mode);
                float outG = MixChannel(backG, foreG, fore.mode);
                float outB = MixChannel(backB, foreB, fore.mode);

                // simplified version
                outR = lerp(backR, outR, foreA);
                outG = lerp(backG, outG, foreA);
                outB = lerp(backB, outB, foreA);

                if (outR < 0.0f) outR = 0.0f;
                if (outR > 1.0f) outR = 1.0f;

                if (outG < 0.0f) outG = 0.0f;
                if (outG > 1.0f) outG = 1.0f;

                if (outB < 0.0f) outB = 0.0f;
                if (outB > 1.0f) outB = 1.0f;

                return new Color((uint)( ((uint)0xFF000000)
                                        |((uint)(255.0f * outR) << 16)
                                        |((uint)(255.0f * outG) << 8)
                                        |((uint)(255.0f * outB))), Mode.NORMAL);
            }

            public static float MixChannel(float back, float fore, Mode mode)
            {
                _InitStatics();

                if (mode == 0)
                    mode = Mode.NORMAL;

                return _colorChannelMixers[mode](back, fore);
            }

            public static Mode ResolveColorMode(String modeStr)
            {
                if (string.IsNullOrEmpty(modeStr))
                    return Mode.NORMAL;

                _InitStatics();

                if (_colorChannelMixersHash.ContainsKey(modeStr))
                    return _colorChannelMixersHash[modeStr];

                return Mode.NORMAL;
            }

            private static float lerp(float a, float b, float pos)
            {
                return a + pos * (b - a);
            }


            private static bool _isInitedStatics = false;
            private static Dictionary<Mode, DColorChannelMixer> _colorChannelMixers;
            private static Dictionary<String, Mode> _colorChannelMixersHash;

            private static void _InitStatics()
            {
                if (_isInitedStatics)
                    return;

                _colorChannelMixers = new Dictionary<Mode, DColorChannelMixer>();
                _colorChannelMixers[Mode.NORMAL] = new DColorChannelMixer(_channelMixer_NORMAL);
                _colorChannelMixers[Mode.ADD] = new DColorChannelMixer(_channelMixer_ADD);
                _colorChannelMixers[Mode.SUBTRACT] = new DColorChannelMixer(_channelMixer_SUBTRACT);
                _colorChannelMixers[Mode.MULTIPLY] = new DColorChannelMixer(_channelMixer_MULTIPLY);
                _colorChannelMixers[Mode.DIVIDE] = new DColorChannelMixer(_channelMixer_DIVIDE);
                _colorChannelMixers[Mode.DIFFERENCE] = new DColorChannelMixer(_channelMixer_DIFFERENCE);
                _colorChannelMixers[Mode.EXCLUSION] = new DColorChannelMixer(_channelMixer_EXCLUSION);
                _colorChannelMixers[Mode.OVERLAY] = new DColorChannelMixer(_channelMixer_OVERLAY);
                _colorChannelMixers[Mode.HARDLIGHT] = new DColorChannelMixer(_channelMixer_HARDLIGHT);

                _colorChannelMixersHash = new Dictionary<string, Mode>();
                _colorChannelMixersHash["NORMAL"] = Mode.NORMAL;
                _colorChannelMixersHash["ADD"] = Mode.ADD;
                _colorChannelMixersHash["SUBTRACT"] = Mode.SUBTRACT;
                _colorChannelMixersHash["MULTIPLY"] = Mode.MULTIPLY;
                _colorChannelMixersHash["DIVIDE"] = Mode.DIVIDE;
                _colorChannelMixersHash["DIFFERENCE"] = Mode.DIFFERENCE;
                _colorChannelMixersHash["EXCLUSION"] = Mode.EXCLUSION;
                _colorChannelMixersHash["OVERLAY"] = Mode.OVERLAY;
                _colorChannelMixersHash["HARDLIGHT"] = Mode.HARDLIGHT;

                _isInitedStatics = true;
            }


            private static float _channelMixer_NORMAL(float back, float fore)
            {
                return fore;
            }
            private static float _channelMixer_ADD(float back, float fore)
            {
                return back + fore;
            }
            private static float _channelMixer_SUBTRACT(float back, float fore)
            {
                return back + fore - 1.0f;
            }
            private static float _channelMixer_MULTIPLY(float back, float fore)
            {
                return back * fore;
            }
            private static float _channelMixer_DIVIDE(float back, float fore)
            {
                return back / fore;
            }
            private static float _channelMixer_DIFFERENCE(float back, float fore)
            {
                return Math.Abs(back - fore);
            }
            private static float _channelMixer_EXCLUSION(float back, float fore)
            {
                return back + fore - 2.0f * back * fore;
            }
            private static float _channelMixer_OVERLAY(float back, float fore)
            {
                if (back < 0.5)
                    return 2.0f * back * fore;
                return 2.0f * (0.5f - (1.0f - fore) * (1.0f - back));
            }
            private static float _channelMixer_HARDLIGHT(float back, float fore)
            {
                return _channelMixer_OVERLAY(fore, back);
            }
            
            #endregion



            public Color()
            {
            }
            public Color(uint color)
            {
                this.color = color;
            }
            public Color(uint color, Mode mode)
            {
                this.color = color;
                this.mode = mode;
            }


            public uint color = 0xFF000000;
            public Mode mode = Mode.NORMAL;


            public Color clone()
            {
                return new Color(color, mode);
            }
            public override string ToString()
            {
                return "[bbCodeStyle.Color"
                       + " color=" + color.ToString("X")
                       + " mode='" + mode + "'"
                       + "]";
            }
        }

        #region SubClasses

        public enum Mode : uint
        {
            NORMAL = 0,
            ADD = 1,
            SUBTRACT = 2,
            MULTIPLY = 3,
            DIVIDE = 4,
            DIFFERENCE = 5,
            EXCLUSION = 6,
            OVERLAY = 7,
            HARDLIGHT = 8
        }

        #endregion

        #endregion

    }
}
