using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.BBCode
{
    public enum StateMode
    {
        ON = 1,
        OFF = 0,
        DEFAULT = -1
    }

    public class BBCodeUtils
    {
        public static string assembleOutput(string input, IndexTree tree)
        {
            if (string.IsNullOrEmpty(input) || tree is null) return null;

            string outStr = "";
            List<IndexTree> flat = IndexTree.flattenTree(tree);
            int i = -1;
            int l = flat.Count;
            while (++i < l)
            {
                var idxA = flat[i].indexA + flat[i].offsetA;
                var idxB = flat[i].indexB + flat[i].offsetB;

                if (idxA > idxB)
                    return input;

                outStr += input.Substring(idxA, idxB - idxA);
            }
            return outStr;
        }

        public static BBCodeStyle getNodeStyle(IndexTree tree)
        {
            IPairTag pairTag = tree?.data as IPairTag;

            BBCodeTagMatch tm = pairTag?.openerMatch as BBCodeTagMatch;

            return tm?.bbCodeStyle;
        }

        public static BBCodeStyle getCascadedNodeStyle(IndexTree tree)
        {
            if (tree is null) return null;

            List<BBCodeStyle> styleHierarchy = new List<BBCodeStyle>();
            IndexTree currTree = tree;
            while (currTree != null)
            {
                styleHierarchy.Add(getNodeStyle(currTree));
                currTree = currTree.parentNode;
            }
            styleHierarchy.Reverse();

            return BBCodeStyle.fuseStyleHierarchy(styleHierarchy);
        }

        public static void applyStyleToTextbox(BBCodeStyle style, RichTextBox tf, int selStart, int selEnd)
        {
            if (style is null || tf is null || selEnd <= selStart || selEnd < 0) return;

            tf.Select(selStart, selEnd);

            FontStyle fontStyle = tf.Font.Style;
            string fontName = tf.Font.Name;
            float fontSize = tf.Font.Size;


            if (style.isBold == StateMode.ON)
                fontStyle |= FontStyle.Bold;
            else if (style.isBold == StateMode.OFF)
                fontStyle &= ~FontStyle.Bold;

            if (style.isItalic == StateMode.ON)
                fontStyle |= FontStyle.Italic;
            else if (style.isItalic == StateMode.OFF)
                fontStyle &= ~FontStyle.Italic;

            if (style.isUnderlined == StateMode.ON)
                fontStyle |= FontStyle.Underline;
            else if (style.isUnderlined == StateMode.OFF)
                fontStyle &= ~FontStyle.Underline;

            if (style.isStriked == StateMode.ON)
                fontStyle |= FontStyle.Strikeout;
            else if (style.isStriked == StateMode.OFF)
                fontStyle &= ~FontStyle.Strikeout;


            if (style.fontSize > 0)
                fontSize = style.fontSize;
            else if (style.fontSize < 0)
                fontSize += style.fontSize;

            if (!string.IsNullOrEmpty(style.fontName))
                fontName = style.fontName;


            if (style.foreColor != null)
                tf.SelectionColor = Color.FromArgb((int)(0xFF000000 | style.foreColor.color & 0xFFFFFF));

            if (style.backColor != null)
                tf.SelectionBackColor = Color.FromArgb((int)(0xFF000000 | style.backColor.color & 0xFFFFFF));


            Font font = new Font(fontName, fontSize > 1.0f ? fontSize : 1.0f, fontStyle);
            tf.SelectionFont = font;
        }

        public static void applyStyleTreeToTextbox(RichTextBox tf, string input, IndexTree bbCodeTree)
        {
            if (tf is null || tf.IsDisposed || bbCodeTree is null || string.IsNullOrEmpty(input)) return;

            tf.Text = "";

            BBCodeStyle rootStyle = new BBCodeStyle();
            rootStyle.isAbsFontSize = false;
            rootStyle.fontName = tf.Font.Name;
            rootStyle.fontSize = tf.Font.Size;
            rootStyle.isAbsFontSize = true;
            rootStyle.foreColor = new BBCodeStyle.Color(0xFF000000 | (uint)(tf.SelectionColor.ToArgb() & 0xFFFFFF), BBCodeStyle.Mode.NORMAL);
            rootStyle.backColor = new BBCodeStyle.Color(0xFF000000 | (uint)(tf.SelectionBackColor.ToArgb() & 0xFFFFFF), BBCodeStyle.Mode.NORMAL);
            rootStyle.isBold = tf.Font.Bold ? StateMode.ON : StateMode.OFF;
            rootStyle.isItalic = tf.Font.Italic ? StateMode.ON : StateMode.OFF;
            rootStyle.isStriked = tf.Font.Strikeout ? StateMode.ON : StateMode.OFF;
            rootStyle.isUnderlined = tf.Font.Underline ? StateMode.ON : StateMode.OFF;

            PairTag rootPair = new PairTag(new BBCodeTagMatch(true, 0, "", "", "", 0, 0, false), new VoidCloserTagMatch(input.Length));
            ((BBCodeTagMatch) rootPair.openerMatch).bbCodeStyle = rootStyle;

            bbCodeTree = IndexTree.cloneTree(bbCodeTree);
            bbCodeTree.data = rootPair;

            List<IndexTree> flatTree = IndexTree.flattenTree(bbCodeTree);
            string flatText = assembleOutput(input, bbCodeTree);
            string corrFlatText = _replaceEnclosures(flatText);

            IndexTree.normalizeTree(bbCodeTree);
            flatTree = IndexTree.flattenTree(bbCodeTree);

            tf.Text = corrFlatText/*flatText*/;

            if (flatText == input)
                return;

            int offsetA = 0;
            int i = -1;
            int l = flatTree.Count;
            while (++i < l)
            {
                var idxA = flatTree[i].indexA;
                var idxB = flatTree[i].indexB;

                var currText = flatText.Substring(idxA, idxB - idxA);
                var currCorrText = _replaceEnclosures(currText);

                var offsetB = currCorrText.Length + idxA - idxB;

                applyStyleToTextbox(getCascadedNodeStyle(flatTree[i]),
                                    tf,
                                    idxA + offsetA,
                                    idxB + offsetA + offsetB);

                offsetA += offsetB;
            }

            var textLength = tf.TextLength;
            tf.Select(textLength, textLength);
        }


        static BottomUpParser bbCodeParser;
        static RichTextBox tempRTB;

        static void Init()
        {
            bbCodeParser ??= new BottomUpParser
            {
                pairTagHandler = new BBCodeTagHandler(),
                pairTagMatcher = new BBCodeTagMatcher()
            };
            tempRTB ??= new RichTextBox {Text = "", WordWrap = false, ScrollBars = RichTextBoxScrollBars.None};
        }

        public static string bbCodeToRtf(string bbCodeText)
        {
            Init();

            return bbCodeToRtf(bbCodeText, tempRTB);
        }
        public static string bbCodeToRtf(string bbCodeText, RichTextBox texbox)
        {
            if (texbox is null || texbox.IsDisposed) return string.Empty;
            Init();

            bbCodeParser.input = bbCodeText;
            applyStyleTreeToTextbox(texbox, bbCodeParser.input, bbCodeParser.parse());

            return texbox.Rtf;
        }

        public static string rtfToText(string rtfText)
        {
            Init();

            tempRTB.Rtf = rtfText;
            return tempRTB.Text;
        }


        static string _replaceEnclosures(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string outStr = input;
            outStr = outStr.Replace("\\[", "[");
            outStr = outStr.Replace("\\]", "]");
            outStr = outStr.Replace("\\\\", "\\");

            return outStr;
        }
    }
}
