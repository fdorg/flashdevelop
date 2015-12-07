using System.Collections.Generic;
using System.Text;
using ASCompletion.Model;

namespace PHPContext
{
    class PhpFilter
    {
        /// <summary>
        /// Called if a FileModel needs filtering
        /// - define inline AS3 ranges
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        static public string FilterSource(string src, List<InlineRange> phpRanges)
        {
            StringBuilder sb = new StringBuilder();
            int len = src.Length - 1;
            int rangeStart = -1;
            int nodeStart = -1;
            int nodeEnd = -1;
            bool skip = true;
            bool hadNL = false;
            for (int i = 0; i < len; i++)
            {
                char c = src[i];
                // keep newlines
                if (c == 10 || c == 13)
                {
                    sb.Append(c);
                    hadNL = true;
                }
                // in XML
                else if (skip)
                {
                    if (c == '<')
                    {
                        if (src[i + 1] == '?')
                        {
                            i += 2;
                            skip = false;
                            hadNL = false;
                            rangeStart = i;
                            continue;
                        }
                        else
                        {
                            nodeStart = i + 1;
                            nodeEnd = -1;
                        }
                    }
                    else if (nodeEnd < 0)
                    {
                        if (nodeStart >= 0 && (c == ' ' || c == '>'))
                        {
                            nodeEnd = i;
                        }
                    }
                }
                // in script
                else
                {
                    if (c == '?' && i < len - 2 && src[i + 1] == '>')
                    {
                        skip = true;
                        if (hadNL) phpRanges.Add(new InlineRange("php", rangeStart, i));
                        rangeStart = -1;
                    }
                    else sb.Append(c);
                }
            }
            if (rangeStart >= 0 && hadNL)
                phpRanges.Add(new InlineRange("php", rangeStart, src.Length));
            return sb.ToString();
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - modify parsed model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        static public void FilterSource(FileModel model, List<InlineRange> phpRanges)
        {
            model.InlinedIn = "html";
            model.InlinedRanges = phpRanges;
        }
    }
}
