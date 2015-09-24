using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class WinFormUtils
    {

        private static RichTextBox _tempRTB = new RichTextBox();

        public static Size MeasureRichTextBox(RichTextBox richTextBox)
        {
            return MeasureRichTextBox(richTextBox, true, richTextBox.Width, richTextBox.Height, richTextBox.WordWrap);
        }
        public static Size MeasureRichTextBox(RichTextBox richTextBox, Boolean useSelfForTest, int width, int height, Boolean wordWrap)
        {
            Size outSize = new Size();

            if (richTextBox == null)
                return outSize;

            String rtf = richTextBox.Rtf;
            if (rtf == null)
                return outSize;

            int lastIdx = rtf.LastIndexOf("}");
            if (lastIdx < 1)
                return outSize;

            _tempRTB.Visible = false;
            RichTextBox rtb = useSelfForTest ? richTextBox : _tempRTB;
            rtb.Rtf = rtf.Substring(0, lastIdx) + @"\par}";
            
            rtb.Width = width;
            rtb.Height = height;
            rtb.WordWrap = false;
            rtb.WordWrap = wordWrap;

            if (rtb.ScrollBars != richTextBox.ScrollBars)
                rtb.ScrollBars = richTextBox.ScrollBars;

            outSize.Height = rtb.GetPositionFromCharIndex(rtb.TextLength).Y;

            lastIdx = -1;
            int maxW = rtb.GetPositionFromCharIndex(rtb.TextLength).X;
            int currW = 0;
            while (true)
            {
                lastIdx = rtb.Text.IndexOf("\n", lastIdx + 1);
                if (lastIdx < 0)
                    break;

                currW = rtb.GetPositionFromCharIndex(lastIdx).X;
                if (currW > maxW)
                    maxW = currW;
            }

            if (wordWrap)
            {
                float delta;
                float maxDelta = -1.0f;
                int firstLineChars = 0;
                int firstLineCount = 1;
                int currLineChars = 0;

                int prevW = -1;
                int i, l = rtb.Text.Length;
                for (i = 0; i < l;)
                {
                    currW = rtb.GetPositionFromCharIndex(i).X;
                    if (currW > maxW)
                        maxW = currW;

                    if (maxDelta < 0.0f && prevW > currW)
                    {
                        if (currLineChars > firstLineChars)
                            firstLineChars = currLineChars;
                        currLineChars = 0;

                        if (--firstLineCount <= 0)
                            maxDelta = 0.5f * (float)firstLineChars;
                    }
                    prevW = currW;

                    if (maxDelta < 0.0f)
                    {
                        ++currLineChars;
                        ++i;
                        continue;
                    }

                    delta = lerp(maxDelta, -0.1f * maxDelta, ((float)currW) / ((float)width));

                    if (delta < 1.0f)
                        delta = 1.0f;

                    i += (int)delta;
                }
            }

            if (useSelfForTest)
                rtb.Rtf = rtf;

            outSize.Width = maxW;
            return outSize;
        }

        // Linear interpolation
        private static float lerp(float a, float b, float pos/*[0..1]*/)
        {
            if (pos < 0.0f) pos = 0.0f;
            if (pos > 1.0f) pos = 1.0f;

            return a + pos * (b - a);
        }
    }
}
