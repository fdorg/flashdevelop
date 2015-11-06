using System;
using System.Text;

namespace ASCompletion.Completion
{
    public class ReformatOptions
    {
        public string Newline = "\r\n";
        public string Tab = "\t";
        public bool CondenseWhitespace = false;
        public bool BraceAfterLine = false;
        public bool SpaceBeforeFunctionCall = false;
        public string CompactChars = "";
        public string SpacedChars = "";
        public string[] AddSpaceAfter = new string[] {};
        public string Operators = "=+-*/%<>&|^";
        public bool IsPhp = false;
        public bool IsHaXe = false;
        public string WordCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_$@";
    }

    public class Reformater
    {
        /// <summary>
        /// Adjust whitespace according to options
        /// </summary>
        /// <param name="txt">Original text</param>
        /// <param name="options">Formatting options</param>
        /// <param name="offset">Cursor position in line</param>
        /// <returns></returns>
        public static string ReformatLine(string txt, ReformatOptions options, ref int offset)
        {
            StringBuilder sb = new StringBuilder(txt.Length);
            int i = 0;
            int n = txt.Length;
            char c = ' ';
            char c2 = ' ';

            // copy indentation
            while (i < n && txt[i] <= 32) sb.Append(txt[i++]);
            string indentation = sb.ToString();

            // first char
            if (i < n && txt[i] != '/' && txt[i] != '"' && txt[i] != '\'' && txt[i] != '<' && txt[i] != ']' && txt[i] != '?') 
            {
                c = c2 = txt[i++];
                sb.Append(c);
                if (options.SpacedChars.IndexOf(c) >= 0)
                    while (i < n && options.SpacedChars.IndexOf(txt[i]) >= 0) sb.Append(txt[i++]);
            }

            // reformat line
            bool fixedOffset = (i > offset);
            bool needSpace = false;
            int inString = 0;
            bool inComments = false;
            while (i < n)
            {
                if (!fixedOffset && i == offset - 1)
                {
                    fixedOffset = true;
                    offset = sb.Length + 1;
                }
                c = txt[i++];
                if (c == '\r') 
                {
                    sb.Append(c);
                    break;
                }
                // skip string literals
                if (c == '\\')
                {
                    sb.Append(c);
                    if (i < n)
                    {
                        c = txt[i++];
                        sb.Append(c);
                    }
                    continue;
                }
                switch (inString)
                {
                    case 0:
                        if (c == '"') inString = 1;
                        else if (c == '\'') inString = 2;
                        break;
                    case 1:
                        sb.Append(c);
                        if (c == '"') inString = 0;
                        c2 = c;
                        continue;
                    case 2:
                        sb.Append(c);
                        if (c == '\'') inString = 0;
                        c2 = c;
                        continue;
                }

                // comments
                if (inComments)
                {
                    sb.Append(c);
                    if (i < n && c == '*' && txt[i] == '/')
                    {
                        i++;
                        sb.Append('/');
                        c2 = ' ';
                        inComments = false;
                    }
                    continue;
                }
                else if (i < n &&  c == '/' && (txt[i] == '*' || txt[i] == '/'))
                {
                    if (txt[i] == '/')
                    {
                        sb.Append(txt.Substring(i - 1));
                        break;
                    }
                    else if (txt[i] == '*')
                    {
                        inComments = true;
                        sb.Append(c);
                        continue;
                    }
                }

                // generic type
                if (c == '<' && (c2 == '.' || (options.IsHaXe && (Char.IsLetterOrDigit(c2) || c2 == '{'))))
                {
                    int i2 = i;
                    if (lookupGeneric(options,txt, ref i))
                    {
                        sb.Append(c).Append(txt.Substring(i2, i - i2));
                        c2 = '$';
                        needSpace = false;
                        continue;
                    }
                }

                // literal XML
                if (c == '<')
                {
                    int i2 = i;
                    if (lookupXML(txt, ref i))
                    {
                        sb.Append(c).Append(txt.Substring(i2, i - i2));
                        c2 = '$';
                        needSpace = false;
                        continue;
                    }
                }
                else if ((c == ']' && i < n - 1 && txt[i] == ']' && txt[i + 1] == '>')
                    || (c == '-' && i < n - 1 && txt[i] == '-' && txt[i + 1] == '>'))
                {
                    sb.Append(c).Append(txt.Substring(i, 2));
                    i += 2;
                    c2 = ' ';
                    needSpace = false;
                    continue;
                }
                else if (c == '?' && i < n - 1 && txt[i] == '>')
                {
                    sb.Append(c).Append(txt[i]);
                    i++;
                    c2 = ' ';
                    needSpace = false;
                    continue;
                }
                // literal Regexp
                else if (c == '/')
                {
                    int i2 = i;
                    if (lookupRegex(txt, ref i))
                    {
                        sb.Append('/').Append(txt.Substring(i2, i - i2));
                        c2 = ' ';
                        needSpace = false;
                        continue;
                    }
                }

                // is a white space needed?
                if (options.CompactChars.IndexOf(c) >= 0)
                {
                    if (c2 == '}' && (c == ';' || c == ','))
                        needSpace = false;
                    else if (options.SpaceBeforeFunctionCall && Char.IsLetterOrDigit(c2) && c == '(')
                        needSpace = true;
                    else
                    {
                        string word = GetLastWord(sb, options.WordCharacters);
                        if (word.Length > 0)
                        foreach (string token in options.AddSpaceAfter)
                            if (token == word)
                            {
                                needSpace = true;
                                break;
                            }
                    }
                }
                else if (options.SpacedChars.IndexOf(c) >= 0)
                {
                    if (c == '*')
                    {
                        if (c2 == '.' || c2 == ':') // import wildchar or any type
                        {
                            needSpace = false;
                            sb.Append('*');
                            c2 = ' ';
                            continue;
                        }
                        else needSpace = true;
                    }
                    else if (i < n && (c == '+' || c == '-') && txt[i] == c) // unary operators
                    {
                        sb.Append(c).Append(c);
                        c2 = c;
                        needSpace = false;
                        i++;
                        continue;
                    }
                    else if (i > 0 && (c == '-' || c == '+') && (c2 == 'e' || c2 == 'E')) // 1e+23 / 1e-23
                    {
                        needSpace = false;
                        sb.Append(c);
                        c2 = ' ';
                        continue;
                    }
                    else if (c2 == '{' && c == '>') // Haxe typedef extension
                    {
                        sb.Append(c);
                        c2 = c;
                        needSpace = true;
                        continue;
                    }
                    else needSpace = (c != '!' || (c2 != '(' && c2 != '['));

                    if (i < n)
                    {
                        if (c == '-' && txt[i] == '>') // php dot operator, haxe function
                        {
                            sb.Append(c).Append(txt[i]);
                            c2 = ' ';
                            needSpace = false;
                            i++;
                            continue;
                        }
                        else if (c2 == '.' && c == '=') // php concat operator
                        {
                            needSpace = false;
                        }
                        else if (c2 == '=' && c == '>') // php array(key => value)
                        {
                            needSpace = false;
                        }
                    }

                    if (options.BraceAfterLine && c == '{')
                    {
                        string start = txt.Substring(0, i - 1).Trim();
                        char prevC = start[start.Length - 1];
                        string end = txt.Substring(i).Trim();
                        if (end.Length == 0)
                        {
                            if (Char.IsLetterOrDigit(prevC) || prevC == ')')
                            {
                                sb.Append(options.Newline).Append(indentation).Append('{');
                                needSpace = false;
                                continue;
                            }
                        }
                    }
                }

                // adding whitespace
                if (c != ' ' && c != '\t')
                {
                    if (needSpace && c != '\r' && c != '\n')
                    {
                        if (c2 != ' ') sb.Append(' ');
                        needSpace = false;
                    }
                    sb.Append(c);
                    if (options.SpacedChars.IndexOf(c) >= 0)
                    {
                        if ((c == '-' || c == '+') && (i >= n || options.Operators.IndexOf(txt[i]) < 0)) // unary sign
                        {
                            needSpace = (c2 == ')' || c2 == ']' || c2 == '\'' || c2 == '"' || Char.IsLetterOrDigit(c2));
                        }
                        else if (c != '!' || (i < n && options.Operators.IndexOf(txt[i]) >= 0)) // operator
                        {
                            while (i < n && (txt[i] == '=' || txt[i] == c))
                            {
                                c = txt[i];
                                sb.Append(txt[i++]);
                            }
                            needSpace = true;
                        }
                    }
                    if (c == ')' && i < n && Char.IsLetter(txt[i]))
                        sb.Append(' ');
                    c2 = c;
                }
                else
                {
                    if (options.CondenseWhitespace) needSpace = true;
                    else 
                    {
                        sb.Append(c);
                        c2 = ' ';
                        needSpace = false;
                    }
                }
            }
            if (!fixedOffset) offset = sb.Length;
            return sb.ToString();
        }

        private static bool lookupGeneric(ReformatOptions options, string txt, ref int index)
        {
            int i = index;
            int n = txt.Length;
            char c = '<', prev = ' ';
            int sub = 0, psub = 0, bsub = 0;
            while (i < n)
            {
                if (c != ' ') prev = c;
                c = txt[i++];
                if (Char.IsLetterOrDigit(c) || c == '.' || c == ' ' || c == ',' || c == ':') continue;
                if (c == '<') sub++;
                else if (c == '>')
                {
                    sub--;
                    if (sub < 0)
                    {
                        index = i;
                        return true;
                    }
                }
                else if (options.IsHaXe && c == '{')
                    psub++;
                else if (psub > 0)
                {
                    if (c == '}') psub--;
                    continue;
                }
                else if (c == '(' && prev == ':')
                    bsub++;
                else if (bsub > 0)
                {
                    if (c == ')') bsub--;
                    continue;
                }
                // haxe function notation (Type->Type)
                else if (options.IsHaXe && c == '-' && (i < n && txt[i] == '>'))
                {
                    sub++;
                    continue;
                }
                else break;
            }
            return false;
        }

        public static bool lookupXML(string txt, ref int index)
        {
            int inString = 0;
            int inComments = 0;
            bool inTag = true;
            bool inExpr = false;
            bool inCData = false;
            bool inComment = false;
            bool isXML = false;

            int i = index;
            int n = txt.Length;
            char c = '<';
            char c2 = '<';
            while (i < n)
            {
                c = txt[i++];

                // AS expression
                if (inExpr)
                {
                    if (c == '{') return false;
                    else if (c != '}') continue;
                }
                // comments
                if (inComments > 0)
                {
                    if (c == '\r' || (i < n && c == '*' && txt[i] == '/'))
                    {
                        i++;
                        inComments = 0;
                    }
                    continue;
                }
                else if (i < n &&  c == '/' && (txt[i] == '*' || txt[i] == '/'))
                {
                    if (txt[i] == '/')
                    {
                        inComments = 1;
                        break;
                    }
                    else if (txt[i] == '*')
                    {
                        inComments = 2;
                        continue;
                    }
                }
                else if (inExpr)
                {
                    if (c == '}') inExpr = false;
                    continue;
                }
                else if (inCData)
                {
                    if (c == ']' && i < n - 1 && txt[i] == ']' && txt[i + 1] == '>')
                    {
                        i += 2;
                        inCData = false;
                    }
                    continue;
                }
                else if (inComment)
                {
                    if (c == '-' && i < n - 1 && txt[i] == '-' && txt[i + 1] == '>')
                    {
                        i += 2;
                        inComment = false;
                    }
                    continue;
                }


                // litteral
                switch (inString)
                {
                    case 0:
                        if (c == '"') inString = 1;
                        else if (c == '\'') inString = 2;
                        if (inString > 0 && c2 != '=') return false;
                        break;
                    case 1:
                        if (c == '"') inString = 0;
                        c2 = c;
                        continue;
                    case 2:
                        if (c == '\'') inString = 0;
                        c2 = c;
                        continue;
                }
                
                // AS expression
                if (c == '{')
                {
                    if ((inTag && (c2 == '<' || c2 == ' ' || c2 == '=' || c2 == '/' || c2 == '$'))
                        || isXML && (c2 == '>'))
                    {
                        inExpr = true;
                        continue;
                    }
                    else return false;
                }

                // new tag
                if (c == '<')
                {
                    if (!inTag)
                    {
                        inTag = true;
                        isXML = false;
                        c2 = '<';
                        continue;
                    }
                    else return false;
                }
                if (c == '>')
                {
                    if (inTag)
                    {
                        inTag = false;
                        isXML = true;
                        c2 = '>';
                        continue;
                    }
                    else return false;
                }

                // CDATA, HTML comments
                if (c == '!' && n - i > 2)
                {
                    if (txt[i] == '[' && txt.Substring(i).StartsWith("[CDATA["))
                    {
                        i += 7;
                        inCData = true;
                        continue;
                    }
                    else if (txt[i] == '-' && txt[i + 1] == '-')
                    {
                        i += 2;
                        inComment = true;
                        continue;
                    }
                }
                if (c == '?' && c2 == '<')
                {
                    index = i + 1;
                    return true;
                }

                if (c >= 32) c2 = c;
                if (char.IsLetterOrDigit(c) || c == ':' || c == '-' || c == '.' || c == '=') continue;
            }

            if (isXML || inCData || inComment)
            {
                index = i;
                return true;
            }
            else return false;
        }

        public static bool lookupRegex(string txt, ref int index)
        {
            int n = txt.Length;
            int i = index - 2;
            char c = '/';
            while (i > 0)
            {
                c = txt[i--];
                if (c == ' ' || c == '\t') continue;
                if (c == '\r' || c == '\n') break;
                if (Char.IsLetterOrDigit(c) || "_$])".IndexOf(c) >= 0) return false;
                break;
            }
            i = index;
            while (i < n)
            {
                c = txt[i++];
                if (c == '\\')
                {
                    i++;
                    continue;
                }
                if (c < 32) return false;
                if (c == '/')
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        private static string GetLastWord(StringBuilder sb, string wordChars)
        {
            string word = "";
            int i = sb.Length - 1;
            while (i > 0 && wordChars.IndexOf(sb[i]) >= 0) word = sb[i--] + word;
            return word;
        }
    }
}
