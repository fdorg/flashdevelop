using System;
using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace CodeFormatter.InfoCollector
{
    public class AntlrUtilities
    {
        public const int CHANNEL_SLCOMMENT=43;
        public const int CHANNEL_MLCOMMENT = 42;
        public const int CHANNEL_WHITESPACE = 41;
        public const int CHANNEL_EOL = 40;

        private static IToken getFirstTreeToken(CommonTree tree)
        {
            IList children = tree.Children;
            if (children == null)
            {
                return tree.Token;
            }

            return getFirstTreeToken((CommonTree)children[0]);
        }

        /**
         * return whether char is whitespace, treating non-breaking space as whitespace as well
         * @param ch
         * @return
         */
        public static bool isASWhitespace(char ch)
        {
            if (Char.IsWhiteSpace(ch))
                return true;
            if (ch == '\u00A0')
                return true;

            return false;
        }

        /**
         * trim the string, treating non-breaking space as whitespace as well
         * @param input
         * @return
         */
        public static String asTrim(String input)
        {
            input = input.Replace('\u00a0', ' ');
            return input.Trim();
        }

        private static IToken getLastTreeToken(CommonTree tree)
        {
            if (tree == null)
                return null;
            IList children = tree.Children;
            if (children == null)
            {
                return tree.Token;
            }

            return getLastTreeToken((CommonTree)children[children.Count - 1]);
        }

        private static int getFirstTreePosition(CommonTree tree)
        {
            if (tree == null)
                return -1;
            IList children = tree.Children;
            if (children == null)
            {
                return ((CommonToken)tree.Token).StartIndex;
            }

            return getFirstTreePosition((CommonTree)children[0]);
        }

        public static int getFirstTreePosition(ParserRuleReturnScope tree)
        {
            return getFirstTreePosition((CommonTree)tree.Tree);
        }

        private static int getLastTreePosition(CommonTree tree)
        {
            IList children = tree.Children;
            if (children == null)
            {
                return ((CommonToken)tree.Token).StopIndex + 1;
            }

            return getLastTreePosition((CommonTree)children[children.Count - 1]);
        }

        public static int getLastTreePosition(ParserRuleReturnScope tree)
        {
            if (tree.Tree==null)
            {
                if (tree.Stop!=null && tree.Stop is CommonToken)
                {
                    return ((CommonToken)tree.Stop).StopIndex+1; //this is necessary for implicit semicolon cases.  You will get an extra CR here
                }
                return -1;
            }
            return getLastTreePosition((CommonTree)tree.Tree);
        }

        //    public static List<String> getTreeListText(ParserRuleReturnScope tree, String delimiter)
        //    {
        //      String allText=getTreeText(tree);
        //      String[] items=allText.split(delimiter);
        //      List<String> results=new ArrayList<String>();
        //      for (String item:items)
        //      {
        //          if (item.length()>0)
        //          {
        //              results.add(item);
        //          }
        //      }
        //      
        //      return results;
        //    }

        private static String getCommonTreeText(CommonTree tree)
        {
            if (tree==null)
                return "";
            IList children=tree.Children;
            if (children==null)
            {
                return tree.Token.Text;
            }
            String buffer="";
            foreach (Object obj in children)
            {
                if (obj is CommonTree)
                {
                    buffer += getCommonTreeText((CommonTree)obj);
                }
            }
            return buffer;
        }

        public static String getTreeText(ParserRuleReturnScope tree)
        {
            return getCommonTreeText((CommonTree)tree.Tree);
        }

        //  public static ASDocComment findPreviousComment(ParserRuleReturnScope t, CommonTokenStream rawTokens) {
        //      
        //      return findPreviousComment(getFirstTreeToken((CommonTree)t.getTree()), rawTokens);
        //  }
        //
        //  public static ASDocComment findPreviousComment(Token tok, CommonTokenStream rawTokens)
        //  {
        //      int currentTokenIndex=((CommonToken)tok).getTokenIndex()-1;
        ////        List<Token> hiddenTokens=new ArrayList<Token>();
        //      
        //      //collect all of the hidden tokens since the last non-whitespace token
        //      while (currentTokenIndex>=0)
        //      {
        //          Token t=rawTokens.get(currentTokenIndex);
        //          if (t.getChannel()==Token.DEFAULT_CHANNEL)
        //              break; 
        //          
        //          if (t.getType()==ASCollectorLexer.COMMENT_MULTILINE && t.getText().startsWith("/**"))
        //          {
        //              return new ASDocComment(t);
        //          }
        ////            hiddenTokens.add(t);
        //          currentTokenIndex--;
        //      }
        ////        Collections.reverse(hiddenTokens);
        //      return null;
        //  }

        /*public static ASDocComment findCommentReverse(List<IToken> hiddenTokens)
        {
            int currentTokenIndex=hiddenTokens.Count-1;
        
            //collect all of the hidden tokens since the last non-whitespace token
            loop: while (currentTokenIndex>=0)
            {
                IToken t=hiddenTokens[currentTokenIndex];
                switch (t.Channel)
                {
                case CHANNEL_MLCOMMENT:
                    if (t.Text.StartsWith("/**"))
                        return new ASDocComment(t);
                    break loop;
                case CHANNEL_WHITESPACE:
                case CHANNEL_EOL:
                    currentTokenIndex--;
                    break;
                default:
                    break loop;
                }
            }
            return null;
        }*/

        public static List<IToken> getPostHiddenTokens(IToken tok, CommonTokenStream rawTokens)
        {
            List<IToken> results = new List<IToken>();
            if (tok == null)
                return results;
            int currentTokenIndex = ((CommonToken)tok).TokenIndex + 1;
            while (currentTokenIndex < rawTokens.Count)
            {
                IToken t = rawTokens.Get(currentTokenIndex);
                if (t.Channel == Token.DEFAULT_CHANNEL)
                    break;

                if (t.Channel == CHANNEL_EOL)
                    break;

                if (t.Channel == CHANNEL_SLCOMMENT)
                {
                    results.Add(t);
                    break;
                }

                results.Add(t);
                currentTokenIndex++;
            }

            //walk backwards to remove whitespace tokens at the end of list
            for (int i = results.Count - 1; i >= 0; i--)
            {
                IToken t = results[i];
                if (t.Channel == CHANNEL_WHITESPACE)
                    results.RemoveAt(i);
            }

            return results;
        }

        public static List<IToken> getPostHiddenTokens(ParserRuleReturnScope tree, CommonTokenStream rawTokens)
        {
            if (tree.Tree==null)
            {
                //I think this only happens with implied semicolons
                if (tree.Start is CommonToken)
                {
                    //I think we should always be on at least token 1.  
                    IToken currentTok=rawTokens.Get(((CommonToken)tree.Start).TokenIndex);
                    //I go back one token if I am on a non-default channel token so that I can search forward for hidden tokens.
                    if (currentTok.Channel!=Token.DEFAULT_CHANNEL)
                        currentTok=rawTokens.Get(((CommonToken)tree.Start).TokenIndex-1);
                    return getPostHiddenTokens(currentTok, rawTokens);
                }
                return null;
            }
            return getPostHiddenTokens(getLastTreeToken((CommonTree)tree.Tree), rawTokens);
        }


        public static List<IToken> getHiddenTokens(IToken tok, CommonTokenStream rawTokens, bool crossLineBoundaries, bool filterNone)
        {
            List<IToken> results = new List<IToken>();
            if (tok == null)
                return results;
            int currentTokenIndex = ((CommonToken)tok).TokenIndex - 1;
            bool seenCR = false;
            int tokensSinceLastCR = 0;
            while (currentTokenIndex >= 0)
            {
                IToken t = rawTokens.Get(currentTokenIndex);
                if (t.Channel == Token.DEFAULT_CHANNEL)
                    break;

                if (t.Channel == CHANNEL_EOL || t.Channel == CHANNEL_SLCOMMENT)
                {
                    if (!crossLineBoundaries)
                        break;
                    tokensSinceLastCR = 0;
                    //              if (t.getChannel()==ASCollectorParser.CHANNEL_SLCOMMENT)
                    //                  tokensSinceLastCR++;
                    seenCR = true;
                }
                else
                {
                    tokensSinceLastCR++;
                }

                results.Insert(0, t);
                currentTokenIndex--;
            }

            //if we want all the hidden tokens (without any post-processing), then just return here
            if (filterNone)
                return results;

            //strip off tokens from previous line that had code on it
            if (seenCR && currentTokenIndex >= 0)
            {
                for (int i = 0; i < tokensSinceLastCR; i++)
                {
                    results.RemoveAt(0);
                }
            }

            if (results.Count > 0 && currentTokenIndex >= 0)
            {
                //remove the first token if it contained a carriage return.  The idea here is to not include the token that should
                //go with the previous line.  Special case for first token of file
                if (results[0].Channel == CHANNEL_EOL || results[0].Channel == CHANNEL_SLCOMMENT)
                {
                    results.RemoveAt(0);
                }
                else if (crossLineBoundaries)
                {
                    //otherwise, remove whitespace tokens up to the first carriage return or all tokens
                    while (results.Count > 0)
                    {
                        IToken t = results[0];
                        if (t.Channel == CHANNEL_EOL)
                            break;
                        results.RemoveAt(0);
                    }
                }
            }

            //leave leading whitespace associated with the element
            //      //now, strip off the leading whitespace
            //      while (results.size()>0)
            //      {
            //          Token t=results.get(0);
            //          if (t.getChannel()==ASCollectorParser.CHANNEL_EOL || t.getChannel()==ASCollectorParser.CHANNEL_WHITESPACE)
            //          {
            //              results.remove(0);
            //              continue;
            //          }
            //          break;
            //      }
            return results;
        }

        public static List<IToken> getHiddenTokens(ParserRuleReturnScope tree, CommonTokenStream rawTokens, bool crossLineBoundaries)
        {
            return getHiddenTokens(getFirstTreeToken((CommonTree)tree.Tree), rawTokens, crossLineBoundaries, false);
        }

        public static String findPreWhitespace(String text)
        {
            String buffer = "";
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (isASWhitespace(ch))
                    buffer += ch;
                else
                    break;
            }
            return buffer;
        }

    }

}