using System;
using System.Text;
using System.Collections.Generic;

namespace CodeFormatter.Handlers
{
	public class ASFormatter
	{
		public ASFormatter(){}
		
		public static int GetNumberOfEmptyLinesAtEnd(StringBuilder buffer)
		{
			// Don't count the last blank line, since it will probably have data on it
			int count = 0;
			bool firstCRFound = false;
			int i = buffer.Length - 1;
			while (i >= 0)
			{
				char c = buffer.ToString().ToCharArray()[i];
				if (!Char.IsWhiteSpace(c)) return count;
				if (c == '\n')
				{
					if (!firstCRFound) firstCRFound = true;
					else count++;;
				}
				i--;
			}
			return count;
		}
		
		public static String GenerateIndent(int spaces, bool useTabs, int tabSize)
		{
			if (spaces == 0) return "";
			StringBuilder buffer = new StringBuilder();
			if (useTabs)
			{
				int tabCount = spaces / tabSize;
				for (int i = 0; i < tabCount; i++)
				{
					buffer.Append('\t');
				}
                // Change value of spaces to be left-over spaces
				spaces = spaces % tabSize;
			}
			for (int i = 0; i < spaces; i++)
			{
				buffer.Append(' ');
			}
			return buffer.ToString();
		}
		
		public static string GenerateSpaceString(int spaces)
		{
			StringBuilder buffer = new StringBuilder();
			for (int i = 0; i < spaces; i++)
			{
				buffer.Append(' ');
			}
			return buffer.ToString();
		}
		
		public static bool IsLineEmpty(StringBuilder buffer)
		{
			// The line isn't empty if it has whitespace on it
			int i = buffer.Length - 1;
			if (i < 0) return true;
			char c=buffer.ToString().ToCharArray()[i];
			if (c == '\n') return true;
			return false;
		}
		
		public static bool IsOnlyWhitespaceOnLastLine(StringBuilder buffer)
		{
			int i = buffer.Length - 1;
			while (i >= 0)
			{
				char c = buffer.ToString().ToCharArray()[i];
				if (!Char.IsWhiteSpace(c)) return false;
				if (c == '\n') return true;
				i--;
			}
			return true;
		}
		
		/// <summary>
        /// This is a weaker validation that just checks to make sure that the number of occurrences of each character is identical.
		/// </summary>
		public static bool ValidateNonWhitespaceCharCounts(string buffer, string originalSource)
		{
			// Some reasonable way of validating. Just count non-whitespace and make sure that we have at least as many
			// chars as before. Could improve to keep counts of each char so that ordering doesn't matter.
			Dictionary<char, int> originalCharMap = new Dictionary<char, int>();
			Dictionary<char, int> newCharMap = new Dictionary<char, int>();
			int originalCharCount = 0;
			for (int i = 0; i < originalSource.Length; i++)
			{
				char c = originalSource.ToCharArray()[i];
				if (!Char.IsWhiteSpace(c))
				{
					originalCharCount++;
					try
					{
						int count = originalCharMap[c];
						originalCharMap[c] = count+1;
					}
					catch (KeyNotFoundException)
					{
						originalCharMap.Add(c, 1);
					}
				}
			}
			int newCharCount = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				char c = buffer.ToCharArray()[i];
				if (!Char.IsWhiteSpace(buffer.ToCharArray()[i]))
				{
					newCharCount++;
					try
					{
						int count = newCharMap[c];
						newCharMap[c] = count + 1;
					}
					catch (KeyNotFoundException)
					{
						newCharMap.Add(c, 1);
					}
				}
			}
			if (newCharMap.Count != originalCharMap.Count) return false;
			foreach (char charAsInt in originalCharMap.Keys)
			{
				int origCount = originalCharMap[charAsInt];
				int newCount = newCharMap[charAsInt];
				if (origCount != newCount) return false;
			}
			if (newCharCount != originalCharCount) return false;
			return true;
		}
		
		public static bool ValidateNonWhitespaceIdentical(String s1, String s2)
		{
			StringBuilder newBuffer1 = new StringBuilder();
			StringBuilder newBuffer2 = new StringBuilder();
            Char[] s1Arr = s1.ToCharArray();
            Char[] s2Arr = s2.ToCharArray();
            for (int i = 0; i < s1Arr.Length; i++)
			{
                char c = s1Arr[i];
				if (!Char.IsWhiteSpace(c))
				{
					newBuffer1.Append(c);
				}
			}
            for (int i = 0; i < s2Arr.Length; i++)
			{
                char c = s2Arr[i];
				if (!Char.IsWhiteSpace(c))
				{
					newBuffer2.Append(c);
				}
			}
			return newBuffer1.ToString().Equals(newBuffer2.ToString());
		}
		
		public static void TrimWhitespaceOnEndOfBuffer(StringBuilder buffer)
		{
			// This is code to trim trailing whitespace on lines.
			for (int i = buffer.Length - 1; i >= 0; i--)
			{
				char c = buffer[i];
				if (c == ' ' || c == '\t') buffer.Remove(i, 1);
				else break;
			}
		}

	}
	
}
