using System;

namespace PluginCore.Utilities
{
    public class LineEndDetector
    {
        /// <summary>
        /// Gets the correct EOL marker
        /// </summary>
        public static String GetNewLineMarker(Int32 eolMode)
        {
            if (eolMode == 1) return "\r";
            else if (eolMode == 2) return "\n";
            else return "\r\n";
        }

        /// <summary>
        /// Basic detection of text's EOL marker
        /// </summary>
        public static Int32 DetectNewLineMarker(String text, Int32 defaultMarker)
        {
            Int32 cr = text.IndexOf("\r");
            Int32 lf = text.IndexOf("\n");
            if ((cr >= 0) && (lf >= 0))
            {
                if (cr < lf) return 0;
                else return 2;
            }
            else if ((cr < 0) && (lf < 0))
            {
                return (Int32)PluginBase.MainForm.Settings.EOLMode;
            }
            else if (lf < 0) return 1;
            else return 2;
        }

    }

}
