using System;
using System.Text;
using System.Text.RegularExpressions;

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

        public static bool isJavaIdentifierPart(string s)
        {
            const string start = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl})";
            const string extend = @"(\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf})";
            Regex ident = new Regex(string.Format("{0}({0}|{1})*", start, extend));
            s = s.Normalize();
            return ident.IsMatch(s);
        }

	}

}
