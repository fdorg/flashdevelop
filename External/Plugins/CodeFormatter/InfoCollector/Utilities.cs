using System;
using System.Text;

namespace CodeFormatter.InfoCollector
{
    public class Utilities
    {
        public static String convertCarriageReturnsToLineFeeds(String source)
        {
            // We know the result string will be same length
            StringBuilder buffer = new StringBuilder(source.Length);
            char[] sourceArray = source.ToCharArray();
            for (int i = 0; i < sourceArray.Length; i++)
            {
                char c = sourceArray[i];
                if (c == '\r')
                {
                    // If this is part of a cr/lf pair, then don't touch it
                    if (i + 1 < sourceArray.Length && sourceArray[i + 1] == '\n')
                    {
                        buffer.Append('\r'); // Keep the CR since it is still appropriate
                        continue;
                    }
                    buffer.Append('\n'); // Convert to be a \n
                }
                else buffer.Append(c);
            }
            return buffer.ToString();
        }

        public static bool isJavaIdentifierPart(char ch)
        {
            return Char.IsLetterOrDigit(ch) || ch == '_' || ch == '$';
        }

    }

}
