// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using antlr.collections;
using antlr;
using System.Text.RegularExpressions;

namespace AS3Context.Grammar
{
    public class AS3Validator
    {
        private static Regex reTags = 
            new Regex("\\[[A-Z][^\\]]+\\]", RegexOptions.Multiline | RegexOptions.Compiled);

        public static string ValidateSource(string fileName, string src, ScintillaNet.ScintillaControl sci)
        {
            Stream str = new MemoryStream(Encoding.Default.GetBytes(src));

            // model generator
            AS3Lexer lexer = new AS3Lexer(str);
            lexer.setFilename(fileName);
            AS3Parser parser = new AS3Parser(lexer);
            parser.setFilename(fileName);
            // start parsing
            try
            {
                parser.compilationUnit();
            }
            catch (RecognitionException rex)
            {
                string result = fileName + ":" + rex.line + ": ";
                string line = (rex.line > 0) ? sci.GetLine(rex.line - 1) : "";
                int col = rex.column;
                if (sci != null && rex.line > 0)
                {
                    Match token = Regex.Match(rex.Message, "found '([^']+)");
                    if (!token.Success)
                        token = Regex.Match(rex.Message, "\"([^\"]+)");
                    // find token position
                    if (token.Success)
                    {
                        string tok = token.Groups[1].Value;
                        int p = line.IndexOf(tok);
                        if (p > 0)
                        {
                            p = sci.MBSafeTextLength(line.Substring(0, p));
                            int len = sci.MBSafeTextLength(tok);
                            return result + "characters " + p + "-" + (p + len) + " : " + rex.Message;
                        }
                    }
                    // fix column index
                    else
                    {
                        for (int i = 0; i < line.Length; i++)
                            if (line[i] == '\t') col -= 7;
                            else if (line[i] != ' ') break;
                    }
                }
                return result + "character "+Math.Max(0, col)+" : "+rex.Message;
            }
            catch (TokenStreamRecognitionException trex)
            {
                int col = trex.recog.column;
                if (trex.recog.line > 0)
                {
                    // fix column index
                    string line = sci.GetLine(trex.recog.line - 1);
                    for (int i = 0; i < line.Length; i++)
                        if (line[i] == '\t') col -= 7;
                        else if (line[i] != ' ') break;
                }
                return fileName + ":" + trex.recog.line + ": character " + col + " : " + trex.Message;
            }
            catch (TokenStreamException tex)
            {
                return fileName + ": IO Error: " + tex.Message;
            }
            catch (Exception ex)
            {
                return fileName + ": Validator Exception: " + ex.Message;
            }
            return null;
        }
    }
}
