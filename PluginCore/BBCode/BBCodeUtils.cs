using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.BBCode
{
    public enum StateMode : int
    {
        ON = 1,
        OFF = 0,
        DEFAULT = -1
    }

    public class BBCodeUtils
    {
        public static String assembleOutput(String input, IndexTree tree)
        {
            if (string.IsNullOrEmpty(input) || tree == null)
                return null;

            String outStr = "";
            List<IndexTree> flat = IndexTree.flattenTree(tree);
            int idxA, idxB;
            int i = -1;
            int l = flat.Count;
            while (++i < l)
            {
                idxA = flat[i].indexA + flat[i].offsetA;
                idxB = flat[i].indexB + flat[i].offsetB;

                if (idxA > idxB)
                    return input;

                outStr += input.Substring(idxA, idxB - idxA);
            }
            return outStr;
        }

        public static BBCodeStyle getNodeStyle(IndexTree tree)
        {
            if (tree == null)
                return null;

            IPairTag pairTag = tree.data as IPairTag;
            if (pairTag == null)
                return null;

            BBCodeTagMatch tm = pairTag.openerMatch as BBCodeTagMatch;
            if (tm == null)
                return null;

            return tm.bbCodeStyle;
        }

        public static BBCodeStyle getCascadedNodeStyle(IndexTree tree)
        {
            if (tree == null)
                return null;

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
            if (style == null || tf == null || selEnd <= selStart || selEnd < 0)
                return;

            tf.Select(selStart, selEnd);

            FontStyle fontStyle = tf.Font.Style;
            String fontName = tf.Font.Name;
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
                fontSize = (float)style.fontSize;
            else if (style.fontSize < 0)
                fontSize += (float)style.fontSize;

            if (!string.IsNullOrEmpty(style.fontName))
                fontName = style.fontName;


            if (style.foreColor != null)
                tf.SelectionColor = Color.FromArgb((int)((uint)(0xFF000000) | (uint)(style.foreColor.color & 0xFFFFFF)));

            if (style.backColor != null)
                tf.SelectionBackColor = Color.FromArgb((int)((uint)(0xFF000000) | (uint)(style.backColor.color & 0xFFFFFF)));


            Font font = new Font(fontName, (float)(fontSize > 1.0f ? fontSize : 1.0f), fontStyle);
            tf.SelectionFont = font;
        }

        public static void applyStyleTreeToTextbox(RichTextBox tf, String input, IndexTree bbCodeTree)
        {
            if (tf == null || bbCodeTree == null || string.IsNullOrEmpty(input))
                return;

            tf.Text = "";

            BBCodeStyle rootStyle = new BBCodeStyle();
            rootStyle.isAbsFontSize = false;
            rootStyle.fontName = tf.Font.Name;
            rootStyle.fontSize = tf.Font.Size;
            rootStyle.isAbsFontSize = true;
            rootStyle.foreColor = new BBCodeStyle.Color((uint)0xFF000000 | (uint)(tf.SelectionColor.ToArgb() & (int)0xFFFFFF), BBCodeStyle.Mode.NORMAL);
            rootStyle.backColor = new BBCodeStyle.Color((uint)0xFF000000 | (uint)(tf.SelectionBackColor.ToArgb() & (int)0xFFFFFF), BBCodeStyle.Mode.NORMAL);
            rootStyle.isBold = tf.Font.Bold ? StateMode.ON : StateMode.OFF;
            rootStyle.isItalic = tf.Font.Italic ? StateMode.ON : StateMode.OFF;
            rootStyle.isStriked = tf.Font.Strikeout ? StateMode.ON : StateMode.OFF;
            rootStyle.isUnderlined = tf.Font.Underline ? StateMode.ON : StateMode.OFF;

            PairTag rootPair = new PairTag(new BBCodeTagMatch(true, 0, "", "", "", 0, 0, false), new VoidCloserTagMatch(input.Length));
            (rootPair.openerMatch as BBCodeTagMatch).bbCodeStyle = rootStyle;

            bbCodeTree = IndexTree.cloneTree(bbCodeTree);
            bbCodeTree.data = rootPair;

            List<IndexTree> flatTree = IndexTree.flattenTree(bbCodeTree);
            String flatText = assembleOutput(input, bbCodeTree);
            String corrFlatText = _replaceEnclosures(flatText);

            IndexTree.normalizeTree(bbCodeTree);
            flatTree = IndexTree.flattenTree(bbCodeTree);

            tf.Text = corrFlatText/*flatText*/;

            if (flatText == input)
                return;

            String currText;
            String currCorrText;
            int offsetA = 0;
            int offsetB = 0;
            int idxA, idxB;
            int i = -1;
            int l = flatTree.Count;
            while (++i < l)
            {
                idxA = flatTree[i].indexA;
                idxB = flatTree[i].indexB;

                currText = flatText.Substring(idxA, idxB - idxA);
                currCorrText = _replaceEnclosures(currText);

                offsetB = currCorrText.Length + idxA - idxB;

                applyStyleToTextbox(getCascadedNodeStyle(flatTree[i]),
                                    tf,
                                    idxA + offsetA,
                                    idxB + offsetA + offsetB);

                offsetA += offsetB;
            }

            tf.Select(tf.Text.Length, tf.Text.Length);
        }


        private static BottomUpParser bbCodeParser = null;
        private static RichTextBox tempRTB = null;

        private static void _init()
        {
            if (bbCodeParser == null)
            {
                bbCodeParser = new BottomUpParser();
                bbCodeParser.pairTagHandler = new BBCodeTagHandler();
                bbCodeParser.pairTagMatcher = new BBCodeTagMatcher();
            }
            if (tempRTB == null)
            {
                tempRTB = new RichTextBox();
                tempRTB.Text = "";
                tempRTB.WordWrap = false;
                tempRTB.ScrollBars = RichTextBoxScrollBars.None;
            }
        }

        public static String bbCodeToRtf(String bbCodeText)
        {
            _init();

            return bbCodeToRtf(bbCodeText, tempRTB);
        }
        public static String bbCodeToRtf(String bbCodeText, RichTextBox texbox)
        {
            _init();

            bbCodeParser.input = bbCodeText;
            applyStyleTreeToTextbox(texbox, bbCodeParser.input, bbCodeParser.parse());

            return texbox.Rtf;
        }

        public static String rtfToText(String rtfText)
        {
            _init();

            tempRTB.Rtf = rtfText;
            return tempRTB.Text;
        }


        private static String _replaceEnclosures(String input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            String outStr = input;
            outStr = outStr.Replace("\\[", "[");
            outStr = outStr.Replace("\\]", "]");
            outStr = outStr.Replace("\\\\", "\\");

            return outStr;
        }
    }
}
