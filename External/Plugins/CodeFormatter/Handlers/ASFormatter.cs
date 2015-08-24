using System;
using System.Collections.Generic;
using System.Text;
using CodeFormatter.InfoCollector;

namespace CodeFormatter.Handlers
{
    public class ASFormatter 
    {

        public static int getNumberOfEmptyLinesAtEnd(StringBuilder buffer)
        {
            return getNumberOfEmptyLinesAtEnd(buffer, -1);
        }

        /**
         * @param buffer
         * @param fromIndex index should be -1 for "at end" or the index just after the search start pos (so equivalent to buffer.lenght())
         * @return
         */
        public static int getNumberOfEmptyLinesAtEnd(StringBuilder buffer, int fromIndex)
        {
            if (fromIndex<0)
                fromIndex=buffer.Length;
        
            //don't count the last blank line, since it will probably have data on it
            int count=0;
            bool firstCRFound=false;
            int i=fromIndex-1;
            while (i>=0)
            {
                char c=buffer[i];
                if (!AntlrUtilities.isASWhitespace(c))
                    return count;
                if (c=='\n')
                {
                    if (!firstCRFound)
                        firstCRFound=true;
                    else
                        count++;;
                }
                i--;
            }

            return count;
        }

        public static String generateSpaceString(int spaces)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i=0;i<spaces;i++)
            {
                buffer.Append(' ');
            }
            return buffer.ToString();
        }
    
        public static String generateIndent(int spaces)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i=0;i<spaces;i++)
            {
                buffer.Append(' ');
            }
            return buffer.ToString();
        }

        public static bool isLineEmpty(StringBuilder buffer)
        {
            //the line isn't empty if it has whitespace on it
            int i=buffer.Length-1;
            if (i<0)
                return true;
        
            char c=buffer[i];
            if (c=='\n')
                return true;
        
            return false;
        }

        public static bool isOnlyWhitespaceOnLastLine(StringBuilder buffer)
        {
            int i=buffer.Length-1;
            while (i>=0)
            {
                char c=buffer[i];
                if (!AntlrUtilities.isASWhitespace(c))
                    return false;
                if (c=='\n')
                    return true;
                i--;
            }

            return true;
        }

        public static String generateIndent(int spaces, bool useTabs, int tabSize)
        {
            if (spaces==0)
                return "";
            String buffer="";
        
            if (useTabs)
            {
                int tabCount=spaces/tabSize;
                for (int i=0;i<tabCount;i++)
                {
                    buffer += '\t';
                }
                spaces=spaces%tabSize; //change value of spaces to be left-over spaces
            }

            for (int i=0;i<spaces;i++)
            {
                buffer += ' ';
            }
            return buffer;
        }

        /**
         * This is a weaker validation that just checks to make sure that the number of occurrences of each character
         * is identical.
         * @param buffer
         * @param originalSource
         * @return
         */
        public static bool validateNonWhitespaceCharCounts(String buffer, String originalSource)
        {
            //some reasonable way of validating.  Just count non-whitespace and make sure that we have at least as many
            //chars as before.  Could improve to keep counts of each char so that ordering doesn't matter.
            Dictionary<char, Int32> originalCharMap = new Dictionary<char, Int32>();
            Dictionary<char, Int32> newCharMap = new Dictionary<char, Int32>();
        
            int originalCharCount=0;
            for (int i=0;i<originalSource.Length;i++)
            {
                char c=originalSource[i];
                if (!AntlrUtilities.isASWhitespace(c))
                {
                    originalCharCount++;
                    try
                    {
                        int count = originalCharMap[c];
                        originalCharMap[c] = count + 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        originalCharMap.Add(c, 1);
                    }
                }
            }
        
            int newCharCount=0;
            for (int i=0;i<buffer.Length;i++)
            {
                char c=buffer[i];
                if (!AntlrUtilities.isASWhitespace(buffer[i]))
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
        
            if (newCharMap.Count!=originalCharMap.Count)
                return false;

            foreach (char charAsInt in originalCharMap.Keys)
            {
                Int32 origCount=originalCharMap[charAsInt];
                Int32 newCount=newCharMap[charAsInt];
                if (origCount!=newCount)
                    return false;
            }
        
            if (newCharCount!=originalCharCount)
                return false;
        
            return true;
        }
    
        public static bool validateNonWhitespaceIdentical(String s1, String s2)
        {
            String newBuffer1 = "";
            String newBuffer2 = "";
            for (int i=0;i<s1.Length;i++)
            {
                char c=s1[i];
                if (!AntlrUtilities.isASWhitespace(c))
                {
                    newBuffer1 += c;
                }
            }
            for (int i=0;i<s2.Length;i++)
            {
                char c=s2[i];
                if (!AntlrUtilities.isASWhitespace(c))
                {
                    newBuffer2 += c;
                }
            }
            return newBuffer1 == newBuffer2;
        }

        public static void trimAllWhitespaceOnEndOfBuffer(StringBuilder buffer)
        {
            //This is code to trim all trailing whitespace in buffer, including newlines.
            for (int i=buffer.Length-1;i>=0;i--)
            {
                char c=buffer[i];
                if (c==' ' || c=='\t' || c=='\n')
                    buffer.Remove(i, 1);
                else
                    break;
            }
        }

        public static void trimWhitespaceOnEndOfBuffer(StringBuilder buffer)
        {
            //This is code to trim trailing whitespace on lines.
            for (int i=buffer.Length-1;i>=0;i--)
            {
                char c=buffer[i];
                if (c==' ' || c=='\t')
                    buffer.Remove(i, 1);
                else
                    break;
            }
        }
    }

}