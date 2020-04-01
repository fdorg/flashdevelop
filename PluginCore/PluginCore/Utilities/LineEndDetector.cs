using System;

namespace PluginCore.Utilities
{
    public class LineEndDetector
    {
        /// <summary>
        /// Gets the correct EOL marker
        /// </summary>
        public static string GetNewLineMarker(int eolMode)
        {
            return eolMode switch
            {
                1 => "\r",
                2 => "\n",
                _ => "\r\n",
            };
        }

        /// <summary>
        /// Basic detection of text's EOL marker
        /// </summary>
        [Obsolete("Please use LineEndDetector.DetectNewLineMarker(string text)")]
        public static int DetectNewLineMarker(string text, int defaultMarker) => DetectNewLineMarker(text);

        /// <summary>
        /// Basic detection of text's EOL marker
        /// </summary>
        public static int DetectNewLineMarker(string text)
        {
            var cr = text.IndexOf('\r');
            var lf = text.IndexOf('\n');
            if (cr >= 0 && lf >= 0) return cr >= lf ? 2 : 0;
            if (cr < 0 && lf < 0) return (int) PluginBase.Settings.EOLMode;
            return lf < 0
                ? 1
                : 2;
        }
    }
}